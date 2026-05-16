namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed record DepartamentoPortalResponse(Guid Id, string Nome, string Sigla);
public sealed record CategoriaPortalResponse(Guid Id, string Nome, Guid? DepartamentoId);
public sealed record SubcategoriaPortalResponse(Guid Id, Guid CategoriaChamadoId, string Nome);
public sealed record PrioridadePortalResponse(Guid Id, string Nome, int Nivel);
public sealed record TipoSolicitacaoPortalResponse(Guid Id, string Nome);
public sealed record LocalUnidadePortalResponse(Guid Id, string Nome);
public sealed record StatusPortalResponse(Guid Id, string Nome, int Codigo);
public sealed record UsuarioPortalResponse(Guid Id, string Nome, string Email, string Login, IReadOnlyCollection<string> Perfis);
public sealed record ConfiguracaoAnexoPortalResponse(
    IReadOnlyCollection<string> TiposPermitidos,
    long? TamanhoMaximoBytes);

public sealed class PortalContextoResponse
{
    public UsuarioPortalResponse Usuario { get; init; } = default!;
    public IReadOnlyCollection<DepartamentoPortalResponse> Departamentos { get; init; } = [];
    public IReadOnlyCollection<CategoriaPortalResponse> Categorias { get; init; } = [];
    public IReadOnlyCollection<SubcategoriaPortalResponse> Subcategorias { get; init; } = [];
    public IReadOnlyCollection<PrioridadePortalResponse> Prioridades { get; init; } = [];
    public IReadOnlyCollection<TipoSolicitacaoPortalResponse> TiposSolicitacao { get; init; } = [];
    public IReadOnlyCollection<LocalUnidadePortalResponse> LocaisUnidade { get; init; } = [];
    public IReadOnlyCollection<StatusPortalResponse> Status { get; init; } = [];
    public ConfiguracaoAnexoPortalResponse? ConfiguracaoAnexos { get; init; }
}
