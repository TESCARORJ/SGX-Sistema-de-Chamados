$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Host "==> Verificando se ha mudancas pendentes no modelo EF Core" -ForegroundColor Cyan

$efArgs = @(
    'ef',
    'migrations',
    'has-pending-model-changes',
    '--project', 'src/SGX.SistemaChamado.Infrastructure',
    '--startup-project', 'src/SGX.SistemaChamado.Api',
    '--context', 'SGXSistemaChamadoDbContext'
)

$output = & dotnet @efArgs 2>&1
$exitCode = $LASTEXITCODE

$output | ForEach-Object { Write-Host $_ }

Write-Host ""
$normalizedOutput = ($output | Out-String)

if ($normalizedOutput -match 'No changes have been made to the model since the last migration\.') {
    Write-Host "[OK] Nao ha migration pendente no source atual." -ForegroundColor Green
    Write-Host "Se a API ainda falhar no startup, suspeite de build travado ou binarios inconsistentes." -ForegroundColor Yellow
    exit 0
}

$pendingPatterns = @(
    'has pending changes',
    'pending model change',
    'differences were found'
)
$hasPendingHint = $false
foreach ($pattern in $pendingPatterns) {
    if ($normalizedOutput -match $pattern) {
        $hasPendingHint = $true
        break
    }
}

if ($hasPendingHint) {
    Write-Host "[ATENCAO] Existem indicios de mudancas pendentes no modelo EF." -ForegroundColor Yellow
    Write-Host "Gerar migration real apenas apos revisar o diff." -ForegroundColor Yellow
    exit 2
}

if ($exitCode -ne 0) {
    Write-Host "[ERRO] Falha ao executar verificacao do modelo EF Core (codigo $exitCode)." -ForegroundColor Red
    Write-Host "Se a API falhar no startup sem mudanca real, suspeite de build travado ou binarios inconsistentes." -ForegroundColor Yellow
    exit $exitCode
}

Write-Host "[ATENCAO] Comando concluido, mas sem padrao conhecido na saida." -ForegroundColor Yellow
Write-Host "Revise a saida acima antes de criar qualquer migration." -ForegroundColor Yellow
exit 3
