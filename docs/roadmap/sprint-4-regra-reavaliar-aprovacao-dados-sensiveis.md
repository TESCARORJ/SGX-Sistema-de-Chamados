# Sprint 4 - Regra para reavaliar aprovacao por dados sensiveis

## 1. Objetivo da regra
Registrar reavaliacao controlada quando dados sensiveis do chamado mudam e o escopo aprovado deixa de ser suficiente ou confiavel.

## 2. Limites desta etapa
Nao aprova, nao reprova, nao cancela, nao expira, nao cria workflow completo e nao altera status do chamado ou SLA.

## 3. Contexto dos itens anteriores
O item 38 gera instancia pendente, o item 39 bloqueia pendencias abertas, o item 40 aprova e o item 41 reprova. O item 42 apenas reavalia o que ja existe quando o contexto sensivel muda.

## 4. Diferenca entre aprovar, rejeitar, cancelar, expirar e reavaliar
- Aprovar: consolida resultado positivo.
- Rejeitar: consolida resultado negativo.
- Cancelar: encerra por interrupcao do fluxo.
- Expirar: encerra por prazo.
- Reavaliar: invalida a suficiencia da aprovacao anterior sem aprovar ou reprovar automaticamente.

## 5. Comportamento legado atual
`AprovacaoChamado` legado permanece intocado. A nova regra atua apenas em `InstanciaAprovacaoChamado`, `EtapaAprovacaoChamado` e `DecisaoAprovacaoChamado`.

## 6. Regra/use case criado
`ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase`

## 7. Interface criada
`IReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase`

## 8. Contratos internos criados ou reutilizados
- Criado `ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest`
- Criado `ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse`
- Reutilizado `ContextoAvaliacaoRegraAprovacaoRequest` via `IAdminConfiguracaoRegraAprovacaoUseCases`

## 9. Fluxo de reavaliacao
1. Valida request e usuario responsavel.
2. Detecta mudancas sensiveis entre contexto anterior e novo.
3. Avalia regra aplicavel antes e depois da mudanca.
4. Localiza a instancia alvo.
5. Decide se a aprovacao ainda cobre o novo contexto.
6. Quando nao cobre, marca instancia em `EmReavaliacao`, marca etapas seguras em `EmReavaliacao` e cria `DecisaoAprovacaoChamado` do tipo `Reavaliacao`.
7. Retorna resposta consultiva ou executada, sem mexer no chamado.

## 10. Dados sensiveis considerados
`NaturezaChamado`, `TipoSolicitacaoId`, `CatalogoServicoId`, `CategoriaId`, `SubcategoriaId`, `ImpactoChamado`, `UrgenciaChamado`, `PrioridadeChamado`, `Custo`, `NivelRisco` e snapshot de escopo sensivel.

## 11. Comparacao entre contexto anterior e novo contexto
A comparacao e feita campo a campo e gera a lista `MudancasSensiveisDetectadas`. O use case tambem consulta a melhor regra antes e depois da mudanca.

## 12. Criterios para manter aprovacao valida
- nao houve mudanca sensivel;
- o novo contexto continua coberto pela mesma regra aplicada;
- identificadores sensiveis continuam compativeis com a instancia;
- impacto, urgencia, prioridade, custo e risco nao extrapolam o escopo aprovado;
- o novo escopo textual nao contradiz o escopo anterior.

## 13. Criterios para exigir reavaliacao
- troca de regra aplicavel;
- troca de catalogo/servico sensivel;
- aumento de impacto, urgencia, prioridade, custo ou risco acima do aprovado;
- mudanca de natureza, tipo, categoria, subcategoria ou escopo que sai da cobertura anterior.

## 14. Criterios para exigir nova aprovacao futura
Quando a melhor regra nova continua exigindo aprovacao, mas a instancia atual nao cobre mais o novo escopo.

## 15. Criterios para nao reavaliar
- nenhuma mudanca sensivel;
- ausencia de instancia relacionada;
- instancia cancelada, expirada ou substituida;
- instancia ja reprovada, sem reabertura automatica;
- reducao de sensibilidade ainda coberta pela aprovacao existente.

## 16. Relacao com `ConfiguracaoRegraAprovacao`
Usa a configuracao apenas para reavaliar cobertura do contexto antigo e do contexto novo. Nao altera a entidade.

## 17. Relacao com servico de regras
Reutiliza `IAdminConfiguracaoRegraAprovacaoUseCases.AvaliarRegraAsync` para manter o mesmo criterio de selecao de regras do motor.

## 18. Relacao com `InstanciaAprovacaoChamado`
Usa `MarcarEmReavaliacao` quando a aprovacao deixa de ser suficiente. Nao substitui nem cancela a instancia.

## 19. Relacao com `EtapaAprovacaoChamado`
Marca etapas `Pendente` ou `Aprovada` como `EmReavaliacao` quando isso e seguro para manter rastreabilidade sem recalcular workflow.

## 20. Relacao com `DecisaoAprovacaoChamado`
Cria decisao com `TipoDecisao = Reavaliacao` e resultado `RequerAjuste` ou `RequerNovaAprovacao`, preservando snapshots e historico.

## 21. Relacao com `AprovacaoChamado` legado
Nao ha migracao, limpeza ou sincronizacao destrutiva do legado.

## 22. Relacao com `GerarAprovacaoObrigatoriaChamadoUseCase`
Pode sinalizar `ExigeNovaAprovacao = true`, mas nao cria nova instancia automaticamente para evitar duplicidade.

## 23. Relacao com `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`
Ao colocar a instancia em `EmReavaliacao`, o bloqueio do item 39 continua funcionando para pendencias bloqueantes.

## 24. Relacao com `AprovarAprovacaoChamadoUseCase`
Mantem a separacao: aprovar continua positivo; reavaliar apenas invalida cobertura e registra nova analise pendente.

## 25. Relacao com `ReprovarAprovacaoChamadoUseCase`
Mantem a separacao: reprovar continua negativo; reavaliar nao reprova automaticamente.

## 26. Relacao com `BloqueiaAvancoAtendimento`
Nao altera comportamento legado diretamente. O efeito operacional decorre da instancia bloqueante em `EmReavaliacao`.

## 27. Relacao com `AguardandoAprovacao`
Nao altera o status do chamado para `AguardandoAprovacao`.

## 28. Relacao com status do chamado
Nenhuma mudanca automatica de status.

## 29. Relacao com abertura
Nenhuma alteracao ampla no fluxo de abertura.

## 30. Relacao com atendimento
Nao bloqueia comentario, anexo, evidencia, consulta ou triagem.

## 31. Relacao com SLA
Nao pausa, nao recalcula e nao encerra SLA.

## 32. Tratamento de instancia pendente
Pode ser movida para `EmReavaliacao` quando o escopo sensivel muda de forma relevante.

## 33. Tratamento de instancia aprovada
Mantem aprovada se o novo contexto continua coberto. Vai para `EmReavaliacao` quando deixa de cobrir.

## 34. Tratamento de instancia reprovada
Nao reabre automaticamente. Apenas sinaliza necessidade futura de nova aprovacao quando aplicavel.

## 35. Tratamento de instancia cancelada, expirada ou substituida
Retorno consultivo, sem reativacao automatica.

## 36. Tratamento de etapa pendente
Pode ir para `EmReavaliacao` quando participa do escopo afetado.

## 37. Tratamento de etapa aprovada
Pode ir para `EmReavaliacao` para explicitar que a aprovacao anterior nao cobre mais o novo contexto.

## 38. Tratamento de decisao anterior
Nenhuma decisao anterior e apagada ou sobrescrita.

## 39. Snapshot do contexto anterior
Vai para observacao resumida da decisao e tambem pode vir explicitamente no request.

## 40. Snapshot do novo contexto
Vai para `EscopoDecididoSnapshot` da decisao e para a resposta funcional.

## 41. Garantias de ausencia de aprovacao/rejeicao/cancelamento/expiracao automatica
O use case nao chama `AprovarAprovacaoChamadoUseCase`, nao chama `ReprovarAprovacaoChamadoUseCase`, nao cancela instancia e nao expira instancia.

## 42. Testes criados
`ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCaseTests`

## 43. Riscos de seguranca e governanca
- ainda nao existe integracao automatica com todos os pontos de alteracao sensivel do chamado;
- sem workflow completo, a reavaliacao e controlada mas nao recalcula sequencia/paralelismo/quorum;
- ausencia de grupo aprovador real e delegacao continua como pendencia futura.

## 44. Decisoes adiadas para proximos itens
Nova geracao automatica de instancia, cancelamento, expiracao, workflow sequencial/paralelo/multinivel completo, quoruns, API e frontend administrativos.

## 45. Conclusao tecnica
O item 42 introduz reavaliacao auditavel e controlada no novo motor de aprovacoes sem quebrar legado nem alterar fluxos externos.

## 46. Proxima etapa recomendada
Executar o item 43: criar endpoints administrativos de regras de aprovacao.
