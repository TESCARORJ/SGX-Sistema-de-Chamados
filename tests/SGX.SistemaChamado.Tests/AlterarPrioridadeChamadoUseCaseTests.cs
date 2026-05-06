using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
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
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin),
            PortalUseCasesTestFactory.Uow(context));

        var antes = context.SlaControles.Single(x => x.ChamadoId == dados.ChamadoId).PrazoResolucaoEm;
        var criticaId = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Critica).Id;

        await useCase.ExecutarAsync(dados.ChamadoId, new AlterarPrioridadeChamadoRequest { PrioridadeId = criticaId });

        var depois = context.SlaControles.Single(x => x.ChamadoId == dados.ChamadoId).PrazoResolucaoEm;
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

        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridadeAlta.Id, 4, 24, "teste"));
        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridadeCritica.Id, 1, 2, "teste"));
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
