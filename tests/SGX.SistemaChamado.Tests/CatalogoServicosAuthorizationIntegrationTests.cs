using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicosAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    public static TheoryData<string, string> EndpointsVisualizacao => new()
    {
        { "GET", "/api/admin/catalogo-servicos" },
        { "GET", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111" }
    };

    public static TheoryData<string, string> EndpointsGerenciamento => new()
    {
        { "POST", "/api/admin/catalogo-servicos" },
        { "PUT", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111" }
    };

    public static TheoryData<string, string> EndpointsPublicacao => new()
    {
        { "POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/publicar" }
    };

    public static TheoryData<string, string> EndpointsArquivamento => new()
    {
        { "POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/arquivar" },
        { "POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/reativar" }
    };

    private readonly ApiIntegrationTestFactory _factory;

    public CatalogoServicosAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("GET", "/api/admin/catalogo-servicos")]
    [InlineData("GET", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/catalogo-servicos")]
    [InlineData("PUT", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/publicar")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/arquivar")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/reativar")]
    public async Task SolicitanteNaoAcessaEndpointsAdministrativos(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.catalogo.{Guid.NewGuid():N}@empresa.com", "Solicitante Catalogo", "Solicitante");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/admin/catalogo-servicos")]
    [InlineData("GET", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/catalogo-servicos")]
    [InlineData("PUT", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/publicar")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/arquivar")]
    [InlineData("POST", "/api/admin/catalogo-servicos/11111111-1111-1111-1111-111111111111/reativar")]
    public async Task AdministradorNaoRecebeForbiddenNosEndpoints(string metodo, string endpoint)
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.catalogo.{Guid.NewGuid():N}@empresa.com", "Admin Catalogo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await EnviarAsync(client, metodo, endpoint);

        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(EndpointsVisualizacao))]
    public async Task AtendenteSemPermissaoVisualizarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosVisualizar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.catalogo.sem.visualizar.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Visualizar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosVisualizar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsGerenciamento))]
    public async Task AtendenteSemPermissaoGerenciarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosGerenciar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.catalogo.sem.gerenciar.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerenciar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosGerenciar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsPublicacao))]
    public async Task AtendenteSemPermissaoPublicarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosPublicar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.catalogo.sem.publicar.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Publicar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosPublicar, true);
        }
    }

    [Theory]
    [MemberData(nameof(EndpointsArquivamento))]
    public async Task AtendenteSemPermissaoArquivarEhBloqueado(string metodo, string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosArquivar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.catalogo.sem.arquivar.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Arquivar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await EnviarAsync(client, metodo, endpoint);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.CatalogoServicosArquivar, true);
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
            "POST" when endpoint.EndsWith("/catalogo-servicos", StringComparison.OrdinalIgnoreCase) =>
                client.PostAsJsonAsync(endpoint, new
                {
                    nome = "Servico HTTP",
                    descricao = "Descricao valida para criacao.",
                    departamentoResponsavelId = "11111111-1111-1111-1111-111111111111",
                    visibilidade = 2
                }),
            "PUT" => client.PutAsJsonAsync(endpoint, new
            {
                nome = "Servico HTTP atualizado",
                descricao = "Descricao valida para atualizacao.",
                departamentoResponsavelId = "11111111-1111-1111-1111-111111111111",
                visibilidade = 2,
                permiteAberturaChamado = true,
                requerAprovacao = false,
                ordem = 1,
                ativo = true
            }),
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
