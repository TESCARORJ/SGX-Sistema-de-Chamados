# Teste de aprovação multi-nível (Sprint 4 - Item 58)

## 1. Objetivo do item
O objetivo do item 58 é validar através de testes automatizados o comportamento atual do motor de aprovações ITSM referente à orquestração e consolidação multi-nível. A finalidade não é projetar um novo motor orquestrador neste sprint, mas provar a robustez das entidades (`InstanciaAprovacaoChamado`, `EtapaAprovacaoChamado`, `DecisaoAprovacaoChamado`) e a consolidação de status no Application no que tange os atributos `Nivel`, `Ordem` e `Ramo`.

## 2. Status do Domínio em relação a Multi-Nível
O sistema já é capaz de registrar e persistir hierarquias de etapas. A lógica de consolidação da Instância em si já possui a mecânica estrutural para avaliar instâncias baseada em `Etapas` agrupadas por Nível. Testes demonstraram que a aprovação de uma etapa não engatilha a instância para "Aprovada" se houver outra etapa pendente na mesma instância (nível subsequente, ou ordem/ramo obrigatório), mantendo o bloqueio adequado do workflow do chamado. O comportamento atual de multi-nível é validado como **estrutural e funcionalmente consolidado**, mas é importante salientar que um *orquestrador avançado ou autônomo (job worker)* não foi desenvolvido neste contexto, o avanço é impulsionado por eventos de decisão parciais/finais do use case. 

## 3. Cenários testados (Arquivos e Regras)
- `AprovarAprovacaoChamadoUseCaseTests.cs`
  - **DeveAprovarEtapaMultiNivelPreservandoNivelEOrdemERamo**: Valida que a execução de uma etapa de nível 1 é registrada na decisão preservando `NivelEtapaSnapshot=1` e `RamoEtapaSnapshot`. Valida adicionalmente que a instância inteira não é dada como Aprovada imediatamente por ter etapa de `Nivel=2` ainda em aberto. Ao final, a segunda aprovação da etapa do nível 2 coroa a instância como aprovada e persiste `NivelEtapaSnapshot=2` na respectiva decisão.

## 4. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-aprovacao-multi-nivel.md` (este documento).
  - Migration (somente seed de checklist): `20260609231800_ConcluirTesteAprovacaoMultiNivelSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/AprovarAprovacaoChamadoUseCaseTests.cs` (inclusão de teste de aprovação multi-nível)
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (item 58 concluído e atualização percentual)
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (check do item 58 e percentual atualizado para 85%)

## 5. Confirmações e Restrições Atendidas
- **Sem migration estrutural:** O EF Model não foi alterado; nenhuma tabela nova foi criada.
- **Não houve entidade/tabela nova:** Seguiu-se as diretrizes arquiteturais à risca.
- **Zero alteração de Controller e UI:** Como os testes giraram em torno do Application e Domain já existentes, não tocamos na borda.
- **Nenhum status/SLA modificado:** A aprovação não afeta regras de abertura/fechamento indevidamente.
- **Quórum/Delegação inalterados:** Não implementamos workflow avançado, motor paralelo ou sequencial automático além das capacidades já implementadas no motor basal. A consolidação é feita pela leitura das etapas obrigatórias atuais de forma íntegra.

## 6. Resultados do Build e Testes
- Todos os testes referentes à suíte Multi-nível e Checklist compilaram sem problemas.
- Testes rodam verdes com sucesso (`10 testes aprovados de 10` em AprovaçãoUseCase).
- `dotnet ef migrations has-pending-model-changes` indicou **nenhuma** modificação estrutural.

## 7. Riscos e decisões adiadas
- **Avanço automático (Orquestração ativa):** Por decisão de projeto, a transição entre níveis multi-níveis aguarda gatilho do usuário e verificação passiva. Um `worker` ou job que dispare notificações automáticas escalonadas para o `Nivel=N+1` após aprovação de `Nivel=N` fica documentado como evolução futura do Roadmap ITSM, pois excede o limite "Testar" deste item 58. 

## 8. Próxima etapa recomendada
Estamos prontos para prosseguir com o Item 59: **Testar regressão do fluxo atual de aprovação**.
