const defaultApiBase = process.env.NODE_ENV === 'production'
  ? '/api'
  : 'http://localhost:5001/api'

export default defineNuxtConfig({
  ssr: false,
  compatibilityDate: '2026-05-21',
  modules: ['@pinia/nuxt', '@nuxtjs/tailwindcss'],
  css: ['~/assets/css/main.css'],
  tailwindcss: {
    cssPath: '~/assets/css/main.css'
  },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || defaultApiBase
    }
  },
  app: {
    baseURL: process.env.NUXT_APP_BASE_URL || '/',
    head: {
      title: 'Escola Conectada',
      meta: [
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ]
    }
  },
  typescript: {
    strict: true
  },
  experimental: {
    appManifest: false,
    viteEnvironmentApi: true
  }
})
