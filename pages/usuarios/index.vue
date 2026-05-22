<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="auth.isAdmin"
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Cadastro</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ editandoId ? 'Editar usuario' : 'Novo usuario' }}
      </h2>

      <div class="grid gap-5">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome</span>
          <input v-model.trim="form.nome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required maxlength="100" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nome.length }}/100</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>E-mail</span>
          <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" required maxlength="150" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.email.length }}/150</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Telefone</span>
          <input v-model.trim="form.telefone" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="tel" required maxlength="20" placeholder="+55 (11) 99999-9999" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.telefone.length }}/20</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Perfil</span>
          <select v-model.number="form.idPerfil" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" required>
            <option disabled :value="0">Selecione</option>
            <option v-for="perfil in perfis" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ perfil.descricaoPerfil }}
            </option>
          </select>
        </label>

        <p class="m-0 rounded-md border border-[#d7e8ff] bg-[#eff6ff] p-3 text-sm font-semibold text-[#24446d]">
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
            {{ salvando ? 'Salvando...' : editandoId ? 'Atualizar usuario' : 'Cadastrar usuario' }}
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
        Seu perfil permite consultar os usuarios cadastrados.
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ usuarios.length }} usuario(s)</p>
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
              <th class="px-4 py-4">Perfil</th>
              <th class="px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="usuario in usuariosPaginados" :key="usuario.idUsuario" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">{{ usuario.nome }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ usuario.email }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ usuario.telefone || '-' }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ usuario.descricaoPerfil }}</td>
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
                    v-if="auth.isAdmin"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar usuario"
                    aria-label="Editar usuario"
                    @click="editar(usuario)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
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
              {{ usuario.descricaoPerfil }}
            </span>
          </div>
          <p class="m-0 mt-3 text-sm text-[#243044]">
            <strong>Telefone:</strong> {{ usuario.telefone || '-' }}
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
              v-if="auth.isAdmin"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Editar usuario"
              aria-label="Editar usuario"
              @click="editar(usuario)"
            >
              <Pencil class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              v-if="auth.isAdmin"
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
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
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
const form = reactive<UsuarioCreate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})

const usuariosFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()

  if (!termo) return usuarios.value

  return usuarios.value.filter((usuario) =>
    [usuario.nome, usuario.email, usuario.telefone, usuario.descricaoPerfil]
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

  if (auth.isAdmin) {
    await carregarPerfis()
  }
})

async function carregar() {
  carregando.value = true
  erroLista.value = ''

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
}

async function carregarPerfis() {
  try {
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function editar(usuario: UsuarioSummary) {
  editandoId.value = usuario.idUsuario
  form.nome = usuario.nome
  form.email = usuario.email
  form.telefone = usuario.telefone
  form.idPerfil = usuario.idPerfil
  mensagem.value = ''
  erro.value = ''
}

function limparForm() {
  editandoId.value = null
  form.nome = ''
  form.email = ''
  form.telefone = ''
  form.idPerfil = 0
}

async function salvar() {
  erro.value = ''
  mensagem.value = ''

  if (isDuplicateUserEmail(usuarios.value, form.email, editandoId.value)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    if (editandoId.value) {
      await $api<UsuarioSummary>(`/usuarios/${editandoId.value}`, {
        method: 'PUT',
        body: { ...form }
      })
      mensagem.value = 'Usuario atualizado.'
    } else {
      await $api<UsuarioSummary>('/usuarios', {
        method: 'POST',
        body: { ...form }
      })
      mensagem.value = 'Usuario cadastrado.'
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
