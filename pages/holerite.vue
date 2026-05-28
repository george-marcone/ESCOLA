<template>
  <section v-if="!auth.isAluno" class="grid gap-5 xl:grid-cols-[minmax(320px,450px)_minmax(0,1fr)]">
    <aside class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Funcionario</p>
      <h2 class="mb-6 mt-2 text-xl font-normal text-[#071d3b]">Holerites</h2>

      <div class="grid gap-4">
        <label v-if="auth.isAdmin" class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Funcionario</span>
          <select
            v-model.number="funcionarioSelecionadoId"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            @change="aoTrocarFuncionario"
          >
            <option
              v-for="funcionario in funcionarios"
              :key="funcionario.idUsuario"
              :value="funcionario.idUsuario"
            >
              {{ funcionario.nome }} - {{ formatPerfilLabel(funcionario.descricaoPerfil) }}
            </option>
          </select>
        </label>

        <div class="grid gap-3 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
          <div>
            <span class="text-xs font-extrabold uppercase text-[#62728a]">Nome</span>
            <strong class="mt-1 block break-words text-sm text-[#071d3b]">{{ funcionarioAtual?.nome || auth.usuario?.nome }}</strong>
          </div>
          <div>
            <span class="text-xs font-extrabold uppercase text-[#62728a]">Perfil</span>
            <strong class="mt-1 block text-sm text-[#071d3b]">{{ formatPerfilLabel(funcionarioAtual?.descricaoPerfil || auth.perfil) }}</strong>
          </div>
          <div>
            <span class="text-xs font-extrabold uppercase text-[#62728a]">Registros</span>
            <strong class="mt-1 block text-sm text-[#071d3b]">{{ holerites.length }}</strong>
          </div>
        </div>

        <form v-if="auth.isAdmin" class="grid gap-4 rounded-lg border border-[#d4dee9] bg-white p-4" @submit.prevent="lancarHolerite">
          <div class="flex items-start justify-between gap-3">
            <div>
              <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Lancamento</p>
              <h3 class="m-0 mt-1 text-base font-extrabold text-[#071d3b]">Valores do holerite</h3>
            </div>
            <button
              class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Restaurar valores"
              aria-label="Restaurar valores"
              @click="preencherValoresPadrao"
            >
              <RefreshCcw class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>

          <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
            <span>Competencia</span>
            <input
              v-model="competencia"
              class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
              type="month"
              @change="preencherValoresPadrao"
            />
          </label>

          <div class="grid gap-3">
            <div>
              <p class="m-0 text-xs font-extrabold uppercase text-[#0f766e]">Proventos</p>
              <div class="mt-2 grid gap-2">
                <label
                  v-for="rubrica in proventos"
                  :key="rubrica.codigo"
                  class="grid gap-2 text-sm font-extrabold text-[#071d3b]"
                >
                  <span>{{ rubrica.descricao }}</span>
                  <input
                    v-model.number="rubrica.valor"
                    class="min-h-10 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                    type="number"
                    min="0"
                    step="0.01"
                  />
                </label>
              </div>
            </div>

            <div>
              <p class="m-0 text-xs font-extrabold uppercase text-[#b42318]">Descontos</p>
              <div class="mt-2 grid gap-2">
                <label
                  v-for="rubrica in descontos"
                  :key="rubrica.codigo"
                  class="grid gap-2 text-sm font-extrabold text-[#071d3b]"
                >
                  <span>{{ rubrica.descricao }}</span>
                  <input
                    v-model.number="rubrica.valor"
                    class="min-h-10 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                    type="number"
                    min="0"
                    step="0.01"
                  />
                </label>
              </div>
            </div>
          </div>

          <div class="grid gap-2 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-sm">
            <span class="flex justify-between gap-3"><strong>Proventos</strong>{{ formatarMoedaHolerite(holeritePreview.totalProventos) }}</span>
            <span class="flex justify-between gap-3"><strong>Descontos</strong>{{ formatarMoedaHolerite(holeritePreview.totalDescontos) }}</span>
            <span class="flex justify-between gap-3 text-[#0f766e]"><strong>Liquido</strong>{{ formatarMoedaHolerite(holeritePreview.valorLiquido) }}</span>
          </div>

          <div class="grid gap-2 sm:grid-cols-2">
            <button
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
              type="button"
              @click="exportarPreviewPdf"
            >
              <Download class="h-5 w-5" aria-hidden="true" />
              Exportar PDF
            </button>
            <button
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-60"
              type="submit"
              :disabled="enviando"
            >
              <Upload class="h-5 w-5" aria-hidden="true" />
              {{ enviando ? 'Lancando...' : 'Lancar' }}
            </button>
          </div>
        </form>

        <button
          class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
          type="button"
          @click="carregarHolerites"
        >
          <RefreshCcw class="h-5 w-5" aria-hidden="true" />
          Atualizar
        </button>

        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
        <p v-if="erro" class="alert alert-error">{{ erro }}</p>
      </div>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">PDF</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Documentos de pagamento</h2>
        </div>
        <span class="inline-flex w-fit items-center rounded-md border border-[#d4dee9] bg-[#f8fbfd] px-3 py-2 text-sm font-extrabold text-[#51627a]">
          {{ carregando ? 'Carregando...' : `${holerites.length} registro(s)` }}
        </span>
      </div>

      <div v-if="!carregando && !holerites.length" class="mt-5 rounded-lg border border-[#d4dee9] bg-[#f8fbfd] p-5 text-sm font-semibold text-[#51627a]">
        Nenhum holerite encontrado.
      </div>

      <div class="mt-5 grid gap-3">
        <article
          v-for="holerite in holerites"
          :key="holerite.idHolerite"
          class="rounded-lg border border-[#d4dee9] bg-white p-4"
        >
          <div class="grid gap-4 lg:grid-cols-[minmax(0,1fr)_auto] lg:items-start">
            <div class="min-w-0">
              <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ holerite.competencia }}</p>
              <h3 class="m-0 mt-1 break-words text-base font-extrabold text-[#071d3b]">{{ holerite.nomeOriginal }}</h3>
              <div class="mt-3 grid gap-2 text-sm text-[#51627a] md:grid-cols-3">
                <span><strong class="text-[#071d3b]">Funcionario:</strong> {{ holerite.nomeUsuario }}</span>
                <span><strong class="text-[#071d3b]">Perfil:</strong> {{ formatPerfilLabel(holerite.perfilUsuario) }}</span>
                <span><strong class="text-[#071d3b]">Tamanho:</strong> {{ formatarTamanhoArquivo(holerite.tamanhoBytes) }}</span>
              </div>
              <span class="mt-2 block text-xs font-semibold text-[#7a8798]">Lancado em {{ formatarData(holerite.criadoEmUtc) }}</span>
            </div>

            <div class="grid grid-cols-2 gap-2 sm:flex sm:flex-wrap sm:justify-end">
              <button
                class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#147f72] text-white transition hover:bg-[#0f6c61]"
                type="button"
                title="Exportar PDF"
                aria-label="Exportar PDF"
                @click="baixarHolerite(holerite)"
              >
                <Download class="h-5 w-5" aria-hidden="true" />
              </button>
              <button
                class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                type="button"
                title="Abrir PDF"
                aria-label="Abrir PDF"
                @click="abrirHolerite(holerite)"
              >
                <FileText class="h-5 w-5" aria-hidden="true" />
              </button>
              <button
                class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                type="button"
                title="Enviar por email"
                aria-label="Enviar por email"
                @click="enviarEmail(holerite)"
              >
                <Mail class="h-5 w-5" aria-hidden="true" />
              </button>
              <button
                class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                type="button"
                title="Enviar por WhatsApp"
                aria-label="Enviar por WhatsApp"
                @click="enviarWhatsapp(holerite)"
              >
                <MessageCircle class="h-5 w-5" aria-hidden="true" />
              </button>
              <button
                v-if="auth.isAdmin"
                class="inline-flex h-10 w-10 items-center justify-center rounded-md border border-[#f2b8b5] bg-white text-[#b42318] transition hover:bg-[#fff1f1]"
                type="button"
                title="Excluir holerite"
                aria-label="Excluir holerite"
                @click="excluirHolerite(holerite)"
              >
                <Trash2 class="h-5 w-5" aria-hidden="true" />
              </button>
            </div>
          </div>
        </article>
      </div>
    </article>
  </section>
</template>

<script setup lang="ts">
import { Download, FileText, Mail, MessageCircle, RefreshCcw, Trash2, Upload } from '@lucide/vue'
import type { Holerite, UsuarioSummary } from '~/types/api'
import type { HoleriteRubricaFicticia } from '~/utils/holerite-ficticio'
import { normalizeApiError, resolveApiBase } from '~/utils/api-client'
import {
  competenciaAtual,
  criarHoleriteFicticio,
  formatarMoedaHolerite,
  formatarTamanhoArquivo,
  gerarHoleritePdfBlob,
  montarHolerite,
  nomeArquivoHolerite,
  separarCompetencia
} from '~/utils/holerite-ficticio'
import { formatPerfilLabel, getUsuarioPerfilTipo } from '~/utils/usuario-permissions'

definePageMeta({
  roles: [],
  middleware: ['funcionario']
})

const auth = useAuthStore()
const { $api } = useNuxtApp()
const config = useRuntimeConfig()
const usuarios = ref<UsuarioSummary[]>([])
const holerites = ref<Holerite[]>([])
const funcionarioSelecionadoId = ref(auth.usuario?.idUsuario ?? 0)
const competencia = ref(competenciaAtual())
const proventos = ref<HoleriteRubricaFicticia[]>([])
const descontos = ref<HoleriteRubricaFicticia[]>([])
const carregando = ref(false)
const enviando = ref(false)
const mensagem = ref('')
const erro = ref('')

const funcionarios = computed(() => {
  const encontrados = usuarios.value.filter((usuario) => {
    const tipo = getUsuarioPerfilTipo(usuario.descricaoPerfil)
    return tipo === 'professor'
      || (usuario.idUsuario === auth.usuario?.idUsuario && (tipo === 'administrador' || tipo === 'diretoria'))
  })

  if (auth.usuario && !encontrados.some((usuario) => usuario.idUsuario === auth.usuario?.idUsuario)) {
    encontrados.unshift(auth.usuario)
  }

  return encontrados
})

const funcionarioAtual = computed(() =>
  funcionarios.value.find((usuario) => usuario.idUsuario === funcionarioSelecionadoId.value) ?? auth.usuario
)

const usuarioPreview = computed(() => funcionarioAtual.value ?? {
  idUsuario: auth.usuario?.idUsuario ?? 0,
  nome: auth.usuario?.nome ?? 'Funcionario',
  email: auth.usuario?.email ?? '',
  descricaoPerfil: auth.usuario?.descricaoPerfil ?? 'Funcionario'
})

const holeritePreview = computed(() => montarHolerite({
  usuario: usuarioPreview.value,
  competencia: competencia.value,
  proventos: proventos.value,
  descontos: descontos.value,
  informativos: []
}))

onMounted(async () => {
  await auth.validateSession()

  if (auth.isAdmin) {
    await carregarUsuarios()
  }

  funcionarioSelecionadoId.value = auth.usuario?.idUsuario ?? funcionarioSelecionadoId.value
  preencherValoresPadrao()
  await carregarHolerites()
})

async function carregarUsuarios() {
  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

async function aoTrocarFuncionario() {
  preencherValoresPadrao()
  await carregarHolerites()
}

async function carregarHolerites() {
  erro.value = ''
  carregando.value = true

  try {
    holerites.value = auth.isAdmin
      ? await $api<Holerite[]>(`/holerites/usuarios/${funcionarioSelecionadoId.value}`)
      : await $api<Holerite[]>('/holerites/me')
  } catch (err) {
    erro.value = normalizeApiError(err)
    holerites.value = []
  } finally {
    carregando.value = false
  }
}

function preencherValoresPadrao() {
  const base = criarHoleriteFicticio(usuarioPreview.value, competencia.value)
  proventos.value = base.proventos.map(clonarRubrica)
  descontos.value = base.descontos.map(clonarRubrica)
}

async function lancarHolerite() {
  erro.value = ''
  mensagem.value = ''

  if (!auth.isAdmin) {
    erro.value = 'Apenas administrador pode lancar holerites.'
    return
  }

  if (!funcionarioSelecionadoId.value) {
    erro.value = 'Selecione um funcionario.'
    return
  }

  const { competenciaAno, competenciaMes } = separarCompetencia(competencia.value)
  const blob = gerarHoleritePdfBlob(holeritePreview.value)
  const arquivo = new File([blob], nomeArquivoHolerite(holeritePreview.value), { type: 'application/pdf' })
  const formData = new FormData()
  formData.append('competenciaAno', String(competenciaAno))
  formData.append('competenciaMes', String(competenciaMes))
  formData.append('arquivo', arquivo)

  enviando.value = true
  try {
    await $api<Holerite>(`/holerites/usuarios/${funcionarioSelecionadoId.value}`, {
      method: 'POST',
      body: formData
    })
    mensagem.value = 'Holerite lancado.'
    await carregarHolerites()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    enviando.value = false
  }
}

function exportarPreviewPdf() {
  const blob = gerarHoleritePdfBlob(holeritePreview.value)
  baixarBlob(blob, nomeArquivoHolerite(holeritePreview.value))
}

async function baixarHolerite(holerite: Holerite) {
  erro.value = ''

  try {
    const blob = await baixarBlobHolerite(holerite)
    baixarBlob(blob, nomeArquivoApi(holerite))
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

async function abrirHolerite(holerite: Holerite) {
  erro.value = ''

  try {
    const blob = await baixarBlobHolerite(holerite)
    const url = URL.createObjectURL(blob)
    window.open(url, '_blank', 'noopener,noreferrer')
    setTimeout(() => URL.revokeObjectURL(url), 60_000)
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function enviarEmail(holerite: Holerite) {
  const subject = `Holerite ${holerite.competencia} - ${holerite.nomeUsuario}`
  const body = montarMensagemCompartilhamento(holerite)
  window.location.href = `mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`
}

function enviarWhatsapp(holerite: Holerite) {
  const text = montarMensagemCompartilhamento(holerite)
  window.open(`https://wa.me/?text=${encodeURIComponent(text)}`, '_blank', 'noopener,noreferrer')
}

async function excluirHolerite(holerite: Holerite) {
  if (!auth.isAdmin || !confirm(`Excluir holerite ${holerite.competencia} de ${holerite.nomeUsuario}?`)) {
    return
  }

  erro.value = ''
  mensagem.value = ''

  try {
    await $api(`/holerites/usuarios/${holerite.idUsuario}/${holerite.idHolerite}`, {
      method: 'DELETE'
    })
    mensagem.value = 'Holerite excluido.'
    await carregarHolerites()
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

async function baixarBlobHolerite(holerite: Holerite) {
  const endpoint = auth.isAdmin
    ? `/holerites/usuarios/${holerite.idUsuario}/${holerite.idHolerite}/download`
    : `/holerites/me/${holerite.idHolerite}/download`
  const headers = new Headers()
  if (auth.token) headers.set('Authorization', `Bearer ${auth.token}`)

  return await $fetch<Blob>(endpoint, {
    baseURL: resolveApiBase(config.public.apiBase),
    headers,
    responseType: 'blob'
  })
}

function baixarBlob(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  link.remove()
  URL.revokeObjectURL(url)
}

function montarMensagemCompartilhamento(holerite: Holerite) {
  const link = holerite.url || `${window.location.origin}/holerite`
  return [
    'Escola High Tech',
    `Holerite: ${holerite.competencia}`,
    `Funcionario: ${holerite.nomeUsuario}`,
    `Arquivo: ${holerite.nomeOriginal}`,
    `Acesso: ${link}`
  ].join('\n')
}

function nomeArquivoApi(holerite: Holerite) {
  const nome = holerite.nomeOriginal?.toLowerCase().endsWith('.pdf')
    ? holerite.nomeOriginal
    : `holerite-${holerite.competencia.replace('/', '-')}.pdf`

  return nome.replace(/[\\/:*?"<>|]/g, '-')
}

function formatarData(value: string) {
  if (!value) return '-'

  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value))
}

function clonarRubrica(rubrica: HoleriteRubricaFicticia) {
  return { ...rubrica }
}
</script>
