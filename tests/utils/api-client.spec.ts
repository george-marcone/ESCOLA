import { describe, expect, it, vi } from 'vitest'
import { buildHeaders, createApiClient, normalizeApiError } from '~/utils/api-client'

describe('api-client', () => {
  it('adds the JWT bearer token when present', async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true })
    vi.stubGlobal('$fetch', fetchMock)

    const api = createApiClient({
      baseURL: 'http://localhost:5001/api',
      getToken: () => 'token-123'
    })

    await api('/diretoria')

    const options = fetchMock.mock.calls[0][1] as { baseURL: string, headers: Headers }
    expect(options.baseURL).toBe('http://localhost:5001/api')
    expect(options.headers.get('Authorization')).toBe('Bearer token-123')
  })

  it('keeps existing headers while adding authorization', () => {
    const headers = buildHeaders({ 'Content-Type': 'application/json' }, 'abc')

    expect(headers.get('Content-Type')).toBe('application/json')
    expect(headers.get('Authorization')).toBe('Bearer abc')
  })

  it('calls the unauthorized handler on 401 responses', async () => {
    const onUnauthorized = vi.fn()
    const fetchMock = vi.fn().mockRejectedValue({ response: { status: 401 } })
    vi.stubGlobal('$fetch', fetchMock)

    const api = createApiClient({
      baseURL: 'http://localhost:5001/api',
      getToken: () => null,
      onUnauthorized
    })

    await expect(api('/auth/me')).rejects.toEqual({ response: { status: 401 } })
    expect(onUnauthorized).toHaveBeenCalledTimes(1)
  })

  it('normalizes validation errors returned by the API', () => {
    const message = normalizeApiError({
      response: {
        _data: {
          errors: {
            email: ['Informe um email valido.'],
            senha: ['A senha e obrigatoria.']
          }
        }
      }
    })

    expect(message).toBe('Informe um email valido. A senha e obrigatoria.')
  })
})
