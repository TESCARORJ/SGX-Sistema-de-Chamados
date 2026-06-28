using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IAbrirRequisicaoServicoCatalogoUseCase
{
    Task<ChamadoDetalheResponse> ExecutarAsync(AbrirRequisicaoServicoCatalogoRequest request, CancellationToken cancellationToken = default);
}
