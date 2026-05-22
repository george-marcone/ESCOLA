<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="auth.canWrite"
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Cadastro</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ editandoId ? 'Editar integrante' : 'Novo integrante' }}
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
            {{ salvando ? 'Salvando...' : editandoId ? 'Atualizar integrante' : 'Cadastrar integrante' }}
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
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Diretoria</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">
        Seu perfil permite consultar a diretoria cadastrada.
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ diretorias.length }} integrante(s)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Diretoria</h2>
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
        <input v-model.trim="busca" class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="search" placeholder="Consultar integrante" />
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>

      <div class="mt-4 max-h-[520px] overflow-auto rounded-lg border border-[#d4dee9]">
        <table class="min-w-[680px] border-collapse text-left lg:min-w-full">
          <thead class="sticky top-0 bg-[#f5f8fb] text-xs uppercase text-[#51627a]">
            <tr>
              <th class="px-4 py-4">Nome</th>
              <th class="px-4 py-4">Usuario vinculado</th>
              <th class="px-4 py-4">Perfil</th>
              <th class="px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in diretoriasPaginadas" :key="item.id" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">{{ item.nome }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ item.usuario?.nome || '-' }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ item.usuario?.descricaoPerfil || '-' }}</td>
              <td class="px-4 py-4">
                <div class="flex justify-center gap-2">
                  <NuxtLink
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
                    :to="`/diretoria/${item.id}`"
                    title="Visualizar integrante"
                    aria-label="Visualizar integrante"
                  >
                    <Eye class="h-5 w-5" aria-hidden="true" />
                  </NuxtLink>
                  <button
                    v-if="auth.canWrite"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar integrante"
                    aria-label="Editar integrante"
                    @click="editar(item)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                    type="button"
                    title="Excluir integrante"
                    aria-label="Excluir integrante"
                    @click="excluir(item)"
                  >
                    <Trash2 class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!carregando && !diretoriasFiltradas.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="4">Nenhum registro encontrado.</td>
            </tr>
            <tr v-if="carregando && !diretorias.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="4">Carregando diretoria...</td>
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
import type { Diretoria, DiretoriaCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const { $api } = useNuxtApp()
const auth = useAuthStore()
const diretorias = ref<Diretoria[]>([])
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
const form = reactive<DiretoriaCreate>({
  nome: '',
  idUsuario: null
})

const diretoriasFiltradas = computed(() => {
  const termo = busca.value.toLowerCase()

  if (!termo) return diretorias.value

  return diretorias.value.filter((item) =>
    [item.nome, item.usuario?.nome, item.usuario?.descricaoPerfil]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const totalPaginas = computed(() => Math.max(1, Math.ceil(diretoriasFiltradas.value.length / porPagina)))
const diretoriasPaginadas = computed(() => {
  const inicio = (pagina.value - 1) * porPagina

  return diretoriasFiltradas.value.slice(inicio, inicio + porPagina)
})
const intervaloTexto = computed(() => {
  if (!diretoriasFiltradas.value.length) return '0 integrante(s)'

  const inicio = (pagina.value - 1) * porPagina + 1
  const fim = Math.min(pagina.value * porPagina, diretoriasFiltradas.value.length)

  return `${inicio}-${fim} de ${diretoriasFiltradas.value.length} integrante(s)`
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
    diretorias.value = await $api<Diretoria[]>('/diretoria')
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

function editar(item: Diretoria) {
  editandoId.value = item.id
  form.nome = item.nome
  form.idUsuario = item.idUsuario ?? null
  idUsuario.value = item.idUsuario ?? ''
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
      await $api<Diretoria>(`/diretoria/${editandoId.value}`, {
        method: 'PUT',
        body
      })
      mensagem.value = 'Integrante atualizado.'
    } else {
      await $api<Diretoria>('/diretoria', {
        method: 'POST',
        body
      })
      mensagem.value = 'Integrante cadastrado.'
    }

    limparForm()
    await carregar()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir(item: Diretoria) {
  if (!confirm(`Excluir ${item.nome} da diretoria?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/diretoria/${item.id}`, { method: 'DELETE' })
    if (editandoId.value === item.id) limparForm()
    await carregar()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}
</script>
