using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminChamadoAprovacoesUseCases
{
    Task<ChamadoAprovacaoAdminResponse> CriarAsync(
        Guid chamadoId,
        CriarChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChamadoAprovacaoAdminResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        bool incluirInativas = false,
        CancellationToken cancellationToken = default);

    Task<ChamadoAprovacaoAdminResponse> AprovarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default);

    Task<ChamadoAprovacaoAdminResponse> ReprovarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default);

    Task CancelarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        CancelarChamadoAprovacaoAdminRequest? request = null,
        CancellationToken cancellationToken = default);

    Task<bool> PossuiAprovacaoPendenteAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<bool> PossuiAprovacaoPendenteBloqueanteAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);
}
