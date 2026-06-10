using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class ListarConfiguracoesRegrasAprovacaoRequest
{
    public string? Termo { get; init; }
    public bool? Ativo { get; init; }
    public TipoRegraAprovacao? TipoRegra { get; init; }
    public EscopoRegraAprovacao? EscopoRegra { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao? TipoResolucaoAprovador { get; init; }
    public bool? Bloqueante { get; init; }
    public bool? ExigeAprovacao { get; init; }
    public DateTime? VigenteEm { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "prioridade";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed class CriarConfiguracaoRegraAprovacaoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public TipoRegraAprovacao TipoRegra { get; init; }
    public EscopoRegraAprovacao EscopoRegra { get; init; }
    public int Ordem { get; init; }
    public int Prioridade { get; init; }
    public int Versao { get; init; } = 1;
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoMinimo { get; init; }
    public UrgenciaChamadoEnum? UrgenciaMinima { get; init; }
    public PrioridadeChamadoEnum? PrioridadeMinima { get; init; }
    public decimal? CustoMinimo { get; init; }
    public int? NivelRiscoMinimo { get; init; }
    public bool ExigeAprovacao { get; init; }
    public bool Bloqueante { get; init; }
    public bool PermiteReenvio { get; init; }
    public bool PermiteFallback { get; init; }
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; init; }
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorEspecificoUsuarioId { get; init; }
    public Guid? AprovadorPadraoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public DateTime? VigenteDe { get; init; }
    public DateTime? VigenteAte { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class AtualizarConfiguracaoRegraAprovacaoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public TipoRegraAprovacao TipoRegra { get; init; }
    public EscopoRegraAprovacao EscopoRegra { get; init; }
    public int Ordem { get; init; }
    public int Prioridade { get; init; }
    public int Versao { get; init; } = 1;
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public ImpactoChamadoEnum? ImpactoMinimo { get; init; }
    public UrgenciaChamadoEnum? UrgenciaMinima { get; init; }
    public PrioridadeChamadoEnum? PrioridadeMinima { get; init; }
    public decimal? CustoMinimo { get; init; }
    public int? NivelRiscoMinimo { get; init; }
    public bool ExigeAprovacao { get; init; }
    public bool Bloqueante { get; init; }
    public bool PermiteReenvio { get; init; }
    public bool PermiteFallback { get; init; }
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; init; }
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; init; }
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; init; }
    public Guid? AprovadorEspecificoUsuarioId { get; init; }
    public Guid? AprovadorPadraoUsuarioId { get; init; }
    public int? PrazoDecisaoHoras { get; init; }
    public DateTime? VigenteDe { get; init; }
    public DateTime? VigenteAte { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class AlterarStatusConfiguracaoRegraAprovacaoRequest
{
    public bool Ativo { get; init; }
}

public sealed class ValidarConfiguracaoRegraAprovacaoRequest
{
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public CriarConfiguracaoRegraAprovacaoRequest Configuracao { get; init; } = new();
}

public sealed class ValidarConfiguracaoRegraAprovacaoResponse
{
    public bool Valida { get; init; }
    public IReadOnlyCollection<string> Erros { get; init; } = [];
    public IReadOnlyCollection<string> Alertas { get; init; } = [];
}

public sealed record ConfiguracaoRegraAprovacaoResumoResponse(
    Guid Id,
    string Nome,
    TipoRegraAprovacao TipoRegra,
    string TipoRegraDescricao,
    EscopoRegraAprovacao EscopoRegra,
    string EscopoRegraDescricao,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador,
    string TipoResolucaoAprovadorDescricao,
    NaturezaChamadoEnum? NaturezaChamado,
    bool ExigeAprovacao,
    bool Bloqueante,
    int Prioridade,
    int Versao,
    bool Ativo,
    DateTime? VigenteDe,
    DateTime? VigenteAte,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record ConfiguracaoRegraAprovacaoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    TipoRegraAprovacao TipoRegra,
    string TipoRegraDescricao,
    EscopoRegraAprovacao EscopoRegra,
    string EscopoRegraDescricao,
    int Ordem,
    int Prioridade,
    int Versao,
    NaturezaChamadoEnum? NaturezaChamado,
    Guid? TipoSolicitacaoId,
    string? TipoSolicitacaoNome,
    Guid? CatalogoServicoId,
    string? CatalogoServicoNome,
    Guid? CategoriaId,
    string? CategoriaNome,
    Guid? SubcategoriaId,
    string? SubcategoriaNome,
    ImpactoChamadoEnum? ImpactoMinimo,
    UrgenciaChamadoEnum? UrgenciaMinima,
    PrioridadeChamadoEnum? PrioridadeMinima,
    decimal? CustoMinimo,
    int? NivelRiscoMinimo,
    bool ExigeAprovacao,
    bool Bloqueante,
    bool PermiteReenvio,
    bool PermiteFallback,
    EfeitoOperacionalRegraAprovacao EfeitoOperacional,
    string EfeitoOperacionalDescricao,
    TipoFluxoAprovacao TipoFluxoAprovacao,
    string TipoFluxoAprovacaoDescricao,
    TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador,
    string TipoResolucaoAprovadorDescricao,
    Guid? AprovadorEspecificoUsuarioId,
    string? AprovadorEspecificoNome,
    Guid? AprovadorPadraoUsuarioId,
    string? AprovadorPadraoNome,
    int? PrazoDecisaoHoras,
    DateTime? VigenteDe,
    DateTime? VigenteAte,
    bool Ativo,
    Guid CriadoPorUsuarioId,
    Guid? AtualizadoPorUsuarioId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);
