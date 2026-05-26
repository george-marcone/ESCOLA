export type CadernetaNotaInput = number | string | null | undefined

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
