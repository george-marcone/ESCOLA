import type { $Fetch, FetchOptions } from 'ofetch'

export type ApiClient = <T>(path: string, options?: FetchOptions<'json'>) => Promise<T>

interface ApiClientConfig {
  baseURL: string
  getToken: () => string | null
  onUnauthorized?: () => void
}

export function createApiClient(config: ApiClientConfig): ApiClient {
  return async <T>(path: string, options: FetchOptions<'json'> = {}) => {
    const headers = buildHeaders(options.headers, config.getToken())
    const apiFetch = $fetch as unknown as $Fetch
    const baseURL = resolveApiBase(config.baseURL)

    try {
      return await apiFetch<T>(path, {
        ...options,
        baseURL,
        headers
      })
    } catch (error) {
      if (isUnauthorized(error)) {
        config.onUnauthorized?.()
      }

      throw error
    }
  }
}

export function resolveApiBase(apiBase: string, hostname = getBrowserHostname()): string {
  const normalizedApiBase = apiBase.trim() || '/api'

  if (hostname && !isLocalHostname(hostname) && isLocalApiUrl(normalizedApiBase)) {
    return '/api'
  }

  return normalizedApiBase
}

export function buildHeaders(headers: HeadersInit | undefined, token: string | null): Headers {
  const nextHeaders = new Headers(headers)

  if (token) {
    nextHeaders.set('Authorization', `Bearer ${token}`)
  }

  return nextHeaders
}

export function normalizeApiError(error: unknown, hostname = getBrowserHostname()): string {
  if (typeof error === 'string') {
    return error
  }

  if (!error || typeof error !== 'object') {
    return 'Nao foi possivel concluir a operacao.'
  }

  const maybeError = error as {
    data?: unknown
    request?: unknown
    response?: { _data?: unknown }
    message?: string
  }
  const data = maybeError.data ?? maybeError.response?._data

  if (typeof data === 'string') {
    return data
  }

  if (data && typeof data === 'object' && 'errors' in data) {
    const errors = (data as { errors?: Record<string, string[]> }).errors
    const messages = Object.values(errors ?? {}).flat()

    if (messages.length) {
      return messages.join(' ')
    }
  }

  if (isNetworkError(maybeError)) {
    const request = String(maybeError.request ?? '')

    if (isLocalApiUrl(request)) {
      if (isLocalHostname(hostname)) {
        return 'Nao foi possivel conectar a API local. Verifique se o backend esta rodando em http://localhost:5001/api.'
      }

      return 'A API publicada esta configurada para localhost. Configure NUXT_PUBLIC_API_BASE com a URL publica do backend.'
    }

    return 'Nao foi possivel conectar a API. Verifique se NUXT_PUBLIC_API_BASE aponta para a URL publica do backend.'
  }

  return maybeError.message || 'Nao foi possivel concluir a operacao.'
}

function isUnauthorized(error: unknown): boolean {
  if (!error || typeof error !== 'object') {
    return false
  }

  const maybeError = error as {
    status?: number
    response?: { status?: number }
  }

  return maybeError.status === 401 || maybeError.response?.status === 401
}

function getBrowserHostname(): string {
  return typeof window === 'undefined' ? '' : window.location.hostname
}

function isLocalHostname(hostname: string): boolean {
  return ['localhost', '127.0.0.1', '::1'].includes(hostname)
}

function isLocalApiUrl(value: string): boolean {
  try {
    return isLocalHostname(new URL(value).hostname)
  } catch {
    return false
  }
}

function isNetworkError(error: { message?: string }): boolean {
  const message = error.message ?? ''
  return message.includes('Failed to fetch') || message.includes('<no response>')
}
