# Sprint 4 - Regra de Bloqueio por Decisao Pendente
## 1. Objetivo da definicao
Definir conceitualmente quando uma decisao de aprovacao pendente deve bloquear acoes do chamado, quando deve apenas sinalizar e quais acoes devem permanecer permitidas no futuro motor de aprovacao ITSM reutilizavel.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra de bloqueio por decisao pendente.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual do bloqueio por aprovacao pendente
- O SGX ja possui bloqueio real por aprovacao pendente, mas ele nao e universal.
- O mapeamento da Sprint 4 confirmou que o bloqueio atual se concentra em pontos operacionais especificos, principalmente `Assumir`, `Reabrir`, fechamento por status final e `Encerrar`.
- O sistema atual ainda permite comentarios, anexos, consulta, abertura de chamado e parte da movimentacao intermediaria mesmo com aprovacao pendente.
- O item 4 da sprint ja confirmou que existe base funcional de bloqueio, mas ela nao cobre todos os cenarios futuros do motor.
## 4. Comportamento atual de `BloqueiaAvancoAtendimento`
- `AprovacaoChamadoHelper` considera apenas aprovacoes ativas com `BloqueiaAvancoAtendimento = true`.
- Se existir aprovacao pendente nessas condicoes, o estado consolidado fica com `AprovacaoPendente = true` e `BloqueiaAvancoAtendimento = true`.
- O comportamento atual usa essa informacao para bloquear sobretudo acoes de avanco operacional.
- Aprovacoes pendentes com `BloqueiaAvancoAtendimento = false` hoje podem existir sem impedir conclusao, encerramento ou continuidade.
- O comportamento atual mostra que o campo funciona como indicador simples de bloqueio, mas ainda nao representa toda a logica futura de escopo, acao, nivel, ramo ou tipo de pendencia.
## 5. Conceito de decisao pendente
Decisao pendente e o estado em que uma aprovacao exigida foi solicitada ou deve ser gerada, mas ainda nao possui decisao suficiente para liberar, rejeitar ou concluir a avaliacao conforme a regra aplicavel.
Ela pode existir tanto em aprovacao simples quanto em fluxos sequenciais, paralelos ou multinivel.
## 6. Diferenca entre pendencia bloqueante e pendencia informativa
- Pendencia bloqueante: impede acoes operacionais que dependem de decisao formal.
- Pendencia informativa: nao impede a continuidade da acao solicitada, mas deve ser exibida para rastreabilidade, governanca e eventual reavaliacao.
- Nem toda aprovacao pendente deve bloquear tudo.
- O motor futuro deve separar:
  - pendencia bloqueante;
  - pendencia informativa;
  - acao permitida apesar da pendencia.
## 7. Quando bloquear avanco operacional
- O bloqueio deve ocorrer quando a acao solicitada executar, concluir, liberar, encerrar, aprovar operacionalmente ou tornar irreversivel algo que depende de aprovacao formal.
- Exemplos conceituais:
  - executar servico sensivel;
  - liberar acesso privilegiado;
  - concluir mudanca;
  - alterar ambiente produtivo;
  - encerrar chamado dependente de aprovacao obrigatoria;
  - prosseguir com custo relevante;
  - executar acao com risco operacional alto;
  - avancar fluxo sequencial sem nivel anterior aprovado;
  - consolidar fluxo paralelo com ramo obrigatorio pendente.
## 8. Quando bloquear apenas acoes sensiveis
- O bloqueio deve ser parcial quando o chamado ainda pode receber triagem, evidencia, comentarios ou complementacao, mas nao pode executar a decisao sensivel.
- Exemplos conceituais:
  - permitir triagem, mas bloquear execucao;
  - permitir comentario, mas bloquear conclusao;
  - permitir anexos, mas bloquear mudanca em producao;
  - permitir reclassificacao para revisao, mas bloquear atendimento definitivo;
  - permitir avaliacao tecnica, mas bloquear liberacao de recurso.
## 9. Quando apenas sinalizar
- O motor deve apenas sinalizar quando a aprovacao pendente for consultiva, informativa ou nao impeditiva para a acao solicitada.
- Exemplos conceituais:
  - parecer consultivo;
  - aprovacao vinculada sem bloqueio;
  - risco moderado com procedimento controlado;
  - problema recorrente em analise;
  - evento monitorado;
  - solicitacao sem execucao sensivel imediata.
## 10. Acoes que devem permanecer permitidas
- Consultar chamado.
- Listar chamados.
- Visualizar historico e linha do tempo.
- Consultar status da aprovacao.
- Comentar.
- Anexar evidencia.
- Baixar anexos.
- Registrar historico e auditoria.
- Complementar informacoes.
- Corrigir dados para permitir reavaliacao.
- Cancelar solicitacao, quando a regra permitir.
- Cancelar chamado, quando a regra permitir.
## 11. Regra conceitual para aprovacao simples
- Em aprovacao simples, a pendencia deve bloquear apenas quando a regra ou origem da aprovacao for impeditiva.
- Se a aprovacao for informativa, a acao pode seguir com sinalizacao.
- O motor futuro deve avaliar a combinacao entre exigencia da aprovacao, natureza da acao e escopo do risco, nao apenas a existencia de uma aprovacao pendente.
## 12. Regra conceitual para aprovacao sequencial
- Em aprovacao sequencial, qualquer nivel obrigatorio pendente pode bloquear acoes sensiveis dependentes da sequencia.
- Um nivel intermediario aprovado nao libera automaticamente o chamado; ele apenas permite o inicio do proximo nivel.
- Enquanto a decisao suficiente nao existir, o bloqueio deve permanecer conforme o escopo da regra.
## 13. Regra conceitual para aprovacao paralela
- Em aprovacao paralela, qualquer ramo obrigatorio pendente pode impedir a consolidacao final.
- A aprovacao de um ramo isolado nao libera o chamado se outros ramos obrigatorios ainda estiverem pendentes.
- O bloqueio deve respeitar a consolidacao exigida pelos ramos bloqueantes e pelo tipo de acao solicitada.
## 14. Regra conceitual para aprovacao multinivel
- Em fluxo multinivel, a liberacao so deve ocorrer quando todos os niveis ou ramos obrigatorios exigidos pela regra forem satisfeitos.
- A pendencia de nivel ou ramo obrigatorio deve manter bloqueio total ou parcial conforme o escopo da aprovacao.
- Multinivel e o conceito amplo; sequencial e paralelo definem o comportamento de bloqueio dentro desse conceito.
## 15. Relacao com natureza ITSM
- Naturezas impeditivas, como `Mudanca`, tendem a gerar pendencia bloqueante.
- Naturezas orientadas a sinalizacao podem gerar pendencia informativa, salvo combinacao com servico sensivel, risco, custo, impacto, urgencia ou excecao.
- A natureza deve influenciar a severidade do bloqueio, mas nao deve ser o unico criterio.
## 16. Relacao com tipo de chamado
- Tipos sensiveis podem elevar a pendencia para bloqueante.
- Tipos comuns podem seguir permitidos, salvo combinacao com servico sensivel, custo, risco, impacto, urgencia ou natureza mais restritiva.
- O tipo de chamado deve participar da leitura da acao sensivel que esta sendo tentada.
## 17. Relacao com servico sensivel
- Servico sensivel e um dos criterios mais fortes para pendencia bloqueante.
- Enquanto a aprovacao do servico sensivel estiver pendente, acoes de execucao, liberacao ou encerramento devem ser bloqueadas conforme escopo.
- O servico sensivel pode permitir triagem e coleta de evidencia, mas nao execucao irreversivel.
## 18. Relacao com impacto e urgencia
- Impacto e urgencia isolados nao devem bloquear automaticamente.
- Eles devem reforcar bloqueio quando combinados com servico sensivel, risco, custo, mudanca ou excecao de processo.
- O motor deve evitar transformar toda alta urgencia em trava absoluta sem leitura do contexto.
## 19. Relacao com custo e risco
- Custo ou risco relevantes tendem a gerar pendencia bloqueante.
- Custo baixo ou risco controlado podem gerar apenas sinalizacao.
- O bloqueio deve priorizar decisoes que criem compromisso financeiro, operacional ou de seguranca antes da autorizacao formal.
## 20. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a fonte atual da instancia de aprovacao.
- O motor futuro deve avaliar conceitualmente status pendente, origem, vinculo com `ChamadoId` e indicacao atual de bloqueio.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 21. Relacao com `BloqueiaAvancoAtendimento`
- `BloqueiaAvancoAtendimento` deve ser tratado como indicador atual de bloqueio simples.
- O futuro motor deve preservar compatibilidade com esse sinal existente.
- O bloqueio futuro nao deve ficar limitado apenas a esse campo; tambem deve considerar acao solicitada, escopo da aprovacao, nivel, ramo, regra e tipo de pendencia.
## 22. Relacao com `AguardandoAprovacao`
- `AguardandoAprovacao` pode representar estado operacional de espera.
- Ele nao deve ser obrigatorio para todo bloqueio.
- O motor deve poder bloquear acoes especificas mesmo quando o chamado nao estiver nesse status, desde que exista decisao pendente bloqueante.
- O status continua sendo um bom candidato para cenarios em que a espera operacional deve ficar explicita, mas isso permanece como decisao futura de implementacao.
## 23. Relacao com historico e auditoria
- Todo bloqueio por decisao pendente deve ser rastreavel.
- O motor futuro deve registrar:
  - qual aprovacao bloqueou;
  - qual regra gerou a pendencia;
  - qual acao foi bloqueada;
  - quem tentou executar a acao;
  - quando tentou;
  - status da aprovacao;
  - escopo do bloqueio;
  - se o bloqueio veio de aprovacao simples, sequencial, paralela ou multinivel.
## 24. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual de atendimento.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 25. Lacunas encontradas
- O bloqueio atual nao e universal.
- `BloqueiaAvancoAtendimento` nao cobre todos os cenarios futuros.
- Hoje falta avaliar a acao solicitada junto da aprovacao pendente.
- Hoje falta diferenciar pendencia bloqueante e informativa de forma estruturada.
- Fluxos sequencial, paralelo e multinivel ainda nao existem estruturalmente.
- Falta auditoria especifica de tentativa bloqueada.
- Falta regra clara para cancelamento, reclassificacao e alteracao de dados sensiveis com pendencia.
## 26. Riscos de seguranca e governanca
- Bloquear demais e travar a operacao.
- Bloquear de menos e permitir execucao sensivel sem decisao.
- Tratar toda pendencia como bloqueante.
- Tratar toda pendencia como informativa.
- Ignorar escopo da aprovacao.
- Permitir conclusao ou encerramento antes da decisao.
- Nao auditar tentativa bloqueada.
- Usar apenas status do chamado como controle de bloqueio.
- Usar apenas `BloqueiaAvancoAtendimento` sem considerar acao, regra ou escopo.
## 27. Decisoes adiadas para proximos itens
- Como implementar a regra de bloqueio em codigo.
- Onde centralizar a validacao no futuro motor.
- Como registrar tentativa bloqueada.
- Como diferenciar pendencia bloqueante e informativa no modelo.
- Como tratar bloqueio parcial por acao.
- Como tratar bloqueio por escopo.
- Como tratar cancelamento com pendencia.
- Como tratar reclassificacao com pendencia.
- Como tratar alteracao de dados sensiveis.
- Como refletir bloqueio na interface.
- Como testar regressao do fluxo atual.
- Como migrar aprovacoes existentes.
## 28. Conclusao tecnica
Bloqueio por decisao pendente deve ser definido como o efeito operacional aplicado pelo motor quando uma aprovacao exigida ainda nao possui decisao formal suficiente para permitir a continuidade de uma acao sensivel. O conceito precisa separar pendencia bloqueante, pendencia informativa e acao permitida apesar da pendencia, preservando a compatibilidade com a base atual e preparando o terreno para regras futuras mais granulares.
## 29. Proxima etapa recomendada
Executar o item 17 do checklist da Sprint 4: definir regra de liberacao apos aprovacao.
