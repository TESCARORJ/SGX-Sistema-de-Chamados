using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class InventarioAtivosAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    public static TheoryData<string, string> EndpointsVisualizacao => new()
    {
        { "GET", "/api/admin/inventario-ativos" },
        { "GET", "/api/admin/inventario-ativos/tipos" },
        { "GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111" },
        { "GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/historico" },
        { "GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/chamados" }
    };

    public static TheoryData<string, string> EndpointsGerenciamento => new()
    {
        { "POST", "/api/admin/inventario-ativos" },
        { "PUT", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111" }
    };

    public static TheoryData<string, string> EndpointsInativacao => new()
    {
        { "POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/inativar" },
        { "POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/reativar" }
    };

    public static TheoryData<string, string> EndpointsMovimentacao => new()
    {
        { "POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/movimentar" }
    };

    public static TheoryData<string, string> EndpointsVinculoChamado => new()
    {
        { "POST", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo/22222222-2222-2222-2222-222222222222" },
        { "DELETE", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo" }
    };

    private readonly ApiIntegrationTestFactory _factory;

    public InventarioAtivosAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("GET", "/api/admin/inventario-ativos")]
    [InlineData("GET", "/api/admin/inventario-ativos/tipos")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/historico")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/chamados")]
    [InlineData("POST", "/api/admin/inventario-ativos")]
    [InlineData("PUT", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/movimentar")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/inativar")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/reativar")]
    [InlineData("POST", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo/22222222-2222-2222-2222-222222222222")]
    [InlineData("DELETE", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo")]
    public async Task SolicitanteNaoAcessaEndpointsAdministrativos(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.inventario.{Guid.NewGuid():N}@empresa.com", "Solicitante Inventario", "Solicitante");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/admin/inventario-ativos")]
    [InlineData("GET", "/api/admin/inventario-ativos/tipos")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/historico")]
    [InlineData("GET", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/chamados")]
    [InlineData("POST", "/api/admin/inventario-ativos")]
    [InlineData("PUT", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/movimentar")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/inativar")]
    [InlineData("POST", "/api/admin/inventario-ativos/11111111-1111-1111-1111-111111111111/reativar")]
    [InlineData("POST", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo/22222222-2222-2222-2222-222222222222")]
    [InlineData("DELETE", "/api/admin/chamados/11111111-1111-1111-1111-111111111111/ativo")]
    public async Task AdministradorNaoRecebeForbiddenNosEndpoints(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.inventario.{Guid.NewGuid():N}@empresa.com", "Admin Inventario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EndpointsVisualizacao))]
    public async Task AtendenteSemPermissaoVisualizarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosVisualizar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.sem.visualizar.inventario.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Visualizar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosVisualizar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsGerenciamento))]
    public async Task AtendenteSemPermissaoGerenciarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosGerenciar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.sem.gerenciar.inventario.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerenciar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosGerenciar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsInativacao))]
    public async Task AtendenteSemPermissaoInativarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosInativar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.sem.inativar.inventario.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Inativar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosInativar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsMovimentacao))]
    public async Task AtendenteSemPermissaoMovimentarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosMovimentar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.sem.movimentar.inventario.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Movimentar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosMovimentar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsVinculoChamado))]
    public async Task AtendenteSemPermissaoVincularChamadoEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosVincularChamado, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.sem.vinculo.inventario.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Vinculo", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.InventarioAtivosVincularChamado, true);
        }
    }

    private async Task DefinirPermissaoAtivaAsync(string codigoPermissao, bool ativa, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var permissao = await dbContext.PermissoesSistema
            .FirstAsync(x => x.Codigo == codigoPermissao, cancellationToken);

        if (permissao.Ativo == ativa)
        {
            return;
        }

        if (ativa)
        {
            permissao.Ativar("integration-test");
        }
        else
        {
            permissao.Desativar("integration-test");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Task<HttpResponseMessage> EnviarAsync(HttpClient client, string metodo, string endpoint)
    {
        return metodo switch
        {
            "GET" => client.GetAsync(endpoint),
            "POST" when endpoint.EndsWith("/inventario-ativos", StringComparison.OrdinalIgnoreCase) =>
                client.PostAsJsonAsync(endpoint, new
                {
                    codigo = "INV-HTTP-001",
                    nome = "Ativo HTTP",
                    tipoAtivoInventarioId = "11111111-1111-1111-1111-111111111111"
                }),
            "PUT" => client.PutAsJsonAsync(endpoint, new
            {
                codigo = "INV-HTTP-001",
                nome = "Ativo HTTP atualizado",
                tipoAtivoInventarioId = "11111111-1111-1111-1111-111111111111",
                statusOperacional = 0,
                statusPatrimonial = 0,
                criticidade = 1
            }),
            "POST" when endpoint.EndsWith("/movimentar", StringComparison.OrdinalIgnoreCase) =>
                client.PostAsJsonAsync(endpoint, new
                {
                    statusOperacional = 1,
                    observacao = "Movimentacao de teste"
                }),
            "DELETE" => client.DeleteAsync(endpoint),
            "POST" => client.PostAsync(endpoint, null),
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
