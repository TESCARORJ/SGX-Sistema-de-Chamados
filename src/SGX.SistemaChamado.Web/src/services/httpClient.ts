const DEFAULT_HTTP_TIMEOUT_MS = 30_000
const API_BASE_URL = normalizarBaseUrl(import.meta.env.VITE_API_BASE_URL)
const HTTP_TIMEOUT_MS = normalizarTimeout(import.meta.env.VITE_HTTP_TIMEOUT_MS)

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

let bearerToken: string | null = null
let devHeaders: Record<string, string> | null = null
let authRedirectSuppressed = false

export class HttpRequestError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
    this.name = 'HttpRequestError'
  }
}

function redirectTo(path: '/login' | '/acesso-negado'): void {
  if (typeof window === 'undefined') {
    return
  }

  if (window.location.pathname === path) {
    return
  }

  window.location.assign(path)
}

function normalizarBaseUrl(value: string | undefined): string {
  const baseUrl = (value ?? '').trim()
  if (!baseUrl) {
    return ''
  }

  return baseUrl.replace(/\/+$/, '')
}

function normalizarTimeout(value: string | number | undefined): number {
  const timeout = Number(value ?? DEFAULT_HTTP_TIMEOUT_MS)
  if (!Number.isFinite(timeout) || timeout <= 0) {
    return DEFAULT_HTTP_TIMEOUT_MS
  }

  return Math.trunc(timeout)
}

function buildRequestUrl(path: string): string {
  return `${API_BASE_URL}${path}`
}

function formatarMensagemTimeout(timeoutMs: number): string {
  return `A API demorou mais de ${Math.trunc(timeoutMs / 1000)} segundos para responder.`
}

function criarSignalComTimeout(
  timeoutMs: number,
  signalExterno?: AbortSignal
): { signal: AbortSignal; limpar: () => void; expirou: () => boolean } {
  const controller = new AbortController()
  let timeoutExpirado = false

  const timeoutHandle = setTimeout(() => {
    timeoutExpirado = true
    controller.abort()
  }, timeoutMs)

  const abortarPorSinalExterno = () => {
    controller.abort(signalExterno?.reason)
  }

  if (signalExterno) {
    if (signalExterno.aborted) {
      abortarPorSinalExterno()
    } else {
      signalExterno.addEventListener('abort', abortarPorSinalExterno, { once: true })
    }
  }

  return {
    signal: controller.signal,
    limpar: () => {
      clearTimeout(timeoutHandle)
      signalExterno?.removeEventListener('abort', abortarPorSinalExterno)
    },
    expirou: () => timeoutExpirado,
  }
}

async function fetchWithTimeout(
  url: string,
  init: RequestInit,
  timeoutMs = HTTP_TIMEOUT_MS
): Promise<Response> {
  const { signal, limpar, expirou } = criarSignalComTimeout(timeoutMs, init.signal)

  try {
    return await fetch(url, {
      ...init,
      signal,
    })
  } catch (error) {
    if (expirou()) {
      throw new HttpRequestError(408, formatarMensagemTimeout(timeoutMs))
    }

    throw error
  } finally {
    limpar()
  }
}

async function request<T>(
  path: string,
  method: HttpMethod,
  body?: unknown,
  timeoutMs = HTTP_TIMEOUT_MS
): Promise<T> {
  const headers: HeadersInit = {}
  const isFormData = typeof FormData !== 'undefined' && body instanceof FormData

  if (!isFormData) {
    headers['Content-Type'] = 'application/json'
  }

  if (bearerToken) {
    headers.Authorization = `Bearer ${bearerToken}`
  }

  if (!import.meta.env.PROD && devHeaders) {
    Object.assign(headers, devHeaders)
  }

  const response = await fetchWithTimeout(buildRequestUrl(path), {
    method,
    headers,
    body: body ? (isFormData ? body : JSON.stringify(body)) : undefined,
  }, timeoutMs)

  if (response.status === 401) {
    if (!authRedirectSuppressed) {
      redirectTo('/login')
    }

    throw new HttpRequestError(401, 'Acesso não autenticado (401).')
  }

  if (response.status === 403) {
    if (!authRedirectSuppressed) {
      redirectTo('/acesso-negado')
    }

    throw new HttpRequestError(403, 'Acesso negado (403).')
  }

  if (!response.ok) {
    const message = await response.text()
    throw new HttpRequestError(
      response.status,
      `HTTP ${response.status}: ${message || 'Erro ao processar requisição.'}`
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

function extrairNomeArquivo(contentDisposition: string | null): string | null {
  if (!contentDisposition) {
    return null
  }

  const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
  if (utf8Match?.[1]) {
    try {
      return decodeURIComponent(utf8Match[1]).replace(/["']/g, '')
    } catch {
      return utf8Match[1].replace(/["']/g, '')
    }
  }

  const asciiMatch = contentDisposition.match(/filename=\"?([^\";]+)\"?/i)
  return asciiMatch?.[1] ? asciiMatch[1].replace(/["']/g, '') : null
}

async function requestFile(
  path: string,
  method: HttpMethod,
  timeoutMs = HTTP_TIMEOUT_MS
): Promise<{ blob: Blob; nomeArquivo: string | null; contentType: string | null }> {
  const headers: HeadersInit = {}

  if (bearerToken) {
    headers.Authorization = `Bearer ${bearerToken}`
  }

  if (!import.meta.env.PROD && devHeaders) {
    Object.assign(headers, devHeaders)
  }

  const response = await fetchWithTimeout(buildRequestUrl(path), {
    method,
    headers,
  }, timeoutMs)

  if (response.status === 401) {
    if (!authRedirectSuppressed) {
      redirectTo('/login')
    }

    throw new HttpRequestError(401, 'Acesso não autenticado (401).')
  }

  if (response.status === 403) {
    if (!authRedirectSuppressed) {
      redirectTo('/acesso-negado')
    }

    throw new HttpRequestError(403, 'Acesso negado (403).')
  }

  if (!response.ok) {
    const message = await response.text()
    throw new HttpRequestError(
      response.status,
      `HTTP ${response.status}: ${message || 'Erro ao processar requisição.'}`
    )
  }

  return {
    blob: await response.blob(),
    nomeArquivo: extrairNomeArquivo(response.headers.get('content-disposition')),
    contentType: response.headers.get('content-type'),
  }
}

export function setHttpAuthToken(token: string | null): void {
  bearerToken = token
}

export function getHttpApiBaseUrl(): string {
  return API_BASE_URL
}

export function setHttpLocalDevHeaders(headers: Record<string, string> | null): void {
  devHeaders = import.meta.env.PROD ? null : headers
}

export function setHttpAuthRedirectSuppressed(suppressed: boolean): void {
  authRedirectSuppressed = suppressed
}

export const httpClient = {
  get: <T>(path: string) => request<T>(path, 'GET'),
  post: <T>(path: string, body?: unknown) => request<T>(path, 'POST', body),
  put: <T>(path: string, body?: unknown) => request<T>(path, 'PUT', body),
  patch: <T>(path: string, body?: unknown) => request<T>(path, 'PATCH', body),
  delete: <T>(path: string) => request<T>(path, 'DELETE'),
  getFile: (path: string) => requestFile(path, 'GET'),
}
