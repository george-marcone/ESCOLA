import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { Perfil, UsuarioCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

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
      const { $api } = useNuxtApp()
      const created = await $api<UsuarioSummary>('/usuarios', {
        method: 'POST',
        body: payload
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

  return {
    usuarios,
    perfis,
    loading,
    error,
    fetchUsuarios,
    fetchPerfis,
    createUsuario
  }
})
