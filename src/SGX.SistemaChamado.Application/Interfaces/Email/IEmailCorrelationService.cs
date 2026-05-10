using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Email;

public sealed record EmailCorrelationResult(
    Chamado? Chamado,
    bool PossuiIndicadorResposta,
    string? CodigoDetectado,
    IReadOnlyCollection<string> HeadersCorrelacaoNormalizados);

public interface IEmailCorrelationService
{
    Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default);
    Task<EmailCorrelationResult> CorrelacionarAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default);
}
