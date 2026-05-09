using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
}
