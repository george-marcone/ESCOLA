<template>
  <section v-if="auth.isAluno" class="grid gap-5 xl:grid-cols-[minmax(300px,420px)_minmax(0,1fr)]">
    <form
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="gerarQrCode"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Aluno</p>
      <h2 class="mb-6 mt-2 text-xl font-normal text-[#071d3b]">Dados bancarios ficticios</h2>

      <div class="grid gap-4">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome do aluno</span>
          <input
            v-model.trim="form.nomeAluno"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="text"
            required
            maxlength="120"
          />
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Banco ficticio</span>
          <select
            v-model="form.banco"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
          >
            <option v-for="banco in BANCOS_FICTICIOS" :key="banco.codigo" :value="banco.nome">
              {{ banco.nome }}
            </option>
          </select>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Valor ficticio</span>
          <input
            v-model.number="form.valor"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="number"
            min="0"
            step="0.01"
            required
          />
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Descricao</span>
          <input
            v-model.trim="form.descricao"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            type="text"
            maxlength="120"
          />
        </label>

        <p v-if="erro" class="alert alert-error">{{ erro }}</p>
        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>

        <button
          class="inline-flex min-h-12 items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70"
          type="submit"
          :disabled="gerando"
        >
          <QrCode class="h-5 w-5" aria-hidden="true" />
          {{ gerando ? 'Gerando QR Code...' : 'Gerar QR Code' }}
        </button>
      </div>
    </form>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Sem valor bancario</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">QR Code do aluno</h2>
        </div>
        <button
          class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Gerar novamente"
          aria-label="Gerar novamente"
          :disabled="gerando"
          @click="gerarQrCode"
        >
          <RefreshCcw class="h-5 w-5" aria-hidden="true" />
        </button>
      </div>

      <div class="mt-5 grid gap-6 lg:grid-cols-[320px_minmax(0,1fr)]">
        <div class="grid min-h-[320px] place-items-center rounded-lg border border-[#d4dee9] bg-[#f5f8fb] p-4">
          <img
            v-if="qrCodeDataUrl"
            class="h-72 w-72 rounded-md bg-white p-3 shadow-sm"
            :src="qrCodeDataUrl"
            alt="QR Code bancario ficticio"
          />
          <div v-else class="grid place-items-center gap-3 text-center text-[#62728a]">
            <Landmark class="h-12 w-12" aria-hidden="true" />
            <strong class="text-sm">QR Code aguardando geracao.</strong>
          </div>
        </div>

        <div class="min-w-0">
          <div class="grid gap-3">
            <div
              v-for="item in detalhes"
              :key="item.label"
              class="grid gap-1 border-b border-[#d4dee9] pb-3 last:border-b-0 last:pb-0"
            >
              <span class="text-xs font-extrabold uppercase text-[#62728a]">{{ item.label }}</span>
              <strong class="break-words text-sm text-[#071d3b]">{{ item.value }}</strong>
            </div>
          </div>

          <div class="mt-6 grid gap-2 sm:grid-cols-2">
            <a
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-[#eaf4f1] px-4 text-sm font-extrabold text-[#006b61] no-underline transition hover:bg-[#dcefeb]"
              :class="{ 'pointer-events-none opacity-60': !qrCodeDataUrl }"
              :href="whatsappHref"
              target="_blank"
              rel="noopener noreferrer"
              :aria-disabled="!qrCodeDataUrl"
              @click="bloquearCompartilhamentoSemQr"
            >
              <MessageCircle class="h-5 w-5" aria-hidden="true" />
              WhatsApp
            </a>
            <a
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md bg-[#eaf4f1] px-4 text-sm font-extrabold text-[#006b61] no-underline transition hover:bg-[#dcefeb]"
              :class="{ 'pointer-events-none opacity-60': !qrCodeDataUrl }"
              :href="emailHref"
              :aria-disabled="!qrCodeDataUrl"
              @click="bloquearCompartilhamentoSemQr"
            >
              <Mail class="h-5 w-5" aria-hidden="true" />
              Email
            </a>
            <button
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8] disabled:opacity-60"
              type="button"
              :disabled="!qrCodeDataUrl"
              @click="copiarDados"
            >
              <Copy class="h-5 w-5" aria-hidden="true" />
              Copiar dados
            </button>
            <button
              class="inline-flex min-h-11 items-center justify-center gap-2 rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8] disabled:opacity-60"
              type="button"
              :disabled="!qrCodeDataUrl"
              @click="baixarQrCode"
            >
              <Download class="h-5 w-5" aria-hidden="true" />
              Baixar QR
            </button>
          </div>
        </div>
      </div>
    </article>
  </section>
</template>

<script setup lang="ts">
import { Copy, Download, Landmark, Mail, MessageCircle, QrCode, RefreshCcw } from '@lucide/vue'
import { toDataURL } from 'qrcode'
import type { UsuarioSummary } from '~/types/api'
import {
  BANCOS_FICTICIOS,
  criarDadosBancariosFicticios,
  formatarMoedaFicticia,
  montarMensagemCompartilhamento,
  montarPayloadQrCode,
  type DadosBancariosFicticios
} from '~/utils/qr-code-bancario'

definePageMeta({
  roles: [],
  middleware: ['aluno']
})

const auth = useAuthStore()
const gerando = ref(false)
const erro = ref('')
const mensagem = ref('')
const qrCodeDataUrl = ref('')
const dadosGerados = ref<DadosBancariosFicticios | null>(null)
const form = reactive({
  nomeAluno: auth.usuario?.nome ?? '',
  banco: BANCOS_FICTICIOS[0].nome,
  valor: 149.9,
  descricao: 'Mensalidade escolar ficticia'
})

const detalhes = computed(() => {
  if (!dadosGerados.value) {
    return [
      { label: 'Status', value: 'Aguardando geracao do QR Code.' }
    ]
  }

  return [
    { label: 'Aluno', value: dadosGerados.value.aluno },
    { label: 'Referencia', value: dadosGerados.value.referencia },
    { label: 'Banco', value: dadosGerados.value.banco },
    { label: 'Agencia', value: dadosGerados.value.agencia },
    { label: 'Conta', value: dadosGerados.value.conta },
    { label: 'Chave demonstrativa', value: dadosGerados.value.chaveDemonstracao },
    { label: 'Valor ficticio', value: formatarMoedaFicticia(dadosGerados.value.valor) },
    { label: 'Descricao', value: dadosGerados.value.descricao }
  ]
})
const mensagemCompartilhamento = computed(() =>
  dadosGerados.value ? montarMensagemCompartilhamento(dadosGerados.value) : ''
)
const whatsappHref = computed(() =>
  mensagemCompartilhamento.value
    ? `https://wa.me/?text=${encodeURIComponent(mensagemCompartilhamento.value)}`
    : '#'
)
const emailHref = computed(() =>
  mensagemCompartilhamento.value
    ? `mailto:?subject=${encodeURIComponent('QR Code bancario ficticio - Escola Conectada')}&body=${encodeURIComponent(mensagemCompartilhamento.value)}`
    : '#'
)

watch(() => auth.usuario, (usuario) => {
  if (!form.nomeAluno && usuario?.nome) {
    form.nomeAluno = usuario.nome
  }
}, { immediate: true })

onMounted(() => {
  if (!auth.isAluno) return

  void gerarQrCode()
})

async function gerarQrCode() {
  erro.value = ''
  mensagem.value = ''
  gerando.value = true

  try {
    const dados = criarDadosBancariosFicticios(criarUsuarioParaQrCode(), {
      banco: form.banco,
      valor: form.valor,
      descricao: form.descricao
    })
    const payload = montarPayloadQrCode(dados)

    dadosGerados.value = dados
    qrCodeDataUrl.value = await toDataURL(payload, {
      errorCorrectionLevel: 'M',
      margin: 2,
      width: 320,
      color: {
        dark: '#071d3b',
        light: '#ffffff'
      }
    })
  } catch {
    erro.value = 'Nao foi possivel gerar o QR Code.'
  } finally {
    gerando.value = false
  }
}

async function copiarDados() {
  if (!mensagemCompartilhamento.value) return

  try {
    await navigator.clipboard.writeText(mensagemCompartilhamento.value)
    mensagem.value = 'Dados copiados.'
  } catch {
    erro.value = 'Nao foi possivel copiar os dados.'
  }
}

function baixarQrCode() {
  if (!qrCodeDataUrl.value || !dadosGerados.value) return

  const link = document.createElement('a')
  link.href = qrCodeDataUrl.value
  link.download = `qr-code-bancario-${dadosGerados.value.referencia.toLowerCase()}.png`
  link.click()
  mensagem.value = 'QR Code baixado.'
}

function bloquearCompartilhamentoSemQr(event: Event) {
  if (!qrCodeDataUrl.value) {
    event.preventDefault()
  }
}

function criarUsuarioParaQrCode(): Pick<UsuarioSummary, 'idUsuario' | 'nome' | 'email'> {
  if (auth.usuario) {
    return {
      idUsuario: auth.usuario.idUsuario,
      nome: form.nomeAluno || auth.usuario.nome,
      email: auth.usuario.email
    }
  }

  return {
    idUsuario: 0,
    nome: form.nomeAluno || 'Aluno demonstracao',
    email: 'aluno.demo@escola.invalid'
  }
}
</script>
