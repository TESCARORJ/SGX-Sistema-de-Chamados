namespace SGX.SistemaChamado.Application.Interfaces.Email;

public sealed record EmailAttachmentData(
    string NomeArquivo,
    string ContentType,
    byte[] Conteudo,
    long TamanhoBytes,
    string? ContentId);

public sealed class EmailMessageData
{
    public string Identificador { get; init; } = string.Empty;
    public string? MessageId { get; init; }
    public string? InReplyTo { get; init; }
    public IReadOnlyCollection<string> References { get; init; } = [];
    public string RemetenteEmail { get; init; } = string.Empty;
    public string? Destinatario { get; init; }
    public string? RemetenteNome { get; init; }
    public string Assunto { get; init; } = string.Empty;
    public string? CorpoTexto { get; init; }
    public string? CorpoHtml { get; init; }
    public DateTime DataRecebimento { get; init; }
    public IReadOnlyCollection<EmailAttachmentData> Anexos { get; init; } = [];
}

public interface IEmailImapClient
{
    Task<IReadOnlyCollection<EmailMessageData>> LerMensagensAsync(int maxMensagens, CancellationToken cancellationToken = default);
    Task MarcarComoLidaAsync(string identificador, CancellationToken cancellationToken = default);
    Task MoverMensagemAsync(string identificador, string pastaDestino, CancellationToken cancellationToken = default);
}
