# Servico de aplicacao para membros de grupos tecnicos

## Contexto

A Sprint 3 ja possui `GrupoTecnico`, `MembroGrupoTecnico`, relacionamento com `Usuario` e indice unico composto por `grupo_tecnico_id` e `usuario_id`. Esta etapa cria a camada de aplicacao para administrar membros sem expor endpoint, tela ou regra funcional de fila.

## Padrao analisado

Foram seguidos os padroes dos use cases administrativos existentes:

- Use cases separados por operacao.
- Interfaces em `IAdminUseCases.cs`.
- Registro `Scoped` no `DependencyInjection.cs`.
- Repositorio generico e `IUnitOfWork`.
- Autorizacao por `AdminCadastrosHelpers`.
- Responses pequenos, sem expor entidade de dominio.

## Contratos criados

Foram adicionados em `AdminGrupoTecnicoDtos.cs`:

- `ListarMembrosGrupoTecnicoRequest`
- `AdicionarMembroGrupoTecnicoRequest`
- `AlterarStatusMembroGrupoTecnicoRequest`
- `MembroGrupoTecnicoResponse`
- `GrupoTecnicoDoUsuarioResponse`

## Use cases criados

Arquivo criado: `src/SGX.SistemaChamado.Application/UseCases/Admin/MembrosGruposTecnicosAdminUseCases.cs`.

- `ListarMembrosGrupoTecnicoAdminUseCase`
- `AdicionarMembroGrupoTecnicoAdminUseCase`
- `AtualizarStatusMembroGrupoTecnicoAdminUseCase`
- `ListarGruposTecnicosDoUsuarioAdminUseCase`

## Operacoes implementadas

- Listar membros de um grupo tecnico, com filtro opcional por ativo.
- Adicionar usuario como membro de grupo tecnico.
- Inativar membro logicamente.
- Reativar membro logicamente.
- Reativar vinculo inativo ao adicionar novamente o mesmo usuario ao mesmo grupo.
- Listar grupos tecnicos de um usuario.

## Validacoes implementadas

- Grupo tecnico inexistente e tratado.
- Grupo tecnico inativo nao recebe novo membro.
- Usuario inexistente e tratado.
- Duplicidade ativa do mesmo usuario no mesmo grupo e rejeitada.
- Vinculo inativo e reativado sem criar novo registro.
- Ids vazios sao rejeitados.

## Regra de duplicidade

O indice unico composto no banco impede duplicidade fisica. Na aplicacao, o use case verifica se ja existe vinculo:

- Ativo: retorna erro de duplicidade.
- Inativo: reativa o mesmo registro.
- Inexistente: cria novo registro ativo.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma regra de fila, roteamento ou transferencia nova.
- Nenhuma alteracao em `Chamado`, `ResponsavelId`, `GrupoTecnicoId` ou `FilaAtendimentoId`.
- Nenhuma alteracao em SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/MembrosGruposTecnicosAdminUseCaseTests.cs`.

Cenarios cobertos:

- Listagem por grupo com filtro ativo.
- Inclusao de membro.
- Rejeicao de grupo inativo.
- Rejeicao de usuario inexistente.
- Rejeicao de duplicidade ativa.
- Reativacao de vinculo inativo.
- Inativacao e reativacao logica.
- Listagem de grupos tecnicos por usuario.

## Roadmap

O item `Criar servico de aplicacao para membros de grupos tecnicos` foi marcado como concluido. Com 18 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 33%.

## Proxima etapa recomendada

Criar regra para direcionar chamado a grupo tecnico, preservando `ResponsavelId` como responsavel individual e sem implementar ainda o fluxo completo de fila.
