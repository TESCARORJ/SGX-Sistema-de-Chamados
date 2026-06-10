# Sprint 4 - Historico e Auditoria de Decisao de Aprovacao
## 1. Objetivo da definicao
Definir conceitualmente quais informacoes devem ser registradas no historico e na auditoria quando uma aprovacao e decidida positivamente, seja por aprovador especifico, aprovador padrao, grupo aprovador, nivel, ramo, aprovacao parcial, aprovacao total ou decisao com efeito operacional limitado.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de historico ou auditoria de decisao de aprovacao.
- Nao foram criadas entidades novas.
- Nao foram criadas tabelas de auditoria.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de decisao de aprovacao
- O SGX atualmente possui decisao simples de aprovacao pelo modulo administrativo.
- A decisao positiva atual atualiza o status da aprovacao, registra aprovador, data da decisao, justificativa opcional e historico funcional do chamado.
- O fluxo atual tambem gera auditoria tecnica basica da edicao da aprovacao.
- Os itens 13 a 20 ja definiram que o motor futuro podera ter fluxo simples, sequencial, paralelo ou multinivel, com liberacao parcial, total ou por escopo.
## 4. Dados atualmente registrados em `AprovacaoChamado` na decisao
- A entidade atual ja registra na decisao:
  - `Status`;
  - `AprovadorId`;
  - `JustificativaDecisao`;
  - `DecididaEm`;
  - `AtualizadoPorUsuarioId`;
  - auditoria base de atualizacao.
- No fluxo atual de aprovacao positiva:
  - o historico funcional registra `ChamadoAprovado`;
  - a auditoria tecnica registra diff simples entre status anterior e novo status;
  - o detalhamento administrativo passa a refletir aprovador e data da decisao.
## 5. Lacunas atuais de auditoria da decisao
- Hoje nao ha trilha estruturada do escopo exato aprovado.
- Nao ha diferenciacao formal entre aprovacao parcial e aprovacao total.
- Nao ha registro estruturado de acoes liberadas e acoes ainda bloqueadas.
- Nao ha trilha completa da autoridade do decisor.
- Nao ha auditoria estruturada para fallback, grupo aprovador, quorum, nivel, ramo ou fluxo multinivel.
- Nao ha relacao formal entre a decisao e o efeito operacional aplicado no chamado.
- Nao ha registro estruturado do impacto da decisao no SLA.
- Nao ha snapshot completo do contexto no momento da aprovacao.
## 6. Conceito de historico e auditoria de decisao de aprovacao
Historico e auditoria de decisao de aprovacao e o registro rastreavel da decisao formal positiva tomada sobre uma aprovacao, indicando quem decidiu, quando decidiu, qual decisao foi tomada, qual justificativa foi apresentada, qual escopo foi aprovado e qual efeito operacional foi produzido no chamado.
O objetivo nao e apenas saber que uma aprovacao foi aprovada. O objetivo e saber o que exatamente foi aprovado e o que essa decisao liberou.
## 7. Dados minimos obrigatorios da decisao
- Toda decisao positiva de aprovacao deve permitir rastrear conceitualmente:
  - identificador do chamado;
  - identificador da aprovacao;
  - data e hora da decisao;
  - quem decidiu;
  - papel, perfil ou autoridade usada pelo decisor;
  - tipo de decisao tomada;
  - justificativa da decisao, quando aplicavel;
  - escopo aprovado;
  - regra que a decisao satisfez;
  - origem da solicitacao aprovada;
  - se a decisao foi manual ou automatica futura;
  - se houve aprovador especifico;
  - se houve aprovador padrao;
  - se houve grupo aprovador;
  - se houve delegacao;
  - se houve fallback;
  - se houve nivel, ramo ou fluxo multinivel;
  - se a aprovacao foi total ou parcial;
  - acoes liberadas;
  - acoes ainda bloqueadas;
  - status anterior da aprovacao;
  - status novo da aprovacao;
  - status anterior do chamado;
  - status novo do chamado, se alterado;
  - impacto em SLA ou prazo de aprovacao;
  - evento de historico gerado.
## 8. Dados do decisor
- A auditoria deve registrar conceitualmente:
  - usuario que aprovou;
  - nome, identificador e vinculo com o chamado;
  - perfil ou papel no momento da decisao;
  - se era aprovador especifico;
  - se era aprovador padrao;
  - se era membro de grupo aprovador;
  - se atuou por delegacao;
  - se era dono do servico;
  - se havia conflito de interesse conhecido;
  - se a decisao exigia segregacao de funcao;
  - se o decisor tinha autoridade compativel com a regra.
## 9. Dados da aprovacao concedida
- Deve ser registrado:
  - status aprovado;
  - data e hora da aprovacao;
  - justificativa ou observacao do aprovador;
  - regra satisfeita;
  - tipo de aprovacao: simples, sequencial, paralela ou multinivel;
  - se e aprovacao de nivel intermediario;
  - se e aprovacao de ramo paralelo;
  - se e aprovacao final;
  - se e aprovacao parcial;
  - se e aprovacao total;
  - validade da decisao;
  - escopo liberado;
  - escopo nao liberado.
## 10. Dados do escopo aprovado
- A auditoria deve registrar o que exatamente foi aprovado:
  - natureza ITSM;
  - tipo de chamado;
  - servico sensivel;
  - impacto;
  - urgencia;
  - custo;
  - risco;
  - acesso;
  - ambiente;
  - alteracao de configuracao;
  - mudanca;
  - execucao tecnica;
  - etapa, nivel ou ramo;
  - limite financeiro, quando aplicavel futuramente;
  - restricoes ou condicoes impostas pelo aprovador.
## 11. Dados do efeito operacional
- A decisao deve registrar se:
  - removeu bloqueio;
  - manteve bloqueio parcial;
  - liberou avanco operacional;
  - liberou apenas acao especifica;
  - liberou apenas um nivel;
  - liberou apenas um ramo;
  - iniciou proximo nivel sequencial;
  - permitiu consolidacao paralela;
  - alterou status do chamado;
  - retirou o chamado de `AguardandoAprovacao`;
  - manteve o chamado em espera por outra aprovacao;
  - atualizou SLA ou prazo;
  - exigiu reavaliacao futura.
## 12. Auditoria de decisao manual
- A decisao manual deve registrar usuario, perfil, contexto, justificativa, acao executada, data e hora e origem administrativa da decisao.
## 13. Auditoria de decisao por aprovador especifico
- Deve registrar qual regra definiu o aprovador especifico e se ele era compativel com o escopo aprovado.
## 14. Auditoria de decisao por aprovador padrao
- Deve registrar que a decisao foi tomada por fallback, por que o fallback foi usado, qual regra nao encontrou aprovador especifico e se isso gerou risco de governanca ou necessidade de revisao posterior.
## 15. Auditoria de decisao por grupo aprovador
- Quando houver grupo aprovador, registrar:
  - grupo acionado;
  - membro que decidiu;
  - papel do membro no grupo;
  - quorum exigido;
  - quorum atingido;
  - se a decisao individual concluiu o grupo;
  - se havia papeis obrigatorios;
  - se havia conflito de interesse;
  - se a decisao foi voto individual ou decisao final do grupo.
## 16. Auditoria para aprovacao simples
- Deve registrar a decisao unica, o escopo aprovado, o bloqueio removido ou mantido e as acoes liberadas.
## 17. Auditoria para aprovacao sequencial
- Deve registrar:
  - nivel decidido;
  - ordem do nivel;
  - se era intermediario ou final;
  - se liberou apenas o proximo nivel;
  - se liberou a execucao final;
  - niveis ainda pendentes;
  - efeito sobre o bloqueio.
## 18. Auditoria para aprovacao paralela
- Deve registrar:
  - ramo decidido;
  - se o ramo era obrigatorio ou opcional;
  - se a aprovacao daquele ramo permitiu consolidacao final;
  - ramos ainda pendentes;
  - ramos rejeitados, cancelados ou expirados;
  - efeito sobre o bloqueio.
## 19. Auditoria para aprovacao multinivel
- Deve registrar:
  - nivel ou ramo decidido;
  - fluxo aplicado;
  - dependencias;
  - consolidacao;
  - se a decisao satisfez uma parte ou todo o fluxo;
  - efeito final ou parcial no chamado.
## 20. Auditoria de aprovacao parcial
- Deve registrar claramente que a aprovacao nao libera todo o chamado:
  - o que foi aprovado;
  - o que nao foi aprovado;
  - quais acoes ficaram liberadas;
  - quais acoes permaneceram bloqueadas;
  - quais aprovacoes ainda sao necessarias.
## 21. Auditoria de aprovacao total
- Deve registrar que todas as regras obrigatorias aplicaveis foram satisfeitas para aquele escopo e que a liberacao operacional definida pela regra foi aplicada.
## 22. Relacao com liberacao operacional
- A auditoria da decisao deve registrar qual liberacao foi produzida:
  - liberacao total;
  - liberacao parcial;
  - liberacao por escopo;
  - liberacao por acao;
  - liberacao por nivel;
  - liberacao por ramo;
  - nenhuma liberacao, quando a decisao for intermediaria.
## 23. Relacao com bloqueio remanescente
- Mesmo apos aprovacao, pode haver bloqueio remanescente. A auditoria deve registrar:
  - bloqueio removido;
  - bloqueio mantido;
  - motivo do bloqueio mantido;
  - aprovacao pendente restante;
  - nivel ou ramo restante;
  - outra regra obrigatoria ainda nao satisfeita.
## 24. Relacao com SLA
- A decisao de aprovacao deve registrar conceitualmente:
  - tempo gasto entre solicitacao e decisao;
  - se houve violacao de prazo de aprovacao;
  - se houve impacto no SLA do chamado;
  - se o SLA estava pausado;
  - se o SLA deve ser retomado;
  - se houve escalonamento antes da decisao.
## 25. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a base atual da decisao.
- A entidade ja cobre status, aprovador, justificativa e data da decisao.
- O motor futuro exigira trilha complementar de autoridade, escopo aprovado, liberacao efetiva, bloqueios remanescentes, grupo, nivel, ramo, quorum e validade da decisao.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 26. Relacao com `BloqueiaAvancoAtendimento`
- Se a aprovacao decidida estava bloqueando avanco, a auditoria deve registrar se o bloqueio foi removido, mantido parcialmente ou mantido por outras regras.
## 27. Relacao com `AguardandoAprovacao`
- Se a decisao retirar ou mantiver o chamado em `AguardandoAprovacao`, isso deve ser auditado como efeito operacional.
- Sair de `AguardandoAprovacao` nao deve ser interpretado como liberacao irrestrita.
## 28. Rastreabilidade esperada
- A auditoria da decisao deve permitir responder:
  - quem aprovou;
  - quando aprovou;
  - com qual autoridade;
  - o que foi aprovado;
  - qual regra foi satisfeita;
  - qual escopo foi liberado;
  - quais acoes foram liberadas;
  - ainda ha bloqueio;
  - qual bloqueio foi removido;
  - qual bloqueio permaneceu;
  - a aprovacao foi parcial ou total;
  - houve fallback;
  - houve grupo;
  - houve nivel ou ramo;
  - houve impacto em SLA;
  - qual efeito final no chamado.
## 29. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 30. Lacunas encontradas
- Historico atual registra decisao, mas nao necessariamente escopo aprovado detalhado.
- Falta distinguir aprovacao parcial e total.
- Falta registrar acoes liberadas e acoes ainda bloqueadas.
- Falta trilha estruturada de autoridade do decisor.
- Falta auditoria estruturada de grupo, nivel, ramo, quorum e fallback.
- Falta relacao explicita entre decisao e efeito operacional aplicado.
- Falta registro estruturado de impacto no SLA.
- Falta snapshot da decisao e do contexto no momento da aprovacao.
## 31. Riscos de seguranca e governanca
- Aprovacao sem escopo claro.
- Aprovacao parcial tratada como aprovacao total.
- Aprovador sem autoridade compativel.
- Fallback sem rastreabilidade.
- Grupo aprovador sem quorum auditado.
- Liberacao operacional sem registro.
- Bloqueio remanescente invisivel.
- Falta de justificativa.
- Falta de trilha sobre impacto no SLA.
- Impossibilidade de explicar por que o chamado foi liberado ou permaneceu bloqueado.
## 32. Decisoes adiadas para proximos itens
- Como implementar tabela ou estrutura de auditoria de decisao.
- Como armazenar escopo aprovado.
- Como registrar acoes liberadas.
- Como registrar bloqueios remanescentes.
- Como versionar regra satisfeita.
- Como registrar autoridade do decisor.
- Como auditar decisao por grupo.
- Como auditar quorum.
- Como auditar niveis e ramos.
- Como auditar fallback.
- Como auditar impacto em SLA.
- Como exibir decisao e efeito operacional na interface.
- Como migrar historico atual.
- Como testar auditoria de decisao.
## 33. Conclusao tecnica
Historico e auditoria de decisao de aprovacao devem permitir reconstruir nao apenas que uma aprovacao foi aprovada, mas quem aprovou, com qual autoridade, qual escopo foi efetivamente liberado, quais bloqueios permaneceram e qual efeito operacional a decisao produziu no chamado. O motor futuro precisa distinguir aprovacao total, parcial, intermediaria e final com trilha auditavel suficiente para governanca e seguranca.
## 34. Proxima etapa recomendada
Executar o item 23 do checklist da Sprint 4: definir historico e auditoria de rejeicao de aprovacao.
