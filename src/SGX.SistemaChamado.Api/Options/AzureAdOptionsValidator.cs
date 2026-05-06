using Microsoft.Extensions.Options;

namespace SGX.SistemaChamado.Api.Options;

public sealed class AzureAdOptionsValidator(
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions) : IValidateOptions<AzureAdOptions>
{
    public ValidateOptionsResult Validate(string? name, AzureAdOptions options)
    {
        if (environment.IsDevelopment() && authOptions.Value.ModoLocalHabilitado)
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

        return erros.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(erros);
    }
}
