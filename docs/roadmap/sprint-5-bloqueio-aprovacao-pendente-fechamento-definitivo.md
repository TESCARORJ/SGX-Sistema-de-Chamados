# Sprint 5 - Bloqueio por aprovacao pendente antes do fechamento definitivo

## Objetivo
Garantir que os fluxos de fechamento definitivo respeitem o bloqueio por aprovacao pendente ja existente no motor de aprovacoes da Sprint 4.

## Diagnostico consolidado
- `AceitarSolucaoChamadoUseCase` altera o chamado para `StatusChamadoEnum.Encerrado` e ja utiliza `TipoAcaoMovimentacaoChamado.AceitarSolucao`.
- `FecharChamadosAutomaticamentePorPrazoAceiteUseCase` altera o chamado para `StatusChamadoEnum.Encerrado` e ja utiliza `TipoAcaoMovimentacaoChamado.FecharAutomaticamentePorPrazoAceite`.
- `EncerrarChamadoUseCase` segue como fluxo legado de encerramento administrativo para `StatusChamadoEnum.Encerrado` e ja utiliza `TipoAcaoMovimentacaoChamado.Encerrar`.
- O validador da Sprint 4 ja tratava essas acoes como sensiveis/finais; a principal lacuna estava em endurecer o aceite manual com fallback legado quando o validador nao estivesse disponivel e ampliar a cobertura de testes de integracao.

## Escopo entregue
- Aceite manual bloqueado antes da transicao final quando existe aprovacao pendente bloqueante.
- Fechamento automatico continua ignorando chamados bloqueados, sem preencher `EncerradoEm` nem gerar historico/auditoria de sucesso.
- Encerramento administrativo legado mantido com bloqueio antes do encerramento definitivo.
- Cobertura de regressao ampliada para aprovacoes pendentes bloqueantes, nao bloqueantes, resolvidas e de outros chamados.

## Decisoes tecnicas
- O item reutiliza `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`; nenhum novo mecanismo de bloqueio foi criado.
- Nenhuma regra interna do motor de aprovacoes foi alterada.
- Historico funcional e auditoria tecnica continuam sendo gravados apenas quando o fechamento definitivo realmente acontece.
- Comentarios, anexos e triagem seguem fora do bloqueio final, conforme o comportamento ja previsto na Sprint 4.

## Resultado
Item 16 concluido. A Sprint 5 passa a `16/32` itens concluidos e `50%` de implementacao. A proxima acao passa a ser `Ajustar endpoints de resolucao, fechamento, aceite e reabertura`.
