# Checklist de Homologacao

## Infraestrutura

- [ ] Banco PostgreSQL criado
- [ ] Migrations aplicadas
- [ ] Secrets fora do repositório
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
- [ ] não ha senha IMAP real versionada
- [ ] não ha segredo real em appsettings de produção

## Perfis e permissoes

- [ ] Validar login como Administrador.
- [ ] Validar login como Atendente.
- [ ] Validar login como Solicitante.
- [ ] Validar GET /api/me com perfis e permissoes.
- [ ] Validar matriz de permissoes em perfil.
- [ ] Validar que Administrador altera permissoes.
- [ ] Validar que Atendente nao altera permissoes.
- [ ] Validar que Solicitante nao acessa /admin.
- [ ] Validar permissoes criticas.
- [ ] Validar controle visual por permissao.
- [ ] Validar bloqueio de backend para acao sem permissao.
- [ ] Validar emulacao de perfis em Development.

## Funcionalidades

- [ ] Login funcionando
- [ ] Portal funcionando
- [ ] Abertura de chamado funcionando
- [ ] comentários e anexos funcionando
- [ ] Area administrativa funcionando
- [ ] Cadastros administrativos funcionando
- [ ] Dashboard e indicadores SLA funcionando
- [ ] integração IMAP funcionando
- [ ] Logs de integração de e-mail funcionando

## Qualidade

- [ ] `dotnet restore` OK
- [ ] `dotnet build` OK
- [ ] `dotnet test` OK
- [ ] `npm run build` OK
- [ ] Varredura de legado/segredos revisada

## Roadmap ITSM

- [ ] Validar edicao de item do roadmap com `Status` (legado) e `StatusImplementacao`.
- [ ] Validar `StatusImplementacao = Implementado funcionalmente` com banner de alerta.
- [ ] Validar `StatusTecnico = Completo com pendencias evolutivas` com destaque visual.
- [ ] Validar percentual de implementacao (0 a 100).
- [ ] Validar CRUD de futuras implementacoes vinculado ao item.
- [ ] Validar concluir/inativar/reativar em futuras implementacoes.
- [ ] Validar acesso de consulta para Atendente (quando permitido).
- [ ] Validar bloqueio de mutacao para nao administrador.
- [ ] Validar categoria como dropdown de cadastro (nao texto livre) no CRUD do roadmap.
- [ ] Validar que categoria inativa nao aparece para novos itens.
- [ ] Validar fallback de categoria legada em itens antigos sem `RoadmapCategoriaId`.
- [ ] Validar checklist da implementacao com criar/editar/concluir/reabrir/inativar/reativar.
- [ ] Validar recalculo automatico do percentual ao marcar/desmarcar checklist.
- [ ] Validar ausencia de enums crus na UI (`EmValidacao`, `NaoIniciado`, `NaoAvaliado` etc.).




