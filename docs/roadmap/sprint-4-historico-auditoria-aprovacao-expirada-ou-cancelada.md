# Sprint 4 - Historico e Auditoria de Aprovacao Expirada ou Cancelada
## 1. Objetivo da definicao
Definir conceitualmente quais informacoes devem ser registradas no historico e na auditoria quando uma aprovacao for expirada ou cancelada, incluindo origem do evento, motivo, escopo afetado, efeito operacional no chamado, bloqueio resultante, necessidade de nova solicitacao e relacao com fluxos simples, sequenciais, paralelos ou multiniveis.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de historico ou auditoria de expiracao/cancelamento.
- Nao foram criadas entidades novas.
- Nao foram criadas tabelas de auditoria.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de expiracao ou cancelamento
- O SGX atualmente possui cancelamento simples de aprovacao e nao possui expiracao automatica estruturada.
- Cancelamento atual registra `Status = Cancelado`, `DecididaEm`, `CanceladoEm`, `CanceladoPorUsuarioId`, `JustificativaDecisao` e `MotivoCancelamento`.
- Expiracao ainda nao existe como status nem como evento tecnico nativo no modulo atual.
- Os itens 19 e 20 ja definiram conceitualmente cancelamento e expiracao como eventos distintos, com efeitos diferentes sobre bloqueio, nova solicitacao e governanca.
## 4. Dados atualmente registrados em `AprovacaoChamado` na expiracao ou cancelamento
- Hoje a entidade atual registra diretamente no cancelamento:
  - `Status`;
  - `DecididaEm`;
  - `CanceladoEm`;
  - `CanceladoPorUsuarioId`;
  - `JustificativaDecisao`;
  - `MotivoCancelamento`;
  - `AtualizadoPorUsuarioId`.
- No fluxo atual:
  - o historico funcional registra `AprovacaoCancelada`;
  - a auditoria tecnica registra diff simples de status;
  - nao existe registro especifico de expiracao porque a expiracao ainda nao foi implementada.
## 5. Lacunas atuais de auditoria da expiracao ou cancelamento
- Hoje nao ha trilha estruturada para diferenciar cancelamento por erro, duplicidade, substituicao, desistência, alteracao sensivel ou governanca.
- Nao ha estrutura para registrar expiracao como evento proprio.
- Nao ha registro formal do prazo esperado da aprovacao.
- Nao ha trilha estruturada do escopo afetado por expiracao ou cancelamento.
- Nao ha relacao explicita entre o evento e o efeito operacional no chamado.
- Nao ha estrutura para registrar fallback, grupo aprovador, quorum, nivel, ramo ou consolidacao afetada.
- Nao ha registro estruturado de bloqueio mantido, removido ou reavaliado.
- Nao ha registro estruturado da permissao ou proibicao de nova solicitacao apos expiracao ou cancelamento.
## 6. Conceito de historico e auditoria de aprovacao expirada ou cancelada
Historico e auditoria de aprovacao expirada ou cancelada e o registro rastreavel do evento que encerra uma solicitacao de aprovacao sem representa-la como decisao positiva de merito, informando quem ou o que encerrou a etapa, quando isso ocorreu, por qual motivo, qual escopo foi afetado e qual efeito operacional foi produzido no chamado.
O objetivo nao e apenas saber que a aprovacao deixou de estar pendente. O objetivo e saber por que ela deixou de estar pendente e o que isso passou a significar para o fluxo operacional.
## 7. Dados minimos obrigatorios da expiracao ou cancelamento
- Toda expiracao ou cancelamento deve permitir rastrear conceitualmente:
  - identificador do chamado;
  - identificador da aprovacao;
  - tipo de evento: expiracao ou cancelamento;
  - data e hora do evento;
  - quem ou o que acionou o evento;
  - papel, perfil ou autoridade do responsavel, quando houver;
  - motivo do cancelamento ou criterio da expiracao;
  - escopo afetado;
  - regra ou prazo relacionado;
  - origem da solicitacao afetada;
  - se havia aprovador especifico, padrao ou grupo;
  - se havia delegacao;
  - se houve fallback;
  - se havia nivel, ramo ou fluxo multinivel;
  - se o evento afetou apenas uma parte ou todo o fluxo;
  - status anterior e novo da aprovacao;
  - status anterior e novo do chamado, se alterado;
  - bloqueio mantido, removido ou reavaliado;
  - se nova solicitacao foi permitida;
  - impacto em SLA ou prazo da aprovacao;
  - evento de historico gerado.
## 8. Dados do responsavel pelo evento
- A auditoria deve registrar conceitualmente:
  - usuario que cancelou, quando for evento manual;
  - sistema, motor, rotina ou regra que expirou, quando for evento automatico futuro;
  - nome, identificador e vinculo com o chamado;
  - perfil ou papel no momento do evento;
  - se atuou como aprovador, administrador, gestor, dono do servico ou fallback;
  - se havia delegacao;
  - se havia conflito de interesse conhecido;
  - se a autoridade era compativel com a regra de expiracao ou cancelamento.
## 9. Dados da expiracao formal
- A expiracao deve registrar:
  - que a aprovacao ultrapassou prazo sem decisao;
  - data e hora da expiracao;
  - prazo esperado;
  - regra de prazo violada;
  - se o escopo era simples, sequencial, paralelo ou multinivel;
  - se havia nivel ou ramo obrigatorio vencido;
  - se o evento gerou escalonamento;
  - se o bloqueio foi mantido;
  - se foi gerada nova solicitacao;
  - se houve cancelamento administrativo subsequente.
## 10. Dados do cancelamento formal
- O cancelamento deve registrar:
  - status cancelado;
  - data e hora do cancelamento;
  - justificativa obrigatoria, quando aplicavel;
  - motivo administrativo do cancelamento;
  - se o cancelamento decorreu de erro, duplicidade, escopo alterado, desistência, reclassificacao, substituicao ou governanca;
  - se havia decisao anterior;
  - se o cancelamento afetou apenas uma etapa ou todo o fluxo;
  - se exige nova aprovacao;
  - se removeu, manteve ou reavaliou bloqueio.
## 11. Dados do escopo afetado
- A auditoria deve registrar o que exatamente foi afetado:
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
  - motivo administrativo ou prazo relacionado;
  - restricoes para reavaliacao ou reapresentacao.
## 12. Dados do efeito operacional
- O evento deve registrar se:
  - manteve bloqueio;
  - removeu bloqueio;
  - reavaliou bloqueio;
  - devolveu para ajuste;
  - gerou nova solicitacao;
  - escalou para novo aprovador;
  - cancelou niveis ou ramos dependentes;
  - manteve o chamado em `AguardandoAprovacao`;
  - retirou o chamado de `AguardandoAprovacao`;
  - alterou status do chamado;
  - atualizou SLA ou prazo;
  - exigiu reavaliacao futura.
## 13. Auditoria de expiracao
- Deve registrar prazo esperado, data/hora do vencimento, responsavel tecnico ou regra que detectou, escopo expirado, efeito operacional e eventual escalonamento.
## 14. Auditoria de cancelamento manual
- Deve registrar usuario, perfil, contexto, justificativa, acao executada, data/hora e origem administrativa do cancelamento.
## 15. Auditoria de cancelamento por aprovador especifico
- Deve registrar a regra que associou o aprovador especifico ao fluxo e o motivo pelo qual a etapa foi cancelada.
## 16. Auditoria de cancelamento por aprovador padrao
- Deve registrar que havia fallback envolvido, por que ele foi usado e se o cancelamento expoe risco de governanca ou problema de configuracao.
## 17. Auditoria de cancelamento por grupo aprovador
- Deve registrar:
  - grupo acionado;
  - membro ou autoridade que cancelou;
  - papel no grupo;
  - quorum exigido e situacao do quorum;
  - se o cancelamento afetou todo o grupo ou um ramo;
  - se havia conflito de interesse;
  - se o cancelamento decorreu de reconfiguracao, substituicao ou expiracao do grupo.
## 18. Auditoria para aprovacao simples
- Deve registrar a expiracao ou cancelamento da instancia unica, o escopo afetado, o bloqueio resultante e a possibilidade ou nao de nova solicitacao.
## 19. Auditoria para aprovacao sequencial
- Deve registrar:
  - nivel expirado ou cancelado;
  - ordem do nivel;
  - se havia niveis posteriores dependentes;
  - se o evento interrompeu a sequencia;
  - se gerou reinicio, retorno ou encerramento da etapa;
  - efeito sobre bloqueio e proximo nivel.
## 20. Auditoria para aprovacao paralela
- Deve registrar:
  - ramo expirado ou cancelado;
  - se o ramo era obrigatorio ou opcional;
  - se o evento impediu consolidacao final;
  - ramos ainda pendentes, aprovados, reprovados ou cancelados;
  - se outros ramos seguem validos;
  - efeito sobre o bloqueio.
## 21. Auditoria para aprovacao multinivel
- Deve registrar:
  - nivel ou ramo afetado;
  - fluxo aplicado;
  - dependencias;
  - consolidacao;
  - se o evento afetou uma parte ou todo o fluxo;
  - efeito final ou parcial no chamado.
## 22. Relacao com bloqueio operacional
- A auditoria deve registrar se o bloqueio foi mantido, removido, ampliado ou colocado em estado de reavaliacao, e com base em qual regra.
## 23. Relacao com nova solicitacao
- A auditoria deve registrar se nova solicitacao foi permitida, proibida ou condicionada a revisao manual, reclassificacao, nova evidencia ou alteracao sensivel.
## 24. Relacao com bloqueio remanescente
- Apos expiracao ou cancelamento, a auditoria deve registrar:
  - bloqueio mantido;
  - bloqueio removido;
  - bloqueio reavaliado;
  - motivo;
  - escopo remanescente;
  - outro nivel, ramo ou aprovacao ainda pendente.
## 25. Relacao com SLA
- A auditoria deve registrar conceitualmente:
  - tempo entre solicitacao e cancelamento/expiracao;
  - se houve violacao do prazo de aprovacao;
  - se o SLA do chamado estava pausado;
  - se o SLA foi retomado;
  - se houve escalonamento;
  - se o evento gerou repercussao operacional no prazo do chamado.
## 26. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a base atual do cancelamento e a fonte indireta da futura expiracao.
- A entidade ja cobre cancelamento simples, mas nao cobre expiracao estruturada nem trilha rica de governanca.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 27. Relacao com `BloqueiaAvancoAtendimento`
- Se a aprovacao expirada ou cancelada era bloqueante, a auditoria deve registrar se o bloqueio permaneceu por regra de compatibilidade, foi removido ou passou a depender de nova avaliacao.
## 28. Relacao com `AguardandoAprovacao`
- A auditoria deve registrar se o chamado permaneceu ou saiu de `AguardandoAprovacao`, sem tratar essa saida como liberacao irrestrita.
## 29. Rastreabilidade esperada
- A auditoria deve permitir responder:
  - quem cancelou ou o que expirou;
  - quando o evento ocorreu;
  - por qual motivo;
  - qual prazo ou regra foi afetado;
  - qual escopo foi impactado;
  - houve bloqueio mantido;
  - houve nova solicitacao;
  - houve escalonamento;
  - qual efeito final no chamado;
  - ainda ha aprovacao obrigatoria remanescente.
## 30. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 31. Lacunas encontradas
- Falta estrutura para expiracao.
- Falta distinguir estruturalmente cancelamento simples, cancelamento substitutivo e expiracao.
- Falta trilha formal do escopo afetado.
- Falta registro do efeito operacional exato.
- Falta relacao formal com SLA.
- Falta trilha de grupo, quorum, nivel, ramo e fallback.
- Falta snapshot do contexto no momento do evento.
## 32. Riscos de seguranca e governanca
- Expiracao ser confundida com rejeicao.
- Cancelamento ser usado para esconder rejeicao.
- Bloqueio ser removido sem base auditavel.
- Bloqueio permanecer sem necessidade.
- Nova solicitacao ser criada sem criterio.
- Evento afetar fluxo paralelo ou multinivel sem trilha.
- Impacto em SLA ficar invisivel.
- Historico nao explicar por que o chamado saiu ou permaneceu em espera.
## 33. Decisoes adiadas para proximos itens
- Como implementar estrutura de auditoria para expiracao e cancelamento.
- Como armazenar data limite e causa de expiracao.
- Como modelar motivos estruturados de cancelamento.
- Como registrar escalonamento e nova solicitacao.
- Como auditar grupo, quorum, niveis e ramos.
- Como refletir isso na interface.
- Como migrar historico atual.
- Como testar auditoria desses eventos.
## 34. Conclusao tecnica
Historico e auditoria de aprovacao expirada ou cancelada devem permitir reconstruir por que a aprovacao deixou de estar pendente sem decisao positiva, quem ou o que encerrou a etapa, qual escopo foi afetado, quais bloqueios permaneceram e qual caminho operacional o chamado passou a seguir.
## 35. Proxima etapa recomendada
Executar o item 25 do checklist da Sprint 4: avaliar compatibilidade com chamados existentes.
