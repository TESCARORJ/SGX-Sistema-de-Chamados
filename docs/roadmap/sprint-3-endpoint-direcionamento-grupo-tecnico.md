# Sprint 3 - Endpoint de direcionamento para grupo tecnico

## Contexto

Esta etapa expõe o direcionamento administrativo de chamado para grupo tecnico pela API. O endpoint reutiliza o use case ja existente e nao altera regras de negocio, estrutura de chamado, SLA, dashboard, relatorios ou frontend.

## Controller alterado

Arquivo alterado: `src/SGX.SistemaChamado.Api/Controllers/AdminChamadosController.cs`.

O controller segue o padrao administrativo de chamados:

- rota base `api/admin`;
- `[ApiController]`;
- autorizacao de classe com `Policies.AdminOuAtendente`;
- metodo `POST` para operacao de mudanca operacional em chamado;
- delegacao direta para use case de aplicacao;
- retorno `200 OK` com `ChamadoAdminDetalheResponse`;
- tratamento local de `UnauthorizedAccessException`, `KeyNotFoundException`, `InvalidOperationException` e `ArgumentException`.

## Endpoint criado

| Metodo | Rota | Use case | Autorizacao |
| --- | --- | --- | --- |
| POST | `/api/admin/chamados/{chamadoId}/direcionar-grupo-tecnico` | `IDirecionarChamadoGrupoTecnicoAdminUseCase` | Administrador ou Atendente |

## Contrato usado

Body: `DirecionarChamadoGrupoTecnicoRequest`.

Campos:

- `GrupoTecnicoId`
- `FilaAtendimentoId`
- `Observacao`

## Direcionamento, transferencia e assumir fila

- Direcionamento define grupo tecnico inicial para chamado sem grupo ou reforça o mesmo grupo ja definido.
- Transferencia entre grupos continua fora desta etapa; se o chamado ja possui outro grupo, o use case rejeita e orienta usar transferencia.
- Assumir fila continua fora desta etapa; direcionar para grupo/fila nao atribui tecnico e nao altera `ResponsavelId`.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/DirecionarChamadoGrupoTecnicoEndpointsIntegrationTests.cs`.

Cenarios cobertos:

- Administrador direciona chamado para grupo tecnico.
- Atendente direciona chamado para grupo tecnico.
- Solicitante e bloqueado.
- Grupo inexistente ou inativo retorna erro do use case.
- Fila de outro grupo retorna erro do use case.
- `ResponsavelId` e preservado.
- Direcionamento nao assume chamado da fila.
- Direcionamento nao transfere quando o chamado ja possui outro grupo.
- Endpoints de assumir fila, transferencia e atribuir tecnico nao foram expostos nesta etapa.

## O que nao foi implementado

- Nenhum endpoint de assumir chamado da fila.
- Nenhum endpoint de transferencia entre grupos.
- Nenhum endpoint de atribuicao de tecnico especifico.
- Nenhum endpoint de fila isolada.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra nova de negocio.
- Nenhuma alteracao estrutural em `Chamado`, `ResponsavelId`, SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar endpoints de direcionamento para grupo` foi marcado como concluido.

Com 28 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 52%.

Foi mantida pendente a proxima etapa `Criar endpoint para assumir chamado`.

## Proxima etapa recomendada

Criar endpoint administrativo para assumir chamado da fila, reaproveitando `IAssumirChamadoFilaAdminUseCase` e mantendo transferencia entre grupos fora da etapa.
