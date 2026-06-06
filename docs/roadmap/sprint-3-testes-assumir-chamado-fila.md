# Sprint 3 - Testes de assumir chamado da fila

## Escopo validado

Esta etapa validou funcional e tecnicamente o fluxo de assumir chamado da fila em use case, API, frontend, permissoes, historico, preservacao de grupo/fila e regressao. Nenhuma funcionalidade nova foi criada.

## Cenarios backend

- Assumir chamado de fila com usuario membro ativo do grupo.
- Preenchimento de `ResponsavelId` com o usuario autenticado.
- Preservacao de `GrupoTecnicoId`.
- Preservacao de `FilaAtendimentoId`.
- Rejeicao de usuario sem perfil administrativo.
- Rejeicao de chamado inexistente.
- Rejeicao de chamado sem grupo tecnico.
- Rejeicao de chamado sem fila de atendimento.
- Rejeicao de chamado ja com responsavel individual.
- Rejeicao de usuario que nao e membro ativo do grupo.
- Rejeicao de vinculo de membro inativo.
- Rejeicao de grupo tecnico inativo.
- Rejeicao de fila inativa.
- Rejeicao de fila pertencente a outro grupo.
- Rejeicao de `UsuarioId` diferente do usuario autenticado.
- Registro de historico `ChamadoAssumidoDaFila`.

## Cenarios de API

- Administrador assume chamado da fila quando tambem e membro ativo do grupo.
- Atendente assume chamado da fila quando e membro ativo do grupo.
- Solicitante recebe `403 Forbidden`.
- API retorna erro para usuario fora do grupo.
- API retorna erro para vinculo inativo.
- API retorna erro para chamado sem grupo.
- API retorna erro para chamado sem fila.
- API retorna erro para grupo inativo.
- API retorna erro para fila inativa.
- API retorna erro para fila de outro grupo.
- API retorna erro para chamado ja com responsavel.
- API retorna erro quando `UsuarioId` nao e o usuario autenticado.
- API preserva grupo/fila e retorna detalhe atualizado.
- API registra historico `ChamadoAssumidoDaFila`.

## Cenarios frontend

- Botao `Assumir da fila` aparece apenas quando ha grupo, fila, usuario autenticado e ausencia de responsavel.
- Botao nao aparece quando nao ha grupo.
- Botao nao aparece quando nao ha fila.
- Botao nao aparece quando ja ha responsavel.
- Botao nao aparece para perfil sem permissao administrativa.
- Ao confirmar, o frontend chama `adminService.assumirChamadoFila` enviando `usuarioId` do usuario autenticado.
- Apos sucesso, o detalhe e recarregado via backend.
- Erros da API sao exibidos pelo fluxo `registrarErro`.
- O frontend nao altera `ResponsavelId` ou `responsavel` localmente sem retorno do backend.

## Permissoes

- Administrador pode assumir desde que seja membro ativo do grupo.
- Atendente pode assumir desde que seja membro ativo do grupo.
- Solicitante nao pode assumir.
- Usuario fora do grupo ou com vinculo inativo nao pode assumir.

## Regressao validada

- Fluxo legado de assumir chamado continua usando `POST /api/admin/chamados/{id}/assumir`.
- Direcionamento para grupo continua usando `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico`.
- Transferencia entre grupos continua usando `POST /api/admin/chamados/{id}/transferir-grupo-tecnico`.
- Atribuicao tecnica continua usando `POST /api/admin/chamados/{id}/atribuir`.
- Listagem e detalhe continuam exibindo grupo tecnico e fila.
- Gestao de membros continua sem alterar chamados.

## Bugs encontrados

Nao houve bug de produto identificado. Foram adicionados testes para lacunas de cobertura HTTP em vinculo inativo, chamado sem grupo/fila, grupo/fila inativos e fila de outro grupo, alem de testes frontend mais explicitos para visibilidade, reload, erro da API e ausencia de mutacao local do responsavel.

## Bugs nao corrigidos

Nenhum bug pendente desta etapa.

## Comandos executados

- `git status --short`
- `dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~AssumirChamadoFila"`
- `dotnet ef migrations add ConcluirTestesAssumirChamadoFilaSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore`
- `dotnet build SGX.SistemaChamado.sln --no-restore`
- `dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore`
- `dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `npm.cmd run test:unit`
- `npm.cmd run build`
- `git status --short`

## Resultado final

Validacao concluida com sucesso. O item `Testar assumir chamado da fila` foi marcado como concluido no checklist da Sprint 3, elevando o progresso esperado para 42/54 itens, aproximadamente 78%.

## Nao implementado

- Nenhum endpoint novo.
- Nenhuma tela nova.
- Nenhuma regra backend nova.
- Nenhuma alteracao estrutural em `Chamado`.
- Nenhuma alteracao em SLA, dashboard ou relatorio.
- Nenhuma migration estrutural.

## Observacao sobre arquivos locais

O workspace possui arquivos de telemetria em `.dotnet-cli-home`. Eles nao fazem parte desta validacao e nao devem ser incluidos em commit; recomenda-se limpar ou ignorar esses artefatos fora desta tarefa.

## Proxima etapa recomendada

Testar transferencia entre grupos tecnicos.
