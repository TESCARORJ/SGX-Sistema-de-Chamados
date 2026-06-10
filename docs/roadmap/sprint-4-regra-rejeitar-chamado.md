# Sprint 4 - Regra para rejeitar chamado

## 1. Objetivo da regra

Criar a regra funcional de reprovacao do novo motor para registrar decisao negativa em `InstanciaAprovacaoChamado`, com suporte a reprovacao direta simples e reprovacao por `EtapaAprovacaoChamado`.

## 2. Limites desta etapa

- reprova apenas a estrutura nova;
- nao implementa cancelamento;
- nao implementa expiracao;
- nao implementa reavaliacao funcional completa;
- nao implementa workflow completo sequencial, paralelo ou multinivel;
- nao altera status do chamado;
- nao altera SLA;
- nao cria endpoint, controller, tela ou frontend.

## 3. Contexto dos itens anteriores

- item 38 passou a gerar `InstanciaAprovacaoChamado` pendente quando uma regra aplicavel exige aprovacao;
- item 39 passou a bloquear movimentacoes sensiveis e finais quando a pendencia for bloqueante;
- item 40 passou a registrar aprovacao positiva controlada;
- item 41 fecha o ciclo minimo negativo do novo motor ao registrar reprovacao controlada.

## 4. Diferenca entre aprovar, rejeitar, cancelar e expirar

- aprovar: registra decisao positiva e pode consolidar a instancia como aprovada;
- rejeitar: registra decisao negativa e pode consolidar a instancia como reprovada;
- cancelar: encerra administrativamente a pendencia sem usar decisao negativa funcional;
- expirar: encerra a pendencia por prazo, e nao por decisao humana.

## 5. Comportamento legado atual

`AprovacaoChamado` continua intacta. A nova regra nao altera `BloqueiaAvancoAtendimento`, nao muda `AguardandoAprovacao`, nao migra aprovacoes antigas e nao interfere no fluxo legado atual de reprovacao.

## 6. Regra/use case criado

Foi criado `ReprovarAprovacaoChamadoUseCase`.

## 7. Interface criada, se houver

Foi criada `IReprovarAprovacaoChamadoUseCase`.

## 8. Contratos internos ou contratos reutilizados

- request reutilizado: `ReprovarAprovacaoChamadoRequest`
- response criado: `ReprovarAprovacaoChamadoResponse`

## 9. Fluxo de rejeicao

1. receber request de reprovacao;
2. validar shape do request e justificativa obrigatoria;
3. localizar a instancia;
4. localizar a etapa, quando informada;
5. validar se instancia e etapa estao em status reprovavel;
6. impedir duplicidade final negativa;
7. resolver decisor;
8. criar `DecisaoAprovacaoChamado` negativa;
9. reprovar etapa, quando aplicavel;
10. reprovar instancia apenas quando a consolidacao negativa for segura;
11. persistir sem alterar status do chamado.

## 10. Criterios para rejeitar

- instancia existente;
- instancia em `Pendente` ou `EmReavaliacao`;
- instancia que exige aprovacao;
- justificativa informada;
- decisor resolvido com usuario valido;
- etapa existente e pertencente a instancia, quando informada;
- etapa em `Pendente` ou `EmReavaliacao`, quando informada;
- ausencia de decisao final negativa duplicada para o mesmo alvo.

## 11. Criterios para nao rejeitar

- instancia inexistente;
- instancia aprovada, reprovada, cancelada, expirada ou substituida;
- etapa inexistente;
- etapa de outra instancia;
- etapa aprovada, reprovada, cancelada, expirada, substituida, ignorada ou aguardando etapa anterior;
- tentativa de reprovar diretamente a instancia com etapa obrigatoria pendente;
- tentativa de combinar decisao parcial com cancelamento logico do fluxo.

## 12. Relacao com `InstanciaAprovacaoChamado`

O use case usa `RegistrarDecisaoResumo` para consolidar reprovacao final da instancia quando o cenario seguro for atendido.

## 13. Relacao com `EtapaAprovacaoChamado`

Quando a reprovacao e por etapa, a etapa recebe `RegistrarDecisaoResumo(StatusEtapaAprovacaoChamado.Reprovada, ...)`.

## 14. Relacao com `DecisaoAprovacaoChamado`

Toda reprovacao funcional gera uma `DecisaoAprovacaoChamado` com:

- `TipoDecisao = Rejeicao`
- `Resultado = Reprovada`
- snapshots de status, regra, escopo e etapa quando existirem.

## 15. Relacao com `AprovacaoChamado` legado

Somente compatibilidade passiva. O item 41 nao escreve nem altera `AprovacaoChamado`.

## 16. Relacao com `AprovarAprovacaoChamadoUseCase`

O item 41 espelha o padrao do item 40 para manter consistencia de validacoes, rastreabilidade, snapshots e separacao entre decisao parcial e final.

## 17. Relacao com regra de bloqueio do item 39

Nao houve alteracao ampla no item 39. O bloqueio continua olhando pendencias pendentes ou em reavaliacao; a instancia reprovada deixa de ser considerada pendencia aberta por aquela regra.

## 18. Relacao com `BloqueiaAvancoAtendimento`

Nenhuma alteracao. O campo legado permanece como fonte exclusiva do legado.

## 19. Relacao com `AguardandoAprovacao`

Nenhuma manipulacao automatica foi implementada.

## 20. Relacao com status do chamado

Nenhuma mudanca automatica de status foi adicionada.

## 21. Relacao com abertura de chamado

Nenhuma alteracao.

## 22. Relacao com atendimento

Nenhuma alteracao no atendimento comum, comentarios, anexos, evidencias, consulta, triagem ou operacao administrativa corrente.

## 23. Relacao com SLA

Nenhuma pausa, retomada, recalcule ou fechamento de SLA foi implementado.

## 24. Rejeicao direta na instancia

Permitida quando nao existe etapa informada e nao ha etapa obrigatoria pendente. Na instancia simples, o use case assume decisao final por padrao quando o request nao marca parcial nem final explicitamente.

## 25. Rejeicao por etapa

Permitida quando a etapa pertence a instancia e esta em estado reprovavel. A etapa sempre e marcada como reprovada, e a instancia so e consolidada como reprovada quando o cenario negativo for seguro.

## 26. Decisao parcial versus final

- parcial: reprova apenas o escopo atual e preserva a instancia em aberto;
- final: consolida a instancia como reprovada;
- etapa critica tambem pode consolidar a instancia como reprovada quando nao houver marcacao parcial.

## 27. Manutencao de bloqueio ou impedimento operacional

`MantemBloqueio` e registrado na decisao. O item nao reabre chamado nem libera avancos automaticamente.

## 28. Tratamento de rejeicao duplicada

O use case rejeita nova decisao final negativa para o mesmo alvo.

## 29. Tratamento de instancia pendente

Pode ser reprovada.

## 30. Tratamento de instancia em reavaliacao

Pode ser reprovada, inclusive por etapa, sem executar reavaliacao funcional completa.

## 31. Tratamento de instancia aprovada

Nao pode receber reprovacao nesta etapa.

## 32. Tratamento de instancia reprovada

Nao pode receber nova reprovacao final nesta etapa.

## 33. Tratamento de instancia cancelada, expirada ou substituida

Nao pode ser reprovada nesta etapa.

## 34. Compatibilidade com aprovacao simples

Implementada.

## 35. Compatibilidade limitada com fluxo sequencial

Implementada apenas no nivel de reprovacao de etapa e consolidacao simples da instancia quando nao ha salto inseguro de etapa.

## 36. Compatibilidade limitada com fluxo paralelo

Implementada apenas no nivel estrutural. Nao existe orquestracao real por ramo nem quorum.

## 37. Compatibilidade limitada com fluxo multinivel

Implementada apenas no nivel estrutural. Nao existe consolidacao completa entre niveis.

## 38. Garantias de ausencia de cancelamento/expiracao/reavaliacao funcional completa

O item 41 cria somente decisao negativa funcional. Nenhum cancelamento, expiracao ou reavaliacao funcional completa foi implementado.

## 39. Testes criados

Foi criada a suite `ReprovarAprovacaoChamadoUseCaseTests` cobrindo:

- reprovacao de instancia simples;
- reprovacao de etapa com efeito parcial;
- consolidacao negativa por etapa critica;
- instancia inexistente;
- instancia aprovada;
- etapa de outra instancia;
- decisao final negativa duplicada;
- tentativa de reprovacao direta com etapa obrigatoria pendente.

## 40. Riscos de seguranca e governanca

- aprovador resolvido ainda nao valida autoridade real por grupo ou delegacao;
- consolidacao negativa parcial ainda nao cobre quorum nem ramos paralelos reais;
- consumidores podem interpretar `ExigeReavaliacao` como execucao automatica, o que nao acontece neste item.

## 41. Decisoes adiadas para proximos itens

- cancelamento funcional;
- expiracao funcional;
- reavaliacao funcional completa;
- workflow sequencial/paralelo/multinivel completo;
- quorum;
- grupo aprovador real;
- delegacao;
- endpoints;
- frontend.

## 42. Conclusao tecnica

O item 41 fecha o primeiro ciclo funcional negativo do motor novo: gerar pendencia, bloquear quando aplicavel, aprovar quando positivo e reprovar de forma rastreavel quando a decisao for negativa.

## 43. Proxima etapa recomendada

Executar o item 42: criar regra para reavaliar aprovacao apos mudanca de dados sensiveis.
