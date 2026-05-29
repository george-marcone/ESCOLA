<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(300px,380px)_minmax(0,1fr)]">
    <div v-if="podeAdministrar" class="grid gap-5">
      <form
        class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
        @submit.prevent="salvarDisciplina"
      >
        <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Disciplina</p>
        <h2 class="mb-6 mt-2 text-xl font-normal text-[#071d3b]">
          {{ editandoDisciplinaId ? 'Editar disciplina' : 'Cadastro' }}
        </h2>

        <div class="grid gap-4">
          <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
            <span>Nome da disciplina</span>
            <input
              v-model.trim="disciplinaForm.nome"
              class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
              type="text"
              required
              maxlength="100"
            />
            <span class="text-xs font-extrabold text-[#62728a]">{{ disciplinaForm.nome.length }}/100</span>
          </label>

          <p v-if="erroDisciplina" class="alert alert-error">{{ erroDisciplina }}</p>

          <div class="grid gap-2">
            <button
              class="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70"
              type="submit"
              :disabled="salvandoDisciplina"
            >
              <BookOpen class="h-5 w-5" aria-hidden="true" />
              {{ editandoDisciplinaId ? 'Atualizar disciplina' : 'Cadastrar disciplina' }}
            </button>
            <button
              v-if="editandoDisciplinaId"
              class="inline-flex min-h-10 items-center justify-center rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
              type="button"
              @click="limparDisciplinaForm"
            >
              Cancelar edicao
            </button>
          </div>

          <div v-if="disciplinas.length" class="grid gap-2">
            <article
              v-for="disciplina in disciplinas"
              :key="disciplina.idDisciplina"
              class="flex items-center justify-between gap-3 rounded-md border border-[#d4dee9] p-3"
            >
              <strong class="min-w-0 truncate text-sm text-[#071d3b]">{{ disciplina.nome }}</strong>
              <div class="flex shrink-0 gap-2">
                <button
                  class="inline-flex h-9 w-9 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                  type="button"
                  title="Editar disciplina"
                  aria-label="Editar disciplina"
                  @click="editarDisciplina(disciplina)"
                >
                  <Pencil class="h-4 w-4" aria-hidden="true" />
                </button>
                <button
                  class="inline-flex h-9 w-9 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                  type="button"
                  title="Excluir disciplina"
                  aria-label="Excluir disciplina"
                  @click="excluirDisciplina(disciplina)"
                >
                  <Trash2 class="h-4 w-4" aria-hidden="true" />
                </button>
              </div>
            </article>
          </div>
        </div>
      </form>

      <div
        class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
        @keydown.enter.prevent="salvarLancamento"
      >
        <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ editandoLancamentoId ? 'Edicao' : 'Lancamento' }}</p>
        <h2 class="mb-6 mt-2 text-xl font-normal text-[#071d3b]">Notas e frequencia</h2>

        <div class="grid gap-4">
          <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
            <span>Aluno</span>
            <select
              v-model.number="lancamentoForm.idAlunoUsuario"
              class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
              required
            >
              <option :value="0">Selecione</option>
              <option v-for="aluno in alunosDisponiveis" :key="aluno.idUsuario" :value="aluno.idUsuario">
                {{ aluno.nome }} - {{ aluno.email }}
              </option>
            </select>
          </label>

          <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
            <span>Disciplina</span>
            <select
              v-model.number="lancamentoForm.idDisciplina"
              class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
              required
            >
              <option :value="0">Selecione</option>
              <option v-for="disciplina in disciplinas" :key="disciplina.idDisciplina" :value="disciplina.idDisciplina">
                {{ disciplina.nome }}
              </option>
            </select>
          </label>

          <div class="grid gap-3 sm:grid-cols-2">
            <label v-for="(_, index) in lancamentoForm.notas" :key="index" class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Nota {{ index + 1 }}</span>
              <input
                v-model="lancamentoForm.notas[index]"
                class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                type="number"
                min="0"
                max="10"
                step="0.1"
              />
            </label>
          </div>

          <div class="grid gap-3 sm:grid-cols-2">
            <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Presencas</span>
              <input
                v-model.number="lancamentoForm.presencas"
                class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                type="number"
                min="0"
                required
              />
            </label>
            <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Faltas</span>
              <input
                v-model.number="lancamentoForm.faltas"
                class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                type="number"
                min="0"
                required
              />
            </label>
          </div>

          <p v-if="erroLancamento" class="alert alert-error">{{ erroLancamento }}</p>
          <p v-if="mensagemLancamento" class="alert alert-success">{{ mensagemLancamento }}</p>

          <div class="grid gap-2">
            <button
              ref="salvarLancamentoButton"
              class="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70"
              type="button"
              :disabled="salvandoLancamento"
              @click.prevent.stop="salvarLancamento"
            >
              <ClipboardCheck class="h-5 w-5" aria-hidden="true" />
              {{ editandoLancamentoId ? 'Atualizar lancamento' : 'Salvar lancamento' }}
            </button>
            <button
              v-if="editandoLancamentoId"
              class="inline-flex min-h-10 items-center justify-center rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
              type="button"
              @click="limparLancamentoForm"
            >
              Cancelar edicao
            </button>
          </div>
        </div>
      </div>
    </div>

    <aside
      v-else
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Consulta</p>
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Caderneta Digital</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">{{ textoPermissao }}</p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ lancamentosVisiveis.length }} registro(s)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Caderneta Digital</h2>
        </div>
        <button
          class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Atualizar dados"
          aria-label="Atualizar dados"
          @click="carregarDados"
        >
          <RefreshCcw class="h-5 w-5" aria-hidden="true" />
        </button>
      </div>

      <div class="mt-5 grid gap-3 md:grid-cols-[minmax(0,1fr)_240px]">
        <div class="relative">
          <Search class="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-[#62728a]" aria-hidden="true" />
          <input
            v-model.trim="busca"
            class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="search"
            placeholder="Consultar caderneta"
          />
        </div>

        <select
          v-model.number="disciplinaFiltro"
          class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
          aria-label="Filtrar por disciplina"
        >
          <option :value="0">Todas as disciplinas</option>
          <option v-for="disciplina in disciplinasVisiveis" :key="disciplina.idDisciplina" :value="disciplina.idDisciplina">
            {{ disciplina.nome }}
          </option>
        </select>
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>
      <p v-if="!carregando && auth.isAluno && !lancamentosVisiveis.length" class="alert alert-warning mt-4">
        Nenhuma disciplina associada ao seu cadastro.
      </p>

      <div class="mt-4 hidden max-h-[560px] overflow-y-auto rounded-lg border border-[#d4dee9] md:block">
        <div
          class="grid items-center gap-3 bg-[#f5f8fb] px-4 py-3 text-xs font-extrabold uppercase text-[#51627a]"
          :style="{ gridTemplateColumns: gradeCadernetaTemplate }"
        >
          <span>Aluno</span>
          <span>Disciplina</span>
          <span class="text-center">Aprendizado</span>
          <span>Frequencia</span>
          <span v-if="podeAdministrar" class="text-center">Acoes</span>
        </div>

        <article
          v-for="lancamento in lancamentosFiltrados"
          :key="lancamento.idCadernetaDigital"
          class="grid items-center gap-3 border-t border-[#d4dee9] px-4 py-4 text-sm"
          :style="{ gridTemplateColumns: gradeCadernetaTemplate }"
        >
          <div class="min-w-0">
            <strong class="block truncate text-[#243044]">{{ lancamento.nomeAluno }}</strong>
            <span class="mt-1 block break-all text-xs font-semibold text-[#51627a]">{{ lancamento.emailAluno }}</span>
          </div>
          <span class="min-w-0 break-words font-semibold text-[#243044]">{{ lancamento.nomeDisciplina }}</span>
          <div class="flex justify-center">
            <button
              class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Ver aprendizado"
              aria-label="Ver aprendizado"
              @click="abrirAprendizado(lancamento)"
            >
              <GraduationCap class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>
          <span class="text-[#243044]">
            <strong>{{ lancamento.presencas }}</strong> presencas<br />
            <strong>{{ lancamento.faltas }}</strong> faltas
          </span>
          <div v-if="podeAdministrar" class="flex justify-center gap-2">
            <button
              class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Editar lancamento"
              aria-label="Editar lancamento"
              @click="editarLancamento(lancamento)"
            >
              <Pencil class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
              type="button"
              title="Excluir lancamento"
              aria-label="Excluir lancamento"
              @click="excluirLancamento(lancamento)"
            >
              <Trash2 class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>
        </article>

        <p v-if="!carregando && !lancamentosFiltrados.length" class="m-0 border-t border-[#d4dee9] px-4 py-6 text-[#62728a]">
          Nenhum registro encontrado.
        </p>
        <p v-if="carregando" class="m-0 border-t border-[#d4dee9] px-4 py-6 text-[#62728a]">
          Carregando caderneta...
        </p>
      </div>

      <div class="mt-4 grid gap-3 md:hidden">
        <article
          v-for="lancamento in lancamentosFiltrados"
          :key="lancamento.idCadernetaDigital"
          class="rounded-lg border border-[#d4dee9] bg-white p-4"
        >
          <div class="flex items-start justify-between gap-3">
            <div class="min-w-0">
              <h3 class="m-0 truncate text-base font-extrabold text-[#071d3b]">{{ lancamento.nomeAluno }}</h3>
              <p class="m-0 mt-1 break-all text-sm text-[#51627a]">{{ lancamento.emailAluno }}</p>
            </div>
            <span class="rounded-md bg-[#eaf4f1] px-2 py-1 text-xs font-extrabold text-[#006b61]">
              {{ lancamento.nomeDisciplina }}
            </span>
          </div>
          <div class="mt-3 grid gap-2 text-sm text-[#243044]">
            <button
              class="inline-flex min-h-10 items-center justify-center gap-2 rounded-md bg-[#edf3f8] px-3 text-sm font-extrabold text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              @click="abrirAprendizado(lancamento)"
            >
              <GraduationCap class="h-5 w-5" aria-hidden="true" />
              Aprendizado
            </button>
            <span><strong>Presencas:</strong> {{ lancamento.presencas }}</span>
            <span><strong>Faltas:</strong> {{ lancamento.faltas }}</span>
          </div>
          <div v-if="podeAdministrar" class="mt-4 flex flex-wrap gap-2">
            <button
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Editar lancamento"
              aria-label="Editar lancamento"
              @click="editarLancamento(lancamento)"
            >
              <Pencil class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
              type="button"
              title="Excluir lancamento"
              aria-label="Excluir lancamento"
              @click="excluirLancamento(lancamento)"
            >
              <Trash2 class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>
        </article>

        <p v-if="!carregando && !lancamentosFiltrados.length" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Nenhum registro encontrado.
        </p>
        <p v-if="carregando" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Carregando caderneta...
        </p>
      </div>
    </article>

    <div
      v-if="lancamentoAprendizadoPopup"
      class="fixed inset-0 z-40 grid place-items-center bg-[#071d3b]/50 px-4 py-6"
      @click.self="fecharAprendizado"
    >
      <article class="grid w-full max-w-lg gap-4 rounded-lg bg-white p-5 shadow-[0_22px_55px_rgba(14,30,53,0.24)]">
        <div class="flex items-start justify-between gap-4">
          <div class="min-w-0">
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Aprendizado</p>
            <h2 class="m-0 mt-1 break-words text-xl font-normal text-[#071d3b]">{{ lancamentoAprendizadoPopup.nomeAluno }}</h2>
            <p class="m-0 mt-1 break-words text-sm font-semibold text-[#62728a]">{{ lancamentoAprendizadoPopup.nomeDisciplina }}</p>
          </div>
          <button
            class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
            type="button"
            title="Fechar"
            aria-label="Fechar"
            @click="fecharAprendizado"
          >
            <X class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>

        <div class="grid gap-3">
          <div class="rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
            <span class="text-xs font-extrabold uppercase text-[#62728a]">Notas</span>
            <strong class="mt-2 block break-words text-[#071d3b]">{{ formatNotas(lancamentoAprendizadoPopup.notas) }}</strong>
          </div>
          <div class="grid gap-3 sm:grid-cols-2">
            <div class="rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
              <span class="text-xs font-extrabold uppercase text-[#62728a]">Media</span>
              <strong class="mt-2 block text-[#071d3b]">{{ formatarMediaLancamento(lancamentoAprendizadoPopup.mediaAritmetica) }}</strong>
            </div>
            <div class="rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
              <span class="text-xs font-extrabold uppercase text-[#62728a]">Situacao</span>
              <strong class="mt-2 block" :class="situacaoCadernetaClass(lancamentoAprendizadoPopup)">
                {{ lancamentoAprendizadoPopup.situacao }}
              </strong>
            </div>
          </div>
        </div>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
import { BookOpen, ClipboardCheck, GraduationCap, Pencil, RefreshCcw, Search, Trash2, X } from '@lucide/vue'
import type {
  CadernetaDigitalPayload,
  CadernetaDigitalSummary,
  DisciplinaCaderneta,
  DisciplinaCadernetaPayload,
  UsuarioSummary
} from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import {
  parseCadernetaNotas,
  type CadernetaNotaInput
} from '~/utils/caderneta-digital'
import { isPerfilAluno } from '~/utils/usuario-permissions'

definePageMeta({
  roles: []
})

const EMPTY_NOTAS: CadernetaNotaInput[] = ['', '', '', '']

const auth = useAuthStore()
const { $api } = useNuxtApp()
const usuarios = ref<UsuarioSummary[]>([])
const disciplinas = ref<DisciplinaCaderneta[]>([])
const lancamentos = ref<CadernetaDigitalSummary[]>([])
const carregando = ref(false)
const salvandoDisciplina = ref(false)
const salvandoLancamento = ref(false)
const erroLista = ref('')
const erroDisciplina = ref('')
const erroLancamento = ref('')
const mensagemLancamento = ref('')
const busca = ref('')
const disciplinaFiltro = ref(0)
const editandoDisciplinaId = ref<number | null>(null)
const editandoLancamentoId = ref<number | null>(null)
const salvarLancamentoButton = ref<HTMLButtonElement | null>(null)
const lancamentoAprendizadoPopup = ref<CadernetaDigitalSummary | null>(null)
const disciplinaForm = reactive({
  nome: ''
})
const lancamentoForm = reactive({
  idAlunoUsuario: 0,
  idDisciplina: 0,
  notas: [...EMPTY_NOTAS],
  presencas: 0,
  faltas: 0
})

const podeAdministrar = computed(() => auth.isProfessor)
const gradeCadernetaTemplate = computed(() =>
  podeAdministrar.value
    ? 'minmax(140px, 1.5fr) minmax(100px, 0.9fr) 110px minmax(90px, 0.75fr) 88px'
    : 'minmax(140px, 1.6fr) minmax(100px, 0.95fr) 110px minmax(90px, 0.75fr)'
)
const alunosDisponiveis = computed(() =>
  usuarios.value.filter((usuario) => isPerfilAluno(usuario.descricaoPerfil))
)
const lancamentosVisiveis = computed(() => {
  if (auth.isAluno) {
    return lancamentos.value.filter((lancamento) => lancamento.idAlunoUsuario === auth.usuario?.idUsuario)
  }

  return lancamentos.value
})
const disciplinasVisiveis = computed(() => {
  const idsVisiveis = new Set(lancamentosVisiveis.value.map((lancamento) => lancamento.idDisciplina))

  return auth.isAluno
    ? disciplinas.value.filter((disciplina) => idsVisiveis.has(disciplina.idDisciplina))
    : disciplinas.value
})
const lancamentosFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()
  const lancamentosDaDisciplina = disciplinaFiltro.value
    ? lancamentosVisiveis.value.filter((lancamento) => lancamento.idDisciplina === disciplinaFiltro.value)
    : lancamentosVisiveis.value

  if (!termo) return lancamentosDaDisciplina

  return lancamentosDaDisciplina.filter((lancamento) =>
    [
      lancamento.nomeAluno,
      lancamento.emailAluno,
      lancamento.nomeDisciplina,
      formatNotas(lancamento.notas),
      formatarMediaLancamento(lancamento.mediaAritmetica),
      lancamento.situacao
    ]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const textoPermissao = computed(() => {
  if (auth.isAluno) return 'Seu perfil permite visualizar apenas disciplinas associadas ao seu cadastro.'
  if (auth.isAdmin) return 'Seu perfil permite visualizar todos os registros da caderneta.'

  return 'Seu perfil permite visualizar a caderneta.'
})

watch(disciplinasVisiveis, (disponiveis) => {
  if (disciplinaFiltro.value && !disponiveis.some((disciplina) => disciplina.idDisciplina === disciplinaFiltro.value)) {
    disciplinaFiltro.value = 0
  }
})

onMounted(async () => {
  instalarEventoNativoLancamento()
  await carregarDados()
})

onBeforeUnmount(() => {
  removerEventoNativoLancamento()
})

async function carregarDados() {
  carregando.value = true
  erroLista.value = ''

  try {
    await Promise.all([
      carregarDisciplinas(),
      carregarLancamentos(),
      podeAdministrar.value ? carregarUsuarios() : Promise.resolve()
    ])
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
}

async function carregarDisciplinas() {
  disciplinas.value = await $api<DisciplinaCaderneta[]>('/caderneta-digital/disciplinas')
}

async function carregarLancamentos() {
  lancamentos.value = await $api<CadernetaDigitalSummary[]>('/caderneta-digital')
}

async function carregarUsuarios() {
  usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
}

async function salvarDisciplina() {
  erroDisciplina.value = ''
  const nome = disciplinaForm.nome.trim()

  if (!nome) {
    erroDisciplina.value = 'Informe o nome da disciplina.'
    return
  }

  salvandoDisciplina.value = true
  const payload: DisciplinaCadernetaPayload = { nome }

  try {
    if (editandoDisciplinaId.value) {
      await $api<DisciplinaCaderneta>(`/caderneta-digital/disciplinas/${editandoDisciplinaId.value}`, {
        method: 'PUT',
        body: payload
      })
    } else {
      await $api<DisciplinaCaderneta>('/caderneta-digital/disciplinas', {
        method: 'POST',
        body: payload
      })
    }

    limparDisciplinaForm()
    await Promise.all([carregarDisciplinas(), carregarLancamentos()])
  } catch (err) {
    erroDisciplina.value = normalizeApiError(err)
  } finally {
    salvandoDisciplina.value = false
  }
}

function editarDisciplina(disciplina: DisciplinaCaderneta) {
  editandoDisciplinaId.value = disciplina.idDisciplina
  disciplinaForm.nome = disciplina.nome
  erroDisciplina.value = ''
}

async function excluirDisciplina(disciplina: DisciplinaCaderneta) {
  if (!confirm(`Excluir a disciplina ${disciplina.nome}?`)) {
    return
  }

  erroDisciplina.value = ''

  try {
    await $api(`/caderneta-digital/disciplinas/${disciplina.idDisciplina}`, { method: 'DELETE' })
    if (editandoDisciplinaId.value === disciplina.idDisciplina) limparDisciplinaForm()
    if (lancamentoForm.idDisciplina === disciplina.idDisciplina) lancamentoForm.idDisciplina = 0
    await Promise.all([carregarDisciplinas(), carregarLancamentos()])
  } catch (err) {
    erroDisciplina.value = normalizeApiError(err)
  }
}

function limparDisciplinaForm() {
  editandoDisciplinaId.value = null
  disciplinaForm.nome = ''
}

async function salvarLancamento() {
  if (salvandoLancamento.value) return

  erroLancamento.value = ''
  mensagemLancamento.value = ''
  const notas = parseCadernetaNotas(lancamentoForm.notas)

  if (!lancamentoForm.idAlunoUsuario || !lancamentoForm.idDisciplina) {
    erroLancamento.value = 'Informe aluno e disciplina.'
    return
  }

  if (!notas) {
    erroLancamento.value = 'Informe ao menos uma nota entre 0 e 10.'
    return
  }

  if (!Number.isInteger(lancamentoForm.presencas) || !Number.isInteger(lancamentoForm.faltas) || lancamentoForm.presencas < 0 || lancamentoForm.faltas < 0) {
    erroLancamento.value = 'Presencas e faltas devem ser numeros inteiros positivos.'
    return
  }

  salvandoLancamento.value = true
  const payload: CadernetaDigitalPayload = {
    idAlunoUsuario: lancamentoForm.idAlunoUsuario,
    idDisciplina: lancamentoForm.idDisciplina,
    notas,
    presencas: lancamentoForm.presencas,
    faltas: lancamentoForm.faltas
  }

  try {
    if (editandoLancamentoId.value) {
      await $api<CadernetaDigitalSummary>(`/caderneta-digital/${editandoLancamentoId.value}`, {
        method: 'PUT',
        body: payload
      })
    } else {
      await $api<CadernetaDigitalSummary>('/caderneta-digital', {
        method: 'POST',
        body: payload
      })
    }

    limparLancamentoForm()
    mensagemLancamento.value = 'Lancamento salvo com sucesso.'
    await carregarLancamentos()
  } catch (err) {
    erroLancamento.value = normalizeApiError(err)
  } finally {
    salvandoLancamento.value = false
  }
}

function editarLancamento(lancamento: CadernetaDigitalSummary) {
  if (!podeAdministrar.value) return

  editandoLancamentoId.value = lancamento.idCadernetaDigital
  lancamentoForm.idAlunoUsuario = lancamento.idAlunoUsuario
  lancamentoForm.idDisciplina = lancamento.idDisciplina
  lancamentoForm.notas = [...lancamento.notas.map((nota) => String(nota)), ...EMPTY_NOTAS].slice(0, EMPTY_NOTAS.length)
  lancamentoForm.presencas = lancamento.presencas
  lancamentoForm.faltas = lancamento.faltas
  erroLancamento.value = ''
}

async function excluirLancamento(lancamento: CadernetaDigitalSummary) {
  if (!podeAdministrar.value || !confirm(`Excluir o lancamento de ${lancamento.nomeAluno} em ${lancamento.nomeDisciplina}?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/caderneta-digital/${lancamento.idCadernetaDigital}`, { method: 'DELETE' })
    if (editandoLancamentoId.value === lancamento.idCadernetaDigital) limparLancamentoForm()
    await carregarLancamentos()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}

function limparLancamentoForm() {
  editandoLancamentoId.value = null
  lancamentoForm.idAlunoUsuario = 0
  lancamentoForm.idDisciplina = 0
  lancamentoForm.notas = [...EMPTY_NOTAS]
  lancamentoForm.presencas = 0
  lancamentoForm.faltas = 0
  erroLancamento.value = ''
}

function salvarLancamentoPorEventoNativo(event: Event) {
  event.preventDefault()
  event.stopPropagation()
  void salvarLancamento()
}

function instalarEventoNativoLancamento() {
  salvarLancamentoButton.value?.addEventListener('click', salvarLancamentoPorEventoNativo)
}

function removerEventoNativoLancamento() {
  salvarLancamentoButton.value?.removeEventListener('click', salvarLancamentoPorEventoNativo)
}

function abrirAprendizado(lancamento: CadernetaDigitalSummary) {
  lancamentoAprendizadoPopup.value = lancamento
}

function fecharAprendizado() {
  lancamentoAprendizadoPopup.value = null
}

function formatNotas(notas: number[]) {
  return notas.length
    ? notas.map((nota) => nota.toLocaleString('pt-BR', { maximumFractionDigits: 1 })).join(' / ')
    : '-'
}

function formatarMediaLancamento(media: number) {
  return Number.isFinite(media)
    ? media.toLocaleString('pt-BR', { maximumFractionDigits: 2 })
    : '-'
}

function situacaoCadernetaClass(lancamento: CadernetaDigitalSummary) {
  const classes: Record<string, string> = {
    azul: 'text-[#1d4ed8]',
    preto: 'text-[#111827]',
    vermelho: 'text-[#dc2626]'
  }

  return classes[lancamento.corSituacao] ?? 'text-[#62728a]'
}
</script>
