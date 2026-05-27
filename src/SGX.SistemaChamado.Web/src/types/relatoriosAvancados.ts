import type { NaturezaChamado } from './portal'

export type TipoRelatorioAvancado =
  | 1
  | 2
  | 3
  | 4
  | 5
  | 6
  | 7
  | 8
  | 9

export type AgrupamentoRelatorio =
  | 1
  | 2
  | 3
  | 4
  | 5
  | 6
  | 7
  | 8
  | 9
  | 10
  | 11
  | 12
  | 13
  | 14

export type AgruparPorRelatorioChamados = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
export type AgruparTempoMedioAprovacoesPor = 1 | 2 | 3

export interface RelatoriosAvancadosMetadados {
  periodosSuportados: string[]
  tiposRelatorioDisponiveis: TipoRelatorioAvancado[]
  agrupamentosSuportados: AgrupamentoRelatorio[]
  filtrosDisponiveis: string[]
  formatosExportacaoPlanejados: number[]
  permissoesRelevantes: string[]
}

export interface FiltroRelatorioBase {
  dataInicio?: string
  dataFim?: string
  dataInicial?: string
  dataFinal?: string
  periodoPreDefinido?: string
}

export interface FiltroRelatorioChamados extends FiltroRelatorioBase {
  departamentoId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadeId?: string
  statusId?: string
  status?: string
  atendenteId?: string
  responsavelId?: string
  solicitanteId?: string
  catalogoServicoId?: string
  inventarioAtivoId?: string
  origem?: string
  naturezaChamado?: NaturezaChamado
  apenasAtivos?: boolean
  agrupamento?: AgrupamentoRelatorio
  agruparPor?: AgruparPorRelatorioChamados
  limiteRanking?: number
}

export interface FiltroRelatorioSla extends FiltroRelatorioBase {
  departamentoId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadeId?: string
  statusId?: string
  status?: string
  atendenteId?: string
  solicitanteId?: string
  catalogoServicoId?: string
  naturezaChamado?: NaturezaChamado
  politicaSlaId?: string
  situacaoSla?: string
  apenasAtivos?: boolean
  limiteRanking?: number
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: 'asc' | 'desc'
}

export interface FiltroRelatorioAprovacoes extends FiltroRelatorioBase {
  departamentoId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadeId?: string
  statusId?: string
  status?: string
  atendenteId?: string
  solicitanteId?: string
  catalogoServicoId?: string
  tipoOrigemAprovacao?: string
  statusAprovacao?: string
  agruparPor?: AgruparTempoMedioAprovacoesPor
  agrupamento?: AgrupamentoRelatorio
  apenasAtivos?: boolean
}

export interface FiltroRelatorioCatalogo extends FiltroRelatorioBase {
  departamentoId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadeId?: string
  statusId?: string
  status?: string
  atendenteId?: string
  solicitanteId?: string
  catalogoServicoId?: string
  tipoOrigemAprovacao?: string
  statusAprovacao?: string
  apenasAtivos?: boolean
  limiteRanking?: number
}

export interface FiltroRelatorioInventario extends FiltroRelatorioBase {
  departamentoId?: string
  localUnidadeId?: string
  usuarioResponsavelId?: string
  tipoAtivoInventarioId?: string
  statusOperacional?: string
  statusPatrimonial?: string
  criticidade?: string
  ativo?: boolean
  limiteRanking?: number
}

export interface FiltroRelatorioBaseConhecimento extends FiltroRelatorioBase {
  categoriaId?: string
  statusArtigo?: string
  visibilidadeArtigo?: string
  ativo?: boolean
  limiteRanking?: number
}

export interface FiltroRelatorioAuditoria extends FiltroRelatorioBase {
  usuarioId?: string
  entidade?: string
  tipoAcao?: string
  termo?: string
  limiteRanking?: number
}

export interface IndicadorResumo {
  chave: string
  nome: string
  quantidade: number
  percentual?: number | null
}

export interface DistribuicaoRelatorio {
  chave: string
  nome: string
  quantidade: number
  percentual: number
}

export interface SerieTemporalRelatorio {
  periodo: string
  abertos: number
  encerrados: number
  reabertos: number
}

export interface RankingRelatorio {
  chave: string
  nome: string
  quantidade: number
  percentual?: number | null
}

export interface RelatorioChamadosResumo {
  totalChamados: number
  totalAbertos: number
  totalEmAtendimento: number
  totalEncerradosOuConcluidos: number
  totalCancelados: number
  totalReabertos: number
  totalComAprovacaoPendente: number
  totalReprovadosNaAprovacao: number
  totalComAtivoVinculado: number
  tempoMedioAtendimentoHoras: number | null
  tempoMedioAtePrimeiraAcaoHoras: number | null
  totalPorPrioridade: IndicadorResumo[]
  totalPorDepartamento: IndicadorResumo[]
  totalPorNatureza: IndicadorResumo[]
  totalPorCategoria: IndicadorResumo[]
}

export interface RelatorioChamadosSerieTemporal {
  agrupamento: AgrupamentoRelatorio
  itens: SerieTemporalRelatorio[]
}

export interface RelatorioChamadosDistribuicao {
  agruparPor: AgruparPorRelatorioChamados
  itens: DistribuicaoRelatorio[]
}

export interface RankingAtendimento {
  atendenteId: string
  atendenteNome: string
  chamadosAssumidos: number
  chamadosConcluidos: number
  chamadosEmAberto: number
  chamadosReabertos: number
  tempoMedioConclusaoHoras: number | null
  percentualConclusao: number
}

export interface RelatorioAtendimentoProdutividade {
  limiteAplicado: number
  ranking: RankingAtendimento[]
}

export interface RelatorioSlaResumo {
  totalChamadosComSla: number
  totalDentroSla: number
  totalForaSla: number
  percentualCumprimento: number | null
  percentualViolacao: number | null
  tempoMedioResolucaoHoras: number | null
  chamadosProximosVencimento: number | null
  chamadosComSlaPausado: number | null
  totalSemSla: number | null
}

export interface RelatorioSlaViolacao {
  chamadoId: string
  numeroProtocolo: string
  titulo: string
  naturezaChamado: NaturezaChamado
  departamento: string
  prioridade: string
  status: string
  dataAbertura: string
  dataLimiteSla: string | null
  dataConclusao: string | null
  horasExcedidas: number | null
}

export interface RelatorioSlaPorDepartamento {
  departamentoId: string | null
  departamentoNome: string
  totalComSla: number
  dentroSla: number
  foraSla: number
  percentualCumprimento: number | null
}

export interface RelatorioSlaPorPrioridade {
  prioridadeId: string
  prioridadeNome: string
  totalComSla: number
  dentroSla: number
  foraSla: number
  percentualCumprimento: number | null
}

export interface RelatorioAprovacoesResumo {
  totalAprovacoes: number
  pendentes: number
  aprovadas: number
  reprovadas: number
  canceladas: number
  taxaAprovacao: number | null
  taxaReprovacao: number | null
  tempoMedioDecisaoHoras: number | null
}

export interface RelatorioAprovacoesTempoMedio {
  grupo: string
  totalDecididas: number
  tempoMedioDecisaoHoras: number | null
}

export interface RelatorioAprovacoesPorOrigem {
  tipoOrigem: string
  total: number
  pendentes: number
  aprovadas: number
  reprovadas: number
  canceladas: number
  tempoMedioDecisaoHoras: number | null
}

export interface RelatorioCatalogoServicosResumo {
  totalServicos: number
  servicosPublicados: number
  servicosArquivados: number
  servicosAtivos: number
  servicosQuePermitemAbertura: number
  servicosQueRequeremAprovacao: number
  chamadosAbertosPorCatalogo: number
  percentualChamadosPorCatalogo: number | null
}

export interface RelatorioCatalogoServicosMaisSolicitados {
  catalogoServicoId: string
  nomeServico: string
  departamentoResponsavel: string
  totalChamados: number
  totalComAprovacao: number
  totalReprovadosNaAprovacao: number | null
  totalForaSla: number | null
}

export interface RelatorioCatalogoServicosPorDepartamento {
  departamentoId: string
  departamentoNome: string
  totalServicos: number
  servicosPublicados: number
  chamadosAbertos: number
  servicosQueRequeremAprovacao: number
}

export interface RelatorioInventarioAtivosResumo {
  totalAtivos: number
  ativosAtivos: number
  ativosInativos: number
  totalPorTipo: IndicadorResumo[]
  totalPorCriticidade: IndicadorResumo[]
  totalPorStatusOperacional: IndicadorResumo[]
  totalPorStatusPatrimonial: IndicadorResumo[]
  totalComChamadosRelacionados: number
  totalEmManutencao: number
  totalComDefeito: number
}

export interface RelatorioInventarioAtivosPorStatus {
  porStatusOperacional: DistribuicaoRelatorio[]
  porStatusPatrimonial: DistribuicaoRelatorio[]
  porCriticidade: DistribuicaoRelatorio[]
}

export interface RelatorioInventarioAtivosChamadosRecorrentes {
  inventarioAtivoId: string
  codigo: string
  nome: string
  tipoAtivo: string
  departamento: string
  usuarioResponsavel: string
  totalChamados: number
  chamadosAbertos: number
  chamadosEncerrados: number
  ultimoChamadoEm: string | null
}

export interface RelatorioInventarioAtivosPorDepartamento {
  departamentoId: string | null
  departamentoNome: string
  totalAtivos: number
  ativosAtivos: number
  ativosInativos: number
  totalComChamados: number
  criticos: number
}

export interface RelatorioBaseConhecimentoResumo {
  totalArtigos: number
  artigosPublicados: number
  artigosRascunho: number
  artigosArquivados: number
  artigosAtivos: number
  artigosInativos: number
  totalPorVisibilidade: IndicadorResumo[]
  artigosVinculadosChamados: number
  chamadosComArtigoVinculado: number
}

export interface RelatorioBaseConhecimentoPorStatus {
  porStatus: DistribuicaoRelatorio[]
  porVisibilidade: DistribuicaoRelatorio[]
}

export interface RelatorioBaseConhecimentoVinculosChamados {
  artigoId: string
  titulo: string
  status: string
  visibilidade: string
  totalChamadosVinculados: number
  ultimoVinculoEm: string | null
}

export interface PontoSerieTemporal {
  referencia: string
  valor: number
  rotulo: string | null
}

export interface RelatorioAuditoriaResumo {
  totalAcoesAuditadas: number
  usuariosComAcoes: number
  entidadesAfetadas: number
  totalPorTipoAcao: IndicadorResumo[]
  totalPorEntidade: IndicadorResumo[]
  totalPorDia: PontoSerieTemporal[]
}

export interface RelatorioAuditoriaPorUsuario {
  usuarioId: string | null
  usuarioNome: string
  totalAcoes: number
  ultimaAcaoEm: string | null
  acoesPorTipo: IndicadorResumo[]
}

export interface RelatorioAuditoriaPorEntidade {
  entidade: string
  totalAcoes: number
  usuariosDistintos: number
  ultimaAcaoEm: string | null
  acoesPorTipo: IndicadorResumo[]
}
