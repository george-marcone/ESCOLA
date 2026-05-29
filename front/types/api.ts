export interface UsuarioSummary {
  idUsuario: number
  nome: string
  email: string
  telefone: string
  dataNascimento?: string | null
  nomeMae?: string | null
  nomePai?: string | null
  endereco?: string | null
  fotoPerfilUrl?: string | null
  idPerfil: number
  descricaoPerfil: string
  tipoUsuario?: string
}

export interface UsuarioForm {
  nome: string
  email: string
  telefone: string
  dataNascimento: string
  nomeMae: string
  nomePai: string
  endereco: string
  idPerfil: number
}

export interface UsuarioCreate {
  nome: string
  email: string
  telefone: string
  dataNascimento?: string | null
  nomeMae?: string | null
  nomePai?: string | null
  endereco?: string | null
  tipoUsuario: string
}

export interface UsuarioUpdate {
  nome: string
  email: string
  telefone: string
  dataNascimento?: string | null
  nomeMae?: string | null
  nomePai?: string | null
  endereco?: string | null
  tipoUsuario?: string
}

export interface UsuarioArquivo {
  idArquivo?: number
  idUsuarioArquivo?: number
  idUsuario?: number | null
  nomeBlob?: string | null
  tipoArquivo?: string | null
  nomeOriginal?: string | null
  url?: string | null
  contentType?: string | null
  tamanhoBytes?: number | null
  criadoEmUtc?: string | null
}

export interface Holerite {
  idHolerite: number
  idUsuario: number
  nomeUsuario: string
  perfilUsuario: string
  competenciaMes: number
  competenciaAno: number
  competencia: string
  nomeOriginal: string
  url: string
  contentType: string
  tamanhoBytes: number
  criadoEmUtc: string
}

export interface HoleriteCompartilhamento {
  token: string
  url: string
  expiraEmUtc: string
}

export interface Notificacao {
  idNotificacao: number
  idUsuario: number
  tipo: string
  titulo: string
  mensagem: string
  link?: string | null
  idCadernetaDigital?: number | null
  idDisciplina?: number | null
  nomeDisciplina?: string | null
  mediaAritmetica?: number | null
  situacao?: string | null
  corSituacao?: string | null
  lida: boolean
  criadaEmUtc: string
  lidaEmUtc?: string | null
}

export interface NotificacaoPerfisPayload {
  idsPerfis?: number[]
  tiposUsuario?: string[]
  todosOsPerfis?: boolean
  titulo: string
  mensagem: string
  tipo?: string
  link?: string | null
}

export interface NotificacaoEnvio {
  total: number
  notificacoes: Notificacao[]
}

export interface Perfil {
  idPerfil: number
  descricaoPerfil: string
}

export interface LoginCredentials {
  email: string
  senha: string
}

export interface AuthResponse {
  token: string
  expiraEm: string
  usuario: UsuarioSummary
  deveAlterarSenhaPadrao: boolean
}

export interface AlterarSenhaPayload {
  senhaAtual: string
  novaSenha: string
  confirmacaoSenha: string
}

export interface EsqueciSenhaPayload {
  email: string
}

export interface EsqueciSenhaResponse {
  mensagem: string
}

export interface DisciplinaCaderneta {
  idDisciplina: number
  nome: string
  idProfessorUsuario: number
  nomeProfessor: string
}

export interface DisciplinaCadernetaPayload {
  nome: string
}

export interface CadernetaDigitalSummary {
  idCadernetaDigital: number
  idAlunoUsuario: number
  nomeAluno: string
  emailAluno: string
  idDisciplina: number
  nomeDisciplina: string
  idProfessorUsuario: number
  nomeProfessor: string
  notas: number[]
  mediaAritmetica: number
  situacao: string
  corSituacao: string
  presencas: number
  faltas: number
}

export interface CadernetaDigitalPayload {
  idAlunoUsuario: number
  idDisciplina: number
  notas: number[]
  presencas: number
  faltas: number
}

export interface DisciplinaEvento {
  idEventoDisciplina: number
  idDisciplina: number
  nomeDisciplina: string
  idProfessorUsuario: number
  nomeProfessor: string
  tipo: string
  titulo: string
  descricao?: string | null
  data: string
  criadoEmUtc: string
  atualizadoEmUtc?: string | null
}

export interface DisciplinaEventoPayload {
  tipo: string
  titulo: string
  descricao?: string | null
  data: string
}

export interface CalendarioEscolarEvento {
  idEventoCalendarioEscolar: number
  data: string
  tipo: string
  titulo: string
  descricao?: string | null
  publicoAlvo: string
  perfisDestinatarios: string[]
  idUsuarioCriador: number
  nomeUsuarioCriador: string
  totalNotificados: number
  criadoEmUtc: string
}

export interface CalendarioEscolarAno {
  ano: number
  mesSelecionado: number
  eventos: CalendarioEscolarEvento[]
}

export interface CalendarioEscolarEventoPayload {
  data: string
  tipo: string
  titulo: string
  descricao?: string | null
  publicoAlvo?: string | null
}
