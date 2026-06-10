# Teste de rejeição de aprovação (Sprint 4 - Item 56)

## 1. Objetivo do item
O objetivo do item 56 é validar através de testes automatizados que o ciclo de vida do motor de aprovações comporta corretamente a cadeia de rejeição/reprovação. Assim como validado no item 55 para o caminho feliz, aqui o foco é comprovar que uma instância bloqueante que impede o avanço de um chamado é corretamente resolvida quando sofre a ação contrária: rejeição via `ReprovarAprovacaoChamadoUseCase`, alterando seus estados persistentes e deixando, imediatamente após, de configurar um bloqueio para o chamado dentro do `BloquearMovimentacaoAprovacaoPendenteUseCase`.

## 2. Cenários testados
Foi introduzido e atestado o cenário principal de integração na suíte `ReprovarAprovacaoChamadoUseCaseTests.cs`:

- **ReprovarInstanciaDeveEncerrarPendenciaDeBloqueioDoChamado**:
  1. O teste constrói um chamado e submete-o a uma instância de aprovação que `ExigeAprovacao` e é `Bloqueante`.
  2. O chamado é despachado ao `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase` testando a ação `Encerrar`, que comprova o bloqueio original (retorna `Bloqueado = true`).
  3. O `ReprovarAprovacaoChamadoUseCase` é então chamado, executando a decisão contrária através de um decisor aprovador válido e salvando na base o novo status `Reprovada` junto de uma respectiva `DecisaoAprovacaoChamado` com `Resultado = Reprovada`.
  4. Nova consulta é feita ao `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`, que agora reconhece a resolução da instância e emite `Bloqueado = false` e `Permitido = true`.

*(As demais garantias vitais de reprovação já estavam testadas nas baterias prévias: registro de decisão, gravação da justificativa, preservação da data de decisão e o disparo adequado de erros quando tentamos reprovar instâncias inexistentes ou que já sofreram resoluções passadas).*

## 3. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-rejeicao-aprovacao.md` (este documento).
  - Migration (dados de seed apenas): `20260609225708_ConcluirTesteRejeicaoAprovacaoSprint4Roadmap.cs`.

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/ReprovarAprovacaoChamadoUseCaseTests.cs` (Cenário principal adicionado).
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (Itens concluídos = 56, percentual = 82%).
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (Status do checklist e ações).

## 4. Confirmações técnicas
- **Sem migration estrutural:** `No changes have been made to the model since the last migration`. Não houve necessidade de acrescentar novos campos na base; as tabelas do domínio já hospedavam corretamente a reprovação.
- **Sem alterações em API e Frontend:** Controller e interfaces foram estritamente preservados em seus comportamentos originais. O frontend continuará exibindo as opções já validadas nos itens 49 e 50.
- **Isolamento de operações:** A reprovação neste contexto altera apenas o motor e a instância; não induz fechamento ou alteração automática de SLA/Status do Chamado (exceto caso futuramente injetemos callbacks via domain events). O comportamento testado manteve esse princípio limpo.

## 5. Resultados de Build e Testes
- Build gerado sem warnings e erros de compilação nas camadas.
- Test suites executaram com perfeição e sem regressions sobre regras de bloqueio e outras tratativas de aprovação. **Aprovado! (62 de 62 testes).**

## 6. Riscos e Decisões Adiadas
O motor em si absorve bem a rejeição, mas como notado, ele apenas remove o bloqueio e altera o status da aprovação (e etapa, se seqüencial). Operações reativas que a rejeição deveria causar de forma cruzada (ex.: cancelar de fato o chamado se a mudança falhou e o solicitante não foi notificado) cabem hoje a quem manipula o serviço mais alto ou por callbacks de eventos a serem criados. Neste item, atestou-se unicamente a higienização do bloqueio.

## 7. Próxima etapa recomendada
Item 57 do roadmap: **Testar aprovação por grupo aprovador**.
