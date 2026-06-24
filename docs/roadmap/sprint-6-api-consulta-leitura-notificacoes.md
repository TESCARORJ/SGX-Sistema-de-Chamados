# Sprint 6 - API de consulta, leitura e marcacao como nao lida

## Objetivo

Disponibilizar a API autenticada da caixa interna do SGX para que o proprio usuario consulte notificacoes entregues no canal `Sistema`, conte pendencias de leitura e altere o estado de leitura sem confundir entrega com visualizacao.

## Estado anterior

- `Notificacao` ja era o registro persistente de entrega.
- `Enviada` ja representava disponibilizacao no canal `Sistema`.
- Ainda nao existia estado funcional de leitura.
- Ainda nao existia API autenticada da caixa interna.

## Diferenca entre entrega e leitura

- `Enviada`: notificacao disponibilizada ao destinatario interno.
- `Lida`: notificacao marcada como visualizada pelo proprio destinatario.
- A leitura nao altera `StatusNotificacao`, `EnviadaEm`, tentativas, erro, conteudo, destinatario ou chave de idempotencia.

## Modelagem de `LidaEm`

- Campo persistido: `Notificacao.LidaEm`.
- Leitura derivada por `LidaEm.HasValue`.
- Nenhum booleano persistente adicional foi criado.

## Estado inicial

- Toda notificacao do canal `Sistema` entregue inicia nao lida.
- `RegistrarEnvio` garante `LidaEm = null` na transicao para `Enviada`.

## Marcar como lida

- Metodo de dominio: `MarcarComoLida(DateTime lidaEm, ...)`.
- Exige:
  - `Canal = Sistema`
  - `Status = Enviada`
  - `EnviadaEm` preenchido
  - data em UTC
  - `LidaEm >= CriadoEm`
  - `LidaEm >= EnviadaEm`
- Repeticao e idempotente e preserva a primeira data.

## Marcar como nao lida

- Metodo de dominio: `MarcarComoNaoLida(...)`.
- Exige a mesma elegibilidade da leitura.
- Apenas limpa `LidaEm`.
- Repeticao e idempotente.

## Idempotencia

- Marcar lida novamente retorna sucesso sem alterar a data original.
- Marcar nao lida novamente retorna sucesso sem nova alteracao.
- `GET` nao possui efeito colateral e nao marca automaticamente como lida.

## Datas UTC

- A leitura usa UTC do servidor.
- Datas anteriores a criacao ou a disponibilizacao sao rejeitadas.

## Notificacoes consultaveis

A caixa propria expõe somente notificacoes com:

- `Ativo = true`
- `Canal = Sistema`
- `Status = Enviada`
- `DestinatarioUsuarioId = usuario autenticado`

Nao sao expostas notificacoes:

- do canal `Email`
- de outro usuario
- pendentes
- agendadas
- em processamento
- falhas
- canceladas

## Isolamento por usuario

- Nenhum endpoint recebe `UsuarioId`.
- O usuario vem exclusivamente de `IUsuarioContextoAplicacaoService`.
- Todas as consultas e marcacoes filtram ownership no proprio `Where`.
- Acesso indevido retorna `404` para nao revelar existencia de notificacoes alheias.

## Identidade autenticada

- A API reutiliza o contexto autenticado existente do projeto.
- Nenhuma leitura direta de claims foi colocada no dominio.

## Ownership

Filtros aplicados em detalhe e marcacao:

- `Id`
- `DestinatarioUsuarioId = usuario atual`
- `Canal = Sistema`
- `Status = Enviada`
- `Ativo = true`

## Listagem

Endpoint:

- `GET /api/notificacoes/minhas`

Suporta:

- paginacao
- filtro `lida = true|false|null`
- ordenacao por `EnviadaEm DESC`, `CriadoEm DESC`, `Id DESC`
- `TotalNaoLidas` na resposta

## Filtros

- todas
- lidas
- nao lidas

## Paginacao

- `Pagina >= 1`
- `TamanhoPagina` entre `1` e `100`
- resposta com `Total`, `TotalPaginas`, `Pagina` e `TamanhoPagina`

## Ordenacao

- `EnviadaEm DESC`
- `CriadoEm DESC`
- `Id DESC`

## Contagem de nao lidas

Endpoint:

- `GET /api/notificacoes/minhas/nao-lidas/contagem`

Retorna apenas a quantidade de notificacoes internas nao lidas do usuario autenticado.

## Detalhe

Endpoint:

- `GET /api/notificacoes/minhas/{id}`

Retorna conteudo completo sem mutacao de estado.

## GET sem efeito colateral

- Obter detalhe nao marca leitura automaticamente.
- A alteracao de leitura depende de `PATCH` explicito.

## Endpoints

- `GET /api/notificacoes/minhas`
- `GET /api/notificacoes/minhas/{id}`
- `GET /api/notificacoes/minhas/nao-lidas/contagem`
- `PATCH /api/notificacoes/minhas/{id}/lida`
- `PATCH /api/notificacoes/minhas/{id}/nao-lida`

## Autorizacao

- Todos os endpoints usam `[Authorize]`.
- Autenticacao e suficiente para a propria caixa.
- Nenhuma permissao administrativa global foi exigida.

## Retornos HTTP

- `200 OK` em listagem, detalhe, contagem e marcacao bem-sucedida
- `404 Not Found` para ID inexistente ou sem ownership
- `401 Unauthorized` quando nao autenticado

## Contratos

- `ListarMinhasNotificacoesRequest`
- `MinhaNotificacaoResumoResponse`
- `ListarMinhasNotificacoesResponse`
- `MinhaNotificacaoDetalheResponse`
- `AlterarLeituraNotificacaoResponse`
- `ContagemMinhasNotificacoesNaoLidasResponse`

## Validators

- `ListarMinhasNotificacoesRequestValidator`
- validacao de IDs vazios feita nos use cases de detalhe e marcacao

## Persistencia

- coluna `lida_em` adicionada em `notificacoes`
- nenhum novo aggregate ou tabela duplicada de inbox

## Indices

- indice composto `ix_notificacoes_destinatario_canal_status_criado_em`

## Constraints

- `ck_notificacoes_lida_em_maior_ou_igual_enviada_em`

## Migration estrutural

- `AdicionarEstadoLeituraNotificacaoSprint6`
- contem apenas `lida_em`, indice novo e constraint temporal

## Migration de checklist

- `ConcluirApiConsultaLeituraNotificacoesSprint6Roadmap`
- contem apenas `UpdateData`

## Compatibilidade com canal Sistema

- A API trabalha exclusivamente sobre `CanalNotificacao.Sistema`.
- `Enviada` continua significando notificacao entregue/disponibilizada.

## Compatibilidade com canal Email

- Notificacoes de `Email` nao sao listadas, detalhadas nem marcadas como lidas nesta API.

## Compatibilidade com processamento

- Nenhuma acao de leitura reinicia processamento.
- Nenhuma acao altera tentativas, backoff ou envio.

## Compatibilidade com `Worker.Email`

- Nenhuma alteracao foi feita no worker inbound.

## Impacto em abertura

- sem integracao automatica nova nesta etapa

## Impacto em atendimento

- sem integracao automatica nova nesta etapa

## Impacto em aprovacao

- sem integracao automatica nova nesta etapa

## Impacto em SLA

- sem integracao automatica nova nesta etapa

## Impacto em fechamento e reabertura

- sem integracao automatica nova nesta etapa

## Testes

- dominio de `Notificacao`
- configuracao EF de `Notificacao`
- persistencia PostgreSQL de leitura
- use cases de listagem, detalhe e marcacao
- controller da API autenticada
- regressao dos canais `Sistema` e `Email`
- regressao de processamento e roadmap

## O que nao foi implementado

- frontend
- sino visual
- badge
- polling
- SignalR
- WebSocket
- push
- arquivamento
- exclusao pelo usuario
- agrupamento
- busca textual avancada
- administracao global da caixa

## Riscos

- `LidaEm` registra o estado atual de leitura, mas ainda nao ha historico detalhado das alternancias.
- A central frontend continua pendente e precisa respeitar o mesmo filtro de ownership da API.

## Decisoes adiadas

- marcar leitura automatica ao abrir detalhe
- arquivamento
- exclusao pelo usuario
- historico de leitura
- categorias visuais da inbox

## Criterios de aceite

- leitura persistida
- entrega separada de leitura
- API autenticada de caixa propria
- ownership por usuario
- filtro lida/nao lida
- contagem de nao lidas
- detalhe sem efeito colateral
- idempotencia de marcacao
- migrations estrutural e checklist separadas

## Proxima etapa

Implementar central de notificacoes no frontend.
