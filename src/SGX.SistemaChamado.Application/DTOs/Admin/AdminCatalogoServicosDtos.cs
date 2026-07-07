using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroCatalogoServicoRequest
{
    public string? Termo { get; init; }
    public Guid? DepartamentoResponsavelId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public Guid? SlaPadraoId { get; init; }
    public Guid? PoliticaSlaId { get; init; }
    public StatusCatalogoServico? Status { get; init; }
    public VisibilidadeCatalogoServico? Visibilidade { get; init; }
    public bool? Ativo { get; init; }
    public bool? PermiteAberturaChamado { get; init; }
    public bool? RequerAprovacao { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "atualizadoEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed record CatalogoServicoListagemDto(
    Guid Id,
    string Nome,
    string Slug,
    string Descricao,
    Guid DepartamentoResponsavelId,
    string? DepartamentoResponsavelNome,
    Guid? GrupoTecnicoId,
    string? NomeGrupoTecnico,
    Guid? CategoriaId,
    string? CategoriaNome,
    Guid? SubcategoriaId,
    string? SubcategoriaNome,
    Guid? PrioridadePadraoId,
    string? PrioridadePadraoNome,
    Guid? SlaPadraoId,
    string? SlaPadraoNome,
    StatusCatalogoServico Status,
    string StatusDescricao,
    VisibilidadeCatalogoServico Visibilidade,
    string VisibilidadeDescricao,
    bool PermiteAberturaChamado,
    bool RequerAprovacao,
    int Ordem,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    DateTime? PublicadoEm,
    DateTime? ArquivadoEm);

public sealed record CatalogoServicoDetalheDto(
    Guid Id,
    string Nome,
    string Slug,
    string Descricao,
    string? InstrucoesSolicitante,
    Guid DepartamentoResponsavelId,
    string? DepartamentoResponsavelNome,
    Guid? GrupoTecnicoId,
    string? NomeGrupoTecnico,
    Guid? CategoriaId,
    string? CategoriaNome,
    Guid? SubcategoriaId,
    string? SubcategoriaNome,
    Guid? PrioridadePadraoId,
    string? PrioridadePadraoNome,
    Guid? SlaPadraoId,
    string? SlaPadraoNome,
    Guid? ArtigoBaseConhecimentoId,
    string? ArtigoBaseConhecimentoTitulo,
    StatusCatalogoServico Status,
    string StatusDescricao,
    VisibilidadeCatalogoServico Visibilidade,
    string VisibilidadeDescricao,
    bool PermiteAberturaChamado,
    bool RequerAprovacao,
    int Ordem,
    bool Ativo,
    DateTime CriadoEm,
    Guid CriadoPorUsuarioId,
    DateTime? AtualizadoEm,
    Guid? AtualizadoPorUsuarioId,
    DateTime? PublicadoEm,
    Guid? PublicadoPorUsuarioId,
    DateTime? ArquivadoEm,
    Guid? ArquivadoPorUsuarioId);

public sealed class CriarCatalogoServicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? InstrucoesSolicitante { get; init; }
    public Guid DepartamentoResponsavelId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public Guid? SlaPadraoId { get; init; }
    public Guid? PoliticaSlaId { get; init; }
    public Guid? ArtigoBaseConhecimentoId { get; init; }
    public Guid? GrupoTecnicoId { get; init; }
    public VisibilidadeCatalogoServico Visibilidade { get; init; } = VisibilidadeCatalogoServico.Interno;
    public bool? PermiteAberturaChamado { get; init; }
    public bool RequerAprovacao { get; init; }
    public int Ordem { get; init; }
}

public sealed class AtualizarCatalogoServicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? InstrucoesSolicitante { get; init; }
    public Guid DepartamentoResponsavelId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public Guid? SlaPadraoId { get; init; }
    public Guid? PoliticaSlaId { get; init; }
    public Guid? ArtigoBaseConhecimentoId { get; init; }
    public Guid? GrupoTecnicoId { get; init; }
    public VisibilidadeCatalogoServico Visibilidade { get; init; } = VisibilidadeCatalogoServico.Interno;
    public bool PermiteAberturaChamado { get; init; } = true;
    public bool RequerAprovacao { get; init; }
    public int Ordem { get; init; }
    public bool Ativo { get; init; } = true;
}
