using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarArtigosBaseConhecimentoUseCase
{
    Task<PagedResultResponse<BaseConhecimentoArtigoListagemDto>> ExecutarAsync(FiltroBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken = default);
}

public interface IObterArtigoBaseConhecimentoUseCase
{
    Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarArtigoBaseConhecimentoUseCase
{
    Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(CriarBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarArtigoBaseConhecimentoUseCase
{
    Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(Guid id, AtualizarBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken = default);
}

public interface IPublicarArtigoBaseConhecimentoUseCase
{
    Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IArquivarArtigoBaseConhecimentoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarArtigoBaseConhecimentoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarArtigosConhecimentoDoChamadoUseCase
{
    Task<IReadOnlyCollection<ChamadoArtigoConhecimentoDto>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default);
}

public interface IVincularArtigoConhecimentoAoChamadoUseCase
{
    Task<ChamadoArtigoConhecimentoDto> ExecutarAsync(
        Guid chamadoId,
        Guid artigoId,
        VincularArtigoChamadoRequest? request = null,
        CancellationToken cancellationToken = default);
}

public interface IRemoverArtigoConhecimentoDoChamadoUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid chamadoId, Guid artigoId, CancellationToken cancellationToken = default);
}

public interface IBuscarArtigosConhecimentoParaVinculoUseCase
{
    Task<PagedResultResponse<ArtigoConhecimentoDisponivelParaVinculoDto>> ExecutarAsync(
        Guid chamadoId,
        BuscarArtigosParaVinculoChamadoRequest request,
        CancellationToken cancellationToken = default);
}
