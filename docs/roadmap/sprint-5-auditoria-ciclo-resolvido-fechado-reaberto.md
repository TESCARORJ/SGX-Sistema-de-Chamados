# Sprint 5 - Auditoria do ciclo resolvido/fechado/reaberto

## Objetivo
Consolidar a auditoria tecnica dos eventos criticos do ciclo de encerramento do chamado sem recriar os fluxos funcionais existentes.

## Escopo entregue
- Resolucao com auditoria padronizada contendo status anterior, status novo, solucao tecnica e `ResolvidoEm`.
- Aceite da solucao com auditoria padronizada contendo status anterior, status novo, `AceitoEm`, `AceitoPorUsuarioId` e `EncerradoEm`.
- Rejeicao da solucao com auditoria padronizada contendo status anterior, status novo, motivo e dados de rejeicao.
- Fechamento automatico por prazo com auditoria padronizada contendo status anterior, status novo, `ResolvidoEm`, prazo usado, origem automatica e `EncerradoEm`.
- Reabertura controlada com auditoria padronizada contendo status anterior, status novo, motivo, prazo maximo aplicado e referencia de `EncerradoEm`.

## Decisoes tecnicas
- Foi mantido o `IAuditoriaService` existente como mecanismo unico de auditoria.
- Foram adicionadas acoes especificas em `TipoAcaoAuditoria` para resolucao, aceite, rejeicao, fechamento automatico por prazo de aceite e reabertura.
- Historico funcional e auditoria tecnica permaneceram separados.
- O comportamento legado de limpar `EncerradoEm` na reabertura foi preservado; a auditoria registra o valor anterior como evidencia do ciclo encerrado.
- A rejeicao e o fechamento automatico passaram a registrar a auditoria somente apos a persistencia principal do fluxo, reduzindo risco de trilha tecnica sem alteracao efetivamente salva.

## Diagnostico consolidado
| Evento | Use case | Metodo de dominio | Historico | Acao de auditoria | Campos principais auditados |
| --- | --- | --- | --- | --- | --- |
| Resolucao | `ResolverChamadoUseCase` | `Chamado.Resolver` | `TipoHistoricoChamado.Resolvido` | `TipoAcaoAuditoria.ResolverChamado` | `ChamadoId`, executor, `StatusAnterior`, `StatusNovo`, `SolucaoTecnica`, `ResolvidoEm`, `DataEventoUtc` |
| Aceite | `AceitarSolucaoChamadoUseCase` | `Chamado.AceitarSolucao` | `TipoHistoricoChamado.SolucaoAceita` | `TipoAcaoAuditoria.AceitarSolucaoChamado` | `ChamadoId`, executor, `StatusAnterior`, `StatusNovo`, `AceitoEm`, `AceitoPorUsuarioId`, `EncerradoEm`, `ObservacaoAceite`, `DataEventoUtc` |
| Rejeicao | `RejeitarSolucaoChamadoUseCase` | `Chamado.RejeitarSolucao` | `TipoHistoricoChamado.SolucaoRejeitada` | `TipoAcaoAuditoria.RejeitarSolucaoChamado` | `ChamadoId`, executor, `StatusAnterior`, `StatusNovo`, `MotivoRejeicaoSolucao`, `SolucaoRejeitadaEm`, `SolucaoRejeitadaPorUsuarioId`, `ResolvidoEm`, `DataEventoUtc` |
| Fechamento automatico | `FecharChamadosAutomaticamentePorPrazoAceiteUseCase` | `Chamado.FecharAutomaticamentePorPrazoAceite` | `TipoHistoricoChamado.FechamentoAutomatico` | `TipoAcaoAuditoria.FecharChamadoAutomaticamentePorPrazoAceite` | `ChamadoId`, executor sistemico, `StatusAnterior`, `StatusNovo`, `ResolvidoEm`, `EncerradoEm`, `PrazoAceiteHoras`, `OrigemFechamento`, `DataEventoUtc` |
| Reabertura | `ReabrirChamadoUseCase` | `Chamado.ReabrirPorPolitica` | `TipoHistoricoChamado.Reaberto` | `TipoAcaoAuditoria.ReabrirChamado` | `ChamadoId`, executor, `StatusAnterior`, `StatusNovo`, `MotivoReabertura`, `PrazoMaximoReaberturaHoras`, `EncerradoEm`, `EncerradoEmOriginal`, `DataEventoUtc` |

## Compatibilidade preservada
- Nenhum novo fluxo funcional foi criado.
- Nenhum endpoint novo foi criado.
- Nao houve mudanca de SLA neste item.
- Nao houve alteracao do motor de aprovacoes da Sprint 4.
- Nao houve duplicacao intencional de historico ou auditoria por evento.

## Evidencias principais
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ResolverChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/AceitarSolucaoChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/RejeitarSolucaoChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/FecharChamadosAutomaticamentePorPrazoAceiteUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ReabrirChamadoUseCase.cs`
- `tests/SGX.SistemaChamado.Tests/AuditoriaCicloFechamentoChamadoTests.cs`
- `tests/SGX.SistemaChamado.Tests/AceitarSolucaoChamadoUseCaseTests.cs`

## Resultado da sprint
Item 15 concluido. A Sprint 5 passa a `15/32` itens concluidos e `47%` de implementacao arredondada.
