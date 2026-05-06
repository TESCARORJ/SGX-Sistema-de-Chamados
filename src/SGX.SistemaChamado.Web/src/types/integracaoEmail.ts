export type StatusProcessamentoEmail = 'Pendente' | 'Processado' | 'IgnoradoDuplicado' | 'Erro'

export interface FiltroLogsEmailRequest {
  dataInicio?: string
  dataFim?: string
  status?: StatusProcessamentoEmail
  remetente?: string
  chamadoId?: string
  texto?: string
  pagina?: number
  tamanhoPagina?: number
}

export interface LogIntegracaoEmailResumoResponse {
  id: string
  dataRecebimento: string
  dataProcessamento: string | null
  remetente: string
  assunto: string | null
  statusProcessamento: StatusProcessamentoEmail
  chamadoId: string | null
  chamadoCodigo: string | null
  erroResumido: string | null
}

export interface ListaLogsIntegracaoEmailResponse {
  items: LogIntegracaoEmailResumoResponse[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface LogIntegracaoEmailDetalheResponse {
  id: string
  messageId: string | null
  fingerprint: string
  remetente: string
  nomeRemetente: string | null
  assunto: string | null
  dataRecebimento: string
  dataProcessamento: string | null
  statusProcessamento: StatusProcessamentoEmail
  erro: string | null
  chamadoId: string | null
  chamadoCodigo: string | null
  tentativas: number
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}
