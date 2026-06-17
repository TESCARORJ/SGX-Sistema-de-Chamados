using System.Threading;
using System.Threading.Tasks;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IObterConfiguracaoAutoFechamentoChamadoUseCase
{
    Task<ObterConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(CancellationToken cancellationToken = default);
}

public interface IAtualizarConfiguracaoAutoFechamentoChamadoUseCase
{
    Task<AtualizarConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(
        AtualizarConfiguracaoAutoFechamentoChamadoRequest request,
        CancellationToken cancellationToken = default);
}
