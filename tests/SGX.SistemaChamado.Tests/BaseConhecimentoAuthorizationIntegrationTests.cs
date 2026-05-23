using System.Net;
using System.Net.Http.Json;

namespace SGX.SistemaChamado.Tests;

public sealed class BaseConhecimentoAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public BaseConhecimentoAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("GET", "/api/admin/base-conhecimento/artigos")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/publicar")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/arquivar")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/reativar")]
    [InlineData("GET", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento")]
    [InlineData("GET", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/disponiveis")]
    [InlineData("POST", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/22222222-2222-2222-2222-222222222222")]
    [InlineData("DELETE", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/22222222-2222-2222-2222-222222222222")]
    public async Task SolicitanteNaoAcessaEndpointsAdministrativos(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.bc.{Guid.NewGuid():N}@empresa.com", "Solicitante BC", "Solicitante");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/admin/base-conhecimento/artigos")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/publicar")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/arquivar")]
    [InlineData("POST", "/api/admin/base-conhecimento/artigos/11111111-1111-1111-1111-111111111111/reativar")]
    [InlineData("GET", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento")]
    [InlineData("GET", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/disponiveis")]
    [InlineData("POST", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/22222222-2222-2222-2222-222222222222")]
    [InlineData("DELETE", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/artigos-conhecimento/22222222-2222-2222-2222-222222222222")]
    public async Task AdministradorNaoRecebeForbiddenNosEndpoints(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.bc.{Guid.NewGuid():N}@empresa.com", "Admin BC", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static Task<HttpResponseMessage> EnviarAsync(HttpClient client, string metodo, string endpoint)
    {
        return metodo switch
        {
            "GET" => client.GetAsync(endpoint),
            "POST" when endpoint.EndsWith("/artigos", StringComparison.OrdinalIgnoreCase) =>
                client.PostAsJsonAsync(endpoint, new
                {
                    titulo = "Base de conhecimento HTTP",
                    conteudo = "Conteudo valido para criacao.",
                    visibilidade = 1
                }),
            "POST" when endpoint.Contains("/artigos-conhecimento/", StringComparison.OrdinalIgnoreCase) =>
                client.PostAsJsonAsync(endpoint, new { observacao = "Vinculo de teste" }),
            "POST" => client.PostAsync(endpoint, null),
            "DELETE" => client.DeleteAsync(endpoint),
            _ => throw new InvalidOperationException("Metodo HTTP nao suportado no teste.")
        };
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
