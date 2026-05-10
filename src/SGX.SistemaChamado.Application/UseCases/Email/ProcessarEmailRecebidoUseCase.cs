using SGX.SistemaChamado.Application.DTOs.Email;
using SGX.SistemaChamado.Application.Interfaces.Email;

namespace SGX.SistemaChamado.Application.UseCases.Email;

public sealed class ProcessarEmailRecebidoUseCase(
    IEmailParaChamadoService emailParaChamadoService) : IProcessarEmailRecebidoUseCase
{
    public Task<EmailProcessingResult> ExecutarAsync(EmailMessageDto mensagem, CancellationToken cancellationToken = default)
        => emailParaChamadoService.ProcessarAsync(mensagem, cancellationToken);
}
