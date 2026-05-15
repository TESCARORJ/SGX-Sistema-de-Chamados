using SGX.SistemaChamado.Application.DTOs.Auditoria;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarEventosAuditoriaUseCase
{
    Task<ListaEventosAuditoriaResponse> ExecutarAsync(
        FiltroEventosAuditoriaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IObterEventoAuditoriaUseCase
{
    Task<EventoAuditoriaDetalheResponse> ExecutarAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

public interface IObterDashboardAuditoriaUseCase
{
    Task<AuditoriaDashboardResponse> ExecutarAsync(
        FiltroDashboardAuditoriaRequest request,
        CancellationToken cancellationToken = default);
}
