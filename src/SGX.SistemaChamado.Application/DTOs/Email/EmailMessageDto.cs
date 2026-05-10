namespace SGX.SistemaChamado.Application.DTOs.Email;

public sealed class EmailMessageDto
{
    public string Identificador { get; init; } = string.Empty;
    public string? MessageId { get; init; }
    public string? InReplyTo { get; init; }
    public IReadOnlyCollection<string> References { get; init; } = [];
    public string RemetenteEmail { get; init; } = string.Empty;
    public string? RemetenteNome { get; init; }
    public string? Destinatario { get; init; }
    public string Assunto { get; init; } = string.Empty;
    public string? CorpoTexto { get; init; }
    public string? CorpoHtml { get; init; }
    public DateTime DataRecebimento { get; init; }
    public IReadOnlyCollection<EmailAttachmentDto> Anexos { get; init; } = [];
}

public sealed record EmailAttachmentDto(
    string NomeArquivo,
    string ContentType,
    byte[] Conteudo,
    long TamanhoBytes,
    string? ContentId);
