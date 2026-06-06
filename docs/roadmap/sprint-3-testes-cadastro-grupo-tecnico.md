# Sprint 3 - Testes do cadastro de grupo tecnico

## Escopo validado

Esta etapa validou o cadastro administrativo de grupos tecnicos em backend, API, frontend, permissoes e regressao basica. Nenhuma funcionalidade nova foi criada.

## Cenarios backend

- Criacao de grupo tecnico com nome valido.
- Criacao com descricao nula.
- Rejeicao de nome vazio.
- Rejeicao de nome duplicado.
- Atualizacao de nome e descricao.
- Rejeicao de atualizacao para nome vazio.
- Rejeicao de atualizacao para nome duplicado de outro grupo.
- Ativacao de grupo inativo.
- Inativacao de grupo ativo.
- Listagem com busca.
- Listagem por ativo e inativo.
- Obtencao por id.
- Rejeicao de grupo inexistente.

## Cenarios de API

- Administrador lista grupos tecnicos.
- Atendente lista grupos tecnicos.
- Administrador cria grupo tecnico.
- Atendente nao cria grupo tecnico.
- Administrador edita grupo tecnico.
- Atendente nao edita grupo tecnico.
- Administrador altera status.
- Atendente nao altera status.
- API retorna erro amigavel para duplicidade.
- API retorna erro amigavel para nome vazio.

## Cenarios frontend

- Tela `/admin/cadastros/grupos-tecnicos` carrega a listagem.
- Busca por nome funciona.
- Filtro por status funciona.
- Modal/formulario de criacao e edicao funciona.
- Nome obrigatorio e validado visualmente.
- Criacao com sucesso atualiza a listagem.
- Edicao com sucesso atualiza a listagem.
- Ativacao e inativacao atualizam o status visual.
- Erros da API sao exibidos para o usuario.
- Atendente visualiza em modo somente leitura.
- Administrador visualiza as acoes de gestao.

## Regressao validada

- Tela de membros por grupo tecnico permanece coberta pela suite frontend.
- Tela de filas por grupo tecnico permanece coberta pela suite frontend.
- Listagem administrativa de chamados com filtros de grupo/fila permanece coberta pela suite frontend.
- Detalhe administrativo do chamado segue exibindo grupo/fila.
- O cadastro de grupo tecnico nao altera chamados, filas, membros ou responsaveis diretamente.

## Bugs encontrados

Nao houve bug de produto identificado. Foi ajustada apenas uma expectativa de teste para refletir a mensagem real da API de nome obrigatorio: `O nome do grupo tecnico e obrigatorio.`

## Testes e comandos executados

- `dotnet ef migrations list --configuration Release --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~GruposTecnicos"`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore`
- `dotnet build SGX.SistemaChamado.sln --no-restore`
- `dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore`
- `npm.cmd run test:unit`
- `npm.cmd run build`
- `dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`

## Resultado final

Validacao concluida com sucesso. O item `Testar cadastro de grupo tecnico` foi marcado como concluido no checklist da Sprint 3, elevando o progresso esperado para 39/54 itens, aproximadamente 72%.

## Nao implementado

- Nenhum endpoint novo.
- Nenhuma tela nova.
- Nenhuma regra backend nova.
- Nenhuma alteracao em Chamado, SLA, dashboard ou relatorio.
- Nenhuma migration estrutural.

## Proxima etapa recomendada

Testar inclusao e remocao de membros.
