using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;

namespace SGX.SistemaChamado.Tests;

public sealed class AuthOptionsValidatorTests
{
    [Fact]
    public void DeveFalharQuandoProvedorLocalComLoginLocalDesabilitado()
    {
        var validator = new AuthOptionsValidator(new FakeEnvironment { EnvironmentName = "Development" });

        var resultado = validator.Validate(null, new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = false
        });

        Assert.True(resultado.Failed);
    }

    [Fact]
    public void DeveFalharQuandoModoLocalDevelopmentHabilitadoForaDeDevelopment()
    {
        var validator = new AuthOptionsValidator(new FakeEnvironment { EnvironmentName = "Production" });

        var resultado = validator.Validate(null, new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.MicrosoftEntraId,
            ModoLocalHabilitado = true
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("ModoLocalHabilitado", StringComparison.Ordinal));
    }

    [Fact]
    public void DeveFalharQuandoLoginLocalHabilitadoSemChaveJwtValida()
    {
        var validator = new AuthOptionsValidator(new FakeEnvironment { EnvironmentName = "Development" });

        var resultado = validator.Validate(null, new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = true,
            JwtLocalIssuer = "SGX.Local",
            JwtLocalAudience = "SGX.Api",
            JwtLocalChaveAssinatura = "curta"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("JwtLocalChaveAssinatura", StringComparison.Ordinal));
    }

    [Fact]
    public void DevePermitirConfiguracaoLocalValida()
    {
        var validator = new AuthOptionsValidator(new FakeEnvironment { EnvironmentName = "Development" });

        var resultado = validator.Validate(null, new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = true,
            JwtLocalIssuer = "SGX.Local",
            JwtLocalAudience = "SGX.Api",
            JwtLocalChaveAssinatura = "sgx-chave-local-super-segura-com-32-caracteres",
            JwtLocalExpiracaoMinutos = 120
        });

        Assert.True(resultado.Succeeded);
    }

    [Fact]
    public void DeveFalharQuandoLocalDevelopmentHabilitadoForaDeDevelopmentPeloCatalogo()
    {
        var validator = new AuthOptionsValidator(new FakeEnvironment { EnvironmentName = "Production" });

        var resultado = validator.Validate(null, new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = true,
            Provedores = new ProvedoresAutenticacaoOptions
            {
                Configurados = [CodigoProvedorAutenticacao.LocalSgx, CodigoProvedorAutenticacao.LocalDevelopment],
                Habilitados = [CodigoProvedorAutenticacao.LocalSgx, CodigoProvedorAutenticacao.LocalDevelopment],
                Principal = CodigoProvedorAutenticacao.LocalSgx
            },
            JwtLocalIssuer = "SGX.Local",
            JwtLocalAudience = "SGX.Api",
            JwtLocalChaveAssinatura = "sgx-chave-local-super-segura-com-32-caracteres",
            JwtLocalExpiracaoMinutos = 120
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("LocalDevelopment", StringComparison.Ordinal));
    }

    private sealed class FakeEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
