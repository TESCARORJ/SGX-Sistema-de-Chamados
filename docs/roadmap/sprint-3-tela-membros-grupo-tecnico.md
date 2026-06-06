# Sprint 3 - Tela de Membros de Grupo Tecnico

## Tela/secao criada

- Tela administrativa `GrupoTecnicoMembrosAdminView.vue`.
- Rota frontend: `/admin/cadastros/grupos-tecnicos/:id/membros`.
- Entrada visual adicionada na tabela de `GruposTecnicosAdminView.vue` pela acao de membros em cada grupo tecnico.

## Services e types

- `adminService.listarMembrosGrupoTecnico`
- `adminService.adicionarMembroGrupoTecnico`
- `adminService.alterarStatusMembroGrupoTecnico`
- `adminService.listarGruposTecnicosDoUsuario`
- Types adicionados em `types/admin.ts` para filtros, payloads, membros e grupos tecnicos do usuario.

## Funcionalidades

- Cabecalho com identificacao do grupo tecnico.
- Exibicao de nome, descricao e status do grupo.
- Listagem de membros com nome do usuario, e-mail, status, data de criacao e atualizacao.
- Filtro por status: ativos, inativos e todos.
- Estado vazio para ausencia de membros.
- Modal para adicionar membro.
- Ativacao e inativacao de membro com confirmacao.
- Mensagens visiveis de erro e sucesso.

## Permissoes visuais

- Administrador visualiza e gerencia membros.
- Atendente visualiza em modo somente leitura.
- A rota frontend fica limitada aos perfis Administrador e Atendente, alinhada aos endpoints administrativos ja existentes.

## Limitacoes

- A adicao de membro usa a lista `atendentes` retornada por `obterAdminContexto`, evitando criar endpoint novo de usuarios nesta etapa.
- O backend permanece fonte da verdade para duplicidade, usuario inexistente e demais regras de consistencia.

## Testes

- `GrupoTecnicoMembrosAdminView.spec.ts`
- `GruposTecnicosAdminView.spec.ts` atualizado para verificar o acesso visual a membros.
- `adminService.spec.ts` atualizado para cobrir os endpoints frontend de membros.

## O que nao foi implementado

- Nao foi criada tela de filas.
- Nao foi criada tela de direcionamento de chamado.
- Nao foi criada tela de assumir fila.
- Nao foi criada tela de transferencia entre grupos tecnicos.
- Nao foi criado endpoint novo.
- Nao houve alteracao de regra de backend, chamados, SLA, dashboard ou relatorios.
- Nao houve migration estrutural.

## Roadmap

- Item concluido: `Criar tela ou secao de membros por grupo tecnico`.
- Percentual esperado da Sprint 3: 33/54, aproximadamente 61%.
- Proxima etapa recomendada: exibir grupo tecnico no detalhe do chamado.
