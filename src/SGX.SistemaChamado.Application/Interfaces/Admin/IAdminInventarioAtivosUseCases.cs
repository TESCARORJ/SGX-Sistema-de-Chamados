using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminInventarioAtivosUseCases
{
    Task<PagedResultResponse<InventarioAtivoListagemDto>> ListarAsync(FiltroInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<InventarioAtivoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventarioAtivoDetalheDto> CriarAsync(CriarInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<InventarioAtivoDetalheDto> AtualizarAsync(Guid id, AtualizarInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<InventarioAtivoDetalheDto> MovimentarAsync(Guid id, MovimentarInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<PagedResultResponse<HistoricoInventarioAtivoDto>> ListarHistoricoAsync(Guid inventarioAtivoId, FiltroHistoricoInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<PagedResultResponse<ChamadoRelacionadoInventarioAtivoDto>> ListarChamadosAsync(Guid inventarioAtivoId, FiltroChamadosRelacionadosInventarioAtivoRequest request, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> InativarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TipoAtivoInventarioDto>> ListarTiposAtivoAsync(CancellationToken cancellationToken = default);
}
