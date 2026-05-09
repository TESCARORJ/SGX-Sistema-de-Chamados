export type TipoNotificacao =
  | 'info'
  | 'warning'
  | 'negative'
  | 'positive'
  | 'message'
  | 'email'
  | 'sla'
  | 'assignment'

export interface Notificacao {
  id: string
  titulo: string
  descricao: string
  tipo: TipoNotificacao
  lida: boolean
  dataHora: string
  tempoRelativo: string
  chamadoId?: string
  chamadoCodigo?: string
  remetente?: string
  detalheTecnico?: string
}
