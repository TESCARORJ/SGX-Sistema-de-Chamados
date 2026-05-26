using SGX.SistemaChamado.Domain.Enums;
using System.Text.Json.Serialization;

namespace SGX.SistemaChamado.Application.DTOs.Auditoria;

public sealed class FiltroEventosAuditoriaRequest
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public Guid? UsuarioId { get; init; }
    public string? UsuarioEmail { get; init; }
    public string? Modulo { get; init; }
    public string? Entidade { get; init; }
    public string? EntidadeId { get; init; }
    public TipoAcaoAuditoria? Acao { get; init; }
    public NivelAuditoria? Nivel { get; init; }
    public bool? Sucesso { get; init; }
    public string? IpOrigem { get; init; }
    public string? CorrelacaoId { get; init; }
    public string? Texto { get; init; }
    public string? Provedor { get; init; }
    public string? TipoEventoAutenticacao { get; init; }
    public ResultadoEventoAutenticacao? ResultadoAutenticacao { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
}

public sealed class FiltroDashboardAuditoriaRequest
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public string? Modulo { get; init; }
    public string? UsuarioEmail { get; init; }
    public NivelAuditoria? Nivel { get; init; }
    public bool? Sucesso { get; init; }
}

public sealed record EventoAuditoriaResumoResponse(
    Guid Id,
    DateTime DataEvento,
    string? UsuarioNome,
    string? UsuarioEmail,
    string Modulo,
    string Entidade,
    string? EntidadeId,
    TipoAcaoAuditoria Acao,
    string Descricao,
    NivelAuditoria Nivel,
    bool Sucesso,
    string? IpOrigem,
    string? CorrelacaoId)
{
    [JsonIgnore]
    public string? Metadados { get; init; }
}

public sealed record AuditoriaAgrupamentoResponse(
    string Chave,
    int Total);

public sealed record AuditoriaAgrupamentoDiaResponse(
    DateTime Dia,
    int Total);

public sealed class AuditoriaDashboardResponse
{
    public int TotalEventos { get; init; }
    public int TotalEventosCriticos { get; init; }
    public int TotalEventosAlerta { get; init; }
    public int TotalEventosInformacao { get; init; }
    public int TotalFalhas { get; init; }
    public int TotalSucessos { get; init; }
    public IReadOnlyCollection<AuditoriaAgrupamentoResponse> EventosPorModulo { get; init; } = [];
    public IReadOnlyCollection<AuditoriaAgrupamentoResponse> EventosPorAcao { get; init; } = [];
    public IReadOnlyCollection<AuditoriaAgrupamentoResponse> EventosPorUsuario { get; init; } = [];
    public IReadOnlyCollection<AuditoriaAgrupamentoDiaResponse> EventosPorDia { get; init; } = [];
    public IReadOnlyCollection<EventoAuditoriaResumoResponse> UltimosEventosCriticos { get; init; } = [];
    public IReadOnlyCollection<EventoAuditoriaResumoResponse> UltimasFalhas { get; init; } = [];
}

public sealed class ListaEventosAuditoriaResponse
{
    public IReadOnlyCollection<EventoAuditoriaResumoResponse> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}

public sealed record EventoAuditoriaAutenticacaoResumoResponse(
    Guid Id,
    DateTime DataEvento,
    string? UsuarioNome,
    string? UsuarioEmail,
    string Provedor,
    string TipoEvento,
    string Resultado,
    string? IpOrigem,
    string Mensagem);

public sealed class ListaEventosAuditoriaAutenticacaoResponse
{
    public IReadOnlyCollection<EventoAuditoriaAutenticacaoResumoResponse> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}
