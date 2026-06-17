# Sprint 5 — Item 21: Testar resolução com solução obrigatória

**Objetivo:** Criar ou reforçar testes automatizados que comprovem que nenhum chamado pode ser resolvido sem solução técnica obrigatória, preservando compatibilidade com o fluxo legado, atendimento, permissões, histórico, auditoria, SLA e regras já implementadas na Sprint 5.

## Status Atual
- **Percentual Concluído:** 66% (21/32 itens concluídos).
- **Testes Realizados:**
  - O `ResolverChamadoUseCase` já exigia que a solução estivesse presente, como garantido pelos testes base da suíte.
  - Novos testes explícitos foram adicionados para validar que **nenhum histórico** e **nenhuma auditoria** são gerados indevidamente se a chamada falhar na regra de solução obrigatória ou inválida (espaços em branco, etc.).
- **Build & Execução:** Build aprovado. Testes aprovados sem falhas (14 de 14 para o ResolverChamadoUseCase, e os demais casos também aprovados). Testes da suíte Checklist aprovados.
- **Migration:** Foi adicionada apenas uma migração `UpdateSeedData_Sprint5_Item21.cs` com a atualização de progresso do SeedData sem alterações estruturais no banco.

## Mudanças Feitas
- `ResolverChamadoUseCaseTests.cs`: Adicionados dois novos testes: `NaoDeveRegistrarHistoricoQuandoSolucaoTecnicaInvalida` e `NaoDeveRegistrarAuditoriaQuandoSolucaoTecnicaInvalida`.
- `SeedData.cs`: Atualizado o `StatusTecnico` e os itens do Checklist referentes ao Roadmap (Id 24).
- `RoadmapSprint5RegrasFechamentoChecklistTests.cs`: Atualizado para refletir o total de 21 itens concluídos e percentual de 66%.
- ROADMAP.md e ROADMAP-ITSM.md: Atualizados refletindo o novo status e progressos.

## Próximo Passo
Avançar para o item: "Testar cancelamento com motivo obrigatório."
