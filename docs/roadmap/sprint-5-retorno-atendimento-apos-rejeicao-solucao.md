# Sprint 5 - Regra de Retorno ao Atendimento Após Rejeição da Solução

## Objetivo
Formalizar a política operacional de retorno de chamados resolvidos para a fila de atendimento, caso a solução apresentada seja rejeitada pelo solicitante. Este documento consolida a garantia de que o chamado retoma seu ciclo de vida sem perdas de histórico e sem falsas interpretações de reabertura.

## Regras de Negócio Implementadas

1. **Retorno ao Status Operacional Ativo:**
   - Ao rejeitar a solução de um chamado `Resolvido`, o sistema reverte o status do chamado para `EmAtendimento`.
   - Isso garante que a fila técnica ou o responsável atual volte a visualizar e atuar no chamado pendente.

2. **Preservação de Evidências:**
   - A coluna `SolucaoTecnica` não é limpa, permitindo que o histórico da tentativa de resolução seja consultado e evite retrabalho.
   - O campo `ResolvidoEm` é mantido preenchido. Isso sinaliza para SLA e relatórios que o chamado teve uma tentativa formal de resolução técnica documentada no passado.
   - Os dados da rejeição (`SolucaoRejeitadaEm`, `SolucaoRejeitadaPorUsuarioId`, `MotivoRejeicaoSolucao`) são mantidos na tabela, garantindo rastreabilidade estruturada além do histórico.

3. **Governança de Ciclo de Vida:**
   - Rejeição de solução **não** é considerada uma Reabertura. Reaberturas aplicam-se apenas a chamados no status `Fechado`.
   - O campo `EncerradoEm` permanece **nulo** ou vazio, indicando que o chamado ainda transita em seu ciclo de vida principal e não sofreu encerramento definitivo.
   - O evento dispara o registro `TipoHistoricoChamado.SolucaoRejeitada` na linha do tempo do chamado, com o motivo escrito nos comentários do sistema ("SOLUÇÃO REJEITADA").

4. **Bloqueios de Aprovação (Aderência à Sprint 4):**
   - Transições de status provocadas por rejeição continuam respeitando os mecanismos de validação do motor de aprovação, garantindo que o chamado não fuja de avaliações bloqueantes.

## Impacto na Base de Dados e Testes
- Não foram criadas migrations estruturais novas, visto que a migration `AdicionarCamposRejeicaoSolucaoSprint5` (criada no Item 10) já contempla todos os campos estruturais da rejeição.
- Foram introduzidos testes de unidade/integração validando a transição de status para `EmAtendimento`, auditoria e rejeição com campos preservados.
- O percentual da Sprint 5 avança para **34%** com a consolidação e blindagem dessa regra.
