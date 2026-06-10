# Sprint 4 - Endpoints de Consulta de Pendências de Aprovação

## Objetivo dos endpoints de pendências
Expor as informações das aprovações pendentes para os aprovadores ou administradores do novo motor de aprovações ITSM. Permitem consultar, por diversos ângulos, o que ainda precisa ser avaliado, servindo como a principal API de leitura para a futura caixa de entrada do aprovador.

## Limites desta etapa
A etapa concentra-se unicamente em **leitura**. Estes endpoints não aprovam, não reprovam, não expiram, não cancelam, não reavaliam e não geram aprovações obrigatórias. Eles tampouco implementam qualquer alteração na regra legada de `AprovacaoChamado` ou no frontend da aplicação. Nenhuma migration estrutural de entidade de domínio foi necessária.

## Contexto dos itens anteriores
Nos itens 40 e 41, os UseCases e validações fundamentais da aprovação foram desenvolvidos. No item 43, os endpoints de regras foram expostos. No item 44, os endpoints de aprovação e rejeição (`POST`) foram criados. Este item 45 complementa o ciclo expondo as instâncias sobre as quais o usuário pode agir.

## Controller alterado
O controller `AdminAprovacoesMotorController` foi estendido. Em vez de criar um controller isolado, preferiu-se agrupar as ações relacionadas ao motor de instâncias neste mesmo espaço de API, garantindo coesão ao expor o ciclo completo de *listar -> visualizar -> aprovar/reprovar*.

## Rotas criadas
- `GET /api/admin/aprovacoes-motor/pendencias`
- `GET /api/admin/aprovacoes-motor/pendencias/minhas`
- `GET /api/admin/aprovacoes-motor/pendencias/aprovador/{usuarioId}`
- `GET /api/admin/aprovacoes-motor/chamados/{chamadoId}/pendencias`
- `GET /api/admin/aprovacoes-motor/instancias/{id}`
- `GET /api/admin/aprovacoes-motor/status/{status}`

## Endpoint de listagem geral de pendências
A rota `GET /pendencias` aceita um payload dinâmico via query parameters usando `ListarInstanciasAprovacaoChamadoRequest`. Retorna resultados paginados no formato `PagedResultResponse<InstanciaAprovacaoChamadoResumoResponse>`.

## Endpoint de pendências por aprovador autenticado
A rota `GET /pendencias/minhas` identifica o usuário autenticado por meio do `IUsuarioContextoAplicacaoService` e repassa o seu Id para o método `ListarPendentesAsync` do application layer. Retorna um array de `InstanciaAprovacaoChamadoResumoResponse`.

## Endpoint de pendências por usuário aprovador
A rota `GET /pendencias/aprovador/{usuarioId}` funciona de maneira idêntica à anterior, mas permitindo que um gestor/admin consulte as pendências de um subordinado ou terceiro através de um GUID de `usuarioId` explícito.

## Endpoint de pendências por chamado
A rota `GET /chamados/{chamadoId}/pendencias` aciona `ListarPorChamadoAsync` informando o Id de um chamado. Útil para renderizar as aprovações no detalhe do chamado, uma necessidade futura (item 46).

## Endpoint de detalhe da pendência
A rota `GET /instancias/{id}` aciona `ObterPorIdAsync`, que hidrata o objeto de forma completa com a `InstanciaAprovacaoChamadoResponse`, as etapas filhas e o histórico de decisões. Útil para exibir os detalhes finos do que o aprovador está julgando.

## Endpoint de pendências por status
A rota `GET /status/{status}` aciona `ListarPorStatusAsync`, servindo para cenários de relatórios operacionais.

## Contratos reutilizados
Reutilizamos as classes criadas nos itens de arquitetura da Sprint:
- `ListarInstanciasAprovacaoChamadoRequest`
- `InstanciaAprovacaoChamadoResumoResponse`
- `InstanciaAprovacaoChamadoResponse`

## Serviço reutilizado
Os métodos invocam a abstração pronta do `IAdminInstanciaAprovacaoChamadoUseCases`, garantindo respeito completo às regras de domínio e arquitetura DDD. Para identificar o usuário autenticado, utilizou-se `IUsuarioContextoAplicacaoService`.

## Padrão de autorização aplicado
Todos os endpoints novos foram expostos sob o envelope base `Policies.AdminOuAtendente` e a policy granular `PermissionPolicies.AprovacaoChamadosConsultar`.

## Padrão de retorno aplicado
Foi utilizado o padrão de envelope `ExecutarAsync` implementado de maneira similar a outros controllers. Esse padrão executa a task e devolve um HTTP 200 OK contendo os resultados encapsulados, ou erro.

## Tratamento de erro
Foi utilizado o tratamento existente no envolpe da Controller que repassa os seguintes erros:
- `UnauthorizedAccessException` -> 403 Forbid
- `KeyNotFoundException` -> 404 Not Found
- `ArgumentException` e `InvalidOperationException` -> 400 Bad Request
Erros de validação fluente seguem o middleware global padrão.

## Dados retornados
O resumo das instâncias provê de imediato o Id da instância, dados de origem, status, fluxo e se é bloqueante, servindo perfeitamente para listas. O detalhamento (endpoint por Id) provê também o nome da regra de origem, dados sensíveis salvos em snapshot na hora da geração (como Custo, Nível de Risco, etc.) e o array de decisões até então.

## Filtros, Paginação e Ordenação suportados
Tudo herdado do contrato rico de `ListarInstanciasAprovacaoChamadoRequest`, suportando ordenação, limite de página, número de página e termo de busca.

## Relações com Entidades
- **Relação com `InstanciaAprovacaoChamado`**: É o objeto principal trafegado nos dados destas consultas.
- **Relação com `EtapaAprovacaoChamado`**: Retornadas dentro da `InstanciaAprovacaoChamadoResponse`.
- **Relação com `DecisaoAprovacaoChamado`**: Igualmente listadas dentro da aba de histórico da `InstanciaAprovacaoChamadoResponse`.
- **Relação com `AprovacaoChamado` legado**: Nenhuma interferência.
- **Relação com endpoints de aprovação/rejeição do item 44**: O front-end usará os Id's e informações lidas aqui para instruir as chamadas POST do item 44.
- **Relação com bloqueio do item 39**: Endpoints lerão instâncias que estão mantendo chamados paralisados.

## Relação futura com frontend
Estes métodos alimentam os widgets (item 46) no detalhe do chamado e a caixa de entrada dedicada para aprovadores do item 47.

## O que os endpoints não fazem
Eles estão expressamente desenhados sem permissão ou código para modificar estado: não geram, cancelam, expiram ou reavaliam.

## Testes criados ou executados
- Executado o `RoadmapSprint4MotorAprovacoesChecklistTests` demonstrando atualização e consistência.
- Efetuado comando de teste no `InstanciaAprovacaoChamado` provando retrocompatibilidade com o core do domínio.

## Riscos de segurança e governança
Nenhum risco de injeção direta de alteração de dado encontrado, dado que a rota expõe somente tarefas de leitura baseadas em policies já homologadas de visualização de chamados. A consulta `pendencias/minhas` atua unicamente com base no Token JWT autenticado seguro.

## Decisões adiadas para próximos itens
A implementação de frontend (UI/SPA) das telas para uso das rotas construídas aqui não foram feitas. Ficarão para os itens 46 e 47 do checklist.

## Conclusão técnica
A separação via UseCases demonstrou sua força: um serviço `InstanciaAprovacaoChamadoAdminUseCases` que foi montado no sub-item 37 agora foi acoplado via Controller perfeitamente, sem precisar reescrever lógica SQL ou regras de banco de dados no nível HTTP.

## Próxima etapa recomendada
Executar o item 46: **Exibir status de aprovação no detalhe do chamado**.
