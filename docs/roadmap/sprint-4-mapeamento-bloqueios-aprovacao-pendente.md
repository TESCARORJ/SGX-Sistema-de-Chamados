# Sprint 4 - Mapeamento de Bloqueios por Aprovação Pendente

## 1. Objetivo do mapeamento

Mapear tecnicamente os pontos do fluxo de chamado em que uma aprovacao pendente ja bloqueia o avancar do atendimento hoje, os pontos que ainda nao bloqueiam mas devem ser avaliados pelo futuro motor reutilizavel de aprovacoes ITSM, e as acoes que em principio nao devem ser bloqueadas.

## 2. Limites desta etapa

- Esta etapa registra apenas mapeamento tecnico, documentacao e atualizacao do roadmap/checklist.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de endpoint, controller, tela ou service frontend novo.
- Nao houve implementacao funcional nova.
- Nao houve alteracao em `BloqueiaAvancoAtendimento`.
- Nao houve alteracao no fluxo atual de aprovacao nem no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.

## 3. Arquivos analisados

- `src/SGX.SistemaChamado.Domain/Entities/AprovacaoChamado.cs`
- `src/SGX.SistemaChamado.Domain/Enums/StatusAprovacaoChamado.cs`
- `src/SGX.SistemaChamado.Domain/Enums/StatusChamadoEnum.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/AprovacaoChamadoHelper.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoFilaAdminUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AlterarStatusChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/EncerrarChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ReabrirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AtribuirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AlterarPrioridadeChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AlterarCategoriaChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ComentarChamadoAdminUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ChamadoAprovacoesUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/ComentariosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/AnexosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/AbrirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/ComentarChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/AnexarArquivoChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/ObterStatusAprovacaoChamadoPortalUseCase.cs`
- `src/SGX.SistemaChamado.Application/Services/AcoesChamadoService.cs`
- `src/SGX.SistemaChamado.Application/Services/Sla/SlaService.cs`
- `src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs`
- `tests/SGX.SistemaChamado.Tests/AlterarStatusChamadoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/ChamadoAprovacaoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/AcoesChamadoServiceTests.cs`
- `tests/SGX.SistemaChamado.Tests/AbrirChamadoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/AnexosChamadoUseCasesTests.cs`
- `tests/SGX.SistemaChamado.Tests/ComentariosChamadoUseCasesTests.cs`
- `docs/APROVACAO-CHAMADOS.md`
- `docs/STATUS-ITSM-ESPECIFICOS.md`
- `docs/roadmap/sprint-4-mapeamento-fluxo-atual-aprovacao.md`

## 4. Conceitos atuais envolvidos

- `AprovacaoChamado` e a entidade central do fluxo atual.
- `ChamadoId` faz o vinculo direto entre chamado e aprovacao.
- `StatusAprovacaoChamado` representa `Pendente`, `Aprovado`, `Reprovado` e `Cancelado`.
- `BloqueiaAvancoAtendimento` determina se uma aprovacao pendente participa do bloqueio operacional.
- `AprovacaoChamadoHelper.ObterEstado` consolida o estado atual de aprovacao que e consumido pelo backend e pelo portal.
- `PossuiAprovacaoPendenteBloqueanteAsync` e a consulta usada quando o bloqueio precisa ser checado diretamente por query.

## 5. Funcionamento atual de `BloqueiaAvancoAtendimento`

- O helper considera apenas aprovacoes `Ativo = true` e `BloqueiaAvancoAtendimento = true`.
- Se existir pelo menos uma aprovacao ativa e pendente com esse flag, o estado retornado e:
  - `RequerAprovacao = true`
  - `AprovacaoPendente = true`
  - `BloqueiaAvancoAtendimento = true`
  - mensagem de bloqueio: "Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida."
- Se nao existir pendencia bloqueante, o helper considera a ultima decisao registrada e deixa `BloqueiaAvancoAtendimento = false`.
- Aprovacoes vinculadas criadas por `ChamadoAprovacoesUseCases` sao geradas com `bloqueiaAvancoAtendimento: false`, portanto nao participam do bloqueio operacional atual.
- Aprovacoes automaticas de catalogo e solicitacoes administrativas do modulo principal mantem o comportamento bloqueante padrao.

## 6. Funcionamento atual de aprovacao pendente

- A abertura via portal nao e bloqueada: o chamado pode nascer com aprovacao pendente quando o catalogo exige aprovacao.
- A consulta do portal apenas informa o estado da aprovacao; ela nao movimenta o chamado.
- O bloqueio atual acontece em pontos operacionais especificos do atendimento, nao como trava universal sobre qualquer acao.
- O status `AguardandoAprovacao` existe no cadastro de status e nas regras por natureza, mas o fluxo atual nao move automaticamente o chamado para esse status quando a aprovacao fica pendente.

## 7. Pontos que ja bloqueiam hoje

### Grupo 1 - Bloqueios ja implementados no codigo

1. Assumir chamado administrativo:
   `AssumirChamadoUseCase` consulta `AprovacaoChamadoHelper.ObterEstado` e rejeita a operacao quando `BloqueiaAvancoAtendimento = true`.
2. Assumir chamado da fila:
   `AssumirChamadoFilaAdminUseCase` aplica a mesma verificacao antes de atribuir responsavel individual.
3. Reabrir chamado:
   `ReabrirChamadoUseCase` bloqueia a reabertura se houver aprovacao pendente bloqueante.
4. Alterar status para fechamento operacional:
   `AlterarStatusChamadoUseCase` bloqueia mudanca para `Resolvido`, `Encerrado`, `Cancelado`, `Concluida` ou qualquer status final quando `PossuiAprovacaoPendenteBloqueanteAsync` retorna verdadeiro.
5. Encerrar chamado:
   `EncerrarChamadoUseCase` bloqueia encerramento quando existe aprovacao pendente bloqueante.
6. Exibicao de acoes administrativas:
   `AcoesChamadoService` retira a acao `Assumir` quando `BloqueiaAvancoAtendimento = true` e faz validacao preventiva para `Assumir` e `Reabrir`.

### Observacoes importantes sobre o que ja bloqueia

- O bloqueio atual depende de aprovacao pendente e bloqueante; nao basta existir qualquer aprovacao.
- Aprovacao `Reprovado` nao mantem `BloqueiaAvancoAtendimento = true` no estado consolidado atual.
- Aprovacao `Cancelado` tambem nao mantem o bloqueio.
- O bloqueio atual e focado em impedir avancos operacionais, principalmente inicio e fechamento de atendimento.

## 8. Pontos que devem bloquear no futuro motor

### Grupo 2 - Pontos que devem ser avaliados pelo motor reutilizavel

1. Iniciar atendimento de chamado sensivel sem aprovacao decidida.
2. Atribuir ou direcionar responsavel para execucao operacional de chamado sensivel sem decisao formal.
3. Avancar status operacional intermediario quando a natureza do chamado exigir decisao previa.
4. Concluir chamado sem aprovacao exigida decidida.
5. Encerrar chamado sem aprovacao exigida decidida.
6. Cancelar operacionalmente um chamado sensivel quando a regra futura exigir decisao anterior ou justificativa de governanca.
7. Reclassificar chamado para natureza, tipo, impacto, urgencia, custo ou risco que passe a exigir aprovacao.
8. Alterar servico solicitado para servico sensivel que passe a exigir aprovacao.
9. Alterar prioridade, categoria, subcategoria, tipo de solicitacao, departamento ou local quando isso representar mudanca sensivel de escopo.
10. Prosseguir com mudanca emergencial sem fluxo minimo de aprovacao definido.
11. Liberar solicitacao de acesso sem aprovacao formal.
12. Prosseguir com chamado com custo, risco ou impacto relevante sem decisao formal.

### Observacoes sobre bloqueios futuros

- O motor futuro deve decidir por regra configuravel o que bloqueia e o que apenas sinaliza.
- O bloqueio futuro nao deve depender exclusivamente de um booleano fixo na aprovacao; ele deve considerar natureza ITSM, tipo, servico, risco, custo, grupo aprovador e nivel.
- O status `AguardandoAprovacao` e um candidato natural para representar o bloqueio de forma mais explicita no fluxo futuro, mas isso ainda nao esta implementado.

## 9. Pontos que nao devem bloquear

### Grupo 3 - Acoes que normalmente devem seguir permitidas

1. Consultar chamado.
2. Listar chamados.
3. Consultar historico e linha do tempo.
4. Consultar status da aprovacao.
5. Registrar comentario interno administrativo, desde que nao represente decisao operacional.
6. Registrar comentario do solicitante.
7. Anexar evidencia.
8. Listar e baixar anexos.
9. Consultar SLA e indicadores associados.
10. Registrar auditoria e historico tecnico.
11. Cancelar a solicitacao de aprovacao, quando a regra administrativa permitir.
12. Cancelar o chamado, se a regra de negocio futura assim permitir e a governanca aceitar esse comportamento.

## 10. Impacto esperado por tipo de acao do chamado

| Tipo de acao | Situacao atual | Leitura para o motor futuro |
|---|---|---|
| Abertura | Permitida; pode gerar aprovacao pendente automatica | Continuar permitindo abertura, mas decidir se o chamado nasce em espera operacional |
| Assumir | Ja bloqueia quando a aprovacao e bloqueante | Manter bloqueio para cenarios sensiveis |
| Atribuir | Nao bloqueia hoje | Avaliar bloqueio quando a atribuicao implicar inicio de execucao |
| Alterar status intermediario | Nao bloqueia hoje | Avaliar por natureza, servico e risco |
| Alterar status final | Ja bloqueia quando a aprovacao e bloqueante | Manter e generalizar por regra |
| Encerrar | Ja bloqueia quando a aprovacao e bloqueante | Manter e generalizar por regra |
| Reabrir | Ja bloqueia quando a aprovacao e bloqueante | Reavaliar regra conforme o motivo da reabertura |
| Reclassificar dados sensiveis | Nao bloqueia hoje | Deve poder exigir nova aprovacao |
| Comentarios e anexos | Nao bloqueiam hoje | Em principio devem permanecer liberados |
| SLA | Nao bloqueia nem pausa explicitamente por aprovacao | Precisa de regra explicita futura se a pendencia de aprovacao pausar ou nao os marcos |

## 11. Impacto esperado em abertura de chamado

- Hoje a abertura pelo portal continua permitida mesmo quando o servico do catalogo requer aprovacao.
- A aprovacao pendente e criada como efeito da abertura, nao como impedimento da abertura.
- O chamado retorna com `AprovacaoPendente = true` para consulta posterior.
- O motor futuro deve decidir se a abertura continua igual ou se o chamado deve nascer em estado operacional mais explicito, como `AguardandoAprovacao`.

## 12. Impacto esperado em atendimento

- Hoje o bloqueio mais claro no atendimento e impedir `Assumir` e `Assumir da fila`.
- Atribuicao administrativa de responsavel ainda nao checa aprovacao pendente bloqueante.
- Comentarios administrativos publicos e internos continuam permitidos.
- Anexos continuam permitidos.
- Isso mostra que o sistema atual bloqueia avancos operacionais especificos, mas ainda nao congela o atendimento inteiro.

## 13. Impacto esperado em movimentacao de status

- Hoje status intermediarios continuam permitidos mesmo com aprovacao pendente bloqueante.
- O teste `AprovacaoPendenteBloqueanteNaoImpedeStatusIntermediario` confirma esse comportamento.
- Hoje o bloqueio so e aplicado quando o status representa fechamento operacional.
- O motor futuro deve separar:
  - status que podem continuar para triagem ou espera;
  - status que representam execucao sensivel;
  - status de conclusao/encerramento.

## 14. Impacto esperado em conclusao e encerramento

- Hoje `AlterarStatusChamadoUseCase` barra mudanca para `Resolvido`, `Encerrado`, `Cancelado`, `Concluida` e outros finais quando existe aprovacao pendente bloqueante.
- Hoje `EncerrarChamadoUseCase` tambem barra a operacao.
- Isso cria uma protecao parcial, suficiente para impedir alguns fechamentos sem decisao formal.
- O motor futuro deve consolidar essa regra de forma mais semantica: fechar, concluir, resolver ou executar servico sensivel nao deve acontecer sem decisao exigida.

## 15. Impacto esperado em cancelamento e reabertura

- Hoje cancelar aprovacao e permitido conforme os fluxos administrativos de aprovacao.
- Hoje cancelar chamado por alteracao de status final cai na mesma protecao usada para fechamento operacional.
- Hoje reabrir chamado e bloqueado quando existe aprovacao pendente bloqueante.
- O motor futuro precisa decidir se:
  - cancelamento do chamado continua permitido com aprovacao pendente;
  - reabertura de chamado reativa exigencia de aprovacao anterior;
  - reabertura apos rejeicao exige nova solicitacao.

## 16. Impacto esperado em reclassificacao de dados sensiveis

- Hoje `AlterarPrioridadeChamadoUseCase` e `AlterarCategoriaChamadoUseCase` nao checam aprovacao pendente.
- Ambas as operacoes podem recalcular SLA e alterar o contexto operacional do chamado.
- Hoje nao existe mecanismo que reavalie aprovacao quando esses dados mudam.
- No motor futuro, reclassificacao para natureza, categoria, prioridade, servico, impacto, urgencia, custo ou risco sensivel deve poder:
  - exigir nova aprovacao;
  - invalidar decisao anterior;
  - colocar o chamado novamente em espera operacional.

## 17. Impacto esperado em comentarios e anexos

- Hoje comentarios administrativos e do portal seguem permitidos.
- Hoje anexos administrativos e do portal seguem permitidos.
- Historico e auditoria dessas acoes tambem seguem ativos.
- Em principio, comentarios e anexos devem continuar liberados por serem evidencia, colaboracao e rastreabilidade, nao decisao operacional.
- A unica ressalva futura e impedir que um comentario ou anexo substitua indevidamente a decisao formal de aprovacao.

## 18. Impacto esperado em SLA

- Hoje nao existe regra explicita de SLA reagindo a aprovacao pendente.
- O SLA e inicializado na abertura, pode registrar primeira resposta em atribuicao, assuncao ou comentario publico administrativo, e pode ser recalculado por categoria/prioridade.
- O bloqueio atual afeta o SLA apenas de forma indireta, porque:
  - se `Assumir` bloqueia, a primeira resposta pode demorar mais;
  - se `Encerrar` bloqueia, o marco de resolucao pode ficar em aberto por mais tempo.
- O status `AguardandoAprovacao` existe e a documentacao de status o descreve com pausa de SLA, mas o fluxo atual de aprovacao nao movimenta automaticamente o chamado para esse status.
- Essa e uma lacuna importante para o desenho futuro do motor.

## 19. Lacunas encontradas

1. Nao existe bloqueio universal para chamado com aprovacao pendente.
2. `AtribuirChamadoUseCase` nao consulta aprovacao pendente bloqueante.
3. Alteracoes de prioridade, categoria e classificacao operacional nao disparam reavaliacao de aprovacao.
4. Status intermediarios continuam liberados mesmo quando pode haver risco operacional.
5. O sistema possui status `AguardandoAprovacao`, mas ele nao e usado automaticamente pelo fluxo atual.
6. Aprovacoes vinculadas criadas no detalhe usam `BloqueiaAvancoAtendimento = false`, o que torna possivel ter aprovacao pendente sem impacto operacional.
7. SLA nao possui regra explicita de pausa ou retomada por aprovacao pendente.
8. Reprovacao hoje sinaliza decisao, mas nao estabelece um bloqueio operacional padronizado futuro para todas as acoes sensiveis.

## 20. Riscos de compatibilidade

1. Tornar o bloqueio universal no futuro pode quebrar o fluxo atual de comentarios, anexos e acompanhamento.
2. Bloquear todos os status intermediarios pode interromper triagem legitima que hoje funciona.
3. Pausar SLA automaticamente por aprovacao pendente pode alterar indicadores existentes e metas ja homologadas tecnicamente.
4. Reclassificacao com reabertura de aprovacao pode afetar chamados ja em andamento e integrações que assumem continuidade direta.
5. Aprovacoes vinculadas nao bloqueantes hoje podem ter uso apenas informativo; tratar todas como bloqueantes no futuro pode causar regressao.

## 21. Conclusao tecnica

O sistema atual ja possui um mecanismo real de bloqueio por aprovacao pendente, mas ele e parcial e intencionalmente concentrado em poucos pontos operacionais: assumir, reabrir, alterar para status final e encerrar. O comportamento vigente depende de `BloqueiaAvancoAtendimento`, nao da simples existencia de uma aprovacao pendente. Isso confirma que a base funcional atual serve como fundacao, mas ainda nao representa um motor reutilizavel de aprovacao ITSM com governanca completa sobre inicio, execucao, reclassificacao e fechamento de chamados sensiveis.

## 22. Proxima etapa recomendada

Definir o conceito do motor de aprovacao ITSM reutilizavel, deixando explicito:

- o que caracteriza aprovacao obrigatoria;
- quando o bloqueio deve ser apenas informativo ou realmente impeditivo;
- quais acoes o motor passa a governar por tipo de chamado, natureza, servico, risco, custo e impacto operacional.
