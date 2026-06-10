using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class ListarInstanciasAprovacaoChamadoRequest
{
    public Guid? ChamadoId { get; init; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public Guid? AprovacaoChamadoLegadaId { get; init; }
    public StatusInstanciaAprovacaoChamado? Status { get; init; }
    public OrigemInstanciaAprovacaoChamado? Origem { get; init; }
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public EscopoRegraAprovacao? EscopoRegra { get; init; }
    public TipoRegraAprovacao? TipoRegra { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoAvaliado { get; init; }
    public UrgenciaChamadoEnum? UrgenciaAvaliada { get; init; }
    public PrioridadeChamadoEnum? PrioridadeAvaliada { get; init; }
    public bool? ExigeAprovacao { get; init; }
    public bool? Bloqueante { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao? TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorResolvidoUsuarioId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public DateTime? SolicitadaDe { get; init; }
    public DateTime? SolicitadaAte { get; init; }
    public DateTime? DeveExpirarDe { get; init; }
    public DateTime? DeveExpirarAte { get; init; }
    public bool ApenasPendentes { get; init; }
    public bool ApenasBloqueantes { get; init; }
    public string? Termo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "solicitadaem";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed class PrepararInstanciaAprovacaoChamadoRequest
{
    public Guid ChamadoId { get; init; }
    public Guid SolicitanteId { get; init; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public Guid? AprovacaoChamadoLegadaId { get; init; }
    public string? Titulo { get; init; }
    public string? Descricao { get; init; }
    public OrigemInstanciaAprovacaoChamado Origem { get; init; } = OrigemInstanciaAprovacaoChamado.Manual;
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public EscopoRegraAprovacao? EscopoRegra { get; init; }
    public TipoRegraAprovacao? TipoRegra { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoAvaliado { get; init; }
    public UrgenciaChamadoEnum? UrgenciaAvaliada { get; init; }
    public PrioridadeChamadoEnum? PrioridadeAvaliada { get; init; }
    public decimal? CustoAvaliado { get; init; }
    public int? NivelRiscoAvaliado { get; init; }
    public bool? ExigeAprovacao { get; init; }
    public bool? Bloqueante { get; init; }
    public bool PermiteReenvio { get; init; }
    public bool PermiteFallback { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao? TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorEspecificoUsuarioId { get; init; }
    public Guid? AprovadorPadraoUsuarioId { get; init; }
    public Guid? AprovadorResolvidoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public DateTime? SolicitadaEm { get; init; }
    public DateTime? DeveExpirarEm { get; init; }
    public string? RegraNomeSnapshot { get; init; }
    public int? RegraVersaoSnapshot { get; init; }
    public string? RegraCriterioSnapshot { get; init; }
}

public sealed class CriarInstanciaAprovacaoChamadoManualRequest
{
    public Guid ChamadoId { get; init; }
    public Guid SolicitanteId { get; init; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public Guid? AprovacaoChamadoLegadaId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public OrigemInstanciaAprovacaoChamado Origem { get; init; } = OrigemInstanciaAprovacaoChamado.Manual;
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; init; } = TipoFluxoAprovacao.Simples;
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; init; } = EfeitoOperacionalRegraAprovacao.ExigirAprovacao;
    public EscopoRegraAprovacao EscopoRegra { get; init; } = EscopoRegraAprovacao.AtendimentoChamado;
    public TipoRegraAprovacao TipoRegra { get; init; } = TipoRegraAprovacao.Geral;
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoAvaliado { get; init; }
    public UrgenciaChamadoEnum? UrgenciaAvaliada { get; init; }
    public PrioridadeChamadoEnum? PrioridadeAvaliada { get; init; }
    public decimal? CustoAvaliado { get; init; }
    public int? NivelRiscoAvaliado { get; init; }
    public bool ExigeAprovacao { get; init; } = true;
    public bool Bloqueante { get; init; }
    public bool PermiteReenvio { get; init; }
    public bool PermiteFallback { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; init; } = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido;
    public Guid? AprovadorEspecificoUsuarioId { get; init; }
    public Guid? AprovadorPadraoUsuarioId { get; init; }
    public Guid? AprovadorResolvidoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public DateTime? DeveExpirarEm { get; init; }
    public string? RegraNomeSnapshot { get; init; }
    public int? RegraVersaoSnapshot { get; init; }
    public string? RegraCriterioSnapshot { get; init; }
}

public sealed class ValidarInstanciaAprovacaoChamadoRequest
{
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public PrepararInstanciaAprovacaoChamadoRequest Instancia { get; init; } = new();
}

public sealed class ValidarInstanciaAprovacaoChamadoResponse
{
    public bool Valida { get; init; }
    public IReadOnlyCollection<string> Erros { get; init; } = [];
    public IReadOnlyCollection<string> Alertas { get; init; } = [];
}

public sealed class PrepararInstanciaAprovacaoChamadoResponse
{
    public bool PodeCriar { get; init; }
    public IReadOnlyCollection<string> Erros { get; init; } = [];
    public IReadOnlyCollection<string> Alertas { get; init; } = [];
    public InstanciaAprovacaoChamadoResponse? Instancia { get; init; }
}

public sealed record InstanciaAprovacaoChamadoEtapaResumoResponse(
    Guid Id,
    StatusEtapaAprovacaoChamado Status,
    string StatusDescricao,
    TipoEtapaAprovacaoChamado TipoEtapa,
    string TipoEtapaDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    int Ordem,
    int Nivel,
    string? Ramo,
    bool Obrigatoria,
    bool Critica,
    Guid? AprovadorUsuarioId,
    string? AprovadorNome,
    DateTime? DeveExpirarEm,
    DateTime? DecididaEm);

public sealed record InstanciaAprovacaoChamadoDecisaoResumoResponse(
    Guid Id,
    Guid InstanciaAprovacaoChamadoId,
    Guid? EtapaAprovacaoChamadoId,
    TipoDecisaoAprovacaoChamado TipoDecisao,
    string TipoDecisaoDescricao,
    ResultadoDecisaoAprovacaoChamado Resultado,
    string ResultadoDescricao,
    DateTime DataDecisao,
    Guid? DecisorUsuarioId,
    string? DecisorNome,
    bool DecisaoParcial,
    bool DecisaoFinal,
    bool LiberaAvanco,
    bool MantemBloqueio,
    bool ExigeReavaliacao,
    bool CancelaFluxo);

public sealed record InstanciaAprovacaoChamadoResumoResponse(
    Guid Id,
    Guid ChamadoId,
    Guid? ConfiguracaoRegraAprovacaoId,
    Guid? AprovacaoChamadoLegadaId,
    string Titulo,
    StatusInstanciaAprovacaoChamado Status,
    string StatusDescricao,
    OrigemInstanciaAprovacaoChamado Origem,
    string OrigemDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    bool ExigeAprovacao,
    bool Bloqueante,
    Guid SolicitanteId,
    string? SolicitanteNome,
    Guid? AprovadorResolvidoUsuarioId,
    string? AprovadorResolvidoNome,
    DateTime SolicitadaEm,
    DateTime? DeveExpirarEm,
    DateTime? DecididaEm,
    string? RegraNomeSnapshot,
    int? RegraVersaoSnapshot,
    int QuantidadeEtapas,
    int QuantidadeDecisoes,
    bool PossuiPendencia,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record InstanciaAprovacaoChamadoResponse(
    Guid Id,
    Guid ChamadoId,
    Guid? ConfiguracaoRegraAprovacaoId,
    string? ConfiguracaoRegraNome,
    Guid? AprovacaoChamadoLegadaId,
    string? StatusAprovacaoLegada,
    string Titulo,
    string? Descricao,
    StatusInstanciaAprovacaoChamado Status,
    string StatusDescricao,
    OrigemInstanciaAprovacaoChamado Origem,
    string OrigemDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    EscopoRegraAprovacao EscopoRegra,
    string EscopoRegraDescricao,
    TipoRegraAprovacao TipoRegra,
    string TipoRegraDescricao,
    NaturezaChamadoEnum? NaturezaChamado,
    Guid? TipoSolicitacaoId,
    string? TipoSolicitacaoNome,
    Guid? CatalogoServicoId,
    string? CatalogoServicoNome,
    Guid? CategoriaId,
    string? CategoriaNome,
    Guid? SubcategoriaId,
    string? SubcategoriaNome,
    ImpactoChamadoEnum? ImpactoAvaliado,
    UrgenciaChamadoEnum? UrgenciaAvaliada,
    PrioridadeChamadoEnum? PrioridadeAvaliada,
    decimal? CustoAvaliado,
    int? NivelRiscoAvaliado,
    bool ExigeAprovacao,
    bool Bloqueante,
    bool PermiteReenvio,
    bool PermiteFallback,
    TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador,
    string TipoResolucaoAprovadorDescricao,
    Guid? AprovadorEspecificoUsuarioId,
    string? AprovadorEspecificoNome,
    Guid? AprovadorPadraoUsuarioId,
    string? AprovadorPadraoNome,
    Guid? AprovadorResolvidoUsuarioId,
    string? AprovadorResolvidoNome,
    Guid SolicitanteId,
    string? SolicitanteNome,
    DateTime SolicitadaEm,
    int? PrazoDecisaoHoras,
    DateTime? DeveExpirarEm,
    DateTime? ExpiradaEm,
    DateTime? CanceladaEm,
    Guid? CanceladaPorUsuarioId,
    string? CanceladaPorNome,
    string? MotivoCancelamento,
    DateTime? DecididaEm,
    string? RegraNomeSnapshot,
    int? RegraVersaoSnapshot,
    string? RegraCriterioSnapshot,
    IReadOnlyCollection<InstanciaAprovacaoChamadoEtapaResumoResponse> Etapas,
    IReadOnlyCollection<InstanciaAprovacaoChamadoDecisaoResumoResponse> Decisoes,
    int QuantidadeEtapas,
    int QuantidadeDecisoes,
    bool PossuiPendencia,
    Guid CriadoPorUsuarioId,
    string? CriadoPorNome,
    Guid? AtualizadoPorUsuarioId,
    string? AtualizadoPorNome,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);
