# SGX.SistemaChamado

Sistema institucional de abertura, atendimento e acompanhamento de chamados com:

- API .NET 9
- Worker de integracao IMAP
- Frontend Vue 3 + Quasar
- PostgreSQL

## Visao geral

O SGX.SistemaChamado separa autenticacao e autorizacao:

- Microsoft Entra ID (Azure AD) autentica identidade
- SGX.SistemaChamado autoriza por perfis internos (`Administrador`, `Atendente`, `Solicitante`)

Fluxos principais:

- Portal do solicitante (`/portal`)
- Area administrativa (`/admin`)
- Dashboard e indicadores de SLA
- Integracao de e-mail IMAP com abertura/correlacao de chamados

## Arquitetura

```text
src/
  SGX.SistemaChamado.Domain
  SGX.SistemaChamado.Application
  SGX.SistemaChamado.Infrastructure
  SGX.SistemaChamado.Api
  SGX.SistemaChamado.Worker.Email
  SGX.SistemaChamado.Web
tests/
  SGX.SistemaChamado.Tests
docs/
  ARQUITETURA.md
  CONFIGURACAO-AZURE-AD.md
  BANCO-DE-DADOS.md
  EXECUCAO-LOCAL.md
  HOMOLOGACAO-CHECKLIST.md
  PUBLICACAO.md
```

## Pre-requisitos

- .NET SDK 9
- Node.js 22+
- PostgreSQL 14+ (local: `localhost:5432`)
- Docker Desktop (opcional)

## Execucao local sem Docker

Antes de subir API/Worker, configure a conexao local por variavel de ambiente (ou User Secrets), por exemplo:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=<SENHA_LOCAL>"
```

1. Restore/build/test:

```bash
dotnet restore SGX.SistemaChamado.sln
dotnet build SGX.SistemaChamado.sln
dotnet test SGX.SistemaChamado.sln
```

2. Banco e migrations:

```bash
dotnet tool run dotnet-ef database update --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
```

3. API:

```bash
dotnet run --project src/SGX.SistemaChamado.Api
```

4. Worker:

```bash
dotnet run --project src/SGX.SistemaChamado.Worker.Email
```

5. Frontend:

```bash
cd src/SGX.SistemaChamado.Web
npm install
npm run build
npm run dev
```

Guia detalhado: `docs/EXECUCAO-LOCAL.md`.

## Execucao local com Docker

```bash
docker compose build
docker compose up -d
docker compose ps
docker compose logs -f api
docker compose logs -f worker-email
docker compose down
```

Arquivos:

- `docker-compose.yml`
- `src/SGX.SistemaChamado.Api/Dockerfile`
- `src/SGX.SistemaChamado.Worker.Email/Dockerfile`
- `src/SGX.SistemaChamado.Web/Dockerfile`

## Variaveis de ambiente principais

### Backend/API

- `ConnectionStrings__DefaultConnection`
- `Authentication__ModoLocalHabilitado`
- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`
- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`
- `Swagger__EnableInNonDevelopment`

### Worker IMAP

- `EmailWorker__ImapHost`
- `EmailWorker__ImapPorta`
- `EmailWorker__Usuario`
- `EmailWorker__Senha`
- `EmailWorker__Pasta`
- `EmailWorker__SslHabilitado`
- `EmailWorker__TlsHabilitado`
- `EmailWorker__IntervaloSegundos`
- `EmailWorker__MaxMensagensPorCiclo`
- `EmailWorker__MarcarComoLidaAoProcessar`
- `EmailWorker__MoverProcessadas`
- `EmailWorker__PastaProcessadas`
- `EmailWorker__MoverComErro`
- `EmailWorker__PastaErro`

### Frontend

- `VITE_API_BASE_URL`
- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_AUTHORITY`
- `VITE_AZURE_REDIRECT_URI`
- `VITE_AUTH_MODO_LOCAL`
- `VITE_AZURE_API_SCOPE`

## Autenticacao Azure AD e modo local

- Em producao/homologacao: usar Azure AD configurado.
- Em Development: suporte a modo local por headers `X-Dev-*`.

Detalhes: `docs/CONFIGURACAO-AZURE-AD.md`.

## Worker de e-mail IMAP

Comportamento:

- Leitura periodica da caixa IMAP
- Abertura de chamado para e-mail novo
- Correlacao por codigo do chamado no assunto e headers (`In-Reply-To`, `References`)
- Deduplicacao por `MessageId` e `Fingerprint`
- Persistencia de logs tecnicos de integracao
- Reuso do `IArquivoStorageService` para anexos validos

Se IMAP nao estiver configurado em Development, o Worker inicia e registra warning sem crash.

## Endpoints de health checks

- `GET /health`
- `GET /health/live`
- `GET /health/ready`
- `GET /api/saude`

## Seguranca

- Swagger habilitado por padrao apenas em Development.
- Em ambiente nao-Development, Swagger so abre com `Swagger__EnableInNonDevelopment=true`.
- CORS sem `AllowAnyOrigin` em producao: configurar `Cors__AllowedOrigins__*`.
- HSTS habilitado fora de Development.
- Nunca versionar segredo real (Azure, IMAP, banco de producao).
- Nao registrar senha IMAP em log.

## Testes

```bash
dotnet test SGX.SistemaChamado.sln
```

Cobertura atual inclui:

- testes de dominio/aplicacao
- testes de autorizacao/politicas
- testes de integracao HTTP com `WebApplicationFactory`

## Documentacao detalhada

- `docs/ARQUITETURA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/BANCO-DE-DADOS.md`
- `docs/EXECUCAO-LOCAL.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`
- `docs/PUBLICACAO.md`
