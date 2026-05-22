<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="auth.canWrite"
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Cadastro</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ editandoId ? 'Editar professor' : 'Novo professor' }}
      </h2>

      <div class="grid gap-5">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome</span>
          <input v-model.trim="form.nome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required maxlength="100" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nome.length }}/100</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Usuario vinculado</span>
          <select v-model="idUsuario" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10">
            <option value="">Sem vinculo</option>
            <option v-for="usuario in usuarios" :key="usuario.idUsuario" :value="usuario.idUsuario">
              {{ usuario.nome }} - {{ usuario.descricaoPerfil }}
            </option>
          </select>
        </label>

        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
        <p v-if="erro" class="alert alert-error">{{ erro }}</p>

        <div class="grid gap-2">
          <button
            class="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70"
            type="submit"
            :disabled="salvando"
          >
            <Plus class="h-5 w-5" aria-hidden="true" />
            {{ salvando ? 'Salvando...' : editandoId ? 'Atualizar professor' : 'Cadastrar professor' }}
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
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Professores</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">
        Seu perfil permite consultar os professores cadastrados.
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ professores.length }} professor(es)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Professores</h2>
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
        <input v-model.trim="busca" class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="search" placeholder="Consultar professor" />
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>

      <div class="mt-4 max-h-[520px] overflow-auto rounded-lg border border-[#d4dee9]">
        <table class="min-w-[680px] border-collapse text-left lg:min-w-full">
          <thead class="sticky top-0 bg-[#f5f8fb] text-xs uppercase text-[#51627a]">
            <tr>
              <th class="px-4 py-4">Nome</th>
              <th class="px-4 py-4">Alunos</th>
              <th class="px-4 py-4">Usuario vinculado</th>
              <th class="px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="professor in professoresPaginados" :key="professor.id" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">{{ professor.nome }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ professor.alunos?.length ?? 0 }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ professor.usuario?.nome || professor.idUsuario || '-' }}</td>
              <td class="px-4 py-4">
                <div class="flex justify-center gap-2">
                  <NuxtLink
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
                    :to="`/professores/${professor.id}`"
                    title="Visualizar professor"
                    aria-label="Visualizar professor"
                  >
                    <Eye class="h-5 w-5" aria-hidden="true" />
                  </NuxtLink>
                  <button
                    v-if="auth.canWrite"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar professor"
                    aria-label="Editar professor"
                    @click="editar(professor)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                    type="button"
                    title="Excluir professor"
                    aria-label="Excluir professor"
                    @click="excluir(professor)"
                  >
                    <Trash2 class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!carregando && !professoresFiltrados.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="4">Nenhum professor encontrado.</td>
            </tr>
            <tr v-if="carregando && !professores.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="4">Carregando professores...</td>
            </tr>
          </tbody>
        </table>
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
import type { Professor, ProfessorCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const { $api } = useNuxtApp()
const auth = useAuthStore()
const professores = ref<Professor[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const carregando = ref(false)
const salvando = ref(false)
const erro = ref('')
const erroLista = ref('')
const mensagem = ref('')
const busca = ref('')
const pagina = ref(1)
const porPagina = 10
const editandoId = ref<number | null>(null)
const idUsuario = ref<number | ''>('')
const form = reactive<ProfessorCreate>({
  nome: '',
  idUsuario: null
})

const professoresFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()

  if (!termo) return professores.value

  return professores.value.filter((professor) =>
    [professor.nome, professor.usuario?.nome, String(professor.idUsuario ?? '')]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const totalPaginas = computed(() => Math.max(1, Math.ceil(professoresFiltrados.value.length / porPagina)))
const professoresPaginados = computed(() => {
  const inicio = (pagina.value - 1) * porPagina

  return professoresFiltrados.value.slice(inicio, inicio + porPagina)
})
const intervaloTexto = computed(() => {
  if (!professoresFiltrados.value.length) return '0 professor(es)'

  const inicio = (pagina.value - 1) * porPagina + 1
  const fim = Math.min(pagina.value * porPagina, professoresFiltrados.value.length)

  return `${inicio}-${fim} de ${professoresFiltrados.value.length} professor(es)`
})

watch(busca, () => {
  pagina.value = 1
})
watch(totalPaginas, (total) => {
  if (pagina.value > total) pagina.value = total
})

onMounted(async () => {
  await Promise.all([carregar(), carregarUsuarios()])
})

async function carregar() {
  carregando.value = true
  erroLista.value = ''

  try {
    professores.value = await $api<Professor[]>('/professor')
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
}

async function carregarUsuarios() {
  if (!auth.canWrite) return

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function editar(professor: Professor) {
  editandoId.value = professor.id
  form.nome = professor.nome
  form.idUsuario = professor.idUsuario ?? null
  idUsuario.value = professor.idUsuario ?? ''
  mensagem.value = ''
  erro.value = ''
}

function limparForm() {
  editandoId.value = null
  form.nome = ''
  form.idUsuario = null
  idUsuario.value = ''
}

async function salvar() {
  salvando.value = true
  erro.value = ''
  mensagem.value = ''

  const body = {
    nome: form.nome,
    idUsuario: idUsuario.value === '' ? null : Number(idUsuario.value)
  }

  try {
    if (editandoId.value) {
      await $api<Professor>(`/professor/${editandoId.value}`, {
        method: 'PUT',
        body
      })
      mensagem.value = 'Professor atualizado.'
    } else {
      await $api<Professor>('/professor', {
        method: 'POST',
        body
      })
      mensagem.value = 'Professor cadastrado.'
    }

    limparForm()
    await carregar()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir(professor: Professor) {
  if (!confirm(`Excluir o professor ${professor.nome}?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/professor/${professor.id}`, { method: 'DELETE' })
    if (editandoId.value === professor.id) limparForm()
    await carregar()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}
</script>
