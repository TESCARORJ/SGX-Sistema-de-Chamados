$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

Write-Host "==> Diagnostico de pending model changes (SGX)" -ForegroundColor Cyan
Write-Host "1) Executando check do modelo EF Core..." -ForegroundColor Gray

& powershell -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot 'check-ef-model.ps1')
$checkExitCode = $LASTEXITCODE

if ($checkExitCode -eq 0) {
    Write-Host ""
    Write-Host "[OK] Modelo sem mudancas pendentes no source atual." -ForegroundColor Green
    Write-Host "Se o startup continuar falhando, priorize diagnostico de lock/build inconsistente." -ForegroundColor Yellow
    exit 0
}

if ($checkExitCode -ne 2) {
    Write-Host ""
    Write-Host "[ERRO] Nao foi possivel concluir o diagnostico automatico (codigo $checkExitCode)." -ForegroundColor Red
    Write-Host "Corrija o erro acima e execute novamente." -ForegroundColor Yellow
    exit $checkExitCode
}

Write-Host ""
Write-Host "[ATENCAO] Existem indicios de mudancas pendentes no modelo." -ForegroundColor Yellow
Write-Host "Recomendacao: gerar migration temporaria de diagnostico e revisar o diff." -ForegroundColor Yellow
Write-Host ""
Write-Host "Comando sugerido:" -ForegroundColor Gray
Write-Host "dotnet ef migrations add _DiagnosticoPending --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Digite CRIAR-DIAGNOSTICO para executar agora ou pressione Enter para sair sem criar migration"
if ($confirm -ne 'CRIAR-DIAGNOSTICO') {
    Write-Host "Nenhuma migration foi criada." -ForegroundColor Gray
    exit 0
}

& dotnet ef migrations add _DiagnosticoPending --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext
$migrationExitCode = $LASTEXITCODE
if ($migrationExitCode -ne 0) {
    Write-Host "[ERRO] Falha ao criar migration de diagnostico (codigo $migrationExitCode)." -ForegroundColor Red
    exit $migrationExitCode
}

Write-Host ""
Write-Host "[OK] Migration de diagnostico criada." -ForegroundColor Green
Write-Host "Proximos passos obrigatorios:" -ForegroundColor Cyan
Write-Host "- Se houver mudancas reais, revisar e renomear adequadamente antes de manter." -ForegroundColor White
Write-Host "- Se vier vazia ou com ruido, remover migration e investigar snapshot/configuracao." -ForegroundColor White
