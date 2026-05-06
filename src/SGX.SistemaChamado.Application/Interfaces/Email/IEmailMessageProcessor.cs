namespace SGX.SistemaChamado.Application.Interfaces.Email;

public enum EmailMensagemProcessamentoStatus
{
    Processado = 1,
    IgnoradoDuplicado = 2,
    Erro = 3
}

public sealed record EmailMensagemProcessamentoResultado(
    EmailMensagemProcessamentoStatus Status,
    Guid? ChamadoId,
    string? Erro);

public interface IEmailMessageProcessor
{
    Task<EmailMensagemProcessamentoResultado> ProcessarAsync(EmailMessageData mensagem, CancellationToken cancellationToken = default);
}
