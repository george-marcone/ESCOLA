<template>
  <div class="min-h-screen bg-[#f3f7fb] text-[#071d3b]">
    <header class="mx-auto flex max-w-7xl flex-col gap-5 px-4 pb-5 pt-6 sm:px-8 lg:flex-row lg:items-start lg:justify-between">
      <div class="min-w-0">
        <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">
          GM Tech Solutions
        </p>
        <NuxtLink class="mt-2 block text-3xl font-normal leading-tight text-[#071d3b] no-underline sm:text-4xl" to="/">
          {{ tituloPagina }}
        </NuxtLink>
        <NuxtLink
          v-if="route.path !== '/'"
          class="mt-3 inline-flex min-h-9 items-center rounded-md border border-[#d4dee9] bg-white px-3 text-sm font-extrabold text-[#51627a] no-underline shadow-sm transition hover:bg-[#edf3f8]"
          to="/"
        >
          Painel
        </NuxtLink>
      </div>

      <div class="flex shrink-0 flex-col items-start gap-3 lg:items-end">
        <p class="m-0 text-sm font-extrabold text-[#071d3b]">
          {{ nomeUsuario }}
        </p>
        <div class="grid w-full grid-cols-1 gap-2 sm:w-auto sm:grid-cols-3">
          <div class="relative">
            <button
              class="relative inline-flex min-h-11 w-full items-center gap-2 rounded-md bg-[#eaf4f1] px-4 text-sm font-extrabold text-[#006b61] transition hover:bg-[#dcefeb]"
              type="button"
              title="Notificacoes"
              aria-label="Notificacoes"
              @click="alternarNotificacoes"
            >
              <Bell class="h-5 w-5" aria-hidden="true" />
              Notificacoes
              <span
                v-if="notificacoesNaoLidas > 0"
                class="ml-auto inline-flex min-h-5 min-w-5 items-center justify-center rounded-full bg-[#d64200] px-1 text-xs font-extrabold text-white"
              >
                {{ notificacoesNaoLidas }}
              </span>
            </button>

            <div
              v-if="notificacoesAbertas"
              class="absolute right-0 z-30 mt-2 grid w-[min(92vw,380px)] gap-3 rounded-lg border border-[#d4dee9] bg-white p-3 text-left shadow-[0_22px_55px_rgba(14,30,53,0.18)]"
            >
              <div class="flex items-center justify-between gap-3">
                <strong class="text-sm text-[#071d3b]">Notificacoes</strong>
                <button
                  v-if="notificacoesNaoLidas > 0"
                  class="inline-flex h-9 w-9 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                  type="button"
                  title="Marcar todas como lidas"
                  aria-label="Marcar todas como lidas"
                  @click="marcarTodasComoLidas"
                >
                  <CheckCheck class="h-5 w-5" aria-hidden="true" />
                </button>
              </div>

              <div class="max-h-80 overflow-auto">
                <button
                  v-for="notificacao in notificacoes"
                  :key="notificacao.idNotificacao"
                  class="grid w-full gap-1 rounded-md border border-transparent p-3 text-left transition hover:border-[#d4dee9] hover:bg-[#f5f8fb]"
                  type="button"
                  @click="abrirNotificacao(notificacao)"
                >
                  <span class="flex items-start justify-between gap-3">
                    <strong class="min-w-0 break-words text-sm text-[#071d3b]">{{ notificacao.titulo }}</strong>
                    <span v-if="!notificacao.lida" class="mt-1 h-2 w-2 shrink-0 rounded-full bg-[#d64200]" />
                  </span>
                  <span class="line-clamp-2 break-words text-sm font-semibold text-[#51627a] [overflow-wrap:anywhere]">{{ notificacao.mensagem }}</span>
                  <span class="text-xs font-extrabold text-[#7a8798]">{{ formatarDataNotificacao(notificacao.criadaEmUtc) }}</span>
                </button>

                <p v-if="!notificacoes.length && !carregandoNotificacoes" class="m-0 rounded-md bg-[#f5f8fb] p-3 text-sm font-semibold text-[#51627a]">
                  Nenhuma notificacao.
                </p>
                <p v-if="carregandoNotificacoes" class="m-0 rounded-md bg-[#f5f8fb] p-3 text-sm font-semibold text-[#51627a]">
                  Carregando notificacoes...
                </p>
              </div>
            </div>
          </div>
          <NuxtLink
            class="inline-flex min-h-11 items-center gap-2 rounded-md bg-[#eaf4f1] px-4 text-sm font-extrabold text-[#006b61] no-underline transition hover:bg-[#dcefeb]"
            to="/alterar-senha"
          >
            <KeyRound class="h-5 w-5" aria-hidden="true" />
            Alterar senha
          </NuxtLink>
          <button
            class="inline-flex min-h-11 items-center gap-2 rounded-md bg-[#eaf4f1] px-4 text-sm font-extrabold text-[#006b61] transition hover:bg-[#dcefeb]"
            type="button"
            @click="sair"
          >
            <LogOut class="h-5 w-5" aria-hidden="true" />
            Sair
          </button>
        </div>
      </div>
    </header>

    <main class="mx-auto w-full max-w-7xl px-4 pb-10 sm:px-8">
      <slot />
    </main>

    <div
      v-if="notificacaoSelecionada"
      class="fixed inset-0 z-40 grid place-items-center bg-[#071d3b]/50 px-4 py-6"
      @click.self="fecharNotificacao"
    >
      <article class="grid max-h-[90vh] w-full max-w-lg min-w-0 gap-4 overflow-y-auto overflow-x-hidden rounded-lg bg-white p-5 shadow-[0_22px_55px_rgba(14,30,53,0.24)]">
        <div class="flex items-start justify-between gap-4">
          <div class="min-w-0">
            <p class="m-0 break-words text-xs font-extrabold uppercase text-[#d64200] [overflow-wrap:anywhere]">{{ formatarTipoNotificacao(notificacaoSelecionada.tipo) }}</p>
            <h2 class="m-0 mt-1 break-words text-xl font-normal text-[#071d3b] [overflow-wrap:anywhere]">{{ notificacaoSelecionada.titulo }}</h2>
          </div>
          <button
            class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
            type="button"
            title="Fechar"
            aria-label="Fechar"
            @click="fecharNotificacao"
          >
            <X class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>

        <p class="m-0 min-w-0 whitespace-pre-wrap break-words text-sm font-semibold leading-6 text-[#243044] [overflow-wrap:anywhere]">
          {{ notificacaoSelecionada.mensagem }}
        </p>
        <p class="m-0 text-xs font-extrabold text-[#7a8798]">
          {{ formatarDataNotificacao(notificacaoSelecionada.criadaEmUtc) }}
        </p>
        <NuxtLink
          v-if="notificacaoSelecionada.link"
          class="inline-flex min-h-10 items-center justify-center rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white no-underline transition hover:bg-[#0f6c61]"
          :to="notificacaoSelecionada.link"
          @click="fecharNotificacao"
        >
          Abrir registro
        </NuxtLink>
      </article>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Bell, CheckCheck, KeyRound, LogOut, X } from '@lucide/vue'
import type { Notificacao } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const auth = useAuthStore()
const route = useRoute()
const { $api } = useNuxtApp()
const notificacoes = ref<Notificacao[]>([])
const notificacoesAbertas = ref(false)
const notificacaoSelecionada = ref<Notificacao | null>(null)
const carregandoNotificacoes = ref(false)

const nomeUsuario = computed(() => auth.usuario?.nome || auth.perfil || 'Usuario')
const notificacoesNaoLidas = computed(() => notificacoes.value.filter((notificacao) => !notificacao.lida).length)
const tituloPagina = computed(() => {
  if (route.path.startsWith('/usuarios')) return 'Gestao de Usuarios'
  if (route.path.startsWith('/caderneta-digital')) return 'Caderneta Digital'
  if (route.path.startsWith('/calendario-escolar')) return 'Calendario Escolar'
  if (route.path.startsWith('/qr-code-bancario')) return 'QR Code Bancario'
  if (route.path.startsWith('/alterar-senha')) return 'Alterar senha'

  return 'Escola High Tech'
})

onMounted(() => {
  carregarNotificacoes()
})

watch(() => auth.token, (token) => {
  notificacoes.value = []
  notificacoesAbertas.value = false
  notificacaoSelecionada.value = null

  if (token) {
    carregarNotificacoes()
  }
})

watch(() => route.fullPath, () => {
  notificacoesAbertas.value = false
})

function sair() {
  auth.logout()
  navigateTo('/login')
}

async function carregarNotificacoes() {
  if (!auth.token) return

  carregandoNotificacoes.value = true

  try {
    notificacoes.value = await $api<Notificacao[]>('/notificacoes')
  } catch (err) {
    console.warn(normalizeApiError(err))
  } finally {
    carregandoNotificacoes.value = false
  }
}

async function alternarNotificacoes() {
  notificacoesAbertas.value = !notificacoesAbertas.value

  if (notificacoesAbertas.value) {
    await carregarNotificacoes()
  }
}

async function abrirNotificacao(notificacao: Notificacao) {
  notificacaoSelecionada.value = notificacao

  if (notificacao.lida) {
    return
  }

  try {
    const updated = await $api<Notificacao>(`/notificacoes/${notificacao.idNotificacao}/lida`, {
      method: 'PATCH'
    })

    notificacoes.value = notificacoes.value.map((item) =>
      item.idNotificacao === updated.idNotificacao ? updated : item
    )
    notificacaoSelecionada.value = updated
  } catch (err) {
    console.warn(normalizeApiError(err))
  }
}

async function marcarTodasComoLidas() {
  try {
    await $api<{ total: number }>('/notificacoes/lidas', {
      method: 'PATCH'
    })
    notificacoes.value = notificacoes.value.map((notificacao) => ({
      ...notificacao,
      lida: true,
      lidaEmUtc: notificacao.lidaEmUtc ?? new Date().toISOString()
    }))
  } catch (err) {
    console.warn(normalizeApiError(err))
  }
}

function fecharNotificacao() {
  notificacaoSelecionada.value = null
}

function formatarTipoNotificacao(value?: string | null) {
  if (!value) return 'Notificacao'

  const key = value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^a-zA-Z0-9]/g, '')
    .toUpperCase()
  const labels: Record<string, string> = {
    DADOSUSUARIOATUALIZADOS: 'Dados usuario atualizados',
    ARQUIVOUSUARIOATUALIZADO: 'Arquivo usuario atualizado',
    ARQUIVOUSUARIOATUALIZADOS: 'Arquivos usuario atualizados',
    CERTIFICADOADICIONADO: 'Certificado adicionado',
    CERTIFICADOUSUARIOADICIONADO: 'Certificado usuario adicionado',
    CERTIFICADOUSUARIOATUALIZADO: 'Certificado usuario atualizado'
  }

  if (labels[key]) {
    return labels[key]
  }

  return value
    .replace(/([a-z])([A-Z])/g, '$1 $2')
    .replace(/([A-Z]+)([A-Z][a-z])/g, '$1 $2')
    .replace(/[_-]+/g, ' ')
    .replace(/\s+/g, ' ')
    .trim()
}

function formatarDataNotificacao(value: string) {
  if (!value) return ''

  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value))
}
</script>
