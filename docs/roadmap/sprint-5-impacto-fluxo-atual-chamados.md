# Sprint 5 - Impacto no fluxo atual de chamados

## Objetivo
Registrar o impacto tecnico e operacional das regras de fechamento, aceite e reabertura sem alterar SLA nem o Motor de Aprovacoes ITSM da Sprint 4.

## Fluxo anterior
- O encerramento ocorria de forma mais direta, sem etapa formal de aceite do solicitante.
- Reaberturas tinham menos governanca.
- Cancelamentos e resolucoes podiam ficar com rastreabilidade inconsistente.

## Fluxo consolidado na Sprint 5
- `Resolvido` passa a representar entrega tecnica aguardando avaliacao do solicitante.
- `Encerrado` representa fechamento definitivo por aceite explicito ou por prazo de aceite expirado.
- Rejeicao da solucao devolve o chamado para `EmAtendimento`.
- Reabertura controlada exige motivo e respeita politica de prazo.
- Fechamento definitivo continua bloqueado quando existir aprovacao pendente bloqueante.

## Impactos preservados
- Nao houve alteracao estrutural de SLA nesta consolidacao.
- Nao houve alteracao estrutural do Motor de Aprovacoes ITSM da Sprint 4.
- Nao houve mistura de implementacao funcional da Sprint 6.

## Impactos por area
- Atendimento: passa a resolver antes de fechar definitivamente.
- Portal do solicitante: ganha aceite e rejeicao da solucao no momento correto do ciclo.
- Auditoria/historico: passa a registrar com mais clareza resolucao, aceite, rejeicao, fechamento automatico e reabertura.
- Administracao: pode governar o prazo de auto-fechamento e manter reabertura sob politica.

## Limitacoes mantidas
- A homologacao formal desta sprint ainda nao foi executada.
- O roteiro de homologacao foi preparado, mas o aceite institucional permanece posterior.

## Conclusao
O fluxo atual de chamados foi consolidado tecnicamente para separar resolucao de fechamento definitivo, aumentar rastreabilidade e preparar a transicao segura para a Sprint 6 - Notificacoes ITSM.
