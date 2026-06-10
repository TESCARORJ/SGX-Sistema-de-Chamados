using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoAprovacaoUseCaseTests
{
    [Fact]
    public async Task DeveCriarAprovacaoVinculadaAChamadoExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(dados.Aprovador.Id, dados.Admin.Id));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(dados.Chamado.Id, response.ChamadoId);
        Assert.Equal("Aprovacao de acesso ao sistema", response.Titulo);
        Assert.Equal(StatusAprovacaoChamado.Pendente, response.Status);
        Assert.Equal(dados.Aprovador.Id, response.AprovadorUsuarioId);
        Assert.Equal(dados.Admin.Id, response.SolicitadoPorUsuarioId);
        Assert.True(response.Ativo);
        Assert.False(context.AprovacoesChamado.Single().BloqueiaAvancoAtendimento);
    }

    [Fact]
    public async Task DevePermitirMaisDeUmaAprovacaoPendenteVinculadaNoMesmoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Aprovacao de acesso"));
        await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Aprovacao de execucao fora de janela"));

        Assert.Equal(2, context.AprovacoesChamado.Count(x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Status == StatusAprovacaoChamado.Pendente));
    }

    [Fact]
    public async Task DeveBloquearCriacaoParaChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.CriarAsync(Guid.NewGuid(), CriarRequest()));

        Assert.Equal("Chamado nao encontrado.", ex.Message);
        Assert.Empty(context.AprovacoesChamado);
    }

    [Fact]
    public async Task DeveBloquearCriacaoSemTitulo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.CriarAsync(dados.Chamado.Id, new CriarChamadoAprovacaoAdminRequest { Titulo = " " }));

        Assert.Contains("titulo da aprovacao e obrigatorio", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveListarApenasAprovacoesDoChamadoInformado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var outroChamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            dados.Solicitante,
            dados.Categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "APR-OUTRO");

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacaoDoChamado = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Aprovacao do chamado"));
        await useCase.CriarAsync(outroChamado.Id, CriarRequest(titulo: "Aprovacao de outro chamado"));

        var aprovacoes = await useCase.ListarPorChamadoAsync(dados.Chamado.Id);

        var aprovacao = Assert.Single(aprovacoes);
        Assert.Equal(aprovacaoDoChamado.Id, aprovacao.Id);
        Assert.Equal(dados.Chamado.Id, aprovacao.ChamadoId);
    }

    [Fact]
    public async Task DeveAprovarAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacao = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        var response = await useCase.AprovarAsync(
            dados.Chamado.Id,
            aprovacao.Id,
            new DecidirChamadoAprovacaoAdminRequest { JustificativaDecisao = "Acesso aprovado" });

        Assert.Equal(StatusAprovacaoChamado.Aprovado, response.Status);
        Assert.NotNull(response.DecididoEm);
        Assert.Equal("Acesso aprovado", response.JustificativaDecisao);
    }

    [Fact]
    public async Task DeveReprovarAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacao = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        var response = await useCase.ReprovarAsync(
            dados.Chamado.Id,
            aprovacao.Id,
            new DecidirChamadoAprovacaoAdminRequest { JustificativaDecisao = "Evidencias insuficientes" });

        Assert.Equal(StatusAprovacaoChamado.Reprovado, response.Status);
        Assert.NotNull(response.DecididoEm);
        Assert.Equal("Evidencias insuficientes", response.JustificativaDecisao);
    }

    [Fact]
    public async Task DeveCancelarAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacao = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        await useCase.CancelarAsync(
            dados.Chamado.Id,
            aprovacao.Id,
            new CancelarChamadoAprovacaoAdminRequest { MotivoCancelamento = "Solicitacao substituida" });

        var entidade = context.AprovacoesChamado.Single(x => x.Id == aprovacao.Id);
        Assert.Equal(StatusAprovacaoChamado.Cancelado, entidade.Status);
        Assert.NotNull(entidade.CanceladoEm);
        Assert.Equal("Solicitacao substituida", entidade.MotivoCancelamento);
        Assert.False(entidade.Ativo);
    }

    [Fact]
    public async Task DeveBloquearDecisaoOuCancelamentoDeAprovacaoJaDecidida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacao = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());
        await useCase.AprovarAsync(dados.Chamado.Id, aprovacao.Id, new DecidirChamadoAprovacaoAdminRequest());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.AprovarAsync(dados.Chamado.Id, aprovacao.Id, new DecidirChamadoAprovacaoAdminRequest()));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ReprovarAsync(dados.Chamado.Id, aprovacao.Id, new DecidirChamadoAprovacaoAdminRequest()));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.CancelarAsync(dados.Chamado.Id, aprovacao.Id, new CancelarChamadoAprovacaoAdminRequest()));
    }

    [Fact]
    public async Task DeveRegistrarHistoricosDaAprovacaoVinculada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var aprovada = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Aprovar acesso"));
        await useCase.AprovarAsync(
            dados.Chamado.Id,
            aprovada.Id,
            new DecidirChamadoAprovacaoAdminRequest { JustificativaDecisao = "Permitido" });

        var reprovada = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Reprovar execucao"));
        await useCase.ReprovarAsync(
            dados.Chamado.Id,
            reprovada.Id,
            new DecidirChamadoAprovacaoAdminRequest { JustificativaDecisao = "Fora da janela" });

        var cancelada = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Cancelar compra"));
        await useCase.CancelarAsync(
            dados.Chamado.Id,
            cancelada.Id,
            new CancelarChamadoAprovacaoAdminRequest { MotivoCancelamento = "Compra nao necessaria" });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.AprovacaoCriada);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.AprovacaoAprovada);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.AprovacaoReprovada);
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.AprovacaoCancelada &&
            x.Descricao.Contains("Compra nao necessaria"));
    }

    [Fact]
    public async Task DevePermitirOperacaoParaAtendenteEBloquearSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var atendenteUseCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Atendente, "Atendente"));
        var response = await atendenteUseCase.CriarAsync(dados.Chamado.Id, CriarRequest());
        Assert.NotEqual(Guid.Empty, response.Id);

        var solicitanteUseCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Solicitante, "Solicitante"));
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            solicitanteUseCase.CriarAsync(dados.Chamado.Id, CriarRequest()));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
    }

    [Fact]
    public async Task DeveCoexistirFluxoLegadoComMotorNovoSemInterferencia()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        // Cria aprovação legada
        var useCaseLegado = CriarUseCase(context, dados.ContextoAdmin);
        var aprovacaoLegada = await useCaseLegado.CriarAsync(dados.Chamado.Id, CriarRequest(titulo: "Aprovacao legada"));

        // Cria instância no motor novo
        var instanciaNova = new InstanciaAprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            solicitanteId: dados.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.Manual,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            criadoPorUsuarioId: dados.Admin.Id,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            aprovadorPadraoUsuarioId: dados.Aprovador.Id,
            regraNomeSnapshot: "Motor Novo",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Mudanca");
            
        context.InstanciasAprovacaoChamado.Add(instanciaNova);
        await context.SaveChangesAsync();

        // Aprova legado
        var responseLegado = await useCaseLegado.AprovarAsync(
            dados.Chamado.Id,
            aprovacaoLegada.Id,
            new DecidirChamadoAprovacaoAdminRequest { JustificativaDecisao = "Acesso aprovado legado" });

        Assert.Equal(StatusAprovacaoChamado.Aprovado, responseLegado.Status);

        // Instancia nova continua pendente sem sofrer interferencia do fluxo legado
        var persistidaNova = await context.InstanciasAprovacaoChamado.SingleAsync(x => x.Id == instanciaNova.Id);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, persistidaNova.Status);
    }

    private static CriarChamadoAprovacaoAdminRequest CriarRequest(
        Guid? aprovadorId = null,
        Guid? solicitadoPorId = null,
        string titulo = "Aprovacao de acesso ao sistema")
        => new()
        {
            Titulo = titulo,
            Descricao = "Decisao formal para continuidade do atendimento.",
            AprovadorUsuarioId = aprovadorId,
            SolicitadoPorUsuarioId = solicitadoPorId,
            JustificativaSolicitacao = "Necessario validar autorizacao operacional."
        };

    private static ChamadoAprovacoesUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<DadosAprovacao> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Aprovacao Vinculada",
            $"admin.aprov.vinc.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Aprovacao Vinculada",
            $"atendente.aprov.vinc.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Aprovacao Vinculada",
            $"sol.aprov.vinc.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var aprovador = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Aprovador Aprovacao Vinculada",
            $"aprovador.aprov.vinc.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Atendente);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Aprovacao {Guid.NewGuid():N}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "APRV-001");

        return new DadosAprovacao(
            chamado,
            categoria,
            admin,
            atendente,
            solicitante,
            aprovador,
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private sealed record DadosAprovacao(
        Chamado Chamado,
        CategoriaChamado Categoria,
        Usuario Admin,
        Usuario Atendente,
        Usuario Solicitante,
        Usuario Aprovador,
        UsuarioContextoAplicacao ContextoAdmin);
}
