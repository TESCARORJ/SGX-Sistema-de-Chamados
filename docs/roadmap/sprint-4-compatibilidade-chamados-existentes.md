# Sprint 4 - Compatibilidade com Chamados Existentes
## 1. Objetivo da avaliacao
Avaliar conceitualmente como o futuro motor de aprovacoes ITSM deve conviver com chamados ja existentes no banco, preservando historico, decisoes ja tomadas e estabilidade operacional sem reinterpretacao retroativa automatica.
## 2. Limites desta etapa
- Esta etapa registra apenas avaliacao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de migracao ou reprocessamento de chamados existentes.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais ou de alteracao funcional de chamados.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `Chamado`, `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado`, SLA ou fluxos atuais.
- Nao houve homologacao nem aceite final.
## 3. Contexto dos chamados existentes
- O SGX ja possui chamados abertos, em atendimento, aguardando aprovacao, resolvidos, encerrados e cancelados.
- Esses chamados podem ter sido criados antes da futura ativacao do motor de aprovacoes ITSM por natureza, tipo, servico sensivel, impacto, urgencia, custo, risco, grupo aprovador e fluxo multinivel.
- O principio central desta compatibilidade e: chamados existentes nao devem ser reinterpretados retroativamente de forma automatica sem regra explicita, auditoria e estrategia de migracao.
## 4. Situacao atual do modelo de chamado
- `Chamado` ja possui:
  - identificacao e dados basicos (`Codigo`, `Titulo`, `Descricao`);
  - classificacao operacional (`CategoriaId`, `SubcategoriaId`, `TipoSolicitacaoId`, `CatalogoServicoId`);
  - classificacao ITSM (`NaturezaChamado`, `ImpactoChamado`, `UrgenciaChamado`);
  - status e datas (`StatusId`, `AbertoEm`, `EncerradoEm`);
  - relacoes com historico, SLA e aprovacoes.
- O modelo nao possui hoje campos estruturados de custo, risco, aprovador padrao, grupo aprovador, nivel, ramo ou marca explicita de chamado legado reavaliado pelo motor novo.
## 5. Situacao atual do modelo de aprovacao
- `AprovacaoChamado` hoje representa aprovacao simples e registra origem, status, bloqueio simples, justificativas, datas de solicitacao/decisao/cancelamento e vinculo com `ChamadoId`.
- O fluxo atual trabalha com `Pendente`, `Aprovado`, `Reprovado` e `Cancelado`.
- Ainda nao existem estrutura nativa para expiracao, grupo aprovador, multilivel, ramo paralelo, quorum ou escopo aprovado/rejeitado estruturado.
## 6. Cenarios de chamados existentes
- Chamado antigo sem aprovacao vinculada.
- Chamado antigo com aprovacao pendente.
- Chamado antigo com aprovacao aprovada.
- Chamado antigo com aprovacao reprovada.
- Chamado antigo com aprovacao cancelada.
- Chamado antigo em status final.
- Chamado antigo em atendimento.
- Chamado antigo em `AguardandoAprovacao`.
- Chamado antigo com dados ITSM incompletos.
- Chamado antigo que sofrer alteracao sensivel apos implantacao do motor novo.
## 7. Chamados sem aprovacao vinculada
- Nao devem receber aprovacao retroativa automaticamente apenas porque uma regra futura passaria a exigir aprovacao.
- Devem ser preservados como legado, salvo se houver alteracao sensivel posterior ou reavaliacao manual autorizada.
- A ausencia de aprovacao legada nao pode ser interpretada automaticamente nem como aprovacao tacita nem como bloqueio absoluto.
## 8. Chamados com aprovacao pendente
- Devem preservar o estado pendente atual.
- O motor futuro deve respeitar a instancia existente de `AprovacaoChamado`, `BloqueiaAvancoAtendimento` e sinais de `AguardandoAprovacao`.
- Nao deve substituir automaticamente essa pendencia por regra nova, grupo novo ou fluxo novo sem trilha auditavel.
## 9. Chamados com aprovacao aprovada
- A decisao aprovada deve ser preservada.
- O motor nao deve invalidar automaticamente aprovacao antiga so porque uma regra nova passou a existir.
- Nova avaliacao so deve ocorrer se houver alteracao relevante de escopo, servico, custo, risco, impacto, urgencia, natureza ou tipo.
## 10. Chamados com aprovacao reprovada
- A rejeicao deve ser preservada.
- O motor nao deve liberar automaticamente o chamado porque a regra nova e diferente da regra antiga.
- Nova solicitacao so deve ser permitida com ajuste real, mudanca de escopo, evidencia nova ou decisao administrativa auditada.
## 11. Chamados com aprovacao cancelada
- O cancelamento deve permanecer como registro historico valido.
- O motor futuro deve avaliar necessidade de nova aprovacao apenas quando houver nova acao, alteracao sensivel ou reavaliacao autorizada.
- Cancelamento antigo nao deve ser convertido automaticamente em aprovacao, rejeicao ou expiracao.
## 12. Chamados em status final
- Chamados resolvidos, encerrados ou cancelados nao devem ser reabertos, bloqueados ou reprocessados automaticamente pelo motor novo.
- Qualquer revisao nesses chamados deve ser excepcional, manual e auditada.
- Status final legado deve continuar valido para historico, relatorios e dashboards.
## 13. Chamados em atendimento
- Chamados em andamento devem ser preservados.
- Regras novas nao devem interromper automaticamente atendimento ja em curso.
- Reavaliacao so deve ocorrer se houver alteracao sensivel, nova acao critica ou regra futura explicitamente configurada para isso.
## 14. Chamados aguardando aprovacao
- O estado atual deve ser preservado.
- O motor futuro nao deve depender exclusivamente de `AguardandoAprovacao`, mas tambem nao deve ignorar esse status em chamados legados.
- Sair desse status em legado nao deve ser interpretado como liberacao irrestrita.
## 15. Chamados sem classificacao ITSM completa
- Nao devem falhar nem bloquear indevidamente apenas por falta de dados legados.
- Devem seguir com fallback seguro: sinalizacao de dados incompletos, preservacao do historico e revisao manual quando houver acao sensivel.
- A falta de classificacao completa nao autoriza reinterpretacao automatica agressiva.
## 16. Chamados sem servico sensivel definido
- Nao devem assumir automaticamente que o servico e sensivel.
- Tambem nao devem assumir automaticamente que o servico esta livre de aprovacao.
- Devem ser tratados como legado sem servico sensivel estruturado, podendo exigir revisao se houver nova acao sensivel.
## 17. Chamados sem custo ou risco estruturado
- Nao devem gerar bloqueio retroativo automatico por custo ou risco.
- Como `Chamado` hoje nao possui campos estruturados de custo ou risco, o motor futuro deve evitar inferencias retroativas sem base de dados.
- Se custo ou risco forem futuramente informados ou alterados, a reavaliacao pode ser disparada a partir desse novo contexto.
## 18. Compatibilidade com `BloqueiaAvancoAtendimento`
- O campo atual deve ser preservado como sinal de bloqueio simples legado.
- Se um chamado antigo tem aprovacao pendente bloqueante, o motor futuro deve respeitar esse bloqueio ate decisao, cancelamento, expiracao ou reavaliacao auditada.
- O motor novo nao deve tratar ausencia desse campo como autorizacao automatica para liberar tudo.
## 19. Compatibilidade com `AguardandoAprovacao`
- O status deve continuar representando espera operacional quando ja estiver aplicado.
- O motor futuro nao deve depender exclusivamente dele, mas deve respeita-lo como indicador legado importante.
- O tratamento de compatibilidade deve evitar contradicoes entre status legado e interpretacao do novo motor.
## 20. Compatibilidade com historico e auditoria legado
- Historicos e auditorias ja existentes devem permanecer validos.
- A ausencia de campos novos de auditoria em dados antigos nao deve invalidar decisoes legadas.
- O motor futuro deve poder registrar conceitualmente que determinadas informacoes nao estavam disponiveis no momento da decisao legada.
## 21. Risco de reprocessamento retroativo
- Reprocessar chamados antigos automaticamente pode:
  - bloquear chamados em andamento;
  - invalidar decisoes antigas;
  - gerar aprovacoes duplicadas;
  - quebrar relatorios;
  - alterar SLA retroativamente;
  - gerar inconsistencias com historico;
  - criar inseguranca operacional;
  - reduzir confianca do usuario no sistema.
## 22. Quando nao reavaliar chamado antigo
- Nao reavaliar automaticamente quando:
  - o chamado esta encerrado, cancelado ou resolvido;
  - o chamado esta em atendimento sem alteracao sensivel;
  - a aprovacao antiga ja foi decidida;
  - nao ha regra explicita de migracao;
  - os dados necessarios para a regra nova nao existem;
  - a reavaliacao alteraria historico sem trilha auditavel.
## 23. Quando reavaliar manualmente
- Reavaliar manualmente quando:
  - houver suspeita de aprovacao indevida;
  - houver mudanca de escopo;
  - houver alteracao de servico sensivel;
  - houver alteracao de custo ou risco;
  - houver mudanca relevante de impacto ou urgencia;
  - houver exigencia de auditoria ou compliance;
  - gestor ou administrador solicitar revisao formal;
  - chamado antigo entrar em etapa sensivel ainda nao executada.
## 24. Quando exigir nova aprovacao
- Exigir nova aprovacao quando:
  - uma nova acao sensivel for solicitada;
  - o servico for alterado para sensivel;
  - custo ou risco aumentarem;
  - impacto ou urgencia mudarem de forma material;
  - natureza ou tipo forem reclassificados;
  - a aprovacao anterior nao cobrir o novo escopo;
  - a aprovacao antiga estiver cancelada, expirada ou rejeitada e houver novo pedido valido.
## 25. Quando preservar decisao antiga
- Preservar decisao antiga quando:
  - a aprovacao cobre o escopo ainda vigente;
  - nao houve alteracao sensivel;
  - a acao ja foi executada;
  - o chamado esta finalizado;
  - a decisao faz parte de historico valido;
  - a regra futura nao declarou migracao ou reavaliacao obrigatoria.
## 26. Quando considerar legado como nao reavaliado
- O motor futuro deve poder tratar chamados antigos como:
  - legado preservado;
  - nao reavaliado pelo motor novo;
  - sem dados suficientes para regra nova;
  - pendente de revisao manual apenas se houver gatilho;
  - fora do escopo de retroatividade automatica.
## 27. Diretrizes de fallback seguro
- Quando dados legados forem insuficientes:
  - nao liberar acao sensivel sem regra clara;
  - nao bloquear tudo sem contexto;
  - sinalizar necessidade de revisao;
  - preservar historico;
  - exigir revisao manual para acao sensivel;
  - evitar criacao automatica de aprovacao sem escopo claro;
  - auditar qualquer reavaliacao futura.
## 28. Diretrizes para migracao futura
- Qualquer migracao futura deve:
  - ser explicita;
  - ser auditavel;
  - ter criterio claro de selecao;
  - nao alterar chamados finalizados sem justificativa;
  - preservar decisoes antigas;
  - registrar chamados reavaliados;
  - registrar chamados nao reavaliados;
  - evitar duplicidade de aprovacoes;
  - permitir rollback ou revisao auditavel;
  - ser testada com massa representativa.
## 29. Impacto em relatorios e dashboards
- O motor futuro nao deve quebrar relatorios atuais.
- Chamados legados devem continuar aparecendo com seus status e decisoes historicas.
- Novos indicadores podem distinguir:
  - aprovacoes legadas;
  - aprovacoes do motor novo;
  - chamados nao reavaliados;
  - chamados reavaliados;
  - bloqueios legados;
  - bloqueios do motor novo.
## 30. Riscos de seguranca e governanca
- Reprocessar tudo e travar operacao.
- Nao reavaliar nada e permitir execucao sensivel indevida.
- Gerar aprovacao duplicada.
- Apagar ou desconsiderar decisao historica.
- Interpretar ausencia de dado como aprovacao.
- Interpretar ausencia de dado como bloqueio absoluto.
- Quebrar SLA e relatorios.
- Reabrir chamado finalizado automaticamente.
- Liberar chamado reprovado por regra nova.
- Bloquear chamado aprovado por regra antiga sem auditoria.
## 31. Decisoes adiadas para proximos itens
- Como marcar chamado legado.
- Como registrar "nao reavaliado pelo motor novo".
- Como implementar reavaliacao manual.
- Como implementar migracao de dados legados.
- Como tratar aprovacoes antigas sem escopo estruturado.
- Como tratar status final com regra nova.
- Como refletir compatibilidade na interface.
- Como testar massa de chamados existentes.
- Como migrar relatorios.
- Como versionar regra aplicada por data.
- Como auditar reprocessamento futuro.
## 32. Conclusao tecnica
Compatibilidade com chamados existentes deve ser tratada como convivencia controlada entre o legado e o futuro motor de aprovacoes. O motor novo nao deve reinterpretar automaticamente todo o historico nem invalidar decisoes antigas sem regra explicita, auditoria e estrategia de migracao. A retroatividade precisa ser excepcional, segura e rastreavel.
## 33. Proxima etapa recomendada
Executar o item 26 do checklist da Sprint 4: avaliar compatibilidade com fluxo atual de abertura de chamado.
