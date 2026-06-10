# Sprint 4 - Modelagem da decisao de aprovacao

## 1. Objetivo da modelagem

Modelar tecnicamente a decisao formal de aprovacao como registro proprio do ato decisorio, separando regra, instancia, etapa e resultado formal praticado sobre um chamado em contexto ITSM.

## 2. Limites desta etapa

- Esta etapa modela apenas a decisao formal de aprovacao.
- Nao houve implementacao do motor de avaliacao.
- Nao houve aprovacao ou rejeicao operacional nova nos use cases.
- Nao houve workflow funcional sequencial, paralelo ou multinivel.
- Nao houve consolidacao de decisao, calculo de quorum ou delegacao funcional.
- Nao houve endpoint, controller, tela ou service frontend.
- Nao houve alteracao operacional em abertura, atendimento ou SLA.

## 3. Contexto das definicoes anteriores

- O item 29 modelou `ConfiguracaoRegraAprovacao` como politica persistivel.
- O item 30 modelou `InstanciaAprovacaoChamado` como processo concreto de aprovacao vinculado ao chamado.
- O item 31 modelou `EtapaAprovacaoChamado` como nivel, ramo ou parte interna do processo.
- Faltava o registro proprio do ato formal que documenta quem decidiu, quando decidiu, o que decidiu e qual efeito a decisao produziu.

## 4. Diferenca entre regra, instancia, etapa e decisao

- regra:
  - define a politica;
  - explica quando e por que uma aprovacao deve existir.
- instancia:
  - representa o processo concreto aberto para um chamado.
- etapa:
  - representa um nivel, ramo ou parte interna da instancia.
- decisao:
  - representa o ato formal praticado por aprovador, grupo, sistema ou processo autorizado;
  - registra o resultado aplicado sobre a instancia inteira ou sobre uma etapa especifica.

## 5. Situacao atual de `AprovacaoChamado`

`AprovacaoChamado` continua sendo a aprovacao simples legada. Ela ja resume:

- chamado;
- origem;
- status simples;
- aprovador;
- justificativa de decisao;
- `BloqueiaAvancoAtendimento`;
- `DecididaEm`;
- cancelamento e auditoria basica.

Ela nao foi removida, nao foi quebrada e nao recebeu migracao de dados para a nova entidade.

## 6. Situacao atual de `InstanciaAprovacaoChamado`

`InstanciaAprovacaoChamado` ja representa o processo concreto e preserva:

- contexto avaliado do chamado;
- snapshot minimo da regra;
- efeito operacional;
- tipo de fluxo;
- dados de solicitante;
- aprovador resolvido em resumo;
- prazo, cancelamento, expiracao e `DecididaEm` resumida.

Ela segue como agregador pai das decisoes.

## 7. Situacao atual de `EtapaAprovacaoChamado`

`EtapaAprovacaoChamado` ja representa a fatia estrutural da instancia:

- nivel;
- ordem;
- ramo;
- tipo da etapa;
- obrigatoriedade;
- criticidade para consolidacao;
- snapshots de escopo e regra;
- dados futuros de grupo, quorum e delegacao.

Ela segue como alvo opcional da decisao.

## 8. Necessidade da decisao formal de aprovacao

A decisao formal e necessaria para:

- separar resumo de status do ato auditavel;
- registrar quem decidiu e com qual autoridade;
- guardar escopo decidido e efeito operacional;
- distinguir aprovacao, rejeicao, cancelamento, expiracao e reavaliacao;
- permitir historico detalhado em fluxos simples, sequenciais, paralelos e multiniveis;
- preparar consolidacao futura sem implementa-la agora.

## 9. Modelo conceitual da decisao

A decisao modelada representa o evento formal de decisao sobre uma aprovacao. Ela responde:

- qual instancia recebeu a decisao;
- se a decisao vale para a instancia inteira ou para uma etapa;
- qual foi o tipo do ato praticado;
- qual foi o resultado reconhecido;
- quem decidiu;
- qual autoridade foi usada;
- qual contexto foi decidido;
- qual efeito operacional foi produzido;
- quais status estavam antes e depois.

## 10. Entidade proposta, se criada

Foi criada a entidade [DecisaoAprovacaoChamado](</c:/Pessoal/SGX/SGX Sistema de Chamados Completo/src/SGX.SistemaChamado.Domain/Entities/DecisaoAprovacaoChamado.cs>).

O nome foi escolhido para permitir decisao em instancia simples e tambem decisao vinculada a etapa sem duplicar entidades.

## 11. Campos propostos

Campos modelados:

- `InstanciaAprovacaoChamadoId`
- `EtapaAprovacaoChamadoId`
- `TipoDecisao`
- `Resultado`
- `DataDecisao`
- `DecisorUsuarioId`
- `PapelDecisorSnapshot`
- `AutoridadeDecisorSnapshot`
- `DecisorEhAprovadorEspecifico`
- `DecisorEhAprovadorPadrao`
- `DecisorEhMembroGrupo`
- `DecisorPorDelegacao`
- `GrupoAprovadorSnapshot`
- `QuorumEsperado`
- `QuorumAtingido`
- `Justificativa`
- `Observacao`
- `EscopoDecididoSnapshot`
- `EfeitoOperacional`
- `DecisaoParcial`
- `DecisaoFinal`
- `LiberaAvanco`
- `MantemBloqueio`
- `ExigeReavaliacao`
- `PermiteNovaSolicitacao`
- `CancelaFluxo`
- `StatusInstanciaAnterior`
- `StatusInstanciaNovo`
- `StatusEtapaAnterior`
- `StatusEtapaNovo`
- `StatusChamadoAnteriorId`
- `StatusChamadoNovoId`
- `NivelEtapaSnapshot`
- `OrdemEtapaSnapshot`
- `RamoEtapaSnapshot`
- `RegraNomeSnapshot`
- `RegraVersaoSnapshot`
- `RegraCriterioSnapshot`
- auditoria base

## 12. Vinculo com `InstanciaAprovacaoChamado`

- `InstanciaAprovacaoChamadoId` e obrigatorio.
- Toda decisao precisa existir dentro de uma instancia.
- `InstanciaAprovacaoChamado` passou a expor a colecao `Decisoes`.

## 13. Vinculo com `EtapaAprovacaoChamado`

- `EtapaAprovacaoChamadoId` e opcional.
- Quando informado, a decisao se aplica a uma etapa especifica.
- Quando nao informado, a decisao pode representar decisao direta na instancia simples.
- A integridade "etapa pertence a mesma instancia" foi reforcada com chave composta entre decisao e etapa.

## 14. Tipo e resultado da decisao

Foi criado o enum [TipoDecisaoAprovacaoChamado](</c:/Pessoal/SGX/SGX Sistema de Chamados Completo/src/SGX.SistemaChamado.Domain/Enums/TipoDecisaoAprovacaoChamado.cs>) com:

- `Aprovacao`
- `Rejeicao`
- `Cancelamento`
- `Expiracao`
- `Reavaliacao`
- `Substituicao`
- `RegistroManual`

Foi criado o enum [ResultadoDecisaoAprovacaoChamado](</c:/Pessoal/SGX/SGX Sistema de Chamados Completo/src/SGX.SistemaChamado.Domain/Enums/ResultadoDecisaoAprovacaoChamado.cs>) com:

- `Aprovada`
- `Reprovada`
- `Cancelada`
- `Expirada`
- `RequerAjuste`
- `RequerNovaAprovacao`
- `SemEfeitoOperacional`

## 15. Decisor e autoridade

O decisor foi modelado por:

- `DecisorUsuarioId`
- `PapelDecisorSnapshot`
- `AutoridadeDecisorSnapshot`
- `DecisorEhAprovadorEspecifico`
- `DecisorEhAprovadorPadrao`
- `DecisorEhMembroGrupo`
- `DecisorPorDelegacao`

Isso permite registrar autoria humana ou automatizada sem exigir implementacao funcional de grupo ou delegacao agora.

## 16. Justificativa da decisao

- `Justificativa` registra a razao principal da decisao.
- `Observacao` permite complemento administrativo.
- Ambas sao textuais, auditaveis e independentes do legado.

## 17. Escopo decidido

O escopo decidido foi representado por:

- `EscopoDecididoSnapshot`
- `NivelEtapaSnapshot`
- `OrdemEtapaSnapshot`
- `RamoEtapaSnapshot`
- snapshots da regra

Assim a decisao pode explicar se foi sobre a instancia inteira, uma etapa simples, um nivel sequencial ou um ramo paralelo.

## 18. Efeito operacional produzido

`EfeitoOperacional` reaproveita `EfeitoOperacionalRegraAprovacao` para registrar o efeito reconhecido pela decisao:

- permitir;
- sinalizar;
- exigir aprovacao;
- exigir aprovacao com bloqueio;
- requerer reavaliacao.

Isso nao executa nada, apenas preserva o efeito auditavel.

## 19. Decisao parcial versus total

- `DecisaoParcial = true` indica decisao sobre parte do processo, como etapa, ramo ou subconjunto do escopo.
- `DecisaoParcial = false` indica decisao total no escopo registrado.

## 20. Decisao intermediaria versus final

- `DecisaoFinal = true` indica que, no escopo da propria decisao, nao ha nova deliberacao pendente.
- `DecisaoFinal = false` indica ato intermediario, preparatorio ou nao consolidado.

Ela nao consolida a instancia inteira nesta etapa.

## 21. Relacao com aprovacao simples

- Em fluxo simples, a decisao pode apontar apenas para a instancia.
- Opcionalmente, no futuro, fluxo simples tambem pode usar uma etapa unica.
- A modelagem nao obriga um dos caminhos agora.

## 22. Relacao com aprovacao sequencial

- Em fluxo sequencial, a decisao tende a apontar para etapa.
- `NivelEtapaSnapshot` e `OrdemEtapaSnapshot` preservam o ponto do fluxo.
- Uma decisao sequencial intermediaria nao significa aprovacao final da instancia.

## 23. Relacao com aprovacao paralela

- Em fluxo paralelo, a decisao tende a apontar para etapa e ramo.
- `RamoEtapaSnapshot` ajuda a identificar a ramificacao decidida.
- Uma decisao em um ramo nao libera a instancia inteira por si so.

## 24. Relacao com aprovacao multinivel

- A combinacao de `EtapaAprovacaoChamadoId`, `NivelEtapaSnapshot`, `OrdemEtapaSnapshot` e `DecisaoFinal` prepara o historico para fluxos multiniveis.
- Nao houve consolidacao funcional por nivel.

## 25. Relacao com grupo aprovador futuro

- `GrupoAprovadorSnapshot` e `DecisorEhMembroGrupo` deixam a trilha pronta para grupo aprovador.
- Nao existe grupo aprovador real implementado neste item.

## 26. Relacao com quorum futuro

- `QuorumEsperado` e `QuorumAtingido` sao opcionais.
- A decisao pode guardar contexto de quorum sem calcular quorum.

## 27. Relacao com delegacao futura

- `DecisorPorDelegacao` deixa registrado que a autoridade foi exercida por delegacao.
- Nao existe fluxo funcional de delegacao nesta etapa.

## 28. Relacao com cancelamento

- `TipoDecisao = Cancelamento` e `Resultado = Cancelada` permitem modelar o ato formal de cancelamento.
- Isso nao substitui o comportamento funcional futuro de cancelamento da instancia ou etapa.

## 29. Relacao com expiracao

- `TipoDecisao = Expiracao` e `Resultado = Expirada` permitem modelar expiracao formal.
- Isso nao implementa job, scheduler ou motor de expiracao.

## 30. Relacao com reavaliacao

- `TipoDecisao = Reavaliacao`
- `Resultado = RequerNovaAprovacao` ou `RequerAjuste`
- `ExigeReavaliacao = true`

Essa combinacao prepara reabertura conceitual sem executar reavaliacao funcional.

## 31. Relacao com bloqueio operacional

- `MantemBloqueio` indica se o efeito reconhecido ainda bloqueia o fluxo.
- Isso nao altera use cases nem bloqueia atendimento neste item.

## 32. Relacao com liberacao parcial ou total

- `LiberaAvanco` permite registrar liberacao.
- Em conjunto com `DecisaoParcial`, a modelagem acomoda liberacao parcial.
- Em conjunto com `DecisaoFinal`, acomoda liberacao final no escopo da decisao.

## 33. Relacao com `BloqueiaAvancoAtendimento`

- A nova entidade nao altera `BloqueiaAvancoAtendimento`.
- Ela apenas registra `LiberaAvanco` e `MantemBloqueio`.
- O legado continua sendo a referencia operacional atual.

## 34. Relacao com `AguardandoAprovacao`

- A modelagem nao altera o status do chamado.
- `StatusChamadoAnteriorId` e `StatusChamadoNovoId` permitem preservar a intencao futura de transicao envolvendo `AguardandoAprovacao`.

## 35. Relacao com SLA

- A decisao nao pausa, recalcula ou reabre SLA.
- Os campos sao apenas metadados auditaveis, sem efeito operacional sobre prazo nesta etapa.

## 36. Relacao com auditoria

A decisao foi desenhada para servir como base auditavel futura:

- quem decidiu;
- quando decidiu;
- com qual papel e autoridade;
- sobre qual escopo;
- com qual resultado;
- com qual efeito operacional;
- com quais status antes e depois;
- se a decisao foi parcial ou final.

## 37. Relacao com `AprovacaoChamado` legado

- `AprovacaoChamado` permanece como resumo simples legado.
- Nenhum dado legado foi migrado para a nova entidade.
- A documentacao assume que o legado resume a decisao simples, enquanto `DecisaoAprovacaoChamado` prepara trilha detalhada para o motor novo.

## 38. Compatibilidade com chamados legados

- Nenhuma decisao foi criada para chamados existentes.
- Nenhum chamado legado foi reprocessado.
- A estrutura apenas prepara compatibilidade futura.

## 39. Constraints, indices e integridade

Foram definidos:

- FK obrigatoria para `InstanciaAprovacaoChamado`;
- FK opcional para `EtapaAprovacaoChamado`;
- chave composta entre decisao e etapa para garantir mesma instancia;
- FKs opcionais para `DecisorUsuarioId`, `StatusChamadoAnteriorId` e `StatusChamadoNovoId`;
- indices por instancia, etapa, tipo, resultado, data e decisor;
- check para quorum positivo;
- check para `QuorumAtingido` depender de `QuorumEsperado`;
- check para `RegraVersaoSnapshot > 0`;
- check para `NivelEtapaSnapshot > 0`;
- check para `OrdemEtapaSnapshot >= 0`;
- check para impedir `LiberaAvanco` e `MantemBloqueio` simultaneos;
- check para exigir status de etapa quando houver decisao vinculada a etapa.

## 40. Riscos de seguranca e governanca

- usar a nova decisao como gatilho funcional antes do motor existir;
- registrar autoridade fraca ou ambigua para decisor;
- confiar apenas em texto livre sem politicas futuras de permissao;
- confundir resumo legado com trilha detalhada nova;
- usar quorum ou grupo como se ja houvesse consolidacao funcional;
- permitir leitura operacional da decisao sem validar permissao de aprovacao futura.

## 41. Decisoes adiadas para proximos itens

- migration estrutural do motor como marco separado do checklist;
- contratos de configuracao e de decisao;
- service de aplicacao do motor;
- consolidacao de instancias e etapas;
- calculo funcional de quorum;
- grupo aprovador real;
- delegacao funcional;
- cancelamento, expiracao e reavaliacao operacionais;
- endpoints, controllers, telas e services frontend.

## 42. Conclusao tecnica

`DecisaoAprovacaoChamado` fecha a quarta camada estrutural do motor de aprovacoes da Sprint 4: o ato formal auditavel. A modelagem separa politica, processo, etapa e decisao, preserva compatibilidade com o legado e deixa pronto o trilho para fluxos simples, sequenciais, paralelos e multiniveis sem antecipar comportamento funcional.

## 43. Proxima etapa recomendada

Executar o item 33 da Sprint 4: criar migrations estruturais do motor de aprovacao.
