# Contratos de grupo tecnico

## Contexto

A Sprint 3 ja possui a entidade `GrupoTecnico` e sua persistencia. Esta etapa prepara somente os contratos da camada de aplicacao para que os proximos itens possam implementar cadastro, endpoints e tela sem acoplar a API diretamente ao dominio.

## Padrao analisado

Foram analisados os DTOs administrativos em `src/SGX.SistemaChamado.Application/DTOs/Admin`, especialmente:

- `AdminCadastrosDtos.cs`
- `AdminRequests.cs`
- `AdminResponses.cs`

O padrao encontrado usa classes para requests com propriedades `init`, records para responses simples, `FiltroCadastroRequest` para filtros de cadastro e `PagedResultResponse<T>` para respostas paginadas nos use cases.

## Contratos criados

Arquivo criado: `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminGrupoTecnicoDtos.cs`.

### ListarGruposTecnicosRequest

Reproduz os campos de `FiltroCadastroRequest`, porque esse tipo base e `sealed`, mantendo o mesmo contrato de paginacao sem alterar DTO existente:

- `Texto`
- `Ativo`
- `Pagina`
- `TamanhoPagina`
- `OrdenarPor`
- `DirecaoOrdenacao`

### CriarGrupoTecnicoRequest

- `Nome`: obrigatorio por contrato, inicializado como string vazia para seguir o padrao atual dos DTOs.
- `Descricao`: opcional.

### AtualizarGrupoTecnicoRequest

- `Nome`: obrigatorio por contrato, inicializado como string vazia para seguir o padrao atual dos DTOs.
- `Descricao`: opcional.

### AlterarStatusGrupoTecnicoRequest

- `Ativo`: indica a situacao desejada para ativacao ou inativacao futura.

### GrupoTecnicoResumoResponse

- `Id`
- `Nome`
- `Ativo`

### GrupoTecnicoResponse

- `Id`
- `Nome`
- `Descricao`
- `Ativo`
- `CriadoEm`
- `AtualizadoEm`

## Decisoes de campos e paginacao

Foram adotados os mesmos campos de `FiltroCadastroRequest` para manter o modelo de busca, ativo, pagina, tamanho e ordenacao usado por cadastros administrativos existentes. A heranca nao foi usada porque `FiltroCadastroRequest` e `sealed`.

Nao foi criado um tipo `ListarGruposTecnicosResponse` dedicado porque o padrao de use cases administrativos usa `PagedResultResponse<GrupoTecnicoResumoResponse>` para listas paginadas.

## Decisoes de escopo

Os contratos nao incluem membros, filas ou chamados. Esses relacionamentos serao tratados em contratos proprios ou responses especificos nas proximas etapas da Sprint 3.

Os contratos tambem nao expõem entidade de dominio nem propriedades internas de EF Core.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint.
- Nenhum service/use case funcional.
- Nenhuma tela Vue.
- Nenhuma regra de negocio.
- Nenhuma migration estrutural.
- Nenhuma alteracao em `Chamado`, `ResponsavelId`, SLA, dashboard ou relatorios.

## Roadmap

O item `Criar contratos de grupo tecnico` foi marcado como concluido. Com 16 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 30%.

## Proxima etapa recomendada

Criar o servico de aplicacao para cadastro de grupos tecnicos usando estes contratos e preservando a validacao de nome obrigatorio no fluxo de aplicacao/dominio.
