using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IReprovarAprovacaoChamadoUseCase
{
    Task<ReprovarAprovacaoChamadoResponse> ExecutarAsync(
        ReprovarAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default);
}
