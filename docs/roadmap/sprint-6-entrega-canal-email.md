# Sprint 6 - Entrega pelo canal Email

## Objetivo

Implementar a entrega outbound real de `Notificacao` pelo canal `Email`, usando payload ja materializado e sem alterar o fluxo inbound do `Worker.Email`.

## Estado anterior

- a notificacao ja existia persistida;
- destinatarios, preferencias, templates e materializacao ja existiam;
- o ciclo de processamento ja controlava tentativas, backoff e reagendamento;
- a entrega pelo canal `Sistema` ja estava implementada;
- ainda nao havia transporte outbound real.

## Separacao inbound/outbound

O `Worker.Email` permaneceu exclusivamente responsavel pelo fluxo inbound IMAP:

- leitura de caixa;
- criacao/correlacao de chamados;
- processamento de mensagens recebidas.

O envio outbound foi implementado na Infrastructure, sem alterar o projeto `Worker.Email`.

## Definicao da entrega por e-mail

Entregar por e-mail significa:

1. receber uma `Notificacao` ja em `EmProcessamento`;
2. validar canal e payload persistido;
3. enviar assunto e conteudo ao endereco snapshot da notificacao;
4. registrar sucesso ou falha no ciclo de processamento.

## Transporte adotado

Foi adotado `MailKit` com SMTP outbound.

## Interface de transporte

`ITransportadorEmailNotificacao` recebe um payload pronto e retorna sucesso ou falha classificada.

## Configuracao

Foi criada a secao `EmailOutbound` com:

- `Habilitado`;
- `Host`;
- `Port`;
- `RemetenteEndereco`;
- `RemetenteNome`;
- `Usuario`;
- `Senha`;
- `UsarSsl`.

## Secrets

Nenhuma credencial real foi versionada. A configuracao de exemplo permanece desabilitada e deve ser sobrescrita por secret manager, variavel de ambiente ou mecanismo equivalente.

## Remetente

O remetente e global e vem da configuracao `EmailOutbound`.

## Destinatario

O transporte usa `DestinatarioEndereco` persistido na propria `Notificacao`. O cadastro atual do usuario nao substitui o snapshot.

## Assunto

Para canal `Email`, o assunto e obrigatorio. Ausencia de assunto gera falha definitiva e nao tenta transporte.

## Conteudo

O conteudo usado e exatamente `Notificacao.Conteudo`. Nao ha releitura de template nem reinterpolacao.

## HTML/texto

Foi adotada a convencao atual do modulo: conteudo do canal `Email` e enviado como HTML. Essa escolha segue a materializacao ja preparada para escapar variaveis em templates de e-mail.

## Encoding

O `MimeMessage` e enviado em UTF-8 via MailKit/MimeKit.

## Estado necessario

A entrega exige:

- `Canal = Email`;
- `Status = EmProcessamento`;
- destinatario persistido valido;
- assunto materializado;
- conteudo materializado.

## Fluxo de entrega

1. validar request;
2. carregar notificacao;
3. validar estado e payload;
4. chamar `ITransportadorEmailNotificacao`;
5. em sucesso, registrar `Enviada`;
6. em falha transitoria, registrar falha e reagendar;
7. em falha definitiva, registrar falha e encerrar.

## Sucesso

Sucesso marca `Status = Enviada` e preenche `EnviadaEm`, reutilizando o caso de uso de sucesso do item 10.

## Falha transitoria

Falhas SMTP 4xx e erros operacionais de conectividade/protocolo sao tratadas como transitorias e passam pelo backoff ja existente.

## Falha definitiva

Falhas de configuracao, payload invalido, autenticacao invalida e respostas SMTP 5xx sao tratadas como definitivas.

## Backoff

O backoff nao foi duplicado. O item reutiliza a politica de `RegistrarFalhaEntregaNotificacaoUseCase`.

## Tentativas

O envio nao incrementa tentativas diretamente. O contador continua sendo incrementado apenas no inicio do processamento.

## Idempotencia

Se a notificacao ja estiver `Enviada`, o use case retorna sucesso idempotente e nao chama o transporte novamente.

## Limitacao de exactly-once

Nao foi prometida entrega exactly-once. Se o SMTP aceitar o envio e o processo cair antes da persistencia do sucesso, um retry futuro ainda pode reenviar. O estado persistido impede reenvio apenas depois do sucesso gravado.

## Concorrencia

O use case exige notificacao ja adquirida em `EmProcessamento`. Chamadas posteriores, apos sucesso persistido, retornam idempotencia e nao reenviam. A janela entre aceite do SMTP e persistencia continua documentada como limitacao.

## Logs

Os logs registram `NotificacaoId`, classificacao da falha e destinatario mascarado. Nao registram conteudo completo nem credenciais.

## Testes unitarios

- entrega valida;
- idempotencia;
- validacao de payload;
- falha transitoria;
- falha definitiva;
- rejeicao para canal `Sistema`.

## Testes SMTP

Foi usado servidor SMTP falso em processo local, sem internet e sem credenciais reais.

## Persistencia

Nao houve mudanca estrutural. O modelo atual de `Notificacao` ja suportava a entrega outbound.

## Migration estrutural

Nao aplicavel neste item.

## Migration de checklist

Foi criada migration de `UpdateData` para concluir o item 12 e atualizar o percentual do roadmap.

## Compatibilidade com `Worker.Email`

Nenhuma alteracao funcional foi feita no `Worker.Email`.

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

## O que nao foi implementado

- leitura de caixa postal outbound;
- anexos outbound;
- fila externa;
- outbox;
- worker outbound;
- API publica;
- frontend;
- alteracoes no inbound.

## Riscos

- ainda existe janela de duplicidade entre aceite do SMTP e persistencia do sucesso;
- a convencao atual assume HTML para o canal `Email`;
- API de consulta e marcacao de leitura seguem pendentes.

## Decisoes adiadas

- worker outbound dedicado;
- `Reply-To`;
- anexos outbound;
- persistencia de identificador externo;
- leitura/nao lida;
- frontend da central.

## Criterios de aceite

- transporte outbound separado do inbound;
- entrega por `Email` funcional;
- falha/sucesso integrados ao ciclo;
- idempotencia apos sucesso persistido;
- sem alteracao no `Worker.Email`.

## Proxima etapa

Criar API de consulta, leitura e marcacao como nao lida.
