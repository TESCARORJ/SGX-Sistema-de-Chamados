using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IObterMinhaNotificacaoUseCase
{
    Task<MinhaNotificacaoDetalheResponse> ExecutarAsync(
        Guid notificacaoId,
        CancellationToken cancellationToken = default);
}
