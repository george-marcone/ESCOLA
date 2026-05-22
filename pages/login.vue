<template>
  <section class="login-panel">
    <div>
      <p class="eyebrow">Acesso seguro</p>
      <h1>Form Escola</h1>
    </div>

    <form v-if="!mostrarAlteracaoSenha" class="form-grid" @submit.prevent="entrar">
      <label>
        <span>Email</span>
        <input v-model.trim="form.email" type="email" autocomplete="email" required />
      </label>

      <label>
        <span>Senha</span>
        <input v-model="form.senha" type="password" autocomplete="current-password" required />
      </label>

      <p v-if="auth.error" class="alert alert-error">{{ auth.error }}</p>

      <button class="btn btn-primary" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>

    <form v-else class="form-grid" @submit.prevent="alterarSenha">
      <p class="alert alert-warning">
        Sua conta ainda usa a senha padrao. Defina uma nova senha para continuar.
      </p>

      <label>
        <span>Nova senha</span>
        <input v-model="senhaForm.novaSenha" type="password" autocomplete="new-password" required />
      </label>

      <PasswordStrengthMeter :password="senhaForm.novaSenha" />

      <label>
        <span>Confirmar nova senha</span>
        <input v-model="senhaForm.confirmacaoSenha" type="password" autocomplete="new-password" required />
      </label>

      <p v-if="erroAlteracao" class="alert alert-error">{{ erroAlteracao }}</p>

      <button class="btn btn-primary" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Alterando...' : 'Alterar senha e continuar' }}
      </button>
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
const erroAlteracao = ref('')

async function entrar() {
  const response = await auth.login(form)
  if (response.deveAlterarSenhaPadrao) {
    mostrarAlteracaoSenha.value = true
    return
  }

  await navigateTo('/')
}

async function alterarSenha() {
  erroAlteracao.value = ''

  try {
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
