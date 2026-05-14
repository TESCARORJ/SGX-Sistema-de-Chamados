export type PerfilUsuario = 'Administrador' | 'Atendente' | 'Solicitante'
export type PermissaoUsuario = string
export type PerfilEmulado = 'Solicitante' | 'Atendente'
export type ProvedorPrincipal = 'MicrosoftEntraId' | 'Local' | 'Hibrido'

export interface UsuarioAutenticado {
  id: string
  nome: string
  email: string
  login: string
  situacao: 'Ativo' | 'Inativo' | 'Bloqueado'
  perfis: PerfilUsuario[]
  permissoes: PermissaoUsuario[]
  departamentoId: string | null
  autenticadoPor: 'MicrosoftEntraId' | 'AzureAd' | 'LocalDevelopment' | 'LocalSgx'
  deveAlterarSenha: boolean
}

export interface MeResponse extends UsuarioAutenticado {}

export interface ProvedoresAutenticacaoResponse {
  provedorPrincipal: ProvedorPrincipal
  loginMicrosoftHabilitado: boolean
  loginLocalSgxHabilitado: boolean
  loginLocalDevelopmentHabilitado: boolean
}

export interface LocalLoginResponse {
  accessToken: string
  tokenType: string
  expiresIn: number
  autenticadoPor: 'LocalSgx'
  deveAlterarSenha: boolean
}

export interface MensagemAuthResponse {
  mensagem: string
}

export interface UsuarioOriginalEmulacao {
  email: string
  nome: string
  perfil: PerfilUsuario
}

export interface AuthState {
  inicializado: boolean
  carregandoSessao: boolean
  inicializandoSessao: boolean
  carregando: boolean
  autenticado: boolean
  token: string | null
  usuario: UsuarioAutenticado | null
  erro: string | null
  erroAutenticacao: string | null
  erroInicializacao: string | null
  modoLocal: boolean
  localDevEmail: string
  localDevNome: string
  localDevPerfil: PerfilUsuario
  emulandoPerfil: boolean
  usuarioOriginal: UsuarioOriginalEmulacao | null
  perfilEmulado: PerfilEmulado | null
}
