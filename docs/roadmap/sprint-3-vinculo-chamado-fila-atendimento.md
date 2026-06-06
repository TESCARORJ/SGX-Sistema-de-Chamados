# Vinculo entre chamado e fila de atendimento

## Contexto

Esta etapa da Sprint 3 adiciona ao chamado a referencia opcional para a fila operacional de atendimento. A mudanca complementa o vinculo ja existente com `GrupoTecnico`, sem alterar o responsavel individual nem implementar regras funcionais de roteamento, transferencia, aceite ou SLA.

## Decisao de modelagem

Foi adicionado `FilaAtendimentoId` opcional em `Chamado`.

Motivos:

- registrar em qual fila operacional o chamado esta posicionado;
- permitir que um chamado esteja em fila antes de ser assumido por um tecnico individual;
- manter `GrupoTecnicoId` como contexto corporativo do atendimento;
- preservar `ResponsavelId` como tecnico individual responsavel;
- garantir compatibilidade com chamados existentes.

## Vinculo opcional

`FilaAtendimentoId` e nullable para nao invalidar chamados existentes e para permitir fluxos legados ou intermediarios onde o chamado tem grupo tecnico, mas ainda nao possui fila especifica.

Nao houve backfill automatico. O preenchimento retroativo exige regra operacional posterior, pois o sistema ainda nao possui roteamento formal por fila.

## Diferenca entre fila, grupo tecnico e responsavel

- `GrupoTecnicoId`: grupo corporativo responsavel pelo atendimento, como Service Desk, Infraestrutura ou Sistemas.
- `FilaAtendimentoId`: fila operacional onde o chamado aguarda tratamento ou acompanhamento.
- `ResponsavelId`: usuario tecnico individual que assumiu ou recebeu o chamado.

## Combinacoes permitidas nesta etapa

- `GrupoTecnicoId` preenchido, `FilaAtendimentoId` preenchido e `ResponsavelId` nulo: chamado em fila de grupo, ainda sem tecnico individual.
- `GrupoTecnicoId` preenchido, `FilaAtendimentoId` preenchido e `ResponsavelId` preenchido: chamado em fila/grupo e sob responsabilidade individual.
- `GrupoTecnicoId` preenchido e `FilaAtendimentoId` nulo: chamado associado ao grupo, ainda sem fila especifica.
- `GrupoTecnicoId` nulo e `FilaAtendimentoId` nulo: chamado legado ou fluxo atual sem grupo/fila.

## Risco de inconsistencia grupo/fila

`FilaAtendimento` possui `GrupoTecnicoId`. Como `Chamado` agora possui `GrupoTecnicoId` e `FilaAtendimentoId`, existe risco futuro de um chamado apontar para um grupo e para uma fila de outro grupo.

Nesta etapa nao foi criada regra funcional complexa nem constraint com validacao cruzada. A regra futura da aplicacao deve garantir:

`Chamado.GrupoTecnicoId == FilaAtendimento.GrupoTecnicoId`.

Tambem deve evitar `FilaAtendimentoId` preenchido com `GrupoTecnicoId` nulo em fluxos novos, salvo decisao explicita e documentada.

## Estrutura tecnica

Dominio:

- `Chamado.FilaAtendimentoId`
- `Chamado.FilaAtendimento`
- `Chamado.DefinirFilaAtendimento`
- `FilaAtendimento.Chamados`

Banco:

- coluna `chamados.fila_atendimento_id`
- FK opcional para `filas_atendimento.id`
- indice `ix_chamados_fila_atendimento_id`
- delete restrito para preservar integridade.

Nao foi criado indice composto com `grupo_tecnico_id` nesta etapa. O indice simples por fila atende a modelagem estrutural atual e evita antecipar filtros ainda nao implementados.

## O que nao foi implementado nesta etapa

- Roteamento automatico.
- Transferencia entre filas.
- Transferencia entre grupos.
- Regra de assumir chamado por fila.
- Alteracao em `ResponsavelId`.
- Alteracao funcional em `GrupoTecnicoId`.
- Auditoria estruturada de movimentacoes.
- Endpoints, controllers, services de aplicacao ou telas Vue.
- Alteracoes em SLA, dashboards, relatorios ou filtros.

## Impacto em chamados existentes

Chamados existentes continuam validos porque `fila_atendimento_id` e nullable. Nenhum registro existente precisa receber valor para que a migration seja aplicada.

## Roadmap

Com o vinculo opcional entre `Chamado` e `FilaAtendimento`, configuracao EF, migration, banco atualizado e build validados, o item `Definir vinculo entre chamado e fila de atendimento` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 7 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 13%.

## Proxima etapa recomendada

Definir as regras de atribuicao individual sem quebrar o responsavel atual, incluindo validacoes para manter `GrupoTecnicoId`, `FilaAtendimentoId` e `ResponsavelId` consistentes nos fluxos futuros.
