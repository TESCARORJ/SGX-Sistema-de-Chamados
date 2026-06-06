# Sprint 3 - Testes de regressao do fluxo atual de abertura e atribuicao de chamado

## Escopo

Validacao funcional e tecnica dos fluxos legados de abertura, listagem, detalhe, assumir e atribuir chamado apos a introducao de grupos tecnicos e filas de atendimento na Sprint 3.

Nao foram criados endpoint, tela, regra backend ou migration estrutural nesta etapa.

## Cenarios de abertura testados

- Solicitante abre chamado pelo endpoint atual do portal com payload legado.
- Abertura nao exige `GrupoTecnicoId`.
- Abertura nao exige `FilaAtendimentoId`.
- Chamado novo permanece com `GrupoTecnicoId = null`, `FilaAtendimentoId = null` e `ResponsavelId = null`.
- Natureza, impacto, urgencia, categoria, subcategoria, tipo de solicitacao, local e prioridade continuam aceitos conforme regras existentes.
- Historico de criacao continua registrado.
- SLA permanece sob responsabilidade das regras existentes de `AbrirChamadoUseCase`/`SlaService`, sem alteracao de regra nesta etapa.

## Cenarios de listagem e detalhe testados

- Chamado aberto aparece na listagem administrativa.
- Detalhe administrativo carrega chamado sem grupo/fila.
- Detalhe do solicitante carrega o chamado do proprio solicitante.
- Listagem administrativa preserva chamados sem grupo/fila.
- Fallbacks de grupo/fila no frontend continuam previstos para chamados legados.

## Cenarios de assumir legado testados

- Endpoint legado `POST /api/admin/chamados/{id}/assumir` continua funcionando.
- Assumir legado preenche `ResponsavelId`.
- Assumir legado preserva `GrupoTecnicoId` e `FilaAtendimentoId` quando nulos.
- Assumir legado nao usa o fluxo de fila e nao registra `ChamadoAssumidoDaFila`.
- Historico de responsavel continua registrado pelo fluxo legado.

## Cenarios de atribuicao testados

- Endpoint legado `POST /api/admin/chamados/{id}/atribuir` continua funcionando.
- Atribuicao de chamado sem grupo altera `ResponsavelId` para tecnico valido.
- Atribuicao preserva `GrupoTecnicoId` e `FilaAtendimentoId`.
- Atribuicao com grupo tecnico continua coberta por testes existentes que exigem membro ativo e rejeitam tecnico fora do grupo.
- Reatribuicao continua coberta por teste existente com historico de troca de responsavel.

## Cenarios de SLA e historico testados

- Historico `Criado` continua registrado na abertura.
- Historico `ResponsavelAlterado` continua registrado em assumir/atribuir legado.
- Testes de `SlaService` e regressao filtrada continuam validando comportamento atual de SLA.
- A etapa nao alterou regra de SLA.

## Regressao Sprint 3 validada

- Direcionamento para grupo continua na bateria filtrada.
- Assumir chamado da fila continua na bateria filtrada.
- Transferencia entre grupos tecnicos continua na bateria filtrada.
- Filtros/listagens por grupo e fila continuam na bateria filtrada.
- Auditoria e linha do tempo continuam na bateria filtrada.
- Cadastro de grupos e membros continua na bateria filtrada.

## Frontend testado

- `portalService` envia payload legado de abertura para `/api/portal/chamados` sem `grupoTecnicoId` ou `filaAtendimentoId`.
- Tela administrativa de listagem mantem acao legado de assumir via `adminService.assumirChamado(id)` e recarrega chamados.
- Detalhe administrativo mantem atribuicao via backend e recarregamento do detalhe, sem mutacao local direta do responsavel.

## Bugs encontrados e corrigidos

Nenhum bug funcional foi encontrado. Foram adicionados testes para lacunas reais de cobertura de regressao HTTP e frontend/service.

## Bugs nao corrigidos

Nenhum bug pendente identificado nesta etapa.

## Comandos executados

- `git status --short`
- `dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~AbrirChamado|FullyQualifiedName~AssumirChamadoUseCase|FullyQualifiedName~AtribuirChamado|FullyQualifiedName~ListarChamados|FullyQualifiedName~DetalharChamado|FullyQualifiedName~Sla|FullyQualifiedName~Historico"`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore`
- `dotnet build SGX.SistemaChamado.sln --no-restore`
- `dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore`
- `npm.cmd run test:unit`
- `npm.cmd run build`
- `dotnet ef migrations add ConcluirTestesRegressaoAberturaAtribuicaoChamadoSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext`
- `git status --short`

## Resultado final

Validacao concluida com sucesso. O fluxo legado de abertura e atribuicao de chamado permanece operavel com chamados sem grupo tecnico e sem fila de atendimento.

Roadmap da Sprint 3 atualizado somente no item `Testar regressao do fluxo atual de abertura e atribuicao de chamado`, com progresso esperado de 47/54, aproximadamente 87%.

## Proxima etapa recomendada

Documentar modelo de grupo tecnico, filas e atribuicao.
