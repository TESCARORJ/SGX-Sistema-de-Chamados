export interface Categoria {
  id: string
  nome: string
  descricao: string | null
  departamentoId: string | null
  ativo: boolean
}
