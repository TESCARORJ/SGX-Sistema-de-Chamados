using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SGX.SistemaChamado.Tests;

public sealed class PrioridadesAdminUseCaseTests
{
    [Fact]
    public async Task CriaPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.admin@empresa.com", TipoPerfil.Administrador);
        var prioridadeExistente = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Critica);
        context.PrioridadesChamado.Remove(prioridadeExistente);
        await context.SaveChangesAsync();

        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = "Urgente Operacional",
            Nivel = 4,
            PrazoPrimeiraRespostaHoras = 1,
            PrazoResolucaoHoras = 4
        });

        Assert.Equal(4, response.Nivel);
    }

    [Fact]
    public async Task RejeitaNivelDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio2.admin@empresa.com", TipoPerfil.Administrador);
        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = "Outra Baixa",
            Nivel = 1,
            PrazoPrimeiraRespostaHoras = 1,
            PrazoResolucaoHoras = 8
        }));
    }

    [Fact]
    public async Task RejeitaPrazoNegativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio3.admin@empresa.com", TipoPerfil.Administrador);
        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAnyAsync<Exception>(() => useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = "Negativa",
            Nivel = 4,
            PrazoPrimeiraRespostaHoras = -1,
            PrazoResolucaoHoras = 2
        }));
    }

    [Fact]
    public async Task InativaPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio4.admin@empresa.com", TipoPerfil.Administrador);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta);

        var useCase = new InativarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(prioridade.Id);
        Assert.False(response.Ativo);
    }

    [Fact]
    public async Task PrioridadeInativaNaoApareceNoContextoDoPortal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio5.admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sol", "prio.sol@empresa.com", TipoPerfil.Solicitante);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Baixa);
        prioridade.Desativar(admin.Login);
        await context.SaveChangesAsync();

        var useCase = new ObterPortalContextoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante")),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao);

        var portal = await useCase.ExecutarAsync();
        Assert.DoesNotContain(portal.Prioridades, x => x.Id == prioridade.Id);
    }
}
