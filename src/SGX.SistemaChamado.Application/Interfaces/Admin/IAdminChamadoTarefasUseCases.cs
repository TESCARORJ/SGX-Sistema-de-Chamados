using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminChamadoTarefasUseCases
{
    Task<ChamadoTarefaAdminResponse> CriarAsync(
        Guid chamadoId,
        CriarChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChamadoTarefaAdminResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        bool incluirInativas = false,
        CancellationToken cancellationToken = default);

    Task<ChamadoTarefaAdminResponse> AtualizarStatusAsync(
        Guid chamadoId,
        Guid tarefaId,
        AtualizarStatusChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken = default);

    Task CancelarAsync(
        Guid chamadoId,
        Guid tarefaId,
        CancelarChamadoTarefaAdminRequest? request = null,
        CancellationToken cancellationToken = default);
}
