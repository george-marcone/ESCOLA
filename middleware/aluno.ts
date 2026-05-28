export default defineNuxtRouteMiddleware(() => {
  const auth = useAuthStore()

  if (!auth.isAluno) {
    return navigateTo('/')
  }
})
