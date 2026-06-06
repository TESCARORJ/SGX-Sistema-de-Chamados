# Sprint 3 - Testes de auditoria das movimentacoes

## Escopo

Validacao funcional e tecnica da auditoria gerada pelos fluxos de grupos tecnicos, filas de atendimento e responsavel individual. A etapa cobriu use cases, endpoints, linha do tempo no frontend, regressao dos fluxos relacionados e integridade do enum `TipoHistoricoChamado`.

Nao foram criados endpoint, tela, modelo de auditoria, tabela ou migration estrutural. A alteracao de banco desta etapa e apenas migration de dados para marcar o checklist do roadmap.

## Tipos de historico validados

- `GrupoTecnicoDefinido`
- `GrupoTecnicoTransferido`
- `FilaAtendimentoDefinida`
- `FilaAtendimentoRemovida`
- `FilaAtendimentoTransferida`
- `ResponsavelRemovidoPorTransferenciaGrupo`
- `ChamadoAssumidoDaFila`
- `ResponsavelAlterado`

## Matriz de movimentacao versus historico

| Movimentacao | Historico esperado | Validacao |
| --- | --- | --- |
| Direcionamento inicial para grupo | `GrupoTecnicoDefinido` | Use case e endpoint |
| Direcionamento inicial para grupo e fila | `GrupoTecnicoDefinido`, `FilaAtendimentoDefinida` | Use case e endpoint |
| Ajuste de fila no mesmo grupo | `FilaAtendimentoDefinida`, `FilaAtendimentoRemovida` ou `FilaAtendimentoTransferida` | Use case |
| Transferencia entre grupos | `GrupoTecnicoTransferido` | Use case e endpoint |
| Transferencia com responsavel atual | `ResponsavelRemovidoPorTransferenciaGrupo` | Use case e endpoint |
| Transferencia removendo fila | `FilaAtendimentoRemovida` | Use case |
| Transferencia definindo fila destino | `FilaAtendimentoDefinida` | Use case |
| Transferencia trocando fila | `FilaAtendimentoTransferida` | Use case e endpoint |
| Assumir chamado da fila | `ChamadoAssumidoDaFila` | Use case e endpoint |
| Atribuicao manual | `ResponsavelAlterado` | Use case e endpoint |
| Reatribuicao manual | `ResponsavelAlterado` com origem e destino | Use case |
| Linha do tempo | Tipos novos e antigos mapeados | Backend e frontend |

## Cenarios backend testados

- Direcionamento registra grupo definido e fila definida.
- Ajuste de fila registra fila definida, removida ou transferida conforme estado anterior.
- Transferencia registra grupo transferido, limpeza de responsavel e movimentacao de fila.
- Assumir fila registra o usuario que assumiu e a fila envolvida.
- Atribuir e reatribuir tecnico registram `ResponsavelAlterado`.
- Linha do tempo reconhece historicos novos e antigos.
- `TipoHistoricoChamado` manteve valores persistidos sem reordenacao.

## Cenarios de API testados

- Endpoint de direcionamento gera historico de grupo/fila.
- Endpoint de transferencia gera historico de transferencia, fila e remocao de responsavel.
- Endpoint de assumir fila gera `ChamadoAssumidoDaFila`.
- Endpoint de atribuicao gera `ResponsavelAlterado`.

## Cenarios frontend e linha do tempo

- A linha do tempo consome `listarLinhaTempoChamado`.
- Eventos exibem titulo, data, descricao, usuario, tipo e indicador interno vindos da API.
- Comentarios e anexos continuam recarregando a linha do tempo apos sucesso.
- O frontend nao fabrica eventos localmente; ele substitui a lista pela resposta do backend.

## Regressao validada

- Direcionamento continua funcionando.
- Transferencia continua funcionando.
- Assumir fila continua funcionando.
- Atribuicao tecnica continua funcionando.
- Listagem/detalhe e linha do tempo continuam exibindo historicos antigos.

## Bugs encontrados e corrigidos

Nenhum bug de regra de negocio foi encontrado. Foram adicionadas apenas lacunas de teste para auditoria do endpoint de atribuicao, mapeamento completo da linha do tempo e trava de valores do enum.

## Bugs nao corrigidos

Nenhum bug pendente identificado nesta etapa.

## Enum

Confirmado por teste automatizado que os valores existentes de `TipoHistoricoChamado` nao foram reordenados. Os tipos da Sprint 3 permanecem no final da sequencia atual, de `GrupoTecnicoTransferido = 33` ate `ChamadoAssumidoDaFila = 39`.

## Comandos executados

```powershell
git status --short
dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~DirecionarChamadoGrupoTecnico|FullyQualifiedName~TransferirGrupoTecnico|FullyQualifiedName~AssumirChamadoFila|FullyQualifiedName~AtribuirChamado|FullyQualifiedName~LinhaTempo"
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore
npm.cmd run test:unit
npm.cmd run build
dotnet build SGX.SistemaChamado.sln --no-restore
dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore
dotnet ef migrations add ConcluirTestesAuditoriaMovimentacoesSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
git status --short
```

## Resultado final

Auditoria das movimentacoes validada com sucesso. O roadmap da Sprint 3 foi atualizado somente no item `Testar auditoria das movimentacoes`, chegando a 45/54 itens ativos concluidos, aproximadamente 83%.

## Proxima etapa recomendada

Testar filtros/listagens por grupo e fila.
