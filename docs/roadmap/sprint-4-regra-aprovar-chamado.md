# Sprint 4 - Regra para aprovar chamado

## 1. Objetivo da regra

Criar a primeira regra funcional do motor novo para registrar aprovacao positiva em `InstanciaAprovacaoChamado`, com suporte a aprovacao direta simples e aprovacao por `EtapaAprovacaoChamado`.

## 2. Limites desta etapa

- aprova apenas a estrutura nova;
- nao implementa rejeicao;
- nao implementa cancelamento ou expiracao;
- nao implementa workflow completo sequencial, paralelo ou multinivel;
- nao altera status do chamado;
- nao altera SLA;
- nao cria endpoint, controller, tela ou frontend.

## 3. Contexto dos itens anteriores

- item 38 passou a gerar `InstanciaAprovacaoChamado` pendente quando uma regra aplicavel exige aprovacao;
- item 39 passou a bloquear apenas movimentacoes sensiveis/finais quando existe pendencia bloqueante;
- item 40 fecha o ciclo minimo positivo do novo motor ao registrar a aprovacao e atualizar a estrutura concreta.

## 4. Diferenca entre bloquear, aprovar e rejeitar

- bloquear: impede a acao sensivel enquanto a pendencia continua aberta;
- aprovar: registra decisao positiva e pode consolidar a instancia quando permitido;
- rejeitar: permanece adiado para o item 41.

## 5. Comportamento legado atual

`AprovacaoChamado` continua intacta. A nova regra nao altera `BloqueiaAvancoAtendimento`, nao migra aprovacoes antigas e nao interfere no fluxo legado de decisao.

## 6. Regra/use case criado

Foi criado `AprovarAprovacaoChamadoUseCase`.

## 7. Interface criada, se houver

Foi criada `IAprovarAprovacaoChamadoUseCase`.

## 8. Contratos internos ou contratos reutilizados

- request reutilizado: `AprovarAprovacaoChamadoRequest`
- response criado: `AprovarAprovacaoChamadoResponse`

## 9. Fluxo de aprovacao

1. receber request de aprovacao;
2. validar shape do request;
3. localizar a instancia;
4. localizar a etapa, quando informada;
5. impedir duplicidade final;
6. validar se instancia e etapa estao em status aprovavel;
7. resolver decisor;
8. criar `DecisaoAprovacaoChamado` positiva;
9. aprovar etapa, quando aplicavel;
10. aprovar instancia apenas quando a consolidacao simples for valida;
11. persistir sem alterar status do chamado.

## 10. Criterios para aprovar

- instancia existente;
- instancia em `Pendente` ou `EmReavaliacao`;
- instancia que exige aprovacao;
- decisor resolvido com usuario valido;
- etapa existente e pertencente a instancia, quando informada;
- etapa em `Pendente` ou `EmReavaliacao`, quando informada;
- ausencia de decisao final positiva duplicada para o mesmo alvo.

## 11. Criterios para nao aprovar

- instancia inexistente;
- instancia cancelada, expirada, substituida ou reprovada;
- instancia ja aprovada com decisao final;
- etapa inexistente;
- etapa de outra instancia;
- etapa cancelada, expirada, substituida, ignorada ou reprovada;
- tentativa de aprovar diretamente a instancia com etapa obrigatoria pendente;
- tentativa de consolidar decisao final com outra etapa obrigatoria ainda pendente.

## 12. Relacao com `InstanciaAprovacaoChamado`

O use case usa `RegistrarDecisaoResumo` para consolidar aprovacao final da instancia quando permitido.

## 13. Relacao com `EtapaAprovacaoChamado`

Quando a aprovacao e por etapa, a etapa recebe `RegistrarDecisaoResumo(StatusEtapaAprovacaoChamado.Aprovada, ...)`.

## 14. Relacao com `DecisaoAprovacaoChamado`

Toda aprovacao funcional gera uma `DecisaoAprovacaoChamado` com:

- `TipoDecisao = Aprovacao`
- `Resultado = Aprovada`
- snapshots de status, regra, escopo e etapa quando existirem.

## 15. Relacao com `AprovacaoChamado` legado

Somente compatibilidade passiva. O item 40 nao escreve nem altera `AprovacaoChamado`.

## 16. Relacao com `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`

Nao houve acoplamento direto. A liberacao ocorre naturalmente porque a instancia aprovada deixa de aparecer como pendente bloqueante no item 39.

## 17. Relacao com `BloqueiaAvancoAtendimento`

Nenhuma alteracao. O campo legado permanece como fonte exclusiva do legado.

## 18. Relacao com `AguardandoAprovacao`

Nenhuma manipulacao automatica foi implementada.

## 19. Relacao com status do chamado

Nenhuma mudanca automatica de status foi adicionada.

## 20. Relacao com abertura de chamado

Nenhuma alteracao.

## 21. Relacao com atendimento

Nenhuma alteracao no atendimento comum, comentarios, anexos, evidencias, triagem ou organizacao operacional.

## 22. Relacao com SLA

Nenhuma pausa, retomada, recalcule ou fechamento de SLA foi implementado.

## 23. Aprovacao direta na instancia

Permitida quando nao existe etapa informada e nao ha etapa obrigatoria pendente. Na instancia simples, o use case assume decisao final por padrao quando o request nao marca parcial nem final explicitamente.

## 24. Aprovacao por etapa

Permitida quando a etapa pertence a instancia e esta em estado aprovavel. A aprovacao da etapa nao consolida automaticamente a instancia se ainda existir outra etapa obrigatoria pendente.

## 25. Decisao parcial versus final

- parcial: registra a aprovacao do escopo atual e mantem a instancia em aberto;
- final: consolida a instancia somente quando o cenario simples estiver satisfeito.

## 26. Liberacao de escopo aprovado

`LiberaAvanco` e registrado na decisao. A liberacao operacional continua dependente das regras de bloqueio e do status da instancia.

## 27. Tratamento de aprovacao duplicada

O use case rejeita nova decisao final positiva para o mesmo alvo.

## 28. Tratamento de instancia pendente

Pode ser aprovada.

## 29. Tratamento de instancia em reavaliacao

Pode ser aprovada, inclusive por etapa, desde que a consolidacao simples seja valida.

## 30. Tratamento de instancia aprovada

Nao pode receber nova aprovacao final.

## 31. Tratamento de instancia reprovada

Nao pode ser aprovada nesta etapa.

## 32. Tratamento de instancia cancelada, expirada ou substituida

Nao pode ser aprovada nesta etapa.

## 33. Compatibilidade com aprovacao simples

Implementada.

## 34. Compatibilidade limitada com fluxo sequencial

Implementada apenas no nivel de aprovacao de etapa e consolidacao simples quando nao restarem etapas obrigatorias pendentes.

## 35. Compatibilidade limitada com fluxo paralelo

Implementada apenas no nivel estrutural. Nao existe consolidacao real por ramo nem por quorum.

## 36. Compatibilidade limitada com fluxo multinivel

Implementada apenas no nivel estrutural. Nao existe orquestracao completa entre niveis.

## 37. Garantias de ausencia de rejeicao/cancelamento/expiracao funcional

O item 40 cria somente decisao positiva. Nenhuma decisao negativa, cancelamento ou expiracao foi implementada.

## 38. Testes criados

Foi criada a suite `AprovarAprovacaoChamadoUseCaseTests` cobrindo:

- aprovacao de instancia simples;
- aprovacao de etapa sem consolidacao total;
- aprovacao da ultima etapa obrigatoria com consolidacao;
- instancia inexistente;
- instancia cancelada;
- etapa de outra instancia;
- decisao final duplicada;
- tentativa de aprovacao direta com etapa obrigatoria pendente.

## 39. Riscos de seguranca e governanca

- aprovador resolvido sem validacao futura de autoridade real por grupo ou delegacao;
- consolidacao simples ainda nao cobre quorum e ramos paralelos;
- risco de consumidores assumirem que `LiberaAvanco` muda status do chamado automaticamente, o que nao acontece.

## 40. Decisoes adiadas para proximos itens

- rejeicao funcional;
- cancelamento funcional;
- expiracao funcional;
- reavaliacao funcional;
- workflow sequencial/paralelo/multinivel completo;
- quorum;
- grupo aprovador real;
- delegacao;
- endpoints;
- frontend.

## 41. Conclusao tecnica

O item 40 fecha o primeiro ciclo funcional positivo do motor novo: gerar instancia, bloquear acao sensivel enquanto pendente e registrar aprovacao controlada com rastreabilidade.

## 42. Proxima etapa recomendada

Executar o item 41: criar regra para rejeitar chamado.
