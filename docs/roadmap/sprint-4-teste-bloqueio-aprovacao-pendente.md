# Teste de bloqueio por aprovação pendente (Sprint 4 - Item 54)

## 1. Objetivo do item
O objetivo do item 54 é atestar, através de testes automatizados, que o mecanismo de bloqueio (`ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`) age corretamente para impedir o avanço e modificação de status de chamados quando há uma aprovação pendente de característica "bloqueante" (`EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco`). Além disso, assegurar que aprovações apenas informativas ou já concluídas (aprovadas/reprovadas/canceladas/etc.) não impeçam o trâmite normal.

## 2. O que foi testado
A bateria de testes foi complementada na classe `BloquearMovimentacaoAprovacaoPendenteUseCaseTests` para garantir os seguintes cenários:

- **Bloqueio Efetivo**:
  - `BloqueiaAcaoFinalQuandoHaInstanciaEmReavaliacaoEBloqueante`: Garante que, além do status "Pendente", o status "EmReavaliacao" também acione o bloqueio impeditivo.
- **Passagem Livre (Aprovações não bloqueantes)**:
  - `NaoBloqueiaQuandoInstanciaNaoExigeAprovacao`: Garante que uma aprovação puramente informativa (`ApenasNotificar`/`Sinalizar`) não bloqueia a movimentação.
- **Isolamento de Chamado**:
  - `NaoBloqueiaPorInstanciaPendenteDeOutroChamado`: Comprova que a existência de uma aprovação bloqueante pendente em um *Chamado A* não interfere e não bloqueia a movimentação de um *Chamado B*.

*(Os demais cenários exigidos, como não bloquear por aprovação já aprovada, reprovada, substituída ou expirada, bem como não bloquear ações secundárias como "Comentar" ou "Triagem", já estavam implementados e testados nativamente).*

## 3. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-bloqueio-aprovacao-pendente.md` (este documento)
  - Nova migration de SeedData: `20260609194645_ConcluirTesteBloqueioAprovacaoPendenteSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/BloquearMovimentacaoAprovacaoPendenteUseCaseTests.cs` (inclusão dos 3 novos testes e ajuste de compilação da enumeração)
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (atualização para 79% de avanço e 54 itens concluídos)
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (marcando o item 54 como `Concluido = true` e atualizando a próxima ação)

## 4. Confirmações técnicas

- **Sem migration estrutural:** O comando `dotnet ef migrations has-pending-model-changes` validou a ausência de mudanças de modelo, retornando "No changes have been made to the model since the last migration."
- **Sem novos endpoints:** Nenhum controller ou API nova foi inserida.
- **Sem alterações visuais (Frontend):** Nenhuma alteração no código Vue.
- **Sem alterações operacionais indevidas:** Todo o comportamento primário de SLA, criação de chamados, atendimento e workflow foi mantido. Apenas comprovamos e aumentamos a cobertura da classe validadora.

## 5. Resultados de Build e Testes
- Build gerado com sucesso em toda a solução.
- Foram executados de forma direcionada os testes que continham `BloquearMovimentacaoAprovacaoPendente`, `InstanciaAprovacaoChamado`, `AprovarAprovacaoChamado`, `ReprovarAprovacaoChamado` e `RoadmapSprint4MotorAprovacoesChecklistTests`.
- Todos os testes foram **Aprovados** (aumento no volume para contemplar os 3 novos). Zero falhas registradas.

## 6. Riscos e Decisões Adiadas
Não há riscos ou regressões identificadas. O mecanismo central de bloqueio opera em conformidade com as regras configuráveis do motor de aprovações. Decisões como o bloqueio da interface (UI) no Front-End já foram implementadas e não sofreram alterações ou impactos, permanecendo estáveis.

## 7. Próxima etapa recomendada
Item 55 do roadmap: **Testar aprovação e liberação do chamado**.
