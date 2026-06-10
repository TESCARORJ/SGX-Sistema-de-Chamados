using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class GerarAprovacaoObrigatoriaChamadoUseCaseTests
{
    [Fact]
    public async Task NaoDeveGerarQuandoNaoHaRegraAplicavel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "sem-regra");
        var useCase = CriarUseCase(context, dados.Administrador);

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.Equal("Nenhuma configuracao de regra aplicavel exigiu aprovacao para o contexto informado.", response.Motivo);
        Assert.Empty(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task NaoDeveGerarQuandoRegraEInformativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "informativa");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra informativa",
            dados.Categoria.Id,
            exigeAprovacao: false,
            bloqueante: false,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.Sinalizar));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.Equal("A regra aplicavel encontrada nao exige aprovacao obrigatoria.", response.Motivo);
        Assert.Empty(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DeveGerarInstanciaPendenteComSnapshotEEscopoDaRegra()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "gera");
        var useCase = CriarUseCase(context, dados.Administrador);

        var regra = CriarRegra(
            dados.Administrador.Id,
            "Mudanca critica",
            dados.Categoria.Id,
            exigeAprovacao: true,
            bloqueante: true,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Sequencial,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            aprovadorPadraoUsuarioId: dados.Administrador.Id,
            prazoDecisaoHoras: 8);

        context.ConfiguracoesRegrasAprovacao.Add(regra);
        await context.SaveChangesAsync();

        var statusAntes = dados.Chamado.StatusId;
        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            PrioridadeChamado = PrioridadeChamadoEnum.Media
        });

        var instancia = await context.InstanciasAprovacaoChamado.SingleAsync();
        var chamadoPersistido = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.Chamado.Id);

        Assert.True(response.GerouAprovacao);
        Assert.Equal(instancia.Id, response.InstanciaAprovacaoChamadoId);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, instancia.Status);
        Assert.Equal(OrigemInstanciaAprovacaoChamado.RegraMotor, instancia.Origem);
        Assert.True(instancia.ExigeAprovacao);
        Assert.True(instancia.Bloqueante);
        Assert.Equal(EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco, instancia.EfeitoOperacional);
        Assert.Equal(TipoFluxoAprovacao.Sequencial, instancia.TipoFluxoAprovacao);
        Assert.Equal(TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao, instancia.TipoResolucaoAprovador);
        Assert.Equal(dados.Administrador.Id, instancia.AprovadorPadraoUsuarioId);
        Assert.Equal(dados.Administrador.Id, instancia.AprovadorResolvidoUsuarioId);
        Assert.Equal(regra.Nome, instancia.RegraNomeSnapshot);
        Assert.Equal(regra.Versao, instancia.RegraVersaoSnapshot);
        Assert.Equal(regra.Descricao, instancia.RegraCriterioSnapshot);
        Assert.Equal(dados.Categoria.Id, instancia.CategoriaId);
        Assert.Equal(dados.Chamado.NaturezaChamado, instancia.NaturezaChamado);
        Assert.Equal(dados.Chamado.ImpactoChamado, instancia.ImpactoAvaliado);
        Assert.Equal(dados.Chamado.UrgenciaChamado, instancia.UrgenciaAvaliada);
        Assert.Equal(PrioridadeChamadoEnum.Media, instancia.PrioridadeAvaliada);
        Assert.Empty(context.EtapasAprovacaoChamado);
        Assert.Empty(context.DecisoesAprovacaoChamado);
        Assert.Empty(context.AprovacoesChamado);
        Assert.Equal(statusAntes, chamadoPersistido.StatusId);
    }

    [Fact]
    public async Task DeveDefinirPrazoEVencimentoQuandoRegraPossuiPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "prazo");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra com prazo",
            dados.Categoria.Id,
            exigeAprovacao: true,
            bloqueante: false,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            prazoDecisaoHoras: 6));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            PrioridadeChamado = PrioridadeChamadoEnum.Media
        });

        var instancia = await context.InstanciasAprovacaoChamado.SingleAsync();
        Assert.True(response.GerouAprovacao);
        Assert.Equal(6, instancia.PrazoDecisaoHoras);
        Assert.NotNull(instancia.DeveExpirarEm);
        Assert.InRange(
            (instancia.DeveExpirarEm.Value - instancia.SolicitadaEm).TotalHours,
            5.99,
            6.01);
        Assert.Equal(instancia.DeveExpirarEm, response.DeveExpirarEm);
    }

    [Fact]
    public async Task NaoDeveGerarDuplicidadeQuandoJaExisteInstanciaEquivalentePendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "duplicada-instancia");
        var useCase = CriarUseCase(context, dados.Administrador);
        var regra = CriarRegra(dados.Administrador.Id, "Regra duplicada", dados.Categoria.Id);

        context.ConfiguracoesRegrasAprovacao.Add(regra);
        await context.SaveChangesAsync();

        context.InstanciasAprovacaoChamado.Add(new InstanciaAprovacaoChamado(
            dados.Chamado.Id,
            dados.Solicitante.Id,
            OrigemInstanciaAprovacaoChamado.RegraMotor,
            regra.TipoFluxoAprovacao,
            regra.EfeitoOperacional,
            regra.EscopoRegra,
            regra.TipoRegra,
            regra.ExigeAprovacao,
            regra.Bloqueante,
            regra.TipoResolucaoAprovador,
            dados.Administrador.Id,
            "teste",
            configuracaoRegraAprovacaoId: regra.Id,
            titulo: "Instancia existente",
            descricao: "Duplicidade",
            naturezaChamado: dados.Chamado.NaturezaChamado,
            categoriaId: dados.Categoria.Id,
            impactoAvaliado: dados.Chamado.ImpactoChamado,
            urgenciaAvaliada: dados.Chamado.UrgenciaChamado,
            prioridadeAvaliada: PrioridadeChamadoEnum.Media,
            regraNomeSnapshot: regra.Nome,
            regraVersaoSnapshot: regra.Versao,
            regraCriterioSnapshot: regra.Descricao));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.True(response.JaExistiaAprovacaoEquivalente);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task NaoDeveGerarQuandoJaExisteAprovacaoLegadaPendentePorCatalogo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "duplicada-legado", possuiCatalogo: true);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(dados.Administrador.Id, "Regra catalogo", dados.Categoria.Id));
        context.AprovacoesChamado.Add(new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Administrador.Id,
            "teste",
            dados.Solicitante.Id,
            "Servico X",
            "Aprovacao legada pendente"));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.True(response.JaExistiaAprovacaoEquivalente);
        Assert.Equal("Ja existe aprovacao legada pendente por catalogo para o chamado.", response.Motivo);
        Assert.Empty(context.InstanciasAprovacaoChamado);
        Assert.Single(context.AprovacoesChamado);
    }

    [Fact]
    public async Task NaoDeveGerarParaChamadoEmStatusFinalSemForcarReavaliacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "final", StatusChamadoEnum.Encerrado);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(dados.Administrador.Id, "Regra final", dados.Categoria.Id));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.Equal("Chamados em status final nao geram aprovacao obrigatoria automaticamente nesta etapa.", response.Motivo);
        Assert.Empty(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DeveGerarQuandoRegraCompativelComNatureza()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "natureza-exata");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra por natureza",
            dados.Categoria.Id,
            naturezaEspecifica: dados.Chamado.NaturezaChamado));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.True(response.GerouAprovacao);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task NaoDeveGerarQuandoRegraExigeNaturezaDiferente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "natureza-diferente");
        var useCase = CriarUseCase(context, dados.Administrador);

        var naturezaIncompativel = dados.Chamado.NaturezaChamado == NaturezaChamadoEnum.Incidente 
            ? NaturezaChamadoEnum.Requisicao 
            : NaturezaChamadoEnum.Incidente;

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra incompativel",
            dados.Categoria.Id,
            naturezaEspecifica: naturezaIncompativel));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.False(response.GerouAprovacao);
        Assert.Empty(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DevePermitirRegraGenericaSemNaturezaDefinida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "natureza-generica");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra generica",
            dados.Categoria.Id,
            naturezaEspecifica: null)); // Sem natureza
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.True(response.GerouAprovacao);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DevePreferirRegraEspecificaEmVezDaGenerica()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "concorrencia-natureza");
        var useCase = CriarUseCase(context, dados.Administrador);

        // Regra Genérica
        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra generica",
            dados.Categoria.Id,
            naturezaEspecifica: null));

        // Regra Específica
        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra especifica",
            dados.Categoria.Id,
            naturezaEspecifica: dados.Chamado.NaturezaChamado));

        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.True(response.GerouAprovacao);
        Assert.Equal("Regra especifica", response.NomeRegra);
    }

    [Fact]
    public async Task DeveAplicarSomenteRegraParaIncidenteQuandoChamadoForIncidente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "teste-incidente");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra Req",
            dados.Categoria.Id,
            naturezaEspecifica: NaturezaChamadoEnum.Requisicao));

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra Incidente",
            dados.Categoria.Id,
            naturezaEspecifica: NaturezaChamadoEnum.Incidente));

        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        });

        Assert.True(response.GerouAprovacao);
        Assert.Equal("Regra Incidente", response.NomeRegra);
    }

    [Fact]
    public async Task DeveAplicarSomenteRegraParaRequisicaoQuandoChamadoForRequisicao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "teste-requisicao");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra Incidente",
            dados.Categoria.Id,
            naturezaEspecifica: NaturezaChamadoEnum.Incidente));

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra Req",
            dados.Categoria.Id,
            naturezaEspecifica: NaturezaChamadoEnum.Requisicao));

        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        Assert.True(response.GerouAprovacao);
        Assert.Equal("Regra Req", response.NomeRegra);
    }

    [Fact]
    public async Task DeveGerarQuandoRegraCompativelComCatalogo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "catalogo-exato", possuiCatalogo: true);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra por catalogo",
            dados.Categoria.Id,
            catalogoServicoEspecifico: dados.Chamado.CatalogoServicoId));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            CatalogoServicoId = dados.Chamado.CatalogoServicoId
        });

        Assert.True(response.GerouAprovacao);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task NaoDeveGerarQuandoRegraExigeCatalogoDiferente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "catalogo-diferente", possuiCatalogo: true);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra incompativel",
            dados.Categoria.Id,
            catalogoServicoEspecifico: Guid.NewGuid()));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            CatalogoServicoId = dados.Chamado.CatalogoServicoId
        });

        Assert.False(response.GerouAprovacao);
        Assert.Empty(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DevePermitirRegraGenericaSemCatalogoDefinido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "catalogo-generica", possuiCatalogo: true);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra generica",
            dados.Categoria.Id,
            catalogoServicoEspecifico: null));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            CatalogoServicoId = dados.Chamado.CatalogoServicoId
        });

        Assert.True(response.GerouAprovacao);
        Assert.Single(context.InstanciasAprovacaoChamado);
    }

    [Fact]
    public async Task DevePreferirRegraEspecificaPorCatalogoEmVezDaGenerica()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "concorrencia-catalogo", possuiCatalogo: true);
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra generica",
            dados.Categoria.Id,
            catalogoServicoEspecifico: null));

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra especifica",
            dados.Categoria.Id,
            catalogoServicoEspecifico: dados.Chamado.CatalogoServicoId));

        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id,
            CatalogoServicoId = dados.Chamado.CatalogoServicoId
        });

        Assert.True(response.GerouAprovacao);
        Assert.Equal("Regra especifica", response.NomeRegra);
    }

    [Fact]
    public async Task DeveGerarInstanciaComTipoResolucaoPorGrupoFuturoSemTentarResolverAprovador()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioAsync(context, "grupo-futuro");
        var useCase = CriarUseCase(context, dados.Administrador);

        context.ConfiguracoesRegrasAprovacao.Add(CriarRegra(
            dados.Administrador.Id,
            "Regra grupo futuro",
            dados.Categoria.Id,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo));
        await context.SaveChangesAsync();

        var response = await useCase.ExecutarAsync(new GerarAprovacaoObrigatoriaChamadoRequest
        {
            ChamadoId = dados.Chamado.Id
        });

        Assert.True(response.GerouAprovacao);
        
        var instancia = await context.InstanciasAprovacaoChamado.SingleAsync();
        Assert.Equal(TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo, instancia.TipoResolucaoAprovador);
        Assert.Null(instancia.AprovadorResolvidoUsuarioId); // Não pode ter resolvido se é grupo futuro
    }

    private static GerarAprovacaoObrigatoriaChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuarioAtual)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<ConfiguracaoRegraAprovacao>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioAtual.Id,
                usuarioAtual.Nome,
                usuarioAtual.Email,
                usuarioAtual.Login,
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

    private static ConfiguracaoRegraAprovacao CriarRegra(
        Guid criadoPorUsuarioId,
        string nome,
        Guid categoriaId,
        bool exigeAprovacao = true,
        bool bloqueante = false,
        EfeitoOperacionalRegraAprovacao efeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
        TipoFluxoAprovacao tipoFluxoAprovacao = TipoFluxoAprovacao.Simples,
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
        Guid? aprovadorPadraoUsuarioId = null,
        int? prazoDecisaoHoras = 4,
        NaturezaChamadoEnum? naturezaEspecifica = NaturezaChamadoEnum.Requisicao,
        Guid? catalogoServicoEspecifico = null)
        => new(
            nome,
            TipoRegraAprovacao.Combinada,
            EscopoRegraAprovacao.AtendimentoChamado,
            efeitoOperacional,
            tipoFluxoAprovacao,
            tipoResolucaoAprovador,
            ordem: 1,
            prioridade: 100,
            versao: 1,
            criadoPorUsuarioId: criadoPorUsuarioId,
            criadoPor: "teste",
            descricao: $"Descricao da {nome}",
            naturezaChamado: naturezaEspecifica,
            catalogoServicoId: catalogoServicoEspecifico,
            categoriaId: categoriaId,
            impactoMinimo: ImpactoChamadoEnum.Baixo,
            urgenciaMinima: UrgenciaChamadoEnum.Baixa,
            exigeAprovacao: exigeAprovacao,
            bloqueante: bloqueante,
            aprovadorPadraoUsuarioId: aprovadorPadraoUsuarioId,
            prazoDecisaoHoras: prazoDecisaoHoras,
            vigenteDe: DateTime.UtcNow.Date.AddDays(-2),
            vigenteAte: DateTime.UtcNow.Date.AddDays(10));

    private static async Task<(Usuario Administrador, Usuario Solicitante, CategoriaChamado Categoria, Chamado Chamado)> CriarCenarioAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo,
        StatusChamadoEnum status = StatusChamadoEnum.Aberto,
        bool possuiCatalogo = false)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin {sufixo}", $"admin.{sufixo}@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol.{sufixo}@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {sufixo}");
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, status, prioridade.Id, sufixo);

        if (possuiCatalogo)
        {
            context.Entry(chamado).Property(nameof(Chamado.CatalogoServicoId)).CurrentValue = Guid.NewGuid();
            await context.SaveChangesAsync();
        }

        return (admin, solicitante, categoria, chamado);
    }
}
