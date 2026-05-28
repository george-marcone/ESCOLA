import { getUsuarioPerfilTipo } from '~/utils/usuario-permissions'

export default defineNuxtRouteMiddleware(() => {
  const auth = useAuthStore()
  const perfilTipo = getUsuarioPerfilTipo(auth.usuario?.descricaoPerfil)

  if (perfilTipo !== 'administrador') {
    return navigateTo('/')
  }
})
