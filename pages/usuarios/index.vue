<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="exibeFormulario"
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ editandoId ? 'Edicao' : 'Cadastro' }}</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ tituloFormulario }}
      </h2>

      <div class="grid gap-5">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome</span>
          <input v-model.trim="form.nome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nome.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>E-mail</span>
          <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.email.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Telefone</span>
          <input
            :value="form.telefone"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="tel"
            required
            inputmode="numeric"
            autocomplete="tel"
            :maxlength="BRAZIL_PHONE_MASK_MAX_LENGTH"
            :placeholder="BRAZIL_PHONE_PLACEHOLDER"
            @input="atualizarTelefone"
          />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.telefone.length }}/{{ BRAZIL_PHONE_MASK_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Tipo de usuario</span>
          <select
            v-model.number="form.idPerfil"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            required
            :disabled="!podeAlterarPerfilNoFormulario"
          >
            <option disabled :value="0">Selecione</option>
            <option v-for="perfil in perfisFormulario" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ formatPerfilLabel(perfil.descricaoPerfil) }}
            </option>
          </select>
        </label>

        <p v-if="!editandoId" class="m-0 rounded-md border border-[#d7e8ff] bg-[#eff6ff] p-3 text-sm font-semibold text-[#24446d]">
          A senha inicial sera definida automaticamente como <strong>Senha@252525</strong>.
        </p>

        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
        <p v-if="erro" class="alert alert-error">{{ erro }}</p>

        <div class="grid gap-2">
          <button
            class="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70"
            type="submit"
            :disabled="salvando"
          >
            <Plus class="h-5 w-5" aria-hidden="true" />
            {{ textoBotaoSalvar }}
          </button>
          <button
            v-if="editandoId"
            class="inline-flex min-h-10 items-center justify-center rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
            type="button"
            @click="limparForm"
          >
            Cancelar edicao
          </button>
        </div>
      </div>
    </form>

    <aside
      v-else
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Consulta</p>
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Usuarios</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">
        {{ textoPermissaoConsulta }}
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ usuariosVisiveis.length }} usuario(s)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Usuarios</h2>
        </div>
        <button
          class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Atualizar lista"
          aria-label="Atualizar lista"
          @click="carregar"
        >
          <RefreshCcw class="h-5 w-5" aria-hidden="true" />
        </button>
      </div>

      <div class="relative mt-5">
        <Search class="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-[#62728a]" aria-hidden="true" />
        <input v-model.trim="busca" class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="search" placeholder="Consultar usuario" />
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>

      <div class="mt-4 hidden max-h-[520px] overflow-auto rounded-lg border border-[#d4dee9] md:block">
        <table class="min-w-[720px] border-collapse text-left lg:min-w-full">
          <thead class="sticky top-0 bg-[#f5f8fb] text-xs uppercase text-[#51627a]">
            <tr>
              <th class="px-4 py-4">Nome</th>
              <th class="px-4 py-4">E-mail</th>
              <th class="px-4 py-4">Telefone</th>
              <th class="px-4 py-4">Tipo</th>
              <th class="px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="usuario in usuariosPaginados" :key="usuario.idUsuario" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">{{ usuario.nome }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ usuario.email }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ formatBrazilPhone(usuario.telefone) || '-' }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ formatPerfilLabel(usuario.descricaoPerfil) }}</td>
              <td class="px-4 py-4">
                <div class="flex justify-center gap-2">
                  <NuxtLink
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
                    :to="`/usuarios/${usuario.idUsuario}`"
                    title="Visualizar usuario"
                    aria-label="Visualizar usuario"
                  >
                    <Eye class="h-5 w-5" aria-hidden="true" />
                  </NuxtLink>
                  <button
                    v-if="podeEditarUsuario(usuario)"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar usuario"
                    aria-label="Editar usuario"
                    @click="editar(usuario)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="podeExcluirUsuario(usuario)"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                    type="button"
                    title="Excluir usuario"
                    aria-label="Excluir usuario"
                    @click="excluir(usuario)"
                  >
                    <Trash2 class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!carregando && !usuariosFiltrados.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Nenhum usuario encontrado.</td>
            </tr>
            <tr v-if="carregando && !usuarios.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Carregando usuarios...</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="mt-4 grid gap-3 md:hidden">
        <article
          v-for="usuario in usuariosPaginados"
          :key="usuario.idUsuario"
          class="rounded-lg border border-[#d4dee9] bg-white p-4"
        >
          <div class="flex items-start justify-between gap-3">
            <div class="min-w-0">
              <h3 class="m-0 truncate text-base font-extrabold text-[#071d3b]">{{ usuario.nome }}</h3>
              <p class="m-0 mt-1 break-all text-sm text-[#51627a]">{{ usuario.email }}</p>
            </div>
            <span class="rounded-md bg-[#eaf4f1] px-2 py-1 text-xs font-extrabold text-[#006b61]">
              {{ formatPerfilLabel(usuario.descricaoPerfil) }}
            </span>
          </div>
          <p class="m-0 mt-3 text-sm text-[#243044]">
            <strong>Telefone:</strong> {{ formatBrazilPhone(usuario.telefone) || '-' }}
          </p>
          <div class="mt-4 flex flex-wrap gap-2">
            <NuxtLink
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
              :to="`/usuarios/${usuario.idUsuario}`"
              title="Visualizar usuario"
              aria-label="Visualizar usuario"
            >
              <Eye class="h-5 w-5" aria-hidden="true" />
            </NuxtLink>
            <button
              v-if="podeEditarUsuario(usuario)"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Editar usuario"
              aria-label="Editar usuario"
              @click="editar(usuario)"
            >
              <Pencil class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              v-if="podeExcluirUsuario(usuario)"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
              type="button"
              title="Excluir usuario"
              aria-label="Excluir usuario"
              @click="excluir(usuario)"
            >
              <Trash2 class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>
        </article>

        <p v-if="!carregando && !usuariosFiltrados.length" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Nenhum usuario encontrado.
        </p>
        <p v-if="carregando && !usuarios.length" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Carregando usuarios...
        </p>
      </div>

      <div class="mt-4 flex flex-col gap-3 text-sm font-extrabold text-[#62728a] sm:flex-row sm:items-center sm:justify-between">
        <span>{{ intervaloTexto }}</span>
        <div class="flex items-center gap-2">
          <button
            class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] disabled:opacity-45"
            type="button"
            :disabled="pagina === 1"
            aria-label="Pagina anterior"
            @click="pagina--"
          >
            <ChevronLeft class="h-5 w-5" aria-hidden="true" />
          </button>
          <span>Pagina {{ pagina }} de {{ totalPaginas }}</span>
          <button
            class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] disabled:opacity-45"
            type="button"
            :disabled="pagina === totalPaginas"
            aria-label="Proxima pagina"
            @click="pagina++"
          >
            <ChevronRight class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>
      </div>
    </article>
  </section>
</template>

<script setup lang="ts">
import { ChevronLeft, ChevronRight, Eye, Pencil, Plus, RefreshCcw, Search, Trash2 } from '@lucide/vue'
import type { Perfil, UsuarioCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import {
  BRAZIL_PHONE_MASK_MAX_LENGTH,
  BRAZIL_PHONE_PLACEHOLDER,
  formatBrazilPhone,
  isCompleteBrazilPhone,
  normalizeBrazilPhoneForApi
} from '~/utils/br-phone'
import {
  canCreateAlunoUsuarios,
  canDeleteUsuario,
  canEditUsuario,
  canChangeUsuarioPerfil,
  canViewUsuarioInList,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getDefaultPerfilId
} from '~/utils/usuario-permissions'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: []
})

const { $api } = useNuxtApp()
const auth = useAuthStore()
const usuarios = ref<UsuarioSummary[]>([])
const perfis = ref<Perfil[]>([])
const carregando = ref(false)
const salvando = ref(false)
const erro = ref('')
const erroLista = ref('')
const mensagem = ref('')
const busca = ref('')
const pagina = ref(1)
const porPagina = 10
const editandoId = ref<number | null>(null)
const USER_TEXT_FIELD_MAX_LENGTH = 50
const PHONE_FORMAT_ERROR = 'Informe um telefone valido no formato +55 (xx) xxxxx-xxxx.'
const REQUIRED_FIELDS_ERROR = 'Nome, e-mail e telefone sao obrigatorios.'
const REQUIRED_PROFILE_ERROR = 'Informe o tipo de usuario.'
const form = reactive<UsuarioCreate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})

const usuariosVisiveis = computed(() =>
  usuarios.value.filter((usuario) => canViewUsuarioInList(auth.usuario, usuario))
)
const podeCadastrarUsuarios = computed(() => canCreateAlunoUsuarios(auth.perfil))
const usuarioEmEdicao = computed(() =>
  editandoId.value ? usuarios.value.find((usuario) => usuario.idUsuario === editandoId.value) ?? null : null
)
const exibeFormulario = computed(() => podeCadastrarUsuarios.value || Boolean(editandoId.value))
const perfisFormulario = computed<Perfil[]>(() => {
  if (editandoId.value && !canChangeUsuarioPerfil(auth.usuario)) {
    return usuarioEmEdicao.value
      ? [{ idPerfil: usuarioEmEdicao.value.idPerfil, descricaoPerfil: usuarioEmEdicao.value.descricaoPerfil }]
      : []
  }

  return filterPerfisForUsuarioCreation(perfis.value, auth.usuario)
})
const podeAlterarPerfilNoFormulario = computed(() =>
  canChangeUsuarioPerfil(auth.usuario) && perfisFormulario.value.length > 1
)
const tituloFormulario = computed(() => {
  if (editandoId.value) return 'Editar usuario'
  if (auth.isProfessor) return 'Novo aluno'

  return 'Novo usuario'
})
const textoBotaoSalvar = computed(() => {
  if (salvando.value) return 'Salvando...'
  if (editandoId.value) return 'Atualizar usuario'

  return auth.isProfessor ? 'Cadastrar aluno' : 'Cadastrar usuario'
})
const textoPermissaoConsulta = computed(() => {
  if (auth.isAluno) return 'Seu perfil permite corrigir seus dados cadastrais.'
  if (auth.isProfessor) return 'Seu perfil permite cadastrar alunos e consultar os usuarios permitidos.'

  return 'Seu perfil permite consultar os usuarios cadastrados.'
})
const usuariosFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()

  if (!termo) return usuariosVisiveis.value

  return usuariosVisiveis.value.filter((usuario) =>
    [usuario.nome, usuario.email, usuario.telefone, formatBrazilPhone(usuario.telefone), usuario.descricaoPerfil, formatPerfilLabel(usuario.descricaoPerfil)]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const totalPaginas = computed(() => Math.max(1, Math.ceil(usuariosFiltrados.value.length / porPagina)))
const usuariosPaginados = computed(() => {
  const inicio = (pagina.value - 1) * porPagina

  return usuariosFiltrados.value.slice(inicio, inicio + porPagina)
})
const intervaloTexto = computed(() => {
  if (!usuariosFiltrados.value.length) return '0 usuario(s)'

  const inicio = (pagina.value - 1) * porPagina + 1
  const fim = Math.min(pagina.value * porPagina, usuariosFiltrados.value.length)

  return `${inicio}-${fim} de ${usuariosFiltrados.value.length} usuario(s)`
})

watch(busca, () => {
  pagina.value = 1
})
watch(totalPaginas, (total) => {
  if (pagina.value > total) pagina.value = total
})

onMounted(async () => {
  await carregar()

  if (podeCadastrarUsuarios.value || canChangeUsuarioPerfil(auth.usuario)) {
    await carregarPerfis()
  }
})

async function carregar() {
  carregando.value = true
  erroLista.value = ''

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    if (auth.usuario) {
      try {
        const usuario = await $api<UsuarioSummary>(`/usuarios/${auth.usuario.idUsuario}`)
        usuarios.value = [usuario]
        return
      } catch {
        usuarios.value = [auth.usuario]
      }
    } else {
      erroLista.value = normalizeApiError(err)
    }
  } finally {
    carregando.value = false
  }
}

async function carregarPerfis() {
  try {
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
    if (!editandoId.value) {
      form.idPerfil = getDefaultPerfilId(perfis.value, auth.usuario)
    }
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function editar(usuario: UsuarioSummary) {
  if (!canEditUsuario(auth.usuario, usuario)) {
    return
  }

  editandoId.value = usuario.idUsuario
  form.nome = usuario.nome
  form.email = usuario.email
  form.telefone = formatBrazilPhone(usuario.telefone)
  form.idPerfil = usuario.idPerfil
  mensagem.value = ''
  erro.value = ''
}

function limparForm() {
  editandoId.value = null
  form.nome = ''
  form.email = ''
  form.telefone = ''
  form.idPerfil = getDefaultPerfilId(perfis.value, auth.usuario)
}

function atualizarTelefone(event: Event) {
  const input = event.target as HTMLInputElement
  form.telefone = formatBrazilPhone(input.value)
}

function montarPayload(): UsuarioCreate {
  return {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    idPerfil: form.idPerfil
  }
}

function validarFormulario() {
  if (!form.nome.trim() || !form.email.trim() || !form.telefone.trim()) {
    erro.value = REQUIRED_FIELDS_ERROR
    return false
  }

  if (!isCompleteBrazilPhone(form.telefone)) {
    erro.value = PHONE_FORMAT_ERROR
    return false
  }

  if (!form.idPerfil) {
    erro.value = REQUIRED_PROFILE_ERROR
    return false
  }

  return true
}

function podeEditarUsuario(usuario: UsuarioSummary) {
  return canEditUsuario(auth.usuario, usuario)
}

function podeExcluirUsuario(_usuario: UsuarioSummary) {
  return canDeleteUsuario(auth.usuario)
}

async function salvar() {
  erro.value = ''
  mensagem.value = ''

  if (!validarFormulario()) return

  if (isDuplicateUserEmail(usuarios.value, form.email, editandoId.value)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    const payload = montarPayload()
    let usuarioSalvo: UsuarioSummary

    if (editandoId.value) {
      usuarioSalvo = await $api<UsuarioSummary>(`/usuarios/${editandoId.value}`, {
        method: 'PUT',
        body: payload
      })
      mensagem.value = 'Usuario atualizado.'
    } else {
      usuarioSalvo = await $api<UsuarioSummary>('/usuarios', {
        method: 'POST',
        body: payload
      })
      mensagem.value = 'Usuario cadastrado.'
    }

    if (usuarioSalvo.idUsuario === auth.usuario?.idUsuario) {
      auth.updateUsuario(usuarioSalvo)
    }

    limparForm()
    await carregar()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir(usuario: UsuarioSummary) {
  if (!podeExcluirUsuario(usuario)) {
    return
  }

  if (!confirm(`Excluir o usuario ${usuario.nome}?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/usuarios/${usuario.idUsuario}`, { method: 'DELETE' })
    if (editandoId.value === usuario.idUsuario) limparForm()
    await carregar()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}
</script>
