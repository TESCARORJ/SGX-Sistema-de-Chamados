# Sprint 6 - Entrega pelo canal Sistema

## Objetivo

Implementar a entrega interna de notificacoes pelo canal `Sistema`, reutilizando a propria entidade `Notificacao` como registro consultavel pelo destinatario.

## Estado anterior

- `Notificacao` ja existia persistida, com conteudo materializado, destinatario interno opcional e controle de processamento.
- O ciclo de tentativas, backoff e reagendamento ja estava implementado.
- Ainda nao havia transporte real por canal.

## Definicao de entrega pelo Sistema

Para o canal `Sistema`, entregar significa concluir uma `Notificacao` em `EmProcessamento` e torna-la disponivel ao usuario interno destinatario sem criar outra copia da mensagem.

## Decisao de reutilizar `Notificacao`

A modelagem atual ja suporta:

- `Canal = Sistema`;
- `DestinatarioUsuarioId`;
- `Assunto` e `Conteudo` materializados;
- `Status = Enviada`;
- `EnviadaEm`.

Por isso, nao foi criada tabela separada de caixa de entrada.

## Canal Sistema

- opera somente quando `Canal = Sistema`;
- nao exige e-mail;
- nao relê template;
- nao reavalia preferencia no momento da entrega.

## Destinatario interno

A entrega exige `DestinatarioUsuarioId` preenchido e usuario existente.

## Estado necessario

A notificacao deve estar em `EmProcessamento`. Estados `Pendente`, `Agendada`, `Cancelada` e `Falhou` sao rejeitados.

## Fluxo de processamento

1. iniciar processamento;
2. validar canal e destinatario;
3. concluir entrega interna de forma atomica;
4. marcar `Status = Enviada`;
5. preservar assunto, conteudo e destinatario;
6. disponibilizar a notificacao para consulta por usuario.

## Idempotencia

Se a notificacao ja estiver `Enviada`, o use case retorna sucesso idempotente:

- `Entregue = false`;
- `JaEstavaEntregue = true`;
- `EnviadaEm` preservado.

## Concorrencia

A conclusao usa update condicional atomico no repositorio de processamento. Assim, duas tentativas simultaneas nao duplicam a entrega nem alteram novamente `EnviadaEm`.

## Sucesso

Sucesso interno significa:

- `Status = Enviada`;
- `EnviadaEm` preenchido;
- `AgendadaEm`, `FalhouEm` e `UltimoErro` limpos pelo fluxo de sucesso ja existente.

## Falhas definitivas

Foram tratadas como invalidacao do fluxo:

- canal diferente de `Sistema`;
- destinatario interno ausente;
- usuario inexistente;
- usuario inelegivel;
- estado incompatível.

## Falhas transitorias

Nao foi criada classificacao nova neste item. Falhas transitorias de banco continuam sendo responsabilidade do ciclo de processamento do item 10.

## Data de entrega

`EnviadaEm` representa a disponibilizacao interna da notificacao ao destinatario.

## Semantica de `Enviada`

`Enviada` significa entregue/disponibilizada no sistema. Nao significa lida.

## Diferenca entre entregue e lida

Nao foram criados:

- `LidaEm`;
- `VisualizadaEm`;
- `StatusLeitura`.

Leitura fica adiada para item futuro.

## Conteudo materializado

Assunto e conteudo ja materializados sao preservados exatamente como foram gerados.

## Preferencias

Preferencias sao avaliadas antes da geracao da notificacao concreta. A entrega interna nao reexecuta essa politica.

## Templates

O template nao e reconsultado no momento da entrega. A notificacao ja contem o snapshot final do conteudo.

## Consulta interna

Foi adicionada consulta minima por usuario filtrando:

- `Canal = Sistema`;
- `Status = Enviada`;
- `Ativo = true`;
- `DestinatarioUsuarioId = usuarioId`.

## Seguranca por usuario

A consulta retorna apenas notificacoes do proprio destinatario informado.

## Persistencia

Nao houve nova tabela para o canal `Sistema`. A propria `notificacoes` permaneceu como fonte de verdade.

## Indices

Nao foi criada migration estrutural. O indice existente por `DestinatarioUsuarioId` foi mantido para a consulta minima deste item.

## Migration estrutural

Nao aplicavel neste item.

## Migration de checklist

Foi criada migration apenas de `UpdateData` para concluir o item 11 da Sprint 6.

## Historico de chamado

Nao foi criado `HistoricoChamado` para entrega interna.

## Auditoria

Nao foi criado `EventoAuditoria` artificial para simular entrega. O proprio estado da `Notificacao` registra a rastreabilidade minima.

## Compatibilidade com `Worker.Email`

Nenhuma alteracao foi realizada no `Worker.Email`.

## Impacto em abertura

Nenhuma integracao automatica foi adicionada.

## Impacto em atendimento

Nenhuma integracao automatica foi adicionada.

## Impacto em aprovacao

Nenhuma integracao automatica foi adicionada.

## Impacto em SLA

Nenhuma integracao automatica foi adicionada.

## Impacto em fechamento e reabertura

Nenhuma integracao automatica foi adicionada.

## Testes

- testes unitarios do use case de entrega;
- testes PostgreSQL de entrega, listagem e idempotencia;
- teste concorrente de entrega da mesma notificacao;
- regressao do ciclo de processamento e do roadmap.

## O que nao foi implementado

- entrega por e-mail;
- frontend completo;
- SignalR/WebSocket;
- lida/nao lida;
- outbox;
- fila externa;
- worker outbound.

## Riscos

- ainda nao existe API/UX de consulta do destinatario;
- `Enviada` significa disponibilizada, nao visualizada;
- futuras evolucoes de leitura/arquivamento exigirao novas decisoes de modelagem.

## Decisoes adiadas

- leitura/nao lida;
- arquivamento;
- exclusao pelo usuario;
- badge/sino/frontend;
- entrega por E-mail;
- integracoes automaticas com eventos ITSM.

## Criterios de aceite

- executor do canal `Sistema` implementado;
- `Notificacao` reutilizada;
- entrega idempotente;
- concorrencia controlada;
- sem duplicacao;
- sem e-mail;
- sem frontend completo.

## Proxima etapa

Implementar entrega pelo canal E-mail.
