using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IRejeitarSolucaoChamadoUseCase
{
    Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, RejeitarSolucaoChamadoRequest request, CancellationToken cancellationToken = default);
}
