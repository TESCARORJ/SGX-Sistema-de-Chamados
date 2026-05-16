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

## Gestao ITSM e Documentacao

Area: Gestao ITSM e Documentacao
Categoria: Governanca

Objetivo:
Centralizar no painel administrativo a consulta ao Roadmap ITSM e a documentacao funcional/tecnica do SGX Sistema de Chamados, facilitando apresentacao, governanca, homologacao e acompanhamento da evolucao do sistema.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Acesso administrativo:
- `Admin > Gestao ITSM > Roadmap`
- `Admin > Gestao ITSM > Documentacao`

Rotas:
- `/admin/gestao-itsm/roadmap`
- `/admin/gestao-itsm/documentacao`
- `/admin/roadmap-itsm` mantida por compatibilidade.

Checklist:
- [x] Grupo Gestao ITSM criado no menu administrativo.
- [x] Roadmap movido ou espelhado para Gestao ITSM.
- [x] Tela de Documentacao ITSM criada.
- [x] Documentos iniciais adicionados.
- [x] Busca de documentos criada.
- [x] Filtro por categoria criado.
- [x] Link entre Roadmap e Documentacao criado.
- [x] Permissoes integradas.
- [x] Documentacao do repositorio atualizada.
- [x] Testes ou validacao tecnica criados.

Pendencias evolutivas:
- Permitir edicao da documentacao pelo proprio sistema.
- Versionar documentacao por release.
- Anexar evidencias de homologacao.
- Exportar documentacao em PDF.
- Vincular documentos diretamente aos itens do roadmap.

## Sprint Historico/Auditoria 1 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Criar trilha de auditoria para registrar acoes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanca, analise de alteracoes e apoio a homologacao.

Situacao atual:
Modulo de auditoria iniciado com estrutura central de eventos auditaveis, service de registro, tabela propria e primeiros eventos do sistema.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual (checklist consolidado Sprints 1-3): 100% (63 de 63 itens)

Checklist Sprint 1:
- [x] Entidade EventoAuditoria criada.
- [x] Enum de acao de auditoria criado.
- [x] Enum de nivel de auditoria criado.
- [x] Migration da tabela eventos_auditoria criada.
- [x] Indices de consulta criados.
- [x] Service centralizado de auditoria criado.
- [x] Context provider de auditoria criado.
- [x] Captura de usuario atual integrada.
- [x] Captura de IP e User-Agent integrada.
- [x] Registro de login integrado.
- [x] Registro de logout avaliado e documentado como nao aplicavel enquanto nao houver fluxo backend controlado.
- [x] Registro de criacao/edicao/inativacao de usuario integrado.
- [x] Registro de perfis/permissoes integrado.
- [x] DTOs de auditoria criados.
- [x] Testes automatizados criados.
- [x] Documentacao atualizada em Gestao ITSM.

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

## Sprint SLA 1 - Modelagem e cadastro administrativo

Área: SLA  
Categoria: SLA

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Resumo:
- cadastro administrativo de políticas de SLA implementado;
- metas de SLA por prioridade implementadas em estrutura própria;
- política padrão de SLA semeada com metas para Baixa, Média, Alta e Crítica;
- checklist Sprint 1 do item SLA criado e vinculado no roadmap;
- percentual do item passa a ser calculado pelo checklist ativo.

Limitação conhecida:
- nesta sprint, a aplicação automática integral da política no fluxo dos chamados fica para Sprint 2.

## Sprint SLA 2 - Aplicação prática nos chamados

Área: SLA  
Categoria: SLA

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Resumo:
- registro próprio de SLA aplicado ao chamado criado em `chamado_slas`;
- política ativa escolhida por compatibilidade de categoria/departamento, ordem e meta ativa por prioridade;
- SLA aplicado na criação do chamado sem impedir abertura quando não há política/meta aplicável;
- primeira resposta registrada por comentário público de atendente ou status `Em atendimento`;
- resolução registrada em status final, `Resolvido` ou `Encerrado`;
- pausa implementada quando o status entra em `AguardandoSolicitante`, respeitando a política aplicada;
- detalhe e listagem de chamados retornam resumo de SLA;
- listagem administrativa possui filtros por situação do SLA;
- documentação técnica atualizada em `docs/SLA.md`.

Checklist Sprint 2:
- [x] Tabela de SLA aplicado ao chamado criada.
- [x] Relacionamento entre chamado e SLA criado.
- [x] Service de cálculo de SLA criado.
- [x] Política aplicável identificada por prioridade/categoria/departamento.
- [x] SLA aplicado na criação do chamado.
- [x] Prazo de primeira resposta calculado.
- [x] Prazo de resolução calculado.
- [x] Primeira resposta registrada.
- [x] Resolução registrada.
- [x] Pausa de SLA preparada ou implementada.
- [x] Situação atual do SLA calculada.
- [x] SLA exibido no detalhe do chamado.
- [x] SLA exibido na listagem administrativa.
- [x] Filtros administrativos de SLA criados.
- [x] DTOs de chamado atualizados com resumo de SLA.
- [x] Testes automatizados criados.
- [x] Documentação atualizada.

Pendências técnicas:
- calendário corporativo para `UsarHorarioComercial=true`;
- homologação com base PostgreSQL real;
- relatórios históricos e indicadores avançados de SLA.

## Sprint SLA 3 - Alertas, vencimentos e painel de SLA

Área: SLA  
Categoria: SLA

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Resumo:
- configuração padrão de alertas criada e exposta em `Admin > SLA > Alertas`;
- eventos de SLA persistidos em `eventos_sla` com chave de idempotência;
- monitoramento periódico configurável por `SlaMonitoring`;
- alertas de primeira resposta/resolução próximos do vencimento e vencidos registrados como eventos;
- painel gerencial em `Admin > SLA > Painel`;
- histórico de SLA exibido no detalhe administrativo do chamado;
- consulta estruturada para relatório futuro.

Checklist Sprint 3:
- [x] Configuração de alerta de SLA criada.
- [x] Tela administrativa de configuração de alerta criada.
- [x] Endpoints de configuração de alerta criados.
- [x] Job de verificação de SLA criado.
- [x] Periodicidade configurável por appsettings criada.
- [x] Controle contra notificações/eventos duplicados criado.
- [x] Histórico de eventos de SLA criado.
- [x] Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolução, pausa e retomada.
- [x] Painel de indicadores de SLA criado.
- [x] Indicador de SLA vencido criado.
- [x] Indicador de SLA próximo do vencimento criado.
- [x] Indicador de percentual de cumprimento criado.
- [x] Métrica de tempo médio de primeira resposta criada.
- [x] Métrica de tempo médio de resolução criada.
- [x] Indicadores por prioridade criados.
- [x] Indicadores por categoria criados.
- [x] Indicadores por departamento criados.
- [x] Histórico de SLA exibido no detalhe administrativo do chamado.
- [x] Estrutura preparada para exportação futura.
- [x] Documentação atualizada.
- [x] Testes automatizados criados.

Pendências técnicas:
- envio real de notificações por canal oficial;
- exportação Excel/PDF;
- evidências de homologação em ambiente publicado;
- calendário por departamento/time;
- importação automática de feriados.

## Sprint SLA 4 - Calendário corporativo e horário comercial

Área: SLA  
Categoria: SLA

Status da implementação: Implementado funcionalmente  
Status técnico: Completo com pendências evolutivas

Entregas:

- estrutura de calendário corporativo criada com expediente semanal e exceções;
- seed do calendário corporativo padrão, ativo, em `America/Sao_Paulo`, segunda a sexta das 09:00 às 18:00;
- política de SLA vinculável a calendário corporativo;
- cálculo de SLA em minutos corridos ou minutos úteis conforme configuração da política;
- endpoints administrativos para calendários, horários e exceções;
- tela `Admin > SLA > Calendários`;
- tela de políticas com seleção de calendário quando horário comercial está ativo;
- detalhe administrativo do chamado mostra tipo de cálculo e calendário utilizado;
- testes automatizados para calendário, cálculo útil e integração com SLA.

Checklist Sprint 4:

- [x] Entidade CalendarioCorporativo criada.
- [x] Entidade HorarioAtendimentoCalendario criada.
- [x] Entidade ExcecaoCalendarioCorporativo criada.
- [x] Migrations de calendário criadas.
- [x] Seed do calendário padrão criado.
- [x] Relacionamento entre Política SLA e Calendário criado.
- [x] Service administrativo de calendário criado.
- [x] Service de cálculo de tempo útil criado.
- [x] Cálculo de prazo de primeira resposta usando horário comercial implementado.
- [x] Cálculo de prazo de resolução usando horário comercial implementado.
- [x] Cálculo de minutos úteis de primeira resposta implementado.
- [x] Cálculo de minutos úteis de resolução implementado.
- [x] Endpoints administrativos de calendário criados.
- [x] Tela Admin > SLA > Calendários criada.
- [x] Tela de política SLA atualizada com seleção de calendário.
- [x] Detalhe do chamado mostra tipo de cálculo e calendário usado.
- [x] Testes automatizados criados.
- [x] Documentação atualizada.

Pendências:

- calendário por departamento/time;
- importação automática de feriados;
- exceções recorrentes;
- regra avançada de prazo remanescente em reabertura.

## Sprint Historico/Auditoria 2 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Registrar acoes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanca, analise de alteracoes, auditoria operacional e apoio a homologacao.

Situacao atual:
Base tecnica de auditoria criada na Sprint 1. Sprint 2 aplica auditoria aos modulos criticos do sistema, incluindo chamados, usuarios, perfis/permissoes, SLA, autenticacao corporativa e roadmap ITSM.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist.

Checklist Sprint 2:
- [x] Helper de diff antes/depois criado.
- [x] Mascaramento de dados sensiveis implementado.
- [x] Auditoria de abertura de chamado implementada.
- [x] Auditoria de alteracao de status implementada.
- [x] Auditoria de alteracao de prioridade implementada.
- [x] Auditoria de alteracao de categoria implementada.
- [x] Auditoria de atribuicao de responsavel implementada.
- [x] Auditoria de assumir chamado implementada.
- [x] Auditoria de comentarios administrativos implementada.
- [x] Auditoria de encerramento/resolucao implementada.
- [x] Auditoria de reabertura implementada.
- [x] Auditoria de anexos preparada ou implementada.
- [x] Auditoria de usuarios revisada e complementada.
- [x] Auditoria de perfis revisada e complementada.
- [x] Auditoria de permissoes revisada e complementada.
- [x] Auditoria de politicas de SLA implementada.
- [x] Auditoria de metas de SLA implementada.
- [x] Auditoria de calendarios de SLA implementada.
- [x] Auditoria de horarios de calendario implementada.
- [x] Auditoria de excecoes de calendario implementada.
- [x] Auditoria de alertas de SLA implementada.
- [x] Auditoria de autenticacao corporativa implementada.
- [x] Auditoria de Roadmap ITSM implementada.
- [x] Auditoria de documentacao ITSM preparada conforme estrutura atual.
- [x] Testes automatizados de auditoria dos modulos criticos criados.
- [x] Documentacao atualizada em Gestao ITSM.
- [x] Validacao no banco com eventos reais em eventos_auditoria preparada/executada.

Observacao:
- leitura da documentacao ITSM nao e auditada na Sprint 2 por ser conteudo estatico;
- edicao/publicacao de documentacao ainda nao existe no sistema e fica para evolucao futura.

## Sprint Historico/Auditoria 3 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Permitir que administradores e gestores consultem eventos de auditoria no painel administrativo, com filtros avancados, paginacao, detalhe e indicadores.

Situacao atual:
Base tecnica da Sprint 1 e auditoria em modulos criticos da Sprint 2 evoluiram para consulta administrativa funcional na Sprint 3.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist (63/63 = 100%).

Checklist Sprint 3:
- [x] Endpoints administrativos de auditoria criados.
- [x] Use cases/services de consulta de auditoria criados.
- [x] Filtros de auditoria criados.
- [x] Paginacao de eventos criada.
- [x] Endpoint de detalhe de evento criado.
- [x] Endpoint de dashboard de auditoria criado.
- [x] Permissoes de auditoria criadas ou integradas.
- [x] Menu Governanca > Auditoria criado.
- [x] Rota /admin/governanca/auditoria criada.
- [x] Tela administrativa de auditoria criada.
- [x] Modal/drawer de detalhe criado.
- [x] Visualizacao de dados antes/depois criada.
- [x] Indicadores basicos de auditoria criados.
- [x] Service frontend de auditoria criado.
- [x] Tipos frontend de auditoria criados.
- [x] Link entre Auditoria e Gestao ITSM criado.
- [x] Documentacao em Gestao ITSM atualizada.
- [x] Testes automatizados backend criados.
- [x] Build frontend validado.
- [x] Validacao com eventos reais em eventos_auditoria executada.

Pendencias evolutivas:
- Exportacao Excel/PDF.
- Retencao configuravel de auditoria.
- Assinatura/hash da trilha de auditoria.
- Alertas para eventos criticos.
- Painel avancado de seguranca.
- Integracao com SIEM/Log Analytics.
- Politica de anonimizaçao/LGPD para eventos antigos.

## Sprint Cadastros Administrativos 1 - Base tecnica

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Criar a fundacao tecnica dos cadastros administrativos para sustentar a evolucao do SGX em chamados, SLA, dashboards, relatorios e governanca operacional.

Entregas tecnicas:
- entidades de dominio criadas/evoluidas para `Departamento`, `CategoriaChamado`, `SubcategoriaChamado`, `PrioridadeChamado`, `TipoSolicitacao` e `LocalUnidade`;
- relacionamento `CategoriaChamado 1:N SubcategoriaChamado` implementado;
- `DbSet` adicionados no `SGXSistemaChamadoDbContext`;
- mapeamentos Fluent API criados para novos cadastros;
- tabela `prioridades_chamado` evoluida com `peso` e `cor`;
- migration `AddCadastrosAdministrativosSprint1` criada e aplicada no banco PostgreSQL;
- documentacao publicada em `docs/CADASTROS-ADMINISTRATIVOS.md`.

Checklist Sprint 1:
- [x] Entidade Departamento validada
- [x] Entidade CategoriaChamado validada
- [x] Entidade SubcategoriaChamado criada
- [x] Entidade PrioridadeChamado evoluida com Peso e Cor
- [x] Entidade TipoSolicitacao criada
- [x] Entidade LocalUnidade criada com Endereco
- [x] DbSet adicionados no DbContext
- [x] Fluent API criada/ajustada
- [x] Relacionamento categoria x subcategoria criado
- [x] Migration criada
- [x] Banco atualizado
- [x] Documentacao inicial criada
- [x] Roadmaps atualizados

Pendencias evolutivas:
- disponibilizar CRUD administrativo de tipos de solicitacao e locais/unidades;
- conectar novos cadastros no fluxo de abertura/edicao de chamado;
- ampliar cobertura de testes automatizados para os novos modelos e endpoints.

## Sprint Cadastros Administrativos 2 - Backend CRUD

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar CRUD administrativo de departamentos, categorias e subcategorias com regras de validacao, ativacao/inativacao e preservacao historica.

Entregas tecnicas:
- CRUD de departamentos com listagem, busca, filtro por status e inativacao logica;
- CRUD de categorias com listagem, busca, filtro por status e inativacao logica;
- CRUD de subcategorias com listagem geral e por categoria;
- validacao de categoria obrigatoria/existente para subcategoria;
- bloqueio de duplicidade de subcategoria dentro da mesma categoria;
- rotas administrativas em `api/admin` com compatibilidade mantida em `api/admin/cadastros`;
- `DELETE` para cadastros convertido para comportamento de inativacao logica;
- testes automatizados de use cases ampliados para os tres cadastros.

Checklist Sprint 2:
- [x] DTOs de subcategoria criados
- [x] Use cases de subcategoria criados
- [x] Endpoints administrativos de subcategoria criados
- [x] Endpoints `PATCH` de ativar/inativar criados
- [x] Endpoints `DELETE` com inativacao logica criados
- [x] Validacoes de duplicidade aplicadas
- [x] Validacao de vinculo categoria/subcategoria aplicada
- [x] Listagem com busca e filtro por status validada
- [x] Testes automatizados criados/atualizados
- [x] Documentacao e roadmaps atualizados

Pendencias evolutivas:
- CRUD administrativo de tipos de solicitacao;
- CRUD administrativo de locais/unidades;
- integracao de subcategoria/tipo/local ao fluxo de abertura e atendimento.

## Sprint Cadastros Administrativos 3 - Backend CRUD

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar CRUD administrativo de prioridades, tipos de solicitacao e locais/unidades com regras de validacao, ativacao/inativacao e preservacao historica.

Entregas tecnicas:
- CRUD de prioridades com validacao de nome duplicado;
- validacao de peso obrigatorio e maior que zero em prioridades;
- validacao de cor opcional em prioridade no formato hexadecimal `#RRGGBB`;
- CRUD de tipos de solicitacao com validacao de nome duplicado;
- CRUD de locais/unidades com validacao de nome duplicado e endereco opcional;
- listagem com busca por nome, filtro por status e paginacao;
- `DELETE` para os tres cadastros com inativacao logica;
- aliases legados mantidos em `api/admin/cadastros/prioridades`, `api/admin/cadastros/tipos-solicitacao` e `api/admin/cadastros/locais`;
- testes automatizados de use case e HTTP para os tres cadastros.

Checklist Sprint 3:
- [x] Endpoints administrativos de prioridades
- [x] Endpoints administrativos de tipos de solicitacao
- [x] Endpoints administrativos de locais/unidades
- [x] Validacoes de duplicidade
- [x] Validacao de peso da prioridade
- [x] Validacao de cor da prioridade
- [x] Ativacao e inativacao
- [x] Inativacao logica em `DELETE`
- [x] Listagem com busca e filtro por status
- [x] Testes automatizados criados/atualizados
- [x] Documentacao e roadmaps atualizados

Pendencias evolutivas:
- integrar `TipoSolicitacao` e `LocalUnidade` na abertura/edicao de chamados;
- evoluir regras de SLA para considerar `Peso` como ordenacao principal de prioridade;
- homologacao funcional com usuarios-chave do modulo de cadastros.

## Sprint Cadastros Administrativos 4 - Frontend Administrativo

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar no frontend administrativo as telas de manutencao dos cadastros de departamentos, categorias, subcategorias, prioridades, tipos de solicitacao e locais/unidades.

Entregas tecnicas:
- menu `Admin > Cadastros` consolidado com todos os itens da trilha de cadastros;
- rotas frontend para listagem e detalhe de subcategorias, tipos de solicitacao e locais/unidades;
- listagens com busca por nome, filtro por status e paginacao;
- acoes de editar, inativar e reativar com confirmacao de usuario;
- formularios de cadastro com validacoes de regras obrigatorias de negocio;
- prioridade atualizada para uso de `Peso` e `Cor`;
- consumo preferencial dos endpoints `api/admin/*` na camada de services do frontend.

Checklist Sprint 4:
- [x] Menu Admin > Cadastros atualizado
- [x] Tela de Departamentos
- [x] Tela de Categorias
- [x] Tela de Subcategorias
- [x] Tela de Prioridades
- [x] Tela de Tipos de Solicitacao
- [x] Tela de Locais / Unidades
- [x] Services de API frontend atualizados
- [x] Rotas frontend criadas/atualizadas
- [x] Busca e filtro por status funcionando
- [x] Ativacao/Inativacao com confirmacao
- [x] Feedback visual de sucesso e erro
- [x] Estados de carregamento e lista vazia
- [x] Documentacao atualizada

Pendencias evolutivas:
- homologacao funcional com usuarios administrativos;
- testes E2E do fluxo de cadastros;
- integracao dos novos cadastros no fluxo de chamados (fora desta sprint).

## Sprint Comentarios no Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] API `GET /api/chamados/{chamadoId}/comentarios` criada/ajustada.
- [x] API `POST /api/chamados/{chamadoId}/comentarios` criada/ajustada.
- [x] Regras por perfil (Administrador, Atendente, Solicitante) aplicadas.
- [x] Solicitante bloqueado para comentario interno.
- [x] Solicitante sem visao de comentario interno.
- [x] Ordenacao cronologica crescente aplicada.
- [x] Validacao de mensagem obrigatoria e limite de 4000 caracteres.
- [x] Frontend de detalhe do chamado com envio de comentarios atualizado.
- [x] Testes automatizados backend/frontend executados.
- [x] Migration incremental aplicada com alteracoes reais.
- [x] Documentacao do modulo de atendimento atualizada.

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/ComentariosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Sprint Anexos no Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] `GET /api/chamados/{chamadoId}/anexos`
- [x] `POST /api/chamados/{chamadoId}/anexos`
- [x] `GET /api/chamados/{chamadoId}/anexos/{anexoId}/download`
- [x] validacoes de seguranca de upload implementadas
- [x] controle de acesso por perfil e por chamado aplicado
- [x] caminho fisico e nome armazenado nao expostos na API
- [x] upload/listagem/download refletidos no frontend de detalhe
- [x] testes backend e frontend executados
- [x] build frontend executado
- [x] **nenhum endpoint DELETE de anexo exposto**
- [x] **nenhum botao de exclusao de anexo criado**

Regra de rastreabilidade aplicada:
- Anexos enviados permanecem como evidencia do atendimento e nao possuem fluxo de exclusao.

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/AnexosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Sprint Historico e Linha do Tempo do Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] `GET /api/chamados/{chamadoId}/linha-do-tempo`
- [x] linha do tempo consolidada com abertura, comentarios, anexos e historico
- [x] visibilidade por perfil aplicada (`Administrador`, `Atendente`, `Solicitante`)
- [x] solicitante sem comentarios internos e sem eventos internos sensiveis
- [x] evento de anexo na timeline com download
- [x] sem exposicao de `Caminho` e `NomeArquivoArmazenado`
- [x] atualizacao de timeline apos comentario e upload de anexo
- [x] sem endpoint DELETE de anexo
- [x] sem botao de exclusao de anexo
- [x] testes backend/frontend executados
- [x] build frontend executado

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/LinhaTempoChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Item de Roadmap - Comentarios e Anexos (Atendimento)

Status final do item:
- Area: Atendimento
- Nome: Comentarios e anexos
- StatusImplementacao: Implementado funcionalmente
- StatusTecnico: Completo
- PercentualImplementacao: 100
- SituacaoAtual: Implementado
- Avaliacao: Aprovado

Checklist consolidado:
- grupo Comentarios: concluido
- grupo Anexos: concluido
- grupo Governanca: concluido

Pendencias:
- tecnicas: nenhuma pendencia bloqueante
- homologacao: validar formalmente em ambiente de homologacao com usuarios reais, se ainda nao houver evidencia formal

Regra obrigatoria mantida:
- anexo salvo no atendimento nao pode ser excluido por nenhum perfil;
- nao existe endpoint DELETE de anexo;
- nao existe botao de exclusao de anexo.

## Sprint Cadastros Administrativos 5 - Integracao com Chamados

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Conectar os cadastros administrativos ao ciclo operacional do chamado (abertura, classificacao, triagem, detalhe e filtros), garantindo uso de ativos em novas operacoes e preservacao historica de inativos.

Entregas consolidadas:
- entidade `Chamado` evoluida com `SubcategoriaId`, `TipoSolicitacaoId` e `LocalUnidadeId`;
- migration `20260515212153_Sprint5IntegracaoCadastrosChamados`;
- validacoes de negocio para ativos e vinculo categoria/subcategoria;
- contexto de portal e admin com subcategorias/tipos/locais ativos;
- filtros administrativos por categoria, subcategoria, prioridade, tipo, departamento e local/unidade;
- detalhe de chamado (portal e admin) exibindo nomes dos cadastros vinculados;
- endpoints operacionais de consulta ativa em `/api/cadastros/*`.

Checklist:
- [x] abertura carrega categorias ativas
- [x] subcategorias filtradas por categoria
- [x] prioridades ativas disponiveis
- [x] tipos de solicitacao ativos disponiveis
- [x] locais/unidades ativos disponiveis
- [x] inativos bloqueados para novas selecoes
- [x] historico de chamados antigos preservado
- [x] filtros administrativos atualizados
- [x] detalhe do chamado com novos nomes vinculados
- [x] build backend/frontend e testes backend sem erro

Pendencias evolutivas:
- ampliar automacao de testes frontend para fluxos completos de triagem;
- avaliar evolucao de departamento em dois papeis (solicitante x responsavel) em sprint futura.

## Sprint Cadastros Administrativos 6 - Seed Inicial, Testes e Fechamento

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar o modulo de cadastros com massa inicial idempotente, revisao de testes e fechamento documental para validacao funcional.

Resumo:
- seed inicial dos principais cadastros administrativos consolidado no `DevelopmentSeedService`;
- protecao contra duplicidade por normalizacao de nomes (inclusive variacoes de acentuacao);
- prioridades padrao consolidadas com peso/cor definidos para a operacao;
- subcategorias padrao consolidadas por categoria com vinculo correto;
- validacao de endpoints operacionais `/api/cadastros/*` para retorno somente de ativos;
- validacao do filtro operacional de subcategorias ativas por categoria;
- documentacao final da trilha de cadastros atualizada.

Checklist Sprint 6:
- [x] Seed inicial aplicado sem duplicidade
- [x] Testes automatizados revisados e passando
- [x] Fluxo operacional validado com ativos/inativos
- [x] Documentacao finalizada
- [x] Roadmap atualizado

Pendencias evolutivas:
- evoluir para seed configuravel por ambiente institucional;
- ampliar testes frontend automatizados para fluxo completo de abertura e triagem.

## Sprint Cadastros Administrativos 7 - Checklist Funcional e Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Finalizar a validacao funcional da trilha de cadastros administrativos integrada ao fluxo de chamados, com foco em consistencia de regras de ativo/inativo e leitura historica.

Resumo:
- checklist tecnico funcional revisado e coberto por testes de use case/integracao;
- filtros administrativos de cadastros validados com `Ativo`, `Inativo` e `Todos`;
- validacoes de abertura e classificacao de chamados mantidas (categoria/subcategoria/prioridade/tipo/local/departamento quando aplicavel);
- validacao de historico preservado para chamados antigos com cadastro inativo;
- ajustes finos de validadores (cor hexadecimal de prioridade e categoria obrigatoria para subcategoria);
- documentacao de homologacao funcional consolidada.

Checklist Sprint 7:
- [x] checklist funcional revisado
- [x] ajustes finos aplicados
- [x] testes executando com sucesso
- [x] build backend OK
- [x] build frontend OK
- [x] documentacao atualizada
- [x] modulo validado funcionalmente

Pendencias evolutivas:
- homologacao manual com evidencias visuais formais em ambiente institucional;
- suite frontend automatizada/E2E de cobertura visual ponta a ponta.

## Sprint Cadastros Administrativos 8 - Consolidacao ITSM e Checklist de Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar a governanca documental do modulo de cadastros administrativos e formalizar o checklist de homologacao para validacao institucional.

Checklist Sprint 8:
- [x] documento ITSM especifico dos cadastros administrativos criado
- [x] checklist de homologacao funcional criado
- [x] documentacao de cadastros atualizada com o fechamento da sprint
- [x] roadmap geral atualizado
- [x] roadmap ITSM atualizado

Evidencias documentais:
- `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md`
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`
- `docs/CADASTROS-ADMINISTRATIVOS.md`

Pendencias evolutivas:
- execucao manual do checklist em ambiente de homologacao com usuarios reais;
- formalizacao de aceite funcional e registro de evidencias visuais.
