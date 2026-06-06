# Sprint 3 - Testes de transferencia entre grupos tecnicos

## Escopo

Validacao funcional e tecnica do fluxo de transferencia de chamado entre grupos tecnicos, sem criacao de endpoint, tela, regra de negocio ou migration estrutural.

Endpoint validado:

- `POST /api/admin/chamados/{chamadoId}/transferir-grupo-tecnico`

Use case validado:

- `TransferirGrupoTecnicoChamadoUseCase`

Frontend validado:

- Acao `Transferir grupo` no detalhe administrativo do chamado.
- Service `adminService.transferirGrupoTecnicoChamado`.

## Cenarios backend testados

- Transferir chamado de grupo origem para grupo destino ativo.
- Rejeitar chamado sem grupo tecnico anterior.
- Rejeitar grupo destino inexistente.
- Rejeitar grupo destino inativo.
- Rejeitar fila destino inexistente.
- Rejeitar fila destino inativa.
- Rejeitar fila destino de outro grupo.
- Alterar `GrupoTecnicoId` para o grupo destino.
- Limpar `ResponsavelId` durante a transferencia.
- Limpar `FilaAtendimentoId` quando nenhuma fila destino e informada.
- Definir `FilaAtendimentoId` quando a fila destino valida e informada.
- Registrar `GrupoTecnicoTransferido`.
- Registrar `ResponsavelRemovidoPorTransferenciaGrupo`.
- Registrar `FilaAtendimentoRemovida`, `FilaAtendimentoDefinida` ou `FilaAtendimentoTransferida`, conforme o cenario.
- Preservar dados de SLA do chamado.

## Cenarios de API testados

- Administrador transfere chamado entre grupos tecnicos.
- Atendente transfere chamado entre grupos tecnicos conforme policy atual.
- Solicitante recebe bloqueio.
- API rejeita chamado sem grupo tecnico anterior.
- API rejeita grupo destino inativo.
- API rejeita fila destino inexistente.
- API rejeita fila destino inativa.
- API rejeita fila destino de outro grupo.
- API retorna detalhe atualizado com grupo/fila atualizados e responsavel limpo.
- API registra historicos de grupo, fila e responsavel.

## Cenarios frontend testados

- Botao `Transferir grupo` aparece apenas quando `canTransferirGrupo` e verdadeiro.
- Detalhe administrativo so habilita transferencia quando ha grupo tecnico e perfil Administrador ou Atendente.
- Modal exige grupo tecnico de destino.
- Grupo destino igual ao grupo atual e bloqueado visualmente e por validacao da acao.
- Ao selecionar grupo destino, o frontend carrega filas ativas do grupo.
- Ao trocar grupo destino, a fila selecionada e limpa.
- Payload envia `grupoTecnicoId` e `filaAtendimentoId` opcional.
- Apos sucesso, o detalhe e recarregado.
- Erros da API sao propagados para o tratamento visual existente.
- Frontend nao limpa `ResponsavelId`, `GrupoTecnicoId` ou `FilaAtendimentoId` localmente sem resposta do backend.

## Permissoes

- Administrador: permitido.
- Atendente: permitido pela policy atual.
- Solicitante: bloqueado.

## Regressao validada

- Direcionamento inicial continua usando endpoint proprio.
- Assumir chamado da fila continua usando endpoint proprio.
- Atribuicao tecnica continua funcionando em fluxo separado.
- Listagem e detalhe continuam exibindo grupo/fila.
- Gestao de grupos e membros continua coberta no suite completo.
- Cadastro de grupo e filtros continuam cobertos no suite frontend/backend.

## Bugs encontrados e corrigidos

Nao houve bug de produto identificado.

Foram adicionadas coberturas de teste para lacunas reais:

- Use case rejeitando fila destino inexistente.
- Endpoint rejeitando fila destino inexistente.
- Endpoint rejeitando fila destino inativa.
- Endpoint validando historico `FilaAtendimentoTransferida`.
- Frontend validando modal, bloqueio de mesmo grupo, limpeza da fila selecionada e ausencia de mutacao local dos campos criticos.

## Bugs nao corrigidos

Nenhum bug funcional ficou pendente nesta etapa.

## Comandos executados

```powershell
git status --short
dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~TransferirGrupoTecnico"
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore
dotnet build SGX.SistemaChamado.sln --no-restore
dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
npm.cmd run test:unit
npm.cmd run build
dotnet ef migrations add ConcluirTestesTransferenciaGruposTecnicosSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
git status --short
```

## Resultado final

Validacao concluida com sucesso.

- Backend/use case: aprovado.
- API/integration: aprovado.
- Frontend/service: aprovado.
- Build backend Debug e Release: aprovado.
- Build frontend: aprovado.
- EF pending model changes: sem alteracoes pendentes.
- Roadmap Sprint 3 atualizado para 43/54 itens, aproximadamente 80%.

Nao houve endpoint novo, tela nova, regra backend nova, alteracao estrutural em `Chamado`, SLA, dashboard, relatorio ou migration estrutural.

## Observacao sobre arquivos locais

Arquivos `.dotnet-cli-home` aparecem no workspace e nao fazem parte desta etapa. Eles nao devem ser incluidos em commit; recomenda-se limpar ou ignorar esses arquivos fora do escopo desta validacao.

## Proxima etapa recomendada

Testar preservacao do responsavel atual do chamado.
