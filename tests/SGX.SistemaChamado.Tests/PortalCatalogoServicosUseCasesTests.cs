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
        Assert.Null(response.Formulario);
    }

    [Fact]
    public async Task PrepararAberturaRetornaMetadadosDoFormularioAtivoPublicadoQuandoDisponivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Solicitar notebook",
            "Abertura guiada com formulario",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante,
            prioridadePadraoId: prioridade.Id);

        var formulario = await CriarFormularioServicoAsync(context, servico.Id, "Formulario de requisicao");
        var versaoRascunho = await CriarVersaoFormularioAsync(context, formulario.Id, 1, publicada: false, publicadoEm: null);
        await CriarCampoFormularioAsync(context, versaoRascunho.Id, "rascunho", "Campo rascunho", TipoCampoFormularioServico.TextoCurto, 1, ativo: true, visivel: true);

        var versaoPublicada = await CriarVersaoFormularioAsync(context, formulario.Id, 2, publicada: true, publicadoEm: new DateTime(2026, 6, 30, 12, 0, 0, DateTimeKind.Utc));
        var campoInativo = await CriarCampoFormularioAsync(context, versaoPublicada.Id, "inativo", "Campo inativo", TipoCampoFormularioServico.TextoCurto, 4, ativo: false, visivel: true);
        var campoInvisivel = await CriarCampoFormularioAsync(context, versaoPublicada.Id, "oculto", "Campo oculto", TipoCampoFormularioServico.TextoCurto, 3, ativo: true, visivel: false);
        var campoTexto = await CriarCampoFormularioAsync(context, versaoPublicada.Id, "justificativa", "Justificativa", TipoCampoFormularioServico.TextoLongo, 2, ativo: true, visivel: true, textoAjuda: "Explique a necessidade");
        var campoSelecao = await CriarCampoFormularioAsync(context, versaoPublicada.Id, "tipoAcesso", "Tipo de acesso", TipoCampoFormularioServico.SelecaoUnica, 1, ativo: true, visivel: true);
        await CriarOpcaoCampoFormularioAsync(context, campoSelecao.Id, "inativa", "Inativa", 3, ativo: false);
        await CriarOpcaoCampoFormularioAsync(context, campoSelecao.Id, "vpn", "VPN", 2, ativo: true);
        await CriarOpcaoCampoFormularioAsync(context, campoSelecao.Id, "email", "E-mail", 1, ativo: true);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.PrepararAberturaChamadoAsync(servico.Slug);

        Assert.NotNull(response.Formulario);
        Assert.Equal(formulario.Id, response.Formulario!.Id);
        Assert.Equal("Formulario de requisicao", response.Formulario.Nome);
        Assert.Equal(versaoPublicada.Id, response.Formulario.Versao.Id);
        Assert.Equal(2, response.Formulario.Versao.Numero);
        Assert.Equal(2, response.Formulario.Versao.Campos.Count);
        Assert.DoesNotContain(response.Formulario.Versao.Campos, x => x.Id == campoInativo.Id);
        Assert.DoesNotContain(response.Formulario.Versao.Campos, x => x.Id == campoInvisivel.Id);
        Assert.Equal(new[] { campoSelecao.Id, campoTexto.Id }, response.Formulario.Versao.Campos.Select(x => x.Id).ToArray());

        var campoSelecaoResponse = response.Formulario.Versao.Campos.First();
        Assert.Equal(campoSelecao.Id, campoSelecaoResponse.Id);
        Assert.Equal(new[] { "email", "vpn" }, campoSelecaoResponse.Opcoes.Select(x => x.Valor).ToArray());
        Assert.DoesNotContain(campoSelecaoResponse.Opcoes, x => x.Valor == "inativa");

        var campoTextoResponse = response.Formulario.Versao.Campos.Last();
        Assert.Equal(campoTexto.Id, campoTextoResponse.Id);
        Assert.Empty(campoTextoResponse.Opcoes);
    }

    [Fact]
    public async Task PrepararAberturaUsaMaiorVersaoAtivaQuandoNaoHouverVersaoPublicada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedUsuariosEContextoCatalogoAsync(context);
        var servico = await CriarServicoAsync(
            context,
            dados.Admin,
            "Servico com versoes ativas",
            "Descricao",
            dados.DepartamentoTi.Id,
            dados.CategoriaTi.Id,
            StatusCatalogoServico.Publicado,
            VisibilidadeCatalogoServico.Solicitante);

        var formulario = await CriarFormularioServicoAsync(context, servico.Id, "Formulario sem publicacao");
        var versao1 = await CriarVersaoFormularioAsync(context, formulario.Id, 1, publicada: false, publicadoEm: null);
        var versao3 = await CriarVersaoFormularioAsync(context, formulario.Id, 3, publicada: false, publicadoEm: null);
        await CriarCampoFormularioAsync(context, versao1.Id, "campoV1", "Campo V1", TipoCampoFormularioServico.TextoCurto, 1, ativo: true, visivel: true);
        await CriarCampoFormularioAsync(context, versao3.Id, "campoV3", "Campo V3", TipoCampoFormularioServico.TextoCurto, 1, ativo: true, visivel: true);

        var useCase = CriarUseCase(context, dados.Solicitante);
        var response = await useCase.PrepararAberturaChamadoAsync(servico.Slug);

        Assert.NotNull(response.Formulario);
        Assert.Equal(versao3.Id, response.Formulario!.Versao.Id);
        Assert.Equal(3, response.Formulario.Versao.Numero);
        Assert.Single(response.Formulario.Versao.Campos);
        Assert.Equal("campoV3", response.Formulario.Versao.Campos.Single().Nome);
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
            PortalUseCasesTestFactory.Repo<FormularioServico>(context),
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

    private static async Task<FormularioServico> CriarFormularioServicoAsync(
        SGXSistemaChamadoDbContext context,
        Guid catalogoServicoId,
        string nome,
        bool ativo = true)
    {
        var formulario = new FormularioServico(catalogoServicoId, nome, "Descricao do formulario", "teste");
        if (!ativo)
        {
            formulario.Inativar("teste");
        }

        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();
        return formulario;
    }

    private static async Task<FormularioServicoVersao> CriarVersaoFormularioAsync(
        SGXSistemaChamadoDbContext context,
        Guid formularioServicoId,
        int numero,
        bool publicada,
        DateTime? publicadoEm,
        bool ativo = true)
    {
        var versao = new FormularioServicoVersao(formularioServicoId, numero, publicada, publicadoEm, "teste");
        if (!ativo)
        {
            versao.Inativar("teste");
        }

        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();
        return versao;
    }

    private static async Task<CampoFormularioServico> CriarCampoFormularioAsync(
        SGXSistemaChamadoDbContext context,
        Guid formularioServicoVersaoId,
        string nome,
        string rotulo,
        TipoCampoFormularioServico tipo,
        int ordem,
        bool ativo,
        bool visivel,
        string? textoAjuda = null)
    {
        var campo = new CampoFormularioServico(
            formularioServicoVersaoId,
            nome,
            rotulo,
            tipo,
            obrigatorio: true,
            ordem,
            textoAjuda,
            visivel,
            "teste");

        if (!ativo)
        {
            campo.Inativar("teste");
        }

        context.CamposFormularioServico.Add(campo);
        await context.SaveChangesAsync();
        return campo;
    }

    private static async Task<OpcaoCampoFormularioServico> CriarOpcaoCampoFormularioAsync(
        SGXSistemaChamadoDbContext context,
        Guid campoFormularioServicoId,
        string valor,
        string rotulo,
        int ordem,
        bool ativo)
    {
        var opcao = new OpcaoCampoFormularioServico(campoFormularioServicoId, valor, rotulo, ordem, "teste");
        if (!ativo)
        {
            opcao.Inativar("teste");
        }

        context.OpcoesCamposFormularioServico.Add(opcao);
        await context.SaveChangesAsync();
        return opcao;
    }
}
