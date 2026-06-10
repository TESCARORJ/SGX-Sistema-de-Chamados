using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class ListarDecisoesAprovacaoChamadoRequest
{
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? ChamadoId { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public TipoDecisaoAprovacaoChamado? TipoDecisao { get; init; }
    public ResultadoDecisaoAprovacaoChamado? Resultado { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public bool? DecisaoParcial { get; init; }
    public bool? DecisaoFinal { get; init; }
    public bool? LiberaAvanco { get; init; }
    public bool? MantemBloqueio { get; init; }
    public bool? ExigeReavaliacao { get; init; }
    public DateTime? DataDecisaoDe { get; init; }
    public DateTime? DataDecisaoAte { get; init; }
    public string? Termo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "datadecisao";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed class RegistrarDecisaoAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public TipoDecisaoAprovacaoChamado TipoDecisao { get; init; }
    public ResultadoDecisaoAprovacaoChamado Resultado { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public string? PapelDecisorSnapshot { get; init; }
    public string? AutoridadeDecisorSnapshot { get; init; }
    public bool DecisorEhAprovadorEspecifico { get; init; }
    public bool DecisorEhAprovadorPadrao { get; init; }
    public bool DecisorEhMembroGrupo { get; init; }
    public bool DecisorPorDelegacao { get; init; }
    public string? GrupoAprovadorSnapshot { get; init; }
    public int? QuorumEsperado { get; init; }
    public int? QuorumAtingido { get; init; }
    public string? Justificativa { get; init; }
    public string? Observacao { get; init; }
    public string? EscopoDecididoSnapshot { get; init; }
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; init; }
    public bool DecisaoParcial { get; init; }
    public bool DecisaoFinal { get; init; }
    public bool LiberaAvanco { get; init; }
    public bool MantemBloqueio { get; init; }
    public bool ExigeReavaliacao { get; init; }
    public bool PermiteNovaSolicitacao { get; init; }
    public bool CancelaFluxo { get; init; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaAnterior { get; init; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaNovo { get; init; }
    public StatusEtapaAprovacaoChamado? StatusEtapaAnterior { get; init; }
    public StatusEtapaAprovacaoChamado? StatusEtapaNovo { get; init; }
    public Guid? StatusChamadoAnteriorId { get; init; }
    public Guid? StatusChamadoNovoId { get; init; }
    public int? NivelSnapshot { get; init; }
    public int? OrdemSnapshot { get; init; }
    public string? RamoSnapshot { get; init; }
    public string? RegraNomeSnapshot { get; init; }
    public int? RegraVersaoSnapshot { get; init; }
    public string? RegraCriterioSnapshot { get; init; }
}

public sealed class AprovarAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public string? PapelDecisorSnapshot { get; init; }
    public string? AutoridadeDecisorSnapshot { get; init; }
    public string? Justificativa { get; init; }
    public string? Observacao { get; init; }
    public string? EscopoDecididoSnapshot { get; init; }
    public bool DecisaoParcial { get; init; }
    public bool DecisaoFinal { get; init; }
    public bool LiberaAvanco { get; init; }
    public bool MantemBloqueio { get; init; }
    public bool ExigeReavaliacao { get; init; }
    public bool PermiteNovaSolicitacao { get; init; }
}

public sealed class ReprovarAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public string? PapelDecisorSnapshot { get; init; }
    public string? AutoridadeDecisorSnapshot { get; init; }
    public string Justificativa { get; init; } = string.Empty;
    public string? Observacao { get; init; }
    public string? EscopoDecididoSnapshot { get; init; }
    public bool DecisaoParcial { get; init; }
    public bool DecisaoFinal { get; init; }
    public bool MantemBloqueio { get; init; } = true;
    public bool ExigeReavaliacao { get; init; }
    public bool PermiteNovaSolicitacao { get; init; }
    public bool CancelaFluxo { get; init; }
}

public sealed class CancelarDecisaoAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public string? Observacao { get; init; }
    public bool CancelaFluxo { get; init; } = true;
    public bool PermiteNovaSolicitacao { get; init; }
    public bool MantemBloqueio { get; init; }
}

public sealed class RegistrarExpiracaoAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? ResponsavelUsuarioId { get; init; }
    public string? ComponenteResponsavel { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public DateTime DataExpiracao { get; init; }
    public bool MantemBloqueio { get; init; } = true;
    public bool ExigeReavaliacao { get; init; }
    public bool PermiteNovaSolicitacao { get; init; }
}

public sealed class SolicitarReavaliacaoAprovacaoChamadoRequest
{
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid? DecisorUsuarioId { get; init; }
    public string? Justificativa { get; init; }
    public string? Observacao { get; init; }
    public string? EscopoDecididoSnapshot { get; init; }
    public bool MantemBloqueio { get; init; }
    public bool ExigeReavaliacao { get; init; } = true;
    public bool PermiteNovaSolicitacao { get; init; } = true;
}

public sealed class ValidarDecisaoAprovacaoChamadoRequest
{
    public Guid? DecisaoAprovacaoChamadoId { get; init; }
    public RegistrarDecisaoAprovacaoChamadoRequest Decisao { get; init; } = new();
}

public sealed class ValidarDecisaoAprovacaoChamadoResponse
{
    public bool Valida { get; init; }
    public IReadOnlyCollection<string> Erros { get; init; } = [];
    public IReadOnlyCollection<string> Alertas { get; init; } = [];
}

public sealed record DecisaoAprovacaoChamadoResumoResponse(
    Guid Id,
    Guid InstanciaAprovacaoChamadoId,
    Guid? EtapaAprovacaoChamadoId,
    Guid? ChamadoId,
    TipoDecisaoAprovacaoChamado TipoDecisao,
    string TipoDecisaoDescricao,
    ResultadoDecisaoAprovacaoChamado Resultado,
    string ResultadoDescricao,
    Guid? DecisorUsuarioId,
    string? DecisorNome,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    bool DecisaoParcial,
    bool DecisaoFinal,
    bool LiberaAvanco,
    bool MantemBloqueio,
    bool ExigeReavaliacao,
    DateTime DataDecisao,
    DateTime CriadoEm);

public sealed record DecisaoAprovacaoChamadoResponse(
    Guid Id,
    Guid InstanciaAprovacaoChamadoId,
    Guid? EtapaAprovacaoChamadoId,
    Guid? ChamadoId,
    TipoDecisaoAprovacaoChamado TipoDecisao,
    string TipoDecisaoDescricao,
    ResultadoDecisaoAprovacaoChamado Resultado,
    string ResultadoDescricao,
    DateTime DataDecisao,
    Guid? DecisorUsuarioId,
    string? DecisorNome,
    string? PapelDecisorSnapshot,
    string? AutoridadeDecisorSnapshot,
    bool DecisorEhAprovadorEspecifico,
    bool DecisorEhAprovadorPadrao,
    bool DecisorEhMembroGrupo,
    bool DecisorPorDelegacao,
    string? GrupoAprovadorSnapshot,
    int? QuorumEsperado,
    int? QuorumAtingido,
    string? Justificativa,
    string? Observacao,
    string? EscopoDecididoSnapshot,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    bool DecisaoParcial,
    bool DecisaoFinal,
    bool LiberaAvanco,
    bool MantemBloqueio,
    bool ExigeReavaliacao,
    bool PermiteNovaSolicitacao,
    bool CancelaFluxo,
    StatusInstanciaAprovacaoChamado StatusInstanciaAnterior,
    string StatusInstanciaAnteriorDescricao,
    StatusInstanciaAprovacaoChamado StatusInstanciaNovo,
    string StatusInstanciaNovoDescricao,
    StatusEtapaAprovacaoChamado? StatusEtapaAnterior,
    string? StatusEtapaAnteriorDescricao,
    StatusEtapaAprovacaoChamado? StatusEtapaNovo,
    string? StatusEtapaNovoDescricao,
    Guid? StatusChamadoAnteriorId,
    string? StatusChamadoAnteriorNome,
    Guid? StatusChamadoNovoId,
    string? StatusChamadoNovoNome,
    int? NivelSnapshot,
    int? OrdemSnapshot,
    string? RamoSnapshot,
    string? RegraNomeSnapshot,
    int? RegraVersaoSnapshot,
    string? RegraCriterioSnapshot,
    Guid CriadoPorUsuarioId,
    Guid? AtualizadoPorUsuarioId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);
