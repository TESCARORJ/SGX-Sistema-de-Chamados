# Sprint 3 - Consultas de chamados com grupo tecnico e fila

## Objetivo

Preparar as consultas administrativas de chamados para carregar, filtrar e retornar informacoes de grupo tecnico e fila de atendimento, mantendo compatibilidade com chamados legados sem esses vinculos.

## Consultas analisadas

- `ListarChamadosAdminUseCase`
- `DetalharChamadoAdminUseCase`
- `AdminChamadoLoader`
- `AdminUseCaseHelpers`
- DTOs administrativos de filtro, resumo e detalhe de chamado
- Types e service administrativo do frontend para os contratos existentes

## Consultas ajustadas

`ListarChamadosAdminUseCase` passou a carregar `GrupoTecnico` e `FilaAtendimento` junto aos demais dados de listagem administrativa.

`AdminChamadoLoader`, usado pelo detalhe administrativo e por retornos de use cases administrativos, passou a carregar `GrupoTecnico` e `FilaAtendimento`.

## Campos adicionados

Foram adicionados aos responses administrativos de resumo e detalhe:

- `GrupoTecnicoId`
- `GrupoTecnicoNome`
- `FilaAtendimentoId`
- `FilaAtendimentoNome`

Os campos sao nullable para preservar chamados legados sem grupo/fila.

## Filtros adicionados

`FiltroChamadosAdminRequest` recebeu filtros opcionais:

- `GrupoTecnicoId`
- `FilaAtendimentoId`

Quando informados, retornam apenas chamados vinculados ao grupo ou fila correspondente. Quando ausentes, o comportamento da listagem permanece o mesmo.

O type `FiltroChamadosAdmin` e o service administrativo existente no frontend tambem foram preparados para serializar esses filtros, sem criar tela ou service novo.

## Null safety

Chamados sem `GrupoTecnicoId` ou `FilaAtendimentoId` continuam sendo carregados e retornados. Os nomes sao resolvidos por navegacao opcional (`GrupoTecnico?.Nome` e `FilaAtendimento?.Nome`) e retornam `null` quando o vinculo nao existe.

## Testes

Foram ajustados testes para cobrir:

- listagem administrativa com chamados sem grupo/fila;
- retorno de `GrupoTecnicoId/Nome` e `FilaAtendimentoId/Nome`;
- filtro por grupo tecnico;
- filtro por fila de atendimento;
- preservacao do filtro por responsavel;
- detalhe administrativo com grupo/fila preenchidos;
- detalhe administrativo com grupo/fila nulos;
- serializacao frontend dos filtros novos.

## Fora do escopo

Nao foram criados controller, endpoint publico, tela Vue, service frontend novo, dashboard, relatorio, regra de SLA, roteamento automatico, nova regra de transferencia ou migration estrutural.

## Roadmap

O checklist da Sprint 3 foi atualizado marcando somente o item "Ajustar consultas de chamados para considerar grupo tecnico e fila" como concluido.

Com 24 itens concluidos de 54 ativos, o percentual esperado da Sprint 3 passa para aproximadamente 44%.

## Proxima etapa recomendada

Validar permissoes de acesso as operacoes de grupo e fila.
