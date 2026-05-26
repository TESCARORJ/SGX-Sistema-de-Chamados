using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ProvedoresAutenticacaoServiceTests
{
    [Fact]
    public async Task DeveRetornarSomenteProvedoresHabilitadosComPrincipalEOrdem()
    {
        await using var dbContext = CriarDbContext();
        var authOptions = CriarAuthOptionsBase(
            provedorPrincipal: ProvedorAutenticacao.Local,
            loginLocalHabilitado: true,
            modoLocalHabilitado: true,
            provedores: new ProvedoresAutenticacaoOptions
            {
                Configurados =
                [
                    CodigoProvedorAutenticacao.MicrosoftEntraId,
                    CodigoProvedorAutenticacao.ActiveDirectory,
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.LocalDevelopment
                ],
                Habilitados =
                [
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.LocalDevelopment
                ],
                Principal = CodigoProvedorAutenticacao.LocalSgx,
                Ordem = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    [CodigoProvedorAutenticacao.LocalSgx] = 5,
                    [CodigoProvedorAutenticacao.LocalDevelopment] = 15
                }
            });

        var service = CriarService(dbContext, authOptions, CriarAzureAdOptions(), "Development");
        var response = await service.ObterProvedoresAutenticacaoAsync();

        Assert.Equal(2, response.Provedores.Count);
        Assert.Contains(response.Provedores, x => x.Codigo == CodigoProvedorAutenticacao.LocalSgx && x.Principal && x.Ordem == 5);
        Assert.Contains(response.Provedores, x => x.Codigo == CodigoProvedorAutenticacao.LocalDevelopment && !x.Principal && x.Ordem == 15);
        Assert.DoesNotContain(response.Provedores, x => x.Codigo == CodigoProvedorAutenticacao.ActiveDirectory);
    }

    [Fact]
    public async Task DeveOcultarLocalDevelopmentForaDoAmbienteDevelopment()
    {
        await using var dbContext = CriarDbContext();
        var authOptions = CriarAuthOptionsBase(
            provedorPrincipal: ProvedorAutenticacao.Local,
            loginLocalHabilitado: true,
            modoLocalHabilitado: true,
            provedores: new ProvedoresAutenticacaoOptions
            {
                Configurados =
                [
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.LocalDevelopment
                ],
                Habilitados =
                [
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.LocalDevelopment
                ],
                Principal = CodigoProvedorAutenticacao.LocalSgx
            });

        var service = CriarService(dbContext, authOptions, CriarAzureAdOptions(), "Production");
        var response = await service.ObterProvedoresAutenticacaoAsync();

        Assert.Single(response.Provedores);
        Assert.Equal(CodigoProvedorAutenticacao.LocalSgx, response.Provedores.Single().Codigo);
    }

    [Fact]
    public async Task DeveRetornarMicrosoftQuandoConfiguradoEHabilitadoComCredenciaisCompletas()
    {
        await using var dbContext = CriarDbContext();
        dbContext.ParametrosSistema.Add(new ParametroSistema(
            "auth.microsoft.api_scope",
            "api://sgx-api/access_as_user",
            "scope",
            false,
            "test"));
        dbContext.ParametrosSistema.Add(new ParametroSistema(
            "auth.microsoft.redirect_uri",
            "http://localhost:5173",
            "redirect",
            false,
            "test"));
        await dbContext.SaveChangesAsync();

        var authOptions = CriarAuthOptionsBase(
            provedorPrincipal: ProvedorAutenticacao.MicrosoftEntraId,
            loginLocalHabilitado: false,
            modoLocalHabilitado: false,
            provedores: new ProvedoresAutenticacaoOptions
            {
                Configurados = [CodigoProvedorAutenticacao.MicrosoftEntraId],
                Habilitados = [CodigoProvedorAutenticacao.MicrosoftEntraId],
                Principal = CodigoProvedorAutenticacao.MicrosoftEntraId
            });

        var service = CriarService(dbContext, authOptions, CriarAzureAdOptions(), "Development");
        var response = await service.ObterProvedoresAutenticacaoAsync();

        Assert.Single(response.Provedores);
        var provedor = response.Provedores.Single();
        Assert.Equal(CodigoProvedorAutenticacao.MicrosoftEntraId, provedor.Codigo);
        Assert.True(provedor.Principal);
    }

    private static SGXSistemaChamadoDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase($"sgx-provedores-{Guid.NewGuid():N}")
            .Options;
        return new SGXSistemaChamadoDbContext(options);
    }

    private static ConfiguracaoIntegracaoMicrosoftService CriarService(
        SGXSistemaChamadoDbContext dbContext,
        AuthOptions authOptions,
        AzureAdOptions azureAdOptions,
        string environmentName)
    {
        return new ConfiguracaoIntegracaoMicrosoftService(
            dbContext,
            Options.Create(authOptions),
            Options.Create(azureAdOptions),
            new FakeEnvironment(environmentName));
    }

    private static AuthOptions CriarAuthOptionsBase(
        string provedorPrincipal,
        bool loginLocalHabilitado,
        bool modoLocalHabilitado,
        ProvedoresAutenticacaoOptions provedores)
    {
        return new AuthOptions
        {
            ProvedorPrincipal = provedorPrincipal,
            LoginLocalHabilitado = loginLocalHabilitado,
            ModoLocalHabilitado = modoLocalHabilitado,
            Provedores = provedores,
            JwtLocalIssuer = "SGX.Local",
            JwtLocalAudience = "SGX.Api",
            JwtLocalChaveAssinatura = "sgx-chave-local-super-segura-com-32-caracteres",
            JwtLocalExpiracaoMinutos = 120
        };
    }

    private static AzureAdOptions CriarAzureAdOptions()
    {
        return new AzureAdOptions
        {
            Instance = "https://login.microsoftonline.com/",
            TenantId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            ClientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
            Audience = "api://sgx-api",
            Issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0"
        };
    }

    private sealed class FakeEnvironment(string environmentName) : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
