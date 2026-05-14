namespace SGX.SistemaChamado.Api.Contracts.Admin;

public sealed record MicrosoftEntraIdIntegracaoResponse(
    bool Habilitado,
    string ProvedorPrincipal,
    bool LoginLocalHabilitado,
    string TenantId,
    string ClientId,
    string Audience,
    string Issuer,
    string Authority,
    string ApiScope,
    string RedirectUri,
    IReadOnlyCollection<string> DominiosPermitidos,
    bool CriarUsuarioAutomaticamente,
    string PerfilPadraoUsuarioMicrosoft,
    string StatusConfiguracao,
    IReadOnlyCollection<string> PendenciasConfiguracao);

public sealed class AtualizarMicrosoftEntraIdIntegracaoRequest
{
    public bool Habilitado { get; init; }
    public string ProvedorPrincipal { get; init; } = string.Empty;
    public bool LoginLocalHabilitado { get; init; }
    public string TenantId { get; init; } = string.Empty;
    public string ClientId { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Authority { get; init; } = string.Empty;
    public string ApiScope { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
    public IReadOnlyCollection<string> DominiosPermitidos { get; init; } = [];
    public bool CriarUsuarioAutomaticamente { get; init; } = true;
    public string PerfilPadraoUsuarioMicrosoft { get; init; } = "Solicitante";
}

