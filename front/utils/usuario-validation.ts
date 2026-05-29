interface UsuarioEmail {
  idUsuario: number
  email: string
}

export const DUPLICATE_USER_EMAIL_MESSAGE = 'Ja existe um usuario cadastrado com este e-mail.'

export function normalizeEmail(email: string) {
  return email.trim().toLowerCase()
}

export function isDuplicateUserEmail(
  usuarios: UsuarioEmail[],
  email: string,
  ignoredUsuarioId?: number | null
) {
  const normalizedEmail = normalizeEmail(email)

  if (!normalizedEmail) {
    return false
  }

  return usuarios.some((usuario) =>
    normalizeEmail(usuario.email) === normalizedEmail &&
    usuario.idUsuario !== ignoredUsuarioId
  )
}
