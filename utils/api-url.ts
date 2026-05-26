export function resolveApiAssetUrl(path?: string | null, apiBase = '') {
  if (!path) return ''

  if (/^https?:\/\//i.test(path)) {
    return path
  }

  if (!path.startsWith('/')) {
    return path
  }

  const normalizedBase = (apiBase || '').trim()

  if (!normalizedBase || normalizedBase === '/api') {
    return path
  }

  if (normalizedBase.startsWith('http://') || normalizedBase.startsWith('https://')) {
    const url = new URL(normalizedBase)
    const basePath = url.pathname.replace(/\/api\/?$/i, '').replace(/\/$/, '')
    return `${url.origin}${basePath}${path}`
  }

  return `${normalizedBase.replace(/\/api\/?$/i, '').replace(/\/$/, '')}${path}`
}
