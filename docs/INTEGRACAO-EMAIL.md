# Integracao de E-mail (Worker IMAP)

## Objetivo
A integracao de e-mail permite abrir e atualizar chamados automaticamente a partir de mensagens recebidas na caixa IMAP configurada.

## Configuracao IMAP
Variaveis suportadas no `EmailWorker__*`:

- `EmailWorker__ImapHost`
- `EmailWorker__ImapPorta`
- `EmailWorker__Usuario`
- `EmailWorker__Senha`
- `EmailWorker__Pasta`
- `EmailWorker__SslHabilitado`
- `EmailWorker__TlsHabilitado`
- `EmailWorker__IntervaloSegundos`
- `EmailWorker__MaxMensagensPorCiclo`
- `EmailWorker__CategoriaPadraoId`
- `EmailWorker__PrioridadePadraoId`
- `EmailWorker__DepartamentoPadraoId`
- `EmailWorker__DominiosPermitidos`
- `EmailWorker__TamanhoMaximoAnexoMb`
- `EmailWorker__ExtensoesPermitidas`

Regras importantes:
- senha IMAP nao deve ser versionada no repositorio;
- usar secret/env para valor real de `EmailWorker__Senha`;
- `appsettings*.json` deve conter apenas exemplos seguros.

## Como e-mail novo cria chamado
Para mensagem nova sem correlacao:

- identifica remetente e tenta localizar usuario interno por e-mail;
- se permitido pela regra atual, cria usuario solicitante quando nao existir;
- usa assunto como titulo (fallback: `Chamado aberto por e-mail`);
- usa corpo textual seguro (ou texto extraido do HTML);
- cria chamado com origem `Email`;
- aplica status inicial `Aberto`;
- cria historico inicial `Chamado criado a partir de e-mail`;
- registra processamento em `LogIntegracaoEmail`.

## Como resposta correlaciona chamado
A correlacao de resposta usa ordem de prioridade tecnica:

- codigo do chamado no assunto (`SGX-2026-000001`, `CHM-...`, `#SGX-...`);
- `Message-Id` de logs anteriores;
- `In-Reply-To`;
- `References`.

Quando correlaciona:
- nao cria novo chamado;
- adiciona comentario publico no chamado existente;
- registra historico de resposta;
- atualiza `LogIntegracaoEmail` com `Processado` e `ChamadoId`.

## Anexos
Comportamento atual:

- valida tamanho maximo (`EmailWorker__TamanhoMaximoAnexoMb`);
- valida extensoes permitidas (`EmailWorker__ExtensoesPermitidas`);
- bloqueia extensoes perigosas;
- rejeita anexos invalidos com log tecnico;
- falha em anexo nao derruba o processamento da mensagem.

Resultado esperado:
- anexos permitidos sao salvos;
- anexos invalidos sao rejeitados e logados;
- mensagem pode ficar como sucesso parcial, sem interromper o Worker.

## Logs administrativos
Consulta administrativa disponivel em:

- frontend: `/admin/integracoes/email`;
- endpoints:
  - `GET /api/admin/integracoes/email/logs`
  - `GET /api/admin/integracoes/email/logs/{id}`

Filtros principais:
- periodo;
- status;
- remetente;
- chamado/codigo;
- assunto;
- messageId.

Status exibidos:
- `Processado`
- `Erro`
- `Duplicado`
- `Ignorado`
- `Nao correlacionado`

## Limitacoes e pendencias
Pendencias reais para evolucao/homologacao:

- validacao IMAP real com caixa de homologacao;
- OAuth Microsoft (se exigido);
- retry/backoff;
- dead-letter;
- reprocessamento manual;
- monitoramento do Worker;
- sanitizacao avancada de HTML;
- antivirus/varredura de anexos;
- teste E2E com IMAP real.
