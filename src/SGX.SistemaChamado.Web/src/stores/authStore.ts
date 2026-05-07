import { defineStore } from 'pinia'
import { authService } from '../services/authService'
import { httpClient, setHttpAuthToken, setHttpLocalDevHeaders } from '../services/httpClient'
import type {
  AuthState,
  MeResponse,
  PerfilEmulado,
  PerfilUsuario,
  UsuarioOriginalEmulacao,
} from '../types/auth'

const modoLocalHabilitado =
  !import.meta.env.PROD &&
  (import.meta.env.DEV || import.meta.env.VITE_AUTH_MODO_LOCAL === 'true')
const adminLocalDevEmail = 'admin@sgxdigital.com'
const adminLocalDevNome = 'Administrador SGX'
const adminLocalDevPerfil: PerfilUsuario = 'Administrador'
const solicitanteDemoEmail = 'solicitante.demo@sgxdigital.com'
const solicitanteDemoNome = 'Solicitante Demo'
const solicitanteDemoPerfil: PerfilEmulado = 'Solicitante'
const emulacaoSessionKey = 'sgx.auth.emulacao'

interface EmulacaoSessionPayload {
  usuarioOriginal: UsuarioOriginalEmulacao
  perfilEmulado: PerfilEmulado
}

function perfilParaRota(perfis: PerfilUsuario[]): '/admin' | '/portal' | '/acesso-negado' {
  if (perfis.includes('Administrador') || perfis.includes('Atendente')) {
    return '/admin'
  }

  if (perfis.includes('Solicitante')) {
    return '/portal'
  }

  return '/acesso-negado'
}

function ehPerfilAdministrativo(perfil: PerfilUsuario): boolean {
  return perfil === 'Administrador' || perfil === 'Atendente'
}

function obterPerfilAdministrativo(perfis: PerfilUsuario[], fallback: PerfilUsuario): PerfilUsuario {
  if (perfis.includes('Administrador')) {
    return 'Administrador'
  }

  if (perfis.includes('Atendente')) {
    return 'Atendente'
  }

  return fallback
}

function salvarEmulacaoSessionStorage(payload: EmulacaoSessionPayload): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.setItem(emulacaoSessionKey, JSON.stringify(payload))
}

function carregarEmulacaoSessionStorage(): EmulacaoSessionPayload | null {
  if (typeof window === 'undefined') {
    return null
  }

  const raw = window.sessionStorage.getItem(emulacaoSessionKey)
  if (!raw) {
    return null
  }

  try {
    const payload = JSON.parse(raw) as EmulacaoSessionPayload
    if (!payload?.usuarioOriginal?.email || !payload?.usuarioOriginal?.nome || !payload?.usuarioOriginal?.perfil) {
      return null
    }

    return payload
  } catch {
    return null
  }
}

function limparEmulacaoSessionStorage(): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.removeItem(emulacaoSessionKey)
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    inicializado: false,
    carregando: false,
    autenticado: false,
    token: null,
    usuario: null,
    erro: null,
    modoLocal: modoLocalHabilitado,
    localDevEmail: adminLocalDevEmail,
    localDevNome: adminLocalDevNome,
    localDevPerfil: adminLocalDevPerfil,
    emulandoPerfil: false,
    usuarioOriginal: null,
    perfilEmulado: null,
  }),

  getters: {
    rotaInicial(state): '/admin' | '/portal' | '/acesso-negado' {
      return perfilParaRota(state.usuario?.perfis ?? [])
    },
    podeEmularSolicitante(state): boolean {
      if (!state.modoLocal || import.meta.env.PROD) {
        return false
      }

      const perfis = state.usuario?.perfis ?? []
      return perfis.some((perfil) => ehPerfilAdministrativo(perfil))
    },
  },

  actions: {
    aplicarHeadersModoLocal(): void {
      if (!this.modoLocal || import.meta.env.PROD) {
        setHttpLocalDevHeaders(null)
        return
      }

      setHttpAuthToken(null)
      setHttpLocalDevHeaders({
        'X-Dev-User-Email': this.localDevEmail,
        'X-Dev-User-Name': this.localDevNome,
        'X-Dev-User-Role': this.localDevPerfil,
      })
    },

    obterUsuarioOriginalPersistido(): UsuarioOriginalEmulacao | null {
      if (this.usuarioOriginal) {
        return this.usuarioOriginal
      }

      const payload = carregarEmulacaoSessionStorage()
      return payload?.usuarioOriginal ?? null
    },

    limparEmulacao(): void {
      this.emulandoPerfil = false
      this.usuarioOriginal = null
      this.perfilEmulado = null
      limparEmulacaoSessionStorage()
    },

    async initialize(): Promise<void> {
      if (this.inicializado || this.carregando) {
        return
      }

      this.carregando = true
      this.erro = null

      try {
        if (this.modoLocal) {
          this.limparEmulacao()
          this.usuario = null
          this.token = null
          this.autenticado = false
          setHttpAuthToken(null)
          setHttpLocalDevHeaders(null)
          return
        }

        const account = await authService.getAccount()
        if (!account) {
          this.autenticado = false
          return
        }

        const token = await authService.acquireAccessToken(account)
        if (!token) {
          this.autenticado = false
          return
        }

        setHttpAuthToken(token)
        setHttpLocalDevHeaders(null)
        this.limparEmulacao()
        this.token = token

        await this.carregarMe()
        this.autenticado = true
      } catch (error) {
        this.reset()
        this.erro = error instanceof Error ? error.message : 'Falha ao inicializar autenticacao.'
      } finally {
        this.inicializado = true
        this.carregando = false
      }
    },

    async loginMicrosoft(): Promise<void> {
      this.carregando = true
      this.erro = null

      try {
        const result = await authService.loginPopup()
        const token =
          result.accessToken ||
          (await authService.acquireAccessToken(result.account ?? (await authService.getAccount())))

        if (!token) {
          throw new Error('Nao foi possivel obter access token do Microsoft Entra ID.')
        }

        setHttpAuthToken(token)
        setHttpLocalDevHeaders(null)
        this.limparEmulacao()
        this.token = token

        await this.carregarMe()
        this.autenticado = true
      } catch (error) {
        this.reset()
        this.erro = error instanceof Error ? error.message : 'Falha no login Microsoft.'
        throw error
      } finally {
        this.inicializado = true
        this.carregando = false
      }
    },

    async loginLocalDev(payload?: { email?: string; nome?: string; perfil?: PerfilUsuario }): Promise<void> {
      if (!this.modoLocal) {
        throw new Error('Modo local de autenticacao esta desabilitado.')
      }

      this.limparEmulacao()
      this.localDevEmail = payload?.email?.trim() || adminLocalDevEmail
      this.localDevNome = payload?.nome?.trim() || adminLocalDevNome
      this.localDevPerfil = payload?.perfil || adminLocalDevPerfil

      setHttpAuthToken(null)
      this.aplicarHeadersModoLocal()

      this.token = null
      this.erro = null
      this.carregando = true

      try {
        await this.carregarMe()
        this.autenticado = true
        this.inicializado = true
      } catch (error) {
        this.reset()
        this.erro = error instanceof Error ? error.message : 'Falha no login local.'
        throw error
      } finally {
        this.carregando = false
      }
    },

    async iniciarEmulacaoSolicitante(): Promise<void> {
      if (import.meta.env.PROD || !this.modoLocal) {
        throw new Error('A emulacao de perfil esta disponivel apenas em Development local.')
      }

      if (!this.autenticado || !this.usuario) {
        throw new Error('Nao foi possivel identificar o usuario autenticado para iniciar a emulacao.')
      }

      const perfis = this.usuario.perfis ?? []
      if (!perfis.some((perfil) => ehPerfilAdministrativo(perfil))) {
        throw new Error('Apenas Administrador ou Atendente podem emular Solicitante.')
      }

      if (this.emulandoPerfil && this.perfilEmulado === solicitanteDemoPerfil) {
        return
      }

      const contextoAntesDaEmulacao: UsuarioOriginalEmulacao = {
        email: this.localDevEmail || this.usuario.email,
        nome: this.localDevNome || this.usuario.nome,
        perfil: obterPerfilAdministrativo(perfis, this.localDevPerfil),
      }

      this.usuarioOriginal = contextoAntesDaEmulacao
      this.perfilEmulado = solicitanteDemoPerfil
      this.emulandoPerfil = true
      salvarEmulacaoSessionStorage({
        usuarioOriginal: contextoAntesDaEmulacao,
        perfilEmulado: solicitanteDemoPerfil,
      })

      this.localDevEmail = solicitanteDemoEmail
      this.localDevNome = solicitanteDemoNome
      this.localDevPerfil = solicitanteDemoPerfil
      this.aplicarHeadersModoLocal()

      try {
        await this.carregarMe()
      } catch (error) {
        this.localDevEmail = contextoAntesDaEmulacao.email
        this.localDevNome = contextoAntesDaEmulacao.nome
        this.localDevPerfil = contextoAntesDaEmulacao.perfil
        this.aplicarHeadersModoLocal()
        this.limparEmulacao()

        const mensagem = error instanceof Error ? error.message : 'Falha ao sincronizar usuario emulado.'
        throw new Error(`Nao foi possivel iniciar a emulacao de Solicitante. ${mensagem}`)
      }
    },

    async encerrarEmulacao(): Promise<void> {
      if (!this.emulandoPerfil) {
        return
      }

      const usuarioOriginal = this.obterUsuarioOriginalPersistido()
      if (!usuarioOriginal) {
        this.reset()
        throw new Error('Contexto original da emulacao nao encontrado. Realize novo login.')
      }

      const contextoEmuladoAtual: UsuarioOriginalEmulacao = {
        email: this.localDevEmail,
        nome: this.localDevNome,
        perfil: this.localDevPerfil,
      }

      this.localDevEmail = usuarioOriginal.email
      this.localDevNome = usuarioOriginal.nome
      this.localDevPerfil = usuarioOriginal.perfil
      this.aplicarHeadersModoLocal()

      try {
        await this.carregarMe()
        this.limparEmulacao()
      } catch (error) {
        this.localDevEmail = contextoEmuladoAtual.email
        this.localDevNome = contextoEmuladoAtual.nome
        this.localDevPerfil = contextoEmuladoAtual.perfil
        this.aplicarHeadersModoLocal()
        this.usuarioOriginal = usuarioOriginal
        this.emulandoPerfil = true
        this.perfilEmulado = solicitanteDemoPerfil
        salvarEmulacaoSessionStorage({
          usuarioOriginal,
          perfilEmulado: solicitanteDemoPerfil,
        })

        const mensagem = error instanceof Error ? error.message : 'Falha ao restaurar usuario administrativo.'
        throw new Error(`Nao foi possivel encerrar a emulacao. ${mensagem}`)
      }
    },

    async logout(): Promise<void> {
      this.limparEmulacao()

      if (!this.modoLocal) {
        await authService.logout()
      }

      this.reset()
    },

    reset(): void {
      this.limparEmulacao()
      this.autenticado = false
      this.usuario = null
      this.token = null
      this.localDevEmail = adminLocalDevEmail
      this.localDevNome = adminLocalDevNome
      this.localDevPerfil = adminLocalDevPerfil
      setHttpAuthToken(null)
      setHttpLocalDevHeaders(null)
    },

    async carregarMe(): Promise<void> {
      const response = await httpClient.get<MeResponse>('/api/me')
      this.usuario = response
    },
  },
})
