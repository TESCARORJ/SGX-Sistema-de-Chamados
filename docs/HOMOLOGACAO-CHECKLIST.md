# Checklist de Homologacao

## Infraestrutura

- [ ] Banco PostgreSQL criado
- [ ] Migrations aplicadas
- [ ] Secrets fora do repositorio
- [ ] CORS configurado para dominios reais
- [ ] Azure AD configurado

## Servicos

- [ ] API sobe
- [ ] Frontend sobe
- [ ] Worker.Email sobe
- [ ] Health checks respondem (`/health`, `/health/live`, `/health/ready`)

## Seguranca

- [ ] Swagger restrito por ambiente
- [ ] Endpoints administrativos protegidos
- [ ] Solicitante bloqueado em rotas admin
- [ ] Nao ha senha IMAP real versionada
- [ ] Nao ha segredo real em appsettings de producao

## Funcionalidades

- [ ] Login funcionando
- [ ] Portal funcionando
- [ ] Abertura de chamado funcionando
- [ ] Comentarios e anexos funcionando
- [ ] Area administrativa funcionando
- [ ] Cadastros administrativos funcionando
- [ ] Dashboard e indicadores SLA funcionando
- [ ] Integracao IMAP funcionando
- [ ] Logs de integracao de e-mail funcionando

## Qualidade

- [ ] `dotnet restore` OK
- [ ] `dotnet build` OK
- [ ] `dotnet test` OK
- [ ] `npm run build` OK
- [ ] Varredura de legado/segredos revisada
