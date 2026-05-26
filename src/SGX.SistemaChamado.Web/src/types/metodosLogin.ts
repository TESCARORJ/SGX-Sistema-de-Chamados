import type { CodigoProvedorAutenticacao } from './auth'

export interface MetodoLoginAdminDto {
  codigo: CodigoProvedorAutenticacao
  nome: string
  descricao: string
  configurado: boolean
  habilitado: boolean
  principal: boolean
  ordem: number
  permiteAutoProvisionamento: boolean
  perfilPadraoAutoProvisionamento: string
  rotuloExibicao: string
  funcional: boolean
  podeHabilitar: boolean
  motivoBloqueioHabilitar: string | null
  podeDesabilitar: boolean
  motivoBloqueioDesabilitar: string | null
}

export interface MetodosLoginAdminResponse {
  provedores: MetodoLoginAdminDto[]
}

export interface MetodoLoginAdminAtualizacaoDto {
  codigo: CodigoProvedorAutenticacao
  habilitado: boolean
  principal: boolean
  ordem: number
  permiteAutoProvisionamento: boolean
  perfilPadraoAutoProvisionamento: string
  rotuloExibicao: string
}

export interface AtualizarMetodosLoginAdminRequest {
  provedores: MetodoLoginAdminAtualizacaoDto[]
}
