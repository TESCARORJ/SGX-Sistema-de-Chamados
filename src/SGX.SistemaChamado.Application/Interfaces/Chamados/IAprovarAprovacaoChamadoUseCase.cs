using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IAprovarAprovacaoChamadoUseCase
{
    Task<AprovarAprovacaoChamadoResponse> ExecutarAsync(
        AprovarAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default);
}
