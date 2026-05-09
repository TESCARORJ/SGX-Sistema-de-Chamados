# Arquitetura SGX.SistemaChamado

## Visao em camadas

- `Domain`: entidades, enums e regras centrais de negocio.
- `Application`: DTOs, interfaces, use cases, servicos de dominio (SLA, e-mail).
- `Infrastructure`: EF Core, repositórios, `DbContext`, migrations, armazenamento local de anexos.
- `Api`: endpoints HTTP, autenticação/autorização, middlewares, health checks.
- `Worker.Email`: ingestao IMAP, processamento de mensagens, correlacao e deduplicacao.
- `Web`: SPA Vue/Quasar para portal e administracao.

## Domain

Elementos principais:

- `Chamado`, `ComentarioChamado`, `HistoricoChamado`, `AnexoChamado`
- `usuário`, `PerfilAcesso`, `UsuarioPerfilAcesso`
- `CategoriaChamado`, `PrioridadeChamado`, `StatusChamado`
- `SlaControle`, `SlaConfiguracao`
- `LogIntegracaoEmail`

## Application

Responsabilidades:

- orchestration de regras por use case
- contratos (`Interfaces`) consumidos por API e Worker
- servicos de SLA
- servicos de e-mail:
  - `IEmailCorrelationService`
  - `IEmailMessageProcessor`
  - `ICodigoChamadoService`

## Infrastructure

Responsabilidades:

- `SGXSistemaChamadoDbContext`
- mapeamentos EF Core e migrations
- `Repository<T>` e `UnitOfWork`
- `LocalArquivoStorageService`
- registro de dependencias (`AddInfrastructure`)

## API

Responsabilidades:

- endpoints `/api/portal/*`, `/api/admin/*`, `/api/me`, `/api/info`, `/api/saude`
- policies de autorização por perfil interno
- middlewares:
  - `CorrelationIdMiddleware`
  - `GlobalExceptionMiddleware`
- health checks:
  - `/health`
  - `/health/live`
  - `/health/ready`

## Worker.Email

Fluxo:

1. Leitura de mensagens IMAP por ciclo.
2. Processamento com deduplicacao (`MessageId`/`Fingerprint`).
3. Correlacao por codigo no assunto ou headers (`In-Reply-To`, `References`).
4. Abertura de chamado novo ou comentário em chamado existente.
5. Persistencia de `LogIntegracaoEmail`.
6. Pos-processamento IMAP (marcar/mover) conforme configuração.

## Fluxo Portal

1. usuário autenticado acessa `/portal`.
2. Abre chamado, comenta, anexa e acompanha status/SLA.
3. Isolamento de dados por solicitante.

## Fluxo Admin

1. `Administrador` e `Atendente` acessam `/admin`.
2. Atendimento operacional de chamados e dashboard.
3. Cadastros mutaveis e parâmetros restritos a `Administrador`.

## Fluxo E-mail

1. E-mail novo sem correlacao -> novo chamado (`Origem=Email`).
2. Resposta correlacionada -> comentário público.
3. Falha em mensagem individual não derruba worker.

## Fluxo SLA

- abertura cria controle de SLA
- assumir/atribuir registra primeira resposta quando aplicavel
- mudanca de prioridade/categoria pode recalcular prazo
- status com pausa interrompe/retoma contagem
- encerramento e reabertura ajustam ciclo de resolução

## autenticação x autorização

- Azure AD autentica.
- SGX autoriza por perfis e permissoes internas.
- Em Development, modo local tecnicamente suportado (`X-Dev-*`).

## Persistencia

- PostgreSQL via EF Core.
- Estrategia de inativacao em vez de exclusao fisica para cadastros.
- Anexos em storage local configuravel.






