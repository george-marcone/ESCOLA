import { describe, expect, it } from 'vitest'
import {
  canCreateAlunoUsuarios,
  canEditUsuario,
  canManageAllUsuarios,
  canViewUsuarioInList,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getTipoUsuarioForApi,
  getTipoUsuarioForApiByPerfilId,
  getUsuarioPerfilTipo
} from '~/utils/usuario-permissions'
import type { Perfil, UsuarioSummary } from '~/types/api'

const aluno: UsuarioSummary = {
  idUsuario: 1,
  nome: 'Aluno',
  email: 'aluno@escola.com',
  telefone: '+5511999999999',
  idPerfil: 3,
  descricaoPerfil: 'Aluno'
}

const professor: UsuarioSummary = {
  idUsuario: 2,
  nome: 'Professor',
  email: 'professor@escola.com',
  telefone: '+5511888888888',
  idPerfil: 2,
  descricaoPerfil: 'Professor'
}

const administrador: UsuarioSummary = {
  idUsuario: 3,
  nome: 'Admin',
  email: 'admin@escola.com',
  telefone: '+5511777777777',
  idPerfil: 1,
  descricaoPerfil: 'Administrador'
}

const perfis: Perfil[] = [
  { idPerfil: 1, descricaoPerfil: 'Administrador' },
  { idPerfil: 2, descricaoPerfil: 'Professor' },
  { idPerfil: 3, descricaoPerfil: 'Aluno' }
]

describe('usuario-permissions', () => {
  it('classifies profile descriptions with aliases', () => {
    expect(getUsuarioPerfilTipo('Membro da Diretoria')).toBe('diretoria')
    expect(getUsuarioPerfilTipo('Contribuinte')).toBe('professor')
    expect(getUsuarioPerfilTipo('Aluno')).toBe('aluno')
  })

  it('allows administrators and directors to manage all users', () => {
    expect(canManageAllUsuarios('Administrador')).toBe(true)
    expect(canManageAllUsuarios('Membro da Diretoria')).toBe(true)
    expect(canManageAllUsuarios('Professor')).toBe(false)
  })

  it('allows professors to view students and professors, but edit only themselves', () => {
    expect(canCreateAlunoUsuarios(professor.descricaoPerfil)).toBe(false)
    expect(canEditUsuario(professor, aluno)).toBe(false)
    expect(canEditUsuario(professor, professor)).toBe(true)
    expect(canEditUsuario(professor, administrador)).toBe(false)
    expect(canViewUsuarioInList(professor, aluno)).toBe(true)
    expect(canViewUsuarioInList(professor, professor)).toBe(true)
  })

  it('allows students to see and edit only their own user', () => {
    expect(canViewUsuarioInList(aluno, aluno)).toBe(true)
    expect(canViewUsuarioInList(aluno, professor)).toBe(false)
    expect(canEditUsuario(aluno, professor)).toBe(false)
  })

  it('filters creation profiles according to the current profile', () => {
    expect(filterPerfisForUsuarioCreation(perfis, administrador).map((perfil) => perfil.descricaoPerfil)).toEqual([
      'Administrador',
      'Professor',
      'Aluno'
    ])
    expect(filterPerfisForUsuarioCreation(perfis, professor)).toEqual([])
    expect(filterPerfisForUsuarioCreation(perfis, aluno)).toEqual([])
  })

  it('formats admin and board profiles as the requested option label', () => {
    expect(formatPerfilLabel('Administrador')).toBe('Membro da Diretoria / Administrador')
  })

  it('maps displayed profile descriptions to API user types', () => {
    expect(getTipoUsuarioForApi('Membro da Diretoria')).toBe('Administrador')
    expect(getTipoUsuarioForApi('Professor')).toBe('Professor')
    expect(getTipoUsuarioForApi('Aluno')).toBe('Aluno')
    expect(getTipoUsuarioForApiByPerfilId(perfis, 2)).toBe('Professor')
  })
})
