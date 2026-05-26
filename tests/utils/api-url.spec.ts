import { describe, expect, it } from 'vitest'
import { resolveApiAssetUrl } from '~/utils/api-url'

describe('resolveApiAssetUrl', () => {
  it('keeps absolute urls', () => {
    expect(resolveApiAssetUrl('https://api.example.com/uploads/foto.jpg', '/api')).toBe('https://api.example.com/uploads/foto.jpg')
  })

  it('resolves uploads against a remote api origin', () => {
    expect(resolveApiAssetUrl('/uploads/foto.jpg', 'https://api.example.com/api')).toBe('https://api.example.com/uploads/foto.jpg')
  })

  it('uses same origin uploads when api base is proxied', () => {
    expect(resolveApiAssetUrl('/uploads/foto.jpg', '/api')).toBe('/uploads/foto.jpg')
  })
})
