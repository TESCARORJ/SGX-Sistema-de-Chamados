# Sprint 4 - Regra de Aprovacao por Custo ou Risco
## 1. Objetivo da definicao
Definir conceitualmente como custo e risco devem influenciar a exigencia de aprovacao no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra por custo ou risco.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de campo de custo, campo de risco, enum novo ou alteracao de enum existente.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, prioridade, SLA ou status do chamado.
- Nao houve alteracao no fluxo atual de abertura, atendimento ou aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto de custo e risco no sistema atual
- O SGX ja trata aprovacao, impacto, urgencia, prioridade e SLA no fluxo de chamados.
- O projeto tambem possui documentacao institucional que cita risco, custo, compliance, seguranca e impacto operacional como direcoes futuras de governanca.
- No entanto, o fluxo atual de chamados ainda nao usa custo ou risco como criterio estruturado de aprovacao formal.
- Nesta etapa, custo e risco entram como definicao conceitual para o futuro motor, preservando compatibilidade com a base atual.
## 4. Representacao atual de custo, se existir
- Nao foi identificado campo estruturado de custo no `Chamado`.
- Nao foi identificado campo estruturado de custo em `CatalogoServico`.
- Nao foi identificado campo estruturado de custo em `TipoSolicitacao`.
- O sistema possui referencias documentais a compra, fornecedor, centro de custo e impacto financeiro, mas ainda nao existe modelo de aprovacao por custo no chamado.
## 5. Representacao atual de risco, se existir
- Nao foi identificado campo estruturado de risco no `Chamado`.
- Nao foi identificado campo estruturado de risco em `CatalogoServico`.
- Nao foi identificado campo estruturado de risco em `TipoSolicitacao`.
- Existem referencias conceituais a risco em documentacao ITSM, mudanca, compliance e seguranca, mas nao como gatilho estruturado no fluxo atual de aprovacao do chamado.
## 6. Lacuna caso custo ou risco ainda nao existam como campos estruturados
- Custo e risco ainda nao existem como atributos formais do motor de aprovacao no estado atual do sistema.
- Isso significa que a governanca futura precisara definir:
  - origem dos dados de custo e risco;
  - nivel de granularidade esperado;
  - momento de captura ou recalculo;
  - relacao entre custo/risco declarado e aprovacao gerada.
- Nesta etapa, a lacuna fica documentada sem introduzir mudanca estrutural.
## 7. Conceito de custo relevante
Custo relevante e qualquer gasto, consumo de orcamento, contratacao, compra, licenca, recurso pago, deslocamento, uso de fornecedor, aquisicao, renovacao, ampliacao de capacidade ou impacto financeiro que precise de autorizacao antes da execucao.
## 8. Conceito de risco relevante
Risco relevante e qualquer possibilidade de impacto operacional, indisponibilidade, falha em servico critico, exposicao de dados, violacao de seguranca, descumprimento de compliance, perda financeira, alteracao em ambiente produtivo, excecao de processo ou dano a continuidade do negocio.
## 9. Relacao entre custo/risco e motor de aprovacao
A regra por custo ou risco deve ser tratada como entrada forte de decisao do motor.
Custo e risco:
1. nao devem ser tratados apenas como informacao auxiliar;
2. podem gerar apenas sinalizacao quando elevarem a criticidade sem exigir decisao previa;
3. devem gerar aprovacao impeditiva quando representarem necessidade de autorizacao formal, aceite gerencial, controle orcamentario, seguranca, compliance ou exposicao operacional relevante.
## 10. Diferenca entre criticidade, sinalizacao e aprovacao formal
- Criticidade indica que o chamado merece atencao operacional, tecnica ou gerencial.
- Sinalizacao indica que o contexto deve aparecer para acompanhamento, rastreabilidade ou supervisao, sem bloquear automaticamente.
- Aprovacao formal indica que a execucao de uma acao sensivel nao deve prosseguir sem decisao registrada.
- Nem toda criticidade vira bloqueio, mas custo ou risco relevantes podem ultrapassar o limiar de mera criticidade e exigir autorizacao.
## 11. Cenarios sem exigencia de aprovacao
Exemplos conceituais:
- Atendimento sem custo.
- Suporte comum ao usuario.
- Correcao simples sem alteracao de ambiente.
- Solicitacao sem compra, licenca, fornecedor ou recurso pago.
- Risco baixo e controlado por procedimento padrao.
- Incidente simples resolvido por acao operacional comum.
- Requisicao comum ja prevista em rotina autorizada.
## 12. Cenarios com apenas sinalizacao
Exemplos conceituais:
- Custo baixo dentro de limite pre-aprovado.
- Uso de recurso interno com impacto financeiro indireto.
- Risco medio controlado por procedimento documentado.
- Servico com possivel impacto futuro, mas sem execucao sensivel imediata.
- Solicitacao que pode gerar custo posterior, mas ainda esta em analise.
- Risco operacional moderado com plano de reversao simples.
- Mudanca de baixo risco em janela autorizada.
## 13. Cenarios que devem elevar para aprovacao impeditiva
Exemplos conceituais:
- Requisicao com custo relevante.
- Compra de equipamento, licenca, servico ou contratacao.
- Renovacao ou contratacao de fornecedor.
- Ampliacao de capacidade com custo.
- Solicitacao com impacto financeiro direto.
- Liberacao de recurso restrito com custo.
- Execucao com risco operacional alto.
- Alteracao em ambiente produtivo com risco.
- Mudanca sem plano de reversao claro.
- Acao com risco de indisponibilidade de servico critico.
- Acesso a dados sensiveis ou risco de exposicao de informacao.
- Excecao de processo normal.
- Acao relacionada a compliance, auditoria obrigatoria ou seguranca da informacao.
- Chamado cuja execucao exige aceite formal do gestor, dono do servico ou responsavel financeiro.
## 14. Regra conceitual na abertura do chamado
- Se custo e risco forem inexistentes, baixos ou ja cobertos por rotina autorizada, o motor deve retornar `Permitido` ou `PermitidoComSinalizacao`.
- Se custo ou risco forem relevantes, o motor deve retornar `RequerGeracaoDeAprovacao`.
- Custo ou risco baixos nao removem exigencia impeditiva ja definida por natureza, tipo ou servico sensivel.
## 15. Regra conceitual na alteracao de custo ou risco
- Elevacao para cenario sensivel: `RequerGeracaoDeAprovacao` ou `RequerReavaliacaoDeAprovacao`.
- Aumento de custo dentro de faixa previamente autorizada pode gerar apenas sinalizacao.
- Aumento de risco controlado por procedimento documentado pode manter o chamado em sinalizacao, conforme contexto.
- Reducao de custo ou risco nao cancela automaticamente aprovacao ja aberta; essa definicao fica para itens futuros.
## 16. Regra conceitual para reavaliacao de aprovacao
O motor deve prever `RequerReavaliacaoDeAprovacao` quando:
- o chamado passar a envolver gasto antes inexistente;
- o custo ultrapassar limite antes considerado toleravel;
- a solucao proposta passar a exigir fornecedor, licenca, compra ou ampliacao de capacidade;
- o risco operacional passar a afetar servico critico, seguranca, compliance ou producao;
- a aprovacao anterior deixar de cobrir o novo contexto financeiro ou operacional.
## 17. Relacao com natureza ITSM
- Custo e risco complementam a natureza.
- Podem elevar `Incidente`, `Requisicao`, `Problema`, `EventoAlerta` e `TarefaOperacional` para sinalizacao ou aprovacao impeditiva.
- Nao devem reduzir exigencia impeditiva ja definida por `Mudanca`.
## 18. Relacao com tipo de chamado
- Custo e risco complementam o tipo.
- Se o tipo ja for sensivel, custo e risco podem reforcar a exigencia.
- Se o tipo for comum, custo e risco podem elevar o caso para aprovacao impeditiva.
## 19. Relacao com servico sensivel
- Servico sensivel e custo/risco se reforcam mutuamente.
- Um servico sensivel com custo ou risco relevante e forte candidato a aprovacao impeditiva.
- Um servico comum tambem pode exigir aprovacao se custo ou risco relevantes aparecerem durante a abertura ou atendimento.
## 20. Relacao com impacto e urgencia
- Impacto e urgencia priorizam o atendimento e influenciam SLA.
- Custo e risco indicam necessidade de decisao formal.
- Alta urgencia nao elimina aprovacao quando houver custo ou risco relevante.
- Impacto baixo nao elimina aprovacao se houver gasto relevante, risco operacional alto ou exigencia de aceite.
## 21. Relacao com AprovacaoChamado
- A regra por custo ou risco deve gerar ou consultar uma instancia de `AprovacaoChamado` quando o cenario for classificado como impeditivo.
- Devem ser preservados `ChamadoId`, status, origem, decisao, historico e auditoria.
- O motor interpreta custo e risco como justificadores de governanca, sem substituir a instancia formal de aprovacao.
## 22. Relacao com BloqueiaAvancoAtendimento
- Para custo ou risco impeditivos, a aprovacao futura deve poder usar `BloqueiaAvancoAtendimento` como ponte de compatibilidade.
- O conceito nao deve ficar restrito a bloqueio total.
- Custo e risco podem resultar em:
  - bloqueio total de avancar atendimento;
  - bloqueio apenas de acoes sensiveis;
  - apenas sinalizacao sem bloqueio.
## 23. Relacao com AguardandoAprovacao
- Cenarios de custo ou risco relevantes podem levar o chamado a `AguardandoAprovacao`.
- Isso nao deve ser obrigatorio em todos os cenarios.
- O motor deve poder bloquear acoes especificas sem depender exclusivamente da troca de status.
## 24. Compatibilidade com fluxo atual
- O fluxo atual de aprovacao continua baseado em `AprovacaoChamado` e nas regras existentes de bloqueio.
- O conceito proposto preserva essa base e adiciona custo/risco como futuros gatilhos de governanca.
- Como ainda nao existem campos estruturados de custo e risco, nenhuma regra funcional nova foi introduzida nesta etapa.
## 25. Lacunas encontradas
- Ausencia de campo estruturado de custo no `Chamado`.
- Ausencia de campo estruturado de risco no `Chamado`.
- Ausencia de classificacao de custo e risco em `CatalogoServico`.
- Ausencia de reavaliacao automatica quando a solucao proposta elevar custo ou risco durante o atendimento.
- Ausencia de matriz institucional de limites, faixas e alcas de aprovacao por custo ou aceite de risco.
## 26. Riscos de compatibilidade
- Tratar qualquer custo pequeno como aprovacao obrigatoria pode burocratizar chamados comuns.
- Tratar qualquer risco operacional como bloqueio pode conflitar com a agilidade exigida em incidentes e atendimentos de rotina.
- A governanca futura precisa separar custo/risco relevante de variacoes operacionais normais para evitar falsos positivos.
## 27. Decisoes adiadas para proximos itens
- Conceito de aprovador padrao.
- Conceito de grupo aprovador.
- Regra final de bloqueio por decisao pendente.
- Regra de liberacao apos aprovacao.
- Regra de rejeicao, cancelamento e expiracao.
- Tratamento da reducao de sensibilidade quando custo ou risco diminuirem.
- Definicao estrutural futura de campos, limites e matrizes de aceite financeiro ou operacional.
## 28. Conclusao tecnica
Custo e risco devem entrar no futuro motor de aprovacao ITSM como entradas fortes de governanca. Quando relevantes, deixam de ser apenas informacao de criticidade e passam a representar necessidade de autorizacao formal antes da execucao. Ao mesmo tempo, o conceito proposto preserva proporcionalidade: custo ou risco baixos nao devem bloquear automaticamente chamados comuns.
## 29. Proxima etapa recomendada
Executar o item 11 do checklist da Sprint 4: definir conceito de aprovador padrao.
