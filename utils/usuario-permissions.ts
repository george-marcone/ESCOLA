import type { Perfil, UsuarioSummary } from '~/types/api'

export type UsuarioPerfilTipo = 'aluno' | 'professor' | 'diretoria' | 'administrador' | 'desconhecido'

function normalizePerfil(value = '') {
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .trim()
    .toLowerCase()
}

export function getUsuarioPerfilTipo(descricaoPerfil?: string | null): UsuarioPerfilTipo {
  const perfil = normalizePerfil(descricaoPerfil ?? '')

  if (!perfil) return 'desconhecido'
  if (perfil.includes('administrador')) return 'administrador'
  if (perfil.includes('diretoria') || perfil.includes('diretor')) return 'diretoria'
  if (perfil.includes('professor') || perfil.includes('contribuinte')) return 'professor'
  if (perfil.includes('aluno')) return 'aluno'

  return 'desconhecido'
}

export function isPerfilAluno(descricaoPerfil?: string | null) {
  return getUsuarioPerfilTipo(descricaoPerfil) === 'aluno'
}

export function isPerfilProfessor(descricaoPerfil?: string | null) {
  return getUsuarioPerfilTipo(descricaoPerfil) === 'professor'
}

export function canManageAllUsuarios(descricaoPerfil?: string | null) {
  return ['administrador', 'diretoria'].includes(getUsuarioPerfilTipo(descricaoPerfil))
}

export function canCreateAlunoUsuarios(descricaoPerfil?: string | null) {
  return canManageAllUsuarios(descricaoPerfil)
}

export function canEditUsuario(currentUsuario: UsuarioSummary | null | undefined, targetUsuario: UsuarioSummary) {
  if (!currentUsuario) return false
  if (canManageAllUsuarios(currentUsuario.descricaoPerfil)) return true
  if (currentUsuario.idUsuario === targetUsuario.idUsuario) return true

  return false
}

export function canDeleteUsuario(currentUsuario: UsuarioSummary | null | undefined) {
  return canManageAllUsuarios(currentUsuario?.descricaoPerfil)
}

export function canChangeUsuarioPerfil(currentUsuario: UsuarioSummary | null | undefined) {
  return canManageAllUsuarios(currentUsuario?.descricaoPerfil)
}

export function canViewUsuarioInList(currentUsuario: UsuarioSummary | null | undefined, targetUsuario: UsuarioSummary) {
  if (!currentUsuario) return false
  if (canManageAllUsuarios(currentUsuario.descricaoPerfil)) return true
  if (isPerfilProfessor(currentUsuario.descricaoPerfil)) {
    return currentUsuario.idUsuario === targetUsuario.idUsuario
      || isPerfilAluno(targetUsuario.descricaoPerfil)
      || isPerfilProfessor(targetUsuario.descricaoPerfil)
  }

  return currentUsuario.idUsuario === targetUsuario.idUsuario
}

export function formatPerfilLabel(descricaoPerfil?: string | null) {
  const tipo = getUsuarioPerfilTipo(descricaoPerfil)

  if (tipo === 'administrador' || tipo === 'diretoria') {
    return 'Membro da Diretoria / Administrador'
  }

  return descricaoPerfil || 'Perfil'
}

export function getTipoUsuarioForApi(descricaoPerfil?: string | null) {
  const tipo = getUsuarioPerfilTipo(descricaoPerfil)

  if (tipo === 'administrador' || tipo === 'diretoria') return 'Administrador'
  if (tipo === 'professor') return 'Professor'
  if (tipo === 'aluno') return 'Aluno'

  return ''
}

export function getTipoUsuarioForApiByPerfilId(perfis: Perfil[], idPerfil: number) {
  const perfil = perfis.find((item) => item.idPerfil === idPerfil)
  return getTipoUsuarioForApi(perfil?.descricaoPerfil)
}

export function filterPerfisForUsuarioCreation(perfis: Perfil[], currentUsuario: UsuarioSummary | null | undefined) {
  if (canManageAllUsuarios(currentUsuario?.descricaoPerfil)) {
    return perfis.filter((perfil) => getUsuarioPerfilTipo(perfil.descricaoPerfil) !== 'desconhecido')
  }

  return []
}

export function getDefaultPerfilId(perfis: Perfil[], currentUsuario: UsuarioSummary | null | undefined) {
  const availablePerfis = filterPerfisForUsuarioCreation(perfis, currentUsuario)

  if (availablePerfis.length === 1) {
    return availablePerfis[0].idPerfil
  }

  return 0
}
