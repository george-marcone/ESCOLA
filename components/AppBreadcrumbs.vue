<template>
  <nav
    v-if="items.length"
    class="flex min-w-0 max-w-full flex-wrap items-center gap-1 text-sm font-extrabold text-[#62728a]"
    aria-label="Caminho da pagina"
  >
    <template v-for="(item, index) in items" :key="`${item.label}-${item.to || index}`">
      <ChevronRight class="h-4 w-4 shrink-0 text-[#9aa7b7]" aria-hidden="true" />
      <NuxtLink
        v-if="item.to && index < items.length - 1"
        class="min-w-0 break-words rounded-md px-2 py-1 text-[#51627a] no-underline transition hover:bg-[#edf3f8] hover:text-[#071d3b]"
        :to="item.to"
      >
        {{ item.label }}
      </NuxtLink>
      <span v-else class="min-w-0 break-words rounded-md bg-[#edf3f8] px-2 py-1 text-[#071d3b]">
        {{ item.label }}
      </span>
    </template>
  </nav>
</template>

<script setup lang="ts">
import { ChevronRight } from '@lucide/vue'

interface BreadcrumbItem {
  label: string
  to?: string
}

const route = useRoute()

const routeLabels: Record<string, string> = {
  '/alterar-senha': 'Alterar senha',
  '/caderneta-digital': 'Caderneta Digital',
  '/calendario-escolar': 'Calendario Escolar',
  '/holerite': 'Holerite',
  '/qr-code-bancario': 'QR Code Bancario',
  '/usuarios': 'Usuarios'
}

const items = computed<BreadcrumbItem[]>(() => {
  const path = normalizarPath(route.path)

  if (path === '/') return []

  if (path === '/usuarios/novo') {
    return [
      { label: 'Usuarios', to: '/usuarios' },
      { label: 'Novo usuario' }
    ]
  }

  if (/^\/usuarios\/[^/]+$/.test(path)) {
    return [
      { label: 'Usuarios', to: '/usuarios' },
      { label: 'Visualizar usuario' }
    ]
  }

  return [{ label: routeLabels[path] ?? formatarSegmento(path.split('/').filter(Boolean).at(-1) ?? '') }]
})

function normalizarPath(path: string) {
  if (!path || path === '/') return '/'

  return path.replace(/\/+$/, '')
}

function formatarSegmento(segmento: string) {
  return segmento
    .replace(/[-_]+/g, ' ')
    .replace(/\b\w/g, (letter) => letter.toUpperCase())
}
</script>
