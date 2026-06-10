# Teste da regra de aprovação por Natureza ITSM (Sprint 4 - Item 52)

## 1. Objetivo do item
O objetivo do item 52 é validar tecnicamente, por meio de testes automatizados unitários, que o motor de aprovações é capaz de selecionar e gerar instâncias de aprovação de forma correta com base no critério `NaturezaChamado`, mantendo compatibilidade com as regras de prioridade e duplicidade já estabelecidas.

## 2. O que foi testado
A bateria de testes do caso de uso `GerarAprovacaoObrigatoriaChamadoUseCase` foi complementada para atestar os comportamentos da `NaturezaChamado`, abrangendo:
- Aprovação gerada quando a regra específica corresponde exatamente à natureza do chamado (`DeveGerarQuandoRegraCompativelComNatureza`).
- Aprovação negada/ignorada quando a regra exige uma natureza diferente da do chamado (`NaoDeveGerarQuandoRegraExigeNaturezaDiferente`).
- Regra genérica (sem `NaturezaChamado` preenchido na regra) abrange qualquer chamado com sucesso (`DevePermitirRegraGenericaSemNaturezaDefinida`).
- Em cenário de conflito/concorrência, o motor escolhe a regra mais específica (aquela com `NaturezaChamado` preenchido) em detrimento da regra genérica (`DevePreferirRegraEspecificaEmVezDaGenerica`).
- Especificamente para *Incidente*, apenas regra compatível com Incidente é aplicada e outras são ignoradas (`DeveAplicarSomenteRegraParaIncidenteQuandoChamadoForIncidente`).
- Especificamente para *Requisição de Serviço*, apenas regra compatível com Requisição é aplicada e outras ignoradas (`DeveAplicarSomenteRegraParaRequisicaoQuandoChamadoForRequisicao`).

Os cenários de duplicidade de instância e legado já possuíam cobertura e continuaram sendo aprovados, não sofrendo interferência.

## 3. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-regra-aprovacao-natureza-itsm.md` (este documento)
  - Nova migration de SeedData: `20260609110753_ConcluirTesteRegraAprovacaoNaturezaItsmSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/GerarAprovacaoObrigatoriaChamadoUseCaseTests.cs` (inclusão dos 6 novos testes)
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (atualização para 76% de avanço e 52 itens concluídos)
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (marcando o item 52 como `Concluido = true`)

## 4. Confirmações técnicas

- **Sem migration estrutural:** O comando `dotnet ef migrations has-pending-model-changes` retornou sem alterações. Nenhuma nova tabela ou coluna foi criada.
- **Sem novos endpoints:** Nenhum controller foi adicionado ou modificado.
- **Sem alterações visuais (Frontend):** O frontend manteve-se íntegro.
- **Sem alterações operacionais (Backend):** Nenhuma classe de entidade (`Chamado`) ou regras core de abertura, SLA, encerramento ou status foi adulterada. A lógica interna do backend já estava funcional e preparada para esse teste.

## 5. Resultados de Build e Testes
- Todos os projetos da solução (Domain, Application, Infrastructure, Worker, Api, Web e Tests) compilaram com êxito sem novos Warnings/Erros na pipeline principal.
- Os 13 cenários de testes do `GerarAprovacaoObrigatoriaChamadoUseCase` foram concluídos (Aprovado: 13, Falha: 0).
- Todos os testes de regras de aprovação preexistentes (`ConfiguracaoRegraAprovacao`) foram executados sem quebras (Aprovado: 7, Falha: 0).
- O teste do Roadmap confirmou 52 de 68 itens finalizados (76%).

## 6. Riscos e Decisões Adiadas
Não há riscos iminentes levantados nesta etapa, uma vez que se trata puramente da confirmação do comportamento já esperado da entidade e do UseCase. O uso da `NaturezaChamado` em futuras etapas da Interface do Usuário foi abstraído.

## 7. Próxima etapa recomendada
Item 53 do roadmap: **Testar regra de aprovação por serviço sensível** (foco em Catálogo de Serviços).
