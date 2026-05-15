using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Interfaces.Chamados;

public interface IListarLinhaTempoChamadoUseCase
{
    Task<LinhaTempoChamadoResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}
