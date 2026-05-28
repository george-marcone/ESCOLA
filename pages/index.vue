<template>
  <section class="grid gap-5 lg:grid-cols-[360px_minmax(0,1fr)]">
    <aside class="rounded-lg border border-[#d4dee9] bg-white p-6 shadow-[0_22px_55px_rgba(14,30,53,0.08)]">
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Painel</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">Sessao ativa</h2>

      <div class="grid gap-4">
        <div class="rounded-lg border border-[#d4dee9] p-4">
          <span class="text-xs font-extrabold text-[#62728a]">Usuario</span>
          <strong class="mt-2 block text-[#071d3b]">{{ auth.usuario?.nome }}</strong>
        </div>
        <div class="rounded-lg border border-[#d4dee9] p-4">
          <span class="text-xs font-extrabold text-[#62728a]">Perfil</span>
          <strong class="mt-2 block text-[#071d3b]">{{ auth.perfil }}</strong>
        </div>
      </div>
    </aside>

    <article class="rounded-lg border border-[#d4dee9] bg-white p-6 shadow-[0_22px_55px_rgba(14,30,53,0.08)]">
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Modulos</p>
      <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Cadastros da escola</h2>

      <div class="mt-5 grid gap-3 md:grid-cols-2">
        <div
          v-for="item in modulos"
          :key="item.id"
          class="relative rounded-lg transition"
          :class="{
            'opacity-60': draggedModuloId === item.id,
            'ring-2 ring-[#147f72] ring-offset-2 ring-offset-white': dragOverModuloId === item.id && draggedModuloId !== item.id
          }"
          @dragenter.prevent="marcarAlvoArraste(item.id)"
          @dragover.prevent="marcarAlvoArraste(item.id)"
          @drop.prevent="soltarModulo(item.id, $event)"
        >
          <button
            class="absolute right-3 top-3 z-10 inline-flex h-9 w-9 cursor-grab items-center justify-center rounded-md bg-[#edf3f8] text-[#51627a] transition hover:bg-[#dfe8f1] hover:text-[#071d3b] active:cursor-grabbing"
            type="button"
            draggable="true"
            title="Arrastar modulo"
            aria-label="Arrastar modulo"
            @click.prevent
            @dragstart.stop="iniciarArrasteModulo(item.id, $event)"
            @dragend="encerrarArrasteModulo"
          >
            <GripVertical class="h-5 w-5" aria-hidden="true" />
          </button>
          <NuxtLink
            class="group grid min-h-32 gap-4 rounded-lg border border-[#d4dee9] bg-white p-5 pr-14 no-underline transition hover:border-[#147f72] hover:shadow-[0_12px_30px_rgba(20,127,114,0.12)]"
            :to="item.to"
          >
            <component :is="item.icon" class="h-7 w-7 text-[#147f72]" aria-hidden="true" />
            <div class="flex items-end justify-between gap-3">
              <div>
                <span class="text-xs font-extrabold uppercase text-[#d64200]">{{ item.label }}</span>
                <strong class="mt-2 block text-[#071d3b]">{{ item.title }}</strong>
              </div>
              <ArrowRight class="h-5 w-5 text-[#62728a] transition group-hover:translate-x-1 group-hover:text-[#147f72]" aria-hidden="true" />
            </div>
          </NuxtLink>
        </div>
      </div>
    </article>
  </section>
</template>

<script setup lang="ts">
import { ArrowRight, BookOpen, CalendarDays, FileText, GripVertical, QrCode, ShieldCheck, UserCog } from '@lucide/vue'
import type { Component } from 'vue'
import { getUsuarioPerfilTipo } from '~/utils/usuario-permissions'

const auth = useAuthStore()
const perfilTipo = computed(() => getUsuarioPerfilTipo(auth.usuario?.descricaoPerfil))
const ordemModulos = ref<string[]>([])
const draggedModuloId = ref('')
const dragOverModuloId = ref('')

interface ModuloPainel {
  id: string
  label: string
  title: string
  to: string
  icon: Component
  show: boolean
}

const baseModulos = computed<ModuloPainel[]>(() => [
  { id: 'usuarios', label: 'Usuarios', title: auth.isAluno ? 'Corrigir meu cadastro' : 'Gerenciar usuarios', to: '/usuarios', icon: UserCog, show: true },
  { id: 'caderneta-digital', label: 'Caderneta Digital', title: auth.isProfessor ? 'Administrar notas e frequencia' : 'Visualizar boletim e frequencia', to: '/caderneta-digital', icon: BookOpen, show: true },
  { id: 'calendario-escolar', label: 'Calendario Escolar', title: auth.isProfessor ? 'Planejar avaliacoes e trabalhos' : 'Consultar agenda escolar', to: '/calendario-escolar', icon: CalendarDays, show: true },
  { id: 'qr-code-bancario', label: 'QR Code', title: 'Gerar dados bancarios ficticios', to: '/qr-code-bancario', icon: QrCode, show: auth.isAluno },
  { id: 'holerite', label: 'Holerite', title: ['administrador', 'diretoria'].includes(perfilTipo.value) ? 'Lancar e consultar PDFs' : 'Consultar meus PDFs', to: '/holerite', icon: FileText, show: ['administrador', 'diretoria', 'professor'].includes(perfilTipo.value) },
  { id: 'seguranca', label: 'Seguranca', title: 'Alterar senha', to: '/alterar-senha', icon: ShieldCheck, show: true }
].filter((item) => item.show))

const storageKey = computed(() =>
  `escola-conectada-painel-ordem-${auth.usuario?.idUsuario ?? perfilTipo.value ?? 'anon'}`
)
const modulos = computed(() => {
  const idsOrdenados = normalizarOrdemModulos(ordemModulos.value)

  return idsOrdenados
    .map((id) => baseModulos.value.find((item) => item.id === id))
    .filter((item): item is ModuloPainel => Boolean(item))
})

onMounted(() => {
  carregarOrdemModulos()
})

watch(storageKey, () => {
  carregarOrdemModulos()
})

watch(baseModulos, () => {
  ordemModulos.value = normalizarOrdemModulos(ordemModulos.value)
}, { immediate: true })

function iniciarArrasteModulo(id: string, event: DragEvent) {
  draggedModuloId.value = id
  dragOverModuloId.value = id
  event.dataTransfer?.setData('text/plain', id)
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
  }
}

function marcarAlvoArraste(id: string) {
  if (!draggedModuloId.value) return

  dragOverModuloId.value = id
}

function soltarModulo(targetId: string, event: DragEvent) {
  const sourceId = event.dataTransfer?.getData('text/plain') || draggedModuloId.value

  if (!sourceId || sourceId === targetId) {
    encerrarArrasteModulo()
    return
  }

  const ids = modulos.value.map((item) => item.id)
  const sourceIndex = ids.indexOf(sourceId)
  const targetIndex = ids.indexOf(targetId)

  if (sourceIndex < 0 || targetIndex < 0) {
    encerrarArrasteModulo()
    return
  }

  const moved = ids.splice(sourceIndex, 1)[0]
  if (!moved) {
    encerrarArrasteModulo()
    return
  }

  ids.splice(targetIndex, 0, moved)
  ordemModulos.value = normalizarOrdemModulos(ids)
  persistirOrdemModulos()
  encerrarArrasteModulo()
}

function encerrarArrasteModulo() {
  draggedModuloId.value = ''
  dragOverModuloId.value = ''
}

function carregarOrdemModulos() {
  if (typeof localStorage === 'undefined') return

  try {
    const raw = localStorage.getItem(storageKey.value)
    const parsed = raw ? JSON.parse(raw) : []
    ordemModulos.value = Array.isArray(parsed) ? normalizarOrdemModulos(parsed) : normalizarOrdemModulos([])
  } catch {
    ordemModulos.value = normalizarOrdemModulos([])
  }
}

function persistirOrdemModulos() {
  if (typeof localStorage === 'undefined') return

  localStorage.setItem(storageKey.value, JSON.stringify(ordemModulos.value))
}

function normalizarOrdemModulos(order: string[]) {
  const visibleIds = baseModulos.value.map((item) => item.id)
  const orderedVisibleIds = order.filter((id) => visibleIds.includes(id))
  const missingIds = visibleIds.filter((id) => !orderedVisibleIds.includes(id))

  return [...orderedVisibleIds, ...missingIds]
}
</script>
