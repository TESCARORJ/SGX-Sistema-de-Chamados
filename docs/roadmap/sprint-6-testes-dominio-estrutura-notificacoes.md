# Sprint 6 - Testes de Dominio e Estrutura Persistente de Notificacoes

## 1. Objetivo

Validar tecnicamente o dominio e a estrutura persistente de `Notificacao` ja implementados, confirmando regras de criacao, transicoes, persistencia, constraints, FKs, idempotencia e compatibilidade com PostgreSQL, sem antecipar comportamento funcional.

## 2. Estado anterior

- `Notificacao` ja existia no dominio.
- `EventoCandidatoNotificacao` ja existia na Application.
- `NotificacaoConfiguration`, `DbSet<Notificacao>` e a tabela `notificacoes` ja estavam criados.
- A migration estrutural `CriarEstruturaNotificacaoSprint6` ja havia sido aplicada.
- Sprint 6 estava em `4/16 - 25%`, com a proxima acao voltada aos testes tecnicos da notificacao.

## 3. Escopo dos testes

- testes de dominio da entidade `Notificacao`;
- testes do contrato `EventoCandidatoNotificacao`;
- testes de configuracao EF;
- testes relacionais reais com PostgreSQL;
- validacao da migration estrutural;
- validacao de constraints, FKs, delete behavior e idempotencia;
- regressao do roadmap da Sprint 6 e preservacao da Sprint 5.

## 4. Testes de dominio

Os testes de dominio foram ampliados para cobrir:

- criacao valida com usuario, endereco, ambos, com e sem chamado;
- estado inicial `Pendente`;
- tentativas iniciando em zero;
- rejeicao de destinatario ausente;
- rejeicao de conteudo, tipo de evento, canal e chave de idempotencia invalidos;
- normalizacao com `Trim`;
- assunto e correlacao opcionais;
- agendamento;
- processamento a partir de `Pendente`, `Agendada` e reprocessamento de `Falhou`;
- envio somente a partir de `EmProcessamento`;
- falha somente a partir de `EmProcessamento`;
- cancelamento em estados permitidos e bloqueio em estados invalidos;
- limites de endereco, assunto, conteudo, correlacao, idempotencia, erro e motivo de cancelamento.

## 5. Testes do contrato

`EventoCandidatoNotificacaoTests` passou a validar:

- criacao valida;
- chamado opcional;
- usuario originador opcional;
- preservacao de tipo, data, correlacao e idempotencia;
- metadados somente leitura;
- copia defensiva de metadados;
- normalizacao de metadados;
- rejeicao de chave vazia;
- ausencia de dependencias de EF ou entidades de dominio no payload.

## 6. Testes de configuracao EF

`NotificacaoConfigurationTests` valida:

- entidade registrada;
- tabela `notificacoes`;
- PK em `id`;
- nomes corretos de colunas;
- nulabilidade;
- enums persistidos como `int`;
- limites de campos;
- FKs opcionais de chamado, destinatario e auditoria;
- `DeleteBehavior.Restrict`;
- indice unico de idempotencia;
- indices operacionais;
- constraints `ck_notificacoes_destinatario` e `ck_notificacoes_quantidade_tentativas_nao_negativa`;
- ausencia de estrutura de template ou preferencia.

## 7. Testes de persistencia

Foi criada a suite `NotificacaoPersistenceTests` com banco PostgreSQL temporario por fixture, aplicando migrations reais e removendo o banco ao final.

Ela cobre:

- persistencia com somente usuario;
- persistencia com somente endereco;
- persistencia com usuario, endereco e chamado;
- recuperacao preservando enums, datas, conteudo, idempotencia e tentativas;
- ausencia de seed indevido de notificacao.

## 8. Testes de constraints

As constraints foram validadas no banco real:

- destinatario obrigatorio por `ck_notificacoes_destinatario`;
- tentativas nao negativas por `ck_notificacoes_quantidade_tentativas_nao_negativa`;
- tamanho maximo de campo pelo provider PostgreSQL.

## 9. Testes de idempotencia

A unicidade da `chave_idempotencia` foi validada por tentativa de persistir duplicidade e observacao de erro relacional no indice unico `ux_notificacoes_chave_idempotencia`.

## 10. Testes de destinatario

Foi validado que:

- notificacao com usuario somente persiste;
- notificacao com endereco somente persiste;
- notificacao com ambos persiste;
- notificacao sem ambos e rejeitada pela constraint do banco.

## 11. Testes de FKs

Foram validadas:

- FK opcional de chamado;
- FK opcional de usuario destinatario;
- FKs de auditoria;
- rejeicao de chamado inexistente;
- rejeicao de usuario inexistente.

## 12. Testes de delete behavior

Foi validado que a exclusao de chamado ou usuario referenciado nao remove a notificacao em cascata e e bloqueada pelo banco, com resposta de `ForeignKeyViolation` ou `RestrictViolation` conforme o caminho executado pelo PostgreSQL.

## 13. Teste da migration

A migration `CriarEstruturaNotificacaoSprint6` foi revisada por teste de arquivo e confirmada com:

- `CreateTable("notificacoes")`;
- PK;
- FKs;
- indices;
- indice unico;
- constraints;
- `Down` removendo somente `notificacoes`.

Tambem foi confirmado que ela nao cria templates, preferencias, fila, tabela de tentativa ou seed de notificacoes.

## 14. Provider utilizado

- `Npgsql.EntityFrameworkCore.PostgreSQL`
- Banco PostgreSQL temporario criado exclusivamente para a suite relacional.

## 15. Limitacoes do ambiente

- a suite relacional depende de acesso ao PostgreSQL local configurado no ambiente de desenvolvimento;
- o teste de delete behavior observou `RestrictViolation` em algumas exclusoes, o que continua valido como protecao relacional;
- os testes nao exercem servicos funcionais de geracao, processamento ou envio.

## 16. Correcoes realizadas

Nao foi necessario corrigir o dominio nem a configuracao EF da `Notificacao`.

Apenas os testes foram ajustados para:

- criar massa minima propria no banco temporario;
- validar corretamente `RestrictViolation` quando o PostgreSQL retorna esse codigo;
- revisar a migration estrutural sem falsos positivos sobre o termo `tentativas`.

## 17. Compatibilidade com PostgreSQL

As migrations aplicaram corretamente, a tabela `notificacoes` foi criada, os checks ficaram ativos, a unicidade de idempotencia foi respeitada e os erros relacionais esperados foram observados no banco.

## 18. Compatibilidade com Worker.Email

Nenhuma alteracao foi feita no `Worker.Email`. O worker continua inbound IMAP e permanece desacoplado do envio outbound futuro.

## 19. Impacto em abertura

Nenhuma integracao funcional com abertura por portal ou e-mail foi implementada.

## 20. Impacto em atendimento

Nenhuma integracao funcional com atribuicao, comentario, transferencia ou mudanca de status foi implementada.

## 21. Impacto em aprovacao

Nenhuma integracao funcional com aprovacoes foi implementada.

## 22. Impacto em SLA

Nenhuma integracao funcional com eventos de SLA foi implementada.

## 23. Impacto em fechamento e reabertura

Nenhuma integracao funcional com resolucao, aceite, fechamento, cancelamento ou reabertura foi implementada alem da modelagem e persistencia ja existentes.

## 24. Riscos

- a geracao futura da chave de idempotencia continua sendo responsabilidade critica do servico da etapa seguinte;
- a separacao entre notificacao persistente, processamento e entrega ainda depende das proximas etapas para evitar acoplamento indevido;
- a estrategia de destinatarios ainda e unitária por notificacao e sera refinada depois.

## 25. Decisoes adiadas

- servico de geracao idempotente;
- resolucao de destinatarios por perfil e participacao;
- templates;
- preferencias;
- processamento de entrega;
- canais funcionais;
- API de consulta e leitura;
- frontend.

## 26. Evidencias

- testes automatizados de dominio, contrato, configuracao e persistencia;
- validacao real de constraints, FKs e idempotencia em PostgreSQL;
- revisao da migration estrutural;
- ausencia de notificacoes seedadas ou estruturas futuras indevidas.

## 27. Criterios de aceite

- testes de dominio completos;
- testes do contrato completos;
- testes de configuracao EF completos;
- testes relacionais executados;
- constraints e FKs validadas;
- idempotencia validada;
- migration estrutural revisada;
- nenhuma estrutura futura criada;
- nenhum comportamento funcional implementado.

## 28. Proxima etapa

Criar servico de geracao idempotente de notificacoes.
