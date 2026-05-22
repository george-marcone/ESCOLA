import { createApiClient } from '~/utils/api-client'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  const auth = useAuthStore()

  const api = createApiClient({
    baseURL: config.public.apiBase,
    getToken: () => auth.token,
    onUnauthorized: () => {
      auth.logout()

      if (useRoute().path !== '/login') {
        navigateTo('/login')
      }
    }
  })

  return {
    provide: {
      api
    }
  }
})
