export const PHONE_COUNTRY_CODE_DIGITS = 2
export const PHONE_LOCAL_DIGITS = 11
export const PHONE_TOTAL_DIGITS = PHONE_COUNTRY_CODE_DIGITS + PHONE_LOCAL_DIGITS
export const PHONE_MASK_MAX_LENGTH = 19
export const PHONE_PLACEHOLDER = '+xx (xx) xxxxx-xxxx'
export const PHONE_DEFAULT_COUNTRY_CODE = '55'

export const BRAZIL_PHONE_COUNTRY_CODE = PHONE_DEFAULT_COUNTRY_CODE
export const BRAZIL_PHONE_LOCAL_DIGITS = PHONE_LOCAL_DIGITS
export const BRAZIL_PHONE_MASK_MAX_LENGTH = PHONE_MASK_MAX_LENGTH
export const BRAZIL_PHONE_PLACEHOLDER = PHONE_PLACEHOLDER

function onlyDigits(value: string) {
  return value.replace(/\D/g, '')
}

function hasExplicitCountryCode(value: string, digits: string) {
  return value.trim().startsWith('+') || digits.length > PHONE_LOCAL_DIGITS
}

function getPhoneDigitsWithOptions(value: string, defaultCountryCode = '') {
  const digits = onlyDigits(value)

  if (!digits) {
    return ''
  }

  if (defaultCountryCode && !hasExplicitCountryCode(value, digits)) {
    return `${defaultCountryCode}${digits.slice(0, PHONE_LOCAL_DIGITS)}`.slice(0, PHONE_TOTAL_DIGITS)
  }

  return digits.slice(0, PHONE_TOTAL_DIGITS)
}

export function getPhoneDigits(value: string) {
  return getPhoneDigitsWithOptions(value)
}

export function getBrazilPhoneLocalDigits(value: string) {
  const digits = getPhoneDigitsWithOptions(value, BRAZIL_PHONE_COUNTRY_CODE)

  return digits.slice(PHONE_COUNTRY_CODE_DIGITS, PHONE_TOTAL_DIGITS)
}

function formatPhoneDigits(digits: string) {
  const countryCode = digits.slice(0, PHONE_COUNTRY_CODE_DIGITS)

  if (!countryCode) {
    return ''
  }

  const ddd = digits.slice(2, 4)
  const firstPart = digits.slice(4, 9)
  const lastPart = digits.slice(9, 13)
  let formatted = `+${countryCode}`

  if (ddd) {
    formatted += ` (${ddd}`
  }

  if (ddd.length === 2) {
    formatted += ')'
  }

  if (firstPart) {
    formatted += ` ${firstPart}`
  }

  if (lastPart) {
    formatted += `-${lastPart}`
  }

  return formatted
}

export function formatPhone(value: string) {
  return formatPhoneDigits(getPhoneDigits(value))
}

export function formatPhoneForDisplay(value: string) {
  return formatPhoneDigits(getPhoneDigitsWithOptions(value, PHONE_DEFAULT_COUNTRY_CODE))
}

export function formatBrazilPhone(value: string) {
  return formatPhoneForDisplay(value)
}

export function isCompletePhone(value: string) {
  return getPhoneDigits(value).length === PHONE_TOTAL_DIGITS
}

export function isCompleteBrazilPhone(value: string) {
  return getPhoneDigitsWithOptions(value, BRAZIL_PHONE_COUNTRY_CODE).length === PHONE_TOTAL_DIGITS
}

export function normalizePhoneForApi(value: string) {
  const digits = getPhoneDigitsWithOptions(value, PHONE_DEFAULT_COUNTRY_CODE)

  return digits ? `+${digits}` : ''
}

export function normalizeBrazilPhoneForApi(value: string) {
  return normalizePhoneForApi(value)
}
