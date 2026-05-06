namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed record ComentarioChamadoResponse(
    Guid Id,
    Guid UsuarioId,
    string Usuario,
    string Mensagem,
    DateTime CriadoEm);

public sealed record AnexoChamadoResponse(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    DateTime CriadoEm,
    Guid UsuarioId,
    string Usuario);

public sealed record HistoricoChamadoResponse(
    Guid Id,
    int Tipo,
    string TipoDescricao,
    string Descricao,
    DateTime CriadoEm,
    Guid? UsuarioId,
    string? Usuario);

public sealed record SlaResumoResponse(
    DateTime PrazoPrimeiraRespostaEm,
    DateTime? PrimeiraRespostaEm,
    DateTime PrazoResolucaoEm,
    DateTime? ResolvidoEm,
    bool EstaVencido,
    bool EstaPausado,
    int TotalMinutosPausado);

public sealed class ChamadoDetalheResponse
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string? Departamento { get; init; }
    public string Solicitante { get; init; } = string.Empty;
    public string? Responsavel { get; init; }
    public DateTime AbertoEm { get; init; }
    public DateTime? EncerradoEm { get; init; }
    public IReadOnlyCollection<ComentarioChamadoResponse> Comentarios { get; init; } = [];
    public IReadOnlyCollection<AnexoChamadoResponse> Anexos { get; init; } = [];
    public IReadOnlyCollection<HistoricoChamadoResponse> Historico { get; init; } = [];
    public SlaResumoResponse? Sla { get; init; }
}
