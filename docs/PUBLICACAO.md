# Publicacao Linux

Guia da publicacao Linux do SGX Sistema de Chamados com frontend em Nginx, API em container ASP.NET, PostgreSQL e Worker de e-mail.

## Visao geral

- O frontend publicado acessa a API por rota relativa `/api`.
- O Nginx faz proxy reverso de `/api/*` para `http://api:8080`.
- A API e os containers publicados devem rodar em `Production`.
- `LocalDevelopment` fica bloqueado fora de `Development`.
- `LocalSgx` permanece disponivel como contingencia administrativa.

## Variaveis de ambiente

Defina via secrets, arquivo de ambiente do servidor ou orquestrador:

- Use `.env.example` apenas como modelo seguro.
- Nao versione `.env` com credenciais reais.
- No compose atual, API e Worker ja sobem fixos em `Production`, independente de `ASPNETCORE_ENVIRONMENT` ou `DOTNET_ENVIRONMENT` definidos no `.env` do host.

- `ConnectionStrings__DefaultConnection`
- `ASPNETCORE_ENVIRONMENT=Production`
- `DOTNET_ENVIRONMENT=Production`
- `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`
- `Authentication__LoginLocalHabilitado`
- `Authentication__ModoLocalHabilitado=false`
- `Authentication__ProvedorPrincipal`
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
- `AUTH_LOGIN_LOCAL_HABILITADO=true` quando a contingencia `LocalSgx` precisar permanecer ativa
- `AUTH_JWT_LOCAL_ISSUER`
- `AUTH_JWT_LOCAL_AUDIENCE`
- `AUTH_JWT_LOCAL_CHAVE_ASSINATURA`
- `Cors__AllowedOrigins__0` e demais, caso a API seja acessada diretamente sem o proxy do Nginx
- `EmailWorker__ImapHost`
- `EmailWorker__ImapPorta`
- `EmailWorker__Usuario`
- `EmailWorker__Senha`
- demais `EmailWorker__*` conforme a integracao de e-mail

## Frontend e proxy `/api`

- O build do frontend nao deve embutir `localhost` para a API.
- Em producao, o acesso deve ser relativo e passar pelo proxy do Nginx.
- O timeout do cliente HTTP pode ser ajustado por `VITE_HTTP_TIMEOUT_MS`, com padrao de 30 segundos.
- Exemplo de acesso externo:
  - `https://<host-ou-dominio>/login`
- Exemplo de acesso direto ao container exposto:
  - `http://<host>:8081/login`

### Nginx

- Mantem a SPA com `try_files`.
- Encaminha `/api/*` para `http://api:8080`.
- Repassa os headers:
  - `Host`
  - `X-Real-IP`
  - `X-Forwarded-For`
  - `X-Forwarded-Proto`
- Quando existir proxy TLS de borda, o Nginx interno deve preservar o `X-Forwarded-Proto` recebido em vez de regravar sempre como `http`.
- Usa timeout controlado no proxy.

## Login e autenticacao

### `LocalSgx`

- Continua habilitado como contingencia administrativa.
- Usa o fluxo local SGX com JWT local.
- Pode existir em producao/homologacao quando configurado.
- Exige `Authentication__LoginLocalHabilitado=true` e uma chave JWT propria no servidor.

### `LocalDevelopment`

- Existe somente em `Development`.
- Depende de `Authentication__ModoLocalHabilitado=true`.
- Nao deve funcionar em `Production`.

## Docker Compose

Os containers publicados devem incluir:

- `restart: unless-stopped`
- rotacao de logs com `json-file`
- API com `ASPNETCORE_ENVIRONMENT=Production`
- API com `Authentication__ModoLocalHabilitado=false`
- Worker com `DOTNET_ENVIRONMENT=Production`
- `Authentication__LoginLocalHabilitado` preservado para manter `LocalSgx` configuravel
- healthchecks para API e frontend
- `depends_on` somente com condicao segura quando o Compose suportar

### Healthchecks

- Finalidade:
  - confirmar que a API responde no endpoint de liveness sem depender de verificacoes de negocio;
  - confirmar que o Nginx do frontend esta servindo a SPA localmente, mesmo se a API estiver temporariamente indisponivel.
- API:
  - consulta interna em `http://localhost:8080/health/live`
- Frontend:
  - consulta interna em `http://127.0.0.1/`
- Estados:
  - `starting`: container ainda dentro da janela inicial de aquecimento;
  - `healthy`: ultimo healthcheck retornou sucesso;
  - `unhealthy`: o container falhou no numero configurado de tentativas consecutivas.
- Comandos de consulta:
  - `docker inspect sgx-api --format '{{json .State.Health}}'`
  - `docker inspect sgx-frontend --format '{{json .State.Health}}'`
- Comandos de diagnostico:
  - `docker compose ps`
  - `docker logs --tail 200 sgx-api`
  - `docker logs --tail 200 sgx-frontend`
  - `curl -fsS http://localhost:8080/health/live`
  - `curl -fsS http://localhost:8081/`
  - `curl -I http://localhost:8080/health/live`
  - `curl -I http://localhost:8081/api/health/live`
  - `curl -I -H 'X-Forwarded-Proto: https' http://localhost:8080/health/live`
  - `docker exec sgx-api bash -c 'echo ok'`
  - `docker exec sgx-api bash -c 'exec 3<>/dev/tcp/127.0.0.1/8080'`

## HTTPS e borda

- A API continua com `UseHttpsRedirection()` fora de `Development`.
- A API processa `X-Forwarded-For` e `X-Forwarded-Proto` antes de `UseHttpsRedirection()`, autenticacao e autorizacao.
- O ambiente publicado precisa encaminhar `X-Forwarded-Proto` corretamente a partir do proxy de borda.
- Se o TLS terminar no Nginx externo, balanceador ou proxy da infraestrutura, ele deve ser o ponto que conhece o esquema original da requisicao.
- O trafego entre containers continua em HTTP interno no Docker; o esquema HTTPS efetivo chega na API por `X-Forwarded-Proto=https`.
- Sem `X-Forwarded-Proto=https`, acessos HTTP diretos continuam sujeitos ao comportamento normal de `UseHttpsRedirection()`.
- O processamento de forwarded headers fica restrito a redes privadas e loopback esperadas para compose e proxies internos, com um salto maximo.
- Nao remova HSTS ou redirecionamento HTTPS sem necessidade.

## Resolucao da API no Nginx

- O frontend usa `resolver 127.0.0.11 valid=30s;`, apontando para o DNS interno do Docker.
- O `proxy_pass` usa variavel para permitir re-resolucao do hostname `api` apos recriacao do container.
- O prefixo `/api` e os codigos HTTP da API permanecem inalterados.
- Uploads, downloads e corpos de requisicao continuam passando pelo proxy.

## Publicacao

### Build e subida

```bash
cp .env.example .env
# ajustar os valores reais no servidor antes da subida
docker compose config
docker compose up -d --build
docker compose ps
```

### Verificacoes basicas

```bash
docker compose logs -f api
docker compose logs -f frontend
docker compose logs -f worker-email
curl -fsS http://localhost:8080/health/live
curl -fsS http://localhost:8081/
```

### Validacoes do proxy

```bash
docker exec sgx-frontend wget -q --spider http://127.0.0.1/
docker exec sgx-frontend sh -c 'grep -R "localhost:8080" -n /usr/share/nginx/html || true'
docker exec sgx-api bash -lc 'printf "GET /health/live HTTP/1.1\r\nHost: localhost\r\nConnection: close\r\n\r\n" | nc 127.0.0.1 8080'
```

## Banco e bootstrap

- O PostgreSQL deve estar acessivel pelo host configurado em `ConnectionStrings__DefaultConnection`.
- Migrations e seeds nao devem ser alterados nesta etapa.
- O bootstrap do primeiro administrador usa `SGX_ADMIN_INICIAL_*`.
- Remova ou rotacione esses segredos apos a primeira subida.

## Comandos de verificacao

```bash
docker compose config
docker compose ps
docker stats --no-stream
docker logs --tail 200 sgx-api
docker logs --tail 200 sgx-frontend
docker logs --tail 200 sgx-worker-email
```

## Resultado esperado

- frontend publicado sem `localhost` embutido para a API;
- `/api/*` funcionando pelo Nginx;
- API e Worker em `Production`;
- `LocalDevelopment` desabilitado em producao;
- `LocalSgx` preservado;
- healthchecks ativos;
- logs com rotacao;
- restart automatico dos containers.
