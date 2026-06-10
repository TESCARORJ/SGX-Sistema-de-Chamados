using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase
{
    Task<ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse> ExecutarAsync(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request,
        CancellationToken cancellationToken = default);
}
