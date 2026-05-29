export interface EmailSharePayload {
  subject: string
  body: string
  to?: string
}

export function buildGmailComposeUrl(payload: EmailSharePayload) {
  const params = new URLSearchParams({
    view: 'cm',
    fs: '1',
    su: payload.subject,
    body: payload.body
  })

  if (payload.to?.trim()) {
    params.set('to', payload.to.trim())
  }

  return `https://mail.google.com/mail/?${params.toString()}`
}

export async function copyShareText(text: string) {
  if (!navigator?.clipboard) {
    return false
  }

  await navigator.clipboard.writeText(text)
  return true
}

export async function openEmailCompose(payload: EmailSharePayload) {
  const url = buildGmailComposeUrl(payload)
  const opened = window.open(url, '_blank', 'noopener,noreferrer')

  if (opened) {
    return true
  }

  await copyShareText(`${payload.subject}\n\n${payload.body}`)
  return false
}
