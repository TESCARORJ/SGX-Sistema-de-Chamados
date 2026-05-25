using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class AprovacaoChamadoListagemDto
{
    public Guid Id { get; init; }
    public Guid ChamadoId { get; init; }
    public string NumeroProtocoloChamado { get; init; } = string.Empty;
    public string TituloChamado { get; init; } = string.Empty;
    public StatusAprovacaoChamado Status { get; init; }
    public string StatusDescricao { get; init; } = string.Empty;
    public TipoOrigemAprovacaoChamado TipoOrigem { get; init; }
    public string TipoOrigemDescricao { get; init; } = string.Empty;
    public string? OrigemDescricao { get; init; }
    public Guid? SolicitanteId { get; init; }
    public string? SolicitanteNome { get; init; }
    public Guid? AprovadorId { get; init; }
    public string? AprovadorNome { get; init; }
    public DateTime SolicitadaEm { get; init; }
    public DateTime? DecididaEm { get; init; }
    public bool Ativo { get; init; }
}

public sealed class AprovacaoChamadoDetalheDto
{
    public Guid Id { get; init; }
    public Guid ChamadoId { get; init; }
    public string NumeroProtocoloChamado { get; init; } = string.Empty;
    public string TituloChamado { get; init; } = string.Empty;
    public string? DescricaoChamado { get; init; }
    public StatusAprovacaoChamado Status { get; init; }
    public string StatusDescricao { get; init; } = string.Empty;
    public TipoOrigemAprovacaoChamado TipoOrigem { get; init; }
    public string TipoOrigemDescricao { get; init; } = string.Empty;
    public string? OrigemDescricao { get; init; }
    public Guid? SolicitanteId { get; init; }
    public string? SolicitanteNome { get; init; }
    public Guid? AprovadorId { get; init; }
    public string? AprovadorNome { get; init; }
    public string? JustificativaSolicitacao { get; init; }
    public string? JustificativaDecisao { get; init; }
    public DateTime SolicitadaEm { get; init; }
    public DateTime? DecididaEm { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }
    public bool Ativo { get; init; }
}

public sealed class SolicitarAprovacaoChamadoRequest
{
    public TipoOrigemAprovacaoChamado TipoOrigem { get; init; }
    public string? OrigemDescricao { get; init; }
    public string? JustificativaSolicitacao { get; init; }
}

public sealed class DecidirAprovacaoChamadoRequest
{
    public string? JustificativaDecisao { get; init; }
}

public sealed class CancelarAprovacaoChamadoRequest
{
    public string JustificativaDecisao { get; init; } = string.Empty;
}

public sealed class FiltroAprovacaoChamadoRequest
{
    public Guid? ChamadoId { get; init; }
    public StatusAprovacaoChamado? Status { get; init; }
    public TipoOrigemAprovacaoChamado? TipoOrigem { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? AprovadorId { get; init; }
    public DateTime? DataSolicitacaoInicial { get; init; }
    public DateTime? DataSolicitacaoFinal { get; init; }
    public DateTime? DataDecisaoInicial { get; init; }
    public DateTime? DataDecisaoFinal { get; init; }
    public string? Termo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "solicitadaEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}
