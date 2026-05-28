export const DATE_BR_PLACEHOLDER = 'dd/mm/aaaa'

export function formatIsoDateToBr(value?: string | null) {
  if (!value) return ''

  const date = parseIsoDate(value)
  if (!date) return ''

  return [
    pad(date.getDate()),
    pad(date.getMonth() + 1),
    date.getFullYear()
  ].join('/')
}

export function parseBrDateToIso(value: string) {
  const digits = value.replace(/\D/g, '')
  if (digits.length !== 8) return null

  const day = Number(digits.slice(0, 2))
  const month = Number(digits.slice(2, 4))
  const year = Number(digits.slice(4, 8))
  const date = new Date(year, month - 1, day)

  if (
    date.getFullYear() !== year
    || date.getMonth() !== month - 1
    || date.getDate() !== day
  ) {
    return null
  }

  return formatDateToIso(date)
}

export function parseIsoDate(value?: string | null) {
  if (!value) return null

  const match = /^(\d{4})-(\d{2})-(\d{2})/.exec(value)
  if (!match) return null

  const year = Number(match[1])
  const month = Number(match[2])
  const day = Number(match[3])
  const date = new Date(year, month - 1, day)

  if (
    date.getFullYear() !== year
    || date.getMonth() !== month - 1
    || date.getDate() !== day
  ) {
    return null
  }

  return date
}

export function formatDateToIso(date: Date) {
  return [
    date.getFullYear(),
    pad(date.getMonth() + 1),
    pad(date.getDate())
  ].join('-')
}

export function maskBrDateInput(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 8)
  const parts = [
    digits.slice(0, 2),
    digits.slice(2, 4),
    digits.slice(4, 8)
  ].filter(Boolean)

  return parts.join('/')
}

export function addMonths(date: Date, amount: number) {
  return new Date(date.getFullYear(), date.getMonth() + amount, 1)
}

export function isSameIsoDate(first?: string | null, second?: string | null) {
  return Boolean(first && second && first.slice(0, 10) === second.slice(0, 10))
}

export function formatIsoDateLongBr(value?: string | null) {
  const date = parseIsoDate(value)
  if (!date) return ''

  return new Intl.DateTimeFormat('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  }).format(date)
}

function pad(value: number) {
  return String(value).padStart(2, '0')
}
