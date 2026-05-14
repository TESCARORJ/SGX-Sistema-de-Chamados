import { defineStore } from 'pinia'
import { authService } from '../services/authService'
import {
  HttpRequestError,
  httpClient,
  setHttpAuthRedirectSuppressed,
  setHttpAuthToken,
  setHttpLocalDevHeaders,
} from '../services/httpClient'
import type {
  AuthState,
  LocalLoginResponse,
  MensagemAuthResponse,
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
const atendenteDemoEmail = 'atendente.demo@sgxdigital.com'
const atendenteDemoNome = 'Atendente Demo'
const atendenteDemoPerfil: PerfilEmulado = 'Atendente'
const emulacaoSessionKey = 'sgx.auth.emulacao'
const localContextSessionKey = 'sgx.auth.localContext'
const localSgxTokenSessionKey = 'sgx.auth.localSgxToken'
let inicializacaoPromise: Promise<boolean> | null = null

interface EmulacaoSessionPayload {
  usuarioOriginal: UsuarioOriginalEmulacao
  perfilEmulado: PerfilEmulado
}

interface LocalContextSessionPayload {
  email: string
  nome: string
  perfil: PerfilUsuario
  emulandoPerfil: boolean
  perfilEmulado: PerfilEmulado | null
  usuarioOriginal: UsuarioOriginalEmulacao | null
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

function ehAdministrador(perfil: PerfilUsuario): boolean {
  return perfil === 'Administrador'
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

function ehPerfilUsuarioValido(perfil: unknown): perfil is PerfilUsuario {
  return perfil === 'Administrador' || perfil === 'Atendente' || perfil === 'Solicitante'
}

function ehPerfilEmuladoValido(perfil: unknown): perfil is PerfilEmulado {
  return perfil === 'Solicitante' || perfil === 'Atendente'
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

    if (!ehPerfilEmuladoValido(payload.perfilEmulado) || !ehPerfilUsuarioValido(payload.usuarioOriginal.perfil)) {
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

function salvarContextoLocalSessionStorage(payload: LocalContextSessionPayload): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.setItem(localContextSessionKey, JSON.stringify(payload))
}

function carregarContextoLocalSessionStorage(): LocalContextSessionPayload | null {
  if (typeof window === 'undefined') {
    return null
  }

  const raw = window.sessionStorage.getItem(localContextSessionKey)
  if (!raw) {
    return null
  }

  try {
    const payload = JSON.parse(raw) as LocalContextSessionPayload
    if (!payload?.email || !payload?.nome || !ehPerfilUsuarioValido(payload?.perfil)) {
      return null
    }

    if (typeof payload.emulandoPerfil !== 'boolean') {
      return null
    }

    if (payload.perfilEmulado !== null && !ehPerfilEmuladoValido(payload.perfilEmulado)) {
      return null
    }

    if (payload.usuarioOriginal) {
      if (
        !payload.usuarioOriginal.email ||
        !payload.usuarioOriginal.nome ||
        !ehPerfilUsuarioValido(payload.usuarioOriginal.perfil)
      ) {
        return null
      }
    }

    return payload
  } catch {
    return null
  }
}

function limparContextoLocalSessionStorage(): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.removeItem(localContextSessionKey)
}

function salvarTokenLocalSgxSessionStorage(token: string): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.setItem(localSgxTokenSessionKey, token)
}

function carregarTokenLocalSgxSessionStorage(): string | null {
  if (typeof window === 'undefined') {
    return null
  }

  const token = window.sessionStorage.getItem(localSgxTokenSessionKey)
  return token && token.trim().length > 0 ? token : null
}

function limparTokenLocalSgxSessionStorage(): void {
  if (typeof window === 'undefined') {
    return
  }

  window.sessionStorage.removeItem(localSgxTokenSessionKey)
}

function obterDadosPerfilEmulado(perfil: PerfilEmulado): {
  email: string
  nome: string
  perfil: PerfilUsuario
  descricao: string
} {
  if (perfil === 'Atendente') {
    return {
      email: atendenteDemoEmail,
      nome: atendenteDemoNome,
      perfil: atendenteDemoPerfil,
      descricao: 'Atendente',
    }
  }

  return {
    email: solicitanteDemoEmail,
    nome: solicitanteDemoNome,
    perfil: solicitanteDemoPerfil,
    descricao: 'Solicitante',
  }
}

function normalizarUsuarioAutenticado(usuario: MeResponse): MeResponse {
  return {
    ...usuario,
    perfis: Array.isArray(usuario.perfis) ? usuario.perfis : [],
    permissoes: Array.isArray(usuario.permissoes) ? usuario.permissoes : [],
    deveAlterarSenha: Boolean(usuario.deveAlterarSenha),
  }
}

function construirUsuarioLocalFallback(contexto: LocalContextSessionPayload): MeResponse {
  return {
    id: contexto.email,
    nome: contexto.nome,
    email: contexto.email,
    login: contexto.email,
    situacao: 'Ativo',
    perfis: [contexto.perfil],
    permissoes: [],
    departamentoId: null,
    autenticadoPor: 'LocalDevelopment',
    deveAlterarSenha: false,
  }
}

function ehErroNaoAutorizado(error: unknown): boolean {
  return error instanceof HttpRequestError && error.status === 401
}

function ehErroAcessoNegado(error: unknown): boolean {
  return error instanceof HttpRequestError && error.status === 403
}

function ehErroCancelamento(error: unknown): boolean {
  if (!error) {
    return false
  }

  if (error instanceof DOMException && error.name === 'AbortError') {
    return true
  }

  if (typeof error === 'object' && error !== null) {
    const candidate = error as { name?: string; code?: string; message?: string }
    const mensagem = candidate.message?.toLowerCase() ?? ''
    return (
      candidate.name === 'CanceledError' ||
      candidate.name === 'AbortError' ||
      candidate.code === 'ERR_CANCELED' ||
      mensagem.includes('aborted') ||
      mensagem.includes('canceled')
    )
  }

  return false
}

function mensagemContaMicrosoftNaoAutorizada(): string {
  return (
    'Esta conta Microsoft não está autorizada para acessar este ambiente. ' +
    'Use uma conta corporativa do tenant configurado para o SGX Sistema de Chamados. ' +
    'Contas pessoais Microsoft não são aceitas neste ambiente. ' +
    'Se você acredita que deveria ter acesso, procure o administrador do sistema.'
  )
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    inicializado: false,
    carregandoSessao: false,
    inicializandoSessao: false,
    carregando: false,
    autenticado: false,
    token: null,
    usuario: null,
    erro: null,
    erroAutenticacao: null,
    erroInicializacao: null,
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
    possuiPerfil:
      (state) =>
      (perfil: PerfilUsuario): boolean => {
        const perfis = state.usuario?.perfis ?? []
        return perfis.includes(perfil)
      },
    possuiAlgumPerfil:
      (state) =>
      (perfisRequeridos: PerfilUsuario[]): boolean => {
        const perfis = state.usuario?.perfis ?? []
        return perfisRequeridos.some((perfil) => perfis.includes(perfil))
      },
    podeEmularSolicitante(state): boolean {
      if (!state.modoLocal || import.meta.env.PROD) {
        return false
      }

      const perfis = state.usuario?.perfis ?? []
      return perfis.some((perfil) => ehAdministrador(perfil))
    },
    podeEmularAtendente(state): boolean {
      if (!state.modoLocal || import.meta.env.PROD) {
        return false
      }

      const perfis = state.usuario?.perfis ?? []
      return perfis.some((perfil) => ehAdministrador(perfil))
    },
    possuiPermissao:
      (state) =>
      (codigo: string): boolean => {
        const codigoNormalizado = codigo?.trim()
        if (!codigoNormalizado) {
          return false
        }

        const permissoes = state.usuario?.permissoes ?? []
        return permissoes.some((permissao) => permissao?.toLowerCase() === codigoNormalizado.toLowerCase())
      },
    possuiAlgumaPermissao:
      (state) =>
      (codigos: string[]): boolean => {
        const permissoes = state.usuario?.permissoes ?? []
        const permissoesNormalizadas = new Set(
          permissoes
            .filter((permissao): permissao is string => typeof permissao === 'string')
            .map((permissao) => permissao.toLowerCase())
        )

        return codigos.some((codigo) => {
          const codigoNormalizado = codigo?.trim().toLowerCase()
          return Boolean(codigoNormalizado) && permissoesNormalizadas.has(codigoNormalizado)
        })
      },
    possuiTodasPermissoes:
      (state) =>
      (codigos: string[]): boolean => {
        const permissoes = state.usuario?.permissoes ?? []
        const permissoesNormalizadas = new Set(
          permissoes
            .filter((permissao): permissao is string => typeof permissao === 'string')
            .map((permissao) => permissao.toLowerCase())
        )

        return codigos.every((codigo) => {
          const codigoNormalizado = codigo?.trim().toLowerCase()
          return Boolean(codigoNormalizado) && permissoesNormalizadas.has(codigoNormalizado)
        })
      },
    deveAlterarSenha(state): boolean {
      return Boolean(state.usuario?.deveAlterarSenha)
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

    persistirContextoLocal(): void {
      if (!this.modoLocal || import.meta.env.PROD || !this.autenticado) {
        limparContextoLocalSessionStorage()
        return
      }

      salvarContextoLocalSessionStorage({
        email: this.localDevEmail,
        nome: this.localDevNome,
        perfil: this.localDevPerfil,
        emulandoPerfil: this.emulandoPerfil,
        perfilEmulado: this.perfilEmulado,
        usuarioOriginal: this.usuarioOriginal,
      })
    },

    restaurarContextoLocal(): LocalContextSessionPayload | null {
      if (import.meta.env.PROD || !this.modoLocal) {
        limparContextoLocalSessionStorage()
        limparEmulacaoSessionStorage()
        return null
      }

      const contextoAtual = carregarContextoLocalSessionStorage()
      if (contextoAtual) {
        return contextoAtual
      }

      const emulacaoLegada = carregarEmulacaoSessionStorage()
      if (!emulacaoLegada) {
        return null
      }

      const dadosPerfilEmulado = obterDadosPerfilEmulado(emulacaoLegada.perfilEmulado)
      return {
        email: dadosPerfilEmulado.email,
        nome: dadosPerfilEmulado.nome,
        perfil: dadosPerfilEmulado.perfil,
        emulandoPerfil: true,
        perfilEmulado: emulacaoLegada.perfilEmulado,
        usuarioOriginal: emulacaoLegada.usuarioOriginal,
      }
    },

    aplicarContextoLocal(contexto: LocalContextSessionPayload): void {
      this.localDevEmail = contexto.email
      this.localDevNome = contexto.nome
      this.localDevPerfil = contexto.perfil
      this.emulandoPerfil = contexto.emulandoPerfil
      this.perfilEmulado = contexto.perfilEmulado
      this.usuarioOriginal = contexto.usuarioOriginal

      if (this.emulandoPerfil && this.usuarioOriginal && this.perfilEmulado) {
        salvarEmulacaoSessionStorage({
          usuarioOriginal: this.usuarioOriginal,
          perfilEmulado: this.perfilEmulado,
        })
      } else {
        limparEmulacaoSessionStorage()
      }
    },

    obterUsuarioOriginalPersistido(): UsuarioOriginalEmulacao | null {
      if (this.usuarioOriginal) {
        return this.usuarioOriginal
      }

      const contexto = carregarContextoLocalSessionStorage()
      if (contexto?.usuarioOriginal) {
        return contexto.usuarioOriginal
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

    async inicializarSessao(): Promise<boolean> {
      if (this.inicializado) {
        return this.autenticado
      }

      if (inicializacaoPromise) {
        return await inicializacaoPromise
      }

      inicializacaoPromise = (async (): Promise<boolean> => {
        this.carregandoSessao = true
        this.inicializandoSessao = true
        this.erro = null
        this.erroAutenticacao = null
        this.erroInicializacao = null
        setHttpAuthRedirectSuppressed(true)

        try {
          if (this.modoLocal && !import.meta.env.PROD) {
            const contexto = this.restaurarContextoLocal()

            if (contexto) {
              this.aplicarContextoLocal(contexto)
            } else {
              this.localDevEmail = adminLocalDevEmail
              this.localDevNome = adminLocalDevNome
              this.localDevPerfil = adminLocalDevPerfil
              this.limparEmulacao()
            }

            this.aplicarHeadersModoLocal()

            try {
              await this.carregarMe()
              this.autenticado = true
              this.persistirContextoLocal()
              return true
            } catch (error) {
              if (ehErroNaoAutorizado(error)) {
                this.reset()
                this.autenticado = false
                return false
              } else if (ehErroAcessoNegado(error)) {
                this.erro = mensagemContaMicrosoftNaoAutorizada()
                this.erroAutenticacao = this.erro
                this.autenticado = true
                return true
              } else if (ehErroCancelamento(error)) {
                const contextoValido = Boolean(contexto || this.usuario || this.autenticado)
                if (contexto && !this.usuario) {
                  this.usuario = construirUsuarioLocalFallback(contexto)
                  this.autenticado = true
                  this.persistirContextoLocal()
                  return true
                }

                this.erroInicializacao = 'Inicialização da sessão cancelada pelo navegador.'
                return contextoValido
              } else if (contexto) {
                this.usuario = construirUsuarioLocalFallback(contexto)
                this.autenticado = true
                this.erro = 'Sessão local restaurada parcialmente. Verifique a conectividade da API.'
                this.erroInicializacao = this.erro
                this.persistirContextoLocal()
                return true
              } else {
                this.autenticado = false
                this.usuario = null
                this.erro = error instanceof Error ? error.message : 'Não foi possível restaurar a sessão local.'
                this.erroInicializacao = this.erro
                this.erroAutenticacao = this.erro
                return false
              }
            }
          }

          limparContextoLocalSessionStorage()
          limparEmulacaoSessionStorage()

          const tokenLocalSgx = carregarTokenLocalSgxSessionStorage()
          if (tokenLocalSgx) {
            setHttpAuthToken(tokenLocalSgx)
            setHttpLocalDevHeaders(null)
            this.token = tokenLocalSgx

            try {
              await this.carregarMe()
              this.autenticado = true
              return true
            } catch (error) {
              if (!ehErroNaoAutorizado(error)) {
                throw error
              }

              limparTokenLocalSgxSessionStorage()
              setHttpAuthToken(null)
              this.token = null
            }
          }

          if (!authService.isAzureConfigured()) {
            this.autenticado = false
            this.usuario = null
            this.token = null
            setHttpAuthToken(null)
            return false
          }

          const account = await authService.getAccount()
          if (!account) {
            this.autenticado = false
            this.usuario = null
            this.token = null
            setHttpAuthToken(null)
            return false
          }

          const token = await authService.acquireAccessToken(account)
          if (!token) {
            this.autenticado = false
            this.usuario = null
            this.token = null
            setHttpAuthToken(null)
            return false
          }

          setHttpAuthToken(token)
          setHttpLocalDevHeaders(null)
          this.limparEmulacao()
          this.token = token

          await this.carregarMe()
          this.autenticado = true
          return true
        } catch (error) {
          if (ehErroCancelamento(error)) {
            this.erroInicializacao = 'Inicialização da sessão cancelada pelo navegador.'
            return this.autenticado
          }

          if (ehErroNaoAutorizado(error)) {
            this.reset()
            this.autenticado = false
            return false
          }

          if (ehErroAcessoNegado(error)) {
            this.erro = mensagemContaMicrosoftNaoAutorizada()
            this.erroAutenticacao = this.erro
            this.autenticado = true
            return true
          }

          this.reset()
          this.erro = error instanceof Error ? error.message : 'Não foi possível inicializar a autenticação.'
          this.erroAutenticacao = this.erro
          this.erroInicializacao = this.erro
          return false
        } finally {
          this.inicializado = true
          this.carregandoSessao = false
          this.inicializandoSessao = false
          setHttpAuthRedirectSuppressed(false)
          inicializacaoPromise = null
        }
      })()

      return await inicializacaoPromise
    },

    async initialize(): Promise<void> {
      await this.inicializarSessao()
    },

    async loginMicrosoft(): Promise<void> {
      this.carregando = true
      this.erro = null
      this.erroAutenticacao = null

      try {
        const result = await authService.loginPopup()
        const token =
          result.accessToken ||
          (await authService.acquireAccessToken(result.account ?? (await authService.getAccount())))

        if (!token) {
          throw new Error('Não foi possível obter access token do Microsoft Entra ID.')
        }

        setHttpAuthToken(token)
        setHttpLocalDevHeaders(null)
        limparTokenLocalSgxSessionStorage()
        limparContextoLocalSessionStorage()
        this.limparEmulacao()
        this.token = token

        await this.carregarMe()
        this.autenticado = true
      } catch (error) {
        if (authService.isUserCancellation(error)) {
          this.erro = 'Autenticação Microsoft cancelada pelo usuário.'
          this.erroAutenticacao = this.erro
          throw new Error(this.erro)
        }

        if (error instanceof HttpRequestError && (error.status === 401 || error.status === 403)) {
          const mensagem = mensagemContaMicrosoftNaoAutorizada()
          this.reset()
          this.erro = mensagem
          this.erroAutenticacao = mensagem
          throw new Error(mensagem)
        }

        this.reset()
        this.erro =
          error instanceof Error
            ? error.message
            : 'Não foi possível concluir a autenticação. Tente novamente.'
        this.erroAutenticacao = this.erro
        throw error
      } finally {
        this.inicializado = true
        this.carregandoSessao = false
        this.inicializandoSessao = false
        this.carregando = false
      }
    },

    async loginLocalSgx(payload: { email: string; senha: string }): Promise<void> {
      const email = payload.email?.trim().toLowerCase() ?? ''
      const senha = payload.senha ?? ''

      if (!email || !senha) {
        throw new Error('Informe e-mail e senha para login local SGX.')
      }

      this.carregando = true
      this.erro = null
      this.erroAutenticacao = null

      try {
        const resposta = await httpClient.post<LocalLoginResponse>('/api/auth/local/login', {
          email,
          senha,
        })

        if (!resposta?.accessToken) {
          throw new Error('A API não retornou token de acesso para login local SGX.')
        }

        setHttpLocalDevHeaders(null)
        setHttpAuthToken(resposta.accessToken)
        salvarTokenLocalSgxSessionStorage(resposta.accessToken)
        limparContextoLocalSessionStorage()
        limparEmulacaoSessionStorage()
        this.limparEmulacao()
        this.token = resposta.accessToken

        await this.carregarMe()
        if (this.usuario && resposta.deveAlterarSenha) {
          this.usuario.deveAlterarSenha = true
        }
        this.autenticado = true
      } catch (error) {
        limparTokenLocalSgxSessionStorage()
        this.reset()
        this.erro =
          error instanceof Error
            ? error.message
            : 'Não foi possível concluir o login local SGX. Tente novamente.'
        this.erroAutenticacao = this.erro
        throw error
      } finally {
        this.inicializado = true
        this.carregandoSessao = false
        this.inicializandoSessao = false
        this.carregando = false
      }
    },

    async loginLocalDev(payload?: { email?: string; nome?: string; perfil?: PerfilUsuario }): Promise<void> {
      if (!this.modoLocal || import.meta.env.PROD) {
        throw new Error('Modo local de autenticacao esta desabilitado.')
      }

      this.limparEmulacao()
      this.localDevEmail = payload?.email?.trim() || adminLocalDevEmail
      this.localDevNome = payload?.nome?.trim() || adminLocalDevNome
      this.localDevPerfil = payload?.perfil || adminLocalDevPerfil

      limparTokenLocalSgxSessionStorage()
      setHttpAuthToken(null)
      this.aplicarHeadersModoLocal()

      this.token = null
      this.erro = null
      this.erroAutenticacao = null
      this.carregando = true

      try {
        await this.carregarMe()
        this.autenticado = true
        this.inicializado = true
        this.persistirContextoLocal()
      } catch (error) {
        this.reset()
        this.erro = error instanceof Error ? error.message : 'Falha no login local.'
        this.erroAutenticacao = this.erro
        throw error
      } finally {
        this.carregando = false
      }
    },

    async iniciarEmulacaoPerfil(perfil: PerfilEmulado): Promise<void> {
      if (import.meta.env.PROD || !this.modoLocal) {
        throw new Error('A emulação de perfil está disponível apenas em Development local.')
      }

      if (!this.autenticado || !this.usuario) {
        throw new Error('Não foi possível identificar o usuário autenticado para iniciar a emulação.')
      }

      const perfis = this.usuario.perfis ?? []
      const usuarioOriginalPersistido = this.obterUsuarioOriginalPersistido()
      const administradorAtual = perfis.some((perfilAtual) => ehAdministrador(perfilAtual))
      const administradorOriginal = usuarioOriginalPersistido?.perfil === 'Administrador'

      if (!administradorAtual && !administradorOriginal) {
        throw new Error('Apenas Administrador pode iniciar emulação de perfis.')
      }

      if (this.emulandoPerfil && this.perfilEmulado === perfil) {
        return
      }

      const dadosPerfilEmulado = obterDadosPerfilEmulado(perfil)
      const perfilAnteriorEmulacao = this.perfilEmulado
      const emulacaoAnteriorAtiva = this.emulandoPerfil
      const usuarioOriginalAnterior = this.usuarioOriginal ?? usuarioOriginalPersistido
      const contextoAnterior: UsuarioOriginalEmulacao = {
        email: this.localDevEmail,
        nome: this.localDevNome,
        perfil: this.localDevPerfil,
      }

      let contextoOriginal: UsuarioOriginalEmulacao
      if (emulacaoAnteriorAtiva) {
        if (!usuarioOriginalPersistido) {
          throw new Error('Contexto original da emulação não encontrado.')
        }

        contextoOriginal = usuarioOriginalPersistido
      } else {
        contextoOriginal = {
          email: this.localDevEmail || this.usuario.email,
          nome: this.localDevNome || this.usuario.nome,
          perfil: obterPerfilAdministrativo(perfis, this.localDevPerfil),
        }
      }

      this.usuarioOriginal = contextoOriginal
      this.perfilEmulado = perfil
      this.emulandoPerfil = true
      salvarEmulacaoSessionStorage({
        usuarioOriginal: contextoOriginal,
        perfilEmulado: perfil,
      })

      this.localDevEmail = dadosPerfilEmulado.email
      this.localDevNome = dadosPerfilEmulado.nome
      this.localDevPerfil = dadosPerfilEmulado.perfil
      this.aplicarHeadersModoLocal()

      try {
        await this.carregarMe()
        this.persistirContextoLocal()
      } catch (error) {
        this.localDevEmail = contextoAnterior.email
        this.localDevNome = contextoAnterior.nome
        this.localDevPerfil = contextoAnterior.perfil
        this.aplicarHeadersModoLocal()

        if (emulacaoAnteriorAtiva && usuarioOriginalAnterior && perfilAnteriorEmulacao) {
          this.usuarioOriginal = usuarioOriginalAnterior
          this.emulandoPerfil = true
          this.perfilEmulado = perfilAnteriorEmulacao
          salvarEmulacaoSessionStorage({
            usuarioOriginal: usuarioOriginalAnterior,
            perfilEmulado: perfilAnteriorEmulacao,
          })
          this.persistirContextoLocal()
        } else {
          this.limparEmulacao()
          this.persistirContextoLocal()
        }

        const mensagem = error instanceof Error ? error.message : 'Não foi possível sincronizar usuário emulado.'
        throw new Error(`Não foi possível iniciar a emulação de ${dadosPerfilEmulado.descricao}. ${mensagem}`)
      }
    },

    async iniciarEmulacaoSolicitante(): Promise<void> {
      await this.iniciarEmulacaoPerfil('Solicitante')
    },

    async iniciarEmulacaoAtendente(): Promise<void> {
      await this.iniciarEmulacaoPerfil('Atendente')
    },

    async encerrarEmulacao(): Promise<void> {
      if (!this.emulandoPerfil) {
        return
      }

      const perfilEmuladoAtual = this.perfilEmulado
      const usuarioOriginal = this.obterUsuarioOriginalPersistido()
      if (!usuarioOriginal) {
        this.reset()
        throw new Error('Contexto original da emulação não encontrado. Realize novo login.')
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
        this.persistirContextoLocal()
      } catch (error) {
        this.localDevEmail = contextoEmuladoAtual.email
        this.localDevNome = contextoEmuladoAtual.nome
        this.localDevPerfil = contextoEmuladoAtual.perfil
        this.aplicarHeadersModoLocal()
        this.usuarioOriginal = usuarioOriginal
        this.emulandoPerfil = true
        this.perfilEmulado = perfilEmuladoAtual

        if (perfilEmuladoAtual) {
          salvarEmulacaoSessionStorage({
            usuarioOriginal,
            perfilEmulado: perfilEmuladoAtual,
          })
        } else {
          limparEmulacaoSessionStorage()
        }

        this.persistirContextoLocal()

        const mensagem = error instanceof Error ? error.message : 'Não foi possível restaurar usuário administrativo.'
        throw new Error(`Não foi possível encerrar a emulação. ${mensagem}`)
      }
    },

    async logout(): Promise<void> {
      this.limparEmulacao()

      if (this.usuario?.autenticadoPor === 'LocalSgx') {
        limparTokenLocalSgxSessionStorage()
      } else if (!this.modoLocal && authService.isAzureConfigured()) {
        await authService.logout()
      }

      this.reset()
    },

    reset(): void {
      this.limparEmulacao()
      this.autenticado = false
      this.usuario = null
      this.token = null
      this.erro = null
      this.erroAutenticacao = null
      this.erroInicializacao = null
      this.localDevEmail = adminLocalDevEmail
      this.localDevNome = adminLocalDevNome
      this.localDevPerfil = adminLocalDevPerfil
      limparTokenLocalSgxSessionStorage()
      limparContextoLocalSessionStorage()
      setHttpAuthToken(null)
      setHttpLocalDevHeaders(null)
    },

    async loginLocalDevelopment(payload?: { email?: string; nome?: string; perfil?: PerfilUsuario }): Promise<void> {
      await this.loginLocalDev(payload)
    },

    async emularSolicitante(): Promise<void> {
      await this.iniciarEmulacaoSolicitante()
    },

    async emularAtendente(): Promise<void> {
      await this.iniciarEmulacaoAtendente()
    },

    async voltarParaAdministrador(): Promise<void> {
      await this.encerrarEmulacao()
    },

    async carregarMe(): Promise<void> {
      const response = await httpClient.get<MeResponse>('/api/me')
      this.usuario = normalizarUsuarioAutenticado(response)
    },

    async alterarSenhaLocal(payload: {
      senhaAtual: string
      novaSenha: string
      confirmacaoNovaSenha: string
    }): Promise<string> {
      const response = await httpClient.post<MensagemAuthResponse>('/api/auth/local/alterar-senha', payload)
      await this.carregarMe()
      return response.mensagem
    },

    async solicitarRecuperacaoSenha(email: string): Promise<string> {
      const response = await httpClient.post<MensagemAuthResponse>('/api/auth/local/recuperar-senha/solicitar', {
        email,
      })

      return response.mensagem
    },

    async redefinirSenhaLocal(payload: {
      token: string
      novaSenha: string
      confirmacaoNovaSenha: string
    }): Promise<string> {
      const response = await httpClient.post<MensagemAuthResponse>('/api/auth/local/recuperar-senha/redefinir', payload)
      return response.mensagem
    },
  },
})

