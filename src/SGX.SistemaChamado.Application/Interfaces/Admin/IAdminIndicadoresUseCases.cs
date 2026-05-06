using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IObterDashboardAdminUseCase
{
    Task<DashboardAdminResponse> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}

public interface IObterIndicadoresChamadosPorStatusUseCase
{
    Task<IReadOnlyCollection<ChamadosPorStatusResponse>> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}

public interface IObterIndicadoresChamadosPorPrioridadeUseCase
{
    Task<IReadOnlyCollection<ChamadosPorPrioridadeResponse>> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}

public interface IObterIndicadoresChamadosPorCategoriaUseCase
{
    Task<IReadOnlyCollection<ChamadosPorCategoriaResponse>> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}

public interface IObterIndicadoresSlaUseCase
{
    Task<IndicadoresSlaResponse> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}

public interface IObterIndicadoresProdutividadeUseCase
{
    Task<IReadOnlyCollection<ProdutividadeAtendenteResponse>> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default);
}
