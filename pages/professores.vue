<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Consulta</p>
      <h1>Professores</h1>
    </div>

    <p v-if="erro" class="alert alert-error">{{ erro }}</p>

    <div class="table-shell">
      <table>
        <thead>
          <tr>
            <th>Codigo</th>
            <th>Nome</th>
            <th>Alunos</th>
            <th>Usuario vinculado</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="professor in professores" :key="professor.id">
            <td>{{ professor.id }}</td>
            <td>{{ professor.nome }}</td>
            <td>{{ professor.alunos?.length ?? 0 }}</td>
            <td>{{ professor.idUsuario || '-' }}</td>
          </tr>
          <tr v-if="!carregando && !professores.length">
            <td colspan="4">Nenhum professor encontrado.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { Professor } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const { $api } = useNuxtApp()
const professores = ref<Professor[]>([])
const carregando = ref(false)
const erro = ref('')

onMounted(async () => {
  carregando.value = true
  try {
    professores.value = await $api<Professor[]>('/professor')
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
})
</script>
