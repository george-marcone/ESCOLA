<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="auth.canWrite"
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Cadastro</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ editandoId ? 'Editar aluno' : 'Novo aluno' }}
      </h2>

      <div class="grid gap-5">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome</span>
          <input v-model.trim="form.nome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required maxlength="100" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nome.length }}/100</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Sobrenome</span>
          <input v-model.trim="form.sobrenome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required maxlength="100" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.sobrenome.length }}/100</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Data de nascimento</span>
          <input v-model.trim="form.dataNasc" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required maxlength="10" placeholder="dd/MM/aaaa" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.dataNasc.length }}/10</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Professor</span>
          <select v-model.number="form.professorId" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" required>
            <option disabled :value="0">Selecione</option>
            <option v-for="professor in professores" :key="professor.id" :value="professor.id">
              {{ professor.nome }}
            </option>
          </select>
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
            {{ salvando ? 'Salvando...' : editandoId ? 'Atualizar aluno' : 'Cadastrar aluno' }}
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
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Alunos</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">
        Seu perfil permite consultar os alunos cadastrados.
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ alunos.length }} aluno(s)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Alunos</h2>
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
        <input v-model.trim="busca" class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="search" placeholder="Consultar aluno" />
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>

      <div class="mt-4 max-h-[520px] overflow-auto rounded-lg border border-[#d4dee9]">
        <table class="min-w-[760px] border-collapse text-left lg:min-w-full">
          <thead class="sticky top-0 bg-[#f5f8fb] text-xs uppercase text-[#51627a]">
            <tr>
              <th class="px-4 py-4">Nome</th>
              <th class="px-4 py-4">Nascimento</th>
              <th class="px-4 py-4">Professor</th>
              <th class="px-4 py-4">Usuario vinculado</th>
              <th class="px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="aluno in alunosPaginados" :key="aluno.id" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">{{ aluno.nome }} {{ aluno.sobrenome }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ aluno.dataNasc }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ aluno.professor?.nome || aluno.professorId }}</td>
              <td class="px-4 py-4 text-[#243044]">{{ aluno.usuario?.nome || '-' }}</td>
              <td class="px-4 py-4">
                <div class="flex justify-center gap-2">
                  <NuxtLink
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
                    :to="`/alunos/${aluno.id}`"
                    title="Visualizar aluno"
                    aria-label="Visualizar aluno"
                  >
                    <Eye class="h-5 w-5" aria-hidden="true" />
                  </NuxtLink>
                  <button
                    v-if="auth.canWrite"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar aluno"
                    aria-label="Editar aluno"
                    @click="editar(aluno)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="auth.isAdmin"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                    type="button"
                    title="Excluir aluno"
                    aria-label="Excluir aluno"
                    @click="excluir(aluno)"
                  >
                    <Trash2 class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!carregando && !alunosFiltrados.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Nenhum aluno encontrado.</td>
            </tr>
            <tr v-if="carregando && !alunos.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Carregando alunos...</td>
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
import type { Aluno, AlunoCreate, Professor, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const { $api } = useNuxtApp()
const auth = useAuthStore()
const alunos = ref<Aluno[]>([])
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
const form = reactive<AlunoCreate>({
  nome: '',
  sobrenome: '',
  dataNasc: '',
  professorId: 0,
  idUsuario: null
})

const alunosFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()

  if (!termo) return alunos.value

  return alunos.value.filter((aluno) =>
    [
      aluno.nome,
      aluno.sobrenome,
      aluno.dataNasc,
      aluno.professor?.nome,
      aluno.usuario?.nome,
      String(aluno.professorId)
    ]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const totalPaginas = computed(() => Math.max(1, Math.ceil(alunosFiltrados.value.length / porPagina)))
const alunosPaginados = computed(() => {
  const inicio = (pagina.value - 1) * porPagina

  return alunosFiltrados.value.slice(inicio, inicio + porPagina)
})
const intervaloTexto = computed(() => {
  if (!alunosFiltrados.value.length) return '0 aluno(s)'

  const inicio = (pagina.value - 1) * porPagina + 1
  const fim = Math.min(pagina.value * porPagina, alunosFiltrados.value.length)

  return `${inicio}-${fim} de ${alunosFiltrados.value.length} aluno(s)`
})

watch(busca, () => {
  pagina.value = 1
})
watch(totalPaginas, (total) => {
  if (pagina.value > total) pagina.value = total
})

onMounted(async () => {
  await Promise.all([carregar(), carregarApoio()])
})

async function carregar() {
  carregando.value = true
  erroLista.value = ''

  try {
    alunos.value = await $api<Aluno[]>('/aluno')
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
}

async function carregarApoio() {
  if (!auth.canWrite) return

  try {
    const [professoresResponse, usuariosResponse] = await Promise.all([
      $api<Professor[]>('/professor'),
      $api<UsuarioSummary[]>('/usuarios')
    ])
    professores.value = professoresResponse
    usuarios.value = usuariosResponse
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function editar(aluno: Aluno) {
  editandoId.value = aluno.id
  form.nome = aluno.nome
  form.sobrenome = aluno.sobrenome
  form.dataNasc = aluno.dataNasc
  form.professorId = aluno.professorId
  form.idUsuario = aluno.idUsuario ?? null
  idUsuario.value = aluno.idUsuario ?? ''
  mensagem.value = ''
  erro.value = ''
}

function limparForm() {
  editandoId.value = null
  form.nome = ''
  form.sobrenome = ''
  form.dataNasc = ''
  form.professorId = 0
  form.idUsuario = null
  idUsuario.value = ''
}

async function salvar() {
  salvando.value = true
  erro.value = ''
  mensagem.value = ''

  const body = {
    ...form,
    idUsuario: idUsuario.value === '' ? null : Number(idUsuario.value)
  }

  try {
    if (editandoId.value) {
      await $api<Aluno>(`/aluno/${editandoId.value}`, {
        method: 'PUT',
        body
      })
      mensagem.value = 'Aluno atualizado.'
    } else {
      await $api<Aluno>('/aluno', {
        method: 'POST',
        body
      })
      mensagem.value = 'Aluno cadastrado.'
    }

    limparForm()
    await carregar()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir(aluno: Aluno) {
  if (!confirm(`Excluir o aluno ${aluno.nome} ${aluno.sobrenome}?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/aluno/${aluno.id}`, { method: 'DELETE' })
    if (editandoId.value === aluno.id) limparForm()
    await carregar()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}
</script>
