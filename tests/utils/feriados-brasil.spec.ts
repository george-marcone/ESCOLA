import { describe, expect, it } from 'vitest'
import { calcularPascoa, getFeriadosNacionaisBrasil } from '~/utils/feriados-brasil'
import { formatDateToIso } from '~/utils/date-utils'

describe('getFeriadosNacionaisBrasil', () => {
  it('inclui feriados nacionais fixos de 2026', () => {
    const dates = getFeriadosNacionaisBrasil(2026).map((feriado) => feriado.date)

    expect(dates).toContain('2026-01-01')
    expect(dates).toContain('2026-04-21')
    expect(dates).toContain('2026-11-20')
    expect(dates).toContain('2026-12-25')
  })

  it('calcula a Paixao de Cristo a partir da Pascoa', () => {
    const feriado = getFeriadosNacionaisBrasil(2026).find((item) => item.name === 'Paixao de Cristo')

    expect(formatDateToIso(calcularPascoa(2026))).toBe('2026-04-05')
    expect(feriado?.date).toBe('2026-04-03')
  })
})
