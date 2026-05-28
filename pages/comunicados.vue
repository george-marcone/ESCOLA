<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(300px,420px)_minmax(0,1fr)]">
    <form
      class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="enviarComunicado"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Comunicados</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">Enviar comunicado</h2>

      <div class="grid gap-5">
        <div class="grid gap-2 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
          <span class="text-xs font-extrabold uppercase text-[#62728a]">Destinatarios</span>
          <strong class="text-sm text-[#071d3b]">Alunos e professores</strong>
        </div>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Tipo</span>
          <select
            v-model="form.tipo"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
          >
            <option value="Comunicado">Comunicado</option>
            <option value="Geral">Geral</option>
            <option value="Calendario">Calendario</option>
            <option value="Reuniao">Reuniao</option>
            <option value="Evento">Evento</option>
          </select>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Titulo</span>
          <input
            v-model.trim="form.titulo"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="text"
            maxlength="120"
            required
          />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.titulo.length }}/120</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Mensagem</span>
          <textarea
            v-model.trim="form.mensagem"
            class="min-h-40 resize-y rounded-md border border-[#ccd8e5] px-3 py-2 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            maxlength="2000"
            required
          />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.mensagem.length }}/2000</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Link relacionado</span>
          <input
            v-model.trim="form.link"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="text"
            maxlength="240"
            placeholder="/calendario-escolar"
          />
        </label>

        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
        <p v-if="erro" class="alert alert-error">{{ erro }}</p>

        <button
          class="inline-flex min-h-12 w-full max-w-full items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-70"
          type="submit"
          :disabled="enviando"
        >
          <Send class="h-5 w-5" aria-hidden="true" />
          {{ enviando ? 'Enviando...' : 'Enviar comunicado' }}
        </button>
      </div>
    </form>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Previa</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">Notificacao</h2>

      <div class="grid gap-5 rounded-lg border border-[#d4dee9] bg-[#f8fbfd] p-4 sm:p-5">
        <div class="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
          <div class="min-w-0">
            <p class="m-0 text-xs font-extrabold uppercase text-[#62728a]">{{ form.tipo }}</p>
            <h3 class="m-0 mt-2 break-words text-lg font-extrabold text-[#071d3b]">
              {{ tituloPrevia }}
            </h3>
          </div>
          <Megaphone class="h-7 w-7 shrink-0 text-[#147f72]" aria-hidden="true" />
        </div>

        <p class="m-0 whitespace-pre-wrap break-words text-sm font-semibold leading-6 text-[#51627a]">
          {{ mensagemPrevia }}
        </p>

        <div v-if="form.link" class="grid gap-2 border-t border-[#d4dee9] pt-4">
          <span class="text-xs font-extrabold uppercase text-[#62728a]">Link</span>
          <strong class="break-words text-sm text-[#071d3b]">{{ form.link }}</strong>
        </div>
      </div>

      <div v-if="ultimoEnvio" class="mt-5 grid gap-2 rounded-md border border-[#d7f2ea] bg-[#eafaf6] p-4">
        <span class="text-xs font-extrabold uppercase text-[#147f72]">Ultimo envio</span>
        <strong class="text-sm text-[#071d3b]">{{ ultimoEnvio.total }} destinatario(s)</strong>
      </div>
    </article>
  </section>
</template>

<script setup lang="ts">
import { Megaphone, Send } from '@lucide/vue'
import type { NotificacaoEnvio, NotificacaoPerfisPayload } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  middleware: ['administrador']
})

const { $api } = useNuxtApp()

const form = reactive({
  tipo: 'Comunicado',
  titulo: '',
  mensagem: '',
  link: ''
})
const enviando = ref(false)
const mensagem = ref('')
const erro = ref('')
const ultimoEnvio = ref<NotificacaoEnvio | null>(null)

const tituloPrevia = computed(() => form.titulo || 'Titulo do comunicado')
const mensagemPrevia = computed(() => form.mensagem || 'Mensagem do comunicado.')

async function enviarComunicado() {
  if (!form.titulo.trim() || !form.mensagem.trim()) {
    erro.value = 'Informe titulo e mensagem do comunicado.'
    return
  }

  enviando.value = true
  erro.value = ''
  mensagem.value = ''

  const payload: NotificacaoPerfisPayload = {
    tiposUsuario: ['Aluno', 'Professor'],
    titulo: form.titulo.trim(),
    mensagem: form.mensagem.trim(),
    tipo: form.tipo,
    link: form.link.trim() || null
  }

  try {
    const response = await $api<NotificacaoEnvio>('/notificacoes/perfis', {
      method: 'POST',
      body: payload
    })

    ultimoEnvio.value = response
    mensagem.value = `Comunicado enviado para ${response.total} destinatario(s).`
    form.titulo = ''
    form.mensagem = ''
    form.link = ''
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    enviando.value = false
  }
}
</script>
