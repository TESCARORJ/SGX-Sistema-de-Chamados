using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IMarcarMinhaNotificacaoComoLidaUseCase
{
    Task<AlterarLeituraNotificacaoResponse> ExecutarAsync(
        Guid notificacaoId,
        CancellationToken cancellationToken = default);
}
