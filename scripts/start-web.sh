#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
WEB_DIR="${ROOT_DIR}/src/SGX.SistemaChamado.Web"

cd "$WEB_DIR"

if [ ! -f ".env.local" ]; then
  cat > .env.local <<'EOF'
VITE_API_BASE_URL=http://localhost:5168
VITE_AUTH_MODO_LOCAL=true
EOF
fi

npm run dev
