import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { AlterarSenhaPayload, AuthResponse, EsqueciSenhaPayload, EsqueciSenhaResponse, LoginCredentials, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const STORAGE_KEY = 'form-escola-auth'

interface StoredSession {
  token: string
  expiraEm: string
  usuario: UsuarioSummary
  deveAlterarSenhaPadrao: boolean
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const expiraEm = ref<string | null>(null)
  const usuario = ref<UsuarioSummary | null>(null)
  const deveAlterarSenhaPadrao = ref(false)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => Boolean(token.value))
  const perfil = computed(() => usuario.value?.descricaoPerfil ?? '')
  const isAdmin = computed(() => perfil.value === 'Administrador')
  const canWrite = computed(() => ['Administrador', 'Contribuinte'].includes(perfil.value))

  async function login(credentials: LoginCredentials) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const response = await $api<AuthResponse>('/auth/login', {
        method: 'POST',
        body: credentials
      })

      setSession(response)
      return response
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchMe() {
    if (!token.value) {
      return null
    }

    const { $api } = useNuxtApp()
    usuario.value = await $api<UsuarioSummary>('/auth/me')
    persist()
    return usuario.value
  }

  async function alterarSenha(payload: AlterarSenhaPayload) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      usuario.value = await $api<UsuarioSummary>('/auth/alterar-senha', {
        method: 'POST',
        body: payload
      })
      deveAlterarSenhaPadrao.value = false
      persist()
      return usuario.value
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function resetarSenhaPadrao(payload: EsqueciSenhaPayload) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      return await $api<EsqueciSenhaResponse>('/auth/esqueci-senha', {
        method: 'POST',
        body: payload
      })
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  function setSession(session: AuthResponse | StoredSession) {
    token.value = session.token
    expiraEm.value = session.expiraEm
    usuario.value = session.usuario
    deveAlterarSenhaPadrao.value = Boolean(session.deveAlterarSenhaPadrao)
    persist()
  }

  function logout() {
    token.value = null
    expiraEm.value = null
    usuario.value = null
    deveAlterarSenhaPadrao.value = false
    error.value = null

    if (canUseStorage()) {
      localStorage.removeItem(STORAGE_KEY)
    }
  }

  function loadFromStorage() {
    if (token.value || !canUseStorage()) {
      return
    }

    const rawSession = localStorage.getItem(STORAGE_KEY)
    if (!rawSession) {
      return
    }

    try {
      const session = JSON.parse(rawSession) as StoredSession
      if (!session.token || !session.usuario) {
        logout()
        return
      }

      setSession(session)
    } catch {
      logout()
    }
  }

  function persist() {
    if (!canUseStorage() || !token.value || !usuario.value || !expiraEm.value) {
      return
    }

    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        token: token.value,
        expiraEm: expiraEm.value,
        usuario: usuario.value,
        deveAlterarSenhaPadrao: deveAlterarSenhaPadrao.value
      })
    )
  }

  return {
    token,
    expiraEm,
    usuario,
    deveAlterarSenhaPadrao,
    loading,
    error,
    isAuthenticated,
    perfil,
    isAdmin,
    canWrite,
    login,
    fetchMe,
    alterarSenha,
    resetarSenhaPadrao,
    setSession,
    logout,
    loadFromStorage
  }
})

function canUseStorage() {
  return typeof localStorage !== 'undefined'
}
