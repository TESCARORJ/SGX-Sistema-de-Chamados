using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;

namespace SGX.SistemaChamado.Tests;

public sealed class AzureAdOptionsValidatorTests
{
    [Fact]
    public void DeveFalharQuandoProvedorPrincipalExigeMicrosoftEAzureAdNaoConfigurado()
    {
        var validator = new AzureAdOptionsValidator(
            Options.Create(new AuthOptions { ProvedorPrincipal = ProvedorAutenticacao.MicrosoftEntraId }));

        var resultado = validator.Validate(null, new AzureAdOptions());

        Assert.True(resultado.Failed);
    }

    [Fact]
    public void DevePermitirQuandoProvedorPrincipalNaoExigeMicrosoft()
    {
        var validator = new AzureAdOptionsValidator(
            Options.Create(new AuthOptions
            {
                ProvedorPrincipal = ProvedorAutenticacao.Local,
                LoginLocalHabilitado = true
            }));

        var resultado = validator.Validate(null, new AzureAdOptions());

        Assert.True(resultado.Succeeded);
    }

    [Fact]
    public void DeveFalharQuandoMetadataAddressForInvalido()
    {
        var validator = new AzureAdOptionsValidator(
            Options.Create(new AuthOptions { ProvedorPrincipal = ProvedorAutenticacao.Hibrido, LoginLocalHabilitado = true }));

        var resultado = validator.Validate(null, new AzureAdOptions
        {
            Instance = "https://login.microsoftonline.com/",
            TenantId = "tenant",
            ClientId = "client",
            Audience = "api://sgx",
            Issuer = "https://login.microsoftonline.com/tenant/v2.0",
            MetadataAddress = "not-an-uri"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("MetadataAddress", StringComparison.Ordinal));
    }

    [Fact]
    public void DeveFalharQuandoTenantIdForCommon()
    {
        var validator = new AzureAdOptionsValidator(
            Options.Create(new AuthOptions { ProvedorPrincipal = ProvedorAutenticacao.MicrosoftEntraId }));

        var resultado = validator.Validate(null, new AzureAdOptions
        {
            Instance = "https://login.microsoftonline.com/",
            TenantId = "common",
            ClientId = "client",
            Audience = "api://sgx",
            Issuer = "https://login.microsoftonline.com/common/v2.0"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("Single Tenant", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DeveFalharQuandoIssuerNaoCorrespondeAoTenantId()
    {
        var validator = new AzureAdOptionsValidator(
            Options.Create(new AuthOptions { ProvedorPrincipal = ProvedorAutenticacao.MicrosoftEntraId }));

        var resultado = validator.Validate(null, new AzureAdOptions
        {
            Instance = "https://login.microsoftonline.com/",
            TenantId = "11111111-1111-1111-1111-111111111111",
            ClientId = "client",
            Audience = "api://sgx",
            Issuer = "https://login.microsoftonline.com/22222222-2222-2222-2222-222222222222/v2.0"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("Issuer", StringComparison.Ordinal));
    }
}
