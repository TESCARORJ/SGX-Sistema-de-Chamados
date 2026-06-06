# Sprint 3 - Endpoint de transferencia entre grupos tecnicos

## Contexto

Esta etapa expoe o fluxo administrativo de transferencia de chamado entre grupos tecnicos. O endpoint reutiliza o use case existente e nao altera regras de negocio, modelo de chamado, SLA, dashboard, relatorios ou frontend.

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
| POST | `/api/admin/chamados/{chamadoId}/transferir-grupo-tecnico` | `ITransferirGrupoTecnicoChamadoUseCase` | Administrador ou Atendente |

## Contrato usado

Body: `TransferirGrupoTecnicoChamadoRequest`.

Campos:

- `GrupoTecnicoId`
- `FilaAtendimentoId`

## Diferencas de fluxo

- Transferencia: usada quando o chamado ja possui grupo tecnico anterior. O use case muda `GrupoTecnicoId`, limpa `ResponsavelId` e limpa ou redefine `FilaAtendimentoId` conforme a fila destino.
- Direcionamento: continua sendo o fluxo para chamado sem grupo inicial. A transferencia nao deve virar direcionamento inicial.
- Assumir fila: continua sendo o fluxo em que um membro ativo do grupo assume o chamado da fila e passa a ser responsavel individual.
- Atribuicao de tecnico: fluxo separado para atribuicao manual; nenhum endpoint novo de atribuicao a terceiros foi criado nesta etapa.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/TransferirGrupoTecnicoChamadoEndpointsIntegrationTests.cs`.

Cenarios cobertos:

- Administrador transfere chamado entre grupos tecnicos.
- Atendente transfere chamado entre grupos tecnicos.
- Solicitante e bloqueado.
- Chamado sem grupo anterior e rejeitado pelo use case.
- Grupo destino inativo e rejeitado pelo use case.
- Fila de outro grupo e rejeitada pelo use case.
- `GrupoTecnicoId` e alterado.
- `ResponsavelId` e limpo.
- `FilaAtendimentoId` e limpo quando nao ha fila destino.
- `FilaAtendimentoId` e redefinido quando ha fila destino valida.
- Historico `GrupoTecnicoTransferido` e `ResponsavelRemovidoPorTransferenciaGrupo` e registrado.
- Transferencia nao direciona chamado inicial, nao assume fila e nao atribui tecnico.

## O que nao foi implementado

- Nenhum endpoint de atribuicao de tecnico especifico a terceiros.
- Nenhum endpoint de fila isolada.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra nova de transferencia.
- Nenhuma alteracao nas regras de direcionamento ou assumir fila.
- Nenhuma alteracao estrutural em `Chamado`, SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar endpoint para transferencia de chamado` foi marcado como concluido.

Com 30 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 56%.

Foi mantida pendente a proxima etapa `Criar endpoint/listagem de fila por grupo tecnico`.

## Proxima etapa recomendada

Criar endpoint administrativo de listagem de filas por grupo tecnico, mantendo atribuicao tecnica e telas frontend fora da etapa.
