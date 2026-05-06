using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IObterAdminContextoUseCase
{
    Task<AdminContextoResponse> ExecutarAsync(CancellationToken cancellationToken = default);
}

public interface IListarChamadosAdminUseCase
{
    Task<ListaChamadosAdminResponse> ExecutarAsync(FiltroChamadosAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IDetalharChamadoAdminUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAssumirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAtribuirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AtribuirChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAlterarStatusChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarStatusChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAlterarPrioridadeChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAlterarCategoriaChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarCategoriaChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IComentarChamadoAdminUseCase
{
    Task<ComentarioAdminResponse> ExecutarAsync(Guid chamadoId, ComentarioAdminChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IEncerrarChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, EncerrarChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IReabrirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ReabrirChamadoRequest request, CancellationToken cancellationToken = default);
}
