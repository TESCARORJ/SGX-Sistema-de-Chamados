using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class LocaisUnidadeAdminUseCaseTests
{
    [Fact]
    public async Task CriarLocalUnidadeValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.criar@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarLocalUnidadeUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarLocalUnidadeRequest
        {
            Nome = "Unidade Matriz",
            Descricao = "Principal",
            Endereco = "Rua A, 100"
        });

        Assert.Equal("Unidade Matriz", response.Nome);
        Assert.Equal("Rua A, 100", response.Endereco);
    }

    [Fact]
    public async Task BloqueiaLocalUnidadeDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.dup@empresa.com", TipoPerfil.Administrador);
        context.LocaisUnidade.Add(new LocalUnidade("Filial Sul", null, null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarLocalUnidadeUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarLocalUnidadeRequest
        {
            Nome = "Filial Sul",
            Endereco = "Endereco novo"
        }));
    }

    [Fact]
    public async Task EditarInativarReativarLocalUnidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.status@empresa.com", TipoPerfil.Administrador);
        var local = new LocalUnidade("Filial Norte", null, "Rua B, 200", "teste");
        context.LocaisUnidade.Add(local);
        await context.SaveChangesAsync();

        var atualizarUseCase = new AtualizarLocalUnidadeUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var atualizado = await atualizarUseCase.ExecutarAsync(local.Id, new AtualizarLocalUnidadeRequest
        {
            Nome = "Filial Norte Atualizada",
            Descricao = "Atualizada",
            Endereco = "Rua B, 250"
        });
        Assert.Equal("Filial Norte Atualizada", atualizado.Nome);
        Assert.Equal("Rua B, 250", atualizado.Endereco);

        var inativarUseCase = new InativarLocalUnidadeUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));
        var inativado = await inativarUseCase.ExecutarAsync(local.Id);
        Assert.False(inativado.Ativo);

        var reativarUseCase = new ReativarLocalUnidadeUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));
        var reativado = await reativarUseCase.ExecutarAsync(local.Id);
        Assert.True(reativado.Ativo);
    }

    [Fact]
    public async Task ListarBuscarEFiltrarLocaisUnidadePorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.list@empresa.com", TipoPerfil.Administrador);
        var ativo = new LocalUnidade("Polo Centro", null, "Centro 10", "teste");
        var inativo = new LocalUnidade("Polo Centro Legado", null, "Centro 12", "teste");
        inativo.Desativar("teste");
        context.LocaisUnidade.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarLocaisUnidadeAdminUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest
        {
            Texto = "Polo Centro",
            Ativo = true,
            OrdenarPor = "nome",
            DirecaoOrdenacao = "asc"
        });

        Assert.Single(response.Items);
        Assert.Equal("Polo Centro", response.Items.Single().Nome);
    }

    [Fact]
    public async Task ListarLocaisUnidadeComFiltroInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.list.inativo@empresa.com", TipoPerfil.Administrador);
        var ativo = new LocalUnidade("Local Ativo", null, "Endereco 1", "teste");
        var inativo = new LocalUnidade("Local Inativo", null, "Endereco 2", "teste");
        inativo.Desativar("teste");
        context.LocaisUnidade.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarLocaisUnidadeAdminUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Ativo = false, Texto = "Local" });

        Assert.Single(response.Items);
        Assert.Equal("Local Inativo", response.Items.Single().Nome);
        Assert.False(response.Items.Single().Ativo);
    }

    [Fact]
    public async Task ListarLocaisUnidadeSemFiltroRetornaAtivosEInativos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "local.list.todos@empresa.com", TipoPerfil.Administrador);
        var ativo = new LocalUnidade("Unidade Ativa", null, "Endereco 3", "teste");
        var inativo = new LocalUnidade("Unidade Inativa", null, "Endereco 4", "teste");
        inativo.Desativar("teste");
        context.LocaisUnidade.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarLocaisUnidadeAdminUseCase(
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Texto = "Unidade" });

        Assert.Contains(response.Items, x => x.Nome == "Unidade Ativa");
        Assert.Contains(response.Items, x => x.Nome == "Unidade Inativa");
    }
}
