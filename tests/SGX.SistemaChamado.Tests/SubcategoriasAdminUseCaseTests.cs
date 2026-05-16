using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class SubcategoriasAdminUseCaseTests
{
    [Fact]
    public async Task CriaSubcategoriaValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);

        var useCase = new CriarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = categoria.Id,
            Nome = "Conta de rede",
            Descricao = "Problemas com conta de rede"
        });

        Assert.Equal(categoria.Id, response.CategoriaChamadoId);
        Assert.Equal("Conta de rede", response.Nome);
    }

    [Fact]
    public async Task BloqueiaSubcategoriaComCategoriaInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "sub1.admin@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = Guid.NewGuid(),
            Nome = "Conta de rede"
        }));
    }

    [Fact]
    public async Task BloqueiaDuplicidadeDentroDaMesmaCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        context.SubcategoriasChamado.Add(new SubcategoriaChamado(categoria.Id, "Conta de rede", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = categoria.Id,
            Nome = "Conta de rede"
        }));
    }

    [Fact]
    public async Task PermiteMesmoNomeEmCategoriasDiferentes()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var categoria2 = new CategoriaChamado("Aplicacoes", null, categoria.DepartamentoId, "teste");
        context.CategoriasChamado.Add(categoria2);
        context.SubcategoriasChamado.Add(new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = categoria2.Id,
            Nome = "Acesso"
        });

        Assert.Equal(categoria2.Id, response.CategoriaChamadoId);
    }

    [Fact]
    public async Task AtualizaSubcategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Email", null, "teste");
        context.SubcategoriasChamado.Add(subcategoria);
        await context.SaveChangesAsync();

        var useCase = new AtualizarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(subcategoria.Id, new AtualizarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = categoria.Id,
            Nome = "Email corporativo",
            Descricao = "Atualizada"
        });

        Assert.Equal("Email corporativo", response.Nome);
    }

    [Fact]
    public async Task InativaEReativaSubcategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var subcategoria = new SubcategoriaChamado(categoria.Id, "VPN", null, "teste");
        context.SubcategoriasChamado.Add(subcategoria);
        await context.SaveChangesAsync();

        var inativarUseCase = new InativarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var inativado = await inativarUseCase.ExecutarAsync(subcategoria.Id);
        Assert.False(inativado.Ativo);

        var reativarUseCase = new ReativarSubcategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var reativado = await reativarUseCase.ExecutarAsync(subcategoria.Id);
        Assert.True(reativado.Ativo);
    }

    [Fact]
    public async Task ListaSubcategoriasComBuscaEFiltroAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var ativa = new SubcategoriaChamado(categoria.Id, "Conta de email", null, "teste");
        var inativa = new SubcategoriaChamado(categoria.Id, "Conta legada", null, "teste");
        inativa.Desativar("teste");
        context.SubcategoriasChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarSubcategoriasAdminUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest
        {
            Texto = "Conta",
            Ativo = true,
            OrdenarPor = "nome",
            DirecaoOrdenacao = "asc"
        });

        Assert.Single(response.Items);
        Assert.Equal("Conta de email", response.Items.Single().Nome);
    }

    [Fact]
    public async Task ListaSubcategoriasComFiltroInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var ativa = new SubcategoriaChamado(categoria.Id, "Acesso ativo", null, "teste");
        var inativa = new SubcategoriaChamado(categoria.Id, "Acesso inativo", null, "teste");
        inativa.Desativar("teste");
        context.SubcategoriasChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarSubcategoriasAdminUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Ativo = false });

        Assert.Single(response.Items);
        Assert.Equal("Acesso inativo", response.Items.Single().Nome);
        Assert.False(response.Items.Single().Ativo);
    }

    [Fact]
    public async Task ListaSubcategoriasSemFiltroRetornaAtivasEInativas()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var ativa = new SubcategoriaChamado(categoria.Id, "Rede ativa", null, "teste");
        var inativa = new SubcategoriaChamado(categoria.Id, "Rede inativa", null, "teste");
        inativa.Desativar("teste");
        context.SubcategoriasChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarSubcategoriasAdminUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest());

        Assert.Contains(response.Items, x => x.Nome == "Rede ativa");
        Assert.Contains(response.Items, x => x.Nome == "Rede inativa");
    }

    [Fact]
    public async Task ListaSubcategoriasPorCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var categoria2 = new CategoriaChamado("Aplicacoes", null, categoria.DepartamentoId, "teste");
        context.CategoriasChamado.Add(categoria2);
        context.SubcategoriasChamado.Add(new SubcategoriaChamado(categoria.Id, "Rede", null, "teste"));
        context.SubcategoriasChamado.Add(new SubcategoriaChamado(categoria2.Id, "Sistema", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new ListarSubcategoriasPorCategoriaUseCase(
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(categoria.Id, true);
        Assert.Single(response);
        Assert.Equal("Rede", response.Single().Nome);
    }

    private static async Task<(Usuario admin, CategoriaChamado categoria)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "sub.seed@empresa.com", TipoPerfil.Administrador);
        var departamento = new Departamento("Tecnologia", "TEC", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria = new CategoriaChamado("Infraestrutura", null, departamento.Id, "teste");
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        return (admin, categoria);
    }
}
