using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarRoadmapItsmUseCase
{
    Task<IReadOnlyCollection<RoadmapItsmResumoResponse>> ExecutarAsync(FiltroRoadmapItsmRequest request, CancellationToken cancellationToken = default);
}

public interface IObterRoadmapItsmItemUseCase
{
    Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarRoadmapItsmItemUseCase
{
    Task<RoadmapItsmDetalheResponse> ExecutarAsync(CriarRoadmapItsmItemRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarRoadmapItsmItemUseCase
{
    Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, AtualizarRoadmapItsmItemRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusRoadmapItsmUseCase
{
    Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, AtualizarStatusRoadmapItsmRequest request, CancellationToken cancellationToken = default);
}

public interface IInativarRoadmapItsmItemUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarRoadmapItsmItemUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarRoadmapImplementacoesFuturasUseCase
{
    Task<PagedResultResponse<RoadmapImplementacaoFuturaResponse>> ExecutarAsync(
        FiltroRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IListarRoadmapImplementacoesPorItemUseCase
{
    Task<IReadOnlyCollection<RoadmapImplementacaoFuturaResponse>> ExecutarAsync(
        Guid roadmapItemId,
        CancellationToken cancellationToken = default);
}

public interface IObterRoadmapImplementacaoFuturaUseCase
{
    Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarRoadmapImplementacaoFuturaUseCase
{
    Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(
        CriarRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IAtualizarRoadmapImplementacaoFuturaUseCase
{
    Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(
        Guid id,
        AtualizarRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IConcluirRoadmapImplementacaoFuturaUseCase
{
    Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IInativarRoadmapImplementacaoFuturaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarRoadmapImplementacaoFuturaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarRoadmapCategoriasUseCase
{
    Task<IReadOnlyCollection<RoadmapCategoriaResponse>> ExecutarAsync(
        FiltroRoadmapCategoriaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IObterRoadmapCategoriaUseCase
{
    Task<RoadmapCategoriaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarRoadmapCategoriaUseCase
{
    Task<RoadmapCategoriaResponse> ExecutarAsync(CriarRoadmapCategoriaRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarRoadmapCategoriaUseCase
{
    Task<RoadmapCategoriaResponse> ExecutarAsync(
        Guid id,
        AtualizarRoadmapCategoriaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IInativarRoadmapCategoriaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarRoadmapCategoriaUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IListarRoadmapChecklistPorItemUseCase
{
    Task<IReadOnlyCollection<RoadmapChecklistItemResponse>> ExecutarAsync(
        Guid roadmapItemId,
        CancellationToken cancellationToken = default);
}

public interface ICriarRoadmapChecklistItemUseCase
{
    Task<RoadmapChecklistItemResponse> ExecutarAsync(
        Guid roadmapItemId,
        CriarRoadmapChecklistItemRequest request,
        CancellationToken cancellationToken = default);
}

public interface IAtualizarRoadmapChecklistItemUseCase
{
    Task<RoadmapChecklistItemResponse> ExecutarAsync(
        Guid id,
        AtualizarRoadmapChecklistItemRequest request,
        CancellationToken cancellationToken = default);
}

public interface IConcluirRoadmapChecklistItemUseCase
{
    Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReabrirRoadmapChecklistItemUseCase
{
    Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IInativarRoadmapChecklistItemUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IExcluirRoadmapChecklistItemUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IReativarRoadmapChecklistItemUseCase
{
    Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
