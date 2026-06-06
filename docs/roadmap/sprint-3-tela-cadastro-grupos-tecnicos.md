# Sprint 3 - Tela de Cadastro de Grupos Tecnicos

## Tela/secao criada

- Tela administrativa `GruposTecnicosAdminView.vue`.
- Rota: `GET /admin/cadastros/grupos-tecnicos` no frontend.
- Menu: item `Grupos Tecnicos` em cadastros administrativos para Administrador e item de leitura no menu administrativo legado de Atendente.

## Services e types

- `adminService.listarGruposTecnicos`
- `adminService.obterGrupoTecnico`
- `adminService.criarGrupoTecnico`
- `adminService.atualizarGrupoTecnico`
- `adminService.atualizarStatusGrupoTecnico`
- Types adicionados em `types/admin.ts` para filtros, lista, detalhe e payloads de grupos tecnicos.

## Funcionalidades

- Listagem paginada de grupos tecnicos.
- Busca por nome via `texto`.
- Filtro de status: ativos, inativos e todos.
- Criacao de grupo tecnico.
- Edicao de nome e descricao.
- Ativacao e inativacao por confirmacao.
- Validacao visual de nome obrigatorio.
- Mensagens visiveis de erro e sucesso.

## Permissoes visuais

- Administrador visualiza e gerencia.
- Atendente visualiza em modo somente leitura.
- A rota frontend fica limitada aos perfis Administrador e Atendente, alinhada aos endpoints administrativos ja existentes.

## O que nao foi implementado

- Nao foi criada tela de membros de grupos tecnicos.
- Nao foi criada tela de filas.
- Nao foi criada tela de direcionamento, assumir fila ou transferencia.
- Nao foi criado endpoint novo.
- Nao houve alteracao de regra de backend, chamados, SLA, dashboard ou relatorios.
- Nao houve migration estrutural.

## Testes

- `GruposTecnicosAdminView.spec.ts`
- `adminService.spec.ts` atualizado para cobrir os metodos de grupos tecnicos.

## Proxima etapa recomendada

Criar tela ou secao de membros por grupo tecnico, usando os endpoints administrativos de membros ja expostos.
