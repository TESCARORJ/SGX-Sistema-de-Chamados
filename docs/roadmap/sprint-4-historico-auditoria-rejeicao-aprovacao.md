# Sprint 4 - Historico e Auditoria de Rejeicao de Aprovacao
## 1. Objetivo da definicao
Definir conceitualmente quais informacoes devem ser registradas no historico e na auditoria quando uma aprovacao e rejeitada ou reprovada, incluindo decisor, justificativa, escopo rejeitado, regra afetada e efeito operacional produzido no chamado.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de historico ou auditoria de rejeicao.
- Nao foram criadas entidades novas.
- Nao foram criadas tabelas de auditoria.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de rejeicao de aprovacao
- O SGX atualmente possui rejeicao simples de aprovacao pelo modulo administrativo.
- A decisao negativa atual atualiza o status da aprovacao, registra aprovador, data da decisao, justificativa obrigatoria e historico funcional do chamado.
- O fluxo atual tambem gera auditoria tecnica basica da edicao da aprovacao.
- Os itens 18 a 20 ja definiram que a rejeicao futura pode encerrar, cancelar, devolver para ajuste, manter bloqueado, permitir nova solicitacao ou afetar apenas escopo, nivel ou ramo.
## 4. Dados atualmente registrados em `AprovacaoChamado` na rejeicao
- A entidade atual ja registra na rejeicao:
  - `Status`;
  - `AprovadorId`;
  - `JustificativaDecisao`;
  - `DecididaEm`;
  - `AtualizadoPorUsuarioId`;
  - auditoria base de atualizacao.
- No fluxo atual de reprovacao:
  - o historico funcional registra `ChamadoReprovado`;
  - a auditoria tecnica registra diff simples entre status anterior e novo status;
  - o detalhamento administrativo passa a refletir aprovador, justificativa e data da decisao.
## 5. Lacunas atuais de auditoria da rejeicao
- Hoje nao ha trilha estruturada do escopo exato rejeitado.
- Nao ha diferenciacao formal entre rejeicao parcial e rejeicao total.
- Nao ha registro estruturado de acoes bloqueadas e acoes ainda permitidas.
- Nao ha trilha completa da autoridade do decisor.
- Nao ha auditoria estruturada para fallback, grupo aprovador, quorum, nivel, ramo ou fluxo multinivel.
- Nao ha relacao formal entre a rejeicao e o efeito operacional aplicado no chamado.
- Nao ha registro estruturado do impacto da rejeicao no SLA.
- Nao ha snapshot completo do contexto no momento da reprovacao.
- Nao ha trilha estruturada para retorno para ajuste ou permissao de nova solicitacao.
## 6. Conceito de historico e auditoria de rejeicao de aprovacao
Historico e auditoria de rejeicao de aprovacao e o registro rastreavel da decisao formal negativa tomada sobre uma aprovacao, indicando quem rejeitou, quando rejeitou, qual justificativa foi apresentada, qual escopo foi rejeitado e qual efeito operacional foi produzido no chamado.
O objetivo nao e apenas saber que uma aprovacao foi reprovada. O objetivo e saber o que exatamente foi rejeitado e o que essa rejeicao causou no fluxo do chamado.
## 7. Dados minimos obrigatorios da rejeicao
- Toda rejeicao deve permitir rastrear conceitualmente:
  - identificador do chamado;
  - identificador da aprovacao;
  - data e hora da rejeicao;
  - quem rejeitou;
  - papel, perfil ou autoridade usada pelo decisor;
  - tipo de decisao tomada;
  - justificativa obrigatoria da rejeicao;
  - escopo rejeitado;
  - regra que foi negada;
  - origem da solicitacao rejeitada;
  - se a decisao foi manual ou automatica futura;
  - se houve aprovador especifico;
  - se houve aprovador padrao;
  - se houve grupo aprovador;
  - se houve delegacao;
  - se houve fallback;
  - se houve nivel, ramo ou fluxo multinivel;
  - se a rejeicao foi total ou parcial;
  - acoes bloqueadas;
  - acoes ainda permitidas;
  - se o chamado foi encerrado;
  - se o chamado foi cancelado;
  - se o chamado foi devolvido para ajuste;
  - se nova solicitacao foi permitida;
  - status anterior da aprovacao;
  - status novo da aprovacao;
  - status anterior do chamado;
  - status novo do chamado, se alterado;
  - impacto em SLA ou prazo de aprovacao;
  - evento de historico gerado.
## 8. Dados do decisor que rejeitou
- A auditoria deve registrar conceitualmente:
  - usuario que rejeitou;
  - nome, identificador e vinculo com o chamado;
  - perfil ou papel no momento da rejeicao;
  - se era aprovador especifico;
  - se era aprovador padrao;
  - se era membro de grupo aprovador;
  - se atuou por delegacao;
  - se era dono do servico;
  - se havia conflito de interesse conhecido;
  - se a rejeicao exigia segregacao de funcao;
  - se o decisor tinha autoridade compativel com a regra.
## 9. Dados da rejeicao formal
- Deve ser registrado:
  - status reprovado ou rejeitado;
  - data e hora da rejeicao;
  - justificativa obrigatoria;
  - regra negada;
  - tipo de aprovacao: simples, sequencial, paralela ou multinivel;
  - se e rejeicao de nivel intermediario;
  - se e rejeicao de ramo paralelo;
  - se e rejeicao final;
  - se e rejeicao parcial;
  - se e rejeicao total;
  - se permite ajuste;
  - se permite nova solicitacao;
  - se encerra ou cancela o chamado;
  - se mantem bloqueio.
## 10. Dados do escopo rejeitado
- A auditoria deve registrar o que exatamente foi rejeitado:
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
  - motivo de rejeicao;
  - restricoes ou condicoes indicadas pelo aprovador para eventual reapresentacao.
## 11. Dados do efeito operacional
- A rejeicao deve registrar se:
  - manteve bloqueio;
  - bloqueou acoes adicionais;
  - encerrou o chamado;
  - cancelou o chamado;
  - devolveu para ajuste;
  - permitiu nova solicitacao;
  - impediu inicio do proximo nivel;
  - impediu consolidacao paralela;
  - cancelou niveis ou ramos dependentes;
  - alterou status do chamado;
  - retirou ou manteve o chamado em `AguardandoAprovacao`;
  - atualizou SLA ou prazo;
  - exigiu reavaliacao futura.
## 12. Auditoria de rejeicao manual
- A rejeicao manual deve registrar usuario, perfil, contexto, justificativa, acao executada, data e hora e origem administrativa da decisao.
## 13. Auditoria de rejeicao por aprovador especifico
- Deve registrar qual regra definiu o aprovador especifico e se ele era compativel com o escopo rejeitado.
## 14. Auditoria de rejeicao por aprovador padrao
- Deve registrar que a rejeicao foi tomada por fallback, por que o fallback foi usado, qual regra nao encontrou aprovador especifico e se isso gerou risco de governanca ou necessidade de revisao posterior.
## 15. Auditoria de rejeicao por grupo aprovador
- Quando houver grupo aprovador, registrar:
  - grupo acionado;
  - membro que rejeitou;
  - papel do membro no grupo;
  - quorum exigido;
  - quorum atingido;
  - se a rejeicao individual concluiu o grupo;
  - se havia papeis obrigatorios;
  - se havia conflito de interesse;
  - se a decisao foi voto individual ou decisao final do grupo.
## 16. Auditoria para rejeicao em aprovacao simples
- Deve registrar a decisao negativa unica, o escopo rejeitado, o bloqueio mantido, o efeito no chamado e se nova solicitacao e permitida.
## 17. Auditoria para rejeicao em aprovacao sequencial
- Deve registrar:
  - nivel rejeitado;
  - ordem do nivel;
  - se era intermediario ou final;
  - se impediu os proximos niveis;
  - se encerrou a sequencia;
  - se devolveu para ajuste;
  - niveis anteriores aprovados;
  - niveis ainda nao iniciados;
  - efeito sobre o bloqueio.
## 18. Auditoria para rejeicao em aprovacao paralela
- Deve registrar:
  - ramo rejeitado;
  - se o ramo era obrigatorio ou opcional;
  - se a rejeicao daquele ramo impediu consolidacao final;
  - ramos ainda pendentes;
  - ramos aprovados;
  - ramos cancelados ou expirados;
  - se outros ramos devem continuar ou ser cancelados;
  - efeito sobre o bloqueio.
## 19. Auditoria para rejeicao em aprovacao multinivel
- Deve registrar:
  - nivel ou ramo rejeitado;
  - fluxo aplicado;
  - dependencias;
  - consolidacao;
  - se a rejeicao afetou uma parte ou todo o fluxo;
  - efeito final ou parcial no chamado.
## 20. Auditoria de rejeicao parcial
- Deve registrar claramente que a rejeicao nao encerra necessariamente todo o chamado:
  - o que foi rejeitado;
  - o que nao foi rejeitado;
  - quais acoes ficaram bloqueadas;
  - quais acoes permaneceram permitidas;
  - quais ajustes podem ser feitos;
  - quais aprovacoes ainda sao necessarias.
## 21. Auditoria de rejeicao total
- Deve registrar que o escopo principal ou todas as regras obrigatorias aplicaveis foram rejeitadas e que o efeito operacional definido pela regra foi aplicado.
## 22. Relacao com encerramento do chamado
- Se a rejeicao encerrar ou cancelar o chamado, a auditoria deve registrar:
  - motivo da rejeicao;
  - regra que determinou encerramento ou cancelamento;
  - quem decidiu;
  - se havia alternativa operacional;
  - status final aplicado;
  - justificativa formal;
  - impacto em SLA e historico.
## 23. Relacao com retorno para ajuste
- Se a rejeicao devolver o chamado para ajuste, a auditoria deve registrar:
  - quais ajustes foram exigidos;
  - quem deve ajustar;
  - prazo ou condicao para reapresentacao;
  - se nova solicitacao e permitida;
  - quais dados precisam mudar;
  - se o bloqueio permanece durante o ajuste.
## 24. Relacao com bloqueio remanescente
- Apos rejeicao, pode haver bloqueio remanescente. A auditoria deve registrar:
  - bloqueio mantido;
  - bloqueio ampliado;
  - bloqueio removido, se houver;
  - motivo do bloqueio;
  - escopo bloqueado;
  - aprovacao pendente restante;
  - nivel ou ramo restante;
  - outra regra obrigatoria ainda nao satisfeita.
## 25. Relacao com nova solicitacao de aprovacao
- A auditoria deve registrar se a nova solicitacao foi:
  - permitida;
  - proibida;
  - condicionada a ajuste;
  - condicionada a nova evidencia;
  - condicionada a reducao de custo;
  - condicionada a mitigacao de risco;
  - condicionada a troca de escopo;
  - condicionada a autorizacao administrativa.
## 26. Relacao com SLA
- A rejeicao deve registrar conceitualmente:
  - tempo gasto entre solicitacao e rejeicao;
  - se houve violacao de prazo de aprovacao;
  - se houve impacto no SLA do chamado;
  - se o SLA estava pausado;
  - se o SLA deve ser retomado;
  - se o chamado foi encerrado, cancelado ou devolvido para ajuste;
  - se houve escalonamento antes da rejeicao.
## 27. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a base atual da decisao negativa.
- A entidade ja cobre status, aprovador, justificativa e data da decisao.
- O motor futuro exigira trilha complementar de autoridade, escopo rejeitado, bloqueios efetivos, retorno para ajuste, nova solicitacao, grupo, nivel, ramo, quorum e impacto operacional.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 28. Relacao com `BloqueiaAvancoAtendimento`
- Se a aprovacao rejeitada estava bloqueando avanco, a auditoria deve registrar se o bloqueio foi mantido, ampliado, substituido por retorno para ajuste ou encerrado pelo cancelamento ou encerramento do chamado.
## 29. Relacao com `AguardandoAprovacao`
- Se a rejeicao retirar ou mantiver o chamado em `AguardandoAprovacao`, isso deve ser auditado como efeito operacional.
- Sair de `AguardandoAprovacao` nao deve ser interpretado como liberacao.
## 30. Rastreabilidade esperada
- A auditoria da rejeicao deve permitir responder:
  - quem rejeitou;
  - quando rejeitou;
  - com qual autoridade;
  - por que rejeitou;
  - o que foi rejeitado;
  - qual regra foi negada;
  - qual escopo foi bloqueado;
  - quais acoes permaneceram permitidas;
  - o chamado foi encerrado;
  - o chamado foi devolvido para ajuste;
  - nova solicitacao e permitida;
  - ainda ha bloqueio;
  - a rejeicao foi parcial ou total;
  - houve fallback;
  - houve grupo;
  - houve nivel ou ramo;
  - houve impacto em SLA;
  - qual efeito final no chamado.
## 31. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 32. Lacunas encontradas
- Historico atual registra rejeicao, mas nao necessariamente escopo rejeitado detalhado.
- Falta distinguir rejeicao parcial e total.
- Falta registrar acoes bloqueadas e acoes ainda permitidas.
- Falta trilha estruturada de autoridade do decisor.
- Falta auditoria estruturada de grupo, nivel, ramo, quorum e fallback.
- Falta relacao explicita entre rejeicao e efeito operacional aplicado.
- Falta registro estruturado de impacto no SLA.
- Falta snapshot da rejeicao e do contexto no momento da reprovacao.
- Falta regra estruturada para retorno para ajuste e nova solicitacao.
## 33. Riscos de seguranca e governanca
- Rejeicao sem escopo claro.
- Rejeicao parcial tratada como rejeicao total.
- Rejeicao total tratada como ajuste simples.
- Aprovador sem autoridade compativel.
- Fallback sem rastreabilidade.
- Grupo aprovador sem quorum auditado.
- Encerramento do chamado sem justificativa suficiente.
- Retorno para ajuste sem criterios.
- Nova solicitacao permitida para burlar rejeicao.
- Bloqueio remanescente invisivel.
- Falta de trilha sobre impacto no SLA.
- Impossibilidade de explicar por que o chamado foi encerrado, devolvido ou permaneceu bloqueado.
## 34. Decisoes adiadas para proximos itens
- Como implementar tabela ou estrutura de auditoria de rejeicao.
- Como armazenar escopo rejeitado.
- Como registrar acoes bloqueadas e acoes ainda permitidas.
- Como registrar retorno para ajuste.
- Como registrar permissao ou proibicao de nova solicitacao.
- Como registrar bloqueios remanescentes.
- Como versionar regra negada.
- Como registrar autoridade do decisor.
- Como auditar rejeicao por grupo.
- Como auditar quorum.
- Como auditar niveis e ramos.
- Como auditar fallback.
- Como auditar impacto em SLA.
- Como exibir rejeicao e efeito operacional na interface.
- Como migrar historico atual.
- Como testar auditoria de rejeicao.
## 35. Conclusao tecnica
Historico e auditoria de rejeicao de aprovacao devem permitir reconstruir nao apenas que uma aprovacao foi rejeitada, mas quem rejeitou, com qual autoridade, qual escopo foi efetivamente negado, quais bloqueios permaneceram, se houve retorno para ajuste, se nova solicitacao pode ocorrer e qual efeito operacional a rejeicao produziu no chamado.
## 36. Proxima etapa recomendada
Executar o item 24 do checklist da Sprint 4: definir historico e auditoria de aprovacao expirada ou cancelada.
