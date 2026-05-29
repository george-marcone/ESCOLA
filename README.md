# Escola Conectada

Monorepo da Escola Conectada com backend ASP.NET Core e frontend Nuxt.

## Estrutura

```text
ESCOLA/
|-- api/    # API REST, banco, migrations, Swagger/Scalar e testes .NET
`-- front/  # Aplicacao Nuxt, telas, stores, utilitarios e testes Vitest
```

Os repositórios antigos foram importados com histórico preservado:

- `ESCOLA_API` -> `api/`
- `ESCOLA_FRONT` -> `front/`

## Executar Localmente

API:

```bash
cd api
dotnet restore ESCOLA_API.csproj
dotnet run
```

Front:

```bash
cd front
npm install
npm run dev
```

Por padrão, o front usa `NUXT_PUBLIC_API_BASE=http://localhost:5001/api` em desenvolvimento.

## Testes

API:

```bash
cd api
dotnet test ESCOLA_API.Tests/ESCOLA_API.Tests.csproj
```

Front:

```bash
cd front
npm run typecheck
npm test
```

## Docker Compose Da API

```bash
cd api
cp .env.example .env
docker compose up -d --build
```

No PowerShell:

```powershell
Set-Location api
Copy-Item .env.example .env
docker compose up -d --build
```

## Deploy

Este monorepo possui um `render.yaml` na raiz com dois serviços:

- API: `rootDir: api`
- Front: `rootDir: front`

Na Vercel, configure o projeto do frontend com:

- Root Directory: `front`
- Build Command: `npm run build`
- Output/Framework: Nuxt, detectado automaticamente
- Environment Variable: `NUXT_PUBLIC_API_BASE=https://sua-api-publica.com/api`

No Render, use o Blueprint de raiz ou configure manualmente:

- API: root directory `api`
- Front: root directory `front`

## Repositórios Antigos

Os diretórios locais antigos continuam existindo:

- `C:\George Marcone\GitHub\personal\ESCOLA_API`
- `C:\George Marcone\GitHub\personal\ESCOLA_FRONT`

A partir da migração, o diretório principal de trabalho deve ser:

```text
C:\George Marcone\GitHub\personal\ESCOLA
```
