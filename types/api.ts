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

export interface Professor {
  id: number
  nome: string
  idUsuario?: number | null
  alunos?: AlunoSummary[]
}

export interface AlunoSummary {
  id: number
  nome: string
  sobrenome: string
  professorId: number
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
  } | null
}
