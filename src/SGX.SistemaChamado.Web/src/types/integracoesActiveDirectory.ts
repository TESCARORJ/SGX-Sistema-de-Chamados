export interface ActiveDirectoryIntegracaoResponse {
  ativo: boolean
  servidor: string
  porta: number
  usarLdaps: boolean
  permitirLdapSemTls: boolean
  dominio: string
  baseDn: string
  userSearchFilter: string
  permitirAutoProvisionamento: boolean
  perfilPadrao: string
  timeoutConexaoSegundos: number
  tecnicamenteConfigurado: boolean
  statusConfiguracao: string
  pendenciasConfiguracao: string[]
  avisosSeguranca: string[]
}

export interface AtualizarActiveDirectoryIntegracaoRequest {
  ativo: boolean
  servidor: string
  porta: number
  usarLdaps: boolean
  permitirLdapSemTls: boolean
  confirmacaoPermitirLdapSemTls: boolean
  dominio: string
  baseDn: string
  userSearchFilter: string
  permitirAutoProvisionamento: boolean
  perfilPadrao: string
  timeoutConexaoSegundos: number
}

export interface TestarConexaoActiveDirectoryRequest {
  ativo: boolean
  servidor: string
  porta: number
  usarLdaps: boolean
  permitirLdapSemTls: boolean
  confirmacaoPermitirLdapSemTls: boolean
  dominio: string
  baseDn: string
  userSearchFilter: string
  permitirAutoProvisionamento: boolean
  perfilPadrao: string
  timeoutConexaoSegundos: number
}

export interface TestarConexaoActiveDirectoryResponse {
  sucesso: boolean
  mensagem: string
  duracaoMs: number
}

export interface TestarAutenticacaoActiveDirectoryRequest {
  usuario: string
  senha: string
  dominio: string
  ativo: boolean
  servidor: string
  porta: number
  usarLdaps: boolean
  permitirLdapSemTls: boolean
  confirmacaoPermitirLdapSemTls: boolean
  baseDn: string
  userSearchFilter: string
  timeoutConexaoSegundos: number
}

export interface TestarAutenticacaoActiveDirectoryResponse {
  sucesso: boolean
  mensagem: string
  usuarioSamAccountName: string | null
  nomeCompleto: string | null
  email: string | null
  userPrincipalName: string | null
  duracaoMs: number
}
