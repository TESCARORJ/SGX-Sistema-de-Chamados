using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarLogsIntegracaoEmailUseCase
{
    Task<ListaLogsIntegracaoEmailResponse> ExecutarAsync(FiltroLogsEmailRequest request, CancellationToken cancellationToken = default);
}

public interface IObterLogIntegracaoEmailUseCase
{
    Task<LogIntegracaoEmailDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
