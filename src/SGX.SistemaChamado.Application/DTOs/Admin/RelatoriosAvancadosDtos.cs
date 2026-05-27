using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public class FiltroPeriodoRelatorioRequest
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public DateTime? DataInicial { get; init; }
    public DateTime? DataFinal { get; init; }
    public string? PeriodoPreDefinido { get; init; }

    public DateTime? ObterDataInicial()
        => DataInicial ?? DataInicio;

    public DateTime? ObterDataFinal()
        => DataFinal ?? DataFim;
}

public sealed class FiltroRelatorioChamadosRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? StatusId { get; init; }
    public string? Status { get; init; }
    public Guid? AtendenteId { get; init; }
    public Guid? ResponsavelId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? InventarioAtivoId { get; init; }
    public string? Origem { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public bool? ApenasAtivos { get; init; } = true;
    public AgrupamentoRelatorio Agrupamento { get; init; } = AgrupamentoRelatorio.Dia;
    public AgruparPorRelatorioChamados AgruparPor { get; init; } = AgruparPorRelatorioChamados.Status;
    public int LimiteRanking { get; init; } = 20;

    public Guid? ObterAtendenteId()
        => AtendenteId ?? ResponsavelId;
}

public sealed class FiltroRelatorioSlaRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? StatusId { get; init; }
    public string? Status { get; init; }
    public Guid? AtendenteId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public Guid? PoliticaSlaId { get; init; }
    public string? SituacaoSla { get; init; }
    public bool? ApenasAtivos { get; init; } = true;
    public int LimiteRanking { get; init; } = 20;
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "prazoResolucaoEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed class FiltroRelatorioAprovacoesRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? StatusId { get; init; }
    public string? Status { get; init; }
    public Guid? AtendenteId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public string? TipoOrigemAprovacao { get; init; }
    public string? StatusAprovacao { get; init; }
    public AgruparTempoMedioAprovacoesPor AgruparPor { get; init; } = AgruparTempoMedioAprovacoesPor.TipoOrigem;
    public AgrupamentoRelatorio? Agrupamento { get; init; }
    public bool? ApenasAtivos { get; init; } = true;
}

public sealed class FiltroRelatorioCatalogoServicosRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? StatusId { get; init; }
    public string? Status { get; init; }
    public Guid? AtendenteId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public string? TipoOrigemAprovacao { get; init; }
    public string? StatusAprovacao { get; init; }
    public bool? ApenasAtivos { get; init; } = true;
    public int LimiteRanking { get; init; } = 20;
}

public sealed class FiltroRelatorioInventarioAtivosRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public Guid? TipoAtivoInventarioId { get; init; }
    public string? StatusOperacional { get; init; }
    public string? StatusPatrimonial { get; init; }
    public string? Criticidade { get; init; }
    public bool? Ativo { get; init; }
    public int LimiteRanking { get; init; } = 20;
}

public sealed class FiltroRelatorioBaseConhecimentoRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? CategoriaId { get; init; }
    public string? StatusArtigo { get; init; }
    public string? VisibilidadeArtigo { get; init; }
    public bool? Ativo { get; init; }
    public int LimiteRanking { get; init; } = 20;
}

public sealed class FiltroRelatorioAuditoriaRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? UsuarioId { get; init; }
    public string? Entidade { get; init; }
    public string? TipoAcao { get; init; }
    public string? Termo { get; init; }
    public int LimiteRanking { get; init; } = 20;
}

public sealed class FiltroRelatorioAtendimentoRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? StatusId { get; init; }
    public string? Status { get; init; }
    public Guid? AtendenteId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public Guid? InventarioAtivoId { get; init; }
    public string? Origem { get; init; }
    public bool? ApenasAtivos { get; init; } = true;
    public int LimiteRanking { get; init; } = 20;

    public Guid? ObterAtendenteId()
        => AtendenteId ?? UsuarioResponsavelId;
}

public sealed class FiltroRelatorioDepartamentoRequest : FiltroPeriodoRelatorioRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "nomeDepartamento";
    public string DirecaoOrdenacao { get; init; } = "asc";
}

public sealed class FiltroRelatorioExportacaoRequest
{
    public TipoRelatorioAvancado TipoRelatorio { get; init; }
    public AgrupamentoRelatorio? Agrupamento { get; init; }
    public FormatoExportacaoRelatorio Formato { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
}

public sealed record RelatorioChamadosResumoDto(
    int TotalChamados,
    int TotalAbertos,
    int TotalEmAtendimento,
    int TotalEncerradosOuConcluidos,
    int TotalCancelados,
    int TotalReabertos,
    int TotalComAprovacaoPendente,
    int TotalReprovadosNaAprovacao,
    int TotalComAtivoVinculado,
    double? TempoMedioAtendimentoHoras,
    double? TempoMedioAtePrimeiraAcaoHoras,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorPrioridade,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorDepartamento,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorNatureza,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorCategoria);

public sealed record RelatorioChamadosSerieTemporalDto(
    AgrupamentoRelatorio Agrupamento,
    IReadOnlyCollection<SerieTemporalRelatorioDto> Itens);

public sealed record RelatorioChamadosDistribuicaoDto(
    AgruparPorRelatorioChamados AgruparPor,
    IReadOnlyCollection<DistribuicaoRelatorioDto> Itens);

public sealed record RelatorioAtendimentoProdutividadeDto(
    int LimiteAplicado,
    IReadOnlyCollection<RankingAtendimentoDto> Ranking);

public sealed record RelatorioSlaResumoDto(
    int TotalChamadosComSla,
    int TotalDentroSla,
    int TotalForaSla,
    decimal? PercentualCumprimento,
    decimal? PercentualViolacao,
    double? TempoMedioResolucaoHoras,
    int? ChamadosProximosVencimento,
    int? ChamadosComSlaPausado,
    int? TotalSemSla);

public sealed record RelatorioSlaViolacaoDto(
    Guid ChamadoId,
    string NumeroProtocolo,
    string Titulo,
    NaturezaChamadoEnum NaturezaChamado,
    string Departamento,
    string Prioridade,
    string Status,
    DateTime DataAbertura,
    DateTime? DataLimiteSla,
    DateTime? DataConclusao,
    double? HorasExcedidas,
    ImpactoChamadoEnum ImpactoChamado = ImpactoChamadoEnum.Baixo,
    UrgenciaChamadoEnum UrgenciaChamado = UrgenciaChamadoEnum.Baixa);

public sealed record RelatorioSlaPorDepartamentoDto(
    Guid? DepartamentoId,
    string DepartamentoNome,
    int TotalComSla,
    int DentroSla,
    int ForaSla,
    decimal? PercentualCumprimento);

public sealed record RelatorioSlaPorPrioridadeDto(
    Guid PrioridadeId,
    string PrioridadeNome,
    int TotalComSla,
    int DentroSla,
    int ForaSla,
    decimal? PercentualCumprimento);

public sealed record RelatorioAprovacoesResumoDto(
    int TotalAprovacoes,
    int Pendentes,
    int Aprovadas,
    int Reprovadas,
    int Canceladas,
    decimal? TaxaAprovacao,
    decimal? TaxaReprovacao,
    double? TempoMedioDecisaoHoras);

public sealed record RelatorioAprovacoesTempoMedioDto(
    string Grupo,
    int TotalDecididas,
    double? TempoMedioDecisaoHoras);

public sealed record RelatorioAprovacoesPorOrigemDto(
    string TipoOrigem,
    int Total,
    int Pendentes,
    int Aprovadas,
    int Reprovadas,
    int Canceladas,
    double? TempoMedioDecisaoHoras);

public sealed record RelatorioCatalogoServicosResumoDto(
    int TotalServicos,
    int ServicosPublicados,
    int ServicosArquivados,
    int ServicosAtivos,
    int ServicosQuePermitemAbertura,
    int ServicosQueRequeremAprovacao,
    int ChamadosAbertosPorCatalogo,
    decimal? PercentualChamadosPorCatalogo);

public sealed record RelatorioCatalogoServicosMaisSolicitadosDto(
    Guid CatalogoServicoId,
    string NomeServico,
    string DepartamentoResponsavel,
    int TotalChamados,
    int TotalComAprovacao,
    int? TotalReprovadosNaAprovacao,
    int? TotalForaSla);

public sealed record RelatorioCatalogoServicosPorDepartamentoDto(
    Guid DepartamentoId,
    string DepartamentoNome,
    int TotalServicos,
    int ServicosPublicados,
    int ChamadosAbertos,
    int ServicosQueRequeremAprovacao);

public sealed record RelatorioInventarioAtivosResumoDto(
    int TotalAtivos,
    int AtivosAtivos,
    int AtivosInativos,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorTipo,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorCriticidade,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorStatusOperacional,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorStatusPatrimonial,
    int TotalComChamadosRelacionados,
    int TotalEmManutencao,
    int TotalComDefeito);

public sealed record RelatorioInventarioAtivosPorStatusDto(
    IReadOnlyCollection<DistribuicaoRelatorioDto> PorStatusOperacional,
    IReadOnlyCollection<DistribuicaoRelatorioDto> PorStatusPatrimonial,
    IReadOnlyCollection<DistribuicaoRelatorioDto> PorCriticidade);

public sealed record RelatorioInventarioAtivosChamadosRecorrentesDto(
    Guid InventarioAtivoId,
    string Codigo,
    string Nome,
    string TipoAtivo,
    string Departamento,
    string UsuarioResponsavel,
    int TotalChamados,
    int ChamadosAbertos,
    int ChamadosEncerrados,
    DateTime? UltimoChamadoEm);

public sealed record RelatorioInventarioAtivosPorDepartamentoDto(
    Guid? DepartamentoId,
    string DepartamentoNome,
    int TotalAtivos,
    int AtivosAtivos,
    int AtivosInativos,
    int TotalComChamados,
    int Criticos);

public sealed record RelatorioBaseConhecimentoResumoDto(
    int TotalArtigos,
    int ArtigosPublicados,
    int ArtigosRascunho,
    int ArtigosArquivados,
    int ArtigosAtivos,
    int ArtigosInativos,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorVisibilidade,
    int ArtigosVinculadosChamados,
    int ChamadosComArtigoVinculado);

public sealed record RelatorioBaseConhecimentoPorStatusDto(
    IReadOnlyCollection<DistribuicaoRelatorioDto> PorStatus,
    IReadOnlyCollection<DistribuicaoRelatorioDto> PorVisibilidade);

public sealed record RelatorioBaseConhecimentoVinculosChamadosDto(
    Guid ArtigoId,
    string Titulo,
    string Status,
    string Visibilidade,
    int TotalChamadosVinculados,
    DateTime? UltimoVinculoEm);

public sealed record RelatorioAuditoriaResumoDto(
    int TotalAcoesAuditadas,
    int UsuariosComAcoes,
    int EntidadesAfetadas,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorTipoAcao,
    IReadOnlyCollection<IndicadorRelatorioDto> TotalPorEntidade,
    IReadOnlyCollection<PontoSerieTemporalDto> TotalPorDia);

public sealed record RelatorioAuditoriaPorUsuarioDto(
    Guid? UsuarioId,
    string UsuarioNome,
    int TotalAcoes,
    DateTime? UltimaAcaoEm,
    IReadOnlyCollection<IndicadorRelatorioDto> AcoesPorTipo);

public sealed record RelatorioAuditoriaPorEntidadeDto(
    string Entidade,
    int TotalAcoes,
    int UsuariosDistintos,
    DateTime? UltimaAcaoEm,
    IReadOnlyCollection<IndicadorRelatorioDto> AcoesPorTipo);

public sealed record IndicadorRelatorioDto(
    string Chave,
    string Nome,
    int Quantidade,
    decimal? Percentual = null);

public sealed record DistribuicaoRelatorioDto(
    string Chave,
    string Nome,
    int Quantidade,
    decimal Percentual);

public sealed record SerieTemporalRelatorioDto(
    string Periodo,
    int Abertos,
    int Encerrados,
    int Reabertos);

public sealed record RankingAtendimentoDto(
    Guid AtendenteId,
    string AtendenteNome,
    int ChamadosAssumidos,
    int ChamadosConcluidos,
    int ChamadosEmAberto,
    int ChamadosReabertos,
    double? TempoMedioConclusaoHoras,
    decimal PercentualConclusao);

public sealed record IndicadorResumoDto(
    string Chave,
    string Titulo,
    decimal Valor,
    string? Unidade,
    string? Descricao);

public sealed record SerieTemporalDto(
    string Chave,
    string Titulo,
    IReadOnlyCollection<PontoSerieTemporalDto> Pontos);

public sealed record PontoSerieTemporalDto(
    DateTime Referencia,
    decimal Valor,
    string? Rotulo = null);

public sealed record DistribuicaoDto(
    string Chave,
    string Titulo,
    IReadOnlyCollection<ItemDistribuicaoDto> Itens);

public sealed record ItemDistribuicaoDto(
    string Nome,
    decimal Valor,
    decimal? Percentual = null);

public sealed record RankingDto(
    string Chave,
    string Titulo,
    IReadOnlyCollection<ItemRankingDto> Itens);

public sealed record ItemRankingDto(
    int Posicao,
    string Nome,
    decimal Valor,
    string? Descricao = null);

public sealed record RelatorioMetadadosDto(
    IReadOnlyCollection<string> PeriodosSuportados,
    IReadOnlyCollection<TipoRelatorioAvancado> TiposRelatorioDisponiveis,
    IReadOnlyCollection<AgrupamentoRelatorio> AgrupamentosSuportados,
    IReadOnlyCollection<string> FiltrosDisponiveis,
    IReadOnlyCollection<FormatoExportacaoRelatorio> FormatosExportacaoPlanejados,
    IReadOnlyCollection<string> PermissoesRelevantes);

public enum TipoRelatorioAvancado
{
    Chamados = 1,
    Sla = 2,
    Atendimento = 3,
    Departamentos = 4,
    CatalogoServicos = 5,
    Aprovacoes = 6,
    InventarioAtivos = 7,
    BaseConhecimento = 8,
    Auditoria = 9
}

public enum AgrupamentoRelatorio
{
    Dia = 1,
    Semana = 2,
    Mes = 3,
    Trimestre = 4,
    Ano = 5,
    Departamento = 6,
    Categoria = 7,
    Prioridade = 8,
    Responsavel = 9,
    Status = 10,
    CatalogoServico = 11,
    Solicitante = 12,
    AtivoVinculado = 13,
    Atendente = 14
}

public enum AgruparPorRelatorioChamados
{
    Status = 1,
    Prioridade = 2,
    Departamento = 3,
    Categoria = 4,
    CatalogoServico = 5,
    Atendente = 6,
    Solicitante = 7,
    AtivoVinculado = 8,
    Natureza = 9
}

public enum AgruparTempoMedioAprovacoesPor
{
    TipoOrigem = 1,
    Departamento = 2,
    Periodo = 3
}

public enum FormatoExportacaoRelatorio
{
    Csv = 1,
    Xlsx = 2,
    Pdf = 3,
    Json = 4
}

