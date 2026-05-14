using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroPoliticaSlaRequest
{
    public bool? Ativo { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public string? Texto { get; init; }
}

public sealed record MetaSlaResponse(
    Guid Id,
    Guid PrioridadeId,
    string PrioridadeNome,
    int PrioridadeNivel,
    int TempoPrimeiraRespostaMinutos,
    int TempoResolucaoMinutos,
    int? TempoAtualizacaoMinutos,
    int? TempoRespostaSubsequenteMinutos,
    bool Ativo);

public sealed record PoliticaSlaResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    bool Ativo,
    int Ordem,
    Guid? CategoriaId,
    string? CategoriaNome,
    Guid? DepartamentoId,
    string? DepartamentoNome,
    bool UsarHorarioComercial,
    Guid? CalendarioCorporativoId,
    string? CalendarioCorporativoNome,
    bool PausarQuandoAguardandoSolicitante,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor,
    IReadOnlyCollection<MetaSlaResponse> Metas);

public sealed class MetaSlaUpsertRequest
{
    public Guid? Id { get; init; }
    public Guid PrioridadeId { get; init; }
    public int TempoPrimeiraRespostaMinutos { get; init; }
    public int TempoResolucaoMinutos { get; init; }
    public int? TempoAtualizacaoMinutos { get; init; }
    public int? TempoRespostaSubsequenteMinutos { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class CriarPoliticaSlaRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; } = true;
    public int Ordem { get; init; } = 1;
    public Guid? CategoriaId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? CalendarioCorporativoId { get; init; }
    public bool UsarHorarioComercial { get; init; }
    public bool PausarQuandoAguardandoSolicitante { get; init; } = true;
    public IReadOnlyCollection<MetaSlaUpsertRequest> Metas { get; init; } = [];
}

public sealed class AtualizarPoliticaSlaRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; } = true;
    public int Ordem { get; init; } = 1;
    public Guid? CategoriaId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? CalendarioCorporativoId { get; init; }
    public bool UsarHorarioComercial { get; init; }
    public bool PausarQuandoAguardandoSolicitante { get; init; } = true;
    public IReadOnlyCollection<MetaSlaUpsertRequest> Metas { get; init; } = [];
}

public sealed class AtualizarStatusPoliticaSlaRequest
{
    public bool Ativo { get; init; }
}

public sealed record ConfiguracaoAlertaSlaResponse(
    Guid Id,
    bool Ativo,
    int MinutosAntesVencimentoPrimeiraResposta,
    int MinutosAntesVencimentoResolucao,
    bool NotificarAtendente,
    bool NotificarGestor,
    bool NotificarDepartamento,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor);

public sealed class AtualizarConfiguracaoAlertaSlaRequest
{
    public bool Ativo { get; init; } = true;
    public int MinutosAntesVencimentoPrimeiraResposta { get; init; } = 30;
    public int MinutosAntesVencimentoResolucao { get; init; } = 120;
    public bool NotificarAtendente { get; init; } = true;
    public bool NotificarGestor { get; init; }
    public bool NotificarDepartamento { get; init; }
}

public sealed class FiltroDashboardSlaRequest
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public SituacaoSlaChamadoEnum? SituacaoSla { get; init; }
}

public sealed record SlaAgrupamentoResponse(Guid? Id, string Nome, int Total, int Vencidos, int Proximos, int Cumpridos, int Violados);

public sealed class SlaDashboardResponse
{
    public int TotalComSlaAplicado { get; init; }
    public int TotalVencidos { get; init; }
    public int TotalProximosDoVencimento { get; init; }
    public int TotalDentroDoPrazo { get; init; }
    public int TotalCumpridos { get; init; }
    public int TotalViolados { get; init; }
    public decimal PercentualCumprimento { get; init; }
    public double? TempoMedioPrimeiraRespostaMinutos { get; init; }
    public double? TempoMedioResolucaoMinutos { get; init; }
    public IReadOnlyCollection<SlaAgrupamentoResponse> PorPrioridade { get; init; } = [];
    public IReadOnlyCollection<SlaAgrupamentoResponse> PorCategoria { get; init; } = [];
    public IReadOnlyCollection<SlaAgrupamentoResponse> PorDepartamento { get; init; } = [];
}

public sealed record SlaRelatorioItemResponse(
    Guid ChamadoId,
    string Codigo,
    string Titulo,
    string Prioridade,
    string Categoria,
    string? Departamento,
    string? PoliticaSlaNome,
    DateTime PrazoPrimeiraResposta,
    DateTime? DataPrimeiraResposta,
    bool? PrimeiraRespostaCumprida,
    DateTime PrazoResolucao,
    DateTime? DataResolucao,
    bool? ResolucaoCumprida,
    SituacaoSlaChamadoEnum SituacaoAtual,
    int? MinutosPrimeiraResposta,
    int? MinutosResolucao,
    int MinutosPausados);

public sealed record HorarioAtendimentoCalendarioResponse(
    Guid Id,
    DayOfWeek DiaSemana,
    string DiaSemanaNome,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    bool Ativo);

public sealed record ExcecaoCalendarioCorporativoResponse(
    Guid Id,
    DateOnly Data,
    TipoExcecaoCalendarioCorporativo Tipo,
    string TipoDescricao,
    string? Descricao,
    TimeOnly? HoraInicio,
    TimeOnly? HoraFim,
    bool Ativo);

public sealed record CalendarioCorporativoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    bool Ativo,
    bool Padrao,
    string TimeZone,
    DateTime CriadoEm,
    string CriadoPor,
    DateTime? AtualizadoEm,
    string? AtualizadoPor,
    IReadOnlyCollection<HorarioAtendimentoCalendarioResponse> HorariosAtendimento,
    IReadOnlyCollection<ExcecaoCalendarioCorporativoResponse> Excecoes);

public sealed class CriarCalendarioCorporativoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; } = true;
    public bool Padrao { get; init; }
    public string TimeZone { get; init; } = "America/Sao_Paulo";
}

public sealed class AtualizarCalendarioCorporativoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string TimeZone { get; init; } = "America/Sao_Paulo";
}

public sealed class AtualizarStatusCalendarioCorporativoRequest
{
    public bool Ativo { get; init; }
}

public sealed class HorarioAtendimentoCalendarioRequest
{
    public DayOfWeek DiaSemana { get; init; }
    public TimeOnly HoraInicio { get; init; }
    public TimeOnly HoraFim { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class ExcecaoCalendarioCorporativoRequest
{
    public DateOnly Data { get; init; }
    public TipoExcecaoCalendarioCorporativo Tipo { get; init; }
    public string? Descricao { get; init; }
    public TimeOnly? HoraInicio { get; init; }
    public TimeOnly? HoraFim { get; init; }
    public bool Ativo { get; init; } = true;
}
