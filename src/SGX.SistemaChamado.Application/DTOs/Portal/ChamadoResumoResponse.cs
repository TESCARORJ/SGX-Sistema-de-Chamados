using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class ChamadoResumoResponse
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string? Subcategoria { get; init; }
    public string? TipoSolicitacao { get; init; }
    public string? LocalUnidade { get; init; }
    public string? Departamento { get; init; }
    public Guid CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid PrioridadeId { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? InventarioAtivoId { get; init; }
    public string? InventarioAtivoCodigo { get; init; }
    public string? InventarioAtivoNome { get; init; }
    public DateTime AbertoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }
    public bool SlaVencido { get; init; }
    public bool SlaProximoVencimento { get; init; }
    public SituacaoSlaChamadoEnum SituacaoSla { get; init; } = SituacaoSlaChamadoEnum.NaoAplicavel;
    public string? PoliticaSlaNome { get; init; }
    public int? TempoRestanteMinutos { get; init; }
    public int? TempoExcedidoMinutos { get; init; }
    public DateTime? PrazoPrimeiraRespostaEm { get; init; }
    public DateTime? PrimeiraRespostaEm { get; init; }
    public DateTime? PrazoResolucaoEm { get; init; }
    public DateTime? ResolvidoEm { get; init; }
    public bool EstaPausado { get; init; }
    public int TotalMinutosPausado { get; init; }
}

public sealed class ListaChamadosPortalResponse
{
    public IReadOnlyCollection<ChamadoResumoResponse> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}
