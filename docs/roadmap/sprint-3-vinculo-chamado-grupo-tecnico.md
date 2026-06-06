# Vinculo entre chamado e grupo tecnico

## Contexto

Esta etapa da Sprint 3 adiciona ao chamado a referencia ao grupo tecnico responsavel pelo atendimento corporativo. A mudanca prepara a substituicao gradual da fila implicita baseada em `ResponsavelId == null`, sem alterar o fluxo atual de atribuicao individual.

## Decisao de modelagem

Foi adicionado `GrupoTecnicoId` opcional em `Chamado`.

Motivos:

- registrar o grupo tecnico responsavel pelo atendimento corporativo;
- permitir que um chamado esteja direcionado a um grupo antes de ser assumido por um tecnico;
- preservar `ResponsavelId` como responsavel individual;
- manter compatibilidade com chamados existentes e com atribuicoes diretas legadas.

## Preservacao de ResponsavelId

`ResponsavelId` nao foi removido, renomeado ou tornado obrigatorio. Ele continua representando o usuario tecnico individual responsavel pelo chamado.

A nova propriedade `GrupoTecnicoId` representa outro conceito: o grupo corporativo responsavel pela fila ou pelo contexto operacional do atendimento.

## Combinacoes permitidas

- `GrupoTecnicoId` preenchido e `ResponsavelId` nulo: chamado direcionado a um grupo, ainda sem tecnico individual.
- `GrupoTecnicoId` preenchido e `ResponsavelId` preenchido: chamado associado a um grupo e assumido ou atribuido a um tecnico.
- `GrupoTecnicoId` nulo e `ResponsavelId` preenchido: fluxo legado ou atribuicao direta continuam validos.
- `GrupoTecnicoId` nulo e `ResponsavelId` nulo: chamado legado sem responsavel ou ainda aguardando tratamento pelo fluxo atual.

## Vinculo com fila

`FilaAtendimentoId` nao foi criado nesta etapa.

O checklist da Sprint 3 possui o item separado `Definir vinculo entre chamado e fila de atendimento`. Por isso, esta etapa ficou limitada ao vinculo entre chamado e grupo tecnico. A fila sera vinculada posteriormente, evitando misturar duas decisoes estruturais em uma unica migration.

## Impacto em chamados existentes

A coluna `grupo_tecnico_id` e nullable. Nenhum chamado existente precisa ser atualizado para que a migration seja aplicada.

Nao houve backfill automatico, pois o sistema ainda nao possui regra formal de roteamento por grupo. Qualquer preenchimento retroativo deve ser tratado em etapa propria, com criterio operacional claro.

## Estrutura tecnica

Dominio:

- `Chamado.GrupoTecnicoId`
- `Chamado.GrupoTecnico`
- `Chamado.DefinirGrupoTecnico`
- `GrupoTecnico.Chamados`

Banco:

- coluna `chamados.grupo_tecnico_id`
- FK opcional para `grupos_tecnicos.id`
- indice `ix_chamados_grupo_tecnico_id`
- delete restrito para preservar integridade e evitar exclusao acidental de grupos vinculados a chamados.

## O que nao foi implementado nesta etapa

- `FilaAtendimentoId` em `Chamado`.
- Roteamento automatico.
- Transferencia entre grupos.
- Regra de assumir chamado.
- Alteracao de SLA.
- Auditoria estruturada de movimentacoes.
- Endpoints, controllers, services de aplicacao ou telas Vue.
- Alteracoes em dashboards, relatorios ou filtros.

## Riscos tecnicos

- Enquanto `FilaAtendimentoId` nao existir, o grupo tecnico registra o contexto corporativo, mas ainda nao identifica a fila operacional especifica.
- Consultas futuras devem diferenciar grupo tecnico, fila e responsavel individual para evitar ambiguidade em relatorios.
- A mudanca nao cria historico de alteracao de grupo; auditoria estruturada precisa ser modelada em etapa posterior.
- Regras futuras devem evitar considerar direcionamento para grupo como aceite individual do chamado.

## Roadmap

Com a entidade `Chamado` vinculada opcionalmente a `GrupoTecnico`, configuracao EF, migration, banco atualizado e build validados, o item `Definir vinculo entre chamado e grupo tecnico` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 6 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 11%.

## Proxima etapa recomendada

Definir o vinculo entre chamado e fila de atendimento, mantendo `GrupoTecnicoId` como contexto corporativo e `ResponsavelId` como responsavel individual opcional.
