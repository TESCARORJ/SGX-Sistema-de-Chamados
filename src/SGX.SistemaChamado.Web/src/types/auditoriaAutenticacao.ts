export type ProvedorAutenticacao =
  | 'MicrosoftEntraId'
  | 'ActiveDirectory'
  | 'LocalSgx'
  | 'LocalDevelopment'
  | 'NaoInformado'
  | string

export type ResultadoEventoAutenticacao = 'Sucesso' | 'Falha' | 'Bloqueado' | 'Negado' | 'NaoInformado' | string

export type TipoEventoAutenticacao = string

export interface EventoAuditoriaAutenticacaoResumo {
  id: string
  dataEvento: string
  usuarioNome: string | null
  usuarioEmail: string | null
  provedor: ProvedorAutenticacao
  tipoEvento: TipoEventoAutenticacao
  resultado: ResultadoEventoAutenticacao
  ipOrigem: string | null
  mensagem: string
}

export interface ListaEventosAuditoriaAutenticacaoResponse {
  items: EventoAuditoriaAutenticacaoResumo[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface FiltroAuditoriaAutenticacaoRequest {
  dataInicio?: string
  dataFim?: string
  usuarioId?: string
  usuarioEmail?: string
  entidade?: string
  entidadeId?: string
  acao?: string
  nivel?: string
  sucesso?: boolean
  ipOrigem?: string
  correlacaoId?: string
  texto?: string
  provedor?: ProvedorAutenticacao
  tipoEventoAutenticacao?: TipoEventoAutenticacao
  resultadoAutenticacao?: ResultadoEventoAutenticacao
  pagina?: number
  tamanhoPagina?: number
}
