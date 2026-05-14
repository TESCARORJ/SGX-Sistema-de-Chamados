# Publicacao

## variáveis de produção

Configurar via cofre/secrets manager/variáveis de ambiente:

- `ConnectionStrings__DefaultConnection`
- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`
- `Authentication__ProvedorPrincipal`
- `Authentication__LoginLocalHabilitado`
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura`
- `Authentication__JwtLocalExpiracaoMinutos`
- `Authentication__PoliticaSenha__TamanhoMinimo`
- `Authentication__PoliticaSenha__ExigirMaiuscula`
- `Authentication__PoliticaSenha__ExigirMinuscula`
- `Authentication__PoliticaSenha__ExigirNumero`
- `Authentication__PoliticaSenha__ExigirEspecial`
- `Authentication__PoliticaSenha__BloquearSenhaAnterior`
- `Authentication__Lockout__TentativasMaximas`
- `Authentication__Lockout__MinutosBloqueio`
- `Authentication__RecuperacaoSenha__ExpiracaoMinutos`
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`
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
- não logar `SGX_ADMIN_INICIAL_SENHA`
- restringir Swagger fora de Development (`Swagger__EnableInNonDevelopment`)
- revisar politicas de autorização em `/api/admin/*`

### Bootstrap do primeiro Administrador

- Usar variáveis `SGX_ADMIN_INICIAL_*` apenas na implantação inicial.
- Garantir senha forte e exclusiva.
- Remover/rotacionar variáveis após a criação do primeiro Administrador.
- Não usar `Admin@123456` em produção.

### Recuperação de senha local SGX

- `POST /api/auth/local/recuperar-senha/solicitar` retorna sempre mensagem genérica.
- `POST /api/auth/local/recuperar-senha/redefinir` exige token válido, não expirado e de uso único.
- Token de recuperação é armazenado apenas como hash (`token_hash`).
- Não registrar token ou senha em logs de produção.
- Pendência evolutiva: envio transacional real de e-mail deve ser homologado no ambiente publicado.

## Status de vulnerabilidade MailKit

- warning `NU1902 / GHSA-9j88-vvj5-vhgr` foi mitigado nesta sprint com atualizacao para `MailKit 4.16.0`.




