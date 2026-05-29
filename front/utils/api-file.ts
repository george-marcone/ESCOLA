import { buildHeaders, resolveApiBase } from '~/utils/api-client'

export async function fetchApiBlob(path: string, apiBase: string, token: string | null) {
  return await $fetch<Blob>(path, {
    baseURL: resolveApiBase(apiBase),
    headers: buildHeaders(undefined, token),
    responseType: 'blob'
  })
}

export function downloadBlob(blob: Blob, filename: string) {
  const objectUrl = URL.createObjectURL(blob)
  const link = document.createElement('a')

  link.href = objectUrl
  link.download = filename
  document.body.appendChild(link)
  link.click()
  link.remove()

  window.setTimeout(() => {
    URL.revokeObjectURL(objectUrl)
  }, 1000)
}
