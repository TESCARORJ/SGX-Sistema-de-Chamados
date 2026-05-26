using Microsoft.Extensions.Options;

namespace SGX.SistemaChamado.Api.Options;

public sealed class AzureAdOptionsValidator(
    IOptions<AuthOptions> authOptions) : IValidateOptions<AzureAdOptions>
{
    public ValidateOptionsResult Validate(string? name, AzureAdOptions options)
    {
        var auth = authOptions.Value;
        var microsoftHabilitado = auth.ObterCodigosProvedoresHabilitadosNormalizados()
            .Contains(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparer.OrdinalIgnoreCase);

        if (!microsoftHabilitado && !auth.UsaMicrosoftComoPrincipalOuHibrido())
        {
            return ValidateOptionsResult.Success;
        }

        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Instance))
        {
            erros.Add("AzureAd:Instance nao configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.TenantId))
        {
            erros.Add("AzureAd:TenantId nao configurado.");
        }
        else
        {
            var tenantId = options.TenantId.Trim();
            if (tenantId.Equals("common", StringComparison.OrdinalIgnoreCase)
                || tenantId.Equals("organizations", StringComparison.OrdinalIgnoreCase)
                || tenantId.Equals("consumers", StringComparison.OrdinalIgnoreCase))
            {
                erros.Add("AzureAd:TenantId deve ser tenant unico explicito para modo Single Tenant.");
            }
        }

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            erros.Add("AzureAd:ClientId nao configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            erros.Add("AzureAd:Audience nao configurado.");
        }

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            erros.Add("AzureAd:Issuer nao configurado.");
        }
        else if (!string.IsNullOrWhiteSpace(options.TenantId))
        {
            var issuer = options.Issuer.Trim().TrimEnd('/');
            var tenantId = options.TenantId.Trim().TrimEnd('/');
            if (!issuer.Contains($"/{tenantId}/", StringComparison.OrdinalIgnoreCase)
                && !issuer.EndsWith($"/{tenantId}", StringComparison.OrdinalIgnoreCase))
            {
                erros.Add("AzureAd:Issuer deve corresponder ao TenantId configurado.");
            }
        }

        var metadataAddress = options.BuildMetadataAddress();
        if (!string.IsNullOrWhiteSpace(metadataAddress)
            && !Uri.TryCreate(metadataAddress, UriKind.Absolute, out _))
        {
            erros.Add("AzureAd:MetadataAddress invalido.");
        }

        return erros.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(erros);
    }
}
