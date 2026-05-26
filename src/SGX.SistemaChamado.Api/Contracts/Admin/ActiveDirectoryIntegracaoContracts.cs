namespace SGX.SistemaChamado.Api.Contracts.Admin;

public sealed record ActiveDirectoryIntegracaoResponse(
    bool Ativo,
    string Servidor,
    int Porta,
    bool UsarLdaps,
    bool PermitirLdapSemTls,
    string Dominio,
    string BaseDn,
    string UserSearchFilter,
    bool PermitirAutoProvisionamento,
    string PerfilPadrao,
    int TimeoutConexaoSegundos,
    bool TecnicamenteConfigurado,
    string StatusConfiguracao,
    IReadOnlyCollection<string> PendenciasConfiguracao,
    IReadOnlyCollection<string> AvisosSeguranca);

public sealed class AtualizarActiveDirectoryIntegracaoRequest
{
    public bool Ativo { get; init; } = true;
    public string Servidor { get; init; } = string.Empty;
    public int Porta { get; init; } = 636;
    public bool UsarLdaps { get; init; } = true;
    public bool PermitirLdapSemTls { get; init; }
    public bool ConfirmacaoPermitirLdapSemTls { get; init; }
    public string Dominio { get; init; } = string.Empty;
    public string BaseDn { get; init; } = string.Empty;
    public string UserSearchFilter { get; init; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public bool PermitirAutoProvisionamento { get; init; }
    public string PerfilPadrao { get; init; } = "Solicitante";
    public int TimeoutConexaoSegundos { get; init; } = 10;
}

public sealed class TestarConexaoActiveDirectoryRequest
{
    public bool Ativo { get; init; } = true;
    public string Servidor { get; init; } = string.Empty;
    public int Porta { get; init; } = 636;
    public bool UsarLdaps { get; init; } = true;
    public bool PermitirLdapSemTls { get; init; }
    public bool ConfirmacaoPermitirLdapSemTls { get; init; }
    public string Dominio { get; init; } = string.Empty;
    public string BaseDn { get; init; } = string.Empty;
    public string UserSearchFilter { get; init; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public bool PermitirAutoProvisionamento { get; init; }
    public string PerfilPadrao { get; init; } = "Solicitante";
    public int TimeoutConexaoSegundos { get; init; } = 10;
}

public sealed record TestarConexaoActiveDirectoryResponse(
    bool Sucesso,
    string Mensagem,
    long DuracaoMs);

public sealed class TestarAutenticacaoActiveDirectoryRequest
{
    public string Usuario { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string Dominio { get; init; } = string.Empty;
    public bool Ativo { get; init; } = true;
    public string Servidor { get; init; } = string.Empty;
    public int Porta { get; init; } = 636;
    public bool UsarLdaps { get; init; } = true;
    public bool PermitirLdapSemTls { get; init; }
    public bool ConfirmacaoPermitirLdapSemTls { get; init; }
    public string BaseDn { get; init; } = string.Empty;
    public string UserSearchFilter { get; init; } = "(&(objectClass=user)(sAMAccountName={0}))";
    public int TimeoutConexaoSegundos { get; init; } = 10;
}

public sealed record TestarAutenticacaoActiveDirectoryResponse(
    bool Sucesso,
    string Mensagem,
    string? UsuarioSamAccountName,
    string? NomeCompleto,
    string? Email,
    string? UserPrincipalName,
    long DuracaoMs);
