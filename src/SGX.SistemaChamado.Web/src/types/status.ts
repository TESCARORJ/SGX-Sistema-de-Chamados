export interface Status {
  id: string
  nome: string
  codigo: number
  descricao: string | null
  ehStatusFinal: boolean
  pausaSla: boolean
  ativo: boolean
}
