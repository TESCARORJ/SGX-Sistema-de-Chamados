export type TipoAcaoAuditoria =
  | 'Login'
  | 'Logout'
  | 'Criacao'
  | 'Edicao'
  | 'ExclusaoLogica'
  | 'Ativacao'
  | 'Inativacao'
  | 'AlteracaoStatus'
  | 'AlteracaoPermissao'
  | 'Visualizacao'
  | 'Exportacao'
  | 'Importacao'
  | 'Erro'
  | 'ExecucaoJob'
  | 'Configuracao'
  | 'Homologacao'
  | 'Outro'

export type NivelAuditoria = 'Informacao' | 'Alerta' | 'Critico'

export interface EventoAuditoriaResumo {
  id: string
  dataEvento: string
  usuarioNome: string | null
  usuarioEmail: string | null
  modulo: string
  entidade: string
  entidadeId: string | null
  acao: TipoAcaoAuditoria
  descricao: string
  nivel: NivelAuditoria
  sucesso: boolean
  ipOrigem: string | null
  correlacaoId: string | null
}

export interface EventoAuditoriaDetalhe {
  id: string
  dataEvento: string
  usuarioId: string | null
  usuarioNome: string | null
  usuarioEmail: string | null
  usuarioLogin: string | null
  ipOrigem: string | null
  userAgent: string | null
  modulo: string
  entidade: string
  entidadeId: string | null
  acao: TipoAcaoAuditoria
  descricao: string
  dadosAntes: string | null
  dadosDepois: string | null
  metadados: string | null
  nivel: NivelAuditoria
  sucesso: boolean
  mensagemErro: string | null
  correlacaoId: string | null
  criadoEm: string
}

export interface ResultadoPaginadoEventoAuditoria {
  items: EventoAuditoriaResumo[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface FiltroAuditoriaRequest {
  dataInicio?: string
  dataFim?: string
  usuarioId?: string
  usuarioEmail?: string
  modulo?: string
  entidade?: string
  entidadeId?: string
  acao?: TipoAcaoAuditoria
  nivel?: NivelAuditoria
  sucesso?: boolean
  ipOrigem?: string
  correlacaoId?: string
  texto?: string
  pagina?: number
  tamanhoPagina?: number
}

export interface FiltroDashboardAuditoriaRequest {
  dataInicio?: string
  dataFim?: string
  modulo?: string
  usuarioEmail?: string
  nivel?: NivelAuditoria
  sucesso?: boolean
}

export interface AuditoriaAgrupamentoResponse {
  chave: string
  total: number
}

export interface AuditoriaAgrupamentoDiaResponse {
  dia: string
  total: number
}

export interface AuditoriaDashboardResponse {
  totalEventos: number
  totalEventosCriticos: number
  totalEventosAlerta: number
  totalEventosInformacao: number
  totalFalhas: number
  totalSucessos: number
  eventosPorModulo: AuditoriaAgrupamentoResponse[]
  eventosPorAcao: AuditoriaAgrupamentoResponse[]
  eventosPorUsuario: AuditoriaAgrupamentoResponse[]
  eventosPorDia: AuditoriaAgrupamentoDiaResponse[]
  ultimosEventosCriticos: EventoAuditoriaResumo[]
  ultimasFalhas: EventoAuditoriaResumo[]
}
