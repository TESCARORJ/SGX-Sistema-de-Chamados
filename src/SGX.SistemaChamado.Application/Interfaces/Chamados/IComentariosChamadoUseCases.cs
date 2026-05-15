using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IListarComentariosChamadoUseCase
{
    Task<IReadOnlyCollection<ComentarioChamadoResponse>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAdicionarComentarioChamadoUseCase
{
    Task<ComentarioChamadoResponse> ExecutarAsync(Guid chamadoId, CriarComentarioChamadoRequest request, CancellationToken cancellationToken = default);
}
