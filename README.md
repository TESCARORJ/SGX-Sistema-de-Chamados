# GETI.SistemaChamado

Fundacao tecnica do Sistema de Chamados com arquitetura DDD, backend Spring Boot multi-modulo e frontend Vue 3 + Quasar.

## Estrutura
- `GETI.SistemaChamado.Dominio`
- `GETI.SistemaChamado.Aplicacao`
- `GETI.SistemaChamado.Infraestrutura`
- `GETI.SistemaChamado.Api`
- `GETI.SistemaChamado.Worker.Email`
- `GETI.SistemaChamado.UI`

## Pre-requisitos
- Java 21
- Maven 3.9+
- Node 20+
- npm 10+
- PostgreSQL local

## Banco local (referencia)
- Database: `chamados_geti`
- Usuario: `chamados_geti_user`
- Senha: `chamadosgeti@001`
- Porta: `5432`

## Build do backend
```bash
mvn clean verify
```

## Rodar API
```bash
mvn -pl GETI.SistemaChamado.Api -am spring-boot:run -Dspring-boot.run.profiles=development
```

## Rodar Worker de e-mail
```bash
mvn -pl GETI.SistemaChamado.Worker.Email spring-boot:run
```

## Frontend
```bash
cd GETI.SistemaChamado.UI
npm install
npm run dev
```

## Endpoints iniciais
- API saude aplicacional: `GET /api/saude`
- Actuator health: `GET /actuator/health`
- Actuator info: `GET /actuator/info`
- Usuario autenticado: `GET /api/me`
- Contexto portal autenticado: `GET /api/portal/contexto`
- Contexto admin autenticado: `GET /api/admin/contexto`

## Perfis
- `development`
- `local`
- `hml`
- `prd`

Defina via `SPRING_PROFILES_ACTIVE`.

## Autenticacao corporativa (Sprint 5)
- Estrategia principal: OAuth2 Resource Server com JWT do AD/Azure.
- Propriedade obrigatoria para `development` (sem `local`), `hml` e `prd`:
  - `APP_SECURITY_OAUTH2_ISSUER_URI`

### Modo local isolado
- No profile `local`, a autenticacao corporativa real fica desabilitada e e usado um modo tecnico isolado.
- Headers aceitos:
  - `X-Auth-Login` (obrigatorio)
  - `X-Auth-Nome` (opcional)
  - `X-Auth-Email` (opcional)
- O backend continua aplicando autorizacao por `PerfilAcesso` interno sincronizado no banco.

