import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { Perfil, UsuarioCreate, UsuarioSummary, UsuarioUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import { normalizeBrazilPhoneForApi } from '~/utils/br-phone'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

export const useUsuariosStore = defineStore('usuarios', () => {
  const usuarios = ref<UsuarioSummary[]>([])
  const perfis = ref<Perfil[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchUsuarios() {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
      return usuarios.value
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchPerfis() {
    const { $api } = useNuxtApp()
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
    return perfis.value
  }

  async function createUsuario(payload: UsuarioCreate) {
    loading.value = true
    error.value = null

    try {
      validateUniqueEmail(payload.email)

      const { $api } = useNuxtApp()
      const created = await $api<UsuarioSummary>('/usuarios', {
        method: 'POST',
        body: normalizeUsuarioPayload(payload)
      })

      usuarios.value = [created, ...usuarios.value]
      return created
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateUsuario(idUsuario: number, payload: UsuarioUpdate) {
    loading.value = true
    error.value = null

    try {
      validateUniqueEmail(payload.email, idUsuario)

      const { $api } = useNuxtApp()
      const updated = await $api<UsuarioSummary>(`/usuarios/${idUsuario}`, {
        method: 'PUT',
        body: normalizeUsuarioPayload(payload)
      })

      usuarios.value = usuarios.value.map((usuario) =>
        usuario.idUsuario === idUsuario ? updated : usuario
      )
      return updated
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteUsuario(idUsuario: number) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      await $api<void>(`/usuarios/${idUsuario}`, {
        method: 'DELETE'
      })

      usuarios.value = usuarios.value.filter((usuario) => usuario.idUsuario !== idUsuario)
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  function validateUniqueEmail(email: string, ignoredUsuarioId?: number | null) {
    if (!isDuplicateUserEmail(usuarios.value, email, ignoredUsuarioId)) {
      return
    }

    error.value = DUPLICATE_USER_EMAIL_MESSAGE
    throw new Error(DUPLICATE_USER_EMAIL_MESSAGE)
  }

  function normalizeUsuarioPayload(payload: UsuarioCreate | UsuarioUpdate) {
    return {
      ...payload,
      telefone: normalizeBrazilPhoneForApi(payload.telefone)
    }
  }

  return {
    usuarios,
    perfis,
    loading,
    error,
    fetchUsuarios,
    fetchPerfis,
    createUsuario,
    updateUsuario,
    deleteUsuario
  }
})
