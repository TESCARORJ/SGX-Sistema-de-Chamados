# Frontend - Validacao de Rotas (Sprint UX 4)

## Escopo da sprint
Sprint de validacao visual final, refinamento de responsividade e fechamento de checklist de rotas.

Regras aplicadas nesta entrega:
- Sem alteracao de regra de negocio.
- Sem alteracao de backend por funcionalidade.
- Sem remocao de autenticacao.
- Sem remocao de login Microsoft.
- Login local mantido apenas para Development.

## Status de validacao
Legenda de status usada na tabela:
- `OK manual`
- `OK inspecao + build`
- `Pendente manual`
- `Corrigido nesta sprint`
- `Pendente real`

| Rota | Perfil necessario | Layout usado | Endpoint principal | Componentes Quasar principais | Status visual | Status funcional | Validacao | Observacoes |
|---|---|---|---|---|---|---|---|---|
| `/login` | Publico | `AuthLayout` | `POST login Microsoft`, `GET /api/me` | `QPage`, `QCard`, `QCardSection`, `QForm`, `QInput`, `QBtn`, `QBanner`, `QSeparator` | OK inspecao + build | OK inspecao + build | OK inspecao + build | Bloco local condicionado a `!PROD`; senha local nao e enviada ao backend e nao e persistida em storage. Validacao manual pendente por limitacao de `npm run dev` no ambiente. |
| `/acesso-negado` | Autenticado sem permissao | `AuthLayout` | Sem endpoint dedicado | `QPage`, `QCard`, `QCardSection`, `QCardActions`, `QIcon`, `QBtn` | OK inspecao + build | OK inspecao + build | OK inspecao + build | Tela amigavel e navegavel; botao de destino muda conforme autenticacao. |
| `/portal` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/chamados` | `QPage`, `QBtn`, `QCard`, `PageHeader`, `MetricCard`, `AppSectionCard`, `LoadingState`, `ErrorState`, `EmptyState` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Ajuste responsivo de header/acoes via componente compartilhado. |
| `/portal/chamados` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/contexto`, `GET /api/portal/chamados` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QCard`, `QBtn`, `StatusBadge`, `PrioridadeBadge`, `SlaBadge` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Tabela com `grid` em mobile e estado vazio/loading/erro presentes. |
| `/portal/chamados/novo` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/contexto`, `POST /api/portal/chamados`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `QForm`, `QInput`, `QSelect`, `QList`, `QItem`, `QBtn`, `QBanner`, `UploadAnexo` | OK inspecao + build | OK inspecao + build | OK inspecao + build | Formulario com validacao visual e botoes com loading assincrono. |
| `/portal/chamados/:id` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/chamados/{id}`, `POST /api/portal/chamados/{id}/comentarios`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `QList`, `QItem`, `QCard`, `QChip`, `QBtn`, `QTimeline`, `QForm`, `QInput` | OK inspecao + build | OK inspecao + build | OK inspecao + build | Mensagem amigavel para 403 validada por inspecao de codigo. |
| `/admin` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/contexto`, `GET /api/admin/dashboard` | `QLayout`, `QHeader`, `QDrawer`, `QPageContainer`, `QPage`, `PageHeader`, `MetricCard`, `AppSectionCard` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Layout administrativo com drawer responsivo e sem menu horizontal indevido. |
| `/admin/chamados` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/contexto`, `GET /api/admin/chamados`, `POST /api/admin/chamados/{id}/assumir` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QPagination`, `QBtn`, `QBanner` | OK inspecao + build | OK inspecao + build | OK inspecao + build | `QTable` com cards em mobile; feedback de sucesso/erro presente. |
| `/admin/chamados/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/chamados/{id}` + mutacoes de atendimento | `QPage`, `QList`, `QItem`, `QBtn`, `QDialog`, `QTimeline`, `QToggle`, `QInput`, `QBanner` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Dialogos e painel de acoes ajustados para mobile (largura fluida e botoes empilhaveis). |
| `/admin/cadastros/usuarios` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/usuarios` | `QPage`, `PageHeader`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Lista padronizada por `CadastroListaBaseView` com `QTable` e estados completos. |
| `/admin/cadastros/usuarios/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/usuarios*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `QBadge`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | `Atendente` em leitura; mutacoes somente para `Administrador`. |
| `/admin/cadastros/perfis` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/perfis` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Header/acoes responsivos via componentes compartilhados. |
| `/admin/cadastros/perfis/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/perfis*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Confirmacoes feitas com `QDialog`/`ConfirmDialog`, sem `alert/confirm` nativo. |
| `/admin/cadastros/departamentos` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/departamentos` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Estados de loading/erro/vazio presentes na base de listagem. |
| `/admin/cadastros/departamentos/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/departamentos*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Formulario padronizado com validacao visual. |
| `/admin/cadastros/categorias` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/categorias` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Lista responsiva por `grid` em mobile na tabela administrativa. |
| `/admin/cadastros/categorias/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/categorias*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Fluxo de salvar/voltar/inativar/reativar validado por inspecao. |
| `/admin/cadastros/prioridades` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/prioridades` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Colunas de SLA mantidas e sem quebra visual na listagem. |
| `/admin/cadastros/prioridades/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/prioridades*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Campos numericos com regra de nao negativo. |
| `/admin/cadastros/status` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/status` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Lista com badges ativo/inativo e acoes alinhadas. |
| `/admin/cadastros/status/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/status*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Toggle de status final/pausa SLA mantido no `QForm`. |
| `/admin/configuracoes/parametros` | `Administrador` | `AdminLayout` | `GET /api/admin/configuracoes/parametros` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Valor sensivel mascarado (`********`) e badge `Sensivel` na listagem. |
| `/admin/configuracoes/parametros/:id` | `Administrador` | `AdminLayout` | `GET/POST/PUT /api/admin/configuracoes/parametros*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | Mutacoes restritas a `Administrador` por guard + tela em leitura para nao admin. |
| `/admin/integracoes/email` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/integracoes/email/logs`, `GET /api/admin/integracoes/email/logs/{id}` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QDialog`, `QExpansionItem`, `PageHeader`, `AppSectionCard` | OK inspecao + build | OK inspecao + build | OK inspecao + build | Detalhe em dialogo com erro tecnico em `QExpansionItem`; sem dominancia visual da falha tecnica. |

## Validacao de guards e permissoes (inspecao de codigo)
- Nao autenticado redireciona para `/login` (`router.beforeEach` + `requiresAuth`).
- `Solicitante` sem perfil admin e redirecionado para `/acesso-negado` ao tentar `/admin`.
- `Atendente` e `Administrador` acessam `/admin`.
- Rotas de parametros (`/admin/configuracoes/parametros*`) restritas a `Administrador`.
- Mutacoes de cadastros visiveis apenas para `Administrador` (acao `Novo`, salvar/inativar/reativar).
- Login local admin em Development redireciona para `/admin`.

## Responsividade revisada (inspecao + build)
Breakpoints revisados por estrutura e classes:
- Desktop 1366px: sem anomalia estrutural identificada.
- Notebook 1024px: drawers com breakpoint 1024, acoes mantidas.
- Tablet 768px: headers/cards/filtros empilham sem overflow estrutural.
- Mobile 390px: ajustes aplicados em dialogs administrativos, painel de acoes, headers e paginacao para evitar quebra.

## Comandos executados
Backend:
- `dotnet build SGX.SistemaChamado.sln` -> **falhou no ambiente** (exit code 1, sem erros compilacao exibidos; falha recorrente no restore graph/MSBuild em .NET SDK 10.0.103).
- `dotnet test SGX.SistemaChamado.sln` -> **falhou no ambiente** (exit code 1, bloqueado pela mesma cadeia de restore/build).

Frontend:
- `npm install` -> **OK**.
- `npm run build` -> **OK**.
- `npm run lint` -> **nao existe script `lint` no `package.json`**.
- `npm run dev` -> **falhou no ambiente** com `Error: spawn EPERM` (Vite/esbuild), impedindo navegacao manual no browser dentro deste ambiente.

## Pendencias reais
- `Pendente real`: validar manualmente as rotas no navegador (`http://localhost:5173/login`) em ambiente com permissao para subir `npm run dev` sem `spawn EPERM`.
- `Pendente real`: concluir `dotnet build` e `dotnet test` em ambiente com cadeia de restore funcional para a solucao completa.

## Status final da UX 4
- **Nao apto para homologacao visual final neste ambiente**, por limitacoes reais de execucao (`npm run dev` indisponivel para navegacao manual e `dotnet build/test` falhando por problema de restore/build do ambiente).
- **Apto por inspecao + build frontend** para continuidade de homologacao assim que o ambiente permitir validacao manual.
