using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class BaseConhecimentoAdminUseCasesTests
{
    [Fact]
    public async Task CriacaoDeArtigoComDadosValidos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);

        var useCase = CriarUseCaseCriacao(context, admin);

        var response = await useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "Reset de senha",
            Conteudo = "Passo a passo para reset.",
            Resumo = "Resumo curto",
            Visibilidade = VisibilidadeArtigoConhecimento.Solicitante,
            CategoriaId = categoria.Id,
            Tags = "senha,acesso"
        });

        Assert.Equal("Reset de senha", response.Titulo);
        Assert.Equal(StatusArtigoConhecimento.Rascunho, response.Status);
        Assert.True(response.Ativo);
        Assert.Equal(admin.Id, response.CriadoPorUsuarioId);
    }

    [Fact]
    public async Task ImpedeCriacaoSemTitulo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _) = await SeedAsync(context);

        var useCase = CriarUseCaseCriacao(context, admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = " ",
            Conteudo = "Conteudo valido",
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        }));
    }

    [Fact]
    public async Task ImpedeCriacaoSemConteudo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _) = await SeedAsync(context);

        var useCase = CriarUseCaseCriacao(context, admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "Titulo valido",
            Conteudo = " ",
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        }));
    }

    [Fact]
    public async Task GeraSlugAutomaticamente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _) = await SeedAsync(context);

        var useCase = CriarUseCaseCriacao(context, admin);

        var response = await useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "Como abrir chamado urgente",
            Conteudo = "conteudo",
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        });

        Assert.Equal("como-abrir-chamado-urgente", response.Slug);
    }

    [Fact]
    public async Task GaranteSlugUnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, _) = await SeedAsync(context);

        var useCase = CriarUseCaseCriacao(context, admin);

        var primeiro = await useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "FAQ VPN",
            Conteudo = "conteudo 1",
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        });

        var segundo = await useCase.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "FAQ VPN",
            Conteudo = "conteudo 2",
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        });

        Assert.Equal("faq-vpn", primeiro.Slug);
        Assert.Equal("faq-vpn-2", segundo.Slug);
    }

    [Fact]
    public async Task PermiteEdicaoDeArtigoEmRascunho()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Manual inicial", "conteudo inicial");

        var useCase = new AtualizarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(artigo.Id, new AtualizarBaseConhecimentoArtigoRequest
        {
            Titulo = "Manual revisado",
            Conteudo = "conteudo revisado",
            Resumo = "novo resumo",
            Visibilidade = VisibilidadeArtigoConhecimento.Administrador,
            CategoriaId = categoria.Id,
            Tags = "manual,rev"
        });

        Assert.Equal("Manual revisado", response.Titulo);
        Assert.Equal(StatusArtigoConhecimento.Rascunho, response.Status);
    }

    [Fact]
    public async Task PublicaArtigoComSucesso()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Publicar artigo", "conteudo pronto");

        var useCase = new PublicarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(artigo.Id);

        Assert.Equal(StatusArtigoConhecimento.Publicado, response.Status);
        Assert.True(response.Ativo);
        Assert.NotNull(response.PublicadoEm);
        Assert.Equal(admin.Id, response.PublicadoPorUsuarioId);
    }

    [Fact]
    public async Task ImpedePublicacaoSemConteudo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Sem conteudo", "conteudo temporario");
        context.Entry(artigo).Property(nameof(BaseConhecimentoArtigo.Conteudo)).CurrentValue = " ";
        await context.SaveChangesAsync();

        var useCase = new PublicarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(artigo.Id));
    }

    [Fact]
    public async Task ArquivaArtigoSemExclusaoFisica()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Artigo legado", "conteudo legado");

        var useCase = new ArquivarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(artigo.Id);

        var salvo = await context.BaseConhecimentoArtigos.FirstAsync(x => x.Id == artigo.Id);
        Assert.False(response.Ativo);
        Assert.False(salvo.Ativo);
        Assert.Equal(StatusArtigoConhecimento.Arquivado, salvo.Status);
        Assert.NotNull(salvo.ArquivadoEm);
    }

    [Fact]
    public async Task ReativaApenasArtigoArquivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Artigo para reativar", "conteudo");
        artigo.Arquivar(admin.Id, admin.Login);
        await context.SaveChangesAsync();

        var useCase = new ReativarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(artigo.Id);

        var salvo = await context.BaseConhecimentoArtigos.FirstAsync(x => x.Id == artigo.Id);
        Assert.True(response.Ativo);
        Assert.True(salvo.Ativo);
        Assert.Equal(StatusArtigoConhecimento.Rascunho, salvo.Status);
        Assert.NotNull(salvo.ArquivadoEm);
    }

    [Fact]
    public async Task FiltraPorStatusEVisibilidade()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigoPublicado = await CriarArtigoAsync(context, admin, categoria.Id, "Publicado", "conteudo");
        artigoPublicado.Publicar(admin.Id, admin.Login);

        var artigoRascunho = await CriarArtigoAsync(context, admin, categoria.Id, "Rascunho", "conteudo");
        artigoRascunho.AtualizarDados(
            artigoRascunho.Titulo,
            artigoRascunho.Slug,
            artigoRascunho.Resumo,
            artigoRascunho.Conteudo,
            artigoRascunho.CategoriaId,
            VisibilidadeArtigoConhecimento.Solicitante,
            artigoRascunho.Tags,
            admin.Id,
            admin.Login);

        await context.SaveChangesAsync();

        var useCase = new ListarArtigosBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var filtradoStatus = await useCase.ExecutarAsync(new FiltroBaseConhecimentoArtigoRequest
        {
            Status = StatusArtigoConhecimento.Publicado
        });

        var filtradoVisibilidade = await useCase.ExecutarAsync(new FiltroBaseConhecimentoArtigoRequest
        {
            Visibilidade = VisibilidadeArtigoConhecimento.Solicitante
        });

        Assert.Single(filtradoStatus.Items);
        Assert.Equal(artigoPublicado.Id, filtradoStatus.Items.Single().Id);
        Assert.Single(filtradoVisibilidade.Items);
        Assert.Equal(artigoRascunho.Id, filtradoVisibilidade.Items.Single().Id);
    }

    [Fact]
    public async Task BuscaPorTermoEmTituloETags()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        await CriarArtigoAsync(context, admin, categoria.Id, "VPN Corporativa", "conteudo", "acesso,seguranca");
        await CriarArtigoAsync(context, admin, categoria.Id, "Manual Impressora", "conteudo", "periferico,driver");

        var useCase = new ListarArtigosBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var porTitulo = await useCase.ExecutarAsync(new FiltroBaseConhecimentoArtigoRequest
        {
            Termo = "VPN"
        });

        var porTag = await useCase.ExecutarAsync(new FiltroBaseConhecimentoArtigoRequest
        {
            Termo = "driver"
        });

        Assert.Single(porTitulo.Items);
        Assert.Equal("VPN Corporativa", porTitulo.Items.Single().Titulo);
        Assert.Single(porTag.Items);
        Assert.Equal("Manual Impressora", porTag.Items.Single().Titulo);
    }

    [Fact]
    public async Task PreservaArtigoSemExclusaoFisicaAposArquivamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, admin, categoria.Id, "Historico", "conteudo");

        var useCase = new ArquivarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(artigo.Id);

        var count = await context.BaseConhecimentoArtigos.CountAsync(x => x.Id == artigo.Id);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task RegistraAuditoriaNasOperacoesPrincipais()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (admin, categoria) = await SeedAsync(context);
        var auditoria = new FakeAuditoriaService();

        var criar = new CriarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        var criado = await criar.ExecutarAsync(new CriarBaseConhecimentoArtigoRequest
        {
            Titulo = "Artigo auditado",
            Conteudo = "conteudo",
            CategoriaId = categoria.Id,
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        });

        var atualizar = new AtualizarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);
        await atualizar.ExecutarAsync(criado.Id, new AtualizarBaseConhecimentoArtigoRequest
        {
            Titulo = "Artigo auditado v2",
            Conteudo = "conteudo v2",
            CategoriaId = categoria.Id,
            Visibilidade = VisibilidadeArtigoConhecimento.Atendente
        });

        var publicar = new PublicarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);
        await publicar.ExecutarAsync(criado.Id);

        var arquivar = new ArquivarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);
        await arquivar.ExecutarAsync(criado.Id);

        var reativar = new ReativarArtigoBaseConhecimentoUseCase(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);
        await reativar.ExecutarAsync(criado.Id);

        Assert.True(auditoria.Eventos.Count >= 5);
    }

    private static CriarArtigoBaseConhecimentoUseCase CriarUseCaseCriacao(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<BaseConhecimentoArtigo> CriarArtigoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario admin,
        Guid? categoriaId,
        string titulo,
        string conteudo,
        string? tags = null)
    {
        var artigo = new BaseConhecimentoArtigo(
            titulo,
            titulo.ToLowerInvariant().Replace(' ', '-'),
            null,
            conteudo,
            categoriaId,
            StatusArtigoConhecimento.Rascunho,
            VisibilidadeArtigoConhecimento.Atendente,
            tags,
            admin.Id,
            admin.Login);

        context.BaseConhecimentoArtigos.Add(artigo);
        await context.SaveChangesAsync();
        return artigo;
    }

    private static async Task<(Usuario admin, CategoriaChamado categoria)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin BC", $"admin.bc.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Administrador);

        var departamento = new Departamento("Tecnologia", "TEC", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria = new CategoriaChamado("Conhecimento", "Categoria base", departamento.Id, "teste");
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        return (admin, categoria);
    }
}