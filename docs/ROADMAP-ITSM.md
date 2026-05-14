# Roadmap ITSM - SGX Sistema de Chamados

## Objetivo do Roadmap ITSM

O Roadmap ITSM organiza a evolucao funcional e tecnica do SGX Sistema de Chamados com foco em governanca, previsibilidade e rastreabilidade de entrega.

## Campos principais do item de roadmap

- Area
- Categoria (legado)
- Objetivo
- RoadmapCategoriaId (referencia principal)
- Ordem
- SituacaoAtual
- AtencaoTecnica
- Status (geral/legado)
- Prioridade
- Impacto
- Decisao
- Responsavel
- PrazoAlvo
- Ativo
- Observacao

## Categoria como cadastro controlado

A categoria do Roadmap ITSM agora e controlada por cadastro em tabela propria (`RoadmapCategoria`).

Regras aplicadas:
- nome obrigatorio e unico;
- inativacao logica (sem exclusao fisica);
- dropdown de criacao/edicao usa somente categorias ativas;
- itens antigos continuam exibindo categoria legada quando necessario.

Campos da categoria:
- Nome
- Descricao
- Cor
- Icone
- Ordem
- Ativo

## Status real da implementacao

O campo `Status` foi mantido para compatibilidade e leitura geral.

Para status real, a referencia principal e `StatusImplementacao`, complementada por `StatusTecnico` e checklist.

Campos da secao:
- StatusImplementacao
- StatusTecnico
- PercentualImplementacao
- PendenciasTecnicas
- PendenciasHomologacao
- EvidenciaImplementacao
- DataConclusaoTecnica
- DataHomologacao
- CriterioAceite
- ProximaAcao

## Campo Objetivo

`Objetivo` explica a finalidade do item no sistema e responde: "qual problema ou necessidade este item resolve?".

Exemplos registrados:
- Abertura de chamado pelo portal: permitir abertura pelo solicitante autenticado com titulo, descricao, categoria, prioridade, anexos opcionais e acompanhamento posterior.
- Abertura por e-mail: processar e-mails via Worker IMAP para abrir chamados, correlacionar respostas, tratar anexos permitidos e registrar logs tecnicos.
- Perfis de acesso: controlar o acesso por perfis e permissoes granulares sem necessidade de alteracao de codigo.

### Significado de "Implementado funcionalmente"

`Implementado funcionalmente` indica entrega tecnica da funcionalidade, sem implicar automaticamente homologacao final ou producao.

### Pendencias evolutivas

Quando `StatusTecnico = Completo com pendencias evolutivas`, registrar obrigatoriamente:
- o que foi concluido;
- o que falta para homologacao/producao;
- proxima acao priorizada.

## Percentual por checklist

O percentual deixou de depender de digitacao manual quando ha checklist ativo.

Regra de calculo:
- `PercentualImplementacao = itens ativos concluidos / itens ativos * 100`

Comportamento:
- se existir checklist ativo, a UI mostra percentual calculado e bloqueia edicao manual;
- se nao existir checklist ativo, o valor legado pode ser usado como fallback.

## Checklist da implementacao

Cada item de roadmap pode ter varios itens em `RoadmapChecklistItem`.

Campos principais:
- Titulo
- Descricao
- Grupo
- Ordem
- Concluido
- Obrigatorio
- Ativo

Grupos sugeridos:
- Planejamento
- Desenvolvimento
- Testes
- Documentacao
- Homologacao
- Producao

## CRUD de futuras implementacoes

Cada item de roadmap pode ter N evolucoes em `RoadmapImplementacaoFutura`.

Campos:
- Titulo
- Descricao
- Tipo
- Prioridade
- Status
- Responsavel
- PrazoAlvo
- DataConclusao
- Observacao
- Ativo

Regras:
- vinculo obrigatorio ao item de roadmap;
- inativacao logica;
- concluir/inativar/reativar;
- filtros por status, tipo, prioridade, responsavel e ativo.

## Labels amigaveis na UI

A interface nao deve exibir enums crus.

Exemplos esperados:
- `EmValidacao` -> `Em validacao`
- `NaoIniciado` -> `Nao iniciado`
- `NaoAvaliado` -> `Nao avaliado`
- `CompletoComPendenciasEvolutivas` -> `Completo com pendencias evolutivas`

Aplicacao:
- `QSelect` mostra label amigavel e salva valor tecnico;
- `QTable` e `QBadge` mostram label amigavel;
- contratos de API preservam valor tecnico para integracao.

## Exemplo - Perfis de acesso

Preenchimento recomendado:
- Categoria: `Seguranca` (via `RoadmapCategoriaId`)
- Status (legado): `Implementado`
- StatusImplementacao: `Implementado funcionalmente`
- StatusTecnico: `Completo com pendencias evolutivas`
- Checklist ativo: 10 itens, 9 concluidos e 1 pendente
- Percentual calculado: `90%`
- PendenciasTecnicas: auditoria detalhada de alteracoes, testes frontend/e2e e validacao fina em homologacao
- PendenciasHomologacao: validacao com usuarios reais (Administrador, Atendente, Solicitante)
- EvidenciaImplementacao: `docs/SEGURANCA-PERFIS-PERMISSOES.md`, `docs/ROADMAP.md`, testes backend e matriz frontend
- CriterioAceite: admin gerencia permissoes; atendente ve acoes permitidas; solicitante nao acessa admin; backend bloqueia sem permissao
- ProximaAcao: executar homologacao real e priorizar auditoria detalhada

Futuras implementacoes sugeridas:
- Auditoria detalhada de alteracoes de permissoes
- Testes frontend/e2e da matriz de permissoes
- Validacao com usuarios reais
- Relatorio de permissoes por perfil
- Exportacao da matriz de permissoes

## Observacao de permissao

Nesta iteracao, endpoints de categoria/checklist reutilizam `Roadmap.Visualizar` e `Roadmap.Gerenciar`.

Pendencia real para evolucao futura:
- avaliar criacao de permissao granular dedicada para categorias/checklist (`RoadmapCategorias.*`, `RoadmapChecklist.*`).

## Sprint Portal 3 - Abertura de chamado pelo portal

Area: Abertura de chamado pelo portal
Categoria: Portal

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Proxima acao: Homologar fluxo com usuario real.

Checklist de entrega tecnica:
- [x] Endpoint de contexto do portal validado
- [x] Endpoint de criacao de chamado validado
- [x] Validacoes obrigatorias implementadas
- [x] Solicitante obtido pelo usuario autenticado
- [x] Status inicial Aberto aplicado
- [x] Historico inicial criado
- [x] Tela /portal/chamados/novo implementada
- [x] Upload de anexo validado
- [x] Redirecionamento para detalhe validado
- [x] Chamado listado no portal
- [x] Chamado visivel no admin
- [x] Detalhe do portal validado
- [x] Historico inicial visivel
- [x] Testes backend criados/atualizados
- [x] Build frontend validado
- [ ] Homologacao manual com usuario real

Pendencias evolutivas:
- homologacao manual com usuario real
- testes E2E frontend do fluxo portal->admin
- validacao de anexos em ambiente real (tipos e limites com arquivos reais)

## Sprint Portal 4 - Fechamento do item Abertura de chamado pelo portal

Item: Abertura de chamado pelo portal
Categoria: Portal

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Interpretacao:
- Implementado funcionalmente significa entrega tecnica concluida e validada por inspecao, build e testes.
- Nao significa homologado com usuario real nem em producao.

Criterio de aceite registrado:
Solicitante autenticado consegue abrir chamado pelo portal com dados obrigatorios, visualizar retorno de sucesso e acessar o detalhe do chamado criado. Backend registra status inicial, historico e vinculo com solicitante. Quando anexos estao disponiveis, arquivos permitidos podem ser enviados e visualizados no detalhe.

Proxima acao:
Validar com usuario real em homologacao.

Pendencias tecnicas registradas:
- homologacao manual com usuario real
- testes E2E frontend
- validacao de anexos em ambiente real

## Sprint Integracoes E-mail 2 - E-mail novo cria chamado

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Em avaliacao continua

Checklist entregue nesta sprint:
- [x] E-mail novo cria chamado
- [x] Origem E-mail aplicada ao chamado
- [x] Status inicial Aberto aplicado
- [x] Historico inicial criado
- [x] Prevencao de duplicidade por MessageId implementada
- [x] Configuracoes de categoria/prioridade padrao definidas
- [x] Testes unitarios de processamento criados/atualizados

Pendencias mantidas para proximas sprints:
- [ ] Correlacao de respostas (regras finais)
- [ ] Anexos por e-mail (escopo completo)
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais

## Sprint Integracoes E-mail 3 - Correlacao de respostas e anexos

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Completo com pendencias evolutivas

Checklist entregue nesta sprint:
- [x] Correlacao por codigo do chamado implementada
- [x] Correlacao por Message-Id/In-Reply-To/References implementada
- [x] Resposta por e-mail adiciona comentario publico
- [x] Anexos por e-mail validados
- [x] Anexos permitidos sao salvos
- [x] Anexos invalidos sao rejeitados e logados
- [x] Testes de correlacao criados/atualizados
- [x] Testes de anexos criados/atualizados

Pendencias mantidas:
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais
- [ ] Validacao com anexos reais
- [ ] OAuth para caixa Microsoft (se exigido)
- [ ] Retry/backoff
- [ ] Dead-letter
- [ ] Monitoramento do Worker
- [ ] Reprocessamento manual de e-mails com erro
- [ ] Sanitizacao avancada de HTML
- [ ] Antivirus/varredura de anexos
- [ ] Teste E2E com IMAP real

## Sprint Integracoes E-mail 4 - Logs administrativos e tela

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Completo com pendencias evolutivas

Checklist entregue nesta sprint:
- [x] Endpoint de logs administrativos implementado
- [x] Tela `/admin/integracoes/email` validada
- [x] Filtros de logs implementados
- [x] Detalhe de log em dialog implementado
- [x] Solicitante bloqueado nos logs administrativos
- [x] Build frontend validado

Pendencias mantidas:
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais
- [ ] Validacao com anexos reais
- [ ] Retry/backoff
- [ ] Dead-letter
- [ ] Reprocessamento manual
- [ ] Monitoramento/health check
- [ ] OAuth Microsoft (se exigido)
- [ ] Antivirus/varredura de anexos
- [ ] Teste E2E com IMAP real

## Sprint Integracoes E-mail 5 - Fechamento tecnico, checklist e homologacao

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Criterio de aceite consolidado:
E-mail recebido na caixa configurada e processado pelo Worker, criando chamado com origem E-mail, status inicial, historico e vinculo com remetente. Respostas correlacionadas adicionam comentario ao chamado existente. Anexos permitidos sao tratados conforme regras de seguranca. Logs tecnicos ficam disponiveis na area administrativa.

Proxima acao:
Validar com caixa IMAP real em homologacao.

Pendencias tecnicas registradas:
- validacao com caixa IMAP real
- homologacao com e-mails reais
- validacao com anexos reais
- OAuth Microsoft (se exigido)
- retry/backoff
- dead-letter
- monitoramento do Worker
- reprocessamento manual
- sanitizacao avancada de HTML
- antivirus/varredura de anexos
- teste E2E com IMAP real

Evidencias de implementacao:
- Worker.Email
- EmailWorkerOptions
- LogIntegracaoEmail
- ProcessarEmailRecebidoUseCase
- EmailParaChamadoService
- correlacao de respostas
- tratamento de anexos
- endpoints administrativos de logs
- tela `/admin/integracoes/email`
- testes automatizados
- `docs/INTEGRACAO-EMAIL.md`

Checklist tecnico (vinculado ao item):
- [x] 1. Projeto Worker.Email validado/criado
- [x] 2. Configuracoes IMAP definidas
- [x] 3. Leitura IMAP implementada
- [x] 4. Processamento em lote implementado
- [x] 5. LogIntegracaoEmail implementado
- [x] 6. Prevencao de duplicidade por MessageId implementada
- [x] 7. E-mail novo cria chamado
- [x] 8. Origem E-mail aplicada ao chamado
- [x] 9. Status inicial Aberto aplicado
- [x] 10. Historico inicial criado
- [x] 11. Correlacao por codigo do chamado implementada
- [x] 12. Correlacao por Message-Id/In-Reply-To implementada
- [x] 13. Resposta por e-mail adiciona comentario
- [x] 14. Anexos por e-mail validados
- [x] 15. Anexos permitidos sao salvos
- [x] 16. Anexos invalidos sao rejeitados e logados
- [x] 17. Endpoint de logs administrativos implementado
- [x] 18. Tela /admin/integracoes/email validada
- [x] 19. Filtros de logs implementados
- [x] 20. Detalhe de log em dialog implementado
- [x] 21. Testes unitarios de processamento criados
- [x] 22. Testes de correlacao criados
- [x] 23. Testes de anexos criados
- [x] 24. Build backend validado
- [x] 25. Testes backend executados
- [x] 26. Build Worker validado
- [x] 27. Build frontend validado

Checklist de evolucao/homologacao (pendente):
- [ ] 28. Validacao com caixa IMAP real
- [ ] 29. Homologacao com e-mails reais
- [ ] 30. Validacao com anexos reais
- [ ] 31. Autenticacao OAuth para caixa Microsoft, se exigido
- [ ] 32. Retry/backoff em falhas temporarias
- [ ] 33. Dead-letter ou fila de mensagens com erro
- [ ] 34. Monitoramento/health check do Worker
- [ ] 35. Painel de reprocessamento manual de e-mails com erro
- [ ] 36. Sanitizacao avancada de HTML
- [ ] 37. Antivirus/varredura de anexos
- [ ] 38. Teste E2E com IMAP real
- [ ] 39. Metricas operacionais do Worker
- [ ] 40. Alertas de falha recorrente no processamento de e-mail

Observacao de percentual:
- percentual do item deve ser calculado automaticamente pelo checklist ativo;
- nao preencher percentual manual quando checklist estiver ativo.

## Sprint Autenticação 1 - Revisão da base e desenho final do fluxo Entra ID

Área: Autenticação corporativa  
Categoria: Segurança

Status anterior:
- Status atual: Não iniciado
- Status técnico: Não avaliado

Status após a Sprint Autenticação 1:
- Status da implementação: Em desenvolvimento
- Status técnico: Completo com pendências evolutivas

Decisão arquitetural consolidada:
- Microsoft Entra ID (Azure AD) autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissões.

Escopo revisado nesta sprint:
- [x] Revisão do login Microsoft no frontend (`LoginView`, `authService`, `AuthStore`)
- [x] Revisão da validação JWT/API (`ServiceCollectionExtensions`)
- [x] Revisão de `GET /api/me` (`MeController`, `UsuarioAtualService`)
- [x] Revisão de `httpClient` e tratamento de `401/403`
- [x] Revisão de router guards (`router.beforeEach`)
- [x] Revisão do login local Development
- [x] Revisão da emulação de perfis em Development
- [x] Consolidação da documentação técnica da autenticação corporativa

Fluxo oficial definido:
1. Usuário acessa o frontend.
2. Usuário clica em `Entrar com Microsoft Entra ID`.
3. Usuário autentica no Microsoft Entra ID.
4. Frontend recebe `access token`.
5. API valida o token JWT.
6. SGX identifica o usuário interno.
7. SGX cria usuário interno quando aplicável.
8. SGX retorna `GET /api/me` com perfis e permissões efetivas.
9. Frontend redireciona conforme perfil/permissão.

Pendências reais para Sprint Autenticação 2:
- [ ] Configurar App Registration definitivo (SPA e API) no tenant institucional.
- [ ] Validar escopo real da API no frontend (`VITE_AZURE_API_SCOPE`).
- [ ] Homologar fluxo real com usuários corporativos (Administrador, Atendente e Solicitante).
- [ ] Definir regra formal para provisionamento e bloqueio de usuário interno conforme ciclo de vida no Entra ID.
- [ ] Registrar evidências formais de homologação para promoção a produção.

Evidências de implementação/documentação:
- `docs/AUTENTICACAO-CORPORATIVA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`

## Sprint Autenticação 2 - Backend Microsoft Entra ID, JWT e usuário interno

Área: Autenticação corporativa  
Categoria: Segurança

Status após a Sprint Autenticação 2:
- Status da implementação: Em desenvolvimento
- Status técnico: Completo com pendências evolutivas

Decisão arquitetural mantida:
- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissões.

Escopo entregue nesta sprint:
- [x] Revisão e reforço da validação JWT (`Authority`, `Issuer`, `Audience`, expiração e assinatura).
- [x] Suporte a `MetadataAddress` opcional em `AzureAdOptions`.
- [x] Fortalecimento das opções de autenticação (`DominiosPermitidos`, `CriarUsuarioAutomaticamente`, `PerfilPadraoUsuarioMicrosoft`).
- [x] Mapeamento de claims Microsoft com fallback definido (`preferred_username`, `email`, `upn`, `unique_name`).
- [x] Regras de bloqueio por domínio não permitido.
- [x] Regras de bloqueio de usuário interno inativo.
- [x] Criação automática de usuário interno com perfil padrão quando permitido.
- [x] Preservação do login local Development e emulação de perfis.
- [x] Preservação do contrato de `GET /api/me` com `autenticadoPor=MicrosoftEntraId` no fluxo Microsoft.
- [x] Testes automatizados de unidade e integração atualizados.

Regras de segurança validadas:
- [x] Perfis e permissões continuam internos no SGX.
- [x] `roles` e `groups` do Azure AD não concedem perfil administrativo automaticamente.
- [x] Login local não é habilitado fora de Development.

Pendências reais para Sprint Autenticação 3:
- [ ] Homologação ponta a ponta com tenant institucional real (Microsoft Entra ID).
- [ ] Validação operacional em ambiente de homologação com usuários reais.
- [ ] Definição final de governança de ciclo de vida de usuário interno (bloqueio, reativação e auditoria).
- [ ] Avaliação de persistência opcional de identificadores corporativos (`oid`/`tid`) sem impacto em migrações indevidas.

Evidências de implementação:
- `src/SGX.SistemaChamado.Api/Services/UsuarioAtualService.cs`
- `src/SGX.SistemaChamado.Api/Extensions/ServiceCollectionExtensions.cs`
- `src/SGX.SistemaChamado.Api/Options/AuthOptions.cs`
- `src/SGX.SistemaChamado.Api/Options/AzureAdOptions.cs`
- `src/SGX.SistemaChamado.Api/Options/AzureAdOptionsValidator.cs`
- `tests/SGX.SistemaChamado.Tests/UsuarioAtualServiceTests.cs`
- `tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs`
- `tests/SGX.SistemaChamado.Tests/AzureAdOptionsValidatorTests.cs`

## Sprint Autenticação 3 - Frontend de login Microsoft e restauração de sessão

Área: Autenticação corporativa  
Categoria: Segurança

Status após a Sprint Autenticação 3:
- Status da implementação: Em desenvolvimento
- Status técnico: Completo com pendências evolutivas

Escopo entregue nesta sprint:
- [x] Consolidação do login Microsoft no frontend (`LoginView`, `authService`, `authStore`).
- [x] Ajuste de mensagens de erro e cancelamento amigável no popup Microsoft.
- [x] Reforço de restauração de sessão com single-flight em `inicializarSessao`.
- [x] Manutenção de `GET /api/me` como fonte de perfis e permissões.
- [x] Preservação dos guards de `/admin`, `/portal`, `/acesso-negado` e `/login`.
- [x] Preservação de login local e emulação apenas em Development.
- [x] Bloqueio explícito de ações concorrentes no login (duplo clique).
- [x] Alinhamento de tipagem do frontend para `autenticadoPor=MicrosoftEntraId`.

Pendências reais para Sprint Autenticação 4:
- [ ] Validar login Microsoft com tenant institucional real e evidências formais.
- [ ] Validar cenários corporativos de MFA/Conditional Access em homologação.
- [ ] Executar rodada completa de validação manual de UX de sessão em ambiente interativo.

Evidências de implementação:
- `src/SGX.SistemaChamado.Web/src/views/LoginView.vue`
- `src/SGX.SistemaChamado.Web/src/services/authService.ts`
- `src/SGX.SistemaChamado.Web/src/stores/authStore.ts`
- `src/SGX.SistemaChamado.Web/src/types/auth.ts`
- `docs/AUTENTICACAO-CORPORATIVA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`

## Sprint Autenticação 4 - Configuração Microsoft Entra ID e homologação técnica

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Em desenvolvimento  
Status técnico: Completo com pendências evolutivas

Checklist entregue nesta sprint:
- [x] App Registration documentado
- [x] Redirect URI documentado
- [x] Logout URI documentado
- [x] Escopo de API documentado
- [x] Variáveis backend documentadas
- [x] Variáveis frontend documentadas
- [x] Segurança MFA/Conditional Access documentada
- [x] Checklist de homologação criado

Pendências mantidas:
- [ ] Configurar tenant institucional real
- [ ] Executar homologação com usuário corporativo real
- [ ] Validar MFA
- [ ] Validar Conditional Access
- [ ] Validar ambiente publicado/VPS
- [ ] Registrar evidências formais de homologação

## Sprint Autenticação 5 - Fechamento do item Autenticação corporativa

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Objetivo:
Permitir que usuários acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autorização interna no SGX por usuários, perfis e permissões. O Azure autentica a identidade; o SGX controla o que cada usuário pode acessar e executar dentro do sistema.

Situação atual:
Fluxo de autenticação corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com validação de token JWT, modo Single Tenant, controle de domínio permitido, integração com `GET /api/me`, criação/identificação de usuário interno e autorização por perfis/permissões do SGX. Ainda depende de homologação com tenant institucional real.

Atenção técnica:
Microsoft Entra ID/Azure AD autentica e o SGX autoriza. Roles/groups do Azure não concedem perfil administrativo automaticamente no SGX. Perfis e permissões continuam internos ao SGX.

Checklist:
- 19 itens técnicos concluídos
- 8 itens pendentes de homologação/governança
- percentual esperado aproximado: 70%

Pendências técnicas:
- homologar com tenant institucional real do Microsoft Entra ID;
- validar login com usuários corporativos reais;
- validar MFA e Conditional Access;
- validar logout corporativo;
- validar ambiente publicado/VPS;
- revisar configuração com a equipe responsável pelo Azure;
- registrar evidências formais de homologação;
- avaliar persistência opcional de `oid/tid`;
- definir governança de ciclo de vida do usuário interno.

Pendências de homologação:
- executar homologação ponta a ponta com usuários reais de perfil Administrador, Atendente e Solicitante;
- validar comportamento com usuário interno inativo;
- validar bloqueio de domínio/tenant não permitido;
- validar mensagens de erro de login;
- validar redirecionamento por perfil/permissão após login;
- registrar evidências com prints, data, ambiente e usuário de teste.

Mensagem para reunião:
A autenticação corporativa do SGX está desenhada para usar Microsoft Entra ID/Azure AD como identidade principal, enquanto o SGX mantém a autorização interna por perfis e permissões. Essa abordagem permite MFA, Conditional Access, acesso fora da rede e melhor governança sem transferir regras internas do sistema para o Azure.

## Sprint Autenticação 7 - Administrador inicial seguro

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Objetivo:
Permitir a criação segura do primeiro Administrador em produção por variáveis de ambiente explícitas, sem senha fixa e sem dependência do modo Development.

Checklist:
- [x] Variáveis de ambiente definidas (`SGX_ADMIN_INICIAL_EMAIL`, `SGX_ADMIN_INICIAL_SENHA`, `SGX_ADMIN_INICIAL_NOME`)
- [x] Validação de e-mail implementada
- [x] Validação de senha forte implementada
- [x] Senha hasheada
- [x] Perfil Administrador associado
- [x] Não cria duplicidade se já existe Administrador ativo
- [x] Documentação atualizada
- [x] Testes criados/atualizados
- [ ] Homologação em ambiente real pendente

Pendências evolutivas:
- processo operacional de rotação de credencial de bootstrap;
- validação formal em homologação/produção;
- auditoria operacional contínua de eventos de criação inicial.

## Sprint Autenticação 8 - Recuperação de senha e hardening do login local SGX

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Objetivo:
Permitir recuperação de senha local SGX, troca obrigatória e hardening de login para produção, sem senha em texto puro, sem enumeração de usuário e com lockout configurável.

Checklist:
- [x] troca de senha autenticada
- [x] troca obrigatória
- [x] recuperação de senha
- [x] token temporário
- [x] token de uso único
- [x] token com expiração
- [x] política de senha
- [x] lockout
- [x] último login
- [x] frontend `/alterar-senha`
- [x] frontend `/recuperar-senha`
- [x] documentação
- [x] testes
- [ ] homologação real pendente

Pendências evolutivas:
- envio transacional real de e-mail para recuperação;
- auditoria dedicada persistida em banco para eventos de autenticação local;
- validação formal em ambiente publicado com evidências de lockout e recuperação.

## Sprint Autenticação 9 - Tenant único, contas permitidas e homologação real Microsoft Entra ID

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Checklist técnico:
- [x] Single Tenant documentado
- [x] TenantId validado
- [x] issuer validado
- [x] tid validado
- [x] audience validada
- [x] contas pessoais Microsoft bloqueadas
- [x] tenants externos bloqueados
- [x] domínio permitido validado quando configurado
- [x] roles/groups Azure não concedem admin
- [x] mensagens frontend amigáveis
- [x] testes automatizados criados/ajustados
- [x] documentação atualizada

Pendências:
- homologação com tenant real;
- teste com usuário externo real;
- teste com conta pessoal Microsoft real;
- MFA;
- Conditional Access;
- logout corporativo;
- evidências formais.

## Correções - Integração Microsoft, usuários demo e senha por Administrador

Área: Autenticação corporativa  
Categoria: Segurança

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Checklist técnico:
- [x] Menu `Integrações` exibe `Microsoft Entra ID`
- [x] Tela `/admin/integracoes/microsoft-entra-id` criada
- [x] Endpoints administrativos de configuração Microsoft criados (`GET/PUT`)
- [x] LoginView consome provedores com fallback amigável
- [x] Seed Development mantém 2 usuários demonstrativos por perfil
- [x] Redefinição de senha por Administrador implementada
- [x] Permissões novas criadas (`IntegracoesMicrosoft.*`, `Usuarios.RedefinirSenha`)
- [x] Testes automatizados atualizados

Pendências evolutivas:
- homologação funcional em banco PostgreSQL real com dados legados;
- governança de limpeza administrativa para bases antigas com usuários demo excedentes;
- revisão de UX para edição de configurações Microsoft em ambiente distribuído (quando exigir restart).
