# Sprint 4 - Regra para gerar aprovacao obrigatoria no chamado

## 1. Objetivo da regra

Criar a camada de aplicacao que transforma uma configuracao de regra aplicavel em uma `InstanciaAprovacaoChamado` concreta, pendente e rastreavel, sem ainda bloquear movimentacoes do chamado.

## 2. Limites desta etapa

- gera instancia real quando a regra exige aprovacao;
- nao bloqueia abertura, atendimento ou mudanca de status;
- nao cria etapa automaticamente;
- nao cria decisao automaticamente;
- nao aprova, reprova, cancela ou expira;
- nao altera SLA;
- nao cria endpoint, controller, tela ou frontend.

## 3. Contexto estrutural e servicos existentes

O item 36 deixou pronta a administracao e avaliacao conceitual de `ConfiguracaoRegraAprovacao`. O item 37 deixou pronta a administracao de `InstanciaAprovacaoChamado`, inclusive contratos e validacoes de preparacao/criacao manual.

## 4. Diferenca entre avaliar regra e gerar aprovacao

- avaliar regra: seleciona a melhor politica aplicavel para um contexto;
- gerar aprovacao: persiste uma instancia concreta baseada nessa politica.

## 5. Diferenca entre gerar aprovacao e bloquear movimentacao

Gerar aprovacao apenas materializa a pendencia. O bloqueio operacional do chamado permanece explicitamente adiado para o item 39.

## 6. Regra criada

Foi criado o use case `GerarAprovacaoObrigatoriaChamadoUseCase`.

## 7. Servico/use case criado

- interface: `IGerarAprovacaoObrigatoriaChamadoUseCase`
- implementacao: `GerarAprovacaoObrigatoriaChamadoUseCase`

## 8. Contratos internos criados

- `GerarAprovacaoObrigatoriaChamadoRequest`
- `GerarAprovacaoObrigatoriaChamadoResponse`

## 9. Fluxo de geracao obrigatoria

1. Recebe contexto do chamado.
2. Carrega o chamado e o status atual.
3. Monta o contexto de avaliacao da regra.
4. Avalia a melhor regra aplicavel.
5. Interrompe quando nao houver regra aplicavel ou quando a regra for apenas informativa.
6. Verifica duplicidade por instancia nova equivalente.
7. Verifica duplicidade por aprovacao legada pendente de catalogo.
8. Cria `InstanciaAprovacaoChamado` com origem `RegraMotor` e status `Pendente`.
9. Retorna o resultado da geracao sem bloquear fluxo operacional.

## 10. Contexto de entrada da geracao

O request interno suporta `ChamadoId`, natureza, tipo de solicitacao, catalogo/servico, categoria, subcategoria, impacto, urgencia, prioridade, custo, risco, solicitante, data de referencia, flag de reavaliacao forcada e origem textual da solicitacao.

## 11. Criterios para gerar aprovacao

- chamado valido;
- chamado nao finalizado, salvo chamada controlada com `ForcarReavaliacao`;
- regra ativa e vigente;
- regra compativel com o contexto;
- regra com `ExigeAprovacao = true`;
- ausencia de instancia equivalente pendente/em reavaliacao;
- ausencia de aprovacao legada pendente equivalente por catalogo.

## 12. Criterios para nao gerar aprovacao

- nenhuma regra aplicavel;
- regra apenas informativa;
- chamado em status final sem reavaliacao forcada;
- duplicidade estrutural na instancia nova;
- duplicidade com `AprovacaoChamado` legada pendente por catalogo.

## 13. Deteccao de aprovacao duplicada

Foi adotada verificacao por:

- mesmo `ChamadoId`;
- mesma `ConfiguracaoRegraAprovacaoId`;
- status `Pendente` ou `EmReavaliacao`;
- mesmo escopo relevante avaliado: natureza, tipo, catalogo, categoria, subcategoria, impacto, urgencia, prioridade, custo e risco.

Para o legado por catalogo, tambem se evita geracao quando ja existe `AprovacaoChamado` pendente com `TipoOrigem = CatalogoServico`.

## 14. Relacao com `ConfiguracaoRegraAprovacao`

A configuracao continua sendo a politica. A regra nova apenas consome a configuracao selecionada para gerar a instancia concreta.

## 15. Relacao com `ConfiguracaoRegraAprovacaoAdminUseCases`

O item 36 foi reutilizado conceitualmente: mesma semantica de criterios, vigencia, prioridade, ordem e especificidade. A avaliacao ficou embutida no novo use case para evitar dependencia operacional de um servico administrativo com guarda de perfil.

## 16. Relacao com `InstanciaAprovacaoChamado`

A instância e o artefato persistido pela regra. O use case gera somente a instancia base, sem etapas e sem decisoes.

## 17. Relacao com `InstanciaAprovacaoChamadoAdminUseCases`

O item 37 foi reutilizado como referencia de contratos, validacoes e shape de criacao manual. A nova regra usa a mesma modelagem de criacao, mas com execucao propria porque o servico administrativo foi mantido como camada de administracao, nao como motor automatico.

## 18. Relacao com `AprovacaoChamado` legado

`AprovacaoChamado` foi preservada sem alteracao estrutural. A nova regra so consulta o legado para evitar duplicidade com a aprovacao automatica existente por catalogo.

## 19. Relacao com catalogo de servico

O comportamento legado de `CatalogoServico.RequerAprovacao` permanece intacto. A regra nova nao altera `CatalogoServico` e nao substitui a aprovacao automatica legadoa; apenas evita sobreposicao indevida.

## 20. Relacao com abertura de chamado

Nao houve integracao ampla com `AbrirChamadoUseCase`. O novo use case ficou isolado e pronto para ser orquestrado futuramente de forma controlada.

## 21. Relacao com atendimento

Nao houve integracao com fluxos de atendimento. A instância gerada pode ser consultada, mas nao impede movimentacoes nesta etapa.

## 22. Relacao com `BloqueiaAvancoAtendimento`

O valor de `Bloqueante` pode ser copiado para a instancia, mas nenhum bloqueio operacional foi executado. O legado `BloqueiaAvancoAtendimento` nao foi alterado.

## 23. Relacao com `AguardandoAprovacao`

O status do chamado nao e alterado automaticamente para `AguardandoAprovacao`.

## 24. Relacao com SLA

Nao houve pausa, retomada ou recalculo de SLA. `PrazoDecisaoHoras` e `DeveExpirarEm` sao tratados apenas como metadados da instancia de aprovacao.

## 25. Snapshot da regra aplicada

Foram preservados:

- `RegraNomeSnapshot`
- `RegraVersaoSnapshot`
- `RegraCriterioSnapshot`

## 26. Escopo avaliado

A instancia copia o contexto relevante do chamado:

- natureza;
- tipo de solicitacao;
- catalogo/servico;
- categoria;
- subcategoria;
- impacto;
- urgencia;
- prioridade;
- custo;
- risco.

## 27. Status inicial da instancia

`Pendente`.

## 28. Origem da instancia

`OrigemInstanciaAprovacaoChamado.RegraMotor`.

## 29. Bloqueante versus informativa

A instancia copia `ExigeAprovacao` e `Bloqueante` da regra. Mesmo quando bloqueante, o bloqueio operacional ainda nao e aplicado.

## 30. Efeito operacional

O valor de `EfeitoOperacionalRegraAprovacao` e copiado para a instancia como metadado do motor.

## 31. Tipo de fluxo

`TipoFluxoAprovacao` e preservado na instancia, mas sem gerar workflow funcional de etapas.

## 32. Estrategia de aprovador

`TipoResolucaoAprovador`, `AprovadorEspecificoUsuarioId`, `AprovadorPadraoUsuarioId` e, quando dedutivel, `AprovadorResolvidoUsuarioId` sao copiados para a instancia.

## 33. Prazo e vencimento

Quando a regra possui `PrazoDecisaoHoras`, a instância recebe `DeveExpirarEm` calculado a partir do instante real da geracao.

## 34. Compatibilidade com chamados legados

Nao houve reprocessamento em massa, migracao de dados ou geracao retroativa para chamados antigos.

## 35. Garantias de ausencia de bloqueio operacional nesta etapa

- nao altera status do chamado;
- nao altera `BloqueiaAvancoAtendimento` legado;
- nao altera `AguardandoAprovacao`;
- nao altera SLA;
- nao cria workflow funcional.

## 36. Testes criados

Foi criada a suite `GerarAprovacaoObrigatoriaChamadoUseCaseTests` cobrindo:

- ausencia de regra aplicavel;
- regra informativa;
- geracao de instancia pendente;
- snapshot e escopo copiados;
- prazo/vencimento;
- deduplicacao por instancia nova;
- deduplicacao por aprovacao legada;
- bloqueio para chamado em status final.

## 37. Riscos de seguranca e governanca

- interpretar `Bloqueante` como bloqueio ja ativo;
- supor que fluxo sequencial/paralelo/multinivel ja esteja orquestrado;
- acoplar indevidamente a nova instancia ao legado por catalogo;
- usar a geracao fora de um ponto de integracao controlado e produzir duplicidade operacional.

## 38. Decisoes adiadas para proximos itens

- bloqueio operacional de movimentacao;
- integracao ampla com abertura/atendimento;
- geracao de etapas;
- geracao de decisoes;
- aprovacao funcional;
- rejeicao funcional;
- cancelamento/expiracao funcionais;
- consolidacao de workflow;
- quórum, grupo aprovador e delegacao.

## 39. Conclusao tecnica

O item 38 materializou a primeira geracao real do motor de aprovacoes ITSM: a politica aplicavel agora pode virar uma instância pendente rastreavel, sem romper o fluxo atual e sem antecipar o bloqueio operacional.

## 40. Proxima etapa recomendada

Executar o item 39: criar a regra para bloquear movimentacao com aprovacao pendente.
