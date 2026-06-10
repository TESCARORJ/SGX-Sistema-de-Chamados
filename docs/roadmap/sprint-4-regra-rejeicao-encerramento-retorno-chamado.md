# Sprint 4 - Regra de Rejeicao e Encerramento ou Retorno do Chamado
## 1. Objetivo da definicao
Definir conceitualmente o que deve acontecer quando uma aprovacao e rejeitada ou reprovada, separando casos de encerramento, cancelamento, retorno para ajuste, manutencao de bloqueio, reapresentacao para nova aprovacao e nova avaliacao.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra de rejeicao.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual da rejeicao/reprovacao de aprovacao
- O SGX ja possui decisao formal de reprovacao no modulo atual de aprovacao simples.
- A reprovacao e registrada em `AprovacaoChamado` com `Status = Reprovado`, `DecididaEm`, `AprovadorId` e `JustificativaDecisao`.
- O portal ja exibe o chamado como reprovado e orienta o solicitante a consultar a justificativa.
- Os itens 16 e 17 da sprint ja definiram que pendencia e liberacao precisam ser avaliadas por escopo, acao e regra, o que tambem impede tratar toda rejeicao como um unico efeito fixo.
## 4. Comportamento atual apos reprovacao
- `AprovacaoChamadoHelper` devolve mensagem orientativa de chamado reprovado e justificativa de reprovacao quando a ultima decisao e `Reprovado`.
- O helper atualmente marca `AprovacaoPendente = false` e `BloqueiaAvancoAtendimento = false` apos a decisao negativa, porque o campo atual representa apenas pendencia bloqueante simples.
- Os testes atuais mostram que `Assumir` continua bloqueado enquanto a aprovacao esta pendente, mas alteracao de status intermediario e `Encerrar` podem voltar a funcionar apos reprovacao simples nesta etapa atual.
- Isso confirma que hoje o sistema registra a reprovacao e sua justificativa, mas ainda nao distingue operacionalmente entre rejeicao que encerra, retorna para ajuste, exige nova aprovacao ou mantem bloqueio por escopo.
## 5. Conceito de rejeicao/reprovacao
Rejeicao ou reprovacao de aprovacao e a decisao formal negativa registrada por aprovador autorizado, grupo, nivel ou ramo, indicando que a acao solicitada nao esta autorizada no escopo avaliado.
Ela pode reprovar um servico, um custo, um risco, um acesso, uma mudanca, um nivel, um ramo ou o objeto principal inteiro do chamado.
## 6. Diferenca entre rejeicao de aprovacao, cancelamento de aprovacao e cancelamento de chamado
- Rejeicao de aprovacao: decisao negativa de um aprovador ou regra, indicando que o pedido avaliado nao foi autorizado.
- Cancelamento de aprovacao: interrupcao da solicitacao de aprovacao antes da decisao final, por erro, duplicidade, mudanca de escopo, desistência, expiracao ou motivo administrativo.
- Cancelamento de chamado: encerramento ou cancelamento do proprio fluxo do chamado, podendo ou nao ser consequencia de uma rejeicao.
- O motor futuro nao deve confundir a negativa de autorizacao com o encerramento automatico do chamado em todos os cenarios.
## 7. Quando encerrar ou cancelar o chamado
- A rejeicao pode encerrar ou cancelar o chamado quando:
  - o objeto principal do chamado foi negado;
  - nao existe alternativa operacional permitida;
  - a solicitacao depende exclusivamente da aprovacao rejeitada;
  - o servico sensivel solicitado nao pode ser executado sem aprovacao;
  - o custo foi negado e nao existe opcao viavel sem custo;
  - o risco foi considerado inaceitavel;
  - a mudanca foi negada sem possibilidade de ajuste;
  - a solicitacao de acesso foi negada;
  - a politica interna determinar encerramento apos reprovação;
  - o solicitante ou gestor confirmar desistência apos a negativa.
## 8. Quando devolver para ajuste
- A rejeicao deve devolver o chamado para ajuste quando:
  - a informacao apresentada era incompleta;
  - a justificativa era insuficiente;
  - o escopo precisa ser reduzido;
  - o custo precisa ser revisto;
  - o risco precisa ser mitigado;
  - o plano de reversao precisa ser complementado;
  - impacto ou urgencia precisam ser recalculados;
  - o servico solicitado precisa ser corrigido;
  - o aprovador indicar condicoes para nova analise.
## 9. Quando manter bloqueado
- A rejeicao deve manter o chamado bloqueado quando:
  - a acao reprovada continua sendo necessaria para prosseguir;
  - o chamado nao pode avancar sem nova decisao formal;
  - a rejeicao e impeditiva, mas o fluxo ainda permite correcao;
  - ha niveis ou ramos dependentes afetados;
  - a regra exige nova aprovacao antes de qualquer execucao;
  - o chamado esta aguardando ajuste antes de retornar ao ciclo de aprovacao.
## 10. Quando permitir nova solicitacao de aprovacao
- A nova solicitacao pode ser permitida quando:
  - o escopo for alterado;
  - a justificativa for complementada;
  - o custo for reduzido;
  - o risco for mitigado;
  - a evidencia tecnica for adicionada;
  - o plano de execucao ou reversao for corrigido;
  - o servico solicitado for alterado;
  - o aprovador indicar possibilidade de reapresentacao;
  - a regra permitir reenvio apos rejeicao.
## 11. Justificativa obrigatoria de rejeicao
- Toda rejeicao deve exigir justificativa formal.
- A justificativa e especialmente obrigatoria quando:
  - bloquear acao sensivel;
  - encerrar ou cancelar chamado;
  - reprovar custo, risco, acesso ou mudanca;
  - impedir execucao de servico critico;
  - encerrar sequencia ou consolidacao paralela;
  - exigir retorno para ajuste;
  - impactar SLA, auditoria ou compliance.
## 12. Regra conceitual para aprovacao simples
- Em aprovacao simples, a rejeicao deve afetar apenas o escopo avaliado.
- Se esse escopo corresponder ao objeto principal do chamado, a regra pode encerrar ou cancelar o chamado.
- Se houver correcao possivel, a regra deve devolver para ajuste ou manter bloqueado ate nova solicitacao.
- Aprovacao simples rejeitada nao deve ser traduzida automaticamente como encerramento universal.
## 13. Regra conceitual para aprovacao sequencial
- Em aprovacao sequencial, a rejeicao de um nivel pode impedir o inicio dos niveis seguintes.
- Se o nivel rejeitado for obrigatorio, a sequencia pode ser encerrada, devolvida para ajuste ou mantida bloqueada conforme a regra.
- Niveis anteriores aprovados devem permanecer auditados, mas nao significam liberacao final.
- A rejeicao precisa informar claramente se interrompe toda a cadeia ou apenas exige correcao antes de reiniciar.
## 14. Regra conceitual para aprovacao paralela
- Em aprovacao paralela, a rejeicao de um ramo pode reprovar apenas aquele ramo ou encerrar a aprovacao inteira se o ramo for critico.
- Outros ramos podem ser cancelados, mantidos para auditoria ou continuar em paralelo se a regra permitir.
- A consolidacao final positiva nao deve ocorrer enquanto existir ramo obrigatorio rejeitado.
- O motor futuro deve distinguir rejeicao de ramo obrigatorio, ramo opcional e ramo apenas consultivo.
## 15. Regra conceitual para aprovacao multinivel
- Em aprovacao multinivel, a rejeicao deve respeitar nivel, ramo, escopo e regra de consolidacao.
- Uma rejeicao critica pode encerrar o fluxo inteiro.
- Uma rejeicao parcial pode exigir ajuste apenas naquele escopo.
- Multinivel e o conceito amplo; simples, sequencial e paralelo definem como a rejeicao afeta a liberacao futura.
## 16. Relacao com natureza ITSM
- `Mudanca` rejeitada tende a impedir execucao da mudanca e pode encerrar, cancelar ou devolver para ajuste conforme o escopo.
- Naturezas informativas ou consultivas podem registrar parecer reprovado sem necessariamente encerrar o chamado.
- A natureza deve influenciar a severidade do efeito, mas nao deve ser o unico criterio.
## 17. Relacao com tipo de chamado
- Tipos sensiveis, como acesso, custo, compra, recurso restrito ou mudanca emergencial, tendem a ter rejeicao mais impeditiva.
- Tipos comuns podem permitir ajuste, reclassificacao ou nova submissao.
- O motor futuro deve olhar o tipo de chamado junto com a acao e o escopo rejeitado.
## 18. Relacao com servico sensivel
- Rejeicao de servico sensivel deve bloquear a execucao daquele servico.
- Se esse servico for o objeto principal do chamado, a rejeicao pode levar a encerramento ou cancelamento.
- Se a regra permitir troca de servico ou reducao de escopo, o chamado pode voltar para ajuste em vez de ser encerrado.
## 19. Relacao com impacto e urgencia
- Impacto e urgencia nao devem, isoladamente, definir encerramento por rejeicao.
- Eles influenciam criticidade, prazo de resposta, prioridade de correcao e necessidade de nova analise.
- O motor futuro deve evitar tratar qualquer alta urgencia como obrigacao de encerrar imediatamente apos a rejeicao.
## 20. Relacao com custo e risco
- Rejeicao de custo ou risco tende a impedir execucao ate ajuste, mitigacao ou nova aprovacao.
- Risco inaceitavel pode justificar encerramento ou cancelamento controlado.
- Custo negado pode permitir retorno para reducao de escopo ou reapresentacao com alternativa menos onerosa.
## 21. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a fonte atual da decisao de rejeicao.
- O motor futuro deve avaliar `Status`, `JustificativaDecisao`, decisor, data da decisao, origem e vinculo com `ChamadoId` para orientar o efeito operacional.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 22. Relacao com `BloqueiaAvancoAtendimento`
- Quando a aprovacao rejeitada era bloqueante, o bloqueio nao deve ser liberado automaticamente como regra futura.
- O motor deve decidir se a rejeicao provoca encerramento, cancelamento, retorno para ajuste ou manutencao de bloqueio.
- `BloqueiaAvancoAtendimento` deve ser preservado apenas como compatibilidade do bloqueio simples atual, nao como definicao completa do efeito da rejeicao.
## 23. Relacao com `AguardandoAprovacao`
- Se o chamado estiver em `AguardandoAprovacao`, a rejeicao pode provocar saida para ajuste, cancelamento, encerramento ou bloqueio, conforme a regra.
- Sair desse status nao deve ser interpretado como liberacao.
- O status operacional futuro precisa refletir o efeito real da rejeicao, nao apenas o fim da espera.
## 24. Relacao com historico e auditoria
- Toda rejeicao deve ser rastreavel.
- O motor futuro deve registrar:
  - quem rejeitou;
  - quando rejeitou;
  - justificativa formal;
  - escopo rejeitado;
  - regra que exigiu a aprovacao;
  - efeito aplicado ao chamado;
  - se houve encerramento, cancelamento, retorno ou bloqueio;
  - se nova solicitacao foi permitida;
  - relacao com niveis ou ramos, quando houver.
## 25. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual de atendimento.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 26. Lacunas encontradas
- Falta diferenciar estruturalmente rejeicao que encerra, retorna ou mantem bloqueio.
- Falta regra formal para reenvio apos rejeicao.
- Falta status especifico de retorno para ajuste por rejeicao.
- Falta auditoria detalhada do efeito da rejeicao no chamado.
- Falta relacao explicita entre rejeicao e SLA.
- Falta tratamento estrutural para rejeicao parcial em fluxo paralelo ou multinivel.
- Falta regra formal separando cancelamento de aprovacao rejeitada de chamado cancelado.
## 27. Riscos de seguranca e governanca
- Encerrar chamado indevidamente por rejeicao parcial.
- Permitir avanco apos rejeicao impeditiva.
- Reabrir aprovacao rejeitada sem ajuste real.
- Nao exigir justificativa formal.
- Perder rastreabilidade do escopo rejeitado.
- Confundir rejeicao de aprovacao com cancelamento do chamado.
- Tratar toda rejeicao como encerramento.
- Tratar toda rejeicao como retorno para ajuste.
- Nao bloquear execucao de servico sensivel rejeitado.
## 28. Decisoes adiadas para proximos itens
- Como implementar o efeito operacional da rejeicao.
- Quais status serao usados apos rejeicao.
- Como registrar retorno para ajuste.
- Como permitir nova solicitacao de aprovacao.
- Como tratar rejeicao parcial em paralelo ou multinivel.
- Como integrar rejeicao com SLA.
- Como exibir rejeicao e proximos passos na interface.
- Como auditar tentativa de avanco apos rejeicao.
- Como testar regressao do fluxo atual.
- Como migrar aprovacoes rejeitadas existentes.
## 29. Conclusao tecnica
Rejeicao ou reprovacao de aprovacao deve ser definida como a decisao formal negativa sobre o escopo avaliado, sem um unico efeito fixo para todos os cenarios. O motor futuro precisa distinguir rejeicao que encerra ou cancela o chamado, rejeicao que devolve para ajuste, rejeicao que mantem bloqueio e rejeicao que permite nova solicitacao, preservando rastreabilidade, compatibilidade com o fluxo atual e seguranca operacional.
## 30. Proxima etapa recomendada
Executar o item 19 do checklist da Sprint 4: definir regra de cancelamento de aprovacao.
