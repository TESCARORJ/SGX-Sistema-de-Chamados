using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IMaterializarConteudoNotificacaoUseCase
{
    Task<MaterializarConteudoNotificacaoResponse> ExecutarAsync(
        MaterializarConteudoNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
