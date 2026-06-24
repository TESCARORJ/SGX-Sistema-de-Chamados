# Sprint 6 - Templates e materializacao de conteudo

## Objetivo

Modelar a base persistente de templates de notificacao e a regra de aplicacao responsavel por materializar assunto e conteudo finais a partir de evento, canal, vigencia, versao e variaveis explicitamente permitidas.

## Estado anterior

Antes deste item, a Sprint 6 ja possuia:

- entidade `Notificacao` persistida;
- contrato `EventoCandidatoNotificacao`;
- geracao idempotente de notificacoes;
- resolucao de destinatarios por participacao e perfil;
- ausencia de templates persistentes e de materializacao reutilizavel.

## Diagnostico de templates existentes

- Nao existia entidade equivalente a `TemplateNotificacao`.
- Nao havia mecanismo de renderizacao seguro e restrito reutilizavel para notificacoes.
- Nao foi identificado fluxo funcional de template persistente por evento/canal.
- Nao havia biblioteca de templating complexa instalada nem necessidade de introduzi-la nesta etapa.

## Entidade criada

Foi criada a entidade persistente `TemplateNotificacao`.

Ela representa um template versionado e ativavel para um tipo de evento e um canal especifico, sem acoplar envio, fila, preferencia ou endpoint administrativo.

## Campos

Campos adotados:

- `Nome`
- `Descricao`
- `TipoEvento`
- `Canal`
- `Versao`
- `AssuntoTemplate`
- `ConteudoTemplate`
- `VariaveisPermitidas`
- `Ativo`
- `VigenteDe`
- `VigenteAte`
- auditoria (`CriadoPorUsuarioId`, `AtualizadoPorUsuarioId` e campos herdados)

## Tipo de evento

O template e associado a `TipoEventoNotificacao` para que a selecao seja deterministica por contexto funcional do evento.

## Canal

O template reutiliza `CanalNotificacao`, sem introduzir novos canais nesta etapa.

## Versao

Cada template possui `Versao > 0`.

Selecao automatica:

1. filtra por `TipoEvento` e `Canal`;
2. considera apenas templates ativos e vigentes;
3. ordena por `Versao` decrescente;
4. escolhe a versao mais recente aplicavel.

## Vigencia

Foi adotada vigencia opcional com:

- `VigenteDe`
- `VigenteAte`

Regra estrutural:

- `VigenteAte` nula ou `VigenteDe` nula sao permitidas;
- quando ambas existem, `VigenteAte >= VigenteDe`.

## Ativo/inativo

O estado administrativo nesta etapa e controlado apenas por `Ativo`.

Nao foram criados status como rascunho, publicado ou arquivado, pois ainda nao existe fluxo administrativo correspondente.

## Variaveis permitidas

As variaveis permitidas sao persistidas no proprio template, em formato simples serializado em JSON.

Regras:

- nomes normalizados para minusculo;
- lista distinta;
- sintaxe restrita e validada;
- sem tabela relacional adicional de variaveis.

## Sintaxe de placeholders

Foi definida a sintaxe:

`{{variavel}}`

Exemplos:

- `{{chamado.codigo}}`
- `{{solicitante.nome}}`

Nao ha suporte a loops, condicionais, includes, chamadas de metodo, reflection arbitraria ou expressoes dinamicas.

## Selecao do template

O `MaterializarConteudoNotificacaoUseCase` permite:

- selecao explicita por `TemplateNotificacaoId`;
- ou selecao automatica por `TipoEvento` + `Canal`.

Na selecao automatica, o use case considera apenas templates ativos e vigentes na `DataReferencia`.

## Materializacao

Fluxo implementado:

1. validar o request;
2. localizar o template aplicavel;
3. validar ativo, vigencia, tipo de evento e canal;
4. validar variaveis permitidas;
5. extrair placeholders realmente usados;
6. rejeitar placeholder desconhecido;
7. rejeitar variavel usada e nao fornecida;
8. substituir placeholders;
9. validar limites finais de assunto e conteudo;
10. retornar assunto, conteudo, template e variaveis utilizadas.

## Validacao

O validator do request protege:

- enums validos;
- data de referencia valida;
- colecao de variaveis nao nula;
- limite de quantidade de variaveis;
- tamanho maximo de chaves e valores;
- `TemplateNotificacaoId` opcional nao vazio.

## Conteudo final

O conteudo final materializado e preparado para ser persistido em `Notificacao`.

Essa decisao preserva rastreabilidade:

- o envio futuro nao precisara reler o template vigente no momento do processamento;
- o historico da notificacao permanecera reproduzivel mesmo se o template mudar.

## Assunto

`AssuntoTemplate` permanece opcional, mas quando materializado precisa respeitar o limite da entidade `Notificacao`.

## Escape e seguranca

Foi implementada renderizacao deterministica e restrita:

- placeholders sao extraidos por regex controlada;
- placeholders malformados sao rejeitados;
- placeholders nao declarados sao rejeitados;
- variaveis nao permitidas sao rejeitadas;
- variaveis extras sao aceitas, mas nao entram no resultado se nao forem usadas.

Politica por canal:

- `Sistema`: texto simples sem escape HTML adicional;
- `Email`: valores inseridos no conteudo sao escapados com `HtmlEncoder`.

Nao foi declarada sanitizacao HTML completa nesta etapa.

## Idempotencia

A materializacao nao altera a estrategia de idempotencia existente.

Ela apenas prepara assunto e conteudo finais para a etapa de geracao persistente.

## Relacao com geracao

A composicao futura permanece separada:

1. resolver destinatarios;
2. materializar assunto e conteudo;
3. chamar `GerarNotificacaoUseCase`.

## Relacao com destinatarios

A materializacao nao resolve usuarios, grupos, perfis nem aprovadores.

Ela consome apenas os dados de contexto e variaveis explicitamente fornecidos.

## Persistencia

Foi criada persistencia explicita para `TemplateNotificacao` com:

- `DbSet<TemplateNotificacao>`;
- `TemplateNotificacaoConfiguration`;
- migration estrutural separada.

Nao foi criada FK obrigatoria de `Notificacao` para `TemplateNotificacao`.

## Indices

Foram criados:

- indice unico `Nome + Versao`;
- indice de busca por `TipoEvento + Canal + Ativo`;
- indice de vigencia;
- indices para FKs de auditoria.

## Constraints

Foram criadas constraints para:

- `Versao > 0`;
- vigencia valida.

## Migration estrutural

Foi criada a migration estrutural `CriarEstruturaTemplateNotificacaoSprint6`.

Ela contem apenas:

- tabela `templates_notificacao`;
- colunas;
- PK;
- FKs;
- indices;
- constraints.

Nao houve seed de templates funcionais nem criacao de estruturas de preferencia, fila ou entrega.

## Migration de checklist

Esta etapa tambem exige migration separada de checklist para concluir o item 8 da Sprint 6 e atualizar o progresso para `8/16 - 50%`.

## Compatibilidade com Worker.Email

Nenhuma alteracao foi feita no `Worker.Email`.

## Impacto em abertura

Preparacao estrutural para materializar notificacoes de abertura futura, sem integracao automatica neste item.

## Impacto em atendimento

Preparacao estrutural para materializar notificacoes de atribuicao, comentarios ou mudancas de status, sem integracao automatica neste item.

## Impacto em aprovacao

Preparacao estrutural para materializar notificacoes de aprovacao, sem disparo automatico nesta entrega.

## Impacto em SLA

Preparacao estrutural para materializar alertas futuros de SLA, sem processamento ou entrega nesta entrega.

## Impacto em fechamento e reabertura

Preparacao estrutural para notificacoes futuras de resolucao, fechamento e reabertura, ainda sem integracao operacional.

## O que nao foi implementado

- preferencias por usuario ou canal;
- fila, outbox ou retry;
- envio por e-mail;
- entrega pelo canal Sistema;
- endpoint administrativo;
- frontend;
- editor visual;
- integracoes automaticas com eventos ITSM.

## Riscos

- uso incorreto de variaveis nao declaradas por futuros produtores;
- mistura indevida entre materializacao e envio nas proximas etapas;
- necessidade futura de politica mais rica para HTML, caso templates de e-mail crescam em complexidade.

## Decisoes adiadas

- preferencias por usuario e evento;
- relacionamento opcional de `Notificacao` com `TemplateNotificacao`;
- editor administrativo;
- multiplos idiomas;
- sanitizacao HTML mais avancada;
- composicao automatica entre resolucao, materializacao e geracao;
- processamento e entrega por canal.

## Criterios de aceite

- template persistente modelado;
- configuracao EF explicita;
- migration estrutural separada;
- selecao por evento e canal;
- versao e vigencia respeitadas;
- placeholders seguros e restritos;
- variaveis permitidas validadas;
- variaveis ausentes rejeitadas;
- placeholders desconhecidos rejeitados;
- conteudo final compativel com `Notificacao`;
- testes de dominio, aplicacao e persistencia aprovados;
- nenhuma implementacao de envio.

## Proxima etapa

Implementar preferencias de notificacao por usuario e evento.
