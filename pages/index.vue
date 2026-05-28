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
        <NuxtLink
          v-for="item in modulos"
          :key="item.to"
          class="group grid min-h-32 gap-4 rounded-lg border border-[#d4dee9] bg-white p-5 no-underline transition hover:border-[#147f72] hover:shadow-[0_12px_30px_rgba(20,127,114,0.12)]"
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
    </article>
  </section>
</template>

<script setup lang="ts">
import { ArrowRight, BookOpen, CalendarDays, QrCode, ShieldCheck, UserCog } from '@lucide/vue'

const auth = useAuthStore()

const modulos = computed(() => [
  { label: 'Usuarios', title: auth.isAluno ? 'Corrigir meu cadastro' : 'Gerenciar usuarios', to: '/usuarios', icon: UserCog, show: true },
  { label: 'Caderneta Digital', title: auth.isProfessor ? 'Administrar notas e frequencia' : 'Visualizar boletim e frequencia', to: '/caderneta-digital', icon: BookOpen, show: true },
  { label: 'Calendario Escolar', title: auth.isProfessor ? 'Planejar avaliacoes e trabalhos' : 'Consultar agenda escolar', to: '/calendario-escolar', icon: CalendarDays, show: true },
  { label: 'QR Code', title: 'Gerar dados bancarios ficticios', to: '/qr-code-bancario', icon: QrCode, show: auth.isAluno },
  { label: 'Seguranca', title: 'Alterar senha', to: '/alterar-senha', icon: ShieldCheck, show: true }
].filter((item) => item.show))
</script>
