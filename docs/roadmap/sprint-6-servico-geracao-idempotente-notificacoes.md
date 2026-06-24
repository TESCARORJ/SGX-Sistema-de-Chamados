# Sprint 6 - Servico de geracao idempotente de notificacoes

## 1. Objetivo

Criar o servico de aplicacao responsavel por materializar e persistir uma notificacao concreta a partir de um evento candidato, sem executar envio por canal.

## 2. Estado anterior

A Sprint 6 ja possuia:
- entidade `Notificacao` modelada no dominio;
- contrato `EventoCandidatoNotificacao`;
- persistencia EF/Core e tabela `notificacoes`;
- testes de dominio, contrato, configuracao e persistencia aprovados.

## 3. Responsabilidade do servico

O servico gera uma notificacao persistente, valida a entrada, protege a idempotencia e retorna um resultado explicito de criacao ou reaproveitamento.

## 4. Contrato de entrada

Foi criado `GerarNotificacaoRequest` com:
- `Evento`;
- `Canal`;
- `DestinatarioUsuarioId`;
- `DestinatarioEndereco`;
- `Assunto`;
- `Conteudo`;
- `AgendadaEm`.

O request representa uma unica notificacao concreta para um destinatario logico ja resolvido.

## 5. Contrato de saida

Foi criado `GerarNotificacaoResponse` com `NotificacaoId`, `Criada`, `JaExistia`, `Status` e `ChaveIdempotencia`.

## 6. Fluxo de geracao

Fluxo implementado:
- validar request;
- normalizar e consultar a chave de idempotencia do evento;
- retornar a notificacao existente quando a chave ja estiver persistida;
- criar `Notificacao` quando a chave ainda nao existir;
- aplicar agendamento quando informado;
- persistir via repositorio + `IUnitOfWork`;
- tratar corrida concorrente por violacao do indice unico.

## 7. Idempotencia

A chave usada e exatamente `Evento.ChaveIdempotencia`.

## 8. Concorrencia

A consulta previa nao e a protecao final. O servico usa o indice unico `ux_notificacoes_chave_idempotencia` como barreira definitiva e, em caso de corrida, recarrega a notificacao existente.

## 9. Chave global

A chave unica global foi mantida sem concatenacao silenciosa de canal ou destinatario.

## 10. Comportamento quando ja existe

Quando a notificacao ja existe:
- `Criada = false`;
- `JaExistia = true`;
- assunto, conteudo, status, tentativas e agendamento permanecem inalterados.

## 11. Destinatario ja resolvido

O servico nao resolve perfil, grupo, aprovador, observador ou fila. O request recebe o destinatario ja materializado por usuario, endereco ou ambos.

## 12. Conteudo materializado

O servico recebe assunto e conteudo prontos. Nao cria template, nao interpolar metadados e nao consulta historico ou auditoria para montar mensagem.

## 13. Agendamento

Quando `AgendadaEm` e informado, a notificacao e persistida como `Agendada`. Sem agendamento, o status inicial fica `Pendente`.

## 14. Persistencia

Foi preservado o padrao do projeto com `IRepository<Notificacao>` e `IUnitOfWork`.

## 15. Transacao

O item utiliza a transacao implicita do `SaveChangesAsync`. Nao houve necessidade de transacao manual adicional.

## 16. Auditoria

O autor material da notificacao usa `Evento.UsuarioOriginadorId` quando informado, com fallback para o usuario atual da aplicacao. O `CriadoPor` textual usa o login do usuario atual.

## 17. Tratamento de unique violation

O servico captura `DbUpdateException`, identifica o nome do indice unico de idempotencia e reconsulta o registro existente sem transformar idempotencia em atualizacao.

## 18. Testes unitarios

Foram adicionados testes cobrindo criacao pendente, criacao agendada, preservacao de dados, retorno idempotente, ausencia de alteracao do registro existente e respeito ao `CancellationToken`.

## 19. Testes relacionais

Foram adicionados testes PostgreSQL reais cobrindo persistencia via servico, reaproveitamento sequencial, concorrencia com a mesma chave, ausencia de duplicidade e propagacao de erro nao relacionado a idempotencia.

## 20. Compatibilidade com PostgreSQL

Os testes relacionais confirmaram a estrategia sobre PostgreSQL usando o indice unico ja criado na tabela `notificacoes`.

## 21. Compatibilidade com Worker.Email

Nenhuma integracao foi criada com `Worker.Email`. O worker inbound permanece isolado do modulo de geracao persistente.

## 22. Impacto em abertura

Nenhum evento de abertura passou a gerar notificacao automaticamente neste item.

## 23. Impacto em atendimento

Nenhum fluxo de atendimento foi integrado automaticamente ao servico.

## 24. Impacto em aprovacao

Nenhuma aprovacao passou a disparar notificacao real nesta etapa.

## 25. Impacto em SLA

Nenhum evento de SLA foi conectado automaticamente ao servico.

## 26. Impacto em fechamento e reabertura

Nenhum evento de fechamento, aceite ou reabertura foi integrado neste item.

## 27. O que nao foi implementado

Permaneceu fora do escopo:
- envio por canal;
- templates;
- preferencias;
- fila/outbox;
- retry;
- endpoint ou controller;
- leitura/nao lida;
- frontend;
- integracao automatica aos eventos ITSM.

## 28. Riscos

O produtor futuro precisara gerar corretamente a chave de idempotencia, e a proxima etapa deve manter a separacao entre geracao, resolucao de destinatarios e entrega.

## 29. Decisoes adiadas

Continuam adiadas a resolucao por participacao/perfil, templates persistentes, preferencias por evento/canal, processamento de entrega, retry, outbox, API e frontend.

## 30. Criterios de aceite

Atendidos neste item:
- interface do servico criada;
- request/response criados;
- validator criado;
- persistencia da `Notificacao` implementada;
- segunda execucao retorna o registro existente;
- concorrencia coberta por testes;
- nenhum envio executado.

## 31. Proxima etapa

Implementar resolucao de destinatarios por participacao e perfil.
