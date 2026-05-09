# SGX.SistemaChamado

Sistema institucional de abertura, atendimento e acompanhamento de chamados com:

- API .NET 9
- Worker de integração IMAP
- Frontend Vue 3 + Quasar
- PostgreSQL

## Visao geral

O SGX.SistemaChamado separa autenticação e autorização:

- Microsoft Entra ID (Azure AD) autentica identidade
- SGX.SistemaChamado autoriza por perfis internos (`Administrador`, `Atendente`, `Solicitante`)

Fluxos principais:

- Portal do solicitante (`/portal`)
- Area administrativa (`/admin`)
- Dashboard e indicadores de SLA
- integração de e-mail IMAP com abertura/correlacao de chamados

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
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=1qaz@2wsx"
```

Para validar o banco local:

```bash
psql -U user_sgxsc -d sgx_sistema_chamados -h localhost
```

Se precisar redefinir apenas a senha local de desenvolvimento:

```sql
ALTER USER user_sgxsc WITH PASSWORD '1qaz@2wsx';
```

Se o banco local não existir:

```sql
CREATE DATABASE sgx_sistema_chamados OWNER user_sgxsc;
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

4. Se aparecer erro de porta em uso (`http://localhost:5168`), finalize a instancia anterior:

```bash
netstat -ano | findstr :5168
taskkill /PID <PID> /F
```

Evite subir duas instâncias da API ao mesmo tempo (por exemplo `dotnet run` + `Run and Debug` no VS Code).

5. Worker:

```bash
dotnet run --project src/SGX.SistemaChamado.Worker.Email
```

6. Frontend:

```bash
cd src/SGX.SistemaChamado.Web
npm install
npm run build
npm run dev
```

Abra manualmente no navegador ja utilizado:
- `http://localhost:5173/login`

Para desenvolvimento local com autenticação tecnica:

```powershell
$env:VITE_API_BASE_URL="http://localhost:5168"
$env:VITE_AUTH_MODO_LOCAL="true"
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

## variáveis de ambiente principais

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

## autenticação Azure AD e modo local

- Em produção/homologacao: usar Azure AD configurado.
- Em Development: suporte a modo local por headers `X-Dev-*`.
- usuário administrativo local de desenvolvimento: `admin@sgxdigital.com` (nome: `Administrador SGX`).
- O modo local usa headers e não senha local persistida na base.

### Login administrativo local em Development

Pre-requisitos:
- `ASPNETCORE_ENVIRONMENT=Development`
- `Authentication__ModoLocalHabilitado=true`
- `VITE_AUTH_MODO_LOCAL=true`

Acesso:
- URL: `http://localhost:5173/login`
- E-mail: `admin@sgxdigital.com`
- Senha (trava visual local): `Admin@123456`

Observacoes:
- Disponivel somente em Development/modo local.
- Em produção, o fluxo oficial permanece Microsoft Entra ID.
- A senha local acima não e enviada para autenticação de backend e não deve ser usada fora de desenvolvimento.

### Frontend local

Passos:
1. `cd src/SGX.SistemaChamado.Web`
2. `npm run dev`
3. Abrir manualmente `http://localhost:5173/login`

Login local Development:
- E-mail: `admin@sgxdigital.com`
- Senha: `Admin@123456`

Observacao:
- O VS Code não deve abrir navegador automaticamente. O acesso deve ser manual na URL local.

Detalhes: `docs/CONFIGURACAO-AZURE-AD.md`.

## Worker de e-mail IMAP

Comportamento:

- Leitura periodica da caixa IMAP
- Abertura de chamado para e-mail novo
- Correlacao por codigo do chamado no assunto e headers (`In-Reply-To`, `References`)
- Deduplicacao por `MessageId` e `Fingerprint`
- Persistencia de logs técnicos de integração
- Reuso do `IArquivoStorageService` para anexos validos

Se IMAP não estiver configurado em Development, o Worker inicia e registra warning sem crash.

## Endpoints de health checks

- `GET /health`
- `GET /health/live`
- `GET /health/ready`
- `GET /api/saude`

## Roadmap e seguranca

O roadmap do projeto e a documentacao de perfis/permissoes estao disponiveis na pasta `docs`.

- `docs/ROADMAP.md`
- `docs/SEGURANCA-PERFIS-PERMISSOES.md`
- `docs/ROADMAP-ITSM.md`

## Seguranca

- Swagger habilitado por padrao apenas em Development.
- Em ambiente não-Development, Swagger so abre com `Swagger__EnableInNonDevelopment=true`.
- CORS sem `AllowAnyOrigin` em produção: configurar `Cors__AllowedOrigins__*`.
- HSTS habilitado fora de Development.
- Nunca versionar segredo real (Azure, IMAP, banco de produção).
- não registrar senha IMAP em log.

## Testes

```bash
dotnet test SGX.SistemaChamado.sln
```

Cobertura atual inclui:

- testes de dominio/aplicacao
- testes de autorização/politicas
- testes de integração HTTP com `WebApplicationFactory`

## Documentacao detalhada

- `docs/ARQUITETURA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/BANCO-DE-DADOS.md`
- `docs/EXECUCAO-LOCAL.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`
- `docs/ROADMAP.md`
- `docs/ROADMAP-ITSM.md`
- `docs/SEGURANCA-PERFIS-PERMISSOES.md`
- `docs/PUBLICACAO.md`





