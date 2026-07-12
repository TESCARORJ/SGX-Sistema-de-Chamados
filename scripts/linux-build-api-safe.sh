#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "${script_dir}/.." && pwd)"
project_path="src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj"

cd "$repo_root"

echo "Build pontual da API do SGX"
echo "Projeto: ${project_path}"
echo "Configuracao: UseSharedCompilation=false, node reuse desabilitado, paralelismo m:1"
echo "Aviso: este script nao executa testes."

start_ts="$(date +%s)"

dotnet build-server shutdown

export MSBUILDDISABLENODEREUSE=1

dotnet build \
  "$project_path" \
  /p:UseSharedCompilation=false \
  /nr:false \
  -m:1

end_ts="$(date +%s)"
duration="$((end_ts - start_ts))"

echo "Build concluido com sucesso."
echo "Duracao aproximada: ${duration}s"
