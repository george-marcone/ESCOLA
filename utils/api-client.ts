import type { FetchOptions } from 'ofetch'

export type ApiClient = <T>(path: string, options?: FetchOptions<'json'>) => Promise<T>

interface ApiClientConfig {
  baseURL: string
  getToken: () => string | null
  onUnauthorized?: () => void
}

export function createApiClient(config: ApiClientConfig): ApiClient {
  return async <T>(path: string, options: FetchOptions<'json'> = {}) => {
    const headers = buildHeaders(options.headers, config.getToken())

    try {
      return await $fetch<T>(path, {
        ...options,
        baseURL: config.baseURL,
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

export function buildHeaders(headers: HeadersInit | undefined, token: string | null): Headers {
  const nextHeaders = new Headers(headers)

  if (token) {
    nextHeaders.set('Authorization', `Bearer ${token}`)
  }

  return nextHeaders
}

export function normalizeApiError(error: unknown): string {
  if (typeof error === 'string') {
    return error
  }

  if (!error || typeof error !== 'object') {
    return 'Nao foi possivel concluir a operacao.'
  }

  const maybeError = error as {
    data?: unknown
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
