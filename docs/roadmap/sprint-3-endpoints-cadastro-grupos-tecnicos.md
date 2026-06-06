# Sprint 3 - Endpoints de cadastro de grupos tecnicos

## Controller

Foi criado o controller `AdminGruposTecnicosController` em `src/SGX.SistemaChamado.Api/Controllers`.

O controller segue o padrao administrativo do projeto:

- rota base `api/admin/grupos-tecnicos`;
- `[ApiController]`;
- autorizacao de classe com `Policies.AdminOuAtendente`;
- metodos de escrita com autorizacao adicional `Policies.Administrador`;
- delegacao direta para use cases de aplicacao;
- tratamento local de `UnauthorizedAccessException`, `KeyNotFoundException`, `ArgumentException` e `InvalidOperationException`.

## Endpoints expostos

| Metodo | Rota | Use case | Autorizacao |
| --- | --- | --- | --- |
| GET | `/api/admin/grupos-tecnicos` | `IListarGruposTecnicosAdminUseCase` | Administrador ou Atendente |
| GET | `/api/admin/grupos-tecnicos/{id}` | `IObterGrupoTecnicoAdminUseCase` | Administrador ou Atendente |
| POST | `/api/admin/grupos-tecnicos` | `ICriarGrupoTecnicoAdminUseCase` | Administrador |
| PUT | `/api/admin/grupos-tecnicos/{id}` | `IAtualizarGrupoTecnicoAdminUseCase` | Administrador |
| PATCH | `/api/admin/grupos-tecnicos/{id}/status` | `IAtualizarStatusGrupoTecnicoAdminUseCase` | Administrador |

## Contratos usados

- `ListarGruposTecnicosRequest`
- `CriarGrupoTecnicoRequest`
- `AtualizarGrupoTecnicoRequest`
- `AlterarStatusGrupoTecnicoRequest`
- `GrupoTecnicoResumoResponse`
- `GrupoTecnicoResponse`
- `AlterarSituacaoCadastroResponse`

## Decisoes tecnicas

- O controller nao duplica regra de negocio; as validacoes de nome, duplicidade, existencia e status permanecem nos use cases.
- As rotas de membros de grupos tecnicos nao foram criadas nesta etapa.
- Nao foram criadas policies novas, pois a matriz de permissao validada ja define `AdminOuAtendente` para leitura e `Administrador` para escrita.
- O retorno usa `200 OK`, seguindo o padrao predominante dos controllers administrativos atuais.

## Testes criados

Foram criados testes HTTP em `GruposTecnicosEndpointsIntegrationTests` cobrindo:

- listagem por Atendente;
- obtencao por Atendente;
- criacao por Administrador;
- bloqueio de criacao por Atendente;
- atualizacao por Administrador;
- bloqueio de atualizacao por Atendente;
- alteracao de status por Administrador;
- ausencia de endpoint de membros nesta etapa.

## O que nao foi implementado

- Endpoints de membros de grupo tecnico.
- Endpoints de direcionamento de chamado.
- Endpoints de assumir fila.
- Tela Vue.
- Service frontend.
- Alteracoes em `Chamado`, `ResponsavelId`, SLA, dashboard ou relatorios.
- Migration estrutural.

## Proxima etapa recomendada

Criar endpoints administrativos de membros de grupos tecnicos, reaproveitando os use cases ja existentes e mantendo a mesma matriz: leitura para Administrador/Atendente e escrita restrita a Administrador.
