using SGX.SistemaChamado.Application.DTOs.Email;

namespace SGX.SistemaChamado.Application.Interfaces.Email;

public interface IProcessarEmailRecebidoUseCase
{
    Task<EmailProcessingResult> ExecutarAsync(EmailMessageDto mensagem, CancellationToken cancellationToken = default);
}
