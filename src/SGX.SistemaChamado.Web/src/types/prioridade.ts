export interface Prioridade {
  id: string
  nome: string
  nivel: number
  descricao: string | null
  prazoPrimeiraRespostaHoras: number
  prazoResolucaoHoras: number
  ativo: boolean
}
