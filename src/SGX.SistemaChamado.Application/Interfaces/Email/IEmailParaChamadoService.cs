using SGX.SistemaChamado.Application.DTOs.Email;

namespace SGX.SistemaChamado.Application.Interfaces.Email;

public interface IEmailParaChamadoService
{
    Task<EmailProcessingResult> ProcessarAsync(EmailMessageDto mensagem, CancellationToken cancellationToken = default);
}
