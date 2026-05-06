export type PerfilUsuario = 'Administrador' | 'Atendente' | 'Solicitante'
export type PermissaoUsuario = string

export interface UsuarioAutenticado {
  id: string
  nome: string
  email: string
  login: string
  situacao: 'Ativo' | 'Inativo' | 'Bloqueado'
  perfis: PerfilUsuario[]
  permissoes: PermissaoUsuario[]
  departamentoId: string | null
  autenticadoPor: 'AzureAd' | 'LocalDevelopment'
}

export interface MeResponse extends UsuarioAutenticado {}

export interface AuthState {
  inicializado: boolean
  carregando: boolean
  autenticado: boolean
  token: string | null
  usuario: UsuarioAutenticado | null
  erro: string | null
  modoLocal: boolean
  localDevEmail: string
  localDevNome: string
  localDevPerfil: PerfilUsuario
}
