# Sprint 5 - Ajuste de endpoints de resolucao, fechamento, aceite e reabertura

## Objetivo

Consolidar os endpoints HTTP do ciclo de fechamento da Sprint 5 para que a API exponha corretamente as regras ja implementadas nos use cases, sem duplicar regras de negocio nos controllers.

## Mapeamento final de rotas

### Admin

- `POST /api/admin/chamados/{id}/resolver`
  - Policy: `Permissao:Chamados.Encerrar`
  - Request: `ResolverChamadoRequest`
  - Use case: `ResolverChamadoUseCase`

- `POST /api/admin/chamados/{id}/encerrar`
  - Policy: `Permissao:Chamados.Encerrar`
  - Request: `EncerrarChamadoRequest`
  - Use case: `EncerrarChamadoUseCase`

- `POST /api/admin/chamados/{id}/reabrir`
  - Policy: `Permissao:Chamados.Reabrir`
  - Request: `ReabrirChamadoRequest`
  - Use case: `ReabrirChamadoUseCase`

- `POST /api/admin/chamados/fechamento-automatico/prazo-aceite/executar`
  - Policy: `Permissao:Chamados.Encerrar`
  - Request: `FecharChamadosAutomaticamentePorPrazoAceiteRequest`
  - Use case: `FecharChamadosAutomaticamentePorPrazoAceiteUseCase`

### Portal

- `POST /api/portal/chamados/{id}/aceitar-solucao`
  - Autenticacao: `Authorize`
  - Request: `AceitarSolucaoChamadoRequest`
  - Use case: `AceitarSolucaoChamadoUseCase`

- `POST /api/portal/chamados/{id}/rejeitar-solucao`
  - Autenticacao: `Authorize`
  - Request: `RejeitarSolucaoChamadoRequest`
  - Use case: `RejeitarSolucaoChamadoUseCase`

## Decisoes tecnicas

- Mantida a separacao entre `Resolvido` e `Encerrado`.
- Mantido o bloqueio por aprovacao pendente apenas nos use cases.
- Mantida a auditoria e o historico fora dos controllers.
- Reabertura administrativa passou a expor policy explicita de permissao.
- O fechamento automatico por prazo ganhou endpoint administrativo manual, sem criar scheduler.
- As rotas existentes foram preservadas; nao houve renomeacao de endpoint publico.

## Tratamento HTTP

- `200 OK` para execucao bem-sucedida.
- `400 BadRequest` para validacao e regras de negocio no padrao atual da API.
- `403 Forbidden` para falta de permissao/perfil.
- `404 NotFound` para recurso inexistente no padrao atual.

## Resultado

Item 17 concluido. A Sprint 5 passa a `17/32` itens concluidos e `53%` de implementacao.
A proxima acao passa a ser `Exibir dados de solucao, aceite e fechamento no detalhe do chamado`.
