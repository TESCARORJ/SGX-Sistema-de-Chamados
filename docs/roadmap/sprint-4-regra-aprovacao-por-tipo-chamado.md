# Sprint 4 - Regra de Aprovação por Tipo de Chamado

## 1. Objetivo da definição

Definir conceitualmente como o tipo de chamado deve influenciar a exigencia de aprovacao no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados.

## 2. Limites desta etapa

- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra por tipo de chamado.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de enum novo nem alteracao de enum existente.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no status do chamado.
- Nao houve alteracao no fluxo atual de abertura, atendimento ou aprovacao.
- Nao houve homologacao nem aceite final.

## 3. Contexto do tipo de chamado no sistema atual

- O sistema nao possui hoje um enum dedicado chamado `TipoChamadoEnum`.
- O papel mais proximo de "tipo de chamado" e exercido por `TipoSolicitacao`.
- Essa classificacao convive com outros eixos operacionais:
  - `NaturezaChamado`;
  - `CategoriaChamado`;
  - `SubcategoriaChamado`;
  - `CatalogoServico`.
- `NaturezaChamado` ja governa processo ITSM.
- `TipoSolicitacao` funciona como refinamento operacional e administrativo.
- Catalogo, categoria e subcategoria ajudam a especializar o contexto, mas nao substituem o tipo.

## 4. Tipos de chamado identificados no codigo

O sistema atual usa `TipoSolicitacao` como cadastro administrativo livre, sem conjunto fechado por enum.

Tipos explicitamente identificados na documentacao e seed:

1. `Incidente`
2. `Solicitacao de Servico`
3. `Duvida`
4. `Melhoria`
5. `Problema Recorrente`

Observacao:

- como `TipoSolicitacao` e cadastro administrativo, o conjunto pode ser ampliado institucionalmente;
- por isso, a regra conceitual precisa trabalhar por grupos semanticos, e nao por lista rigida de valores.

## 5. Relacao entre tipo de chamado e natureza ITSM

- `NaturezaChamado` continua sendo o contexto principal do processo ITSM.
- `TipoSolicitacao` deve ser tratado como refinador operacional desse contexto.
- O tipo nao substitui a natureza.
- O tipo complementa a analise do motor em cenarios onde a natureza sozinha ainda e muito ampla.

Conceitualmente:

- a natureza responde "que processo ITSM e este?";
- o tipo ajuda a responder "qual variante operacional deste processo esta sendo tratada?".

## 6. Relacao entre tipo de chamado e motor de aprovacao

A regra por tipo de chamado deve ser a segunda entrada de decisao do motor, aplicada apos ou junto da natureza.

Ela deve permitir que o motor:

1. mantenha a decisao herdada da natureza;
2. eleve a sensibilidade para aprovacao impeditiva;
3. gere apenas sinalizacao;
4. mantenha o chamado livre de aprovacao.

## 7. Classificacao conceitual dos tipos

Como o sistema usa `TipoSolicitacao` parametrizavel, a classificacao conceitual proposta e por grupos:

### Grupo 1 - Tipos que herdam a regra da natureza

- `Incidente`
- `Solicitacao de Servico` comum
- `Melhoria` de baixa criticidade
- atendimento informativo
- registro operacional comum

### Grupo 2 - Tipos que elevam para aprovacao impeditiva

- solicitacao de acesso
- liberacao de permissao privilegiada
- alteracao de perfil de usuario
- requisicao com custo
- requisicao de compra
- mudanca emergencial
- mudanca em ambiente produtivo
- alteracao de configuracao critica
- liberacao de recurso restrito
- execucao que impacta servico critico
- solicitacao com risco operacional relevante

### Grupo 3 - Tipos que geram apenas sinalizacao

- `Problema Recorrente`
- incidente recorrente
- problema em analise
- evento monitorado
- tarefa operacional monitorada
- atendimento consultivo
- solicitacao com possivel impacto futuro

### Grupo 4 - Tipos sem exigencia de aprovacao

- `Duvida`
- informacao
- solicitacao simples
- ajuste operacional simples
- incidente simples sem alteracao de ambiente
- registro sem acao sensivel

## 8. Tipos que herdam a regra da natureza

Esses tipos nao aumentam nem reduzem a sensibilidade do chamado.

Exemplos conceituais:

- `Incidente` comum sobre natureza `Incidente`;
- `Solicitacao de Servico` comum sobre natureza `Requisicao`;
- tarefa operacional rotineira sobre natureza `TarefaOperacional`;
- melhoria leve sem custo ou risco adicional.

Nesses casos:

- o motor deve priorizar a decisao ja herdada da natureza;
- o tipo atua como classificacao descritiva, nao como gatilho autonomo de aprovacao.

## 9. Tipos que elevam para aprovacao impeditiva

Esses tipos devem poder elevar a exigencia mesmo quando a natureza original nao for bloqueante por si so.

Casos conceituais:

- solicitacao de acesso;
- liberacao de privilegio elevado;
- alteracao de perfil ou permissao;
- requisicao com custo ou compra;
- mudanca emergencial;
- mudanca em ambiente produtivo;
- alteracao de configuracao critica;
- liberacao de recurso restrito;
- execucao com impacto em servico critico;
- atividade com risco operacional relevante.

Nesses casos, o motor deve poder retornar:

- `RequerGeracaoDeAprovacao`;
- `RequerReavaliacaoDeAprovacao`;
- `BloqueadoPorAprovacaoPendente`;
- `BloqueadoPorAprovacaoReprovada`.

## 10. Tipos que geram apenas sinalizacao

Esses tipos devem aumentar a atencao de governanca, mas sem impor bloqueio obrigatorio por padrao.

Exemplos:

- `Problema Recorrente`;
- incidente recorrente;
- atendimento consultivo;
- tarefa monitorada;
- evento ou alerta em acompanhamento;
- solicitacao que pode exigir decisao posterior, mas nao previa.

Nesses casos, a resposta conceitual tipica e `PermitidoComSinalizacao`.

## 11. Tipos sem exigencia de aprovacao

Esses tipos permanecem livres por padrao quando nao houver combinacao com servico sensivel, custo, risco, impacto ou urgencia relevantes.

Exemplos:

- `Duvida`;
- informacao;
- solicitacao simples;
- ajuste operacional simples;
- incidente simples sem alteracao de ambiente;
- registro administrativo sem acao sensivel.

## 12. Regra conceitual na abertura do chamado

Na abertura:

1. se o tipo apenas herdar a regra da natureza, o motor deve manter a decisao ja produzida pela natureza;
2. se o tipo elevar a sensibilidade, o motor deve retornar `RequerGeracaoDeAprovacao` quando ainda nao houver aprovacao adequada;
3. se o tipo for apenas de sinalizacao, o motor deve poder retornar `PermitidoComSinalizacao`;
4. se o tipo for livre, o motor deve manter `Permitido`, salvo combinacoes futuras com servico, custo, risco, impacto ou urgencia.

Compatibilidade com o sistema atual:

- hoje o portal ja envia `TipoSolicitacaoId`;
- o backend ja valida se o tipo esta ativo;
- a nova regra conceitual deve aproveitar essa classificacao sem alterar o fluxo atual.

## 13. Regra conceitual na alteracao do tipo

Quando o tipo mudar:

1. de comum para sensivel:
   - o motor deve indicar `RequerGeracaoDeAprovacao` ou `RequerReavaliacaoDeAprovacao`;
2. de herdado para tipo impeditivo:
   - o motor deve elevar a decisao mesmo que a natureza original fosse livre;
3. de impeditivo para comum:
   - fica pendente decidir se a aprovacao sera cancelada, mantida como historico ou apenas deixara de bloquear;
4. de tipo comum para tipo apenas sinalizado:
   - o motor pode acrescentar visibilidade de governanca sem bloquear a acao.

## 14. Regra conceitual para reavaliacao de aprovacao

A reavaliacao deve ser prevista quando:

- o tipo for alterado para variante mais sensivel;
- o tipo aprovado anteriormente deixar de refletir o contexto atual do chamado;
- a alteracao do tipo vier acompanhada de mudanca de servico, custo, risco, impacto ou urgencia.

Nesses casos, a resposta conceitual esperada do motor e `RequerReavaliacaoDeAprovacao`.

## 15. Relacao com servico sensivel

- O tipo nao substitui a regra por servico sensivel.
- Um tipo comum pode se tornar impeditivo quando vinculado a servico sensivel.
- Um tipo impeditivo pode ser reforcado por um servico ja classificado como sensivel.
- A regra especifica por servico continua adiada para o item 8.

## 16. Relacao com impacto e urgencia

- Impacto e urgencia funcionam como moduladores da classificacao por tipo.
- Um tipo apenas informativo pode exigir revisao em cenarios de alto impacto e alta urgencia.
- Um tipo livre pode ser elevado quando combinado com urgencia e impacto extremos.
- A regra detalhada fica para o item 9.

## 17. Relacao com custo e risco

- Custo e risco podem transformar um tipo normalmente livre em impeditivo.
- Requisicoes de compra, acesso privilegiado e alteracoes criticas sao os principais exemplos.
- A regra detalhada por custo e risco fica para o item 10.

## 18. Relacao com `AprovacaoChamado`

A regra por tipo de chamado deve gerar ou consultar uma instancia de `AprovacaoChamado`, preservando:

- vinculo com `ChamadoId`;
- status da aprovacao;
- origem da aprovacao;
- decisao registrada;
- historico e auditoria.

Conceitualmente:

- o tipo e uma das entradas que justificam criar, consultar ou reavaliar a aprovacao;
- `AprovacaoChamado` continua sendo a base persistente da instancia.

## 19. Relacao com `BloqueiaAvancoAtendimento`

- Para tipos impeditivos, a aprovacao futura deve poder usar `BloqueiaAvancoAtendimento` como ponte de compatibilidade.
- Nem todo tipo sensivel precisa bloquear todo o fluxo.
- O conceito deve suportar:
  - bloqueio total de avancos operacionais;
  - bloqueio apenas de acoes sensiveis;
  - apenas sinalizacao.

## 20. Relacao com `AguardandoAprovacao`

- Tipos impeditivos podem levar o chamado a `AguardandoAprovacao`, especialmente quando combinados com `Mudanca`.
- Esse status nao deve ser obrigatorio em todos os cenarios.
- O motor deve poder bloquear acoes especificas sem depender exclusivamente da troca de status.

## 21. Compatibilidade com fluxo atual

Para preservar compatibilidade:

1. `TipoSolicitacao` deve ser tratado como proxy atual de tipo de chamado, sem criar estrutura nova nesta etapa;
2. o fluxo de abertura com `TipoSolicitacaoId` deve permanecer intacto;
3. aprovacoes automaticas por catalogo nao devem ser invalidadas pela regra por tipo;
4. tipos informativos nao devem virar bloqueantes automaticamente;
5. categoria, subcategoria e catalogo continuam como classificacoes complementares, nao como substitutos do tipo.

## 22. Lacunas encontradas

1. O sistema nao possui hoje um enum dedicado de tipo de chamado.
2. `TipoSolicitacao` e cadastro livre, o que exige regra por grupo semantico e nao por lista fechada.
3. Nao existe no fluxo atual reavaliacao automatica quando `TipoSolicitacaoId` muda.
4. O catalogo pode especializar bastante a solicitacao, mas isso ainda nao esta consolidado como regra de aprovacao por tipo.

## 23. Riscos de compatibilidade

1. Tratar todo `TipoSolicitacao` como gatilho forte de aprovacao pode bloquear chamadas comuns demais.
2. Ignorar `TipoSolicitacao` e depender apenas da natureza manteria o motor generico demais.
3. Misturar papel de tipo com categoria ou subcategoria pode gerar regra difusa e pouco rastreavel.
4. Fixar lista rigida de tipos pode quebrar ambientes que usem nomes administrativos diferentes.

## 24. Decisoes adiadas para proximos itens

Ficam adiadas:

1. regra de aprovacao por servico sensivel;
2. regra por impacto e urgencia;
3. regra por custo ou risco;
4. regra de bloqueio por decisao pendente;
5. regra de liberacao apos aprovacao;
6. regra de rejeicao, cancelamento e expiracao;
7. criterios exatos de nomenclatura institucional para grupos semanticos de `TipoSolicitacao`;
8. decisao sobre o tratamento quando o tipo for reduzido de sensivel para comum.

## 25. Conclusao tecnica

O tipo de chamado deve entrar no futuro motor de aprovacao como refinador operacional da natureza ITSM. No estado atual do SGX, esse papel e exercido principalmente por `TipoSolicitacao`, complementado por catalogo, categoria e subcategoria. A regra conceitual definida nesta etapa estabelece que alguns tipos apenas herdam a natureza, alguns elevam o chamado para aprovacao impeditiva, outros geram somente sinalizacao e outros permanecem livres, sem substituir a natureza como eixo principal do processo ITSM.

## 26. Proxima etapa recomendada

Definir a regra de aprovacao por servico sensivel, separando:

- o que nasce da classificacao do servico;
- o que apenas reforca tipo e natureza;
- o que deve disparar aprovacao mesmo em chamados operacionais comuns.
