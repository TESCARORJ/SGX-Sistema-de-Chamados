using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroBaseConhecimentoArtigoRequest
{
    public string? Termo { get; init; }
    public StatusArtigoConhecimento? Status { get; init; }
    public VisibilidadeArtigoConhecimento? Visibilidade { get; init; }
    public Guid? CategoriaId { get; init; }
    public bool? Ativo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "atualizadoEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed record BaseConhecimentoArtigoListagemDto(
    Guid Id,
    string Titulo,
    string Slug,
    string? Resumo,
    StatusArtigoConhecimento Status,
    string StatusDescricao,
    VisibilidadeArtigoConhecimento Visibilidade,
    string VisibilidadeDescricao,
    Guid? CategoriaId,
    string? CategoriaNome,
    string? Tags,
    DateTime? PublicadoEm,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record BaseConhecimentoArtigoDetalheDto(
    Guid Id,
    string Titulo,
    string Slug,
    string? Resumo,
    string Conteudo,
    Guid? CategoriaId,
    string? CategoriaNome,
    StatusArtigoConhecimento Status,
    string StatusDescricao,
    VisibilidadeArtigoConhecimento Visibilidade,
    string VisibilidadeDescricao,
    string? Tags,
    DateTime? PublicadoEm,
    Guid? PublicadoPorUsuarioId,
    DateTime CriadoEm,
    Guid CriadoPorUsuarioId,
    DateTime? AtualizadoEm,
    Guid? AtualizadoPorUsuarioId,
    DateTime? ArquivadoEm,
    Guid? ArquivadoPorUsuarioId,
    bool Ativo);

public sealed class CriarBaseConhecimentoArtigoRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Resumo { get; init; }
    public string Conteudo { get; init; } = string.Empty;
    public Guid? CategoriaId { get; init; }
    public VisibilidadeArtigoConhecimento Visibilidade { get; init; }
    public string? Tags { get; init; }
}

public sealed class AtualizarBaseConhecimentoArtigoRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Resumo { get; init; }
    public string Conteudo { get; init; } = string.Empty;
    public Guid? CategoriaId { get; init; }
    public VisibilidadeArtigoConhecimento Visibilidade { get; init; }
    public string? Tags { get; init; }
}