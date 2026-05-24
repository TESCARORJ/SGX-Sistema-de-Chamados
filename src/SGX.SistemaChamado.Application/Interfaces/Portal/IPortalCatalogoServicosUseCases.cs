using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IPortalCatalogoServicosUseCases
{
    Task<PortalListaCatalogoServicosResponse> ListarAsync(PortalFiltroCatalogoServicoRequest request, CancellationToken cancellationToken = default);
    Task<PortalCatalogoServicoDetalheDto> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PortalPrepararChamadoCatalogoServicoDto> PrepararAberturaChamadoAsync(string slug, CancellationToken cancellationToken = default);
}
