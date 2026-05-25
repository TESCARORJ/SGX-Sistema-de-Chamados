param(
    [Parameter(Mandatory = $true)]
    [int[]]$ProcessIds
)

$ErrorActionPreference = 'Stop'

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

Write-Host "Encerrando PIDs informados..." -ForegroundColor Cyan

foreach ($processId in $ProcessIds | Select-Object -Unique) {
    $proc = Get-Process -Id $processId -ErrorAction SilentlyContinue
    if (-not $proc) {
        Write-InfoLine "PID $processId nao esta em execucao."
        continue
    }

    try {
        Stop-Process -Id $processId -Force -ErrorAction Stop
        Write-OkLine "Processo encerrado: $($proc.ProcessName) (PID $processId)."
    } catch {
        Write-WarnLine "Falha ao encerrar PID ${processId}: $($_.Exception.Message)"
    }
}

Write-Host "`nSugestao: executar em seguida:" -ForegroundColor Cyan
Write-Host "powershell -ExecutionPolicy Bypass -File scripts/dev-unlock-dotnet-build.ps1 -ForceKill"
