using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AlterarPrioridadeChamadoUseCaseTests
{
    [Fact]
    public async Task RecalculaSlaAoMudarPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new AlterarPrioridadeChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin),
            PortalUseCasesTestFactory.Uow(context));

        var antes = context.ChamadosSla.Single(x => x.ChamadoId == dados.ChamadoId).PrazoResolucao;
        var criticaId = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Critica).Id;

        await useCase.ExecutarAsync(dados.ChamadoId, new AlterarPrioridadeChamadoRequest { PrioridadeId = criticaId });

        var depois = context.ChamadosSla.Single(x => x.ChamadoId == dados.ChamadoId).PrazoResolucao;
        Assert.True(depois < antes);
    }

    private static async Task<(UsuarioContextoAplicacao ContextoAdmin, Guid ChamadoId)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Prioridade", "admin.prioridade@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Prioridade", "sol.prioridade@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Prioridade Categoria");
        var prioridadeAlta = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var prioridadeCritica = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Critica);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        foreach (var politicaExistente in context.SlaPoliticas.Where(x => x.Ativo).ToList())
        {
            politicaExistente.Desativar("teste");
        }

        foreach (var metaExistente in context.SlaMetas.Where(x => x.Ativo).ToList())
        {
            metaExistente.Desativar("teste");
        }

        var politica = new PoliticaSla("SLA Teste Prioridade", "Teste", 1, null, null, null, false, true, "teste");
        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();

        context.SlaMetas.AddRange(
            new MetaSla(politica.Id, prioridadeAlta.Id, 240, 1440, null, null, "teste"),
            new MetaSla(politica.Id, prioridadeCritica.Id, 60, 120, null, null, "teste"));
        await context.SaveChangesAsync();

        var chamado = new Chamado("CH-APR-1", "Alterar prioridade", "Descricao", solicitante.Id, categoria.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();

        var slaService = SlaTestFactory.CriarService(context);
        await slaService.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow.AddHours(-2), default);
        await context.SaveChangesAsync();

        return (AdminUseCasesTestFactory.Contexto(admin, "Administrador"), chamado.Id);
    }
}
