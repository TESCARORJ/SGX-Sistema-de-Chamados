# Sprint 4 - Modelagem da configuracao de regra de aprovacao

## 1. Objetivo da modelagem

Modelar tecnicamente a configuracao da regra de aprovacao do futuro motor ITSM, definindo a estrutura que descreve quando uma aprovacao e exigida, por qual criterio, para qual escopo e com qual efeito operacional, sem ainda executar o motor completo.

## 2. Limites desta etapa

- Esta etapa modela apenas a configuracao da regra de aprovacao.
- Nao houve implementacao do motor de avaliacao completo.
- Nao houve geracao automatica de aprovacao por regra nova.
- Nao houve alteracao do fluxo atual de abertura, atendimento ou SLA.
- Nao houve alteracao funcional em `AprovacaoChamado`.
- Nao houve implementacao de etapas, ramos, decisoes, grupo aprovador, endpoint ou tela.
- O seed foi atualizado apenas para roadmap/checklist.

## 3. Contexto das definicoes anteriores

- A Sprint 4 ja definiu criterios conceituais por natureza ITSM, tipo, servico sensivel, impacto, urgencia, custo e risco.
- Tambem ja definiu comportamento simples, sequencial, paralelo e multinivel, alem de bloqueio, liberacao, rejeicao, cancelamento, expiracao, auditoria e compatibilidade.
- Faltava uma estrutura persistivel que representasse a politica da regra, sem confundir essa politica com a aprovacao concreta de um chamado.

## 4. Necessidade da configuracao de regra

A configuracao da regra e necessaria para separar politica de execucao:

- configuracao de regra:
  - define quando e como a aprovacao deve existir;
  - representa governanca reutilizavel;
  - nao depende de um chamado especifico.
- instancia de aprovacao do chamado:
  - sera modelada no item 30;
  - representara a aprovacao criada para um chamado real.
- etapa de aprovacao:
  - sera modelada no item 31.
- decisao de aprovacao:
  - sera modelada no item 32.

## 5. Modelo conceitual da regra de aprovacao

A regra modelada representa uma politica de aprovacao que responde:

- qual criterio dispara a regra;
- em qual escopo do chamado ela deve ser avaliada;
- qual efeito operacional ela produz;
- se a regra e apenas informativa ou bloqueante;
- qual estrategia futura de resolucao de aprovador deve ser usada;
- qual tipo de fluxo a regra pretende acionar;
- quando a regra esta vigente;
- como a regra deve ser priorizada entre outras regras.

## 6. Entidade proposta, se criada

Foi criada a entidade [ConfiguracaoRegraAprovacao](</c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/ConfiguracaoRegraAprovacao.cs>) como configuracao persistivel da politica de aprovacao.

Motivo da escolha do nome:

- deixa explicito que se trata de configuracao;
- evita confundir com aprovacao concreta do chamado;
- fica coerente com o padrao de nomenclatura do dominio atual.

## 7. Campos propostos

Campos estruturais modelados:

- `Id`
- `Nome`
- `Descricao`
- `TipoRegra`
- `EscopoRegra`
- `Ordem`
- `Prioridade`
- `Versao`
- `NaturezaChamado`
- `TipoSolicitacaoId`
- `CatalogoServicoId`
- `CategoriaId`
- `SubcategoriaId`
- `ImpactoMinimo`
- `UrgenciaMinima`
- `PrioridadeMinima`
- `CustoMinimo`
- `NivelRiscoMinimo`
- `ExigeAprovacao`
- `Bloqueante`
- `PermiteReenvio`
- `PermiteFallback`
- `EfeitoOperacional`
- `TipoFluxoAprovacao`
- `TipoResolucaoAprovador`
- `AprovadorEspecificoUsuarioId`
- `AprovadorPadraoUsuarioId`
- `PrazoDecisaoHoras`
- `VigenteDe`
- `VigenteAte`
- `CriadoPorUsuarioId`
- `AtualizadoPorUsuarioId`
- auditoria base (`CriadoEm`, `CriadoPor`, `AtualizadoEm`, `AtualizadoPor`, `Ativo`)

## 8. Criterios de aplicacao da regra

A entidade foi modelada para suportar combinacao de criterios, em vez de obrigar uma unica coluna classificatoria por regra. Isso permite:

- regra apenas por natureza;
- regra apenas por catalogo/servico;
- regra por tipo de solicitacao;
- regra por categoria/subcategoria;
- regra por impacto e urgencia;
- regra combinada por varios filtros ao mesmo tempo.

## 9. Criterios por natureza ITSM

- `NaturezaChamado` foi modelado como criterio opcional.
- Isso permite expressar regras para `Incidente`, `Requisicao`, `Mudanca`, `Problema`, `EventoAlerta` e `TarefaOperacional`.
- O criterio e opcional para nao limitar regras que dependem apenas de outros fatores.

## 10. Criterios por tipo de chamado

- O projeto atual usa `TipoSolicitacao`.
- Por isso, a entidade foi modelada com `TipoSolicitacaoId`, mantendo aderencia ao dominio atual.
- Isso permite separar regra por tipo sem introduzir nova semantica artificial de "tipo de chamado" fora do modelo existente.

## 11. Criterios por catalogo/servico

- `CatalogoServicoId` foi modelado como criterio opcional.
- Isso permite compatibilidade futura com servicos sensiveis e com a regra atual de catalogo que ja pode gerar aprovacao.
- A modelagem nao altera o comportamento atual do catalogo; apenas cria a estrutura de configuracao.

## 12. Criterios por impacto e urgência

- `ImpactoMinimo` e `UrgenciaMinima` foram modelados como thresholds opcionais.
- Isso permite expressar regra do tipo:
  - "a partir de impacto alto";
  - "a partir de urgencia alta";
  - "somente quando impacto e urgencia atingirem patamar minimo".

## 13. Criterios futuros por custo e risco

- `CustoMinimo` foi modelado como `decimal?`.
- `NivelRiscoMinimo` foi modelado como `int?`.
- Esses campos foram mantidos opcionais e sem acoplamento com o `Chamado` atual, porque custo e risco ainda nao estao plenamente estruturados no fluxo funcional.
- Assim, a entidade ja suporta evolucao futura sem quebrar o modelo atual.

## 14. Efeito operacional da regra

Foi criado o enum [EfeitoOperacionalRegraAprovacao](</c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Enums/EfeitoOperacionalRegraAprovacao.cs>) para expressar:

- `Permitir`
- `Sinalizar`
- `ExigirAprovacao`
- `ExigirAprovacaoEBloquearAvanco`
- `RequerReavaliacao`

Isso cobre:

- abertura permitida sem interferencia;
- abertura ou atendimento com sinalizacao;
- exigencia de aprovacao sem bloqueio imediato;
- exigencia de aprovacao bloqueante;
- necessidade futura de reavaliacao.

## 15. Regra bloqueante versus informativa

- `Bloqueante` foi mantido como campo explicito para compatibilidade com o conceito atual de bloqueio simples.
- `ExigeAprovacao` tambem foi mantido para dar clareza semantica.
- A entidade valida coerencia minima:
  - `Permitir` e `Sinalizar` nao podem exigir aprovacao nem marcar bloqueio;
  - `ExigirAprovacao` exige aprovacao e nao bloqueia;
  - `ExigirAprovacaoEBloquearAvanco` exige aprovacao e bloqueia.

## 16. Resolucao de aprovador

Foi criado o enum [TipoResolucaoAprovadorRegraAprovacao](</c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Enums/TipoResolucaoAprovadorRegraAprovacao.cs>) com:

- `NaoDefinido`
- `AprovadorEspecifico`
- `AprovadorPadrao`
- `GrupoAprovadorFuturo`
- `ResolucaoDinamicaFutura`

Isso permite distinguir a estrategia pretendida pela regra sem implementar ainda a resolucao efetiva.

## 17. Compatibilidade com aprovador padrão

- Foi modelado `AprovadorPadraoUsuarioId` opcional.
- Ele so pode ser usado quando `TipoResolucaoAprovador = AprovadorPadrao`.
- Isso deixa o caso simples suportado sem precisar implementar ainda logica de fallback real.

## 18. Compatibilidade futura com grupo aprovador

- Grupo aprovador nao foi implementado nem relacionado por FK nesta etapa.
- A compatibilidade ficou representada pelo valor `GrupoAprovadorFuturo` no tipo de resolucao.
- Isso evita acoplamento prematuro com estrutura ainda nao modelada.

## 19. Compatibilidade com aprovação simples

- O modelo ja suporta o caso simples:
  - regra ativa;
  - criterio aplicavel;
  - exigencia de aprovacao;
  - aprovacao bloqueante ou informativa;
  - aprovador especifico ou padrao opcional;
  - fluxo `Simples`.

## 20. Compatibilidade futura com aprovação sequencial

- O enum [TipoFluxoAprovacao](</c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Enums/TipoFluxoAprovacao.cs>) inclui `Sequencial`.
- Nenhuma etapa foi modelada agora.
- A configuracao apenas registra a intencao de fluxo.

## 21. Compatibilidade futura com aprovação paralela

- O mesmo enum inclui `Paralela`.
- A regra pode declarar que o fluxo pretendido e paralelo, sem criar ramos ainda.
- A estrutura de ramos ficara para os itens seguintes.

## 22. Compatibilidade futura com aprovação multinível

- O enum inclui `Multinivel`.
- Isso permite registrar que a regra exigira algo mais complexo do que aprovacao simples, preservando compatibilidade futura sem antecipar a modelagem de etapas e decisoes.

## 23. Vigência, prioridade e versão da regra

- `Ativo` vem da auditoria base.
- `VigenteDe` e `VigenteAte` permitem controlar janela de vigencia.
- `Ordem` ajuda a manter ordem de avaliacao.
- `Prioridade` ajuda a resolver conflitos semanticos entre regras.
- `Versao` permite rastrear evolucao da mesma regra ao longo do tempo.

## 24. Relação com `Chamado`

- Nao foi criada relacao direta com `Chamado`.
- Isso foi intencional:
  - a configuracao nao representa ocorrencia concreta;
  - o item 30 modelara a instancia de aprovacao do chamado;
  - o `Chamado` atual nao deve receber acoplamento novo desnecessario agora.

## 25. Relação com `AprovacaoChamado`

- Nao houve alteracao em `AprovacaoChamado`.
- A relacao, por enquanto, e apenas conceitual:
  - `ConfiguracaoRegraAprovacao` define a politica;
  - `AprovacaoChamado` continua como base atual da aprovacao simples;
  - a futura instancia do item 30 devera ligar chamada concreta e regra aplicada.

## 26. Relação com catálogo de serviço

- Foi criada relacao opcional com `CatalogoServico`.
- Isso permite que uma regra futura se aplique a um servico especifico do catalogo, sem mudar a regra atual de `RequerAprovacao`.
- O catalogo atual continua funcionando como antes.

## 27. Relação com `BloqueiaAvancoAtendimento`

- A configuracao nao altera o campo atual.
- O conceito de bloqueio da regra foi modelado em `Bloqueante` e no efeito operacional.
- No futuro, essa configuracao podera orientar quando uma instancia de aprovacao deve refletir em `BloqueiaAvancoAtendimento`, preservando compatibilidade com o legado.

## 28. Relação com `AguardandoAprovacao`

- A configuracao tambem nao altera esse status.
- O `EscopoRegra` e o `EfeitoOperacional` permitem declarar quando a regra deve afetar abertura, atendimento, encerramento ou reavaliacao.
- A eventual transicao para `AguardandoAprovacao` ficara para a implementacao futura do motor.

## 29. Relação com SLA

- A regra pode declarar `PrazoDecisaoHoras` como metadado futuro.
- Isso nao altera o SLA operacional do chamado nesta etapa.
- O objetivo e apenas deixar a configuracao preparada para eventual prazo proprio de aprovacao, sem confundir com o SLA do chamado.

## 30. Relação com auditoria da solicitação

- A entidade nao cria auditoria por si so.
- Mas ela adiciona rastreabilidade estrutural para a futura auditoria de solicitacao:
  - regra aplicada;
  - versao;
  - criterio usado;
  - efeito operacional esperado;
  - tipo de fluxo;
  - estrategia de aprovador.

## 31. Compatibilidade com chamados legados

- A modelagem nao reprocessa chamados existentes.
- Nao cria aprovacoes reais.
- Nao altera relacoes legadas.
- Ela apenas introduz a estrutura para regras futuras coexistirem com o legado de forma controlada.

## 32. Constraints, índices e integridade

Foram definidos:

- FK opcional para `TipoSolicitacao`, `CatalogoServico`, `CategoriaChamado` e `SubcategoriaChamado`;
- FK para `CriadoPorUsuario`, `AtualizadoPorUsuario`, `AprovadorEspecificoUsuario` e `AprovadorPadraoUsuario`;
- indice unico por `Nome + Versao`;
- indices por atividade, escopo, ordem, prioridade e criterios principais;
- check constraint para vigencia (`VigenteAte >= VigenteDe`);
- check constraint para impedir subcategoria sem categoria;
- check constraint para `CustoMinimo >= 0`;
- check constraint para `NivelRiscoMinimo > 0`;
- check constraint para `PrazoDecisaoHoras > 0`.

## 33. Riscos de segurança e governança

- ativar regra estrutural sem homologacao funcional;
- usar configuracao nova para inferir bloqueio automatico antes do motor existir;
- confundir regra com aprovacao concreta;
- acoplar grupo aprovador prematuramente;
- tratar `PrazoDecisaoHoras` como SLA funcional antes da definicao adequada;
- criar versoes conflitantes da mesma regra sem governanca;
- deixar regras informativas inconsistentes com bloqueio.

## 34. Decisões adiadas para próximos itens

- modelagem da instancia concreta de aprovacao do chamado;
- modelagem de etapas e ramos;
- modelagem da decisao formal;
- representacao estruturada de grupo aprovador;
- execucao automatica da avaliacao da regra;
- geracao real de aprovacao a partir da regra;
- bloqueio operacional nos use cases;
- API administrativa da configuracao;
- tela de administracao da regra;
- seed funcional de regras ativas;
- homologacao da governanca da configuracao.

## 35. Conclusão técnica

A entidade `ConfiguracaoRegraAprovacao` fecha a primeira camada estrutural do motor de aprovacao: a politica. Ela permite governar criterios, escopo, efeito, vigencia, versao e estrategia futura de aprovador, sem confundir configuracao com aprovacao concreta e sem alterar o comportamento atual do sistema.

## 36. Próxima etapa recomendada

Executar o item 30 da Sprint 4: modelar a instancia de aprovacao do chamado.
