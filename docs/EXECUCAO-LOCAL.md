# Execucao Local

## 1. PostgreSQL

Suba um PostgreSQL local (ou via Docker) e configure:

- `ConnectionStrings__DefaultConnection`

Exemplo em PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=1qaz@2wsx"
```

Validacao direta no PostgreSQL local:

```bash
psql -U user_sgxsc -d sgx_sistema_chamados -h localhost
```

Se a senha local estiver divergente:

```sql
ALTER USER user_sgxsc WITH PASSWORD '1qaz@2wsx';
```

Se o banco ainda nao existir:

```sql
CREATE DATABASE sgx_sistema_chamados OWNER user_sgxsc;
```

Importante:
- Em `Development`, variavel de ambiente `ConnectionStrings__DefaultConnection` sobrescreve `appsettings.Development.json`.
- Remova variavel antiga/sessao de terminal antiga caso ainda exista senha diferente.

## 2. Migrations

```bash
dotnet tool run dotnet-ef database update --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
```

## 3. API

```bash
dotnet run --project src/SGX.SistemaChamado.Api
```

Em `Development`, por padrao o modo local pode ser habilitado para testes tecnicos.

Se ocorrer erro `Failed to bind to address http://127.0.0.1:5168: address already in use`:

```bash
netstat -ano | findstr :5168
taskkill /PID <PID> /F
```

Nao execute duas instancias da API ao mesmo tempo (ex.: `dotnet run` e debug do VS Code em paralelo).

## 4. Worker de e-mail

```bash
dotnet run --project src/SGX.SistemaChamado.Worker.Email
```

Sem configuracao IMAP completa, o Worker registra warning e nao processa mensagens.

## 5. Frontend

```bash
cd src/SGX.SistemaChamado.Web
npm install
npm run dev
```

Abra manualmente no navegador ja utilizado:
- `http://localhost:5173/login`

Build de producao:

```bash
npm run build
```

Rotas principais para navegacao:

- `http://localhost:5173/login`
- `http://localhost:5173/portal`
- `http://localhost:5173/admin`

Observacao:
- O VS Code nao deve abrir navegador automaticamente. O acesso ao frontend deve ser manual na URL desejada.

## 6. Modo local Development (`X-Dev-*`)

Headers suportados:

- `X-Dev-User-Email`
- `X-Dev-User-Name`
- `X-Dev-User-Role` (`Administrador`, `Atendente`, `Solicitante`)

Exemplo:

```http
X-Dev-User-Email: admin@sgxdigital.com
X-Dev-User-Name: Administrador SGX
X-Dev-User-Role: Administrador
```

## 7. Login administrativo local em Development

Pre-requisitos:
- `ASPNETCORE_ENVIRONMENT=Development`
- `Authentication__ModoLocalHabilitado=true`
- `VITE_AUTH_MODO_LOCAL=true`
- `VITE_API_BASE_URL=http://localhost:5168`

Acesso no frontend:
- URL: `http://localhost:5173/login`
- E-mail: `admin@sgxdigital.com`
- Senha (trava visual local): `Admin@123456`

Comportamento:
- O login local existe apenas para desenvolvimento.
- A senha acima e validada apenas no frontend (nao e enviada para o backend).
- A autenticacao tecnica do backend continua via headers `X-Dev-*`.
- Em producao, o fluxo oficial e Microsoft Entra ID.

## 7.1 Emulacao de perfis em Development

Disponivel apenas em `Development`/modo local.

Como usar:
- Acesse `http://localhost:5173/login`.
- Entre como `admin@sgxdigital.com` (senha local: `Admin@123456`).
- Entre em `/admin`.
- Clique em `Visualizar como Solicitante` para testar o portal.
- Clique em `Visualizar como Atendente` para testar a fila administrativa.

Dados de emulacao:
- `solicitante.demo@sgxdigital.com` / `Solicitante Demo` / `Solicitante`
- `atendente.demo@sgxdigital.com` / `Atendente Demo` / `Atendente`

Headers `X-Dev-*`:
- Solicitante:
  - `X-Dev-User-Email: solicitante.demo@sgxdigital.com`
  - `X-Dev-User-Name: Solicitante Demo`
  - `X-Dev-User-Role: Solicitante`
- Atendente:
  - `X-Dev-User-Email: atendente.demo@sgxdigital.com`
  - `X-Dev-User-Name: Atendente Demo`
  - `X-Dev-User-Role: Atendente`

Comportamento:
- `Visualizar como Solicitante` redireciona para `/portal` e exibe aviso de emulacao.
- `Visualizar como Atendente` redireciona para `/admin/chamados` e exibe aviso de emulacao no layout administrativo.
- Para retornar, use `Voltar para Administrador`.

Regras:
- O botao nao aparece em `Production`.
- O botao aparece apenas para perfil `Administrador` no modo local.
- A emulacao nao remove autorizacao do backend.
- O contexto anterior e guardado em memoria/sessao para restauracao.
- Logout limpa qualquer contexto de emulacao.
- Recurso exclusivo para testes locais de UX/permissoes.

## 8. Validacoes recomendadas

- `GET /api/me`
- `GET /api/portal/chamados`
- bloqueio de `GET /api/admin/chamados` para `Solicitante`
- `GET /api/admin/dashboard` para `Administrador`/`Atendente`
- `GET /api/admin/integracoes/email/logs` para `Administrador`/`Atendente`
- `GET /health`
- `GET /health/live`
- `GET /health/ready`

Validacao UX/UI frontend (manual):

1. Confirmar card de login Quasar em `/login` com botoes Microsoft/local dev.
2. Confirmar layout administrativo com `QHeader + QDrawer + QPageContainer` em `/admin`.
3. Confirmar lista de chamados administrativa em `QTable` em `/admin/chamados`.
4. Confirmar detalhe administrativo organizado em cards/timelines em `/admin/chamados/:id`.
5. Confirmar cadastros em tabelas/formularios Quasar (`/admin/cadastros/*`).
6. Confirmar parametros com badge de sensivel/ativo e valor mascarado quando aplicavel.
7. Confirmar logs de e-mail com filtros, tabela e dialog em `/admin/integracoes/email`.
8. Confirmar portal do solicitante (`/portal`, `/portal/chamados`, `/portal/chamados/novo`, `/portal/chamados/:id`).
9. Validar responsividade basica (desktop/mobile).

## 9. Docker Compose local

```bash
docker compose up -d
docker compose logs -f api
docker compose logs -f worker-email
docker compose down
```

## 10. Executando pelo VS Code

1. Abra a pasta raiz onde esta `SGX.SistemaChamado.sln`.
2. Execute `Terminal > Run Task > dotnet: restore`.
3. Execute `Terminal > Run Task > ef: database update`.
4. Para iniciar API + Web juntos sem abrir navegador automaticamente, execute:
   - `Terminal > Run Task > app: run api + web`
5. Abra manualmente `http://localhost:5173/login`.
6. O navegador nao sera aberto automaticamente pelo VS Code.
7. Em `Run and Debug`, inicie `API - SGX.SistemaChamado.Api` quando precisar debugar apenas a API.
8. Em `Run and Debug`, inicie `Worker - SGX.SistemaChamado.Worker.Email` quando precisar debugar apenas o Worker.
9. Para subir os dois processos .NET juntos em debug, use `Run and Debug > API + Worker`.
10. Para subir API + Worker + Web com task unica, use `Terminal > Run Task > app: run full local`.
11. Para modo local em Development, mantenha `Authentication__ModoLocalHabilitado=true` e use:
   - `X-Dev-User-Email`
   - `X-Dev-User-Name`
   - `X-Dev-User-Role`
12. Caso a API ja esteja rodando em outra sessao, finalize o processo da porta `5168` antes de iniciar novo debug.

Observacoes:
- Nao configure senha IMAP real em `launch.json`/`tasks.json`.
- Use variaveis de ambiente ou User Secrets para segredos.
- Se a aba Run and Debug nao detectar C#, instale `C# Dev Kit` e `C#`.
- O fluxo local nao deve abrir navegador automaticamente via VS Code.
