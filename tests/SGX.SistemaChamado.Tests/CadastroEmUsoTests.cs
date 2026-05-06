using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SGX.SistemaChamado.Tests;

public sealed class CadastroEmUsoTests
{
    [Fact]
    public async Task NaoExcluiFisicamenteCadastroEmUso()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "uso.admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sol", "uso.sol@empresa.com", TipoPerfil.Solicitante);
        var departamento = new Departamento("Infra", "INF", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria = new CategoriaChamado("Rede", null, departamento.Id, "teste");
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "USO1");

        var useCase = new InativarCategoriaUseCase(
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(categoria.Id);

        Assert.NotNull(await context.CategoriasChamado.FindAsync(categoria.Id));
        Assert.NotNull(await context.Chamados.FindAsync(chamado.Id));
    }

    [Fact]
    public async Task InativacaoMantemChamadosHistoricosIntegros()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "uso2.admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sol", "uso2.sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Aplicacao");
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, prioridade.Id, "USO2");

        var inativarUseCase = new InativarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await inativarUseCase.ExecutarAsync(prioridade.Id);

        var chamadoPersistido = await context.Chamados.FindAsync(chamado.Id);
        Assert.NotNull(chamadoPersistido);
        Assert.Equal(prioridade.Id, chamadoPersistido!.PrioridadeId);
    }
}
