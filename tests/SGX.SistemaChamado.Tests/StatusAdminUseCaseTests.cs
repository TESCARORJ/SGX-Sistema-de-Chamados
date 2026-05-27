using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SGX.SistemaChamado.Tests;

public sealed class StatusAdminUseCaseTests
{
    [Fact]
    public async Task CriaStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "status.admin@empresa.com", TipoPerfil.Administrador);
        var statusCancelado = await context.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Cancelado);
        context.StatusChamado.Remove(statusCancelado);
        await context.SaveChangesAsync();

        var useCase = new CriarStatusUseCase(
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarStatusChamadoRequest
        {
            Nome = "Cancelado Operacional",
            Codigo = (int)StatusChamadoEnum.Cancelado,
            Descricao = "Status de cancelamento",
            EhStatusFinal = true,
            PausaSla = true
        });

        Assert.Equal((int)StatusChamadoEnum.Cancelado, response.Codigo);
    }

    [Fact]
    public async Task RejeitaCodigoDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "status2.admin@empresa.com", TipoPerfil.Administrador);
        var useCase = new CriarStatusUseCase(
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarStatusChamadoRequest
        {
            Nome = "Duplicado Aberto",
            Codigo = (int)StatusChamadoEnum.Aberto
        }));
    }

    [Fact]
    public async Task RejeitaInativarTodosOsStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "status3.admin@empresa.com", TipoPerfil.Administrador);
        var statusAtivos = await context.StatusChamado.Where(x => x.Ativo).OrderBy(x => x.Codigo).ToListAsync();
        Assert.True(statusAtivos.Count >= 2);
        var statusQuePermaneceraAtivo = statusAtivos.Last();
        foreach (var status in statusAtivos.Where(x => x.Id != statusQuePermaneceraAtivo.Id))
        {
            status.Desativar(admin.Login);
        }
        await context.SaveChangesAsync();

        var useCase = new InativarStatusUseCase(
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(statusQuePermaneceraAtivo.Id));
    }

    [Fact]
    public async Task StatusInativoNaoApareceParaAlteracaoAdministrativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "status4.admin@empresa.com", TipoPerfil.Administrador);
        var status = await context.StatusChamado.FirstAsync(x => x.Ativo && x.Codigo != StatusChamadoEnum.Aberto);
        status.Desativar(admin.Login);
        await context.SaveChangesAsync();

        var useCase = new ListarStatusAdminUseCase(
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Ativo = true });
        Assert.DoesNotContain(response.Items, x => x.Id == status.Id);
    }
}
