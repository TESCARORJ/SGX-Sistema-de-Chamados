using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IEntregarNotificacaoSistemaUseCase
{
    Task<EntregarNotificacaoSistemaResponse> ExecutarAsync(
        EntregarNotificacaoSistemaRequest request,
        CancellationToken cancellationToken = default);
}
