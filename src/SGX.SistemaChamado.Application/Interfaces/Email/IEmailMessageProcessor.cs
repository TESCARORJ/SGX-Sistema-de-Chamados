namespace SGX.SistemaChamado.Application.Interfaces.Email;

public enum EmailMensagemProcessamentoStatus
{
    Processado = 1,
    Ignorado = 2,
    Erro = 3,
    Duplicado = 4,
    NaoCorrelacionado = 5
}

public sealed record EmailMensagemProcessamentoResultado(
    EmailMensagemProcessamentoStatus Status,
    Guid? ChamadoId,
    string? Erro);

public interface IEmailMessageProcessor
{
    Task<EmailMensagemProcessamentoResultado> ProcessarAsync(EmailMessageData mensagem, CancellationToken cancellationToken = default);
}
