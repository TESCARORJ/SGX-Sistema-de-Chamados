# Sprint 6 - Preferencias de notificacao por usuario e evento

## Objetivo

Implementar a modelagem persistente e a regra de aplicacao responsavel por decidir se um usuario pode ou deseja receber determinada notificacao em um canal especifico, antes da geracao concreta da `Notificacao`.

## Estado anterior

Antes deste item, a Sprint 6 ja possuia:

- `Notificacao` persistida;
- geracao idempotente;
- resolucao de destinatarios;
- templates persistentes;
- materializacao de conteudo;
- ausencia de preferencia persistente por usuario, evento e canal.

## Diagnostico das configuracoes existentes

- nao existia entidade equivalente a preferencia de notificacao;
- nao havia tabela generica de configuracao por usuario reutilizavel para este caso;
- nao havia conceito funcional implementado de evento obrigatorio com granularidade suficiente;
- a resolucao de destinatarios e a geracao de notificacoes precisavam continuar separadas da politica de preferencia.

## Entidade

Foi criada a entidade `PreferenciaNotificacaoUsuario`.

Ela representa uma configuracao explicita de habilitacao ou desabilitacao para uma combinacao unica de:

- usuario;
- tipo de evento;
- canal.

## Campos

Campos adotados:

- `UsuarioId`
- `TipoEvento`
- `Canal`
- `Habilitada`
- `CriadoPorUsuarioId`
- `AtualizadoPorUsuarioId`
- auditoria herdada

## Granularidade

A granularidade adotada foi:

`UsuarioId + TipoEvento + Canal`

Nao foram criadas preferencias:

- globais por usuario;
- apenas por canal;
- apenas por evento;
- por participacao;
- por chamado;
- por template;
- por perfil.

## Usuario

A preferencia e sempre vinculada a um `Usuario` existente.

A chave logica nao pode ser alterada apos a criacao da entidade.

## Evento

O evento reutiliza `TipoEventoNotificacao`.

## Canal

O canal reutiliza `CanalNotificacao`.

## Habilitada

`Habilitada = true` permite o recebimento daquele evento naquele canal.

`Habilitada = false` bloqueia o recebimento daquele evento naquele canal.

## Ausencia de preferencia

A ausencia de registro nao bloqueia automaticamente.

## Fallback

Regra adotada:

- preferencia explicita habilitada: permitir;
- preferencia explicita desabilitada: bloquear;
- ausencia de preferencia: permitir por fallback;
- usuario inexistente: bloquear;
- usuario inativo: bloquear;
- usuario bloqueado: bloquear;
- canal `Email` sem endereco valido: bloquear.

## Eventos obrigatorios

Eventos obrigatorios foram adiados.

Justificativa:

- os tipos atuais de evento ainda sao amplos;
- ainda nao existe classificacao suficientemente especifica para afirmar obrigatoriedade sem risco semantico.

## Elegibilidade

Foi aplicada a elegibilidade funcional minima:

- usuario existente;
- `Ativo = true`;
- `Situacao = Ativo`;
- `BloqueadoAte` ausente ou expirado.

## Canal Sistema

O canal `Sistema` nao exige e-mail.

Ele pode ser bloqueado por preferencia explicita, mas nao foi declarado obrigatorio nesta etapa.

## Canal Email

O canal `Email` exige endereco eletronico valido no usuario.

Sem e-mail, a avaliacao retorna bloqueio com motivo explicito.

## E-mail ausente

O item nao altera cadastro de usuario.

Ele apenas retorna decisao bloqueada para `Email` quando o endereco nao estiver disponivel.

## Precedencia

Ordem aplicada:

1. validar request;
2. localizar usuario;
3. avaliar elegibilidade;
4. avaliar compatibilidade do canal;
5. buscar preferencia explicita;
6. aplicar preferencia;
7. aplicar fallback;
8. retornar motivo deterministico.

## Contrato de definicao

Foi criado request/response para definicao interna da preferencia.

O fluxo atualiza registro existente ou cria novo, sem duplicidade.

## Contrato de avaliacao

Foi criado request/response para avaliacao.

O retorno diferencia:

- permitido por preferencia explicita;
- bloqueado por preferencia explicita;
- permitido por fallback;
- bloqueado por usuario inexistente;
- bloqueado por usuario inativo;
- bloqueado por usuario bloqueado;
- bloqueado por canal sem endereco.

## Fluxo de definicao

1. validar request;
2. garantir existencia do usuario;
3. localizar preferencia pela chave composta;
4. criar quando inexistente;
5. atualizar quando existente;
6. registrar auditoria;
7. salvar;
8. retornar o estado atual.

## Fluxo de avaliacao

1. validar request;
2. carregar usuario;
3. aplicar elegibilidade;
4. validar canal;
5. buscar preferencia explicita;
6. aplicar fallback quando nao houver registro;
7. retornar motivo deterministico;
8. nao persistir;
9. nao gerar notificacao;
10. nao materializar template;
11. nao enviar.

## Auditoria

A definicao de preferencia registra usuario criador e usuario atualizador.

## Persistencia

Foi criada persistencia explicita com:

- entidade `PreferenciaNotificacaoUsuario`;
- `DbSet`;
- `PreferenciaNotificacaoUsuarioConfiguration`;
- migration estrutural separada.

## Indices

Foram criados:

- indice unico `UsuarioId + TipoEvento + Canal`;
- indice por `UsuarioId`;
- indice por `TipoEvento + Canal`;
- indices das FKs de auditoria.

## Constraints

Nao foram adicionadas constraints artificiais extras.

A integridade principal e garantida por:

- FKs;
- enums mapeados;
- unicidade da chave composta;
- invariantes da entidade.

## Migration estrutural

Foi criada a migration `CriarEstruturaPreferenciaNotificacaoUsuarioSprint6`.

Ela contem apenas a tabela de preferencia, PK, FKs e indices.

## Migration de checklist

Esta etapa tambem exige migration separada de checklist para concluir o item 9 da Sprint 6.

## Relacao com destinatarios

Preferencia nao substitui resolucao de destinatarios.

Ela e avaliada somente depois que os usuarios elegiveis sao resolvidos.

## Relacao com templates

Preferencia nao escolhe template nem materializa conteudo.

## Relacao com geracao

Preferencia nao foi embutida no `GerarNotificacaoUseCase`.

A etapa de geracao continua recebendo uma notificacao ja autorizada por politica de publico.

## Compatibilidade com Worker.Email

Nenhuma alteracao foi feita no `Worker.Email`.

## Impacto em abertura

Prepara o filtro de recebimento por usuario antes da futura integracao automatica com eventos de abertura.

## Impacto em atendimento

Prepara o filtro de recebimento para atribuicao, comentarios e mudancas de status futuras, sem entrega funcional nesta etapa.

## Impacto em aprovacao

Prepara o filtro para notificacoes futuras de aprovacao, sem disparo automatico.

## Impacto em SLA

Prepara o filtro para futuros alertas de SLA.

## Impacto em fechamento e reabertura

Prepara o filtro para futuros eventos de resolucao, fechamento e reabertura.

## Testes

Foram criados testes de:

- dominio;
- configuracao EF;
- validators;
- use case de definicao;
- use case de avaliacao;
- persistencia relacional;
- fluxo integrado de definicao + avaliacao;
- revisao da migration estrutural.

## O que nao foi implementado

- envio por e-mail;
- entrega pelo canal Sistema;
- worker outbound;
- fila;
- outbox;
- retry;
- API publica;
- frontend;
- pagina de preferencias;
- eventos obrigatorios especificos;
- preferencias por perfil, grupo, participacao, chamado ou template.

## Riscos

- a granularidade atual de evento ainda e ampla para politicas mais criticas;
- proximas etapas precisam evitar acoplar preferencia a geracao concreta ou ao envio;
- evolucoes futuras por perfil ou canal global exigirao regra de precedencia mais rica.

## Decisoes adiadas

- eventos obrigatorios com granularidade fina;
- preferencias por perfil;
- preferencias globais por usuario;
- horarios silenciosos;
- idioma;
- resumo diario;
- endereco alternativo;
- interface administrativa/publica de manutencao.

## Criterios de aceite

- entidade persistente criada;
- configuracao EF explicita;
- `DbSet` criado;
- migration estrutural separada;
- definicao e avaliacao separadas;
- fallback permissivo documentado;
- usuario inelegivel bloqueado;
- canal `Sistema` sem exigencia de e-mail;
- canal `Email` exigindo endereco;
- sem duplicidade na chave composta;
- sem criacao de notificacao;
- sem envio.

## Proxima etapa

Implementar processamento e controle de tentativas de entrega
