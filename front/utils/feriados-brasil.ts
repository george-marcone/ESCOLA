import { formatDateToIso } from '~/utils/date-utils'

export interface FeriadoBrasil {
  date: string
  name: string
  type: 'national'
}

export function getFeriadosNacionaisBrasil(year: number): FeriadoBrasil[] {
  const easter = calcularPascoa(year)
  const goodFriday = new Date(easter)
  goodFriday.setDate(easter.getDate() - 2)

  const feriados: FeriadoBrasil[] = [
    { date: `${year}-01-01`, name: 'Confraternizacao Universal', type: 'national' },
    { date: formatDateToIso(goodFriday), name: 'Paixao de Cristo', type: 'national' },
    { date: `${year}-04-21`, name: 'Tiradentes', type: 'national' },
    { date: `${year}-05-01`, name: 'Dia do Trabalhador', type: 'national' },
    { date: `${year}-09-07`, name: 'Independencia do Brasil', type: 'national' },
    { date: `${year}-10-12`, name: 'Nossa Senhora Aparecida', type: 'national' },
    { date: `${year}-11-02`, name: 'Finados', type: 'national' },
    { date: `${year}-11-15`, name: 'Proclamacao da Republica', type: 'national' },
    { date: `${year}-11-20`, name: 'Dia Nacional de Zumbi e da Consciencia Negra', type: 'national' },
    { date: `${year}-12-25`, name: 'Natal', type: 'national' }
  ]

  return feriados.sort((a, b) => a.date.localeCompare(b.date))
}

export function getFeriadosPorData(year: number) {
  return Object.fromEntries(
    getFeriadosNacionaisBrasil(year).map((feriado) => [feriado.date, feriado])
  ) as Record<string, FeriadoBrasil>
}

export function calcularPascoa(year: number) {
  const a = year % 19
  const b = Math.floor(year / 100)
  const c = year % 100
  const d = Math.floor(b / 4)
  const e = b % 4
  const f = Math.floor((b + 8) / 25)
  const g = Math.floor((b - f + 1) / 3)
  const h = (19 * a + b - d - g + 15) % 30
  const i = Math.floor(c / 4)
  const k = c % 4
  const l = (32 + 2 * e + 2 * i - h - k) % 7
  const m = Math.floor((a + 11 * h + 22 * l) / 451)
  const month = Math.floor((h + l - 7 * m + 114) / 31)
  const day = ((h + l - 7 * m + 114) % 31) + 1

  return new Date(year, month - 1, day)
}
