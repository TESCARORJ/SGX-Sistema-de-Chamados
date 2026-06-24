using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IAvaliarPreferenciaNotificacaoUseCase
{
    Task<AvaliarPreferenciaNotificacaoResponse> ExecutarAsync(
        AvaliarPreferenciaNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
