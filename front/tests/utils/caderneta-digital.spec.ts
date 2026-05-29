import { describe, expect, it } from 'vitest'
import {
  calcularMediaCaderneta,
  calcularSituacaoCaderneta,
  formatarMediaCaderneta,
  parseCadernetaNotas
} from '~/utils/caderneta-digital'

describe('parseCadernetaNotas', () => {
  it('aceita valores numericos vindos de inputs number', () => {
    expect(parseCadernetaNotas([10, 7, 8, 9.4])).toEqual([10, 7, 8, 9.4])
  })

  it('aceita notas em texto com virgula decimal', () => {
    expect(parseCadernetaNotas(['10', '7', '8', '9,4'])).toEqual([10, 7, 8, 9.4])
  })

  it('ignora notas vazias e mantem nota zero', () => {
    expect(parseCadernetaNotas(['', ' ', 0, null, undefined])).toEqual([0])
  })

  it('rejeita lista sem notas validas', () => {
    expect(parseCadernetaNotas(['', null, undefined])).toBeNull()
  })

  it('rejeita notas invalidas ou fora do intervalo', () => {
    expect(parseCadernetaNotas(['abc'])).toBeNull()
    expect(parseCadernetaNotas([11])).toBeNull()
    expect(parseCadernetaNotas([-1])).toBeNull()
  })
})

describe('calcularMediaCaderneta', () => {
  it('calcula a media aritmetica das notas', () => {
    expect(calcularMediaCaderneta([10, 7, 8, 9.4])).toBeCloseTo(8.6)
  })

  it('retorna null quando nao ha notas', () => {
    expect(calcularMediaCaderneta([])).toBeNull()
  })
})

describe('calcularSituacaoCaderneta', () => {
  it('reprova por faltas quando o aluno atinge 10 faltas', () => {
    expect(calcularSituacaoCaderneta([10, 10], 10)).toEqual({
      label: 'Reprovado por faltas',
      tipo: 'reprovado'
    })
  })

  it('reprova por media menor que 6', () => {
    expect(calcularSituacaoCaderneta([5, 6], 2)).toEqual({
      label: 'Reprovado',
      tipo: 'reprovado'
    })
  })

  it('coloca em recuperacao com media entre 6 e 7', () => {
    expect(calcularSituacaoCaderneta([6, 7], 2)).toEqual({
      label: 'Em recuperacao',
      tipo: 'recuperacao'
    })
  })

  it('aprova com media maior que 7', () => {
    expect(calcularSituacaoCaderneta([7, 8], 2)).toEqual({
      label: 'Aprovado',
      tipo: 'aprovado'
    })
  })
})

describe('formatarMediaCaderneta', () => {
  it('formata a media no padrao brasileiro', () => {
    expect(formatarMediaCaderneta([10, 7, 8, 9.4])).toBe('8,6')
  })

  it('exibe traco quando nao ha media', () => {
    expect(formatarMediaCaderneta([])).toBe('-')
  })
})
