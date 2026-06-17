# Sprint 5 — Item 22: Testar cancelamento com motivo obrigatório

**Objetivo:** Criar ou reforçar testes automatizados que comprovem que nenhum chamado pode ser cancelado sem um motivo de cancelamento obrigatório, preservando compatibilidade com o fluxo legado, SLA, motor de aprovações e frontend. Não deve registrar histórico/auditoria indevidamente quando o cancelamento for recusado.

## Status Atual
- **Percentual Concluído:** 69% (22/32 itens concluídos).
- **Testes Realizados:**
  - O caso de uso `CancelarChamadoUseCase` já exigia que o motivo estivesse preenchido e não permitia o cancelamento com motivo vazio, nulo ou preenchido apenas com espaços, como já validado por testes existentes (`NaoDeveCancelarChamadoComMotivoVazio`, `NaoDeveCancelarChamadoComMotivoNulo`, `NaoDeveCancelarChamadoComMotivoSomenteEspacos`).
  - Novos testes foram adicionados para atestar a ausência de registro indevido em histórico e auditoria: `NaoDeveRegistrarHistoricoQuandoMotivoCancelamentoInvalido` e `NaoDeveRegistrarAuditoriaQuandoMotivoCancelamentoInvalido`.
- **Build & Execução:** Build backend aprovado. Testes aprovados sem falhas (11 testes em `CancelarChamadoUseCaseTests`).
- **Migration:** Foi executada a migração (`UpdateSeedData_Sprint5_Item22`) para a atualização do progresso no banco de dados.

## Mudanças Feitas
- `CancelarChamadoUseCaseTests.cs`: Inclusão dos testes faltantes de histórico e auditoria garantindo que não há persistência caso o cancelamento falhe por motivo inválido.
- `SeedData.cs`: Atualizado o `PercentualImplementacao` para 69% e a `ProximaAcao`. O item de checklist `Testar cancelamento com motivo obrigatorio` (Ordem 22) foi concluído.
- `RoadmapSprint5RegrasFechamentoChecklistTests.cs`: Modificado para aprovar percentual de 69% e verificar 22 itens concluídos da lista de verificação.
- `ROADMAP.md` e `ROADMAP-ITSM.md`: Atualizados para refletir a nova métrica de progresso (69%).

## Próximo Passo
Avançar para o item: "Testar aceite e fechamento definitivo."
