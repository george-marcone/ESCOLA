import { describe, expect, it } from 'vitest'
import {
  formatIsoDateToBr,
  maskBrDateInput,
  parseBrDateToIso,
  parseIsoDate
} from '~/utils/date-utils'

describe('date-utils', () => {
  it('formata data ISO para o padrao brasileiro', () => {
    expect(formatIsoDateToBr('2026-05-28')).toBe('28/05/2026')
  })

  it('converte data brasileira valida para ISO', () => {
    expect(parseBrDateToIso('28/05/2026')).toBe('2026-05-28')
  })

  it('rejeita datas invalidas', () => {
    expect(parseBrDateToIso('31/02/2026')).toBeNull()
    expect(parseIsoDate('2026-02-31')).toBeNull()
  })

  it('aplica mascara de data durante digitacao', () => {
    expect(maskBrDateInput('28052026')).toBe('28/05/2026')
  })
})
