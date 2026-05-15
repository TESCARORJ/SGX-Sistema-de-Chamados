const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5168'

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

async function request<T>(path: string, method: HttpMethod, body?: unknown): Promise<T> {
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

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body ? (isFormData ? body : JSON.stringify(body)) : undefined,
  })

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

async function requestFile(path: string, method: HttpMethod): Promise<{ blob: Blob; nomeArquivo: string | null; contentType: string | null }> {
  const headers: HeadersInit = {}

  if (bearerToken) {
    headers.Authorization = `Bearer ${bearerToken}`
  }

  if (!import.meta.env.PROD && devHeaders) {
    Object.assign(headers, devHeaders)
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
  })

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

