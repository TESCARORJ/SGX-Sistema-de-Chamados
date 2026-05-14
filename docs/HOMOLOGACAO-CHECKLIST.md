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





## Sprint Portal 3 - Fluxo portal/admin

- [ ] Validar criacao de chamado em `/portal/chamados/novo`
- [ ] Validar upload de anexo permitido na abertura
- [ ] Validar mensagem de falha parcial de anexos
- [ ] Validar redirecionamento para `/portal/chamados/:id`
- [ ] Validar historico inicial no detalhe do portal
- [ ] Validar anexo exibido no detalhe do portal
- [ ] Validar chamado listado em `/portal/chamados`
- [ ] Validar chamado visivel em `/admin/chamados`
- [ ] Validar detalhe administrativo em `/admin/chamados/:id` (origem, historico, anexos)
- [ ] Validar erro amigavel para anexo invalido

## Abertura de chamado pelo portal

- [ ] Entrar como Solicitante.
- [ ] Abrir /portal/chamados/novo.
- [ ] Validar obrigatoriedade de titulo.
- [ ] Validar obrigatoriedade de descricao.
- [ ] Validar obrigatoriedade de categoria.
- [ ] Validar obrigatoriedade de prioridade.
- [ ] Abrir chamado com dados validos.
- [ ] Confirmar mensagem de sucesso.
- [ ] Confirmar redirecionamento para detalhe.
- [ ] Confirmar chamado listado em /portal/chamados.
- [ ] Confirmar historico inicial.
- [ ] Confirmar anexo no portal, se aplicavel.
- [ ] Confirmar chamado na fila administrativa.
- [ ] Confirmar detalhe no admin.
- [ ] Confirmar anexo no admin, se aplicavel.
- [ ] Confirmar que solicitante nao acessa chamado de outro solicitante.
- [ ] Confirmar que comentarios internos nao aparecem no portal.

## Integracao de e-mail - Sprint 2

- [ ] Confirmar e-mail novo abrindo chamado automaticamente
- [ ] Confirmar origem `Email` no chamado criado
- [ ] Confirmar status inicial `Aberto`
- [ ] Confirmar historico inicial `Chamado criado a partir de e-mail`
- [ ] Confirmar deduplicacao por `MessageId`
- [ ] Confirmar bloqueio por `DominiosPermitidos` (quando configurado)
- [ ] Confirmar log tecnico em sucesso/duplicado/ignorado/erro

## Integracao de e-mail - Sprint 3

- [ ] Confirmar correlacao por codigo no assunto (`SGX`/`CHM`)
- [ ] Confirmar correlacao por `InReplyTo`
- [ ] Confirmar correlacao por `References`
- [ ] Confirmar resposta correlacionada criando comentario publico
- [ ] Confirmar historico `Resposta recebida por e-mail`
- [ ] Confirmar anexo permitido salvo no chamado
- [ ] Confirmar anexo invalido rejeitado e registrado no log
- [ ] Confirmar sucesso parcial quando apenas anexos falham
- [ ] Confirmar `NaoCorrelacionado` quando houver indicio de resposta sem chamado

## Integracao de e-mail - Sprint 4

- [ ] Entrar como Administrador e abrir `/admin/integracoes/email`
- [ ] Confirmar listagem de logs
- [ ] Filtrar por status
- [ ] Filtrar por remetente
- [ ] Filtrar por assunto
- [ ] Filtrar por MessageId
- [ ] Filtrar por chamado/codigo
- [ ] Abrir detalhe do log
- [ ] Validar `MessageId`, `InReplyTo` e `References`
- [ ] Expandir `Erro tecnico`
- [ ] Abrir chamado vinculado quando houver `ChamadoId`
- [ ] Entrar como Solicitante e confirmar bloqueio

## Abertura por e-mail

- [ ] Configurar caixa IMAP de homologacao.
- [ ] Rodar Worker.Email.
- [ ] Enviar e-mail novo.
- [ ] Confirmar criacao do chamado.
- [ ] Confirmar origem E-mail.
- [ ] Confirmar status inicial Aberto.
- [ ] Confirmar historico inicial.
- [ ] Confirmar log em /admin/integracoes/email.
- [ ] Responder e-mail com codigo do chamado.
- [ ] Confirmar comentario no chamado.
- [ ] Enviar anexo permitido.
- [ ] Confirmar anexo no chamado.
- [ ] Enviar anexo invalido.
- [ ] Confirmar rejeicao e log tecnico.
- [ ] Validar duplicidade por MessageId.
- [ ] Validar e-mail nao correlacionado.
- [ ] Validar filtros da tela de logs.

## Sprint Autenticação 1 - Validação de autenticação corporativa

- [ ] Validar botão `Entrar com Microsoft Entra ID` na tela `/login`.
- [ ] Validar autenticação no tenant institucional do Microsoft Entra ID.
- [ ] Validar recebimento de `access token` no frontend.
- [ ] Validar chamada autenticada de `GET /api/me` com `Authorization: Bearer`.
- [ ] Validar criação automática de usuário interno quando aplicável.
- [ ] Validar retorno de perfis e permissões efetivas em `GET /api/me`.
- [ ] Validar redirecionamento para `/admin` conforme perfil permitido.
- [ ] Validar redirecionamento para `/portal` conforme perfil permitido.
- [ ] Validar redirecionamento para `/acesso-negado` quando não houver perfil permitido.
- [ ] Validar bloqueio de acesso sem token (`401`) e sem permissão (`403`).
- [ ] Validar que login local e emulação de perfis permanecem restritos a Development.

## Sprint Autenticação 2 - Backend Microsoft Entra ID e usuário interno

- [ ] Validar token Microsoft Entra ID com `issuer` e `audience` corretos.
- [ ] Validar rejeição de token com `issuer` inválido.
- [ ] Validar rejeição de token com `audience` inválida.
- [ ] Validar resolução por `preferred_username`.
- [ ] Validar fallback por `email`.
- [ ] Validar fallback por `upn`.
- [ ] Validar erro controlado quando não houver identificador confiável.
- [ ] Validar reutilização de usuário interno existente.
- [ ] Validar criação automática de usuário com perfil padrão `Solicitante`, quando habilitada.
- [ ] Validar bloqueio de usuário novo quando `CriarUsuarioAutomaticamente=false`.
- [ ] Validar bloqueio de usuário interno inativo.
- [ ] Validar aceitação de domínio permitido.
- [ ] Validar bloqueio de domínio não permitido.
- [ ] Validar `autenticadoPor=MicrosoftEntraId` no fluxo Microsoft.
- [ ] Validar que perfis e permissões permanecem internos no SGX.
- [ ] Validar que `roles/groups` do Azure AD não concedem perfil administrativo automaticamente.

## Sprint Autenticação 3 - Frontend de login e sessão

- [ ] Validar botão `Entrar com Microsoft Entra ID` em `/login`.
- [ ] Validar consumo de `GET /api/me` após login Microsoft.
- [ ] Validar redirecionamento por perfis internos do SGX (`/admin`, `/portal`, `/acesso-negado`).
- [ ] Validar refresh com F5 sem falso logoff em rota protegida.
- [ ] Validar refresh com Ctrl+F5 sem falso logoff em rota protegida.
- [ ] Validar que login local Development não aparece em Production.
- [ ] Validar que emulação aparece apenas em Development.
- [ ] Validar persistência da emulação em F5/Ctrl+F5 em Development.
- [ ] Validar logout explícito limpando sessão e emulação.

## Autenticação corporativa - Microsoft Entra ID

- [ ] Criar App Registration da SPA/frontend.
- [ ] Criar App Registration da API/resource, se aplicável.
- [ ] Configurar Redirect URI local.
- [ ] Configurar Redirect URI de homologação.
- [ ] Configurar Logout URI.
- [ ] Configurar escopo da API.
- [ ] Conceder consentimento no tenant.
- [ ] Configurar variáveis backend AzureAd__*.
- [ ] Configurar variáveis frontend VITE_AZURE_*.
- [ ] Confirmar que login local está desabilitado fora de Development.
- [ ] Homologar com tenant real Microsoft Entra ID.
- [ ] Acessar /login.
- [ ] Clicar em Entrar com Microsoft Entra ID.
- [ ] Testar usuário real do domínio institucional.
- [ ] Confirmar chamada GET /api/me com Authorization: Bearer.
- [ ] Confirmar retorno de perfis e permissões internas.
- [ ] Confirmar usuário novo criado como Solicitante, se regra estiver habilitada.
- [ ] Confirmar usuário interno inativo bloqueado.
- [ ] Confirmar Administrador acessando /admin.
- [ ] Confirmar Atendente acessando /admin.
- [ ] Confirmar Solicitante acessando /portal.
- [ ] Confirmar usuário sem perfil adequado em /acesso-negado.
- [ ] Testar logout corporativo.
- [ ] Confirmar refresh/F5 sem perda indevida de sessão.
- [ ] Testar MFA.
- [ ] Testar Conditional Access.
- [ ] Testar ambiente publicado/VPS.
- [ ] Revisar configuração com equipe responsável pelo Azure.
- [ ] Registrar evidência formal de homologação.

## Sprint Autenticação 7 - Administrador inicial seguro

- [ ] Validar criação do primeiro Administrador via `SGX_ADMIN_INICIAL_*`.
- [ ] Validar que não cria Administrador inicial quando variáveis estiverem ausentes.
- [ ] Validar que não cria segundo Administrador quando já existe Administrador ativo.
- [ ] Validar que a senha é armazenada com hash.
- [ ] Validar que senha fraca é rejeitada.
- [ ] Validar que e-mail inválido é rejeitado.
- [ ] Validar que nome vazio é rejeitado.
- [ ] Validar que log técnico não contém senha.
- [ ] Remover/rotacionar `SGX_ADMIN_INICIAL_*` após bootstrap inicial.

## Sprint Autenticação 8 - Recuperação de senha e hardening do login local SGX

- [ ] Validar `POST /api/auth/local/alterar-senha` com senha atual correta.
- [ ] Validar rejeição de alteração quando senha atual estiver incorreta.
- [ ] Validar rejeição de alteração quando confirmação divergir.
- [ ] Validar rejeição de senha fraca na alteração.
- [ ] Validar `DeveAlterarSenha=true` redirecionando para `/alterar-senha`.
- [ ] Validar bloqueio de navegação protegida enquanto `DeveAlterarSenha=true`.
- [ ] Validar `POST /api/auth/local/recuperar-senha/solicitar` sem revelar se e-mail existe.
- [ ] Validar geração de token de recuperação com expiração.
- [ ] Validar que token de recuperação é de uso único.
- [ ] Validar `POST /api/auth/local/recuperar-senha/redefinir` com token válido.
- [ ] Validar rejeição de token expirado.
- [ ] Validar lockout após limite de tentativas inválidas.
- [ ] Validar reset de tentativas após login bem-sucedido.
- [ ] Validar `UltimoLoginEm` atualizado em login local bem-sucedido.
- [ ] Validar que senha/token não aparecem em logs.
- [ ] Validar telas `/alterar-senha` e `/recuperar-senha` no frontend.
- [ ] Homologar envio transacional real de e-mail (pendência evolutiva).

## Autenticação corporativa - Single Tenant

- [ ] Validar login com usuário real do tenant institucional.
- [ ] Validar bloqueio de conta Microsoft pessoal.
- [ ] Validar bloqueio de usuário de tenant externo.
- [ ] Validar bloqueio de domínio fora da lista permitida.
- [ ] Validar token com issuer correto.
- [ ] Validar token com audience correta.
- [ ] Validar que usuário interno inativo é bloqueado.
- [ ] Validar que Azure roles/groups não concedem Administrador.
- [ ] Validar MFA, se política estiver ativa.
- [ ] Validar Conditional Access, se política estiver ativa.
- [ ] Registrar evidências formais.

## Integrações - Microsoft Entra ID (admin)

- [ ] Validar menu `Integrações` exibindo `Microsoft Entra ID`.
- [ ] Validar tela `/admin/integracoes/microsoft-entra-id`.
- [ ] Validar atualização de configuração via `PUT /api/admin/integracoes/microsoft-entra-id`.
- [ ] Validar bloqueio de Solicitante nos endpoints administrativos da integração.
- [ ] Validar que estado sem provedor ativo é rejeitado.
- [ ] Validar que o LoginView respeita `GET /api/auth/provedores`.

## Correção urgente - usuários demonstrativos

- [ ] Validar que, em base limpa, apenas 6 usuários demonstrativos oficiais estão ativos.
- [ ] Validar 2 Administradores ativos (`admin@sgxdigital.com`, `admin2@sgxdigital.com`).
- [ ] Validar 2 Atendentes ativos (`atendente.demo@sgxdigital.com`, `atendente2.demo@sgxdigital.com`).
- [ ] Validar 2 Solicitantes ativos (`solicitante.demo@sgxdigital.com`, `solicitante2.demo@sgxdigital.com`).
- [ ] Validar que usuários demonstrativos antigos (`@sgx.local`, `homol`, `local`) permanecem inativos.
- [ ] Validar que usuários demonstrativos legados inativados não são recriados automaticamente.
- [ ] Validar que usuários reais não são inativados automaticamente.
- [ ] Validar proteção do administrador inicial seguro (`SGX_ADMIN_INICIAL_*`).

## Correção urgente - configuração Microsoft Entra ID

- [ ] Validar indicação visual de obrigatoriedade para Tenant ID, Client ID, Audience, Issuer, Authority, API Scope e Redirect URI.
- [ ] Validar mensagens por campo ao salvar configuração Microsoft incompleta.
- [ ] Validar aviso do modo `Local`.
- [ ] Validar aviso do modo `Hibrido`.
- [ ] Validar aviso do modo `MicrosoftEntraId`.
- [ ] Validar que `Local` não exige campos Microsoft quando integração Microsoft estiver desabilitada.
- [ ] Validar mensagem específica para `Local` com login local desabilitado.
- [ ] Validar rejeição de `PUT` com Microsoft habilitado e qualquer campo obrigatório ausente.
- [ ] Validar rejeição de configuração sem nenhum provedor ativo.

## Cadastros - Redefinição de senha por Administrador

- [ ] Validar ação `Redefinir senha` em `/admin/cadastros/usuarios/:id`.
- [ ] Validar `POST /api/admin/cadastros/usuarios/{id}/redefinir-senha`.
- [ ] Validar rejeição de senha fraca.
- [ ] Validar rejeição de confirmação divergente.
- [ ] Validar `deveAlterarSenha=true` forçando troca no próximo login.
- [ ] Validar que Atendente/Solicitante não conseguem redefinir senha.
