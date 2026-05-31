export type PerfilUsuario =
  | 'Administrador'
  | 'Atendente'
  | 'Solicitante'
  | 'Atendente N1'
  | 'Técnico N2'
  | 'Coordenador Service Desk'
  | 'Gestor TI'
  | 'Auditor Governança'
  | 'AtendenteN1'
  | 'TecnicoN2'
  | 'CoordenadorServiceDesk'
  | 'GestorTI'
  | 'AuditorGovernanca'
export type PermissaoUsuario = string
export type PerfilEmulado =
  | 'Administrador'
  | 'Solicitante'
  | 'Atendente'
  | 'Atendente N1'
  | 'Técnico N2'
  | 'Coordenador Service Desk'
  | 'Gestor TI'
  | 'Auditor Governança'
  | 'AtendenteN1'
  | 'TecnicoN2'
  | 'CoordenadorServiceDesk'
  | 'GestorTI'
  | 'AuditorGovernanca'
export type ProvedorPrincipal = 'MicrosoftEntraId' | 'Local' | 'Hibrido'
export type CodigoProvedorAutenticacao =
  | 'MicrosoftEntraId'
  | 'ActiveDirectory'
  | 'LocalSgx'
  | 'LocalDevelopment'

export interface UsuarioAutenticado {
  id: string
  nome: string
  email: string
  login: string
  situacao: 'Ativo' | 'Inativo' | 'Bloqueado'
  perfis: PerfilUsuario[]
  permissoes: PermissaoUsuario[]
  departamentoId: string | null
  autenticadoPor: 'MicrosoftEntraId' | 'AzureAd' | 'ActiveDirectory' | 'LocalDevelopment' | 'LocalSgx'
  deveAlterarSenha: boolean
}

export interface MeResponse extends UsuarioAutenticado {}

export interface ProvedoresAutenticacaoResponse {
  provedores: ProvedorAutenticacaoDto[]
}

export interface ProvedorAutenticacaoDto {
  codigo: CodigoProvedorAutenticacao
  nome: string
  descricao: string
  habilitado: boolean
  principal: boolean
  ordem: number
}

export interface LocalLoginResponse {
  accessToken: string
  tokenType: string
  expiresIn: number
  autenticadoPor: 'LocalSgx' | 'ActiveDirectory'
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
