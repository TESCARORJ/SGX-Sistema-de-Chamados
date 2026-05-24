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

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Step "SGX dev reset para locks de build (Windows)"
Write-Host "Repositorio: $repoRoot"
Write-WarnLine "Este script pode encerrar processos de debug/API relacionados ao SGX para liberar DLL/PDB bloqueados."
Write-WarnLine "Sem -KillDotnet, processos dotnet.exe nao serao encerrados automaticamente."

$processes = Get-CimInstance Win32_Process

$relatedProcesses = $processes | Where-Object {
    ($_.Name -in @('SGX.SistemaChamado.Api.exe', 'SGX.SistemaChamado.Worker.Email.exe', 'vsdbg.exe', 'VSCodeDebugAdapterHost.exe')) -or
    ($_.CommandLine -like '*SGX.SistemaChamado*')
}

if (-not $relatedProcesses) {
    Write-Host "Nenhum processo relacionado ao SGX foi identificado." -ForegroundColor DarkGray
} else {
    Write-Step "Processos relacionados identificados"
    $relatedProcesses |
        Sort-Object Name, ProcessId |
        Select-Object ProcessId, Name, CommandLine |
        Format-Table -AutoSize
}

Write-Step "Tentando encerrar processos relacionados"

foreach ($proc in $relatedProcesses) {
    $name = [string]$proc.Name
    $id = [int]$proc.ProcessId
    $commandLine = [string]$proc.CommandLine

    if ($name -ieq 'dotnet.exe') {
        if (-not $KillDotnet) {
            Write-WarnLine "Ignorando dotnet.exe PID $id (use -KillDotnet se quiser encerrar dotnet relacionado ao SGX)."
            continue
        }

        if ($commandLine -notlike '*SGX.SistemaChamado*') {
            Write-WarnLine "Ignorando dotnet.exe PID $id porque nao parece relacionado ao SGX."
            continue
        }
    }

    try {
        Write-Host "Encerrando $name (PID $id)..." -ForegroundColor Yellow
        Stop-Process -Id $id -Force -ErrorAction Stop
        Write-OkLine "$name (PID $id) encerrado."
    } catch {
        Write-WarnLine "Nao foi possivel encerrar $name (PID $id): $($_.Exception.Message)"
    }
}

Write-Step "Aguardando liberacao de handles"
Start-Sleep -Seconds 1

$pathsToClean = @(
    'src/SGX.SistemaChamado.Api/bin',
    'src/SGX.SistemaChamado.Api/obj',
    'src/SGX.SistemaChamado.Infrastructure/bin',
    'src/SGX.SistemaChamado.Infrastructure/obj',
    'src/SGX.SistemaChamado.Domain/bin',
    'src/SGX.SistemaChamado.Domain/obj',
    'src/SGX.SistemaChamado.Application/bin',
    'src/SGX.SistemaChamado.Application/obj',
    'tests/SGX.SistemaChamado.Tests/bin',
    'tests/SGX.SistemaChamado.Tests/obj'
)

Write-Step "Limpando pastas bin/obj"
foreach ($relativePath in $pathsToClean) {
    $fullPath = Join-Path $repoRoot $relativePath

    if (-not (Test-Path -LiteralPath $fullPath)) {
        Write-Host "Nao encontrado: $relativePath" -ForegroundColor DarkGray
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
if ($LASTEXITCODE -ne 0) {
    Write-WarnLine "dotnet clean retornou codigo $LASTEXITCODE."
} else {
    Write-OkLine "dotnet clean concluido."
}

Write-Step "Executando dotnet build Debug"
& dotnet build SGX.SistemaChamado.sln -c Debug
$buildExitCode = $LASTEXITCODE
if ($buildExitCode -ne 0) {
    Write-WarnLine "dotnet build Debug falhou com codigo $buildExitCode."
    Write-Host "Dica: rode este script novamente com -KillDotnet caso ainda exista lock de dotnet.exe." -ForegroundColor Yellow
    exit $buildExitCode
}

Write-OkLine "dotnet build Debug concluido com sucesso."

Write-Step "Proximos passos"
Write-Host "1. Validar modelo EF: scripts/check-ef-model.ps1"
Write-Host "2. Iniciar API novamente (dotnet run ou debug pelo IDE)."
Write-Host "3. Se lock persistir, feche sessoes de debug e execute este script com -KillDotnet."
