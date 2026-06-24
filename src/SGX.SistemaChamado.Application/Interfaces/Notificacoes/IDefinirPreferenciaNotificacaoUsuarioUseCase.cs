using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IDefinirPreferenciaNotificacaoUsuarioUseCase
{
    Task<PreferenciaNotificacaoUsuarioResponse> ExecutarAsync(
        DefinirPreferenciaNotificacaoUsuarioRequest request,
        CancellationToken cancellationToken = default);
}
