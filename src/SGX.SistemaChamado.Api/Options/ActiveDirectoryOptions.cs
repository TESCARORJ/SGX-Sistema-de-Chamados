namespace SGX.SistemaChamado.Api.Options;

public sealed class ActiveDirectoryOptions
{
    public const string SectionName = "ActiveDirectory";

    public string Servidor { get; init; } = string.Empty;
    public int Porta { get; init; } = 636;
    public bool UsarLdaps { get; init; } = true;
    public bool PermitirLdapSemTls { get; init; }
    public string Dominio { get; init; } = string.Empty;
    public string BaseDn { get; init; } = string.Empty;
    public string UserSearchFilter { get; init; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public bool PermitirAutoProvisionamento { get; init; }
    public string PerfilPadrao { get; init; } = "Solicitante";

    public bool EstaConfigurado()
    {
        return !string.IsNullOrWhiteSpace(Servidor)
            && Porta > 0
            && !string.IsNullOrWhiteSpace(BaseDn)
            && !string.IsNullOrWhiteSpace(UserSearchFilter);
    }
}
