<template>
  <section class="stack">
    <div class="page-heading with-action">
      <div>
        <p class="eyebrow">Diretoria</p>
        <h1>Integrantes da diretoria</h1>
      </div>
      <NuxtLink v-if="auth.canWrite" class="btn btn-primary" to="/diretoria/novo">Novo</NuxtLink>
    </div>

    <p v-if="diretoria.error" class="alert alert-error">{{ diretoria.error }}</p>

    <div class="table-shell">
      <table>
        <thead>
          <tr>
            <th>Codigo</th>
            <th>Nome</th>
            <th>Usuario vinculado</th>
            <th>Perfil</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in diretoria.diretorias" :key="item.id">
            <td>{{ item.id }}</td>
            <td>{{ item.nome }}</td>
            <td>{{ item.usuario?.nome || '-' }}</td>
            <td>{{ item.usuario?.descricaoPerfil || '-' }}</td>
          </tr>
          <tr v-if="!diretoria.loading && !diretoria.diretorias.length">
            <td colspan="4">Nenhum registro encontrado.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
const auth = useAuthStore()
const diretoria = useDiretoriaStore()

onMounted(() => {
  diretoria.fetchDiretorias().catch(() => {})
})
</script>
