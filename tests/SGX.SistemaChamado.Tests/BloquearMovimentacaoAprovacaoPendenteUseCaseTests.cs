using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class BloquearMovimentacaoAprovacaoPendenteUseCaseTests
{
    private const string MensagemBloqueioAprovacaoPendente =
        "Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.";

    [Fact]
    public async Task PermiteAcaoQuandoNaoHaAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
        Assert.False(response.ApenasSinalizacao);
    }

    [Fact]
    public async Task BloqueiaAcaoFinalQuandoHaAprovacaoLegadaPendenteEBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        context.AprovacoesChamado.Add(CriarAprovacaoLegada(dados, bloqueiaAvancoAtendimento: true));
        await context.SaveChangesAsync();

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.False(response.Permitido);
        Assert.True(response.Bloqueado);
        Assert.Equal("AprovacaoChamado", response.OrigemBloqueio);
        Assert.True(response.Bloqueante);
    }

    [Fact]
    public async Task BloqueiaAcaoFinalQuandoHaInstanciaPendenteEBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.False(response.Permitido);
        Assert.True(response.Bloqueado);
        Assert.Equal("InstanciaAprovacaoChamado", response.OrigemBloqueio);
        Assert.NotNull(response.InstanciaAprovacaoChamadoId);
    }

    [Fact]
    public async Task BloqueiaAcaoFinalQuandoHaInstanciaEmReavaliacaoEBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.EmReavaliacao, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.False(response.Permitido);
        Assert.True(response.Bloqueado);
        Assert.Equal("InstanciaAprovacaoChamado", response.OrigemBloqueio);
        Assert.NotNull(response.InstanciaAprovacaoChamadoId);
    }

    [Fact]
    public async Task PermiteComentarioMesmoComAprovacaoPendenteBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Comentar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
        Assert.True(response.PodeContinuarTriagem);
    }

    [Fact]
    public async Task PermiteTriagemMesmoComAprovacaoPendenteBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        context.AprovacoesChamado.Add(CriarAprovacaoLegada(dados, bloqueiaAvancoAtendimento: true));
        await context.SaveChangesAsync();

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Triagem
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
    }

    [Fact]
    public async Task NaoBloqueiaQuandoInstanciaExigeAprovacaoMasNaoEhBloqueante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, false, EfeitoOperacionalRegraAprovacao.ExigirAprovacao);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
        Assert.True(response.ApenasSinalizacao);
    }

    [Fact]
    public async Task NaoBloqueiaQuandoInstanciaNaoExigeAprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, false, EfeitoOperacionalRegraAprovacao.Sinalizar);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
    }

    [Fact]
    public async Task NaoBloqueiaPorInstanciaPendenteDeOutroChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados1 = await SeedAsync(context);
        var dados2 = await SeedAsync(context);
        
        await CriarInstanciaAsync(context, dados2, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados1.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
    }

    [Theory]
    [InlineData(StatusInstanciaAprovacaoChamado.Aprovada)]
    [InlineData(StatusInstanciaAprovacaoChamado.Reprovada)]
    [InlineData(StatusInstanciaAprovacaoChamado.Cancelada)]
    [InlineData(StatusInstanciaAprovacaoChamado.Expirada)]
    [InlineData(StatusInstanciaAprovacaoChamado.Substituida)]
    public async Task NaoBloqueiaQuandoInstanciaNaoEstaMaisPendente(StatusInstanciaAprovacaoChamado status)
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, status, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.Encerrar
        });

        Assert.True(response.Permitido);
        Assert.False(response.Bloqueado);
    }

    [Fact]
    public async Task BloqueiaAlteracaoParaStatusFinalQuandoPendenciaBloqueanteExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var statusResolvidoId = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Resolvido).Id;
        var useCase = CriarValidador(context);
        var response = await useCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
        {
            ChamadoId = dados.Chamado.Id,
            TipoAcao = TipoAcaoMovimentacaoChamado.AlterarStatus,
            StatusDestinoId = statusResolvidoId
        });

        Assert.True(response.Bloqueado);
    }

    [Fact]
    public async Task IntegracaoImpedeAlterarStatusFinalQuandoInstanciaPendenteBloqueanteExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var statusResolvidoId = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Resolvido).Id;
        var useCase = CriarAlterarStatusUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AlterarStatusChamadoRequest { StatusId = statusResolvidoId }));

        Assert.Equal(MensagemBloqueioAprovacaoPendente, ex.Message);
    }

    [Fact]
    public async Task IntegracaoImpedeAssumirQuandoInstanciaPendenteBloqueanteExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await CriarInstanciaAsync(context, dados, StatusInstanciaAprovacaoChamado.Pendente, true, EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco);

        var useCase = CriarAssumirUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id));

        Assert.Equal(MensagemBloqueioAprovacaoPendente, ex.Message);
    }

    private static ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase CriarValidador(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context));

    private static AlterarStatusChamadoUseCase CriarAlterarStatusUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
    {
        var validador = CriarValidador(context);
        return new AlterarStatusChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            CriarRelacionamentosUseCase(context, contexto),
            CriarAprovacoesUseCase(context, contexto),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            null,
            validador);
    }

    private static AssumirChamadoUseCase CriarAssumirUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
    {
        var validador = CriarValidador(context);
        return new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context),
            null,
            validador);
    }

    private static RelacionamentosChamadoUseCases CriarRelacionamentosUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static ChamadoAprovacoesUseCases CriarAprovacoesUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static AprovacaoChamado CriarAprovacaoLegada(
        (Chamado Chamado, Usuario Admin, UsuarioContextoAplicacao AdminContexto) dados,
        bool bloqueiaAvancoAtendimento)
        => new(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao",
            bloqueiaAvancoAtendimento: bloqueiaAvancoAtendimento);

    private static async Task CriarInstanciaAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        (Chamado Chamado, Usuario Admin, UsuarioContextoAplicacao AdminContexto) dados,
        StatusInstanciaAprovacaoChamado status,
        bool bloqueante,
        EfeitoOperacionalRegraAprovacao efeitoOperacional)
    {
        var instancia = new InstanciaAprovacaoChamado(
            dados.Chamado.Id,
            dados.Chamado.SolicitanteId,
            OrigemInstanciaAprovacaoChamado.RegraMotor,
            TipoFluxoAprovacao.Simples,
            efeitoOperacional,
            EscopoRegraAprovacao.AtendimentoChamado,
            TipoRegraAprovacao.TipoSolicitacao,
            exigeAprovacao: bloqueante || efeitoOperacional == EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            bloqueante: bloqueante,
            TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            dados.Admin.Id,
            dados.Admin.Login,
            titulo: "Instancia teste",
            descricao: "Instancia de aprovacao para teste de bloqueio.",
            naturezaChamado: dados.Chamado.NaturezaChamado,
            categoriaId: dados.Chamado.CategoriaId,
            prazoDecisaoHoras: 4);

        switch (status)
        {
            case StatusInstanciaAprovacaoChamado.Aprovada:
                instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, dados.Admin.Id, dados.Admin.Id, dados.Admin.Login);
                break;
            case StatusInstanciaAprovacaoChamado.Reprovada:
                instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Reprovada, dados.Admin.Id, dados.Admin.Id, dados.Admin.Login);
                break;
            case StatusInstanciaAprovacaoChamado.Cancelada:
                instancia.MarcarCancelada(dados.Admin.Id, dados.Admin.Id, dados.Admin.Login, "Cancelada");
                break;
            case StatusInstanciaAprovacaoChamado.Expirada:
                instancia.MarcarExpirada(dados.Admin.Id, dados.Admin.Login);
                break;
            case StatusInstanciaAprovacaoChamado.Substituida:
                instancia.MarcarSubstituida(dados.Admin.Id, dados.Admin.Login);
                break;
            case StatusInstanciaAprovacaoChamado.EmReavaliacao:
                instancia.MarcarEmReavaliacao(dados.Admin.Id, dados.Admin.Login);
                break;
        }

        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();
    }

    private static async Task<(Chamado Chamado, Usuario Admin, UsuarioContextoAplicacao AdminContexto)> SeedAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Bloqueio", $"admin.bloqueio.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Bloqueio", $"sol.bloqueio.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Infra Bloqueio {Guid.NewGuid():N}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "BLQ1");

        return (chamado, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
