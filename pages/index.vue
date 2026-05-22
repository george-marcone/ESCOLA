<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Painel</p>
      <h1>Cadastros da escola</h1>
    </div>

    <div class="summary-strip">
      <div>
        <span>Usuario</span>
        <strong>{{ auth.usuario?.nome }}</strong>
      </div>
      <div>
        <span>Perfil</span>
        <strong>{{ auth.perfil }}</strong>
      </div>
    </div>

    <div class="action-grid">
      <NuxtLink class="action-link" to="/diretoria">
        <span>Diretoria</span>
        <strong>Consultar diretoria</strong>
      </NuxtLink>
      <NuxtLink v-if="auth.canWrite" class="action-link" to="/diretoria/novo">
        <span>Cadastro</span>
        <strong>Nova diretoria</strong>
      </NuxtLink>
      <NuxtLink v-if="auth.isAdmin" class="action-link" to="/usuarios/novo">
        <span>Cadastro</span>
        <strong>Novo usuario</strong>
      </NuxtLink>
      <NuxtLink class="action-link" to="/professores">
        <span>Consulta</span>
        <strong>Professores</strong>
      </NuxtLink>
      <NuxtLink class="action-link" to="/alunos">
        <span>Consulta</span>
        <strong>Alunos</strong>
      </NuxtLink>
    </div>
  </section>
</template>

<script setup lang="ts">
const auth = useAuthStore()
</script>
