# Catalogo de Servicos - Fechamento Sprint 6

## Visao geral do modulo
O Catalogo de Servicos do SGX Sistema de Chamados organiza servicos institucionais para consulta e abertura orientada de chamados.

O modulo e institucional e multiárea, nao restrito a TI. Departamentos como RH, Financeiro, Patrimonio, Compras, Juridico e outros podem publicar servicos no mesmo fluxo, com governanca e visibilidade por perfil.

## Status consolidado da Sprint 6
- Area: Catalogo de Servicos
- Categoria: Conhecimento
- Status da implementacao: Implementado funcionalmente
- Status tecnico: Homologacao funcional preparada
- Percentual: 90%

## Entidades e enums principais
- Entidade principal: `CatalogoServico`
- Integracao com chamados: `Chamado.CatalogoServicoId` (opcional)
- Enum de status: `StatusCatalogoServico`
  - `Rascunho`
  - `Publicado`
  - `Arquivado`
- Enum de visibilidade: `VisibilidadeCatalogoServico`
  - `Interno`
  - `Solicitante`
  - `Atendente`
  - `Administrador`

## Permissoes do modulo
- `CatalogoServicos.Visualizar`
- `CatalogoServicos.Gerenciar`
- `CatalogoServicos.Publicar`
- `CatalogoServicos.Arquivar`

## Fluxo administrativo
1. Administrador/atendente autorizado cria ou edita servico.
2. Sistema valida dados obrigatorios e relacionamentos.
3. Slug e gerado automaticamente com garantia de unicidade.
4. Servico pode ser publicado quando atender regras de publicacao.
5. Servico pode ser arquivado e depois reativado para rascunho.
6. Auditoria registra criacao, edicao, publicacao, arquivamento e reativacao.

## Fluxo do portal
1. Usuario autenticado acessa o catalogo no portal.
2. Backend retorna apenas servicos `Publicado` e `Ativo`.
3. Backend aplica visibilidade por perfil.
4. Usuario pode filtrar por departamento/categoria/subcategoria e por permite abertura.
5. Usuario abre detalhe por slug.

## Fluxo de abertura de chamado por servico
1. No detalhe do servico, usuario aciona abertura.
2. Frontend chama `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`.
3. Backend valida elegibilidade do servico.
4. Frontend envia `CatalogoServicoId` na criacao do chamado.
5. Backend aplica dados oficiais do servico no chamado e ignora divergencias enviadas pelo frontend.
6. Chamado e persistido com associacao `CatalogoServicoId` e historico de abertura por catalogo.

## Regras de bloqueio
- Bloqueia servico em `Rascunho`.
- Bloqueia servico `Arquivado`.
- Bloqueia servico `Ativo = false`.
- Bloqueia servico sem visibilidade para o perfil.
- Bloqueia servico com `PermiteAberturaChamado = false`.

## Integracao com aprovacao de chamados (Sprint 3)
- Quando CatalogoServico.RequerAprovacao = true, a abertura cria automaticamente uma AprovacaoChamado pendente.
- A aprovacao automatica e criada com TipoOrigem = CatalogoServico e OrigemDescricao = nome do servico.
- O chamado passa a expor sinalizadores de aprovacao nos DTOs (RequerAprovacao, AprovacaoPendente, StatusAprovacao, AprovacaoChamadoId).
- Chamados com aprovacao pendente ou reprovada ficam bloqueados para avancar atendimento ate decisao valida.
- Historico AprovacaoSolicitada e registrado no momento da abertura automatica.

## Historico do chamado
Quando o chamado e aberto por catalogo, o sistema registra evento especifico de historico (`ChamadoCriadoPorCatalogoServico`) com referencia do servico utilizado.

## Endpoints administrativos
- `GET /api/admin/catalogo-servicos`
- `GET /api/admin/catalogo-servicos/{id}`
- `POST /api/admin/catalogo-servicos`
- `PUT /api/admin/catalogo-servicos/{id}`
- `POST /api/admin/catalogo-servicos/{id}/publicar`
- `POST /api/admin/catalogo-servicos/{id}/arquivar`
- `POST /api/admin/catalogo-servicos/{id}/reativar`

## Endpoints do portal
- `GET /api/portal/catalogo-servicos`
- `GET /api/portal/catalogo-servicos/{slug}`
- `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`

## Validacoes e testes existentes
- Backend build Release: OK.
- Testes backend: OK (`582` aprovados).
- Frontend unit tests: OK (`49` aprovados).
- Frontend build: OK.

## Revisao de UX da Sprint 6
Arquivos revisados:
- `CatalogoServicosListPage.vue`
- `CatalogoServicosFormPage.vue`
- `CatalogoServicosPortalPage.vue`
- `CatalogoServicoDetalhePage.vue`
- `NovoChamadoView.vue`

Resultado da revisao:
- estados de loading, erro e vazio presentes nas telas do modulo;
- mensagens amigaveis para falha de consulta e abertura;
- servico sem abertura exibido como consulta (mensagem explicita);
- botao de abertura apenas habilitado quando `PermiteAberturaChamado = true`;
- coerencia de acoes administrativas (editar/publicar/arquivar/reativar);
- responsividade basica por grid `col-12/col-md` nas telas principais;
- nenhuma ocorrencia de `console.log`, `debugger`, `TODO` ou `FIXME` nos 5 arquivos revisados.

## Revisao de seguranca da Sprint 6
- Portal exibe somente servicos `Publicado` e `Ativo`.
- Portal respeita visibilidade por perfil no backend.
- Endpoints admin exigem permissao `CatalogoServicos.*` conforme acao.
- Abertura por catalogo valida o servico no backend.
- Backend bloqueia rascunho, arquivado, inativo, sem visibilidade e `PermiteAberturaChamado = false`.
- Backend aplica departamento/categoria/subcategoria/prioridade/SLA oficiais do servico.
- Frontend nao e barreira principal de seguranca.
- Abertura sem catalogo permanece preservada.

## Homologacao e evidencias
- Checklist criado: `docs/CHECKLIST-HOMOLOGACAO-CATALOGO-SERVICOS.md`.
- Estrutura de evidencias criada: `docs/evidencias/catalogo-servicos/README.md`.

## Testes E2E
Nesta sprint nao foi identificado framework E2E (Playwright/Cypress) no projeto frontend.

Decisao da Sprint 6:
- nao instalar framework novo;
- manter E2E completo como pendencia evolutiva.

## Pendencias evolutivas
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- formularios dinamicos por servico;
- campos obrigatorios por servico;
- workflow de aprovacao por servico;
- aprovacoes por departamento;
- indicadores de servicos mais solicitados;
- relatorios por departamento;
- SLA avancado por servico;
- automacao de triagem por servico;
- sugestao de artigos da Base de Conhecimento por servico;
- melhoria de encoding dos arquivos `docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md`.
