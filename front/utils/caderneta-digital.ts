export type CadernetaNotaInput = number | string | null | undefined
export type CadernetaSituacaoTipo = 'aprovado' | 'recuperacao' | 'reprovado' | 'sem-nota'

export interface CadernetaSituacao {
  label: string
  tipo: CadernetaSituacaoTipo
}

export function parseCadernetaNotas(values: CadernetaNotaInput[]) {
  const notas = values
    .map((value) => String(value ?? '').trim().replace(',', '.'))
    .filter(Boolean)
    .map(Number)

  if (!notas.length || notas.some((nota) => !Number.isFinite(nota) || nota < 0 || nota > 10)) {
    return null
  }

  return notas
}

export function calcularMediaCaderneta(notas: number[]) {
  if (!notas.length) return null

  const soma = notas.reduce((total, nota) => total + nota, 0)
  return soma / notas.length
}

export function calcularSituacaoCaderneta(notas: number[], faltas: number): CadernetaSituacao {
  if (faltas >= 10) {
    return { label: 'Reprovado por faltas', tipo: 'reprovado' }
  }

  const media = calcularMediaCaderneta(notas)

  if (media === null) {
    return { label: 'Sem nota', tipo: 'sem-nota' }
  }

  if (media < 6) {
    return { label: 'Reprovado', tipo: 'reprovado' }
  }

  if (media <= 7) {
    return { label: 'Em recuperacao', tipo: 'recuperacao' }
  }

  return { label: 'Aprovado', tipo: 'aprovado' }
}

export function formatarMediaCaderneta(notas: number[]) {
  const media = calcularMediaCaderneta(notas)

  return media === null
    ? '-'
    : media.toLocaleString('pt-BR', { maximumFractionDigits: 2 })
}
