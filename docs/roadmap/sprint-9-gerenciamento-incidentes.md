# Sprint 9 - Gerenciamento de Incidentes

## Objetivo

Consolidar o backlog tecnico da Sprint 9 como um checklist rastreavel de governanca, preparacao funcional e compatibilidade ITSM, sem implementar o fluxo de incidente nesta entrega.

## Estado atual

- Area: `Sprint 9 - Gerenciamento de Incidentes`
- Categoria: `ITIL/ITSM`
- Ordem: `109`
- Status da implementacao: `Em desenvolvimento`
- Status tecnico: `Parcial`
- Percentual recalculado: `36%`
- Checklist ativo: `50`
- Checklist concluido: `18`
- Checklist pendente: `32`

## Escopo desta entrega

Esta tarefa nao implementa endpoints, telas, DTOs, validators, use cases, tabelas, colunas, enums ou regras operacionais de incidente.

A entrega se limita a:
- substituir o checklist generico por um checklist tecnico e rastreavel;
- registrar o que ja existe em termos de natureza, status, SLA, prioridade e compatibilidade com o legado;
- explicitar o que fica pendente para a implementacao futura;
- sincronizar seed, teste, migration e roadmap.

## O que foi consolidado

### Planejamento e governanca

- O escopo real da Sprint 9 foi confirmado como Gerenciamento de Incidentes.
- Os criterios de aceite do futuro fluxo foram registrados.
- As limitacoes foram documentadas sem assumir CMDB funcional.
- A Sprint 9 foi mantida fora da abertura legada de chamados e fora da Sprint 8.

### Compatibilidade tecnica

- A natureza `Incidente` ja existe no modelo ITSM.
- O fluxo de incidente continua separado de `Requisicao` e do chamado legado.
- O status de incidente continua compatibilizado com o fluxo atual de chamados.
- O SLA de incidente continua dependente da base atual ate existir regra dedicada.
- A prioridade por impacto e urgencia permanece como referencia de evolucao futura sem quebrar a prioridade atual.
- A classificacao por e-mail, os filtros administrativos, os relatorios e as acoes disponiveis ja reconhecem Incidente no codigo existente.

### Governanca de entrega

- `SeedData.cs` foi atualizado.
- O teste de checklist da Sprint 9 foi criado/atualizado.
- A migration de dados da Sprint 9 foi preparada.
- O percentual da Sprint 9 foi recalculado com base no novo checklist e na evidencia real de 18 itens concluidos.
- O status real permaneceu como `Em desenvolvimento`.

## O que nao foi implementado

- abertura funcional de incidente;
- triagem funcional de incidente;
- atendimento funcional de incidente;
- diagnostico funcional de incidente;
- solucao de contorno funcional;
- resolucao funcional;
- reabertura funcional de incidente;
- fechamento funcional de incidente;
- endpoints especificos;
- telas especificas;
- validadores ou DTOs novos;
- modelagem de CI afetado, caso CMDB continue ausente;
- fluxo de Problema;
- fluxo de Mudanca;
- mudanca estrutural de banco de dados.

## Checklist por grupos

### Planejamento e governanca

Concluidos: diagnostico do contexto, confirmacao de escopo, criterios de aceite, diferenca entre incidente e chamado legado, limitacoes atuais e dependencias/riscos da sprint.

Pendentes: aprofundamento funcional do fluxo de incidente e definicoes operacionais ainda nao implementadas.

### Modelagem e compatibilidade

Concluidos: existencia de `Incidente`, compatibilidade de status, impacto e urgencia obrigatorios, classificacao por e-mail, filtros, relatorios, acoes disponiveis, abertura legada e sincronizacao documental.

Pendentes: campos especificos, servico afetado, CI afetado, causa provavel, diagnostico, workaround, resolucao, reabertura e fechamento.

### Backend e API

Concluidos: nenhuma funcionalidade nova foi entregue, apenas rastreabilidade do que ja existe.

Pendentes: DTOs, validators, use cases, contratos, endpoints e regras operacionais especificas.

### Frontend

Concluidos: reconhecimento visual de Incidente ja existe em filtros, dashboard e relatorios.

Pendentes: telas especificas de abertura, atendimento, diagnostico, resolucao, reabertura e fechamento.

### Testes

Concluidos: teste de checklist, recalculo de percentual e rastreabilidade do novo conjunto de itens.

Pendentes: suites funcionais do futuro fluxo de incidente.

### Seguranca

Concluidos: a classificacao e a visualizacao atuais nao expuseram novo fluxo sensivel nesta entrega.

Pendentes: autorizacao por acao operacional, protecao de payload e validacao de metadados do incidente.

### Documentacao e homologacao

Concluidos: roadmap, seed, migration e documento dedicado foram sincronizados.

Pendentes: roteiro de homologacao funcional, visual, permissao e aceite formal.

## Riscos e decisoes adiadas

- `CI afetado` continua dependente de uma CMDB que ainda nao existe nesta entrega.
- `SLA de incidente` pode reutilizar a base atual de SLA ate existir um item especifico.
- O fluxo de incidente nao deve ser confundido com problema, mudanca ou requisicao.
- Nao houve alteracao do fluxo de abertura legada nem da Sprint 8.

## Proxima etapa recomendada

Implementar incrementalmente o fluxo de incidente a partir dos contratos e campos especificos, mantendo o checklist como rastreio de progresso ate a homologacao funcional.
