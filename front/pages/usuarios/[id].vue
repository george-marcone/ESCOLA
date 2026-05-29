<template>
  <section class="grid gap-6">
    <div class="page-heading with-action">
      <div class="min-w-0">
        <p class="eyebrow">Usuarios</p>
        <h1>
          {{ tituloPagina }}
        </h1>
      </div>
      <div class="form-actions w-full sm:w-auto">
        <NuxtLink class="btn btn-secondary gap-2" to="/usuarios">
          <ArrowLeft class="h-5 w-5" aria-hidden="true" />
          Voltar
        </NuxtLink>
        <button
          v-if="podeEditar && !editando"
          class="btn btn-primary gap-2"
          type="button"
          @click="editando = true"
        >
          <Pencil class="h-5 w-5" aria-hidden="true" />
          Editar
        </button>
        <button
          v-if="podeExcluir"
          class="btn btn-danger gap-2"
          type="button"
          @click="excluir"
        >
          <Trash2 class="h-5 w-5" aria-hidden="true" />
          Excluir
        </button>
      </div>
    </div>

    <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
    <p v-if="erro" class="alert alert-error">{{ erro }}</p>

    <div v-if="usuario" class="grid gap-5 xl:grid-cols-[320px_minmax(0,1fr)] xl:items-start">
      <aside class="form-panel min-w-0 overflow-hidden">
        <div class="grid min-w-0 gap-4 min-[420px]:flex min-[420px]:items-center">
          <div class="flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-full bg-[#edf3f8] text-xl font-extrabold text-[#071d3b] min-[420px]:h-24 min-[420px]:w-24 min-[420px]:text-2xl">
            <img
              v-if="fotoUsuarioUrl"
              class="h-full w-full object-cover"
              :src="fotoUsuarioUrl"
              :alt="`Foto de ${usuario.nome}`"
            />
            <span v-else>{{ obterIniciais(usuario.nome) }}</span>
          </div>
          <div class="min-w-0">
            <p class="eyebrow">Perfil</p>
            <h2 class="m-0 mt-1 break-words text-xl font-normal text-[#071d3b]">{{ usuario.nome }}</h2>
            <p class="m-0 mt-1 break-words text-sm font-semibold text-[#62728a]">{{ usuario.email }}</p>
            <p class="m-0 mt-1 break-words text-sm font-semibold text-[#62728a]">{{ formatPerfilLabel(usuario.descricaoPerfil) }}</p>
          </div>
        </div>

        <section v-if="podeEnviarFoto" class="grid min-w-0 gap-3 border-t border-[#d4dee9] pt-5">
          <div>
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Foto</p>
            <h3 class="m-0 mt-1 text-base font-extrabold text-[#071d3b]">Foto do perfil</h3>
          </div>
          <label>
            <span>Arquivo de imagem</span>
            <input
              ref="fotoInputRef"
              class="min-h-11 w-full min-w-0 max-w-full"
              type="file"
              accept="image/jpeg,image/png,image/webp"
              @change="selecionarFoto"
            />
          </label>
          <button
            class="btn btn-primary min-h-11 w-full max-w-full gap-2"
            type="button"
            :disabled="!fotoSelecionada || enviandoFoto"
            @click="enviarFoto"
          >
            <Camera class="h-5 w-5" aria-hidden="true" />
            {{ enviandoFoto ? 'Enviando...' : 'Atualizar foto' }}
          </button>
        </section>

        <section v-if="deveExibirCertificados" class="grid min-w-0 gap-3 border-t border-[#d4dee9] pt-5">
          <div>
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Certificados</p>
            <h3 class="m-0 mt-1 text-base font-extrabold text-[#071d3b]">Documentos PDF</h3>
          </div>

          <div v-if="podeEnviarCertificado" class="grid gap-3">
            <label>
              <span>Certificado PDF</span>
              <input
                ref="certificadoInputRef"
                class="min-h-11 w-full min-w-0 max-w-full"
                type="file"
                accept="application/pdf"
                @change="selecionarCertificado"
              />
            </label>
            <button
              class="btn btn-primary min-h-11 w-full max-w-full gap-2"
              type="button"
              :disabled="!certificadoSelecionado || enviandoCertificado"
              @click="enviarCertificado"
            >
              <Upload class="h-5 w-5" aria-hidden="true" />
              {{ enviandoCertificado ? 'Enviando...' : 'Adicionar certificado' }}
            </button>
          </div>

          <div v-if="carregandoArquivos" class="rounded-md border border-[#d4dee9] bg-white p-3 text-sm font-semibold text-[#62728a]">
            Carregando certificados...
          </div>
          <div v-else-if="certificadosUsuario.length" class="grid gap-2">
            <div
              v-for="arquivo in certificadosUsuario"
              :key="obterArquivoId(arquivo)"
              class="flex min-w-0 items-center justify-between gap-3 rounded-md border border-[#d4dee9] bg-white p-3"
            >
              <button
                class="inline-flex min-w-0 items-center gap-2 border-0 bg-transparent p-0 text-left text-sm font-extrabold text-[#071d3b] transition hover:text-[#147f72] disabled:cursor-wait disabled:opacity-70"
                type="button"
                :disabled="arquivoBaixandoId === obterArquivoId(arquivo)"
                @click="baixarArquivo(arquivo)"
              >
                <FileText class="h-5 w-5 shrink-0" aria-hidden="true" />
                <span class="truncate">{{ arquivo.nomeOriginal || 'Certificado' }}</span>
              </button>
              <button
                v-if="podeExcluirArquivo"
                class="inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7] disabled:cursor-not-allowed disabled:opacity-70"
                type="button"
                title="Excluir certificado"
                aria-label="Excluir certificado"
                :disabled="arquivoExcluindoId === obterArquivoId(arquivo)"
                @click="excluirArquivo(arquivo)"
              >
                <Trash2 class="h-5 w-5" aria-hidden="true" />
              </button>
            </div>
          </div>
          <p v-else class="m-0 rounded-md border border-[#d4dee9] bg-white p-3 text-sm font-semibold text-[#62728a]">
            Nenhum certificado cadastrado.
          </p>
        </section>

        <p v-if="mensagemArquivos" class="alert alert-success">{{ mensagemArquivos }}</p>
        <p v-if="erroArquivos" class="alert alert-error">{{ erroArquivos }}</p>
      </aside>

      <form class="form-panel min-w-0" @submit.prevent="salvar">
        <div class="form-panel-header">
          <div>
            <p class="eyebrow">Cadastro</p>
            <strong>Dados do usuario</strong>
          </div>
          <span>{{ editando ? 'Edicao liberada' : 'Somente leitura' }}</span>
        </div>

        <div class="form-grid two-columns">
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
            :disabled="!editando"
          />

          <label>
            <span>Nome da mae</span>
            <input v-model.trim="form.nomeMae" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" :disabled="!editando" autocomplete="off" />
            <span class="text-xs font-extrabold text-slate-500">{{ form.nomeMae.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
          </label>

          <label>
            <span>Nome do pai</span>
            <input v-model.trim="form.nomePai" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" :disabled="!editando" autocomplete="off" />
            <span class="text-xs font-extrabold text-slate-500">{{ form.nomePai.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
          </label>

          <label class="md:col-span-2">
            <span>Endereco</span>
            <input v-model.trim="form.endereco" type="text" :maxlength="ADDRESS_FIELD_MAX_LENGTH" :disabled="!editando" autocomplete="street-address" />
            <span class="text-xs font-extrabold text-slate-500">{{ form.endereco.length }}/{{ ADDRESS_FIELD_MAX_LENGTH }}</span>
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

        <div v-if="editando" class="form-actions">
          <button class="btn btn-secondary gap-2" type="button" @click="cancelar">
            <XCircle class="h-5 w-5" aria-hidden="true" />
            Cancelar
          </button>
          <button class="btn btn-primary gap-2" type="submit" :disabled="salvando">
            <Save class="h-5 w-5" aria-hidden="true" />
            {{ salvando ? 'Salvando...' : 'Salvar alteracoes' }}
          </button>
        </div>
      </form>
    </div>

    <p v-else-if="!erro" class="alert alert-warning">Carregando usuario...</p>
  </section>
</template>

<script setup lang="ts">
import { ArrowLeft, Camera, FileText, Pencil, Save, Trash2, Upload, XCircle } from '@lucide/vue'
import type { Perfil, UsuarioArquivo, UsuarioForm, UsuarioSummary, UsuarioUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import { downloadBlob, fetchApiBlob } from '~/utils/api-file'
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
  canChangeUsuarioPerfil,
  canDeleteUsuario,
  canEditUsuario,
  canViewUsuarioInList,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getUsuarioPerfilTipo,
  getTipoUsuarioForApiByPerfilId
} from '~/utils/usuario-permissions'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: []
})

const { $api } = useNuxtApp()
const config = useRuntimeConfig()
const route = useRoute()
const auth = useAuthStore()
const usuario = ref<UsuarioSummary | null>(null)
const perfis = ref<Perfil[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const arquivosUsuario = ref<UsuarioArquivo[]>([])
const editando = ref(false)
const salvando = ref(false)
const carregandoArquivos = ref(false)
const enviandoFoto = ref(false)
const enviandoCertificado = ref(false)
const arquivoExcluindoId = ref(0)
const arquivoBaixandoId = ref(0)
const erro = ref('')
const mensagem = ref('')
const erroArquivos = ref('')
const mensagemArquivos = ref('')
const fotoInputRef = ref<HTMLInputElement | null>(null)
const certificadoInputRef = ref<HTMLInputElement | null>(null)
const fotoSelecionada = ref<File | null>(null)
const certificadoSelecionado = ref<File | null>(null)
const fotoPreviewUrl = ref('')
const fotoArquivoUrl = ref('')
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

const usuarioId = computed(() => Number(route.params.id))
const podeEditar = computed(() => usuario.value ? canEditUsuario(auth.usuario, usuario.value) : false)
const podeExcluir = computed(() => usuario.value ? canDeleteUsuario(auth.usuario) : false)
const fotoUsuarioUrl = computed(() =>
  fotoPreviewUrl.value || fotoArquivoUrl.value
)
const certificadosUsuario = computed(() =>
  arquivosUsuario.value.filter((arquivo) => arquivo.tipoArquivo?.toLowerCase() === 'certificado')
)
const usuarioEhProfessor = computed(() =>
  getUsuarioPerfilTipo(usuario.value?.descricaoPerfil) === 'professor'
)
const podeConsultarArquivos = computed(() =>
  Boolean(usuario.value && (auth.isAdmin || auth.usuario?.idUsuario === usuario.value.idUsuario || (auth.isProfessor && usuarioEhProfessor.value)))
)
const podeEnviarFoto = computed(() =>
  usuario.value ? editando.value && canEditUsuario(auth.usuario, usuario.value) : false
)
const podeEnviarCertificado = computed(() =>
  Boolean(editando.value && usuario.value && usuarioEhProfessor.value && (auth.isAdmin || auth.usuario?.idUsuario === usuario.value.idUsuario))
)
const podeExcluirArquivo = computed(() => podeEnviarCertificado.value)
const deveExibirCertificados = computed(() =>
  podeConsultarArquivos.value && (usuarioEhProfessor.value || certificadosUsuario.value.length > 0 || carregandoArquivos.value)
)
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

onBeforeUnmount(() => {
  limparFotoPreview()
  limparFotoArquivo()
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
    return
  }

  await carregarFotoUsuario()

  if (podeConsultarArquivos.value) {
    await carregarArquivos()
  } else {
    resetarArquivosUsuario()
  }
}

async function carregarArquivos() {
  if (!podeConsultarArquivos.value) {
    resetarArquivosUsuario()
    return
  }

  carregandoArquivos.value = true
  erroArquivos.value = ''

  try {
    arquivosUsuario.value = await $api<UsuarioArquivo[]>(`/usuarios/${usuarioId.value}/arquivos`)
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    carregandoArquivos.value = false
  }
}

function resetarArquivosUsuario() {
  arquivosUsuario.value = []
  fotoSelecionada.value = null
  certificadoSelecionado.value = null
  mensagemArquivos.value = ''
  erroArquivos.value = ''
  limparFotoPreview()
  limparInputArquivo(fotoInputRef.value)
  limparInputArquivo(certificadoInputRef.value)
}

function preencherForm(value: UsuarioSummary) {
  form.nome = value.nome
  form.email = value.email
  form.telefone = formatBrazilPhone(value.telefone)
  form.dataNascimento = value.dataNascimento?.slice(0, 10) ?? ''
  form.nomeMae = value.nomeMae ?? ''
  form.nomePai = value.nomePai ?? ''
  form.endereco = value.endereco ?? ''
  form.idPerfil = value.idPerfil
}

function cancelar() {
  if (usuario.value) {
    preencherForm(usuario.value)
  }

  editando.value = false
}

function selecionarFoto(event: Event) {
  const arquivo = obterArquivoSelecionado(event)
  fotoSelecionada.value = arquivo
  atualizarFotoPreview(arquivo)
  mensagemArquivos.value = ''
  erroArquivos.value = ''
}

function selecionarCertificado(event: Event) {
  certificadoSelecionado.value = obterArquivoSelecionado(event)
  mensagemArquivos.value = ''
  erroArquivos.value = ''
}

function obterArquivoSelecionado(event: Event) {
  const input = event.target as HTMLInputElement
  return input.files?.[0] ?? null
}

function limparInputArquivo(input: HTMLInputElement | null) {
  if (input) {
    input.value = ''
  }
}

function atualizarFotoPreview(arquivo: File | null) {
  limparFotoPreview()

  if (arquivo?.type.startsWith('image/')) {
    fotoPreviewUrl.value = URL.createObjectURL(arquivo)
  }
}

function limparFotoPreview() {
  if (fotoPreviewUrl.value) {
    URL.revokeObjectURL(fotoPreviewUrl.value)
    fotoPreviewUrl.value = ''
  }
}

function limparFotoArquivo() {
  if (fotoArquivoUrl.value) {
    URL.revokeObjectURL(fotoArquivoUrl.value)
    fotoArquivoUrl.value = ''
  }
}

async function carregarFotoUsuario() {
  limparFotoArquivo()

  if (!usuario.value?.fotoPerfilUrl) {
    return
  }

  try {
    const blob = await fetchApiBlob(`/usuarios/${usuarioId.value}/foto`, config.public.apiBase, auth.token)
    fotoArquivoUrl.value = URL.createObjectURL(blob)
  } catch {
    fotoArquivoUrl.value = ''
  }
}

async function enviarFoto() {
  if (!usuario.value || !podeEnviarFoto.value || !fotoSelecionada.value) {
    erroArquivos.value = 'Selecione uma foto para enviar.'
    return
  }

  enviandoFoto.value = true
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    const formData = new FormData()
    formData.append('arquivo', fotoSelecionada.value)

    usuario.value = await $api<UsuarioSummary>(`/usuarios/${usuarioId.value}/foto`, {
      method: 'POST',
      body: formData
    })

    if (usuario.value.idUsuario === auth.usuario?.idUsuario) {
      auth.updateUsuario(usuario.value)
    }

    preencherForm(usuario.value)
    fotoSelecionada.value = null
    limparFotoPreview()
    limparInputArquivo(fotoInputRef.value)
    await carregarFotoUsuario()
    mensagemArquivos.value = 'Foto atualizada.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    enviandoFoto.value = false
  }
}

async function baixarArquivo(arquivo: UsuarioArquivo) {
  const arquivoId = obterArquivoId(arquivo)
  if (!arquivoId) {
    erroArquivos.value = 'Arquivo sem identificador para download.'
    return
  }

  arquivoBaixandoId.value = arquivoId
  erroArquivos.value = ''

  try {
    const blob = await fetchApiBlob(`/usuarios/${usuarioId.value}/arquivos/${arquivoId}/download`, config.public.apiBase, auth.token)
    downloadBlob(blob, arquivo.nomeOriginal || 'certificado.pdf')
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    arquivoBaixandoId.value = 0
  }
}

async function enviarCertificado() {
  if (!usuario.value || !podeEnviarCertificado.value || !certificadoSelecionado.value) {
    erroArquivos.value = 'Selecione um PDF para enviar.'
    return
  }

  enviandoCertificado.value = true
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    const formData = new FormData()
    formData.append('arquivo', certificadoSelecionado.value)

    await $api<UsuarioArquivo>(`/usuarios/${usuarioId.value}/certificados`, {
      method: 'POST',
      body: formData
    })

    certificadoSelecionado.value = null
    limparInputArquivo(certificadoInputRef.value)
    await carregarArquivos()
    mensagemArquivos.value = 'Certificado adicionado.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    enviandoCertificado.value = false
  }
}

async function excluirArquivo(arquivo: UsuarioArquivo) {
  if (!usuario.value || !podeExcluirArquivo.value) {
    return
  }

  const arquivoId = obterArquivoId(arquivo)
  if (!arquivoId) {
    erroArquivos.value = 'Arquivo sem identificador para exclusao.'
    return
  }

  if (!confirm(`Excluir o arquivo ${arquivo.nomeOriginal || 'selecionado'}?`)) {
    return
  }

  arquivoExcluindoId.value = arquivoId
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    await $api(`/usuarios/${usuarioId.value}/arquivos/${arquivoId}`, {
      method: 'DELETE'
    })
    await carregarArquivos()
    mensagemArquivos.value = 'Arquivo excluido.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    arquivoExcluindoId.value = 0
  }
}

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

function montarPayload(): UsuarioUpdate {
  const payload: UsuarioUpdate = {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    dataNascimento: form.dataNascimento || null,
    nomeMae: form.nomeMae.trim() || null,
    nomePai: form.nomePai.trim() || null,
    endereco: form.endereco.trim() || null
  }

  if (canChangeUsuarioPerfil(auth.usuario)) {
    payload.tipoUsuario = getTipoUsuarioForApiByPerfilId(perfisFormulario.value, form.idPerfil)
  }

  return payload
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

  if (!form.idPerfil || (canChangeUsuarioPerfil(auth.usuario) && !getTipoUsuarioForApiByPerfilId(perfisFormulario.value, form.idPerfil))) {
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
    if (podeConsultarArquivos.value) {
      await carregarArquivos()
    } else {
      resetarArquivosUsuario()
    }
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

function obterArquivoId(arquivo: UsuarioArquivo) {
  return arquivo.idArquivo ?? arquivo.idUsuarioArquivo ?? 0
}

function obterIniciais(nome?: string | null) {
  const partes = (nome ?? '')
    .trim()
    .split(/\s+/)
    .filter(Boolean)

  if (!partes.length) return 'U'

  return partes
    .slice(0, 2)
    .map((parte) => parte[0]?.toUpperCase() ?? '')
    .join('')
}
</script>
