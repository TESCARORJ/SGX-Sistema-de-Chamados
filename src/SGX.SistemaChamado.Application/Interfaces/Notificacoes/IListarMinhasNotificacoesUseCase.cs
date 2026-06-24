using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IListarMinhasNotificacoesUseCase
{
    Task<ListarMinhasNotificacoesResponse> ExecutarAsync(
        ListarMinhasNotificacoesRequest request,
        CancellationToken cancellationToken = default);
}
