# Sprint 3 - Testes de listagem e filtros por grupo/fila

## Escopo

Validacao funcional e tecnica da listagem administrativa de chamados considerando grupo tecnico e fila de atendimento. A etapa cobriu use case, endpoint HTTP, filtros dependentes no frontend, tabela desktop/mobile, valores nulos, paginacao, filtros antigos e regressao dos fluxos relacionados.

Nao foram criados endpoint, tela, acao operacional nova na listagem ou regra backend nova. A alteracao de banco desta etapa e apenas migration de dados para marcar o checklist do roadmap.

## Cenarios backend testados

- Listagem sem filtro retorna chamados com grupo/fila e chamados legados sem grupo/fila.
- Projecao retorna `GrupoTecnicoId`, `GrupoTecnicoNome`, `FilaAtendimentoId` e `FilaAtendimentoNome`.
- Filtro por `GrupoTecnicoId` retorna somente chamados do grupo.
- Filtro por `FilaAtendimentoId` retorna somente chamados da fila.
- Filtro combinado por grupo + fila retorna somente chamados compativeis.
- Filtros antigos por status, natureza, prioridade, categoria, subcategoria, tipo de solicitacao, local/unidade e responsavel continuam funcionando.
- Paginacao continua retornando total e pagina corretamente.
- Detalhe administrativo continua retornando grupo/fila e aceita valores nulos.
- Listagem/filtros nao alteram responsavel, grupo, fila ou historico.

## Cenarios de API testados

- `GET /api/admin/chamados` aceita `grupoTecnicoId`.
- `GET /api/admin/chamados` aceita `filaAtendimentoId`.
- Endpoint retorna os campos de grupo/fila na resposta.
- Endpoint preserva comportamento sem filtros e retorna chamados com e sem grupo/fila.
- Endpoint mantem autorizacao existente para Administrador e Atendente.
- Endpoint nao altera dados nem cria historico.

## Cenarios frontend testados

- Filtro de grupo tecnico carrega grupos ativos via `adminService.listarGruposTecnicos`.
- Filtro de fila carrega filas ativas do grupo selecionado.
- Troca de grupo limpa `filaAtendimentoId`.
- Filtro de fila fica desabilitado sem grupo selecionado.
- `adminService.listarChamadosAdmin` envia `grupoTecnicoId` e `filaAtendimentoId`.
- Filtros antigos continuam enviando os parametros existentes.
- Tabela exibe bloco de atendimento com grupo/fila.
- Fallbacks `Sem grupo` e `Sem fila` aparecem em tabela e card mobile.
- Nao foram adicionadas acoes novas de direcionamento, transferencia ou atribuicao na listagem.

## Cenarios de regressao testados

- Detalhe do chamado continua exibindo grupo/fila.
- Direcionamento para grupo continua funcionando.
- Assumir chamado da fila continua funcionando.
- Transferencia entre grupos continua funcionando.
- Cadastro de grupos, membros e filas continua funcionando.
- Historico/linha do tempo continua funcionando.

## Bugs encontrados e corrigidos

Nenhum bug de regra de negocio foi encontrado. Foram adicionadas lacunas de cobertura para filtro combinado, endpoint HTTP da listagem e validacoes frontend de filtro/tabela.

## Bugs nao corrigidos

Nenhum bug pendente identificado nesta etapa.

## Comandos executados

```powershell
git status --short
dotnet ef migrations list --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore --filter "FullyQualifiedName~ListarChamadosAdmin|FullyQualifiedName~DetalharChamadoAdmin|FullyQualifiedName~GruposTecnicos|FullyQualifiedName~DirecionarChamadoGrupoTecnico|FullyQualifiedName~AssumirChamadoFila|FullyQualifiedName~TransferirGrupoTecnico|FullyQualifiedName~LinhaTempo"
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-restore
npm.cmd run test:unit
npm.cmd run build
dotnet build SGX.SistemaChamado.sln --no-restore
dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore
dotnet ef migrations add ConcluirTestesListagemFiltrosGrupoFilaSprint3Roadmap --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef database update --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj --context SGXSistemaChamadoDbContext
git status --short
```

## Resultado final

Listagem administrativa e filtros por grupo/fila validados com sucesso. O roadmap da Sprint 3 foi atualizado somente no item `Testar filtros/listagens por grupo e fila`, chegando a 46/54 itens ativos concluidos, aproximadamente 85%.

## Proxima etapa recomendada

Testar regressao do fluxo atual de abertura e atribuicao de chamado.
