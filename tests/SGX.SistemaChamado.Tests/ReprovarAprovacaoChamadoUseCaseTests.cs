using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ReprovarAprovacaoChamadoUseCaseTests
{
    [Fact]
    public async Task DeveReprovarInstanciaSimplesPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "simples");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var statusChamadoAntes = dados.Chamado.StatusId;

        var response = await useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Mudanca rejeitada por risco nao mitigado."
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        var decisao = await context.DecisoesAprovacaoChamado.AsNoTracking().SingleAsync();
        var chamadoPersistido = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.Chamado.Id);

        Assert.True(response.Reprovada);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, response.StatusInstanciaAnterior);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Reprovada, response.StatusInstanciaNovo);
        Assert.True(response.DecisaoFinal);
        Assert.True(response.MantemBloqueio);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Reprovada, instanciaPersistida.Status);
        Assert.NotNull(instanciaPersistida.DecididaEm);
        Assert.Equal(TipoDecisaoAprovacaoChamado.Rejeicao, decisao.TipoDecisao);
        Assert.Equal(ResultadoDecisaoAprovacaoChamado.Reprovada, decisao.Resultado);
        Assert.Equal(dados.Aprovador.Id, decisao.DecisorUsuarioId);
        Assert.Equal(statusChamadoAntes, chamadoPersistido.StatusId);
    }

    [Fact]
    public async Task DeveReprovarEtapaSemConsolidarInstanciaQuandoDecisaoForParcial()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "etapa-parcial");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapa = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1, criticaParaConsolidacao: false);
        await CriarEtapaAsync(context, instancia, dados, ordem: 2, nivel: 1, criticaParaConsolidacao: false);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Escopo parcial precisa de ajuste.",
            DecisaoParcial = true,
            ExigeReavaliacao = true,
            PermiteNovaSolicitacao = true
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        var etapaPersistida = await context.EtapasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == etapa.Id);
        var decisao = await context.DecisoesAprovacaoChamado.AsNoTracking().SingleAsync();

        Assert.Equal(StatusEtapaAprovacaoChamado.Pendente, response.StatusEtapaAnterior);
        Assert.Equal(StatusEtapaAprovacaoChamado.Reprovada, response.StatusEtapaNovo);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, response.StatusInstanciaNovo);
        Assert.False(response.DecisaoFinal);
        Assert.True(response.ExigeReavaliacao);
        Assert.Equal(StatusEtapaAprovacaoChamado.Reprovada, etapaPersistida.Status);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, instanciaPersistida.Status);
        Assert.True(decisao.DecisaoParcial);
        Assert.False(decisao.DecisaoFinal);
    }

    [Fact]
    public async Task DeveReprovarInstanciaQuandoEtapaCriticaReceberDecisaoNegativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "etapa-critica");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapa = await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1, criticaParaConsolidacao: true);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var response = await useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Risco critico sem controle compensatorio."
        });

        var instanciaPersistida = await context.InstanciasAprovacaoChamado.AsNoTracking().SingleAsync(x => x.Id == instancia.Id);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Reprovada, instanciaPersistida.Status);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Reprovada, response.StatusInstanciaNovo);
    }

    [Fact]
    public async Task NaoDeveReprovarInstanciaInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "inexistente");
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = Guid.NewGuid(),
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Nao conforme."
        }));

        Assert.Contains("Instancia de aprovacao nao encontrada", ex.Message);
    }

    [Fact]
    public async Task NaoDeveReprovarInstanciaAprovada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "aprovada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, dados.Aprovador.Id, dados.Administrador.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Nao deveria reprovar."
        }));

        Assert.Contains("ja foi aprovada", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveReprovarEtapaDeOutraInstancia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "outra-etapa");
        var instancia1 = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var instancia2 = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        var etapaDaOutra = await CriarEtapaAsync(context, instancia2, dados, ordem: 1, nivel: 1, criticaParaConsolidacao: false);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia1.Id,
            EtapaAprovacaoChamadoId = etapaDaOutra.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Etapa incorreta."
        }));

        Assert.Contains("nao pertence", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveCriarDecisaoFinalNegativaDuplicada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "duplicada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        var useCase = CriarUseCase(context, dados.Aprovador);

        await useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Primeira reprovacao."
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Segunda reprovacao."
        }));

        Assert.Contains("reprovada", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, await context.DecisoesAprovacaoChamado.CountAsync());
    }

    [Fact]
    public async Task NaoDeveReprovarDiretamenteQuandoHaEtapaObrigatoriaPendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "direta-bloqueada");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Sequencial);
        await CriarEtapaAsync(context, instancia, dados, ordem: 1, nivel: 1, criticaParaConsolidacao: true);
        var useCase = CriarUseCase(context, dados.Aprovador);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Nao pode pular etapa."
        }));

        Assert.Contains("etapa obrigatoria pendente", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReprovarInstanciaDeveEncerrarPendenciaDeBloqueioDoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "integracao-reprovacao");
        var instancia = await CriarInstanciaAsync(context, dados, TipoFluxoAprovacao.Simples);
        
        var useCaseBloqueio = new ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context));

        // 1. Antes da reprovação: bloqueia
        var requestBloqueio = new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        };
        var bloqueioAntes = await useCaseBloqueio.ExecutarAsync(requestBloqueio);
        Assert.True(bloqueioAntes.Bloqueado);

        // 2. Executa reprovação
        var useCaseReprovacao = CriarUseCase(context, dados.Aprovador);
        await useCaseReprovacao.ExecutarAsync(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisorUsuarioId = dados.Aprovador.Id,
            Justificativa = "Escopo reprovado e encerrado."
        });

        // 3. Depois da reprovação: não bloqueia mais por essa instância
        var bloqueioDepois = await useCaseBloqueio.ExecutarAsync(requestBloqueio);
        Assert.False(bloqueioDepois.Bloqueado);
        Assert.True(bloqueioDepois.Permitido);
    }

    private static ReprovarAprovacaoChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuarioAtual)
        => new(
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<EtapaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<DecisaoAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioAtual.Id,
                usuarioAtual.Nome,
                usuarioAtual.Email,
                usuarioAtual.Login,
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado)> CriarCenarioAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin {sufixo}", $"admin.repr.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol.repr.{sufixo}@sgx.local", TipoPerfil.Solicitante);
        var aprovador = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Aprovador {sufixo}", $"aprov.repr.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria reprovacao {sufixo}");
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, prioridade.Id, sufixo);
        return (admin, solicitante, aprovador, categoria, chamado);
    }

    private static async Task<InstanciaAprovacaoChamado> CriarInstanciaAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado) dados,
        TipoFluxoAprovacao fluxo)
    {
        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            solicitanteId: dados.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: fluxo,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            categoriaId: dados.Categoria.Id,
            impactoAvaliado: dados.Chamado.ImpactoChamado,
            urgenciaAvaliada: dados.Chamado.UrgenciaChamado,
            prioridadeAvaliada: PrioridadeChamadoEnum.Media,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            regraNomeSnapshot: "Regra de aprovacao",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Categoria e prioridade");

        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();
        return instancia;
    }

    private static async Task<EtapaAprovacaoChamado> CriarEtapaAsync(
        SGXSistemaChamadoDbContext context,
        InstanciaAprovacaoChamado instancia,
        (Usuario Administrador, Usuario Solicitante, Usuario Aprovador, CategoriaChamado Categoria, Chamado Chamado) dados,
        int ordem,
        int nivel,
        bool criticaParaConsolidacao)
    {
        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: dados.Solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Simples,
            tipoFluxoAprovacao: instancia.TipoFluxoAprovacao,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            ordem: ordem,
            nivel: nivel,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            obrigatoria: true,
            criticaParaConsolidacao: criticaParaConsolidacao,
            aprovadorEspecificoUsuarioId: dados.Aprovador.Id,
            escopoResumoSnapshot: $"Etapa {ordem}",
            regraNomeSnapshot: instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: instancia.RegraCriterioSnapshot);

        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();
        return etapa;
    }
}
