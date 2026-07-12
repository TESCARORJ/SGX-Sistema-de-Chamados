#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"

cd "$ROOT_DIR"

export ASPNETCORE_ENVIRONMENT=Development
export Authentication__ModoLocalHabilitado=true

dotnet run --project src/SGX.SistemaChamado.Api
