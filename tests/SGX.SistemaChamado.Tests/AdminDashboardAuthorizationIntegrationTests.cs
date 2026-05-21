using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AdminDashboardAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    public static TheoryData<string> EndpointsDashboard => new()
    {
        "/api/admin/dashboard",
        "/api/admin/indicadores/chamados-por-status",
        "/api/admin/indicadores/chamados-por-prioridade",
        "/api/admin/indicadores/chamados-por-categoria",
        "/api/admin/indicadores/sla",
        "/api/admin/indicadores/produtividade"
    };

    private readonly ApiIntegrationTestFactory _factory;

    public AdminDashboardAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [MemberData(nameof(EndpointsDashboard))]
    public async Task AtendenteComPermissaoDashboardVisualizarAcessaEndpoints(string endpoint)
    {
        await DefinirPermissaoDashboardVisualizarAtivaAsync(true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.dashboard.ok.{Guid.NewGuid():N}@empresa.com", "Atendente Dashboard", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EndpointsDashboard))]
    public async Task AtendenteSemPermissaoDashboardVisualizarEhBloqueado(string endpoint)
    {
        await DefinirPermissaoDashboardVisualizarAtivaAsync(false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.dashboard.semperm.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Permissao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(endpoint);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoDashboardVisualizarAtivaAsync(true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsDashboard))]
    public async Task SolicitantePermaneceSemAcessoAdministrativoAoDashboard(string endpoint)
    {
        await DefinirPermissaoDashboardVisualizarAtivaAsync(true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solicitante.dashboard.{Guid.NewGuid():N}@empresa.com", "Solicitante Dashboard", "Solicitante");

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task DefinirPermissaoDashboardVisualizarAtivaAsync(bool ativa, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var permissaoDashboard = await dbContext.PermissoesSistema
            .FirstAsync(x => x.Codigo == PermissoesConstants.DashboardVisualizar, cancellationToken);

        if (permissaoDashboard.Ativo == ativa)
        {
            return;
        }

        if (ativa)
        {
            permissaoDashboard.Ativar("integration-test");
        }
        else
        {
            permissaoDashboard.Desativar("integration-test");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
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
