# Escola Conectada - Frontend

Frontend da aplicacao Escola Conectada, construido em Nuxt 3. Este README cobre somente o projeto do front, separado do backend/API.

## Visao geral

O projeto e uma SPA para gestao escolar. A aplicacao consome uma API REST externa e oferece autenticacao, painel inicial, gestao de usuarios, caderneta digital, notificacoes, QR Code bancario ficticio para alunos, calendario escolar e holerites para professores/administradores.

O layout usa fundo claro, cards brancos, destaque laranja para marca/secao, botoes em verde/teal e icones Lucide.

## Tecnologias usadas

| Tecnologia | Uso |
| --- | --- |
| Nuxt 3.21.6 | Framework Vue, roteamento por arquivos, plugins e build |
| Vue 3 | Componentes e Composition API |
| TypeScript strict | Tipagem de stores, API, paginas e utilitarios |
| Pinia | Estado global de autenticacao |
| Tailwind CSS | Estilizacao utilitaria |
| ofetch / `$fetch` | Cliente HTTP usado pelo wrapper `$api` |
| @lucide/vue | Icones de interface |
| qrcode | Geracao local de QR Code ficticio |
| jsPDF | Geracao de PDF de holerite no cliente |
| Vitest | Testes unitarios |
| happy-dom | Ambiente DOM para testes |
| @nuxt/test-utils | Configuracao de testes Nuxt |
| Nginx | Servir o build estatico no Docker |

## Estrutura principal

```text
ESCOLA_FRONT/
|-- assets/css/main.css              # Tailwind e estilos globais
|-- components/                      # Componentes reutilizaveis
|   `-- DatePicker.vue               # Campo de data com digitacao e calendario
|-- docs/                            # Documentacao tecnica em PDF/Markdown
|-- layouts/                         # Layouts default e auth
|-- middleware/auth.global.ts        # Guarda global de autenticacao/autorizacao
|-- middleware/aluno.ts              # Restringe rotas exclusivas de alunos
|-- middleware/funcionario.ts        # Restringe rotas exclusivas de funcionarios
|-- pages/                           # Rotas Nuxt
|-- plugins/api.ts                   # Injeta o cliente HTTP $api
|-- stores/                          # Stores Pinia
|-- tests/                           # Testes unitarios
|-- types/api.ts                     # Contratos TypeScript da API
|-- utils/                           # Utilitarios compartilhados
|-- Dockerfile                       # Build estatico + Nginx
`-- nuxt.config.ts                   # Configuracao Nuxt
```

## Configuracao

Variaveis suportadas:

| Variavel | Padrao local | Descricao |
| --- | --- | --- |
| `NUXT_PUBLIC_API_BASE` | `http://localhost:5001/api` em dev, `/api` em build sem variavel | URL base da API consumida pelo front |
| `NUXT_APP_BASE_URL` | `/` | Base URL da aplicacao Nuxt |

Em deploy na Vercel, Render ou GitHub Pages, configure `NUXT_PUBLIC_API_BASE` com a URL publica do backend. Nao use `localhost` em producao, porque no navegador ele aponta para a maquina do usuario.

No Render, crie o front como `Static Site` e use:

| Campo | Valor |
| --- | --- |
| Branch | `master` |
| Build Command | `npm run generate` |
| Publish Directory | `.output/public` |
| Environment Variable | `NUXT_PUBLIC_API_BASE=https://sua-api-publica.com/api` |

Tambem adicione uma regra de rewrite para a SPA:

| Action | Source | Destination |
| --- | --- | --- |
| Rewrite | `/*` | `/index.html` |

O repositorio inclui `render.yaml` com essa configuracao. Se o servico ja existir no Render, ajuste os mesmos campos em `Settings` e execute `Manual Deploy > Clear build cache & deploy`.

## Como executar

Instalar dependencias:

```bash
npm install
```

Rodar em desenvolvimento:

```bash
npm run dev
```

Gerar build de producao:

```bash
npm run build
```

Gerar build estatico:

```bash
npm run generate
```

Validar tipos:

```bash
npm run typecheck
```

Rodar testes:

```bash
npm run test
```

## Fluxo de navegacao

```mermaid
flowchart TD
  A[Inicio da SPA] --> B{Usuario autenticado?}
  B -- Nao --> L[/login]
  L --> C{Login aceito?}
  C -- Nao --> L
  C -- Sim --> D{Senha padrao?}
  B -- Sim --> D
  D -- Sim --> S[/alterar-senha]
  D -- Nao --> P[/ Painel Escola Conectada]
  S --> P

  P --> US[/usuarios]
  P --> CD[/caderneta-digital]
  P --> CO[/comunicados]
  P --> CE[/calendario-escolar]
  P --> QR[/qr-code-bancario]
  P --> HO[/holerite]
  P --> AS[/alterar-senha]

  US --> USN[/usuarios/novo]
  US --> USD[/usuarios/:id]
```

## Telas

| Rota | Finalidade |
| --- | --- |
| `/login` | Login e recuperacao/reset da senha padrao |
| `/` | Painel com atalhos para os modulos e ordenacao por drag and drop |
| `/alterar-senha` | Alteracao manual de senha |
| `/usuarios` | Consulta e CRUD de usuarios conforme perfil |
| `/usuarios/novo` | Cadastro dedicado de usuario |
| `/usuarios/:id` | Visualizacao, edicao e exclusao de usuario conforme perfil |
| `/caderneta-digital` | Cadastro de disciplinas, lancamento de notas/frequencia e consulta da caderneta |
| `/comunicados` | Envio administrativo de comunicados para alunos e professores |
| `/calendario-escolar` | Calendario anual, feriados nacionais, eventos escolares e agenda de avaliacoes/trabalhos |
| `/qr-code-bancario` | Geracao de QR Code com dados bancarios ficticios, exclusiva para aluno |
| `/holerite` | Consulta e lancamento de holerites, exclusivo para professor e administrador |

## Perfis de acesso

A sessao autenticada guarda usuario, token JWT, data de expiracao e flag de senha padrao no `localStorage`, usando a chave `form-escola-auth`.

Os perfis reconhecidos no front sao:

- `Administrador`: acesso completo a usuarios, comunicados para alunos e professores, consulta geral, documentos permitidos, eventos escolares e lancamento/consulta de holerites.
- `Membro da Diretoria`: acesso completo a usuarios, notificacoes, consulta geral e documentos permitidos.
- `Professor`: consulta usuarios permitidos, administra caderneta, agenda avaliacoes/trabalhos, consulta documentos conforme regra, edita dados permitidos e visualiza seus proprios holerites.
- `Aluno`: consulta o proprio cadastro, caderneta, calendario escolar e QR Code ficticio.

Matriz resumida:

| Area | Administrador | Diretoria | Professor | Aluno |
| --- | --- | --- | --- | --- |
| Login / senha | Sim | Sim | Sim | Sim |
| Painel | Sim | Sim | Sim | Sim |
| Usuarios | Criar, visualizar, editar, alterar tipo e excluir | Criar, visualizar, editar, alterar tipo e excluir | Visualizar permitidos e editar conforme regra | Editar o proprio cadastro |
| Foto de perfil | Edita conforme permissao | Edita conforme permissao | Edita conforme permissao | Edita propria foto quando permitido |
| Certificados PDF | Consulta/gerencia conforme regra | Consulta/gerencia conforme regra | Consulta/gerencia conforme regra | Consulta conforme regra |
| Caderneta Digital | Consulta | Consulta | Administra disciplinas, notas e frequencia | Consulta dados associados |
| Comunicados | Envia para alunos e professores | Nao acessa | Nao acessa | Nao acessa |
| Calendario Escolar | Lanca eventos escolares e consulta | Consulta | Marca avaliacoes e trabalhos | Consulta somente leitura |
| QR Code ficticio | Nao acessa | Nao acessa | Nao acessa | Gera |
| Holerite | Lanca, consulta, exporta, envia e exclui | Nao acessa | Consulta, exporta e envia os proprios | Nao acessa |

Observacao: o front controla a experiencia e evita acoes indevidas na UI, mas a API deve continuar sendo a fonte final de autorizacao.

## Modulos principais

### Autenticacao

Arquivos principais:

- `stores/auth.ts`: login, logout, persistencia da sessao, validacao via `/auth/me` e alteracao de senha.
- `middleware/auth.global.ts`: guarda global de rotas.
- `plugins/api.ts`: injeta `$api` com token JWT e tratamento de `401`.
- `utils/api-client.ts`: monta headers, normaliza erros e executa chamadas HTTP.

### Usuarios

O modulo de usuarios permite cadastrar, listar, filtrar, editar e excluir conforme perfil. O cadastro inclui:

- Nome.
- E-mail.
- Telefone com mascara brasileira `+55 (xx) xxxxx-xxxx`.
- Data de aniversario com `DatePicker`.
- Nome da mae, nome do pai e endereco.
- Tipo de usuario.
- Foto de perfil.
- Certificados PDF para professores, quando permitido.
- Envio manual de comunicados administrativos para alunos e professores.

Os campos `dataNascimento`, `nomeMae`, `nomePai` e `endereco` sao enviados ao backend nos DTOs de criacao/edicao e retornados nas consultas de usuario.

### Caderneta Digital

Disponivel em `/caderneta-digital`.

- Professores administram disciplinas.
- Professores lancam notas, presencas e faltas.
- Notas, media e situacao ficam em um popup de aprendizado acionado por icone na listagem.
- Alunos visualizam apenas registros associados ao proprio cadastro.

### Notificacoes

O layout principal carrega notificacoes via API, exibe contador de nao lidas, permite abrir detalhes, marcar uma notificacao como lida e marcar todas como lidas.

Administradores podem enviar comunicados para alunos e professores pelo endpoint `POST /notificacoes/perfis`, usando `tiposUsuario: ["Aluno", "Professor"]`.

Mensagens longas, URLs de fotos e URLs de PDF sao quebradas dentro do popup para evitar rolagem horizontal.

### Painel inicial

Disponivel em `/`.

O painel exibe os modulos permitidos para cada perfil e permite reorganizar os cards por drag and drop. A ordem e salva no `localStorage` por usuario autenticado, usando uma chave especifica da Escola Conectada. Para sincronizar essa preferencia entre navegadores ou dispositivos, sera necessario um endpoint de preferencias no backend.

As telas internas exibem breadcrumbs ao lado do botao `Painel`, montados a partir da rota atual.

### QR Code bancario ficticio

Disponivel em `/qr-code-bancario`.

Disponivel somente para alunos. O painel oculta o modulo para professor/administrador/diretoria, e a rota usa `middleware/aluno.ts` para redirecionar perfis nao alunos.

O front gera dados bancarios demonstrativos por aluno e transforma o payload em QR Code com a biblioteca `qrcode`.

A tela permite:

- Escolher banco ficticio.
- Informar valor ficticio.
- Gerar QR Code.
- Compartilhar texto por WhatsApp.
- Abrir e-mail via `mailto`.
- Copiar dados.
- Baixar imagem PNG do QR.

Todo payload e marcado como `SEM VALOR BANCARIO`.

### Holerite

Disponivel em `/holerite`.

Disponivel somente para professor e administrador. O painel exibe o modulo conforme perfil, e a rota usa `middleware/funcionario.ts` para redirecionar alunos e perfis sem acesso.

A tela possui duas visoes:

- Professor: consulta os proprios holerites retornados pela API, baixa PDF e compartilha por e-mail/WhatsApp.
- Administrador: seleciona professor/administrador, lanca rubricas editaveis, gera PDF no cliente, salva o PDF na API, baixa, compartilha e exclui holerites.

Rubricas padrao sugeridas para lancamento:

- Salario base.
- Gratificacao pedagogica ou administrativa.
- Auxilio alimentacao.
- Auxilio transporte.
- INSS demonstrativo.
- IRRF demonstrativo.
- Vale transporte.
- Plano de saude.
- Contribuicao associativa.
- FGTS apenas informativo.

Integracao com API:

- `GET /holerites/me`: lista holerites do professor/administrador logado.
- `GET /holerites/me/{holeriteId}/download`: baixa PDF do proprio holerite.
- `GET /holerites/usuarios/{usuarioId}`: administrador lista holerites de um funcionario.
- `POST /holerites/usuarios/{usuarioId}`: administrador envia PDF gerado pelo front com `arquivo`, `competenciaMes` e `competenciaAno`.
- `GET /holerites/usuarios/{usuarioId}/{holeriteId}/download`: administrador baixa PDF de um funcionario.
- `DELETE /holerites/usuarios/{usuarioId}/{holeriteId}`: administrador exclui holerite.
- `POST /holerites/me/{holeriteId}/compartilhamento` e `POST /holerites/usuarios/{usuarioId}/{holeriteId}/compartilhamento`: criam link temporario para compartilhamento externo quando suportado pela API/storage.

O compartilhamento por e-mail e WhatsApp abre o cliente externo com mensagem e link temporario retornado pela API. Ao lancar holerite, a API cria a notificacao para o funcionario informando quem lancou, competencia, arquivo e link para `/holerite`.

### Calendario Escolar

Disponivel em `/calendario-escolar`.

O painel exibe:

- Calendario do ano selecionado.
- Mes vigente selecionado por padrao.
- Feriados nacionais brasileiros.
- Agenda mensal com eventos escolares, avaliacoes e trabalhos.
- Formulario para professor marcar datas por disciplina.
- Formulario para administrador lancar festas, reunioes com professores e reunioes de pais e mestres.
- Visualizacao somente leitura para alunos e demais perfis que nao gerenciam agenda.

Os feriados ficam em `utils/feriados-brasil.ts` e incluem feriados nacionais fixos e a Paixao de Cristo calculada a partir da Pascoa.

A agenda de avaliacoes/trabalhos por disciplina usa endpoints da API e o backend notifica os alunos matriculados na disciplina quando o professor marca ou atualiza um evento. Eventos escolares lancados pelo administrador usam `GET /calendario-escolar` e `POST /calendario-escolar/eventos`; a API persiste o criador, publico-alvo e dispara as notificacoes conforme o tipo do evento.

## Componentes e utilitarios relevantes

| Arquivo | Responsabilidade |
| --- | --- |
| `components/AppBreadcrumbs.vue` | Breadcrumbs globais exibidos ao lado do botao Painel |
| `components/DatePicker.vue` | Entrada de data com digitacao `dd/mm/aaaa`, selecao visual, hoje e limpar |
| `utils/date-utils.ts` | Parse, mascara e formatacao de datas |
| `utils/feriados-brasil.ts` | Feriados nacionais brasileiros por ano |
| `utils/qr-code-bancario.ts` | Dados ficticios, payload e mensagem de compartilhamento do QR |
| `utils/holerite-ficticio.ts` | Rubricas, totais, resumo e geracao de PDF de holerite |
| `utils/br-phone.ts` | Mascara e normalizacao de telefone brasileiro |
| `utils/password-strength.ts` | Regras de forca de senha |
| `utils/usuario-permissions.ts` | Normalizacao de perfis e regras de permissao da UI |
| `utils/usuario-validation.ts` | Validacao de e-mail duplicado no front |

## Integracao com a API

O cliente `$api` usa `NUXT_PUBLIC_API_BASE` como `baseURL` e envia `Authorization: Bearer <token>` quando existe token.

Endpoints consumidos:

| Modulo | Endpoints |
| --- | --- |
| Auth | `POST /auth/login`, `GET /auth/me`, `POST /auth/alterar-senha`, `POST /auth/esqueci-senha` |
| Usuarios | `GET /usuarios`, `GET /usuarios/:id`, `POST /usuarios`, `PUT /usuarios/:id`, `DELETE /usuarios/:id`, `GET /usuarios/perfis` |
| Arquivos de usuario | `GET /usuarios/:id/arquivos`, `GET /usuarios/:id/foto`, `POST /usuarios/:id/foto`, `POST /usuarios/:id/certificados`, `GET /usuarios/:id/arquivos/:arquivoId/download`, `DELETE /usuarios/:id/arquivos/:arquivoId` |
| Notificacoes | `GET /notificacoes`, `POST /notificacoes`, `POST /notificacoes/perfis`, `PATCH /notificacoes/:id/lida`, `PATCH /notificacoes/lidas` |
| Caderneta Digital | `GET /caderneta-digital`, `POST /caderneta-digital`, `PUT /caderneta-digital/:id`, `DELETE /caderneta-digital/:id`, `GET /caderneta-digital/disciplinas`, `POST /caderneta-digital/disciplinas`, `PUT /caderneta-digital/disciplinas/:id`, `DELETE /caderneta-digital/disciplinas/:id`, `GET /caderneta-digital/disciplinas/eventos`, `POST /caderneta-digital/disciplinas/:id/eventos`, `PUT /caderneta-digital/disciplinas/:id/eventos/:eventoId`, `DELETE /caderneta-digital/disciplinas/:id/eventos/:eventoId` |
| Holerites | `GET /holerites/me`, `GET /holerites/me/:id/download`, `POST /holerites/me/:id/compartilhamento`, `GET /holerites/usuarios/:usuarioId`, `POST /holerites/usuarios/:usuarioId`, `GET /holerites/usuarios/:usuarioId/:id/download`, `POST /holerites/usuarios/:usuarioId/:id/compartilhamento`, `DELETE /holerites/usuarios/:usuarioId/:id` |

Contratos TypeScript ficam em `types/api.ts`.

## Validacoes

O projeto usa validacoes locais simples:

- Validacao nativa do HTML (`required`, `type`, `maxlength`).
- Tipagem TypeScript para contratos de payload/resposta.
- Mascara e validacao de telefone em `utils/br-phone.ts`.
- Mascara, parse e validacao de data em `utils/date-utils.ts`.
- Validacao de e-mail duplicado em `utils/usuario-validation.ts`.
- Regras de permissao de UI em `utils/usuario-permissions.ts`.
- Normalizacao de erros da API em `utils/api-client.ts`.

Regras de forca de senha:

- Minimo de 8 caracteres.
- Pelo menos uma letra maiuscula.
- Pelo menos uma letra minuscula.
- Pelo menos um numero.
- Pelo menos um caractere especial.

## Testes unitarios

Configuracao:

- `vitest.config.ts` usa `defineVitestConfig` de `@nuxt/test-utils/config`.
- Ambiente: `happy-dom`.
- Setup global: `tests/setup.ts`.

Cobertura atual:

| Arquivo | O que valida |
| --- | --- |
| `tests/stores/auth.spec.ts` | Login, persistencia de JWT, logout e alteracao de senha |
| `tests/utils/api-client.spec.ts` | Header Authorization, preservacao de headers, callback de 401 e normalizacao de erros |
| `tests/utils/br-phone.spec.ts` | Mascara e normalizacao de telefone |
| `tests/utils/caderneta-digital.spec.ts` | Parse de notas, media e situacao |
| `tests/utils/date-utils.spec.ts` | Mascara, parse e formatacao de datas |
| `tests/utils/feriados-brasil.spec.ts` | Feriados nacionais e Paixao de Cristo |
| `tests/utils/holerite-ficticio.spec.ts` | Rubricas, totais, resumo e geracao de PDF do holerite |
| `tests/utils/password-strength.spec.ts` | Classificacao de senha |
| `tests/utils/qr-code-bancario.spec.ts` | Dados ficticios e payload do QR Code |
| `tests/utils/usuario-permissions.spec.ts` | Regras de perfil/permissao |
| `tests/utils/usuario-validation.spec.ts` | E-mail duplicado |

## Build e deploy

O projeto esta configurado como SPA com `ssr: false`.

Fluxo do Dockerfile:

1. Usa `node:22` para instalar dependencias.
2. Executa `npm run generate`.
3. Copia `.output/public` para uma imagem `nginx:alpine`.
4. O Nginx serve a SPA e redireciona rotas internas para `index.html`.

## Scripts disponiveis

| Script | Acao |
| --- | --- |
| `npm run dev` | Sobe o servidor de desenvolvimento Nuxt |
| `npm run build` | Build de producao Nuxt |
| `npm run generate` | Gera build estatico |
| `npm run preview` | Preview do build |
| `npm run typecheck` | Checagem TypeScript/Nuxt |
| `npm run test` | Executa testes unitarios |
| `npm run test:watch` | Executa Vitest em modo watch |
| `npm run postinstall` | Gera tipos Nuxt apos instalar dependencias |

## Pendencias para backend

As funcionalidades abaixo ja possuem front, mas precisam de apoio do backend para uso persistente/multiusuario:

- Endpoints `PUT`/`DELETE` para eventos escolares institucionais, caso seja necessario editar ou excluir eventos ja lancados.
- Endpoint de preferencias de usuario, caso a ordenacao drag and drop do painel precise sincronizar entre dispositivos.
- Envio real server-side de e-mail/WhatsApp para holerite com anexo, auditoria e fila de envio, caso seja necessario sair do compartilhamento via cliente externo.
- Envio real de e-mail/WhatsApp para QR Code, caso necessario.

## Observacoes de manutencao

- Este README documenta somente o front.
- Regras de banco, migrations e seguranca definitiva pertencem ao backend.
- Ao adicionar nova rota protegida, definir `definePageMeta({ roles: [...] })` quando houver restricao por perfil.
- Ao adicionar novos endpoints, atualizar `types/api.ts`, paginas/stores correspondentes e testes quando houver logica compartilhada.
