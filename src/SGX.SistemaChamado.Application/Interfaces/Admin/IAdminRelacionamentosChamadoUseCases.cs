using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminRelacionamentosChamadoUseCases
{
    Task<ChamadoRelacionamentoAdminResponse> CriarAsync(
        CriarChamadoRelacionamentoRequest request,
        CancellationToken cancellationToken = default);

    Task<ChamadoRelacionamentoAdminResponse> CriarNaUnidadeDeTrabalhoAsync(
        CriarChamadoRelacionamentoRequest request,
        string chamadoOrigemCodigo,
        string chamadoDestinoCodigo,
        CancellationToken cancellationToken = default);

    Task RemoverAsync(
        RemoverChamadoRelacionamentoRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChamadoRelacionamentoAdminResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        bool incluirInativos = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DependenciaChamadoAdminResponse>> ListarDependenciasPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<bool> PossuiDependenciasAtivasAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<bool> EstaBloqueadoPorDependenciaAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<BloqueioChamadoAdminResponse> ObterBloqueioPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default);

    Task<ChamadoRelacionamentoAdminResponse> ObterPorIdAsync(
        Guid relacionamentoId,
        CancellationToken cancellationToken = default);
}
