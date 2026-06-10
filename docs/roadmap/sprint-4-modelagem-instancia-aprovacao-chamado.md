# Sprint 4 - Modelagem da instancia de aprovacao do chamado

## 1. Objetivo da modelagem

Modelar tecnicamente a instancia concreta de aprovacao vinculada a um chamado, separando a politica da regra do registro real de aprovacao aberto para um caso especifico.

## 2. Limites desta etapa

- Esta etapa modela apenas a instancia de aprovacao do chamado.
- Nao houve implementacao do motor de avaliacao.
- Nao houve geracao automatica real de aprovacao por regra nova.
- Nao houve implementacao de etapa, ramo ou decisao formal estruturada.
- Nao houve alteracao do fluxo atual de abertura, atendimento ou SLA.
- Nao houve alteracao funcional em `AprovacaoChamado`.
- Nao houve endpoint, controller, tela ou service frontend.

## 3. Contexto das definicoes anteriores

- O item 29 modelou `ConfiguracaoRegraAprovacao` como politica persistivel.
- A Sprint 4 ja definiu requisitos conceituais para bloqueio, sinalizacao, compatibilidade com legado, fluxo simples e compatibilidade futura com sequencial, paralela e multinivel.
- Faltava a estrutura que representa a aprovacao concreta aberta para um chamado especifico.

## 4. Diferenca entre configuracao de regra e instancia de aprovacao

- configuracao de regra:
  - define a politica;
  - diz quando aprovar;
  - nao pertence a um chamado especifico.
- instancia de aprovacao:
  - pertence a um chamado concreto;
  - registra o escopo avaliado;
  - preserva snapshot da regra aplicada;
  - indica status, bloqueio e efeito operacional da aprovacao aberta.

## 5. Diferenca entre instancia, etapa e decisao

- instancia:
  - e o processo de aprovacao aberto para um chamado.
- etapa:
  - sera a divisao por nivel, ramo ou parte do fluxo;
  - fica para o item 31.
- decisao:
  - sera o ato formal de aprovar, reprovar, cancelar ou expirar;
  - fica para o item 32.

## 6. Situacao atual de `AprovacaoChamado`

`AprovacaoChamado` continua sendo a base funcional legada de aprovacao simples. Hoje ela ja possui:

- vinculo com chamado;
- status simples;
- origem;
- solicitante e aprovador;
- bloqueio por `BloqueiaAvancoAtendimento`;
- datas de solicitacao, decisao e cancelamento;
- justificativas e motivo de cancelamento.

Ela nao foi removida nem quebrada nesta etapa.

## 7. Necessidade da instancia de aprovacao do chamado

A nova instancia e necessaria para:

- separar o legado simples do motor novo;
- guardar o escopo avaliado no momento da criacao;
- manter snapshot da regra aplicada;
- permitir tipos de fluxo futuros sem alterar `AprovacaoChamado`;
- preparar a relacao futura com etapa e decisao sem implementa-las agora.

## 8. Modelo conceitual da instancia

A instancia modelada representa a aprovacao concreta em andamento ou concluida para um chamado especifico.

Ela responde:

- para qual chamado a aprovacao foi aberta;
- qual regra originou a instancia, se houver;
- qual escopo foi avaliado;
- qual efeito operacional ela produziu;
- se ela e bloqueante ou informativa;
- qual tipo de fluxo ela pretende usar;
- qual foi a estrategia de resolucao de aprovador;
- qual snapshot da regra deve permanecer historico.

## 9. Entidade proposta, se criada

Foi criada a entidade [InstanciaAprovacaoChamado](C:\Pessoal\SGX\SGX%20Sistema%20de%20Chamados%20Completo\src\SGX.SistemaChamado.Domain\Entities\InstanciaAprovacaoChamado.cs).

O nome foi escolhido para nao colidir semanticamente com `AprovacaoChamado`.

## 10. Campos propostos

Campos modelados:

- `ChamadoId`
- `ConfiguracaoRegraAprovacaoId`
- `AprovacaoChamadoLegadaId`
- `Titulo`
- `Descricao`
- `Status`
- `Origem`
- `TipoFluxoAprovacao`
- `EfeitoOperacional`
- `EscopoRegra`
- `TipoRegra`
- `NaturezaChamado`
- `TipoSolicitacaoId`
- `CatalogoServicoId`
- `CategoriaId`
- `SubcategoriaId`
- `ImpactoAvaliado`
- `UrgenciaAvaliada`
- `PrioridadeAvaliada`
- `CustoAvaliado`
- `NivelRiscoAvaliado`
- `ExigeAprovacao`
- `Bloqueante`
- `PermiteReenvio`
- `PermiteFallback`
- `TipoResolucaoAprovador`
- `AprovadorEspecificoUsuarioId`
- `AprovadorPadraoUsuarioId`
- `AprovadorResolvidoUsuarioId`
- `SolicitanteId`
- `SolicitadaEm`
- `PrazoDecisaoHoras`
- `DeveExpirarEm`
- `ExpiradaEm`
- `CanceladaEm`
- `CanceladaPorUsuarioId`
- `MotivoCancelamento`
- `DecididaEm`
- `RegraNomeSnapshot`
- `RegraVersaoSnapshot`
- `RegraCriterioSnapshot`
- auditoria base e controle de criacao/atualizacao

## 11. Vínculo com `Chamado`

- `ChamadoId` e obrigatorio.
- A instancia pertence sempre a um chamado concreto.
- O `Chamado` passou a expor a colecao `InstanciasAprovacao`.

## 12. Vínculo com `ConfiguracaoRegraAprovacao`

- `ConfiguracaoRegraAprovacaoId` foi modelado como opcional.
- Isso permite instancias originadas por regra nova e tambem instancias abertas por fluxo manual ou legado.
- Quando existe configuracao vinculada, a instancia exige snapshot minimo da regra.

## 13. Status da instância

Foi criado o enum `StatusInstanciaAprovacaoChamado` com:

- `Pendente`
- `Aprovada`
- `Reprovada`
- `Cancelada`
- `Expirada`
- `EmReavaliacao`
- `Substituida`

## 14. Origem da instância

Foi criado o enum `OrigemInstanciaAprovacaoChamado` com:

- `Manual`
- `CatalogoServico`
- `RegraMotor`
- `Reavaliacao`
- `MigracaoLegada`
- `Sistema`

## 15. Escopo da instância

O escopo foi representado de forma minima por:

- `EscopoRegra`
- `TipoRegra`
- `NaturezaChamado`
- `TipoSolicitacaoId`
- `CatalogoServicoId`
- `CategoriaId`
- `SubcategoriaId`
- `ImpactoAvaliado`
- `UrgenciaAvaliada`
- `PrioridadeAvaliada`
- `CustoAvaliado`
- `NivelRiscoAvaliado`

Isso permite que a aprovacao concreta preserve o contexto avaliado no momento da solicitacao.

## 16. Efeito operacional da instância

A instancia reutiliza `EfeitoOperacionalRegraAprovacao` para indicar se ela:

- apenas sinaliza;
- exige aprovacao;
- exige aprovacao e bloqueia;
- representa reavaliacao;
- nao interfere diretamente no fluxo.

## 17. Bloqueante versus informativa

- `ExigeAprovacao` e `Bloqueante` foram mantidos na instancia.
- A entidade valida coerencia minima com o efeito operacional.
- Isso preserva compatibilidade conceitual com o bloqueio simples legado sem implementar novos bloqueios agora.

## 18. Tipo de fluxo da instância

A instancia reutiliza `TipoFluxoAprovacao`:

- `Simples`
- `Sequencial`
- `Paralela`
- `Multinivel`

Nesta etapa, isso registra a intencao do fluxo, mas nao cria etapas nem ramos.

## 19. Rastreabilidade da regra aplicada

Mesmo com FK opcional para `ConfiguracaoRegraAprovacao`, a instancia precisa sobreviver a mudancas futuras na regra. Por isso, ela guarda snapshot proprio e nao depende apenas da referencia.

## 20. Snapshot mínimo da regra aplicada

Snapshot minimo modelado:

- `RegraNomeSnapshot`
- `RegraVersaoSnapshot`
- `RegraCriterioSnapshot`
- `TipoRegra`
- `EscopoRegra`
- `EfeitoOperacional`
- `TipoFluxoAprovacao`
- `TipoResolucaoAprovador`

## 21. Dados de solicitante e aprovador resolvido

Foram modelados:

- `SolicitanteId`
- `AprovadorEspecificoUsuarioId`
- `AprovadorPadraoUsuarioId`
- `AprovadorResolvidoUsuarioId`

Isso nao implementa a resolucao real de aprovador, mas deixa o historico minimo pronto.

## 22. Dados de prazo e vencimento

Foram modelados:

- `PrazoDecisaoHoras`
- `DeveExpirarEm`
- `ExpiradaEm`

Esses campos sao metadados de governanca. Eles nao alteram SLA operacional nesta etapa.

## 23. Dados futuros de cancelamento e expiração

Foram modelados:

- `CanceladaEm`
- `CanceladaPorUsuarioId`
- `MotivoCancelamento`
- `ExpiradaEm`
- `DecididaEm`

Isso oferece resumo temporal da instancia antes da decisao formal separada do item 32.

## 24. Relação futura com etapa de aprovação

- Nenhuma etapa foi criada agora.
- A instancia foi desenhada para ser o agregado pai futuro das etapas.
- O item 31 devera detalhar nivel, ramo e ordem operacional.

## 25. Relação futura com decisão de aprovação

- Nenhuma entidade de decisao foi criada nesta etapa.
- A instancia ficou com `Status` e `DecididaEm` como resumo inicial.
- O item 32 devera estruturar a decisao formal detalhada.

## 26. Relação com `AprovacaoChamado`

- `AprovacaoChamado` foi preservada.
- A nova entidade pode apontar opcionalmente para `AprovacaoChamadoLegadaId`.
- Isso ajuda a compatibilidade e a transicao sem migrar dados antigos.

## 27. Relação com `BloqueiaAvancoAtendimento`

- A instancia nao altera `BloqueiaAvancoAtendimento`.
- Ela apenas registra `Bloqueante` e `EfeitoOperacional`.
- O item futuro de execucao podera decidir como refletir isso no legado.

## 28. Relação com `AguardandoAprovacao`

- A modelagem nao altera o status do chamado.
- A instancia apenas prepara o contexto para que, no futuro, o motor decida se a espera operacional deve ou nao ser refletida em `AguardandoAprovacao`.

## 29. Relação com SLA

- A instancia nao altera SLA operacional.
- `PrazoDecisaoHoras` e `DeveExpirarEm` ficam como metadados de aprovacao.
- Qualquer pausa, SLA proprio ou escalonamento fica para itens futuros.

## 30. Compatibilidade com chamados legados

- Nenhuma instancia foi criada para chamados antigos.
- Nenhum legado foi reprocessado.
- Nenhuma aprovacao antiga foi migrada.
- A estrutura apenas prepara compatibilidade futura.

## 31. Constraints, índices e integridade

Foram definidos:

- FK obrigatoria para `Chamado`;
- FK opcional para `ConfiguracaoRegraAprovacao`;
- FK opcional para `AprovacaoChamado`;
- FK opcionais para `TipoSolicitacao`, `CatalogoServico`, `CategoriaChamado`, `SubcategoriaChamado`;
- FK para usuarios de solicitacao, resolucao, cancelamento e auditoria;
- indice unico para `AprovacaoChamadoLegadaId`;
- indices por `ChamadoId`, `Status`, `Origem`, `Ativo`, `SolicitadaEm` e `DeveExpirarEm`;
- check constraint para subcategoria exigir categoria;
- check constraint para custo nao negativo;
- check constraint para nivel de risco positivo;
- check constraint para prazo positivo;
- check constraint para `RegraVersaoSnapshot > 0`;
- check constraint para expiracao planejada nao ser anterior a solicitacao.

## 32. Riscos de segurança e governança

- duplicar semanticamente o legado se a relacao com `AprovacaoChamado` nao for bem usada;
- tratar snapshot de regra como opcional quando a instancia nasce de configuracao;
- usar a nova instancia para bloquear use cases antes do motor existir;
- confundir `DecididaEm` resumida com decisao formal detalhada;
- misturar fluxo simples legado com instancia nova sem criterio de transicao;
- gerar vinculos multiplos indevidos para a mesma aprovacao legada.

## 33. Decisões adiadas para próximos itens

- modelagem das etapas;
- modelagem da decisao formal;
- resolucao real de aprovador;
- workflow sequencial, paralelo e multinivel funcional;
- auditoria completa de solicitacao, etapa e decisao;
- uso operacional da instancia nos use cases;
- migracao ou conciliacao de legados;
- API e interface administrativa da instancia.

## 34. Conclusão técnica

`InstanciaAprovacaoChamado` fecha a segunda camada estrutural do motor: a aprovacao concreta vinculada ao chamado. Ela complementa `ConfiguracaoRegraAprovacao`, preserva `AprovacaoChamado` legado, guarda escopo avaliado e snapshot da regra, e prepara o terreno para etapa e decisao sem mudar o comportamento funcional atual.

## 35. Próxima etapa recomendada

Executar o item 31 da Sprint 4: modelar etapa de aprovacao.
