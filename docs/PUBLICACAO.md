# Publicacao

## variáveis de produção

Configurar via cofre/secrets manager/variáveis de ambiente:

- `ConnectionStrings__DefaultConnection`
- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`
- `Cors__AllowedOrigins__0` (e demais)
- `EmailWorker__ImapHost`
- `EmailWorker__ImapPorta`
- `EmailWorker__Usuario`
- `EmailWorker__Senha`
- `EmailWorker__Pasta`
- demais `EmailWorker__*` conforme necessidade

## Docker

Arquivos:

- `src/SGX.SistemaChamado.Api/Dockerfile`
- `src/SGX.SistemaChamado.Worker.Email/Dockerfile`
- `src/SGX.SistemaChamado.Web/Dockerfile`
- `docker-compose.yml` (referencia local/dev)

## Publicacao da API

- publicar imagem da API
- configurar `ASPNETCORE_ENVIRONMENT=Production`
- habilitar HTTPS, HSTS e CORS com origem explicita

## Publicacao do Worker

- publicar imagem dedicada do Worker
- configurar conexao de banco
- configurar IMAP por segredo externo
- monitorar logs de processamento por `CorrelationId` quando aplicavel

## Publicacao do Frontend

- build da SPA com `VITE_API_BASE_URL` do ambiente alvo
- servir por nginx/reverse proxy

## Logs e observabilidade

- API com logs estruturados
- `X-Correlation-Id` em request/response
- health endpoints:
  - `/health`
  - `/health/live`
  - `/health/ready`

## Backup e rollback

- manter rotina de backup do PostgreSQL
- validar restore periodicamente
- versionar releases para rollback de imagem/aplicacao

## Seguranca

- não versionar segredos reais
- não logar senha IMAP/token
- restringir Swagger fora de Development (`Swagger__EnableInNonDevelopment`)
- revisar politicas de autorização em `/api/admin/*`

## Status de vulnerabilidade MailKit

- warning `NU1902 / GHSA-9j88-vvj5-vhgr` foi mitigado nesta sprint com atualizacao para `MailKit 4.16.0`.




