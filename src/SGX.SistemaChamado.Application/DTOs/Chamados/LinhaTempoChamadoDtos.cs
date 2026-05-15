namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class LinhaTempoChamadoResponse
{
    public Guid ChamadoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public IReadOnlyCollection<LinhaTempoChamadoItemResponse> Items { get; init; } = [];
}

public sealed class LinhaTempoChamadoItemResponse
{
    public Guid Id { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string TipoDescricao { get; init; } = string.Empty;
    public DateTime DataHora { get; init; }
    public Guid? UsuarioId { get; init; }
    public string? Usuario { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public bool Interno { get; init; }
    public Guid? ReferenciaId { get; init; }
    public string? ReferenciaTipo { get; init; }
    public string? NomeArquivo { get; init; }
    public string? ContentType { get; init; }
    public long? TamanhoBytes { get; init; }
    public string? Status { get; init; }
    public string? Prioridade { get; init; }
    public string? Categoria { get; init; }
    public string? Responsavel { get; init; }
}
