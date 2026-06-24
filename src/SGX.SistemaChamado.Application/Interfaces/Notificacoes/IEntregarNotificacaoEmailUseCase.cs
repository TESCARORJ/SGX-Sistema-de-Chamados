using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IEntregarNotificacaoEmailUseCase
{
    Task<EntregarNotificacaoEmailResponse> ExecutarAsync(
        EntregarNotificacaoEmailRequest request,
        CancellationToken cancellationToken = default);
}
