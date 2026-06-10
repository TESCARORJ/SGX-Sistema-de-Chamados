# Sprint 4 - Regra de Aprovacao por Impacto e Urgencia
## 1. Objetivo da definicao
Definir conceitualmente como impacto e urgencia devem influenciar a exigencia de aprovacao no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra por impacto e urgencia.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de enum novo nem alteracao de enum existente.
- Nao houve alteracao em AprovacaoChamado, BloqueiaAvancoAtendimento, prioridade calculada ou regras de SLA.
- Nao houve alteracao no fluxo atual de abertura, atendimento ou aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto de impacto e urgencia no sistema atual
- O SGX ja trata impacto e urgencia como conceitos obrigatorios em boa parte do fluxo ITSM.
- Esses campos alimentam a matriz de prioridade do chamado.
- A prioridade calculada repercute na aplicacao do SLA.
- No estado atual, impacto e urgencia orientam priorizacao e prazo, nao governanca de aprovacao formal.
## 4. Representacao atual de impacto
- ImpactoChamadoEnum representa impacto em tres niveis:
  - Baixo
  - Medio
  - Alto
- O impacto compoe o estado operacional do chamado.
- O impacto e persistido no proprio Chamado.
## 5. Representacao atual de urgencia
- UrgenciaChamadoEnum representa urgencia em tres niveis:
  - Baixa
  - Media
  - Alta
- A urgencia compoe o estado operacional do chamado.
- A urgencia e persistida no proprio Chamado.
## 6. Relacao atual com prioridade
- O sistema usa PrioridadeChamadoMatrizService para transformar impacto e urgencia em prioridade.
- A matriz atual e:
| Impacto | Urgencia | Prioridade |
|---|---|---|
| Alto | Alta | Critica |
| Alto | Media | Alta |
| Alto | Baixa | Media |
| Medio | Alta | Alta |
| Medio | Media | Media |
| Medio | Baixa | Baixa |
| Baixo | Alta | Media |
| Baixo | Media | Baixa |
| Baixo | Baixa | Baixa |
- Essa relacao e operacional e voltada a ordenacao do atendimento.
## 7. Relacao atual com SLA
- O SLA e aplicado com base na prioridade efetiva do chamado.
- A prioridade e usada para localizar metas e politicas de SLA.
- Mudancas de prioridade repercutem em recalculo de prazos de SLA.
- O sistema ja separa claramente:
  - impacto e urgencia como insumos da prioridade;
  - prioridade como insumo do SLA.
## 8. Relacao entre impacto/urgencia e motor de aprovacao
A regra por impacto e urgencia deve ser tratada como entrada complementar do motor de aprovacao.
Impacto e urgencia:
1. nao devem ser tratados automaticamente como aprovacao obrigatoria;
2. podem gerar apenas sinalizacao quando indicarem atencao gerencial ou operacional;
3. podem elevar para aprovacao impeditiva quando vierem acompanhados de fator sensivel, como mudanca, acesso, custo, risco, seguranca, compliance ou servico critico.
## 9. Diferenca entre priorizacao e aprovacao
- Priorizacao organiza a ordem e o prazo de atendimento.
- Aprovacao formal autoriza ou bloqueia a execucao de uma acao sensivel.
- Impacto e urgencia, isoladamente, servem primeiro para priorizacao e SLA.
- Aprovacao so deve surgir quando a combinacao impacto/urgencia representar risco decisorio relevante ou reforcar outro gatilho sensivel.
## 10. Combinacoes sem exigencia de aprovacao
Exemplos conceituais:
- Baixo impacto e baixa urgencia.
- Baixo impacto e urgencia media.
- Medio impacto e baixa urgencia.
- Incidente simples com impacto limitado.
- Requisicao comum sem custo, risco, acesso privilegiado ou servico sensivel.
- Atendimento operacional comum, mesmo com prazo curto.
## 11. Combinacoes com apenas sinalizacao
Exemplos conceituais:
- Medio impacto e urgencia alta sem servico sensivel.
- Alto impacto e urgencia baixa sem mudanca, custo ou risco.
- Problema recorrente com impacto crescente.
- Evento de alerta com urgencia alta, mas sem acao sensivel imediata.
- Tarefa operacional com prazo critico, mas sem alteracao de ambiente.
- Incidente com muitos usuarios afetados, mas cuja correcao segue procedimento padrao.
## 12. Combinacoes que podem elevar para aprovacao impeditiva
Exemplos conceituais:
- Alto impacto e urgencia alta em mudanca de producao.
- Alto impacto em servico critico com alteracao de configuracao.
- Urgencia alta para liberacao de acesso privilegiado.
- Alto impacto associado a custo relevante.
- Alto impacto associado a risco operacional.
- Urgencia alta em mudanca emergencial.
- Alto impacto envolvendo dados sensiveis, seguranca ou compliance.
- Chamado urgente que exige excecao de processo normal.
- Chamado que, para resolver rapido, exige autorizacao de gestor, aprovador ou responsavel pelo servico.
## 13. Regra conceitual na abertura do chamado
- Se impacto e urgencia apenas aumentarem a prioridade, o motor deve retornar Permitido ou PermitidoComSinalizacao, conforme o contexto.
- Se impacto e urgencia estiverem combinados com servico sensivel, mudanca, custo, risco, acesso ou seguranca, o motor pode retornar RequerGeracaoDeAprovacao.
- Impacto e urgencia baixos nao removem exigencia impeditiva ja definida por natureza, tipo ou servico.
## 14. Regra conceitual na alteracao de impacto ou urgencia
- Elevacao de impacto ou urgencia para combinacao sensivel: RequerGeracaoDeAprovacao ou RequerReavaliacaoDeAprovacao.
- Aumento isolado de urgencia em chamado comum nao deve gerar aprovacao automaticamente.
- Aumento isolado de impacto pode gerar sinalizacao, mas so deve gerar aprovacao se vier junto de fator decisorio relevante.
- Reducao de impacto ou urgencia nao cancela automaticamente exigencia ja estabelecida; essa definicao fica para itens futuros.
## 15. Regra conceitual para reavaliacao de aprovacao
O motor deve prever RequerReavaliacaoDeAprovacao quando:
- a combinacao passar para alto impacto e alta urgencia com fator sensivel associado;
- a urgencia elevada passar a exigir excecao de processo;
- o impacto elevado passar a representar exposicao relevante em servico critico, custo, risco ou seguranca;
- a aprovacao anterior deixar de cobrir o novo contexto operacional.
## 16. Relacao com natureza ITSM
- Impacto e urgencia complementam a natureza.
- Podem elevar Requisicao, Incidente, Problema, EventoAlerta ou TarefaOperacional para sinalizacao ou aprovacao quando houver combinacao sensivel.
- Nao devem reduzir exigencia impeditiva de Mudanca.
## 17. Relacao com tipo de chamado
- Impacto e urgencia complementam o tipo.
- Se o tipo ja for sensivel, impacto e urgencia podem reforcar a exigencia.
- Se o tipo for comum, impacto e urgencia so devem elevar para aprovacao quando houver fator decisorio relevante.
## 18. Relacao com servico sensivel
- Servico sensivel tem peso forte na decisao.
- Impacto e urgencia podem reforcar a necessidade de aprovacao quando o servico ja for sensivel.
- Servico comum com alta urgencia nao deve virar aprovacao automaticamente sem risco, custo, acesso ou mudanca.
## 19. Relacao com custo e risco
- Impacto e urgencia sao moduladores importantes quando custo ou risco entram no contexto.
- Alto impacto ou alta urgencia podem elevar a necessidade de decisao formal quando houver custo relevante ou risco operacional significativo.
- A regra detalhada de custo e risco fica adiada para o item especifico.
## 20. Relacao com AprovacaoChamado
- A regra por impacto e urgencia deve gerar ou consultar uma instancia de AprovacaoChamado quando a combinacao for classificada como impeditiva.
- Devem ser preservados ChamadoId, status, origem, decisao, historico e auditoria.
- O motor interpreta impacto e urgencia como justificadores contextuais da aprovacao, nao como substitutos da instancia formal.
## 21. Relacao com BloqueiaAvancoAtendimento
- Para combinacoes impeditivas, a aprovacao futura deve poder usar BloqueiaAvancoAtendimento como ponte de compatibilidade.
- A combinacao impacto/urgencia pode resultar em:
  - bloqueio total de avancar atendimento;
  - bloqueio apenas de acoes sensiveis;
  - apenas sinalizacao sem bloqueio.
## 22. Relacao com AguardandoAprovacao
- Combinacoes de impacto e urgencia associadas a fator sensivel podem levar o chamado a AguardandoAprovacao.
- Isso nao deve ser obrigatorio em todos os cenarios.
- O motor deve poder bloquear acoes especificas sem depender exclusivamente da troca de status.
## 23. Compatibilidade com fluxo atual
- O SGX ja usa impacto e urgencia para calcular prioridade e repercutir no SLA.
- O conceito proposto preserva essa funcao original.
- O futuro motor nao deve confundir criticidade operacional com autorizacao formal.
- A nova regra deve complementar o fluxo atual, nao substitui-lo.
## 24. Lacunas encontradas
- O sistema atual nao possui diferenciacao formal entre combinacao critica para priorizacao e combinacao critica para aprovacao.
- Impacto e urgencia ainda nao sao usados como gatilho conceitual de governanca.
- Nao existe reavaliacao automatica quando impacto ou urgencia aumentam durante o atendimento.
## 25. Riscos de compatibilidade
- Elevar impacto e urgencia diretamente para aprovacao pode gerar excesso de bloqueio em chamados comuns.
- Misturar prioridade alta com aprovacao obrigatoria por padrao pode distorcer o comportamento operacional atual.
- A governanca futura precisa evitar que urgencia operacional legitima se torne gargalo burocratico sem justificativa sensivel.
## 26. Decisoes adiadas para proximos itens
- Regra de aprovacao por custo ou risco.
- Regra final de bloqueio por decisao pendente.
- Regra de liberacao apos aprovacao.
- Regra de rejeicao, cancelamento e expiracao.
- Tratamento da reducao de sensibilidade quando impacto ou urgencia diminuirem.
- Matriz institucional final de combinacoes que exigem excecao gerencial.
## 27. Conclusao tecnica
Impacto e urgencia devem entrar no futuro motor de aprovacao ITSM como entradas complementares de governanca. Por padrao, continuam servindo a priorizacao e ao SLA. Somente quando reforcarem fatores sensiveis, como mudanca, acesso, servico critico, custo, risco, seguranca ou excecao de processo, podem elevar a necessidade de aprovacao formal.
## 28. Proxima etapa recomendada
Executar o item 10 do checklist da Sprint 4: definir regra de aprovacao por custo ou risco.
