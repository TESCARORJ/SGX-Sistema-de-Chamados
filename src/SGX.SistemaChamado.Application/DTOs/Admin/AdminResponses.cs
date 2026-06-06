using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed record AdminUsuarioContextoResponse(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    IReadOnlyCollection<string> Perfis,
    IReadOnlyCollection<string> Permissoes);

public sealed record AtendenteResumoResponse(
    Guid Id,
    string Nome,
    string Email,
    IReadOnlyCollection<string> Perfis);

public sealed record DepartamentoAdminResponse(Guid Id, string Nome, string Sigla);
public sealed record CategoriaAdminResponse(Guid Id, string Nome, Guid? DepartamentoId);
public sealed record SubcategoriaAdminResponse(Guid Id, Guid CategoriaChamadoId, string Nome);
public sealed record PrioridadeAdminResponse(Guid Id, string Nome, int Nivel);
public sealed record TipoSolicitacaoAdminResponse(Guid Id, string Nome);
public sealed record LocalUnidadeAdminResponse(Guid Id, string Nome);
public sealed record StatusAdminResponse(Guid Id, string Nome, int Codigo);

public sealed class AdminContextoResponse
{
    public AdminUsuarioContextoResponse Usuario { get; init; } = default!;
    public IReadOnlyCollection<DepartamentoAdminResponse> Departamentos { get; init; } = [];
    public IReadOnlyCollection<CategoriaAdminResponse> Categorias { get; init; } = [];
    public IReadOnlyCollection<SubcategoriaAdminResponse> Subcategorias { get; init; } = [];
    public IReadOnlyCollection<PrioridadeAdminResponse> Prioridades { get; init; } = [];
    public IReadOnlyCollection<TipoSolicitacaoAdminResponse> TiposSolicitacao { get; init; } = [];
    public IReadOnlyCollection<LocalUnidadeAdminResponse> LocaisUnidade { get; init; } = [];
    public IReadOnlyCollection<StatusAdminResponse> Status { get; init; } = [];
    public IReadOnlyCollection<AtendenteResumoResponse> Atendentes { get; init; } = [];
}

public sealed class ChamadoAdminResumoResponse
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string SolicitanteNome { get; init; } = string.Empty;
    public string SolicitanteEmail { get; init; } = string.Empty;
    public string? ResponsavelNome { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
    public NaturezaChamadoEnum NaturezaChamado { get; init; } = NaturezaChamadoEnum.Requisicao;
    public ImpactoChamadoEnum ImpactoChamado { get; init; } = ImpactoChamadoEnum.Baixo;
    public UrgenciaChamadoEnum UrgenciaChamado { get; init; } = UrgenciaChamadoEnum.Baixa;
    public string Categoria { get; init; } = string.Empty;
    public string? Subcategoria { get; init; }
    public string? TipoSolicitacao { get; init; }
    public string? LocalUnidade { get; init; }
    public string? Departamento { get; init; }
    public Guid? GrupoTecnicoId { get; init; }
    public string? GrupoTecnicoNome { get; init; }
    public Guid? FilaAtendimentoId { get; init; }
    public string? FilaAtendimentoNome { get; init; }
    public Guid CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid PrioridadeId { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? InventarioAtivoId { get; init; }
    public string? InventarioAtivoCodigo { get; init; }
    public string? InventarioAtivoNome { get; init; }
    public DateTime AbertoEm { get; init; }
    public DateTime? AtualizadoEm { get; init; }
    public DateTime? EncerradoEm { get; init; }
    public bool SlaVencido { get; init; }
    public bool SlaProximoVencimento { get; init; }
    public SituacaoSlaChamadoEnum SituacaoSla { get; init; } = SituacaoSlaChamadoEnum.NaoAplicavel;
    public string? PoliticaSlaNome { get; init; }
    public int? TempoRestanteMinutos { get; init; }
    public int? TempoExcedidoMinutos { get; init; }
    public DateTime? PrazoPrimeiraRespostaEm { get; init; }
    public DateTime? PrimeiraRespostaEm { get; init; }
    public DateTime? PrazoResolucaoEm { get; init; }
    public DateTime? ResolvidoEm { get; init; }
    public bool EstaPausado { get; init; }
    public int TotalMinutosPausado { get; init; }
    public bool RequerAprovacao { get; init; }
    public bool AprovacaoPendente { get; init; }
    public StatusAprovacaoChamado? StatusAprovacao { get; init; }
    public Guid? AprovacaoChamadoId { get; init; }
}

public sealed class ListaChamadosAdminResponse
{
    public IReadOnlyCollection<ChamadoAdminResumoResponse> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}

public sealed record SolicitanteAdminResponse(Guid Id, string Nome, string Email);
public sealed record ResponsavelAdminResponse(Guid Id, string Nome, string Email);

public sealed record HistoricoAdminResponse(
    Guid Id,
    int Tipo,
    string TipoDescricao,
    string Descricao,
    DateTime CriadoEm,
    Guid? UsuarioId,
    string? Usuario);

public sealed record EventoSlaAdminResponse(
    Guid Id,
    int TipoEvento,
    string TipoEventoDescricao,
    string Descricao,
    DateTime DataEvento,
    Guid? UsuarioId,
    string? Usuario);

public sealed record ComentarioAdminResponse(
    Guid Id,
    Guid UsuarioId,
    string Usuario,
    string Mensagem,
    bool Interno,
    DateTime CriadoEm);

public sealed record AnexoAdminResponse(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    DateTime CriadoEm,
    Guid UsuarioId,
    string Usuario);

public sealed record SlaAdminResponse(
    string? PoliticaSlaNome,
    string Prioridade,
    DateTime DataInicio,
    DateTime PrazoPrimeiraRespostaEm,
    DateTime? PrimeiraRespostaEm,
    DateTime PrazoResolucaoEm,
    DateTime? ResolvidoEm,
    bool? PrimeiraRespostaCumprida,
    bool? ResolucaoCumprida,
    bool PrimeiraRespostaViolada,
    bool ResolucaoViolada,
    bool EstaVencido,
    bool EstaPausado,
    SituacaoSlaChamadoEnum Situacao,
    int? MinutosPrimeiraResposta,
    int? MinutosResolucao,
    int? TempoRestanteMinutos,
    int? TempoExcedidoMinutos,
    int TotalMinutosPausado,
    bool UsarHorarioComercial,
    string? CalendarioCorporativoNome);

public sealed class ChamadoAdminDetalheResponse
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public SolicitanteAdminResponse Solicitante { get; init; } = default!;
    public ResponsavelAdminResponse? Responsavel { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
    public NaturezaChamadoEnum NaturezaChamado { get; init; } = NaturezaChamadoEnum.Requisicao;
    public ImpactoChamadoEnum ImpactoChamado { get; init; } = ImpactoChamadoEnum.Baixo;
    public UrgenciaChamadoEnum UrgenciaChamado { get; init; } = UrgenciaChamadoEnum.Baixa;
    public string Categoria { get; init; } = string.Empty;
    public string? Subcategoria { get; init; }
    public string? TipoSolicitacao { get; init; }
    public string? LocalUnidade { get; init; }
    public string? Departamento { get; init; }
    public Guid? GrupoTecnicoId { get; init; }
    public string? GrupoTecnicoNome { get; init; }
    public Guid? FilaAtendimentoId { get; init; }
    public string? FilaAtendimentoNome { get; init; }
    public Guid CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid PrioridadeId { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? InventarioAtivoId { get; init; }
    public string? InventarioAtivoCodigo { get; init; }
    public string? InventarioAtivoNome { get; init; }
    public string Origem { get; init; } = string.Empty;
    public DateTime AbertoEm { get; init; }
    public DateTime? EncerradoEm { get; init; }
    public bool RequerAprovacao { get; init; }
    public bool AprovacaoPendente { get; init; }
    public StatusAprovacaoChamado? StatusAprovacao { get; init; }
    public Guid? AprovacaoChamadoId { get; init; }
    public IReadOnlyCollection<int> StatusPermitidosCodigos { get; init; } = [];
    public IReadOnlyCollection<string> AcoesDisponiveisCodigos { get; init; } = [];
    public IReadOnlyCollection<ComentarioAdminResponse> Comentarios { get; init; } = [];
    public IReadOnlyCollection<AnexoAdminResponse> Anexos { get; init; } = [];
    public IReadOnlyCollection<HistoricoAdminResponse> Historico { get; init; } = [];
    public IReadOnlyCollection<EventoSlaAdminResponse> HistoricoSla { get; init; } = [];
    public SlaAdminResponse? Sla { get; init; }
}

public sealed class ChamadoDerivadoAdminResponse
{
    public Guid ChamadoOrigemId { get; init; }
    public string ChamadoOrigemCodigo { get; init; } = string.Empty;
    public Guid ChamadoDerivadoId { get; init; }
    public string ChamadoDerivadoCodigo { get; init; } = string.Empty;
    public Guid? RelacionamentoId { get; init; }
    public TipoRelacionamentoChamadoEnum? TipoRelacionamento { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
}

public sealed record ChamadoArtigoConhecimentoDto(
    Guid ArtigoId,
    string Titulo,
    string Slug,
    string? Resumo,
    int Status,
    string StatusDescricao,
    int Visibilidade,
    string VisibilidadeDescricao,
    Guid? CategoriaId,
    string? CategoriaNome,
    DateTime VinculadoEm,
    Guid VinculadoPorUsuarioId,
    string VinculadoPorUsuario,
    string? Observacao);

public sealed record ArtigoConhecimentoDisponivelParaVinculoDto(
    Guid ArtigoId,
    string Titulo,
    string Slug,
    string? Resumo,
    int Status,
    string StatusDescricao,
    int Visibilidade,
    string VisibilidadeDescricao,
    Guid? CategoriaId,
    string? CategoriaNome,
    string? Tags,
    DateTime? PublicadoEm);
