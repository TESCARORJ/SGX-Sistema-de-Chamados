namespace SGX.SistemaChamado.Infrastructure.Email;

public sealed class EmailOutboundOptions
{
    public const string SectionName = "EmailOutbound";

    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string RemetenteEndereco { get; init; } = string.Empty;
    public string RemetenteNome { get; init; } = string.Empty;
    public string? Usuario { get; init; }
    public string? Senha { get; init; }
    public bool UsarSsl { get; init; }
    public bool Habilitado { get; init; }
}
