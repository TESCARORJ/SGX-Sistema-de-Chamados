using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class ContextoAvaliacaoRegraAprovacaoRequest
{
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
    public DateTime? DataReferencia { get; init; }
}

public sealed record RegraAprovacaoCandidataResponse(
    Guid ConfiguracaoRegraAprovacaoId,
    string NomeRegra,
    int VersaoRegra,
    int Prioridade,
    int Ordem,
    int Especificidade,
    bool ExigeAprovacao,
    bool Bloqueante,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador,
    string TipoResolucaoAprovadorDescricao,
    Guid? AprovadorEspecificoUsuarioId,
    Guid? AprovadorPadraoUsuarioId,
    int? PrazoDecisaoHoras,
    string Motivo);

public sealed class AvaliacaoConfiguracaoRegraAprovacaoResponse
{
    public bool RegraAplicavel { get; init; }
    public RegraAprovacaoCandidataResponse? MelhorRegra { get; init; }
    public IReadOnlyCollection<RegraAprovacaoCandidataResponse> RegrasCandidatas { get; init; } = [];
    public bool ExigeAprovacao { get; init; }
    public bool Bloqueante { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao? TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorEspecificoUsuarioId { get; init; }
    public Guid? AprovadorPadraoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public string? Motivo { get; init; }
    public IReadOnlyCollection<string> Avisos { get; init; } = [];
}
