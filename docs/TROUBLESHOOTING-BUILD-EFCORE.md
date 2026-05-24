# Troubleshooting - Build/Startup EF Core (PendingModelChangesWarning + locks de DLL/PDB)

## Contexto
Este guia cobre falhas de inicializacao/build no SGX Sistema de Chamados quando aparecem juntos:
- `PendingModelChangesWarning` durante `MigrateAsync` no startup da API.
- erros `MSB3021`/`MSB3027` ao copiar DLL/PDB (arquivo em uso por outro processo).

## O que significa PendingModelChangesWarning
A partir do EF Core 9, diferencas entre o modelo atual e o ultimo snapshot de migration podem gerar excecao durante `Migrate`/`MigrateAsync`.

Em termos praticos, pode significar:
1. **Migration realmente pendente** (mudanca de modelo sem migration).
2. **Assemblies inconsistentes** em runtime/build (ex.: `Infrastructure.dll` antigo + `Domain.dll` novo), normalmente por lock de arquivo em `bin/obj`.

## Como diferenciar migration pendente real de assembly inconsistente

### 1) Verificar modelo via EF CLI
Execute na raiz do repositorio:

```bash
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext
```

Interprete o resultado:
- Se retornar **`No changes have been made to the model since the last migration.`**:
  - nao ha migration pendente no source atual;
  - suspeite de build travado/assemblies misturados.
- Se indicar mudancas pendentes reais:
  - crie migration apropriada;
  - aplique migration antes de subir a API.

### 2) Verificar erros de lock em build
Sinais comuns:
- `MSB3021` e `MSB3027`
- mensagens do tipo: `The process cannot access the file ... because it is being used by another process.`
- processos bloqueando: `Microsoft .NET Core Debugger`, `.NET Host`, `dotnet.exe`, processo da API.

Quando isso ocorre, o build pode terminar com artefatos parcialmente atualizados, gerando falso positivo de `PendingModelChangesWarning` na inicializacao.

## Fluxo recomendado de recuperacao (Windows)

1. Pare depuracao e instancias em execucao da API/Worker.
2. Rode o reset conservador:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/dev-reset-build-locks.ps1
```

3. Se ainda houver lock de `dotnet.exe`, rode com opcao explicita:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/dev-reset-build-locks.ps1 -KillDotnet
```

4. Revalide o modelo EF:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check-ef-model.ps1
```

5. Suba a API novamente.

## Scripts de apoio

- `scripts/dev-reset-build-locks.ps1`
  - identifica processos relacionados ao SGX;
  - por padrao **nao** encerra `dotnet.exe`;
  - com `-KillDotnet`, encerra apenas `dotnet.exe` relacionado ao SGX;
  - remove `bin/obj` de projetos principais;
  - executa `dotnet clean` e `dotnet build -c Debug`.

- `scripts/check-ef-model.ps1`
  - executa verificacao de `has-pending-model-changes`;
  - orienta se o problema e migration real ou suspeita de lock/inconsistencia de build.

## O que nao fazer como primeira acao
- Nao suprimir `PendingModelChangesWarning` globalmente.
- Nao remover migracao automatica no startup sem decisao arquitetural.
- Nao criar migration nova quando o EF CLI diz que nao ha mudancas.
- Nao mascarar erro real de migration.

## Observacao sobre startup da API
A API aplica migrations automaticamente no startup. Isso e esperado e util para detectar divergencia real.

Quando houver falha em `MigrateAsync`, revise:
1. se ha migration pendente real via EF CLI;
2. se ha lock de DLL/PDB no build local.

## Comandos de validacao final

```bash
dotnet build SGX.SistemaChamado.sln -c Debug
dotnet build SGX.SistemaChamado.sln -c Release
dotnet test SGX.SistemaChamado.sln -c Release --no-build
```