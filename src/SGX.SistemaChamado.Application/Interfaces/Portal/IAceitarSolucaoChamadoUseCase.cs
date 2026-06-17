using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IAceitarSolucaoChamadoUseCase
{
    Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, AceitarSolucaoChamadoRequest request, CancellationToken cancellationToken = default);
}
