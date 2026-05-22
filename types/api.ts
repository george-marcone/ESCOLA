export interface UsuarioSummary {
  idUsuario: number
  nome: string
  email: string
  telefone: string
  idPerfil: number
  descricaoPerfil: string
}

export interface UsuarioCreate {
  nome: string
  email: string
  telefone: string
  idPerfil: number
}

export type UsuarioUpdate = UsuarioCreate

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

export interface Diretoria {
  id: number
  nome: string
  idUsuario: number | null
  usuario?: UsuarioSummary | null
}

export interface DiretoriaCreate {
  nome: string
  idUsuario?: number | null
}

export type DiretoriaUpdate = DiretoriaCreate

export interface Professor {
  id: number
  nome: string
  idUsuario?: number | null
  usuario?: UsuarioSummary | null
  alunos?: AlunoSummary[]
}

export interface ProfessorCreate {
  nome: string
  idUsuario?: number | null
}

export type ProfessorUpdate = ProfessorCreate

export interface AlunoSummary {
  id: number
  nome: string
  sobrenome: string
  professorId: number
  idUsuario?: number | null
}

export interface Aluno {
  id: number
  nome: string
  sobrenome: string
  dataNasc: string
  professorId: number
  professor?: {
    id: number
    nome: string
    idUsuario?: number | null
  } | null
  idUsuario?: number | null
  usuario?: UsuarioSummary | null
}

export interface AlunoCreate {
  nome: string
  sobrenome: string
  dataNasc: string
  professorId: number
  idUsuario?: number | null
}

export type AlunoUpdate = AlunoCreate
