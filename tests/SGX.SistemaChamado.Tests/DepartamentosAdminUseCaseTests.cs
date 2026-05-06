using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DepartamentosAdminUseCaseTests
{
    [Fact]
    public async Task CriaDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "dep.admin@empresa.com", TipoPerfil.Administrador);
        var useCase = CriarCriarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(new CriarDepartamentoRequest { Nome = "Jurídico", Sigla = "JUR", Descricao = "Suporte legal" });
        Assert.Equal("JUR", response.Sigla);
    }

    [Fact]
    public async Task RejeitaSiglaDuplicada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "dep2.admin@empresa.com", TipoPerfil.Administrador);
        context.Departamentos.Add(new Departamento("Compras", "CMP", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarCriarUseCase(context, admin);
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarDepartamentoRequest
        {
            Nome = "Comercial",
            Sigla = "CMP",
            Descricao = "Duplicado"
        }));
    }

    [Fact]
    public async Task AtualizaDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "dep3.admin@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("Atendimento", "ATD", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var useCase = new AtualizarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(departamento.Id, new AtualizarDepartamentoRequest { Nome = "Atendimento N1", Sigla = "AT1", Descricao = "Atualizado" });
        Assert.Equal("AT1", response.Sigla);
    }

    [Fact]
    public async Task InativaDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "dep4.admin@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("Contábil", "CON", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var useCase = new InativarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(departamento.Id);
        Assert.False(response.Ativo);
    }

    [Fact]
    public async Task ReativaDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "dep5.admin@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("RH", "RH", null, "teste");
        departamento.Desativar("teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var useCase = new ReativarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(departamento.Id);
        Assert.True(response.Ativo);
    }

    private static CriarDepartamentoUseCase CriarCriarUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));
}
