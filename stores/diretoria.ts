import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { Diretoria, DiretoriaCreate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

export const useDiretoriaStore = defineStore('diretoria', () => {
  const diretorias = ref<Diretoria[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchDiretorias() {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      diretorias.value = await $api<Diretoria[]>('/diretoria')
      return diretorias.value
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createDiretoria(payload: DiretoriaCreate) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const created = await $api<Diretoria>('/diretoria', {
        method: 'POST',
        body: payload
      })

      diretorias.value = [created, ...diretorias.value]
      return created
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    diretorias,
    loading,
    error,
    fetchDiretorias,
    createDiretoria
  }
})
