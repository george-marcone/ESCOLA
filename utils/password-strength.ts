export interface PasswordStrength {
  score: number
  label: string
  percent: number
  className: string
  hints: string[]
}

const rules = [
  {
    test: (password: string) => password.length >= 8,
    hint: 'Use ao menos 8 caracteres.'
  },
  {
    test: (password: string) => /[A-Z]/.test(password),
    hint: 'Inclua uma letra maiuscula.'
  },
  {
    test: (password: string) => /[a-z]/.test(password),
    hint: 'Inclua uma letra minuscula.'
  },
  {
    test: (password: string) => /[0-9]/.test(password),
    hint: 'Inclua um numero.'
  },
  {
    test: (password: string) => /[^a-zA-Z0-9]/.test(password),
    hint: 'Inclua um caractere especial.'
  }
]

export function getPasswordStrength(password: string): PasswordStrength {
  const passedRules = rules.filter(rule => rule.test(password))
  const score = passedRules.length
  const hints = rules.filter(rule => !rule.test(password)).map(rule => rule.hint)

  if (!password) {
    return {
      score: 0,
      label: 'Nao informada',
      percent: 0,
      className: 'strength-empty',
      hints
    }
  }

  if (score <= 2) {
    return {
      score,
      label: 'Fraca',
      percent: Math.max(20, score * 20),
      className: 'strength-weak',
      hints
    }
  }

  if (score <= 4) {
    return {
      score,
      label: 'Media',
      percent: score * 20,
      className: 'strength-medium',
      hints
    }
  }

  return {
    score,
    label: 'Forte',
    percent: 100,
    className: 'strength-strong',
    hints
  }
}
