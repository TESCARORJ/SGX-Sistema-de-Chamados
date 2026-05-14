export interface FiltroPoliticaSlaRequest {
  ativo?: boolean
  categoriaId?: string
  departamentoId?: string
  texto?: string
}

export interface MetaSlaResponse {
  id: string
  prioridadeId: string
  prioridadeNome: string
  prioridadeNivel: number
  tempoPrimeiraRespostaMinutos: number
  tempoResolucaoMinutos: number
  tempoAtualizacaoMinutos: number | null
  tempoRespostaSubsequenteMinutos: number | null
  ativo: boolean
}

export interface PoliticaSlaResponse {
  id: string
  nome: string
  descricao: string | null
  ativo: boolean
  ordem: number
  categoriaId: string | null
  categoriaNome: string | null
  departamentoId: string | null
  departamentoNome: string | null
  usarHorarioComercial: boolean
  calendarioCorporativoId: string | null
  calendarioCorporativoNome: string | null
  pausarQuandoAguardandoSolicitante: boolean
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
  metas: MetaSlaResponse[]
}

export interface MetaSlaUpsertRequest {
  id?: string | null
  prioridadeId: string
  tempoPrimeiraRespostaMinutos: number
  tempoResolucaoMinutos: number
  tempoAtualizacaoMinutos?: number | null
  tempoRespostaSubsequenteMinutos?: number | null
  ativo: boolean
}

export interface CriarPoliticaSlaRequest {
  nome: string
  descricao?: string | null
  ativo: boolean
  ordem: number
  categoriaId?: string | null
  departamentoId?: string | null
  calendarioCorporativoId?: string | null
  usarHorarioComercial: boolean
  pausarQuandoAguardandoSolicitante: boolean
  metas: MetaSlaUpsertRequest[]
}

export interface AtualizarPoliticaSlaRequest extends CriarPoliticaSlaRequest {}

export interface AtualizarStatusPoliticaSlaRequest {
  ativo: boolean
}

export type SituacaoSlaChamado =
  | 'NaoAplicavel'
  | 'DentroDoPrazo'
  | 'ProximoDoVencimento'
  | 'Vencido'
  | 'Cumprido'
  | 'Violado'
  | 'Pausado'

export interface ConfiguracaoAlertaSlaResponse {
  id: string
  ativo: boolean
  minutosAntesVencimentoPrimeiraResposta: number
  minutosAntesVencimentoResolucao: number
  notificarAtendente: boolean
  notificarGestor: boolean
  notificarDepartamento: boolean
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export interface AtualizarConfiguracaoAlertaSlaRequest {
  ativo: boolean
  minutosAntesVencimentoPrimeiraResposta: number
  minutosAntesVencimentoResolucao: number
  notificarAtendente: boolean
  notificarGestor: boolean
  notificarDepartamento: boolean
}

export interface FiltroDashboardSlaRequest {
  dataInicio?: string
  dataFim?: string
  prioridadeId?: string
  categoriaId?: string
  departamentoId?: string
  situacaoSla?: SituacaoSlaChamado
}

export interface SlaAgrupamentoResponse {
  id: string | null
  nome: string
  total: number
  vencidos: number
  proximos: number
  cumpridos: number
  violados: number
}

export interface SlaDashboardResponse {
  totalComSlaAplicado: number
  totalVencidos: number
  totalProximosDoVencimento: number
  totalDentroDoPrazo: number
  totalCumpridos: number
  totalViolados: number
  percentualCumprimento: number
  tempoMedioPrimeiraRespostaMinutos: number | null
  tempoMedioResolucaoMinutos: number | null
  porPrioridade: SlaAgrupamentoResponse[]
  porCategoria: SlaAgrupamentoResponse[]
  porDepartamento: SlaAgrupamentoResponse[]
}

export interface HorarioAtendimentoCalendarioResponse {
  id: string
  diaSemana: number
  diaSemanaNome: string
  horaInicio: string
  horaFim: string
  ativo: boolean
}

export type TipoExcecaoCalendario = 'Feriado' | 'Recesso' | 'ExpedienteEspecial' | 'SemExpediente' | number

export interface ExcecaoCalendarioCorporativoResponse {
  id: string
  data: string
  tipo: TipoExcecaoCalendario
  tipoDescricao: string
  descricao: string | null
  horaInicio: string | null
  horaFim: string | null
  ativo: boolean
}

export interface CalendarioCorporativoResponse {
  id: string
  nome: string
  descricao: string | null
  ativo: boolean
  padrao: boolean
  timeZone: string
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
  horariosAtendimento: HorarioAtendimentoCalendarioResponse[]
  excecoes: ExcecaoCalendarioCorporativoResponse[]
}

export interface CriarCalendarioCorporativoRequest {
  nome: string
  descricao?: string | null
  ativo: boolean
  padrao: boolean
  timeZone: string
}

export interface AtualizarCalendarioCorporativoRequest {
  nome: string
  descricao?: string | null
  timeZone: string
}

export interface AtualizarStatusCalendarioCorporativoRequest {
  ativo: boolean
}

export interface HorarioAtendimentoCalendarioRequest {
  diaSemana: number
  horaInicio: string
  horaFim: string
  ativo: boolean
}

export interface ExcecaoCalendarioCorporativoRequest {
  data: string
  tipo: number
  descricao?: string | null
  horaInicio?: string | null
  horaFim?: string | null
  ativo: boolean
}
