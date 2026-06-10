# Teste de regressão do fluxo atual de aprovação (Sprint 4 - Item 59)

## 1. Objetivo do item
O objetivo do item 59 é garantir que a implantação do novo motor de aprovações ITSM (`InstanciaAprovacaoChamado`) não quebre as regras e fluxos legados associados à entidade `AprovacaoChamado`. Validamos, por meio de testes unitários e de integração, que os mecanismos originais operam em harmonia com o novo motor, sem causar duplicidades, anomalias de status ou sobreposições indevidas.

## 2. Fluxos legados validados
- O fluxo de criação de **aprovação legada (`AprovacaoChamado`)** não foi desativado e se mantém operacional pelos controllers e Use Cases originais (`ChamadoAprovacoesUseCases`).
- **Aprovação, reprovação e cancelamento** de chamados criados na lógica legada funcionam corretamente (`ChamadoAprovacaoUseCaseTests`).
- **Bloqueio de movimentação** continua atuando em simetria, garantindo que aprovações legadas com `bloqueiaAvancoAtendimento = true` impeçam movimentações finais (`BloquearMovimentacaoAprovacaoPendenteUseCaseTests.cs` - Cenário `BloqueiaAcaoFinalQuandoHaAprovacaoLegadaPendenteEBloqueante`).

## 3. Fluxos do motor novo validados em conjunto
- Adição de testes comprovando a **coexistência** pacífica. Instâncias baseadas no novo motor (`InstanciaAprovacaoChamado`) e aprovações do fluxo antigo (`AprovacaoChamado`) pendentes num mesmo chamado não geram curto-circuito.
- O processamento de um request voltado a entidades de tipo legado apenas manipula as entidades de tipo legado e ignora instâncias do motor ITSM, deixando-as intactas e `Pendentes` sem interferência.

## 4. Confirmação de compatibilidade
- Entidades `AprovacaoChamado` e `InstanciaAprovacaoChamado` provaram coexistir com sucesso na `DbContext` de testes. O bloqueador `ValidarBloqueioMovimentacaoAprovacaoPendenteRequest` compreende ambas e identifica qual fonte está trancando a progressão do ticket. 

## 5. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-regressao-fluxo-atual-aprovacao.md` (este documento).
  - Migration (SeedData apenas): `20260609233000_ConcluirTesteRegressaoFluxoAtualAprovacaoSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/ChamadoAprovacaoUseCaseTests.cs`: Adicionado teste de coexistência `DeveCoexistirFluxoLegadoComMotorNovoSemInterferencia` (comprova não sobreposição dos módulos).
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs`: (59/68 - 87%).
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs`: Registro de conclusão do Item 59.

## 6. Confirmações e Restrições Atendidas
- **Sem migration estrutural:** Nenhuma coluna ou tabela do `AprovacaoChamado` original foi removida. O EF Model original permanece intocado.
- **Substituição Evitada:** O fluxo do motor novo não forçou a substituição arbitrária dos contratos atuais de aprovação.
- **Nenhum controller ou endpoint novo:** As APIs seguem isoladas.
- **Workflow e SLAs Inalterados:** O ticket segue sua contagem base de acordo com as permissões da operação escolhida.
- **Quórum/Delegação/Worker:** Mantidos sem desenvolvimento precoce.

## 7. Resultados do Build e Testes
- Suíte `AprovacaoChamado` inteira verde (Fluxo Legado).
- Suítes de Aprovar/Reprovar, Bloqueio e Geração (Fluxo Novo) todas verdes e estáveis.
- `dotnet ef migrations has-pending-model-changes` sem perdas / alterações identificadas.

## 8. Próxima etapa recomendada
Estamos prontos para prosseguir com o Item 60: **Testar regressão de abertura e atendimento de chamado**.
