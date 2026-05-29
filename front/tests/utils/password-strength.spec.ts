import { describe, expect, it } from 'vitest'
import { getPasswordStrength } from '~/utils/password-strength'

describe('password-strength', () => {
  it('classifies empty passwords', () => {
    const strength = getPasswordStrength('')

    expect(strength.label).toBe('Nao informada')
    expect(strength.percent).toBe(0)
  })

  it('classifies weak passwords', () => {
    const strength = getPasswordStrength('abc')

    expect(strength.label).toBe('Fraca')
    expect(strength.hints.length).toBeGreaterThan(0)
  })

  it('classifies strong passwords', () => {
    const strength = getPasswordStrength('Senha@252526')

    expect(strength.label).toBe('Forte')
    expect(strength.percent).toBe(100)
    expect(strength.hints).toHaveLength(0)
  })
})
