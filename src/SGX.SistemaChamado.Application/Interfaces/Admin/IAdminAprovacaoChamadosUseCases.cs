using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminAprovacaoChamadosUseCases
{
    Task<PagedResultResponse<AprovacaoChamadoListagemDto>> ListarAsync(FiltroAprovacaoChamadoRequest request, CancellationToken cancellationToken = default);
    Task<AprovacaoChamadoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AprovacaoChamadoDetalheDto> SolicitarAsync(Guid chamadoId, SolicitarAprovacaoChamadoRequest request, CancellationToken cancellationToken = default);
    Task<AprovacaoChamadoDetalheDto> AprovarAsync(Guid id, DecidirAprovacaoChamadoRequest request, CancellationToken cancellationToken = default);
    Task<AprovacaoChamadoDetalheDto> ReprovarAsync(Guid id, DecidirAprovacaoChamadoRequest request, CancellationToken cancellationToken = default);
    Task<AprovacaoChamadoDetalheDto> CancelarAsync(Guid id, CancelarAprovacaoChamadoRequest request, CancellationToken cancellationToken = default);
}
