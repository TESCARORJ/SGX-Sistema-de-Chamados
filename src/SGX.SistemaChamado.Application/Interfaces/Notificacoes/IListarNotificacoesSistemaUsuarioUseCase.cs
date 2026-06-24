using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IListarNotificacoesSistemaUsuarioUseCase
{
    Task<IReadOnlyCollection<NotificacaoSistemaResumoResponse>> ExecutarAsync(
        ListarNotificacoesSistemaUsuarioRequest request,
        CancellationToken cancellationToken = default);
}
