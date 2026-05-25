param(
    [switch]$ForceKill,
    [int[]]$ProcessIds
)

$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Write-InfoLine {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Gray
}

function Write-WarnLine {
    param([string]$Message)
    Write-Host "[AVISO] $Message" -ForegroundColor Yellow
}

function Write-OkLine {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Stop-ProcessSafelyById {
    param([int]$Id)

    try {
        $proc = Get-Process -Id $Id -ErrorAction Stop
        Stop-Process -Id $Id -Force -ErrorAction Stop
        Write-OkLine "Processo encerrado: $($proc.ProcessName) (PID $Id)."
    } catch {
        Write-WarnLine "Nao foi possivel encerrar PID ${Id}: $($_.Exception.Message)"
    }
}

function Resolve-CommandLine {
    param([int]$ProcessId)

    try {
        $proc = Get-CimInstance Win32_Process -Filter "ProcessId = $ProcessId" -ErrorAction Stop
        return [string]$proc.CommandLine
    } catch {
        return ''
    }
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Step "Diagnostico de lock de build .NET (MSB3021/MSB3027)"
Write-Host "Repositorio: $repoRoot"
Write-WarnLine "Esse erro costuma ocorrer quando API/debugger ainda estao em execucao."
Write-WarnLine "Pare a depuracao no Visual Studio/VS Code antes de continuar."

$targetNames = @('dotnet', 'SGX.SistemaChamado.Api', 'vsdbg', 'VBCSCompiler')
$allByName = foreach ($name in $targetNames) {
    Get-Process -Name $name -ErrorAction SilentlyContinue
}

$allByName = $allByName | Sort-Object ProcessName, Id -Unique

if (-not $allByName) {
    Write-InfoLine "Nenhum processo alvo encontrado por nome."
} else {
    Write-Step "Processos potencialmente bloqueando DLL/PDB"
    $diagnosticoProcessos = foreach ($proc in $allByName) {
        $commandLine = Resolve-CommandLine -ProcessId $proc.Id
        $shortCmd = if ([string]::IsNullOrWhiteSpace($commandLine)) { '' } else { $commandLine }
        [pscustomobject]@{
            PID = $proc.Id
            Nome = $proc.ProcessName
            CommandLine = $shortCmd
        }
    }

    $diagnosticoProcessos | Format-Table -AutoSize
}

if ($ProcessIds -and $ProcessIds.Count -gt 0) {
    Write-Step "Encerrando PIDs informados manualmente"
    foreach ($pid in $ProcessIds | Select-Object -Unique) {
        Stop-ProcessSafelyById -Id $pid
    }
}

if ($ForceKill) {
    Write-Step "ForceKill ativo: encerrando processos conhecidos de desenvolvimento"
    foreach ($name in $targetNames) {
        $procs = Get-Process -Name $name -ErrorAction SilentlyContinue
        if (-not $procs) {
            Write-InfoLine "Nenhum processo encontrado: $name"
            continue
        }

        foreach ($proc in $procs) {
            try {
                Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
                Write-OkLine "Encerrado: $($proc.ProcessName) (PID $($proc.Id))"
            } catch {
                Write-WarnLine "Falha ao encerrar $($proc.ProcessName) (PID $($proc.Id)): $($_.Exception.Message)"
            }
        }
    }
} else {
    Write-Step "Modo seguro (padrao)"
    Write-InfoLine "Nenhum processo sera encerrado automaticamente sem -ForceKill."
    Write-InfoLine "Se necessario, rode novamente com -ForceKill."
}

Write-Step "Limpando artefatos bin/obj"
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

foreach ($relativePath in $pathsToClean) {
    $fullPath = Join-Path $repoRoot $relativePath
    Remove-Item -Recurse -Force -LiteralPath $fullPath -ErrorAction SilentlyContinue
    if (Test-Path -LiteralPath $fullPath) {
        Write-WarnLine "Nao foi possivel remover totalmente: $relativePath"
    } else {
        Write-OkLine "Limpo: $relativePath"
    }
}

Write-Step "Executando limpeza da solucao"
& dotnet clean SGX.SistemaChamado.sln
$cleanExitCode = $LASTEXITCODE
if ($cleanExitCode -ne 0) {
    Write-WarnLine "dotnet clean retornou codigo $cleanExitCode."
}

Write-Step "Executando build Debug"
& dotnet build SGX.SistemaChamado.sln -c Debug
$buildExitCode = $LASTEXITCODE
if ($buildExitCode -ne 0) {
    Write-WarnLine "dotnet build Debug falhou com codigo $buildExitCode."
    Write-Host "Dica: pare o debugger e rode novamente com -ForceKill."
    exit $buildExitCode
}

Write-OkLine "Build Debug concluido com sucesso."
Write-Step "Proxima validacao recomendada"
Write-Host "dotnet build SGX.SistemaChamado.sln -c Release"
Write-Host "dotnet test SGX.SistemaChamado.sln -c Release --no-build"
