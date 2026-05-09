using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CategoriasAdminUseCaseTests
{
    [Fact]
    public async Task CriaCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, departamento) = await SeedAsync(context);

        var useCase = new CriarCategoriaUseCase(
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarCategoriaChamadoRequest
        {
            Nome = "Infraestrutura",
            DepartamentoId = departamento.Id
        });

        Assert.Equal("Infraestrutura", response.Nome);
    }

    [Fact]
    public async Task RejeitaDuplicidadeNoMesmoDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, departamento) = await SeedAsync(context);
        context.CategoriasChamado.Add(new CategoriaChamado("Rede", null, departamento.Id, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarCategoriaUseCase(
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarCategoriaChamadoRequest
        {
            Nome = "Rede",
            DepartamentoId = departamento.Id
        }));
    }

    [Fact]
    public async Task InativaCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, departamento) = await SeedAsync(context);
        var categoria = new CategoriaChamado("Sistemas", null, departamento.Id, "teste");
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        var useCase = new InativarCategoriaUseCase(
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(categoria.Id);
        Assert.False(response.Ativo);
    }

    [Fact]
    public async Task CategoriaInativaNaoApareceNoContextoDoPortal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "cat.admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sol", "cat.sol@empresa.com", TipoPerfil.Solicitante);
        var departamento = new Departamento("Tecnologia", "TI", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var ativa = new CategoriaChamado("Ativa", null, departamento.Id, "teste");
        var inativa = new CategoriaChamado("Inativa", null, departamento.Id, "teste");
        inativa.Desativar(admin.Login);
        context.CategoriasChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ObterPortalContextoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante")),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao);

        var contexto = await useCase.ExecutarAsync();
        Assert.DoesNotContain(contexto.Categorias, x => x.Nome == "Inativa");
    }

    private static async Task<(Usuario admin, Departamento departamento)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "cat.seed@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("Operacoes", "OPE", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();
        return (admin, departamento);
    }
}
