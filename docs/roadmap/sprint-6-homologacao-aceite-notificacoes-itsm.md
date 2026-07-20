# Sprint 6 - Homologacao e Aceite de Notificacoes ITSM

## Estado formal atual

- Sprint 6 mantida em `15/16 - 94%`.
- `StatusImplementacao = ImplementadoFuncionalmente`.
- `StatusTecnico = CompletoComPendenciasEvolutivas`.
- Item `16 - Documentar, homologar e registrar aceite da Sprint 6` permanece pendente.
- Nenhuma migration de conclusao foi criada.
- Nenhum aceite formal foi registrado.

## Objetivo desta etapa

Diagnosticar e corrigir as falhas da suite completa por causa raiz, sem concluir a sprint prematuramente e sem ampliar escopo funcional.

## Resultado inicial da suite completa

Comando:

```powershell
dotnet test tests/SGX.SistemaChamado.Tests /p:UseSharedCompilation=false --logger "console;verbosity=normal" --logger "trx;LogFileName=sprint6-suite-completa.trx"
```

Resultado observado nesta execucao:

- Total: `1765`
- Aprovados: `1736`
- Falhas: `29`
- Ignorados: `0`
- Duracao aproximada: `1m38s`

Observacao:

- o briefing citava `39` falhas, mas o inventario reproduzido no ambiente atual apontou `29` falhas deterministicas;
- nenhuma falha foi classificada como "preexistente" sem evidencia.

## Inventario consolidado das falhas

### Grupo A - Roadmap e seed

1. `RoadmapSprint6NotificacoesChecklistTests.RoadmapSprint6DeveRefletirIntegracaoEventosConcluidaSemAnteciparAceiteFinal`
   - Classificacao: `3. Seed inconsistente` e `2. Teste desatualizado`
   - Causa: `SeedData.cs` ainda refletia estado parcial da Sprint 6 e o teste esperava `StatusTecnico` antigo.

### Grupo B - Configuracao de fechamento automatico

1. `ConfiguracaoAutoFechamentoChamadoUseCasesTests.Obtem_Configuracao_Atual_De_Prazo_De_Auto_Fechamento`
2. `ConfiguracaoAutoFechamentoChamadoUseCasesTests.Atualiza_Prazo_Com_Valor_Valido`
   - Classificacao: `3. Seed inconsistente`
   - Causa: `ParametroSistemaConfiguration` nao aplicava `HasData`, entao os contextos de teste sem migration ficavam sem os parametros seedados.

### Grupo C - Resolver chamado e auditoria de fechamento

1. `AuditoriaCicloFechamentoChamadoTests.AuditoriaCicloFechamentoChamado_DeveRegistrarUmUnicoEventoPorFluxoCritico`
2. `ResolverChamadoUseCaseTests.Deve_Resolver_Chamado_Com_Sucesso`
3. `ResolverChamadoUseCaseTests.Deve_Gerar_Historico_Ao_Resolver`
4. `ResolverChamadoUseCaseTests.Deve_Manter_Data_Resolucao_Do_Request_Quando_Informada`
5. `ResolverChamadoUseCaseTests.Deve_Definir_Data_Resolucao_Atual_Quando_Request_Nao_Informa_Data`
6. `ResolverChamadoUseCaseTests.Deve_Permitir_Resolver_Chamado_Com_Aprovacao_Aprovada`
7. `ResolverChamadoUseCaseTests.Deve_Resolver_Chamado_Sem_Aprovacao`
8. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Com_Aprovacao_Pendente`
9. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Sem_Descricao_Da_Solucao`
10. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Ja_Fechado`
11. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Cancelado`
12. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Com_Approval_Rejeitada`
13. `ResolverChamadoUseCaseTests.Nao_Deve_Resolver_Chamado_Sem_Permissao`
   - Classificacao predominante: `1. Regressao funcional real`
   - Causa principal: `AcaoChamadoEnum.Resolver` nao era disponibilizada por `AcoesChamadoService`.
   - Causa complementar: o use case mutava `ResolvidoEm` antes de rejeitar entrada sem solucao.

### Grupo D - Ciclo de fechamento via controller

1. `ChamadoCicloFechamentoControllerIntegrationTests.Admin_Deve_Resolver_Chamado_Com_Sucesso`
2. `ChamadoCicloFechamentoControllerIntegrationTests.Admin_Resolver_Deve_Retornar_400_Quando_Request_Invalido`
3. `ChamadoCicloFechamentoControllerIntegrationTests.Admin_Resolver_Deve_Retornar_404_Quando_Chamado_Nao_Encontrado`
4. `ChamadoCicloFechamentoControllerIntegrationTests.Portal_Deve_Aceitar_Solucao_Com_Sucesso`
5. `ChamadoCicloFechamentoControllerIntegrationTests.Portal_AceitarSolucao_Deve_Retornar_404_Quando_Chamado_Nao_Encontrado`
6. `ChamadoCicloFechamentoControllerIntegrationTests.Portal_Deve_Rejeitar_Solucao_Com_Sucesso`
7. `ChamadoCicloFechamentoControllerIntegrationTests.Portal_RejeitarSolucao_Deve_Retornar_400_Quando_Request_Invalido`
8. `ChamadoCicloFechamentoControllerIntegrationTests.Portal_RejeitarSolucao_Deve_Retornar_404_Quando_Chamado_Nao_Encontrado`
9. `ChamadoCicloFechamentoControllerIntegrationTests.Admin_Deve_Executar_Fechamento_Automatico_Com_Sucesso`
10. `ChamadoCicloFechamentoControllerIntegrationTests.Admin_ExecutarFechamentoAutomatico_Deve_Retornar_401_Sem_Autenticacao`
   - Classificacao predominante: `6. Contrato ou rota alterada`
   - Causa: rotas publicas esperadas nao existiam, e alguns use cases/contratos nao estavam registrados no DI.

### Grupo E - Persistencia PostgreSQL

- Nenhuma falha na suite completa inicial.
- Instabilidade reproduzida depois em filtro amplo de persistencia de notificacoes.
  - Classificacao: `8. Fixture PostgreSQL instavel` e `10. Ordem de execucao`
  - Causa: colecao relacional de notificacoes rodando em paralelo e disputando fixture/conexoes.

### Grupo F - Demais falhas

1. `ProcessarEventoCandidatoNotificacaoUseCaseTests.Deve_Gerar_Notificacao_Quando_Evento_Eh_Valido`
2. `ProcessarEventoCandidatoNotificacaoUseCaseTests.Deve_Gerar_Duas_Notificacoes_Quando_Houver_Destinatarios_Distintos`
3. `ProcessarEventoCandidatoNotificacaoUseCaseTests.Deve_Respeitar_Canal_Do_Template_Ao_Gerar_Notificacao`
   - Classificacao: `5. Mock/fake incompleto`
   - Causa: template de teste com `VigenteDe = DateTime.UtcNow.AddDays(-1)` deixava de ser vigente em relacao ao timestamp fixo do evento.

## Correcao aplicada por grupo

### Grupo A

- `SeedData.cs` foi sincronizado para o estado correto da Sprint 6:
  - `Ativos = 16`
  - `Concluidos = 15`
  - `Pendentes = 1`
  - `Percentual = 94`
  - item 16 pendente
  - `StatusImplementacao = EmDesenvolvimento`
  - `StatusTecnico = Bloqueado`
- `RoadmapSprint6NotificacoesChecklistTests.cs` foi atualizado apenas para alinhar a expectativa valida de `StatusTecnico`.

### Grupo B

- `ParametroSistemaConfiguration.cs` passou a aplicar `SeedData.ParametrosSistema`.
- Os parametros de auto fechamento e reabertura passaram a existir tanto em migration quanto em contextos de teste baseados em `EnsureCreated`.

### Grupo C

- `AcoesChamadoService.cs` passou a expor a acao `Resolver` quando o fluxo e realmente permitido.
- `ResolverChamadoUseCase.cs` ganhou validacao defensiva antes de mutar o agregado para impedir efeito colateral em request invalido.

### Grupo D

- `AdminChamadosController.cs` recebeu rotas para:
  - resolver chamado;
  - executar fechamento automatico por prazo de aceite.
- `PortalController.cs` recebeu rotas para:
  - aceitar solucao;
  - rejeitar solucao.
- `DependencyInjection.cs` passou a registrar os use cases e contratos faltantes do ciclo de fechamento.

### Grupo E

- Foi criada a colecao `NotificacoesPersistenceRelational` com `DisableParallelization = true`.
- Os testes relacionais de notificacoes foram vinculados apenas a essa colecao, sem desabilitar o paralelismo global da suite.

### Grupo F

- `NotificacoesItsmTestFactory.cs` passou a usar data deterministica para `VigenteDe`, removendo dependencia do relogio da maquina.

## Revisao da migration de sincronizacao

Migration revisada:

- `20260624025716_SincronizarSeedRoadmapSprint6Notificacoes`

Arquivos inspecionados:

- `20260624025716_SincronizarSeedRoadmapSprint6Notificacoes.cs`
- `20260624025716_SincronizarSeedRoadmapSprint6Notificacoes.Designer.cs`
- `SGXSistemaChamadoDbContextModelSnapshot.cs`
- `SeedData.cs`

Conclusoes da revisao:

- nao remove Sprint 5;
- nao conclui a Sprint 6;
- nao promove percentual para `100`;
- preserva o item `16` como pendente;
- nao altera `Worker.Email`;
- nao mistura regra de producao nova com escopo fora da sprint.

Correcao adicional necessaria:

- foi criada a migration corretiva `20260624150827_SincronizarSeedSprint6AutoFechamentoERoadmap`;
- motivo: alinhar `parametros_sistema` com o seed e sincronizar o roadmap/percentual sem depender de `InsertData` cego;
- a migration corretiva foi ajustada para `upsert` dos parametros estaveis, porque o banco local ja possuia os IDs seedados.

## Regresses e suite final

### Testes por grupo

- `RoadmapSprint6NotificacoesChecklistTests`: aprovado
- `RoadmapPercentualChecklistRulesTests`: `5/5`
- `ConfiguracaoAutoFechamentoChamadoUseCasesTests`: `9/9`
- `ResolverChamadoUseCaseTests`: `13/13`
- `AuditoriaCicloFechamentoChamadoTests`: `2/2`
- `ChamadoCicloFechamentoControllerIntegrationTests`: `14/14`
- `NotificacaoPersistenceTests` repetido `3x`: aprovado em todas

### Regresses direcionadas

- `Roadmap|Checklist`: `32/32`
- `ConfiguracaoAutoFechamento|ResolverChamado|CicloFechamento`: `41/41`
- `NotificacaoPersistence|ProcessamentoNotificacao|EntregaNotificacao|LeituraNotificacao`: `49/49`
- bloco completo de notificacoes: `265/265`
- atendimento: `155/155`
- aprovacao: `90/90`
- SLA: `59/59`
- autenticacao/API: `47/47`

### Suite completa final

Comando:

```powershell
dotnet test tests/SGX.SistemaChamado.Tests /p:UseSharedCompilation=false --logger "trx;LogFileName=sprint6-suite-final.trx"
```

Resultado:

- Total: `1765`
- Aprovados: `1765`
- Falhas: `0`
- Ignorados: `0`
- Duracao aproximada: `2m02s`

## Backend, frontend, banco e EF

- `dotnet clean SGX.SistemaChamado.sln`: aprovado
- `dotnet build SGX.SistemaChamado.sln /p:UseSharedCompilation=false`: aprovado
- `npm.cmd run test:unit -- notificacoes`: `8/8`
- `npx.cmd vue-tsc --noEmit`: aprovado
- `npm.cmd run build`: aprovado
- `dotnet ef migrations list -p src/SGX.SistemaChamado.Infrastructure -s src/SGX.SistemaChamado.Api`: executado
- `dotnet ef database update -p src/SGX.SistemaChamado.Infrastructure -s src/SGX.SistemaChamado.Api`: aprovado apos ajuste da migration corretiva
- `dotnet ef migrations has-pending-model-changes -p src/SGX.SistemaChamado.Infrastructure -s src/SGX.SistemaChamado.Api`: sem pending changes

## Riscos e limitacoes

- a suite foi estabilizada sem ampliar funcionalidade, mas a homologacao visual ainda nao foi executada;
- existe warning nao bloqueante de chunk size no build frontend;
- o item 16 ainda depende de validacao manual dos fluxos visuais `/portal/notificacoes` e `/admin/notificacoes`.

## Homologacao visual

Estado:

- nao executada nesta rodada.

Ressalva:

- nao ha evidencias formais de verificacao visual em `320px`, `375px`, `768px` e desktop;
- por isso, o item 16 nao deve ser marcado como concluido nesta entrega.

## Resultado formal desta rodada

- Situacao: `Bloqueada`

Justificativa:

- os bloqueios tecnicos automatizados foram resolvidos;
- porem a nova homologacao visual/manual exigida para o item 16 ainda nao foi executada;
- sem essa etapa, nao cabe registrar aceite formal nem promover a Sprint 6 para `16/16`.
