using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalBaseConhecimentoUseCasesTests
{
    [Fact]
    public async Task ListarSomenteArtigosPublicados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "publicado", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);
        await CriarArtigoAsync(context, dados.Admin, "rascunho", StatusArtigoConhecimento.Rascunho, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest());

        Assert.Single(response.Items);
        Assert.Equal("publicado", response.Items.Single().Titulo);
    }

    [Fact]
    public async Task NaoListarRascunhos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "rascunho", StatusArtigoConhecimento.Rascunho, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task NaoListarArtigosEmRevisao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "revisao", StatusArtigoConhecimento.EmRevisao, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task NaoListarArtigosArquivados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "arquivado", StatusArtigoConhecimento.Arquivado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task NaoListarArtigosInativos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "inativo", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id, ativo: false);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task BuscarPorTermoNoTitulo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "Acesso VPN", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);
        await CriarArtigoAsync(context, dados.Admin, "Rede local", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest { Termo = "VPN" });

        Assert.Single(response.Items);
        Assert.Equal("Acesso VPN", response.Items.Single().Titulo);
    }

    [Fact]
    public async Task BuscarPorTermoNasTags()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "Manual de impressora", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id, tags: "driver,impressao");
        await CriarArtigoAsync(context, dados.Admin, "Wi-Fi", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id, tags: "rede");

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest { Termo = "driver" });

        Assert.Single(response.Items);
        Assert.Equal("Manual de impressora", response.Items.Single().Titulo);
    }

    [Fact]
    public async Task FiltrarPorCategoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        await CriarArtigoAsync(context, dados.Admin, "Categoria 1", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);
        await CriarArtigoAsync(context, dados.Admin, "Categoria 2", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria2.Id);

        var useCase = CriarListarUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(new PortalFiltroBaseConhecimentoRequest { CategoriaId = dados.Categoria2.Id });

        Assert.Single(response.Items);
        Assert.Equal("Categoria 2", response.Items.Single().Titulo);
    }

    [Fact]
    public async Task ObterArtigoPublicadoPorSlug()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        var artigo = await CriarArtigoAsync(context, dados.Admin, "Conhecimento VPN", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarObterPorSlugUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(artigo.Slug);

        Assert.Equal(artigo.Id, response.Id);
        Assert.Equal("Conhecimento VPN", response.Titulo);
        Assert.Equal("conteudo Conhecimento VPN", response.Conteudo);
    }

    [Fact]
    public async Task Retorna404ParaSlugInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        var useCase = CriarObterPorSlugUseCase(context, dados.Solicitante);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync("nao-existe"));
    }

    [Theory]
    [InlineData(StatusArtigoConhecimento.Rascunho)]
    [InlineData(StatusArtigoConhecimento.EmRevisao)]
    [InlineData(StatusArtigoConhecimento.Arquivado)]
    public async Task Retorna404ParaArtigoNaoPublicado(StatusArtigoConhecimento status)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, $"Nao publicado {status}", status, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarObterPorSlugUseCase(context, dados.Solicitante);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(artigo.Slug));
    }

    [Fact]
    public async Task RespeitarVisibilidadeSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        var artigoSolicitante = await CriarArtigoAsync(context, dados.Admin, "Publico Portal", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Solicitante, dados.Categoria1.Id);

        var useCase = CriarObterPorSlugUseCase(context, dados.Solicitante);
        var response = await useCase.ExecutarAsync(artigoSolicitante.Slug);

        Assert.Equal(artigoSolicitante.Id, response.Id);
    }

    [Fact]
    public async Task RespeitarVisibilidadeAtendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        var artigoAtendente = await CriarArtigoAsync(context, dados.Admin, "Interno Atendimento", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Atendente, dados.Categoria1.Id);

        var useCaseAtendente = CriarObterPorSlugUseCase(context, dados.Atendente);
        var useCaseSolicitante = CriarObterPorSlugUseCase(context, dados.Solicitante);

        var permitido = await useCaseAtendente.ExecutarAsync(artigoAtendente.Slug);
        Assert.Equal(artigoAtendente.Id, permitido.Id);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCaseSolicitante.ExecutarAsync(artigoAtendente.Slug));
    }

    [Fact]
    public async Task RespeitarVisibilidadeAdministrador()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosECategoriasAsync(context);

        var artigoAdmin = await CriarArtigoAsync(context, dados.Admin, "Somente Admin", StatusArtigoConhecimento.Publicado, VisibilidadeArtigoConhecimento.Administrador, dados.Categoria1.Id);

        var useCaseAdmin = CriarObterPorSlugUseCase(context, dados.Admin);
        var useCaseAtendente = CriarObterPorSlugUseCase(context, dados.Atendente);

        var permitido = await useCaseAdmin.ExecutarAsync(artigoAdmin.Slug);
        Assert.Equal(artigoAdmin.Id, permitido.Id);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCaseAtendente.ExecutarAsync(artigoAdmin.Slug));
    }

    private static ListarArtigosPortalBaseConhecimentoUseCase CriarListarUseCase(SGXSistemaChamadoDbContext context, Usuario usuario)
        => new(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(Contexto(usuario)));

    private static ObterArtigoPortalBaseConhecimentoPorSlugUseCase CriarObterPorSlugUseCase(SGXSistemaChamadoDbContext context, Usuario usuario)
        => new(
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            new FakeUsuarioContextoAplicacaoService(Contexto(usuario)));

    private static UsuarioContextoAplicacao Contexto(Usuario usuario)
    {
        var perfis = usuario.UsuarioPerfis
            .Select(x => x.PerfilAcesso.TipoPerfil.ToString())
            .ToArray();

        return new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, perfis);
    }

    private static async Task<BaseConhecimentoArtigo> CriarArtigoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario criador,
        string titulo,
        StatusArtigoConhecimento status,
        VisibilidadeArtigoConhecimento visibilidade,
        Guid? categoriaId,
        string? tags = null,
        bool ativo = true)
    {
        var slug = $"{titulo.Trim().ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}";

        var artigo = new BaseConhecimentoArtigo(
            titulo,
            slug,
            $"resumo {titulo}",
            $"conteudo {titulo}",
            categoriaId,
            status,
            visibilidade,
            tags,
            criador.Id,
            criador.Login);

        if (!ativo)
        {
            artigo.Desativar(criador.Login);
        }

        context.BaseConhecimentoArtigos.Add(artigo);
        await context.SaveChangesAsync();
        return artigo;
    }

    private static async Task<(Usuario Admin, Usuario Atendente, Usuario Solicitante, CategoriaChamado Categoria1, CategoriaChamado Categoria2)> SeedUsuariosECategoriasAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"admin.portal.bc.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", $"aten.portal.bc.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"sol.portal.bc.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Solicitante);

        var departamento = new Departamento("Operacoes", "OPE", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria1 = new CategoriaChamado("Categoria One", null, departamento.Id, "teste");
        var categoria2 = new CategoriaChamado("Categoria Two", null, departamento.Id, "teste");

        context.CategoriasChamado.AddRange(categoria1, categoria2);
        await context.SaveChangesAsync();

        var adminCompleto = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == admin.Id);

        var atendenteCompleto = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == atendente.Id);

        var solicitanteCompleto = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstAsync(x => x.Id == solicitante.Id);

        return (adminCompleto, atendenteCompleto, solicitanteCompleto, categoria1, categoria2);
    }
}
