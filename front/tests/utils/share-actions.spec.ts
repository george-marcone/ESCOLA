import { describe, expect, it } from 'vitest'
import { buildGmailComposeUrl } from '~/utils/share-actions'

describe('share-actions', () => {
  it('builds a Gmail compose URL with subject and body', () => {
    const url = new URL(buildGmailComposeUrl({
      subject: 'Holerite 05/2026',
      body: 'Competencia: 05/2026\nArquivo: holerite.pdf'
    }))

    expect(url.origin).toBe('https://mail.google.com')
    expect(url.searchParams.get('view')).toBe('cm')
    expect(url.searchParams.get('fs')).toBe('1')
    expect(url.searchParams.get('su')).toBe('Holerite 05/2026')
    expect(url.searchParams.get('body')).toContain('Competencia: 05/2026')
  })

  it('includes the recipient when provided', () => {
    const url = new URL(buildGmailComposeUrl({
      to: ' professor@escola.com ',
      subject: 'Aviso',
      body: 'Mensagem'
    }))

    expect(url.searchParams.get('to')).toBe('professor@escola.com')
  })
})
