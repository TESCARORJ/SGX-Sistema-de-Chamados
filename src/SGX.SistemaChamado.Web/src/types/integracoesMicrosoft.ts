import type { ProvedorPrincipal } from './auth'

export interface MicrosoftEntraIdIntegracaoResponse {
  habilitado: boolean
  provedorPrincipal: ProvedorPrincipal
  loginLocalHabilitado: boolean
  tenantId: string
  clientId: string
  audience: string
  issuer: string
  authority: string
  apiScope: string
  redirectUri: string
  dominiosPermitidos: string[]
  criarUsuarioAutomaticamente: boolean
  perfilPadraoUsuarioMicrosoft: string
  statusConfiguracao: string
  pendenciasConfiguracao: string[]
}

export interface AtualizarMicrosoftEntraIdIntegracaoRequest {
  habilitado: boolean
  provedorPrincipal: ProvedorPrincipal
  loginLocalHabilitado: boolean
  tenantId: string
  clientId: string
  audience: string
  issuer: string
  authority: string
  apiScope: string
  redirectUri: string
  dominiosPermitidos: string[]
  criarUsuarioAutomaticamente: boolean
  perfilPadraoUsuarioMicrosoft: string
}

