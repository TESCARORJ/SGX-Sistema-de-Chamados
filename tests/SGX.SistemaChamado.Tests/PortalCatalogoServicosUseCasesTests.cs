using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalCatalogoServicosUseCasesTests
{
    [Fact]
    public async Task ListarApenasServicosPublicadosEAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Publicado", "Servico publicado", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Rascunho", "Servico rascunho", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Rascunho, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Arquivado", "Servico arquivado", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Arquivado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Inativo", "Servico inativo", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante, ativo: false);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Single(response.Items);
        Assert.Equal("Publicado", response.Items.Single().Nome);
    }

    [Fact]
    public async Task NaoListarRascunho()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Rascunho", "Servico em rascunho", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Rascunho, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task NaoListarArquivado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Arquivado", "Servico arquivado", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Arquivado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task NaoListarServicoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Inativo", "Servico inativo", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante, ativo: false);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Empty(response.Items);
    }

    [Fact]
    public async Task FiltrarPorDepartamentoResponsavelId()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "TI", "Servico TI", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "RH", "Servico RH", dados.DepartamentoRh.Id, dados.CategoriaRh.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest
        {
            DepartamentoResponsavelId = dados.DepartamentoRh.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("RH", response.Items.Single().Nome);
    }

    [Fact]
    public async Task FiltrarPorCategoriaId()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Categoria TI", "Servico TI", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Categoria RH", "Servico RH", dados.DepartamentoRh.Id, dados.CategoriaRh.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest
        {
            CategoriaId = dados.CategoriaRh.Id
        });

        Assert.Single(response.Items);
        Assert.Equal("Categoria RH", response.Items.Single().Nome);
    }

    [Fact]
    public async Task BuscarPorTermoEmNome()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Solicitar Notebook", "Servico para notebook", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Solicitar Cadeira", "Servico para cadeira", dados.DepartamentoRh.Id, dados.CategoriaRh.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest
        {
            Termo = "Notebook"
        });

        Assert.Single(response.Items);
        Assert.Equal("Solicitar Notebook", response.Items.Single().Nome);
    }

    [Fact]
    public async Task BuscarPorTermoEmDescricao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);

        await CriarServicoAsync(context, dados.Admin, "Acesso remoto", "Fluxo para concessao de VPN", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);
        await CriarServicoAsync(context, dados.Admin, "Folha de ponto", "Fluxo para ajuste de ponto", dados.DepartamentoRh.Id, dados.CategoriaRh.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.ListarAsync(new PortalFiltroCatalogoServicoRequest
        {
            Termo = "VPN"
        });

        Assert.Single(response.Items);
        Assert.Equal("Acesso remoto", response.Items.Single().Nome);
    }

    [Fact]
    public async Task RespeitarVisibilidadeSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Portal Publico", "Servico para todos", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCaseSolicitante = CriarUseCase(context, dados.Solicitante);
        var useCaseAtendente = CriarUseCase(context, dados.Atendente);

        var listaSolicitante = await useCaseSolicitante.ListarAsync(new PortalFiltroCatalogoServicoRequest());
        var listaAtendente = await useCaseAtendente.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Contains(listaSolicitante.Items, x => x.Id == servico.Id);
        Assert.Contains(listaAtendente.Items, x => x.Id == servico.Id);
    }

    [Fact]
    public async Task RespeitarVisibilidadeAtendente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Atendimento Interno", "Somente atendimento", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Atendente);

        var useCaseSolicitante = CriarUseCase(context, dados.Solicitante);
        var useCaseAtendente = CriarUseCase(context, dados.Atendente);

        var listaSolicitante = await useCaseSolicitante.ListarAsync(new PortalFiltroCatalogoServicoRequest());
        var listaAtendente = await useCaseAtendente.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.DoesNotContain(listaSolicitante.Items, x => x.Id == servico.Id);
        Assert.Contains(listaAtendente.Items, x => x.Id == servico.Id);
    }

    [Fact]
    public async Task RespeitarVisibilidadeAdministrador()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Somente Admin", "Uso administrativo", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Administrador);

        var useCaseAdmin = CriarUseCase(context, dados.Admin);
        var useCaseAtendente = CriarUseCase(context, dados.Atendente);

        var listaAdmin = await useCaseAdmin.ListarAsync(new PortalFiltroCatalogoServicoRequest());
        var listaAtendente = await useCaseAtendente.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Contains(listaAdmin.Items, x => x.Id == servico.Id);
        Assert.DoesNotContain(listaAtendente.Items, x => x.Id == servico.Id);
    }

    [Fact]
    public async Task RespeitarVisibilidadeInterno()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Interno", "Uso interno", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Interno);

        var useCaseAtendente = CriarUseCase(context, dados.Atendente);
        var useCaseSolicitante = CriarUseCase(context, dados.Solicitante);

        var listaAtendente = await useCaseAtendente.ListarAsync(new PortalFiltroCatalogoServicoRequest());
        var listaSolicitante = await useCaseSolicitante.ListarAsync(new PortalFiltroCatalogoServicoRequest());

        Assert.Contains(listaAtendente.Items, x => x.Id == servico.Id);
        Assert.DoesNotContain(listaSolicitante.Items, x => x.Id == servico.Id);
    }

    [Fact]
    public async Task ObterDetalhePorSlugValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Detalhe Valido", "Descricao detalhada", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var detalhe = await useCase.ObterPorSlugAsync(servico.Slug);

        Assert.Equal(servico.Id, detalhe.Id);
        Assert.Equal("Detalhe Valido", detalhe.Nome);
    }

    [Fact]
    public async Task Retornar404ParaSlugInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var useCase = CriarUseCase(context, dados.Solicitante);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ObterPorSlugAsync("slug-inexistente"));
    }

    [Fact]
    public async Task Retornar404ParaServicoNaoPublicado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Nao Publicado", "Descricao", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Rascunho, VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ObterPorSlugAsync(servico.Slug));
    }

    [Fact]
    public async Task Retornar404ParaServicoSemVisibilidade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(context, dados.Admin, "Atendente Only", "Descricao", dados.DepartamentoTi.Id, dados.CategoriaTi.Id, StatusCatalogoServico.Publicado, VisibilidadeCatalogoServico.Atendente);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ObterPorSlugAsync(servico.Slug));
    }

    [Fact]
    public async Task PrepararAberturaComServicoValido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Solicitar acesso VPN",
            "Abertura guiada para VPN",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            prioridadePadraoId: prioridade.Id);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.PrepararAberturaChamadoAsync(servico.Slug);

        Assert.Equal(servico.Id, response.CatalogoServicoId);
        Assert.Equal("Solicitar acesso VPN", response.Nome);
        Assert.True(response.PermiteAberturaChamado);
    }

    [Fact]
    public async Task PrepararAberturaComServicoInexistenteRetorna404()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var useCase = CriarUseCase(context, dados.Solicitante);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.PrepararAberturaChamadoAsync("slug-inexistente"));
    }

    [Fact]
    public async Task PrepararAberturaComServicoArquivadoBloqueia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Servico arquivado para abertura",
            "Descricao",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Arquivado,
            VisibilidadeCatalogoServico.Solicitante);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.PrepararAberturaChamadoAsync(servico.Slug));
    }

    [Fact]
    public async Task PrepararAberturaComServicoInativoBloqueia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Servico inativo para abertura",
            "Descricao",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            ativo: false);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.PrepararAberturaChamadoAsync(servico.Slug));
    }

    [Fact]
    public async Task PrepararAberturaComServicoSemVisibilidadeBloqueia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Servico restrito",
            "Descricao",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Atendente);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.PrepararAberturaChamadoAsync(servico.Slug));
    }

    [Fact]
    public async Task PrepararAberturaComPermiteAberturaChamadoFalseBloqueia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Servico somente consulta",
            "Descricao",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            permiteAberturaChamado: false,
            prioridadePadraoId: prioridade.Id);

        var useCase = CriarUseCase(context, dados.Solicitante);
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.PrepararAberturaChamadoAsync(servico.Slug));
    }

    private static CatalogoServicosPortalUseCases CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuario)
        => new(
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            new FakeUsuarioContextoAplicacaoService(Contexto(usuario)));

    private static UsuarioContextoAplicacao Contexto(Usuario usuario)
    {
        var perfis = usuario.UsuarioPerfis
            .Select(x => x.PerfilAcesso.TipoPerfil.ToString())
            .ToArray();

        return new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, perfis);
    }

    private static async Task<CatalogoServico> CriarServicoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario criador,
        string nome,
        string descricao,
        Guid departamentoResponsavelId,
        Guid? categoriaId,
        StatusCatalogoServico status,
        VisibilidadeCatalogoServico visibilidade,
        bool ativo = true,
        bool permiteAberturaChamado = true,
        Guid? subcategoriaId = null,
        Guid? prioridadePadraoId = null,
        Guid? slaPadraoId = null)
    {
        var slug = $"{nome.Trim().ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}";

        var servico = new CatalogoServico(
            nome,
            slug,
            descricao,
            $"instrucoes {nome}",
            departamentoResponsavelId,
            categoriaId,
            subcategoriaId,
            prioridadePadraoId,
            slaPadraoId,
            null,
            visibilidade,
            permiteAberturaChamado,
            false,
            1,
            criador.Id,
            criador.Login);

        if (status == StatusCatalogoServico.Publicado)
        {
            servico.Publicar(criador.Id, criador.Login);
        }
        else if (status == StatusCatalogoServico.Arquivado)
        {
            servico.Arquivar(criador.Id, criador.Login);
        }

        if (!ativo)
        {
            servico.Desativar(criador.Login);
        }

        context.CatalogosServico.Add(servico);
        await context.SaveChangesAsync();
        return servico;
    }

    private static async Task<(Usuario Admin, Usuario Atendente, Usuario Solicitante, Departamento DepartamentoTi, Departamento DepartamentoRh, CategoriaChamado CategoriaTi, CategoriaChamado CategoriaRh)> SeedUsuariosEContextoCatalogoAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", $"admin.portal.catalogo.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", $"aten.portal.catalogo.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", $"sol.portal.catalogo.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Solicitante);

        var departamentoTi = new Departamento("Tecnologia", "TI", null, "teste");
        var departamentoRh = new Departamento("Recursos Humanos", "RH", null, "teste");
        context.Departamentos.AddRange(departamentoTi, departamentoRh);
        await context.SaveChangesAsync();

        var categoriaTi = new CategoriaChamado("Suporte TI", null, departamentoTi.Id, "teste");
        var categoriaRh = new CategoriaChamado("Atendimento RH", null, departamentoRh.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaTi, categoriaRh);
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

        return (adminCompleto, atendenteCompleto, solicitanteCompleto, departamentoTi, departamentoRh, categoriaTi, categoriaRh);
    }
}
