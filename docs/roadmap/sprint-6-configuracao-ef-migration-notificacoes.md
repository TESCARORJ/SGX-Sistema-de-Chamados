# Sprint 6 - Configuracao EF e Migration Estrutural de Notificacoes

## 1. Objetivo

Persistir estruturalmente a entidade `Notificacao` no Entity Framework Core com configuracao explicita, tabela propria, relacionamentos opcionais, indices e constraints, sem implementar comportamento funcional de geracao, processamento ou envio.

## 2. Estado anterior

- `Notificacao` ja existia no dominio.
- `StatusNotificacao` e `TipoEventoNotificacao` ja estavam criados.
- `CanalNotificacao` ja era reutilizado.
- `EventoCandidatoNotificacao` ja existia na Application.
- Sprint 6 estava em `3/16 - 19%`, com proxima acao voltada a configuracao EF e migration estrutural.

## 3. Entidade persistida

- `Notificacao` foi registrada no `SGXSistemaChamadoDbContext`.
- Foi criada a configuracao `NotificacaoConfiguration`.

## 4. Nome da tabela

- Tabela criada: `notificacoes`.

## 5. Colunas

- `id`
- `chamado_id`
- `tipo_evento`
- `canal`
- `status`
- `destinatario_usuario_id`
- `destinatario_endereco`
- `assunto`
- `conteudo`
- `chave_correlacao`
- `chave_idempotencia`
- `agendada_em`
- `processada_em`
- `enviada_em`
- `falhou_em`
- `cancelada_em`
- `quantidade_tentativas`
- `ultimo_erro`
- `motivo_cancelamento`
- `criado_por_usuario_id`
- `atualizado_por_usuario_id`
- `criado_em`
- `criado_por`
- `atualizado_em`
- `atualizado_por`
- `ativo`

## 6. Campos obrigatorios

- `tipo_evento`
- `canal`
- `status`
- `conteudo`
- `chave_idempotencia`
- `quantidade_tentativas`
- `criado_em`
- `criado_por`
- `ativo`

## 7. Campos opcionais

- `chamado_id`
- `destinatario_usuario_id`
- `destinatario_endereco`
- `assunto`
- `chave_correlacao`
- `agendada_em`
- `processada_em`
- `enviada_em`
- `falhou_em`
- `cancelada_em`
- `ultimo_erro`
- `motivo_cancelamento`
- `criado_por_usuario_id`
- `atualizado_por_usuario_id`
- `atualizado_em`
- `atualizado_por`

## 8. Conversao de enums

Os enums foram mapeados explicitamente como `int`, seguindo o padrao predominante do projeto:

- `TipoEventoNotificacao`
- `CanalNotificacao`
- `StatusNotificacao`

## 9. Relacionamento com chamado

- `Notificacao.ChamadoId -> Chamado.Id`
- relacionamento opcional;
- sem cascata;
- `DeleteBehavior.Restrict`;
- nenhuma colecao reversa foi adicionada em `Chamado`.

## 10. Relacionamento com destinatario

- `Notificacao.DestinatarioUsuarioId -> Usuario.Id`
- relacionamento opcional;
- sem cascata;
- `DeleteBehavior.Restrict`;
- endereco externo permanece valido por `DestinatarioEndereco`.

## 11. Relacionamentos de auditoria

Tambem foram persistidos:

- `CriadoPorUsuarioId -> Usuario.Id`
- `AtualizadoPorUsuarioId -> Usuario.Id`

Ambos opcionais e com `DeleteBehavior.Restrict`.

## 12. Idempotencia

Foi adotado indice unico global em `ChaveIdempotencia`:

- `ux_notificacoes_chave_idempotencia`

Decisao: cada registro representa uma materializacao unica de notificacao. O servico futuro devera gerar a chave considerando, quando necessario, evento, chamado, canal e destinatario.

## 13. Indices

Indices criados:

- `ix_notificacoes_chamado_id`
- `ix_notificacoes_status`
- `ix_notificacoes_canal`
- `ix_notificacoes_agendada_em`
- `ix_notificacoes_destinatario_usuario_id`
- `ix_notificacoes_criado_em`
- `ix_notificacoes_status_agendada_em`
- `ux_notificacoes_chave_idempotencia`

## 14. Constraints

Constraints criadas:

- `ck_notificacoes_destinatario`
- `ck_notificacoes_quantidade_tentativas_nao_negativa`

## 15. Decisao sobre destinatario duplo

O banco nao proibe `DestinatarioUsuarioId` e `DestinatarioEndereco` preenchidos ao mesmo tempo. A unica exigencia estrutural e haver pelo menos uma forma valida de destinatario.

## 16. Decisao sobre chave global

A chave de idempotencia foi mantida globalmente unica para simplificar a protecao contra duplicidade nesta fase estrutural.

## 17. Migration estrutural

- `CriarEstruturaNotificacaoSprint6`

Ela contem somente a criacao da tabela `notificacoes`, suas colunas, FKs, indices e constraints.

## 18. Migration de checklist

- `ConcluirConfiguracaoEfMigrationEstruturalNotificacoesSprint6Roadmap`

Ela contem somente atualizacao de checklist e metadados do roadmap.

## 19. Ausencia de templates

Nao foi criada qualquer tabela ou relacao de templates de notificacao.

## 20. Ausencia de preferencias

Nao foi criada tabela de preferencia por usuario, evento ou canal.

## 21. Ausencia de fila/outbox

Nao foi criada tabela de fila, outbox ou processamento pendente.

## 22. Ausencia de retry

Nao foi criado retry automatico, tentativa individual, backoff ou reprocessamento funcional.

## 23. Compatibilidade com Worker.Email

O `Worker.Email` continua exclusivamente inbound IMAP. A persistencia estrutural de `Notificacao` nao alterou o fluxo do worker.

## 24. Impacto em abertura

Nenhuma integracao funcional com abertura por portal ou e-mail foi implementada.

## 25. Impacto em atendimento

Nenhuma integracao funcional com atribuicao, comentario, transferencia ou mudanca de status foi implementada.

## 26. Impacto em aprovacao

Nenhuma integracao funcional com aprovacoes foi implementada.

## 27. Impacto em SLA

Nenhuma integracao funcional com eventos de SLA foi implementada.

## 28. Impacto em fechamento e reabertura

Nenhuma integracao funcional com resolucao, aceite, fechamento ou reabertura foi implementada.

## 29. Testes

Validacoes cobertas:

- entidade registrada no modelo;
- nome da tabela;
- PK;
- nulabilidade;
- limites de campos;
- conversao de enums;
- FKs opcionais;
- ausencia de cascata perigosa;
- indice unico de idempotencia;
- indices essenciais;
- check constraints.

## 30. Riscos

- o tamanho de `conteudo` em `varchar(10000)` e coerente com o dominio atual, mas consultas futuras de alto volume podem exigir ajuste se o uso mudar;
- a unicidade global da idempotencia depende de chave bem gerada pelo servico futuro;
- a semantica de processamento futuro ainda nao distingue fila/outbox de consulta operacional.

## 31. Decisoes adiadas

- repositorio especifico;
- use case;
- geracao de notificacoes;
- resolucao de destinatarios;
- templates;
- preferencias;
- fila/outbox;
- retry;
- worker outbound;
- API e frontend;
- integracoes com eventos ITSM reais.

## 32. Criterios de aceite

- `NotificacaoConfiguration` criada;
- `DbSet<Notificacao>` adicionado;
- entidade registrada no modelo;
- migration estrutural separada criada;
- migration de checklist separada criada;
- tabela `notificacoes` criada com FKs, indices e constraints;
- nenhum comportamento funcional de notificacao implementado.

## 33. Proxima etapa

Testar dominio e estrutura persistente de notificacoes.
