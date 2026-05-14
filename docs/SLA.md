# SLA no SGX Sistema de Chamados

## Objetivo

O SLA no SGX define prazos de atendimento para chamados com foco em previsibilidade operacional, rastreabilidade e melhoria continua.

## Conceitos principais

- `Politica de SLA`: conjunto de regras ativo/inativo, ordenado e opcionalmente vinculado a categoria e departamento.
- `Meta de SLA`: prazo de primeira resposta e resolucao por prioridade dentro de uma politica.
- `ChamadoSla`: registro aplicado a um chamado especifico, com politica, prioridade, prazos, datas realizadas, violacoes, pausa e tipo de calculo.
- `Calendario corporativo`: agenda de expediente usada quando a politica calcula SLA em horario comercial.
- `Primeira resposta`: primeiro retorno publico de atendente ao solicitante ou mudanca para status `Em atendimento`.
- `Resolucao`: mudanca para status final, `Resolvido` ou `Encerrado`.

## Sprint 1 - Base administrativa

Entregas mantidas:

- entidades `PoliticaSla` e `MetaSla`;
- tabelas `sla_politicas` e `sla_metas`;
- seed idempotente da politica `SLA Padrao`;
- metas padrao por prioridade;
- DTOs, validacoes, endpoints e permissoes administrativas;
- tela administrativa em `/admin/sla/policies`.

Politica padrao:

- Baixa: primeira resposta `480` min, resolucao `2880` min.
- Media: primeira resposta `240` min, resolucao `1440` min.
- Alta: primeira resposta `60` min, resolucao `480` min.
- Critica: primeira resposta `30` min, resolucao `240` min.

## Sprint 2 - Aplicacao em chamados

Quando um chamado e criado, o caso de uso chama o servico de SLA. O servico localiza uma politica ativa, encontra a meta ativa da prioridade do chamado e cria um registro em `chamado_slas`.

A contagem comeca em `DataInicio`, usando o momento da abertura ou do recalculo. Os prazos iniciais sao:

- `PrazoPrimeiraResposta = DataInicio + TempoPrimeiraRespostaMinutos`
- `PrazoResolucao = DataInicio + TempoResolucaoMinutos`

Se `UsarHorarioComercial=false`, o calculo usa minutos corridos. Se `UsarHorarioComercial=true`, o calculo usa o calendario corporativo vinculado a politica ou, na ausencia dele, o calendario padrao ativo.

## Escolha da politica

Somente politicas e metas ativas sao consideradas. A escolha segue esta ordem:

- politica compativel com categoria do chamado tem maior peso;
- politica compativel com departamento tem peso adicional;
- politica geral ativa tambem concorre;
- em empate, vence a menor `Ordem` e depois o nome.

Se nao existir politica ou meta para a prioridade, o chamado continua sendo criado normalmente e o SLA fica `NaoAplicavel`.

## Primeira resposta

A primeira resposta e registrada quando:

- um atendente adiciona comentario publico no chamado; ou
- o chamado muda para status `Em atendimento`.

O sistema preenche `DataPrimeiraResposta`, calcula `MinutosPrimeiraResposta` descontando pausas encerradas e marca:

- `PrimeiraRespostaCumprida=true` quando a resposta ocorre dentro do prazo;
- `PrimeiraRespostaViolada=true` quando ocorre fora do prazo.

`DataPrimeiraResposta` nao e sobrescrita se ja existir.

## Resolucao

A resolucao e registrada quando o chamado entra em status final, `Resolvido` ou `Encerrado`, ou quando o fluxo administrativo de encerramento e executado.

O sistema preenche `DataResolucao`, calcula `MinutosResolucao` descontando pausas encerradas e marca:

- `ResolucaoCumprida=true` quando a resolucao ocorre dentro do prazo;
- `ResolucaoViolada=true` quando ocorre fora do prazo.

`DataResolucao` nao e sobrescrita. Em reabertura, a implementacao atual limpa a resolucao do SLA e recalcula o prazo de resolucao a partir da reabertura.

## Pausa

Se a politica aplicada tiver `PausarQuandoAguardandoSolicitante=true`, o SLA pausa quando o chamado entra no status `AguardandoSolicitante`.

Ao pausar:

- `Pausado=true`;
- `DataPausa` recebe o momento da pausa.

Ao sair desse status:

- o tempo parado e somado em `MinutosPausados`;
- prazos ainda pendentes sao prorrogados pela duracao da pausa;
- `Pausado=false` e `DataPausa=null`.

## Situacao do SLA

A situacao atual e calculada pela camada de aplicacao:

- `NaoAplicavel`: chamado sem `ChamadoSla`;
- `Pausado`: SLA pausado;
- `Cumprido`: resolucao registrada dentro do prazo;
- `Violado`: resolucao registrada fora do prazo;
- `Vencido`: chamado aberto com prazo de resolucao expirado;
- `ProximoDoVencimento`: faltam ate 60 minutos ou ate 20% do tempo total;
- `DentroDoPrazo`: demais chamados com SLA ativo.

## Exibicao e filtros

O detalhe administrativo e o detalhe do portal exibem politica aplicada, prioridade, prazos, primeira resposta, resolucao, situacao, minutos restantes/excedidos, pausa e tipo de calculo. Quando o SLA usa horario comercial, o detalhe administrativo tambem exibe o calendario usado.

A listagem administrativa e a listagem do portal exibem resumo com situacao do SLA, prioridade e prazo de resolucao. A listagem administrativa aceita filtros por `slaVencido` e `slaSituacao`, incluindo vencido, proximo, dentro do prazo, cumprido, violado, pausado e sem politica aplicada.

## Limites atuais

- Nao ha politica por tipo de chamado porque o dominio atual nao possui esse conceito.
- Pausas abertas ainda nao descontam minutos em tempo real no calculo de minutos realizados; o prazo e ajustado quando a pausa e encerrada.
- Homologacao funcional com base PostgreSQL real ainda deve ser executada.

## Melhorias futuras

- calendario corporativo por departamento/time;
- importacao automatica de feriados nacionais, estaduais e municipais;
- SLA por tipo de chamado, se o dominio passar a ter esse conceito;
- relatorios historicos de violacao e produtividade;
- indicadores por departamento/categoria;
- evidencias formais de homologacao ponta a ponta.

## Sprint 3 - Alertas, eventos e painel

A Sprint 3 adiciona monitoramento periodico do SLA, historico tecnico de eventos e painel gerencial administrativo.

### Configuracao de alertas

A configuracao fica em `configuracoes_alerta_sla` e pode ser administrada em `Admin > SLA > Alertas`.

Valores padrao:

- ativo: `true`;
- primeira resposta proxima do vencimento: `30` minutos antes;
- resolucao proxima do vencimento: `120` minutos antes;
- notificar atendente: `true`;
- notificar gestor: `false`;
- notificar departamento: `false`.

Endpoints:

- `GET /api/admin/sla/alert-config`
- `PUT /api/admin/sla/alert-config`

### Monitoramento periodico

O background service `SlaMonitoringBackgroundService` executa a verificacao conforme `SlaMonitoring` no `appsettings`.

Exemplo:

```json
"SlaMonitoring": {
  "Enabled": true,
  "IntervalMinutes": 5
}
```

Quando ativo, o job busca chamados com `ChamadoSla`, ignora chamados sem SLA, ignora SLA pausado e registra eventos para:

- primeira resposta proxima do vencimento;
- resolucao proxima do vencimento;
- primeira resposta vencida;
- resolucao vencida.

Erros por chamado sao registrados em log e nao interrompem o ciclo completo.

### Controle contra duplicidade

Eventos possuem `ChaveIdempotencia` e indice unico quando preenchida. As chaves seguem o padrao:

- `chamado-sla:{ChamadoSlaId}:sla-aplicado`
- `chamado-sla:{ChamadoSlaId}:primeira-resposta-registrada`
- `chamado-sla:{ChamadoSlaId}:resolucao-registrada`
- `chamado-sla:{ChamadoSlaId}:primeira-resposta-proximo-vencimento`
- `chamado-sla:{ChamadoSlaId}:resolucao-proximo-vencimento`
- `chamado-sla:{ChamadoSlaId}:primeira-resposta-vencida`
- `chamado-sla:{ChamadoSlaId}:resolucao-vencida`

### Eventos registrados

Os eventos ficam em `eventos_sla`:

- `SlaAplicado`
- `PrimeiraRespostaDentroDoPrazo`
- `PrimeiraRespostaVencida`
- `ResolucaoDentroDoPrazo`
- `ResolucaoVencida`
- `SlaPausado`
- `SlaRetomado`
- `AlertaPrimeiraRespostaProximoVencimento`
- `AlertaResolucaoProximoVencimento`
- `AlertaPrimeiraRespostaVencida`
- `AlertaResolucaoVencida`
- `AlertaEnviado`

O detalhe administrativo do chamado exibe a secao `Historico de SLA` com data, tipo, descricao e usuario quando houver.

### Painel de SLA

O painel fica em `Admin > SLA > Painel` e usa `GET /api/admin/sla/dashboard`.

Indicadores disponiveis:

- total de chamados com SLA aplicado;
- vencidos;
- proximos do vencimento;
- dentro do prazo;
- cumpridos;
- violados;
- percentual de cumprimento;
- tempo medio de primeira resposta;
- tempo medio de resolucao;
- agrupamento por prioridade;
- agrupamento por categoria;
- agrupamento por departamento.

Filtros disponiveis:

- data inicial;
- data final;
- prioridade;
- categoria;
- departamento;
- situacao do SLA.

### Relatorio futuro

`GET /api/admin/sla/report` retorna dados estruturados para futura exportacao, incluindo chamado, prioridade, categoria, departamento, politica, prazos, datas realizadas, situacao e minutos apurados.

### Limites da Sprint 3

- Eventos sao registrados e preparados para notificacao futura, mas nao ha envio real de e-mail, WhatsApp ou push.
- `AlertaEnviado` fica reservado para integracao posterior com servico oficial de notificacao.
- Exportacao Excel/PDF ainda nao foi implementada.

## Sprint 4 - Calendario corporativo e horario comercial

A Sprint 4 implementa calendario corporativo real para politicas com `UsarHorarioComercial=true`.

### Estrutura de calendario

Novas tabelas:

- `calendarios_corporativos`: nome, descricao, ativo, padrao e time zone.
- `horarios_atendimento_calendario`: janelas semanais de atendimento por dia da semana.
- `excecoes_calendario_corporativo`: feriados, recessos, expediente especial e dias sem expediente.

O seed cria o `Calendario Corporativo Padrao`, ativo, padrao, em `America/Sao_Paulo`, com expediente de segunda a sexta das `09:00` as `18:00`. Sabado e domingo ficam sem expediente por nao terem horario cadastrado.

### Configuracao administrativa

Calendarios sao administrados em `Admin > SLA > Calendarios`.

Endpoints:

- `GET /api/admin/sla/calendars`
- `GET /api/admin/sla/calendars/{id}`
- `POST /api/admin/sla/calendars`
- `PUT /api/admin/sla/calendars/{id}`
- `PATCH /api/admin/sla/calendars/{id}/status`
- `PATCH /api/admin/sla/calendars/{id}/default`
- `POST /api/admin/sla/calendars/{id}/schedules`
- `PUT /api/admin/sla/calendars/{id}/schedules/{scheduleId}`
- `DELETE /api/admin/sla/calendars/{id}/schedules/{scheduleId}`
- `POST /api/admin/sla/calendars/{id}/exceptions`
- `PUT /api/admin/sla/calendars/{id}/exceptions/{exceptionId}`
- `DELETE /api/admin/sla/calendars/{id}/exceptions/{exceptionId}`

O cadastro valida sobreposicao de horarios ativos no mesmo dia e permite manter apenas um calendario padrao ativo.

### Vinculo com politica de SLA

A politica possui `CalendarioCorporativoId` opcional.

Regra de escolha:

- se `UsarHorarioComercial=false`, o SLA continua usando minutos corridos;
- se `UsarHorarioComercial=true` e a politica possui calendario ativo, esse calendario e usado;
- se a politica nao possui calendario, o calendario padrao ativo e usado;
- se nao existir calendario padrao ativo, o calculo cai para minutos corridos para nao bloquear o chamado.

A tela `Admin > SLA > Politicas` exibe a selecao de calendario apenas quando `UsarHorarioComercial` esta ativo. Sem selecao explicita, a politica usa o calendario padrao.

### Calculo de minutos uteis

O calculo fica centralizado em `SlaBusinessTimeCalculator`.

Metodos principais:

- `AddBusinessMinutes`: soma minutos uteis a partir de uma data/hora.
- `CountBusinessMinutes`: conta minutos uteis entre duas datas.
- `IsBusinessTime`: identifica se uma data/hora esta dentro do expediente.
- `NextBusinessTime`: avanca para o proximo periodo util.

O calculador:

- converte datas para o time zone do calendario;
- ignora dias sem horario cadastrado;
- ignora feriados, recessos e dias sem expediente;
- substitui o horario semanal quando houver expediente especial;
- retorna os prazos em UTC para persistencia.

### Efeito no SLA aplicado

Ao aplicar SLA em chamado com horario comercial:

- `PrazoPrimeiraResposta` usa `AddBusinessMinutes`;
- `PrazoResolucao` usa `AddBusinessMinutes`;
- `MinutosPrimeiraResposta` usa `CountBusinessMinutes`;
- `MinutosResolucao` usa `CountBusinessMinutes`;
- pausas encerradas usam minutos uteis quando o SLA aplicado possui calendario.

O registro `ChamadoSla` guarda `UsarHorarioComercial` e `CalendarioCorporativoId` para manter rastreabilidade mesmo que a politica seja alterada depois.

### Limites da Sprint 4

- Ainda nao ha calendario especifico por departamento; a modelagem ja permite evoluir a politica para esse uso.
- Feriados nao sao importados automaticamente.
- Excecoes recorrentes ainda devem ser cadastradas manualmente.
- O reaproveitamento avancado de prazo remanescente em reabertura continua pendente de refinamento funcional.
