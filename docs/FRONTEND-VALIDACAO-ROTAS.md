# Frontend - validação de Rotas (Sprint UX 5)

## Escopo da sprint
Sprint de consolidacao do design system institucional, modernizacao do dashboard administrativo e padronizacao visual das rotas do portal/admin.

Regras aplicadas nesta entrega:
- Sem alteracao de regra de negocio.
- Sem alteracao de backend por funcionalidade.
- Sem remocao de autenticação.
- Sem remocao de login Microsoft.
- Login local mantido apenas para Development.

## Status de validação
Legenda de status usada na tabela:
- `OK manual`
- `OK inspecao + build`
- `Pendente manual`
- `Corrigido nesta sprint`
- `Pendente real`

| Rota | Perfil necessario | Layout usado | Endpoint principal | Componentes Quasar principais | Status visual | Status funcional | validação | Aproximacao da proposta visual | Observacoes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `/login` | público | `AuthLayout` | `POST login Microsoft`, `GET /api/me` | `QPage`, `QCard`, `QCardSection`, `QForm`, `QInput`, `QBtn`, `QBanner`, `QSeparator` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Bloco local condicionado a `!PROD`; senha local não e enviada ao backend e não e persistida em storage. validação manual pendente por limitacao de `npm run dev` no ambiente. |
| `/acesso-negado` | Autenticado sem permissão | `AuthLayout` | Sem endpoint dedicado | `QPage`, `QCard`, `QCardSection`, `QCardActions`, `QIcon`, `QBtn` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Tela amigavel e navegavel; botao de destino muda conforme autenticação. |
| `/portal` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/chamados` | `QPage`, `QBtn`, `QCard`, `PageHeader`, `MetricCard`, `AppSectionCard`, `LoadingState`, `ErrorState`, `EmptyState` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Ajuste responsivo de header/acoes via componente compartilhado. |
| `/portal/chamados` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/contexto`, `GET /api/portal/chamados` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QCard`, `QBtn`, `StatusBadge`, `PrioridadeBadge`, `SlaBadge` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Tabela com `grid` em mobile e estado vazio/loading/erro presentes. |
| `/portal/chamados/novo` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/contexto`, `POST /api/portal/chamados`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `QForm`, `QInput`, `QSelect`, `QList`, `QItem`, `QBtn`, `QBanner`, `UploadAnexo` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Formulario com validação visual e botoes com loading assincrono. |
| `/portal/chamados/:id` | `Solicitante`, `Atendente`, `Administrador` | `PortalLayout` | `GET /api/portal/chamados/{id}`, `POST /api/portal/chamados/{id}/comentarios`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `QList`, `QItem`, `QCard`, `QChip`, `QBtn`, `QTimeline`, `QForm`, `QInput` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Mensagem amigavel para 403 validada por inspecao de codigo. |
| `/admin` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/contexto`, `GET /api/admin/dashboard` | `QLayout`, `QHeader`, `QDrawer`, `QPageContainer`, `QPage`, `PageHeader`, `MetricCard`, `AppSectionCard` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Layout administrativo com drawer responsivo e sem menu horizontal indevido. |
| `/admin/chamados` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/contexto`, `GET /api/admin/chamados`, `POST /api/admin/chamados/{id}/assumir` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QPagination`, `QBtn`, `QBanner` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | `QTable` com cards em mobile; feedback de sucesso/erro presente. |
| `/admin/notificacoes` | `Administrador`, `Atendente` | `AdminLayout` | Dados locais centralizados em `notificacoesStore` (preparado para API futura) | `QPage`, `PageHeader`, `AppSectionCard`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QChip`, `QDialog`, `QExpansionItem`, `QBtn`, `EmptyState`, `ErrorState`, `LoadingState` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Central de notificacoes administrativa com filtros, detalhe e leitura; preparada para futura integração com API de notificacoes. |
| `/admin/chamados/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/chamados/{id}` + mutacoes de atendimento | `QPage`, `QList`, `QItem`, `QBtn`, `QDialog`, `QTimeline`, `QToggle`, `QInput`, `QBanner` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Dialogos e painel de acoes ajustados para mobile (largura fluida e botoes empilhaveis). |
| `/admin/cadastros/usuarios` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/usuarios` | `QPage`, `PageHeader`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Lista padronizada por `CadastroListaBaseView` com `QTable` e estados completos. |
| `/admin/cadastros/usuarios/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/usuarios*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `QBadge`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | `Atendente` em leitura; mutacoes somente para `Administrador`. |
| `/admin/cadastros/perfis` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/perfis` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Header/acoes responsivos via componentes compartilhados. |
| `/admin/cadastros/perfis/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/perfis*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Confirmacoes feitas com `QDialog`/`ConfirmDialog`, sem `alert/confirm` nativo. |
| `/admin/cadastros/departamentos` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/departamentos` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Estados de loading/erro/vazio presentes na base de listagem. |
| `/admin/cadastros/departamentos/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/departamentos*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Formulario padronizado com validação visual. |
| `/admin/cadastros/categorias` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/categorias` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Lista responsiva por `grid` em mobile na tabela administrativa. |
| `/admin/cadastros/categorias/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/categorias*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Fluxo de salvar/voltar/inativar/reativar validado por inspecao. |
| `/admin/cadastros/prioridades` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/prioridades` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Colunas de SLA mantidas e sem quebra visual na listagem. |
| `/admin/cadastros/prioridades/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/prioridades*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Campos numericos com regra de não negativo. |
| `/admin/cadastros/status` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/cadastros/status` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Lista com badges ativo/inativo e acoes alinhadas. |
| `/admin/cadastros/status/:id` | `Administrador`, `Atendente` | `AdminLayout` | `GET/POST/PUT /api/admin/cadastros/status*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Toggle de status final/pausa SLA mantido no `QForm`. |
| `/admin/configuracoes/parametros` | `Administrador` | `AdminLayout` | `GET /api/admin/configuracoes/parametros` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QBtn`, `QPagination` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Valor sensível mascarado (`********`) e badge `sensível` na listagem. |
| `/admin/configuracoes/parametros/:id` | `Administrador` | `AdminLayout` | `GET/POST/PUT /api/admin/configuracoes/parametros*` | `QPage`, `QForm`, `QInput`, `QSelect`, `QToggle`, `QBtn`, `QDialog`, `ConfirmDialog` | Corrigido nesta sprint | OK inspecao + build | OK inspecao + build | OK | Mutacoes restritas a `Administrador` por guard + tela em leitura para não admin. |
| `/admin/integracoes/email` | `Administrador`, `Atendente` | `AdminLayout` | `GET /api/admin/integracoes/email/logs`, `GET /api/admin/integracoes/email/logs/{id}` | `QPage`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QDialog`, `QExpansionItem`, `PageHeader`, `AppSectionCard` | OK inspecao + build | OK inspecao + build | OK inspecao + build | OK | Detalhe em dialogo com erro técnico em `QExpansionItem`; sem dominancia visual da falha tecnica. |

## validação de guards e permissoes (inspecao de codigo)
- não autenticado redireciona para `/login` (`router.beforeEach` + `requiresAuth`).
- `Solicitante` sem perfil admin e redirecionado para `/acesso-negado` ao tentar `/admin`.
- `Atendente` e `Administrador` acessam `/admin`.
- Rotas de parâmetros (`/admin/configuracoes/parametros*`) restritas a `Administrador`.
- Mutacoes de cadastros visiveis apenas para `Administrador` (acao `Novo`, salvar/inativar/reativar).
- Login local admin em Development redireciona para `/admin`.

## Responsividade revisada (inspecao + build)
Breakpoints revisados por estrutura e classes:
- Desktop 1366px: sem anomalia estrutural identificada.
- Notebook 1024px: drawers com breakpoint 1024, acoes mantidas.
- Tablet 768px: headers/cards/filtros empilham sem overflow estrutural.
- Mobile 390px: ajustes aplicados em dialogs administrativos, painel de ações, headers e paginação para evitar quebra.

## Comandos executados
Backend:
- `dotnet build SGX.SistemaChamado.sln` -> **OK** (build concluido com sucesso).
- `dotnet test SGX.SistemaChamado.sln` -> **OK** (153 testes aprovados, 0 falhas).

Frontend:
- `npm install` -> **OK**.
- `npm run build` -> **OK**.
- `npm run lint` -> **não existe script `lint` no `package.json`**.
- `npm run dev` -> **executado com timeout técnico no ambiente automatizado** (processo interativo não mantido nesta execucao).

## Pendencias reais
- `Pendente real`: validação manual final de UX no navegador para confirmar aderencia visual em 1366/1024/768/390 com dados reais.
- `Pendente real`: adicionar script `lint` no frontend para governanca estatica continua.

## Status final da UX 5
- **Apto por inspecao + build** para homologacao visual.
- Persistem apenas pendencias de validação manual em ambiente interativo e adicao de `lint` ao `package.json`.








## Sprint Segurança 3 - Matriz de permissões (/admin/cadastros/perfis/:id)

### Rota
- `/admin/cadastros/perfis/:id`

### Endpoints consumidos
- `GET /api/admin/cadastros/permissoes`
- `GET /api/admin/cadastros/perfis/{id}/permissoes`
- `PUT /api/admin/cadastros/perfis/{id}/permissoes`

### Matriz de permissões no frontend
- Card: `Permissões do perfil`.
- Agrupamento visual por módulo com `QExpansionItem`.
- Checkbox por permissão com nome, código e descrição.
- Destaque de permissões críticas:
  - `Usuarios.Gerenciar`
  - `Perfis.Gerenciar`
  - `Perfis.AlterarPermissoes`

### Regras visuais aplicadas
- Administrador com `Perfis.AlterarPermissoes` pode editar e salvar permissões.
- Atendente visualiza em modo somente leitura (checkboxes desabilitados e banner explicativo).
- Solicitante segue sem acesso ao admin pelos guards existentes.
- Botão `Salvar permissões` aparece apenas quando há permissão de alteração.

### Status de validação
- Implementado no frontend com carregamento, erro, vazio e sucesso.
- Mantida compatibilidade com o contrato atual de `/api/me` e cadastros administrativos.
- Backend continua como fonte real de segurança (validação final de autorização no servidor).

## Sprint Seguranca 4 - Complemento de validacao de rotas

### Rota /admin/cadastros/perfis/:id
- Matriz de permissoes: implementada no card de permissoes do perfil.
- Endpoints de permissoes utilizados:
  - `GET /api/admin/cadastros/permissoes`
  - `GET /api/admin/cadastros/perfis/{id}/permissoes`
  - `PUT /api/admin/cadastros/perfis/{id}/permissoes`
- Controle visual:
  - Administrador com `Perfis.AlterarPermissoes` edita e salva.
  - Atendente visualiza em modo leitura.
  - Permissoes criticas com destaque visual.
- Status da validacao: `OK inspecao + build` (validacao manual com usuarios reais permanece em homologacao).

### Rotas protegidas - perfil, permissao ou ambos
- `/portal*`: depende de perfil (`Solicitante`, `Atendente`, `Administrador`), com controles visuais por permissao em acoes especificas.
- `/admin*`: depende de perfil (`Administrador` ou `Atendente`) e controles de permissao por tela/acao.
- `/admin/configuracoes/parametros*`: depende de perfil (`Administrador`) e permissao para acoes de gerenciamento.
- `/admin/cadastros/perfis/:id`: depende de perfil admin/atendente + permissao `Perfis.AlterarPermissoes` para mutacao.
- Rotas e botoes de administracao usam combinacao de guard por perfil (router) e validacao por permissao (store/layout/views).

## Sprint Roadmap ITSM - Status real e futuras implementacoes

### Rota `/admin/roadmap-itsm`
- Perfil: `Administrador` e `Atendente`.
- Permissao para leitura: `Roadmap.Visualizar`.
- Permissao para mutacao: `Roadmap.Gerenciar`.
- Endpoints principais:
  - `GET /api/admin/roadmap`
  - `GET /api/admin/roadmap/{id}`
  - `POST /api/admin/roadmap`
  - `PUT /api/admin/roadmap/{id}`
  - `POST /api/admin/roadmap/{id}/inativar`
  - `POST /api/admin/roadmap/{id}/reativar`

### Futuras implementacoes no detalhe do roadmap
- Permissao para leitura: `RoadmapImplementacoes.Visualizar`.
- Permissao para mutacao: `RoadmapImplementacoes.Gerenciar`.
- Endpoints principais:
  - `GET /api/admin/roadmap/implementacoes`
  - `GET /api/admin/roadmap/{roadmapItemId}/implementacoes`
  - `GET /api/admin/roadmap/implementacoes/{id}`
  - `POST /api/admin/roadmap/implementacoes`
  - `PUT /api/admin/roadmap/implementacoes/{id}`
  - `POST /api/admin/roadmap/implementacoes/{id}/concluir`
  - `POST /api/admin/roadmap/implementacoes/{id}/inativar`
  - `POST /api/admin/roadmap/implementacoes/{id}/reativar`

### Status da validacao
- Secao `Status real da implementacao`: implementada com campos novos, progress bar e banners de contexto.
- Secao `Futuras implementacoes`: implementada com `QTable`, `QDialog`, estados de loading/erro/vazio e acoes de ciclo de vida.
- Validacao funcional: `OK inspecao + build`; homologacao manual com usuarios reais permanece pendente.

## Sprint Roadmap ITSM - Categoria, checklist e labels amigaveis

### Rota `/admin/roadmap-itsm`
- Categoria passou a ser selecionada por `QSelect` (dropdown de categorias ativas via cadastro).
- Categoria legada permanece como fallback para itens antigos sem `RoadmapCategoriaId`.
- Checklist da implementacao disponivel no detalhe com acoes: criar, editar, concluir, reabrir, inativar e reativar.
- Percentual de implementacao exibido por calculo automatico do checklist ativo (com `QLinearProgress`).
- Labels amigaveis aplicados em `QSelect`, `QTable` e `QBadge` para status/enum do roadmap.

### Endpoints adicionais utilizados na rota
- `GET /api/admin/roadmap/categorias`
- `GET /api/admin/roadmap/categorias/{id}`
- `POST /api/admin/roadmap/categorias`
- `PUT /api/admin/roadmap/categorias/{id}`
- `POST /api/admin/roadmap/categorias/{id}/inativar`
- `POST /api/admin/roadmap/categorias/{id}/reativar`
- `GET /api/admin/roadmap/{roadmapItemId}/checklist`
- `POST /api/admin/roadmap/{roadmapItemId}/checklist`
- `PUT /api/admin/roadmap/checklist/{id}`
- `POST /api/admin/roadmap/checklist/{id}/concluir`
- `POST /api/admin/roadmap/checklist/{id}/reabrir`
- `POST /api/admin/roadmap/checklist/{id}/inativar`
- `POST /api/admin/roadmap/checklist/{id}/reativar`

### Dependencia de seguranca
- Consulta: perfil `Administrador` ou `Atendente` + permissao `Roadmap.Visualizar`.
- Mutacao: perfil `Administrador` + permissao `Roadmap.Gerenciar`.
- Observacao: permissao granular dedicada para categoria/checklist permanece como evolucao futura.

## Sprint Portal 3 - Validacao de fluxo portal/admin

### Rotas validadas
- `/portal/chamados`
  - Endpoints: `GET /api/portal/contexto`, `GET /api/portal/chamados`
  - Componentes: `QPage`, `PageHeader`, `AppSectionCard`, `QTable`, `QCard`, `QBtn`, `QChip/QBadge`
  - Status visual: OK inspecao + build
  - Status funcional: OK inspecao + build
  - Pendencias: validacao manual com usuario real

- `/portal/chamados/novo`
  - Endpoints: `GET /api/portal/contexto`, `POST /api/portal/chamados`, `POST /api/portal/chamados/{id}/anexos`
  - Componentes: `QPage`, `QForm`, `QInput`, `QSelect`, `UploadAnexo`, `QBanner`, `QBtn`, `LoadingState`, `ErrorState`
  - Status visual: OK inspecao + build
  - Status funcional: OK inspecao + build
  - Pendencias: validacao manual do fluxo de falha parcial de anexos

- `/portal/chamados/:id`
  - Endpoints: `GET /api/portal/chamados/{id}`, `POST /api/portal/chamados/{id}/comentarios`, `POST /api/portal/chamados/{id}/anexos`
  - Componentes: `QPage`, `PageHeader`, `AppSectionCard`, `QList`, `QItem`, `QTimeline`, `StatusBadge`, `PrioridadeBadge`, `SlaBadge`, `UploadAnexo`, `FormComentario`
  - Status visual: OK inspecao + build
  - Status funcional: OK inspecao + build
  - Pendencias: validacao manual de mensagens 403/404 em ambiente real

- `/admin/chamados`
  - Endpoint: `GET /api/admin/chamados`
  - Componentes: `QPage`, `PageHeader`, `AppSectionCard`, `QTable`, `QPagination`, `QBtn`, `QBanner`
  - Status visual: OK inspecao + build
  - Status funcional: OK inspecao + build
  - Pendencias: validacao manual de filtros apos abertura via portal

- `/admin/chamados/:id`
  - Endpoint: `GET /api/admin/chamados/{id}`
  - Componentes: `QPage`, `PageHeader`, `AppSectionCard`, `QList`, `QItem`, `QTimeline`, `QChip/QBadge`, `QBtn`
  - Status visual: OK inspecao + build
  - Status funcional: OK inspecao + build
  - Pendencias: homologacao manual com usuario administrativo real

## Sprint Portal 4 - Matriz de validacao das rotas portal/admin

| Rota | Layout | Endpoints consumidos | Componentes Quasar principais | Status visual | Status funcional | Validacao | Pendencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `/portal/chamados/novo` | `PortalLayout` | `GET /api/portal/contexto`, `POST /api/portal/chamados`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `PageHeader`, `AppSectionCard`, `QForm`, `QInput`, `QSelect`, `UploadAnexo`, `QBanner`, `QBtn`, `LoadingState`, `ErrorState` | OK inspecao + build | OK inspecao + build | Pendente manual | homologacao real e teste de anexo invalido em ambiente real |
| `/portal/chamados` | `PortalLayout` | `GET /api/portal/contexto`, `GET /api/portal/chamados` | `QPage`, `PageHeader`, `AppSectionCard`, `QTable`, `QCard`, `QBtn`, `QChip`, `QBadge`, `LoadingState`, `ErrorState`, `EmptyState` | OK inspecao + build | OK inspecao + build | Pendente manual | validar dados reais de producao/homologacao |
| `/portal/chamados/:id` | `PortalLayout` | `GET /api/portal/chamados/{id}`, `POST /api/portal/chamados/{id}/comentarios`, `POST /api/portal/chamados/{id}/anexos` | `QPage`, `PageHeader`, `AppSectionCard`, `QList`, `QItem`, `QTimeline`, `QCard`, `QBtn`, `QChip`, `StatusBadge`, `PrioridadeBadge`, `SlaBadge`, `UploadAnexo`, `FormComentario`, `LoadingState`, `ErrorState` | OK inspecao + build | OK inspecao + build | Pendente manual | validar 403/404 com usuarios reais |
| `/admin/chamados` | `AdminLayout` | `GET /api/admin/chamados` | `QPage`, `PageHeader`, `AppSectionCard`, `QTable`, `QPagination`, `QBtn`, `QBanner`, `LoadingState`, `ErrorState`, `EmptyState` | OK inspecao + build | OK inspecao + build | Pendente manual | validar filtros com dados reais apos abertura no portal |
| `/admin/chamados/:id` | `AdminLayout` | `GET /api/admin/chamados/{id}` | `QPage`, `PageHeader`, `AppSectionCard`, `QList`, `QItem`, `QTimeline`, `QChip`, `QBadge`, `QBtn`, `LoadingState`, `ErrorState` | OK inspecao + build | OK inspecao + build | Pendente manual | validar historico/anexo com usuario real |

## Sprint Integracoes E-mail 4 - Validacao da rota administrativa

### Rota `/admin/integracoes/email`
- Layout: `AdminLayout`
- Endpoints consumidos:
  - `GET /api/admin/integracoes/email/logs`
  - `GET /api/admin/integracoes/email/logs/{id}`
- Componentes Quasar usados:
  - `QPage`, `PageHeader`, `AppSectionCard`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QDialog`, `QExpansionItem`, `QBtn`
- Status visual: OK inspecao + build
- Status funcional: OK inspecao + build
- Pendencias reais:
  - validacao manual com base de logs reais em homologacao
  - confirmacao de acesso com perfis reais (Administrador/Atendente/Solicitante)

## Sprint Integracoes E-mail 5 - Confirmacao da rota administrativa

- Rota: `/admin/integracoes/email`
- Layout: `AdminLayout`
- Endpoints consumidos:
  - `GET /api/admin/integracoes/email/logs`
  - `GET /api/admin/integracoes/email/logs/{id}`
- Componentes Quasar:
  - `QPage`, `PageHeader`, `AppSectionCard`, `QForm`, `QInput`, `QSelect`, `QTable`, `QBadge`, `QDialog`, `QExpansionItem`, `QBtn`
- Status visual: OK inspecao + build
- Status funcional: OK inspecao + build
- Pendencias reais:
  - validacao manual com massa de logs reais
  - validacao com caixa IMAP real em homologacao

## Sprint Autenticação 3 - Login Microsoft e restauração de sessão

### Fluxo consolidado
- Login Microsoft via `LoginView` + `authService` (MSAL popup).
- Após autenticação, frontend obtém Bearer token e chama `GET /api/me`.
- Perfis e permissões retornados pelo SGX determinam acesso e redirecionamento.

### Sessão e refresh
- `router.beforeEach` aguarda `authStore.inicializarSessao()`.
- Single-flight ativo para evitar chamadas concorrentes de inicialização.
- F5/Ctrl+F5 não deve causar falso logoff quando sessão é restaurável.

### Segurança de ambiente
- `Authorization: Bearer` no fluxo Microsoft.
- `X-Dev-*` apenas em Development com modo local.
- Login local e emulação não aparecem em Production.
