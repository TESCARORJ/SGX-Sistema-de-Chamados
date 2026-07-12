#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd -- "${script_dir}/.." && pwd)"
web_dir="${repo_root}/src/SGX.SistemaChamado.Web"

cd "$web_dir"

if [[ ! -f ".env.local" ]]; then
  cat > ".env.local" <<'EOF'
VITE_API_BASE_URL=http://localhost:5168
VITE_AUTH_MODO_LOCAL=true
EOF
  echo "Arquivo .env.local criado com os valores esperados para Development."
fi

echo "Instalando dependencias do frontend, se necessario..."
npm install

echo "Iniciando Vite em http://localhost:5173/login"
npm run dev
