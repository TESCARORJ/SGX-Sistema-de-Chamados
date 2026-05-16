using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class PrioridadesAdminUseCaseTests
{
    [Fact]
    public async Task CriarPrioridadeValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.admin@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = "Urgente Operacional",
            Descricao = "Atendimento imediato",
            Peso = 10,
            Cor = "#FF0000"
        });

        Assert.Equal("Urgente Operacional", response.Nome);
        Assert.Equal(10, response.Peso);
        Assert.Equal("#FF0000", response.Cor);
        Assert.True(response.Ativo);
    }

    [Fact]
    public async Task BloqueiaNomeDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.dup@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = "Baixa",
            Peso = 5
        }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task BloqueiaPesoInvalido(int pesoInvalido)
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.peso@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => useCase.ExecutarAsync(new CriarPrioridadeChamadoRequest
        {
            Nome = $"Prioridade {pesoInvalido}",
            Peso = pesoInvalido
        }));
    }

    [Fact]
    public async Task EditarPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.edit@empresa.com", TipoPerfil.Administrador);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nome == "Alta");

        var useCase = new AtualizarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(prioridade.Id, new AtualizarPrioridadeChamadoRequest
        {
            Nome = "Alta Revisada",
            Descricao = "Atualizada",
            Peso = 7,
            Cor = "#112233"
        });

        Assert.Equal("Alta Revisada", response.Nome);
        Assert.Equal(7, response.Peso);
        Assert.Equal("#112233", response.Cor);
    }

    [Fact]
    public async Task InativarEReativarPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.status@empresa.com", TipoPerfil.Administrador);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Nome == "Media");

        var inativarUseCase = new InativarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var inativarResponse = await inativarUseCase.ExecutarAsync(prioridade.Id);
        Assert.False(inativarResponse.Ativo);

        var reativarUseCase = new ReativarPrioridadeUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var reativarResponse = await reativarUseCase.ExecutarAsync(prioridade.Id);
        Assert.True(reativarResponse.Ativo);
    }

    [Fact]
    public async Task ListarBuscarEFiltrarPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.list@empresa.com", TipoPerfil.Administrador);

        var ativa = new PrioridadeChamado("Prioridade Busca", PrioridadeChamadoEnum.Media, "Ativa", 2, 8, "teste");
        ativa.DefinirPesoECor(9, "#00AA00");
        var inativa = new PrioridadeChamado("Prioridade Busca Inativa", PrioridadeChamadoEnum.Alta, "Inativa", 1, 4, "teste");
        inativa.DefinirPesoECor(11, "#AA0000");
        inativa.Desativar("teste");
        context.PrioridadesChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarPrioridadesAdminUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest
        {
            Texto = "Busca",
            Ativo = true,
            OrdenarPor = "peso",
            DirecaoOrdenacao = "asc"
        });

        Assert.Single(response.Items);
        Assert.Equal("Prioridade Busca", response.Items.Single().Nome);
    }

    [Fact]
    public async Task ListarPrioridadesComFiltroInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.list.inativo@empresa.com", TipoPerfil.Administrador);

        var inativa = new PrioridadeChamado("Prioridade Somente Inativa", PrioridadeChamadoEnum.Critica, null, 1, 4, "teste");
        inativa.DefinirPesoECor(20, "#550000");
        inativa.Desativar("teste");
        context.PrioridadesChamado.Add(inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarPrioridadesAdminUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest
        {
            Texto = "Somente Inativa",
            Ativo = false
        });

        Assert.Single(response.Items);
        Assert.Equal("Prioridade Somente Inativa", response.Items.Single().Nome);
        Assert.False(response.Items.Single().Ativo);
    }

    [Fact]
    public async Task ListarPrioridadesSemFiltroRetornaAtivasEInativas()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "prio.list.todos@empresa.com", TipoPerfil.Administrador);

        var ativa = new PrioridadeChamado("Prioridade Todos Ativa", PrioridadeChamadoEnum.Media, null, 2, 8, "teste");
        ativa.DefinirPesoECor(30, "#005500");
        var inativa = new PrioridadeChamado("Prioridade Todos Inativa", PrioridadeChamadoEnum.Alta, null, 1, 4, "teste");
        inativa.DefinirPesoECor(31, "#550055");
        inativa.Desativar("teste");
        context.PrioridadesChamado.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarPrioridadesAdminUseCase(
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Texto = "Prioridade Todos" });

        Assert.Contains(response.Items, x => x.Nome == "Prioridade Todos Ativa");
        Assert.Contains(response.Items, x => x.Nome == "Prioridade Todos Inativa");
    }
}
