# Sprint 3 - Endpoint de listagem de filas por grupo tecnico

## Contexto

Esta etapa expoe a listagem administrativa de filas de atendimento vinculadas a um grupo tecnico. O objetivo e permitir que fluxos como direcionamento e transferencia selecionem uma fila valida do grupo sem criar manutencao de filas nesta etapa.

## Contratos criados

Arquivo alterado: `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminGrupoTecnicoDtos.cs`.

- `ListarFilasAtendimentoGrupoTecnicoRequest`
  - `Ativo`
  - `Busca`
- `FilaAtendimentoResumoResponse`
  - `Id`
  - `GrupoTecnicoId`
  - `Nome`
  - `Descricao`
  - `Ativo`

## Use case criado

Arquivo criado: `src/SGX.SistemaChamado.Application/UseCases/Admin/FilasAtendimentoGrupoTecnicoAdminUseCases.cs`.

Use case: `ListarFilasAtendimentoGrupoTecnicoAdminUseCase`.

Responsabilidades:

- validar o id do grupo tecnico;
- aplicar autorizacao de aplicacao para Administrador ou Atendente;
- validar existencia do grupo tecnico;
- retornar somente filas cujo `GrupoTecnicoId` pertence ao grupo informado;
- aplicar filtro opcional por `Ativo`;
- aplicar busca opcional por nome ou descricao;
- ordenar por nome.

## Controller alterado

Arquivo alterado: `src/SGX.SistemaChamado.Api/Controllers/AdminGruposTecnicosController.cs`.

O controller segue o padrao administrativo de grupos tecnicos:

- rota base `api/admin/grupos-tecnicos`;
- `[ApiController]`;
- autorizacao de classe com `Policies.AdminOuAtendente`;
- delegacao direta para use case de aplicacao;
- tratamento padronizado por `ExecutarAsync`.

## Endpoint criado

| Metodo | Rota | Use case | Autorizacao |
| --- | --- | --- | --- |
| GET | `/api/admin/grupos-tecnicos/{grupoTecnicoId}/filas` | `IListarFilasAtendimentoGrupoTecnicoAdminUseCase` | Administrador ou Atendente |

Query:

- `ativo`
- `busca`

## O que nao foi implementado

- Nenhum endpoint de cadastro de fila.
- Nenhum endpoint de edicao de fila.
- Nenhum endpoint de inativacao/reativacao de fila.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra de roteamento automatico.
- Nenhuma transferencia entre filas.
- Nenhuma alteracao em `Chamado`, `ResponsavelId`, `GrupoTecnicoId`, `FilaAtendimentoId`, SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Testes criados ou alterados

Arquivos alterados:

- `tests/SGX.SistemaChamado.Tests/GruposTecnicosAdminUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/GruposTecnicosEndpointsIntegrationTests.cs`

Cenarios cobertos:

- Administrador lista filas de um grupo.
- Atendente lista filas de um grupo.
- Solicitante e bloqueado.
- Grupo inexistente retorna `404`.
- Retorna apenas filas do grupo informado.
- Filtro por ativo funciona.
- Busca por nome/descricao funciona.
- Nao retorna filas de outro grupo.
- Listagem nao altera chamados.
- Endpoint de cadastro/edicao de fila nao foi criado.

## Roadmap

O item `Criar endpoint/listagem de fila por grupo tecnico` foi marcado como concluido.

Com 31 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 57%.

Foi mantida pendente a proxima etapa `Criar tela ou secao de cadastro de grupos tecnicos`.

## Proxima etapa recomendada

Criar tela ou secao administrativa de cadastro de grupos tecnicos, reaproveitando os endpoints administrativos ja expostos.
