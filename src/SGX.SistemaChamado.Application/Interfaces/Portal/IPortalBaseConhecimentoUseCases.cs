using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Interfaces.Portal;

public interface IListarArtigosPortalBaseConhecimentoUseCase
{
    Task<PortalListaBaseConhecimentoArtigosResponse> ExecutarAsync(PortalFiltroBaseConhecimentoRequest request, CancellationToken cancellationToken = default);
}

public interface IObterArtigoPortalBaseConhecimentoPorSlugUseCase
{
    Task<PortalBaseConhecimentoArtigoDetalheDto> ExecutarAsync(string slug, CancellationToken cancellationToken = default);
}

public interface IPortalBaseConhecimentoUseCases :
    IListarArtigosPortalBaseConhecimentoUseCase,
    IObterArtigoPortalBaseConhecimentoPorSlugUseCase
{
}