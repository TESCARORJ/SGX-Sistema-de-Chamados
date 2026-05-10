using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroLogsEmailRequest
{
    public DateTime? DataInicial { get; init; }
    public DateTime? DataFinal { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public StatusProcessamentoEmail? Status { get; init; }
    public string? Remetente { get; init; }
    public Guid? ChamadoId { get; init; }
    public string? CodigoChamado { get; init; }
    public string? Assunto { get; init; }
    public string? MessageId { get; init; }
    public string? Texto { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string? OrdenarPor { get; init; }
    public string? Direcao { get; init; }

    public DateTime? DataInicialEfetiva => DataInicial ?? DataInicio;
    public DateTime? DataFinalEfetiva => DataFinal ?? DataFim;
}

public sealed class LogIntegracaoEmailResumoResponse
{
    public Guid Id { get; init; }
    public string? MessageId { get; init; }
    public DateTime DataRecebimento { get; init; }
    public DateTime? DataProcessamento { get; init; }
    public string Remetente { get; init; } = string.Empty;
    public string? Destinatario { get; init; }
    public string? Assunto { get; init; }
    public StatusProcessamentoEmail StatusProcessamento { get; init; }
    public string StatusProcessamentoLabel { get; init; } = string.Empty;
    public bool TemErro { get; init; }
    public Guid? ChamadoId { get; init; }
    public string? ChamadoCodigo { get; init; }
    public string? ErroResumido { get; init; }
}

public sealed class ListaLogsIntegracaoEmailResponse
{
    public IReadOnlyCollection<LogIntegracaoEmailResumoResponse> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}

public sealed class LogIntegracaoEmailDetalheResponse
{
    public Guid Id { get; init; }
    public string? MessageId { get; init; }
    public string? InReplyTo { get; init; }
    public string? References { get; init; }
    public string Fingerprint { get; init; } = string.Empty;
    public string Remetente { get; init; } = string.Empty;
    public string? Destinatario { get; init; }
    public string? NomeRemetente { get; init; }
    public string? Assunto { get; init; }
    public DateTime DataRecebimento { get; init; }
    public DateTime? DataProcessamento { get; init; }
    public StatusProcessamentoEmail StatusProcessamento { get; init; }
    public string? Erro { get; init; }
    public Guid? ChamadoId { get; init; }
    public string? ChamadoCodigo { get; init; }
    public string? ChamadoTitulo { get; init; }
    public int Tentativas { get; init; }
    public DateTime CriadoEm { get; init; }
    public string CriadoPor { get; init; } = string.Empty;
    public DateTime? AtualizadoEm { get; init; }
    public string? AtualizadoPor { get; init; }
}
