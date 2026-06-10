# Sprint 4 - Servico de Aplicacao para Instancia de Aprovacao

## 1. Objetivo do servico de aplicacao
Criar a primeira camada de orquestracao administrativa para instancias de aprovacao do chamado, permitindo consultar, validar, preparar e criar instancias manuais sem acoplar o motor ao fluxo operacional do SGX.

## 2. Limites desta etapa
- Nao gera aprovacao obrigatoria automaticamente.
- Nao integra abertura nem atendimento.
- Nao cria etapas automaticamente no fluxo normal.
- Nao cria decisoes automaticamente.
- Nao aprova, reprova, cancela ou expira funcionalmente.
- Nao bloqueia movimentacao.
- Nao altera SLA.
- Nao cria endpoint, controller, tela ou frontend.

## 3. Contexto estrutural e contratual existente
A Sprint 4 ja possui:
- `ConfiguracaoRegraAprovacao` como politica.
- `InstanciaAprovacaoChamado` como processo concreto.
- `EtapaAprovacaoChamado` como estrutura interna futura.
- `DecisaoAprovacaoChamado` como ato formal.
- contratos administrativos de configuracao e de decisao.
- servico administrativo de regras criado no item 36.

## 4. Entidade base usada pelo servico
`InstanciaAprovacaoChamado`.

## 5. Entidades relacionadas
- `ConfiguracaoRegraAprovacao`
- `EtapaAprovacaoChamado`
- `DecisaoAprovacaoChamado`
- `AprovacaoChamado`
- `Chamado`
- entidades auxiliares de usuario, categoria, catalogo e tipo de solicitacao

## 6. Contratos usados ou criados
Foram criados contratos administrativos especificos em `AdminInstanciaAprovacaoChamadoDtos.cs`:
- `ListarInstanciasAprovacaoChamadoRequest`
- `InstanciaAprovacaoChamadoResumoResponse`
- `InstanciaAprovacaoChamadoResponse`
- `InstanciaAprovacaoChamadoEtapaResumoResponse`
- `InstanciaAprovacaoChamadoDecisaoResumoResponse`
- `ValidarInstanciaAprovacaoChamadoRequest`
- `ValidarInstanciaAprovacaoChamadoResponse`
- `PrepararInstanciaAprovacaoChamadoRequest`
- `PrepararInstanciaAprovacaoChamadoResponse`
- `CriarInstanciaAprovacaoChamadoManualRequest`

## 7. Validators usados ou criados
Foi criado `InstanciaAprovacaoChamadoValidators.cs` com validacoes simples para listagem, preparacao, criacao manual e validacao conceitual.

## 8. Padrao de service/use case identificado no projeto
Foi mantido o mesmo padrao dos casos administrativos da aplicacao:
- interface em `Application/Interfaces/Admin`
- implementacao em `Application/UseCases/Admin`
- DTOs em `Application/DTOs/Admin`
- validators em `Application/Validators`
- uso de `IRepository<>`, `IUnitOfWork` e `IUsuarioContextoAplicacaoService`

## 9. Servico criado
`InstanciaAprovacaoChamadoAdminUseCases`.

## 10. Interface criada
`IAdminInstanciaAprovacaoChamadoUseCases`.

## 11. Operacoes administrativas implementadas
- `ListarAsync`
- `ObterPorIdAsync`
- `ListarPorChamadoAsync`
- `ListarPendentesAsync`
- `ListarPorStatusAsync`
- `ValidarAsync`
- `PrepararAsync`
- `CriarManualAsync`

## 12. Listagem de instancias
A listagem suporta filtros por chamado, configuracao, aprovacao legada, status, origem, fluxo, efeito operacional, escopo, tipo de regra, natureza, tipo de solicitacao, catalogo, categoria, subcategoria, impacto, urgencia, prioridade, flags de pendencia e bloqueio, solicitante, aprovador resolvido, periodo e termo textual.

## 13. Consulta de detalhe
O detalhe retorna:
- dados centrais da instancia
- contexto avaliado
- snapshots da regra
- aprovadores associados
- contagem e resumo de etapas
- contagem e resumo de decisoes
- vinculo com aprovacao legada, quando houver

## 14. Consulta por chamado
`ListarPorChamadoAsync` retorna todas as instancias do chamado, sem produzir qualquer efeito funcional.

## 15. Consulta de pendencias
`ListarPendentesAsync` retorna instancias em `Pendente` ou `EmReavaliacao`, com filtro opcional por usuario relacionado ao aprovador resolvido, especifico ou padrao.

## 16. Consulta por status
`ListarPorStatusAsync` expõe leitura administrativa por `StatusInstanciaAprovacaoChamado`.

## 17. Validacao de instancia
`ValidarAsync` consolida:
- validacao de shape do contrato
- validacao de coerencia simples
- existencia de referencias basicas
- alertas de vigencia da configuracao, quando informada

## 18. Preparacao de criacao conceitual
`PrepararAsync` monta uma visao previa da instancia sem persistir dados. A operacao calcula defaults, herda dados da configuracao informada quando aplicavel e devolve alertas sobre comportamento bloqueante sem executar bloqueio.

## 19. Criacao manual/administrativa
`CriarManualAsync` foi criada como operacao administrativa controlada:
- cria somente a `InstanciaAprovacaoChamado`
- nao cria etapas
- nao cria decisoes
- nao altera abertura, atendimento ou SLA
- nao integra o fluxo automatico do motor

## 20. Relacao com regra aplicada
O servico consulta e expõe `ConfiguracaoRegraAprovacaoId`, nome da configuracao e snapshots da regra quando existentes. A preparacao tambem consegue usar a configuracao como base conceitual.

## 21. Relacao com etapas
O servico expõe quantidade de etapas e resumo ordenado por nivel, ordem e ramo. Nao consolida workflow.

## 22. Relacao com decisoes
O servico expõe quantidade de decisoes e resumo por data. Nao executa decisao funcional.

## 23. Relacao com aprovacao legada
`AprovacaoChamadoLegadaId` permanece opcional e apenas consultivo. A criacao manual pode vincular a aprovacao legada sem alterar o estado da aprovacao existente.

## 24. Tratamento de status da instancia
O servico le e expõe os status da entidade. Nao implementa transicoes operacionais novas, exceto a criacao manual iniciar em `Pendente` pela propria entidade.

## 25. Tratamento de origem da instancia
O servico expõe `OrigemInstanciaAprovacaoChamado`, incluindo `Manual`, `CatalogoServico`, `RegraMotor`, `Reavaliacao`, `MigracaoLegada` e `Sistema`, sem produzir origem automatica no fluxo real.

## 26. Tratamento de escopo
O servico preserva contexto por natureza, tipo, catalogo, categoria, subcategoria, impacto, urgencia, prioridade, custo e risco, apenas para consulta, validacao e preparacao.

## 27. Tratamento de efeito operacional
`EfeitoOperacional`, `ExigeAprovacao` e `Bloqueante` sao lidos, validados e retornados, mas nao executados.

## 28. Tratamento de bloqueante versus informativa
As validacoes impedem combinacoes incoerentes, como instancia informativa bloqueante ou instancia bloqueante sem exigencia de aprovacao.

## 29. Tratamento de prazo/vencimento
O servico trabalha com `PrazoDecisaoHoras`, `DeveExpirarEm`, `ExpiradaEm` e `DecididaEm` apenas como dados. A preparacao consegue inferir `DeveExpirarEm` a partir do prazo quando cabivel.

## 30. Tratamento de snapshot da regra
O servico preserva `RegraNomeSnapshot`, `RegraVersaoSnapshot` e `RegraCriterioSnapshot`, inclusive na preparacao conceitual.

## 31. Compatibilidade com chamados legados
Nenhum chamado legado foi reprocessado. Nenhuma instancia foi criada em massa. O servico apenas consulta o que existir e permite criacao manual controlada.

## 32. Compatibilidade com `AprovacaoChamado`
`AprovacaoChamado` foi preservada integralmente. O novo servico apenas reconhece o vinculo opcional com a aprovacao simples legada.

## 33. Garantias de ausencia de efeitos colaterais operacionais
Esta etapa nao:
- gera aprovacao obrigatoria
- cria etapa automaticamente
- cria decisao automaticamente
- bloqueia atendimento
- altera status do chamado
- altera SLA

## 34. Relacao futura com geracao de aprovacao obrigatoria
A geracao obrigatoria real fica para o item 38. O servico atual apenas prepara e administra a camada concreta de instancias.

## 35. Relacao futura com bloqueio operacional
O bloqueio funcional fica para o item 39. Nesta etapa, `Bloqueante` e apenas metadado consultavel e validavel.

## 36. Relacao futura com decisao funcional
Aprovacao, rejeicao, cancelamento, expiracao e reavaliacao funcionais ficam para os itens operacionais posteriores.

## 37. Relacao futura com endpoints
Os contratos e use cases ficam prontos para exposicao futura, sem criar API neste item.

## 38. Relacao futura com frontend
Os DTOs e responses ja suportam uma futura tela administrativa e consultas operacionais, sem criar interface agora.

## 39. Testes criados
Foi criado `ServicoAplicacaoInstanciaAprovacaoTests.cs` cobrindo:
- listagem com filtros basicos
- detalhe com relacoes e contagens
- consulta por chamado
- validacao de combinacao invalida bloqueante sem aprovacao
- preparacao sem persistencia
- criacao manual sem etapas/decisoes e sem alteracao da aprovacao legada

## 40. Riscos de seguranca e governanca
- interpretar a criacao manual como substituta do motor futuro
- assumir que flags bloqueantes ja geram bloqueio operacional
- acoplamento excessivo da consulta ao shape estrutural atual
- leitura prematura de etapas e decisoes como workflow completo

## 41. Decisoes adiadas para proximos itens
- geracao automatica obrigatoria
- bloqueio operacional
- workflow sequencial, paralelo e multinivel
- consolidacao de etapas
- quorom
- delegacao
- grupo aprovador real
- endpoints
- frontend
- homologacao

## 42. Conclusao tecnica
O item 37 entrega a primeira camada administrativa de instancias de aprovacao do chamado, separada do motor operacional. A estrutura agora permite listar, detalhar, validar, preparar e criar instancias manuais sem alterar o comportamento atual do sistema.

## 43. Proxima etapa recomendada
Executar o item 38: criar regra para gerar aprovacao obrigatoria no chamado.
