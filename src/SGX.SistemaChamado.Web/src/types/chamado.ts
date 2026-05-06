export interface Chamado {
  id: string
  codigo: string
  titulo: string
  descricao: string
  solicitanteId: string
  responsavelId: string | null
  departamentoId: string | null
  categoriaId: string
  prioridadeId: string
  statusId: string
  origem: 'Portal' | 'Email' | 'Admin'
  abertoEm: string
  encerradoEm: string | null
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
  ativo: boolean
  slaVencido?: boolean
  slaProximoVencimento?: boolean
  prazoPrimeiraRespostaEm?: string | null
  primeiraRespostaEm?: string | null
  prazoResolucaoEm?: string | null
  resolvidoEm?: string | null
  estaPausado?: boolean
  totalMinutosPausado?: number
}

export interface CriarChamadoRequest {
  titulo: string
  descricao: string
  solicitanteId: string
  categoriaId: string
  prioridadeId: string
  departamentoId?: string
  origem?: 'Portal' | 'Email' | 'Admin'
}
