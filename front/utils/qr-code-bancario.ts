import type { UsuarioSummary } from '~/types/api'

export interface BancoFicticio {
  nome: string
  codigo: string
}

export interface DadosBancariosFicticiosInput {
  banco: string
  valor: number | string
  descricao: string
  geradoEmIso?: string
}

export interface DadosBancariosFicticios {
  aluno: string
  email: string
  banco: string
  codigoBanco: string
  agencia: string
  conta: string
  chaveDemonstracao: string
  documentoFicticio: string
  valor: number
  descricao: string
  referencia: string
  geradoEmIso: string
}

export const BANCOS_FICTICIOS: BancoFicticio[] = [
  { nome: 'Banco Escola Demo', codigo: '900' },
  { nome: 'Banco Conectado Simulado', codigo: '901' },
  { nome: 'Cooperativa Aula Ficticia', codigo: '902' }
]

const AVISO_FICTICIO = 'SEM VALOR BANCARIO'

export function criarDadosBancariosFicticios(
  usuario: Pick<UsuarioSummary, 'idUsuario' | 'nome' | 'email'>,
  input: DadosBancariosFicticiosInput
): DadosBancariosFicticios {
  const aluno = usuario.nome?.trim() || 'Aluno demonstracao'
  const email = usuario.email?.trim() || 'aluno.demo@escola.invalid'
  const banco = BANCOS_FICTICIOS.find((item) => item.nome === input.banco) ?? BANCOS_FICTICIOS[0]
  const seed = `${usuario.idUsuario || 0}|${aluno}|${email}`
  const hash = hashString(seed)
  const contaBase = padNumber((Math.floor(hash / 7) % 900000) + 100000, 6)
  const agencia = padNumber((hash % 9000) + 1000, 4)
  const digitoConta = String((hash + usuario.idUsuario) % 10)
  const referencia = `ALU-${padNumber(usuario.idUsuario || (hash % 9999), 4)}-${padNumber(hash % 999999, 6)}`
  const alunoSlug = slugify(aluno) || 'aluno'

  return {
    aluno,
    email,
    banco: banco.nome,
    codigoBanco: banco.codigo,
    agencia,
    conta: `${contaBase}-${digitoConta}`,
    chaveDemonstracao: `demo-${usuario.idUsuario || hash}-${alunoSlug}@escola-conectada.invalid`,
    documentoFicticio: `DOC-DEMO-${padNumber(hash % 999999999, 9)}`,
    valor: normalizarValorQrCode(input.valor),
    descricao: input.descricao?.trim() || 'Mensalidade escolar ficticia',
    referencia,
    geradoEmIso: input.geradoEmIso ?? new Date().toISOString()
  }
}

export function montarPayloadQrCode(dados: DadosBancariosFicticios) {
  return [
    'ESCOLA CONECTADA - QR CODE BANCARIO FICTICIO',
    AVISO_FICTICIO,
    `Referencia: ${dados.referencia}`,
    `Aluno: ${dados.aluno}`,
    `Email: ${dados.email}`,
    `Banco: ${dados.banco} (${dados.codigoBanco})`,
    `Agencia: ${dados.agencia}`,
    `Conta: ${dados.conta}`,
    `Chave demonstrativa: ${dados.chaveDemonstracao}`,
    `Documento: ${dados.documentoFicticio}`,
    `Valor ficticio: ${formatarMoedaFicticia(dados.valor)}`,
    `Descricao: ${dados.descricao}`,
    `Gerado em: ${dados.geradoEmIso}`
  ].join('\n')
}

export function montarMensagemCompartilhamento(dados: DadosBancariosFicticios) {
  return [
    'QR Code bancario ficticio - Escola Conectada',
    AVISO_FICTICIO,
    '',
    `Aluno: ${dados.aluno}`,
    `Referencia: ${dados.referencia}`,
    `Banco: ${dados.banco}`,
    `Agencia: ${dados.agencia}`,
    `Conta: ${dados.conta}`,
    `Chave demonstrativa: ${dados.chaveDemonstracao}`,
    `Valor ficticio: ${formatarMoedaFicticia(dados.valor)}`,
    `Descricao: ${dados.descricao}`
  ].join('\n')
}

export function normalizarValorQrCode(value: number | string) {
  const numberValue = typeof value === 'string'
    ? Number(value.replace(',', '.'))
    : Number(value)

  if (!Number.isFinite(numberValue) || numberValue < 0) {
    return 0
  }

  return Math.round(numberValue * 100) / 100
}

export function formatarMoedaFicticia(value: number) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

function hashString(value: string) {
  let hash = 0

  for (let index = 0; index < value.length; index += 1) {
    hash = Math.imul(31, hash) + value.charCodeAt(index) | 0
  }

  return Math.abs(hash)
}

function padNumber(value: number, size: number) {
  return String(value).padStart(size, '0')
}

function slugify(value: string) {
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-|-$/g, '')
}
