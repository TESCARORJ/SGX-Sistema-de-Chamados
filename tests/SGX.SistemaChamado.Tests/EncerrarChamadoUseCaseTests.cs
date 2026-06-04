using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class EncerrarChamadoUseCaseTests
{
    [Fact]
    public async Task EncerraChamadoComSolucao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Problema resolvido" });

        Assert.Equal("Encerrado", response.Status);
        Assert.NotNull(context.Chamados.Single().EncerradoEm);
    }

    [Fact]
    public async Task ExigeSolucaoComentario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "" }));
    }

    [Fact]
    public async Task CriaHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.Encerrado);
    }

    [Fact]
    public async Task BloqueadoPorAtivoImpedeEncerramentoSemGravarHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var bloqueador = await CriarChamadoRelacionadoAsync(context, dados.Chamado, "ENC-BLOQ-1");
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.Chamado.Id,
            bloqueador.Id,
            TipoRelacionamentoChamadoEnum.BloqueadoPor,
            dados.Admin.Id,
            dados.Admin.Login,
            "Aguarda chamado bloqueador"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Tentativa bloqueada" }));

        Assert.Equal("Este chamado possui dependencia ativa e nao pode ser fechado enquanto estiver bloqueado por outro chamado.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).EncerradoEm);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Encerrado);
    }

    [Fact]
    public async Task BloqueiaNormalizadoImpedeEncerramentoDoChamadoDependente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var bloqueador = await CriarChamadoRelacionadoAsync(context, dados.Chamado, "ENC-BLOQ-2");
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            bloqueador.Id,
            dados.Chamado.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.Admin.Id,
            dados.Admin.Login,
            "Bloqueia encerramento"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Tentativa bloqueada" }));
    }

    [Fact]
    public async Task PermiteEncerrarQuandoVinculoDeBloqueioEstaInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var bloqueador = await CriarChamadoRelacionadoAsync(context, dados.Chamado, "ENC-INAT");
        var relacionamento = new ChamadoRelacionamento(
            bloqueador.Id,
            dados.Chamado.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.Admin.Id,
            dados.Admin.Login,
            "Bloqueio removido");
        relacionamento.Inativar(dados.Admin.Id, dados.Admin.Login, "Resolvido");
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Encerrado);
    }

    [Theory]
    [InlineData(TipoRelacionamentoChamadoEnum.Relacionado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Duplicado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Pai)]
    [InlineData(TipoRelacionamentoChamadoEnum.Filho)]
    [InlineData(TipoRelacionamentoChamadoEnum.DerivadoDe)]
    [InlineData(TipoRelacionamentoChamadoEnum.Origina)]
    public async Task TiposInformativosNaoImpedemEncerramento(TipoRelacionamentoChamadoEnum tipo)
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var outroChamado = await CriarChamadoRelacionadoAsync(context, dados.Chamado, $"ENC-INF-{tipo}");
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            dados.Chamado.Id,
            outroChamado.Id,
            tipo,
            dados.Admin.Id,
            dados.Admin.Login,
            "Vinculo informativo"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
    }

    [Fact]
    public async Task BloqueiaEncerramentoDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Tentativa duplicada" }));
    }

    [Fact]
    public async Task BloqueiaEncerramentoQuandoChamadoAguardaAprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Admin.Id,
            dados.Admin.Login,
            dados.Chamado.SolicitanteId,
            "Servico catalogo",
            "Aguarda aprovacao");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Tentativa" }));

        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).EncerradoEm);
        Assert.DoesNotContain(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.Encerrado);
    }

    [Fact]
    public async Task AprovacaoPendenteNaoBloqueanteNaoImpedeEncerramento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.AprovacoesChamado.Add(CriarAprovacao(dados, bloqueiaAvancoAtendimento: false));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
    }

    [Fact]
    public async Task AprovacaoAprovadaNaoImpedeEncerramento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = CriarAprovacao(dados, bloqueiaAvancoAtendimento: true);
        aprovacao.Aprovar(dados.Admin.Id, dados.Admin.Id, dados.Admin.Login, "Aprovado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
    }

    [Fact]
    public async Task AprovacaoReprovadaNaoImpedeEncerramentoNestaEtapa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = CriarAprovacao(dados, bloqueiaAvancoAtendimento: true);
        aprovacao.Reprovar(dados.Admin.Id, dados.Admin.Id, dados.Admin.Login, "Reprovado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
    }

    [Fact]
    public async Task AprovacaoCanceladaOuInativaNaoImpedeEncerramento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var cancelada = CriarAprovacao(dados, bloqueiaAvancoAtendimento: true);
        cancelada.CancelarVinculada(dados.Admin.Id, dados.Admin.Login, "Cancelada");
        var inativa = CriarAprovacao(dados, bloqueiaAvancoAtendimento: true);
        inativa.Desativar(dados.Admin.Login);
        context.AprovacoesChamado.AddRange(cancelada, inativa);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Resolvido" });

        Assert.Equal("Encerrado", response.Status);
    }

    [Fact]
    public async Task AprovacaoPendenteBloqueanteTemPrioridadeSobreDependenciaAtiva()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        context.AprovacoesChamado.Add(CriarAprovacao(dados, bloqueiaAvancoAtendimento: true));
        var bloqueador = await CriarChamadoRelacionadoAsync(context, dados.Chamado, "ENC-APR-DEP");
        context.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            bloqueador.Id,
            dados.Chamado.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.Admin.Id,
            dados.Admin.Login,
            "Bloqueia encerramento"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new EncerrarChamadoRequest { Solucao = "Tentativa" }));

        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);
    }

    private static EncerrarChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            CriarRelacionamentosUseCase(context, contexto),
            CriarAprovacoesUseCase(context, contexto),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

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

    private static AprovacaoChamado CriarAprovacao(
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

    private static async Task<Chamado> CriarChamadoRelacionadoAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Chamado chamadoBase,
        string sufixoCodigo)
    {
        var solicitante = await context.Usuarios.FindAsync(chamadoBase.SolicitanteId)
            ?? throw new InvalidOperationException("Solicitante de teste nao encontrado.");
        var categoria = await context.CategoriasChamado.FindAsync(chamadoBase.CategoriaId)
            ?? throw new InvalidOperationException("Categoria de teste nao encontrada.");

        return await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.EmAtendimento,
            sufixoCodigo: sufixoCodigo);
    }

    private static async Task<(Chamado Chamado, Usuario Admin, UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "ENC1");

        return (chamado, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}

