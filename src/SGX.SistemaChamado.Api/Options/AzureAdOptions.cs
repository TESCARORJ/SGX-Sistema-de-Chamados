namespace SGX.SistemaChamado.Api.Options;

public sealed class AzureAdOptions
{
    public const string SectionName = "AzureAd";

    public string Instance { get; init; } = "https://login.microsoftonline.com/";
    public string TenantId { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;

    public string BuildAuthority()
    {
        var instance = (Instance ?? string.Empty).Trim();
        if (!instance.EndsWith('/'))
        {
            instance += "/";
        }

        return string.IsNullOrWhiteSpace(TenantId)
            ? instance
            : $"{instance}{TenantId}/v2.0";
    }
}
