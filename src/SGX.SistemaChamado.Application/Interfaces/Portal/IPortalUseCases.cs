using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IAbrirChamadoUseCase
{
    Task<ChamadoDetalheResponse> ExecutarAsync(CriarChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IListarMeusChamadosUseCase
{
    Task<ListaChamadosPortalResponse> ExecutarAsync(FiltroChamadosPortalRequest request, CancellationToken cancellationToken = default);
}

public interface IDetalharMeuChamadoUseCase
{
    Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IComentarChamadoUseCase
{
    Task<ComentarioChamadoResponse> ExecutarAsync(Guid chamadoId, ComentarioChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAnexarArquivoChamadoUseCase
{
    Task<AnexoChamadoResponse> ExecutarAsync(Guid chamadoId, UploadAnexoChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IObterPortalContextoUseCase
{
    Task<PortalContextoResponse> ExecutarAsync(CancellationToken cancellationToken = default);
}
