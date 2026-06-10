# Teste de aprovação por grupo aprovador (Sprint 4 - Item 57)

## 1. Objetivo do item
O objetivo do item 57 é validar através de testes automatizados o comportamento atual em relação a aprovações por "grupo aprovador". Antes de projetar entidades e regras completas de times e atribuições múltiplas, precisávamos confirmar se a estrutura da `InstanciaAprovacaoChamado` e `DecisaoAprovacaoChamado` está preparada para suportar essas configurações, bem como os enumeradores `GrupoAprovadorFuturo`. O resultado almejado é provar a flexibilidade do nosso modelo base sem criar precipitadamente endpoints não planejados no sprint.

## 2. Status do Domínio em relação a Grupo Aprovador
Confirmamos que o **grupo aprovador ainda não é uma funcionalidade plena com tabelas próprias de times/usuários vinculados de forma dinâmica**. Ele atua como um previsor / marcador estrutural (Snapshots, flags e o Enum `TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo`). Por consequência:
- A base aceita configurações que apontem "tipo de resolução será por grupo".
- Instâncias são geradas com esse tipo marcado, mas sem atrelar um aprovador prévio específico (não forja usuário).
- Decisões registradas na base já aceitam a flag `DecisorEhMembroGrupo`.
- Não há ainda resolução de distribuição do quórum de aprovações entre multi-membros, o qual será alvo de itens dedicados a quórum/multi-nível no futuro da roadmap.

## 3. Cenários testados (Arquivos e Regras)
- `ConfiguracaoRegraAprovacaoTests.cs`
  - **DeveCriarRegraSinalizandoGrupoAprovadorFuturo**: Assegura que podemos construir as configurações usando o enumerador em questão.

- `GerarAprovacaoObrigatoriaChamadoUseCaseTests.cs`
  - **DeveGerarInstanciaComTipoResolucaoPorGrupoFuturoSemTentarResolverAprovador**: Confirma que o motor enxerga o enumerador e assina o `AprovadorResolvidoUsuarioId` como nulo (ou seja, não insere um usuário padrão forçosamente), atestando a configuração para "grupo".

- `AprovarAprovacaoChamadoUseCaseTests.cs`
  - **DeveAceitarAprovacaoParaInstanciaDeGrupoAprovadorFuturo**: Simula um cenário onde um aprovador vota em prol da instância com este tipo, passando a flag de membro e atestando que a aprovação avança validando o Snapshot nulo sem erros (por não existirem dados estruturais dinâmicos ainda implementados).

## 4. Arquivos criados e alterados

- **Criados:**
  - `docs/roadmap/sprint-4-teste-aprovacao-grupo-aprovador.md` (este documento).
  - Migration (somente seed de checklist): `20260609230408_ConcluirTesteAprovacaoGrupoAprovadorSprint4Roadmap.cs`

- **Alterados:**
  - `tests/SGX.SistemaChamado.Tests/ConfiguracaoRegraAprovacaoTests.cs` (inclusão de teste de grupo)
  - `tests/SGX.SistemaChamado.Tests/GerarAprovacaoObrigatoriaChamadoUseCaseTests.cs` (inclusão de teste do grupo futuro)
  - `tests/SGX.SistemaChamado.Tests/AprovarAprovacaoChamadoUseCaseTests.cs` (inclusão de teste de aprovação de grupo)
  - `tests/SGX.SistemaChamado.Tests/RoadmapSprint4MotorAprovacoesChecklistTests.cs` (item 57 concluído e atualização percentual)
  - `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs` (check do item 57 e percentual atualizado para 84%)

## 5. Confirmações e Restrições Atendidas
- **Sem migration estrutural de grupos:** O EF Model não foi alterado; nenhuma tabela nova como `GrupoAprovador` foi criada.
- **Não houve entidade/tabela nova:** Seguiu-se as diretrizes arquiteturais à risca.
- **Zero alteração de Controller e UI:** Como os testes giraram em torno do Application e Domain já existentes, não encostamos na borda ou no SPA.
- **Nenhum status/SLA modificado:** O Workflow do chamado se manteve inalterado.
- **Quórum/Delegação/Multi-nível inalterados:** Como ditava o pedido, o quórum e a delegação reais ainda serão avaliados em itens posteriores; não inventamos workflow autônomo.

## 6. Resultados do Build e Testes
- Todos os testes referentes à suíte de Grupo Aprovador e Checklist compilaram sem problemas.
- Testes específicos rodam verdes em total cobertura de requisitos.
- `dotnet ef migrations has-pending-model-changes` indicou **nenhuma** modificação estrutural.

## 7. Próxima etapa recomendada
Estamos prontos para prosseguir com o Item 58: **Testar aprovação multi-nível**.
