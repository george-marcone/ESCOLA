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
        <div class="grid w-full grid-cols-1 gap-2 sm:w-auto sm:grid-cols-2">
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
  </div>
</template>

<script setup lang="ts">
import { KeyRound, LogOut } from '@lucide/vue'

const auth = useAuthStore()
const route = useRoute()

const nomeUsuario = computed(() => auth.usuario?.nome || auth.perfil || 'Usuario')
const tituloPagina = computed(() => {
  if (route.path.startsWith('/usuarios')) return 'Gestao de Usuarios'
  if (route.path.startsWith('/caderneta-digital')) return 'Caderneta Digital'
  if (route.path.startsWith('/alterar-senha')) return 'Alterar senha'

  return 'Escola High Tech'
})

function sair() {
  auth.logout()
  navigateTo('/login')
}
</script>
