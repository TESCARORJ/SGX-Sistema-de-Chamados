using SGX.SistemaChamado.Domain.Enums;

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
    string? PoliticaSlaNome,
    string Prioridade,
    DateTime DataInicio,
    DateTime PrazoPrimeiraRespostaEm,
    DateTime? PrimeiraRespostaEm,
    DateTime PrazoResolucaoEm,
    DateTime? ResolvidoEm,
    bool? PrimeiraRespostaCumprida,
    bool? ResolucaoCumprida,
    bool PrimeiraRespostaViolada,
    bool ResolucaoViolada,
    bool EstaVencido,
    bool EstaPausado,
    SituacaoSlaChamadoEnum Situacao,
    int? MinutosPrimeiraResposta,
    int? MinutosResolucao,
    int? TempoRestanteMinutos,
    int? TempoExcedidoMinutos,
    int TotalMinutosPausado,
    bool UsarHorarioComercial,
    string? CalendarioCorporativoNome);

public sealed class ChamadoDetalheResponse
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
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
    public string Solicitante { get; init; } = string.Empty;
    public string? Responsavel { get; init; }
    public DateTime AbertoEm { get; init; }
    public DateTime? EncerradoEm { get; init; }
    public IReadOnlyCollection<ComentarioChamadoResponse> Comentarios { get; init; } = [];
    public IReadOnlyCollection<AnexoChamadoResponse> Anexos { get; init; } = [];
    public IReadOnlyCollection<HistoricoChamadoResponse> Historico { get; init; } = [];
    public SlaResumoResponse? Sla { get; init; }
}
