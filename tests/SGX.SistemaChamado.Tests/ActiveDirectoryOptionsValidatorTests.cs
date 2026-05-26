using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;

namespace SGX.SistemaChamado.Tests;

public sealed class ActiveDirectoryOptionsValidatorTests
{
    [Fact]
    public void DeveFalharQuandoProvedorAdHabilitadoSemServidor()
    {
        var validator = new ActiveDirectoryOptionsValidator(
            Options.Create(CriarAuthComAdHabilitado()));

        var resultado = validator.Validate(null, new ActiveDirectoryOptions
        {
            Servidor = "",
            Porta = 636,
            UsarLdaps = true,
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("Servidor", StringComparison.Ordinal));
    }

    [Fact]
    public void DeveFalharQuandoLdapSemTlsNaoFoiExplicitamentePermitido()
    {
        var validator = new ActiveDirectoryOptionsValidator(
            Options.Create(CriarAuthComAdHabilitado()));

        var resultado = validator.Validate(null, new ActiveDirectoryOptions
        {
            Servidor = "ldap://dc01.empresa.local",
            Porta = 389,
            UsarLdaps = false,
            PermitirLdapSemTls = false,
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
        });

        Assert.True(resultado.Failed);
        Assert.Contains(resultado.Failures!, x => x.Contains("PermitirLdapSemTls", StringComparison.Ordinal));
    }

    [Fact]
    public void DevePermitirConfiguracaoLdapSemTlsQuandoExplicitamentePermitida()
    {
        var validator = new ActiveDirectoryOptionsValidator(
            Options.Create(CriarAuthComAdHabilitado()));

        var resultado = validator.Validate(null, new ActiveDirectoryOptions
        {
            Servidor = "ldap://dc01.empresa.local",
            Porta = 389,
            UsarLdaps = false,
            PermitirLdapSemTls = true,
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
        });

        Assert.True(resultado.Succeeded);
    }

    [Fact]
    public void DevePermitirQuandoProvedorAdNaoEstaHabilitado()
    {
        var validator = new ActiveDirectoryOptionsValidator(
            Options.Create(new AuthOptions()));

        var resultado = validator.Validate(null, new ActiveDirectoryOptions());

        Assert.True(resultado.Succeeded);
    }

    private static AuthOptions CriarAuthComAdHabilitado()
    {
        return new AuthOptions
        {
            Provedores = new ProvedoresAutenticacaoOptions
            {
                Configurados = [CodigoProvedorAutenticacao.ActiveDirectory],
                Habilitados = [CodigoProvedorAutenticacao.ActiveDirectory],
                Principal = CodigoProvedorAutenticacao.ActiveDirectory
            }
        };
    }
}
