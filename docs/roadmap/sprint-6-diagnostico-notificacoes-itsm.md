# Sprint 6 - Diagnostico de Notificacoes ITSM

## 1. Titulo

Sprint 6 - Notificacoes ITSM: diagnostico tecnico do estado atual, com foco em entrada de e-mail, pontos de disparo e base existente para futura camada de notificacoes.

## 2. Objetivo

Mapear o que ja existe no SGX Sistema de Chamados para suportar notificacoes ITSM, sem implementar ainda o modulo funcional de envio, templates, fila, outbox ou preferencia de destinatarios.

## 3. Escopo

- Ler o roadmap atual e o seed da Sprint 6.
- Identificar o primeiro item aberto real da Sprint 6.
- Mapear entidades, enums, value objects, servicos e workers relacionados a e-mail e rastreabilidade.
- Identificar pontos de integracao candidatos para futuras notificacoes ITSM.
- Documentar lacunas, riscos e proxima evolucao incremental.

## 4. Fora de escopo

- Implementar envio de notificacoes.
- Criar entidade, tabela, enum ou campo novo para notificacao.
- Criar template persistido, outbox, fila ou mecanismo de agendamento.
- Alterar SLA.
- Alterar o motor de aprovacoes ITSM.
- Alterar frontend.
- Alterar regras de dominio de chamado sem necessidade tecnica.

## 5. Estado atual encontrado

O roadmap aponta a Sprint 6 como `Sprint 6 - Notificacoes ITSM`, com status geral `Planejado` e percentual `25%`.

No seed e no teste de checklist, o percentual esta coerente com o estado atual:

- 4 itens ativos de checklist.
- 1 item concluido.
- 3 itens ainda abertos.
- Percentual calculado: 1/4 = 25%.

Importante:

- `Planejado` e a classificacao de maturidade da sprint no roadmap.
- `25%` representa progresso real do checklist, nao conclusao da sprint.
- A homologacao da Sprint 5 continua registrada como posterior.

## 6. Roadmap e percentual atual

### Item de roadmap

- Area: `Sprint 6 - Notificacoes ITSM`
- Status de implementacao: `Planejado`
- Status tecnico: `NaoAvaliado`
- Percentual: `25%`
- Proxima acao registrada: `Modelar entidade Notificacao e pipeline de eventos.`

### Checklist da Sprint 6

1. `Planejar escopo e criterios de aceite` - concluido
2. `Implementar entregas centrais da sprint` - aberto
3. `Executar testes funcionais e tecnicos` - aberto
4. `Registrar homologacao e aceite` - aberto

### Leitura tecnica do percentual

O percentual de `25%` e matematicamente correto porque apenas o primeiro item do checklist foi concluido. Nao significa que a sprint esteja entregue, homologada ou pronta para producao.

## 7. Entidades e enums existentes

### Capacidades funcionalmente existentes

- `CanalNotificacao`
  - `Sistema`
  - `Email`
- `StatusProcessamentoEmail`
  - `Pendente`
  - `Processado`
  - `Ignorado`
  - `Erro`
  - `Duplicado`
  - `NaoCorrelacionado`
- `EmailCorporativo`
- `LogIntegracaoEmail`
- `TipoHistoricoChamado`
  - inclui `IntegracaoEmail`, `AprovacaoSolicitada`, `ChamadoAprovado`, `ChamadoReprovado`, `Resolvido`, `FechamentoAutomatico`, `SolucaoAceita`, `SolucaoRejeitada`, `Reaberto`, `Cancelado`
- `TipoAcaoAuditoria`
  - inclui `ResolverChamado`, `FecharChamadoAutomaticamentePorPrazoAceite`, `RejeitarSolucaoChamado`, `AceitarSolucaoChamado`, `ReabrirChamado`

### Lacunas confirmadas

- Nenhuma entidade persistente de notificacao foi localizada.
- Nenhum enum de status de envio de notificacao foi localizado.
- Nenhuma configuracao de template persistido foi localizada.
- Nenhuma fila/outbox/tentativa de envio foi localizada.

## 8. Servicos existentes

### Servicos de e-mail e correlacao

- `EmailParaChamadoService`
  - adaptador fino de DTO para o processamento de e-mail.
- `EmailMessageProcessor`
  - processa a entrada de e-mail, faz deduplicacao, correlacao, criacao/atualizacao de chamado, anexos, historico e auditoria.
- `EmailCorrelationService`
  - correlaciona respostas por codigo do chamado no assunto e por headers `InReplyTo`/`References`.

### Worker e infraestrutura

- `SGX.SistemaChamado.Worker.Email`
  - worker de ingestao IMAP.
- `EmailIngestionService`
  - leitura IMAP, encaminhamento para o use case e pos-processamento da mensagem.
- `MailKitEmailImapClient`
  - cliente IMAP.
- `EmailWorkerOptions`
  - configuracao exclusivamente IMAP/ingestao.

### Servicos relacionados ao chamado

- `HistoricoChamado`
- `EventoAuditoria`
- `IAuditoriaService`
- `ISlaService`
- use cases de aceitacao, rejeicao, reabertura, resolucao e fechamento automatico

## 9. Analise do Worker.Email

O `Worker.Email` e um worker de entrada, nao de saida.

Ele executa:

- leitura de mensagens IMAP;
- conversao para DTO;
- envio para o caso de uso de processamento;
- marcacao como lida;
- movimentacao para pastas de processadas/erro.

Ele nao executa:

- envio de notificacoes;
- disparo de e-mail de saida;
- persistencia de templates;
- regras de destinatarios;
- reprocessamento de notificacoes de negocio.

Conclusao:

- o worker atual suporta a entrada e correlacao de e-mails;
- nao deve ser confundido com infraestrutura de notificacao ITSM.

## 10. Entrada versus saida de e-mail

### Entrada

Fluxo funcional existente:

- IMAP -> `Worker.Email` -> `EmailIngestionService` -> `IProcessarEmailRecebidoUseCase` -> `EmailMessageProcessor`.

Esse fluxo:

- deduplica mensagens;
- identifica remetente;
- correlaciona respostas;
- cria chamado quando necessario;
- adiciona comentarios em chamado existente;
- processa anexos;
- grava `LogIntegracaoEmail`;
- registra historico e auditoria.

### Saida

Fluxo de saida nao identificado:

- nao ha SMTP configurado;
- nao ha servico de envio de notificacoes;
- nao ha template persistido;
- nao ha outbox;
- nao ha fila persistente de notificacao.

## 11. Eventos candidatos a notificacao

Eventos atuais que fazem sentido como gatilhos futuros:

- abertura do chamado
- triagem
- atribuicao
- encaminhamento
- comentario
- anexo
- mudanca de status
- inicio de atendimento
- aprovacao solicitada
- aprovacao aprovada
- aprovacao reprovada
- aprovacao expirada
- SLA proximo do vencimento
- SLA vencido
- resolucao
- aceite da solucao
- rejeicao da solucao
- fechamento automatico
- encerramento administrativo
- reabertura
- cancelamento

## 12. Matriz evento x destinatario x canal

| Evento | Origem/use case | Destinatarios | Canal atual | Canal futuro | Template existente | Persistencia existente | Auditoria existente | Risco de duplicidade | Status |
|---|---|---|---|---|---|---|---|---|---|
| Abertura do chamado | `AbrirChamadoUseCase`, `EmailMessageProcessor` | Solicitante, atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Parcial via historico/log | Sim | Medio | Parcial |
| Triagem | `AtribuirChamadoUseCase`, fila/gupo tecnico | Atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Historico | Sim | Medio | Parcial |
| Atribuicao | `AtribuirChamadoUseCase` | Atendente, grupo tecnico, solicitante | Nenhum | Email, in-app | Nao | Historico | Sim | Medio | Parcial |
| Encaminhamento | `TransferirGrupoTecnicoUseCase` | Grupo tecnico destino, atendente atual | Nenhum | Email, in-app | Nao | Historico | Sim | Medio | Parcial |
| Comentario | `AdicionarComentarioUseCase`, `EmailMessageProcessor` | Solicitante, atendente, observadores | Nenhum | Email, in-app | Nao | Historico e comentario persistidos | Sim | Medio | Parcial |
| Anexo | `AnexarArquivoUseCase`, `EmailMessageProcessor` | Solicitante, atendente, observadores | Nenhum | Email, in-app | Nao | Anexo + historico | Sim | Baixo | Parcial |
| Mudanca de status | `AlterarStatusChamadoUseCase` | Solicitante, atendente, gestor | Nenhum | Email, in-app | Nao | Historico | Sim | Medio | Parcial |
| Inicio de atendimento | `AssumirChamadoUseCase`, fila/admin | Solicitante, atendente | Nenhum | Email, in-app | Nao | Historico | Sim | Medio | Parcial |
| Aprovacao solicitada | Motor de aprovacoes ITSM | Aprovador, gestor, solicitante | Nenhum | Email, in-app | Nao | Historico de aprovacao | Sim | Alto | Parcial |
| Aprovacao aprovada | Motor de aprovacoes ITSM | Solicitante, atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Historico de aprovacao | Sim | Alto | Parcial |
| Aprovacao reprovada | Motor de aprovacoes ITSM | Solicitante, atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Historico de aprovacao | Sim | Alto | Parcial |
| Aprovacao expirada | Motor de aprovacoes ITSM | Solicitante, aprovador, admin | Nenhum | Email, in-app | Nao | Historico de aprovacao | Sim | Alto | Futura |
| SLA proximo do vencimento | `ISlaService`, monitoramento SLA | Atendente, gestor, admin | Nenhum | Email, in-app | Nao | EventoSla e historico tecnico | Sim | Alto | Parcial |
| SLA vencido | `ISlaService`, monitoramento SLA | Atendente, gestor, admin | Nenhum | Email, in-app | Nao | EventoSla e historico tecnico | Sim | Alto | Parcial |
| Resolucao | `ResolverChamadoUseCase` | Solicitante, atendente, gestor | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Aceite da solucao | `AceitarSolucaoChamadoUseCase` | Solicitante, atendente | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Rejeicao da solucao | `RejeitarSolucaoChamadoUseCase` | Solicitante, atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Fechamento automatico | `FecharChamadosAutomaticamentePorPrazoAceite` | Solicitante, atendente, admin | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Encerramento administrativo | `EncerrarChamadoUseCase` | Solicitante, atendente, admin | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Reabertura | `ReabrirChamadoUseCase` | Solicitante, atendente, grupo tecnico | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |
| Cancelamento | `CancelarChamadoUseCase` ou regra equivalente | Solicitante, admin, atendente | Nenhum | Email, in-app | Nao | Historico + auditoria | Sim | Medio | Parcial |

## 13. Templates existentes e lacunas

### Encontrado

- Textos hardcoded e mensagens de historico existem em varios use cases.
- O processamento de e-mail usa assunto e corpo da mensagem de entrada.

### Nao encontrado

- templates persistidos em banco;
- versionamento de templates;
- configuracao administrativa de templates;
- fallback por evento/canal/idioma;
- placeholders padronizados para notificacoes de saida.

### Leitura tecnica

Hoje existe texto operacional disperso, mas nao existe motor de template para notificacao ITSM.

## 14. Persistencia e confiabilidade

### O que existe

- `LogIntegracaoEmail` persiste rastreio de entrada.
- `HistoricoChamado` registra eventos funcionais do chamado.
- `EventoAuditoria` registra rastreabilidade de seguranca/governanca.
- `EventoSla` registra eventos de SLA.

### O que nao existe

- outbox de notificacoes;
- fila persistente de notificacoes;
- tabela de notificacoes;
- status de envio de notificacao;
- tentativas de envio de notificacao;
- reprocessamento de notificacao de negocio;
- agendamento de notificacao;
- idempotencia de envio de notificacao.

### Conclusao

O sistema possui base de rastreabilidade, mas ainda nao possui persistencia especifica para entrega de notificacao ITSM.

## 15. Idempotencia e prevencao de duplicidade

### Ja existente

- deduplicacao de e-mail por `Fingerprint` e `MessageId`;
- correlacao de respostas por headers;
- registro de tentativas em `LogIntegracaoEmail`;
- controle de duplicado no processamento de entrada.

### Ainda ausente para notificacao

- chave de idempotencia por evento de notificacao;
- deduplicacao por destinatario/canal/evento;
- reprocessamento seguro de envio;
- controle de tentativa por destino.

## 16. Auditoria e rastreabilidade

### Auditoria funcional

- `HistoricoChamado` cobre a trilha funcional do chamado.
- `TipoHistoricoChamado` ja contem varios eventos relevantes para futuras notificacoes.

### Auditoria de governanca

- `EventoAuditoria` e `TipoAcaoAuditoria` cobrem alteracoes administrativas, homologacao e eventos criticos.

### Log tecnico

- `ILogger` continua sendo o log tecnico do worker e dos servicos.
- `LogIntegracaoEmail` e o log persistente da entrada de e-mail.

### Registro de entrega

- ainda nao existe registro persistente da entrega de notificacao de saida.

## 17. Permissoes e preferencias

### Permissoes existentes

- `Notificacoes.Visualizar`
- `Notificacoes.Gerenciar`

### Preferencias de usuario

- nao foi localizado modelo persistente de preferencia por usuario.
- nao foi localizado opt-in/opt-out.
- nao foi localizado controle de notificacao obrigatoria por perfil.

### Leitura tecnica

Existem permissões administrativas para o futuro modulo, mas nao existe ainda a camada funcional de preferencia/regras de destinatario.

## 18. Integracao com abertura

- `AbrirChamadoUseCase` e o processamento de e-mail criam o ponto natural para notificacao de abertura.
- Destinatario mais provavel: solicitante e equipe de atendimento.
- Risco principal: duplicar notificacao quando a origem for e-mail e portal ao mesmo tempo.

## 19. Integracao com atendimento

- Alteracoes de fila, responsavel e status ja possuem historico.
- Notificacao futura pode ser disparada por mudanca de status, atribuicao e assuncao.
- Risco principal: excesso de notificacoes em cascata durante transicao operacional.

## 20. Integracao com aprovacao ITSM

- O motor de aprovacoes ITSM ja existe e e a principal fonte de eventos de aprovacao.
- Eventos candidatos: solicitacao, aprovacao, reprovacao, expiracao e cancelamento.
- Risco principal: reenvio duplicado em reprocessamento de aprovacao.

## 21. Integracao com SLA

- `ISlaService` e `EventoSla` sao os pontos naturais para alerta de proximidade e vencimento.
- O sistema ainda nao possui canal de notificacao de SLA.
- Risco principal: alertas repetidos por janela de monitoramento.

## 22. Integracao com resolucao, aceite e fechamento

- `ResolverChamadoUseCase`
- `AceitarSolucaoChamadoUseCase`
- `RejeitarSolucaoChamadoUseCase`
- `FecharChamadosAutomaticamentePorPrazoAceite`
- `EncerrarChamadoUseCase`

Esses casos de uso ja registram historico/auditoria e podem ser gatilhos futuros de notificacao.

## 23. Integracao com reabertura e cancelamento

- `ReabrirChamadoUseCase`
- cancelamento por regra administrativa ou funcional equivalente

Esses eventos exigem cuidadosa idempotencia para evitar tempestade de notificacoes em fluxos de idas e vindas.

## 24. Compatibilidade com fluxo legado

O desenho atual e compativel com o fluxo legado porque:

- nao altera as regras de fechamento ja consolidadas;
- nao remove o fluxo IMAP de entrada;
- reaproveita historico e auditoria ja existentes;
- preserva a Sprint 5 em `32/32` e `100%`;
- mantem a Sprint 6 aberta e sem conclusao indevida.

## 25. Riscos

- disparo duplicado por evento repetido;
- sobrecarga de notificacoes por cascata de status;
- acoplamento excessivo entre evento e canal;
- mistura de entrada e saida de e-mail;
- tentativa de criar estrutura nova antes de validar reaproveitamento de historico/auditoria;
- confusao entre `Planejado` como status e `25%` como progresso real.

## 26. Limitacoes

- nao existe motor de template;
- nao existe envio de e-mail de saida;
- nao existe outbox;
- nao existe fila persistente;
- nao existe preferencia por usuario;
- nao existe modelagem de notificacao persistida.

## 27. Decisoes adiadas

- formato final da entidade de notificacao;
- estrategia de canal unico versus multicanal;
- uso de outbox ou tabela dedicada;
- politica de reprocessamento;
- resolucao avancada de destinatarios;
- versionamento de templates;
- suporte futuro a outros canais.

## 28. Proposta incremental para os proximos itens

1. Definir o dominio minimo de notificacao persistente.
2. Definir eventos geradores e chave de idempotencia.
3. Definir template simples por evento/canal.
4. Definir fila/outbox apenas se o caso de uso exigir entrega assincrona.
5. Definir regras basicas de destinatario.
6. Integrar os primeiros eventos de chamado sem mexer no fluxo legado.

## 29. Criterios de aceite

- primeiro item real da Sprint 6 identificado;
- diagnostico documentado;
- nenhuma funcionalidade de envio implementada;
- nenhuma tabela estrutural criada;
- nenhum item futuro concluido;
- percentual da Sprint 6 matematicamente correto;
- proxima acao corresponde ao item real seguinte;
- Sprint 5 preservada em `32/32` e `100%`;
- homologacao da Sprint 5 continua posterior;
- build e validacao permanecem coerentes com o estado atual.

## 30. Conclusao

O sistema ja possui uma base solida para o futuro modulo de notificacoes ITSM, principalmente em entrada de e-mail, historico, auditoria e pontos de evento do chamado. Ainda assim, a camada de notificacao de saida nao existe de forma persistente e nao deve ser inferida a partir do `Worker.Email`.

Conclusao pratica:

- o primeiro item realmente aberto da Sprint 6 e `Implementar entregas centrais da sprint`;
- o roadmap continua com `25%` por ter 1 de 4 itens do checklist concluido;
- a proxima acao registrada no roadmap permanece `Modelar entidade Notificacao e pipeline de eventos.`
