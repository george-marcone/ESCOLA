<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Diretoria</p>
      <h1>Nova diretoria</h1>
    </div>

    <form class="form-panel" @submit.prevent="salvar">
      <div class="form-grid two-columns">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required maxlength="100" />
        </label>

        <label>
          <span>Usuario vinculado</span>
          <select v-model="idUsuario">
            <option value="">Sem vinculo</option>
            <option v-for="usuario in usuarios.usuarios" :key="usuario.idUsuario" :value="usuario.idUsuario">
              {{ usuario.nome }} - {{ usuario.descricaoPerfil }}
            </option>
          </select>
        </label>
      </div>

      <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="form-actions">
        <NuxtLink class="btn btn-secondary" to="/diretoria">Cancelar</NuxtLink>
        <button class="btn btn-primary" type="submit" :disabled="diretoria.loading">
          {{ diretoria.loading ? 'Salvando...' : 'Salvar diretoria' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const diretoria = useDiretoriaStore()
const usuarios = useUsuariosStore()
const mensagem = ref('')
const erro = ref('')
const idUsuario = ref('')
const form = reactive({
  nome: ''
})

onMounted(() => {
  usuarios.fetchUsuarios().catch((err) => {
    erro.value = normalizeApiError(err)
  })
})

async function salvar() {
  mensagem.value = ''
  erro.value = ''

  try {
    const created = await diretoria.createDiretoria({
      nome: form.nome,
      idUsuario: idUsuario.value ? Number(idUsuario.value) : null
    })

    mensagem.value = `Diretoria ${created.nome} cadastrada.`
    form.nome = ''
    idUsuario.value = ''
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
