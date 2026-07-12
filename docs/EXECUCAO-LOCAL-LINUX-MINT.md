# Execucao Local no Linux Mint

Guia para rodar o SGX Sistema de Chamados diretamente no Linux Mint, sem Docker, usando PostgreSQL local, API em `dotnet run` e frontend em `npm run dev`.

## Premissas

- Linux Mint 22.x ou equivalente baseado em Ubuntu 24.04.
- Acesso administrativo ao servidor PostgreSQL local.
- Repositorio aberto na raiz do projeto `SGX.SistemaChamado.sln`.

## 1. Pre-requisitos

Verifique as versoes instaladas:

```bash
dotnet --version
node --version
npm --version
psql --version
dotnet tool restore
dotnet tool run dotnet-ef --version
```

Se `dotnet-ef` ainda nao estiver disponivel, rode novamente:

```bash
dotnet tool restore
```

## 2. Instalacao do .NET 9

O projeto usa `.NET 9`. A documentacao oficial da Microsoft para Ubuntu recomenda instalar o SDK quando voce vai desenvolver e rodar aplicacoes localmente.

### Opcao via repositorio da Microsoft

```bash
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-9.0
```

Valide com:

```bash
dotnet --version
```

## 3. Instalacao do Node.js e npm

O frontend usa Vite, Vue 3 e Quasar. O projeto ja funciona com Node.js 22.x ou superior.

Use o metodo oficial do Node.js para obter um instalador LTS compativel com seu sistema e valide depois:

```bash
node --version
npm --version
```

Se voce preferir um fluxo mais direto para este repositorio, confirme apenas que a maquina esta em uma versao LTS recente e que `npm install` e `npm run dev` funcionam na pasta `src/SGX.SistemaChamado.Web`.

## 4. Instalacao do PostgreSQL

A documentacao oficial do PostgreSQL para Ubuntu indica o uso do `apt` ou do repositorio PGDG. Como o Linux Mint 22.x e baseado no Ubuntu 24.04, o caminho da familia `noble` se aplica aqui.

### Instalacao basica

```bash
sudo apt update
sudo apt install -y postgresql
```

### Se precisar da versao da PGDG

```bash
sudo apt install -y postgresql-common
sudo /usr/share/postgresql-common/pgdg/apt.postgresql.org.sh
sudo apt update
sudo apt install -y postgresql-16 postgresql-client-16
```

### Verificacao

```bash
pg_isready -h localhost -p 5432
psql --version
```

## 5. Criacao do banco e do usuario local

O projeto espera:

- banco: `sgx_sistema_chamados`
- usuario: `user_sgxsc`
- senha local: `1qaz@2wsx`

Se voce tiver acesso a um superusuario do PostgreSQL:

```bash
sudo -u postgres psql
```

Dentro do `psql`:

```sql
CREATE USER user_sgxsc WITH PASSWORD '1qaz@2wsx';
CREATE DATABASE sgx_sistema_chamados OWNER user_sgxsc;
GRANT ALL PRIVILEGES ON DATABASE sgx_sistema_chamados TO user_sgxsc;
```

Se o usuario e o banco ja existirem, apenas ajuste a senha:

```sql
ALTER USER user_sgxsc WITH PASSWORD '1qaz@2wsx';
```

Teste a conexao:

```bash
PGPASSWORD='1qaz@2wsx' psql -h localhost -p 5432 -U user_sgxsc -d sgx_sistema_chamados -c "select 1;"
```

## 6. Configuracao do backend

O arquivo `src/SGX.SistemaChamado.Api/appsettings.Development.json` ja aponta para:

```text
Host=localhost;Port=5432;Database=sgx_sistema_chamados;Username=user_sgxsc;Password=1qaz@2wsx
```

Tambem deixa habilitado:

- `Authentication__ModoLocalHabilitado=true`
- `Authentication__LoginLocalHabilitado=true`

Se alguma variavel de ambiente antiga estiver sobrescrevendo a configuracao, remova a variavel e reabra o terminal.

## 7. Aplicacao das migrations

Rode a partir da raiz do repositorio:

```bash
./scripts/linux-db-update.sh
```

Equivalentemente:

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update \
  --project src/SGX.SistemaChamado.Infrastructure \
  --startup-project src/SGX.SistemaChamado.Api \
  --context SGXSistemaChamadoDbContext
```

Se aparecer `PendingModelChangesWarning`, pare e compare o snapshot antes de criar migration nova.

## 8. Execucao da API

Para validacoes pontuais de compilacao da API sem carregar a solucao inteira, use:

```bash
./scripts/linux-build-api-safe.sh
```

Esse script:

- compila somente `src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj`;
- desabilita reutilizacao de processos do MSBuild;
- limita o paralelismo para reduzir risco de consumo excessivo de memoria;
- nao executa testes, publish, migrations nem a solucao inteira.

Use esse atalho quando voce precisar apenas confirmar se a API continua compilando apos uma alteracao localizada.

Ele nao substitui a suite completa de CI. Build e testes completos devem ser executados separadamente, de forma controlada, quando a mudanca exigir validacao mais ampla.

```bash
./scripts/linux-start-api.sh
```

Ou manualmente:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development
export Authentication__ModoLocalHabilitado=true
dotnet run --project src/SGX.SistemaChamado.Api
```

Valide:

- `http://localhost:5168/health`
- `http://localhost:5168/health/live`
- `http://localhost:5168/health/ready`

Se a porta estiver ocupada, encerre o processo que ja esta usando `5168`.

## 9. Execucao do frontend

Entre na pasta do app web:

```bash
cd src/SGX.SistemaChamado.Web
npm install
npm run dev
```

O arquivo `.env.local` deve conter:

```text
VITE_API_BASE_URL=http://localhost:5168
VITE_AUTH_MODO_LOCAL=true
```

Se preferir o atalho do repositorio:

```bash
./scripts/linux-start-web.sh
```

Abra:

- `http://localhost:5173/login`

## 10. Login local de desenvolvimento

Na tela de login:

- usuario: `admin@sgxdigital.com`
- senha: `Admin@123456`

Esse login so funciona em `Development` com `Authentication__ModoLocalHabilitado=true`.

## 11. Troubleshooting basico

### `dotnet-ef` nao encontrado

```bash
dotnet tool restore
dotnet tool run dotnet-ef --version
```

### PostgreSQL nao responde

```bash
pg_isready -h localhost -p 5432
sudo systemctl status postgresql
```

### Falha de autenticacao no banco

Confirme usuario, senha e dono do banco:

```bash
PGPASSWORD='1qaz@2wsx' psql -h localhost -p 5432 -U user_sgxsc -d postgres -c "select current_user;"
```

### API nao sobe na porta esperada

Confirme que nao existe outra instancia ocupando `5168` e que o perfil de execucao esta em `Development`.

### Frontend nao abre o login local

Confirme que existe `src/SGX.SistemaChamado.Web/.env.local` com `VITE_AUTH_MODO_LOCAL=true` e que a API esta em `http://localhost:5168`.
