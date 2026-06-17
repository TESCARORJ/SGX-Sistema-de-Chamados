# Sprint 5 - Regra de Rejeição de Solução pelo Solicitante

## Objetivo
Estabelecer a governança sobre a devolução de chamados com soluções inadequadas, garantindo que o solicitante possa sinalizar a não aceitação da solução apresentada, exigindo motivo e retornando o chamado ao status de atendimento.

## Regras de Negócio

1. **Condição Inicial:**
   - Apenas chamados no status `Resolvido` podem sofrer rejeição de solução.

2. **Permissão:**
   - Apenas o usuário solicitante do chamado pode rejeitar a solução apresentada.

3. **Obrigatoriedade de Motivo:**
   - O solicitante deve, obrigatoriamente, fornecer um texto de justificativa para a rejeição.
   - Chamadas à API sem o motivo retornarão falha de validação.

4. **Comportamento do Chamado:**
   - O status do chamado retrocede para `EmAtendimento`.
   - A solução técnica original **não** é apagada, permanecendo para fins de histórico e consulta.
   - O chamado **não** é tratado como `Reaberto` (visto que ainda não estava Fechado).
   - O motivo da rejeição é registrado nos comentários do chamado (indicando ação gerada pelo sistema: "SOLUÇÃO REJEITADA").

5. **Rastreabilidade e Auditoria:**
   - Inserção de registro no histórico (`TipoHistoricoChamado.SolucaoRejeitada`).
   - Gravação dos dados no `IAuditoriaService` para segurança da governança ITIL/ITSM.

6. **Bloqueios de Aprovação Pendente:**
   - Assim como nas transações de reabertura, a rejeição passa pela verificação do Motor de Aprovações para avaliar se há conflito de status e movimentação indevida de fluxos.

## Impacto na Base de Dados
Foi gerada a migration `AdicionarCamposRejeicaoSolucaoSprint5`, contemplando a inclusão das colunas em `Chamados`:
- `SolucaoRejeitadaEm` (datetime nulo)
- `SolucaoRejeitadaPorUsuarioId` (guid nulo)
- `MotivoRejeicaoSolucao` (string nulo)
