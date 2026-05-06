const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5085'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

let bearerToken: string | null = null
let devHeaders: Record<string, string> | null = null

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

  if (devHeaders) {
    Object.assign(headers, devHeaders)
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body ? (isFormData ? body : JSON.stringify(body)) : undefined,
  })

  if (response.status === 401) {
    redirectTo('/login')
    throw new Error('Acesso nao autenticado (401).')
  }

  if (response.status === 403) {
    redirectTo('/acesso-negado')
    throw new Error('Acesso negado (403).')
  }

  if (!response.ok) {
    const message = await response.text()
    throw new Error(`HTTP ${response.status}: ${message || 'Erro ao processar requisicao.'}`)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

export function setHttpAuthToken(token: string | null): void {
  bearerToken = token
}

export function setHttpLocalDevHeaders(headers: Record<string, string> | null): void {
  devHeaders = headers
}

export const httpClient = {
  get: <T>(path: string) => request<T>(path, 'GET'),
  post: <T>(path: string, body?: unknown) => request<T>(path, 'POST', body),
  put: <T>(path: string, body?: unknown) => request<T>(path, 'PUT', body),
  patch: <T>(path: string, body?: unknown) => request<T>(path, 'PATCH', body),
  delete: <T>(path: string) => request<T>(path, 'DELETE'),
}
