$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Host "==> Verificando se ha mudancas pendentes no modelo EF Core" -ForegroundColor Cyan

$args = @(
    'ef',
    'migrations',
    'has-pending-model-changes',
    '--project', 'src/SGX.SistemaChamado.Infrastructure',
    '--startup-project', 'src/SGX.SistemaChamado.Api',
    '--context', 'SGXSistemaChamadoDbContext'
)

$output = & dotnet @args 2>&1
$exitCode = $LASTEXITCODE

$output | ForEach-Object { Write-Host $_ }

Write-Host ""
if ($output -match 'No changes have been made to the model since the last migration\.') {
    Write-Host "[OK] Nao ha migration pendente no modelo atual." -ForegroundColor Green
    Write-Host "Se a API ainda falhar no startup, suspeite de build travado/assembly inconsistente (DLL/PDB bloqueado)." -ForegroundColor Yellow
} elseif ($exitCode -eq 0) {
    Write-Host "[ATENCAO] O comando concluiu sem erro, mas nao retornou a mensagem padrao de ausencia de mudancas." -ForegroundColor Yellow
    Write-Host "Revise a saida acima e confirme se existe mudanca de modelo pendente." -ForegroundColor Yellow
} else {
    Write-Host "[ERRO] Falha ao executar verificacao de modelo EF Core (codigo $exitCode)." -ForegroundColor Red
    Write-Host "Se a saida indicar mudancas pendentes reais, gere migration apropriada antes de atualizar o banco." -ForegroundColor Yellow
    exit $exitCode
}