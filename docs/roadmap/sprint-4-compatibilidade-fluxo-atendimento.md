# Sprint 4 - Compatibilidade com fluxo atual de atendimento

## 1. Objetivo da avaliacao

Avaliar conceitualmente como o futuro motor de aprovacoes ITSM deve se encaixar no fluxo atual de atendimento de chamados sem quebrar o atendimento comum, sem impedir triagem, comentarios e anexos, sem liberar acoes sensiveis sem decisao formal e sem alterar o comportamento atual antes da implementacao formal do motor.

## 2. Limites desta etapa

- Esta etapa registra apenas avaliacao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional do motor de aprovacao no atendimento.
- Nao foram alterados use cases de assumir, atribuir, alterar status, encerrar, reabrir, comentar ou anexar.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `AguardandoAprovacao`, SLA ou frontend/admin.
- Nao houve homologacao nem aceite final.

## 3. Contexto atual do fluxo de atendimento

O atendimento atual ja possui acoes operacionais para assumir chamado, atribuir ou encaminhar, alterar status, comentar, anexar evidencias, consultar detalhes, encerrar e reabrir. A aprovacao existente entra hoje como camada simples de bloqueio legado e afeta apenas parte dessas operacoes, principalmente quando existe aprovacao ativa, pendente e com `BloqueiaAvancoAtendimento = true`.

O principio central desta compatibilidade e: o motor de aprovacao deve bloquear acoes sensiveis, nao paralisar todo o atendimento.

## 4. Componentes atuais envolvidos no atendimento

- `DetalharChamadoAdminUseCase` e `AdminUseCaseHelpers` montam o detalhe administrativo do chamado, incluindo estado atual de aprovacao e acoes disponiveis.
- `AcoesChamadoService` calcula acoes operacionais permitidas conforme status, natureza, permissao e estado legado de aprovacao.
- `AprovacaoChamadoHelper` resume o estado da aprovacao em `RequerAprovacao`, `AprovacaoPendente`, `StatusAprovacao`, `BloqueiaAvancoAtendimento` e mensagem de bloqueio.
- `AssumirChamadoUseCase` e `AssumirChamadoFilaAdminUseCase` tratam assuncao direta ou por fila.
- `AtribuirChamadoUseCase`, `DirecionarChamadoGrupoTecnicoAdminUseCase` e `TransferirGrupoTecnicoChamadoUseCase` tratam organizacao operacional.
- `AlterarStatusChamadoUseCase` trata mudancas de status durante o fluxo.
- `EncerrarChamadoUseCase` trata fechamento tecnico/administrativo.
- `ReabrirChamadoUseCase` trata retomada de chamados finalizados.
- `ComentarChamadoAdminUseCase`, `ComentarChamadoUseCase` e `AnexarArquivoChamadoUseCase` tratam comentarios, anexos e evidencias.
- `AdminDetalheChamadoView.vue` consome o detalhe e exibe indicadores, estado de aprovacao e acoes disponiveis no frontend/admin.

## 5. Relacao atual com `AprovacaoChamado`

- `AprovacaoChamado` continua sendo a base atual de aprovacao simples no atendimento.
- O helper legado olha aprovacoes `Ativo = true` e `BloqueiaAvancoAtendimento = true`.
- Se existir alguma pendente bloqueante, o sistema marca `AprovacaoPendente = true` e `BloqueiaAvancoAtendimento = true`.
- Se nao houver pendencia, o helper expoe a ultima decisao relevante como `Aprovado`, `Reprovado` ou `Cancelado`, sem bloquear automaticamente o atendimento.
- O modelo atual nao diferencia bloqueio por acao, escopo, nivel, ramo, quorum ou fluxo multinivel.

## 6. Relacao atual com `BloqueiaAvancoAtendimento`

- `BloqueiaAvancoAtendimento` hoje e o principal sinal de bloqueio simples legado.
- Quando existe aprovacao pendente bloqueante, o fluxo atual impede:
  - assumir chamado;
  - assumir chamado pela fila;
  - alterar para status final;
  - encerrar chamado;
  - reabrir chamado.
- O futuro motor deve preservar esse campo como compatibilidade do bloqueio simples atual e complementar, no futuro, com bloqueio por acao, escopo, nivel ou ramo.

## 7. Relacao atual com `AguardandoAprovacao`

- `AguardandoAprovacao` continua sendo um status operacional valido, mas o bloqueio atual nao depende exclusivamente dele.
- O fluxo atual usa mais fortemente a aprovacao ativa e bloqueante do que o status em si.
- O futuro motor deve manter essa compatibilidade: pode haver bloqueio sensivel mesmo fora de `AguardandoAprovacao`, e a saida desse status nao deve ser interpretada automaticamente como liberacao irrestrita.

## 8. Relacao atual com `AprovacaoPendente` no detalhe do chamado

- O detalhe administrativo do chamado ja expoe `RequerAprovacao`, `AprovacaoPendente`, `StatusAprovacao` e `AprovacaoChamadoId`.
- O frontend/admin ja usa essa informacao para mostrar indicadores visuais, acoes administrativas de aprovacao e disponibilidade operacional.
- Conceitualmente, o futuro motor deve enriquecer esse detalhe com escopo bloqueado, acao afetada, aprovador, grupo, nivel, ramo e tipo da pendencia, sem perder compatibilidade com o formato atual.

## 9. Relacao atual com historico, comentarios e anexos

- Historico funcional do chamado continua registrando criacao, comentarios, anexos, mudancas de status, aprovacao, reprovacao e cancelamento de aprovacao.
- Comentarios administrativos e do portal permanecem permitidos hoje, sem bloqueio por aprovacao pendente.
- Anexos e evidencias tambem permanecem permitidos hoje, sem bloqueio por aprovacao pendente.
- Isso e compativel com a diretriz futura de permitir triagem, complemento de informacoes e preparacao da decisao sem executar a acao sensivel.

## 10. Relacao atual com SLA durante atendimento

- O atendimento atual segue atualizando SLA nas operacoes ja implementadas:
  - assuncao, atribuicao e comentario publico podem registrar primeira resposta;
  - mudanca de status aplica transicao operacional no SLA;
  - encerramento registra fechamento;
  - reabertura reabre o ciclo correspondente.
- Hoje a aprovacao pendente nao pausa SLA, nao cria SLA proprio de aprovacao e nao redefine automaticamente metas durante o atendimento.
- O motor futuro precisara decidir isso explicitamente, mas esta etapa nao altera o comportamento atual.

## 11. Cenario de assumir chamado

- Hoje assumir chamado e bloqueado quando existe aprovacao pendente bloqueante.
- Conceitualmente, o futuro motor deve distinguir assumir para triagem de assumir para execucao.
- A regra recomendada e permitir assuncao para analise e organizacao quando isso nao representar execucao sensivel, preservando a compatibilidade atual ate que essa diferenciacao seja implementada formalmente.

## 12. Cenario de atribuir ou encaminhar chamado

- Atribuir, direcionar ou transferir grupo tecnico hoje nao possuem bloqueio especifico por aprovacao pendente.
- Essas acoes devem continuar majoritariamente permitidas porque organizam a operacao, distribuem carga e viabilizam triagem.
- Excecoes futuras podem existir quando a mudanca de responsavel implicar conflito de interesse, delegacao indevida ou execucao sensivel associada.

## 13. Cenario de triagem

- Triagem deve permanecer permitida.
- O atendimento precisa continuar conseguindo classificar, complementar dados, esclarecer escopo e preparar a decisao de aprovacao.
- O motor futuro nao deve usar aprovacao pendente como desculpa para impedir diagnostico e organizacao basica do trabalho.

## 14. Cenario de comentar chamado

- Comentarios hoje permanecem permitidos mesmo com aprovacao pendente.
- Isso deve ser preservado porque comentario ajuda a esclarecer contexto, registrar justificativa, orientar ajuste e manter rastreabilidade.
- Comentario nao deve ser tratado como execucao sensivel por padrao.

## 15. Cenario de anexar evidencia

- Anexos e evidencias hoje permanecem permitidos mesmo com aprovacao pendente.
- Isso e desejavel para sustentar a decisao de aprovacao, auditoria e eventual reavaliacao.
- O motor futuro deve preservar envio de evidencia e nao bloquear esse fluxo de suporte a analise.

## 16. Cenario de execucao de acao sensivel

- O sistema atual nao modela ainda execucao por escopo sensivel detalhado, mas o bloqueio legado ja indica que determinadas pendencias nao permitem avancar.
- Conceitualmente, o futuro motor deve bloquear:
  - liberacao de acesso;
  - mudanca critica;
  - alteracao de configuracao sensivel;
  - execucao tecnica de escopo que depende de aprovacao;
  - consolidacao de fluxo sequencial, paralelo ou multinivel ainda nao satisfeito.
- A execucao sensivel nao pode ser liberada apenas porque o chamado esta em andamento ou porque a triagem foi concluida.

## 17. Cenario de alteracao de status

- O comportamento atual diferencia bem status intermediario de status final.
- Aprovacao pendente bloqueante nao impede status intermediarios, mas impede avancar para status final como `Resolvido`, `Encerrado`, `Cancelado` ou equivalente final.
- Essa diretriz e compativel com o motor futuro: alteracoes intermediarias podem seguir quando nao representarem liberacao ou conclusao do escopo sensivel.

## 18. Cenario de resolucao

- Resolver chamado deve ser avaliado com mais rigor quando a resolucao depende do objeto aprovado.
- No fluxo atual, a protecao pratica aparece sobretudo nas transicoes finais.
- Conceitualmente, o futuro motor deve bloquear resolucao quando o escopo resolvido depender da aprovacao pendente, reprovada, cancelada ainda necessaria ou expirada sem substituicao valida.

## 19. Cenario de encerramento

- O fluxo atual ja bloqueia encerramento quando existe aprovacao pendente bloqueante.
- Isso deve ser preservado como regra minima de compatibilidade.
- No futuro, o motor deve refinar a decisao por escopo para impedir encerramento usado como atalho para mascarar ausencia de decisao formal.

## 20. Cenario de cancelamento

- Cancelamento do chamado pode continuar existindo como acao operacional em cenarios validos, mas nao deve apagar o historico da aprovacao.
- O futuro motor deve exigir auditoria da motivacao e preservar a trilha da aprovacao pendente, aprovada, reprovada, cancelada ou expirada relacionada ao chamado.
- Cancelar chamado nao pode virar mecanismo informal para esconder pendencia impeditiva.

## 21. Cenario de reabertura

- O fluxo atual bloqueia reabertura quando existe aprovacao pendente bloqueante.
- Se a aprovacao ja foi reprovada ou cancelada, a reabertura hoje pode ocorrer.
- Conceitualmente, o futuro motor deve reavaliar escopo e exigir nova aprovacao quando a reabertura retomar acao sensivel nao mais coberta pela decisao anterior.

## 22. Atendimento com aprovacao pendente

- Permitir consulta, historico, comentario, anexo, evidencia, triagem, organizacao e analise.
- Bloquear execucao sensivel, resolucao final, encerramento, avancos finais e consolidacoes que dependem de decisao formal.
- Nao tratar pendencia como bloqueio total do chamado.

## 23. Atendimento com aprovacao aprovada

- Liberar apenas o escopo aprovado.
- Manter bloqueio para outros escopos, outras acoes sensiveis ou outras regras ainda nao satisfeitas.
- Nao tratar aprovacao aprovada como liberacao total e irrestrita do atendimento.

## 24. Atendimento com aprovacao reprovada

- Preservar a rejeicao como decisao valida.
- Bloquear o escopo rejeitado e impedir conclusao do chamado como se aprovado estivesse.
- Permitir ajuste, comentario, evidencia e eventual nova solicitacao apenas quando houver mudanca real, nova evidencia ou decisao administrativa auditada.

## 25. Atendimento com aprovacao cancelada

- Preservar o cancelamento como parte do historico.
- Nao liberar automaticamente a acao original.
- Reavaliar necessidade de nova aprovacao se a acao sensivel voltar a ser pretendida no atendimento.

## 26. Atendimento com aprovacao expirada futura

- A expiracao ainda nao existe como comportamento funcional no fluxo atual, mas a compatibilidade futura deve prever que expiracao nao libera acao automaticamente.
- O caminho futuro pode incluir manutencao do bloqueio, escalonamento, nova solicitacao, retorno para ajuste ou sinalizacao operacional, conforme regra a ser implementada.
- Ate la, o comportamento legado nao deve ser alterado.

## 27. Atendimento de chamados legados

- Chamados legados nao devem ser reprocessados automaticamente no atendimento.
- O fluxo deve respeitar historico, decisoes antigas, bloqueios legados e ausencia de campos novos.
- Reavaliacao de legado deve ocorrer apenas com gatilho claro, alteracao sensivel ou revisao manual auditada.

## 28. Acoes que devem permanecer permitidas com aprovacao pendente

- consultar chamado;
- visualizar detalhes;
- visualizar historico;
- consultar status da aprovacao;
- comentar;
- anexar evidencia;
- baixar anexos;
- complementar informacoes;
- corrigir dados nao sensiveis;
- registrar triagem;
- registrar observacao;
- atribuir ou encaminhar para organizacao operacional, quando isso nao representar execucao sensivel;
- preparar analise tecnica sem executar a acao bloqueada.

## 29. Acoes que devem ser bloqueadas com aprovacao pendente bloqueante

- executar servico sensivel;
- liberar acesso;
- aplicar mudanca critica;
- alterar configuracao critica;
- concluir acao sensivel;
- resolver chamado quando a resolucao depende do escopo pendente;
- encerrar chamado que depende da aprovacao;
- avancar para status final;
- consolidar fluxo sequencial, paralelo ou multinivel ainda nao satisfeito;
- executar custo ou risco nao aprovado.

## 30. Acoes que devem ser bloqueadas apos rejeicao impeditiva

- executar o escopo rejeitado;
- concluir o chamado como se aprovado estivesse;
- repetir acao sensivel sem ajuste real;
- gerar nova solicitacao sem mudanca concreta de contexto;
- avancar fluxo dependente da aprovacao rejeitada;
- encerrar como resolvido quando o objeto principal foi negado;
- liberar acesso ou servico explicitamente reprovado.

## 31. Acoes que exigem reavaliacao por alteracao sensivel

- mudanca de natureza ITSM;
- mudanca de tipo de chamado;
- troca de servico solicitado;
- aumento de impacto;
- aumento de urgencia;
- mudanca material de prioridade derivada;
- inclusao de custo;
- aumento de custo;
- inclusao ou aumento de risco;
- mudanca de ambiente;
- mudanca de escopo tecnico;
- troca de responsavel com impacto de governanca;
- alteracao de configuracao sensivel.

## 32. Compatibilidade com auditoria do atendimento

- O motor futuro deve tornar auditavel:
  - quem tentou a acao;
  - qual acao tentou;
  - qual aprovacao bloqueou ou liberou;
  - qual escopo estava envolvido;
  - qual regra se aplicava;
  - qual status existia no momento;
  - qual efeito foi aplicado.
- Comentarios, historicos e anexos atuais ja oferecem parte da rastreabilidade funcional e devem ser preservados.

## 33. Compatibilidade com SLA

- Nesta etapa, o SLA atual do atendimento permanece inalterado.
- Futuramente, o motor deve definir explicitamente se aprovacao pendente pausa SLA, cria SLA proprio, gera escalonamento ou apenas sinaliza risco.
- Enquanto essa definicao nao for implementada, o sistema nao deve quebrar calculos, dashboards ou eventos atuais de SLA do atendimento.

## 34. Diretrizes para encaixe futuro do motor

- Chamar o motor antes de acoes operacionais sensiveis.
- Passar contexto da acao solicitada, nao apenas o estado geral do chamado.
- Avaliar aprovacao existente, escopo, status, pendencia, rejeicao, cancelamento e expiracao.
- Retornar decisao clara: `Permitido`, `PermitidoComSinalizacao`, `Bloqueado`, `RequerNovaAprovacao` ou `RequerReavaliacao`.
- Preservar comentario, anexo, evidencia e triagem.
- Evitar duplicidade de aprovacao.
- Auditar tentativas bloqueadas e liberacoes relevantes.

## 35. Diretrizes para preservar comportamento atual

- Nao alterar atendimento comum nesta etapa.
- Nao alterar comentarios nem anexos.
- Nao alterar fluxo atual de historico.
- Nao alterar SLA nesta etapa.
- Nao alterar status atuais.
- Nao alterar o bloqueio simples legado.
- Nao alterar frontend/admin.
- Nao criar dependencia de dados que ainda nao existem.
- Nao bloquear tudo por padrao.

## 36. Riscos de seguranca e governanca

- bloquear atendimento comum e travar a operacao;
- permitir execucao sensivel sem aprovacao;
- encerrar chamado com aprovacao pendente;
- resolver chamado ignorando rejeicao impeditiva;
- cancelar chamado para esconder pendencia;
- reabrir chamado e executar escopo sensivel sem reavaliacao;
- tratar aprovacao aprovada como liberacao total;
- ignorar aprovacao cancelada ou expirada;
- nao auditar tentativas bloqueadas;
- quebrar SLA e relatorios atuais;
- depender apenas de `AguardandoAprovacao`;
- depender apenas de `BloqueiaAvancoAtendimento` sem escopo.

## 37. Decisoes adiadas para proximos itens

- onde exatamente chamar o motor no atendimento;
- quais use cases serao interceptados;
- como diferenciar tecnicamente triagem de execucao;
- como auditar tentativa bloqueada;
- como representar bloqueio parcial por escopo;
- como exibir bloqueio no frontend/admin;
- como tratar SLA durante aprovacao pendente;
- como tratar reabertura com aprovacao antiga;
- como tratar cancelamento do chamado com aprovacao pendente;
- como testar regressao do atendimento;
- como migrar comportamento legado.

## 38. Conclusao tecnica

O fluxo atual de atendimento ja oferece uma base valida para compatibilidade com o futuro motor de aprovacoes, desde que a integracao preserve o principio de separar triagem e suporte operacional de execucao sensivel. Aprovacao pendente bloqueante deve continuar impedindo avancos finais ou sensiveis, mas comentario, anexo, evidencias e organizacao operacional nao devem ser sacrificados por um bloqueio generico.

## 39. Proxima etapa recomendada

Executar o item 28 da Sprint 4: avaliar compatibilidade com SLA atual.
