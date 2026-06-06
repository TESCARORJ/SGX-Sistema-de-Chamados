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

    [Fact]
    public async Task ListagemAdminAceitaChamadosComGrupoEFilaNulos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest());
        var chamadoLegado = context.Chamados.Single(x => x.Codigo == "CH-ADMIN-001");

        Assert.Null(chamadoLegado.GrupoTecnicoId);
        Assert.Null(chamadoLegado.FilaAtendimentoId);
        Assert.Contains(response.Items, item => item.Codigo == "CH-ADMIN-001");
    }

    [Fact]
    public async Task ListagemAdminRetornaGrupoEFilaQuandoPreenchidos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest());
        var item = Assert.Single(response.Items, x => x.Codigo == "CH-ADMIN-002");

        Assert.Equal(dados.GrupoTecnicoId, item.GrupoTecnicoId);
        Assert.Equal("Grupo Sistemas", item.GrupoTecnicoNome);
        Assert.Equal(dados.FilaAtendimentoId, item.FilaAtendimentoId);
        Assert.Equal("Fila Sistemas", item.FilaAtendimentoNome);
    }

    [Fact]
    public async Task AplicaFiltroPorGrupoTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { GrupoTecnicoId = dados.GrupoTecnicoId });

        Assert.Single(response.Items);
        Assert.All(response.Items, x => Assert.Equal(dados.GrupoTecnicoId, x.GrupoTecnicoId));
        Assert.Contains(response.Items, x => x.Codigo == "CH-ADMIN-002");
    }

    [Fact]
    public async Task AplicaFiltroPorFilaAtendimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { FilaAtendimentoId = dados.FilaAtendimentoId });

        Assert.Single(response.Items);
        Assert.All(response.Items, x => Assert.Equal(dados.FilaAtendimentoId, x.FilaAtendimentoId));
        Assert.Contains(response.Items, x => x.Codigo == "CH-ADMIN-002");
    }

    [Fact]
    public async Task AplicaFiltroCombinadoPorGrupoTecnicoEFilaAtendimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest
        {
            GrupoTecnicoId = dados.GrupoTecnicoId,
            FilaAtendimentoId = dados.FilaAtendimentoId
        });

        var item = Assert.Single(response.Items);
        Assert.Equal("CH-ADMIN-002", item.Codigo);
        Assert.Equal(dados.GrupoTecnicoId, item.GrupoTecnicoId);
        Assert.Equal(dados.FilaAtendimentoId, item.FilaAtendimentoId);
        Assert.Equal(1, response.Total);
    }

    [Fact]
    public async Task FiltroPorResponsavelContinuaFuncionando()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        var response = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { ResponsavelId = dados.AtendenteId });

        Assert.Single(response.Items);
        Assert.Contains(response.Items, x => x.Codigo == "CH-ADMIN-002");
    }

    [Fact]
    public async Task ListagemFiltrosNaoAlteramResponsavelDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var chamadoAntes = context.Chamados.Single(x => x.Codigo == "CH-ADMIN-002");
        Assert.Equal(dados.AtendenteId, chamadoAntes.ResponsavelId);

        var useCase = new ListarChamadosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto));

        _ = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest());
        _ = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { GrupoTecnicoId = dados.GrupoTecnicoId });
        _ = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { FilaAtendimentoId = dados.FilaAtendimentoId });
        _ = await useCase.ExecutarAsync(new FiltroChamadosAdminRequest { ResponsavelId = dados.AtendenteId });

        var chamadoDepois = context.Chamados.Single(x => x.Codigo == "CH-ADMIN-002");
        Assert.Equal(dados.AtendenteId, chamadoDepois.ResponsavelId);
    }

    private static async Task<(UsuarioContextoAplicacao AtendenteContexto, Guid AtendenteId, Guid StatusEmAtendimentoId, Guid PrioridadeAltaId, Guid CategoriaInfraId, Guid SubcategoriaInfraId, Guid TipoSolicitacaoIncidenteId, Guid LocalUnidadeMatrizId, Guid GrupoTecnicoId, Guid FilaAtendimentoId)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
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
        var chamadoComGrupoFila = await AdminUseCasesTestFactory.CriarChamadoAsync(
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

        var grupo = new GrupoTecnico("Grupo Sistemas", "Grupo para testes de listagem", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var fila = new FilaAtendimento(grupo.Id, "Fila Sistemas", "Fila para testes de listagem", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();

        chamadoComGrupoFila.DefinirGrupoTecnico(grupo.Id, "teste");
        chamadoComGrupoFila.DefinirFilaAtendimento(fila.Id, "teste");
        chamadoComGrupoFila.AtribuirResponsavel(atendente.Id, "teste");
        await context.SaveChangesAsync();

        var statusEmAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);

        return (
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            atendente.Id,
            statusEmAtendimento.Id,
            prioridadeAlta.Id,
            categoriaInfra.Id,
            subcategoriaInfra.Id,
            tipoIncidente.Id,
            localMatriz.Id,
            grupo.Id,
            fila.Id);
    }
}

