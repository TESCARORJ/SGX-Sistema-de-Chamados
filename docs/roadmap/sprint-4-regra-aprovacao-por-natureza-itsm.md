# Sprint 4 - Regra de Aprovação por Natureza ITSM

## 1. Objetivo da definição

Definir conceitualmente como a natureza ITSM do chamado deve influenciar a exigencia de aprovacao no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados.

## 2. Limites desta etapa

- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra por natureza.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de enum novo nem alteracao de enum existente.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no status do chamado.
- Nao houve alteracao no fluxo atual de abertura, atendimento ou aprovacao.
- Nao houve homologacao nem aceite final.

## 3. Contexto da natureza ITSM no sistema atual

- `NaturezaChamadoEnum` ja estrutura o chamado por processo ITSM.
- A natureza ja influencia:
  - campos obrigatorios na abertura;
  - fluxo de status permitido;
  - acoes administrativas disponiveis;
  - leitura operacional do atendimento.
- O sistema ja distingue hoje seis naturezas operacionais, mas ainda nao possui regra centralizada de aprovacao disparada pela natureza em si.
- A aprovacao atual existe principalmente por catalogo de servicos e por solicitacao manual, nao por classificacao formal da natureza.

## 4. Naturezas ITSM identificadas no codigo

As naturezas atuais identificadas em `NaturezaChamadoEnum` sao:

1. `Incidente`
2. `Requisicao`
3. `Mudanca`
4. `Problema`
5. `EventoAlerta`
6. `TarefaOperacional`

## 5. Relacao entre natureza ITSM e motor de aprovacao

A natureza ITSM deve ser tratada como uma entrada inicial de governanca do motor, e nao como decisao isolada.

Conceitualmente:

- a natureza indica o nivel inicial de sensibilidade do chamado;
- o motor usa essa classificacao para decidir se a aprovacao tende a ser impeditiva, informativa ou dispensavel;
- a decisao final do motor continua dependente do contexto adicional do chamado, como servico, impacto, urgencia, custo, risco e acao solicitada.

## 6. Classificacao conceitual das naturezas

Nesta etapa, a classificacao conceitual proposta fica assim:

### Grupo 1 - Naturezas com aprovacao impeditiva

- `Mudanca`

### Grupo 2 - Naturezas com apenas sinalizacao

- `Problema`
- `EventoAlerta`
- `TarefaOperacional`

### Grupo 3 - Naturezas sem exigencia de aprovacao por padrao

- `Incidente`
- `Requisicao`

Observacao importante:

- a classificacao acima e a base padrao por natureza;
- `Requisicao` e `Incidente` podem se tornar bloqueantes em itens futuros quando combinados com servico sensivel, acesso, custo, risco, impacto ou urgencia;
- a natureza sozinha nao encerra a decisao do motor.

## 7. Naturezas com aprovacao impeditiva

### `Mudanca`

`Mudanca` deve ser a natureza com aprovacao impeditiva por padrao porque:

- ja possui status especificos como `AguardandoAprovacao`, `Aprovada` e `Reprovada`;
- representa alteracao controlada de ambiente ou servico;
- pode envolver risco operacional, janela de execucao, impacto em servico critico e necessidade de autorizacao formal.

Regra conceitual base:

- na abertura, `Mudanca` deve levar o motor a indicar `RequerGeracaoDeAprovacao` quando ainda nao existir aprovacao relacionada;
- se houver aprovacao pendente, o motor deve poder retornar `BloqueadoPorAprovacaoPendente` para acoes operacionais sensiveis;
- se houver reprovacao, o motor deve retornar `BloqueadoPorAprovacaoReprovada` para as acoes dependentes;
- se houver aprovacao valida, o motor pode permitir continuidade, sem dispensar regras futuras de risco, custo e servico.

## 8. Naturezas com apenas sinalizacao

### `Problema`

Deve gerar sinalizacao de governanca, rastreabilidade e acompanhamento reforcado, porque:

- tende a envolver analise de causa raiz e recorrencia;
- pode demandar aprovacao futura em cenarios derivados, mas nao exige decisao formal previa em todos os casos.

### `EventoAlerta`

Deve gerar sinalizacao porque:

- pode representar contexto monitorado e operacionalmente sensivel;
- nem todo evento ou alerta precisa de aprovacao antes do tratamento inicial.

### `TarefaOperacional`

Deve gerar sinalizacao por padrao porque:

- pode incluir execucoes planejadas ou rotineiras;
- nem toda tarefa operacional exige autorizacao formal previa;
- parte desse universo pode migrar para bloqueio futuro quando houver relacao com servico sensivel, custo ou risco.

## 9. Naturezas sem exigencia de aprovacao

### `Incidente`

Nao deve exigir aprovacao por padrao porque:

- o tratamento inicial de incidente costuma privilegiar restauracao rapida do servico;
- a aprovacao pode ser excecao futura em incidentes com alto impacto, acesso privilegiado ou mudanca emergencial embutida.

### `Requisicao`

Nao deve exigir aprovacao por padrao porque:

- muitas requisicoes sao operacionais e recorrentes;
- a necessidade de aprovacao tende a nascer mais do servico solicitado do que da natureza abstrata.

## 10. Regra conceitual na abertura do chamado

Na abertura:

1. se a natureza estiver no grupo impeditivo, o motor deve indicar `RequerGeracaoDeAprovacao` quando ainda nao existir instancia adequada de aprovacao;
2. se a aprovacao ja existir e estiver pendente, o motor deve poder indicar `BloqueadoPorAprovacaoPendente` para acoes que dependam daquela decisao;
3. se a natureza estiver no grupo de sinalizacao, o motor deve permitir a abertura com `PermitidoComSinalizacao`;
4. se a natureza estiver no grupo sem exigencia, o motor deve retornar `Permitido`, salvo combinacoes futuras com servico, risco, custo, impacto ou urgencia.

Compatibilidade com o estado atual:

- hoje a abertura automatica de aprovacao acontece por catalogo de servicos;
- a regra por natureza deve ser adicionada futuramente sem quebrar essa base existente.

## 11. Regra conceitual na alteracao da natureza

Quando a natureza for alterada:

1. de livre para impeditiva:
   - o motor deve indicar `RequerGeracaoDeAprovacao` ou `RequerReavaliacaoDeAprovacao`;
2. de sinalizacao para impeditiva:
   - o motor deve indicar necessidade de reavaliar o chamado antes de seguir com acoes sensiveis;
3. de impeditiva para sinalizacao ou livre:
   - a decisao sobre cancelar, manter como historico ou reaproveitar a aprovacao atual fica adiada para itens futuros;
4. entre naturezas do mesmo grupo:
   - o motor deve poder sinalizar reavaliacao se houver mudanca material de contexto operacional.

## 12. Regra conceitual para reavaliacao de aprovacao

A reavaliacao deve ser prevista quando:

- a natureza mudar para uma classificacao mais sensivel;
- a natureza originalmente aprovada deixar de representar o contexto atual do chamado;
- a mudanca de natureza vier acompanhada de alteracao de servico, impacto, urgencia, custo ou risco.

Nesses casos, a resposta conceitual esperada do motor e `RequerReavaliacaoDeAprovacao`.

## 13. Relacao com servico sensivel

- A natureza nao substitui a regra por servico.
- `Requisicao` e `Incidente`, mesmo livres por padrao, podem exigir aprovacao quando vinculados a servico sensivel.
- `Mudanca` vinculada a servico sensivel tende a reforcar bloqueio impeditivo.
- A regra por servico continua adiada para o item 8 do checklist.

## 14. Relacao com impacto e urgencia

- Impacto e urgencia funcionam como moduladores da decisao do motor.
- Naturezas livres ou informativas podem se tornar mais restritivas em combinacoes de alto impacto e alta urgencia.
- Nesta etapa, a natureza apenas define a inclinacao inicial da governanca; a regra detalhada por impacto e urgencia fica para o item 9.

## 15. Relacao com custo e risco

- Custo e risco podem elevar a severidade da exigencia de aprovacao mesmo em naturezas nao bloqueantes por padrao.
- `Mudanca` e o principal caso em que natureza e risco tendem a se somar.
- A regra detalhada por custo e risco fica adiada para o item 10.

## 16. Relacao com `AprovacaoChamado`

A regra por natureza deve gerar ou consultar uma instancia de `AprovacaoChamado`, preservando:

- vinculo com `ChamadoId`;
- status da aprovacao;
- origem da aprovacao;
- decisao registrada;
- historico e auditoria.

Conceitualmente:

- `AprovacaoChamado` continua sendo a base persistente;
- a natureza passa a ser um dos disparadores conceituais que justificam sua criacao, consulta ou reavaliacao.

## 17. Relacao com `BloqueiaAvancoAtendimento`

- Para naturezas impeditivas, a aprovacao futura deve poder usar `BloqueiaAvancoAtendimento` como mecanismo de compatibilidade do bloqueio simples atual.
- Nem toda aprovacao disparada por natureza precisa bloquear todo o fluxo.
- O conceito documentado deve suportar tres cenarios:
  - bloqueio total de avancos operacionais;
  - bloqueio apenas de acoes sensiveis;
  - sinalizacao sem bloqueio.

## 18. Relacao com `AguardandoAprovacao`

- `AguardandoAprovacao` e aderente principalmente a `Mudanca`, mas nao deve ser obrigatorio em todos os cenarios.
- O motor deve poder bloquear acoes especificas mesmo sem exigir mudanca imediata de status.
- O uso desse status deve permanecer como representacao operacional possivel, nao como unico mecanismo de controle.

## 19. Compatibilidade com fluxo atual

Para preservar compatibilidade:

1. a natureza nao pode invalidar a aprovacao automatica ja existente por catalogo;
2. aprovacoes informativas atuais nao devem virar bloqueantes automaticamente;
3. o fluxo atual de abertura e atendimento deve continuar funcionando sem a implementacao dessa regra;
4. `Mudanca` ja possui ecossistema de status mais aderente ao futuro motor e deve ser a principal ponte evolutiva;
5. a classificacao por natureza deve se acoplar ao fluxo atual por interpretacao conceitual antes de qualquer refatoracao estrutural.

## 20. Lacunas encontradas

1. A natureza hoje influencia status, campos e acoes, mas nao a aprovacao.
2. `Mudanca` possui status de aprovacao no fluxo, mas ainda sem motor real de aprovacao por natureza.
3. `Requisicao` e `Incidente` nao distinguem por si so requisicoes sensiveis ou acessos privilegiados.
4. Nao existe hoje reavaliacao automatica quando a natureza e alterada.
5. Nao existe definicao atual para reducao de sensibilidade apos mudanca de natureza.

## 21. Riscos de compatibilidade

1. Tornar toda `Requisicao` bloqueante quebraria o fluxo operacional comum.
2. Tratar toda `Problema` como impeditiva pode atrasar analise tecnica sem ganho proporcional.
3. Acoplar `Mudanca` obrigatoriamente a `AguardandoAprovacao` pode gerar rigidez excessiva.
4. Ignorar combinacoes com servico, risco e custo produziria falso positivo ou falso negativo de aprovacao.

## 22. Decisoes adiadas para proximos itens

Ficam adiadas:

1. regra de aprovacao por tipo de chamado;
2. regra de aprovacao por servico sensivel;
3. regra de aprovacao por impacto e urgencia;
4. regra de aprovacao por custo ou risco;
5. regra de bloqueio por decisao pendente;
6. regra de liberacao apos aprovacao;
7. regra de rejeicao, cancelamento e expiracao;
8. decisao sobre o que fazer quando a natureza for reduzida de impeditiva para livre ou sinalizacao;
9. reavaliacao automatica combinada com alteracao de classificacao operacional detalhada.

## 23. Conclusao tecnica

A natureza ITSM deve entrar no futuro motor de aprovacao como uma regra-base de governanca. Ela nao decide sozinha, mas estabelece o nivel inicial de sensibilidade do chamado. Nesta definicao conceitual, `Mudanca` e a natureza que deve exigir aprovacao impeditiva por padrao, `Problema`, `EventoAlerta` e `TarefaOperacional` devem gerar sinalizacao, e `Incidente` e `Requisicao` permanecem livres por padrao, sem prejuizo de regras futuras mais restritivas por servico, risco, custo, impacto e urgencia.

## 24. Proxima etapa recomendada

Definir a regra de aprovacao por tipo de chamado, separando:

- classificacao operacional que herda da natureza;
- classificacao que eleva a sensibilidade do chamado;
- combinacoes em que tipo e natureza devem disparar ou reavaliar aprovacao juntos.
