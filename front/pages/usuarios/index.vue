<template>
  <section class="grid gap-5 xl:grid-cols-[minmax(280px,360px)_minmax(0,1fr)]">
    <form
      v-if="exibeFormulario"
      class="min-w-0 overflow-hidden rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
      @submit.prevent="salvar"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ editandoId ? 'Edicao' : 'Cadastro' }}</p>
      <h2 class="mb-8 mt-2 text-xl font-normal text-[#071d3b]">
        {{ tituloFormulario }}
      </h2>

      <div class="grid gap-5">
        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome</span>
          <input v-model.trim="form.nome" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nome.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>E-mail</span>
          <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" required :maxlength="USER_TEXT_FIELD_MAX_LENGTH" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.email.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Telefone</span>
          <input
            :value="form.telefone"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
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
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.telefone.length }}/{{ BRAZIL_PHONE_MASK_MAX_LENGTH }}</span>
        </label>

        <DatePicker
          v-model="form.dataNascimento"
          label="Data de aniversario"
          hint="Digite no formato dd/mm/aaaa ou selecione no calendario."
        />

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome da mae</span>
          <input v-model.trim="form.nomeMae" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" autocomplete="off" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nomeMae.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Nome do pai</span>
          <input v-model.trim="form.nomePai" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" :maxlength="USER_TEXT_FIELD_MAX_LENGTH" autocomplete="off" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.nomePai.length }}/{{ USER_TEXT_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Endereco</span>
          <input v-model.trim="form.endereco" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" :maxlength="ADDRESS_FIELD_MAX_LENGTH" autocomplete="street-address" />
          <span class="text-xs font-extrabold text-[#62728a]">{{ form.endereco.length }}/{{ ADDRESS_FIELD_MAX_LENGTH }}</span>
        </label>

        <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
          <span>Tipo de usuario</span>
          <select
            v-model.number="form.idPerfil"
            class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
            required
            :disabled="!podeAlterarPerfilNoFormulario"
          >
            <option disabled :value="0">Selecione</option>
            <option v-for="perfil in perfisFormulario" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ formatPerfilLabel(perfil.descricaoPerfil) }}
            </option>
          </select>
        </label>

        <div v-if="editandoId" class="grid min-w-0 gap-4 overflow-hidden rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-4">
          <div class="flex items-center gap-3">
            <div class="flex h-16 w-16 shrink-0 items-center justify-center overflow-hidden rounded-full bg-[#edf3f8] text-lg font-extrabold text-[#071d3b]">
              <img
                v-if="fotoUsuarioEmEdicao"
                class="h-full w-full object-cover"
                :src="fotoUsuarioEmEdicao"
                :alt="`Foto de ${usuarioEmEdicao?.nome}`"
              />
              <span v-else>{{ obterIniciais(usuarioEmEdicao?.nome) }}</span>
            </div>
            <div class="min-w-0">
              <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Perfil</p>
              <p class="m-0 mt-1 break-words text-sm font-extrabold text-[#071d3b]">{{ usuarioEmEdicao?.nome }}</p>
            </div>
          </div>

          <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
            <span>Foto do perfil</span>
            <input
              ref="fotoInputRef"
              class="min-h-11 w-full min-w-0 max-w-full rounded-md border border-[#ccd8e5] bg-white px-3 py-2 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
              type="file"
              accept="image/jpeg,image/png,image/webp"
              @change="selecionarFoto"
            />
          </label>
          <button
            class="inline-flex min-h-11 w-full max-w-full items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-70"
            type="button"
            :disabled="!fotoSelecionada || enviandoFoto"
            @click="enviarFoto"
          >
            <Camera class="h-5 w-5" aria-hidden="true" />
            {{ enviandoFoto ? 'Enviando...' : 'Atualizar foto' }}
          </button>

          <div v-if="podeEnviarCertificado" class="grid gap-3 border-t border-[#d4dee9] pt-4">
            <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Certificado PDF</span>
              <input
                ref="certificadoInputRef"
                class="min-h-11 w-full min-w-0 max-w-full rounded-md border border-[#ccd8e5] bg-white px-3 py-2 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                type="file"
                accept="application/pdf"
                @change="selecionarCertificado"
              />
            </label>
            <button
              class="inline-flex min-h-11 w-full max-w-full items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-70"
              type="button"
              :disabled="!certificadoSelecionado || enviandoCertificado"
              @click="enviarCertificado"
            >
              <Upload class="h-5 w-5" aria-hidden="true" />
              {{ enviandoCertificado ? 'Enviando...' : 'Adicionar certificado' }}
            </button>

            <div v-if="certificadosUsuario.length" class="grid gap-2">
              <div
                v-for="arquivo in certificadosUsuario"
                :key="obterArquivoId(arquivo)"
                class="flex min-w-0 items-center justify-between gap-3 rounded-md border border-[#d4dee9] bg-white p-3"
              >
                <button
                  class="inline-flex min-w-0 items-center gap-2 border-0 bg-transparent p-0 text-left text-sm font-extrabold text-[#071d3b] transition hover:text-[#147f72] disabled:cursor-wait disabled:opacity-70"
                  type="button"
                  :disabled="arquivoBaixandoId === obterArquivoId(arquivo)"
                  @click="baixarArquivoUsuario(editandoId, arquivo)"
                >
                  <FileText class="h-5 w-5 shrink-0" aria-hidden="true" />
                  <span class="truncate">{{ arquivo.nomeOriginal }}</span>
                </button>
                <button
                  class="inline-flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                  type="button"
                  title="Excluir certificado"
                  aria-label="Excluir certificado"
                  @click="excluirArquivo(arquivo)"
                >
                  <Trash2 class="h-5 w-5" aria-hidden="true" />
                </button>
              </div>
            </div>
            <p v-else-if="!carregandoArquivos" class="m-0 text-sm font-semibold text-[#62728a]">
              Nenhum certificado cadastrado.
            </p>
          </div>

          <div v-if="podeEnviarNotificacao" class="grid gap-3 border-t border-[#d4dee9] pt-4">
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Notificacao</p>
            <p class="m-0 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-sm font-semibold text-[#51627a]">
              Destinatarios: alunos e professores.
            </p>
            <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Titulo</span>
              <input v-model.trim="notificacaoForm.titulo" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="text" maxlength="120" />
            </label>
            <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
              <span>Mensagem</span>
              <textarea
                v-model.trim="notificacaoForm.mensagem"
                class="min-h-28 resize-y rounded-md border border-[#ccd8e5] px-3 py-2 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
                maxlength="2000"
              />
            </label>
            <button
              class="inline-flex min-h-11 w-full max-w-full items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-70"
              type="button"
              :disabled="enviandoNotificacao"
              @click="enviarNotificacao"
            >
              <Send class="h-5 w-5" aria-hidden="true" />
              {{ enviandoNotificacao ? 'Enviando...' : 'Enviar notificacao' }}
            </button>
          </div>

          <p v-if="mensagemArquivos" class="alert alert-success">{{ mensagemArquivos }}</p>
          <p v-if="erroArquivos" class="alert alert-error">{{ erroArquivos }}</p>
        </div>

        <p v-if="!editandoId" class="m-0 rounded-md border border-[#d7e8ff] bg-[#eff6ff] p-3 text-sm font-semibold text-[#24446d]">
          A senha inicial sera definida automaticamente como <strong>Senha@252525</strong>.
        </p>

        <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
        <p v-if="erro" class="alert alert-error">{{ erro }}</p>

        <div class="grid gap-2">
          <button
            class="inline-flex min-h-12 w-full max-w-full items-center justify-center gap-2 rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-not-allowed disabled:opacity-70"
            type="submit"
            :disabled="salvando"
          >
            <Plus class="h-5 w-5" aria-hidden="true" />
            {{ textoBotaoSalvar }}
          </button>
          <button
            v-if="editandoId"
            class="inline-flex min-h-10 w-full max-w-full items-center justify-center rounded-md border border-[#d4dee9] bg-white px-4 text-sm font-extrabold text-[#51627a] transition hover:bg-[#edf3f8]"
            type="button"
            @click="limparForm"
          >
            Cancelar edicao
          </button>
        </div>
      </div>
    </form>

    <aside
      v-else
      class="rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6"
    >
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Consulta</p>
      <h2 class="mb-3 mt-2 text-xl font-normal text-[#071d3b]">Usuarios</h2>
      <p class="m-0 text-sm font-semibold text-[#62728a]">
        {{ textoPermissaoConsulta }}
      </p>
    </aside>

    <article class="min-w-0 rounded-lg border border-[#d4dee9] bg-white p-4 shadow-[0_22px_55px_rgba(14,30,53,0.08)] sm:p-6" :aria-busy="carregandoArquivosLista">
      <div class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div class="min-w-0">
          <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">{{ usuariosVisiveis.length }} usuario(s)</p>
          <h2 class="m-0 mt-2 text-xl font-normal text-[#071d3b]">Usuarios</h2>
        </div>
        <button
          class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
          type="button"
          title="Atualizar lista"
          aria-label="Atualizar lista"
          @click="carregar"
        >
          <RefreshCcw class="h-5 w-5" aria-hidden="true" />
        </button>
      </div>

      <div class="mt-5 grid gap-3 md:grid-cols-[minmax(0,1fr)_240px]">
        <div class="relative">
          <Search class="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-[#62728a]" aria-hidden="true" />
          <input v-model.trim="busca" class="min-h-11 rounded-md border border-[#ccd8e5] pl-12 pr-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="search" placeholder="Consultar usuario" />
        </div>

        <select
          v-model.number="perfilFiltro"
          class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10"
          aria-label="Filtrar por perfil"
        >
          <option :value="0">Todos os perfis</option>
          <option v-for="perfil in perfisFiltro" :key="perfil.idPerfil" :value="perfil.idPerfil">
            {{ formatPerfilLabel(perfil.descricaoPerfil) }}
          </option>
        </select>
      </div>

      <p v-if="erroLista" class="alert alert-error mt-4">{{ erroLista }}</p>

      <div class="mt-4 hidden max-h-[520px] overflow-auto rounded-lg border border-[#d4dee9] md:block">
        <table class="w-full table-fixed border-collapse text-left">
          <thead class="sticky top-0 bg-[#f5f8fb] text-xs uppercase text-[#51627a]">
            <tr>
              <th class="w-[38%] px-4 py-4">Nome</th>
              <th class="w-[13%] px-4 py-4 text-center">Contato</th>
              <th class="w-[16%] px-4 py-4">Tipo</th>
              <th class="w-[13%] px-4 py-4 text-center">Docs</th>
              <th class="w-[20%] px-4 py-4 text-center">Acoes</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="usuario in usuariosPaginados" :key="usuario.idUsuario" class="border-t border-[#d4dee9]">
              <td class="px-4 py-4 font-semibold text-[#243044]">
                <div class="flex min-w-0 items-center gap-3">
                  <div class="flex h-10 w-10 shrink-0 items-center justify-center overflow-hidden rounded-full bg-[#edf3f8] text-xs font-extrabold text-[#071d3b]">
                    <img
                      v-if="fotoUsuarioListaUrl(usuario)"
                      class="h-full w-full object-cover"
                      :src="fotoUsuarioListaUrl(usuario)"
                      :alt="`Foto de ${usuario.nome}`"
                    />
                    <span v-else>{{ obterIniciais(usuario.nome) }}</span>
                  </div>
                  <span class="min-w-0 break-words">{{ usuario.nome }}</span>
                </div>
              </td>
              <td class="px-4 py-4">
                <div class="flex justify-center">
                  <button
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Ver contato"
                    aria-label="Ver contato"
                    @click="abrirContatoUsuario(usuario)"
                  >
                    <Contact class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
              <td class="px-4 py-4 text-[#243044]">{{ formatPerfilLabel(usuario.descricaoPerfil) }}</td>
              <td class="px-4 py-4">
                <div class="flex justify-center">
                  <button
                    v-if="usuarioTemCertificados(usuario)"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Ver documentos"
                    aria-label="Ver documentos"
                    @click="abrirArquivosProfessor(usuario)"
                  >
                    <FileText class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <span v-else class="inline-flex h-10 w-10 items-center justify-center text-[#9aa8ba]">-</span>
                </div>
              </td>
              <td class="px-4 py-4">
                <div class="flex justify-center gap-2">
                  <NuxtLink
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
                    :to="`/usuarios/${usuario.idUsuario}`"
                    title="Visualizar usuario"
                    aria-label="Visualizar usuario"
                  >
                    <Eye class="h-5 w-5" aria-hidden="true" />
                  </NuxtLink>
                  <button
                    v-if="podeEditarUsuario(usuario)"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
                    type="button"
                    title="Editar usuario"
                    aria-label="Editar usuario"
                    @click="editar(usuario)"
                  >
                    <Pencil class="h-5 w-5" aria-hidden="true" />
                  </button>
                  <button
                    v-if="podeExcluirUsuario(usuario)"
                    class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
                    type="button"
                    title="Excluir usuario"
                    aria-label="Excluir usuario"
                    @click="excluir(usuario)"
                  >
                    <Trash2 class="h-5 w-5" aria-hidden="true" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="!carregando && !usuariosFiltrados.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Nenhum usuario encontrado.</td>
            </tr>
            <tr v-if="carregando && !usuarios.length">
              <td class="px-4 py-6 text-[#62728a]" colspan="5">Carregando usuarios...</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="mt-4 grid gap-3 md:hidden">
        <article
          v-for="usuario in usuariosPaginados"
          :key="usuario.idUsuario"
          class="rounded-lg border border-[#d4dee9] bg-white p-4"
        >
          <div class="flex items-start justify-between gap-3">
            <div class="flex min-w-0 items-start gap-3">
              <div class="flex h-11 w-11 shrink-0 items-center justify-center overflow-hidden rounded-full bg-[#edf3f8] text-xs font-extrabold text-[#071d3b]">
                <img
                  v-if="fotoUsuarioListaUrl(usuario)"
                  class="h-full w-full object-cover"
                  :src="fotoUsuarioListaUrl(usuario)"
                  :alt="`Foto de ${usuario.nome}`"
                />
                <span v-else>{{ obterIniciais(usuario.nome) }}</span>
              </div>
              <div class="min-w-0">
                <h3 class="m-0 truncate text-base font-extrabold text-[#071d3b]">{{ usuario.nome }}</h3>
              </div>
            </div>
            <span class="max-w-[42%] break-words rounded-md bg-[#eaf4f1] px-2 py-1 text-right text-xs font-extrabold text-[#006b61]">
              {{ formatPerfilLabel(usuario.descricaoPerfil) }}
            </span>
          </div>
          <div class="mt-4 flex flex-wrap gap-2">
            <button
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Ver contato"
              aria-label="Ver contato"
              @click="abrirContatoUsuario(usuario)"
            >
              <Contact class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              v-if="usuarioTemCertificados(usuario)"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Ver documentos"
              aria-label="Ver documentos"
              @click="abrirArquivosProfessor(usuario)"
            >
              <FileText class="h-5 w-5" aria-hidden="true" />
            </button>
            <NuxtLink
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] no-underline transition hover:bg-[#dfe8f1]"
              :to="`/usuarios/${usuario.idUsuario}`"
              title="Visualizar usuario"
              aria-label="Visualizar usuario"
            >
              <Eye class="h-5 w-5" aria-hidden="true" />
            </NuxtLink>
            <button
              v-if="podeEditarUsuario(usuario)"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
              type="button"
              title="Editar usuario"
              aria-label="Editar usuario"
              @click="editar(usuario)"
            >
              <Pencil class="h-5 w-5" aria-hidden="true" />
            </button>
            <button
              v-if="podeExcluirUsuario(usuario)"
              class="inline-flex h-10 flex-1 items-center justify-center rounded-md bg-[#ffe1e3] text-[#dc2626] transition hover:bg-[#ffd4d7]"
              type="button"
              title="Excluir usuario"
              aria-label="Excluir usuario"
              @click="excluir(usuario)"
            >
              <Trash2 class="h-5 w-5" aria-hidden="true" />
            </button>
          </div>
        </article>

        <p v-if="!carregando && !usuariosFiltrados.length" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Nenhum usuario encontrado.
        </p>
        <p v-if="carregando && !usuarios.length" class="m-0 rounded-lg border border-[#d4dee9] bg-white p-4 text-[#62728a]">
          Carregando usuarios...
        </p>
      </div>

      <div class="mt-4 flex flex-col gap-3 text-sm font-extrabold text-[#62728a] sm:flex-row sm:items-center sm:justify-between">
        <span>{{ intervaloTexto }}</span>
        <div class="flex items-center gap-2">
          <button
            class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] disabled:opacity-45"
            type="button"
            :disabled="pagina === 1"
            aria-label="Pagina anterior"
            @click="pagina--"
          >
            <ChevronLeft class="h-5 w-5" aria-hidden="true" />
          </button>
          <span>Pagina {{ pagina }} de {{ totalPaginas }}</span>
          <button
            class="inline-flex h-10 w-10 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] disabled:opacity-45"
            type="button"
            :disabled="pagina === totalPaginas"
            aria-label="Proxima pagina"
            @click="pagina++"
          >
            <ChevronRight class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>
      </div>
    </article>

    <div
      v-if="usuarioContatoPopup"
      class="fixed inset-0 z-40 grid place-items-center bg-[#071d3b]/50 px-4 py-6"
      @click.self="fecharContatoUsuario"
    >
      <article class="grid w-full max-w-md gap-4 rounded-lg bg-white p-5 shadow-[0_22px_55px_rgba(14,30,53,0.24)]">
        <div class="flex items-start justify-between gap-4">
          <div class="min-w-0">
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Contato</p>
            <h2 class="m-0 mt-1 break-words text-xl font-normal text-[#071d3b]">{{ usuarioContatoPopup.nome }}</h2>
          </div>
          <button
            class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
            type="button"
            title="Fechar"
            aria-label="Fechar"
            @click="fecharContatoUsuario"
          >
            <X class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>

        <div class="grid gap-2">
          <a
            class="flex min-w-0 items-center gap-3 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-[#071d3b] no-underline transition hover:border-[#147f72]"
            :href="`mailto:${usuarioContatoPopup.email}`"
          >
            <Mail class="h-5 w-5 shrink-0" aria-hidden="true" />
            <span class="break-all text-sm font-extrabold">{{ usuarioContatoPopup.email }}</span>
          </a>
          <a
            class="flex min-w-0 items-center gap-3 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-[#071d3b] no-underline transition hover:border-[#147f72]"
            :href="telefoneContatoHref"
          >
            <Phone class="h-5 w-5 shrink-0" aria-hidden="true" />
            <span class="text-sm font-extrabold">{{ telefoneContatoFormatado }}</span>
          </a>
          <div class="grid min-w-0 gap-2 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-sm font-semibold text-[#51627a]">
            <span><strong class="text-[#071d3b]">Mae:</strong> {{ usuarioContatoPopup.nomeMae || '-' }}</span>
            <span><strong class="text-[#071d3b]">Pai:</strong> {{ usuarioContatoPopup.nomePai || '-' }}</span>
            <span class="break-words"><strong class="text-[#071d3b]">Endereco:</strong> {{ usuarioContatoPopup.endereco || '-' }}</span>
          </div>
        </div>
      </article>
    </div>

    <div
      v-if="usuarioArquivosPopup"
      class="fixed inset-0 z-40 grid place-items-center bg-[#071d3b]/50 px-4 py-6"
      @click.self="fecharArquivosProfessor"
    >
      <article class="grid max-h-[90vh] w-full max-w-lg min-w-0 gap-4 overflow-y-auto overflow-x-hidden rounded-lg bg-white p-5 shadow-[0_22px_55px_rgba(14,30,53,0.24)]">
        <div class="flex items-start justify-between gap-4">
          <div class="min-w-0">
            <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">Documentos</p>
            <h2 class="m-0 mt-1 break-words text-xl font-normal text-[#071d3b] [overflow-wrap:anywhere]">{{ usuarioArquivosPopup.nome }}</h2>
          </div>
          <button
            class="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-[#edf3f8] text-[#071d3b] transition hover:bg-[#dfe8f1]"
            type="button"
            title="Fechar"
            aria-label="Fechar"
            @click="fecharArquivosProfessor"
          >
            <X class="h-5 w-5" aria-hidden="true" />
          </button>
        </div>

        <div class="grid gap-2">
          <button
            v-for="arquivo in arquivosPopup"
            :key="obterArquivoId(arquivo)"
            class="flex min-w-0 items-center justify-between gap-3 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-left text-[#071d3b] transition hover:border-[#147f72] disabled:cursor-wait disabled:opacity-70"
            type="button"
            :disabled="arquivoBaixandoId === obterArquivoId(arquivo)"
            @click="baixarArquivoProfessor(arquivo)"
          >
            <span class="inline-flex min-w-0 items-center gap-2">
              <FileText class="h-5 w-5 shrink-0" aria-hidden="true" />
              <span class="truncate text-sm font-extrabold [overflow-wrap:anywhere]">{{ arquivo.nomeOriginal || 'Certificado PDF' }}</span>
            </span>
            <span class="inline-flex shrink-0 items-center gap-2 text-xs font-extrabold text-[#62728a]">
              {{ arquivoBaixandoId === obterArquivoId(arquivo) ? 'Baixando...' : formatarTamanhoArquivo(arquivo.tamanhoBytes) }}
              <Download class="h-4 w-4" aria-hidden="true" />
            </span>
          </button>
        </div>

        <p v-if="!arquivosPopup.length" class="m-0 rounded-md border border-[#d4dee9] bg-[#f8fbfd] p-3 text-sm font-semibold text-[#62728a]">
          Nenhum documento cadastrado.
        </p>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
import { Camera, ChevronLeft, ChevronRight, Contact, Download, Eye, FileText, Mail, Pencil, Phone, Plus, RefreshCcw, Search, Send, Trash2, Upload, X } from '@lucide/vue'
import type { Perfil, UsuarioArquivo, UsuarioCreate, UsuarioForm, UsuarioSummary, UsuarioUpdate } from '~/types/api'
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
  canCreateAlunoUsuarios,
  canDeleteUsuario,
  canEditUsuario,
  canChangeUsuarioPerfil,
  canViewUsuarioInList,
  filterPerfisForUsuarioCreation,
  formatPerfilLabel,
  getDefaultPerfilId,
  getUsuarioPerfilTipo,
  getTipoUsuarioForApiByPerfilId
} from '~/utils/usuario-permissions'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: []
})

const { $api } = useNuxtApp()
const config = useRuntimeConfig()
const auth = useAuthStore()
const usuarios = ref<UsuarioSummary[]>([])
const perfis = ref<Perfil[]>([])
const arquivosUsuario = ref<UsuarioArquivo[]>([])
const arquivosPorUsuario = ref<Record<number, UsuarioArquivo[]>>({})
const fotosPorUsuario = ref<Record<number, string>>({})
const usuarioContatoPopup = ref<UsuarioSummary | null>(null)
const usuarioArquivosPopup = ref<UsuarioSummary | null>(null)
const carregando = ref(false)
const salvando = ref(false)
const carregandoArquivos = ref(false)
const carregandoArquivosLista = ref(false)
const enviandoFoto = ref(false)
const enviandoCertificado = ref(false)
const enviandoNotificacao = ref(false)
const arquivoBaixandoId = ref(0)
const erro = ref('')
const erroLista = ref('')
const erroArquivos = ref('')
const mensagem = ref('')
const mensagemArquivos = ref('')
const busca = ref('')
const perfilFiltro = ref(0)
const pagina = ref(1)
const fotoInputRef = ref<HTMLInputElement | null>(null)
const certificadoInputRef = ref<HTMLInputElement | null>(null)
const fotoSelecionada = ref<File | null>(null)
const certificadoSelecionado = ref<File | null>(null)
const fotoPreviewUrl = ref('')
const porPagina = 10
const editandoId = ref<number | null>(null)
const USER_TEXT_FIELD_MAX_LENGTH = 100
const ADDRESS_FIELD_MAX_LENGTH = 200
const PHONE_FORMAT_ERROR = 'Informe um telefone valido no formato +55 (xx) xxxxx-xxxx.'
const REQUIRED_FIELDS_ERROR = 'Nome, e-mail e telefone sao obrigatorios.'
const REQUIRED_PROFILE_ERROR = 'Informe o tipo de usuario.'
const notificacaoForm = reactive({
  titulo: '',
  mensagem: ''
})
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

const usuariosVisiveis = computed(() =>
  usuarios.value.filter((usuario) => canViewUsuarioInList(auth.usuario, usuario))
)
const perfisFiltro = computed<Perfil[]>(() => {
  const perfisMap = new Map<number, string>()

  for (const usuario of usuariosVisiveis.value) {
    if (usuario.idPerfil && usuario.descricaoPerfil) {
      perfisMap.set(usuario.idPerfil, usuario.descricaoPerfil)
    }
  }

  return Array.from(perfisMap, ([idPerfil, descricaoPerfil]) => ({ idPerfil, descricaoPerfil }))
    .sort((a, b) => formatPerfilLabel(a.descricaoPerfil).localeCompare(formatPerfilLabel(b.descricaoPerfil)))
})
const podeCadastrarUsuarios = computed(() => canCreateAlunoUsuarios(auth.perfil))
const usuarioEmEdicao = computed(() =>
  editandoId.value ? usuarios.value.find((usuario) => usuario.idUsuario === editandoId.value) ?? null : null
)
const fotoUsuarioEmEdicao = computed(() =>
  fotoPreviewUrl.value || (usuarioEmEdicao.value ? fotoUsuarioListaUrl(usuarioEmEdicao.value) : '')
)
const certificadosUsuario = computed(() =>
  arquivosUsuario.value.filter((arquivo) => arquivo.tipoArquivo?.toLowerCase() === 'certificado')
)
const arquivosPopup = computed(() =>
  usuarioArquivosPopup.value ? obterCertificadosDoUsuario(usuarioArquivosPopup.value.idUsuario) : []
)
const telefoneContatoFormatado = computed(() =>
  usuarioContatoPopup.value ? formatBrazilPhone(usuarioContatoPopup.value.telefone) || '-' : '-'
)
const telefoneContatoHref = computed(() =>
  usuarioContatoPopup.value ? `tel:${normalizeBrazilPhoneForApi(usuarioContatoPopup.value.telefone)}` : ''
)
const podeEnviarCertificado = computed(() =>
  getUsuarioPerfilTipo(usuarioEmEdicao.value?.descricaoPerfil) === 'professor'
)
const podeEnviarNotificacao = computed(() => {
  return auth.isAdmin
})
const exibeFormulario = computed(() => podeCadastrarUsuarios.value || Boolean(editandoId.value))
const perfisFormulario = computed<Perfil[]>(() => {
  if (editandoId.value && !canChangeUsuarioPerfil(auth.usuario)) {
    return usuarioEmEdicao.value
      ? [{ idPerfil: usuarioEmEdicao.value.idPerfil, descricaoPerfil: usuarioEmEdicao.value.descricaoPerfil }]
      : []
  }

  return filterPerfisForUsuarioCreation(perfis.value, auth.usuario)
})
const podeAlterarPerfilNoFormulario = computed(() =>
  canChangeUsuarioPerfil(auth.usuario) && perfisFormulario.value.length > 1
)
const tituloFormulario = computed(() => {
  if (editandoId.value) return 'Editar usuario'

  return 'Novo usuario'
})
const textoBotaoSalvar = computed(() => {
  if (salvando.value) return 'Salvando...'
  if (editandoId.value) return 'Atualizar usuario'

  return 'Cadastrar usuario'
})
const textoPermissaoConsulta = computed(() => {
  if (auth.isAluno) return 'Seu perfil permite corrigir seus dados cadastrais.'
  if (auth.isProfessor) return 'Seu perfil permite consultar alunos e professores cadastrados, e alterar apenas seus proprios dados.'

  return 'Seu perfil permite consultar os usuarios cadastrados.'
})
const usuariosFiltrados = computed(() => {
  const termo = busca.value.toLowerCase()
  const usuariosDoPerfil = perfilFiltro.value
    ? usuariosVisiveis.value.filter((usuario) => usuario.idPerfil === perfilFiltro.value)
    : usuariosVisiveis.value

  if (!termo) return usuariosDoPerfil

  return usuariosDoPerfil.filter((usuario) =>
    [usuario.nome, usuario.email, usuario.telefone, formatBrazilPhone(usuario.telefone), usuario.nomeMae, usuario.nomePai, usuario.endereco, usuario.descricaoPerfil, formatPerfilLabel(usuario.descricaoPerfil)]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(termo))
  )
})
const totalPaginas = computed(() => Math.max(1, Math.ceil(usuariosFiltrados.value.length / porPagina)))
const usuariosPaginados = computed(() => {
  const inicio = (pagina.value - 1) * porPagina

  return usuariosFiltrados.value.slice(inicio, inicio + porPagina)
})
const intervaloTexto = computed(() => {
  if (!usuariosFiltrados.value.length) return '0 usuario(s)'

  const inicio = (pagina.value - 1) * porPagina + 1
  const fim = Math.min(pagina.value * porPagina, usuariosFiltrados.value.length)

  return `${inicio}-${fim} de ${usuariosFiltrados.value.length} usuario(s)`
})

watch([busca, perfilFiltro], () => {
  pagina.value = 1
})
watch(perfisFiltro, (perfisDisponiveis) => {
  if (perfilFiltro.value && !perfisDisponiveis.some((perfil) => perfil.idPerfil === perfilFiltro.value)) {
    perfilFiltro.value = 0
  }
})
watch(totalPaginas, (total) => {
  if (pagina.value > total) pagina.value = total
})
watch(editandoId, (idUsuario) => {
  resetarArquivosUsuario()

  if (idUsuario) {
    carregarArquivos(idUsuario)
  }
})

onMounted(async () => {
  await carregar()

  if (podeCadastrarUsuarios.value || canChangeUsuarioPerfil(auth.usuario)) {
    await carregarPerfis()
  }
})

onBeforeUnmount(() => {
  limparFotoPreview()
  limparFotosUsuarios()
})

async function carregar() {
  carregando.value = true
  erroLista.value = ''

  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    if (auth.usuario) {
      try {
        const usuario = await $api<UsuarioSummary>(`/usuarios/${auth.usuario.idUsuario}`)
        usuarios.value = [usuario]
      } catch {
        usuarios.value = [auth.usuario]
      }
    } else {
      erroLista.value = normalizeApiError(err)
    }
  } finally {
    carregando.value = false
  }

  await Promise.all([
    carregarArquivosProfessores(),
    carregarFotosUsuarios()
  ])
}

async function carregarPerfis() {
  try {
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
    if (!editandoId.value) {
      form.idPerfil = getDefaultPerfilId(perfis.value, auth.usuario)
    }
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

async function carregarArquivos(idUsuario: number) {
  carregandoArquivos.value = true
  erroArquivos.value = ''

  try {
    arquivosUsuario.value = await $api<UsuarioArquivo[]>(`/usuarios/${idUsuario}/arquivos`)
    arquivosPorUsuario.value = {
      ...arquivosPorUsuario.value,
      [idUsuario]: obterApenasCertificados(arquivosUsuario.value)
    }
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    carregandoArquivos.value = false
  }
}

async function carregarArquivosProfessores() {
  const professores = usuariosVisiveis.value.filter(usuarioEhProfessorDaLista)
  arquivosPorUsuario.value = {}

  if (!professores.length) {
    return
  }

  carregandoArquivosLista.value = true

  try {
    const resultados = await Promise.all(professores.map(async (usuario) => {
      try {
        const arquivos = await $api<UsuarioArquivo[]>(`/usuarios/${usuario.idUsuario}/arquivos`)
        return [usuario.idUsuario, obterApenasCertificados(arquivos)] as const
      } catch {
        return [usuario.idUsuario, []] as const
      }
    }))

    arquivosPorUsuario.value = Object.fromEntries(resultados)
  } finally {
    carregandoArquivosLista.value = false
  }
}

async function carregarFotosUsuarios() {
  limparFotosUsuarios()

  const usuariosComFoto = usuariosVisiveis.value.filter((usuario) => usuario.fotoPerfilUrl)
  if (!usuariosComFoto.length) {
    return
  }

  const resultados = await Promise.all(usuariosComFoto.map(async (usuario) => {
    try {
      const blob = await fetchApiBlob(`/usuarios/${usuario.idUsuario}/foto`, config.public.apiBase, auth.token)
      return [usuario.idUsuario, URL.createObjectURL(blob)] as const
    } catch {
      return [usuario.idUsuario, ''] as const
    }
  }))

  fotosPorUsuario.value = Object.fromEntries(resultados.filter(([, url]) => Boolean(url)))
}

async function carregarFotoUsuarioLista(idUsuario: number) {
  const usuario = usuarios.value.find((item) => item.idUsuario === idUsuario)
  limparFotoUsuarioLista(idUsuario)

  if (!usuario?.fotoPerfilUrl) {
    return
  }

  try {
    const blob = await fetchApiBlob(`/usuarios/${idUsuario}/foto`, config.public.apiBase, auth.token)
    fotosPorUsuario.value = {
      ...fotosPorUsuario.value,
      [idUsuario]: URL.createObjectURL(blob)
    }
  } catch {
    fotosPorUsuario.value = {
      ...fotosPorUsuario.value,
      [idUsuario]: ''
    }
  }
}

function limparFotoUsuarioLista(idUsuario: number) {
  const currentUrl = fotosPorUsuario.value[idUsuario]
  if (currentUrl) {
    URL.revokeObjectURL(currentUrl)
  }

  const { [idUsuario]: _removida, ...restantes } = fotosPorUsuario.value
  fotosPorUsuario.value = restantes
}

function limparFotosUsuarios() {
  Object.values(fotosPorUsuario.value).forEach((url) => {
    if (url) URL.revokeObjectURL(url)
  })
  fotosPorUsuario.value = {}
}

function fotoUsuarioListaUrl(usuario: UsuarioSummary) {
  return fotosPorUsuario.value[usuario.idUsuario] || ''
}

function obterApenasCertificados(arquivos: UsuarioArquivo[]) {
  return arquivos.filter((arquivo) => arquivo.tipoArquivo?.toLowerCase() === 'certificado')
}

function usuarioEhProfessorDaLista(usuario: UsuarioSummary) {
  return getUsuarioPerfilTipo(usuario.descricaoPerfil) === 'professor'
}

function obterCertificadosDoUsuario(idUsuario: number) {
  return arquivosPorUsuario.value[idUsuario] ?? []
}

function usuarioTemCertificados(usuario: UsuarioSummary) {
  return usuarioEhProfessorDaLista(usuario) && obterCertificadosDoUsuario(usuario.idUsuario).length > 0
}

function abrirContatoUsuario(usuario: UsuarioSummary) {
  usuarioContatoPopup.value = usuario
}

function fecharContatoUsuario() {
  usuarioContatoPopup.value = null
}

function abrirArquivosProfessor(usuario: UsuarioSummary) {
  if (!usuarioTemCertificados(usuario)) {
    return
  }

  usuarioArquivosPopup.value = usuario
}

function fecharArquivosProfessor() {
  usuarioArquivosPopup.value = null
}

function editar(usuario: UsuarioSummary) {
  if (!canEditUsuario(auth.usuario, usuario)) {
    return
  }

  editandoId.value = usuario.idUsuario
  form.nome = usuario.nome
  form.email = usuario.email
  form.telefone = formatBrazilPhone(usuario.telefone)
  form.dataNascimento = usuario.dataNascimento?.slice(0, 10) ?? ''
  form.nomeMae = usuario.nomeMae ?? ''
  form.nomePai = usuario.nomePai ?? ''
  form.endereco = usuario.endereco ?? ''
  form.idPerfil = usuario.idPerfil
  mensagem.value = ''
  erro.value = ''
  mensagemArquivos.value = ''
  erroArquivos.value = ''
}

function limparForm() {
  editandoId.value = null
  form.nome = ''
  form.email = ''
  form.telefone = ''
  form.dataNascimento = ''
  form.nomeMae = ''
  form.nomePai = ''
  form.endereco = ''
  form.idPerfil = getDefaultPerfilId(perfis.value, auth.usuario)
  notificacaoForm.titulo = ''
  notificacaoForm.mensagem = ''
  resetarArquivosUsuario()
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

function obterTipoUsuarioSelecionado() {
  return getTipoUsuarioForApiByPerfilId(perfisFormulario.value, form.idPerfil)
}

function montarPayload(): UsuarioCreate | UsuarioUpdate {
  const payload: UsuarioUpdate = {
    nome: form.nome.trim(),
    email: form.email.trim(),
    telefone: normalizeBrazilPhoneForApi(form.telefone),
    dataNascimento: form.dataNascimento || null,
    nomeMae: form.nomeMae.trim() || null,
    nomePai: form.nomePai.trim() || null,
    endereco: form.endereco.trim() || null
  }

  if (!editandoId.value || canChangeUsuarioPerfil(auth.usuario)) {
    payload.tipoUsuario = obterTipoUsuarioSelecionado()
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

  if (!form.idPerfil || ((!editandoId.value || canChangeUsuarioPerfil(auth.usuario)) && !obterTipoUsuarioSelecionado())) {
    erro.value = REQUIRED_PROFILE_ERROR
    return false
  }

  return true
}

function podeEditarUsuario(usuario: UsuarioSummary) {
  return canEditUsuario(auth.usuario, usuario)
}

function podeExcluirUsuario(_usuario: UsuarioSummary) {
  return canDeleteUsuario(auth.usuario)
}

async function salvar() {
  erro.value = ''
  mensagem.value = ''

  if (!validarFormulario()) return

  if (isDuplicateUserEmail(usuarios.value, form.email, editandoId.value)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    const payload = montarPayload()
    let usuarioSalvo: UsuarioSummary

    if (editandoId.value) {
      usuarioSalvo = await $api<UsuarioSummary>(`/usuarios/${editandoId.value}`, {
        method: 'PUT',
        body: payload
      })
      mensagem.value = 'Usuario atualizado.'
    } else {
      usuarioSalvo = await $api<UsuarioSummary>('/usuarios', {
        method: 'POST',
        body: payload
      })
      mensagem.value = 'Usuario cadastrado.'
    }

    if (usuarioSalvo.idUsuario === auth.usuario?.idUsuario) {
      auth.updateUsuario(usuarioSalvo)
    }

    limparForm()
    await carregar()
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function enviarFoto() {
  if (!editandoId.value || !fotoSelecionada.value) {
    erroArquivos.value = 'Selecione uma foto para enviar.'
    return
  }

  enviandoFoto.value = true
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    const formData = new FormData()
    formData.append('arquivo', fotoSelecionada.value)

    const updated = await $api<UsuarioSummary>(`/usuarios/${editandoId.value}/foto`, {
      method: 'POST',
      body: formData
    })

    atualizarUsuarioLocal(updated)
    fotoSelecionada.value = null
    limparFotoPreview()
    limparInputArquivo(fotoInputRef.value)
    await carregarFotoUsuarioLista(updated.idUsuario)
    mensagemArquivos.value = 'Foto atualizada.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    enviandoFoto.value = false
  }
}

async function baixarArquivoProfessor(arquivo: UsuarioArquivo) {
  await baixarArquivoUsuario(usuarioArquivosPopup.value?.idUsuario, arquivo)
}

async function baixarArquivoUsuario(idUsuario: number | null | undefined, arquivo: UsuarioArquivo) {
  const arquivoId = obterArquivoId(arquivo)
  if (!idUsuario || !arquivoId) {
    erroLista.value = 'Arquivo sem identificador para download.'
    return
  }

  arquivoBaixandoId.value = arquivoId
  erroLista.value = ''

  try {
    const blob = await fetchApiBlob(`/usuarios/${idUsuario}/arquivos/${arquivoId}/download`, config.public.apiBase, auth.token)
    downloadBlob(blob, arquivo.nomeOriginal || 'certificado.pdf')
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  } finally {
    arquivoBaixandoId.value = 0
  }
}

async function enviarCertificado() {
  if (!editandoId.value || !certificadoSelecionado.value) {
    erroArquivos.value = 'Selecione um PDF para enviar.'
    return
  }

  enviandoCertificado.value = true
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    const formData = new FormData()
    formData.append('arquivo', certificadoSelecionado.value)

    await $api<UsuarioArquivo>(`/usuarios/${editandoId.value}/certificados`, {
      method: 'POST',
      body: formData
    })

    certificadoSelecionado.value = null
    limparInputArquivo(certificadoInputRef.value)
    await carregarArquivos(editandoId.value)
    mensagemArquivos.value = 'Certificado adicionado.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    enviandoCertificado.value = false
  }
}

async function excluirArquivo(arquivo: UsuarioArquivo) {
  if (!editandoId.value || !confirm(`Excluir o arquivo ${arquivo.nomeOriginal}?`)) {
    return
  }

  const arquivoId = obterArquivoId(arquivo)
  if (!arquivoId) {
    erroArquivos.value = 'Arquivo sem identificador para exclusao.'
    return
  }

  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    await $api(`/usuarios/${editandoId.value}/arquivos/${arquivoId}`, {
      method: 'DELETE'
    })
    await carregarArquivos(editandoId.value)
    mensagemArquivos.value = 'Arquivo excluido.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  }
}

async function enviarNotificacao() {
  if (!notificacaoForm.titulo.trim() || !notificacaoForm.mensagem.trim()) {
    erroArquivos.value = 'Informe titulo e mensagem da notificacao.'
    return
  }

  enviandoNotificacao.value = true
  erroArquivos.value = ''
  mensagemArquivos.value = ''

  try {
    const body = {
      titulo: notificacaoForm.titulo.trim(),
      mensagem: notificacaoForm.mensagem.trim(),
      tipo: 'Geral'
    }

    await $api('/notificacoes/perfis', {
      method: 'POST',
      body: {
        ...body,
        tiposUsuario: ['Aluno', 'Professor']
      }
    })

    notificacaoForm.titulo = ''
    notificacaoForm.mensagem = ''
    mensagemArquivos.value = 'Notificacao enviada para alunos e professores.'
  } catch (err) {
    erroArquivos.value = normalizeApiError(err)
  } finally {
    enviandoNotificacao.value = false
  }
}

function atualizarUsuarioLocal(updated: UsuarioSummary) {
  usuarios.value = usuarios.value.map((usuario) =>
    usuario.idUsuario === updated.idUsuario ? updated : usuario
  )

  if (updated.idUsuario === auth.usuario?.idUsuario) {
    auth.updateUsuario(updated)
  }
}

async function excluir(usuario: UsuarioSummary) {
  if (!podeExcluirUsuario(usuario)) {
    return
  }

  if (!confirm(`Excluir o usuario ${usuario.nome}?`)) {
    return
  }

  erroLista.value = ''

  try {
    await $api(`/usuarios/${usuario.idUsuario}`, { method: 'DELETE' })
    if (editandoId.value === usuario.idUsuario) limparForm()
    await carregar()
  } catch (err) {
    erroLista.value = normalizeApiError(err)
  }
}

function formatarTamanhoArquivo(tamanhoBytes?: number | null) {
  if (!tamanhoBytes || tamanhoBytes <= 0) return 'PDF'

  if (tamanhoBytes < 1024 * 1024) {
    return `${Math.ceil(tamanhoBytes / 1024)} KB`
  }

  return `${(tamanhoBytes / (1024 * 1024)).toFixed(1).replace('.', ',')} MB`
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
