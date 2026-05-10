export type StatusProcessamentoEmail =
  | 'Pendente'
  | 'Processado'
  | 'Ignorado'
  | 'Erro'
  | 'Duplicado'
  | 'NaoCorrelacionado'

export interface FiltroLogsEmailRequest {
  dataInicial?: string
  dataFinal?: string
  dataInicio?: string
  dataFim?: string
  status?: StatusProcessamentoEmail
  remetente?: string
  chamadoId?: string
  codigoChamado?: string
  assunto?: string
  messageId?: string
  texto?: string
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: 'dataRecebimento' | 'dataProcessamento' | 'status' | 'remetente'
  direcao?: 'asc' | 'desc'
}

export interface LogIntegracaoEmailResumoResponse {
  id: string
  messageId: string | null
  dataRecebimento: string
  dataProcessamento: string | null
  remetente: string
  destinatario: string | null
  assunto: string | null
  statusProcessamento: StatusProcessamentoEmail
  statusProcessamentoLabel: string
  temErro: boolean
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
  inReplyTo: string | null
  references: string | null
  fingerprint: string
  remetente: string
  destinatario: string | null
  nomeRemetente: string | null
  assunto: string | null
  dataRecebimento: string
  dataProcessamento: string | null
  statusProcessamento: StatusProcessamentoEmail
  erro: string | null
  chamadoId: string | null
  chamadoCodigo: string | null
  chamadoTitulo: string | null
  tentativas: number
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export type FiltroEmailLogs = FiltroLogsEmailRequest
export type EmailLogResumo = LogIntegracaoEmailResumoResponse
export type EmailLogDetalhe = LogIntegracaoEmailDetalheResponse
export type ResultadoPaginadoEmailLogs = ListaLogsIntegracaoEmailResponse
