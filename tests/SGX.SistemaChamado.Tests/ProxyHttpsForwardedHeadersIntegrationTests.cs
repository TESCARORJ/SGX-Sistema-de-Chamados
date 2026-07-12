using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ProxyHttpsForwardedHeadersIntegrationTests : IClassFixture<ApiProductionProxyIntegrationTestFactory>
{
    private readonly ApiProductionProxyIntegrationTestFactory _factory;

    public ProxyHttpsForwardedHeadersIntegrationTests(ApiProductionProxyIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthLiveComXForwardedProtoHttpsNaoRedireciona()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });

        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.TryAddWithoutValidation("X-Forwarded-Proto", "https");
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", "203.0.113.10");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthLiveHttpDiretoMantemRedirecionamentoHttps()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("https", response.Headers.Location!.Scheme);
        Assert.Equal("/health/live", response.Headers.Location.AbsolutePath);
    }

    [Fact]
    public async Task HealthLivePermaneceAcessivelEmHttpsEfetivo()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class ApiProductionProxyIntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"sgx-production-proxy-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseSetting("https_port", "8443");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=sgx_dummy;Username=sgx;Password=sgx",
                ["Authentication:ProvedorPrincipal"] = "Local",
                ["Authentication:LoginLocalHabilitado"] = "true",
                ["Authentication:ModoLocalHabilitado"] = "false",
                ["Authentication:Provedores:Configurados:0"] = "LocalSgx",
                ["Authentication:Provedores:Habilitados:0"] = "LocalSgx",
                ["Authentication:Provedores:Principal"] = "LocalSgx",
                ["Authentication:Provedores:Ordem:LocalSgx"] = "10",
                ["Authentication:AdminLocalEmail"] = "admin.local@sgx.local",
                ["Authentication:AdminLocalNome"] = "Administrador Local",
                ["Authentication:JwtLocalIssuer"] = "SGX.Local.Testes",
                ["Authentication:JwtLocalAudience"] = "SGX.SistemaChamado.Api",
                ["Authentication:JwtLocalChaveAssinatura"] = "sgx-testes-login-local-chave-com-minimo-32-caracteres",
                ["Authentication:JwtLocalExpiracaoMinutos"] = "120",
                ["ActiveDirectory:Servidor"] = "ldaps://dc01.empresa.local",
                ["ActiveDirectory:Porta"] = "636",
                ["ActiveDirectory:UsarLdaps"] = "true",
                ["ActiveDirectory:PermitirLdapSemTls"] = "false",
                ["ActiveDirectory:Dominio"] = "EMPRESA",
                ["ActiveDirectory:BaseDn"] = "DC=empresa,DC=local",
                ["ActiveDirectory:UserSearchFilter"] = "(&(objectClass=user)(sAMAccountName={0}))",
                ["ActiveDirectory:PermitirAutoProvisionamento"] = "false",
                ["ActiveDirectory:PerfilPadrao"] = "Solicitante",
                ["Swagger:EnableInNonDevelopment"] = "false",
                ["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true",
                ["HTTPS_PORT"] = "8443"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<SGXSistemaChamadoDbContext>));
            services.RemoveAll(typeof(SGXSistemaChamadoDbContext));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<SGXSistemaChamadoDbContext>));

            services.AddDbContext<SGXSistemaChamadoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }
}
