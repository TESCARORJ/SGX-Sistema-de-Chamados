export interface LinhaTempoChamadoItem {
  id: string
  tipo: string
  tipoDescricao: string
  dataHora: string
  usuarioId: string | null
  usuario: string | null
  titulo: string
  descricao: string
  interno: boolean
  referenciaId: string | null
  referenciaTipo: string | null
  nomeArquivo?: string | null
  contentType?: string | null
  tamanhoBytes?: number | null
  status?: string | null
  prioridade?: string | null
  categoria?: string | null
  responsavel?: string | null
}

export interface LinhaTempoChamadoResponse {
  chamadoId: string
  codigo: string
  items: LinhaTempoChamadoItem[]
}
