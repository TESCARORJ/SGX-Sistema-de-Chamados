#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "${script_dir}/.." && pwd)"

cd "$repo_root"

export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"
export DOTNET_ENVIRONMENT="${DOTNET_ENVIRONMENT:-Development}"
export Authentication__ModoLocalHabilitado="${Authentication__ModoLocalHabilitado:-true}"

echo "Iniciando API em Development com modo local habilitado."
echo "URL esperada: http://localhost:5168"

dotnet run --project src/SGX.SistemaChamado.Api
