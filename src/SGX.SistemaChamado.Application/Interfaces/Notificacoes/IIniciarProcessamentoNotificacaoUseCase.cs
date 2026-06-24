using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IIniciarProcessamentoNotificacaoUseCase
{
    Task<IniciarProcessamentoNotificacaoResponse> ExecutarAsync(
        IniciarProcessamentoNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
