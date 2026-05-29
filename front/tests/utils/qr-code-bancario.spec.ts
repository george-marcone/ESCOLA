import { describe, expect, it } from 'vitest'
import {
  criarDadosBancariosFicticios,
  montarPayloadQrCode,
  normalizarValorQrCode
} from '~/utils/qr-code-bancario'

const usuario = {
  idUsuario: 12,
  nome: 'Maria Aluna',
  email: 'maria.aluna@example.com'
}

describe('criarDadosBancariosFicticios', () => {
  it('gera dados deterministicos para o mesmo aluno', () => {
    const input = {
      banco: 'Banco Escola Demo',
      valor: 99.9,
      descricao: 'Mensalidade',
      geradoEmIso: '2026-05-28T10:00:00.000Z'
    }

    expect(criarDadosBancariosFicticios(usuario, input)).toEqual(criarDadosBancariosFicticios(usuario, input))
  })

  it('usa dominio invalido na chave demonstrativa', () => {
    const dados = criarDadosBancariosFicticios(usuario, {
      banco: 'Banco Escola Demo',
      valor: 99.9,
      descricao: 'Mensalidade',
      geradoEmIso: '2026-05-28T10:00:00.000Z'
    })

    expect(dados.chaveDemonstracao).toContain('.invalid')
  })
})

describe('montarPayloadQrCode', () => {
  it('marca o conteudo como ficticio e sem valor bancario', () => {
    const dados = criarDadosBancariosFicticios(usuario, {
      banco: 'Banco Escola Demo',
      valor: 99.9,
      descricao: 'Mensalidade',
      geradoEmIso: '2026-05-28T10:00:00.000Z'
    })

    expect(montarPayloadQrCode(dados)).toContain('SEM VALOR BANCARIO')
  })
})

describe('normalizarValorQrCode', () => {
  it('arredonda valores para duas casas', () => {
    expect(normalizarValorQrCode(10.129)).toBe(10.13)
  })

  it('aceita virgula decimal em texto', () => {
    expect(normalizarValorQrCode('10,50')).toBe(10.5)
  })

  it('transforma valores invalidos em zero', () => {
    expect(normalizarValorQrCode('abc')).toBe(0)
    expect(normalizarValorQrCode(-10)).toBe(0)
  })
})
