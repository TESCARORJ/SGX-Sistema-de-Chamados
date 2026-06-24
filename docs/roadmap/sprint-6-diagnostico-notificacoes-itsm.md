# Sprint 6 - Diagnostico de Notificacoes ITSM

## 1. Objetivo

Consolidar o diagnostico tecnico das estruturas existentes relacionadas a notificacoes e eventos no SGX Sistema de Chamados, sem implementar ainda o modulo funcional de notificacoes.

## 2. Escopo

- analisar codigo, seed, roadmap, testes e documentacao ja existentes;
- identificar estruturas reutilizaveis para o futuro modulo de notificacoes ITSM;
- diferenciar historico, auditoria, eventos de origem e notificacao persistente futura;
- registrar lacunas, riscos, decisoes adiadas e proxima etapa tecnica.

## 3. Fora de escopo

- criar entidade `Notificacao`;
- criar tabela de notificacoes;
- criar templates persistentes;
- criar preferencias persistentes por usuario/evento/canal;
- criar fila, outbox, retry funcional ou worker outbound;
- integrar eventos ITSM ao envio real;
- alterar `Worker.Email`;
- alterar fluxo funcional de abertura, atendimento, aprovacao, SLA ou fechamento.

## 4. Estado atual encontrado

- Sprint 5 permanece encerrada tecnicamente em `32/32 - 100%`, com homologacao formal ainda pendente.
- Sprint 6 possui `16` itens ativos de checklist.
- Antes deste fechamento documental, apenas o item `1` estava concluido.
- O item `2` desta sprint e exclusivamente de diagnostico, documentacao, checklist e roadmap.
- Nao foi localizada entidade persistente backend equivalente a `Notificacao`.
- Nao foi localizada tabela dedicada de notificacoes no `SGXSistemaChamadoDbContext`.

## 5. Entidades existentes relacionadas

- `HistoricoChamado`: trilha operacional do chamado.
- `EventoAuditoria`: trilha de governanca, seguranca e rastreabilidade administrativa.
- `EventoSla`: historico de eventos do ciclo de SLA.
- `LogIntegracaoEmail`: log persistente do fluxo inbound de e-mail.
- `Chamado`, `ComentarioChamado` e `AnexoChamado`: fatos operacionais que poderao originar notificacoes futuras.

Conclusao:

- existem fatos persistidos e logs tecnicos reaproveitaveis como origem e rastreabilidade;
- nao existe hoje a materializacao persistente da comunicacao ao destinatario.

## 6. Enums existentes

### `CanalNotificacao`

`CanalNotificacao` e um enum localizado em `src/SGX.SistemaChamado.Domain/Enums/CanalNotificacao.cs`.

Valores atuais:

- `Sistema`
- `Email`

### Outros enums relacionados

- `StatusProcessamentoEmail`: uso exclusivo do processamento inbound de e-mail.
- `TipoHistoricoChamado`: classifica fatos operacionais do chamado.
- `TipoEventoSla`: classifica eventos do ciclo de SLA.

Conclusao:

- `CanalNotificacao` pode ser reutilizado;
- nao existe enum dedicado de status de envio/processamento de notificacoes persistentes.

## 7. Servicos existentes

- `EmailParaChamadoService`: adaptador fino de DTO para o processamento de e-mail recebido.
- `EmailMessageProcessor`: processamento principal da mensagem inbound, com deduplicacao, correlacao, criacao de chamado, comentario, anexo e `LogIntegracaoEmail`.
- `EmailCorrelationService`: correlacao por codigo de chamado no assunto e por headers `InReplyTo` e `References`.
- `ISlaService` e rotinas de SLA: origem potencial de eventos futuros, sem notificacao persistente associada hoje.

## 8. Worker.Email

O projeto `src/SGX.SistemaChamado.Worker.Email` e um worker de ingestao IMAP.

Responsabilidades confirmadas:

- ler mensagens via IMAP;
- encaminhar mensagens para `IProcessarEmailRecebidoUseCase`;
- marcar mensagem como lida, quando configurado;
- mover mensagem para pasta processada/erro;
- registrar logs tecnicos de execucao.

Conclusao:

- o `Worker.Email` processa entrada IMAP;
- nao envia notificacoes outbound;
- nao e worker de entrega de notificacoes.

## 9. Historico do chamado

`HistoricoChamado` registra fatos operacionais relacionados ao chamado, como criacao, mudanca de status, atribuicao, comentario, anexo, aprovacao, transferencia, resolucao, aceite, rejeicao, reabertura e cancelamento.

Conclusao:

- `HistoricoChamado` nao pode ser considerado notificacao persistente;
- ele registra o fato operacional, nao a comunicacao materializada para um destinatario e canal;
- pode servir como origem de eventos futuros.

## 10. Auditoria

`EventoAuditoria` registra governanca e rastreabilidade institucional com campos de usuario, modulo, entidade, acao, descricao, dados antes/depois, metadados, nivel, sucesso e erro.

Conclusao:

- `EventoAuditoria` nao substitui notificacao persistida;
- tambem nao substitui log de entrega;
- serve como trilha complementar de rastreabilidade.

## 11. Eventos de SLA

`EventoSla` registra fatos como aplicacao do SLA, alertas de proximidade/vencimento, pausa, retomada e cumprimento/violacao, inclusive com `ChaveIdempotencia`.

Conclusao:

- `EventoSla` representa evento de origem;
- pode gerar notificacoes futuras;
- nao e a notificacao em si.

## 12. Integracao de e-mail

`LogIntegracaoEmail` persiste:

- `MessageId`;
- `InReplyTo`;
- `References`;
- `Fingerprint`;
- remetente/destinatario;
- status de processamento;
- erro;
- chamado correlacionado;
- tentativas de processamento.

Conclusao:

- `LogIntegracaoEmail` pertence ao fluxo inbound;
- nao deve ser tratado como log de envio outbound;
- o retry existente e retry de processamento de entrada, nao retry funcional de notificacoes.

## 13. Permissoes

Permissoes localizadas e ja seedadas:

- `Notificacoes.Visualizar`
- `Notificacoes.Gerenciar`

Conclusao:

- as permissoes existem e devem ser reutilizadas pelo futuro modulo;
- a existencia da permissao nao significa existencia do backend persistente.

## 14. Estruturas frontend

O frontend possui:

- rota `/admin/notificacoes`;
- `AdminNotificacoesView.vue`;
- `NotificationsMenu.vue`;
- `notificacoesStore.ts`;
- `types/notificacao.ts`;
- controle visual por permissao.

A store carrega dados por `criarNotificacoesMock()` e mantem leitura local em memoria.

Conclusao:

- existe infraestrutura frontend local;
- nao ha integracao com API persistente de notificacoes;
- nao ha controle persistente de lida/nao lida.

## 15. Canais existentes

Canais confirmados em codigo:

- `Sistema`
- `Email`

## 16. Canais futuros

Canais futuros possiveis, ainda nao modelados:

- notificacao in-app persistente;
- e-mail outbound persistente;
- canais adicionais como webhook, Teams ou SMS, se algum dia aprovados.

Conclusao:

- os dois canais atuais existem apenas como enum;
- ainda nao ha adaptadores de entrega outbound por canal.

## 17. Templates e lacunas

Nao foi localizado:

- template persistente;
- tabela de templates;
- versionamento de template;
- selecao por evento/canal;
- materializacao persistida do conteudo enviado.

Conclusao:

- textos operacionais espalhados no sistema nao equivalem a um modulo de templates de notificacao.

## 18. Preferencias e lacunas

Nao foi localizado:

- preferencia persistente por usuario;
- preferencia por evento;
- preferencia por canal;
- opt-in/opt-out;
- override por perfil ou grupo para notificacoes nao obrigatorias.

Conclusao:

- nao existe preferencia persistente de notificacao.

## 19. Destinatarios e lacunas

O sistema possui relacoes e fatos que permitirao destinatarios futuros, como:

- solicitante;
- responsavel;
- grupo tecnico;
- fila de atendimento;
- aprovador;
- participantes operacionais.

Nao foi localizado hoje:

- resolvedor persistente de destinatarios para notificacao;
- composicao por usuario, perfil, grupo, aprovador, observador ou gestor;
- tabela de destinatarios materializados.

Conclusao:

- existe base de dominio para descobrir destinatarios no futuro;
- nao existe motor de resolucao de destinatarios de notificacao.

## 20. Fila, outbox e retry

Nao foi localizado:

- tabela de fila de notificacoes;
- outbox;
- controle de tentativas de envio por notificacao/canal;
- dead-letter;
- reprocessamento funcional de notificacoes.

Conclusao:

- nao existe fila ou outbox dedicada;
- nao existe retry funcional de notificacoes.

## 21. Eventos candidatos

Eventos ITSM candidatos futuros:

- abertura pelo portal;
- abertura por e-mail;
- triagem;
- atribuicao de responsavel;
- transferencia de responsavel;
- transferencia de grupo ou fila;
- comentario publico;
- comentario interno;
- anexo ou evidencia;
- mudanca de status;
- aprovacao pendente;
- aprovacao aprovada;
- aprovacao rejeitada;
- aprovacao cancelada ou expirada;
- SLA proximo do vencimento;
- SLA vencido;
- resolucao;
- aceite da solucao;
- rejeicao da solucao;
- fechamento;
- fechamento automatico;
- reabertura;
- cancelamento.

Para todos esses grupos, ainda serao necessarios:

- chave de idempotencia;
- destinatarios;
- canais;
- template;
- regra de preferencia;
- processamento;
- auditoria;
- protecao contra duplicidade.

## 22. Idempotencia

Ja existe:

- deduplicacao inbound por `MessageId` e `Fingerprint` em `LogIntegracaoEmail`;
- correlacao de resposta por assunto e headers;
- `ChaveIdempotencia` em `EventoSla`.

Ainda falta para notificacoes:

- chave de idempotencia por evento gerador de notificacao;
- controle por destinatario/canal;
- protecao contra duplicidade de entrega;
- reprocessamento seguro de envio.

## 23. Correlacao com chamado

Hoje a correlacao existente e voltada ao fluxo inbound:

- assunto com codigo do chamado;
- headers de resposta;
- relacionamento de `LogIntegracaoEmail` com `ChamadoId`.

Conclusao:

- existe base util para correlacao com chamado;
- nao existe correlacao de notificacao persistente enviada para destinatarios.

## 24. Impacto em abertura

Fontes naturais para eventos futuros:

- `AbrirChamadoUseCase`;
- `EmailMessageProcessor`.

Lacunas:

- falta materializacao da notificacao;
- falta definicao de destinatarios;
- falta evitar duplicidade entre abertura por portal e e-mail.

## 25. Impacto em atendimento

Fontes naturais para eventos futuros:

- mudancas de status;
- atribuicao;
- transferencia de grupo/fila;
- comentarios;
- anexos.

Lacunas:

- falta regra de destinatario;
- falta preferencia;
- falta protecao contra excesso de notificacoes em cascata.

## 26. Impacto em aprovacao

O motor de aprovacoes ja fornece fatos candidatos, como:

- aprovacao solicitada;
- aprovacao aprovada;
- aprovacao rejeitada;
- aprovacao cancelada;
- expiracao futura, quando aplicavel.

Lacunas:

- falta contrato interno de evento para notificacao;
- falta definicao de aprovador/demais destinatarios;
- falta protecao contra reenvio duplicado.

## 27. Impacto em SLA

Fontes naturais:

- `EventoSla`;
- monitoramento periodico de SLA.

Lacunas:

- falta canal oficial de alerta;
- falta politica de repeticao;
- falta controle para nao reenviar o mesmo alerta a cada ciclo de monitoramento.

## 28. Impacto em fechamento e reabertura

Fontes naturais:

- resolucao;
- aceite;
- rejeicao da solucao;
- fechamento;
- fechamento automatico;
- reabertura;
- cancelamento.

Lacunas:

- falta materializacao persistente;
- falta regra de quem deve ser avisado;
- falta idempotencia para fluxos de ida e volta.

## 29. Riscos

- confundir historico com notificacao entregue;
- confundir auditoria com comunicacao ao destinatario;
- misturar inbound e outbound no `Worker.Email`;
- implementar envio antes de modelar idempotencia;
- criar acoplamento forte entre evento e canal;
- gerar duplicidade ou tempestade de notificacoes.

## 30. Decisoes adiadas

- formato final da entidade `Notificacao`;
- se havera tabela unica, outbox ou combinacao das duas;
- estrategia de templates;
- estrategia de preferencias;
- politica de retry/reprocessamento;
- resolucao detalhada de destinatarios;
- expansao para canais futuros alem de `Sistema` e `Email`.

## 31. Conclusao

O estado real encontrado confirma que:

- nao existe modulo backend persistente completo de notificacoes ITSM;
- nao existe entidade persistente equivalente a `Notificacao`;
- `CanalNotificacao` e enum e possui apenas `Sistema` e `Email`;
- `HistoricoChamado` representa fatos operacionais, nao entrega de notificacao;
- `EventoAuditoria` representa rastreabilidade, nao comunicacao ao destinatario;
- `EventoSla` pode originar notificacoes futuras, mas nao e notificacao;
- `LogIntegracaoEmail` pertence ao fluxo inbound;
- o `Worker.Email` processa entrada IMAP e nao envio outbound;
- nao existe fila ou outbox dedicada;
- nao existe retry funcional de notificacoes;
- nao existe modulo persistente de templates;
- nao existe preferencia persistente por evento/canal;
- o frontend possui estruturas locais que nao equivalem a um modulo backend;
- as permissoes de notificacao ja existem e devem ser reaproveitadas.

## 32. Proxima etapa recomendada

Modelar a entidade `Notificacao` e o contrato interno de eventos.

## 33. Criterios de aceite do diagnostico

- codigo e documentacao analisados;
- perguntas obrigatorias respondidas pelo estado real encontrado;
- historico, auditoria, evento de origem e notificacao futura diferenciados;
- responsabilidade do `Worker.Email` registrada corretamente;
- lacunas de template, preferencia, destinatario, fila e retry registradas;
- eventos candidatos documentados sem declaracao de envio funcional;
- roadmap/checklist aptos a marcar apenas o item `2` como concluido.
