using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminCatalogoServicosUseCases
{
    Task<PagedResultResponse<CatalogoServicoListagemDto>> ListarAsync(FiltroCatalogoServicoRequest request, CancellationToken cancellationToken = default);
    Task<CatalogoServicoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CatalogoServicoDetalheDto> CriarAsync(CriarCatalogoServicoRequest request, CancellationToken cancellationToken = default);
    Task<CatalogoServicoDetalheDto> AtualizarAsync(Guid id, AtualizarCatalogoServicoRequest request, CancellationToken cancellationToken = default);
    Task<CatalogoServicoDetalheDto> PublicarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ArquivarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default);
}
