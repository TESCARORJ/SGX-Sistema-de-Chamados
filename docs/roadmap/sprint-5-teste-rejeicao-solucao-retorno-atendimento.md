# Sprint 5 - Teste de Rejeição da Solução e Retorno ao Atendimento (Item 24)

## Objetivo
Criar e reforçar testes automatizados que comprovam que a rejeição da solução pelo solicitante devolve corretamente o chamado ao atendimento, preservando histórico, auditoria, solução registrada e compatibilidade com o ciclo de vida do chamado.

## Contexto
O Item 24 consolida a cobertura de testes para a rejeição da solução no portal do solicitante. A rejeição da solução não deve ser tratada como reabertura de chamado, e a evidência de que a solução foi aplicada anteriormente (`ResolvidoEm`) deve ser preservada, conforme documentado anteriormente.

## Testes Adicionados / Verificados
As seguintes regras de negócio foram formalizadas em testes no `RejeitarSolucaoChamadoUseCaseTests` e integrados com os testes de detalhe:

1. **Retorno ao Atendimento:** Rejeitar a solução de um chamado Resolvido devolve o status para Em Atendimento.
2. **Registro de Rejeição:** A rejeição preenche `SolucaoRejeitadaEm` e `SolucaoRejeitadaPorUsuarioId`.
3. **Motivo Obrigatório:** A rejeição exige e persiste `MotivoRejeicaoSolucao`. A operação falha se o motivo for nulo, vazio ou contiver apenas espaços.
4. **Preservação do Resolvido:** A rejeição preserva o campo `ResolvidoEm` da resolução anterior, garantindo evidência.
5. **Diferenciação de Reabertura e Encerramento:** A rejeição não preenche `ReabertoEm` nem `EncerradoEm`.
6. **Controle de Acesso:** A rejeição falha com `UnauthorizedAccessException` caso o usuário logado não seja o solicitante do chamado.
7. **Consistência de Estado Incompleto:** Se a operação falha (por motivo inválido, acesso negado ou status inválido), não altera status, não registra auditoria técnica e não adiciona histórico funcional.
8. **Detalhamento Mantido:** Os testes de `DetalharChamadoAdminUseCaseTests` continuam validando com sucesso a exibição dos dados da rejeição (`SolucaoRejeitadaEm`, `SolucaoRejeitadaPorNome`, `MotivoRejeicaoSolucao`) no contrato.

## Governança
- **Status do Item 24:** Concluído.
- **Percentual da Sprint 5:** Atualizado para 75% (24/32 itens concluídos).
- **Próxima Ação:** Testar fechamento automático após prazo.
- **Migration de Dados:** Gerada migration com UpdateData para consolidar o estado do SeedData.
