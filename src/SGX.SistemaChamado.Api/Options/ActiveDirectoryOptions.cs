namespace SGX.SistemaChamado.Api.Options;

public sealed class ActiveDirectoryOptions
{
    public const string SectionName = "ActiveDirectory";

    public bool Ativo { get; init; } = true;
    public string Servidor { get; init; } = string.Empty;
    public int Porta { get; init; } = 636;
    public bool UsarLdaps { get; init; } = true;
    public bool PermitirLdapSemTls { get; init; }
    public string Dominio { get; init; } = string.Empty;
    public string BaseDn { get; init; } = string.Empty;
    public string UserSearchFilter { get; init; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public bool PermitirAutoProvisionamento { get; init; }
    public string PerfilPadrao { get; init; } = "Solicitante";
    public int TimeoutConexaoSegundos { get; init; } = 10;

    public bool EstaConfigurado()
    {
        return Ativo
            && !string.IsNullOrWhiteSpace(Servidor)
            && Porta > 0
            && TimeoutConexaoSegundos > 0
            && !string.IsNullOrWhiteSpace(BaseDn)
            && !string.IsNullOrWhiteSpace(UserSearchFilter);
    }
}
