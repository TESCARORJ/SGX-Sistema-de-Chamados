# Sprint 3 - Endpoints de membros de grupos tecnicos

## Contexto

A etapa expõe pela API administrativa os use cases de membros de grupos tecnicos ja existentes, mantendo o controller fino e sem alterar regras de negocio, chamados, filas, SLA, dashboard, relatorios, frontend ou estrutura de banco.

## Controller alterado

Arquivo alterado: `src/SGX.SistemaChamado.Api/Controllers/AdminGruposTecnicosController.cs`.

O controller manteve o padrao administrativo atual:

- rota base `api/admin/grupos-tecnicos`;
- `[ApiController]`;
- autorizacao de classe com `Policies.AdminOuAtendente`;
- operacoes de escrita com `Policies.Administrador`;
- delegacao direta para use cases;
- tratamento local de `UnauthorizedAccessException`, `KeyNotFoundException`, `ArgumentException` e `InvalidOperationException`;
- respostas `200 OK`, `403 Forbidden`, `404 NotFound` e `400 BadRequest` conforme o padrao ja usado no controller.

## Endpoints expostos

| Metodo | Rota | Use case | Autorizacao |
| --- | --- | --- | --- |
| GET | `/api/admin/grupos-tecnicos/{grupoTecnicoId}/membros` | `IListarMembrosGrupoTecnicoAdminUseCase` | Administrador ou Atendente |
| POST | `/api/admin/grupos-tecnicos/{grupoTecnicoId}/membros` | `IAdicionarMembroGrupoTecnicoAdminUseCase` | Administrador |
| PATCH | `/api/admin/grupos-tecnicos/{grupoTecnicoId}/membros/{membroId}/status` | `IAtualizarStatusMembroGrupoTecnicoAdminUseCase` | Administrador |
| GET | `/api/admin/usuarios/{usuarioId}/grupos-tecnicos` | `IListarGruposTecnicosDoUsuarioAdminUseCase` | Administrador ou Atendente |

## Contratos usados

- `ListarMembrosGrupoTecnicoRequest`
- `AdicionarMembroGrupoTecnicoRequest`
- `AlterarStatusMembroGrupoTecnicoRequest`
- `MembroGrupoTecnicoResponse`
- `GrupoTecnicoDoUsuarioResponse`

## Decisoes tecnicas

- Os endpoints de membros foram publicados no contexto de grupos tecnicos.
- A rota de grupos tecnicos por usuario foi exposta porque o use case ja existia e a rota administrativa faz sentido para consulta operacional.
- O controller nao valida existencia de grupo, usuario, duplicidade, status ou vinculo; essas regras permanecem nos use cases.
- Nao foram criadas policies novas. Foram reutilizadas `Policies.AdminOuAtendente` e `Policies.Administrador`.
- O retorno de criacao de membro segue o padrao atual do controller de grupos tecnicos e usa `200 OK`.

## Testes criados ou alterados

Arquivo alterado: `tests/SGX.SistemaChamado.Tests/GruposTecnicosEndpointsIntegrationTests.cs`.

Cenarios cobertos:

- Administrador lista membros do grupo.
- Atendente lista membros do grupo.
- Administrador adiciona membro ao grupo.
- Atendente nao adiciona membro.
- Administrador altera status do membro.
- Atendente nao altera status do membro.
- Endpoint rejeita duplicidade ativa retornada pelo use case.
- Gerenciar membros nao altera chamados.
- Atendente lista grupos tecnicos de um usuario.
- Nao foram expostos endpoints de direcionamento, assumir fila ou transferencia nesta etapa.

## O que nao foi implementado

- Nenhum endpoint de direcionamento de chamado.
- Nenhum endpoint de assumir fila.
- Nenhum endpoint de transferencia entre grupos tecnicos.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra nova de negocio.
- Nenhuma alteracao em `Chamado`, `ResponsavelId`, `GrupoTecnicoId` ou `FilaAtendimentoId`.
- Nenhuma alteracao em SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar endpoints de membros de grupos tecnicos` foi marcado como concluido.

Com 27 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser exatamente 50%.

Foi mantida pendente a proxima etapa `Criar endpoints de direcionamento para grupo`.

## Proxima etapa recomendada

Criar endpoints administrativos de direcionamento de chamado para grupo tecnico, reaproveitando `IDirecionarChamadoGrupoTecnicoAdminUseCase` e sem misturar assumir fila ou transferencia entre grupos.
