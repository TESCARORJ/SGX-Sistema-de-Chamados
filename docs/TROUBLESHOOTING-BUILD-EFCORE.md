# Troubleshooting - Build/Debug e EF Core

## Objetivo
Padronizar diagnostico e mitigacao para o problema recorrente no desenvolvimento local:
- falha de build com `MSB3021`/`MSB3027` por DLL/PDB bloqueado;
- `PendingModelChangesWarning` no startup da API sem mudanca real de modelo.

## Erros MSB3021/MSB3027
`MSB3021` e `MSB3027` sao erros de copia de artefatos durante o build.

No SGX, o caso mais comum e:
- API em execucao no debugger;
- processo `dotnet`/`SGX.SistemaChamado.Api`/`Microsoft .NET Core Debugger` mantendo lock em:
  - `SGX.SistemaChamado.Infrastructure.dll`
  - `SGX.SistemaChamado.Infrastructure.pdb`
  - outros binarios referenciados.

Quando isso ocorre, o build pode ficar parcial (alguns assemblies atualizados e outros nao).

### Erro MSB3021/MSB3027 - DLL/PDB bloqueado pelo debugger
Este erro ocorre quando a API ainda esta em execucao/depuracao e o build tenta sobrescrever arquivos em `bin/Debug`.

No log, normalmente aparece quem esta bloqueando, por exemplo:
- `Microsoft .NET Core Debugger`
- `.NET Host`

Importante:
- isso nao e erro de migration;
- isso nao e erro de codigo de dominio;
- nao deve ser resolvido criando migration.

Correcao recomendada:
1. Pare a depuracao no Visual Studio/VS Code.
2. Mate processos travados (manual ou script).
3. Limpe `bin/obj`.
4. Rode `clean` e `build` novamente.

Comandos manuais:

```powershell
Stop-Process -Id 15032,18696 -Force

dotnet clean SGX.SistemaChamado.sln

Remove-Item -Recurse -Force ".\src\SGX.SistemaChamado.Api\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\src\SGX.SistemaChamado.Api\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\src\SGX.SistemaChamado.Infrastructure\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ".\src\SGX.SistemaChamado.Infrastructure\obj" -ErrorAction SilentlyContinue

dotnet build SGX.SistemaChamado.sln -c Debug
```

Scripts de apoio:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/dev-kill-locked-build-pids.ps1 -ProcessIds 15032,18696
powershell -ExecutionPolicy Bypass -File scripts/dev-unlock-dotnet-build.ps1
powershell -ExecutionPolicy Bypass -File scripts/dev-unlock-dotnet-build.ps1 -ForceKill
```

## O que e PendingModelChangesWarning
No EF Core, o warning indica diferenca entre:
- modelo atual carregado em runtime; e
- snapshot da ultima migration.

Isso pode ser:
1. migration realmente pendente; ou
2. falso positivo por binarios inconsistentes apos build quebrado por lock.

## Como diferenciar migration real de falso positivo
Execute sempre na raiz do repositorio:

```bash
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext
```

Interpretacao:
- retorno com `No changes have been made to the model since the last migration.`:
  - nao ha migration pendente no source atual;
  - suspeitar de lock de build e artefatos inconsistentes.
- retorno indicando mudancas pendentes:
  - revisar diff antes de criar migration;
  - gerar migration real somente com alteracao confirmada.

## Como identificar arquivos em uso
Sinais comuns:
- mensagens `The process cannot access the file ... because it is being used by another process.`;
- bloqueio por:
  - `Microsoft .NET Core Debugger`
  - `.NET Host`
  - `dotnet.exe`
  - processo da API.

Em IDE, pare debug antes do build. Em terminal, confira processos ativos.

## Fluxo recomendado de recuperacao
1. Pare depuracao e execucao da API/worker.
2. Rode reset conservador:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/dev-reset-build-locks.ps1
```

3. Se lock persistir em `dotnet.exe`, rode com opcao explicita:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/dev-reset-build-locks.ps1 -KillDotnet
```

4. Revalide modelo EF:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check-ef-model.ps1
```

5. Suba a API novamente.

## Regras de seguranca para migrations
- Nao criar migration automaticamente sem revisar o diff.
- Nao criar migration so porque houve `PendingModelChangesWarning` no startup.
- Nao suprimir `PendingModelChangesWarning` como primeira resposta.
- Nao remover `MigrateAsync` do startup apenas para esconder falha local.

## Diagnostico adicional (quando necessario)
Script auxiliar:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/diagnose-pending-migration.ps1
```

Ele orienta a geracao manual de migration temporaria `_DiagnosticoPending` apenas com confirmacao textual.

## Validacoes finais recomendadas
```bash
dotnet build SGX.SistemaChamado.sln -c Debug
dotnet build SGX.SistemaChamado.sln -c Release
dotnet test SGX.SistemaChamado.sln -c Release --no-build
```
