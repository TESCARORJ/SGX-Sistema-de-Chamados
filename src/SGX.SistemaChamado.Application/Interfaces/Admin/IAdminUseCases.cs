using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IObterAdminContextoUseCase
{
    Task<AdminContextoResponse> ExecutarAsync(CancellationToken cancellationToken = default);
}

public interface IListarChamadosAdminUseCase
{
    Task<ListaChamadosAdminResponse> ExecutarAsync(FiltroChamadosAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IDetalharChamadoAdminUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAssumirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IAtribuirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AtribuirChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface ITransferirGrupoTecnicoChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, TransferirGrupoTecnicoChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IDirecionarChamadoGrupoTecnicoAdminUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, DirecionarChamadoGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IAssumirChamadoFilaAdminUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AssumirChamadoFilaRequest request, CancellationToken cancellationToken = default);
}

public interface IListarGruposTecnicosAdminUseCase
{
    Task<PagedResultResponse<GrupoTecnicoResumoResponse>> ExecutarAsync(ListarGruposTecnicosRequest request, CancellationToken cancellationToken = default);
}

public interface IObterGrupoTecnicoAdminUseCase
{
    Task<GrupoTecnicoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarGrupoTecnicoAdminUseCase
{
    Task<GrupoTecnicoResponse> ExecutarAsync(CriarGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarGrupoTecnicoAdminUseCase
{
    Task<GrupoTecnicoResponse> ExecutarAsync(Guid id, AtualizarGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusGrupoTecnicoAdminUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, AlterarStatusGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IListarMembrosGrupoTecnicoAdminUseCase
{
    Task<IReadOnlyCollection<MembroGrupoTecnicoResponse>> ExecutarAsync(Guid grupoTecnicoId, ListarMembrosGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IListarFilasAtendimentoGrupoTecnicoAdminUseCase
{
    Task<IReadOnlyCollection<FilaAtendimentoResumoResponse>> ExecutarAsync(Guid grupoTecnicoId, ListarFilasAtendimentoGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IAdicionarMembroGrupoTecnicoAdminUseCase
{
    Task<MembroGrupoTecnicoResponse> ExecutarAsync(Guid grupoTecnicoId, AdicionarMembroGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusMembroGrupoTecnicoAdminUseCase
{
    Task<MembroGrupoTecnicoResponse> ExecutarAsync(Guid membroId, AlterarStatusMembroGrupoTecnicoRequest request, CancellationToken cancellationToken = default);
}

public interface IListarGruposTecnicosDoUsuarioAdminUseCase
{
    Task<IReadOnlyCollection<GrupoTecnicoDoUsuarioResponse>> ExecutarAsync(Guid usuarioId, bool? ativo = true, CancellationToken cancellationToken = default);
}

public interface IAlterarStatusChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarStatusChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAlterarPrioridadeChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IAlterarCategoriaChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarCategoriaChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IComentarChamadoAdminUseCase
{
    Task<ComentarioAdminResponse> ExecutarAsync(Guid chamadoId, ComentarioAdminChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IEncerrarChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, EncerrarChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface IReabrirChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ReabrirChamadoRequest request, CancellationToken cancellationToken = default);
}

public interface ICriarChamadoDerivadoAdminUseCase
{
    Task<ChamadoDerivadoAdminResponse> ExecutarAsync(Guid chamadoOrigemId, CriarChamadoDerivadoAdminRequest request, CancellationToken cancellationToken = default);
}

public interface IVincularInventarioAtivoChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, Guid ativoId, CancellationToken cancellationToken = default);
}

public interface IRemoverInventarioAtivoChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}
