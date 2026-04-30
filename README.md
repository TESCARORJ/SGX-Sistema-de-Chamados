# GETI.SistemaChamado

Sistema corporativo de service desk com arquitetura DDD, backend Java 21 + Spring Boot, frontend Vue 3 + Quasar e worker IMAP dedicado para integracao por e-mail.

## Estrutura de modulos
- `GETI.SistemaChamado.Dominio`: entidades, agregados, VOs, enums e contratos de repositorio.
- `GETI.SistemaChamado.Aplicacao`: casos de uso, comandos, consultas, DTOs e servicos de orquestracao.
- `GETI.SistemaChamado.Infraestrutura`: persistencia JPA/Hibernate, Flyway e adaptadores tecnicos.
- `GETI.SistemaChamado.Api`: endpoints REST, seguranca, validacao e tratador global de excecao.
- `GETI.SistemaChamado.Worker.Email`: leitura IMAP, abertura/correlacao de chamados e log tecnico.
- `GETI.SistemaChamado.UI`: portal do solicitante e area administrativa em `/admin`.

## Pre-requisitos locais
- Java 21
- Maven 3.9+
- Node 20+
- npm 10+
- PostgreSQL local

## Banco PostgreSQL local (referencia)
- Database: `chamados_geti`
- Usuario: `chamados_geti_user`
- Senha: `chamadosgeti@001`
- Porta: `5432`

## Perfis Spring
- `local`: inclui `development` e habilita autenticacao tecnica local.
- `development`: base local de desenvolvimento.
- `hml`: homologacao com autenticacao corporativa.
- `prd`: producao com restricoes de exposicao e log.

Defina via `SPRING_PROFILES_ACTIVE`.

## Variaveis de ambiente principais
### API
- `SPRING_PROFILES_ACTIVE`
- `APP_SECURITY_OAUTH2_ISSUER_URI` (obrigatoria em `development` sem local, `hml` e `prd`)

### Worker IMAP
- `SPRING_PROFILES_ACTIVE`
- `APP_WORKER_EMAIL_IMAP_HOST`
- `APP_WORKER_EMAIL_IMAP_PORTA` (padrao: `993`)
- `APP_WORKER_EMAIL_IMAP_USUARIO`
- `APP_WORKER_EMAIL_IMAP_SENHA`
- `APP_WORKER_EMAIL_IMAP_PASTA` (padrao: `INBOX`)
- `APP_WORKER_EMAIL_IMAP_SSL_HABILITADO` (padrao: `true`)
- `APP_WORKER_EMAIL_IMAP_TLS_HABILITADO` (padrao: `true`)
- `APP_WORKER_EMAIL_IMAP_TIMEOUT_MILLIS` (padrao: `30000`)
- `APP_WORKER_EMAIL_IMAP_CONNECT_TIMEOUT_MILLIS` (padrao: `30000`)
- `APP_WORKER_EMAIL_IMAP_MAX_MENSAGENS_POR_CICLO` (padrao: `50`)

## Seguranca AD/Azure
- Estrategia oficial: OAuth2 Resource Server JWT do AD/Azure.
- Configuracao por `spring.security.oauth2.resourceserver.jwt.issuer-uri`.
- Backend e a fonte de verdade da autorizacao por `PerfilAcesso`.

### Modo local tecnico
- Habilitado apenas no profile `local`.
- Headers aceitos:
  - `X-Auth-Login` (obrigatorio)
  - `X-Auth-Nome` (opcional)
  - `X-Auth-Email` (opcional)
- Endpoints tecnicos da API (`/api/tecnico/**`) ficam desabilitados por padrao e devem ser expostos apenas por propriedade local.

## Flyway e migracoes
- Local: `GETI.SistemaChamado.Infraestrutura/src/main/resources/db/migration`
- Versoes atuais:
  - `V1__baseline.sql`
  - `V2__criar_persistencia_inicial.sql`
  - `V3__adicionar_login_usuario.sql`
  - `V4__criar_cadastros_mestres_administrativos.sql`
  - `V5__normalizar_auditoria_data_criacao.sql`
  - `V6__criar_nucleo_chamados_portal.sql`
  - `V7__expandir_operacao_administrativa_chamado.sql`
  - `V8__criar_log_integracao_email.sql`
  - `V9__adicionar_controle_sla_chamado.sql`
- `ddl-auto` permanece `validate` para impedir drift estrutural.

## Execucao local
### Backend (build completo)
```bash
mvn clean verify
```

Se necessario, use ferramentas locais do repositorio:
```powershell
$env:JAVA_HOME='C:\CREA\Sistema de Chamados\.tools\jdk-21'
& '.\.tools\apache-maven-3.9.9\bin\mvn.cmd' clean verify
```

### API
```bash
mvn -pl GETI.SistemaChamado.Api -am spring-boot:run -Dspring-boot.run.profiles=local
```

### Worker Email
```bash
mvn -pl GETI.SistemaChamado.Worker.Email -am spring-boot:run -Dspring-boot.run.profiles=local
```

### Frontend
```bash
cd GETI.SistemaChamado.UI
npm install
npm run lint
npm run build
npm run dev
```

## Endpoints de verificacao
- `GET /api/saude`
- `GET /actuator/health`
- `GET /actuator/info`
- `GET /api/me`
- `GET /api/portal/contexto`
- `GET /api/admin/contexto`

## Fluxo basico para homologacao
1. Aplicar variaveis de ambiente da API e Worker.
2. Garantir conectividade com PostgreSQL local/ambiente e IMAP corporativo.
3. Executar build backend e frontend.
4. Subir API, Worker e UI.
5. Validar fluxo minimo: login, abertura manual, abertura por e-mail, correlacao de resposta, operacao admin e indicadores SLA.

## Checklist
Checklist detalhado em `docs/HOMOLOGACAO-CHECKLIST.md`.

