using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class IndicadoresUseCaseTests
{
    [Fact]
    public async Task RetornaChamadosPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var ctx = await SeedAsync(context);
        var useCase = CriarUseCase(context, ctx);

        var response = await ((IObterIndicadoresChamadosPorStatusUseCase)useCase).ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task RetornaChamadosPorPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var ctx = await SeedAsync(context);
        var useCase = CriarUseCase(context, ctx);

        var response = await ((IObterIndicadoresChamadosPorPrioridadeUseCase)useCase).ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task RetornaChamadosPorCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var ctx = await SeedAsync(context);
        var useCase = CriarUseCase(context, ctx);

        var response = await ((IObterIndicadoresChamadosPorCategoriaUseCase)useCase).ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task RetornaIndicadoresDeSla()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var ctx = await SeedAsync(context);
        var useCase = CriarUseCase(context, ctx);

        var response = await ((IObterIndicadoresSlaUseCase)useCase).ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.True(response.TotalChamados >= 1);
        Assert.True(response.TotalVencidos >= 1);
    }

    [Fact]
    public async Task RetornaProdutividadePorAtendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var ctx = await SeedAsync(context);
        var useCase = CriarUseCase(context, ctx);

        var response = await ((IObterIndicadoresProdutividadeUseCase)useCase).ExecutarAsync(new FiltroIndicadoresRequest());

        Assert.NotEmpty(response);
        Assert.All(response, x => Assert.False(string.IsNullOrWhiteSpace(x.ResponsavelNome)));
    }

    private static AdminIndicadoresUseCases CriarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto));

    private static async Task<UsuarioContextoAplicacao> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Indicador", "admin.indicador@sgx.local", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Indicador", "at.indicador@sgx.local", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Indicador", "sol.indicador@sgx.local", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Indicadores");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Media);
        var statusAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var statusEncerrado = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Encerrado);

        var chamado1 = new Chamado("CH-IND-001", "Indicador 1", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAtendimento.Id, OrigemChamado.Portal, "teste");
        chamado1.AtribuirResponsavel(atendente.Id, "teste");
        var chamado2 = new Chamado("CH-IND-002", "Indicador 2", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusEncerrado.Id, OrigemChamado.Portal, "teste");
        chamado2.AtribuirResponsavel(atendente.Id, "teste");

        context.Chamados.AddRange(chamado1, chamado2);
        await context.SaveChangesAsync();

        var sla1 = new SlaControle(chamado1.Id, DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(1), "teste");
        var sla2 = new SlaControle(chamado2.Id, DateTime.UtcNow.AddHours(-8), DateTime.UtcNow.AddHours(-1), "teste");
        sla2.RegistrarResolucao(DateTime.UtcNow, "teste");
        context.SlaControles.AddRange(sla1, sla2);
        await context.SaveChangesAsync();

        return AdminUseCasesTestFactory.Contexto(admin, "Administrador");
    }
}
