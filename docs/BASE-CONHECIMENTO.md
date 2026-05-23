# Base de Conhecimento - Fechamento Sprint 6

## Visao geral final

A Base de Conhecimento do SGX esta implementada funcionalmente do backend ao frontend, com:
- ciclo de vida completo de artigos;
- consulta no portal com controle de visibilidade por perfil;
- integracao com fluxo administrativo de chamados (vinculo e remocao de vinculo);
- trilha de historico e auditoria;
- cobertura de testes backend/frontend.

Status consolidado:
- Area: Base de conhecimento
- Categoria: Conhecimento
- Status da implementacao: Implementado funcionalmente
- Status tecnico: Homologacao funcional preparada
- Percentual: 90%

## Perfis e permissoes

Permissoes do modulo:
- `BaseConhecimento.Visualizar`
- `BaseConhecimento.Gerenciar`
- `BaseConhecimento.Publicar`
- `BaseConhecimento.Arquivar`
- `BaseConhecimento.VincularChamado`

Aplicacao por fluxo:
- listar/detalhar artigo (admin): `BaseConhecimento.Visualizar`
- criar/editar artigo: `BaseConhecimento.Gerenciar`
- publicar artigo: `BaseConhecimento.Publicar`
- arquivar/reativar artigo: `BaseConhecimento.Arquivar`
- vincular/remover vinculo em chamado: `BaseConhecimento.VincularChamado`

## Fluxo administrativo

1. Usuario autorizado acessa `/admin/conhecimento/base-conhecimento`.
2. Lista artigos com filtros por termo, status, visibilidade, categoria e ativo.
3. Cria/edita artigo em `/admin/conhecimento/base-conhecimento/novo` e `/:id`.
4. Publica artigo quando validado.
5. Arquiva (inativacao logica) ou reativa para novo ciclo.
6. Acoes registradas em auditoria conforme padrao do sistema.

## Fluxo do portal

1. Usuario autenticado acessa `/portal/base-conhecimento`.
2. Consulta apenas artigos `Publicado` e `Ativo` visiveis para seu perfil.
3. Busca por termo (titulo, resumo, conteudo e tags) e categoria.
4. Abre detalhe por slug em `/portal/base-conhecimento/:slug`.
5. Recebe `404` para artigo inexistente, nao publicado, inativo ou sem visibilidade.

## Fluxo de vinculo com chamados

1. Atendente/administrador com permissao acessa detalhe do chamado em `/admin/chamados/:id`.
2. Secao "Base de conhecimento" lista artigos vinculados.
3. Usuario busca artigos disponiveis e vincula ao chamado.
4. Usuario pode remover vinculo existente.
5. Historico do chamado registra vinculacao/remocao.
6. Auditoria registra evento de vinculacao/remocao.

## Regras de status

- `Rascunho`: estado inicial de criacao.
- `EmRevisao`: estado intermediario de validacao editorial.
- `Publicado`: unico status elegivel para consulta no portal e vinculo em chamado.
- `Arquivado`: artigo inativado logicamente.

Regras adicionais:
- sem exclusao fisica de artigo;
- arquivamento com `Status = Arquivado` e `Ativo = false`;
- reativacao retorna para `Rascunho` e `Ativo = true`;
- slug gerado automaticamente e unico.

## Regras de visibilidade

- `Solicitante`: visivel para solicitante, atendente e administrador.
- `Atendente`: visivel para atendente e administrador.
- `Administrador`: visivel somente para administrador.

## Endpoints

Administrativo de artigos:
- `GET /api/admin/base-conhecimento/artigos`
- `GET /api/admin/base-conhecimento/artigos/{id}`
- `POST /api/admin/base-conhecimento/artigos`
- `PUT /api/admin/base-conhecimento/artigos/{id}`
- `POST /api/admin/base-conhecimento/artigos/{id}/publicar`
- `POST /api/admin/base-conhecimento/artigos/{id}/arquivar`
- `POST /api/admin/base-conhecimento/artigos/{id}/reativar`

Portal:
- `GET /api/portal/base-conhecimento/artigos`
- `GET /api/portal/base-conhecimento/artigos/{slug}`

Integracao com chamados:
- `GET /api/admin/chamados/{chamadoId}/artigos-conhecimento`
- `GET /api/admin/chamados/{chamadoId}/artigos-conhecimento/disponiveis`
- `POST /api/admin/chamados/{chamadoId}/artigos-conhecimento/{artigoId}`
- `DELETE /api/admin/chamados/{chamadoId}/artigos-conhecimento/{artigoId}`

## Testes existentes

Backend:
- build Release validado;
- testes unitarios e integracao do modulo administrativos, portal, autorizacao e vinculo com chamado;
- status atual: 499 testes backend aprovados.

Frontend:
- testes unitarios de services da Base de Conhecimento;
- teste de cobertura da secao de vinculo no detalhe administrativo do chamado;
- status atual: 30 testes frontend aprovados;
- build frontend validado.

Testes E2E:
- nao ha Playwright/Cypress/framework E2E instalado no projeto nesta sprint;
- cobertura mantida por testes unitarios/integrados existentes;
- E2E completo permanece como pendencia evolutiva.

## Revisao de seguranca (Sprint 6)

Validacoes confirmadas:
- portal nao expoe artigos nao publicados/inativos;
- portal aplica visibilidade por perfil;
- endpoints admin exigem permissoes do modulo;
- vinculo com chamado exige `BaseConhecimento.VincularChamado`;
- backend valida status, ativo e duplicidade no vinculo;
- frontend nao e barreira unica de seguranca.

## Evidencias e homologacao

- Checklist funcional: `docs/CHECKLIST-HOMOLOGACAO-BASE-CONHECIMENTO.md`
- Estrutura de evidencias: `docs/evidencias/base-conhecimento/README.md`

## Pendencias evolutivas

- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos (quando houver framework E2E institucional);
- versionamento de artigos;
- workflow formal de aprovacao;
- anexos em artigos;
- avaliacao de utilidade do artigo;
- relatorio de artigos mais acessados;
- sugestao automatica de artigos durante abertura do chamado;
- sugestao automatica de artigos durante atendimento;
- busca semantica/IA.
