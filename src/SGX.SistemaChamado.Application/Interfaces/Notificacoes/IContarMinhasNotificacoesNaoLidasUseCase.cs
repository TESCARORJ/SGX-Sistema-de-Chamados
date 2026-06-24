using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IContarMinhasNotificacoesNaoLidasUseCase
{
    Task<ContagemMinhasNotificacoesNaoLidasResponse> ExecutarAsync(
        CancellationToken cancellationToken = default);
}
