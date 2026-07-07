using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalCatalogoServicosIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public PortalCatalogoServicosIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListagemRetornaSomenteServicosPublicadosAtivosEVisiveis()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        await SeedServicosAsync(prefixo, adminEmail);

        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos?termo={prefixo}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PortalListaCatalogoServicosResponse>();

        Assert.NotNull(payload);
        Assert.Single(payload!.Items);
        Assert.Contains(prefixo, payload.Items.Single().Nome);
    }

    [Fact]
    public async Task DetalhePorSlugRetorna404QuandoServicoNaoPublicado()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoRascunhoAsync(prefixo, adminEmail);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DetalhePorSlugRespeitaVisibilidade()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";
        var atendenteEmail = $"aten.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();
        using var clientAtendente = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");
        AddDevHeaders(clientAtendente, atendenteEmail, "Atendente Catalogo", "Atendente");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");
        _ = await clientAtendente.GetAsync("/api/me");

        var slug = await SeedServicoVisibilidadeAtendenteAsync(prefixo, adminEmail);

        var responseSolicitante = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}");
        var responseAtendente = await clientAtendente.GetAsync($"/api/portal/catalogo-servicos/{slug}");

        Assert.Equal(HttpStatusCode.NotFound, responseSolicitante.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseAtendente.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetornaServicoValido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, permiteAberturaChamado: true);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PortalPrepararChamadoCatalogoServicoDto>();
        Assert.NotNull(payload);
        Assert.Equal(slug, payload!.Slug);
        Assert.True(payload.PermiteAberturaChamado);
        Assert.Null(payload.Formulario);
    }

    [Fact]
    public async Task PrepararChamadoRetornaFormularioComCamposVisiveisEOpcoesAtivasOrdenadas()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var dados = await SeedServicoParaPreparacaoComFormularioAsync(prefixo, adminEmail);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{dados.Slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PortalPrepararChamadoCatalogoServicoDto>();
        Assert.NotNull(payload);
        Assert.Equal(dados.ServicoId, payload!.CatalogoServicoId);
        Assert.Equal(dados.SlaPadraoId, payload.SlaPadraoId);
        Assert.True(payload.RequerAprovacao);

        Assert.NotNull(payload.Formulario);
        Assert.Equal(2, payload.Formulario!.Versao.Numero);
        Assert.Equal(2, payload.Formulario.Versao.Campos.Count);
        Assert.Equal(new[] { "tipoAcesso", "justificativa" }, payload.Formulario.Versao.Campos.Select(x => x.Nome).ToArray());

        var campos = payload.Formulario.Versao.Campos.ToArray();
        var campoSelecao = campos[0];
        Assert.Equal(new[] { "email", "vpn" }, campoSelecao.Opcoes.Select(x => x.Valor).ToArray());

        var campoTexto = campos[1];
        Assert.Empty(campoTexto.Opcoes);
    }

    [Fact]
    public async Task PrepararChamadoRetorna404QuandoServicoInexistente()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");
        _ = await clientSolicitante.GetAsync("/api/me");

        var response = await clientSolicitante.GetAsync("/api/portal/catalogo-servicos/inexistente/preparar-chamado");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetorna400QuandoServicoNaoPermiteAbertura()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, permiteAberturaChamado: false);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetorna404QuandoServicoSemVisibilidade()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, visibilidade: VisibilidadeCatalogoServico.Atendente);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoUsaContratoDedicadoESemanticaDeRequisicao()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, _) = await SeedServicoParaRequisicaoCatalogoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Solicitar acesso remoto",
            Descricao = "Preciso de acesso remoto para trabalho externo."
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, payload!.NaturezaChamado);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.FirstAsync(x => x.Id == payload.Id);
        Assert.Equal(servicoId, chamado.CatalogoServicoId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
    }

    [Fact]
    public async Task AberturaGuiadaNoPortalAplicaClassificacaoDefinidaNoCatalogo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.class.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.class.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var dados = await SeedServicoParaClassificacaoCatalogoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/chamados", new CriarChamadoRequest
        {
            Titulo = "Solicitar notebook",
            Descricao = "A classificacao deve vir do catalogo.",
            CatalogoServicoId = dados.ServicoId,
            CategoriaId = dados.CategoriaAlternativaId,
            SubcategoriaId = dados.SubcategoriaAlternativaId,
            PrioridadeId = dados.PrioridadeAlternativaId,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, payload!.NaturezaChamado);
        Assert.Equal(dados.CategoriaCatalogoId, payload.CategoriaId);
        Assert.Equal(dados.SubcategoriaCatalogoId, payload.SubcategoriaId);
        Assert.Equal(dados.PrioridadeCatalogoId, payload.PrioridadeId);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.SingleAsync(x => x.Id == payload.Id);
        Assert.Equal(dados.ServicoId, chamado.CatalogoServicoId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
        Assert.Equal(dados.CategoriaCatalogoId, chamado.CategoriaId);
        Assert.Equal(dados.SubcategoriaCatalogoId, chamado.SubcategoriaId);
        Assert.Equal(dados.PrioridadeCatalogoId, chamado.PrioridadeId);
    }

    [Fact]
    public async Task AberturaGuiadaRejeitaCamposSensiveisEnviadosForaDoContratoPublico()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.protecao.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.protecao.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var dados = await SeedServicoParaRequisicaoCatalogoComFormularioCompletoAsync(prefixo, adminEmail);
        var categoriaMaliciosaId = Guid.NewGuid();
        var subcategoriaMaliciosaId = Guid.NewGuid();
        var prioridadeMaliciosaId = Guid.NewGuid();
        var grupoTecnicoMaliciosoId = Guid.NewGuid();
        var slaMaliciosoId = Guid.NewGuid();
        var aprovacaoMaliciosaId = Guid.NewGuid();

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new
        {
            catalogoServicoId = dados.ServicoId,
            titulo = "Tentativa de sobrescrever backend",
            descricao = "O backend deve prevalecer sobre o payload do solicitante.",
            naturezaChamado = (int)NaturezaChamadoEnum.Incidente,
            categoriaId = categoriaMaliciosaId,
            subcategoriaId = subcategoriaMaliciosaId,
            prioridadeId = prioridadeMaliciosaId,
            grupoTecnicoId = grupoTecnicoMaliciosoId,
            slaId = slaMaliciosoId,
            requerAprovacao = false,
            aprovacaoPendente = false,
            aprovacaoChamadoId = aprovacaoMaliciosaId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(payload));

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Empty(await dbContext.Chamados
            .Where(x => x.CatalogoServicoId == dados.ServicoId && x.Titulo == "Tentativa de sobrescrever backend")
            .ToListAsync());
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoAceitaRespostasFormularioNulasQuandoServicoNaoPossuiFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, _) = await SeedServicoParaRequisicaoCatalogoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Solicitar acessos",
            Descricao = "Servico sem formulario deve aceitar null.",
            RespostasFormulario = null
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, payload!.NaturezaChamado);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.SingleAsync(x => x.Id == payload.Id);
        Assert.Equal(servicoId, chamado.CatalogoServicoId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
        Assert.False(await dbContext.RespostasFormularioChamado.AnyAsync(x => x.ChamadoId == payload.Id));
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoAceitaListaVaziaQuandoServicoNaoPossuiFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, _) = await SeedServicoParaRequisicaoCatalogoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Solicitar acessos",
            Descricao = "Servico sem formulario deve aceitar lista vazia.",
            RespostasFormulario = []
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.False(await dbContext.RespostasFormularioChamado.AnyAsync(x => x.ChamadoId == payload!.Id));
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaRespostasPreenchidasQuandoServicoNaoPossuiFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, _) = await SeedServicoParaRequisicaoCatalogoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Solicitar acessos",
            Descricao = "Sem formulario nao deve aceitar respostas preenchidas.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valor = "vpn"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("nao possui formulario configurado", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaQuandoCampoObrigatorioNaoForRespondido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, _) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao sem obrigatorio",
            Descricao = "Deve falhar pela ausencia da resposta.",
            RespostasFormulario = []
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Campo obrigatorio", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaQuandoCampoObrigatorioReceberValorVazio()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com obrigatorio vazio",
            Descricao = "Deve falhar por valor vazio.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "   "
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("RespostasFormulario[0]", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Informe Valor ou Valores", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaQuandoCampoObrigatorioReceberValoresVazios()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(
            prefixo,
            adminEmail,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoMultipla);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com lista obrigatoria vazia",
            Descricao = "Deve falhar por lista vazia.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valores = []
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("RespostasFormulario[0]", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Informe Valor ou Valores", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoAceitaRespostaObrigatoriaComValor()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com obrigatorio",
            Descricao = "Deve abrir normalmente com a resposta obrigatoria.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "VPN"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.SingleAsync(x => x.CatalogoServicoId == servicoId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRegistraHistoricoResumoQuandoFormularioForPreenchido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com historico de formulario",
            Descricao = "Deve registrar historico resumido.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "VPN"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var historicos = await dbContext.HistoricosChamado.Where(x => x.ChamadoId == payload!.Id).ToListAsync();

        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        Assert.Contains(
            historicos,
            x => x.Tipo == TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura
                 && x.Descricao == "Chamado aberto com formulario do servico preenchido.");
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("VPN", StringComparison.Ordinal));
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaRespostaComFormatoInvalido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(
            prefixo,
            adminEmail,
            tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com formato invalido",
            Descricao = "Deve falhar por formato.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "abc"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("numero decimal valido", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRetornaErroPadraoQuandoPayloadForContratualmenteInvalido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Payload invalido",
            Descricao = "Valor e Valores ao mesmo tempo devem falhar no validator.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "vpn",
                    Valores = ["email"]
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("[", payload, StringComparison.Ordinal);
        Assert.Contains("\"campo\"", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"mensagem\"", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRetornaErroPadraoQuandoRespostaForInvalidaNoUseCase()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(
            prefixo,
            adminEmail,
            tipoCampoObrigatorio: TipoCampoFormularioServico.Numero);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Use case invalido",
            Descricao = "Formato numerico invalido deve manter o padrao de erro.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "abc"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("{", payload, StringComparison.Ordinal);
        Assert.Contains("\"mensagem\"", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("numero decimal valido", payload, StringComparison.OrdinalIgnoreCase);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Empty(await dbContext.Chamados.Where(x => x.CatalogoServicoId == servicoId && x.Titulo == "Use case invalido").ToListAsync());
        Assert.Empty(await dbContext.RespostasFormularioChamado.Where(x => x.CampoFormularioServicoId == campoObrigatorioId).ToListAsync());
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRetornaErroPadraoQuandoOpcaoNaoForPermitidaParaOCampo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(
            prefixo,
            adminEmail,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Opcao invalida",
            Descricao = "Opcao nao configurada deve falhar com erro padrao.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "inexistente"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("{", payload, StringComparison.Ordinal);
        Assert.Contains("\"mensagem\"", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("opcao ativa", payload, StringComparison.OrdinalIgnoreCase);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Empty(await dbContext.Chamados.Where(x => x.CatalogoServicoId == servicoId && x.Titulo == "Opcao invalida").ToListAsync());
        Assert.Empty(await dbContext.RespostasFormularioChamado.Where(x => x.CampoFormularioServicoId == campoObrigatorioId).ToListAsync());
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRetornaErroPadraoQuandoOpcaoEstiverInativa()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(
            prefixo,
            adminEmail,
            tipoCampoObrigatorio: TipoCampoFormularioServico.SelecaoUnica);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            var opcao = await dbContext.OpcoesCamposFormularioServico
                .SingleAsync(x => x.CampoFormularioServicoId == campoObrigatorioId && x.Valor == "vpn");
            opcao.Inativar("integration-test");
            await dbContext.SaveChangesAsync();
        }

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Opcao inativa",
            Descricao = "Opcao inativa deve falhar com erro padrao.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "vpn"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.StartsWith("{", payload, StringComparison.Ordinal);
        Assert.Contains("\"mensagem\"", payload, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("opcao ativa", payload, StringComparison.OrdinalIgnoreCase);

        using var scopeValidacao = _factory.Services.CreateScope();
        var dbContextValidacao = scopeValidacao.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Empty(await dbContextValidacao.Chamados.Where(x => x.CatalogoServicoId == servicoId && x.Titulo == "Opcao inativa").ToListAsync());
        Assert.Empty(await dbContextValidacao.RespostasFormularioChamado.Where(x => x.CampoFormularioServicoId == campoObrigatorioId).ToListAsync());
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoComFormularioValidoRetornaSucessoEPreservaRegrasLegadas()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var dados = await SeedServicoParaRequisicaoCatalogoComFormularioCompletoAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = dados.ServicoId,
            Titulo = "Requisicao valida completa",
            Descricao = "Fluxo valido com formulario completo.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoTextoCurtoId, Valor = "vpn" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoTextoLongoId, Valor = "Justificativa detalhada do acesso." },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoNumeroId, Valor = "123.45" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoDataId, Valor = "2026-07-01" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoBooleanoId, Valor = "false" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoSelecaoUnicaId, Valor = "email" },
                new RespostaFormularioAberturaRequest { CampoFormularioServicoId = dados.CampoSelecaoMultiplaId, Valores = ["vpn", "teams"] }
            ]
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, payload!.NaturezaChamado);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.SingleAsync(x => x.Id == payload.Id);
        var chamadoSla = await dbContext.ChamadosSla.SingleAsync(x => x.ChamadoId == payload.Id);
        var aprovacao = await dbContext.AprovacoesChamado.SingleAsync(x => x.ChamadoId == payload.Id);
        var status = await dbContext.StatusChamado.SingleAsync(x => x.Id == chamado.StatusId);
        var historicos = await dbContext.HistoricosChamado.Where(x => x.ChamadoId == payload.Id).ToListAsync();
        var respostasPersistidas = await dbContext.RespostasFormularioChamado
            .Where(x => x.ChamadoId == payload.Id)
            .OrderBy(x => x.CampoFormularioServicoId)
            .ToListAsync();

        Assert.Equal(dados.ServicoId, chamado.CatalogoServicoId);
        Assert.Equal(dados.GrupoTecnicoId, chamado.GrupoTecnicoId);
        Assert.Equal(dados.SlaPadraoId, chamadoSla.PoliticaSlaId);
        Assert.Equal(StatusAprovacaoChamado.Pendente, aprovacao.Status);
        Assert.Equal(StatusChamadoEnum.Aberto, status.Codigo);
        Assert.Equal(7, respostasPersistidas.Count);
        Assert.All(respostasPersistidas, x => Assert.Equal(payload.Id, x.ChamadoId));
        Assert.All(respostasPersistidas, x => Assert.Equal(dados.FormularioServicoVersaoId, x.FormularioServicoVersaoId));
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoTextoCurtoId && x.Valor == "vpn" && x.ValoresJson is null);
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoTextoLongoId && x.Valor == "Justificativa detalhada do acesso.");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoNumeroId && x.Valor == "123.45");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoDataId && x.Valor == "2026-07-01");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoBooleanoId && x.Valor == "false");
        Assert.Contains(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoSelecaoUnicaId && x.Valor == "email");
        var respostaMultipla = Assert.Single(respostasPersistidas, x => x.CampoFormularioServicoId == dados.CampoSelecaoMultiplaId);
        Assert.Null(respostaMultipla.Valor);
        Assert.Equal(["vpn", "teams"], respostaMultipla.ObterValores());
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("Justificativa detalhada", StringComparison.Ordinal));
        Assert.DoesNotContain(historicos, x => x.Descricao.Contains("123.45", StringComparison.Ordinal));
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoComRespostasPersistidasRegistraAuditoriaTecnicaSegura()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, campoObrigatorioId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Requisicao com auditoria tecnica",
            Descricao = "Deve registrar auditoria segura.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoObrigatorioId,
                    Valor = "VPN"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var evento = await dbContext.EventosAuditoria
            .OrderByDescending(x => x.DataEvento)
            .FirstAsync(x => x.Entidade == "RespostaFormularioChamado" && x.EntidadeId == payload!.Id.ToString());

        Assert.Equal("Chamados", evento.Modulo);
        Assert.Equal(TipoAcaoAuditoria.Criacao, evento.Acao);
        Assert.Equal("Respostas do formulario persistidas na abertura guiada.", evento.Descricao);
        Assert.DoesNotContain("VPN", evento.DadosDepois, StringComparison.Ordinal);

        var dadosDepois = JsonNode.Parse(evento.DadosDepois!)!.AsObject();
        Assert.Equal(payload.Id.ToString(), dadosDepois["ChamadoId"]!.ToString());
        Assert.Equal(1, dadosDepois["QuantidadeRespostasPersistidas"]!.GetValue<int>());
        Assert.Equal("AberturaGuiadaCatalogo", dadosDepois["Origem"]!.GetValue<string>());
        Assert.DoesNotContain("Campo obrigatorio", evento.DadosDepois, StringComparison.Ordinal);

        var metadados = JsonNode.Parse(evento.Metadados!)!.AsObject();
        Assert.Equal("AberturaGuiadaCatalogo", metadados["origem"]!.GetValue<string>());
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoRejeitaRespostaDeOutroServico()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoOrigemId, _, _) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync(prefixo, adminEmail);
        var (servicoOutroId, _, campoOutroServicoId) = await SeedServicoParaRequisicaoCatalogoComFormularioAsync($"{prefixo}-outro", adminEmail);

        Assert.NotEqual(servicoOrigemId, servicoOutroId);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoOrigemId,
            Titulo = "Requisicao com campo externo",
            Descricao = "Deve falhar por escopo.",
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoOutroServicoId,
                    Valor = "abc"
                }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("fora do escopo", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AbrirRequisicaoServicoPorCatalogoAplicaGrupoTecnicoConfigurado()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var (servicoId, _, grupoTecnicoId) = await SeedServicoParaRequisicaoCatalogoAsync(prefixo, adminEmail, configurarGrupoTecnico: true);

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = servicoId,
            Titulo = "Solicitar acesso remoto",
            Descricao = "Preciso de acesso remoto para trabalho externo."
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.FirstAsync(x => x.Id == payload!.Id);
        Assert.Equal(grupoTecnicoId, chamado.GrupoTecnicoId);
        Assert.Equal(NaturezaChamadoEnum.Requisicao, chamado.NaturezaChamado);
    }
    [Fact]
    public async Task AbrirRequisicaoServicoRetorna400QuandoRequisicaoForInvalida()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var response = await clientSolicitante.PostAsJsonAsync("/api/portal/catalogo-servicos/requisicoes", new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.Empty, // Inválido
            Titulo = "", // Inválido
            Descricao = "" // Inválido
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task SeedServicosAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");

        var publicado = new CatalogoServico(
            $"{prefixo}-publicado",
            $"{prefixo}-publicado",
            "descricao publicada",
            "instrucao publicada",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        publicado.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(publicado);

        dbContext.CatalogosServico.Add(new CatalogoServico(
            $"{prefixo}-rascunho",
            $"{prefixo}-rascunho",
            "descricao rascunho",
            "instrucao rascunho",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test"));

        var inativo = new CatalogoServico(
            $"{prefixo}-inativo",
            $"{prefixo}-inativo",
            "descricao inativa",
            "instrucao inativa",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        inativo.Publicar(admin.Id, "integration-test");
        inativo.Desativar("integration-test");
        dbContext.CatalogosServico.Add(inativo);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Guid ServicoId, string Slug, Guid? GrupoTecnicoId)> SeedServicoParaRequisicaoCatalogoAsync(
        string prefixo,
        string emailAdmin,
        bool configurarGrupoTecnico = false,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de requisicao por catalogo.");
        Guid? grupoTecnicoId = null;

        if (configurarGrupoTecnico)
        {
            var grupoTecnico = new GrupoTecnico($"Grupo Catalogo {prefixo}", null, "integration-test");
            dbContext.GruposTecnicos.Add(grupoTecnico);
            await dbContext.SaveChangesAsync(cancellationToken);
            grupoTecnicoId = grupoTecnico.Id;
        }

        var servico = new CatalogoServico(
            $"{prefixo}-requisicao",
            $"{prefixo}-requisicao",
            "descricao para requisicao",
            "instrucao para requisicao",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test",
            grupoTecnicoId);

        servico.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (servico.Id, servico.Slug, grupoTecnicoId);
    }

    private async Task<(
        Guid ServicoId,
        Guid CategoriaCatalogoId,
        Guid SubcategoriaCatalogoId,
        Guid PrioridadeCatalogoId,
        Guid CategoriaAlternativaId,
        Guid SubcategoriaAlternativaId,
        Guid PrioridadeAlternativaId)> SeedServicoParaClassificacaoCatalogoAsync(
        string prefixo,
        string emailAdmin,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoriaCatalogo = await ObterOuCriarCategoriaAsync(dbContext, $"{prefixo}-catalogo", cancellationToken);
        var departamentoId = categoriaCatalogo.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de classificacao do catalogo.");
        var subcategoriaCatalogo = new SubcategoriaChamado(categoriaCatalogo.Id, $"Subcategoria catalogo {prefixo}", null, "integration-test");
        var categoriaAlternativa = new CategoriaChamado($"Categoria alternativa {prefixo}", null, departamentoId, "integration-test");
        await dbContext.CategoriasChamado.AddAsync(categoriaAlternativa, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var subcategoriaAlternativa = new SubcategoriaChamado(categoriaAlternativa.Id, $"Subcategoria alternativa {prefixo}", null, "integration-test");
        dbContext.SubcategoriasChamado.AddRange(subcategoriaCatalogo, subcategoriaAlternativa);
        await dbContext.SaveChangesAsync(cancellationToken);

        var prioridadeCatalogo = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo && x.Nivel == PrioridadeChamadoEnum.Baixa, cancellationToken);
        var prioridadeAlternativa = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo && x.Nivel == PrioridadeChamadoEnum.Critica, cancellationToken);

        var servico = new CatalogoServico(
            $"{prefixo}-classificacao",
            $"{prefixo}-classificacao",
            "descricao para classificacao",
            "instrucao para classificacao",
            departamentoId,
            categoriaCatalogo.Id,
            subcategoriaCatalogo.Id,
            prioridadeCatalogo.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");

        servico.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (
            servico.Id,
            categoriaCatalogo.Id,
            subcategoriaCatalogo.Id,
            prioridadeCatalogo.Id,
            categoriaAlternativa.Id,
            subcategoriaAlternativa.Id,
            prioridadeAlternativa.Id);
    }

    private async Task<(Guid ServicoId, string Slug, Guid CampoObrigatorioId)> SeedServicoParaRequisicaoCatalogoComFormularioAsync(
        string prefixo,
        string emailAdmin,
        TipoCampoFormularioServico tipoCampoObrigatorio = TipoCampoFormularioServico.TextoCurto,
        bool obrigatorio = true,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de requisicao por catalogo com formulario.");

        var servico = new CatalogoServico(
            $"{prefixo}-requisicao-formulario",
            $"{prefixo}-requisicao-formulario",
            "descricao para requisicao com formulario",
            "instrucao para requisicao com formulario",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");

        servico.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);

        var formulario = new FormularioServico(servico.Id, "Formulario requisicao", "Obrigatoriedade", "integration-test");
        dbContext.FormulariosServico.Add(formulario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var versao = new FormularioServicoVersao(formulario.Id, 2, true, new DateTime(2026, 7, 1, 15, 0, 0, DateTimeKind.Utc), "integration-test");
        dbContext.FormulariosServicoVersoes.Add(versao);
        await dbContext.SaveChangesAsync(cancellationToken);

        var campoObrigatorio = new CampoFormularioServico(
            versao.Id,
            "acessoSolicitado",
            "Campo obrigatorio",
            tipoCampoObrigatorio,
            obrigatorio,
            1,
            "Informe o acesso",
            true,
            "integration-test");

        dbContext.CamposFormularioServico.Add(campoObrigatorio);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (tipoCampoObrigatorio is TipoCampoFormularioServico.SelecaoUnica or TipoCampoFormularioServico.SelecaoMultipla)
        {
            dbContext.OpcoesCamposFormularioServico.AddRange(
                new OpcaoCampoFormularioServico(campoObrigatorio.Id, "vpn", "VPN", 1, "integration-test"),
                new OpcaoCampoFormularioServico(campoObrigatorio.Id, "email", "E-mail", 2, "integration-test"));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return (servico.Id, servico.Slug, campoObrigatorio.Id);
    }

    private async Task<(
        Guid ServicoId,
        string Slug,
        Guid GrupoTecnicoId,
        Guid SlaPadraoId,
        Guid CategoriaId,
        Guid SubcategoriaId,
        Guid PrioridadeId,
        Guid FormularioServicoVersaoId,
        Guid CampoTextoCurtoId,
        Guid CampoTextoLongoId,
        Guid CampoNumeroId,
        Guid CampoDataId,
        Guid CampoBooleanoId,
        Guid CampoSelecaoUnicaId,
        Guid CampoSelecaoMultiplaId)> SeedServicoParaRequisicaoCatalogoComFormularioCompletoAsync(
        string prefixo,
        string emailAdmin,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste completo de requisicao.");
        var subcategoria = new SubcategoriaChamado(categoria.Id, $"Subcategoria completa {prefixo}", null, "integration-test");
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo, cancellationToken);
        var sla = await dbContext.SlaPoliticas.FirstAsync(x => x.Ativo, cancellationToken);

        var grupoTecnico = new GrupoTecnico($"Grupo completo {prefixo}", null, "integration-test");
        dbContext.GruposTecnicos.Add(grupoTecnico);
        dbContext.SubcategoriasChamado.Add(subcategoria);
        await dbContext.SaveChangesAsync(cancellationToken);

        var servico = new CatalogoServico(
            $"{prefixo}-requisicao-formulario-completo",
            $"{prefixo}-requisicao-formulario-completo",
            "descricao completa",
            "instrucao completa",
            departamentoId,
            categoria.Id,
            subcategoria.Id,
            prioridade.Id,
            sla.Id,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            true,
            1,
            admin.Id,
            "integration-test",
            grupoTecnico.Id);

        servico.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);

        var formulario = new FormularioServico(servico.Id, "Formulario completo API", "Teste integrado", "integration-test");
        dbContext.FormulariosServico.Add(formulario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var versao = new FormularioServicoVersao(formulario.Id, 2, true, new DateTime(2026, 7, 1, 15, 0, 0, DateTimeKind.Utc), "integration-test");
        dbContext.FormulariosServicoVersoes.Add(versao);
        await dbContext.SaveChangesAsync(cancellationToken);

        var campoTextoCurto = new CampoFormularioServico(versao.Id, "textoCurto", "Texto curto", TipoCampoFormularioServico.TextoCurto, true, 1, null, true, "integration-test");
        var campoTextoLongo = new CampoFormularioServico(versao.Id, "textoLongo", "Texto longo", TipoCampoFormularioServico.TextoLongo, true, 2, null, true, "integration-test");
        var campoNumero = new CampoFormularioServico(versao.Id, "numero", "Numero", TipoCampoFormularioServico.Numero, true, 3, null, true, "integration-test");
        var campoData = new CampoFormularioServico(versao.Id, "data", "Data", TipoCampoFormularioServico.Data, true, 4, null, true, "integration-test");
        var campoBooleano = new CampoFormularioServico(versao.Id, "booleano", "Booleano", TipoCampoFormularioServico.Booleano, true, 5, null, true, "integration-test");
        var campoSelecaoUnica = new CampoFormularioServico(versao.Id, "selecaoUnica", "Selecao unica", TipoCampoFormularioServico.SelecaoUnica, true, 6, null, true, "integration-test");
        var campoSelecaoMultipla = new CampoFormularioServico(versao.Id, "selecaoMultipla", "Selecao multipla", TipoCampoFormularioServico.SelecaoMultipla, true, 7, null, true, "integration-test");

        dbContext.CamposFormularioServico.AddRange(
            campoTextoCurto,
            campoTextoLongo,
            campoNumero,
            campoData,
            campoBooleano,
            campoSelecaoUnica,
            campoSelecaoMultipla);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.OpcoesCamposFormularioServico.AddRange(
            new OpcaoCampoFormularioServico(campoSelecaoUnica.Id, "email", "E-mail", 1, "integration-test"),
            new OpcaoCampoFormularioServico(campoSelecaoUnica.Id, "vpn", "VPN", 2, "integration-test"),
            new OpcaoCampoFormularioServico(campoSelecaoMultipla.Id, "teams", "Teams", 1, "integration-test"),
            new OpcaoCampoFormularioServico(campoSelecaoMultipla.Id, "vpn", "VPN", 2, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        return (
            servico.Id,
            servico.Slug,
            grupoTecnico.Id,
            sla.Id,
            categoria.Id,
            subcategoria.Id,
            prioridade.Id,
            versao.Id,
            campoTextoCurto.Id,
            campoTextoLongo.Id,
            campoNumero.Id,
            campoData.Id,
            campoBooleano.Id,
            campoSelecaoUnica.Id,
            campoSelecaoMultipla.Id);
    }

    private async Task<string> SeedServicoRascunhoAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-rascunho-detalhe";

        dbContext.CatalogosServico.Add(new CatalogoServico(
            "Rascunho detalhe",
            slug,
            "descricao rascunho",
            "instrucao rascunho",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test"));

        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<string> SeedServicoVisibilidadeAtendenteAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-atendente-detalhe";

        var servico = new CatalogoServico(
            "Servico atendente",
            slug,
            "descricao atendente",
            "instrucao atendente",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Atendente,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        servico.Publicar(admin.Id, "integration-test");

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<string> SeedServicoParaPreparacaoAsync(
        string prefixo,
        string emailAdmin,
        bool permiteAberturaChamado = true,
        VisibilidadeCatalogoServico visibilidade = VisibilidadeCatalogoServico.Solicitante,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-preparar-chamado";

        var servico = new CatalogoServico(
            "Servico preparar chamado",
            slug,
            "descricao",
            "instrucoes",
            departamentoId,
            categoria.Id,
            null,
            prioridade.Id,
            null,
            null,
            visibilidade,
            permiteAberturaChamado,
            false,
            1,
            admin.Id,
            "integration-test");
        servico.Publicar(admin.Id, "integration-test");

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<(Guid ServicoId, string Slug, Guid SlaPadraoId)> SeedServicoParaPreparacaoComFormularioAsync(
        string prefixo,
        string emailAdmin,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo, cancellationToken);
        var sla = await dbContext.SlaPoliticas.FirstAsync(x => x.Ativo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-preparar-chamado-formulario";

        var servico = new CatalogoServico(
            "Servico preparar chamado com formulario",
            slug,
            "descricao",
            "instrucoes",
            departamentoId,
            categoria.Id,
            null,
            prioridade.Id,
            sla.Id,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            true,
            1,
            admin.Id,
            "integration-test");
        servico.Publicar(admin.Id, "integration-test");

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);

        var formulario = new FormularioServico(servico.Id, "Formulario portal", "Metadados para abertura", "integration-test");
        dbContext.FormulariosServico.Add(formulario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var versao1 = new FormularioServicoVersao(formulario.Id, 1, false, null, "integration-test");
        var versao2 = new FormularioServicoVersao(formulario.Id, 2, true, new DateTime(2026, 6, 30, 12, 0, 0, DateTimeKind.Utc), "integration-test");
        dbContext.FormulariosServicoVersoes.AddRange(versao1, versao2);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.CamposFormularioServico.Add(new CampoFormularioServico(
            versao1.Id,
            "rascunho",
            "Rascunho",
            TipoCampoFormularioServico.TextoCurto,
            false,
            1,
            null,
            true,
            "integration-test"));

        var campoSelecao = new CampoFormularioServico(
            versao2.Id,
            "tipoAcesso",
            "Tipo de acesso",
            TipoCampoFormularioServico.SelecaoUnica,
            true,
            1,
            "Selecione uma opcao",
            true,
            "integration-test");

        var campoTexto = new CampoFormularioServico(
            versao2.Id,
            "justificativa",
            "Justificativa",
            TipoCampoFormularioServico.TextoLongo,
            true,
            2,
            "Explique a necessidade",
            true,
            "integration-test");

        var campoInvisivel = new CampoFormularioServico(
            versao2.Id,
            "oculto",
            "Oculto",
            TipoCampoFormularioServico.TextoCurto,
            false,
            3,
            null,
            false,
            "integration-test");

        var campoInativo = new CampoFormularioServico(
            versao2.Id,
            "inativo",
            "Inativo",
            TipoCampoFormularioServico.TextoCurto,
            false,
            4,
            null,
            true,
            "integration-test");
        campoInativo.Inativar("integration-test");

        dbContext.CamposFormularioServico.AddRange(campoSelecao, campoTexto, campoInvisivel, campoInativo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var opcaoInativa = new OpcaoCampoFormularioServico(campoSelecao.Id, "inativa", "Inativa", 3, "integration-test");
        opcaoInativa.Inativar("integration-test");
        dbContext.OpcoesCamposFormularioServico.AddRange(
            new OpcaoCampoFormularioServico(campoSelecao.Id, "vpn", "VPN", 2, "integration-test"),
            new OpcaoCampoFormularioServico(campoSelecao.Id, "email", "E-mail", 1, "integration-test"),
            opcaoInativa);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (servico.Id, slug, sla.Id);
    }

    private static async Task<CategoriaChamado> ObterOuCriarCategoriaAsync(SGXSistemaChamadoDbContext dbContext, string prefixo, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(x => x.Nome == $"Categoria Catalogo {prefixo}", cancellationToken);
        if (categoria is not null)
        {
            return categoria;
        }

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(x => x.Nome == $"Departamento Catalogo {prefixo}", cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento($"Departamento Catalogo {prefixo}", "DCA", null, "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        categoria = new CategoriaChamado($"Categoria Catalogo {prefixo}", null, departamento.Id, "integration-test");
        dbContext.CategoriasChamado.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);
        return categoria;
    }

    private static void AddDevHeaders(HttpClient client, string email, string nome, string role)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Add("X-Dev-User-Email", email);
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", nome);
        client.DefaultRequestHeaders.Add("X-Dev-User-Role", role);
    }
}
