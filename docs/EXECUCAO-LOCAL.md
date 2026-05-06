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
