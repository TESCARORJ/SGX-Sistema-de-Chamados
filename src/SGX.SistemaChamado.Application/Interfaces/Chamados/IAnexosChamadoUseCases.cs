using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IListarAnexosChamadoUseCase
{
    Task<IReadOnlyCollection<AnexoChamadoResponse>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAdicionarAnexoChamadoUseCase
{
    Task<UploadAnexoChamadoResponse> ExecutarAsync(Guid chamadoId, CriarAnexoChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IBaixarAnexoChamadoUseCase
{
    Task<DownloadAnexoChamadoResponse> ExecutarAsync(Guid chamadoId, Guid anexoId, CancellationToken cancellationToken = default);
}
