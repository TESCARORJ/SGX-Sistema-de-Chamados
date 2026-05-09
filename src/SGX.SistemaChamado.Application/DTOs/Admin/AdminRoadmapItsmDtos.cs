using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroRoadmapItsmRequest
{
    public StatusRoadmapItsm? Status { get; init; }
    public PrioridadeRoadmapItsm? Prioridade { get; init; }
    public ImpactoRoadmapItsm? Impacto { get; init; }
    public Guid? RoadmapCategoriaId { get; init; }
    public string? Categoria { get; init; }
    public bool? Ativo { get; init; }
}

public sealed record RoadmapItsmResumoResponse(
    Guid Id,
    string Area,
    string Categoria,
    Guid? RoadmapCategoriaId,
    string? RoadmapCategoriaNome,
    string? RoadmapCategoriaCor,
    string? RoadmapCategoriaIcone,
    string SituacaoAtual,
    StatusRoadmapItsm Status,
    string StatusDescricao,
    PrioridadeRoadmapItsm Prioridade,
    string PrioridadeDescricao,
    ImpactoRoadmapItsm Impacto,
    string ImpactoDescricao,
    DecisaoRoadmapItsm Decisao,
    string DecisaoDescricao,
    string? Responsavel,
    DateTime? PrazoAlvo,
    int Ordem,
    bool Ativo,
    StatusImplementacaoRoadmapItsm StatusImplementacao,
    string StatusImplementacaoDescricao,
    StatusTecnicoRoadmapItsm StatusTecnico,
    string StatusTecnicoDescricao,
    int PercentualImplementacao,
    bool PercentualCalculadoPorChecklist,
    int QuantidadeChecklistAtivo,
    int QuantidadeChecklistConcluido,
    string? PendenciasTecnicas,
    string? PendenciasHomologacao,
    string? EvidenciaImplementacao,
    DateTime? DataConclusaoTecnica,
    DateTime? DataHomologacao,
    string? CriterioAceite,
    string? ProximaAcao);

public sealed record RoadmapItsmDetalheResponse(
    Guid Id,
    string Area,
    string Categoria,
    Guid? RoadmapCategoriaId,
    string? RoadmapCategoriaNome,
    string? RoadmapCategoriaCor,
    string? RoadmapCategoriaIcone,
    string SituacaoAtual,
    string AtencaoTecnica,
    StatusRoadmapItsm Status,
    string StatusDescricao,
    PrioridadeRoadmapItsm Prioridade,
    string PrioridadeDescricao,
    ImpactoRoadmapItsm Impacto,
    string ImpactoDescricao,
    DecisaoRoadmapItsm Decisao,
    string DecisaoDescricao,
    string? Observacao,
    string? Responsavel,
    DateTime? PrazoAlvo,
    int Ordem,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor,
    StatusImplementacaoRoadmapItsm StatusImplementacao,
    string StatusImplementacaoDescricao,
    StatusTecnicoRoadmapItsm StatusTecnico,
    string StatusTecnicoDescricao,
    int PercentualImplementacao,
    bool PercentualCalculadoPorChecklist,
    int QuantidadeChecklistAtivo,
    int QuantidadeChecklistConcluido,
    string? PendenciasTecnicas,
    string? PendenciasHomologacao,
    string? EvidenciaImplementacao,
    DateTime? DataConclusaoTecnica,
    DateTime? DataHomologacao,
    string? CriterioAceite,
    string? ProximaAcao);

public sealed class CriarRoadmapItsmItemRequest
{
    public string Area { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public Guid? RoadmapCategoriaId { get; init; }
    public string SituacaoAtual { get; init; } = string.Empty;
    public string AtencaoTecnica { get; init; } = string.Empty;
    public StatusRoadmapItsm Status { get; init; }
    public PrioridadeRoadmapItsm Prioridade { get; init; }
    public ImpactoRoadmapItsm Impacto { get; init; }
    public DecisaoRoadmapItsm Decisao { get; init; }
    public string? Observacao { get; init; }
    public string? Responsavel { get; init; }
    public DateTime? PrazoAlvo { get; init; }
    public int Ordem { get; init; }
    public bool Ativo { get; init; } = true;
    public StatusImplementacaoRoadmapItsm StatusImplementacao { get; init; }
    public StatusTecnicoRoadmapItsm StatusTecnico { get; init; }
    public int? PercentualImplementacao { get; init; }
    public string? PendenciasTecnicas { get; init; }
    public string? PendenciasHomologacao { get; init; }
    public string? EvidenciaImplementacao { get; init; }
    public DateTime? DataConclusaoTecnica { get; init; }
    public DateTime? DataHomologacao { get; init; }
    public string? CriterioAceite { get; init; }
    public string? ProximaAcao { get; init; }
}

public sealed class AtualizarRoadmapItsmItemRequest
{
    public string Area { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public Guid? RoadmapCategoriaId { get; init; }
    public string SituacaoAtual { get; init; } = string.Empty;
    public string AtencaoTecnica { get; init; } = string.Empty;
    public StatusRoadmapItsm Status { get; init; }
    public PrioridadeRoadmapItsm Prioridade { get; init; }
    public ImpactoRoadmapItsm Impacto { get; init; }
    public DecisaoRoadmapItsm Decisao { get; init; }
    public string? Observacao { get; init; }
    public string? Responsavel { get; init; }
    public DateTime? PrazoAlvo { get; init; }
    public int Ordem { get; init; }
    public bool Ativo { get; init; } = true;
    public StatusImplementacaoRoadmapItsm StatusImplementacao { get; init; }
    public StatusTecnicoRoadmapItsm StatusTecnico { get; init; }
    public int? PercentualImplementacao { get; init; }
    public string? PendenciasTecnicas { get; init; }
    public string? PendenciasHomologacao { get; init; }
    public string? EvidenciaImplementacao { get; init; }
    public DateTime? DataConclusaoTecnica { get; init; }
    public DateTime? DataHomologacao { get; init; }
    public string? CriterioAceite { get; init; }
    public string? ProximaAcao { get; init; }
}

public sealed class AtualizarStatusRoadmapItsmRequest
{
    public StatusRoadmapItsm Status { get; init; }
    public PrioridadeRoadmapItsm Prioridade { get; init; }
    public DecisaoRoadmapItsm Decisao { get; init; }
    public string? Responsavel { get; init; }
    public DateTime? PrazoAlvo { get; init; }
    public string? Observacao { get; init; }
}

public sealed class FiltroRoadmapImplementacaoFuturaRequest
{
    public Guid? RoadmapItemId { get; init; }
    public string? Texto { get; init; }
    public TipoRoadmapImplementacaoFutura? Tipo { get; init; }
    public PrioridadeRoadmapImplementacaoFutura? Prioridade { get; init; }
    public StatusRoadmapImplementacaoFutura? Status { get; init; }
    public string? Responsavel { get; init; }
    public bool? Ativo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
}

public sealed record RoadmapImplementacaoFuturaResponse(
    Guid Id,
    Guid RoadmapItemId,
    string Titulo,
    string? Descricao,
    TipoRoadmapImplementacaoFutura Tipo,
    string TipoDescricao,
    PrioridadeRoadmapImplementacaoFutura Prioridade,
    string PrioridadeDescricao,
    StatusRoadmapImplementacaoFutura Status,
    string StatusDescricao,
    string? Responsavel,
    DateTime? PrazoAlvo,
    DateTime? DataConclusao,
    string? Observacao,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor);

public sealed class CriarRoadmapImplementacaoFuturaRequest
{
    public Guid RoadmapItemId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public TipoRoadmapImplementacaoFutura Tipo { get; init; }
    public PrioridadeRoadmapImplementacaoFutura Prioridade { get; init; }
    public StatusRoadmapImplementacaoFutura Status { get; init; }
    public string? Responsavel { get; init; }
    public DateTime? PrazoAlvo { get; init; }
    public string? Observacao { get; init; }
}

public sealed class AtualizarRoadmapImplementacaoFuturaRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public TipoRoadmapImplementacaoFutura Tipo { get; init; }
    public PrioridadeRoadmapImplementacaoFutura Prioridade { get; init; }
    public StatusRoadmapImplementacaoFutura Status { get; init; }
    public string? Responsavel { get; init; }
    public DateTime? PrazoAlvo { get; init; }
    public DateTime? DataConclusao { get; init; }
    public string? Observacao { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class FiltroRoadmapCategoriaRequest
{
    public bool? Ativo { get; init; }
    public string? Texto { get; init; }
}

public sealed record RoadmapCategoriaResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    string? Cor,
    string? Icone,
    int? Ordem,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor);

public sealed class CriarRoadmapCategoriaRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? Cor { get; init; }
    public string? Icone { get; init; }
    public int? Ordem { get; init; }
}

public sealed class AtualizarRoadmapCategoriaRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? Cor { get; init; }
    public string? Icone { get; init; }
    public int? Ordem { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class CriarRoadmapChecklistItemRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public GrupoRoadmapChecklist Grupo { get; init; }
    public int Ordem { get; init; }
    public bool Concluido { get; init; }
    public bool Obrigatorio { get; init; } = true;
}

public sealed class AtualizarRoadmapChecklistItemRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public GrupoRoadmapChecklist Grupo { get; init; }
    public int Ordem { get; init; }
    public bool Concluido { get; init; }
    public bool Obrigatorio { get; init; } = true;
    public bool Ativo { get; init; } = true;
}

public sealed record RoadmapChecklistItemResponse(
    Guid Id,
    Guid RoadmapItemId,
    string Titulo,
    string? Descricao,
    GrupoRoadmapChecklist Grupo,
    string GrupoDescricao,
    int Ordem,
    bool Concluido,
    bool Obrigatorio,
    bool Ativo,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor);
