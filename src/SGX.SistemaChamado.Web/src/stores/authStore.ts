import { defineStore } from 'pinia'
import { authService } from '../services/authService'
import { httpClient, setHttpAuthToken, setHttpLocalDevHeaders } from '../services/httpClient'
import type { AuthState, MeResponse, PerfilUsuario } from '../types/auth'

const modoLocalHabilitado = import.meta.env.DEV && import.meta.env.VITE_AUTH_MODO_LOCAL === 'true'

function perfilParaRota(perfis: PerfilUsuario[]): '/admin' | '/portal' | '/acesso-negado' {
  if (perfis.includes('Administrador') || perfis.includes('Atendente')) {
    return '/admin'
  }

  if (perfis.includes('Solicitante')) {
    return '/portal'
  }

  return '/acesso-negado'
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
    localDevEmail: 'admin.local@sgx.local',
    localDevNome: 'Administrador Local',
    localDevPerfil: 'Administrador',
  }),

  getters: {
    rotaInicial(state): '/admin' | '/portal' | '/acesso-negado' {
      return perfilParaRota(state.usuario?.perfis ?? [])
    },
  },

  actions: {
    async initialize(): Promise<void> {
      if (this.inicializado || this.carregando) {
        return
      }

      this.carregando = true
      this.erro = null

      try {
        if (this.modoLocal) {
          this.autenticado = false
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

      this.localDevEmail = payload?.email?.trim() || this.localDevEmail
      this.localDevNome = payload?.nome?.trim() || this.localDevNome
      this.localDevPerfil = payload?.perfil || this.localDevPerfil

      setHttpAuthToken(null)
      setHttpLocalDevHeaders({
        'X-Dev-User-Email': this.localDevEmail,
        'X-Dev-User-Name': this.localDevNome,
        'X-Dev-User-Role': this.localDevPerfil,
      })

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

    async logout(): Promise<void> {
      if (!this.modoLocal) {
        await authService.logout()
      }

      this.reset()
    },

    reset(): void {
      this.autenticado = false
      this.usuario = null
      this.token = null
      setHttpAuthToken(null)
      setHttpLocalDevHeaders(null)
    },

    async carregarMe(): Promise<void> {
      const response = await httpClient.get<MeResponse>('/api/me')
      this.usuario = response
    },
  },
})
