using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DashboardAdminUseCaseTests
{
    [Fact]
    public async Task RetornaCardsComDadosReais()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.True(response.TotalAbertos >= 1);
        Assert.True(response.TotalSemResponsavel >= 1);
        Assert.NotEmpty(response.Cards);
    }

    [Fact]
    public async Task AplicaFiltroPorPeriodo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest
        {
            DataInicio = DateTime.UtcNow.AddDays(-1),
            DataFim = DateTime.UtcNow.AddDays(1)
        });

        Assert.True(response.TotalAbertos + response.TotalEmAtendimento + response.TotalAguardandoSolicitante + response.TotalResolvidosPeriodo + response.TotalEncerradosPeriodo >= 1);
    }

    [Fact]
    public async Task AplicaFiltroPorDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest { DepartamentoId = dados.DepartamentoInfraId });

        Assert.All(response.ChamadosPorCategoria, item => Assert.NotNull(item.Categoria));
    }

    [Fact]
    public async Task AplicaFiltroPorCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest { CategoriaId = dados.CategoriaInfraId });

        Assert.True(response.ChamadosPorCategoria.Sum(x => x.Total) >= 1);
    }

    [Fact]
    public async Task AplicaFiltroPorResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest { ResponsavelId = dados.Atendente1Id });

        Assert.True(response.ProdutividadePorAtendente.All(x => x.ResponsavelId == dados.Atendente1Id));
    }

    [Fact]
    public async Task ContaChamadosVencidosEProximosDoVencimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.True(response.TotalVencidos >= 1);
        Assert.True(response.TotalProximosDoVencimento >= 1);
    }

    [Fact]
    public async Task DashboardRetornaContadoresPorNatureza()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.Equal(6, response.ChamadosPorNatureza.Count);
        Assert.Contains(response.ChamadosPorNatureza, x => x.Codigo == (int)NaturezaChamadoEnum.Incidente && x.Total >= 1);
        Assert.Contains(response.ChamadosPorNatureza, x => x.Codigo == (int)NaturezaChamadoEnum.Requisicao && x.Total >= 1);
    }

    [Fact]
    public async Task DashboardAplicaFiltroPorNatureza()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var response = await useCase.ExecutarAsync(new FiltroIndicadoresRequest
        {
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        });

        Assert.All(response.ChamadosPorNatureza.Where(x => x.Codigo != (int)NaturezaChamadoEnum.Incidente), x => Assert.Equal(0, x.Total));
        Assert.Contains(response.ChamadosPorNatureza, x => x.Codigo == (int)NaturezaChamadoEnum.Incidente && x.Total >= 1);
    }

    private static AdminIndicadoresUseCases CriarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto));

    private static async Task<(UsuarioContextoAplicacao ContextoAdmin, Guid DepartamentoInfraId, Guid CategoriaInfraId, Guid Atendente1Id)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.dashboard@sgx.local", TipoPerfil.Administrador);
        var atendente1 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente 1", "at1@sgx.local", TipoPerfil.Atendente);
        var atendente2 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente 2", "at2@sgx.local", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.dashboard@sgx.local", TipoPerfil.Solicitante);

        var departamentoInfra = new Departamento("Infra", "INF", null, "teste");
        var departamentoApps = new Departamento("Apps", "APP", null, "teste");
        context.Departamentos.AddRange(departamentoInfra, departamentoApps);
        await context.SaveChangesAsync();

        var categoriaInfra = new CategoriaChamado("Infra", null, departamentoInfra.Id, "teste");
        var categoriaApps = new CategoriaChamado("Aplicacoes", null, departamentoApps.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaInfra, categoriaApps);
        await context.SaveChangesAsync();

        var prioridadeAlta = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var statusAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var statusAguardando = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.AguardandoSolicitante);

        var chamadoVencido = new Chamado("CH-DB-001", "Vencido", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAtendimento.Id, OrigemChamado.Portal, "teste", departamentoInfra.Id, naturezaChamado: NaturezaChamadoEnum.Incidente);
        var chamadoProximo = new Chamado("CH-DB-002", "Proximo", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste", departamentoInfra.Id, naturezaChamado: NaturezaChamadoEnum.Requisicao);
        var chamadoSemResponsavel = new Chamado("CH-DB-003", "Sem responsavel", "Descricao", solicitante.Id, categoriaApps.Id, prioridadeAlta.Id, statusAguardando.Id, OrigemChamado.Portal, "teste", departamentoApps.Id, naturezaChamado: NaturezaChamadoEnum.Mudanca);

        chamadoVencido.AtribuirResponsavel(atendente1.Id, "teste");
        chamadoProximo.AtribuirResponsavel(atendente1.Id, "teste");
        chamadoSemResponsavel.AtribuirResponsavel(atendente2.Id, "teste");
        chamadoSemResponsavel.AtribuirResponsavel(null, "teste");

        context.Chamados.AddRange(chamadoVencido, chamadoProximo, chamadoSemResponsavel);
        await context.SaveChangesAsync();

        var vencido = new ChamadoSla(
            chamadoVencido.Id,
            null,
            prioridadeAlta.Id,
            DateTime.UtcNow.AddHours(-8),
            DateTime.UtcNow.AddHours(-6),
            DateTime.UtcNow.AddHours(-1),
            true,
            false,
            null,
            "teste");
        vencido.RegistrarResolucao(DateTime.UtcNow, "teste");

        var proximo = new ChamadoSla(
            chamadoProximo.Id,
            null,
            prioridadeAlta.Id,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddMinutes(50),
            true,
            false,
            null,
            "teste");

        var normal = new ChamadoSla(
            chamadoSemResponsavel.Id,
            null,
            prioridadeAlta.Id,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(10),
            true,
            false,
            null,
            "teste");
        normal.IniciarPausa(DateTime.UtcNow.AddMinutes(-20), "teste");

        context.ChamadosSla.AddRange(vencido, proximo, normal);
        await context.SaveChangesAsync();

        return (
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"),
            departamentoInfra.Id,
            categoriaInfra.Id,
            atendente1.Id);
    }
}
