<template>
  <section class="grid gap-6">
    <div>
      <p class="eyebrow">Usuarios</p>
      <h1 class="m-0 text-3xl font-extrabold text-slate-900">{{ auth.isProfessor ? 'Novo aluno' : 'Novo usuario' }}</h1>
    </div>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-5" @submit.prevent="salvar">
      <div class="grid gap-4 md:grid-cols-2">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.nome.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Email</span>
          <input v-model.trim="form.email" type="email" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.email.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Telefone</span>
          <input
            :value="form.telefone"
            type="tel"
            required
            inputmode="numeric"
            autocomplete="tel"
            :maxlength="BRAZIL_PHONE_MASK_MAX_LENGTH"
            :placeholder="BRAZIL_PHONE_PLACEHOLDER"
            @input="atualizarTelefone"
          />
          <span class="text-xs font-extrabold text-slate-500">{{ form.telefone.length }}/{{ BRAZIL_PHONE_MASK_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Tipo de usuario</span>
          <select v-model.number="form.idPerfil" required :disabled="perfisDisponiveis.length <= 1">
            <option disabled :value="0">Selecione</option>
            <option v-for="perfil in perfisDisponiveis" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ formatPerfilLabel(perfil.descricaoPerfil) }}
            </option>
          </select>
        </label>

        <div class="rounded-md border border-[#d7e8ff] bg-[#eff6ff] p-4 text-sm font-semibold text-[#24446d] md:col-span-2">
          A senha inicial sera definida automaticamente como <strong>Senha@252525</strong>.
        </div>
      </div>

      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="flex flex-wrap justify-end gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/usuarios">
          Cancelar
        </NuxtLink>
        <button class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : auth.isProfessor ? 'Salvar aluno' : 'Salvar usuario' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Perfil, UsuarioCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import {
  BRAZIL_PHONE_MASK_MAX_LENGTH,
  BRAZIL_PHONE_PLACEHOLDER,
  formatBrazilPhone,
  isCompleteBrazilPhone,
  normalizeBrazilPhoneForApi
} from '~/utils/br-phone'
import {
  canCreateAlunoUsuarios,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getDefaultPerfilId
} from '~/utils/usuario-permissions'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: []
})

const { $api } = useNuxtApp()
const auth = useAuthStore()
const perfis = ref<Perfil[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const salvando = ref(false)
const erro = ref('')
const USER_TEXT_FIELD_MAX_LENGTH = 50
const PHONE_FORMAT_ERROR = 'Informe um telefone valido no formato +55 (xx) xxxxx-xxxx.'
const REQUIRED_FIELDS_ERROR = 'Nome, e-mail e telefone sao obrigatorios.'
const REQUIRED_PROFILE_ERROR = 'Informe o tipo de usuario.'
const form = reactive<UsuarioCreate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})
const podeCadastrar = computed(() => canCreateAlunoUsuarios(auth.perfil))
const perfisDisponiveis = computed(() => filterPerfisForUsuarioCreation(perfis.value, auth.usuario))

onMounted(async () => {
  if (!podeCadastrar.value) {
    await navigateTo('/usuarios', { replace: true })
    return
  }

  try {
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
    form.idPerfil = getDefaultPerfilId(perfis.value, auth.usuario)

    if (!perfisDisponiveis.value.length) {
      erro.value = 'Nenhum tipo de usuario permitido foi retornado pela API.'
    }
  } catch (err) {
    erro.value = normalizeApiError(err)
    return
  }

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch {
    usuarios.value = auth.usuario ? [auth.usuario] : []
  }
})

function atualizarTelefone(event: Event) {
  const input = event.target as HTMLInputElement
  form.telefone = formatBrazilPhone(input.value)
}

function montarPayload(): UsuarioCreate {
  return {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    idPerfil: form.idPerfil
  }
}

async function salvar() {
  erro.value = ''

  if (!podeCadastrar.value) {
    await navigateTo('/usuarios', { replace: true })
    return
  }

  if (!form.nome.trim() || !form.email.trim() || !form.telefone.trim()) {
    erro.value = REQUIRED_FIELDS_ERROR
    return
  }

  if (!isCompleteBrazilPhone(form.telefone)) {
    erro.value = PHONE_FORMAT_ERROR
    return
  }

  if (!form.idPerfil) {
    erro.value = REQUIRED_PROFILE_ERROR
    return
  }

  if (isDuplicateUserEmail(usuarios.value, form.email)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    const created = await $api<UsuarioSummary>('/usuarios', {
      method: 'POST',
      body: montarPayload()
    })
    await navigateTo(`/usuarios/${created.idUsuario}`)
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}
</script>
