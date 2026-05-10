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
