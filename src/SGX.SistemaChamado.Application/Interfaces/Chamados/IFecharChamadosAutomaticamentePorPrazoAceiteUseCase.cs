using System.Threading;
using System.Threading.Tasks;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IFecharChamadosAutomaticamentePorPrazoAceiteUseCase
{
    Task<FecharChamadosAutomaticamentePorPrazoAceiteResponse> ExecutarAsync(
        FecharChamadosAutomaticamentePorPrazoAceiteRequest request,
        CancellationToken cancellationToken = default);
}
