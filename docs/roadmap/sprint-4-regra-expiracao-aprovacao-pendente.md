# Sprint 4 - Regra de Expiracao de Aprovacao Pendente
## 1. Objetivo da definicao
Definir conceitualmente quando uma aprovacao pendente deve expirar, qual efeito a expiracao deve produzir no chamado, quando deve manter bloqueio, quando deve escalar, quando deve permitir nova solicitacao e como deve ser auditada.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra de expiracao de aprovacao pendente.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de expiracao de aprovacao
- O SGX possui aprovacao simples com `Status = Pendente`, `Aprovado`, `Reprovado` e `Cancelado`.
- Hoje a entidade `AprovacaoChamado` possui `SolicitadaEm`, `DecididaEm` e campos de cancelamento, mas nao possui prazo estruturado, data limite, status expirada ou rotina de vencimento.
- O modulo atual trata a aprovacao pendente como aberta ate decisao ou cancelamento.
- O item 19 ja definiu que cancelamento nao deve esconder expiracao e que expiracao precisa de regra propria.
## 4. Comportamento atual de aprovacoes pendentes sem decisao
- `AprovacaoChamadoHelper` considera aprovacao pendente bloqueante como pendencia ativa, sem qualquer leitura de prazo.
- O helper nao diferencia pendencia nova de pendencia antiga; ambas continuam como `Pendente`.
- Os testes atuais validam criacao de aprovacao pendente, impedimento de duplicidade e cancelamento manual, mas nao existe hoje tratamento automatico equivalente a expirar uma aprovacao.
- Na pratica, uma aprovacao pendente sem decisao pode permanecer aberta indefinidamente no fluxo atual.
## 5. Conceito de expiracao de aprovacao pendente
Expiracao de aprovacao pendente e o estado ou evento em que uma solicitacao de aprovacao permanece sem decisao formal por tempo superior ao permitido pela regra, pelo SLA de aprovacao ou pela politica de governanca.
Expiracao nao e aprovacao, nao e rejeicao e nao e cancelamento administrativo comum.
## 6. Diferenca entre expiracao, cancelamento, rejeicao e pendencia
- Pendencia: a aprovacao aguarda decisao e ainda esta dentro da condicao esperada de analise.
- Expiracao: a aprovacao aguardou decisao alem do prazo definido e exige tratamento operacional ou de governanca.
- Cancelamento: interrupcao administrativa, manual ou sistêmica da aprovacao por erro, duplicidade, mudanca de escopo, desistencia ou substituicao.
- Rejeicao: decisao formal negativa sobre o merito da solicitacao.
## 7. Quando uma aprovacao pendente deve expirar
- Uma aprovacao pendente deve poder expirar quando:
  - ultrapassar prazo maximo de decisao definido pela regra;
  - ultrapassar SLA de aprovacao;
  - ficar sem aprovador valido por tempo excessivo;
  - grupo aprovador nao atingir quorum no prazo;
  - nivel sequencial ficar sem decisao no prazo;
  - ramo paralelo obrigatorio ficar sem decisao no prazo;
  - aprovador estiver ausente sem delegacao valida;
  - aprovacao ficar parada em estado pendente sem movimentacao;
  - a regra de governanca exigir revalidacao apos determinado tempo;
  - mudanca de contexto tornar a pendencia antiga inadequada.
## 8. Quando uma aprovacao pendente nao deve expirar automaticamente
- Nao deve expirar automaticamente quando:
  - nao houver prazo definido pela regra;
  - a aprovacao estiver aguardando complemento de informacao permitido;
  - houver suspensao formal do prazo;
  - a regra permitir pendencia indefinida por decisao administrativa;
  - o chamado estiver pausado por motivo valido;
  - a aprovacao estiver em fluxo critico que exige decisao explicita;
  - expirar causaria liberacao indevida ou perda de governanca.
## 9. Quem ou o que pode acionar expiracao
- Conceitualmente, a expiracao pode ser acionada por:
  - motor de aprovacao;
  - rotina agendada futura;
  - administrador;
  - gestor;
  - dono do servico;
  - regra de SLA;
  - processo de escalonamento;
  - evento de reavaliacao;
  - acao manual autorizada.
## 10. Efeito no bloqueio operacional
- A expiracao nao deve liberar automaticamente o chamado.
- Apos expirar, o motor deve avaliar:
  - a aprovacao era bloqueante;
  - o escopo ainda exige aprovacao;
  - ha outro aprovador disponivel;
  - deve escalar;
  - deve gerar nova aprovacao;
  - deve manter bloqueio;
  - deve retornar para ajuste;
  - deve cancelar administrativamente a aprovacao;
  - deve encerrar ou cancelar o chamado por regra especifica.
- Possiveis efeitos conceituais:
  - manter bloqueio ate nova decisao;
  - escalar para outro aprovador;
  - acionar aprovador padrao, se permitido;
  - gerar nova solicitacao de aprovacao;
  - cancelar administrativamente a aprovacao expirada;
  - devolver chamado para ajuste;
  - manter apenas sinalizacao, se a aprovacao era informativa;
  - encerrar ou cancelar o chamado somente se regra explicita determinar.
## 11. Efeito na possibilidade de nova solicitacao
- Nova solicitacao pode ser permitida quando:
  - a aprovacao expirou sem decisao;
  - houve troca de aprovador;
  - houve correcao de escopo;
  - houve reavaliacao de custo, risco, servico ou natureza;
  - a regra exigir reinicio apos expiracao;
  - a aprovacao antiga perdeu validade operacional.
- Nova solicitacao nao deve ser permitida automaticamente quando:
  - for tentativa de contornar aprovador;
  - nao houver alteracao real de escopo ou responsavel;
  - a expiracao ocorrer em fluxo critico que exige escalonamento;
  - houver suspeita de conflito, fraude ou omissao;
  - a regra exigir intervencao administrativa antes do reenvio.
## 12. Regra conceitual para aprovacao simples
- Em aprovacao simples, a expiracao deve afetar somente aquela instancia.
- O chamado so deve ser liberado se o escopo nao exigir mais aprovacao ou se outra aprovacao valida existir.
- Caso contrario, o chamado permanece bloqueado, escala ou gera nova solicitacao conforme a regra.
## 13. Regra conceitual para aprovacao sequencial
- Em fluxo sequencial, a expiracao de um nivel pode impedir inicio dos niveis seguintes, manter bloqueio, escalar, reiniciar o nivel ou cancelar a sequencia.
- Niveis anteriores aprovados permanecem auditados, mas nao liberam o fluxo final se houver nivel obrigatorio expirado.
## 14. Regra conceitual para aprovacao paralela
- Em fluxo paralelo, a expiracao de um ramo obrigatorio pode impedir consolidacao final.
- Ramos ja decididos permanecem auditados.
- A regra futura deve decidir se o ramo expira isoladamente, escala ou invalida todo o fluxo.
## 15. Regra conceitual para aprovacao multinivel
- Em fluxo multinivel, a expiracao deve respeitar nivel, ramo, escopo, dependencia e consolidacao.
- Ela pode afetar apenas uma parte do fluxo ou todo o fluxo, conforme criticidade e regra.
## 16. Relacao com aprovador padrao
- O aprovador padrao pode ser usado como fallback apos expiracao somente se a regra permitir.
- Ele nao deve ser usado automaticamente para burlar ausencia de decisao de aprovador especifico, grupo, dono do servico ou nivel critico.
## 17. Relacao com grupo aprovador
- Se o grupo nao atingir quorum no prazo, a expiracao pode:
  - manter pendencia;
  - escalar;
  - acionar fallback;
  - cancelar a etapa;
  - gerar nova solicitacao;
  - bloquear consolidacao final.
## 18. Relacao com escalonamento
- Escalonamento e uma resposta possivel a expiracao.
- Ele pode direcionar a aprovacao para gestor, aprovador padrao, dono do servico, grupo superior ou responsavel administrativo.
- Escalonamento nao deve apagar historico da expiracao.
## 19. Relacao com natureza ITSM
- `Mudanca`, servico sensivel, custo e risco tendem a manter bloqueio apos expiracao.
- Naturezas informativas podem expirar apenas como sinalizacao, conforme regra.
- A natureza influencia severidade, mas nao deve ser o unico criterio.
## 20. Relacao com tipo de chamado
- Tipos sensiveis devem manter bloqueio enquanto nao houver decisao valida.
- Tipos comuns podem permitir expiracao informativa se nao houver acao sensivel associada.
## 21. Relacao com servico sensivel
- Expiracao de aprovacao de servico sensivel nao deve liberar execucao do servico.
- Se o servico ainda for sensivel, deve manter bloqueio, escalar ou exigir nova aprovacao.
## 22. Relacao com impacto e urgencia
- Urgencia alta pode exigir escalonamento mais rapido, mas nao deve pular aprovacao.
- Impacto alto pode reforcar manutencao do bloqueio ou acionamento de escala.
- O motor futuro deve evitar usar urgencia como justificativa para liberar sem decisao.
## 23. Relacao com custo e risco
- Custo ou risco relevante com aprovacao expirada deve manter bloqueio ate nova decisao, mitigacao, escalonamento ou aprovacao substituta.
- Expiracao nao deve dissolver necessidade de autorizacao financeira ou de risco ainda exigida.
## 24. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a fonte atual da aprovacao pendente.
- O motor futuro deve avaliar `Status`, datas, origem, escopo, bloqueio e historico para determinar se a aprovacao expirou.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 25. Relacao com `BloqueiaAvancoAtendimento`
- Quando a aprovacao expirada era bloqueante, `BloqueiaAvancoAtendimento` nao deve ser interpretado como resolvido.
- O bloqueio deve permanecer, ser escalado ou ser substituido por nova aprovacao conforme a regra.
## 26. Relacao com `AguardandoAprovacao`
- Se o chamado estiver em `AguardandoAprovacao`, a expiracao pode:
  - manter aguardando nova decisao;
  - escalar para outro aprovador;
  - retornar para ajuste;
  - cancelar administrativamente a aprovacao;
  - cancelar ou encerrar o chamado somente se regra explicita determinar.
- Sair de `AguardandoAprovacao` nao deve significar liberacao irrestrita.
## 27. Relacao com SLA
- A expiracao de aprovacao deve ser conceitualmente separada do SLA do chamado.
- Decisoes futuras devem definir:
  - se o SLA do chamado pausa enquanto aguarda aprovacao;
  - se existe SLA proprio da aprovacao;
  - se expiracao da aprovacao afeta SLA operacional;
  - se escalonamento deve ocorrer antes de violacao de SLA;
  - se aprovacao expirada gera evento de governanca.
## 28. Relacao com historico e auditoria
- Toda expiracao deve registrar:
  - aprovacao expirada;
  - prazo esperado;
  - data e hora da expiracao;
  - quem ou o que acionou a expiracao;
  - escopo afetado;
  - regra aplicada;
  - se havia bloqueio;
  - efeito aplicado ao chamado;
  - escalonamento, se houve;
  - nova aprovacao gerada, se houve;
  - relacao com niveis ou ramos, quando houver.
## 29. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 30. Lacunas encontradas
- Falta campo estruturado de prazo ou expiracao.
- Falta SLA proprio de aprovacao.
- Falta status especifico de aprovacao expirada.
- Falta rotina de expiracao.
- Falta escalonamento apos expiracao.
- Falta auditoria especifica de expiracao.
- Falta regra para expiracao em grupo aprovador.
- Falta regra para expiracao em fluxo sequencial, paralelo e multinivel.
- Falta relacao formal entre expiracao de aprovacao e SLA do chamado.
## 31. Riscos de seguranca e governanca
- Expiracao liberar chamado indevidamente.
- Expiracao virar rejeicao automatica sem regra explicita.
- Expiracao virar cancelamento sem auditoria.
- Manter pendencias expiradas indefinidamente.
- Nao escalar aprovacao critica.
- Usar aprovador padrao automaticamente sem regra.
- Ignorar impacto de custo, risco, acesso, mudanca ou servico sensivel.
- Nao auditar prazo, motivo e efeito da expiracao.
- Confundir SLA do chamado com prazo da aprovacao.
## 32. Decisoes adiadas para proximos itens
- Como implementar prazo de expiracao.
- Como armazenar data limite.
- Como criar status de expirada.
- Como implementar rotina ou job de expiracao.
- Como configurar SLA de aprovacao.
- Como escalar antes e depois da expiracao.
- Como tratar expiracao em grupo aprovador.
- Como tratar expiracao em fluxo sequencial, paralelo e multinivel.
- Como refletir expiracao na interface.
- Como auditar efeito da expiracao no bloqueio.
- Como relacionar expiracao com cancelamento administrativo.
- Como testar regressao do fluxo atual.
- Como migrar aprovacoes pendentes existentes.
## 33. Conclusao tecnica
Expiracao de aprovacao pendente deve ser definida como o evento em que uma solicitacao permanece sem decisao alem do prazo permitido pela regra e exige tratamento adicional de governanca. Ela nao deve liberar automaticamente o chamado nem ser confundida com aprovacao, rejeicao ou cancelamento comum. O motor futuro precisa distinguir pendencia dentro do prazo, pendencia expirada, escalonamento, manutencao de bloqueio e nova solicitacao, sempre com trilha auditavel.
## 34. Proxima etapa recomendada
Executar o item 21 do checklist da Sprint 4: definir historico e auditoria de solicitacao de aprovacao.
