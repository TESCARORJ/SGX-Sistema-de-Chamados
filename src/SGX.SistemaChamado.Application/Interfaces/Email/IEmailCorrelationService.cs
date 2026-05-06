using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Email;

public interface IEmailCorrelationService
{
    Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default);
}
