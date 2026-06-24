using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IGerarNotificacaoUseCase
{
    Task<GerarNotificacaoResponse> ExecutarAsync(
        GerarNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
