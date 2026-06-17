using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface ICancelarChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancelarChamadoRequest request, CancellationToken cancellationToken = default);
}
