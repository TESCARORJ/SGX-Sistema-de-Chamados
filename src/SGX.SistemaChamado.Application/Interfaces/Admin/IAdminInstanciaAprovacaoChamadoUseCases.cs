using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminInstanciaAprovacaoChamadoUseCases
{
    Task<PagedResultResponse<InstanciaAprovacaoChamadoResumoResponse>> ListarAsync(
        ListarInstanciasAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default);

    Task<InstanciaAprovacaoChamadoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPendentesAsync(
        Guid? aprovadorUsuarioId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPorStatusAsync(
        StatusInstanciaAprovacaoChamado status,
        CancellationToken cancellationToken = default);

    Task<ValidarInstanciaAprovacaoChamadoResponse> ValidarAsync(
        ValidarInstanciaAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default);

    Task<PrepararInstanciaAprovacaoChamadoResponse> PrepararAsync(
        PrepararInstanciaAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default);

    Task<InstanciaAprovacaoChamadoResponse> CriarManualAsync(
        CriarInstanciaAprovacaoChamadoManualRequest request,
        CancellationToken cancellationToken = default);
}
