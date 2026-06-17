# Sprint 5 - Politica de fechamento automatico apos prazo de aceite

## Objetivo
Implementar o Item 12 com a menor mudanca segura para transformar chamados `Resolvidos` em `Encerrados` quando o prazo de aceite do solicitante expirar sem aceite manual nem rejeicao da solucao.

## Regras implementadas
1. Apenas chamados em status `Resolvido` podem ser fechados automaticamente.
2. O fechamento exige `ResolvidoEm` preenchido e prazo positivo.
3. A expiracao e validada com base em `DataReferencia` e `PrazoAceiteHoras`, permitindo testes deterministas.
4. O fechamento definitivo usa o status canonicamente mapeado em `StatusChamadoEnum.Encerrado`.
5. `SolucaoTecnica`, `ResolvidoEm`, dados de rejeicao e demais rastros da resolucao sao preservados.
6. `AceitoEm` e `AceitoPorUsuarioId` nao sao preenchidos no fechamento automatico.
7. `EncerradoEm` passa a registrar a data/hora efetiva do auto-fechamento.
8. Antes de cada fechamento, a politica consulta `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase` para respeitar aprovacoes pendentes bloqueantes.
9. Cada fechamento gera `TipoHistoricoChamado.FechamentoAutomatico` com prazo utilizado, data de resolucao e transicao de status.
10. Cada fechamento gera auditoria especifica no modulo `Chamados`.
11. A partir do Item 13, o prazo passou a ser obtido de configuracao administrativa governada quando o request nao informa valor explicito.

## Escopo entregue
- Metodo de dominio endurecido para fechamento automatico por prazo de aceite.
- Use case de aplicacao ajustado para usar enums canonicos, registrar historico, registrar auditoria e devolver resumo detalhado da execucao.
- Suite de testes ampliada com cenarios de expiracao, bloqueio, preservacao de dados, rejeicao previa e validacao de prazo.
- Roadmap e checklist da Sprint 5 atualizados para 12 de 32 itens concluidos (38%) no Item 12, preservando a evolucao para 13 de 32 itens (41%) no Item 13.

## Fora de escopo
- Scheduler definitivo, job recorrente ou hosted service.
- Frontend, telas administrativas ou portal.
- Nova politica de SLA.
