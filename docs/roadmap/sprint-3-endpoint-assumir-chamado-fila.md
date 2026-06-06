# Sprint 3 - Endpoint para assumir chamado da fila

## Contexto

Esta etapa expoe o fluxo administrativo para assumir chamado que ja esta em grupo tecnico e fila de atendimento. O endpoint reutiliza o use case existente e nao adiciona regra de negocio no controller.

## Controller alterado

Arquivo alterado: `src/SGX.SistemaChamado.Api/Controllers/AdminChamadosController.cs`.

O controller segue o padrao administrativo de chamados:

- rota base `api/admin`;
- `[ApiController]`;
- autorizacao de classe com `Policies.AdminOuAtendente`;
- metodo `POST` para acao operacional em chamado;
- delegacao direta para use case de aplicacao;
- retorno `200 OK` com `ChamadoAdminDetalheResponse`;
- tratamento local de `UnauthorizedAccessException`, `KeyNotFoundException`, `InvalidOperationException` e `ArgumentException`.

## Endpoint criado

| Metodo | Rota | Use case | Autorizacao |
| --- | --- | --- | --- |
| POST | `/api/admin/chamados/{chamadoId}/assumir-fila` | `IAssumirChamadoFilaAdminUseCase` | Administrador ou Atendente |

## Contrato usado

Body: `AssumirChamadoFilaRequest`.

Campos:

- `UsuarioId`
- `Observacao`

O controller nao valida se o usuario do request e o usuario autenticado. Essa regra permanece no use case.

## Diferencas de fluxo

- Assumir fila: usado quando o chamado ja esta em grupo tecnico e fila, sem responsavel individual. O use case preenche `ResponsavelId` com o proprio usuario autenticado, preservando `GrupoTecnicoId` e `FilaAtendimentoId`.
- Assumir legado: continua existindo no endpoint de assumir chamado e nao foi alterado nesta etapa.
- Atribuir tecnico: fluxo separado para atribuicao manual; nao foi criado endpoint novo de atribuicao a terceiros.
- Transferir grupo: fluxo futuro separado para chamados que precisam mudar de grupo tecnico; nao foi criado nesta etapa.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/AssumirChamadoFilaEndpointsIntegrationTests.cs`.

Cenarios cobertos:

- Administrador assume chamado da fila quando e membro ativo do grupo.
- Atendente assume chamado da fila quando e membro ativo do grupo.
- Solicitante e bloqueado.
- Usuario fora do grupo e rejeitado pelo use case.
- Chamado ja com responsavel e rejeitado pelo use case.
- `ResponsavelId` e preenchido.
- `GrupoTecnicoId` e preservado.
- `FilaAtendimentoId` e preservado.
- Historico `ChamadoAssumidoDaFila` e registrado.
- Endpoint nao expoe transferencia nem atribuicao tecnica a terceiros nesta etapa.

## O que nao foi implementado

- Nenhum endpoint de transferencia entre grupos.
- Nenhum endpoint de atribuicao de tecnico especifico a terceiros.
- Nenhum endpoint de fila isolada.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra nova de negocio.
- Nenhuma alteracao estrutural em `Chamado`, `ResponsavelId`, `GrupoTecnicoId`, `FilaAtendimentoId`, SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar endpoint para assumir chamado` foi marcado como concluido.

Com 29 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 54%.

Foi mantida pendente a proxima etapa `Criar endpoint para transferencia de chamado`.

## Proxima etapa recomendada

Criar endpoint administrativo para transferencia de chamado entre grupos tecnicos, reaproveitando `ITransferirGrupoTecnicoChamadoAdminUseCase` e mantendo atribuicao manual a terceiros fora da etapa.
