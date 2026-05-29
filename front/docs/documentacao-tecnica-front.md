# Documentacao Tecnica - Escola Conectada Frontend

Versao: 0.2.0
Data de revisao: 2026-05-28

## 1. Objetivo

Este documento descreve a arquitetura, os modulos, os contratos e os pontos de manutencao do frontend da aplicacao Escola Conectada.

O frontend e uma SPA Nuxt 3 que consome uma API REST externa. Ele controla a experiencia de usuario, roteamento, autenticacao no navegador, validacoes locais, permissoes visuais e integracoes de tela. A API continua sendo a fonte final de dados, autorizacao e persistencia.

As funcionalidades de QR Code bancario para alunos e geracao de PDF de holerite no cliente ficam no front. Holerites salvos, listados, baixados e excluidos usam os endpoints da API.

## 2. Stack tecnica

| Tecnologia | Responsabilidade |
| --- | --- |
| Nuxt 3.21.6 | Framework Vue, roteamento por arquivos, plugins e build |
| Vue 3 | Componentes e Composition API |
| TypeScript strict | Tipagem de contratos, paginas e utilitarios |
| Pinia | Estado global, principalmente autenticacao |
| Tailwind CSS | Estilizacao utilitaria |
| ofetch / `$fetch` | Cliente HTTP |
| @lucide/vue | Iconografia |
| qrcode | Geracao de QR Code no cliente |
| jsPDF | Geracao de PDF de holerite no cliente |
| Vitest + happy-dom | Testes unitarios |
| Nginx | Servir build estatico no Docker |

## 3. Configuracao Nuxt

Arquivo principal: `nuxt.config.ts`.

Configuracoes relevantes:

- `ssr: false`: aplicacao gerada como SPA.
- `modules`: `@pinia/nuxt` e `@nuxtjs/tailwindcss`.
- `css`: `~/assets/css/main.css`.
- `runtimeConfig.public.apiBase`: controla a URL base da API.
- `app.baseURL`: permite deploy em subcaminho.
- `typescript.strict: true`.

Variaveis:

| Variavel | Uso |
| --- | --- |
| `NUXT_PUBLIC_API_BASE` | URL publica/base da API |
| `NUXT_APP_BASE_URL` | Base URL da aplicacao |

## 4. Estrutura de diretorios

```text
assets/css/main.css              # Tailwind e estilos globais
components/                      # Componentes reutilizaveis
docs/                            # Documentacao tecnica
layouts/                         # Layouts default e auth
middleware/auth.global.ts        # Middleware global de autenticacao
middleware/aluno.ts              # Restricao para rotas exclusivas de alunos
middleware/funcionario.ts        # Restricao para rotas exclusivas de funcionarios
pages/                           # Rotas Nuxt
plugins/api.ts                   # Plugin que injeta $api
stores/auth.ts                   # Store de autenticacao
tests/                           # Testes unitarios
types/api.ts                     # Contratos TypeScript da API
utils/                           # Utilitarios compartilhados
```

## 5. Autenticacao

Arquivos:

- `stores/auth.ts`
- `middleware/auth.global.ts`
- `plugins/auth.client.ts`
- `plugins/api.ts`
- `utils/api-client.ts`

Fluxo:

1. O usuario autentica em `/login`.
2. A resposta de login e persistida no `localStorage` com a chave `form-escola-auth`.
3. O middleware global carrega a sessao, valida via `/auth/me` e protege rotas privadas.
4. O plugin `$api` injeta o token JWT no header `Authorization`.
5. Em resposta `401`, a sessao e encerrada e o usuario volta para `/login`.

Regras do middleware:

- Rotas publicas nao exigem token.
- Usuario autenticado que acessa `/login` volta ao painel.
- Usuario com senha padrao e direcionado a `/alterar-senha`.
- Rotas com `meta.roles` verificam `usuario.descricaoPerfil`.

## 6. Perfis e permissoes

Utilitario principal: `utils/usuario-permissions.ts`.

Perfis normalizados:

- `administrador`
- `diretoria`
- `professor`
- `aluno`
- `desconhecido`

O front usa essas permissoes para esconder botoes, campos e acoes. A API deve repetir as regras para seguranca real.

## 7. Modulos de tela

### 7.1 Login e senha

Rotas:

- `/login`
- `/alterar-senha`

Recursos:

- Login via API.
- Reset/esqueci senha.
- Troca obrigatoria quando `deveAlterarSenhaPadrao` estiver ativo.
- Medidor de forca de senha.

### 7.2 Painel inicial

Rota: `/`.

Exibe atalhos para:

- Usuarios.
- Caderneta Digital.
- Calendario Escolar.
- QR Code, somente para aluno.
- Holerite, somente para professor e administrador.
- Alterar senha.

Os cards do painel podem ser reorganizados por drag and drop. A ordem fica salva no `localStorage` por usuario autenticado; sincronizacao entre dispositivos depende de um futuro endpoint de preferencias.

As telas internas exibem breadcrumbs ao lado do botao `Painel` no cabecalho global.

### 7.3 Usuarios

Rotas:

- `/usuarios`
- `/usuarios/novo`
- `/usuarios/:id`
- `/comunicados`

Funcionalidades:

- Cadastro e edicao de usuario.
- Busca e filtro por perfil.
- Controle de permissoes por perfil.
- Upload e visualizacao de foto.
- Upload, download e exclusao de certificados PDF.
- Popup de contatos.
- Popup de documentos.
- Card administrativo no painel para acessar `/comunicados`.
- Envio manual de comunicados para alunos e professores.
- Campos `dataNascimento`, `nomeMae`, `nomePai` e `endereco`.

Campos cadastrais:

- Componente: `components/DatePicker.vue`.
- Entrada digitavel: `dd/mm/aaaa`.
- Valor salvo no estado/payload: `yyyy-mm-dd`.
- Valores vazios opcionais enviados como `null`.
- Campos de responsaveis e endereco ficam nos formularios `/usuarios`, `/usuarios/novo` e `/usuarios/:id`.

### 7.4 Caderneta Digital

Rota: `/caderneta-digital`.

Funcionalidades:

- Cadastro, edicao e exclusao de disciplinas.
- Lancamento de notas, presencas e faltas.
- Consulta por disciplina.
- Professores administram lancamentos.
- Alunos visualizam apenas registros associados ao proprio cadastro.
- A listagem exibe um icone de aprendizado; notas, media e situacao ficam em popup para evitar quebra de linhas.

### 7.5 Notificacoes

Implementadas no layout principal.

Funcionalidades:

- Listagem de notificacoes.
- Contador de nao lidas.
- Modal de detalhe.
- Marcar notificacao como lida.
- Marcar todas como lidas.
- Quebra de mensagens longas, URLs de fotos e URLs de PDFs sem overflow horizontal.
- Envio administrativo para alunos e professores via `POST /notificacoes/perfis`, usando `tiposUsuario: ["Aluno", "Professor"]`.

### 7.6 QR Code bancario ficticio

Rota: `/qr-code-bancario`.

Arquivos:

- `pages/qr-code-bancario.vue`
- `middleware/aluno.ts`
- `utils/qr-code-bancario.ts`

Funcionalidades:

- Disponivel somente para alunos.
- Gera dados bancarios demonstrativos por aluno.
- Gera QR Code localmente.
- Compartilha texto por WhatsApp.
- Abre e-mail via `mailto`.
- Copia dados.
- Baixa PNG do QR Code.

Todo payload gerado contem `SEM VALOR BANCARIO`.

### 7.7 Calendario Escolar

Rota: `/calendario-escolar`.

Arquivos:

- `pages/calendario-escolar.vue`
- `utils/feriados-brasil.ts`
- `utils/date-utils.ts`

Funcionalidades:

- Exibe calendario anual.
- Seleciona o mes vigente por padrao.
- Lista feriados nacionais brasileiros.
- Exibe grade mensal detalhada.
- Permite professor marcar avaliacoes e trabalhos por disciplina.
- Permite administrador lancar festas da escola, reunioes com professores e reunioes de pais e mestres.
- Permite editar/excluir eventos.
- Mantem alunos e demais perfis sem permissao de gerenciamento em modo somente leitura.

Persistencia atual:

- Avaliacoes e trabalhos por disciplina usam endpoints da API em `/caderneta-digital/disciplinas/.../eventos`.
- O backend relaciona disciplina, matriculas e notificacoes para alunos matriculados.
- Eventos escolares administrativos usam `GET /calendario-escolar` e `POST /calendario-escolar/eventos`.
- O backend persiste criador, tipo, publico-alvo e dispara notificacoes conforme o evento escolar cadastrado pelo administrador.

Feriados:

- Feriados fixos nacionais.
- Paixao de Cristo calculada a partir da Pascoa.
- Dia Nacional de Zumbi e da Consciencia Negra incluido como feriado nacional.

### 7.8 Holerite

Rota: `/holerite`.

Arquivos:

- `pages/holerite.vue`
- `middleware/funcionario.ts`
- `utils/holerite-ficticio.ts`

Funcionalidades:

- Disponivel somente para professor e administrador.
- Professor consulta somente os proprios holerites retornados por `GET /holerites/me`.
- Administrador seleciona professor/administrador, lanca rubricas editaveis e gera PDF no cliente.
- Administrador salva o PDF gerado em `POST /holerites/usuarios/{usuarioId}` usando `multipart/form-data`.
- Administrador lista, baixa e exclui holerites de funcionarios permitidos.
- Professor e administrador baixam PDF, exportam e compartilham dados por e-mail/WhatsApp.
- Ao lancar holerite, a API envia notificacao com administrador responsavel, competencia, arquivo e link para `/holerite`.

Rubricas padrao sugeridas:

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

Observacoes:

- Os valores sao preenchidos no front pelo administrador antes da geracao do PDF.
- A API atual recebe e persiste o PDF; nao recebe rubricas estruturadas em JSON.
- E-mail/WhatsApp abrem clientes externos com mensagem e link publico quando o storage retornar `url`.
- Links publicos usam os endpoints de compartilhamento de holerite quando disponiveis.
- Envio server-side real com anexo/auditoria precisa de endpoint dedicado no backend.

## 8. Componentes reutilizaveis

### DatePicker

Arquivo: `components/DatePicker.vue`.

Responsabilidades:

- Entrada manual com mascara `dd/mm/aaaa`.
- Conversao para `yyyy-mm-dd`.
- Selecao visual em calendario mensal.
- Navegacao de mes.
- Acoes `Hoje`, `Limpar` e `OK`.
- Estado `disabled` e `required`.

## 9. Utilitarios

| Arquivo | Responsabilidade |
| --- | --- |
| `components/AppBreadcrumbs.vue` | Breadcrumbs globais por rota |
| `utils/api-client.ts` | Cliente HTTP e normalizacao de erros |
| `utils/api-file.ts` | Download de blobs protegidos por token |
| `utils/api-url.ts` | Resolucao de URLs da API |
| `utils/br-phone.ts` | Mascara e normalizacao de telefone |
| `utils/caderneta-digital.ts` | Parse de notas e regras locais da caderneta |
| `utils/date-utils.ts` | Mascara, parse e formatacao de datas |
| `utils/feriados-brasil.ts` | Feriados nacionais brasileiros |
| `utils/holerite-ficticio.ts` | Rubricas, totais, resumo e geracao de PDF de holerite |
| `utils/password-strength.ts` | Regras de forca de senha |
| `utils/qr-code-bancario.ts` | Dados ficticios e payload de QR Code |
| `utils/usuario-permissions.ts` | Regras visuais de permissao |
| `utils/usuario-validation.ts` | Validacao de e-mail duplicado |

## 10. Contratos de API

Arquivo: `types/api.ts`.

Principais interfaces:

- `UsuarioSummary`
- `UsuarioForm`
- `UsuarioCreate`
- `UsuarioUpdate`
- `UsuarioArquivo`
- `Notificacao`
- `Perfil`
- `AuthResponse`
- `DisciplinaCaderneta`
- `DisciplinaEvento`
- `CadernetaDigitalSummary`
- `Holerite`
- `NotificacaoEnvio`

Campo novo:

- `dataNascimento?: string | null` em usuarios.
- `nomeMae?: string | null`, `nomePai?: string | null` e `endereco?: string | null` em usuarios.

Funcionalidades locais sem contrato de API:

- QR Code bancario ficticio.
- Rubricas editaveis usadas para gerar PDF de holerite antes do upload.

## 11. Endpoints consumidos

| Modulo | Endpoints |
| --- | --- |
| Auth | `POST /auth/login`, `GET /auth/me`, `POST /auth/alterar-senha`, `POST /auth/esqueci-senha` |
| Usuarios | `GET /usuarios`, `GET /usuarios/:id`, `POST /usuarios`, `PUT /usuarios/:id`, `DELETE /usuarios/:id`, `GET /usuarios/perfis` |
| Arquivos | `GET /usuarios/:id/arquivos`, `GET /usuarios/:id/foto`, `POST /usuarios/:id/foto`, `POST /usuarios/:id/certificados`, `GET /usuarios/:id/arquivos/:arquivoId/download`, `DELETE /usuarios/:id/arquivos/:arquivoId` |
| Notificacoes | `GET /notificacoes`, `POST /notificacoes`, `POST /notificacoes/perfis`, `PATCH /notificacoes/:id/lida`, `PATCH /notificacoes/lidas` |
| Caderneta | `GET /caderneta-digital`, `POST /caderneta-digital`, `PUT /caderneta-digital/:id`, `DELETE /caderneta-digital/:id` |
| Disciplinas | `GET /caderneta-digital/disciplinas`, `POST /caderneta-digital/disciplinas`, `PUT /caderneta-digital/disciplinas/:id`, `DELETE /caderneta-digital/disciplinas/:id`, `GET /caderneta-digital/disciplinas/eventos`, `POST /caderneta-digital/disciplinas/:id/eventos`, `PUT /caderneta-digital/disciplinas/:id/eventos/:eventoId`, `DELETE /caderneta-digital/disciplinas/:id/eventos/:eventoId` |
| Holerites | `GET /holerites/me`, `GET /holerites/me/:id/download`, `POST /holerites/me/:id/compartilhamento`, `GET /holerites/usuarios/:usuarioId`, `POST /holerites/usuarios/:usuarioId`, `GET /holerites/usuarios/:usuarioId/:id/download`, `POST /holerites/usuarios/:usuarioId/:id/compartilhamento`, `DELETE /holerites/usuarios/:usuarioId/:id` |

## 12. Validacoes

Validacoes locais:

- HTML nativo (`required`, `type`, `maxlength`).
- `utils/br-phone.ts` para telefone.
- `utils/date-utils.ts` para data.
- `utils/password-strength.ts` para senha.
- `utils/usuario-validation.ts` para e-mail duplicado.
- `normalizeApiError` para mensagens vindas da API.

## 13. Testes

Comando:

```bash
npm run test
```

Cobertura:

- Store de autenticacao.
- Cliente API.
- Telefone brasileiro.
- Caderneta digital.
- Date utils.
- Feriados brasileiros.
- Holerite, totais e geracao de PDF.
- Forca de senha.
- QR Code bancario ficticio.
- Permissoes de usuario.
- Validacao de usuario.

## 14. Build e deploy

Comandos:

```bash
npm run typecheck
npm run test
npm run build
npm run generate
```

Deploy estatico:

- Usar `.output/public`.
- Configurar rewrite de SPA para `index.html`.
- Definir `NUXT_PUBLIC_API_BASE`.

Docker:

- Build com Node 22.
- Geracao estatica.
- Servido por `nginx:alpine`.

## 15. Pendencias e integracoes futuras

1. Criar `PUT`/`DELETE` para eventos escolares institucionais caso seja necessario editar ou excluir eventos ja lancados.
2. Criar endpoint de preferencias de usuario caso a ordenacao drag and drop do painel precise sincronizar entre dispositivos.
3. Criar endpoints de envio real de holerite por e-mail/WhatsApp caso seja necessario envio server-side com anexo/auditoria.
4. Opcionalmente criar envio real de e-mail/WhatsApp para QR Code.
5. Opcionalmente mover feriados para endpoint configuravel, caso haja feriados estaduais/municipais.

## 16. Manutencao

Ao adicionar novas telas:

- Atualizar `pages/index.vue` se precisar aparecer no painel.
- Atualizar `layouts/default.vue` se precisar titulo dinamico.
- Atualizar `types/api.ts` se houver contrato de API.
- Criar utilitario testavel quando houver regra compartilhada.
- Atualizar README e esta documentacao tecnica.
