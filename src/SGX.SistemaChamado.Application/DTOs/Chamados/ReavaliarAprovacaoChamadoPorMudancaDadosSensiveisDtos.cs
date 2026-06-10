using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest
{
    public Guid ChamadoId { get; init; }
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public Guid? UsuarioId { get; init; }
    public NaturezaChamadoEnum? NaturezaAnterior { get; init; }
    public NaturezaChamadoEnum? NaturezaNova { get; init; }
    public Guid? TipoSolicitacaoAnteriorId { get; init; }
    public Guid? TipoSolicitacaoNovoId { get; init; }
    public Guid? CatalogoServicoAnteriorId { get; init; }
    public Guid? CatalogoServicoNovoId { get; init; }
    public Guid? CategoriaAnteriorId { get; init; }
    public Guid? CategoriaNovaId { get; init; }
    public Guid? SubcategoriaAnteriorId { get; init; }
    public Guid? SubcategoriaNovaId { get; init; }
    public ImpactoChamadoEnum? ImpactoAnterior { get; init; }
    public ImpactoChamadoEnum? ImpactoNovo { get; init; }
    public UrgenciaChamadoEnum? UrgenciaAnterior { get; init; }
    public UrgenciaChamadoEnum? UrgenciaNova { get; init; }
    public PrioridadeChamadoEnum? PrioridadeAnterior { get; init; }
    public PrioridadeChamadoEnum? PrioridadeNova { get; init; }
    public decimal? CustoAnterior { get; init; }
    public decimal? CustoNovo { get; init; }
    public int? NivelRiscoAnterior { get; init; }
    public int? NivelRiscoNovo { get; init; }
    public string? EscopoAnteriorSnapshot { get; init; }
    public string? EscopoNovoSnapshot { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public DateTime? DataReferencia { get; init; }
}

public sealed class ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
{
    public bool ReavaliacaoExecutada { get; init; }
    public bool ReavaliacaoNecessaria { get; init; }
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public Guid? DecisaoAprovacaoChamadoId { get; init; }
    public StatusInstanciaAprovacaoChamado? StatusInstanciaAnterior { get; init; }
    public StatusInstanciaAprovacaoChamado? StatusInstanciaNovo { get; init; }
    public IReadOnlyCollection<string> MudancasSensiveisDetectadas { get; init; } = [];
    public bool ExigeNovaAprovacao { get; init; }
    public bool MantemBloqueio { get; init; }
    public bool PermiteContinuar { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Avisos { get; init; } = [];
}
