import { describe, expect, it } from 'vitest'
import { isDuplicateUserEmail, normalizeEmail } from '~/utils/usuario-validation'

const usuarios = [
  { idUsuario: 1, email: 'admin@escola.com' },
  { idUsuario: 2, email: 'usuario@escola.com' }
]

describe('usuario-validation', () => {
  it('normalizes emails before comparison', () => {
    expect(normalizeEmail(' Admin@Escola.com ')).toBe('admin@escola.com')
  })

  it('detects duplicate emails ignoring case and spaces', () => {
    expect(isDuplicateUserEmail(usuarios, ' ADMIN@ESCOLA.COM ')).toBe(true)
  })

  it('does not mark the current user email as duplicate while editing', () => {
    expect(isDuplicateUserEmail(usuarios, 'admin@escola.com', 1)).toBe(false)
  })

  it('allows a new email', () => {
    expect(isDuplicateUserEmail(usuarios, 'novo@escola.com')).toBe(false)
  })
})
