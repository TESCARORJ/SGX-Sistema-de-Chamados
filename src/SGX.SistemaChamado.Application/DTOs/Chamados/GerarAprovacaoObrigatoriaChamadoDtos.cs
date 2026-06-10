using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class GerarAprovacaoObrigatoriaChamadoRequest
{
    public Guid ChamadoId { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoChamado { get; init; }
    public UrgenciaChamadoEnum? UrgenciaChamado { get; init; }
    public PrioridadeChamadoEnum? PrioridadeChamado { get; init; }
    public decimal? Custo { get; init; }
    public int? NivelRisco { get; init; }
    public Guid? SolicitanteId { get; init; }
    public DateTime? DataReferencia { get; init; }
    public bool ForcarReavaliacao { get; init; }
    public string? OrigemSolicitacao { get; init; }
}

public sealed class GerarAprovacaoObrigatoriaChamadoResponse
{
    public bool GerouAprovacao { get; init; }
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public string? NomeRegra { get; init; }
    public int? VersaoRegra { get; init; }
    public bool ExigeAprovacao { get; init; }
    public bool Bloqueante { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao? TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorResolvidoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public DateTime? DeveExpirarEm { get; init; }
    public bool JaExistiaAprovacaoEquivalente { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Avisos { get; init; } = [];
}
