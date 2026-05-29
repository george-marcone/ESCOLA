export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthStore()
  auth.loadFromStorage()

  if (auth.isAuthenticated) {
    const usuario = await auth.validateSession()

    if (!usuario) {
      return to.meta.public ? undefined : navigateTo('/login')
    }
  }

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
