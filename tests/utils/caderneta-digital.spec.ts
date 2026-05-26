import { describe, expect, it } from 'vitest'
import { parseCadernetaNotas } from '~/utils/caderneta-digital'

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
