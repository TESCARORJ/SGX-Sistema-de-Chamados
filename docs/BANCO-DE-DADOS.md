# Banco de Dados (PostgreSQL)

## Ambiente local

- Host: `localhost`
- Porta: `5432`
- Database: `sgx_sistema_chamados`
- Usuario: `user_sgxsc`

Use preferencialmente:

- `ConnectionStrings__DefaultConnection`

## Secrets

- Nao versionar senha real em `appsettings.json` de producao.
- Em producao, usar secret manager/cofre/variaveis de ambiente.

## Migrations

Migrations atuais:

- `InitialCreate`
- `AddEmailIntegrationLogSupport`

Comandos:

```bash
dotnet tool run dotnet-ef migrations list --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
dotnet tool run dotnet-ef database update --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
```

Para nova migration (somente com mudanca real de schema):

```bash
dotnet tool run dotnet-ef migrations add NomeMigration --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --output-dir Persistence/Migrations
```

## Tabelas principais

- `Chamados`
- `ComentariosChamado`
- `HistoricosChamado`
- `AnexosChamado`
- `SlaControles`
- `SlaConfiguracoes`
- `LogsIntegracaoEmail`
- `Usuarios`, `PerfisAcesso`, `UsuariosPerfisAcesso`
- `CategoriasChamado`, `PrioridadesChamado`, `StatusChamado`, `Departamentos`

## Estrategia de exclusao

- Cadastros administrativos usam inativacao (`Ativo=false`), evitando exclusao fisica.

## Logs de integracao de e-mail

- Persistidos em `LogsIntegracaoEmail`.
- Indexacao para filtros por status/data/chamado.
- Deduplicacao por `MessageId`/`Fingerprint`.

## SLA

- `SlaControle` registra prazos, pausas, primeira resposta e resolucao.
- Regras de calculo ficam na camada de aplicacao.

## Backup e restauracao (orientativo)

- Backup logico: `pg_dump`.
- Restore: `pg_restore`/`psql` conforme formato.
- Em homologacao/producao, manter rotina automatizada e testes de restore.
