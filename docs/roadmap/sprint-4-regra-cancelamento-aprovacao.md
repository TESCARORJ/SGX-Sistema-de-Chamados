# Sprint 4 - Regra de Cancelamento de Aprovacao
## 1. Objetivo da definicao
Definir conceitualmente quando uma aprovacao pode ser cancelada, quem pode cancelar, quais justificativas sao obrigatorias e qual efeito o cancelamento deve produzir no chamado, no bloqueio operacional, na auditoria e na possibilidade de nova solicitacao de aprovacao.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra de cancelamento de aprovacao.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de cancelamento de aprovacao
- O SGX ja possui cancelamento formal de aprovacao simples no modulo atual.
- O cancelamento atual exige justificativa e registra `Status = Cancelado`, `DecididaEm`, `CanceladoEm`, `CanceladoPorUsuarioId`, `JustificativaDecisao` e `MotivoCancelamento`.
- O fluxo administrativo atual trata cancelamento como acao separada de aprovar e reprovar.
- Os testes atuais confirmam que a plataforma impede cancelamento de aprovacao ja decidida e registra historico especifico de aprovacao cancelada.
## 4. Comportamento atual apos cancelamento
- `AprovacaoChamadosAdminUseCases` exige justificativa obrigatoria para cancelar e restringe a operacao a aprovacoes pendentes.
- `AprovacaoChamado.Cancelar` e `CancelarVinculada` impedem cancelamento de aprovacao aprovada ou reprovada no fluxo atual.
- A documentacao atual do modulo diz que o bloqueio e removido se nao houver outra pendencia ativa.
- Os testes atuais mostram que, apos cancelamento simples, alteracoes intermediarias e avancos operacionais voltam a funcionar na logica atual, sem avaliacao adicional de escopo, ramo, nivel ou necessidade de nova aprovacao.
## 5. Conceito de cancelamento de aprovacao
Cancelamento de aprovacao e a acao administrativa ou sistêmica que invalida, interrompe ou encerra uma solicitacao de aprovacao sem representar uma decisao positiva ou negativa sobre o merito da solicitacao.
Ele nao significa aprovacao, nem rejeicao, nem cancelamento automatico do chamado.
## 6. Diferenca entre cancelamento de aprovacao, rejeicao, expiracao e cancelamento de chamado
- Cancelamento de aprovacao: interrompe ou invalida a solicitacao de aprovacao.
- Rejeicao de aprovacao: decisao formal negativa sobre o merito avaliado.
- Expiracao de aprovacao: encerramento ou mudanca de estado por decurso de prazo, sem decisao no tempo esperado.
- Cancelamento de chamado: encerra o fluxo operacional do chamado.
- O motor futuro nao deve tratar cancelamento como uma forma disfarçada de rejeicao nem como encerramento automatico do chamado.
## 7. Quando cancelar aprovacao pendente
- Uma aprovacao pendente pode ser cancelada quando:
  - foi criada por engano;
  - esta duplicada;
  - o escopo do chamado mudou;
  - o servico solicitado mudou;
  - a natureza ou o tipo do chamado foi reclassificado;
  - custo ou risco foram alterados;
  - o solicitante desistiu;
  - o chamado foi cancelado;
  - o chamado foi encerrado por outra regra valida;
  - a aprovacao precisa ser substituida por outra mais adequada;
  - houve erro de aprovador, grupo, nivel ou ramo;
  - houve erro de configuracao;
  - o processo exige reinicio da solicitacao.
## 8. Quando cancelar aprovacao aprovada
- Uma aprovacao aprovada nao deve ser cancelada livremente.
- O cancelamento ou invalidacao controlada so deve ser admitido conceitualmente quando:
  - a aprovacao foi concedida para escopo incorreto;
  - houve erro material;
  - houve fraude, conflito de interesse ou violacao de politica;
  - os dados sensiveis mudaram e a aprovacao perdeu validade;
  - o servico, custo, risco, ambiente ou acesso aprovado mudou;
  - a execucao ainda nao ocorreu e a regra permitir invalidacao controlada;
  - a aprovacao foi substituida por nova aprovacao mais adequada.
- Se a acao aprovada ja foi executada, o cancelamento nao deve apagar o efeito historico; deve gerar auditoria, incidente, revisao ou acao corretiva.
## 9. Quando cancelar aprovacao reprovada
- Uma aprovacao reprovada normalmente deve permanecer como registro historico.
- O cancelamento administrativo so deve ser admitido conceitualmente quando:
  - a aprovacao foi aberta por erro;
  - havia duplicidade;
  - a reprovação ocorreu sobre escopo invalido;
  - a solicitacao sera substituida por nova aprovacao com escopo corrigido;
  - houve erro de aprovador, grupo ou fluxo;
  - a regra permitir desconsiderar a reprovacao anterior sem apagar o historico.
## 10. Quem pode cancelar aprovacao
- O cancelamento deve ser restrito a perfis ou papeis autorizados.
- Exemplos conceituais:
  - solicitante, apenas antes da decisao, se a regra permitir;
  - aprovador responsavel, se ainda nao houver decisao final;
  - administrador do sistema;
  - gestor responsavel;
  - dono do servico;
  - grupo aprovador, conforme regra;
  - motor de aprovacao, em cenarios automáticos futuros de substituicao ou reavaliacao;
  - processo de expiracao, quando tratado como cancelamento administrativo.
## 11. Justificativa obrigatoria de cancelamento
- Todo cancelamento deve exigir justificativa formal quando:
  - a aprovacao era bloqueante;
  - ja havia decisao aprovada ou reprovada;
  - o cancelamento altera o bloqueio do chamado;
  - envolve servico sensivel, custo, risco, acesso, mudanca, seguranca ou compliance;
  - afeta fluxo sequencial, paralelo ou multinivel;
  - substitui uma aprovacao por outra;
  - ocorre por erro administrativo;
  - ocorre apos tentativa de execucao.
## 12. Efeito no bloqueio operacional
- O cancelamento nao deve liberar automaticamente o chamado como regra futura.
- Apos o cancelamento, o motor deve avaliar:
  - a aprovacao cancelada era bloqueante;
  - existe outra aprovacao valida para o mesmo escopo;
  - o escopo ainda exige aprovacao;
  - deve ser gerada nova aprovacao;
  - a acao deve permanecer bloqueada;
  - o chamado deve retornar para ajuste;
  - o chamado pode seguir apenas com sinalizacao.
- Possiveis efeitos conceituais:
  - remover bloqueio, se a aprovacao nao for mais necessaria;
  - manter bloqueio, se o escopo continuar exigindo aprovacao;
  - gerar nova aprovacao;
  - retornar o chamado para ajuste;
  - manter apenas historico, se a aprovacao era informativa;
  - cancelar parte do fluxo sequencial, paralelo ou multinivel relacionado.
## 13. Efeito na possibilidade de nova solicitacao
- Nova solicitacao pode ser permitida quando:
  - o cancelamento ocorreu por escopo incorreto;
  - o servico foi alterado;
  - custo ou risco foram revistos;
  - houve erro de aprovador;
  - o fluxo foi corrigido;
  - a aprovacao anterior era duplicada;
  - ha justificativa formal para reapresentacao.
- Nova solicitacao nao deve ser permitida automaticamente quando:
  - o cancelamento tenta burlar rejeicao;
  - nao houve mudanca real de escopo;
  - ha conflito de interesse;
  - a aprovacao aprovada ja foi executada;
  - a regra exige investigacao, auditoria ou correcao antes de reapresentar.
## 14. Regra conceitual para aprovacao simples
- Em aprovacao simples, cancelar a aprovacao deve afetar somente aquela instancia.
- O chamado so deve ser liberado se o escopo nao exigir mais aprovacao ou se existir outra aprovacao valida.
- Se o escopo continuar sensivel, o cancelamento deve manter bloqueio ou gerar nova aprovacao.
## 15. Regra conceitual para aprovacao sequencial
- Em fluxo sequencial, cancelar um nivel pode invalidar niveis posteriores, impedir avanco da sequencia, exigir reinicio do nivel, retornar ao nivel anterior ou cancelar toda a sequencia, conforme a regra.
- O motor futuro deve distinguir cancelamento de nivel ainda nao consolidado de cancelamento de nivel que ja impactou a cadeia.
## 16. Regra conceitual para aprovacao paralela
- Em fluxo paralelo, cancelar um ramo pode afetar apenas aquele ramo ou invalidar a consolidacao final se o ramo for obrigatorio.
- Ramos ja decididos devem permanecer auditados.
- O cancelamento de ramo obrigatorio nao deve ser confundido com aprovacao do conjunto.
## 17. Regra conceitual para aprovacao multinivel
- Em fluxo multinivel, o cancelamento deve respeitar nivel, ramo, escopo, dependencia e consolidacao.
- Ele pode invalidar parte do fluxo ou todo o fluxo conforme criticidade e momento da interrupcao.
## 18. Relacao com natureza ITSM
- Em `Mudanca`, o cancelamento tende a manter bloqueio ate nova avaliacao se o escopo continuar exigindo autorizacao.
- Em naturezas mais informativas, pode apenas remover uma pendencia consultiva.
- A natureza influencia a severidade do efeito, mas nao deve ser o unico criterio.
## 19. Relacao com tipo de chamado
- Tipos sensiveis, como acesso, compra, recurso restrito e mudanca emergencial, devem manter bloqueio se a aprovacao cancelada ainda for necessaria.
- Tipos menos sensiveis podem permitir substituicao simples ou reclassificacao antes de nova aprovacao.
## 20. Relacao com servico sensivel
- Cancelamento de aprovacao de servico sensivel nao deve liberar a execucao do servico se ele continuar sensivel e sem outra aprovacao valida.
- Se o servico mudar para escopo nao sensivel, o cancelamento pode deixar de manter o bloqueio, desde que a regra o permita.
## 21. Relacao com impacto e urgencia
- Impacto e urgencia podem exigir reavaliacao quando mudarem apos o cancelamento.
- Urgencia alta nao deve justificar cancelamento para burlar aprovacao.
- O motor futuro deve impedir uso do cancelamento como atalho operacional.
## 22. Relacao com custo e risco
- Cancelamento de aprovacao por custo ou risco deve manter bloqueio se custo ou risco relevante permanecer.
- Reducao real de custo ou mitigacao real de risco pode justificar nova avaliacao.
- O cancelamento nao deve apagar a necessidade de autorizacao financeira ou de risco ainda existente.
## 23. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a fonte atual da aprovacao cancelada.
- O cancelamento deve preservar status, responsavel pelo cancelamento, data e hora, justificativa, origem e vinculo com `ChamadoId`.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 24. Relacao com `BloqueiaAvancoAtendimento`
- Quando a aprovacao cancelada era bloqueante, o bloqueio so deve ser removido se a regra concluir que aquela aprovacao nao e mais exigida.
- Caso contrario, o bloqueio deve permanecer ou nova aprovacao deve ser gerada.
- `BloqueiaAvancoAtendimento` deve seguir como compatibilidade do bloqueio simples atual, mas nao pode ser a unica regra futura.
## 25. Relacao com `AguardandoAprovacao`
- Se o chamado estiver em `AguardandoAprovacao`, o cancelamento pode:
  - manter o chamado aguardando nova aprovacao;
  - retornar para ajuste;
  - liberar se a aprovacao nao for mais necessaria;
  - cancelar o chamado, se outra regra operacional tambem determinar.
- Sair de `AguardandoAprovacao` nao deve significar liberacao irrestrita.
## 26. Relacao com historico e auditoria
- Todo cancelamento deve registrar:
  - quem cancelou;
  - quando cancelou;
  - justificativa;
  - aprovacao cancelada;
  - status anterior;
  - escopo afetado;
  - motivo do cancelamento;
  - se havia bloqueio;
  - efeito aplicado ao chamado;
  - se nova aprovacao foi exigida;
  - se houve substituicao por outra aprovacao;
  - relacao com niveis ou ramos, quando houver.
## 27. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 28. Lacunas encontradas
- Falta diferenciar cancelamento por erro, duplicidade, escopo alterado, desistencia e substituicao.
- Falta definir efeitos operacionais especificos do cancelamento.
- Falta diferenciar cancelamento de aprovacao pendente, aprovada e reprovada.
- Falta regra formal para nova solicitacao apos cancelamento.
- Falta regra formal para cancelamento em fluxo sequencial, paralelo e multinivel.
- Falta auditoria detalhada do efeito do cancelamento no bloqueio.
- Falta tratamento de cancelamento apos execucao de acao aprovada.
- Falta relacao explicita com expiracao, que sera definida no item 20.
## 29. Riscos de seguranca e governanca
- Usar cancelamento para burlar rejeicao.
- Cancelar aprovacao aprovada apos execucao sem rastreabilidade.
- Remover bloqueio indevidamente.
- Manter bloqueio indevidamente.
- Permitir nova solicitacao sem mudanca real de escopo.
- Confundir cancelamento de aprovacao com cancelamento de chamado.
- Apagar historico de decisao.
- Cancelar ramos ou niveis sem refletir no fluxo completo.
- Nao auditar motivo e efeito do cancelamento.
## 30. Decisoes adiadas para proximos itens
- Como implementar cancelamento de aprovacao.
- Quais perfis poderao cancelar.
- Como modelar motivo estruturado de cancelamento.
- Como tratar cancelamento automatico por reavaliacao.
- Como tratar cancelamento apos execucao.
- Como tratar cancelamento em fluxos sequenciais, paralelos e multiniveis.
- Como refletir cancelamento na interface.
- Como registrar substituicao por nova aprovacao.
- Como auditar impacto no bloqueio.
- Como relacionar cancelamento com expiracao.
- Como testar regressao do fluxo atual.
- Como migrar aprovacoes canceladas existentes.
## 31. Conclusao tecnica
Cancelamento de aprovacao deve ser definido como a interrupcao controlada de uma solicitacao de aprovacao, sem equivaler a aprovacao, rejeicao ou cancelamento automatico do chamado. O motor futuro precisa distinguir cancelamento de aprovacao pendente, aprovada e reprovada, avaliar o efeito no bloqueio operacional e decidir se o chamado deve ser liberado, permanecer bloqueado, retornar para ajuste ou gerar nova aprovacao.
## 32. Proxima etapa recomendada
Executar o item 20 do checklist da Sprint 4: definir regra de expiracao de aprovacao pendente.
