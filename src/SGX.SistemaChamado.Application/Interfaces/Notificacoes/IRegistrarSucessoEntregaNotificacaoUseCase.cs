using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IRegistrarSucessoEntregaNotificacaoUseCase
{
    Task ExecutarAsync(
        RegistrarSucessoEntregaNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
