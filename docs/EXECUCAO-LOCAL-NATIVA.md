# Execução Local Nativa — Sem Docker

## Objetivo

Explicar como rodar o sistema localmente sem Docker, usando PostgreSQL, .NET 9 e Node.js instalados diretamente no sistema operacional.

## Stack

- Backend: .NET 9 / ASP.NET Core
- Frontend: Vue 3 + Quasar + Vite
- Banco: PostgreSQL
- Persistência: Entity Framework Core

## Pré-requisitos

- .NET SDK 9
- Node.js
- npm
- PostgreSQL
- Git
- VS Code opcional

## Banco local esperado

- Host: `localhost`
- Port: `5432`
- Database: `sgx_sistema_chamados`
- Username: `user_sgxsc`
- Password: `1qaz@2wsx`

Essa senha é apenas para desenvolvimento local.

## Criar banco local PostgreSQL

```bash
sudo -u postgres psql <<'SQL'
DO $$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'user_sgxsc') THEN
      CREATE ROLE user_sgxsc LOGIN PASSWORD '1qaz@2wsx';
   ELSE
      ALTER USER user_sgxsc WITH PASSWORD '1qaz@2wsx';
   END IF;
END
$$;

SELECT 'CREATE DATABASE sgx_sistema_chamados OWNER user_sgxsc'
WHERE NOT EXISTS (
  SELECT FROM pg_database WHERE datname = 'sgx_sistema_chamados'
)\gexec
SQL
```

## Restaurar backend

```bash
dotnet restore SGX.SistemaChamado.sln
```

## Aplicar migrations

```bash
dotnet tool run dotnet-ef database update --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
```

## Rodar API

```bash
dotnet run --project src/SGX.SistemaChamado.Api
```

URL esperada:

`http://localhost:5168`

## Configurar frontend

```bash
cd src/SGX.SistemaChamado.Web
```

Criar `.env.local` com:

```env
VITE_API_BASE_URL=http://localhost:5168
VITE_AUTH_MODO_LOCAL=true
```

## Rodar frontend

```bash
npm install
npm run dev
```

URL esperada:

`http://localhost:5173/login`

## Login local de desenvolvimento

- E-mail: `admin@sgxdigital.com`
- Senha: `Admin@123456`

## Healthchecks

```bash
curl http://localhost:5168/health
curl http://localhost:5168/health/live
curl http://localhost:5168/health/ready
```

## Atenção

- Não usar Docker nesse fluxo.
- Não rodar o worker de e-mail inicialmente.
- Rodar primeiro API e frontend.
- Só rodar testes depois que a aplicação estiver funcionando.
- Se houver erro com Active Directory, verificar se o provedor principal local está ativo antes de alterar código.
