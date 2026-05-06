using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ListarChamadosAdminUseCaseTests
{
    [Fact]
    public async Task ListaChamadosDeDiferentesSolicitantes()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest());

        Assert.True(response.Total >= 2);
    }

    [Fact]
    public async Task AplicaFiltroPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { StatusId = dados.StatusEmAtendimentoId });

        Assert.All(response.Items, x => Assert.Equal("Em Atendimento", x.Status));
    }

    [Fact]
    public async Task AplicaFiltroPorPrioridade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { PrioridadeId = dados.PrioridadeAltaId });

        Assert.All(response.Items, x => Assert.Equal("Alta", x.Prioridade));
    }

    [Fact]
    public async Task AplicaFiltroPorCategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { CategoriaId = dados.CategoriaInfraId });

        Assert.All(response.Items, x => Assert.Equal("Infra", x.Categoria));
    }

    [Fact]
    public async Task AplicaPaginacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { Pagina = 1, TamanhoPagina = 1 });

        Assert.Single(response.Items);
        Assert.True(response.Total >= 2);
    }

    private static async Task<(UsuarioContextoAplicacao AtendenteContexto, Guid StatusEmAtendimentoId, Guid PrioridadeAltaId, Guid CategoriaInfraId)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten@empresa.com", TipoPerfil.Atendente);
        var solicitante1 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante 1", "sol1@empresa.com", TipoPerfil.Solicitante);
        var solicitante2 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante 2", "sol2@empresa.com", TipoPerfil.Solicitante);

        var categoriaInfra = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var categoriaSistemas = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Sistemas");

        var prioridadeAlta = context.PrioridadesChamado.First(x => x.Nome == "Alta");
        _ = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante1, categoriaInfra, StatusChamadoEnum.EmAtendimento, prioridadeAlta.Id, "001");
        _ = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante2, categoriaSistemas, StatusChamadoEnum.Aberto, null, "002");

        var statusEmAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        return (
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            statusEmAtendimento.Id,
            prioridadeAlta.Id,
            categoriaInfra.Id);
    }
}

