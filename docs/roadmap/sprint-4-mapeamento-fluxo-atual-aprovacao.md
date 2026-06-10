# Sprint 4 - Mapeamento do Fluxo Atual de Aprovação

## 1. Objetivo do mapeamento

Mapear tecnicamente como o fluxo atual de aprovacao funciona dentro do SGX Sistema de Chamados, identificando onde a aprovacao e criada, registrada, consultada, decidida e usada no fluxo operacional atual dos chamados.

## 2. Limites desta etapa

- Esta etapa registra apenas mapeamento tecnico e documentacao.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao de dominio.
- Nao foram criados endpoints, controllers, telas ou services frontend novos.
- Nao houve implementacao funcional nova.
- Nao houve mudanca no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.

## 3. Arquivos analisados

- `src/SGX.SistemaChamado.Domain/Entities/AprovacaoChamado.cs`
- `src/SGX.SistemaChamado.Domain/Entities/Chamado.cs`
- `src/SGX.SistemaChamado.Domain/Enums/StatusAprovacaoChamado.cs`
- `src/SGX.SistemaChamado.Domain/Enums/TipoOrigemAprovacaoChamado.cs`
- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/AprovacaoChamadoConfiguration.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AprovacaoChamadosAdminUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ChamadoAprovacoesUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/AbrirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/ObterStatusAprovacaoChamadoPortalUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Portal/PortalUseCaseHelpers.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/AprovacaoChamadoHelper.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AlterarStatusChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoFilaAdminUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/EncerrarChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ReabrirChamadoUseCase.cs`
- `src/SGX.SistemaChamado.Application/Services/AcoesChamadoService.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminAprovacaoChamadosDtos.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminChamadoAprovacoesDtos.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminResponses.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Portal/PortalStatusAprovacaoChamadoDto.cs`
- `src/SGX.SistemaChamado.Application/Validators/AprovacaoChamadosValidators.cs`
- `src/SGX.SistemaChamado.Api/Controllers/AdminAprovacaoChamadosController.cs`
- `src/SGX.SistemaChamado.Api/Controllers/AdminChamadosController.cs`
- `src/SGX.SistemaChamado.Api/Controllers/PortalController.cs`
- `src/SGX.SistemaChamado.Web/src/services/aprovacaoChamadosAdminService.ts`
- `src/SGX.SistemaChamado.Web/src/types/aprovacaoChamados.ts`
- `src/SGX.SistemaChamado.Web/src/views/AprovacaoChamadosListPage.vue`
- `src/SGX.SistemaChamado.Web/src/views/AprovacaoChamadosDetalhePage.vue`
- `src/SGX.SistemaChamado.Web/src/views/AdminDetalheChamadoView.vue`
- `src/SGX.SistemaChamado.Web/src/views/PortalChamadosView.vue`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`
- `tests/SGX.SistemaChamado.Tests/AprovacaoChamadosAdminUseCasesTests.cs`
- `tests/SGX.SistemaChamado.Tests/AprovacaoChamadosEndpointsIntegrationTests.cs`
- `tests/SGX.SistemaChamado.Tests/AprovacaoChamadosAuthorizationIntegrationTests.cs`
- `tests/SGX.SistemaChamado.Tests/ChamadoAprovacaoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/ChamadoAprovacaoEndpointsIntegrationTests.cs`
- `tests/SGX.SistemaChamado.Tests/ObterStatusAprovacaoChamadoPortalUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/AbrirChamadoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/AlterarStatusChamadoUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs`
- `docs/APROVACAO-CHAMADOS.md`

## 4. Entidades relacionadas a aprovacao

### `AprovacaoChamado`

Entidade central do fluxo atual de aprovacao. Mantem:

- `ChamadoId`
- `Titulo`
- `Descricao`
- `Status`
- `TipoOrigem`
- `BloqueiaAvancoAtendimento`
- `OrigemDescricao`
- `SolicitanteId`
- `AprovadorId`
- `JustificativaSolicitacao`
- `JustificativaDecisao`
- `SolicitadaEm`
- `DecididaEm`
- `CanceladoEm`
- `CanceladoPorUsuarioId`
- `MotivoCancelamento`

Comportamentos principais:

- `Aprovar(...)`
- `Reprovar(...)`
- `Cancelar(...)`
- `CancelarVinculada(...)`
- `DefinirAprovador(...)`

### `Chamado`

Mantem vinculo direto com aprovacoes por meio da colecao:

- `ICollection<AprovacaoChamado> Aprovacoes`

Esse relacionamento e usado para:

- montar estado resumido da aprovacao do chamado;
- identificar aprovacao pendente bloqueante;
- expor sinalizadores no admin e no portal.

## 5. Enums relacionados a aprovacao

### `StatusAprovacaoChamado`

- `Pendente`
- `Aprovado`
- `Reprovado`
- `Cancelado`

### `TipoOrigemAprovacaoChamado`

- `Manual`
- `CatalogoServico`
- `Categoria`
- `Departamento`
- `RegraAdministrativa`

## 6. Use cases e services relacionados

### Fluxo administrativo principal

- `AprovacaoChamadosAdminUseCases`
  - lista aprovacoes;
  - detalha aprovacao;
  - solicita aprovacao manual/administrativa;
  - aprova;
  - reprova;
  - cancela.

### Fluxo de aprovacoes vinculadas ao detalhe do chamado

- `ChamadoAprovacoesUseCases`
  - cria aprovacao vinculada;
  - lista aprovacoes do chamado;
  - aprova;
  - reprova;
  - cancela;
  - consulta pendencia simples e pendencia bloqueante.

Observacao tecnica relevante:

- nesse fluxo vinculado, a criacao usa `bloqueiaAvancoAtendimento: false`;
- portanto ele registra aprovacoes no chamado, mas nao bloqueia o atendimento por padrao.

### Fluxo portal

- `ObterStatusAprovacaoChamadoPortalUseCase`
  - carrega o chamado com aprovacoes;
  - valida se o solicitante pode ver o chamado;
  - retorna DTO orientativo de status de aprovacao.

### Service/helper transversal

- `AprovacaoChamadoHelper`
  - consolida o estado atual da aprovacao do chamado;
  - identifica se ha aprovacao bloqueante pendente;
  - gera mensagens orientativas para portal e bloqueio operacional.

- `PortalUseCaseHelpers`
  - transforma o estado calculado em DTO para portal.

- `AcoesChamadoService`
  - usa o estado de aprovacao para permitir ou bloquear acoes como assumir e reabrir.

## 7. Controllers e endpoints existentes

### Modulo administrativo principal de aprovacao

- `GET /api/admin/aprovacao-chamados`
- `GET /api/admin/aprovacao-chamados/{id}`
- `POST /api/admin/chamados/{chamadoId}/aprovacao/solicitar`
- `POST /api/admin/aprovacao-chamados/{id}/aprovar`
- `POST /api/admin/aprovacao-chamados/{id}/reprovar`
- `POST /api/admin/aprovacao-chamados/{id}/cancelar`

Controller:

- `AdminAprovacaoChamadosController`

### Aprovacoes vinculadas ao detalhe do chamado

- `GET /api/admin/chamados/{chamadoId}/aprovacoes`
- `POST /api/admin/chamados/{chamadoId}/aprovacoes`
- `POST /api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoId}/aprovar`
- `POST /api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoId}/reprovar`
- `DELETE /api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoId}`

Controller:

- `AdminChamadosController`

### Consulta portal

- `GET /api/portal/chamados/{chamadoId}/aprovacao`

Controller:

- `PortalController`

## 8. DTOs e contratos existentes

### Admin - modulo principal

- `AprovacaoChamadoListagemDto`
- `AprovacaoChamadoDetalheDto`
- `SolicitarAprovacaoChamadoRequest`
- `DecidirAprovacaoChamadoRequest`
- `CancelarAprovacaoChamadoRequest`
- `FiltroAprovacaoChamadoRequest`

### Admin - vinculadas ao chamado

- `CriarChamadoAprovacaoAdminRequest`
- `DecidirChamadoAprovacaoAdminRequest`
- `CancelarChamadoAprovacaoAdminRequest`
- `ChamadoAprovacaoAdminResponse`

### Portal

- `PortalStatusAprovacaoChamadoDto`

### DTOs que ja expoem sinalizadores de aprovacao no chamado

Admin e portal ja expoem campos de resumo do estado atual:

- `RequerAprovacao`
- `AprovacaoPendente`
- `StatusAprovacao`
- `AprovacaoChamadoId`

## 9. Fluxo atual de solicitacao de aprovacao

O fluxo atual possui duas entradas principais.

### 9.1 Solicitacao administrativa manual

Origem:

- `AdminAprovacaoChamadosController`
- `AprovacaoChamadosAdminUseCases.SolicitarAsync`

Comportamento:

1. Admin/atendente solicita aprovacao para um chamado existente.
2. O use case impede duplicidade de aprovacao pendente ativa para o mesmo chamado.
3. Uma `AprovacaoChamado` e criada com status `Pendente`.
4. O chamado recebe historico `TipoHistoricoChamado.AprovacaoSolicitada`.
5. Auditoria registra a solicitacao quando o servico de auditoria esta ativo.

Caracteristica importante:

- nesse fluxo, a aprovacao criada usa o construtor padrao, mantendo `BloqueiaAvancoAtendimento = true`.

### 9.2 Solicitacao automatica na abertura via catalogo

Origem:

- `AbrirChamadoUseCase`

Condicao:

- o servico de catalogo possui `RequerAprovacao = true`.

Comportamento:

1. O chamado e aberto normalmente.
2. O use case verifica se ja existe aprovacao pendente ativa para o chamado.
3. Nao existindo, cria `AprovacaoChamado` com `TipoOrigem = CatalogoServico`.
4. `OrigemDescricao` recebe o nome do servico.
5. `JustificativaSolicitacao` recebe a justificativa padrao de aprovacao por catalogo.
6. O historico do chamado registra `AprovacaoSolicitada`.
7. Auditoria registra a criacao automatica da aprovacao.

Caracteristica importante:

- essa aprovacao automatica e bloqueante por padrao.

### 9.3 Criacao de aprovacao vinculada ao detalhe do chamado

Origem:

- `AdminChamadosController`
- `ChamadoAprovacoesUseCases.CriarAsync`

Comportamento:

1. Admin/atendente cria uma aprovacao vinculada informando titulo e dados opcionais.
2. O registro e salvo em `AprovacoesChamado`.
3. O historico do chamado registra `TipoHistoricoChamado.AprovacaoCriada`.

Caracteristica importante:

- esse fluxo cria a aprovacao com `bloqueiaAvancoAtendimento: false`;
- por isso, ele nao atua como bloqueio operacional do atendimento.

## 10. Fluxo atual de decisao/aprovacao

### Modulo principal

Origem:

- `POST /api/admin/aprovacao-chamados/{id}/aprovar`
- `AprovacaoChamadosAdminUseCases.AprovarAsync`

Comportamento:

1. Carrega a aprovacao.
2. Garante que o status atual e `Pendente`.
3. Registra aprovador, justificativa opcional e `DecididaEm`.
4. Muda o status para `Aprovado`.
5. Grava historico do chamado com `TipoHistoricoChamado.ChamadoAprovado`.
6. Registra auditoria da decisao, quando habilitada.

### Aprovacao vinculada ao chamado

Origem:

- `POST /api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoId}/aprovar`
- `ChamadoAprovacoesUseCases.AprovarAsync`

Comportamento:

1. Valida que a aprovacao pertence ao chamado informado.
2. Exige que a aprovacao esteja pendente.
3. Muda o status para `Aprovado`.
4. Registra historico `TipoHistoricoChamado.AprovacaoAprovada`.

## 11. Fluxo atual de rejeicao

### Modulo principal

Origem:

- `POST /api/admin/aprovacao-chamados/{id}/reprovar`
- `AprovacaoChamadosAdminUseCases.ReprovarAsync`

Comportamento:

1. Exige justificativa de reprovacao.
2. Garante status atual `Pendente`.
3. Muda o status para `Reprovado`.
4. Preenche justificativa e data de decisao.
5. Registra historico do chamado com `TipoHistoricoChamado.ChamadoReprovado`.
6. Registra auditoria da decisao, quando habilitada.

### Aprovacao vinculada ao chamado

Origem:

- `POST /api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoId}/reprovar`
- `ChamadoAprovacoesUseCases.ReprovarAsync`

Comportamento:

1. Valida vinculo com o chamado.
2. Exige pendencia.
3. Muda o status para `Reprovado`.
4. Registra historico `TipoHistoricoChamado.AprovacaoReprovada`.

## 12. Relacao entre aprovacao e chamado

Existe vinculo direto e persistente:

- `AprovacaoChamado.ChamadoId`
- `Chamado.Aprovacoes`

Configuracao de persistencia:

- tabela `aprovacoes_chamado`;
- foreign key de `ChamadoId` com `DeleteBehavior.Restrict`;
- indice composto `ChamadoId + Ativo + Status`.

Conclusao:

- o modelo atual ja suporta multiplas aprovacoes por chamado;
- o estado resumido do chamado e calculado sobre a colecao de aprovacoes ativas.

## 13. Impacto atual da aprovacao no status e na situacao do chamado

O impacto atual nao altera diretamente o `StatusChamado` para um status proprio de aprovacao. Em vez disso, o sistema:

- mantem aprovacao em entidade separada;
- calcula estado resumido do chamado via helper;
- exibe sinalizadores de aprovacao no admin e no portal;
- bloqueia algumas operacoes quando existe aprovacao pendente bloqueante.

Campos e sinais afetados no chamado:

- `RequerAprovacao`
- `AprovacaoPendente`
- `StatusAprovacao`
- `AprovacaoChamadoId`
- `MensagemOrientativa`

Operacoes atualmente bloqueadas quando ha aprovacao pendente bloqueante:

- assumir chamado;
- assumir chamado da fila;
- reabrir chamado;
- alterar para status final ou operacionalmente conclusivo;
- encerrar chamado.

Comportamento quando reprovado:

- o helper retorna estado `Reprovado`;
- nao ha bloqueio generico em todas as operacoes baseado apenas em reprovacao;
- o foco atual do bloqueio operacional esta em `Pendente` com `BloqueiaAvancoAtendimento = true`.

## 14. Pontos onde o chamado ainda pode seguir mesmo com aprovacao pendente

O fluxo atual nao bloqueia tudo.

Casos identificados:

- `AlterarStatusChamadoUseCase` permite mudanca para status intermediario mesmo com aprovacao pendente bloqueante.
- `ChamadoAprovacoesUseCases` cria aprovacoes vinculadas com `BloqueiaAvancoAtendimento = false`, entao essas aprovacoes nao bloqueiam o atendimento.
- o estado de aprovacao do chamado e resumido a partir de aprovacoes ativas e bloqueantes; aprovacoes nao bloqueantes podem coexistir sem travar o fluxo.

Em outras palavras:

- aprovacao pendente bloqueante hoje impede avancos finais e certas acoes operacionais;
- ela ainda nao representa um travamento universal de todo o ciclo do chamado.

## 15. Testes existentes identificados

### Use cases administrativos principais

- `AprovacaoChamadosAdminUseCasesTests`
  - solicita aprovacao;
  - impede pendencia duplicada;
  - aprova;
  - reprova;
  - exige justificativa em reprovacao;
  - cancela;
  - exige justificativa em cancelamento;
  - registra historicos.

### Aprovacoes vinculadas ao chamado

- `ChamadoAprovacaoUseCaseTests`
  - cria aprovacao vinculada;
  - permite mais de uma pendencia vinculada;
  - aprova, reprova e cancela;
  - registra historicos;
  - valida permissao admin/atendente.

- `ChamadoAprovacaoEndpointsIntegrationTests`
  - cobre endpoints de criacao, listagem, aprovacao, reprovacao e cancelamento;
  - valida isolamento entre chamados;
  - valida respostas 400/404 e bloqueio sem autenticacao.

### Endpoints e autorizacao do modulo principal

- `AprovacaoChamadosEndpointsIntegrationTests`
  - cobre fluxo administrativo solicitar, listar, detalhar e aprovar.

- `AprovacaoChamadosAuthorizationIntegrationTests`
  - valida politicas `Visualizar`, `Gerenciar`, `Aprovar`, `Reprovar` e `Cancelar`.

### Portal

- `ObterStatusAprovacaoChamadoPortalUseCaseTests`
  - retorno pendente;
  - retorno sem aprovacao;
  - retorno reprovado com justificativa;
  - bloqueio de acesso a chamado de outro solicitante.

- `ApiHttpIntegrationTests`
  - cobre endpoint portal de status de aprovacao.

### Abertura automatica por catalogo

- `AbrirChamadoUseCaseTests`
  - cria aprovacao automatica quando `RequerAprovacao = true`;
  - nao cria quando `RequerAprovacao = false`.

### Bloqueio operacional parcial

- `AlterarStatusChamadoUseCaseTests`
  - pendencia bloqueante nao impede status intermediario;
  - impede status final;
  - pendencia nao bloqueante nao impede status final;
  - fluxo segue apos aprovacao, reprovacao e cancelamento.

### Frontend

- `AprovacaoChamadosListPage.spec.ts`
- `AprovacaoChamadosDetalhePage.spec.ts`
- `AdminDetalheChamadoView.aprovacao.spec.ts`
- `PortalChamadosView.aprovacao.spec.ts`
- `DetalheChamadoView.aprovacao.spec.ts`
- `aprovacaoChamadosAdminService.spec.ts`

## 16. Lacunas encontradas

- Nao existe motor multi-nivel.
- Nao existe aprovacao por grupo aprovador.
- Nao existe aprovador padrao por regra.
- Nao existe regra reutilizavel por natureza ITSM, tipo de chamado, custo, risco ou impacto.
- Nao existe bloqueio universal do chamado por qualquer aprovacao pendente; hoje o bloqueio depende de `BloqueiaAvancoAtendimento`.
- Nao existe status proprio do chamado para "aguardando aprovacao"; o estado e derivado da entidade de aprovacao.
- Existem dois fluxos coexistentes:
  - modulo principal de aprovacao do chamado;
  - aprovacoes vinculadas ao detalhe do chamado.
- Esses dois fluxos compartilham a mesma entidade, mas nao representam ainda um motor unificado.

## 17. Conclusao tecnica

O SGX ja possui uma base funcional consistente de aprovacao, com:

- entidade dedicada;
- vinculo direto com chamado;
- criacao manual e automatica;
- consulta administrativa e portal;
- decisao de aprovacao, reprovacao e cancelamento;
- historico do chamado;
- auditoria;
- sinalizadores expostos em DTOs e frontend;
- bloqueio operacional parcial baseado em pendencia bloqueante.

Tecnicamente, o fluxo atual ja resolve cenarios de aprovacao linear simples, mas ainda nao se comporta como motor ITSM reutilizavel. O comportamento atual esta centrado em aprovacoes pontuais e em um bloqueio operacional parcial, sem camada de configuracao por regra, grupo ou multiplos niveis.

## 18. Proxima etapa recomendada

Executar o item 4 do checklist:

- mapear os pontos onde o chamado deve ficar bloqueado por aprovacao pendente,
- distinguindo o que hoje ja e bloqueado,
- o que ainda passa com pendencia,
- e quais operacoes precisarao ser absorvidas pelo futuro motor de aprovacao ITSM.
