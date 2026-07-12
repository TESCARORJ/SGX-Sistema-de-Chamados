#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "${script_dir}/.." && pwd)"

cd "$repo_root"

echo "Restaurando ferramentas locais do .NET..."
dotnet tool restore

echo "Aplicando migrations no PostgreSQL local..."
dotnet tool run dotnet-ef database update \
  --project src/SGX.SistemaChamado.Infrastructure \
  --startup-project src/SGX.SistemaChamado.Api \
  --context SGXSistemaChamadoDbContext
