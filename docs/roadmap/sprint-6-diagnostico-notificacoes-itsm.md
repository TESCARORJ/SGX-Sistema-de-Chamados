# Sprint 6 - Diagnostico tecnico de Notificacoes ITSM

## 1. Objetivo

Iniciar a Sprint 6 - Notificacoes ITSM com diagnostico tecnico, inventario do estado atual e desenho incremental do modulo de notificacoes, sem implementar disparos automaticos, persistencia nova, templates funcionais, preferencias, fila, worker de envio ou integracao operacional.

O objetivo deste item e reduzir risco arquitetural antes da modelagem funcional: identificar o que ja existe, o que pode ser reaproveitado e quais decisoes devem ser adiadas para os proximos itens da Sprint 6.

## 2. Escopo

Este documento cobre:

- leitura do roadmap atual;
- inventario de entidades, enums, servicos, permissoes e pontos de integracao existentes;
- analise do Worker.Email atual;
- mapeamento de eventos ITSM candidatos a notificacao;
- identificacao de canais existentes e canais futuros;
- avaliacao inicial de templates, auditoria, rastreabilidade, idempotencia e governanca;
- proposta incremental para os proximos itens da Sprint 6.

## 3. Fora de escopo

Ficam expressamente fora deste item:

- criar tabela persistente de notificacao;
- criar tabela de template;
- criar tabela de preferencias;
- criar fila de envio;
- criar worker de envio;
- criar endpoint/API de notificacoes ITSM;
- alterar frontend para consumir notificacoes persistentes;
- alterar abertura de chamado;
- alterar atendimento;
- alterar SLA;
- alterar Motor de Aprovacoes ITSM;
- alterar regras de fechamento, aceite, rejeicao ou reabertura;
- disparar e-mail automatico;
- reaproveitar indevidamente o Worker.Email como emissor de notificacoes;
- marcar itens futuros da Sprint 6 como concluidos.

## 4. Estado atual encontrado

O roadmap principal registra o SGX como solucao com abertura, acompanhamento, atendimento, SLA, gestao administrativa e integracao de chamados por e-mail. Tambem registra que ja existe Worker IMAP, logs de integracao de e-mail, frontend Vue 3 + Quasar e uma central de notificacoes frontend/local. A Sprint 6 aparece como Notificacoes ITSM em estado planejado.

No estado atual do codigo analisado, existe capacidade parcial relacionada a notificacao, mas nao um modulo ITSM persistente de notificacoes por evento. O frontend possui tipos/store/views/componentes de notificacao local, mas isso nao deve ser tratado como modulo backend persistente. A Sprint 6 deve fechar primeiro a arquitetura de dominio, contratos, governanca e eventos antes de qualquer disparo automatico.

Foi identificada uma divergencia de base: o contexto operacional informado indica Sprint 5 fechada em 32/32 itens e 100%, enquanto o `main` inspecionado ainda registra a Sprint 5 como em desenvolvimento/parcial em alguns pontos do roadmap/seed. Este diagnostico nao corrige essa divergencia para evitar sobrescrever trabalho local ou branch ainda nao sincronizada.

## 5. Entidades existentes relacionadas

### CanalNotificacao

Existe o enum `CanalNotificacao` no dominio com os valores:

- `Sistema = 1`;
- `Email = 2`.

Isso indica previsao inicial de canal interno e e-mail. Nao foram identificados valores funcionais para WhatsApp, webhook, SMS ou push neste enum.

### Historico de chamado

O dominio ja usa historico operacional do chamado para eventos como integracao de e-mail, abertura, comentarios, atendimento e demais alteracoes do fluxo. Esse historico deve ser tratado como trilha operacional do chamado, nao como notificacao enviada.

Para Sprint 6, o historico pode ser fonte de eventos ou evidencia operacional, mas nao deve ser usado como substituto de notificacao persistente, auditoria de envio ou fila.

### EventoAuditoria

Existe entidade `EventoAuditoria` e servico de auditoria. Esse modulo cobre rastreabilidade de acoes relevantes e deve ser reaproveitado para registrar governanca administrativa das configuracoes de notificacao e, futuramente, eventos relevantes de envio/reprocessamento.

A auditoria nao substitui uma tabela de notificacoes ou uma fila de envio. Ela registra rastreabilidade; a notificacao representa comunicacao ao usuario/destinatario.

### EventoSla

O sistema possui estrutura relacionada a eventos de SLA e alertas em migrations/modelo existentes. Para Sprint 6, eventos como SLA proximo do vencimento e SLA vencido sao candidatos naturais a notificacao, mas este diagnostico nao altera calculo, politica, pausa, vencimento ou escalonamento de SLA.

### Entidades com nomes relacionados

Foram buscados nomes relacionados a `Notificacao`, `Template`, `Canal`, `Mensagem`, `Preferencia`, `Destinatario` e `Fila`. O estado encontrado indica:

- existe enum `CanalNotificacao`;
- existem artefatos frontend/local de notificacoes;
- existem permissoes de notificacao;
- nao foi identificado modulo backend persistente completo para notificacao ITSM por evento;
- nao foi identificado modulo funcional de templates de notificacao;
- nao foi identificada preferencia persistente de usuario por canal/evento;
- nao foi identificada fila dedicada de envio de notificacoes ITSM.

## 6. Servicos existentes relacionados

### EmailParaChamadoService

`EmailParaChamadoService` atua como adaptador de entrada: recebe `EmailMessageDto`, converte para `EmailMessageData`, delega processamento ao `IEmailMessageProcessor` e devolve resultado de processamento. Ele pertence ao fluxo de ingestao de e-mails para chamados.

Nao deve ser usado como servico de envio de notificacoes.

### EmailMessageProcessor

`EmailMessageProcessor` processa mensagens recebidas, calcula fingerprint, verifica duplicidade, registra `LogIntegracaoEmail`, correlaciona mensagem com chamado existente, cria novo chamado por e-mail quando necessario, adiciona comentario quando houver correlacao, processa anexos e inicializa SLA na abertura por e-mail.

Ele possui logica forte de inbound mail: deduplicacao por message id/fingerprint, permissao de remetente, correlacao, criacao de usuario solicitante, criacao de chamado, comentario, historico, anexos e SLA. Essa responsabilidade nao deve ser misturada com notificacoes outbound.

### EmailCorrelationService

O servico de correlacao de e-mail deve permanecer restrito a encontrar chamado relacionado a mensagens recebidas, usando cabecalhos, referencias e padroes de assunto/codigo. Ele e candidato a inspirar regras de idempotencia, mas nao deve ser reaproveitado diretamente como correlacionador de notificacoes.

### Servicos de auditoria

A Sprint 6 deve reaproveitar o padrao existente de auditoria para registrar administracao de configuracoes, templates e reprocessamentos futuros. Auditoria de envio deve ser separada de historico operacional do chamado e de log de integracao de e-mail inbound.

### Servicos de usuario/contexto

O sistema ja possui servicos de usuario/contexto usados por use cases e controllers. A Sprint 6 deve usa-los para identificar usuario autenticado, perfil, permissoes efetivas e autoria de acoes administrativas. Destinatarios de notificacao devem ser resolvidos por contratos/use cases, nao por acesso direto do frontend ao dominio.

### Servicos de permissoes

Existem permissoes granulares para notificacoes: `Notificacoes.Visualizar` e `Notificacoes.Gerenciar`. Elas devem ser preservadas e possivelmente refinadas em itens futuros, sem duplicar permissoes ja existentes.

### Servicos de SLA

O SLA ja e inicializado na abertura por e-mail e possui estrutura propria. Eventos de SLA podem se tornar gatilhos de notificacao, mas este item nao altera SLA nem cria escalonamento automatico.

### Servicos de historico de chamado

O historico deve continuar registrando fatos operacionais do chamado. A Sprint 6 pode consumir fatos/eventos do chamado para gerar notificacoes, mas nao deve transformar historico em fila de notificacao.

## 7. Worker.Email e responsabilidades atuais

O projeto possui `src/SGX.SistemaChamado.Worker.Email` com `Worker`, `EmailIngestionService`, cliente IMAP MailKit e providers de contexto para worker.

A responsabilidade atual e ingestao de e-mails via IMAP:

1. O `Worker` executa ciclos periodicos.
2. Em cada ciclo, resolve `EmailIngestionService` por escopo.
3. `EmailIngestionService` verifica configuracao IMAP.
4. Le mensagens do IMAP respeitando limite por ciclo.
5. Encaminha cada mensagem para `IProcessarEmailRecebidoUseCase`.
6. Aplica pos-processamento IMAP: marcar como lida, mover para pasta de processadas ou erro.

Nao foi identificado nesse pipeline papel de envio outbound. Portanto, o Worker.Email nao deve ser tratado como worker de notificacoes. Reaproveita-lo para envio pode misturar inbound e outbound, dificultar idempotencia, observabilidade, retry e isolamento operacional.

## 8. Eventos ITSM candidatos a notificacao

Eventos candidatos, ainda sem implementacao neste item:

- abertura de chamado pelo portal;
- abertura de chamado por e-mail;
- triagem inicial;
- atribuicao de responsavel;
- transferencia de responsavel/grupo/fila;
- comentario publico;
- comentario interno, quando aplicavel apenas a atendentes;
- anexo ou evidencia adicionada;
- mudanca de status;
- aprovacao pendente;
- aprovacao aprovada;
- aprovacao reprovada;
- aprovacao cancelada/expirada, se aplicavel em item futuro;
- SLA proximo do vencimento;
- SLA vencido;
- resolucao do chamado;
- aceite pelo solicitante;
- rejeicao da solucao;
- fechamento automatico;
- reabertura;
- cancelamento.

Cada evento precisara, em item futuro, definir: chave de idempotencia, origem, destinatarios, canais elegiveis, template, politica de retry, auditoria e criterio para evitar duplicidade.

## 9. Canais de notificacao existentes e futuros

### Existentes

- Sistema/in-app: previsto pelo enum `CanalNotificacao.Sistema` e por artefatos frontend locais.
- E-mail: previsto pelo enum `CanalNotificacao.Email` e pelo ecossistema de integracao de e-mail inbound.

### Futuros

- WhatsApp: deve ser tratado como canal futuro, dependente de provedor, consentimento, template aprovado e governanca de envio.
- Webhook: canal futuro para integracao com sistemas externos, exigindo autenticacao, assinatura, retry e idempotencia.
- Outros canais: SMS/push podem ser considerados no futuro, mas nao devem ser adicionados sem necessidade real e contrato claro.

## 10. Templates existentes ou lacunas

Nao foi identificado modulo backend persistente de templates de notificacao ITSM.

Ha textos hardcoded em fluxos existentes, especialmente historico de chamado, integracao de e-mail, logs e mensagens operacionais. Esses textos nao devem ser reaproveitados como template de notificacao sem contrato. Uma futura modelagem deve separar:

- assunto/titulo;
- corpo;
- canal;
- evento;
- variaveis permitidas;
- fallback;
- versao do template;
- ativo/inativo;
- auditoria de alteracao.

## 11. Permissoes existentes

Foram identificadas permissoes relacionadas a notificacoes:

- `Notificacoes.Visualizar`;
- `Notificacoes.Gerenciar`.

Essas permissoes indicam que o modulo ja foi previsto no catalogo de permissoes. A Sprint 6 deve reaproveita-las antes de criar novas permissoes. Permissoes futuras so devem ser criadas se houver necessidade especifica, como reprocessar fila, gerenciar templates ou visualizar auditoria de envio.

## 12. Auditoria e rastreabilidade

A Sprint 6 deve prever rastreabilidade em camadas separadas:

- historico do chamado: registra fato operacional no chamado;
- auditoria: registra acao administrativa ou critica;
- log de envio: registra tentativa de envio, canal, destinatario, status, erro, chave de idempotencia e reprocessamento;
- fila: quando existir, registra pendencia de entrega e controle de retry.

Nao se deve gravar apenas historico e considerar notificacao enviada. Tambem nao se deve usar `LogIntegracaoEmail` como log de envio, pois ele representa integracao inbound de e-mail.

## 13. Integracao com abertura de chamado

Abertura de chamado e candidato natural a notificacao para:

- solicitante;
- atendente/grupo/fila responsavel;
- observadores futuros;
- administradores em casos criticos.

Neste item nao ha alteracao em abertura. A integracao futura deve ocorrer por use case/evento de aplicacao apos persistencia segura do chamado, com idempotencia por `ChamadoId + Evento + Canal + Destinatario`.

## 14. Integracao com atendimento

Eventos de atendimento candidatos:

- triagem;
- assumir chamado;
- atribuir responsavel;
- transferir grupo/fila;
- comentario publico;
- anexo/evidencia;
- mudanca de status.

A integracao futura deve preservar comentarios, anexos, evidencias e acompanhamento normal, sem criar bloqueios adicionais. Notificacao nao deve alterar status operacional.

## 15. Integracao com aprovacao ITSM

Eventos candidatos:

- instancia de aprovacao criada/pendente;
- etapa pendente, se aplicavel;
- aprovacao aprovada;
- aprovacao reprovada;
- aprovacao expirada/cancelada, se a funcionalidade existir;
- reavaliacao.

Este diagnostico nao altera Motor de Aprovacoes ITSM. A futura integracao deve consumir contratos/eventos do motor, evitando acoplamento direto a detalhes internos de dominio no frontend.

## 16. Integracao com SLA

Eventos candidatos:

- SLA proximo do vencimento;
- SLA vencido;
- primeira resposta pendente;
- resolucao pendente;
- escalonamento futuro.

Este item nao altera calculo de SLA, metas, pausa, vencimento, calendario ou OLA. A integracao futura precisa definir janela de disparo, anti-duplicidade, periodicidade e destinatarios por papel/grupo.

## 17. Integracao com fechamento, aceite, rejeicao e reabertura

Eventos candidatos:

- chamado resolvido aguardando aceite;
- solicitante aceitou a solucao;
- solicitante rejeitou a solucao;
- fechamento automatico executado;
- chamado reaberto;
- chamado cancelado.

Este diagnostico nao altera regras da Sprint 5. A notificacao futura deve apenas comunicar eventos ja decididos pelo fluxo de fechamento, aceite, rejeicao e reabertura.

## 18. Riscos tecnicos

- Confundir notificacao frontend/local com modulo persistente de notificacoes ITSM.
- Reaproveitar Worker.Email inbound como emissor outbound e misturar responsabilidades.
- Usar `LogIntegracaoEmail` como auditoria de envio outbound.
- Disparar e-mail antes de definir eventos, idempotencia e destinatarios.
- Criar templates sem governanca de variaveis e versao.
- Notificar comentario interno para solicitante por erro de escopo.
- Notificar evento sensivel sem verificar permissoes/visibilidade.
- Alterar SLA indiretamente ao implementar alertas.
- Acoplar frontend diretamente a entidades de dominio.
- Marcar canais futuros como funcionais apenas por estarem documentados.

## 19. Decisoes arquiteturais adiadas

Ficam adiadas para proximos itens:

- modelo persistente de notificacao;
- modelo de template;
- modelo de preferencia/consentimento;
- fila de envio;
- worker de envio;
- estrategia de retry/backoff/dead-letter;
- politica de idempotencia oficial;
- endpoints administrativos;
- endpoints de leitura do usuario;
- integracao real com e-mail outbound;
- integracao futura com WhatsApp;
- integracao futura com webhook;
- matriz de destinatarios por evento ITSM;
- governanca de variaveis de template.

## 20. Proposta incremental para os proximos itens da Sprint 6

Sequencia recomendada:

1. Consolidar checklist real da Sprint 6 no roadmap, alinhando a divergencia da Sprint 5 antes de atualizar percentuais.
2. Modelar eventos de notificacao ITSM como contrato interno, sem disparo automatico.
3. Modelar entidade persistente de notificacao in-app, se confirmada a necessidade.
4. Criar contratos/DTOs de leitura e administracao.
5. Criar regra de destinatarios por evento.
6. Criar templates administrativos apenas depois de estabilizar eventos e variaveis.
7. Criar fila de envio e log de tentativas, se e-mail outbound for priorizado.
8. Criar worker separado de envio, sem reutilizar Worker.Email inbound.
9. Integrar abertura/atendimento de forma incremental.
10. Integrar aprovacao, SLA e fechamento apenas com testes de regressao especificos.

## 21. Criterios de aceite do diagnostico

Este item sera considerado aceito quando:

- o documento tecnico de diagnostico existir em `docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md`;
- o documento deixar explicito o que existe e o que nao existe;
- Worker.Email estiver classificado como inbound, nao outbound;
- eventos ITSM candidatos estiverem mapeados;
- canais existentes e futuros estiverem separados;
- templates e lacunas estiverem documentados;
- riscos e decisoes adiadas estiverem registrados;
- nenhuma funcionalidade automatica de notificacao tiver sido implementada;
- nenhuma tabela estrutural tiver sido criada;
- SLA, aprovacao ITSM e regras de fechamento tiverem permanecido sem alteracao funcional;
- a divergencia entre contexto operacional e `main` disponivel estiver registrada para saneamento antes de marcar percentuais definitivos.
