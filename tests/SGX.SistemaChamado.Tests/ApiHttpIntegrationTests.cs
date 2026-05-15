using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class ApiHttpIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ApiHttpIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MeRetornaOkEmModoLocalDevelopment()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.me@empresa.com", "Solicitante Teste", "Solicitante");

        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        Assert.Equal("LocalDevelopment", json.RootElement.GetProperty("autenticadoPor").GetString());
    }

    [Fact]
    public async Task LoginLocalSgxRetornaTokenEApiMeComAutenticadoPorLocalSgx()
    {
        const string email = "local.login@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Local", senha);

        using var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/local/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginPayload = await loginResponse.Content.ReadAsStringAsync();
        using var loginJson = JsonDocument.Parse(loginPayload);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(accessToken));

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var meResponse = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var mePayload = await meResponse.Content.ReadAsStringAsync();
        using var meJson = JsonDocument.Parse(mePayload);
        Assert.Equal("LocalSgx", meJson.RootElement.GetProperty("autenticadoPor").GetString());
    }

    [Fact]
    public async Task LoginLocalSgxBloqueiaUsuarioInativo()
    {
        const string email = "local.inativo@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Inativo", senha, TipoPerfil.Solicitante);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            var usuario = await dbContext.Usuarios.FirstAsync(x => x.Email == email);
            usuario.AlterarSituacao(SituacaoUsuario.Inativo, "integration-test");
            await dbContext.SaveChangesAsync();
        }

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/local/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AuthProvedoresRetornaConfiguracaoEsperada()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.provedores@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        _ = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", new
        {
            habilitado = false,
            provedorPrincipal = "Local",
            loginLocalHabilitado = true,
            tenantId = "",
            clientId = "",
            audience = "",
            issuer = "",
            authority = "",
            apiScope = "",
            redirectUri = "",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        });

        var response = await client.GetAsync("/api/auth/provedores");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);

        var provedorPrincipal = json.RootElement.GetProperty("provedorPrincipal").GetString();
        Assert.True(
            provedorPrincipal is "Local" or "Hibrido",
            $"Provedor principal inesperado: {provedorPrincipal}");
        _ = json.RootElement.GetProperty("loginMicrosoftHabilitado").GetBoolean();
        Assert.True(json.RootElement.GetProperty("loginLocalSgxHabilitado").GetBoolean());
        Assert.True(json.RootElement.GetProperty("loginLocalDevelopmentHabilitado").GetBoolean());
    }

    [Fact]
    public async Task MeRetornaPermissoesEfetivas()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.me@empresa.com", "Atendente Teste", "Atendente");

        _ = await client.GetAsync("/api/me");
        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        Assert.True(json.RootElement.TryGetProperty("permissoes", out var permissoes));
        Assert.Equal(JsonValueKind.Array, permissoes.ValueKind);
        Assert.Contains(permissoes.EnumerateArray().Select(x => x.GetString()), x => x == "Chamados.Assumir");
    }

    [Fact]
    public async Task PortalChamadosMantemIsolamentoPorSolicitante()
    {
        const string solicitanteA = "solicitante.a@empresa.com";
        const string solicitanteB = "solicitante.b@empresa.com";

        using (var clienteA = _factory.CreateClient())
        {
            AddDevHeaders(clienteA, solicitanteA, "Solicitante A", "Solicitante");
            _ = await clienteA.GetAsync("/api/me");
        }

        using (var clienteB = _factory.CreateClient())
        {
            AddDevHeaders(clienteB, solicitanteB, "Solicitante B", "Solicitante");
            _ = await clienteB.GetAsync("/api/me");
        }

        await _factory.SeedPortalChamadosAsync(solicitanteA, solicitanteB);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, solicitanteA, "Solicitante A", "Solicitante");
        var response = await client.GetAsync("/api/portal/chamados");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("SGX-2026-900001", content);
        Assert.DoesNotContain("SGX-2026-900002", content);
    }

    [Fact]
    public async Task AdminChamadosBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.admin@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/chamados");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminDashboardBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.dashboard@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosMutacaoBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.cadastro@empresa.com", "Solicitante", "Solicitante");

        var response = await client.PostAsJsonAsync("/api/admin/cadastros/departamentos", new
        {
            nome = "Departamento Teste",
            sigla = "DTT",
            descricao = "Departamento de teste"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoEmailLogsBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.email@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/integracoes/email/logs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoEmailLogsPermiteAtendente()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.email@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/integracoes/email/logs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.auditoria@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/auditoria/eventos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaAtendenteSemPermissaoNaoVisualiza()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.auditoria@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/auditoria/eventos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaAdministradorVisualizaListagemEDashboard()
    {
        await SeedEventoAuditoriaApiAsync();
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.auditoria@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var listagem = await client.GetAsync("/api/admin/auditoria/eventos?modulo=Chamados&pagina=1&tamanhoPagina=10");
        var dashboard = await client.GetAsync("/api/admin/auditoria/dashboard?modulo=Chamados");

        Assert.Equal(HttpStatusCode.OK, listagem.StatusCode);
        Assert.Equal(HttpStatusCode.OK, dashboard.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaDetalheInexistenteRetornaNotFound()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.auditoria.notfound@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync($"/api/admin/auditoria/eventos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.microsoft@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/integracoes/microsoft-entra-id");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAtendenteSemPermissaoNaoVisualiza()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.microsoft@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/integracoes/microsoft-entra-id");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAdministradorAtualizaConfiguracao()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = true,
            provedorPrincipal = "Hibrido",
            loginLocalHabilitado = true,
            tenantId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
            audience = "api://sgx-api",
            issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            apiScope = "api://sgx-api/access_as_user",
            redirectUri = "http://localhost:5173",
            dominiosPermitidos = new[] { "empresa.com" },
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        Assert.Equal("Hibrido", json.RootElement.GetProperty("provedorPrincipal").GetString());
        Assert.True(json.RootElement.GetProperty("habilitado").GetBoolean());
        Assert.True(json.RootElement.GetProperty("loginLocalHabilitado").GetBoolean());
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemTenantQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.rejeita@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = true,
            provedorPrincipal = "MicrosoftEntraId",
            loginLocalHabilitado = false,
            tenantId = "",
            clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
            audience = "api://sgx-api",
            issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            apiScope = "api://sgx-api/access_as_user",
            redirectUri = "http://localhost:5173",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Tenant ID é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemClientIdQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semclient@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(clientId: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Client ID é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemAudienceQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semaudience@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(audience: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Audience é obrigatória quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemIssuerQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semissuer@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(issuer: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Issuer é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemAuthorityQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semauthority@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(authority: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Authority é obrigatória quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemApiScopeQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semscope@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(apiScope: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "API Scope é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemRedirectUriQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semredirect@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(redirectUri: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Redirect URI é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAceitaModoLocalSemCamposMicrosoft()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.local@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(
            habilitado: false,
            provedorPrincipal: "Local",
            loginLocalHabilitado: true,
            tenantId: "",
            clientId: "",
            audience: "",
            issuer: "",
            authority: "",
            apiScope: "",
            redirectUri: "");

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaModoLocalSemLoginLocalHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.localsemlocal@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(
            habilitado: false,
            provedorPrincipal: "Local",
            loginLocalHabilitado: false,
            tenantId: "",
            clientId: "",
            audience: "",
            issuer: "",
            authority: "",
            apiScope: "",
            redirectUri: "");

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemProvedorAtivo()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semprovedor@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = false,
            provedorPrincipal = "MicrosoftEntraId",
            loginLocalHabilitado = false,
            tenantId = "",
            clientId = "",
            audience = "",
            issuer = "",
            authority = "",
            apiScope = "",
            redirectUri = "",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = false,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Ao menos um provedor de autenticação deve permanecer habilitado.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminSlaPoliciesBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.sla@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/sla/policies");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminSlaPoliciesPermiteAtendenteVisualizar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.sla@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/sla/policies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminSlaPoliciesAdministradorCriaPolitica()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.sla@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            nome = "SLA Infraestrutura",
            descricao = "Politica de SLA para infraestrutura.",
            ativo = true,
            ordem = 20,
            categoriaId = (string?)null,
            departamentoId = (string?)null,
            usarHorarioComercial = false,
            pausarQuandoAguardandoSolicitante = true,
            metas = new[]
            {
                new
                {
                    prioridadeId = SeedData.PrioridadeBaixaId,
                    tempoPrimeiraRespostaMinutos = 480,
                    tempoResolucaoMinutos = 2880,
                    tempoAtualizacaoMinutos = (int?)null,
                    tempoRespostaSubsequenteMinutos = (int?)null,
                    ativo = true
                },
                new
                {
                    prioridadeId = SeedData.PrioridadeMediaId,
                    tempoPrimeiraRespostaMinutos = 240,
                    tempoResolucaoMinutos = 1440,
                    tempoAtualizacaoMinutos = (int?)null,
                    tempoRespostaSubsequenteMinutos = (int?)null,
                    ativo = true
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/admin/sla/policies", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosPermissoesListaComSucessoParaAtendente()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.permissoes@empresa.com", "Atendente", "Atendente");

        var response = await client.GetAsync("/api/admin/cadastros/permissoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosPermissoesPerfilRetornaComSucesso()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.perfil@empresa.com", "Atendente", "Atendente");

        var response = await client.GetAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AtendenteNaoAtualizaPermissoesDePerfil()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.put.permissao@empresa.com", "Atendente", "Atendente");

        var response = await client.PutAsJsonAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes", new
        {
            codigosPermissoes = new[] { "Chamados.Visualizar", "Chamados.Assumir" }
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorAtualizaPermissoesDePerfil()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.put.permissao@empresa.com", "Administrador", "Administrador");

        var response = await client.PutAsJsonAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes", new
        {
            codigosPermissoes = new[] { "Chamados.Visualizar", "Chamados.Assumir", "Dashboard.Visualizar" }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorRedefineSenhaDeUsuario()
    {
        const string email = "usuario.redefinir@empresa.com";
        const string senhaInicial = "Senha@123456";
        const string senhaNova = "NovaSenha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Redefinir", senhaInicial);

        using var clientAdmin = _factory.CreateClient();
        AddDevHeaders(clientAdmin, "admin.redefinir@empresa.com", "Administrador", "Administrador");
        _ = await clientAdmin.GetAsync("/api/me");
        var usuarioId = await ObterUsuarioIdPorEmail(email);

        var redefinirResponse = await clientAdmin.PostAsJsonAsync($"/api/admin/cadastros/usuarios/{usuarioId}/redefinir-senha", new
        {
            novaSenha = senhaNova,
            confirmarNovaSenha = senhaNova,
            deveAlterarSenha = true
        });

        Assert.Equal(HttpStatusCode.OK, redefinirResponse.StatusCode);

        using var clientLogin = _factory.CreateClient();
        var loginNovo = await clientLogin.PostAsJsonAsync("/api/auth/local/login", new { email, senha = senhaNova });
        Assert.Equal(HttpStatusCode.OK, loginNovo.StatusCode);
    }

    [Fact]
    public async Task AtendenteNaoPodeRedefinirSenhaDeUsuario()
    {
        const string email = "usuario.redefinir2@empresa.com";
        const string senhaInicial = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Redefinir 2", senhaInicial);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.redefinir@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");
        var usuarioId = await ObterUsuarioIdPorEmail(email);

        var response = await client.PostAsJsonAsync($"/api/admin/cadastros/usuarios/{usuarioId}/redefinir-senha", new
        {
            novaSenha = "NovaSenha@123456",
            confirmarNovaSenha = "NovaSenha@123456",
            deveAlterarSenha = true
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task HealthRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(payload));
    }

    [Fact]
    public async Task HealthReadyRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthLiveRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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

    private async Task<Guid> ObterUsuarioIdPorEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var usuario = await dbContext.Usuarios.FirstAsync(x => x.Email == email);
        return usuario.Id;
    }

    private static async Task<string?> ObterMensagemErroAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        using var json = JsonDocument.Parse(payload);
        return json.RootElement.TryGetProperty("mensagem", out var mensagem)
            ? mensagem.GetString()
            : null;
    }

    private static object CriarPayloadMicrosoftValido(
        bool habilitado = true,
        string provedorPrincipal = "MicrosoftEntraId",
        bool loginLocalHabilitado = false,
        string tenantId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
        string clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
        string audience = "api://sgx-api",
        string issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
        string authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
        string apiScope = "api://sgx-api/access_as_user",
        string redirectUri = "http://localhost:5173")
    {
        return new
        {
            habilitado,
            provedorPrincipal,
            loginLocalHabilitado,
            tenantId,
            clientId,
            audience,
            issuer,
            authority,
            apiScope,
            redirectUri,
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };
    }

    private async Task SeedEventoAuditoriaApiAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        dbContext.EventosAuditoria.Add(new EventoAuditoria(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Admin Auditoria",
            "admin.auditoria@empresa.com",
            "admin.auditoria@empresa.com",
            "127.0.0.1",
            "integration-test",
            "Chamados",
            "Chamado",
            "SGX-INT-1",
            TipoAcaoAuditoria.AlteracaoStatus,
            "Status do chamado alterado em teste de integração.",
            "{\"statusAnterior\":\"Aberto\"}",
            "{\"statusNovo\":\"EmAtendimento\"}",
            "{\"origem\":\"api\"}",
            NivelAuditoria.Informacao,
            true,
            null,
            "corr-int-1"));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
