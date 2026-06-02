<template>
  <section class="w-[min(420px,100%)] rounded-lg border border-[#d4dee9] bg-white p-5 shadow-[0_22px_55px_rgba(14,30,53,0.12)] sm:p-8">
    <div>
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">GM Tech Solutions</p>
      <h1 class="mb-2 mt-2 text-3xl font-normal leading-tight text-[#071d3b] sm:text-4xl">Escola Conectada</h1>
      <p class="m-0 text-sm font-bold uppercase text-[#147f72]">Acesso seguro</p>
    </div>

    <form v-if="!mostrarAlteracaoSenha" class="mt-8 grid gap-5" @submit.prevent="entrar">
      <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Email</span>
        <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" autocomplete="email" required />
      </label>

      <PasswordInput v-model="form.senha" label="Senha" autocomplete="current-password" required />

      <p v-if="auth.error" class="alert alert-error">{{ auth.error }}</p>
      <p v-if="erroReset" class="alert alert-error">{{ erroReset }}</p>

      <button class="inline-flex min-h-12 items-center justify-center rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Entrando...' : 'Entrar' }}
      </button>

      <button class="inline-flex min-h-10 items-center justify-center rounded-md border border-[#147f72] bg-white px-4 text-sm font-extrabold text-[#147f72] transition hover:bg-[#e9f7f5] disabled:cursor-wait disabled:opacity-70" type="button" :disabled="auth.loading" @click="abrirTrocaSenha">
        Esqueceu a senha?
      </button>
    </form>

    <form v-else class="mt-8 grid gap-5" @submit.prevent="alterarSenha">
      <p class="alert alert-warning">
        {{ mensagemTrocaSenha }}
      </p>

      <label v-if="trocaSolicitada" class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Email</span>
        <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" autocomplete="email" required />
      </label>

      <PasswordInput v-if="trocaSolicitada" v-model="form.senha" label="Senha atual ou senha padrao" autocomplete="current-password" required />

      <PasswordInput v-model="senhaForm.novaSenha" label="Nova senha" autocomplete="new-password" required />

      <PasswordStrengthMeter :password="senhaForm.novaSenha" />

      <PasswordInput v-model="senhaForm.confirmacaoSenha" label="Confirmar nova senha" autocomplete="new-password" required />

      <p v-if="erroAlteracao" class="alert alert-error">{{ erroAlteracao }}</p>

      <div class="grid gap-3">
        <button class="inline-flex min-h-12 items-center justify-center rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70" type="submit" :disabled="auth.loading">
          {{ auth.loading ? 'Alterando...' : 'Alterar senha e continuar' }}
        </button>

        <button v-if="trocaSolicitada" class="inline-flex min-h-10 items-center justify-center rounded-md border border-[#147f72] bg-white px-4 text-sm font-extrabold text-[#147f72] transition hover:bg-[#e9f7f5] disabled:cursor-wait disabled:opacity-70" type="button" :disabled="auth.loading" @click="voltarLogin">
          Voltar para login
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  layout: 'auth',
  public: true
})

const auth = useAuthStore()
const form = reactive({
  email: '',
  senha: ''
})
const senhaForm = reactive({
  novaSenha: '',
  confirmacaoSenha: ''
})
const mostrarAlteracaoSenha = ref(false)
const trocaSolicitada = ref(false)
const erroAlteracao = ref('')
const erroReset = ref('')
const mensagemTrocaSenha = computed(() =>
  trocaSolicitada.value
    ? 'Informe sua senha atual ou a senha padrao para definir uma nova senha.'
    : 'Sua conta ainda usa a senha padrao. Defina uma nova senha para continuar.'
)

async function entrar() {
  erroReset.value = ''
  trocaSolicitada.value = false

  const response = await auth.login(form)
  if (response.deveAlterarSenhaPadrao) {
    mostrarAlteracaoSenha.value = true
    return
  }

  await navigateTo('/')
}

function abrirTrocaSenha() {
  erroReset.value = ''
  erroAlteracao.value = ''

  if (!form.email) {
    erroReset.value = 'Informe o email para alterar a senha.'
    return
  }

  trocaSolicitada.value = true
  mostrarAlteracaoSenha.value = true
}

function voltarLogin() {
  mostrarAlteracaoSenha.value = false
  trocaSolicitada.value = false
  erroAlteracao.value = ''
  erroReset.value = ''
  senhaForm.novaSenha = ''
  senhaForm.confirmacaoSenha = ''
}

async function alterarSenha() {
  erroAlteracao.value = ''

  try {
    if (trocaSolicitada.value && !auth.isAuthenticated) {
      await auth.login(form)
    }

    await auth.alterarSenha({
      senhaAtual: form.senha,
      novaSenha: senhaForm.novaSenha,
      confirmacaoSenha: senhaForm.confirmacaoSenha
    })
  } catch (err) {
    erroAlteracao.value = normalizeApiError(err)
    return
  }

  await navigateTo('/')
}
</script>
