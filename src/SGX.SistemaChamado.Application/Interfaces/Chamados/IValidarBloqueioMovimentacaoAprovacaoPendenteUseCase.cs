using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase
{
    Task<ValidarBloqueioMovimentacaoAprovacaoPendenteResponse> ExecutarAsync(
        ValidarBloqueioMovimentacaoAprovacaoPendenteRequest request,
        CancellationToken cancellationToken = default);
}
