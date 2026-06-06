# Servico de aplicacao para cadastro de grupos tecnicos

## Contexto

A Sprint 3 ja possui a entidade `GrupoTecnico`, a tabela `grupos_tecnicos` e os contratos administrativos de grupo tecnico. Esta etapa cria a camada de aplicacao para operar o cadastro sem expor endpoint ou tela.

## Padrao analisado

Foram usados como referencia os use cases administrativos de cadastros:

- `DepartamentosAdminUseCases.cs`
- `CategoriasAdminUseCases.cs`
- `AdminCadastrosHelpers.cs`
- `IAdminUseCases.cs`
- `DependencyInjection.cs`

O padrao adotado usa use cases separados por operacao, repositorio generico, `IUnitOfWork`, autorizacao por contexto de usuario e `PagedResultResponse<T>` para listagens.

## Use cases criados

Arquivo criado: `src/SGX.SistemaChamado.Application/UseCases/Admin/GruposTecnicosAdminUseCases.cs`.

- `ListarGruposTecnicosAdminUseCase`
- `ObterGrupoTecnicoAdminUseCase`
- `CriarGrupoTecnicoAdminUseCase`
- `AtualizarGrupoTecnicoAdminUseCase`
- `AtualizarStatusGrupoTecnicoAdminUseCase`

As respectivas interfaces foram adicionadas em `IAdminUseCases.cs` e registradas no `DependencyInjection.cs`.

## Operacoes implementadas

- Listar grupos tecnicos com filtro por texto, filtro por ativo, ordenacao por nome/ativo e paginacao.
- Obter grupo tecnico por id.
- Criar grupo tecnico ativo por padrao.
- Atualizar nome e descricao.
- Ativar ou inativar grupo tecnico sem exclusao fisica.

## Validacoes implementadas

- Id vazio e rejeitado nas operacoes por id.
- Nome vazio ou composto apenas por espacos e rejeitado.
- Nome e normalizado com `Trim()`.
- Duplicidade de nome e validada antes do banco na criacao e atualizacao.
- Listagem e detalhe permitem Administrador ou Atendente, seguindo cadastros existentes.
- Criacao, atualizacao e alteracao de status exigem Administrador.

## Regra de duplicidade

A aplicacao verifica duplicidade por `Nome` antes de persistir. Essa validacao complementa o indice unico `ux_grupos_tecnicos_nome` ja existente no banco.

## Regra de ativacao/inativacao

Foi usado `AlterarStatusGrupoTecnicoRequest` com `Ativo` para permitir uma unica operacao de aplicacao para ativar ou inativar. A entidade continua usando seus metodos de dominio `Reativar` e `Inativar`.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/GruposTecnicosAdminUseCaseTests.cs`.

Cenarios cobertos:

- Listagem com busca, ativo e paginacao.
- Obter por id.
- Criar ativo por padrao.
- Rejeitar nome vazio.
- Rejeitar nome duplicado na criacao.
- Atualizar dados.
- Rejeitar duplicidade na atualizacao.
- Inativar e reativar.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhum cadastro de membros.
- Nenhuma regra de fila, roteamento ou transferencia nova.
- Nenhuma alteracao em `Chamado`, `ResponsavelId`, SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar servico de aplicacao para cadastro de grupos tecnicos` foi marcado como concluido. Com 17 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 31%.

## Proxima etapa recomendada

Criar o servico de aplicacao para membros de grupos tecnicos, mantendo a separacao entre cadastro de grupo e composicao de usuarios.
