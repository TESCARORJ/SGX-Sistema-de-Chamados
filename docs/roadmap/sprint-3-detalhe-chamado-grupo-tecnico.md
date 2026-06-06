# Sprint 3 - Detalhe do Chamado com Grupo Tecnico

## Tela alterada

- Tela administrativa `AdminDetalheChamadoView.vue`.

## Campos exibidos

- `grupoTecnicoNome` como `Grupo tecnico`.
- `filaAtendimentoNome` como `Fila` no resumo superior e `Fila de atendimento` no resumo do chamado.
- `responsavel` mantido proximo aos dados de grupo e fila.

## Comportamento para nulos

- Chamados sem grupo exibem `Sem grupo tecnico`.
- Chamados sem fila exibem `Sem fila`.
- Chamados sem responsavel continuam exibindo o fallback visual existente para responsavel nao atribuido.

## Posicao visual

- Cards de resumo operacional no topo do detalhe.
- Linha de atendimento dentro de `Resumo do chamado`, antes dos dados de solicitante e datas.

## Testes

- `AdminDetalheChamadoView.itsm.spec.ts` atualizado para validar exibicao de grupo/fila e fallback de chamados legados.

## O que nao foi implementado

- Nao foi criada acao de direcionar grupo.
- Nao foi criada acao de transferir grupo.
- Nao foi criada acao de assumir fila.
- Nao foi criada acao de atribuir tecnico.
- Nao foi criada tela de fila.
- Nao foi criado endpoint novo.
- Nao houve alteracao de regra backend, contrato backend, Chamado, SLA, dashboard ou relatorios.
- Nao houve migration estrutural.

## Proxima etapa recomendada

Exibir fila de atendimento por grupo tecnico.
