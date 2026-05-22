<template>
  <section class="stack narrow-stack">
    <div class="page-heading">
      <p class="eyebrow">Seguranca</p>
      <h1>Alterar senha</h1>
    </div>

    <form class="form-panel" @submit.prevent="salvar">
      <div class="form-grid">
        <p v-if="auth.deveAlterarSenhaPadrao" class="alert alert-warning">
          Troque a senha padrao para liberar o acesso ao sistema.
        </p>

        <label>
          <span>Senha atual</span>
          <input v-model="form.senhaAtual" type="password" autocomplete="current-password" required />
        </label>

        <label>
          <span>Nova senha</span>
          <input v-model="form.novaSenha" type="password" autocomplete="new-password" required />
        </label>

        <PasswordStrengthMeter :password="form.novaSenha" />

        <label>
          <span>Confirmar nova senha</span>
          <input v-model="form.confirmacaoSenha" type="password" autocomplete="new-password" required />
        </label>
      </div>

      <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="form-actions">
        <button class="btn btn-primary" type="submit" :disabled="auth.loading">
          {{ auth.loading ? 'Alterando...' : 'Alterar senha' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { normalizeApiError } from '~/utils/api-client'

const auth = useAuthStore()
const mensagem = ref('')
const erro = ref('')
const form = reactive({
  senhaAtual: '',
  novaSenha: '',
  confirmacaoSenha: ''
})

async function salvar() {
  mensagem.value = ''
  erro.value = ''

  try {
    await auth.alterarSenha({ ...form })
    mensagem.value = 'Senha alterada com sucesso.'
    form.senhaAtual = ''
    form.novaSenha = ''
    form.confirmacaoSenha = ''
    setTimeout(() => navigateTo('/'), 600)
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
