import { jsPDF } from 'jspdf'
import type { UsuarioSummary } from '~/types/api'
import { getUsuarioPerfilTipo } from '~/utils/usuario-permissions'

export type HoleriteRubricaTipo = 'provento' | 'desconto' | 'informativo'

export interface HoleriteRubricaFicticia {
  codigo: string
  descricao: string
  referencia: string
  tipo: HoleriteRubricaTipo
  valor: number
}

export interface HoleriteFicticio {
  funcionario: string
  email: string
  perfil: string
  cargo: string
  matricula: string
  competencia: string
  geradoEmIso: string
  proventos: HoleriteRubricaFicticia[]
  descontos: HoleriteRubricaFicticia[]
  informativos: HoleriteRubricaFicticia[]
  totalProventos: number
  totalDescontos: number
  valorLiquido: number
}

export function criarHoleriteFicticio(
  usuario: Pick<UsuarioSummary, 'idUsuario' | 'nome' | 'email' | 'descricaoPerfil'>,
  competencia = competenciaAtual()
): HoleriteFicticio {
  const perfilTipo = getUsuarioPerfilTipo(usuario.descricaoPerfil)
  const salarioBase = obterSalarioBaseFicticio(perfilTipo)
  const gratificacao = perfilTipo === 'professor' ? 420 : perfilTipo === 'desconhecido' ? 180 : 720
  const auxilioAlimentacao = perfilTipo === 'professor' ? 680 : 820
  const auxilioTransporte = perfilTipo === 'professor' ? 280 : 320
  const baseTributavel = salarioBase + gratificacao
  const inss = arredondar(baseTributavel * obterAliquotaInssFicticia(baseTributavel))
  const irrf = arredondar(Math.max(0, (baseTributavel - inss) * obterAliquotaIrrfFicticia(baseTributavel) - 180))
  const valeTransporte = arredondar(salarioBase * 0.06)
  const planoSaude = perfilTipo === 'professor' ? 165 : 220
  const contribuicaoAssociativa = arredondar(salarioBase * 0.01)

  return montarHolerite({
    usuario,
    competencia,
    proventos: [
      rubrica('100', 'Salario base', '30 dias', 'provento', salarioBase),
      rubrica('115', perfilTipo === 'professor' ? 'Gratificacao pedagogica' : 'Gratificacao administrativa', 'mensal', 'provento', gratificacao),
      rubrica('220', 'Auxilio alimentacao', 'mensal', 'provento', auxilioAlimentacao),
      rubrica('230', 'Auxilio transporte', 'mensal', 'provento', auxilioTransporte)
    ],
    descontos: [
      rubrica('501', 'INSS demonstrativo', `${formatarPercentual(obterAliquotaInssFicticia(baseTributavel))}`, 'desconto', inss),
      rubrica('502', 'IRRF demonstrativo', `${formatarPercentual(obterAliquotaIrrfFicticia(baseTributavel))}`, 'desconto', irrf),
      rubrica('510', 'Vale transporte', '6%', 'desconto', valeTransporte),
      rubrica('520', 'Plano de saude', 'mensal', 'desconto', planoSaude),
      rubrica('530', 'Contribuicao associativa', '1%', 'desconto', contribuicaoAssociativa)
    ],
    informativos: []
  })
}

export function montarHolerite(input: {
  usuario: Pick<UsuarioSummary, 'idUsuario' | 'nome' | 'email' | 'descricaoPerfil'>
  competencia: string
  proventos: HoleriteRubricaFicticia[]
  descontos: HoleriteRubricaFicticia[]
  informativos: HoleriteRubricaFicticia[]
}): HoleriteFicticio {
  const perfilTipo = getUsuarioPerfilTipo(input.usuario.descricaoPerfil)
  const proventos = input.proventos.map(normalizarRubrica)
  const descontos = input.descontos.map(normalizarRubrica)
  const totalProventos = somarRubricas(proventos)
  const totalDescontos = somarRubricas(descontos)
  const baseInss = somarRubricas(proventos.filter((rubrica) => rubrica.codigo !== '220' && rubrica.codigo !== '230'))
  const baseIrrf = Math.max(0, baseInss - (descontos.find((rubrica) => rubrica.codigo === '501')?.valor ?? 0))
  const informativos = input.informativos.length
    ? input.informativos.map(normalizarRubrica)
    : [
        rubrica('801', 'FGTS informativo', '8%', 'informativo', arredondar(totalProventos * 0.08)),
        rubrica('802', 'Base INSS', 'informativo', 'informativo', baseInss),
        rubrica('803', 'Base IRRF', 'informativo', 'informativo', baseIrrf)
      ]

  return {
    funcionario: input.usuario.nome?.trim() || 'Funcionario demonstracao',
    email: input.usuario.email?.trim() || 'funcionario.demo@escola.invalid',
    perfil: input.usuario.descricaoPerfil || 'Funcionario',
    cargo: obterCargoFicticio(perfilTipo),
    matricula: `FUN-${String(input.usuario.idUsuario || 0).padStart(4, '0')}-${hashString(input.usuario.email || input.usuario.nome).toString().slice(0, 4)}`,
    competencia: input.competencia,
    geradoEmIso: new Date().toISOString(),
    proventos,
    descontos,
    informativos,
    totalProventos,
    totalDescontos,
    valorLiquido: arredondar(totalProventos - totalDescontos)
  }
}

export function montarResumoHolerite(holerite: HoleriteFicticio) {
  return [
    'Holerite - Escola Conectada',
    'Documento gerado pelo sistema escolar.',
    '',
    `Funcionario: ${holerite.funcionario}`,
    `Cargo: ${holerite.cargo}`,
    `Competencia: ${formatarCompetencia(holerite.competencia)}`,
    `Total de proventos: ${formatarMoedaHolerite(holerite.totalProventos)}`,
    `Total de descontos: ${formatarMoedaHolerite(holerite.totalDescontos)}`,
    `Valor liquido: ${formatarMoedaHolerite(holerite.valorLiquido)}`
  ].join('\n')
}

export function nomeArquivoHolerite(holerite: Pick<HoleriteFicticio, 'funcionario' | 'competencia'>) {
  return `holerite-${slug(holerite.funcionario)}-${holerite.competencia}.pdf`
}

export function gerarHoleritePdfBlob(holerite: HoleriteFicticio) {
  const doc = new jsPDF({ unit: 'mm', format: 'a4' })
  let y = 18

  doc.setFont('helvetica', 'bold')
  doc.setFontSize(16)
  doc.text('Escola Conectada', 14, y)
  doc.setFontSize(12)
  doc.text('Holerite', 14, y + 8)

  doc.setFont('helvetica', 'normal')
  doc.setFontSize(10)
  doc.text(`Competencia: ${formatarCompetencia(holerite.competencia)}`, 150, y, { align: 'right' })
  doc.text(`Gerado em: ${formatarDataHora(holerite.geradoEmIso)}`, 150, y + 6, { align: 'right' })

  y += 24
  doc.setDrawColor(212, 222, 233)
  doc.line(14, y, 196, y)
  y += 8

  doc.setFont('helvetica', 'bold')
  doc.text('Funcionario', 14, y)
  doc.text('Cargo', 104, y)
  y += 6
  doc.setFont('helvetica', 'normal')
  doc.text(limitText(doc, holerite.funcionario, 80), 14, y)
  doc.text(limitText(doc, holerite.cargo, 80), 104, y)
  y += 6
  doc.setFont('helvetica', 'bold')
  doc.text('Matricula', 14, y)
  doc.text('Email', 104, y)
  y += 6
  doc.setFont('helvetica', 'normal')
  doc.text(holerite.matricula, 14, y)
  doc.text(limitText(doc, holerite.email, 80), 104, y)

  y += 12
  y = escreverRubricas(doc, 'Proventos', holerite.proventos, y)
  y = escreverRubricas(doc, 'Descontos', holerite.descontos, y + 4)
  y = escreverRubricas(doc, 'Informativos', holerite.informativos, y + 4)

  y += 8
  doc.setFillColor(245, 248, 251)
  doc.rect(14, y, 182, 24, 'F')
  doc.setFont('helvetica', 'bold')
  doc.text('Total de proventos', 20, y + 8)
  doc.text('Total de descontos', 78, y + 8)
  doc.text('Valor liquido', 138, y + 8)
  doc.setFontSize(11)
  doc.text(formatarMoedaHolerite(holerite.totalProventos), 20, y + 17)
  doc.text(formatarMoedaHolerite(holerite.totalDescontos), 78, y + 17)
  doc.text(formatarMoedaHolerite(holerite.valorLiquido), 138, y + 17)

  doc.setFontSize(8)
  doc.setFont('helvetica', 'normal')
  doc.text('Documento gerado eletronicamente pelo sistema Escola Conectada.', 14, 286)

  return doc.output('blob')
}

export function formatarMoedaHolerite(value: number) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(Number.isFinite(value) ? value : 0)
}

export function formatarCompetencia(value: string) {
  const [year, month] = value.split('-').map(Number)
  const date = new Date(year || new Date().getFullYear(), (month || 1) - 1, 1)

  return new Intl.DateTimeFormat('pt-BR', {
    month: 'long',
    year: 'numeric'
  }).format(date)
}

export function competenciaAtual() {
  const now = new Date()
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
}

export function separarCompetencia(value: string) {
  const [ano, mes] = value.split('-').map(Number)

  return {
    competenciaAno: ano || new Date().getFullYear(),
    competenciaMes: mes || new Date().getMonth() + 1
  }
}

export function formatarTamanhoArquivo(bytes: number) {
  if (!Number.isFinite(bytes) || bytes <= 0) return '0 KB'
  if (bytes < 1024 * 1024) return `${Math.ceil(bytes / 1024)} KB`

  return `${(bytes / 1024 / 1024).toFixed(1).replace('.', ',')} MB`
}

function escreverRubricas(doc: jsPDF, titulo: string, rubricas: HoleriteRubricaFicticia[], startY: number) {
  let y = startY

  if (y > 244) {
    doc.addPage()
    y = 18
  }

  doc.setFontSize(11)
  doc.setFont('helvetica', 'bold')
  doc.text(titulo, 14, y)
  y += 7

  doc.setFontSize(9)
  doc.setFillColor(245, 248, 251)
  doc.rect(14, y - 5, 182, 7, 'F')
  doc.text('Codigo', 16, y)
  doc.text('Descricao', 36, y)
  doc.text('Ref.', 142, y)
  doc.text('Valor', 194, y, { align: 'right' })
  y += 5

  doc.setFont('helvetica', 'normal')
  for (const rubrica of rubricas) {
    if (y > 274) {
      doc.addPage()
      y = 18
    }

    doc.text(rubrica.codigo, 16, y)
    doc.text(limitText(doc, rubrica.descricao, 98), 36, y)
    doc.text(limitText(doc, rubrica.referencia, 30), 142, y)
    doc.text(formatarMoedaHolerite(rubrica.valor), 194, y, { align: 'right' })
    y += 6
  }

  return y
}

function obterSalarioBaseFicticio(perfilTipo: ReturnType<typeof getUsuarioPerfilTipo>) {
  if (perfilTipo === 'professor') return 5200
  if (perfilTipo === 'administrador' || perfilTipo === 'diretoria') return 7800

  return 3900
}

function obterCargoFicticio(perfilTipo: ReturnType<typeof getUsuarioPerfilTipo>) {
  if (perfilTipo === 'professor') return 'Professor escolar'
  if (perfilTipo === 'administrador') return 'Administrador escolar'
  if (perfilTipo === 'diretoria') return 'Diretoria escolar'

  return 'Funcionario escolar'
}

function obterAliquotaInssFicticia(base: number) {
  if (base <= 3000) return 0.08
  if (base <= 6000) return 0.11

  return 0.14
}

function obterAliquotaIrrfFicticia(base: number) {
  if (base <= 3000) return 0
  if (base <= 5500) return 0.075

  return 0.15
}

function rubrica(
  codigo: string,
  descricao: string,
  referencia: string,
  tipo: HoleriteRubricaTipo,
  valor: number
): HoleriteRubricaFicticia {
  return {
    codigo,
    descricao,
    referencia,
    tipo,
    valor: arredondar(valor)
  }
}

function normalizarRubrica(rubrica: HoleriteRubricaFicticia) {
  return {
    ...rubrica,
    valor: arredondar(Number(rubrica.valor) || 0)
  }
}

function somarRubricas(rubricas: HoleriteRubricaFicticia[]) {
  return arredondar(rubricas.reduce((total, rubrica) => total + rubrica.valor, 0))
}

function arredondar(value: number) {
  return Math.round(value * 100) / 100
}

function formatarPercentual(value: number) {
  return `${Math.round(value * 1000) / 10}%`
}

function formatarDataHora(value: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short'
  }).format(new Date(value))
}

function limitText(doc: jsPDF, value: string, maxWidth: number) {
  return doc.splitTextToSize(value || '-', maxWidth)[0] || '-'
}

function hashString(value = '') {
  let hash = 0

  for (let index = 0; index < value.length; index += 1) {
    hash = Math.imul(31, hash) + value.charCodeAt(index) | 0
  }

  return Math.abs(hash)
}

function slug(value: string) {
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-|-$/g, '') || 'funcionario'
}
