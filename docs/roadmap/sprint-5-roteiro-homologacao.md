# Roteiro de homologacao da Sprint 5

## Objetivo
Orientar a homologacao formal posterior da Sprint 5 - Regras de fechamento, aceite e reabertura, sem confundir este roteiro com homologacao executada.

## Registro desta consolidacao
- A Sprint 5 esta encerrada tecnicamente.
- A homologacao formal nao foi executada nesta etapa.
- Este documento apenas prepara a rodada posterior de validacao institucional/manual.
- O aceite formal deve ser registrado depois da execucao real dos cenarios abaixo.

## Pre-requisitos
- Ambiente atualizado com as migrations incrementais da Sprint 5 aplicadas.
- Perfis disponiveis: Administrador, Atendente e Solicitante.
- Parametro `chamados.fechamento_automatico.prazo_aceite_horas` configurado.
- Motor de Aprovacoes ITSM da Sprint 4 ativo para cenarios bloqueantes.

## Cenarios obrigatorios
1. Resolver chamado com solucao tecnica obrigatoria.
2. Cancelar chamado com motivo obrigatorio.
3. Aceitar solucao no portal e fechar definitivamente o chamado.
4. Rejeitar solucao no portal e devolver o chamado para atendimento.
5. Fechar automaticamente chamado resolvido por prazo de aceite expirado.
6. Reabrir chamado encerrado dentro da politica de prazo com motivo obrigatorio.
7. Validar bloqueio de fechamento definitivo quando houver aprovacao pendente bloqueante.

## Evidencias a coletar
- Identificacao do ambiente, data e responsavel.
- Prints ou registros dos estados `Resolvido`, `Encerrado` e `EmAtendimento`.
- Evidencia do motivo de cancelamento, rejeicao e reabertura.
- Evidencia do aceite pelo solicitante.
- Evidencia de auditoria/historico.
- Evidencia do bloqueio por aprovacao pendente.

## Resultado esperado
- Todos os cenarios executados com sucesso.
- Sem regressao no fluxo atual de chamados.
- Sem alteracao indevida de SLA ou do Motor de Aprovacoes.
- Aceite formal registrado em documento proprio apos a rodada real.

## Observacao final
A homologacao formal sera executada posteriormente. Enquanto isso, este roteiro permanece como artefato preparatorio do fechamento tecnico da Sprint 5.
