# Sprint 3 - Filas por Grupo Tecnico

## Tela criada

- Tela administrativa `GrupoTecnicoFilasAdminView.vue`.

## Rota criada

- `/admin/cadastros/grupos-tecnicos/:id/filas`.

## Entrada visual

- A tela `GruposTecnicosAdminView.vue` recebeu a acao visual `Filas do grupo tecnico` na coluna de acoes.
- A navegacao segue o mesmo padrao da subtela de membros por grupo tecnico.

## Services e types

- `adminService.listarFilasAtendimentoGrupoTecnico` consome `GET /api/admin/grupos-tecnicos/{grupoTecnicoId}/filas`.
- `ListarFilasAtendimentoGrupoTecnicoFiltro` modela os filtros `ativo` e `busca`.
- `FilaAtendimentoGrupoTecnicoResponse` modela os campos retornados para listagem.

## Filtros disponiveis

- Busca por nome ou descricao, usando o parametro `busca` suportado pelo backend.
- Status: todas, ativas e inativas, usando o parametro `ativo` suportado pelo backend.

## Permissoes visuais

- Administrador visualiza.
- Atendente visualiza.
- A tela nao exibe acoes administrativas de cadastro, edicao, ativacao ou inativacao de fila.

## Limitacoes

- A tela e somente de listagem.
- Nao ha cadastro, edicao, inativacao ou manutencao de filas nesta etapa.
- Nao ha selecao de fila para movimentar chamado.
- Nao ha vinculo direto com chamados nesta tela.

## Testes

- `GrupoTecnicoFilasAdminView.spec.ts` valida cabecalho, filtros, tabela, estado vazio e ausencia de acoes administrativas de fila.
- `GruposTecnicosAdminView.spec.ts` valida a entrada visual para filas.
- `adminService.spec.ts` valida a chamada do endpoint de filas com filtros.

## O que nao foi implementado

- Nao foi criada tela de cadastro de fila.
- Nao foi criada acao de criar, editar, ativar ou inativar fila.
- Nao foi criado endpoint novo.
- Nao houve alteracao de regra backend, Chamado, SLA, dashboard ou relatorios.
- Nao houve migration estrutural.

## Proxima etapa recomendada

Permitir assumir chamado pela fila.
