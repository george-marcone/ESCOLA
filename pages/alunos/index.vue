<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Consulta</p>
      <h1>Alunos</h1>
    </div>

    <p v-if="erro" class="alert alert-error">{{ erro }}</p>

    <div class="table-shell">
      <table>
        <thead>
          <tr>
            <th>Matricula</th>
            <th>Nome</th>
            <th>Nascimento</th>
            <th>Professor</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="aluno in alunos" :key="aluno.id">
            <td>{{ aluno.id }}</td>
            <td>{{ aluno.nome }} {{ aluno.sobrenome }}</td>
            <td>{{ aluno.dataNasc }}</td>
            <td>{{ aluno.professor?.nome || aluno.professorId }}</td>
          </tr>
          <tr v-if="!carregando && !alunos.length">
            <td colspan="4">Nenhum aluno encontrado.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup lang="ts">
import type { Aluno } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const { $api } = useNuxtApp()
const alunos = ref<Aluno[]>([])
const carregando = ref(false)
const erro = ref('')

onMounted(async () => {
  carregando.value = true
  try {
    alunos.value = await $api<Aluno[]>('/aluno')
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    carregando.value = false
  }
})
</script>
