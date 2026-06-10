# Sprint 4 - Endpoints de Aprovação e Rejeição

## Objetivo dos endpoints
Expor a lógica funcional para aprovação e rejeição/reprovação de instâncias ou etapas de aprovação geradas pelo novo Motor de Aprovações ITSM.

## Limites desta etapa
Esta etapa concentra-se unicamente em receber requisições de aprovação ou rejeição e repassá-las aos casos de uso já desenvolvidos (`AprovarAprovacaoChamadoUseCase` e `ReprovarAprovacaoChamadoUseCase`). Não são criados endpoints para configuração de regras, listagem de pendências, cancelamento, expiração ou reavaliação. Nenhuma tela (frontend) foi criada.

## Contexto dos itens anteriores
Nos itens anteriores (40 e 41), foram desenvolvidos os domínios e UseCases de aprovação e reprovação de uma `InstanciaAprovacaoChamado` ou `EtapaAprovacaoChamado`. No item 43, desenvolvemos a área administrativa das configurações das regras do motor.

## Controller criado
Foi criado um novo controller dedicado chamado `AdminAprovacoesMotorController`, mantendo a separação conceitual do antigo controller de aprovação legada (`AdminAprovacaoChamadosController`). 

## Rotas criadas
- `POST /api/admin/aprovacoes-motor/aprovar`
- `POST /api/admin/aprovacoes-motor/reprovar`

## Endpoint de aprovação
Recebe a requisição via `POST` e invoca o `IAprovarAprovacaoChamadoUseCase`.

## Endpoint de rejeição/reprovação
Recebe a requisição via `POST` e invoca o `IReprovarAprovacaoChamadoUseCase`.

## Contratos reutilizados
Reutilizamos as classes já definidas em `AdminDecisaoAprovacaoChamadoDtos.cs`:
- `AprovarAprovacaoChamadoRequest`
- `ReprovarAprovacaoChamadoRequest`

## Use cases reutilizados
- `IAprovarAprovacaoChamadoUseCase`
- `IReprovarAprovacaoChamadoUseCase`

## Padrão de autorização aplicado
A classe de controller utiliza a policy `AdminOuAtendente` para autorização base. Os métodos utilizam as permissões granulares:
- `/aprovar` -> `PermissionPolicies.AprovacaoChamadosAprovar`
- `/reprovar` -> `PermissionPolicies.AprovacaoChamadosReprovar`

## Padrão de retorno aplicado
Foi utilizado o padrão de envelope `ExecutarAsync` implementado de maneira similar aos outros controllers do sistema. Esse padrão executa a task e devolve um HTTP 200 OK em caso de sucesso.

## Tratamento de erro
Foi utilizado o padrão existente para capturar:
- `UnauthorizedAccessException` -> 403 Forbid
- `KeyNotFoundException` -> 404 Not Found
- `ArgumentException` e `InvalidOperationException` -> 400 Bad Request
Erros de validação estrutural do FluentValidation (que enviam `ValidationException`) são tratados globalmente pelo middleware do projeto.

## Aprovação por instância
Permitida, basta preencher `InstanciaAprovacaoChamadoId` no `AprovarAprovacaoChamadoRequest` e omitir a EtapaId.

## Aprovação por etapa
Permitida, bastando preencher `EtapaAprovacaoChamadoId` no respectivo request.

## Reprovação por instância
Permitida com o preenchimento de `InstanciaAprovacaoChamadoId` e obrigatoriedade da `Justificativa`.

## Reprovação por etapa
Permitida também através de `EtapaAprovacaoChamadoId`.

## Relações com os Modelos e Casos de Uso
- **Relação com `AprovarAprovacaoChamadoUseCase`**: Acionado pela rota `/aprovar`.
- **Relação com `ReprovarAprovacaoChamadoUseCase`**: Acionado pela rota `/reprovar`.
- **Relação com `DecisaoAprovacaoChamado`**: Uma nova decisão será persistida internamente pelos UseCases.
- **Relação com `InstanciaAprovacaoChamado`**: O alvo das decisões, tendo seu status alterado para `Aprovada` ou `Reprovada` de acordo.
- **Relação com `EtapaAprovacaoChamado`**: Alternativa para uma decisão granular em níveis específicos do fluxo.
- **Relação com `AprovacaoChamado` legado**: A API antiga não foi afetada; esse controller gerencia exclusivamente o novo motor.
- **Relação com bloqueio do item 39**: Chamados bloqueados aguardando decisão serão potencialmente liberados pela execução destes endpoints operacionais.
- **Relação com endpoints administrativos do item 43**: Não interferem entre si; as regras não mudam aqui, apenas as instâncias vivas são gerenciadas.

## Relações Futuras
- **Relação com pendências do item 45**: As decisões que são realizadas aqui retirarão as instâncias pendentes do futuro painel de aprovadores (item 45).
- **Relação com frontend**: Estas rotas alimentarão os botões de ação ("Aprovar" / "Rejeitar") na interface de visualização do chamado para o aprovador.

## O que os endpoints não fazem
- Eles não administram políticas/regras.
- Eles não cancelam instâncias, expiram-nas ou solicitam reavaliações.
- Não alteram SLA e não possuem relação direta com a rotina principal de Help Desk / Abertura de chamados a não ser por eventos disparados.

## Testes criados ou executados
- Executados `RoadmapSprint4MotorAprovacoesChecklistTests` que validam as alterações de roadmap (agora em 65%, Item 44 concluído).
- Testes dos casos de uso de aprovação e reprovação foram executados novamente.

## Riscos de segurança e governança
O endpoint `/reprovar` ou `/aprovar` não exige estritamente um token do aprovador nominado, confiando na checagem de privilégio/policy `AprovacaoChamadosAprovar` geral. Futuras regras avançadas de "aprovador específico" ou "delegado" deverão validar essa segurança diretamente nos UseCases (como já é feito pelo validator interno que checa a associação).

## Decisões adiadas para próximos itens
A injeção do Validator no Controller não foi feita propositalmente, delegando as proteções de modelo (FluentValidation) ao próprio Application/UseCase Layer para evitar código repetitivo que já é tratado globalmente.

## Conclusão técnica
A funcionalidade restrita foi isolada num endpoint novo, mantendo estabilidade para o projeto e permitindo escalar as interações de API do Motor independentes dos endpoints de configurações.

## Próxima etapa recomendada
Executar o item 45 do checklist: **Criar endpoints de consulta de pendências de aprovação**.
