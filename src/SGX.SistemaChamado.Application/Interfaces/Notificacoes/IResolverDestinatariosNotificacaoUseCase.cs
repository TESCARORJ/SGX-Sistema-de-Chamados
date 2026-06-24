using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IResolverDestinatariosNotificacaoUseCase
{
    Task<ResolverDestinatariosNotificacaoResponse> ExecutarAsync(
        ResolverDestinatariosNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
