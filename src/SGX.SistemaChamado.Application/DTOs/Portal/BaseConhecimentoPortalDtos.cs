namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class PortalFiltroBaseConhecimentoRequest
{
    public string? Termo { get; set; }
    public Guid? CategoriaId { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}

public sealed class PortalBaseConhecimentoArtigoListagemDto
{
    public Guid Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Resumo { get; init; }
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public string? Tags { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }
}

public sealed class PortalBaseConhecimentoArtigoDetalheDto
{
    public Guid Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Resumo { get; init; }
    public string Conteudo { get; init; } = string.Empty;
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public string? Tags { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }
}

public sealed class PortalListaBaseConhecimentoArtigosResponse
{
    public IReadOnlyCollection<PortalBaseConhecimentoArtigoListagemDto> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}