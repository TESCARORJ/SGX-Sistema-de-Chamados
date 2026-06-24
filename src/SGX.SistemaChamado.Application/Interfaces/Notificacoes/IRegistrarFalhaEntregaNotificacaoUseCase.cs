using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IRegistrarFalhaEntregaNotificacaoUseCase
{
    Task<RegistrarFalhaEntregaNotificacaoResponse> ExecutarAsync(
        RegistrarFalhaEntregaNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
