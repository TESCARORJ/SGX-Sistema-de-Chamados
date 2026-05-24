using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicosAdminUseCasesTests
{
    [Fact]
    public async Task CriarServicoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Acesso VPN",
            Descricao = "Solicitacao de acesso remoto seguro.",
            InstrucoesSolicitante = "Informe seu equipamento.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id,
            SubcategoriaId = seed.SubcategoriaTi.Id,
            PrioridadePadraoId = seed.PrioridadeMedia.Id,
            SlaPadraoId = seed.PoliticaSla.Id,
            ArtigoBaseConhecimentoId = seed.ArtigoBaseConhecimento.Id,
            Visibilidade = VisibilidadeCatalogoServico.Solicitante,
            RequerAprovacao = true,
            Ordem = 10
        });

        Assert.Equal("Acesso VPN", response.Nome);
        Assert.Equal(StatusCatalogoServico.Rascunho, response.Status);
        Assert.True(response.Ativo);
        Assert.True(response.PermiteAberturaChamado);
        Assert.Equal(seed.Admin.Id, response.CriadoPorUsuarioId);
    }

    [Fact]
    public async Task ImpedeCriacaoSemNome()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = " ",
            Descricao = "Descricao valida.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id
        }));
    }

    [Fact]
    public async Task ImpedeCriacaoSemDescricao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico sem descricao",
            Descricao = " ",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id
        }));
    }

    [Fact]
    public async Task ImpedeCriacaoSemDepartamentoResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico invalido",
            Descricao = "Descricao valida.",
            DepartamentoResponsavelId = Guid.Empty
        }));
    }

    [Fact]
    public async Task ImpedeCriacaoComDepartamentoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico com departamento inexistente",
            Descricao = "Descricao valida.",
            DepartamentoResponsavelId = Guid.NewGuid()
        }));
    }

    [Fact]
    public async Task GeraSlugAutomaticamente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitacao de cracha provisoria",
            Descricao = "Servico de exemplo.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        Assert.Equal("solicitacao-de-cracha-provisoria", response.Slug);
    }

    [Fact]
    public async Task GaranteSlugUnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var primeiro = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Reserva de sala",
            Descricao = "Reserva interna.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        var segundo = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Reserva de sala",
            Descricao = "Reserva institucional.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        Assert.Equal("reserva-de-sala", primeiro.Slug);
        Assert.Equal("reserva-de-sala-2", segundo.Slug);
    }

    [Fact]
    public async Task AceitaPoliticaSlaIdComoAliasDeSlaPadraoId()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico com politica SLA",
            Descricao = "Descricao valida",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            PoliticaSlaId = seed.PoliticaSla.Id
        });

        Assert.Equal(seed.PoliticaSla.Id, response.SlaPadraoId);
    }

    [Fact]
    public async Task ImpedeRequestComSlaPadraoIdEDiferenteDePoliticaSlaId()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico inconsistente SLA",
            Descricao = "Descricao valida",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            SlaPadraoId = Guid.NewGuid(),
            PoliticaSlaId = seed.PoliticaSla.Id
        }));
    }

    [Fact]
    public async Task EditarServico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Onboarding",
            Descricao = "Primeira versao.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id,
            CategoriaId = seed.CategoriaRh.Id
        });

        var atualizado = await useCase.AtualizarAsync(criado.Id, new AtualizarCatalogoServicoRequest
        {
            Nome = "Onboarding corporativo",
            Descricao = "Versao atualizada.",
            InstrucoesSolicitante = "Anexe documentos.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id,
            CategoriaId = seed.CategoriaRh.Id,
            PermiteAberturaChamado = true,
            RequerAprovacao = true,
            Ordem = 3,
            Visibilidade = VisibilidadeCatalogoServico.Atendente,
            Ativo = true
        });

        Assert.Equal("Onboarding corporativo", atualizado.Nome);
        Assert.Equal("onboarding-corporativo", atualizado.Slug);
        Assert.Equal(StatusCatalogoServico.Rascunho, atualizado.Status);
        Assert.True(atualizado.RequerAprovacao);
    }

    [Fact]
    public async Task ImpedeEdicaoDeServicoArquivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico legado",
            Descricao = "Descricao inicial.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });

        await useCase.ArquivarAsync(criado.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.AtualizarAsync(criado.Id, new AtualizarCatalogoServicoRequest
        {
            Nome = "Servico legado v2",
            Descricao = "Descricao revisada.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id,
            PermiteAberturaChamado = true,
            RequerAprovacao = false,
            Ordem = 1,
            Visibilidade = VisibilidadeCatalogoServico.Atendente,
            Ativo = true
        }));
    }

    [Fact]
    public async Task PublicarServicoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitar notebook",
            Descricao = "Fluxo de requisicao de notebook.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });

        var publicado = await useCase.PublicarAsync(criado.Id);

        Assert.Equal(StatusCatalogoServico.Publicado, publicado.Status);
        Assert.True(publicado.Ativo);
        Assert.NotNull(publicado.PublicadoEm);
        Assert.Equal(seed.Admin.Id, publicado.PublicadoPorUsuarioId);
    }

    [Fact]
    public async Task ImpedePublicacaoDeServicoArquivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitar desktop",
            Descricao = "Fluxo de requisicao de desktop.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });

        await useCase.ArquivarAsync(criado.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.PublicarAsync(criado.Id));
    }

    [Fact]
    public async Task ArquivarServico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitar periferico",
            Descricao = "Servico para perifericos.",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });

        var response = await useCase.ArquivarAsync(criado.Id);
        var salvo = await context.CatalogosServico.FirstAsync(x => x.Id == criado.Id);

        Assert.False(response.Ativo);
        Assert.Equal(StatusCatalogoServico.Arquivado, salvo.Status);
        Assert.False(salvo.Ativo);
        Assert.NotNull(salvo.ArquivadoEm);
    }

    [Fact]
    public async Task ReativarServico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitar reembolso",
            Descricao = "Fluxo de reembolso.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id,
            CategoriaId = seed.CategoriaRh.Id
        });

        await useCase.ArquivarAsync(criado.Id);
        var response = await useCase.ReativarAsync(criado.Id);

        var salvo = await context.CatalogosServico.FirstAsync(x => x.Id == criado.Id);
        Assert.True(response.Ativo);
        Assert.True(salvo.Ativo);
        Assert.Equal(StatusCatalogoServico.Rascunho, salvo.Status);
        Assert.NotNull(salvo.ArquivadoEm);
    }

    [Fact]
    public async Task FiltrarPorDepartamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico TI",
            Descricao = "Descricao TI",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id
        });

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico RH",
            Descricao = "Descricao RH",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        var response = await useCase.ListarAsync(new FiltroCatalogoServicoRequest
        {
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("Servico RH", response.Items.Single().Nome);
    }

    [Fact]
    public async Task FiltrarPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var publicado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico publicado",
            Descricao = "Descricao publica",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });
        await useCase.PublicarAsync(publicado.Id);

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico rascunho",
            Descricao = "Descricao rascunho",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        var filtrado = await useCase.ListarAsync(new FiltroCatalogoServicoRequest
        {
            Status = StatusCatalogoServico.Publicado
        });

        Assert.Single(filtrado.Items);
        Assert.Equal("Servico publicado", filtrado.Items.Single().Nome);
    }

    [Fact]
    public async Task FiltrarPorVisibilidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico solicitante",
            Descricao = "Descricao solicitante",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            Visibilidade = VisibilidadeCatalogoServico.Solicitante
        });

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico interno",
            Descricao = "Descricao interno",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            Visibilidade = VisibilidadeCatalogoServico.Interno
        });

        var filtrado = await useCase.ListarAsync(new FiltroCatalogoServicoRequest
        {
            Visibilidade = VisibilidadeCatalogoServico.Solicitante
        });

        Assert.Single(filtrado.Items);
        Assert.Equal("Servico solicitante", filtrado.Items.Single().Nome);
    }

    [Fact]
    public async Task BuscarPorTermoEmNome()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Solicitar assinatura digital",
            Descricao = "Descricao geral.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Reservar estacionamento",
            Descricao = "Descricao geral.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        var response = await useCase.ListarAsync(new FiltroCatalogoServicoRequest
        {
            Termo = "assinatura"
        });

        Assert.Single(response.Items);
        Assert.Equal("Solicitar assinatura digital", response.Items.Single().Nome);
    }

    [Fact]
    public async Task BuscarPorTermoEmDescricao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Beneficio alimentacao",
            Descricao = "Fluxo para concessao de vale alimentacao.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Cadastro visitante",
            Descricao = "Fluxo de cadastro externo.",
            DepartamentoResponsavelId = seed.DepartamentoRh.Id
        });

        var response = await useCase.ListarAsync(new FiltroCatalogoServicoRequest
        {
            Termo = "vale alimentacao"
        });

        Assert.Single(response.Items);
        Assert.Equal("Beneficio alimentacao", response.Items.Single().Nome);
    }

    [Fact]
    public async Task RegistraAuditoriaNasOperacoesPrincipais()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarUseCase(context, seed.Admin, auditoria);

        var criado = await useCase.CriarAsync(new CriarCatalogoServicoRequest
        {
            Nome = "Servico auditado",
            Descricao = "Descricao auditada",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id
        });

        await useCase.AtualizarAsync(criado.Id, new AtualizarCatalogoServicoRequest
        {
            Nome = "Servico auditado v2",
            Descricao = "Descricao auditada v2",
            DepartamentoResponsavelId = seed.DepartamentoTi.Id,
            CategoriaId = seed.CategoriaTi.Id,
            PermiteAberturaChamado = true,
            RequerAprovacao = false,
            Ordem = 1,
            Visibilidade = VisibilidadeCatalogoServico.Atendente,
            Ativo = true
        });

        await useCase.PublicarAsync(criado.Id);
        await useCase.ArquivarAsync(criado.Id);
        await useCase.ReativarAsync(criado.Id);

        Assert.True(auditoria.Eventos.Count >= 5);
    }

    private static CatalogoServicosAdminUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        Usuario admin,
        FakeAuditoriaService? auditoria = null)
        => new(
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<PoliticaSla>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

    private static async Task<SeedCatalogoServicoContexto> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Catalogo",
            $"admin.catalogo.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);

        var departamentoTi = new Departamento("Tecnologia", "TI", null, "teste");
        var departamentoRh = new Departamento("Recursos Humanos", "RH", null, "teste");
        context.Departamentos.AddRange(departamentoTi, departamentoRh);
        await context.SaveChangesAsync();

        var categoriaTi = new CategoriaChamado("Suporte TI", null, departamentoTi.Id, "teste");
        var categoriaRh = new CategoriaChamado("Atendimento RH", null, departamentoRh.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaTi, categoriaRh);
        await context.SaveChangesAsync();

        var subcategoriaTi = new SubcategoriaChamado(categoriaTi.Id, "Hardware", null, "teste");
        context.SubcategoriasChamado.Add(subcategoriaTi);
        await context.SaveChangesAsync();

        var prioridadeMedia = new PrioridadeChamado("Media", PrioridadeChamadoEnum.Media, null, 4, 24, "teste");
        context.PrioridadesChamado.Add(prioridadeMedia);
        await context.SaveChangesAsync();

        var politicaSla = new PoliticaSla(
            "SLA Corporativo",
            null,
            1,
            categoriaTi.Id,
            departamentoTi.Id,
            null,
            false,
            true,
            "teste");
        context.SlaPoliticas.Add(politicaSla);
        await context.SaveChangesAsync();

        var artigo = new BaseConhecimentoArtigo(
            "Acesso remoto",
            "acesso-remoto",
            null,
            "Conteudo de referencia.",
            categoriaTi.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Atendente,
            null,
            admin.Id,
            "teste");
        context.BaseConhecimentoArtigos.Add(artigo);
        await context.SaveChangesAsync();

        return new SeedCatalogoServicoContexto(
            admin,
            departamentoTi,
            departamentoRh,
            categoriaTi,
            categoriaRh,
            subcategoriaTi,
            prioridadeMedia,
            politicaSla,
            artigo);
    }

    private sealed record SeedCatalogoServicoContexto(
        Usuario Admin,
        Departamento DepartamentoTi,
        Departamento DepartamentoRh,
        CategoriaChamado CategoriaTi,
        CategoriaChamado CategoriaRh,
        SubcategoriaChamado SubcategoriaTi,
        PrioridadeChamado PrioridadeMedia,
        PoliticaSla PoliticaSla,
        BaseConhecimentoArtigo ArtigoBaseConhecimento);
}
