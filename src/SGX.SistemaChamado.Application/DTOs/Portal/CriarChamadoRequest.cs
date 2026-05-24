namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class CriarChamadoRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public Guid? CatalogoServicoId { get; set; }
    public string? CatalogoServicoSlug { get; set; }
    public Guid? DepartamentoId { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? SubcategoriaId { get; set; }
    public Guid? PrioridadeId { get; set; }
    public Guid? TipoSolicitacaoId { get; set; }
    public Guid? LocalUnidadeId { get; set; }
}
