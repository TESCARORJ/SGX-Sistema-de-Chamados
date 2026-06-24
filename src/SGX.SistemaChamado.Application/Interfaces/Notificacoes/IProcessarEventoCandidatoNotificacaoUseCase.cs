using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface IProcessarEventoCandidatoNotificacaoUseCase
{
    Task<ProcessarEventoCandidatoNotificacaoResponse> ExecutarAsync(
        ProcessarEventoCandidatoNotificacaoRequest request,
        CancellationToken cancellationToken = default);
}
