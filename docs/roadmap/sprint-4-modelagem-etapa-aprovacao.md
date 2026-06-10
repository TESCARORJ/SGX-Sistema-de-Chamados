# Sprint 4 - Modelagem da etapa de aprovacao

## 1. Objetivo da modelagem

Modelar tecnicamente a etapa de aprovacao como parte interna da instancia de aprovacao do chamado, permitindo representar ordem, nivel, ramo, papel, aprovador esperado e dados de prazo sem implementar workflow funcional.

## 2. Limites desta etapa

- Esta etapa modela apenas a etapa de aprovacao.
- Nao houve implementacao do motor de avaliacao.
- Nao houve geracao automatica real de etapas.
- Nao houve decisao formal detalhada.
- Nao houve workflow funcional sequencial, paralelo ou multinivel.
- Nao houve alteracao funcional de abertura, atendimento ou SLA.
- Nao houve endpoint, controller, tela ou service frontend.

## 3. Contexto das definicoes anteriores

- O item 29 modelou `ConfiguracaoRegraAprovacao`.
- O item 30 modelou `InstanciaAprovacaoChamado`.
- Faltava a parte interna da instancia para suportar representacao estrutural de fluxos simples, sequenciais, paralelos e multiniveis.

## 4. Diferença entre instância, etapa e decisão

- instancia:
  - representa o processo concreto de aprovacao do chamado;
  - pertence ao chamado.
- etapa:
  - representa uma parte interna da instancia;
  - identifica nivel, ramo, ordem e responsabilidade esperada.
- decisao:
  - representara o ato formal de aprovar, reprovar, cancelar ou expirar;
  - fica para o item 32.

## 5. Situação atual de `InstanciaAprovacaoChamado`

`InstanciaAprovacaoChamado` ja possui:

- status geral;
- origem;
- escopo avaliado;
- efeito operacional;
- tipo de fluxo;
- dados de solicitante e aprovador;
- prazo e snapshot da regra;
- vinculo com `Chamado`, `ConfiguracaoRegraAprovacao` e `AprovacaoChamado` legado.

Ela continua sendo o pai estrutural da aprovacao concreta.

## 6. Necessidade da etapa de aprovação

A etapa e necessaria para:

- representar aprovacoes simples como uma etapa unica;
- representar fluxo sequencial por nivel e ordem;
- representar fluxo paralelo por ramo;
- representar fluxo multinivel como combinacao de niveis e ramos;
- preparar auditoria e consolidacao futura sem implementar o workflow agora.

## 7. Modelo conceitual da etapa

A etapa modelada representa uma unidade interna de aprovacao com:

- responsabilidade esperada;
- posicao no fluxo;
- status proprio;
- dados de prazo;
- metadados de consolidacao;
- snapshot resumido do escopo e da regra.

## 8. Entidade proposta, se criada

Foi criada a entidade [EtapaAprovacaoChamado](C:\Pessoal\SGX\SGX%20Sistema%20de%20Chamados%20Completo\src\SGX.SistemaChamado.Domain\Entities\EtapaAprovacaoChamado.cs).

## 9. Campos propostos

Campos modelados:

- `InstanciaAprovacaoChamadoId`
- `Titulo`
- `Descricao`
- `Status`
- `TipoEtapa`
- `TipoFluxoAprovacao`
- `Ordem`
- `Nivel`
- `Ramo`
- `Obrigatoria`
- `CriticaParaConsolidacao`
- `PermiteReenvio`
- `PermiteFallback`
- `PermiteDelegacao`
- `TipoResolucaoAprovador`
- `AprovadorEspecificoUsuarioId`
- `AprovadorPadraoUsuarioId`
- `AprovadorResolvidoUsuarioId`
- `GrupoAprovadorSnapshot`
- `QuorumMinimo`
- `QuantidadeAprovacoesNecessarias`
- `SolicitanteId`
- `SolicitadaEm`
- `PrazoDecisaoHoras`
- `DeveExpirarEm`
- `ExpiradaEm`
- `CanceladaEm`
- `CanceladaPorUsuarioId`
- `MotivoCancelamento`
- `DecididaEm`
- `EscopoResumoSnapshot`
- `RegraNomeSnapshot`
- `RegraVersaoSnapshot`
- `RegraCriterioSnapshot`
- auditoria base

## 10. Vínculo com `InstanciaAprovacaoChamado`

- `InstanciaAprovacaoChamadoId` e obrigatorio.
- A instancia passou a expor a colecao `Etapas`.
- A etapa nao existe fora da instancia.

## 11. Ordem, nível e ramo

- `Ordem` representa posicao de execucao.
- `Nivel` representa camada logica ou nivel sequencial.
- `Ramo` identifica o ramo paralelo quando aplicavel.
- Etapas paralelas exigem `Ramo`.

## 12. Etapa obrigatória versus opcional

- `Obrigatoria` indica se a etapa precisa participar da consolidacao.
- `CriticaParaConsolidacao` exige `Obrigatoria = true`.
- Isso prepara bloqueio e liberacao parcial futuros sem implementa-los agora.

## 13. Status da etapa

Foi criado o enum `StatusEtapaAprovacaoChamado` com:

- `Pendente`
- `Aprovada`
- `Reprovada`
- `Cancelada`
- `Expirada`
- `AguardandoEtapaAnterior`
- `EmReavaliacao`
- `Substituida`
- `Ignorada`

## 14. Tipo ou papel da etapa

Foi criado o enum `TipoEtapaAprovacaoChamado` com:

- `Simples`
- `Tecnica`
- `Gerencial`
- `Financeira`
- `Seguranca`
- `Compliance`
- `DonoServico`
- `GrupoAprovador`
- `AprovadorPadrao`
- `Outro`

## 15. Aprovador esperado e aprovador resolvido

Foram modelados:

- `TipoResolucaoAprovador`
- `AprovadorEspecificoUsuarioId`
- `AprovadorPadraoUsuarioId`
- `AprovadorResolvidoUsuarioId`

Isso cobre expectativa de decisao e resumo do resolvedor sem implementar a decisao formal.

## 16. Compatibilidade futura com grupo aprovador

- `GrupoAprovadorSnapshot` registra um identificador textual minimo futuro.
- Nao ha entidade real de grupo ainda.
- A compatibilidade fica estrutural, nao funcional.

## 17. Compatibilidade futura com quórum

- `QuorumMinimo` e `QuantidadeAprovacoesNecessarias` foram modelados como opcionais.
- Nao ha calculo funcional de quorum nesta etapa.

## 18. Compatibilidade futura com delegação

- `PermiteDelegacao` foi modelado como flag estrutural.
- Nao ha implementacao funcional de delegacao agora.

## 19. Dados de solicitação da etapa

A etapa preserva:

- `SolicitanteId`
- `SolicitadaEm`
- `Titulo`
- `Descricao`

## 20. Dados de prazo e vencimento

Foram modelados:

- `PrazoDecisaoHoras`
- `DeveExpirarEm`
- `ExpiradaEm`

Sem impacto no SLA operacional.

## 21. Dados futuros de cancelamento e expiração

Foram modelados:

- `CanceladaEm`
- `CanceladaPorUsuarioId`
- `MotivoCancelamento`
- `ExpiradaEm`
- `DecididaEm`

## 22. Relação futura com decisão formal

- Nenhuma entidade de decisao foi criada agora.
- `Status` e `DecididaEm` funcionam apenas como resumo estrutural.
- O detalhamento formal fica para o item 32.

## 23. Relação com bloqueio operacional

- A etapa nao bloqueia use cases nesta etapa.
- Ela apenas registra se e `Obrigatoria` e `CriticaParaConsolidacao`.
- Isso servira de insumo futuro para consolidacao e bloqueio.

## 24. Relação com liberação parcial

- A modelagem permite rastrear liberacao por nivel, ramo e criticidade.
- Nenhuma liberacao funcional foi implementada.

## 25. Relação com fluxo simples

- Um fluxo simples pode ser representado por uma etapa unica `Simples`.
- A modelagem nao obriga multiplas etapas.

## 26. Relação com fluxo sequencial

- `TipoFluxoAprovacao = Sequencial`
- `Nivel`
- `Ordem`
- `AguardandoEtapaAnterior`

Esses dados permitem modelar a estrutura sem executar o encadeamento.

## 27. Relação com fluxo paralelo

- `TipoFluxoAprovacao = Paralela`
- `Ramo` obrigatorio
- etapas podem compartilhar `Nivel` e ter ramos distintos

## 28. Relação com fluxo multinível

- `TipoFluxoAprovacao = Multinivel`
- combinacao de `Nivel`, `Ordem`, `Ramo`, `Obrigatoria` e `CriticaParaConsolidacao`

## 29. Relação com auditoria

A etapa preserva dados suficientes para auditoria futura:

- quando foi solicitada;
- quem devia decidir;
- quem resolveu;
- qual nivel;
- qual ramo;
- qual regra resumida;
- qual escopo resumido;
- qual prazo.

## 30. Relação com `AprovacaoChamado` legado

- Nenhuma alteracao foi feita em `AprovacaoChamado`.
- O legado simples continua valido.
- Em transicao futura, o fluxo simples pode ser representado por instancia sem etapa ou por etapa simples unica, conforme estrategia posterior.

## 31. Compatibilidade com chamados legados

- Nao foram criadas etapas para chamados antigos.
- Nao houve reprocessamento.
- Nao houve migracao funcional.

## 32. Constraints, índices e integridade

Foram definidos:

- FK obrigatoria para `InstanciaAprovacaoChamado`;
- FKs para solicitante, aprovadores e auditoria;
- indices por `InstanciaAprovacaoChamadoId`, `Status`, `TipoEtapa`, `TipoFluxoAprovacao`, `SolicitadaEm` e `DeveExpirarEm`;
- indice composto por `InstanciaAprovacaoChamadoId`, `Nivel`, `Ordem`, `Ramo`;
- checks para `Ordem >= 0`, `Nivel > 0`, `QuorumMinimo > 0`, `QuantidadeAprovacoesNecessarias > 0`, `PrazoDecisaoHoras > 0`, `RegraVersaoSnapshot > 0` e expiracao planejada valida.

## 33. Riscos de segurança e governança

- usar etapa estrutural para impor workflow antes do motor existir;
- deixar fluxo paralelo sem ramo identificavel;
- tratar resumo da etapa como decisao formal;
- modelar quorum sem grupo real e induzir interpretacao funcional incorreta;
- duplicar significado entre status da instancia e status da etapa sem criterio de consolidacao.

## 34. Decisões adiadas para próximos itens

- entidade formal de decisao;
- consolidacao de etapa para instancia;
- calculo funcional de quorum;
- grupo aprovador real;
- delegacao funcional;
- liberacao parcial operacional;
- uso da etapa nos use cases;
- API e tela administrativa da etapa.

## 35. Conclusão técnica

`EtapaAprovacaoChamado` fecha a terceira camada estrutural do motor: a decomposicao interna da instancia de aprovacao. Ela permite representar nivel, ramo, ordem, papel, responsabilidade esperada e metadados de consolidacao sem executar workflow nem decisao formal.

## 36. Próxima etapa recomendada

Executar o item 32 da Sprint 4: modelar decisao de aprovacao.
