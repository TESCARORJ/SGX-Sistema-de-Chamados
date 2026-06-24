# Sprint 6 - Processamento e controle de tentativas de entrega

## Objetivo
Implementar o controle do ciclo de processamento das notificacoes persistidas, sem acoplar SMTP, canal Sistema, fila externa ou `Worker.Email`.

## Estado anterior
A Sprint 6 ja possuia `Notificacao` persistida, geracao idempotente, resolucao de destinatarios, templates/materializacao e preferencias por usuario, evento e canal. Ainda nao havia selecao processavel, inicio seguro nem controle de retry.

## Diagnostico do modelo atual
O modelo existente ja possuia `Status`, `QuantidadeTentativas`, `AgendadaEm`, `ProcessadaEm`, `EnviadaEm`, `FalhouEm` e `UltimoErro`. Isso foi suficiente para o controle agregado das tentativas sem criar tabela historica adicional.

## Estados utilizados
`Pendente`, `Agendada`, `EmProcessamento`, `Enviada`, `Falhou` e `Cancelada`.

## Criterios de processabilidade
Uma notificacao e processavel quando:
- esta ativa;
- esta em `Pendente`; ou
- esta em `Agendada` com `AgendadaEm <= data de referencia`;
- possui `QuantidadeTentativas` abaixo do limite;
- nao esta `Enviada`, `Cancelada`, `EmProcessamento` nem `Falhou`.

## Selecao
Foi criado `SelecionarNotificacoesProcessaveisUseCase`, com ordenacao por `AgendadaEm ?? CriadoEm`, depois `CriadoEm` e `Id`.

## Aquisicao concorrente
A aquisicao segura acontece em `IniciarProcessamentoNotificacaoUseCase` por meio de `ExecuteUpdateAsync` atomico em PostgreSQL/EF, com filtro condicional de estado e limite. Isso impede que duas execucoes iniciem a mesma notificacao em paralelo.

## Inicio do processamento
O inicio:
- valida a entrada;
- verifica elegibilidade operacional;
- marca `EmProcessamento`;
- grava `ProcessadaEm`;
- incrementa `QuantidadeTentativas` uma unica vez;
- limpa dados de falha anterior.

## Contador de tentativas
`QuantidadeTentativas` representa tentativas iniciadas. O contador nao e incrementado no registro de falha.

## Sucesso
`RegistrarSucessoEntregaNotificacaoUseCase` exige `EmProcessamento`, marca `Enviada`, grava `EnviadaEm` e limpa `AgendadaEm`, `FalhouEm` e `UltimoErro`.

## Falha transitoria
`RegistrarFalhaEntregaNotificacaoUseCase` registra erro e data da falha. Se a falha for transitoria e o limite ainda nao tiver sido atingido, a notificacao e reagendada e volta para `Agendada`.

## Falha definitiva
Falhas definitivas, ou falhas transitorias apos atingir o limite, encerram a notificacao em `Falhou`, sem novo agendamento.

## Limite de tentativas
O limite padrao implementado nesta etapa e `5`.

## Backoff
Politica deterministica em UTC:
- tentativa 1: `+1 minuto`
- tentativa 2: `+5 minutos`
- tentativa 3: `+15 minutos`
- tentativa 4: `+30 minutos`
- tentativa 5 ou mais: `+60 minutos`

## Reagendamento
O dominio ganhou `ReagendarAposFalha`, permitindo transicao explicita de `Falhou` para `Agendada` sem perder `UltimoErro` e `FalhouEm`.

## Semantica de `AgendadaEm`
`AgendadaEm` representa a data planejada da primeira entrega ou da proxima tentativa. Ela e limpa quando a notificacao encerra em `Falhou` ou `Enviada`.

## Semantica de `ProcessadaEm`
`ProcessadaEm` representa o inicio da ultima tentativa efetivamente iniciada.

## Semantica de `FalhouEm`
`FalhouEm` representa a ultima falha registrada, inclusive quando a notificacao e reagendada para nova tentativa.

## Semantica de `UltimoErro`
`UltimoErro` guarda o ultimo erro agregado conhecido. Nao existe historico individual de cada tentativa nesta etapa.

## Persistencia
Nao foi necessaria migration estrutural. O item reutilizou a tabela `notificacoes` e adicionou apenas contratos, use cases, repositorio especializado e testes.

## Transacao e concorrencia
A protecao critica ficou no inicio atomico do processamento. O restante do ciclo reaproveita as garantias de estado do dominio e do `SaveChangesAsync`.

## PostgreSQL
Os testes relacionais foram executados com PostgreSQL real via `Npgsql`, incluindo concorrencia sobre o inicio do processamento.

## Historico agregado versus detalhado
Foi adotado historico agregado: contador de tentativas e ultimo erro. Tabela detalhada por tentativa foi adiada.

## Relacao com geracao
A geracao idempotente continua separada. A notificacao so entra no ciclo de processamento depois de persistida.

## Relacao com preferencia
Preferencias sao avaliadas antes da geracao. Este item nao reavalia preferencia depois que a notificacao concreta ja existe.

## Relacao com template
Templates e materializacao continuam separados. O processamento nao altera assunto, conteudo nem destinatario.

## Relacao com envio futuro
O item prepara o ciclo operacional para que a proxima etapa conecte um transporte real por canal sem reescrever o controle de estado.

## Compatibilidade com `Worker.Email`
Nenhuma alteracao foi feita no `Worker.Email`, que permanece dedicado ao fluxo inbound.

## Impacto em abertura
Sem integracao automatica nesta etapa.

## Impacto em atendimento
Sem integracao automatica nesta etapa.

## Impacto em aprovacao
Sem integracao automatica nesta etapa.

## Impacto em SLA
Sem integracao automatica nesta etapa.

## Impacto em fechamento e reabertura
Sem integracao automatica nesta etapa.

## Testes
- `NotificacaoTests`
- `SelecionarNotificacoesProcessaveisUseCaseTests`
- `IniciarProcessamentoNotificacaoUseCaseTests`
- `RegistrarSucessoEntregaNotificacaoUseCaseTests`
- `RegistrarFalhaEntregaNotificacaoUseCaseTests`
- `ProcessamentoNotificacaoPersistenceTests`
- regressao de geracao, destinatarios, materializacao, preferencias, roadmap Sprint 6 e Sprint 5

## O que nao foi implementado
Sem SMTP, sem entrega pelo canal Sistema, sem fila externa, sem outbox, sem worker outbound, sem API e sem tabela detalhada de tentativas.

## Riscos
Ainda nao existe historico detalhado por tentativa. A futura etapa de transporte precisa respeitar a separacao entre aquisicao, execucao do canal e conclusao da tentativa.

## Decisoes adiadas
Entrega real por canal, worker outbound dedicado, historico individual de tentativas, outbox, dead-letter e dashboard operacional.

## Criterios de aceite
Selecao processavel, inicio seguro, sucesso, falha transitoria, falha definitiva, limite, backoff e concorrencia validos sem envio real.

## Proxima etapa
Implementar entrega pelo canal Sistema.
