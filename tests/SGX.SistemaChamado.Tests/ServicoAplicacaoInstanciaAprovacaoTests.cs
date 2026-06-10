using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ServicoAplicacaoInstanciaAprovacaoTests
{
    [Fact]
    public async Task ListarInstanciasDevePermitirFiltrosBasicos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var useCases = CriarUseCases(context);

        context.InstanciasAprovacaoChamado.Add(CriarInstancia(
            dados.Chamado.Id,
            dados.Solicitante.Id,
            dados.Administrador.Id,
            titulo: "Instancia pendente bloqueante",
            bloqueante: true,
            exigeAprovacao: true,
            aprovadorPadraoUsuarioId: dados.Administrador.Id));

        var aprovada = CriarInstancia(
            dados.Chamado.Id,
            dados.Solicitante.Id,
            dados.Administrador.Id,
            titulo: "Instancia concluida",
            bloqueante: false,
            exigeAprovacao: true,
            aprovadorPadraoUsuarioId: dados.Administrador.Id);
        aprovada.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, dados.Administrador.Id, dados.Administrador.Id, "teste");
        context.InstanciasAprovacaoChamado.Add(aprovada);
        await context.SaveChangesAsync();

        var response = await useCases.ListarAsync(new ListarInstanciasAprovacaoChamadoRequest
        {
            ApenasPendentes = true,
            ApenasBloqueantes = true
        });

        Assert.Single(response.Items);
        Assert.Equal("Instancia pendente bloqueante", response.Items.Single().Titulo);
    }

    [Fact]
    public async Task ObterDetalheDeveExporRelacoesContagensEEloLegado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var useCases = CriarUseCases(context);

        var aprovacaoLegada = new AprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            tipoOrigem: TipoOrigemAprovacaoChamado.RegraAdministrativa,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            solicitanteId: dados.Solicitante.Id,
            titulo: "Aprovacao legada vinculada",
            aprovadorId: dados.Administrador.Id);
        context.AprovacoesChamado.Add(aprovacaoLegada);
        await context.SaveChangesAsync();

        var instancia = CriarInstancia(
            dados.Chamado.Id,
            dados.Solicitante.Id,
            dados.Administrador.Id,
            titulo: "Instancia com relacoes",
            bloqueante: true,
            exigeAprovacao: true,
            aprovacaoChamadoLegadaId: aprovacaoLegada.Id,
            aprovadorPadraoUsuarioId: dados.Administrador.Id);
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: dados.Solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Gerencial,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Sequencial,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            ordem: 1,
            nivel: 1,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            obrigatoria: true,
            criticaParaConsolidacao: true,
            aprovadorPadraoUsuarioId: dados.Administrador.Id,
            escopoResumoSnapshot: "Escopo completo",
            regraNomeSnapshot: "Regra da instancia",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca");
        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();

        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            tipoDecisao: TipoDecisaoAprovacaoChamado.RegistroManual,
            resultado: ResultadoDecisaoAprovacaoChamado.SemEfeitoOperacional,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.Pendente,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            etapaAprovacaoChamadoId: etapa.Id,
            decisorUsuarioId: dados.Administrador.Id,
            decisaoParcial: true,
            decisaoFinal: false,
            mantemBloqueio: true,
            statusEtapaAnterior: StatusEtapaAprovacaoChamado.Pendente,
            statusEtapaNovo: StatusEtapaAprovacaoChamado.Pendente,
            regraNomeSnapshot: "Regra da instancia",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca",
            nivelEtapaSnapshot: 1,
            ordemEtapaSnapshot: 1,
            ramoEtapaSnapshot: "principal");
        context.DecisoesAprovacaoChamado.Add(decisao);
        await context.SaveChangesAsync();

        var detalhe = await useCases.ObterPorIdAsync(instancia.Id);

        Assert.Equal(aprovacaoLegada.Id, detalhe.AprovacaoChamadoLegadaId);
        Assert.Equal(1, detalhe.QuantidadeEtapas);
        Assert.Equal(1, detalhe.QuantidadeDecisoes);
        Assert.Single(detalhe.Etapas);
        Assert.Single(detalhe.Decisoes);
    }

    [Fact]
    public async Task ListarPorChamadoDeveRetornarSomenteInstanciasDoChamadoInformado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var outroDados = await CriarDadosBaseAsync(context, "OUT");
        var useCases = CriarUseCases(context);

        context.InstanciasAprovacaoChamado.Add(CriarInstancia(dados.Chamado.Id, dados.Solicitante.Id, dados.Administrador.Id, "Do chamado alvo", aprovadorPadraoUsuarioId: dados.Administrador.Id));
        context.InstanciasAprovacaoChamado.Add(CriarInstancia(outroDados.Chamado.Id, outroDados.Solicitante.Id, outroDados.Administrador.Id, "De outro chamado", aprovadorPadraoUsuarioId: outroDados.Administrador.Id));
        await context.SaveChangesAsync();

        var itens = await useCases.ListarPorChamadoAsync(dados.Chamado.Id);

        Assert.Single(itens);
        Assert.Equal("Do chamado alvo", itens.Single().Titulo);
    }

    [Fact]
    public async Task ValidarECriarManualDevemRejeitarInstanciaBloqueanteSemAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var useCases = CriarUseCases(context);

        var validar = await useCases.ValidarAsync(new ValidarInstanciaAprovacaoChamadoRequest
        {
            Instancia = new PrepararInstanciaAprovacaoChamadoRequest
            {
                ChamadoId = dados.Chamado.Id,
                SolicitanteId = dados.Solicitante.Id,
                ExigeAprovacao = false,
                Bloqueante = true,
                EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco
            }
        });

        Assert.False(validar.Valida);
        Assert.Contains(validar.Erros, x => x.Contains("bloqueante", StringComparison.OrdinalIgnoreCase));

        await Assert.ThrowsAsync<ValidationException>(() => useCases.CriarManualAsync(new CriarInstanciaAprovacaoChamadoManualRequest
        {
            ChamadoId = dados.Chamado.Id,
            SolicitanteId = dados.Solicitante.Id,
            Titulo = "Criacao invalida",
            ExigeAprovacao = false,
            Bloqueante = true,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco
        }));
    }

    [Fact]
    public async Task PrepararInstanciaNaoDevePersistirDadosNemGerarEtapasOuDecisoes()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var useCases = CriarUseCases(context);

        var instanciasAntes = await context.InstanciasAprovacaoChamado.CountAsync();
        var etapasAntes = await context.EtapasAprovacaoChamado.CountAsync();
        var decisoesAntes = await context.DecisoesAprovacaoChamado.CountAsync();

        var response = await useCases.PrepararAsync(new PrepararInstanciaAprovacaoChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            SolicitanteId = dados.Solicitante.Id,
            Titulo = "Preparacao sem persistencia",
            ExigeAprovacao = true,
            Bloqueante = true,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Sequencial,
            EscopoRegra = EscopoRegraAprovacao.AtendimentoChamado,
            TipoRegra = TipoRegraAprovacao.Combinada
        });

        Assert.True(response.PodeCriar);
        Assert.NotNull(response.Instancia);
        Assert.Equal(instanciasAntes, await context.InstanciasAprovacaoChamado.CountAsync());
        Assert.Equal(etapasAntes, await context.EtapasAprovacaoChamado.CountAsync());
        Assert.Equal(decisoesAntes, await context.DecisoesAprovacaoChamado.CountAsync());
    }

    [Fact]
    public async Task CriarManualNaoDeveCriarEtapasDecisoesNemAlterarAprovacaoLegada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarDadosBaseAsync(context);
        var useCases = CriarUseCases(context);

        var aprovacaoLegada = new AprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            tipoOrigem: TipoOrigemAprovacaoChamado.Manual,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            solicitanteId: dados.Solicitante.Id,
            titulo: "Legada pendente",
            aprovadorId: dados.Administrador.Id);
        context.AprovacoesChamado.Add(aprovacaoLegada);
        await context.SaveChangesAsync();

        var criada = await useCases.CriarManualAsync(new CriarInstanciaAprovacaoChamadoManualRequest
        {
            ChamadoId = dados.Chamado.Id,
            SolicitanteId = dados.Solicitante.Id,
            AprovacaoChamadoLegadaId = aprovacaoLegada.Id,
            Titulo = "Instancia manual",
            ExigeAprovacao = true,
            Bloqueante = false,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            EscopoRegra = EscopoRegraAprovacao.AtendimentoChamado,
            TipoRegra = TipoRegraAprovacao.Geral,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Simples
        });

        Assert.Equal("Instancia manual", criada.Titulo);
        Assert.Empty(context.EtapasAprovacaoChamado);
        Assert.Empty(context.DecisoesAprovacaoChamado);
        Assert.Equal(StatusAprovacaoChamado.Pendente, (await context.AprovacoesChamado.SingleAsync()).Status);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    private static InstanciaAprovacaoChamadoAdminUseCases CriarUseCases(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ConfiguracaoRegraAprovacao>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Administrador Instancia",
                "admin.instancia@sgx.local",
                "admin.instancia",
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Solicitante, Usuario Administrador, Chamado Chamado)> CriarDadosBaseAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo = "BASE")
    {
        var solicitante = new Usuario($"Solicitante {sufixo}", $"solicitante.{sufixo.ToLowerInvariant()}@sgx.local", $"solicitante.{sufixo.ToLowerInvariant()}", "teste");
        var administrador = new Usuario($"Administrador {sufixo}", $"admin.{sufixo.ToLowerInvariant()}@sgx.local", $"admin.{sufixo.ToLowerInvariant()}", "teste");
        context.Usuarios.AddRange(solicitante, administrador);

        var categoria = new CategoriaChamado($"Categoria {sufixo}", null, null, "teste");
        var prioridade = new PrioridadeChamado($"Alta {sufixo}", PrioridadeChamadoEnum.Alta, null, 4, 8, "teste");
        var status = new StatusChamado($"Aberto {sufixo}", StatusChamadoEnum.Aberto, null, false, false, "teste");
        context.CategoriasChamado.Add(categoria);
        context.PrioridadesChamado.Add(prioridade);
        context.StatusChamado.Add(status);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            codigo: $"CH-{sufixo}-{Guid.NewGuid():N}"[..16],
            titulo: $"Chamado {sufixo}",
            descricao: "Descricao do chamado",
            solicitanteId: solicitante.Id,
            categoriaId: categoria.Id,
            prioridadeId: prioridade.Id,
            statusId: status.Id,
            origem: OrigemChamado.Portal,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoChamado: ImpactoChamadoEnum.Alto,
            urgenciaChamado: UrgenciaChamadoEnum.Alta);
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();

        return (solicitante, administrador, chamado);
    }

    private static InstanciaAprovacaoChamado CriarInstancia(
        Guid chamadoId,
        Guid solicitanteId,
        Guid criadoPorUsuarioId,
        string titulo,
        bool bloqueante = false,
        bool exigeAprovacao = true,
        Guid? aprovacaoChamadoLegadaId = null,
        Guid? aprovadorPadraoUsuarioId = null)
        => new(
            chamadoId: chamadoId,
            solicitanteId: solicitanteId,
            origem: OrigemInstanciaAprovacaoChamado.Manual,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: bloqueante
                ? EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco
                : EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Geral,
            exigeAprovacao: exigeAprovacao,
            bloqueante: bloqueante,
            tipoResolucaoAprovador: aprovadorPadraoUsuarioId.HasValue
                ? TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao
                : TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            criadoPorUsuarioId: criadoPorUsuarioId,
            criadoPor: "teste",
            aprovacaoChamadoLegadaId: aprovacaoChamadoLegadaId,
            titulo: titulo,
            descricao: "Descricao da instancia",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoAvaliado: ImpactoChamadoEnum.Alto,
            urgenciaAvaliada: UrgenciaChamadoEnum.Alta,
            prioridadeAvaliada: PrioridadeChamadoEnum.Alta,
            permiteReenvio: true,
            aprovadorPadraoUsuarioId: aprovadorPadraoUsuarioId,
            prazoDecisaoHoras: 4,
            deveExpirarEm: DateTime.UtcNow.AddHours(4),
            regraNomeSnapshot: "Regra da instancia",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca");
}
