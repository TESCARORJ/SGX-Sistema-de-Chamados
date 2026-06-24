using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Tests;

internal sealed class FakeAdminRelacionamentosChamadoUseCases : IAdminRelacionamentosChamadoUseCases
{
    public Task<ChamadoRelacionamentoAdminResponse> CriarAsync(CriarChamadoRelacionamentoRequest request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<ChamadoRelacionamentoAdminResponse> CriarNaUnidadeDeTrabalhoAsync(CriarChamadoRelacionamentoRequest request, string chamadoOrigemCodigo, string chamadoDestinoCodigo, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task RemoverAsync(RemoverChamadoRelacionamentoRequest request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IReadOnlyList<ChamadoRelacionamentoAdminResponse>> ListarPorChamadoAsync(Guid chamadoId, bool incluirInativos = false, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IReadOnlyList<DependenciaChamadoAdminResponse>> ListarDependenciasPorChamadoAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> PossuiDependenciasAtivasAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<bool> EstaBloqueadoPorDependenciaAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<BloqueioChamadoAdminResponse> ObterBloqueioPorChamadoAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<ChamadoRelacionamentoAdminResponse> ObterPorIdAsync(Guid relacionamentoId, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}

internal sealed class FakeAdminChamadoAprovacoesUseCases : IAdminChamadoAprovacoesUseCases
{
    public Task<ChamadoAprovacaoAdminResponse> CriarAsync(Guid chamadoId, CriarChamadoAprovacaoAdminRequest request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<IReadOnlyList<ChamadoAprovacaoAdminResponse>> ListarPorChamadoAsync(Guid chamadoId, bool incluirInativas = false, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<ChamadoAprovacaoAdminResponse> AprovarAsync(Guid chamadoId, Guid aprovacaoId, DecidirChamadoAprovacaoAdminRequest request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<ChamadoAprovacaoAdminResponse> ReprovarAsync(Guid chamadoId, Guid aprovacaoId, DecidirChamadoAprovacaoAdminRequest request, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task CancelarAsync(Guid chamadoId, Guid aprovacaoId, CancelarChamadoAprovacaoAdminRequest? request = null, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<bool> PossuiAprovacaoPendenteAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task<bool> PossuiAprovacaoPendenteBloqueanteAsync(Guid chamadoId, CancellationToken cancellationToken = default)
        => Task.FromResult(false);
}
