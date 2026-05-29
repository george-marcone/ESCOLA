<template>
  <section class="grid gap-6">
    <div>
      <p class="eyebrow">Usuarios</p>
      <h1 class="m-0 break-words text-[1.75rem] font-extrabold leading-tight text-slate-900 sm:text-3xl">Novo usuario</h1>
    </div>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-4 sm:p-5" @submit.prevent="salvar">
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
            @beforeinput="impedirEntradaTelefoneNaoNumerica"
            @input="atualizarTelefone"
            @keydown="impedirTeclaTelefoneNaoNumerica"
            @paste="colarTelefone"
            @drop.prevent
          />
          <span class="text-xs font-extrabold text-slate-500">{{ form.telefone.length }}/{{ BRAZIL_PHONE_MASK_MAX_LENGTH }}</span>
        </label>

        <DatePicker
          v-model="form.dataNascimento"
          label="Data de aniversario"
          hint="Digite no formato dd/mm/aaaa ou selecione no calendario."
        />

        <label>
          <span>Nome da mae</span>
          <input v-model.trim="form.nomeMae" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" autocomplete="off" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.nomeMae.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label>
          <span>Nome do pai</span>
          <input v-model.trim="form.nomePai" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" autocomplete="off" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.nomePai.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="md:col-span-2">
          <span>Endereco</span>
          <input v-model.trim="form.endereco" type="text" :maxlength="ADDRESS_FIELD_MAX_LENGTH" autocomplete="street-address" />
          <span class="text-xs font-extrabold text-slate-500">{{ form.endereco.length }}/{{ ADDRESS_FIELD_MAX_LENGTH }}</span>
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

      <div class="grid gap-2 sm:flex sm:flex-wrap sm:justify-end">
        <NuxtLink class="inline-flex min-h-11 items-center justify-center rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/usuarios">
          Cancelar
        </NuxtLink>
        <button class="inline-flex min-h-11 items-center justify-center rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar usuario' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Perfil, UsuarioCreate, UsuarioForm, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import {
  BRAZIL_PHONE_MASK_MAX_LENGTH,
  BRAZIL_PHONE_PLACEHOLDER,
  formatBrazilPhone,
  isCompleteBrazilPhone,
  normalizeBrazilPhoneForApi,
  shouldBlockNonNumericPhoneInput,
  shouldBlockNonNumericPhoneKey
} from '~/utils/br-phone'
import {
  canCreateAlunoUsuarios,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getDefaultPerfilId,
  getTipoUsuarioForApiByPerfilId
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
const USER_TEXT_FIELD_MAX_LENGTH = 100
const ADDRESS_FIELD_MAX_LENGTH = 200
const PHONE_FORMAT_ERROR = 'Informe um telefone valido no formato +55 (xx) xxxxx-xxxx.'
const REQUIRED_FIELDS_ERROR = 'Nome, e-mail e telefone sao obrigatorios.'
const REQUIRED_PROFILE_ERROR = 'Informe o tipo de usuario.'
const form = reactive<UsuarioForm>({
  nome: '',
  email: '',
  telefone: '',
  dataNascimento: '',
  nomeMae: '',
  nomePai: '',
  endereco: '',
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
  const telefone = formatBrazilPhone(input.value)

  form.telefone = telefone
  input.value = telefone
}

function impedirEntradaTelefoneNaoNumerica(event: InputEvent) {
  if (shouldBlockNonNumericPhoneInput(event)) {
    event.preventDefault()
  }
}

function impedirTeclaTelefoneNaoNumerica(event: KeyboardEvent) {
  if (shouldBlockNonNumericPhoneKey(event)) {
    event.preventDefault()
  }
}

function colarTelefone(event: ClipboardEvent) {
  event.preventDefault()
  const input = event.target as HTMLInputElement
  const telefone = formatBrazilPhone(event.clipboardData?.getData('text') ?? '')

  form.telefone = telefone
  input.value = telefone
}

function montarPayload(): UsuarioCreate {
  return {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    dataNascimento: form.dataNascimento || null,
    nomeMae: form.nomeMae.trim() || null,
    nomePai: form.nomePai.trim() || null,
    endereco: form.endereco.trim() || null,
    tipoUsuario: getTipoUsuarioForApiByPerfilId(perfisDisponiveis.value, form.idPerfil)
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

  if (!form.idPerfil || !getTipoUsuarioForApiByPerfilId(perfisDisponiveis.value, form.idPerfil)) {
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
