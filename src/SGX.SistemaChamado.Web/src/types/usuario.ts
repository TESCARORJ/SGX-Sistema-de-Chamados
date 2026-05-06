export interface PerfilAcessoResumo {
  id: string
  nome: string
  tipoPerfil: number
  tipoPerfilDescricao?: string
  ativo?: boolean
}

export interface Usuario {
  id: string
  nome: string
  email: string
  login: string
  situacao: 'Ativo' | 'Inativo' | 'Bloqueado'
  ultimoAcessoEm: string | null
  departamentoId: string | null
  departamento?: string | null
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
  ativo: boolean
  perfis: PerfilAcessoResumo[]
}
