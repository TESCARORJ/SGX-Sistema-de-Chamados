# Sprint 3 - Testes de preservacao do responsavel do chamado

## Escopo

Validacao transversal da regra de `ResponsavelId` nos fluxos de grupos tecnicos, filas e atribuicao da Sprint 3.

Nao foram criados endpoint, tela, regra de negocio ou migration estrutural.

## Matriz de comportamento do ResponsavelId

| Fluxo | Comportamento esperado | Resultado |
| --- | --- | --- |
| Direcionar chamado para grupo tecnico | Preserva `ResponsavelId` | Validado |
| Direcionar chamado para grupo tecnico com fila | Preserva `ResponsavelId` | Validado |
| Ajustar fila no mesmo grupo via direcionamento | Preserva `ResponsavelId` | Validado |
| Assumir chamado da fila | Preenche `ResponsavelId` com usuario autenticado | Validado |
| Atribuir chamado a tecnico especifico | Altera `ResponsavelId` para tecnico informado | Validado |
| Reatribuir chamado | Altera `ResponsavelId` para novo tecnico e registra historico | Validado |
| Transferir chamado entre grupos | Limpa `ResponsavelId` e registra historico | Validado |
| Cadastro/edicao/status de grupo tecnico | Nao altera `ResponsavelId` | Validado |
| Inclusao/inativacao/reativacao de membro | Nao altera `ResponsavelId` | Validado |
| Listagem/filtros/detalhe | Nao altera `ResponsavelId` | Validado |
| Frontend de assumir/transferir/atribuir | Nao altera responsavel localmente; recarrega detalhe | Validado |

## Cenarios backend testados

- Direcionamento inicial preserva responsavel.
- Direcionamento inicial com fila preserva responsavel.
- Ajuste de fila no mesmo grupo preserva responsavel.
- Assumir fila preenche responsavel com usuario autenticado.
- Atribuicao altera responsavel para tecnico informado.
- Reatribuicao altera responsavel e registra `ResponsavelAlterado`.
- Transferencia entre grupos limpa responsavel e registra `ResponsavelRemovidoPorTransferenciaGrupo`.
- Criar, atualizar, inativar e reativar grupo tecnico nao alteram responsaveis de chamados.
- Adicionar, reativar, inativar e reativar membro de grupo tecnico nao alteram responsaveis de chamados.
- Listagem, filtros por grupo/fila/responsavel e detalhe administrativo nao alteram responsaveis.

## Cenarios de API testados

- Endpoint de direcionamento preserva responsavel.
- Endpoint de assumir fila preenche responsavel.
- Endpoint de transferencia limpa responsavel.
- Endpoints de grupo e membros continuam cobertos por testes de integracao e regressao.
- Endpoint real de atribuicao administrativa permanece `POST /api/admin/chamados/{id}/atribuir`; a alteracao controlada do responsavel e validada no use case e regressao completa.

## Cenarios frontend testados

- Acao de assumir fila chama backend, recarrega detalhe e nao altera responsavel localmente.
- Acao de transferir grupo chama backend, recarrega detalhe e nao limpa responsavel localmente.
- Acao de atribuir chama backend, recarrega detalhe e nao altera responsavel localmente.
- Listagem e detalhe exibem responsavel sem mutacao indevida.

## Regressao validada

- Direcionamento, assumir fila, transferencia e atribuicao continuam passando.
- Cadastro de grupo e membros continuam passando.
- Listagem e detalhe continuam passando.
- Historico de responsavel continua coerente.

## Bugs encontrados e corrigidos

Nenhum bug de produto foi identificado.

Foram adicionadas coberturas de teste para lacunas reais:

- Direcionamento inicial com fila preservando responsavel.
- Ajuste de fila no mesmo grupo preservando responsavel.
- Cadastro/edicao/status de grupo tecnico sem alterar responsavel.
- Gestao de membros sem alterar responsavel.
- Listagem/filtros/detalhe sem alterar responsavel.
- Frontend de atribuicao sem mutacao local direta de responsavel.

## Bugs nao corrigidos

Nenhum bug funcional ficou pendente nesta etapa.

## Comandos executados

```powershell
git status --short
dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~DirecionarChamadoGrupoTecnico|FullyQualifiedName~AssumirChamadoFila|FullyQualifiedName~AtribuirChamado|FullyQualifiedName~TransferirGrupoTecnico|FullyQualifiedName~MembrosGruposTecnicos|FullyQualifiedName~GruposTecnicos"
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore
dotnet build SGX.SistemaChamado.sln --no-restore
dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
npm.cmd run test:unit
npm.cmd run build
dotnet ef migrations add ConcluirTestesPreservacaoResponsavelChamadoSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
git status --short
```

## Resultado final

Validacao concluida com sucesso.

- Backend/use case: aprovado.
- API/integration: aprovado.
- Frontend: aprovado.
- Build backend Debug e Release: aprovado.
- Build frontend: aprovado.
- EF pending model changes: sem alteracoes pendentes.
- Roadmap Sprint 3 atualizado para 44/54 itens, aproximadamente 81%.

## Observacao sobre arquivos locais

Arquivos `.dotnet-cli-home` aparecem no workspace e nao fazem parte desta etapa. Eles nao devem ser incluidos em commit; recomenda-se limpar ou ignorar esses arquivos fora do escopo desta validacao.

## Proxima etapa recomendada

Testar auditoria das movimentacoes.
