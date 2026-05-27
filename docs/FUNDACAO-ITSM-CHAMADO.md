# Fundacao ITSM do Chamado

## Objetivo funcional
Consolidar a base ITSM do chamado para que o sistema deixe de operar como ticket generico e passe a aplicar comportamento por processo (incidente, requisicao, mudanca, problema, evento/alerta e tarefa operacional), mantendo o backend como autoridade final.

## Visao geral do escopo implementado
- Natureza do chamado obrigatoria.
- Impacto e urgencia obrigatorios conforme regras por natureza.
- Prioridade calculada por matriz impacto x urgencia.
- Fluxo de status permitido por natureza com regra centralizada.
- Campos obrigatorios por natureza com regra centralizada.
- Acoes disponiveis por natureza/status/permissao com regra centralizada.
- Status ITSM especificos adicionados por natureza.
- Portal, admin, dashboard e relatorios adaptados para consumir regras backend.

## Decisoes arquiteturais
- Dominio orientado a enums de negocio: `NaturezaChamadoEnum`, `ImpactoChamadoEnum`, `UrgenciaChamadoEnum`, `PrioridadeChamadoEnum`, `StatusChamadoEnum`, `AcaoChamadoEnum`.
- Regras centralizadas na camada de aplicacao:
  - `IPrioridadeChamadoMatrizService`
  - `IFluxoStatusChamadoService`
  - `ICamposObrigatoriosChamadoService`
  - `IAcoesChamadoService`
- Registro no DI confirmado em `src/SGX.SistemaChamado.Infrastructure/DependencyInjection.cs`.
- Frontend sem duplicar matriz de status/acoes: consome contexto e `acoesDisponiveisCodigos` do backend.

## Relacao entre NaturezaChamado e TipoSolicitacao
- `NaturezaChamado` representa o processo ITSM e direciona regra de negocio.
- `TipoSolicitacao` permanece como classificacao operacional/catalogo.
- Nesta fundacao, `NaturezaChamado` nao substitui `TipoSolicitacao`.

## Matriz impacto x urgencia x prioridade
Regra central em `PrioridadeChamadoMatrizService`:

| Impacto | Urgencia | Prioridade |
|---|---|---|
| Alto | Alta | Critica |
| Alto | Media | Alta |
| Alto | Baixa | Media |
| Medio | Alta | Alta |
| Medio | Media | Media |
| Medio | Baixa | Baixa |
| Baixo | Alta | Media |
| Baixo | Media | Baixa |
| Baixo | Baixa | Baixa |

## Status por natureza
Base central em `FluxoStatusChamadoService`.

- Incidente:
  - Aberto, EmAtendimento, AguardandoSolicitante, Resolvido, Encerrado, Cancelado
- Requisicao:
  - Aberto, EmAtendimento, AguardandoSolicitante, Resolvido, Encerrado, Cancelado
- Mudanca:
  - Aberto, EmAnalise, AguardandoAprovacao, Aprovada, Reprovada, EmExecucao, Concluida, Encerrado, Cancelado
- Problema:
  - Aberto, EmAnalise, CausaRaizIdentificada, SolucaoDeContorno, Resolvido, Encerrado, Cancelado
- EventoAlerta:
  - Aberto, EmAnalise, Correlacionado, Tratado, Encerrado, Cancelado
- TarefaOperacional:
  - Aberto, Planejada, EmExecucao, Concluida, Encerrado, Cancelado

## Campos obrigatorios por natureza
Base central em `CamposObrigatoriosChamadoService`.

- Obrigatorios sempre:
  - `NaturezaChamado`, `Titulo`, `Descricao`
- Impacto/Urgencia obrigatorios (origem nao-email):
  - Incidente, Mudanca, Problema, EventoAlerta, TarefaOperacional
- Requisicao:
  - aceita ausencia de impacto/urgencia explicitos (com fallback seguro por regra vigente)
- Categoria/Tipo/Catalogo:
  - ao menos uma referencia obrigatoria (`CategoriaId` ou `TipoSolicitacaoId` ou catalogo)
- Detalhamento minimo:
  - Mudanca e Problema exigem descricao com minimo de detalhamento.

## Acoes disponiveis por natureza/status
Base central em `AcoesChamadoService`.

- Acoes operacionais existentes controladas centralmente:
  - Assumir, Atribuir, AlterarStatus, AlterarPrioridade, AlterarCategoria, Encerrar, Reabrir, Comentar, Anexar, Cancelar
- Acoes futuras registradas apenas como identificador:
  - AprovarMudanca, ReprovarMudanca, ExecutarMudanca, RegistrarCausaRaiz, RegistrarSolucaoContorno, CorrelacionarEvento, TratarEvento, ConcluirTarefa
- Endpoints sensiveis usam validacao central de acao de forma progressiva, mantendo compatibilidade.

## Impacto no portal
- Portal de abertura com natureza, impacto e urgencia.
- Validacao final permanece no backend.
- Sem duplicacao de regra de matriz no frontend.

## Impacto no admin
- Listagem e detalhe exibem natureza, impacto, urgencia e prioridade.
- Modal de alterar status respeita status permitidos por natureza vindos do backend.
- Acoes de atendimento respeitam `acoesDisponiveisCodigos` do backend.

## Impacto em dashboard e relatorios
- Filtros por natureza em dashboard e relatorios administrativos.
- Consolidacoes por natureza implementadas no backend.

## Abertura por e-mail e fallback
- E-mail continua com fallback seguro para natureza/impacto/urgencia conforme regra vigente.
- Sem alteracao de autenticacao, AD ou fluxo de notificacao nesta consolidacao.

## Dados legados
- Chamados antigos preservados.
- Fallback de natureza aplicado de forma conservadora na fundacao.
- Sem reclassificacao agressiva automatica de historico.

## Migrations e consistencia tecnica
- Migrations de fundacao ITSM identificadas e ordenadas por timestamp:
  - `20260526225210_Sprint1NaturezaChamadoFundacao`
  - `20260526232629_Sprint12ImpactoUrgenciaChamado`
  - `20260527031715_Sprint13StatusItsmEspecificos`
- Migration manual `Sprint13StatusItsmEspecificos` revisada:
  - adiciona seeds de status especificos sem remover status antigos;
  - ajusta `EhStatusFinal` de `Resolvido` para `true`.
- Snapshot EF (`SGXSistemaChamadoDbContextModelSnapshot`) contem os status e flags coerentes com enum/seed atuais.

## Rastreabilidade tecnica (principais pontos)
- Enums: `src/SGX.SistemaChamado.Domain/Enums`.
- Servicos centrais: `src/SGX.SistemaChamado.Application/Services`.
- Seed de status: `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs`.
- Migration Sprint 13: `src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260527031715_Sprint13StatusItsmEspecificos.cs`.
- DTOs/UseCases admin/portal/dashboard/relatorios: `src/SGX.SistemaChamado.Application/DTOs` e `UseCases`.
- Frontend admin (status e acoes): `src/SGX.SistemaChamado.Web/src`.

## Checklist de aceite da Sprint 1 (Fundacao ITSM)
- [x] Todo chamado possui `NaturezaChamado`.
- [x] `NaturezaChamado` e obrigatoria.
- [x] Existem Incidente, Requisicao, Mudanca, Problema, Evento/Alerta e Tarefa Operacional.
- [x] `ImpactoChamado` foi implementado.
- [x] `UrgenciaChamado` foi implementada.
- [x] Prioridade e calculada por impacto x urgencia.
- [x] Status permitidos variam por natureza.
- [x] Campos obrigatorios variam por natureza.
- [x] Portal permite selecionar natureza, impacto e urgencia.
- [x] E-mail define natureza/impacto/urgencia com fallback seguro.
- [x] Admin exibe natureza, impacto, urgencia e prioridade.
- [x] Admin filtra status por natureza.
- [x] Dashboard filtra e consolida por natureza.
- [x] Relatorios filtram e consolidam por natureza.
- [x] Acoes disponiveis sao centralizadas por natureza/status/permissao.
- [x] Status ITSM especificos foram adicionados.
- [x] Chamados antigos receberam fallback seguro.
- [x] Backend e autoridade final das regras.
- [x] Frontend nao duplica matriz de status/acoes.
- [x] Testes estao verdes.
- [x] Documentacao foi consolidada.
- [x] Roadmap foi atualizado.

## Testes e validacao
- Cobertura dedicada confirmada para:
  - natureza obrigatoria;
  - impacto/urgencia;
  - matriz de prioridade;
  - fluxo de status por natureza;
  - campos obrigatorios por natureza;
  - portal/admin/dashboard/relatorios;
  - acoes disponiveis;
  - status ITSM especificos.

## Limitacoes conhecidas
- Sem workflow completo de mudanca (CAB/aprovacao operacional detalhada).
- Sem causa raiz estruturada de problema.
- Sem correlacao real de eventos/alertas com ferramenta externa.
- Sem automacoes de fechamento especificas por natureza.

## Proximas evolucoes recomendadas
- Observacao de planejamento:
  - A Fundacao ITSM do chamado permanece 100% implementada e validada.
  - Evolucoes de orquestracao entre chamados, dependencias, aprovacoes, planejamento e registros derivados passam para a Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM.
- Aprovacao real de mudanca e fluxo CAB.
- Workflow completo de mudanca ponta a ponta.
- Gerenciamento de problema com causa raiz estruturada.
- Solucao de contorno estruturada com governanca.
- Correlacao real de eventos e monitoramento.
- Conclusao especifica de tarefa operacional.
