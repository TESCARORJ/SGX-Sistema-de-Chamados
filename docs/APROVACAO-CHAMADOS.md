# Aprovacao de Chamados - SGX Sistema de Chamados

## Visao geral do modulo
O modulo Aprovacao de Chamados controla quando um chamado precisa de aprovacao antes de seguir para atendimento operacional. O objetivo e garantir governanca, rastreabilidade, seguranca e conformidade sem perder fluidez do processo.

Area: Aprovacao de chamados
Categoria: Atendimento
Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%

## Conceito de aprovacao
A aprovacao e um ciclo formal associado ao chamado. Uma aprovacao pode ser aberta manualmente no administrativo ou automaticamente na abertura por catalogo, e segue ate decisao (aprovar/reprovar) ou cancelamento.

## Status de aprovacao
- `Pendente`: chamado aguardando decisao de aprovacao.
- `Aprovado`: chamado liberado para fluxo operacional.
- `Reprovado`: chamado permanece bloqueado para avanco operacional.
- `Cancelado`: ciclo de aprovacao cancelado; o bloqueio e removido se nao houver outra pendencia ativa.

## Tipos de origem
- `Manual`: solicitacao feita por usuario autorizado no administrativo.
- `CatalogoServico`: solicitacao criada automaticamente quando o servico exige aprovacao.

## Permissoes
- `AprovacaoChamados.Visualizar`: listagem e detalhe administrativo.
- `AprovacaoChamados.Gerenciar`: solicitacao manual de aprovacao.
- `AprovacaoChamados.Aprovar`: decisao de aprovacao.
- `AprovacaoChamados.Reprovar`: decisao de reprovacao.
- `AprovacaoChamados.Cancelar`: cancelamento de aprovacao pendente.

## Fluxo manual de aprovacao
1. Usuario com permissao abre o detalhe administrativo do chamado.
2. Aciona "Solicitar aprovacao" informando origem manual e justificativa de solicitacao.
3. Aprovacao e criada como pendente, com historico e auditoria.
4. Usuario com permissao decide: aprovar, reprovar (com justificativa obrigatoria) ou cancelar (com justificativa obrigatoria).

## Fluxo automatico por Catalogo de Servicos
1. Solicitante abre chamado via servico com `RequerAprovacao = true`.
2. Sistema cria aprovacao pendente automaticamente com `TipoOrigem = CatalogoServico`.
3. `OrigemDescricao` recebe o nome do servico.
4. Historico registra `AprovacaoSolicitada` automaticamente.

## Bloqueios operacionais
- Chamado com aprovacao pendente nao pode ser assumido.
- Chamado com aprovacao pendente nao pode avancar status de atendimento.
- Chamado com aprovacao pendente nao pode ser encerrado.
- Chamado reprovado permanece bloqueado para avanco operacional.
- Consulta, comentarios e visualizacao continuam disponiveis quando aplicavel.

## Comportamento apos aprovacao
- Chamado e liberado para fluxo normal de atendimento.
- Historico registra evento de chamado aprovado.
- Auditoria registra o ato e o executor da decisao.

## Comportamento apos reprovacao
- Chamado permanece bloqueado para avanco operacional.
- Reprovacao exige justificativa.
- Historico registra evento de chamado reprovado.
- Auditoria registra o ato e o executor da decisao.

## Comportamento apos cancelamento
- Cancelamento exige justificativa.
- Historico registra evento de aprovacao cancelada.
- Bloqueio e removido se nao houver outra aprovacao pendente ativa.
- Auditoria registra o ato e o executor do cancelamento.

## Acompanhamento no portal
- Listagem do portal exibe indicador discreto de aprovacao (nao requer, pendente, aprovado, reprovado, cancelado).
- Detalhe do chamado no portal exibe secao "Aprovacao" com status, datas e justificativa de decisao quando aplicavel.
- Mensagens orientativas padronizadas:
  - "Seu chamado esta aguardando aprovacao antes de seguir para atendimento."
  - "Seu chamado foi aprovado e esta liberado para atendimento."
  - "Seu chamado foi reprovado. Verifique a justificativa."
  - "A aprovacao deste chamado foi cancelada."

## Regras de seguranca
- Endpoints administrativos exigem permissoes `AprovacaoChamados.*` conforme acao.
- Backend bloqueia decisao duplicada.
- Backend exige justificativa para reprovar e cancelar.
- Backend bloqueia avancos indevidos de chamado pendente/reprovado.
- Portal permite consultar aprovacao apenas de chamado do proprio solicitante.
- Portal nao expoe dados administrativos sensiveis (ex.: aprovador).
- Frontend nao e barreira principal de seguranca; validacao principal permanece no backend.
- Historico de chamado e auditoria coexistem para rastreabilidade funcional e tecnica.

## Endpoints administrativos
- `GET /api/admin/aprovacao-chamados`
- `GET /api/admin/aprovacao-chamados/{id}`
- `POST /api/admin/chamados/{chamadoId}/aprovacao/solicitar`
- `POST /api/admin/aprovacao-chamados/{id}/aprovar`
- `POST /api/admin/aprovacao-chamados/{id}/reprovar`
- `POST /api/admin/aprovacao-chamados/{id}/cancelar`

## Endpoints de portal
- `GET /api/portal/chamados`
- `GET /api/portal/chamados/{id}`
- `GET /api/portal/chamados/{chamadoId}/aprovacao`

## Testes existentes
- Backend:
  - testes unitarios e de integracao cobrindo regras de aprovacao, autorizacao, bloqueios operacionais e portal;
  - execucao mais recente: 718 testes aprovados.
- Frontend:
  - testes unitarios de services e views administrativas/portal do modulo;
  - execucao mais recente: 84 testes aprovados.

## Revisao UX (Sprint 6)
Itens revisados em `AprovacaoChamadosListPage.vue`, `AprovacaoChamadosDetalhePage.vue`, `AdminDetalheChamadoView.vue`, `PortalChamadosView.vue` e `DetalheChamadoView.vue`:
- estados de loading, erro e vazio presentes;
- mensagens de erro amigaveis mantidas;
- botoes condicionais por status/permissao presentes;
- reprovacao/cancelamento com justificativa obrigatoria visivel;
- acoes sensiveis com confirmacao mantidas;
- alertas claros de chamado pendente/reprovado;
- linguagem do portal ajustada para solicitante;
- sem exposicao de dados administrativos sensiveis no portal;
- sem `console.log`, `debugger` ou `TODO/FIXME` indevidos.

## Revisao de seguranca (Sprint 6)
- validado que permissoes administrativas continuam exigidas para acoes sensiveis;
- validado que regras de negocio de aprovacao permanecem no backend;
- validado que bloqueios de chamado pendente/reprovado continuam ativos;
- validado que endpoint de portal respeita escopo do solicitante;
- nenhuma regra de seguranca foi relaxada nesta sprint.

## Notificacoes - pontos de integracao futura
Nesta sprint nao foi implantada infraestrutura nova de notificacao.
Pontos de integracao futura permanecem mapeados para:
- aprovacao solicitada;
- chamado aprovado;
- chamado reprovado;
- aprovacao cancelada.

## Homologacao e evidencias
- checklist funcional criado em `docs/CHECKLIST-HOMOLOGACAO-APROVACAO-CHAMADOS.md`;
- estrutura de evidencias preparada em `docs/evidencias/aprovacao-chamados/README.md`.

## Pendencias evolutivas
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos (framework E2E nao instalado nesta sprint);
- multiplos niveis de aprovacao;
- aprovacao por alcada;
- aprovacao por departamento;
- aprovacao por centro de custo;
- delegacao de aprovador;
- prazo limite para aprovacao;
- escalonamento automatico;
- notificacoes por e-mail;
- notificacoes por Teams;
- aprovacao por celular;
- relatorios de tempo medio de aprovacao;
- indicadores de chamados reprovados;
- regras configuraveis por catalogo, categoria, departamento e prioridade.
