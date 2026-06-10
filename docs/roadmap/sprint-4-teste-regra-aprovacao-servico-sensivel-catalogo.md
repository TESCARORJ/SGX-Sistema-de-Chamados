# Teste da regra de aprovação por Serviço Sensível / Catálogo de Serviços (Sprint 4 - Item 53)

## 1. Objetivo do item
O objetivo do item 53 é atestar, através de testes automatizados, que o motor de aprovações utiliza e avalia o critério `CatalogoServicoId` de maneira correta para impor aprovação a chamados vinculados a um serviço sensível pertencente ao Catálogo de Serviços.

## 2. O que foi testado
Foram adicionados os seguintes testes ao caso de uso `GerarAprovacaoObrigatoriaChamadoUseCase` para comprovar os comportamentos de catálogo:

- `DeveGerarQuandoRegraCompativelComCatalogo`: Garante que, caso a regra de aprovação exija um `CatalogoServicoId` e o chamado o contenha de forma idêntica, a aprovação é gerada de maneira obrigatória.
- `NaoDeveGerarQuandoRegraExigeCatalogoDiferente`: Caso o chamado tenha um serviço diferente do estipulado na regra, a aprovação não é gerada e o fluxo segue.
- `DevePermitirRegraGenericaSemCatalogoDefinido`: Caso o chamado possua um catálogo e a regra não exija nenhum, a regra genérica se aplica normalmente.
- `DevePreferirRegraEspecificaPorCatalogoEmVezDaGenerica`: Quando as duas (acima) concorrem, o sistema dá precedência para a regra específica vinculada ao Catálogo de Serviços, gerando aprovação por essa via em vez da regra genérica.

Os cenários foram escritos de forma isolada, contendo a injeção do valor `CatalogoServicoId` na entidade Chamado, evitando vazamento e acoplamento com lógicas de persistência reais.

## 3. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-regra-aprovacao-servico-sensivel-catalogo.md` (este documento)
  - Nova migration de SeedData: `20260609115903_ConcluirTesteRegraAprovacaoServicoSensivelCatalogoSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/GerarAprovacaoObrigatoriaChamadoUseCaseTests.cs` (inclusão dos 4 novos testes e de um parâmetro opcional em `CriarRegra`)
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (atualização para 78% de avanço e 53 itens concluídos)
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (marcando o item 53 como `Concluido = true` e atualizando a próxima ação)

## 4. Confirmações técnicas

- **Sem migration estrutural:** O comando `dotnet ef migrations has-pending-model-changes` validou a ausência de mudanças, retornando a mensagem "No changes have been made to the model since the last migration."
- **Sem novos endpoints:** Nenhum controller ou API foi modificada, e a estabilidade foi preservada.
- **Sem alterações visuais (Frontend):** O ambiente do Vue permaneceu intocado.
- **Sem alterações operacionais de domínio/fluxo:** As regras primárias de SLA, criação de chamados ou atendimento mantiveram-se as mesmas; apenas testamos a regra dentro do motor ITSM.

## 5. Resultados de Build e Testes
- Build gerado sem quebras.
- Foram executados todos os 33 testes que se enquadraram no escopo de *GerarAprovacaoObrigatoria*, *ConfiguracaoRegraAprovacao*, *ServicoAplicacaoRegrasAprovacao*, além de *RoadmapSprint4MotorAprovacoesChecklistTests*.
- Todos os testes foram **Aprovados (33 de 33)**, atestando o não vazamento da alteração e sucesso total nos novos cenários.

## 6. Riscos e Decisões Adiadas
Atualmente, o domínio utiliza `CatalogoServicoId` para relacionar um chamado ao serviço, e a regra também. Em iterações futuras, se a tabela real do Catálogo de Serviços (`CatalogoServicos`) sofrer expansão ou requerer lógicas booleanas diretas (ex.: `catalogo.RequerAprovacao`), caberá um refinamento na sincronia de cadastro para alimentar automaticamente as regras do motor.

## 7. Próxima etapa recomendada
Item 54 do roadmap: **Testar bloqueio por aprovação pendente**.
