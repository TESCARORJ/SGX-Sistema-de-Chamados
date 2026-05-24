param(
    [switch]$KillDotnet
)

$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Write-WarnLine {
    param([string]$Message)
    Write-Host "[AVISO] $Message" -ForegroundColor Yellow
}

function Write-OkLine {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-InfoLine {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Gray
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Step "Reset conservador de locks de build/debug (SGX)"
Write-Host "Repositorio: $repoRoot"
Write-WarnLine "Pare o debugger da API/Worker antes de continuar."
Write-WarnLine "Sem -KillDotnet, este script nao encerra dotnet.exe automaticamente."

$processes = Get-CimInstance Win32_Process
$relatedProcesses = $processes | Where-Object {
    ($_.Name -in @('SGX.SistemaChamado.Api.exe', 'SGX.SistemaChamado.Worker.Email.exe', 'vsdbg.exe', 'VSCodeDebugAdapterHost.exe')) -or
    ($_.CommandLine -like '*SGX.SistemaChamado*')
}

if (-not $relatedProcesses) {
    Write-InfoLine "Nenhum processo relacionado ao SGX foi identificado."
} else {
    Write-Step "Processos possivelmente relacionados"
    $relatedProcesses |
        Sort-Object Name, ProcessId |
        Select-Object ProcessId, Name, CommandLine |
        Format-Table -AutoSize
}

Write-Step "Encerrando processos relacionados (modo seguro)"
foreach ($proc in $relatedProcesses) {
    $name = [string]$proc.Name
    $id = [int]$proc.ProcessId
    $commandLine = [string]$proc.CommandLine

    if ($name -ieq 'dotnet.exe' -and -not $KillDotnet) {
        Write-WarnLine "Ignorando dotnet.exe PID $id (use -KillDotnet para encerrar dotnet relacionado ao SGX)."
        continue
    }

    if ($name -ieq 'dotnet.exe' -and $commandLine -notlike '*SGX.SistemaChamado*') {
        Write-WarnLine "Ignorando dotnet.exe PID $id por nao parecer relacionado ao SGX."
        continue
    }

    try {
        Stop-Process -Id $id -Force -ErrorAction Stop
        Write-OkLine "$name (PID $id) encerrado."
    } catch {
        Write-WarnLine "Nao foi possivel encerrar $name (PID $id): $($_.Exception.Message)"
    }
}

if ($KillDotnet) {
    $stillRunningDotnet = Get-Process -Name dotnet -ErrorAction SilentlyContinue
    if ($stillRunningDotnet) {
        Write-WarnLine "Ainda existem processos dotnet.exe ativos."
        Write-WarnLine "Para manter seguranca, o script NAO mata todos globalmente sem confirmacao."
        $confirmation = Read-Host "Digite MATAR-TODOS para encerrar todos os dotnet.exe restantes (acao ampla), ou pressione Enter para manter seguro"
        if ($confirmation -eq 'MATAR-TODOS') {
            foreach ($dotnetProc in $stillRunningDotnet) {
                try {
                    Stop-Process -Id $dotnetProc.Id -Force -ErrorAction Stop
                    Write-OkLine "dotnet.exe PID $($dotnetProc.Id) encerrado."
                } catch {
                    Write-WarnLine "Nao foi possivel encerrar dotnet.exe PID $($dotnetProc.Id): $($_.Exception.Message)"
                }
            }
        } else {
            Write-InfoLine "Mantido modo seguro: dotnet.exe global nao foi encerrado."
        }
    }
}

Write-Step "Aguardando liberacao de arquivos"
Start-Sleep -Seconds 1

$pathsToClean = @(
    'src/SGX.SistemaChamado.Api/bin',
    'src/SGX.SistemaChamado.Api/obj',
    'src/SGX.SistemaChamado.Infrastructure/bin',
    'src/SGX.SistemaChamado.Infrastructure/obj',
    'src/SGX.SistemaChamado.Application/bin',
    'src/SGX.SistemaChamado.Application/obj',
    'src/SGX.SistemaChamado.Domain/bin',
    'src/SGX.SistemaChamado.Domain/obj',
    'tests/SGX.SistemaChamado.Tests/bin',
    'tests/SGX.SistemaChamado.Tests/obj'
)

Write-Step "Limpando bin/obj dos projetos principais"
foreach ($relativePath in $pathsToClean) {
    $fullPath = Join-Path $repoRoot $relativePath

    if (-not (Test-Path -LiteralPath $fullPath)) {
        Write-InfoLine "Nao encontrado: $relativePath"
        continue
    }

    try {
        Remove-Item -LiteralPath $fullPath -Recurse -Force -ErrorAction Stop
        Write-OkLine "Removido: $relativePath"
    } catch {
        Write-WarnLine "Falha ao remover ${relativePath}: $($_.Exception.Message)"
    }
}

Write-Step "Executando dotnet clean"
& dotnet clean SGX.SistemaChamado.sln
$cleanExitCode = $LASTEXITCODE
if ($cleanExitCode -ne 0) {
    Write-WarnLine "dotnet clean retornou codigo $cleanExitCode."
} else {
    Write-OkLine "dotnet clean concluido."
}

Write-Step "Executando dotnet build -c Debug"
& dotnet build SGX.SistemaChamado.sln -c Debug
$buildExitCode = $LASTEXITCODE
if ($buildExitCode -ne 0) {
    Write-WarnLine "dotnet build Debug falhou com codigo $buildExitCode."
    Write-Host "Revise locks ativos e rode novamente com -KillDotnet se necessario." -ForegroundColor Yellow
    exit $buildExitCode
}

Write-OkLine "dotnet build Debug concluido com sucesso."
Write-Step "Proximos passos"
Write-Host "1. powershell -ExecutionPolicy Bypass -File scripts/check-ef-model.ps1"
Write-Host "2. Reiniciar API/Debug normalmente."
