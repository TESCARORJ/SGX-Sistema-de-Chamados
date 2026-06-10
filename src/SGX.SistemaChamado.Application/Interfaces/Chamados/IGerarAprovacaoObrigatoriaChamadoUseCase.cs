using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IGerarAprovacaoObrigatoriaChamadoUseCase
{
    Task<GerarAprovacaoObrigatoriaChamadoResponse> ExecutarAsync(
        GerarAprovacaoObrigatoriaChamadoRequest request,
        CancellationToken cancellationToken = default);
}
