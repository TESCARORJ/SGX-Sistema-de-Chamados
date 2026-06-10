# Sprint 4 - Compatibilidade com SLA atual

## 1. Objetivo da avaliacao

Avaliar conceitualmente como o futuro motor de aprovacoes ITSM deve conviver com o SLA atual do chamado sem alterar retroativamente prazos, sem pausar automaticamente o atendimento, sem criar violacoes artificiais e sem mascarar atrasos causados por pendencia de aprovacao.

## 2. Limites desta etapa

- Esta etapa registra apenas avaliacao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de pausa de SLA por aprovacao.
- Nao houve implementacao de SLA proprio de aprovacao.
- Nao houve implementacao de escalonamento automatico por aprovacao.
- Nao foram alterados `ISlaService`, `SlaService`, `ChamadoSla`, `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou `AguardandoAprovacao`.
- Nao houve alteracao no fluxo atual de abertura, atendimento, encerramento ou reabertura.
- Nao foram criadas entidades novas nem migrations estruturais.
- Nao houve homologacao nem aceite final.

## 3. Contexto atual do SLA

O SLA atual do SGX mede o ciclo operacional do chamado. Hoje ele ja cobre:

- inicializacao na abertura;
- definicao de prazo de primeira resposta e prazo de resolucao;
- registro de primeira resposta em eventos operacionais;
- ajuste por mudanca de status;
- encerramento da resolucao;
- reabertura do ciclo de resolucao;
- indicadores de situacao do SLA e historico de eventos.

O principio central desta compatibilidade e: aprovacao pendente nao deve alterar o SLA atual automaticamente nesta etapa.

## 4. Componentes atuais envolvidos no SLA

- `ISlaService` define operacoes de inicializacao, primeira resposta, mudanca de status, encerramento e reabertura.
- `SlaService` implementa o comportamento atual do SLA.
- `ChamadoSla` guarda prazos, datas de resposta/resolucao, pausa, violacao e minutos acumulados.
- `SlaCalculator` e `MetaSla` definem prazos conforme politica ativa, prioridade, categoria e departamento.
- `SlaEventService` registra eventos de SLA.
- `AbrirChamadoUseCase` inicializa SLA na criacao.
- `AssumirChamadoUseCase`, `AtribuirChamadoUseCase` e `ComentarChamadoAdminUseCase` podem registrar primeira resposta.
- `AlterarStatusChamadoUseCase` aciona efeitos de SLA conforme status.
- `EncerrarChamadoUseCase` registra resolucao/fechamento.
- `ReabrirChamadoUseCase` reabre o ciclo de resolucao.
- `AdminUseCaseHelpers` expoe situacao, prazos, pausas e historico de SLA no detalhe administrativo.

## 5. SLA na abertura do chamado

- O SLA e inicializado em `AbrirChamadoUseCase` por `slaService.InicializarNaAberturaAsync`.
- O calculo usa a politica mais adequada ao chamado, considerando prioridade, categoria, departamento e, quando houver, `SlaPadraoId` preferencial do catalogo.
- Se nao houver politica ativa ou meta aplicavel, o sistema nao quebra; apenas nao cria `ChamadoSla`.
- A abertura atual nao pausa SLA por aprovacao pendente nem cria cronometro separado de aprovacao.

## 6. SLA na primeira resposta

- A primeira resposta pode ser registrada ao assumir chamado, atribuir responsavel, comentar publicamente no atendimento administrativo ou entrar em `EmAtendimento`.
- O registro acontece apenas na primeira vez; tentativas posteriores nao sobrescrevem a data original.
- O sistema calcula se a primeira resposta foi cumprida ou violada com base no prazo da politica aplicada.
- Aprovacao pendente hoje nao impede tecnicamente esse registro nem cria excecao especifica de SLA.

## 7. SLA durante atendimento

- Durante o atendimento o SLA segue contando normalmente, salvo a pausa ja existente por politica quando o chamado entra em `AguardandoSolicitante`.
- O atendimento atual nao possui pausa por `AguardandoAprovacao`, por aprovacao pendente, por cancelamento de aprovacao ou por expiracao.
- O estado de aprovacao pode bloquear certas acoes operacionais, mas hoje nao altera o cronometro do SLA do chamado.

## 8. SLA na alteracao de status

- `AplicarMudancaStatusAsync` trata o impacto do status sobre o SLA.
- Se a politica tiver `PausarQuandoAguardandoSolicitante = true`, entrar em `AguardandoSolicitante` pausa o SLA e sair desse status retoma o prazo.
- Entrar em `EmAtendimento` pode registrar primeira resposta.
- Entrar em status final ou em `Resolvido`/`Encerrado` registra resolucao.
- Nao existe hoje efeito nativo de `AguardandoAprovacao` sobre pausa, retomada ou recalculo de SLA.

## 9. SLA no encerramento

- `EncerrarChamadoUseCase` chama `RegistrarEncerramentoAsync`.
- O SLA registra a data de resolucao, calcula minutos decorridos e marca se a resolucao foi cumprida ou violada.
- O registro e idempotente do ponto de vista funcional: nao sobrescreve resolucao ja gravada.
- Aprovacao pendente bloqueante hoje impede o encerramento operacional, mas nao pausa nem recalcula o SLA enquanto a decisao nao sai.

## 10. SLA na reabertura

- `ReabrirChamadoUseCase` chama `slaService.ReabrirAsync`.
- A reabertura reinicia o ciclo de resolucao com novo prazo, mantendo a logica operacional do chamado.
- A reabertura nao cria hoje prazo separado de aprovacao nem considera aprovacao antiga como eixo de calculo do SLA.
- Se no futuro a reabertura retomar escopo sensivel, isso pode exigir nova aprovacao, mas essa regra ainda nao existe no SLA atual.

## 11. Relacao atual entre aprovacao e SLA

- Hoje aprovacao e SLA coexistem, mas nao possuem acoplamento temporal formal.
- Aprovacao pendente pode bloquear certas acoes do chamado, porem nao pausa automaticamente o SLA.
- Nao existe SLA proprio de aprovacao.
- Nao existe violacao de prazo de aprovacao.
- Nao existe escalonamento de aprovacao integrado ao motor atual de SLA.

## 12. Aprovacao pendente e SLA

- No estado atual, aprovacao pendente apenas convive com o SLA do chamado.
- Ela pode aumentar risco operacional de violacao, mas nao para o cronometro por si so.
- Futuramente existem tres possibilidades conceituais validas:
  - nao pausar o SLA do chamado e apenas sinalizar risco;
  - pausar o SLA operacional quando a pendencia for bloqueante e externa ao time executor;
  - criar SLA proprio de aprovacao separado do SLA do chamado.
- Nesta etapa, nenhuma dessas abordagens foi implementada.

## 13. Aprovacao aprovada e SLA

- Hoje a aprovacao aprovada nao retoma SLA porque nao houve pausa por aprovacao.
- Tambem nao cria evento especifico de impacto temporal no chamado.
- Conceitualmente, no futuro, a aprovacao aprovada pode:
  - liberar o escopo aprovado;
  - registrar quanto tempo o chamado aguardou governanca;
  - retomar SLA, se houver pausa futura formal;
  - alimentar relatorios de atraso por aprovacao.

## 14. Aprovacao reprovada e SLA

- Hoje a reprovacao nao altera diretamente o SLA do chamado.
- Como nao existe regra formal de pausa por aprovacao, tambem nao existe tratamento de retomada por rejeicao.
- Futuramente, a rejeicao pode levar a encerramento, cancelamento, retorno para ajuste ou manutencao de bloqueio, e cada um desses efeitos pode demandar comportamento especifico de SLA, mas isso ainda nao foi modelado.

## 15. Aprovacao cancelada e SLA

- Hoje cancelamento de aprovacao nao zera, nao recalcula e nao mascara tempo de SLA.
- Se o chamado continuar em andamento, o SLA segue sua vida operacional normal.
- Conceitualmente, o cancelamento futuro pode remover necessidade de aprovacao ou abrir caminho para nova aprovacao, mas qualquer reflexo temporal precisa ser auditavel e nao retroativo por padrao.

## 16. Aprovacao expirada futura e SLA

- Expiracao ainda nao existe como comportamento funcional no fluxo atual.
- Conceitualmente, expiracao futura pode representar falha no prazo de decisao da aprovacao, e nao liberacao do chamado.
- O efeito futuro pode envolver:
  - escalonamento;
  - nova solicitacao;
  - retorno para ajuste;
  - violacao de SLA proprio de aprovacao.
- Nada disso deve alterar automaticamente o SLA atual do chamado nesta etapa.

## 17. SLA operacional do chamado versus prazo de aprovacao

- O SLA do chamado mede atendimento operacional.
- O prazo de aprovacao mede tempo de decisao de governanca.
- Esses dois cronometros podem se relacionar, mas nao devem ser confundidos.
- Um atraso de aprovador pode impactar o prazo operacional do chamado, mas isso nao significa que o SLA atual do chamado deva ser recalculado automaticamente.

## 18. Possivel SLA proprio de aprovacao

- O futuro motor pode precisar de SLA proprio para aprovacao, separado do chamado.
- Esse SLA proprio poderia medir:
  - tempo entre solicitacao e decisao;
  - tempo por nivel;
  - tempo por ramo;
  - tempo para quorum;
  - tempo ate escalonamento;
  - tempo ate expiracao.
- Essa possibilidade foi avaliada como plausivel, mas explicitamente adiada.

## 19. Possivel pausa do SLA por aprovacao pendente

- A pausa futura so seria aceitavel se for:
  - explicita;
  - baseada em regra;
  - auditavel;
  - limitada ao periodo de pendencia bloqueante;
  - reversivel em aprovacao, rejeicao, cancelamento ou expiracao;
  - compativel com relatorios e dashboards.
- A recomendacao desta etapa e nao aplicar pausa automatica por aprovacao pendente no modelo atual.

## 20. Possivel escalonamento por aprovacao

- Escalonamento futuro pode existir por prazo de aprovacao, prioridade, impacto, urgencia, servico sensivel, grupo aprovador ou ausencia de decisao.
- Esse escalonamento nao precisa ser o mesmo do SLA operacional do chamado.
- A avaliacao recomenda tratar escalonamento de aprovacao como trilha de governanca separada, ainda que relacionada ao risco de violacao do chamado.

## 21. Compatibilidade com SLA em chamados legados

- Chamados legados nao devem ter SLA recalculado retroativamente por regra nova de aprovacao.
- Historicos e violacoes antigas devem ser preservados.
- Aprovacoes antigas sem prazo proprio nao devem gerar violacao retroativa de aprovacao.
- A ausencia de trilha temporal de aprovacao em legado nao pode distorcer relatorios atuais de SLA.

## 22. Compatibilidade com SLA em aprovacao simples

- Em aprovacao simples, o SLA do chamado deve permanecer como hoje ate regra futura explicita.
- Se surgir SLA proprio de aprovacao, ele deve medir solicitacao ate decisao, cancelamento ou expiracao, separado do cronometro operacional do chamado.
- A aprovacao simples nao deve ser confundida com pausa automatica do SLA do chamado no modelo atual.

## 23. Compatibilidade com SLA em aprovacao sequencial

- Cada nivel sequencial pode exigir prazo proprio futuro.
- A soma desses prazos nao deve ser confundida automaticamente com o SLA do chamado.
- O motor futuro precisara decidir se o gargalo fica no chamado, na aprovacao ou em ambos, mas isso ainda nao deve alterar a operacao atual.

## 24. Compatibilidade com SLA em aprovacao paralela

- Em aprovacao paralela, cada ramo pode ter prazo proprio futuro.
- A consolidacao final pode depender do ramo mais lento ou de regra de quorum.
- O SLA atual do chamado nao possui estrutura para representar isso; por isso, a compatibilidade recomendada e manter o SLA atual inalterado ate modelagem especifica.

## 25. Compatibilidade com SLA em aprovacao multinivel

- Aprovacao multinivel pode combinar prazo por nivel, por ramo e por consolidacao.
- O SLA atual do chamado nao modela esse comportamento.
- A avaliacao recomenda separar claramente cronometro operacional do chamado e cronometro de governanca da aprovacao antes de qualquer implementacao futura.

## 26. Relacao com violacao de SLA

- Aprovacao pendente pode contribuir para risco de violacao do chamado, mas nao deve automaticamente justificar, apagar ou recalcular a violacao sem regra formal.
- O futuro motor deve conseguir separar:
  - SLA violado por atendimento;
  - SLA comprometido por espera de aprovacao;
  - prazo de aprovacao vencido;
  - atraso de aprovador;
  - pausa formal aprovada.
- Nesta etapa, essa separacao fica apenas definida conceitualmente.

## 27. Relacao com auditoria de aprovacao

- Toda interacao futura entre aprovacao e SLA deve ser auditavel.
- A trilha minima futura deve permitir responder:
  - quando a aprovacao foi solicitada;
  - quando eventual pausa comecou;
  - quando terminou;
  - quem aprovou, reprovou, cancelou ou deixou expirar;
  - se houve escalonamento;
  - se o SLA do chamado foi afetado;
  - se o prazo proprio de aprovacao foi violado;
  - qual regra justificou o efeito temporal.

## 28. Riscos de alterar SLA retroativamente

- recalcular prazo de chamados antigos;
- criar violacao artificial onde antes nao existia;
- mascarar atraso real com pausa indevida;
- apagar historico operacional;
- distorcer dashboards e indicadores atuais;
- justificar atraso de atendimento sem trilha auditavel;
- tratar expiracao como liberacao ou neutralizacao de atraso;
- comprometer comparabilidade entre legado e motor novo.

## 29. Diretrizes para encaixe futuro do motor

- Nao alterar o SLA atual sem regra explicita.
- Separar SLA do chamado de SLA de aprovacao.
- Registrar tempo em aprovacao sem recalcular automaticamente o legado.
- Evitar pausar SLA por aprovacao apenas informativa.
- Considerar pausa apenas para pendencia bloqueante, se regra futura permitir.
- Auditar pausa, retomada, escalonamento e violacao.
- Preservar relatorios atuais.
- Tratar expiracao como evento de governanca, nao como liberacao.

## 30. Diretrizes para preservar comportamento atual

- Nao alterar inicializacao de SLA na abertura.
- Nao alterar registro de primeira resposta.
- Nao alterar efeito de mudanca de status.
- Nao alterar encerramento.
- Nao alterar reabertura.
- Nao pausar SLA por aprovacao nesta etapa.
- Nao criar SLA proprio nesta etapa.
- Nao recalcular chamados antigos.
- Nao alterar relatorios atuais.

## 31. Riscos de seguranca e governanca

- pausar SLA indevidamente e mascarar atraso real;
- nao pausar SLA quando aprovacao externa bloquear execucao em regra futura;
- misturar SLA de atendimento com prazo de aprovacao;
- gerar violacao retroativa em legado;
- apagar historico de SLA;
- usar aprovacao pendente para justificar atraso sem auditoria;
- ignorar tempo parado em aprovacao;
- escalonar aprovacao sem rastreabilidade;
- tratar expiracao como liberacao;
- quebrar indicadores atuais.

## 32. Decisoes adiadas para proximos itens

- se o SLA do chamado sera pausado por aprovacao;
- em quais cenarios a pausa sera permitida;
- se havera SLA proprio de aprovacao;
- como calcular prazo de decisao;
- como escalar aprovacao antes do vencimento;
- como tratar aprovacao sequencial, paralela e multinivel no SLA;
- como exibir tempo aguardando aprovacao;
- como auditar pausa e retomada;
- como migrar chamados antigos;
- como ajustar dashboards e relatorios;
- como testar SLA com motor ativo.

## 33. Conclusao tecnica

O SLA atual do SGX ja tem ciclo operacional bem definido e nao deve ser modificado implicitamente pela camada futura de aprovacao. A compatibilidade correta e manter separado o que o chamado mede hoje do que a governanca de aprovacao podera medir amanha, evitando recalculo retroativo, pausa automatica prematura e distorcao de indicadores.

## 34. Proxima etapa recomendada

Executar o item 29 da Sprint 4: modelar configuracao de regra de aprovacao.
