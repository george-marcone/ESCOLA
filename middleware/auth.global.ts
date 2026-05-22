export default defineNuxtRouteMiddleware((to) => {
  const auth = useAuthStore()
  auth.loadFromStorage()

  if (to.meta.public) {
    if (auth.isAuthenticated) {
      if (auth.deveAlterarSenhaPadrao) {
        return navigateTo('/alterar-senha')
      }

      return navigateTo('/')
    }

    return
  }

  if (!auth.isAuthenticated) {
    return navigateTo('/login')
  }

  if (auth.deveAlterarSenhaPadrao && to.path !== '/alterar-senha') {
    return navigateTo('/alterar-senha')
  }

  const roles = to.meta.roles as string[] | undefined
  if (roles?.length && auth.usuario && !roles.includes(auth.usuario.descricaoPerfil)) {
    return navigateTo('/')
  }
})
