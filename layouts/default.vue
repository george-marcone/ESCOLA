<template>
  <div class="app-shell">
    <header class="topbar">
      <NuxtLink class="brand" to="/">Form Escola</NuxtLink>

      <nav class="main-nav" aria-label="Navegacao principal">
        <NuxtLink to="/diretoria">Diretoria</NuxtLink>
        <NuxtLink v-if="auth.canWrite" to="/diretoria/novo">Nova diretoria</NuxtLink>
        <NuxtLink v-if="auth.isAdmin" to="/usuarios/novo">Novo usuario</NuxtLink>
        <NuxtLink to="/alterar-senha">Alterar senha</NuxtLink>
        <NuxtLink to="/professores">Professores</NuxtLink>
        <NuxtLink to="/alunos">Alunos</NuxtLink>
      </nav>

      <div class="session">
        <span v-if="auth.usuario" class="session-user">{{ auth.usuario.nome }}</span>
        <button class="btn btn-ghost" type="button" @click="sair">Sair</button>
      </div>
    </header>

    <main class="page">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
const auth = useAuthStore()

function sair() {
  auth.logout()
  navigateTo('/login')
}
</script>
