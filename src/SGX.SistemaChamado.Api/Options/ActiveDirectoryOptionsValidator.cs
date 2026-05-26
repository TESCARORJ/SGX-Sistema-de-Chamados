using Microsoft.Extensions.Options;

namespace SGX.SistemaChamado.Api.Options;

public sealed class ActiveDirectoryOptionsValidator(
    IOptions<AuthOptions> authOptions) : IValidateOptions<ActiveDirectoryOptions>
{
    public ValidateOptionsResult Validate(string? name, ActiveDirectoryOptions options)
    {
        var adHabilitado = authOptions.Value.ObterCodigosProvedoresHabilitadosNormalizados()
            .Contains(CodigoProvedorAutenticacao.ActiveDirectory, StringComparer.OrdinalIgnoreCase);

        if (!adHabilitado)
        {
            return ValidateOptionsResult.Success;
        }

        var erros = new List<string>();
        var servidor = (options.Servidor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(servidor))
        {
            erros.Add("ActiveDirectory:Servidor nao configurado.");
        }
        else if (Uri.TryCreate(servidor, UriKind.Absolute, out var uri))
        {
            if (!string.Equals(uri.Scheme, "ldap", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(uri.Scheme, "ldaps", StringComparison.OrdinalIgnoreCase))
            {
                erros.Add("ActiveDirectory:Servidor deve usar esquema ldap:// ou ldaps://.");
            }
        }
        else if (servidor.Contains("://", StringComparison.Ordinal))
        {
            erros.Add("ActiveDirectory:Servidor invalido.");
        }

        if (options.Porta <= 0 || options.Porta > 65535)
        {
            erros.Add("ActiveDirectory:Porta deve estar entre 1 e 65535.");
        }

        if (!options.UsarLdaps && !options.PermitirLdapSemTls)
        {
            erros.Add("ActiveDirectory:PermitirLdapSemTls deve ser true para habilitar LDAP sem TLS.");
        }

        if (string.IsNullOrWhiteSpace(options.BaseDn))
        {
            erros.Add("ActiveDirectory:BaseDn nao configurado.");
        }

        var filtro = (options.UserSearchFilter ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(filtro))
        {
            erros.Add("ActiveDirectory:UserSearchFilter nao configurado.");
        }
        else if (!filtro.Contains("{0}", StringComparison.Ordinal))
        {
            erros.Add("ActiveDirectory:UserSearchFilter deve conter o placeholder {0}.");
        }

        if (options.PermitirAutoProvisionamento && string.IsNullOrWhiteSpace(options.PerfilPadrao))
        {
            erros.Add("ActiveDirectory:PerfilPadrao nao configurado para auto provisionamento.");
        }

        return erros.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(erros);
    }
}
