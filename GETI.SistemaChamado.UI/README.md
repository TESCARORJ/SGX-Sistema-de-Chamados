# GETI.SistemaChamado.UI

Frontend do Sistema de Chamados CREA-RJ, implementado com Vue.js 3 e Quasar Framework.

## Requisitos
- Node.js 20+
- npm 10+
- Backend da API em execucao (`http://localhost:18080`)

## Instalacao
```bash
cd GETI.SistemaChamado.UI
npm install
```

## Variaveis de ambiente
Arquivo base: `.env.development`

Variavel principal:
- `VITE_API_BASE_URL=http://localhost:18080`

Opcional para desenvolvimento local com autenticacao tecnica:
- `VITE_AUTH_LOCAL_EMAIL`
- `VITE_AUTH_LOCAL_SENHA`

## Execucao local
```bash
npm run dev
```

URL padrao:
- `http://localhost:9000`

## Build
```bash
npm run build
```

## Integracao com backend
- Backend: `http://localhost:18080`
- Frontend: `http://localhost:9000`
- O frontend usa servico HTTP centralizado (`src/services/http/clienteHttp.ts`) e trata:
  - `401` usuario nao autenticado
  - `403` acesso negado
  - `500` erro interno do backend
  - falha de conexao com a API

## Estrutura de pastas
- `src/layouts`: layouts publico, portal e administrativo
- `src/pages`: paginas por contexto (`acesso`, `portal`, `admin`)
- `src/router`: definicao de rotas e guards
- `src/services`: integracao HTTP, sessao e servicos de dominio de UI
- `src/styles`: estilos globais
