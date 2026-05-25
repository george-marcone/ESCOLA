<template>
  <section class="grid gap-6">
    <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
      <div>
        <p class="eyebrow">Usuarios</p>
        <h1 class="m-0 text-3xl font-extrabold text-slate-900">
          {{ tituloPagina }}
        </h1>
      </div>
      <div class="flex flex-wrap gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/usuarios">
          Voltar
        </NuxtLink>
        <button
          v-if="podeEditar && !editando"
          class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]"
          type="button"
          @click="editando = true"
        >
          Editar
        </button>
        <button
          v-if="podeExcluir"
          class="rounded-md border border-red-200 px-4 py-2 text-sm font-bold text-red-700 hover:bg-red-50"
          type="button"
          @click="excluir"
        >
          Excluir
        </button>
      </div>
    </div>

    <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
    <p v-if="erro" class="alert alert-error">{{ erro }}</p>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-5" @submit.prevent="salvar">
      <div class="grid gap-4 md:grid-cols-2">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" :disabled="!editando" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.nome.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Email</span>
          <input v-model.trim="form.email" type="email" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" :disabled="!editando" />
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
            :disabled="!editando"
            @input="atualizarTelefone"
          />
          <span class="text-xs font-extrabold text-slate-500">{{ form.telefone.length }}/{{ BRAZIL_PHONE_MASK_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Tipo de usuario</span>
          <select v-model.number="form.idPerfil" required :disabled="!podeAlterarPerfil">
            <option v-if="!perfisFormulario.length" :value="form.idPerfil">{{ usuario?.descricaoPerfil || 'Perfil atual' }}</option>
            <option v-for="perfil in perfisFormulario" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ formatPerfilLabel(perfil.descricaoPerfil) }}
            </option>
          </select>
        </label>
      </div>

      <div v-if="editando" class="flex flex-wrap justify-end gap-2">
        <button class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold hover:bg-slate-100" type="button" @click="cancelar">
          Cancelar
        </button>
        <button class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar alteracoes' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Perfil, UsuarioSummary, UsuarioUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import {
  BRAZIL_PHONE_MASK_MAX_LENGTH,
  BRAZIL_PHONE_PLACEHOLDER,
  formatBrazilPhone,
  isCompleteBrazilPhone,
  normalizeBrazilPhoneForApi
} from '~/utils/br-phone'
import {
  canChangeUsuarioPerfil,
  canDeleteUsuario,
  canEditUsuario,
  canViewUsuarioInList,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel
} from '~/utils/usuario-permissions'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: []
})

const { $api } = useNuxtApp()
const route = useRoute()
const auth = useAuthStore()
const usuario = ref<UsuarioSummary | null>(null)
const perfis = ref<Perfil[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const editando = ref(false)
const salvando = ref(false)
const erro = ref('')
const mensagem = ref('')
const USER_TEXT_FIELD_MAX_LENGTH = 50
const PHONE_FORMAT_ERROR = 'Informe um telefone valido no formato +55 (xx) xxxxx-xxxx.'
const REQUIRED_FIELDS_ERROR = 'Nome, e-mail e telefone sao obrigatorios.'
const REQUIRED_PROFILE_ERROR = 'Informe o tipo de usuario.'
const form = reactive<UsuarioUpdate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})

const usuarioId = computed(() => Number(route.params.id))
const podeEditar = computed(() => usuario.value ? canEditUsuario(auth.usuario, usuario.value) : false)
const podeExcluir = computed(() => usuario.value ? canDeleteUsuario(auth.usuario) : false)
const perfisFormulario = computed<Perfil[]>(() => {
  if (!usuario.value) return []

  if (!canChangeUsuarioPerfil(auth.usuario)) {
    return [{ idPerfil: usuario.value.idPerfil, descricaoPerfil: usuario.value.descricaoPerfil }]
  }

  return filterPerfisForUsuarioCreation(perfis.value, auth.usuario)
})
const podeAlterarPerfil = computed(() =>
  editando.value && canChangeUsuarioPerfil(auth.usuario) && perfisFormulario.value.length > 1
)
const tituloPagina = computed(() => {
  if (editando.value) return 'Editar usuario'
  if (usuario.value?.idUsuario === auth.usuario?.idUsuario) return 'Meu cadastro'

  return 'Visualizar usuario'
})

onMounted(async () => {
  await carregar()

  if (canChangeUsuarioPerfil(auth.usuario)) {
    try {
      perfis.value = await $api<Perfil[]>('/usuarios/perfis')
    } catch (err) {
      erro.value = normalizeApiError(err)
    }
  }

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch {
    usuarios.value = usuario.value ? [usuario.value] : []
  }
})

async function carregar() {
  erro.value = ''

  try {
    usuario.value = await $api<UsuarioSummary>(`/usuarios/${usuarioId.value}`)
    preencherForm(usuario.value)
  } catch (err) {
    if (auth.usuario?.idUsuario === usuarioId.value) {
      usuario.value = auth.usuario
      preencherForm(auth.usuario)
    } else {
      erro.value = normalizeApiError(err)
      return
    }
  }

  if (usuario.value && !canViewUsuarioInList(auth.usuario, usuario.value)) {
    await navigateTo('/usuarios', { replace: true })
  }
}

function preencherForm(value: UsuarioSummary) {
  form.nome = value.nome
  form.email = value.email
  form.telefone = formatBrazilPhone(value.telefone)
  form.idPerfil = value.idPerfil
}

function cancelar() {
  if (usuario.value) {
    preencherForm(usuario.value)
  }

  editando.value = false
}

function atualizarTelefone(event: Event) {
  const input = event.target as HTMLInputElement
  form.telefone = formatBrazilPhone(input.value)
}

function montarPayload(): UsuarioUpdate {
  return {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    idPerfil: form.idPerfil
  }
}

function validarFormulario() {
  if (!form.nome.trim() || !form.email.trim() || !form.telefone.trim()) {
    erro.value = REQUIRED_FIELDS_ERROR
    return false
  }

  if (!isCompleteBrazilPhone(form.telefone)) {
    erro.value = PHONE_FORMAT_ERROR
    return false
  }

  if (!form.idPerfil) {
    erro.value = REQUIRED_PROFILE_ERROR
    return false
  }

  return true
}

async function salvar() {
  erro.value = ''
  mensagem.value = ''

  if (!usuario.value || !podeEditar.value || !validarFormulario()) return

  if (isDuplicateUserEmail(usuarios.value, form.email, usuarioId.value)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    usuario.value = await $api<UsuarioSummary>(`/usuarios/${usuarioId.value}`, {
      method: 'PUT',
      body: montarPayload()
    })
    if (usuario.value.idUsuario === auth.usuario?.idUsuario) {
      auth.updateUsuario(usuario.value)
    }
    preencherForm(usuario.value)
    mensagem.value = 'Usuario atualizado.'
    editando.value = false
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir() {
  if (!usuario.value || !podeExcluir.value || !confirm(`Excluir o usuario ${usuario.value.nome}?`)) {
    return
  }

  erro.value = ''

  try {
    await $api(`/usuarios/${usuarioId.value}`, { method: 'DELETE' })
    await navigateTo('/usuarios')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
