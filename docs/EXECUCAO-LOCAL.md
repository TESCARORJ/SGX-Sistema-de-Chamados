# Execucao Local

## 1. PostgreSQL

Suba um PostgreSQL local (ou via Docker) e configure:

- `ConnectionStrings__DefaultConnection`

Exemplo em PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=<SENHA_LOCAL>"
```

## 2. Migrations

```bash
dotnet tool run dotnet-ef database update --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
```

## 3. API

```bash
dotnet run --project src/SGX.SistemaChamado.Api
```

Em `Development`, por padrao o modo local pode ser habilitado para testes tecnicos.

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

Build de producao:

```bash
npm run build
```

## 6. Modo local Development (`X-Dev-*`)

Headers suportados:

- `X-Dev-User-Email`
- `X-Dev-User-Name`
- `X-Dev-User-Role` (`Administrador`, `Atendente`, `Solicitante`)

Exemplo:

```http
X-Dev-User-Email: atendente.local@sgx.local
X-Dev-User-Name: Atendente Local
X-Dev-User-Role: Atendente
```

## 7. Validacoes recomendadas

- `GET /api/me`
- `GET /api/portal/chamados`
- bloqueio de `GET /api/admin/chamados` para `Solicitante`
- `GET /api/admin/dashboard` para `Administrador`/`Atendente`
- `GET /api/admin/integracoes/email/logs` para `Administrador`/`Atendente`
- `GET /health`
- `GET /health/live`
- `GET /health/ready`

## 8. Docker Compose local

```bash
docker compose up -d
docker compose logs -f api
docker compose logs -f worker-email
docker compose down
```

## 9. Executando pelo VS Code

1. Abra a pasta raiz onde esta `SGX.SistemaChamado.sln`.
2. Execute `Terminal > Run Task > dotnet: restore`.
3. Execute `Terminal > Run Task > ef: database update`.
4. Em `Run and Debug`, inicie `API - SGX.SistemaChamado.Api`.
5. Em `Run and Debug`, inicie `Worker - SGX.SistemaChamado.Worker.Email`.
6. Execute `Terminal > Run Task > npm: dev web` para frontend local.
7. Para subir os dois processos .NET juntos, use `Run and Debug > API + Worker`.
8. Para modo local em Development, mantenha `Authentication__ModoLocalHabilitado=true` e use:
   - `X-Dev-User-Email`
   - `X-Dev-User-Name`
   - `X-Dev-User-Role`

Observacoes:
- Nao configure senha IMAP real em `launch.json`/`tasks.json`.
- Use variaveis de ambiente ou User Secrets para segredos.
- Se a aba Run and Debug nao detectar C#, instale `C# Dev Kit` e `C#`.
