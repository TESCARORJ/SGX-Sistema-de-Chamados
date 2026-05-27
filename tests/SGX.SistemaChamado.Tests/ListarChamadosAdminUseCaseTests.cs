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
        Assert.All(response.Items, item => Assert.True(Enum.IsDefined(item.NaturezaChamado)));
        Assert.All(response.Items, item => Assert.True(Enum.IsDefined(item.ImpactoChamado)));
        Assert.All(response.Items, item => Assert.True(Enum.IsDefined(item.UrgenciaChamado)));
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
    public async Task AplicaFiltroPorNaturezaIncidente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { NaturezaChamado = NaturezaChamadoEnum.Incidente });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Equal(NaturezaChamadoEnum.Incidente, x.NaturezaChamado));
    }

    [Fact]
    public async Task AplicaFiltroPorNaturezaRequisicao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { NaturezaChamado = NaturezaChamadoEnum.Requisicao });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Equal(NaturezaChamadoEnum.Requisicao, x.NaturezaChamado));
    }

    [Fact]
    public async Task CombinaFiltroNaturezaComStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest
        {
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            StatusId = dados.StatusEmAtendimentoId
        });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x =>
        {
            Assert.Equal(NaturezaChamadoEnum.Incidente, x.NaturezaChamado);
            Assert.Equal("Em Atendimento", x.Status);
        });
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
    public async Task AplicaFiltroPorSubcategoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { SubcategoriaId = dados.SubcategoriaInfraId });

        Assert.All(response.Items, x => Assert.Equal("Acesso", x.Subcategoria));
    }

    [Fact]
    public async Task AplicaFiltroPorTipoSolicitacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { TipoSolicitacaoId = dados.TipoSolicitacaoIncidenteId });

        Assert.All(response.Items, x => Assert.Equal("Incidente", x.TipoSolicitacao));
    }

    [Fact]
    public async Task AplicaFiltroPorLocalUnidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { LocalUnidadeId = dados.LocalUnidadeMatrizId });

        Assert.All(response.Items, x => Assert.Equal("Matriz", x.LocalUnidade));
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

    [Fact]
    public async Task ChamadoAbertoNoPortalDeveAparecerNaFilaAdministrativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest());

        Assert.Contains(response.Items, item => item.Codigo == "CH-ADMIN-001");
    }

    private static async Task<(UsuarioContextoAplicacao AtendenteContexto, Guid StatusEmAtendimentoId, Guid PrioridadeAltaId, Guid CategoriaInfraId, Guid SubcategoriaInfraId, Guid TipoSolicitacaoIncidenteId, Guid LocalUnidadeMatrizId)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten@empresa.com", TipoPerfil.Atendente);
        var solicitante1 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante 1", "sol1@empresa.com", TipoPerfil.Solicitante);
        var solicitante2 = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante 2", "sol2@empresa.com", TipoPerfil.Solicitante);

        var categoriaInfra = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var categoriaSistemas = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Sistemas");
        var subcategoriaInfra = new SubcategoriaChamado(categoriaInfra.Id, "Acesso", null, "teste");
        var subcategoriaSistemas = new SubcategoriaChamado(categoriaSistemas.Id, "ERP", null, "teste");
        var tipoIncidente = new TipoSolicitacao("Incidente", null, "teste");
        var tipoMelhoria = new TipoSolicitacao("Melhoria", null, "teste");
        var localMatriz = new LocalUnidade("Matriz", null, null, "teste");
        var localFilial = new LocalUnidade("Filial", null, null, "teste");
        context.SubcategoriasChamado.AddRange(subcategoriaInfra, subcategoriaSistemas);
        context.TiposSolicitacao.AddRange(tipoIncidente, tipoMelhoria);
        context.LocaisUnidade.AddRange(localMatriz, localFilial);
        await context.SaveChangesAsync();

        var prioridadeAlta = context.PrioridadesChamado.First(x => x.Nome == "Alta");
        _ = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante1,
            categoriaInfra,
            StatusChamadoEnum.EmAtendimento,
            prioridadeAlta.Id,
            "001",
            subcategoriaId: subcategoriaInfra.Id,
            tipoSolicitacaoId: tipoIncidente.Id,
            localUnidadeId: localMatriz.Id,
            naturezaChamado: NaturezaChamadoEnum.Incidente);
        _ = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante2,
            categoriaSistemas,
            StatusChamadoEnum.Aberto,
            null,
            "002",
            subcategoriaId: subcategoriaSistemas.Id,
            tipoSolicitacaoId: tipoMelhoria.Id,
            localUnidadeId: localFilial.Id,
            naturezaChamado: NaturezaChamadoEnum.Requisicao);

        var statusEmAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        return (
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            statusEmAtendimento.Id,
            prioridadeAlta.Id,
            categoriaInfra.Id,
            subcategoriaInfra.Id,
            tipoIncidente.Id,
            localMatriz.Id);
    }
}

