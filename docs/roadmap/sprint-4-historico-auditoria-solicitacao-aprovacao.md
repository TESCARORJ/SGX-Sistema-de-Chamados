# Sprint 4 - Historico e Auditoria de Solicitacao de Aprovacao
## 1. Objetivo da definicao
Definir conceitualmente quais informacoes devem ser registradas no historico e na auditoria quando uma aprovacao e solicitada, seja por regra automatica do motor, acao manual administrativa, servico sensivel, natureza ITSM, tipo de chamado, custo, risco, impacto, urgencia, fluxo simples, sequencial, paralelo ou multinivel.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de historico ou auditoria de solicitacao de aprovacao.
- Nao foram criadas entidades novas.
- Nao foram criadas tabelas de auditoria.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de solicitacao de aprovacao
- O SGX ja possui solicitacao manual de aprovacao pelo modulo administrativo.
- O SGX tambem possui solicitacao automatica simples na abertura de chamado por catalogo de servico quando o servico exige aprovacao.
- O fluxo atual registra aprovacao pendente, historico funcional do chamado e evento tecnico de auditoria, mas ainda sem trilha conceitual completa da regra que originou a solicitacao.
- Os itens 5 a 20 ja definiram o motor futuro, seus tipos de fluxo, bloqueio, liberacao, rejeicao, cancelamento e expiracao.
## 4. Dados atualmente registrados em `AprovacaoChamado`
- A entidade atual permite registrar:
  - `Id` da aprovacao;
  - `ChamadoId`;
  - `Titulo`;
  - `Descricao`;
  - `Status`;
  - `TipoOrigem`;
  - `BloqueiaAvancoAtendimento`;
  - `OrigemDescricao`;
  - `SolicitanteId`;
  - `AprovadorId`;
  - `JustificativaSolicitacao`;
  - `JustificativaDecisao`;
  - `SolicitadaEm`;
  - `DecididaEm`;
  - `CanceladoEm`;
  - `CanceladoPorUsuarioId`;
  - `MotivoCancelamento`;
  - `CriadoPorUsuarioId`;
  - `AtualizadoPorUsuarioId`;
  - campos de auditoria base de criacao e atualizacao.
- No fluxo atual, a solicitacao manual tambem gera historico `AprovacaoSolicitada`.
- A solicitacao automatica por catalogo tambem gera historico `AprovacaoSolicitada`.
- A auditoria tecnica atual da criacao registra um conjunto reduzido de dados, como `Id`, `ChamadoId`, `Status`, `TipoOrigem`, `OrigemDescricao`, `JustificativaSolicitacao` e `SolicitadaEm`.
## 5. Lacunas atuais de auditoria na solicitacao
- Hoje nao ha registro estruturado da regra exata que exigiu a aprovacao.
- Nao ha snapshot completo do chamado no momento da solicitacao.
- Nao ha diferenciacao estruturada entre solicitacao manual, automatica por catalogo e automatica por regras futuras do motor.
- Nao ha auditoria de resolucao de aprovador, grupo aprovador, fallback, aprovador padrao, nivel, ramo ou quorum.
- Nao ha registro estruturado do efeito operacional exato da solicitacao, como bloqueio total, bloqueio parcial ou mera sinalizacao.
- Nao ha relacao formal com SLA da aprovacao.
- Nao ha trilha suficiente para responder integralmente por que determinada aprovacao existe.
## 6. Conceito de historico e auditoria de solicitacao de aprovacao
Historico e auditoria de solicitacao de aprovacao e o registro rastreavel do evento que originou uma solicitacao de aprovacao, indicando quem ou qual regra solicitou, quando solicitou, por qual motivo, para qual escopo, com qual efeito operacional e quem foi definido como responsavel pela decisao.
O objetivo nao e apenas saber que uma aprovacao existe. O objetivo e saber por que ela existe.
## 7. Dados minimos obrigatorios da solicitacao
- Toda solicitacao de aprovacao deve permitir rastrear conceitualmente:
  - identificador do chamado;
  - identificador da aprovacao;
  - data e hora da solicitacao;
  - quem solicitou ou qual componente ou regra solicitou;
  - origem da solicitacao;
  - motivo da solicitacao;
  - regra que exigiu aprovacao;
  - escopo aprovado ou pendente de aprovacao;
  - status inicial da aprovacao;
  - se a aprovacao e bloqueante ou informativa;
  - se gerou bloqueio operacional;
  - se alterou o status do chamado;
  - aprovador, grupo ou nivel resolvido;
  - se houve fallback;
  - justificativa inicial, quando aplicavel;
  - dados relevantes do chamado no momento da solicitacao.
## 8. Dados da regra que gerou a solicitacao
- A auditoria conceitual da regra geradora deve registrar:
  - natureza ITSM que disparou a aprovacao;
  - tipo de chamado que disparou a aprovacao;
  - servico sensivel relacionado;
  - impacto;
  - urgencia;
  - custo;
  - risco;
  - regra de bloqueio;
  - regra de fluxo simples, sequencial, paralelo ou multinivel;
  - regra de aprovador, grupo ou fallback;
  - se a solicitacao foi manual ou automatica.
## 9. Dados do chamado no momento da solicitacao
- Deve ser registrado um retrato conceitual do contexto no instante da solicitacao:
  - status do chamado;
  - natureza ITSM;
  - tipo de chamado;
  - categoria;
  - subcategoria;
  - catalogo ou servico solicitado;
  - prioridade;
  - impacto;
  - urgencia;
  - solicitante do chamado;
  - responsavel ou tecnico atual, se houver;
  - grupo tecnico, se houver;
  - SLA vigente, se aplicavel;
  - dados de custo ou risco, se existirem no futuro;
  - escopo ou descricao da acao sensivel.
## 10. Dados do solicitante da aprovacao
- A auditoria deve registrar:
  - usuario que solicitou manualmente;
  - sistema ou motor que solicitou automaticamente;
  - origem administrativa, portal ou fluxo interno;
  - data e hora;
  - perfil ou papel do solicitante;
  - justificativa da solicitacao;
  - se a solicitacao foi gerada por alteracao de dados sensiveis.
## 11. Dados do aprovador resolvido
- A trilha conceitual deve registrar:
  - aprovador especifico, se houver;
  - aprovador padrao, se usado;
  - dono do servico, se usado;
  - grupo aprovador, se usado;
  - nivel ou ramo, se aplicavel;
  - motivo da escolha;
  - regra de resolucao;
  - se houve delegacao;
  - se houve fallback;
  - se houve ausencia de aprovador especifico.
## 12. Dados de fallback e aprovador padrao
- Quando o sistema usar fallback ou aprovador padrao, deve ser possivel auditar:
  - qual regra tentou resolver aprovador especifico;
  - por que nao encontrou aprovador especifico;
  - por que o fallback foi usado;
  - qual fallback foi escolhido;
  - se isso representa risco de governanca;
  - se exige revisao posterior da configuracao.
## 13. Dados de grupo aprovador
- Quando houver grupo aprovador, registrar conceitualmente:
  - grupo acionado;
  - regra que escolheu o grupo;
  - membros elegiveis no momento da solicitacao;
  - quorum esperado;
  - papeis obrigatorios;
  - se havia conflito de interesse conhecido;
  - se o grupo foi usado como autoridade ou apenas notificacao.
## 14. Dados de nivel, ramo ou fluxo multinivel
- Quando o motor futuro usar fluxo composto, a solicitacao deve registrar:
  - fluxo aplicado;
  - niveis previstos;
  - ramos previstos;
  - dependencias entre etapas;
  - consolidacao esperada;
  - escopo de cada etapa;
  - bloqueio gerado por etapa;
  - regra final de liberacao.
## 15. Auditoria de solicitacao manual
- A solicitacao manual deve registrar:
  - usuario solicitante;
  - perfil ou permissao que permitiu a acao;
  - justificativa administrativa;
  - origem manual da acao;
  - contexto do chamado no momento da solicitacao;
  - aprovador ou grupo definido;
  - efeito operacional inicial.
## 16. Auditoria de solicitacao automatica
- A solicitacao automatica deve registrar:
  - modulo ou componente que gerou a solicitacao;
  - regra automatica aplicada;
  - dados do chamado que motivaram a regra;
  - resultado da resolucao de aprovador;
  - escopo da aprovacao gerada;
  - efeito operacional inicial;
  - se a solicitacao ocorreu na abertura, alteracao, reavaliacao ou outro evento futuro.
## 17. Auditoria por natureza ITSM
- Quando a natureza ITSM for o gatilho, a auditoria deve registrar:
  - natureza que disparou a aprovacao;
  - por que essa natureza exige, sinaliza ou dispensa aprovacao;
  - se a natureza por si so gerou bloqueio ou se dependeu de combinacao com outros fatores.
## 18. Auditoria por tipo de chamado
- Quando o tipo de chamado for o gatilho, registrar:
  - tipo que disparou a aprovacao;
  - regra aplicada ao tipo;
  - relacao entre o tipo e o escopo avaliado;
  - se o tipo elevou a solicitacao para bloqueante ou apenas sinalizou.
## 19. Auditoria por servico sensivel
- Quando o servico sensivel for o gatilho, registrar:
  - servico solicitado;
  - motivo pelo qual ele e sensivel;
  - regra que exigiu aprovacao;
  - escopo sensivel coberto;
  - efeito de bloqueio ou sinalizacao.
## 20. Auditoria por impacto e urgencia
- Quando impacto e urgencia participarem da regra, registrar:
  - valores avaliados;
  - matriz ou regra que interpretou a combinacao;
  - se os valores reforcaram bloqueio, sinalizacao ou necessidade de escalonamento futuro.
## 21. Auditoria por custo ou risco
- Quando custo ou risco participarem da regra, registrar:
  - valores ou faixas avaliadas;
  - regra financeira ou de risco aplicada;
  - escopo financeiro ou de risco coberto;
  - se a solicitacao nasceu bloqueante, condicionada ou informativa.
## 22. Relacao com bloqueio operacional
- A auditoria da solicitacao deve registrar se a aprovacao solicitada:
  - bloqueou todo avanco;
  - bloqueou apenas acoes sensiveis;
  - apenas sinalizou;
  - alterou o status para `AguardandoAprovacao`;
  - ativou `BloqueiaAvancoAtendimento`;
  - deixou acoes permitidas apesar da pendencia.
## 23. Relacao com SLA
- A solicitacao de aprovacao deve registrar conceitualmente:
  - se o SLA do chamado foi afetado;
  - se o SLA foi pausado;
  - se existe prazo proprio da aprovacao;
  - se a aprovacao pode expirar;
  - se escalonamento futuro sera necessario;
  - se o tempo de aprovacao sera contado separadamente.
## 24. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a base atual da aprovacao simples.
- A entidade ja cobre identificacao, origem simples, solicitante, aprovador, justificativa, bloqueio e datas principais.
- O motor futuro exigira trilha complementar para regra geradora, snapshot do chamado, fallback, grupo, nivel, ramo, quorum e efeito operacional detalhado.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 25. Relacao com `BloqueiaAvancoAtendimento`
- Se `BloqueiaAvancoAtendimento` for definido na solicitacao, a auditoria deve registrar que a aprovacao nasceu como bloqueante e qual acao ou escopo foi bloqueado.
- O motor futuro nao deve depender apenas desse campo, mas a trilha deve manter compatibilidade com seu significado atual.
## 26. Relacao com `AguardandoAprovacao`
- Se a solicitacao colocar o chamado em `AguardandoAprovacao`, isso deve ser auditado como efeito operacional da solicitacao.
- O registro tambem deve deixar claro que bloqueio pode existir mesmo sem uso obrigatorio desse status.
## 27. Rastreabilidade esperada
- A auditoria de solicitacao deve permitir responder:
  - quem solicitou a aprovacao;
  - por que ela foi solicitada;
  - qual regra exigiu aprovacao;
  - qual escopo estava sendo avaliado;
  - quem deveria aprovar;
  - por que esse aprovador ou grupo foi escolhido;
  - havia fallback;
  - havia bloqueio;
  - quais dados do chamado existiam no momento;
  - qual era o efeito operacional da solicitacao.
## 28. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 29. Lacunas encontradas
- Historico atual pode registrar aprovacao criada, mas nao necessariamente todos os detalhes da regra geradora.
- Falta snapshot completo do chamado no momento da solicitacao.
- Falta diferenciacao estruturada entre solicitacao manual e automatica pelo motor.
- Falta registro estruturado da regra que disparou a aprovacao.
- Falta rastreabilidade completa de fallback.
- Falta auditoria de grupo, nivel, ramo e quorum.
- Falta registro do efeito operacional exato da solicitacao.
- Falta registro de impacto em SLA ou prazo de aprovacao.
## 30. Riscos de seguranca e governanca
- Aprovacao existir sem motivo rastreavel.
- Aprovador ser escolhido sem explicacao.
- Fallback ser usado sem auditoria.
- Bloqueio ser aplicado sem registro claro.
- Solicitacao automatica nao ser distinguida de solicitacao manual.
- Falta de snapshot permitir alteracao posterior sem rastreabilidade.
- Impossibilidade de auditar por que um chamado ficou bloqueado.
- Impossibilidade de explicar decisoes futuras de aprovacao, rejeicao, cancelamento ou expiracao.
## 31. Decisoes adiadas para proximos itens
- Como implementar tabela ou estrutura de auditoria.
- Como armazenar snapshot do chamado.
- Como versionar regra aplicada.
- Como registrar fallback de aprovador.
- Como armazenar membros elegiveis de grupo.
- Como auditar niveis e ramos.
- Como auditar impacto no SLA.
- Como exibir auditoria na interface.
- Como migrar historico atual.
- Como testar auditoria de solicitacao.
## 32. Conclusao tecnica
Historico e auditoria de solicitacao de aprovacao devem permitir reconstruir a origem, a regra, o contexto, os responsaveis e o efeito operacional inicial de cada aprovacao exigida pelo sistema. O foco nao e apenas registrar que houve uma aprovacao, mas demonstrar por que ela nasceu, qual escopo cobria e qual governanca foi aplicada desde o primeiro instante.
## 33. Proxima etapa recomendada
Executar o item 22 do checklist da Sprint 4: definir historico e auditoria de decisao de aprovacao.
