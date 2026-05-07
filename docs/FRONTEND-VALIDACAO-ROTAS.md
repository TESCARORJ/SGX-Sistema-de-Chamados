# Frontend - Validacao de Rotas

## Escopo
Checklist manual de validacao visual e funcional das rotas principais do frontend Quasar do `SGX.SistemaChamado.Web`.

Legenda de status:
- `OK (inspecao)`: rota validada por inspeção de codigo/fluxo.
- `Pendente manual`: requer navegacao manual no browser para confirmar ambiente/local.

| Rota | Perfil necessario | Endpoint consumido (principal) | Status esperado | Status | Observacoes |
|---|---|---|---|---|---|
| `/login` | Publico | `POST/Azure Popup` + `GET /api/me` | Card Quasar, botoes Microsoft/local dev, loading e erro | OK (inspecao) | Login local visivel somente fora de `PROD`. |
| `/acesso-negado` | Autenticado sem permissao | nenhum | Mensagem de bloqueio com CTA para login | OK (inspecao) | Renderizado em `AuthLayout`. |
| `/portal` | `Solicitante`, `Atendente`, `Administrador` | `GET /api/portal/chamados` | Dashboard em cards Quasar | OK (inspecao) | Usa `PortalLayout` com header/drawer. |
| `/portal/chamados` | `Solicitante`, `Atendente`, `Administrador` | `GET /api/portal/contexto`, `GET /api/portal/chamados` | Filtros + lista em cards Quasar | OK (inspecao) | Estado vazio e loading incluidos. |
| `/portal/chamados/novo` | `Solicitante`, `Atendente`, `Administrador` | `GET /api/portal/contexto`, `POST /api/portal/chamados` | Formulario `QForm` com `QInput/QSelect` | OK (inspecao) | Redireciona para detalhe apos criar. |
| `/portal/chamados/:id` | `Solicitante`, `Atendente`, `Administrador` | `GET /api/portal/chamados/:id` | Detalhe com cards, badges SLA, anexos e historico | OK (inspecao) | Comentario e upload mantidos. |
| `/admin` | `Administrador`, `Atendente` | `GET /api/admin/contexto`, `GET /api/admin/dashboard` | Dashboard com cards/listas/tabela | OK (inspecao) | Layout admin com drawer responsivo. |
| `/admin/chamados` | `Administrador`, `Atendente` | `GET /api/admin/chamados` | Filtros + tabela + paginacao visual | OK (inspecao) | Acoes de assumir/detalhar mantidas. |
| `/admin/chamados/:id` | `Administrador`, `Atendente` | `GET /api/admin/chamados/:id` | Detalhe com painel de acoes, comentarios e historico | OK (inspecao) | Dialogs e modais administrativos mantidos. |
| `/admin/cadastros/usuarios` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/usuarios` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Botao novo so para admin. |
| `/admin/cadastros/usuarios/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/usuarios/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Salvar permitido apenas admin. |
| `/admin/cadastros/perfis` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/perfis` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/perfis/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/perfis/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/departamentos` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/departamentos` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/departamentos/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/departamentos/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/categorias` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/categorias` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/categorias/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/categorias/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/prioridades` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/prioridades` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/prioridades/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/prioridades/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/status` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/status` | Tabela Quasar com filtros/paginacao | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/cadastros/status/:id` | `Administrador`, `Atendente` | `GET /api/admin/cadastros/status/:id` | Formulario Quasar, inativar/reativar | OK (inspecao) | Herdado da base de cadastros. |
| `/admin/configuracoes/parametros` | `Administrador` | `GET /api/admin/configuracoes/parametros` | Tabela com badge para sensivel/ativo | OK (inspecao) | Controle de perfil no router e backend. |
| `/admin/configuracoes/parametros/:id` | `Administrador` | `GET /api/admin/configuracoes/parametros/:id` | Form com mascara para valor sensivel | OK (inspecao) | Valor mascarado quando `sensivel=true`. |
| `/admin/integracoes/email` | `Administrador`, `Atendente` | `GET /api/admin/integracoes/email/logs` | Filtros + tabela + dialog de detalhe | OK (inspecao) | Estado vazio e erro tecnico tratados. |

## Validacao manual sugerida
1. Subir API e frontend local.
2. Abrir navegador Chrome normal (nao perfil temporario do VS Code).
3. Validar o fluxo `/login -> /admin` com modo local Development.
4. Navegar por cada rota da tabela acima e confirmar responsividade desktop/mobile.
