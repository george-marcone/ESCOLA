import { describe, expect, it } from 'vitest'
import {
  formatPhone,
  formatPhoneForDisplay,
  formatBrazilPhone,
  getBrazilPhoneLocalDigits,
  isCompletePhone,
  isCompleteBrazilPhone,
  normalizePhoneForApi
} from '~/utils/br-phone'

describe('br-phone', () => {
  it('formats a phone with editable two-digit country code', () => {
    expect(formatPhone('3511999999999')).toBe('+35 (11) 99999-9999')
  })

  it('formats API values without duplicating the country code', () => {
    expect(formatPhone('+5511999999999')).toBe('+55 (11) 99999-9999')
  })

  it('formats legacy local Brazilian values for display', () => {
    expect(formatPhoneForDisplay('11999999999')).toBe('+55 (11) 99999-9999')
    expect(formatBrazilPhone('11999999999')).toBe('+55 (11) 99999-9999')
  })

  it('keeps DDD 55 when the value has no explicit country code', () => {
    expect(getBrazilPhoneLocalDigits('55999999999')).toBe('55999999999')
  })

  it('normalizes the masked value for API payloads', () => {
    expect(normalizePhoneForApi('+35 (11) 99999-9999')).toBe('+3511999999999')
  })

  it('identifies incomplete phone values', () => {
    expect(isCompletePhone('+55 (11) 9999')).toBe(false)
    expect(isCompletePhone('+55 (11) 99999-9999')).toBe(true)
    expect(isCompleteBrazilPhone('11999999999')).toBe(true)
  })
})
