using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface ISelecionarNotificacoesProcessaveisUseCase
{
    Task<IReadOnlyCollection<NotificacaoProcessavelResponse>> ExecutarAsync(
        SelecionarNotificacoesProcessaveisRequest request,
        CancellationToken cancellationToken = default);
}
