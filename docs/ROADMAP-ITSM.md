# Roadmap ITSM - SGX Sistema de Chamados

## Objetivo do Roadmap ITSM

O Roadmap ITSM organiza a evolucao funcional e tecnica do SGX Sistema de Chamados com foco em governanca, previsibilidade e rastreabilidade de entrega.

## Atualizacao 2026-05-26 - Novo Roadmap ITIL (20 sprints)

A partir desta data, o roadmap administrativo passa a exibir uma trilha canonica de 20 sprints ITIL/ITSM na categoria `ITIL/ITSM`, mantendo os itens antigos como historico inativo.

Diretriz central da trilha:
- todo chamado deve possuir natureza ITSM obrigatoria;
- a natureza deve influenciar fluxo, status, campos obrigatorios, SLA, prioridade, impacto, urgencia, aprovacoes, permissoes, notificacoes, auditoria e relatorios.

Sprints consolidadas:
1. Sprint 1 - Fundacao ITSM do chamado (100% - Implementado e validado)
2. Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM (25% - Planejado)
3. Sprint 3 - Grupos tecnicos, filas e atribuicao (90% - Implementado funcionalmente, pendente apenas homologacao)
4. Sprint 4 - Motor de Aprovacoes ITSM (94% - Implementado funcionalmente, pendente apenas homologacao)
5. Sprint 5 - Regras de fechamento, aceite e reabertura (100% - Implementado funcionalmente)
6. Sprint 6 - Notificacoes ITSM (94% - Em desenvolvimento)
7. Sprint 7 - Gerenciamento de Requisicoes (100% - Implementado funcionalmente)
8. Sprint 8 - Catalogo de Servicos 2.0 (96% - Em desenvolvimento)
9. Sprint 9 - Gerenciamento de Incidentes (46% - Em desenvolvimento)
10. Sprint 10 - Gerenciamento de Mudancas (25% - Nao iniciado)
11. Sprint 11 - Gerenciamento de Problemas (25% - Planejado)
12. Sprint 12 - CMDB e Itens de Configuracao (50% - Em desenvolvimento)
13. Sprint 13 - Analise de impacto (25% - Planejado)
14. Sprint 14 - SLA 2.0, OLA e matriz impacto x urgencia (50% - Implementado funcionalmente)
15. Sprint 15 - Observadores de chamados (25% - Nao iniciado)
16. Sprint 16 - Monitoramento, eventos e Zabbix (25% - Planejado)
17. Sprint 17 - Relatorios ITSM avancados (50% - Implementado funcionalmente)
18. Sprint 18 - Base de Conhecimento 2.0 (50% - Implementado funcionalmente)
19. Sprint 19 - Pesquisa de satisfacao (25% - Nao iniciado)
20. Sprint 20 - Homologacao institucional ITSM (75% - Em homologacao)
21. Sprint 21 - Produto, implantacao e operacao (25% - Em desenvolvimento)

Observacao:
- os percentuais seguem a referencia operacional: nao iniciado (0%), parcial (30% a 60%), evolucao planejada (0% a 20%), homologacao preparada (80% a 95%).
- Sprint 5 foi encerrada tecnicamente com checklist 32/32; a homologacao formal permanece registrada como etapa posterior.
- Sprint 9 foi sincronizada com checklist tecnico de 50 itens; 23 estao concluidos e 27 permanecem pendentes para a evolucao funcional incremental.

## Atualizacao 2026-07-12 - Sprint 9 Gerenciamento de Incidentes - Checklist tecnico consolidado

- Percentual recalculado para `46%` com base em `23` itens concluidos e `27` pendentes.
- O checklist foi reorganizado em trilhas tecnicas de planejamento, modelagem, backend, API, frontend, testes, seguranca, governanca, documentacao e homologacao.
- Os itens concluidos refletem somente evidencias reais ja existentes em codigo, testes e documentacao, sem implementar funcionalidade nova de incidente.
- O item `Definir fluxo-alvo ponta a ponta do incidente` foi concluido com evidencia em `docs/sprint-9-fluxo-alvo-incidente.md`.
- Proxima acao registrada: `Modelar dados operacionais especificos do incidente: servico afetado, CI afetado, causa provavel e solucao de contorno.`
- A entrega continua sem funcionalidade nova de incidente, sem alteracao no fluxo legado e sem impacto na Sprint 8.
- A documentacao tecnica, a seed e o teste de checklist foram sincronizados para refletir o novo estado.

### Quadro resumido da ordem atual

| Sprint | Area | Status | Avaliacao | Progresso |
|---|---|---|---|---|
| Sprint 1 - Fundacao ITSM do chamado | ITIL/ITSM | Implementado e validado | Concluido | 100% |
| Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM | ITIL/ITSM | Planejado | Nao avaliado | 25% |
| Sprint 3 - Grupos tecnicos, filas e atribuicao | ITIL/ITSM | Planejado | Nao avaliado | 90% |
| Sprint 4 - Motor de Aprovacoes ITSM | ITIL/ITSM | Implementado funcionalmente | Completo com pendencias evolutivas | 94% |
| Sprint 5 - Regras de fechamento, aceite e reabertura | ITIL/ITSM | Implementado funcionalmente | Completo com pendencias evolutivas | 100% |
| Sprint 6 - Notificacoes ITSM | ITIL/ITSM | Em desenvolvimento | Bloqueado | 94% |
| Sprint 7 - Gerenciamento de Requisicoes | ITIL/ITSM | Implementado funcionalmente | Completo | 100% |
| Sprint 8 - Catalogo de Servicos 2.0 | ITIL/ITSM | Em desenvolvimento | Parcial | 96% |

## Atualizacao 2026-07-07 - Sprint 8 Catalogo de Servicos 2.0 - Item 66

- Percentual recalculado para `96%` com base em `73` itens concluidos e `3` pendentes.
- O item `66` foi concluido consolidando a cobertura de seguranca do formulario dinamico e das respostas na abertura guiada.
- A regressao cobre campos inexistentes, de outro servico, de outra versao, inativos, invisiveis, opcoes inexistentes, opcoes inativas e tentativa de manipulacao de metadados do chamado.
- Tambem foi validado que payload invalido nao cria chamado, nao persiste respostas e nao expõe valores na auditoria tecnica.
- Nenhuma funcionalidade nova fora desse escopo foi criada nesta etapa.
- Os proximos itens pendentes reais registrados no checklist consolidado passam a ser `74`, `75` e `76`.

## Atualizacao 2026-07-07 - Sprint 8 Catalogo de Servicos 2.0 - Documento de homologacao funcional

- Foi criado o documento [docs/homologacao/sprint-8-homologacao-funcional.md](/c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/docs/homologacao/sprint-8-homologacao-funcional.md) para consolidar a homologacao funcional da Sprint 8 em formato formal.
- O documento organiza identificacao, objetivo, cenarios obrigatorios, resultado geral, pendencias e campos de responsabilidade.
- O artefato ainda aguarda preenchimento das evidencias reais e aprovacao dos responsaveis; por isso, o item `74` permanece pendente.
- O progresso da Sprint 8 permanece em `96%`, sem alteracao de checklist nesta etapa.

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 65

- Percentual recalculado para `95%` com base em `72` itens concluidos e `4` pendentes.
- O item `65` foi concluido garantindo que o solicitante so consegue enviar respostas permitidas para o formulario aplicavel ao servico.
- O backend agora valida se `SelecaoUnica` usa apenas `Valor` presente em opcao ativa do campo e se `SelecaoMultipla` usa apenas itens de `Valores` presentes em opcoes ativas do proprio campo.
- Opcoes inexistentes ou inativas sao rejeitadas antes da persistencia, preservando a integridade da abertura guiada.
- Nenhuma funcionalidade nova fora desse escopo foi criada nesta etapa.
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `66` (`Testar seguranca do formulario e respostas`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 64

- Percentual recalculado para `93%` com base em `71` itens concluidos e `5` pendentes.
- O item `64` foi concluido garantindo que o solicitante nao consegue manipular grupo tecnico, SLA, aprovacao ou classificacao operacional na abertura guiada.
- O contrato publico dedicado de abertura guiada continua sem expor `NaturezaChamado`, `CategoriaId`, `SubcategoriaId`, `PrioridadeId`, `GrupoTecnicoId`, `SlaId` ou campos de aprovacao.
- Os testes validam que o backend e o catalogo permanecem como fonte de verdade, mesmo quando campos sensiveis sao enviados indevidamente no JSON.
- Nenhuma funcionalidade nova fora desse escopo foi criada nesta etapa.
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `65` (`Garantir que solicitante so envie respostas permitidas para o servico`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 55

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 63

- Percentual recalculado para `92%` com base em `70` itens concluidos e `6` pendentes.
- O item `63` foi concluido garantindo autorizacao para manutencao administrativa do formulario do servico.
- A leitura administrativa de formulario permanece disponivel para `Administrador` e `Atendente`, sem alterar o fluxo operacional de consulta.
- As operacoes de criacao, atualizacao, inativacao e reativacao de formulario, versao, campo e opcao agora exigem `Administrador` tambem na API e na camada de aplicacao.
- Nenhuma funcionalidade nova fora desse escopo foi criada nesta etapa.
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `64` (`Garantir que solicitante nao manipule grupo, SLA, aprovacao ou classificacao`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 55

- Percentual recalculado para `91%` com base em `69` itens concluidos e `7` pendentes.
- O item `55` foi concluido garantindo a aplicacao de tipo, categoria, subcategoria e prioridade do catalogo durante a abertura guiada.
- A abertura com `CatalogoServicoId` passa a forcar `NaturezaChamadoEnum.Requisicao` e a usar a prioridade padrao do servico quando ela estiver configurada.
- Quando o catalogo nao define categoria, subcategoria ou prioridade, o fallback legado permanece ativo.
- Nenhuma funcionalidade nova fora desse escopo foi criada nesta etapa.
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `63` (`Garantir autorizacao para manutencao administrativa do formulario`), porque os itens `56` a `62` ja estavam concluidos no seed anterior.

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 54

- Percentual recalculado para `89%` com base em `68` itens concluidos e `8` pendentes.
- O item `54` foi concluido exclusivamente com o reforco dos testes de exibicao das respostas do formulario na area administrativa de atendimento.
- Os testes validam resposta simples, resposta multipla, rotulo, tipo do campo conforme o layout atual e preservacao da ordem recebida.
- A tela continua usando apenas o endpoint existente de detalhe administrativo, sem endpoint novo para respostas do formulario.
- Nenhuma funcionalidade nova foi criada nesta etapa.
- O proximo item pendente da Sprint 8 passa a ser o item `55` (`Garantir aplicacao de tipo, categoria, subcategoria e prioridade do catalogo`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 53

- Percentual recalculado para `88%` com base em `67` itens concluidos e `9` pendentes.
- O item `53` foi concluido exclusivamente com o reforco dos testes de exibicao das respostas do formulario no portal do solicitante.
- Os testes validam resposta simples, resposta multipla, rotulo, tipo do campo conforme o layout atual e preservacao da ordem recebida.
- A tela continua usando apenas o endpoint existente de detalhe do chamado, sem endpoint novo para respostas do formulario.
- Nenhuma funcionalidade nova foi criada nesta etapa.
- O proximo item pendente da Sprint 8 passa a ser o item `54` (`Testar exibicao das respostas no atendimento administrativo`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 52

- Percentual recalculado para `87%` com base em `66` itens concluidos e `10` pendentes.
- O item `52` foi concluido exclusivamente com o reforco dos testes de persistencia das respostas do formulario na abertura guiada.
- Os testes validam `Valor`, `ValoresJson`, vinculos com `ChamadoId`, `FormularioServicoVersaoId` e `CampoFormularioServicoId`.
- O payload invalido continua sem persistir chamado nem respostas, e a auditoria tecnica do item `51` permanece sem expor valores das respostas.
- Nenhuma funcionalidade nova foi criada nesta etapa.
- O proximo item pendente da Sprint 8 passa a ser o item `53` (`Testar exibicao das respostas no portal`).

## Atualizacao 2026-07-06 - Sprint 8 Catalogo de Servicos 2.0 - Item 51

- Percentual recalculado para `86%` com base em `65` itens concluidos e `11` pendentes.
- O item `51` foi concluido com a auditoria tecnica especifica das respostas persistidas na abertura guiada por catalogo.
- A auditoria registra `ChamadoId`, `FormularioServicoVersaoId`, quantidade de respostas persistidas e origem `AberturaGuiadaCatalogo`, sem gravar valores das respostas.
- O historico funcional resumido do item `50` permanece ativo e separado da trilha tecnica.
- O proximo item pendente da Sprint 8 passa a ser o item `52` (`Testar persistencia das respostas do formulario`).
| Sprint 9 - Gerenciamento de Incidentes | ITIL/ITSM | Em desenvolvimento | Parcial | 46% |
| Sprint 10 - Gerenciamento de Mudancas | ITIL/ITSM | Nao iniciado | Nao avaliado | 25% |
| Sprint 11 - Gerenciamento de Problemas | ITIL/ITSM | Planejado | Nao avaliado | 25% |
| Sprint 12 - CMDB e Itens de Configuracao | ITIL/ITSM | Em desenvolvimento | Parcial | 50% |
| Sprint 13 - Analise de impacto | ITIL/ITSM | Planejado | Nao avaliado | 25% |
| Sprint 14 - SLA 2.0, OLA e matriz impacto x urgencia | ITIL/ITSM | Implementado funcionalmente | Completo com pendencias evolutivas | 50% |
| Sprint 15 - Observadores de chamados | ITIL/ITSM | Nao iniciado | Nao avaliado | 25% |
| Sprint 16 - Monitoramento, eventos e Zabbix | ITIL/ITSM | Planejado | Nao avaliado | 25% |
| Sprint 17 - Relatorios ITSM avancados | ITIL/ITSM | Implementado funcionalmente | Completo com pendencias evolutivas | 50% |
| Sprint 18 - Base de Conhecimento 2.0 | ITIL/ITSM | Implementado funcionalmente | Completo com pendencias evolutivas | 50% |
| Sprint 19 - Pesquisa de satisfacao | ITIL/ITSM | Nao iniciado | Nao avaliado | 25% |
| Sprint 20 - Homologacao institucional ITSM | ITIL/ITSM | Em homologacao | Homologacao funcional preparada | 75% |
| Sprint 21 - Produto, implantacao e operacao | ITIL/ITSM | Em desenvolvimento | Parcial | 25% |

## Atualizacao 2026-07-03 - Sprint 8 Catalogo de Servicos 2.0 - Item 49

- Percentual recalculado para `83%` com base em `63` itens concluidos e `13` pendentes.
- O item `49` foi concluido com a exibicao das respostas persistidas do formulario na area administrativa de atendimento.
- A tela administrativa agora mostra `Rotulo`, `Tipo`, `Valor` e `Valores`, preservando a ordem recebida do contrato ja existente.
- Nenhum backend funcional novo foi criado nesta etapa e nao houve alteracao na persistencia das respostas.
- Auditoria e historico especifico dessas respostas ainda nao existem.
- O proximo item pendente da Sprint 8 passa a ser o item `50` (`Registrar historico da abertura com formulario preenchido`).

## Atualizacao 2026-07-02 - Sprint 8 Catalogo de Servicos 2.0 - Item 44

- Percentual recalculado para `76%` com base em `58` itens concluidos e `18` pendentes.
- O item `44` foi concluido com a configuracao explicita do EF Core para `RespostaFormularioChamado`.
- Os testes agora validam metadata da entidade, tabela, FKs, `DeleteBehavior.Restrict`, limites e indices.
- A migration estrutural da tabela de respostas permanece reservada para o item `45`.
- As respostas ainda nao sao persistidas na abertura guiada e ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `45` (`Criar migration estrutural para respostas do formulario`).

## Atualizacao 2026-07-02 - Sprint 8 Catalogo de Servicos 2.0 - Item 43

- Percentual recalculado para `75%` com base em `57` itens concluidos e `19` pendentes.
- O item `43` foi concluido com a modelagem estrutural da entidade de dominio para respostas do formulario no chamado.
- EF Core e migration estrutural da tabela de respostas permanecem pendentes para os itens `44` e `45`.
- As respostas ainda nao sao persistidas na abertura guiada e ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `44` (`Configurar EF Core para respostas do formulario`).

## Atualizacao 2026-07-02 - Sprint 8 Catalogo de Servicos 2.0

- Percentual recalculado para `74%` com base em `56` itens concluidos e `20` pendentes.
- O item `42` foi concluido com cobertura automatizada da abertura guiada sem formulario configurado, incluindo `GET` com `Formulario = null`, `POST` com sucesso sem respostas, rejeicao de respostas preenchidas e limpeza de respostas antigas no frontend.
- As respostas do formulario continuam sem persistencia nesta etapa.
- O proximo item pendente da Sprint 8 passa a ser o item `43` (`Modelar persistencia das respostas do formulario no chamado`).

## Atualizacao 2026-06-27 - Sprint 8 Catalogo de Servicos 2.0

- Checklist expandido de `4` para `76` itens rastreaveis.
- Percentual recalculado para `72%` com base em `55` itens concluidos e `21` pendentes.
- Item `23` concluido com a auditoria das migrations estruturais do formulario dinamico e comprovacao de separacao entre estrutura e checklist.
- Item `24` concluido com os contratos administrativos de formulario, versao, campo e opcao, sem validacao administrativa ou API nesta etapa.
- Item `25` concluido com validators administrativos contratuais, preservando a ausencia de endpoints, frontend e regras relacionais nesta etapa.
- Item `26` concluido com use cases administrativos para formulario, versao, campo e opcao, preservando a ausencia de endpoints e frontend nesta etapa.
- Item `27` concluido com endpoints administrativos para formulario, versao, campo e opcao, preservando a ausencia de frontend e de uso funcional do formulario na abertura guiada.
- Item `28` concluido com o frontend administrativo do catalogo para configurar formulario, versoes, campos e opcoes no detalhe do servico.
- Item `29` concluido com a consolidacao dos testes automatizados da configuracao administrativa de formulario por servico.
- O portal do solicitante ja renderiza formulario dinamico, a abertura guiada agora envia respostas e a persistencia dessas respostas continua pendente.
- O item `30` concluiu a exposicao dos metadados do formulario no endpoint de preparacao da abertura, mantendo a abertura sem respostas, sem validacao dinamica e sem persistencia.
- O item `33` concluiu a validacao de obrigatoriedade dos campos ativos e visiveis no backend, sem validacao de tipo/formato, opcoes permitidas ou persistencia das respostas.
- O item `34` concluiu a validacao de tipos e formatos das respostas no backend, sem persistencia, sem validacao completa de campo indevido e sem validar pertencimento das opcoes.
- O item `35` concluiu a validacao de escopo dos campos respondidos, rejeitando ids inexistentes, de outra versao, de outro formulario/servico e campos inativos ou invisiveis.
- Item `36` concluido com regressao dedicada para abertura guiada sem formulario e rejeicao de respostas preenchidas fora desse contexto.
- Item `37` concluido com a renderizacao dinamica do formulario no portal.
- Item `38` concluido com o envio das respostas do formulario no request de abertura guiada, ainda sem persistencia.
- Item `39` concluido com a cobertura automatizada do fluxo valido de abertura guiada com formulario.
- O item `41` foi concluido com cobertura automatizada da rejeicao de respostas invalidas na abertura guiada.
- Os itens `61` e `62` tambem foram sincronizados no checklist por ja possuirem evidencia automatizada preexistente de regressao, sem funcionalidade nova nesta entrega.
- O item `42` foi concluido com cobertura automatizada da abertura guiada sem formulario configurado, incluindo `GET` com `Formulario = null`, `POST` com sucesso sem respostas, rejeicao de respostas preenchidas e limpeza de respostas antigas no frontend.
- As respostas do formulario continuam sem persistencia nesta etapa.
- O proximo item pendente da Sprint 8 passa a ser o item `43` (`Modelar persistencia das respostas do formulario no chamado`).
- As respostas do formulario continuam sem persistencia nesta etapa.
- A abertura guiada agora aplica o grupo tecnico configurado no catalogo quando ativo e preserva o fallback legado quando nao houver configuracao.
- Os itens `5-10` foram promovidos no checklist porque ja possuiam evidencia tecnica e testes no backend.
- O item `11` foi concluido somente apos testes dedicados de consulta administrativa `GET` para listagem e detalhe.
- O item `16` foi concluido com a modelagem estrutural da entidade `FormularioServico`, ainda sem campos, respostas ou renderizacao dinamica.
- O item `17` foi concluido com a modelagem estrutural da entidade `CampoFormularioServico`, ainda sem tipos, opcoes, respostas ou validacao dinamica.
- O item `18` foi concluido com a modelagem dos tipos permitidos para `CampoFormularioServico`, ainda sem opcoes, respostas ou validacao dinamica.
- O item `19` foi concluido com a modelagem de obrigatoriedade, ordem, ajuda e visibilidade como metadados do campo, ainda sem validacao dinamica ou frontend.
- O item `20` foi concluido com a modelagem estrutural de `OpcaoCampoFormularioServico`, ainda sem endpoint administrativo, renderizacao dinamica, versionamento ou respostas persistidas.
- O item `21` foi concluido com a modelagem estrutural de `FormularioServicoVersao`, preservando `FormularioServico` como cabecalho e deslocando os campos para uma versao especifica, ainda sem publicacao funcional, clonagem ou respostas versionadas.
- O item `22` foi concluido com a auditoria e consolidacao das configuracoes EF Core de formulario, versao, campos e opcoes, sem adicionar comportamento funcional novo fora da camada de persistencia.
- Itens transferidos da Sprint 7 (grupo responsavel do catalogo, formulario por servico e validacao/persistencia de respostas) foram concluidos com a modelagem estrutural da Sprint 8, fechando os itens 10, 13 e 14 do checklist da Sprint 7.
- Documento tecnico de apoio: `docs/roadmap/sprint-8-catalogo-servicos-2.md`.

## Atualizacao 2026-05-27 - Consolidacao da Fundacao ITSM

- Sprint 1 (Fundacao ITSM do chamado) consolidada como implementada e validada.
- Evidencias centralizadas em:
  - `docs/FUNDACAO-ITSM-CHAMADO.md`
  - `docs/APRESENTACAO-FUNDACAO-ITSM-DIRETORIA.md`
  - `docs/PROXIMAS-EVOLUCOES-ITSM.md`

## Atualizacao 2026-05-27 - Reorganizacao estrategica da ordem das sprints ITSM

- A numeracao das sprints ITSM foi reorganizada apos a conclusao da Fundacao ITSM para refletir melhor sequencia de evolucao do produto.
- A reorganizacao nao altera funcionalidades implementadas; altera apenas a ordem estrategica do roadmap.
- Sprint 1 permanece inalterada: Fundacao ITSM do chamado, 100%, implementada e validada.
- Nova Sprint 2: Relacionamentos, dependencias e orquestracao ITSM.

### Rastreabilidade de renumeracao (ordem anterior -> ordem atual)

| Ordem anterior | Nome anterior | Ordem atual | Nome atual |
|---|---|---|---|
| Sprint 1 | Fundacao ITSM do chamado | Sprint 1 | Fundacao ITSM do chamado |
| Sprint 5 | Grupos tecnicos, filas e atribuicao | Sprint 3 | Grupos tecnicos, filas e atribuicao |
| Sprint 7 | Motor de Aprovacoes ITSM | Sprint 4 | Motor de Aprovacoes ITSM |
| Sprint 9 | Regras de fechamento, aceite e reabertura | Sprint 5 | Regras de fechamento, aceite e reabertura |
| Sprint 10 | Notificacoes ITSM | Sprint 6 | Notificacoes ITSM |
| Sprint 3 | Gerenciamento de Requisicoes | Sprint 7 | Gerenciamento de Requisicoes |
| Sprint 4 | Catalogo de Servicos 2.0 | Sprint 8 | Catalogo de Servicos 2.0 |
| Sprint 2 | Gerenciamento de Incidentes | Sprint 9 | Gerenciamento de Incidentes |
| Sprint 12 | Gerenciamento de Mudancas | Sprint 10 | Gerenciamento de Mudancas |
| Sprint 15 | Gerenciamento de Problemas | Sprint 11 | Gerenciamento de Problemas |
| Sprint 13 | CMDB e Itens de Configuracao | Sprint 12 | CMDB e Itens de Configuracao |
| Sprint 14 | Analise de impacto | Sprint 13 | Analise de impacto |
| Sprint 8 | SLA 2.0, OLA e matriz impacto x urgencia | Sprint 14 | SLA 2.0, OLA e matriz impacto x urgencia |
| Sprint 6 | Observadores de chamados | Sprint 15 | Observadores de chamados |
| Sprint 17 | Monitoramento, eventos e Zabbix | Sprint 16 | Monitoramento, eventos e Zabbix |
| Sprint 11 | Relatorios ITSM avancados | Sprint 17 | Relatorios ITSM avancados |
| Sprint 18 | Base de Conhecimento 2.0 | Sprint 18 | Base de Conhecimento 2.0 |
| Sprint 16 | Pesquisa de satisfacao | Sprint 19 | Pesquisa de satisfacao |
| Sprint 19 | Homologacao institucional ITSM | Sprint 20 | Homologacao institucional ITSM |
| Sprint 20 | Produto, implantacao e operacao | Sprint 21 | Produto, implantacao e operacao |

### Nova Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM

Objetivo:
- Permitir relacionamento e orquestracao entre chamados ITSM, com dependencias, derivacoes e tarefas vinculadas.

Entregas previstas:
- Criar relacionamento entre chamados.
- Permitir vincular chamados relacionados.
- Permitir indicar dependencia entre chamados.
- Permitir que um chamado gere outro chamado relacionado.
- Permitir que incidente gere problema.
- Permitir que problema gere mudanca.
- Permitir que requisicao acione aprovacao.
- Permitir que mudanca acione planejamento.
- Permitir tarefas vinculadas ao chamado principal.
- Exibir relacionamentos no detalhe administrativo do chamado.
- Registrar historico de vinculos, derivacoes e dependencias.
- Preparar base para Gerenciamento de Mudancas, Problemas, CMDB, Analise de Impacto e Eventos.

Tipos de relacionamento previstos:
- Relacionado a
- Depende de
- Bloqueia
- Gerou
- Gerado por
- Causado por
- Resolvido por
- Duplicado de
- Convertido para
- Convertido de

## Campos principais do item de roadmap

- Area
- Categoria (legado)
- Objetivo
- RoadmapCategoriaId (referencia principal)
- Ordem
- SituacaoAtual
- AtencaoTecnica
- Status (geral/legado)
- Prioridade
- Impacto
- Decisao
- Responsavel
- PrazoAlvo
- Ativo
- Observacao

## Categoria como cadastro controlado

A categoria do Roadmap ITSM agora e controlada por cadastro em tabela propria (`RoadmapCategoria`).

Regras aplicadas:
- nome obrigatorio e unico;
- inativacao logica (sem exclusao fisica);
- dropdown de criacao/edicao usa somente categorias ativas;
- itens antigos continuam exibindo categoria legada quando necessario.

Campos da categoria:
- Nome
- Descricao
- Cor
- Icone
- Ordem
- Ativo

## Status real da implementacao

O campo `Status` foi mantido para compatibilidade e leitura geral.

Para status real, a referencia principal e `StatusImplementacao`, complementada por `StatusTecnico` e checklist.

Campos da secao:
- StatusImplementacao
- StatusTecnico
- PercentualImplementacao
- PendenciasTecnicas
- PendenciasHomologacao
- EvidenciaImplementacao
- DataConclusaoTecnica
- DataHomologacao
- CriterioAceite
- ProximaAcao

## Campo Objetivo

`Objetivo` explica a finalidade do item no sistema e responde: "qual problema ou necessidade este item resolve?".

Exemplos registrados:
- Abertura de chamado pelo portal: permitir abertura pelo solicitante autenticado com titulo, descricao, categoria, prioridade, anexos opcionais e acompanhamento posterior.
- Abertura por e-mail: processar e-mails via Worker IMAP para abrir chamados, correlacionar respostas, tratar anexos permitidos e registrar logs tecnicos.
- Perfis de acesso: controlar o acesso por perfis e permissoes granulares sem necessidade de alteracao de codigo.

### Significado de "Implementado funcionalmente"

`Implementado funcionalmente` indica entrega tecnica da funcionalidade, sem implicar automaticamente homologacao final ou producao.

### Pendencias evolutivas

Quando `StatusTecnico = Completo com pendencias evolutivas`, registrar obrigatoriamente:
- o que foi concluido;
- o que falta para homologacao/producao;
- proxima acao priorizada.

## Percentual por checklist

O percentual deixou de depender de digitacao manual quando ha checklist ativo.

Regra de calculo:
- `PercentualImplementacao = itens ativos concluidos / itens ativos * 100`

Comportamento:
- se existir checklist ativo, a UI mostra percentual calculado e bloqueia edicao manual;
- se nao existir checklist ativo, o valor legado pode ser usado como fallback.

## Checklist da implementacao

Cada item de roadmap pode ter varios itens em `RoadmapChecklistItem`.

Campos principais:
- Titulo
- Descricao
- Grupo
- Ordem
- Concluido
- Obrigatorio
- Ativo

Grupos sugeridos:
- Planejamento
- Desenvolvimento
- Testes
- Documentacao
- Homologacao
- Producao
- Seguranca
- Implantacao
- Governanca

Regras operacionais do CRUD:
- listagem ordenada por `Ordem`, depois `Grupo`, depois `Titulo`;
- acao principal de remocao e `Inativar` (nao `Excluir`);
- inativacao define `Ativo = false`, preserva historico e retira o item do calculo do percentual;
- reativacao define `Ativo = true` e retorna o item ao calculo do percentual;
- exclusao fisica permanece apenas como acao administrativa excepcional via endpoint tecnico;
- apos criar, editar, concluir, reabrir, inativar ou reativar, a tela recarrega detalhe, checklist e lista principal.

Auditoria esperada em checklist:
- criacao, edicao, conclusao, reabertura, inativacao, reativacao e exclusao;
- metadados com `checklistId`, `roadmapItemId`, `areaRoadmap` e `tituloChecklist`;
- dados antes/depois quando aplicavel.

## CRUD de futuras implementacoes

Cada item de roadmap pode ter N evolucoes em `RoadmapImplementacaoFutura`.

Campos:
- Titulo
- Descricao
- Tipo
- Prioridade
- Status
- Responsavel
- PrazoAlvo
- DataConclusao
- Observacao
- Ativo

Regras:
- vinculo obrigatorio ao item de roadmap;
- inativacao logica;
- concluir/inativar/reativar;
- filtros por status, tipo, prioridade, responsavel e ativo.

## Labels amigaveis na UI

A interface nao deve exibir enums crus.

Exemplos esperados:
- `EmValidacao` -> `Em validacao`
- `NaoIniciado` -> `Nao iniciado`
- `NaoAvaliado` -> `Nao avaliado`
- `CompletoComPendenciasEvolutivas` -> `Completo com pendencias evolutivas`

Aplicacao:
- `QSelect` mostra label amigavel e salva valor tecnico;
- `QTable` e `QBadge` mostram label amigavel;
- contratos de API preservam valor tecnico para integracao.

## Exemplo - Perfis de acesso

Preenchimento recomendado:
- Categoria: `Seguranca` (via `RoadmapCategoriaId`)
- Status (legado): `Implementado`
- StatusImplementacao: `Implementado funcionalmente`
- StatusTecnico: `Completo com pendencias evolutivas`
- Checklist ativo: 10 itens, 9 concluidos e 1 pendente
- Percentual calculado: `90%`
- PendenciasTecnicas: auditoria detalhada de alteracoes, testes frontend/e2e e validacao fina em homologacao
- PendenciasHomologacao: validacao com usuarios reais (Administrador, Atendente, Solicitante)
- EvidenciaImplementacao: `docs/SEGURANCA-PERFIS-PERMISSOES.md`, `docs/ROADMAP.md`, testes backend e matriz frontend
- CriterioAceite: admin gerencia permissoes; atendente ve acoes permitidas; solicitante nao acessa admin; backend bloqueia sem permissao
- ProximaAcao: executar homologacao real e priorizar auditoria detalhada

Futuras implementacoes sugeridas:
- Auditoria detalhada de alteracoes de permissoes
- Testes frontend/e2e da matriz de permissoes
- Validacao com usuarios reais
- Relatorio de permissoes por perfil
- Exportacao da matriz de permissoes

## Observacao de permissao

Nesta iteracao, endpoints de categoria/checklist reutilizam `Roadmap.Visualizar` e `Roadmap.Gerenciar`.

Pendencia real para evolucao futura:
- avaliar criacao de permissao granular dedicada para categorias/checklist (`RoadmapCategorias.*`, `RoadmapChecklist.*`).

## Gestao ITSM e Documentacao

Area: Gestao ITSM e Documentacao
Categoria: Governanca

Objetivo:
Centralizar no painel administrativo a consulta ao Roadmap ITSM e a documentacao funcional/tecnica do SGX Sistema de Chamados, facilitando apresentacao, governanca, homologacao e acompanhamento da evolucao do sistema.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Acesso administrativo:
- `Admin > Gestao ITSM > Roadmap`
- `Admin > Gestao ITSM > Documentacao`

Rotas:
- `/admin/gestao-itsm/roadmap`
- `/admin/gestao-itsm/documentacao`
- `/admin/roadmap-itsm` mantida por compatibilidade.

Checklist:
- [x] Grupo Gestao ITSM criado no menu administrativo.
- [x] Roadmap movido ou espelhado para Gestao ITSM.
- [x] Tela de Documentacao ITSM criada.
- [x] Documentos iniciais adicionados.
- [x] Busca de documentos criada.
- [x] Filtro por categoria criado.
- [x] Link entre Roadmap e Documentacao criado.
- [x] Permissoes integradas.
- [x] Documentacao do repositorio atualizada.
- [x] Testes ou validacao tecnica criados.

Pendencias evolutivas:
- Permitir edicao da documentacao pelo proprio sistema.
- Versionar documentacao por release.
- Anexar evidencias de homologacao.
- Exportar documentacao em PDF.
- Vincular documentos diretamente aos itens do roadmap.

## Sprint 9 - Execucao assistida da homologacao da autenticacao

Area: Seguranca e Governanca  
Categoria: Governanca

Status da implementacao: Homologacao assistida executada  
Status tecnico: Aprovado com ressalvas em 2026-05-26

Objetivo:
Executar a homologacao pratica assistida do modulo de autenticacao, preencher evidencias e ata institucional, e consolidar parecer final para TI/diretoria.

Entregas consolidadas:
- `docs/EVIDENCIAS-HOMOLOGACAO-AUTENTICACAO.md` preenchido com os 15 cenarios minimos;
- classificacao por cenario registrada (`Aprovado`, `Aprovado com ressalvas`, `Reprovado`);
- `docs/CHECKLIST-EXECUTIVO-AUTENTICACAO.md` preenchido com resumo executivo;
- `docs/ATA-HOMOLOGACAO-AUTENTICACAO.md` preenchida com parecer final e plano pos-homologacao.

Resultado consolidado da rodada:
- 15 cenarios executados;
- 10 aprovados;
- 5 aprovados com ressalvas;
- 0 reprovados;
- parecer final: `Aprovado com ressalvas`.

Ressalvas principais:
- pendente anexar evidencias visuais (prints) no ambiente publicado;
- pendente rodada fim-a-fim com contas corporativas reais para AD/LDAPS e Entra ID;
- pendente assinatura formal da TI/diretoria na ata.

Checklist da sprint:
- [x] Execucao assistida dos 15 cenarios minimos.
- [x] Caderno de evidencias preenchido.
- [x] Checklist executivo preenchido.
- [x] Ata de homologacao preenchida.
- [x] Parecer final consolidado.
- [x] ROADMAP-ITSM atualizado com status real.

Proxima acao:
- executar rodada presencial no ambiente publicado, anexar prints oficiais por cenario e concluir assinatura institucional da ata.

## Sprint 11 - Configuracao administrativa Active Directory / LDAP

Area: Seguranca  
Categoria: Autenticacao

Status da implementacao: Implementado funcionalmente  
Status tecnico: Sprint 11 concluida em 2026-05-26

Objetivo:
Disponibilizar uma tela administrativa dedicada para configuracao tecnica do provedor Active Directory/LDAP, separada da tela de Metodos de login.

Entregas consolidadas:
- tela `Active Directory / LDAP` no menu `Configuracoes > Integracoes`;
- endpoints administrativos:
  - `GET /api/admin/integracoes/active-directory`
  - `PUT /api/admin/integracoes/active-directory`
  - `POST /api/admin/integracoes/active-directory/testar-conexao`
  - `POST /api/admin/integracoes/active-directory/testar-autenticacao`
- persistencia da configuracao tecnica em `ParametroSistema` (`auth.active_directory.*`);
- validacao de configuracao minima tecnica e status de configuracao;
- confirmacao explicita obrigatoria para LDAP sem TLS;
- teste de autenticacao controlada sem persistencia de senha;
- novas permissoes:
  - `IntegracoesActiveDirectory.Visualizar`
  - `IntegracoesActiveDirectory.Gerenciar`
- integracao com Metodos de login para bloquear habilitacao do AD quando nao houver configuracao tecnica viavel.

Checklist da sprint:
- [x] Backend administrativo de configuracao AD/LDAP implementado.
- [x] Frontend administrativo AD/LDAP implementado.
- [x] Rota e menu protegidos por permissao.
- [x] Seed de permissoes atualizado para perfil Administrador.
- [x] Testes backend atualizados e ampliados.
- [x] Documentacao atualizada.

## Sprint 8 - Evidencias e Ata de Homologacao da Autenticacao

Area: Seguranca e Governanca  
Categoria: Governanca

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo e consolidado em 2026-05-26

Objetivo:
Preparar o pacote institucional de homologacao da autenticacao para execucao pratica em ambiente homologado, coleta de evidencias e apresentacao para diretoria de TI ou avaliador tecnico.

Entregas consolidadas:
- modelo de ata institucional de homologacao criado;
- modelo de evidencias por cenario criado;
- checklist executivo resumido criado;
- matriz de resultado institucional (`Aprovado`, `Aprovado com ressalvas`, `Reprovado`) formalizada;
- secoes de riscos, ressalvas, responsaveis, datas e proximos passos incluidas;
- `docs/HOMOLOGACAO-AUTENTICACAO.md` atualizado com campos de preenchimento real e referencias do pacote Sprint 8.

Checklist da sprint:
- [x] Documento `docs/ATA-HOMOLOGACAO-AUTENTICACAO.md` criado.
- [x] Documento `docs/EVIDENCIAS-HOMOLOGACAO-AUTENTICACAO.md` criado.
- [x] Documento `docs/CHECKLIST-EXECUTIVO-AUTENTICACAO.md` criado.
- [x] `docs/HOMOLOGACAO-AUTENTICACAO.md` atualizado com referencia aos novos documentos.
- [x] `docs/ROADMAP-ITSM.md` atualizado com status da Sprint 8.

Criterio de aceite da sprint:
- pacote documental institucional pronto para rodada real de homologacao;
- sem alteracao no codigo do modulo de autenticacao;
- trilha formal de decisao preparada para governanca e diretoria.

## Sprint 7 - Homologacao operacional e implantacao Cloud/On-Premises

Area: Seguranca e Governanca  
Categoria: Governanca

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo e consolidado em 2026-05-26

Objetivo:
Consolidar o pacote operacional de homologacao e implantacao do modulo de autenticacao do SGX para cenarios cloud, on-premises e hibridos.

Entregas consolidadas:
- checklist funcional de homologacao com 20 validacoes obrigatorias e campo de evidencias;
- runbook operacional para validacao de:
  - Administrador Local da Instancia;
  - login `LocalSgx`;
  - login `ActiveDirectory`;
  - login `MicrosoftEntraId`;
  - metodos de login configuraveis;
  - auditoria de autenticacao;
- guias de implantacao revisados:
  - `docs/IMPLANTACAO-CLOUD.md`
  - `docs/IMPLANTACAO-ON-PREMISES.md`
  - `docs/IMPLANTACAO-HIBRIDA.md`
- atualizacao de `README.md` com resumo dos modos de autenticacao e pacote de homologacao.

Checklist da sprint:
- [x] Documento `docs/HOMOLOGACAO-AUTENTICACAO.md` consolidado com checklist minimo obrigatorio (20 itens).
- [x] Documento `docs/RUNBOOK-AUTENTICACAO.md` consolidado com roteiro operacional completo.
- [x] Documento `docs/IMPLANTACAO-CLOUD.md` revisado para operacao cloud.
- [x] Documento `docs/IMPLANTACAO-ON-PREMISES.md` revisado para operacao on-premises.
- [x] Documento `docs/IMPLANTACAO-HIBRIDA.md` revisado para operacao hibrida.
- [x] `README.md` atualizado com resumo dos modos de autenticacao.
- [x] `ROADMAP-ITSM.md` atualizado com status consolidado.

Status consolidado do modulo de autenticacao (Sprints 1 a 7):
- provedores configuraveis entregues;
- Administrador Local da Instancia entregue;
- Active Directory LDAP/LDAPS entregue;
- gestao administrativa de metodos de login entregue;
- auditoria persistida de autenticacao entregue;
- tela administrativa de auditoria de autenticacao entregue;
- pacote operacional de homologacao e implantacao entregue.

Criterio de aceite da sprint:
- documentacao operacional pronta para execucao em homologacao;
- sem alteracao funcional no modulo de autenticacao;
- rastreabilidade de validacao por checklist, runbook e evidencias.

## Sprint 6 - Consulta administrativa da Auditoria de Autenticacao (Frontend)

Area: Seguranca e Governanca  
Categoria: Governanca

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo

Entrega consolidada:
- tela administrativa dedicada criada em `Admin > Governanca > Auditoria de autenticacao`;
- rota protegida por permissao `AuditoriaAutenticacao.Visualizar`;
- item de menu condicionado a permissao;
- filtros por periodo, provedor, resultado, tipo de evento e usuario/e-mail;
- paginação de resultados;
- ordenacao por eventos mais recentes;
- estados de vazio e erro de carregamento;
- contrato de endpoint de autenticacao ajustado para retorno estruturado (`provedor`, `tipoEvento`, `resultado`, `mensagem`) e filtros dedicados de autenticacao.

Checklist da sprint:
- [x] Service frontend de auditoria de autenticacao criado.
- [x] Types/interfaces TypeScript criados.
- [x] View Vue/Quasar de auditoria de autenticacao criada.
- [x] Rota protegida registrada.
- [x] Menu administrativo atualizado.
- [x] Permissao `AuditoriaAutenticacao.Visualizar` aplicada em menu e rota.
- [x] Documentacao de auditoria atualizada.
- [x] Roadmap ITSM atualizado.

## Sprint Historico/Auditoria 1 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Criar trilha de auditoria para registrar acoes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanca, analise de alteracoes e apoio a homologacao.

Situacao atual:
Modulo de auditoria iniciado com estrutura central de eventos auditaveis, service de registro, tabela propria e primeiros eventos do sistema.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual (checklist consolidado Sprints 1-3): 100% (63 de 63 itens)

Checklist Sprint 1:
- [x] Entidade EventoAuditoria criada.
- [x] Enum de acao de auditoria criado.
- [x] Enum de nivel de auditoria criado.
- [x] Migration da tabela eventos_auditoria criada.
- [x] Indices de consulta criados.
- [x] Service centralizado de auditoria criado.
- [x] Context provider de auditoria criado.
- [x] Captura de usuario atual integrada.
- [x] Captura de IP e User-Agent integrada.
- [x] Registro de login integrado.
- [x] Registro de logout avaliado e documentado como nao aplicavel enquanto nao houver fluxo backend controlado.
- [x] Registro de criacao/edicao/inativacao de usuario integrado.
- [x] Registro de perfis/permissoes integrado.
- [x] DTOs de auditoria criados.
- [x] Testes automatizados criados.
- [x] Documentacao atualizada em Gestao ITSM.

## Sprint Portal 3 - Abertura de chamado pelo portal

Area: Abertura de chamado pelo portal
Categoria: Portal

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Proxima acao: Homologar fluxo com usuario real.

Checklist de entrega tecnica:
- [x] Endpoint de contexto do portal validado
- [x] Endpoint de criacao de chamado validado
- [x] Validacoes obrigatorias implementadas
- [x] Solicitante obtido pelo usuario autenticado
- [x] Status inicial Aberto aplicado
- [x] Historico inicial criado
- [x] Tela /portal/chamados/novo implementada
- [x] Upload de anexo validado
- [x] Redirecionamento para detalhe validado
- [x] Chamado listado no portal
- [x] Chamado visivel no admin
- [x] Detalhe do portal validado
- [x] Historico inicial visivel
- [x] Testes backend criados/atualizados
- [x] Build frontend validado
- [ ] Homologacao manual com usuario real

Pendencias evolutivas:
- homologacao manual com usuario real
- testes E2E frontend do fluxo portal->admin
- validacao de anexos em ambiente real (tipos e limites com arquivos reais)

## Sprint Portal 4 - Fechamento do item Abertura de chamado pelo portal

Item: Abertura de chamado pelo portal
Categoria: Portal

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Interpretacao:
- Implementado funcionalmente significa entrega tecnica concluida e validada por inspecao, build e testes.
- Nao significa homologado com usuario real nem em producao.

Criterio de aceite registrado:
Solicitante autenticado consegue abrir chamado pelo portal com dados obrigatorios, visualizar retorno de sucesso e acessar o detalhe do chamado criado. Backend registra status inicial, historico e vinculo com solicitante. Quando anexos estao disponiveis, arquivos permitidos podem ser enviados e visualizados no detalhe.

Proxima acao:
Validar com usuario real em homologacao.

Pendencias tecnicas registradas:
- homologacao manual com usuario real
- testes E2E frontend
- validacao de anexos em ambiente real

## Sprint Integracoes E-mail 2 - E-mail novo cria chamado

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Em avaliacao continua

Checklist entregue nesta sprint:
- [x] E-mail novo cria chamado
- [x] Origem E-mail aplicada ao chamado
- [x] Status inicial Aberto aplicado
- [x] Historico inicial criado
- [x] Prevencao de duplicidade por MessageId implementada
- [x] Configuracoes de categoria/prioridade padrao definidas
- [x] Testes unitarios de processamento criados/atualizados

Pendencias mantidas para proximas sprints:
- [ ] Correlacao de respostas (regras finais)
- [ ] Anexos por e-mail (escopo completo)
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais

## Sprint Integracoes E-mail 3 - Correlacao de respostas e anexos

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Completo com pendencias evolutivas

Checklist entregue nesta sprint:
- [x] Correlacao por codigo do chamado implementada
- [x] Correlacao por Message-Id/In-Reply-To/References implementada
- [x] Resposta por e-mail adiciona comentario publico
- [x] Anexos por e-mail validados
- [x] Anexos permitidos sao salvos
- [x] Anexos invalidos sao rejeitados e logados
- [x] Testes de correlacao criados/atualizados
- [x] Testes de anexos criados/atualizados

Pendencias mantidas:
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais
- [ ] Validacao com anexos reais
- [ ] OAuth para caixa Microsoft (se exigido)
- [ ] Retry/backoff
- [ ] Dead-letter
- [ ] Monitoramento do Worker
- [ ] Reprocessamento manual de e-mails com erro
- [ ] Sanitizacao avancada de HTML
- [ ] Antivirus/varredura de anexos
- [ ] Teste E2E com IMAP real

## Sprint Integracoes E-mail 4 - Logs administrativos e tela

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Em desenvolvimento  
Status tecnico: Completo com pendencias evolutivas

Checklist entregue nesta sprint:
- [x] Endpoint de logs administrativos implementado
- [x] Tela `/admin/integracoes/email` validada
- [x] Filtros de logs implementados
- [x] Detalhe de log em dialog implementado
- [x] Solicitante bloqueado nos logs administrativos
- [x] Build frontend validado

Pendencias mantidas:
- [ ] Validacao com caixa IMAP real
- [ ] Homologacao com e-mails reais
- [ ] Validacao com anexos reais
- [ ] Retry/backoff
- [ ] Dead-letter
- [ ] Reprocessamento manual
- [ ] Monitoramento/health check
- [ ] OAuth Microsoft (se exigido)
- [ ] Antivirus/varredura de anexos
- [ ] Teste E2E com IMAP real

## Sprint Integracoes E-mail 5 - Fechamento tecnico, checklist e homologacao

Area: Abertura por e-mail  
Categoria: Integracoes

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Criterio de aceite consolidado:
E-mail recebido na caixa configurada e processado pelo Worker, criando chamado com origem E-mail, status inicial, historico e vinculo com remetente. Respostas correlacionadas adicionam comentario ao chamado existente. Anexos permitidos sao tratados conforme regras de seguranca. Logs tecnicos ficam disponiveis na area administrativa.

Proxima acao:
Validar com caixa IMAP real em homologacao.

Pendencias tecnicas registradas:
- validacao com caixa IMAP real
- homologacao com e-mails reais
- validacao com anexos reais
- OAuth Microsoft (se exigido)
- retry/backoff
- dead-letter
- monitoramento do Worker
- reprocessamento manual
- sanitizacao avancada de HTML
- antivirus/varredura de anexos
- teste E2E com IMAP real

Evidencias de implementacao:
- Worker.Email
- EmailWorkerOptions
- LogIntegracaoEmail
- ProcessarEmailRecebidoUseCase
- EmailParaChamadoService
- correlacao de respostas
- tratamento de anexos
- endpoints administrativos de logs
- tela `/admin/integracoes/email`
- testes automatizados
- `docs/INTEGRACAO-EMAIL.md`

Checklist tecnico (vinculado ao item):
- [x] 1. Projeto Worker.Email validado/criado
- [x] 2. Configuracoes IMAP definidas
- [x] 3. Leitura IMAP implementada
- [x] 4. Processamento em lote implementado
- [x] 5. LogIntegracaoEmail implementado
- [x] 6. Prevencao de duplicidade por MessageId implementada
- [x] 7. E-mail novo cria chamado
- [x] 8. Origem E-mail aplicada ao chamado
- [x] 9. Status inicial Aberto aplicado
- [x] 10. Historico inicial criado
- [x] 11. Correlacao por codigo do chamado implementada
- [x] 12. Correlacao por Message-Id/In-Reply-To implementada
- [x] 13. Resposta por e-mail adiciona comentario
- [x] 14. Anexos por e-mail validados
- [x] 15. Anexos permitidos sao salvos
- [x] 16. Anexos invalidos sao rejeitados e logados
- [x] 17. Endpoint de logs administrativos implementado
- [x] 18. Tela /admin/integracoes/email validada
- [x] 19. Filtros de logs implementados
- [x] 20. Detalhe de log em dialog implementado
- [x] 21. Testes unitarios de processamento criados
- [x] 22. Testes de correlacao criados
- [x] 23. Testes de anexos criados
- [x] 24. Build backend validado
- [x] 25. Testes backend executados
- [x] 26. Build Worker validado
- [x] 27. Build frontend validado

Checklist de evolucao/homologacao (pendente):
- [ ] 28. Validacao com caixa IMAP real
- [ ] 29. Homologacao com e-mails reais
- [ ] 30. Validacao com anexos reais
- [ ] 31. Autenticacao OAuth para caixa Microsoft, se exigido
- [ ] 32. Retry/backoff em falhas temporarias
- [ ] 33. Dead-letter ou fila de mensagens com erro
- [ ] 34. Monitoramento/health check do Worker
- [ ] 35. Painel de reprocessamento manual de e-mails com erro
- [ ] 36. Sanitizacao avancada de HTML
- [ ] 37. Antivirus/varredura de anexos
- [ ] 38. Teste E2E com IMAP real
- [ ] 39. Metricas operacionais do Worker
- [ ] 40. Alertas de falha recorrente no processamento de e-mail

Observacao de percentual:
- percentual do item deve ser calculado automaticamente pelo checklist ativo;
- nao preencher percentual manual quando checklist estiver ativo.

## Sprint Autentica��o 1 - Revis�o da base e desenho final do fluxo Entra ID

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status anterior:
- Status atual: N�o iniciado
- Status t�cnico: N�o avaliado

Status ap�s a Sprint Autentica��o 1:
- Status da implementa��o: Em desenvolvimento
- Status t�cnico: Completo com pend�ncias evolutivas

Decis�o arquitetural consolidada:
- Microsoft Entra ID (Azure AD) autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permiss�es.

Escopo revisado nesta sprint:
- [x] Revis�o do login Microsoft no frontend (`LoginView`, `authService`, `AuthStore`)
- [x] Revis�o da valida��o JWT/API (`ServiceCollectionExtensions`)
- [x] Revis�o de `GET /api/me` (`MeController`, `UsuarioAtualService`)
- [x] Revis�o de `httpClient` e tratamento de `401/403`
- [x] Revis�o de router guards (`router.beforeEach`)
- [x] Revis�o do login local Development
- [x] Revis�o da emula��o de perfis em Development
- [x] Consolida��o da documenta��o t�cnica da autentica��o corporativa

Fluxo oficial definido:
1. Usu�rio acessa o frontend.
2. Usu�rio clica em `Entrar com Microsoft Entra ID`.
3. Usu�rio autentica no Microsoft Entra ID.
4. Frontend recebe `access token`.
5. API valida o token JWT.
6. SGX identifica o usu�rio interno.
7. SGX cria usu�rio interno quando aplic�vel.
8. SGX retorna `GET /api/me` com perfis e permiss�es efetivas.
9. Frontend redireciona conforme perfil/permiss�o.

Pend�ncias reais para Sprint Autentica��o 2:
- [ ] Configurar App Registration definitivo (SPA e API) no tenant institucional.
- [ ] Validar escopo real da API no frontend (`VITE_AZURE_API_SCOPE`).
- [ ] Homologar fluxo real com usu�rios corporativos (Administrador, Atendente e Solicitante).
- [ ] Definir regra formal para provisionamento e bloqueio de usu�rio interno conforme ciclo de vida no Entra ID.
- [ ] Registrar evid�ncias formais de homologa��o para promo��o a produ��o.

Evid�ncias de implementa��o/documenta��o:
- `docs/AUTENTICACAO-CORPORATIVA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`

## Sprint Autentica��o 2 - Backend Microsoft Entra ID, JWT e usu�rio interno

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status ap�s a Sprint Autentica��o 2:
- Status da implementa��o: Em desenvolvimento
- Status t�cnico: Completo com pend�ncias evolutivas

Decis�o arquitetural mantida:
- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permiss�es.

Escopo entregue nesta sprint:
- [x] Revis�o e refor�o da valida��o JWT (`Authority`, `Issuer`, `Audience`, expira��o e assinatura).
- [x] Suporte a `MetadataAddress` opcional em `AzureAdOptions`.
- [x] Fortalecimento das op��es de autentica��o (`DominiosPermitidos`, `CriarUsuarioAutomaticamente`, `PerfilPadraoUsuarioMicrosoft`).
- [x] Mapeamento de claims Microsoft com fallback definido (`preferred_username`, `email`, `upn`, `unique_name`).
- [x] Regras de bloqueio por dom�nio n�o permitido.
- [x] Regras de bloqueio de usu�rio interno inativo.
- [x] Cria��o autom�tica de usu�rio interno com perfil padr�o quando permitido.
- [x] Preserva��o do login local Development e emula��o de perfis.
- [x] Preserva��o do contrato de `GET /api/me` com `autenticadoPor=MicrosoftEntraId` no fluxo Microsoft.
- [x] Testes automatizados de unidade e integra��o atualizados.

Regras de seguran�a validadas:
- [x] Perfis e permiss�es continuam internos no SGX.
- [x] `roles` e `groups` do Azure AD n�o concedem perfil administrativo automaticamente.
- [x] Login local n�o � habilitado fora de Development.

Pend�ncias reais para Sprint Autentica��o 3:
- [ ] Homologa��o ponta a ponta com tenant institucional real (Microsoft Entra ID).
- [ ] Valida��o operacional em ambiente de homologa��o com usu�rios reais.
- [ ] Defini��o final de governan�a de ciclo de vida de usu�rio interno (bloqueio, reativa��o e auditoria).
- [ ] Avalia��o de persist�ncia opcional de identificadores corporativos (`oid`/`tid`) sem impacto em migra��es indevidas.

Evid�ncias de implementa��o:
- `src/SGX.SistemaChamado.Api/Services/UsuarioAtualService.cs`
- `src/SGX.SistemaChamado.Api/Extensions/ServiceCollectionExtensions.cs`
- `src/SGX.SistemaChamado.Api/Options/AuthOptions.cs`
- `src/SGX.SistemaChamado.Api/Options/AzureAdOptions.cs`
- `src/SGX.SistemaChamado.Api/Options/AzureAdOptionsValidator.cs`
- `tests/SGX.SistemaChamado.Tests/UsuarioAtualServiceTests.cs`
- `tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs`
- `tests/SGX.SistemaChamado.Tests/AzureAdOptionsValidatorTests.cs`

## Sprint Autentica��o 3 - Frontend de login Microsoft e restaura��o de sess�o

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status ap�s a Sprint Autentica��o 3:
- Status da implementa��o: Em desenvolvimento
- Status t�cnico: Completo com pend�ncias evolutivas

Escopo entregue nesta sprint:
- [x] Consolida��o do login Microsoft no frontend (`LoginView`, `authService`, `authStore`).
- [x] Ajuste de mensagens de erro e cancelamento amig�vel no popup Microsoft.
- [x] Refor�o de restaura��o de sess�o com single-flight em `inicializarSessao`.
- [x] Manuten��o de `GET /api/me` como fonte de perfis e permiss�es.
- [x] Preserva��o dos guards de `/admin`, `/portal`, `/acesso-negado` e `/login`.
- [x] Preserva��o de login local e emula��o apenas em Development.
- [x] Bloqueio expl�cito de a��es concorrentes no login (duplo clique).
- [x] Alinhamento de tipagem do frontend para `autenticadoPor=MicrosoftEntraId`.

Pend�ncias reais para Sprint Autentica��o 4:
- [ ] Validar login Microsoft com tenant institucional real e evid�ncias formais.
- [ ] Validar cen�rios corporativos de MFA/Conditional Access em homologa��o.
- [ ] Executar rodada completa de valida��o manual de UX de sess�o em ambiente interativo.

Evid�ncias de implementa��o:
- `src/SGX.SistemaChamado.Web/src/views/LoginView.vue`
- `src/SGX.SistemaChamado.Web/src/services/authService.ts`
- `src/SGX.SistemaChamado.Web/src/stores/authStore.ts`
- `src/SGX.SistemaChamado.Web/src/types/auth.ts`
- `docs/AUTENTICACAO-CORPORATIVA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`

## Sprint Autentica��o 4 - Configura��o Microsoft Entra ID e homologa��o t�cnica

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Em desenvolvimento  
Status t�cnico: Completo com pend�ncias evolutivas

Checklist entregue nesta sprint:
- [x] App Registration documentado
- [x] Redirect URI documentado
- [x] Logout URI documentado
- [x] Escopo de API documentado
- [x] Vari�veis backend documentadas
- [x] Vari�veis frontend documentadas
- [x] Seguran�a MFA/Conditional Access documentada
- [x] Checklist de homologa��o criado

Pend�ncias mantidas:
- [ ] Configurar tenant institucional real
- [ ] Executar homologa��o com usu�rio corporativo real
- [ ] Validar MFA
- [ ] Validar Conditional Access
- [ ] Validar ambiente publicado/VPS
- [ ] Registrar evid�ncias formais de homologa��o

## Sprint Autentica��o 5 - Fechamento do item Autentica��o corporativa

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Objetivo:
Permitir que usu�rios acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autoriza��o interna no SGX por usu�rios, perfis e permiss�es. O Azure autentica a identidade; o SGX controla o que cada usu�rio pode acessar e executar dentro do sistema.

Situa��o atual:
Fluxo de autentica��o corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com valida��o de token JWT, modo Single Tenant, controle de dom�nio permitido, integra��o com `GET /api/me`, cria��o/identifica��o de usu�rio interno e autoriza��o por perfis/permiss�es do SGX. Ainda depende de homologa��o com tenant institucional real.

Aten��o t�cnica:
Microsoft Entra ID/Azure AD autentica e o SGX autoriza. Roles/groups do Azure n�o concedem perfil administrativo automaticamente no SGX. Perfis e permiss�es continuam internos ao SGX.

Checklist:
- 19 itens t�cnicos conclu�dos
- 8 itens pendentes de homologa��o/governan�a
- percentual esperado aproximado: 70%

Pend�ncias t�cnicas:
- homologar com tenant institucional real do Microsoft Entra ID;
- validar login com usu�rios corporativos reais;
- validar MFA e Conditional Access;
- validar logout corporativo;
- validar ambiente publicado/VPS;
- revisar configura��o com a equipe respons�vel pelo Azure;
- registrar evid�ncias formais de homologa��o;
- avaliar persist�ncia opcional de `oid/tid`;
- definir governan�a de ciclo de vida do usu�rio interno.

Pend�ncias de homologa��o:
- executar homologa��o ponta a ponta com usu�rios reais de perfil Administrador, Atendente e Solicitante;
- validar comportamento com usu�rio interno inativo;
- validar bloqueio de dom�nio/tenant n�o permitido;
- validar mensagens de erro de login;
- validar redirecionamento por perfil/permiss�o ap�s login;
- registrar evid�ncias com prints, data, ambiente e usu�rio de teste.

Mensagem para reuni�o:
A autentica��o corporativa do SGX est� desenhada para usar Microsoft Entra ID/Azure AD como identidade principal, enquanto o SGX mant�m a autoriza��o interna por perfis e permiss�es. Essa abordagem permite MFA, Conditional Access, acesso fora da rede e melhor governan�a sem transferir regras internas do sistema para o Azure.

## Sprint 2 - Administrador Local da Instancia

Area: Autenticacao corporativa  
Categoria: Seguranca

Status da implementacao: Implementado funcionalmente  
Status tecnico: Concluido

Objetivo:
- consolidar o Administrador Local da Instancia como primeiro acesso administrativo seguro e contingencia operacional.

Checklist tecnico:
- [x] bootstrap por `SGX_ADMIN_INICIAL_EMAIL`, `SGX_ADMIN_INICIAL_SENHA`, `SGX_ADMIN_INICIAL_NOME`
- [x] nao criar usuario quando variaveis obrigatorias estao ausentes ou incompletas
- [x] nao duplicar quando ja existe Administrador ativo
- [x] rejeitar senha fraca por politica centralizada
- [x] armazenar senha somente com hash seguro
- [x] impedir exposicao de senha em logs
- [x] criar usuario inicial com `DeveAlterarSenha=true`
- [x] exigir troca de senha no primeiro acesso
- [x] login local SGX apos troca com `DeveAlterarSenha=false`
- [x] bloquear login local de usuario inativo
- [x] validar contingencia administrativa com Microsoft desabilitado
- [x] documentacao atualizada (`AUTENTICACAO-CORPORATIVA.md` e `ADMINISTRADOR-LOCAL-INSTANCIA.md`)

## Sprint 3 - Active Directory dedicado via LDAP/LDAPS

Area: Autenticacao corporativa  
Categoria: Seguranca

Status da implementacao: Implementado funcionalmente  
Status tecnico: Concluido com pendencia de homologacao em AD real

Objetivo:
- suportar autenticacao on-premises em Active Directory local, com emissao de JWT interno SGX.

Checklist tecnico:
- [x] `ActiveDirectoryOptions` criado
- [x] `ActiveDirectoryOptionsValidator` criado
- [x] `IActiveDirectoryAuthenticationService` criado
- [x] `ActiveDirectoryAuthenticationService` criado
- [x] `POST /api/auth/ad/login` implementado
- [x] emissao de JWT interno com `autenticadoPor=ActiveDirectory`
- [x] bloqueio quando provedor AD esta desabilitado
- [x] bloqueio de usuario inativo no SGX com credencial AD valida
- [x] auto provisionamento configuravel com perfil padrao
- [x] sem persistencia de senha AD
- [x] sem log de senha AD
- [x] testes automatizados de servico/validador e integracao basica
- [x] documentacao atualizada (`AUTENTICACAO-CORPORATIVA.md` e `CONFIGURACAO-ACTIVE-DIRECTORY.md`)

## Sprint Autentica��o 7 - Administrador inicial seguro

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Objetivo:
Permitir a cria��o segura do primeiro Administrador em produ��o por vari�veis de ambiente expl�citas, sem senha fixa e sem depend�ncia do modo Development.

Checklist:
- [x] Vari�veis de ambiente definidas (`SGX_ADMIN_INICIAL_EMAIL`, `SGX_ADMIN_INICIAL_SENHA`, `SGX_ADMIN_INICIAL_NOME`)
- [x] Valida��o de e-mail implementada
- [x] Valida��o de senha forte implementada
- [x] Senha hasheada
- [x] Perfil Administrador associado
- [x] N�o cria duplicidade se j� existe Administrador ativo
- [x] Documenta��o atualizada
- [x] Testes criados/atualizados
- [ ] Homologa��o em ambiente real pendente

Pend�ncias evolutivas:
- processo operacional de rota��o de credencial de bootstrap;
- valida��o formal em homologa��o/produ��o;
- auditoria operacional cont�nua de eventos de cria��o inicial.

## Sprint Autentica��o 8 - Recupera��o de senha e hardening do login local SGX

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Objetivo:
Permitir recupera��o de senha local SGX, troca obrigat�ria e hardening de login para produ��o, sem senha em texto puro, sem enumera��o de usu�rio e com lockout configur�vel.

Checklist:
- [x] troca de senha autenticada
- [x] troca obrigat�ria
- [x] recupera��o de senha
- [x] token tempor�rio
- [x] token de uso �nico
- [x] token com expira��o
- [x] pol�tica de senha
- [x] lockout
- [x] �ltimo login
- [x] frontend `/alterar-senha`
- [x] frontend `/recuperar-senha`
- [x] documenta��o
- [x] testes
- [ ] homologa��o real pendente

Pend�ncias evolutivas:
- envio transacional real de e-mail para recupera��o;
- auditoria dedicada persistida em banco para eventos de autentica��o local;
- valida��o formal em ambiente publicado com evid�ncias de lockout e recupera��o.

## Sprint Autentica��o 9 - Tenant �nico, contas permitidas e homologa��o real Microsoft Entra ID

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Checklist t�cnico:
- [x] Single Tenant documentado
- [x] TenantId validado
- [x] issuer validado
- [x] tid validado
- [x] audience validada
- [x] contas pessoais Microsoft bloqueadas
- [x] tenants externos bloqueados
- [x] dom�nio permitido validado quando configurado
- [x] roles/groups Azure n�o concedem admin
- [x] mensagens frontend amig�veis
- [x] testes automatizados criados/ajustados
- [x] documenta��o atualizada

Pend�ncias:
- homologa��o com tenant real;
- teste com usu�rio externo real;
- teste com conta pessoal Microsoft real;
- MFA;
- Conditional Access;
- logout corporativo;
- evid�ncias formais.

## Corre��es - Integra��o Microsoft, usu�rios demo e senha por Administrador

�rea: Autentica��o corporativa  
Categoria: Seguran�a

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Checklist t�cnico:
- [x] Menu `Integra��es` exibe `Microsoft Entra ID`
- [x] Tela `/admin/integracoes/microsoft-entra-id` criada
- [x] Endpoints administrativos de configura��o Microsoft criados (`GET/PUT`)
- [x] LoginView consome provedores com fallback amig�vel
- [x] Seed Development mant�m 2 usu�rios demonstrativos por perfil
- [x] Redefini��o de senha por Administrador implementada
- [x] Permiss�es novas criadas (`IntegracoesMicrosoft.*`, `Usuarios.RedefinirSenha`)
- [x] Testes automatizados atualizados

Pend�ncias evolutivas:
- homologa��o funcional em banco PostgreSQL real com dados legados;
- governan�a de limpeza administrativa para bases antigas com usu�rios demo excedentes;
- revis�o de UX para edi��o de configura��es Microsoft em ambiente distribu�do (quando exigir restart).

## Sprint SLA 1 - Modelagem e cadastro administrativo

�rea: SLA  
Categoria: SLA

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Resumo:
- cadastro administrativo de pol�ticas de SLA implementado;
- metas de SLA por prioridade implementadas em estrutura pr�pria;
- pol�tica padr�o de SLA semeada com metas para Baixa, M�dia, Alta e Cr�tica;
- checklist Sprint 1 do item SLA criado e vinculado no roadmap;
- percentual do item passa a ser calculado pelo checklist ativo.

Limita��o conhecida:
- nesta sprint, a aplica��o autom�tica integral da pol�tica no fluxo dos chamados fica para Sprint 2.

## Sprint SLA 2 - Aplica��o pr�tica nos chamados

�rea: SLA  
Categoria: SLA

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Resumo:
- registro pr�prio de SLA aplicado ao chamado criado em `chamado_slas`;
- pol�tica ativa escolhida por compatibilidade de categoria/departamento, ordem e meta ativa por prioridade;
- SLA aplicado na cria��o do chamado sem impedir abertura quando n�o h� pol�tica/meta aplic�vel;
- primeira resposta registrada por coment�rio p�blico de atendente ou status `Em atendimento`;
- resolu��o registrada em status final, `Resolvido` ou `Encerrado`;
- pausa implementada quando o status entra em `AguardandoSolicitante`, respeitando a pol�tica aplicada;
- detalhe e listagem de chamados retornam resumo de SLA;
- listagem administrativa possui filtros por situa��o do SLA;
- documenta��o t�cnica atualizada em `docs/SLA.md`.

Checklist Sprint 2:
- [x] Tabela de SLA aplicado ao chamado criada.
- [x] Relacionamento entre chamado e SLA criado.
- [x] Service de c�lculo de SLA criado.
- [x] Pol�tica aplic�vel identificada por prioridade/categoria/departamento.
- [x] SLA aplicado na cria��o do chamado.
- [x] Prazo de primeira resposta calculado.
- [x] Prazo de resolu��o calculado.
- [x] Primeira resposta registrada.
- [x] Resolu��o registrada.
- [x] Pausa de SLA preparada ou implementada.
- [x] Situa��o atual do SLA calculada.
- [x] SLA exibido no detalhe do chamado.
- [x] SLA exibido na listagem administrativa.
- [x] Filtros administrativos de SLA criados.
- [x] DTOs de chamado atualizados com resumo de SLA.
- [x] Testes automatizados criados.
- [x] Documenta��o atualizada.

Pend�ncias t�cnicas:
- calend�rio corporativo para `UsarHorarioComercial=true`;
- homologa��o com base PostgreSQL real;
- relat�rios hist�ricos e indicadores avan�ados de SLA.

## Sprint SLA 3 - Alertas, vencimentos e painel de SLA

�rea: SLA  
Categoria: SLA

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Resumo:
- configura��o padr�o de alertas criada e exposta em `Admin > SLA > Alertas`;
- eventos de SLA persistidos em `eventos_sla` com chave de idempot�ncia;
- monitoramento peri�dico configur�vel por `SlaMonitoring`;
- alertas de primeira resposta/resolu��o pr�ximos do vencimento e vencidos registrados como eventos;
- painel gerencial em `Admin > SLA > Painel`;
- hist�rico de SLA exibido no detalhe administrativo do chamado;
- consulta estruturada para relat�rio futuro.

Checklist Sprint 3:
- [x] Configura��o de alerta de SLA criada.
- [x] Tela administrativa de configura��o de alerta criada.
- [x] Endpoints de configura��o de alerta criados.
- [x] Job de verifica��o de SLA criado.
- [x] Periodicidade configur�vel por appsettings criada.
- [x] Controle contra notifica��es/eventos duplicados criado.
- [x] Hist�rico de eventos de SLA criado.
- [x] Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolu��o, pausa e retomada.
- [x] Painel de indicadores de SLA criado.
- [x] Indicador de SLA vencido criado.
- [x] Indicador de SLA pr�ximo do vencimento criado.
- [x] Indicador de percentual de cumprimento criado.
- [x] M�trica de tempo m�dio de primeira resposta criada.
- [x] M�trica de tempo m�dio de resolu��o criada.
- [x] Indicadores por prioridade criados.
- [x] Indicadores por categoria criados.
- [x] Indicadores por departamento criados.
- [x] Hist�rico de SLA exibido no detalhe administrativo do chamado.
- [x] Estrutura preparada para exporta��o futura.
- [x] Documenta��o atualizada.
- [x] Testes automatizados criados.

Pend�ncias t�cnicas:
- envio real de notifica��es por canal oficial;
- exporta��o Excel/PDF;
- evid�ncias de homologa��o em ambiente publicado;
- calend�rio por departamento/time;
- importa��o autom�tica de feriados.

## Sprint SLA 4 - Calend�rio corporativo e hor�rio comercial

�rea: SLA  
Categoria: SLA

Status da implementa��o: Implementado funcionalmente  
Status t�cnico: Completo com pend�ncias evolutivas

Entregas:

- estrutura de calend�rio corporativo criada com expediente semanal e exce��es;
- seed do calend�rio corporativo padr�o, ativo, em `America/Sao_Paulo`, segunda a sexta das 09:00 �s 18:00;
- pol�tica de SLA vincul�vel a calend�rio corporativo;
- c�lculo de SLA em minutos corridos ou minutos �teis conforme configura��o da pol�tica;
- endpoints administrativos para calend�rios, hor�rios e exce��es;
- tela `Admin > SLA > Calend�rios`;
- tela de pol�ticas com sele��o de calend�rio quando hor�rio comercial est� ativo;
- detalhe administrativo do chamado mostra tipo de c�lculo e calend�rio utilizado;
- testes automatizados para calend�rio, c�lculo �til e integra��o com SLA.

Checklist Sprint 4:

- [x] Entidade CalendarioCorporativo criada.
- [x] Entidade HorarioAtendimentoCalendario criada.
- [x] Entidade ExcecaoCalendarioCorporativo criada.
- [x] Migrations de calend�rio criadas.
- [x] Seed do calend�rio padr�o criado.
- [x] Relacionamento entre Pol�tica SLA e Calend�rio criado.
- [x] Service administrativo de calend�rio criado.
- [x] Service de c�lculo de tempo �til criado.
- [x] C�lculo de prazo de primeira resposta usando hor�rio comercial implementado.
- [x] C�lculo de prazo de resolu��o usando hor�rio comercial implementado.
- [x] C�lculo de minutos �teis de primeira resposta implementado.
- [x] C�lculo de minutos �teis de resolu��o implementado.
- [x] Endpoints administrativos de calend�rio criados.
- [x] Tela Admin > SLA > Calend�rios criada.
- [x] Tela de pol�tica SLA atualizada com sele��o de calend�rio.
- [x] Detalhe do chamado mostra tipo de c�lculo e calend�rio usado.
- [x] Testes automatizados criados.
- [x] Documenta��o atualizada.

Pend�ncias:

- calend�rio por departamento/time;
- importa��o autom�tica de feriados;
- exce��es recorrentes;
- regra avan�ada de prazo remanescente em reabertura.

## Sprint Historico/Auditoria 2 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Registrar acoes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanca, analise de alteracoes, auditoria operacional e apoio a homologacao.

Situacao atual:
Base tecnica de auditoria criada na Sprint 1. Sprint 2 aplica auditoria aos modulos criticos do sistema, incluindo chamados, usuarios, perfis/permissoes, SLA, autenticacao corporativa e roadmap ITSM.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist.

Checklist Sprint 2:
- [x] Helper de diff antes/depois criado.
- [x] Mascaramento de dados sensiveis implementado.
- [x] Auditoria de abertura de chamado implementada.
- [x] Auditoria de alteracao de status implementada.
- [x] Auditoria de alteracao de prioridade implementada.
- [x] Auditoria de alteracao de categoria implementada.
- [x] Auditoria de atribuicao de responsavel implementada.
- [x] Auditoria de assumir chamado implementada.
- [x] Auditoria de comentarios administrativos implementada.
- [x] Auditoria de encerramento/resolucao implementada.
- [x] Auditoria de reabertura implementada.
- [x] Auditoria de anexos preparada ou implementada.
- [x] Auditoria de usuarios revisada e complementada.
- [x] Auditoria de perfis revisada e complementada.
- [x] Auditoria de permissoes revisada e complementada.
- [x] Auditoria de politicas de SLA implementada.
- [x] Auditoria de metas de SLA implementada.
- [x] Auditoria de calendarios de SLA implementada.
- [x] Auditoria de horarios de calendario implementada.
- [x] Auditoria de excecoes de calendario implementada.
- [x] Auditoria de alertas de SLA implementada.
- [x] Auditoria de autenticacao corporativa implementada.
- [x] Auditoria de Roadmap ITSM implementada.
- [x] Auditoria de documentacao ITSM preparada conforme estrutura atual.
- [x] Testes automatizados de auditoria dos modulos criticos criados.
- [x] Documentacao atualizada em Gestao ITSM.
- [x] Validacao no banco com eventos reais em eventos_auditoria preparada/executada.

Observacao:
- leitura da documentacao ITSM nao e auditada na Sprint 2 por ser conteudo estatico;
- edicao/publicacao de documentacao ainda nao existe no sistema e fica para evolucao futura.

## Sprint Historico/Auditoria 3 - Governanca

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Permitir que administradores e gestores consultem eventos de auditoria no painel administrativo, com filtros avancados, paginacao, detalhe e indicadores.

Situacao atual:
Base tecnica da Sprint 1 e auditoria em modulos criticos da Sprint 2 evoluiram para consulta administrativa funcional na Sprint 3.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist (63/63 = 100%).

Checklist Sprint 3:
- [x] Endpoints administrativos de auditoria criados.
- [x] Use cases/services de consulta de auditoria criados.
- [x] Filtros de auditoria criados.
- [x] Paginacao de eventos criada.
- [x] Endpoint de detalhe de evento criado.
- [x] Endpoint de dashboard de auditoria criado.
- [x] Permissoes de auditoria criadas ou integradas.
- [x] Menu Governanca > Auditoria criado.
- [x] Rota /admin/governanca/auditoria criada.
- [x] Tela administrativa de auditoria criada.
- [x] Modal/drawer de detalhe criado.
- [x] Visualizacao de dados antes/depois criada.
- [x] Indicadores basicos de auditoria criados.
- [x] Service frontend de auditoria criado.
- [x] Tipos frontend de auditoria criados.
- [x] Link entre Auditoria e Gestao ITSM criado.
- [x] Documentacao em Gestao ITSM atualizada.
- [x] Testes automatizados backend criados.
- [x] Build frontend validado.
- [x] Validacao com eventos reais em eventos_auditoria executada.

Pendencias evolutivas:
- Exportacao Excel/PDF.
- Retencao configuravel de auditoria.
- Assinatura/hash da trilha de auditoria.
- Alertas para eventos criticos.
- Painel avancado de seguranca.
- Integracao com SIEM/Log Analytics.
- Politica de anonimiza�ao/LGPD para eventos antigos.

## Sprint Cadastros Administrativos 1 - Base tecnica

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Criar a fundacao tecnica dos cadastros administrativos para sustentar a evolucao do SGX em chamados, SLA, dashboards, relatorios e governanca operacional.

Entregas tecnicas:
- entidades de dominio criadas/evoluidas para `Departamento`, `CategoriaChamado`, `SubcategoriaChamado`, `PrioridadeChamado`, `TipoSolicitacao` e `LocalUnidade`;
- relacionamento `CategoriaChamado 1:N SubcategoriaChamado` implementado;
- `DbSet` adicionados no `SGXSistemaChamadoDbContext`;
- mapeamentos Fluent API criados para novos cadastros;
- tabela `prioridades_chamado` evoluida com `peso` e `cor`;
- migration `AddCadastrosAdministrativosSprint1` criada e aplicada no banco PostgreSQL;
- documentacao publicada em `docs/CADASTROS-ADMINISTRATIVOS.md`.

Checklist Sprint 1:
- [x] Entidade Departamento validada
- [x] Entidade CategoriaChamado validada
- [x] Entidade SubcategoriaChamado criada
- [x] Entidade PrioridadeChamado evoluida com Peso e Cor
- [x] Entidade TipoSolicitacao criada
- [x] Entidade LocalUnidade criada com Endereco
- [x] DbSet adicionados no DbContext
- [x] Fluent API criada/ajustada
- [x] Relacionamento categoria x subcategoria criado
- [x] Migration criada
- [x] Banco atualizado
- [x] Documentacao inicial criada
- [x] Roadmaps atualizados

Pendencias evolutivas:
- disponibilizar CRUD administrativo de tipos de solicitacao e locais/unidades;
- conectar novos cadastros no fluxo de abertura/edicao de chamado;
- ampliar cobertura de testes automatizados para os novos modelos e endpoints.

## Sprint Cadastros Administrativos 2 - Backend CRUD

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar CRUD administrativo de departamentos, categorias e subcategorias com regras de validacao, ativacao/inativacao e preservacao historica.

Entregas tecnicas:
- CRUD de departamentos com listagem, busca, filtro por status e inativacao logica;
- CRUD de categorias com listagem, busca, filtro por status e inativacao logica;
- CRUD de subcategorias com listagem geral e por categoria;
- validacao de categoria obrigatoria/existente para subcategoria;
- bloqueio de duplicidade de subcategoria dentro da mesma categoria;
- rotas administrativas em `api/admin` com compatibilidade mantida em `api/admin/cadastros`;
- `DELETE` para cadastros convertido para comportamento de inativacao logica;
- testes automatizados de use cases ampliados para os tres cadastros.

Checklist Sprint 2:
- [x] DTOs de subcategoria criados
- [x] Use cases de subcategoria criados
- [x] Endpoints administrativos de subcategoria criados
- [x] Endpoints `PATCH` de ativar/inativar criados
- [x] Endpoints `DELETE` com inativacao logica criados
- [x] Validacoes de duplicidade aplicadas
- [x] Validacao de vinculo categoria/subcategoria aplicada
- [x] Listagem com busca e filtro por status validada
- [x] Testes automatizados criados/atualizados
- [x] Documentacao e roadmaps atualizados

Pendencias evolutivas:
- CRUD administrativo de tipos de solicitacao;
- CRUD administrativo de locais/unidades;
- integracao de subcategoria/tipo/local ao fluxo de abertura e atendimento.

## Sprint Cadastros Administrativos 3 - Backend CRUD

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar CRUD administrativo de prioridades, tipos de solicitacao e locais/unidades com regras de validacao, ativacao/inativacao e preservacao historica.

Entregas tecnicas:
- CRUD de prioridades com validacao de nome duplicado;
- validacao de peso obrigatorio e maior que zero em prioridades;
- validacao de cor opcional em prioridade no formato hexadecimal `#RRGGBB`;
- CRUD de tipos de solicitacao com validacao de nome duplicado;
- CRUD de locais/unidades com validacao de nome duplicado e endereco opcional;
- listagem com busca por nome, filtro por status e paginacao;
- `DELETE` para os tres cadastros com inativacao logica;
- aliases legados mantidos em `api/admin/cadastros/prioridades`, `api/admin/cadastros/tipos-solicitacao` e `api/admin/cadastros/locais`;
- testes automatizados de use case e HTTP para os tres cadastros.

Checklist Sprint 3:
- [x] Endpoints administrativos de prioridades
- [x] Endpoints administrativos de tipos de solicitacao
- [x] Endpoints administrativos de locais/unidades
- [x] Validacoes de duplicidade
- [x] Validacao de peso da prioridade
- [x] Validacao de cor da prioridade
- [x] Ativacao e inativacao
- [x] Inativacao logica em `DELETE`
- [x] Listagem com busca e filtro por status
- [x] Testes automatizados criados/atualizados
- [x] Documentacao e roadmaps atualizados

Pendencias evolutivas:
- integrar `TipoSolicitacao` e `LocalUnidade` na abertura/edicao de chamados;
- evoluir regras de SLA para considerar `Peso` como ordenacao principal de prioridade;
- homologacao funcional com usuarios-chave do modulo de cadastros.

## Sprint Cadastros Administrativos 4 - Frontend Administrativo

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar no frontend administrativo as telas de manutencao dos cadastros de departamentos, categorias, subcategorias, prioridades, tipos de solicitacao e locais/unidades.

Entregas tecnicas:
- menu `Admin > Cadastros` consolidado com todos os itens da trilha de cadastros;
- rotas frontend para listagem e detalhe de subcategorias, tipos de solicitacao e locais/unidades;
- listagens com busca por nome, filtro por status e paginacao;
- acoes de editar, inativar e reativar com confirmacao de usuario;
- formularios de cadastro com validacoes de regras obrigatorias de negocio;
- prioridade atualizada para uso de `Peso` e `Cor`;
- consumo preferencial dos endpoints `api/admin/*` na camada de services do frontend.

Checklist Sprint 4:
- [x] Menu Admin > Cadastros atualizado
- [x] Tela de Departamentos
- [x] Tela de Categorias
- [x] Tela de Subcategorias
- [x] Tela de Prioridades
- [x] Tela de Tipos de Solicitacao
- [x] Tela de Locais / Unidades
- [x] Services de API frontend atualizados
- [x] Rotas frontend criadas/atualizadas
- [x] Busca e filtro por status funcionando
- [x] Ativacao/Inativacao com confirmacao
- [x] Feedback visual de sucesso e erro
- [x] Estados de carregamento e lista vazia
- [x] Documentacao atualizada

Pendencias evolutivas:
- homologacao funcional com usuarios administrativos;
- testes E2E do fluxo de cadastros;
- integracao dos novos cadastros no fluxo de chamados (fora desta sprint).

## Sprint Comentarios no Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] API `GET /api/chamados/{chamadoId}/comentarios` criada/ajustada.
- [x] API `POST /api/chamados/{chamadoId}/comentarios` criada/ajustada.
- [x] Regras por perfil (Administrador, Atendente, Solicitante) aplicadas.
- [x] Solicitante bloqueado para comentario interno.
- [x] Solicitante sem visao de comentario interno.
- [x] Ordenacao cronologica crescente aplicada.
- [x] Validacao de mensagem obrigatoria e limite de 4000 caracteres.
- [x] Frontend de detalhe do chamado com envio de comentarios atualizado.
- [x] Testes automatizados backend/frontend executados.
- [x] Migration incremental aplicada com alteracoes reais.
- [x] Documentacao do modulo de atendimento atualizada.

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/ComentariosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Sprint Anexos no Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] `GET /api/chamados/{chamadoId}/anexos`
- [x] `POST /api/chamados/{chamadoId}/anexos`
- [x] `GET /api/chamados/{chamadoId}/anexos/{anexoId}/download`
- [x] validacoes de seguranca de upload implementadas
- [x] controle de acesso por perfil e por chamado aplicado
- [x] caminho fisico e nome armazenado nao expostos na API
- [x] upload/listagem/download refletidos no frontend de detalhe
- [x] testes backend e frontend executados
- [x] build frontend executado
- [x] **nenhum endpoint DELETE de anexo exposto**
- [x] **nenhum botao de exclusao de anexo criado**

Regra de rastreabilidade aplicada:
- Anexos enviados permanecem como evidencia do atendimento e nao possuem fluxo de exclusao.

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/AnexosChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Sprint Historico e Linha do Tempo do Atendimento - Conclusao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Checklist da sprint:
- [x] `GET /api/chamados/{chamadoId}/linha-do-tempo`
- [x] linha do tempo consolidada com abertura, comentarios, anexos e historico
- [x] visibilidade por perfil aplicada (`Administrador`, `Atendente`, `Solicitante`)
- [x] solicitante sem comentarios internos e sem eventos internos sensiveis
- [x] evento de anexo na timeline com download
- [x] sem exposicao de `Caminho` e `NomeArquivoArmazenado`
- [x] atualizacao de timeline apos comentario e upload de anexo
- [x] sem endpoint DELETE de anexo
- [x] sem botao de exclusao de anexo
- [x] testes backend/frontend executados
- [x] build frontend executado

Evidencias:
- `docs/ATENDIMENTO.md`
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/LinhaTempoChamadoUseCases.cs`
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`

## Item de Roadmap - Comentarios e Anexos (Atendimento)

Status final do item:
- Area: Atendimento
- Nome: Comentarios e anexos
- StatusImplementacao: Implementado funcionalmente
- StatusTecnico: Completo
- PercentualImplementacao: 100
- SituacaoAtual: Implementado
- Avaliacao: Aprovado

Checklist consolidado:
- grupo Comentarios: concluido
- grupo Anexos: concluido
- grupo Governanca: concluido

Pendencias:
- tecnicas: nenhuma pendencia bloqueante
- homologacao: validar formalmente em ambiente de homologacao com usuarios reais, se ainda nao houver evidencia formal

Regra obrigatoria mantida:
- anexo salvo no atendimento nao pode ser excluido por nenhum perfil;
- nao existe endpoint DELETE de anexo;
- nao existe botao de exclusao de anexo.

## Sprint Cadastros Administrativos 5 - Integracao com Chamados

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Conectar os cadastros administrativos ao ciclo operacional do chamado (abertura, classificacao, triagem, detalhe e filtros), garantindo uso de ativos em novas operacoes e preservacao historica de inativos.

Entregas consolidadas:
- entidade `Chamado` evoluida com `SubcategoriaId`, `TipoSolicitacaoId` e `LocalUnidadeId`;
- migration `20260515212153_Sprint5IntegracaoCadastrosChamados`;
- validacoes de negocio para ativos e vinculo categoria/subcategoria;
- contexto de portal e admin com subcategorias/tipos/locais ativos;
- filtros administrativos por categoria, subcategoria, prioridade, tipo, departamento e local/unidade;
- detalhe de chamado (portal e admin) exibindo nomes dos cadastros vinculados;
- endpoints operacionais de consulta ativa em `/api/cadastros/*`.

Checklist:
- [x] abertura carrega categorias ativas
- [x] subcategorias filtradas por categoria
- [x] prioridades ativas disponiveis
- [x] tipos de solicitacao ativos disponiveis
- [x] locais/unidades ativos disponiveis
- [x] inativos bloqueados para novas selecoes
- [x] historico de chamados antigos preservado
- [x] filtros administrativos atualizados
- [x] detalhe do chamado com novos nomes vinculados
- [x] build backend/frontend e testes backend sem erro

Pendencias evolutivas:
- ampliar automacao de testes frontend para fluxos completos de triagem;
- avaliar evolucao de departamento em dois papeis (solicitante x responsavel) em sprint futura.

## Sprint Cadastros Administrativos 6 - Seed Inicial, Testes e Fechamento

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar o modulo de cadastros com massa inicial idempotente, revisao de testes e fechamento documental para validacao funcional.

Resumo:
- seed inicial dos principais cadastros administrativos consolidado no `DevelopmentSeedService`;
- protecao contra duplicidade por normalizacao de nomes (inclusive variacoes de acentuacao);
- prioridades padrao consolidadas com peso/cor definidos para a operacao;
- subcategorias padrao consolidadas por categoria com vinculo correto;
- validacao de endpoints operacionais `/api/cadastros/*` para retorno somente de ativos;
- validacao do filtro operacional de subcategorias ativas por categoria;
- documentacao final da trilha de cadastros atualizada.

Checklist Sprint 6:
- [x] Seed inicial aplicado sem duplicidade
- [x] Testes automatizados revisados e passando
- [x] Fluxo operacional validado com ativos/inativos
- [x] Documentacao finalizada
- [x] Roadmap atualizado

Pendencias evolutivas:
- evoluir para seed configuravel por ambiente institucional;
- ampliar testes frontend automatizados para fluxo completo de abertura e triagem.

## Sprint Cadastros Administrativos 7 - Checklist Funcional e Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Finalizar a validacao funcional da trilha de cadastros administrativos integrada ao fluxo de chamados, com foco em consistencia de regras de ativo/inativo e leitura historica.

Resumo:
- checklist tecnico funcional revisado e coberto por testes de use case/integracao;
- filtros administrativos de cadastros validados com `Ativo`, `Inativo` e `Todos`;
- validacoes de abertura e classificacao de chamados mantidas (categoria/subcategoria/prioridade/tipo/local/departamento quando aplicavel);
- validacao de historico preservado para chamados antigos com cadastro inativo;
- ajustes finos de validadores (cor hexadecimal de prioridade e categoria obrigatoria para subcategoria);
- documentacao de homologacao funcional consolidada.

Checklist Sprint 7:
- [x] checklist funcional revisado
- [x] ajustes finos aplicados
- [x] testes executando com sucesso
- [x] build backend OK
- [x] build frontend OK
- [x] documentacao atualizada
- [x] modulo validado funcionalmente

Pendencias evolutivas:
- homologacao manual com evidencias visuais formais em ambiente institucional;
- suite frontend automatizada/E2E de cobertura visual ponta a ponta.

## Sprint Cadastros Administrativos 8 - Consolidacao ITSM e Checklist de Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar a governanca documental do modulo de cadastros administrativos e formalizar o checklist de homologacao para validacao institucional.

Checklist Sprint 8:
- [x] documento ITSM especifico dos cadastros administrativos criado
- [x] checklist de homologacao funcional criado
- [x] documentacao de cadastros atualizada com o fechamento da sprint
- [x] roadmap geral atualizado
- [x] roadmap ITSM atualizado

Evidencias documentais:
- `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md`
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`
- `docs/CADASTROS-ADMINISTRATIVOS.md`

Pendencias evolutivas:
- execucao manual do checklist em ambiente de homologacao com usuarios reais;
- formalizacao de aceite funcional e registro de evidencias visuais.

## Cadastros Administrativos no Roadmap ITSM

Area:
- Cadastros administrativos

Categoria:
- Cadastros

Ordem:
- 8

Objetivo:
Disponibilizar cadastros administrativos parametrizaveis para apoiar a classificacao, priorizacao, organizacao, triagem, filtros, historico e evolucao ITSM do sistema de chamados. O modulo deve contemplar departamentos, categorias, subcategorias, prioridades, tipos de solicitacao, locais/unidades e demais cadastros estruturais necessarios para a operacao do service desk.

Situacao atual:
Modulo de Cadastros Administrativos implementado e validado funcionalmente em nivel tecnico. Backend, frontend administrativo, integracao com abertura/gestao de chamados, seed inicial e validacao funcional foram concluidos. A homologacao institucional/manual com evidencias formais permanece pendente.

Apoio ITSM direto:
- Classificacao de chamados.
- Triagem.
- Priorizacao.
- Historico.
- Filtros administrativos.
- Futuro catalogo de servicos.
- Futuro SLA avancado.
- Relatorios por categoria, prioridade e localidade.
- Gestao de conhecimento.

Atencao tecnica:
Verificar se todos os cadastros permitirao ativacao/inativacao sem exclusao fisica, evitando perda de historico em chamados antigos. Validar quais cadastros serao parametrizaveis pela area administrativa e se o status do chamado permanecera como fluxo controlado do sistema ou se sera tratado futuramente como cadastro configuravel. Priorizar inativacao logica, validacao de duplicidade, uso apenas de registros ativos em novas operacoes e preservacao historica.

Status da implementacao:
- Fluxo funcional validado

Status tecnico:
- Aguardando homologacao institucional

Percentual (%):
- 90

Checklist:
- 7/8 concluidos
- [x] Criar documentacao ITSM.
- [x] Criar checklist de homologacao.
- [x] Implementar backend dos cadastros.
- [x] Implementar frontend administrativo.
- [x] Integrar cadastros com abertura de chamados.
- [x] Criar seed inicial.
- [x] Validar fluxo funcional.
- [ ] Homologar em ambiente institucional.

Pendencias tecnicas:
- Nao ha pendencias tecnicas bloqueantes identificadas para o modulo.
- Manter como evolucao futura a cobertura frontend E2E completa.
- Avaliar futuramente se status de chamado continuara como fluxo controlado ou se sera parametrizado em cadastro proprio.

Pendencias de homologacao:
- Executar homologacao institucional/manual.
- Coletar evidencias formais de tela.
- Registrar responsavel pela homologacao.
- Registrar data da homologacao.
- Registrar ambiente utilizado.
- Registrar resultado final: aprovado, aprovado com ressalvas ou reprovado.

Evidencia da implementacao:
Documentacao criada:
- `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md`
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`

Documentacao atualizada:
- `docs/CADASTROS-ADMINISTRATIVOS.md`
- `docs/ROADMAP.md`
- `docs/ROADMAP-ITSM.md`

Validacoes tecnicas:
- Backend dos cadastros implementado e validado.
- Frontend administrativo implementado e validado.
- Integracao com abertura e gestao de chamados validada.
- Seed inicial validado.
- Fluxo funcional validado.
- dotnet build OK.
- dotnet test OK com 420 testes aprovados.
- npm build OK.

Homologacao institucional (Item 8) - situacao real:
- roteiro formal de homologacao manual registrado em `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`;
- evidencias obrigatorias (prints, responsavel, data, ambiente e resultado) documentadas;
- sem evidencias institucionais anexadas nesta etapa, status mantido em `90%` e `7/8`.

Somente arquivos `docs/*.md` foram alterados nesta etapa.

Data de conclusao tecnica:
- (deixar em branco)

Data de homologacao:
- (deixar em branco)

Criterio de aceite:
- Documentacao ITSM criada.
- Checklist de homologacao criado.
- Backend dos cadastros implementado e validado.
- Frontend administrativo implementado e validado.
- Cadastros integrados ao fluxo de abertura e gestao de chamados.
- Seed inicial criado e validado.
- Fluxo funcional validado tecnicamente.
- Registros ativos usados em novas operacoes.
- Registros inativos preservados para historico.
- Homologacao institucional pendente como aceite formal final.

Proxima acao:
Executar homologacao institucional/manual com evidencias formais, incluindo prints das telas administrativas, abertura de chamado com cadastros, detalhe do chamado, filtros administrativos, responsavel, data, ambiente e resultado da validacao.

## Dashboard / Gest�o - Consolida��o t�cnica

Area:
- Dashboard

Categoria:
- Gest�o

Ordem:
- 9

Objetivo:
Disponibilizar uma vis�o gerencial da opera��o de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no per�odo, chamados sem respons�vel, riscos de SLA, distribui��o por status, prioridade, categoria, produtividade por atendente e situa��o da integra��o de e-mail.

Situacao atual:
Dashboard administrativo implementado funcionalmente no backend e frontend. A API disponibiliza indicadores consolidados, filtros por per�odo e contexto administrativo. A interface apresenta cards gerenciais, gr�ficos/listagens por status, prioridade e categoria, indicadores de SLA, produtividade por atendente, fila de chamados e resumo da integra��o de e-mail. Pendente valida��o com usu�rios reais, refinamento visual final, testes frontend/e2e e homologa��o institucional.

Status da implementacao:
- Implementado funcionalmente

Status tecnico:
- Completo com pend�ncias evolutivas

Percentual:
- 87%

Pendencias tecnicas:
- Aplicar ou validar permiss�o granular `Dashboard.Visualizar` no backend, al�m da prote��o por perfil.
- Validar performance com volume maior de chamados.
- Criar ou consolidar testes automatizados espec�ficos do dashboard em n�vel HTTP.
- Criar testes frontend/e2e para `dashboardAdminService` e `AdminDashboardView`, se o projeto j� tiver estrutura para isso.
- Avaliar cache ou otimiza��o das consultas agregadas, caso necess�rio.
- Revisar regras de permiss�o dos indicadores por perfil.

Pendencias de homologacao:
- Validar com Administrador.
- Validar com Atendente.
- Conferir n�meros do dashboard contra consultas reais no banco.
- Validar filtros por per�odo, departamento, categoria e respons�vel.
- Confirmar se os indicadores atendem � necessidade de gest�o da opera��o.
- Registrar evid�ncias formais de homologa��o.

Evidencias:
- `src/SGX.SistemaChamado.Api/Controllers/AdminDashboardController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminIndicadoresUseCases.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminDashboardDtos.cs`
- `src/SGX.SistemaChamado.Web/src/services/dashboardAdminService.ts`
- `src/SGX.SistemaChamado.Web/src/types/dashboard.ts`
- `src/SGX.SistemaChamado.Web/src/views/AdminDashboardView.vue`
- `tests/SGX.SistemaChamado.Tests/DashboardAdminUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/IndicadoresUseCaseTests.cs`
- `docs/DASHBOARD-GESTAO.md`

Crit�rio de aceite:
O usu�rio autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da opera��o. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, pr�ximos do vencimento e resolvidos no per�odo. A tela deve permitir navega��o para fila de chamados, gest�o de chamados e integra��o de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.

Pr�xima a��o:
Executar valida��o t�cnica e homologa��o funcional do dashboard com dados reais ou massa simulada mais pr�xima da opera��o institucional.

Checklist (40 itens):
- 34 conclu�dos
- 6 pendentes
- Itens pendentes: policy granular de dashboard, performance agregada, testes HTTP de sucesso, teste frontend/e2e, registro formal de evid�ncias e homologa��o com usu�rios.

## Sprint Base de Conhecimento 1 - Fundacao tecnica

Area: Base de Conhecimento
Categoria: Conhecimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Modelagem inicial concluida

Objetivo:
Estabelecer a base tecnica do modulo de Base de Conhecimento para suportar o ciclo de vida de artigos, controle de visibilidade e vinculacao com chamados, mantendo rastreabilidade e preservacao historica.

Entregas da sprint:
- entidades `BaseConhecimentoArtigo` e `ChamadoArtigoConhecimento`;
- enums `StatusArtigoConhecimento` e `VisibilidadeArtigoConhecimento`;
- migration incremental com tabelas `base_conhecimento_artigos` e `chamados_artigos_conhecimento`;
- indices e relacionamentos com `usuarios`, `chamados` e `categorias_chamado`;
- permissoes granulares `BaseConhecimento.Visualizar`, `BaseConhecimento.Gerenciar`, `BaseConhecimento.Publicar`, `BaseConhecimento.Arquivar` e `BaseConhecimento.VincularChamado`.

Regras consolidadas:
- sem exclusao fisica de artigos;
- inativacao logica aplicada no arquivamento;
- preservacao historica de artigos e vinculos com chamados.

Pendencias evolutivas:
- implementar casos de uso e endpoints administrativos/operacionais;
- implementar fluxo completo de revisao, publicacao e arquivamento;
- implementar UX administrativa e portal;
- ampliar testes automatizados do modulo.

## Sprint Base de Conhecimento 2 - CRUD administrativo backend

Area: Base de conhecimento
Categoria: Conhecimento

Status da implementacao: Backend administrativo em implementacao
Status tecnico: Sprint 2 concluida apos validacao
Percentual sugerido apos conclusao: 45%

Objetivo:
Concluir a camada backend administrativa da Base de Conhecimento, mantendo governanca, rastreabilidade e preservacao historica.

Entregas consolidadas:
- contratos de API (DTOs/requests) para administracao de artigos;
- casos de uso de listagem, detalhe, criacao, edicao, publicacao, arquivamento e reativacao;
- controller administrativo dedicado com endpoints REST;
- aplicacao de permissoes `BaseConhecimento.Visualizar`, `BaseConhecimento.Gerenciar`, `BaseConhecimento.Publicar` e `BaseConhecimento.Arquivar`;
- geracao de slug unico e validacoes de transicao de status;
- auditoria de criar, editar, publicar, arquivar e reativar;
- testes automatizados de regras de negocio e autorizacao.

Checklist Sprint 2:
- [x] Endpoints administrativos da base de conhecimento implementados.
- [x] Filtros administrativos com paginacao e ordenacao implementados.
- [x] Regras de publicacao/arquivamento/reativacao implementadas.
- [x] Exclusao fisica de artigo mantida como proibida.
- [x] Auditoria integrada ao modulo.
- [x] Cobertura de testes da sprint implementada.
- [x] Documentacao de Base de Conhecimento e roadmap atualizada.

Pendencias evolutivas:
- vincular artigos ao fluxo operacional de chamados na API de atendimento;
- criar telas administrativas e de portal para consumo dos endpoints;
- executar homologacao funcional institucional do modulo.

## Sprint Base de Conhecimento 3 - Consulta no portal

Area: Base de conhecimento
Categoria: Conhecimento

Status da implementacao: Backend administrativo e consulta do portal implementados
Status tecnico: Sprint 3 concluida apos validacao
Percentual sugerido apos conclusao: 62%

Objetivo:
Permitir consulta da Base de Conhecimento no portal com seguranca por perfil e sem exposicao de artigos internos para usuarios nao autorizados.

Entregas consolidadas:
- contratos de portal (`PortalFiltroBaseConhecimentoRequest`, `PortalBaseConhecimentoArtigoListagemDto`, `PortalBaseConhecimentoArtigoDetalheDto`);
- casos de uso de listagem e detalhe por slug;
- controller de portal dedicado para Base de Conhecimento;
- filtros de busca por titulo, resumo, conteudo e tags;
- filtro por categoria com paginacao;
- retorno exclusivo de artigos `Publicado` e `Ativo`;
- visibilidade aplicada:
  - `Solicitante`: solicitante, atendente e administrador
  - `Atendente`: atendente e administrador
  - `Administrador`: somente administrador
- resposta `404` para slug inexistente, artigo nao publicado/inativo ou sem visibilidade;
- cobertura de testes para regras de publicacao/status/visibilidade e endpoints do portal.

Checklist Sprint 3:
- [x] Endpoints de consulta no portal implementados.
- [x] Exposicao restrita a artigos publicados e ativos.
- [x] Listagem resumida sem conteudo completo.
- [x] Detalhe por slug com conteudo completo.
- [x] Regras de visibilidade por perfil implementadas.
- [x] Testes automatizados da sprint implementados.
- [x] Documentacao atualizada.

Pendencias evolutivas:
- evoluir vinculacao operacional de artigos com chamados;
- evoluir UX/frontend administrativo e portal;
- realizar homologacao institucional com evidencias formais.

## Sprint Base de Conhecimento 4 - Frontend administrativo e portal

Area: Base de conhecimento
Categoria: Conhecimento

Status da implementacao: Frontend administrativo e portal implementados
Status tecnico: Sprint 4 concluida apos validacao
Percentual sugerido apos conclusao: 75%

Objetivo:
Concluir a camada frontend da Base de Conhecimento no SGX com navegacao administrativa e consulta no portal.

Escopo validado:
- rotas administrativas:
  - `/admin/conhecimento/base-conhecimento`
  - `/admin/conhecimento/base-conhecimento/novo`
  - `/admin/conhecimento/base-conhecimento/:id`
- rotas de portal:
  - `/portal/base-conhecimento`
  - `/portal/base-conhecimento/:slug`
- listagem administrativa com filtros por termo/status/visibilidade/categoria/ativo;
- formulario administrativo com validacoes de titulo, conteudo e visibilidade;
- acoes administrativas com confirmacao: publicar, arquivar e reativar;
- listagem de portal em cards e detalhe por slug;
- exibicao de loading, erro e estado vazio no admin e no portal;
- consumo dos endpoints backend sem novos contratos fora da Sprint 1-3.

Checklist Sprint 4:
- [x] Tipos e services frontend do modulo criados.
- [x] Telas administrativas do modulo criadas.
- [x] Telas de portal do modulo criadas.
- [x] Rotas e menus do modulo integrados.
- [x] Tratamento de estados e erros implementado.
- [x] Documentacao atualizada.

Pendencias evolutivas:
- homologacao institucional com usuarios reais;
- testes e2e para cenarios de navegacao e publicacao;
- evolucao futura da vinculacao operacional artigo x chamado.

## Sprint Base de Conhecimento 5 - Integracao com chamados

Area: Base de conhecimento
Categoria: Conhecimento

Status da implementacao: Integrado ao fluxo de chamados
Status tecnico: Sprint 5 concluida apos validacao
Percentual sugerido apos conclusao: 87%

Objetivo:
Integrar a Base de Conhecimento ao atendimento administrativo de chamados, permitindo vincular artigos publicados ao chamado e consultar/remover vinculos com trilha de historico e auditoria.

Entregas consolidadas:
- use cases de listagem, vinculo, remocao e busca de artigos disponiveis para vinculo;
- endpoints administrativos:
  - `GET /api/admin/chamados/{chamadoId}/artigos-conhecimento`
  - `GET /api/admin/chamados/{chamadoId}/artigos-conhecimento/disponiveis`
  - `POST /api/admin/chamados/{chamadoId}/artigos-conhecimento/{artigoId}`
  - `DELETE /api/admin/chamados/{chamadoId}/artigos-conhecimento/{artigoId}`
- validacoes de negocio:
  - somente artigo `Publicado` e `Ativo` pode ser vinculado;
  - artigo arquivado, inativo e nao publicado e bloqueado;
  - duplicidade de vinculo chamada + artigo e bloqueada;
- historico de chamado e auditoria para vinculo/remocao;
- secao \"Base de conhecimento\" no detalhe administrativo do chamado com fluxo de vincular/remover;
- service frontend dedicado para integracao chamado x conhecimento.

Checklist Sprint 5:
- [x] Integracao backend chamada-artigo implementada.
- [x] Integracao frontend no detalhe administrativo implementada.
- [x] Seguranca por permissao `BaseConhecimento.VincularChamado` aplicada.
- [x] Regras de bloqueio de vinculo invalido implementadas.
- [x] Auditoria e historico de vinculo/remocao implementados.
- [x] Build backend/frontend validado.
- [x] Testes automatizados da sprint validados.
- [x] Checklist de homologacao da Base de Conhecimento criado/atualizado.

Pendencias evolutivas:
- homologacao institucional manual com usuarios reais;
- coleta de evidencias formais de aceite;
- ampliacao de cobertura e2e para jornada completa de conhecimento + chamado.

## Sprint Base de Conhecimento 6 - Fechamento da entrega

Area: Base de conhecimento
Categoria: Conhecimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%

Objetivo:
Consolidar o fechamento funcional da Base de Conhecimento com checklist de homologacao completo, estrutura formal de evidencias e revisao final de UX e seguranca.

Entregas consolidadas:
- atualizacao do checklist de homologacao com cenarios completos de admin, portal e chamado;
- criacao da estrutura `docs/evidencias/base-conhecimento/README.md` para registro de validacoes;
- validacao de que nao ha framework E2E instalado nesta sprint (Playwright/Cypress inexistentes no projeto);
- registro da pendencia E2E como evolucao futura, mantendo cobertura atual por testes unitarios/integrados;
- revisao final de seguranca das regras de exposicao no portal, autorizacao admin e vinculo de chamado;
- consolidacao da documentacao tecnica final do modulo.

Checklist Sprint 6:
- [x] Checklist funcional completo.
- [x] Evidencias formais preparadas.
- [x] Revisao de seguranca concluida.
- [x] Revisao de UX concluida.
- [x] Documentacao final consolidada.
- [x] Atualizacao final do roadmap aplicada.

Pendencias evolutivas:
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos, quando houver framework institucional;
- versionamento de artigos;
- workflow formal de aprovacao;
- anexos em artigos;
- avaliacao de utilidade do artigo;
- relatorio de artigos mais acessados;
- sugestao automatica de artigos durante abertura do chamado;
- sugestao automatica de artigos durante atendimento;
- busca semantica/IA.

## Sprint Catalogo de Servicos 1 - Fundacao tecnica

Area: Catalogo de Servicos
Categoria: Conhecimento

Status da implementacao: Fundacao tecnica implementada
Status tecnico: Sprint 1 concluida apos validacao
Percentual: 20%

Objetivo:
Estabelecer a base tecnica do Catalogo de Servicos como modulo institucional e multiarea, permitindo evolucao controlada para publicacao de servicos solicitaveis por diferentes departamentos.

Escopo entregue:
- entidade `CatalogoServico` no dominio;
- enums `StatusCatalogoServico` (`Rascunho`, `Publicado`, `Arquivado`) e `VisibilidadeCatalogoServico` (`Interno`, `Solicitante`, `Atendente`, `Administrador`);
- relacionamento obrigatorio com `Departamento`;
- relacionamentos opcionais com categoria, subcategoria, prioridade, politica de SLA e artigo da base de conhecimento;
- mapeamento EF Core da tabela `catalogo_servicos`;
- indice unico de `slug` e indices por departamento, status e ativo;
- migration `Sprint1CatalogoServicosFundacao`;
- permissoes `CatalogoServicos.Visualizar`, `CatalogoServicos.Gerenciar`, `CatalogoServicos.Publicar`, `CatalogoServicos.Arquivar` com seed atualizado;
- atualizacao de constantes de permissao no frontend;
- documentacao inicial em `docs/CATALOGO-SERVICOS.md`.

Regras tecnicas consolidadas:
- modulo nao restrito a TI;
- sem exclusao fisica de servico;
- arquivamento com preservacao historica;
- exposicao para solicitantes condicionada a servico ativo e publicado nas proximas camadas de consulta.

## Sprint Catalogo de Servicos 2 - CRUD administrativo backend

Area: Catalogo de Servicos
Categoria: Conhecimento

Status da implementacao: CRUD administrativo implementado
Status tecnico: Sprint 2 concluida apos validacao
Percentual: 45%

Objetivo:
Implementar o CRUD administrativo backend do Catalogo de Servicos com governanca de permissao, transicoes de status, auditoria e cobertura de testes.

Entregas consolidadas:
- contratos administrativos do modulo (`FiltroCatalogoServicoRequest`, `CatalogoServicoListagemDto`, `CatalogoServicoDetalheDto`, `CriarCatalogoServicoRequest`, `AtualizarCatalogoServicoRequest`);
- interface `IAdminCatalogoServicosUseCases`;
- implementacao `CatalogoServicosAdminUseCases` com operacoes de listar, obter, criar, atualizar, publicar, arquivar e reativar;
- controller `AdminCatalogoServicosController` com endpoints em `/api/admin/catalogo-servicos`;
- aplicacao de permissoes granulares `CatalogoServicos.*` por endpoint;
- validacoes de negocio para dados obrigatorios, relacionamentos opcionais e regras de ciclo de vida;
- slug automatico e unico com sufixo incremental;
- auditoria de criacao, edicao, publicacao, arquivamento e reativacao;
- testes automatizados do modulo e de autorizacao HTTP.

Checklist Sprint 2:
- [x] CRUD administrativo backend implementado.
- [x] Endpoints administrativos implementados.
- [x] Permissoes `CatalogoServicos.*` aplicadas.
- [x] Slug unico implementado.
- [x] Sem exclusao fisica de servicos.
- [x] Auditoria nas operacoes principais implementada.
- [x] Testes automatizados adicionados e validados.
- [x] Documentacao atualizada sem duplicar item de roadmap.

Pendencias evolutivas:
- frontend administrativo do catalogo;
- abertura guiada de chamado por servico;
- homologacao institucional com evidencias formais.
## Sprint Catalogo de Servicos 3 - Consulta do portal
Area: Catalogo de Servicos
Categoria: Conhecimento
Status da implementacao: Consulta do portal implementada
Status tecnico: Sprint 3 concluida apos validacao
Percentual: 62%
Objetivo:
Habilitar consulta de servicos publicados no portal com controle de visibilidade por perfil, mantendo o catalogo institucional e multiarea.
Entregas consolidadas:
- contratos de portal (PortalFiltroCatalogoServicoRequest, PortalCatalogoServicoListagemDto, PortalCatalogoServicoDetalheDto);
- interface IPortalCatalogoServicosUseCases;
- implementacao CatalogoServicosPortalUseCases para listagem e detalhe por slug;
- controller PortalCatalogoServicosController com endpoints:
  - GET /api/portal/catalogo-servicos
  - GET /api/portal/catalogo-servicos/{slug};
- retorno restrito a servicos Publicado e Ativo;
- filtros por termo, departamento, categoria, subcategoria e permite abertura;
- paginacao e ordenacao por departamento, ordem e nome;
- regras de visibilidade backend para Solicitante, Atendente, Administrador e Interno;
- retorno 404 para slug inexistente, servico inelegivel ou sem visibilidade;
- testes automatizados de use case e integracao HTTP.
Checklist Sprint 3:
- [x] Consulta do portal implementada.
- [x] Listagem restrita a servicos publicados e ativos.
- [x] Visibilidade por perfil aplicada no backend.
- [x] Endpoints de portal publicados.
- [x] Filtros e paginacao implementados.
- [x] Detalhe por slug com 404 para cenarios inelegiveis implementado.
- [x] Testes automatizados da sprint adicionados e validados.
- [x] Documentacao atualizada sem duplicidade de roadmap.
Pendencias evolutivas:
- frontend administrativo do catalogo;
- abertura guiada de chamado por servico;
- homologacao institucional com evidencias formais.

## Sprint Catalogo de Servicos 4 - Frontend administrativo e portal

Area: Catalogo de Servicos
Categoria: Conhecimento

Status da implementacao: Frontend administrativo e portal implementados
Status tecnico: Sprint 4 concluida apos validacao
Percentual: 75%

Objetivo:
Concluir a camada frontend do Catalogo de Servicos no SGX com experiencia administrativa completa e consulta no portal autenticado.

Entregas consolidadas:
- types TypeScript do modulo (src/types/catalogoServicos.ts) para contratos admin e portal;
- services frontend catalogoServicosAdminService.ts e catalogoServicosPortalService.ts;
- telas administrativas em /admin/conhecimento/catalogo-servicos, /admin/conhecimento/catalogo-servicos/novo e /admin/conhecimento/catalogo-servicos/:id;
- acoes administrativas de publicar, arquivar e reativar com confirmacao;
- filtros administrativos com paginacao e estados de loading/erro/vazio;
- telas de portal em /portal/catalogo-servicos e /portal/catalogo-servicos/:slug;
- menu administrativo e menu do portal atualizados para o Catalogo de servicos;
- testes unitarios de services e testes minimos de views da listagem admin/portal.

Checklist Sprint 4:
- [x] Frontend administrativo implementado.
- [x] Frontend de consulta do portal implementado.
- [x] Rotas administrativas e de portal publicadas.
- [x] Menus administrativo e portal atualizados.
- [x] Acoes de ciclo de vida com confirmacao implementadas.
- [x] Filtros, cards e paginacao implementados.
- [x] Estados de loading, erro e vazio implementados.
- [x] Testes de services frontend do modulo implementados.
- [x] Documentacao do modulo e roadmaps atualizada.

Pendencias evolutivas:
- integrar abertura guiada de chamado por servico (Sprint 5);
- homologacao institucional com evidencias formais.


## Sprint Catalogo de Servicos 5 - Integracao com abertura de chamados

Area: Catalogo de Servicos
Categoria: Conhecimento

Status da implementacao: Integrado a abertura de chamados
Status tecnico: Sprint 5 concluida apos validacao
Percentual: 87%

Objetivo:
Conectar o Catalogo de Servicos ao fluxo de abertura de chamados do portal com validacao central no backend e aplicacao oficial dos dados do servico.

Entregas consolidadas:
- associacao opcional `CatalogoServicoId` em `Chamado`;
- migration `Sprint5CatalogoServicosChamado` com coluna, indice e chave estrangeira;
- endpoint `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`;
- abertura de chamado orientada por catalogo com validacoes de status, ativo, visibilidade e permissao de abertura;
- aplicacao backend de departamento, categoria, subcategoria, prioridade e politica SLA do servico quando configuradas;
- historico de chamado com evento `ChamadoCriadoPorCatalogoServico`;
- frontend do portal atualizado para preparar abertura a partir do detalhe e enviar `CatalogoServicoId`;
- mensagens amigaveis para servico somente consulta e falha na preparacao;
- testes backend e frontend cobrindo fluxo positivo e bloqueios.

Checklist Sprint 5:
- [x] Endpoint de preparar abertura implementado.
- [x] Validacoes de elegibilidade do servico implementadas.
- [x] Aplicacao backend dos dados oficiais do servico implementada.
- [x] Associacao `CatalogoServicoId` no chamado implementada.
- [x] Historico de abertura por catalogo implementado.
- [x] Integracao frontend detalhe -> novo chamado implementada.
- [x] Payload de abertura com `CatalogoServicoId` implementado.
- [x] Testes backend e frontend da sprint validados.
- [x] Documentacao de modulo e roadmaps atualizada.

Pendencias evolutivas:
- homologacao institucional com evidencias formais.
## Sprint Catalogo de Servicos 6 - Fechamento funcional

Area: Catalogo de Servicos
Categoria: Conhecimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%

Situacao atual:
Catalogo de Servicos implementado funcionalmente como modulo institucional multiarea. O modulo contempla fundacao tecnica, CRUD administrativo, frontend administrativo, consulta no portal, frontend do portal, controle de permissoes, visibilidade por perfil, integracao com abertura de chamados, associacao CatalogoServicoId ao chamado, aplicacao backend dos dados oficiais do servico, historico de abertura por catalogo, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras.

Entregas da Sprint 6:
- checklist de homologacao criado em `docs/CHECKLIST-HOMOLOGACAO-CATALOGO-SERVICOS.md`;
- estrutura de evidencias criada em `docs/evidencias/catalogo-servicos/README.md`;
- documentacao principal do modulo revisada e consolidada;
- revisao de UX registrada para listagem/form admin, listagem/detalhe portal e abertura de chamado;
- revisao de seguranca registrada sem relaxamento de regras;
- verificacao de framework E2E: nao identificado Playwright/Cypress nesta sprint;
- validacoes tecnicas executadas: build release, testes backend, testes unitarios frontend e build frontend.

Pendencias evolutivas:
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- formularios dinamicos por servico;
- campos obrigatorios por servico;
- workflow de aprovacao por servico;
- aprovacoes por departamento;
- indicadores de servicos mais solicitados;
- relatorios por departamento;
- SLA avancado por servico;
- automacao de triagem por servico;
- sugestao de artigos da Base de Conhecimento por servico;
- melhoria de encoding de `docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md`.
## Sprint Inventario/Ativos 1 - Fundacao tecnica

Area: Inventario/Ativos
Categoria: Infraestrutura

Status da implementacao: Fundacao tecnica implementada
Status tecnico: Sprint 1 concluida apos validacao
Percentual: 20%

Objetivo:
Criar a fundacao tecnica do modulo Inventario/Ativos para cadastro e rastreabilidade de ativos de infraestrutura, preparando evolucoes de vinculo com chamados, usuarios, departamentos, locais/unidades e historico de movimentacoes.

Entregas consolidadas:
- entidade `InventarioAtivo` com identificacao por `Codigo` e trilha de inativacao logica;
- entidade `TipoAtivoInventario` com seed inicial de tipos de ativo;
- enums `StatusOperacionalAtivo`, `StatusPatrimonialAtivo` e `CriticidadeAtivo`;
- tabelas `inventario_ativos` e `tipos_ativo_inventario` via migration `Sprint1InventarioAtivosFundacao`;
- indices de busca e unicidade (incluindo filtros para `NumeroPatrimonio` e `NumeroSerie` quando preenchidos);
- permissoes granulares `InventarioAtivos.*` com seed e constantes frontend atualizadas;
- documentacao inicial do modulo em `docs/INVENTARIO-ATIVOS.md`.

Regras consolidadas:
- sem exclusao fisica de ativo;
- inativacao preserva historico;
- `Codigo` e identificador institucional do ativo no SGX;
- `NumeroPatrimonio` e `NumeroSerie` permanecem opcionais para atender ativos sem tombamento/serie conhecida.

Pendencias evolutivas:
- CRUD administrativo completo e filtros operacionais;
- trilha de movimentacoes do ativo;
- vinculo com chamados e historico de recorrencia;
- homologacao institucional com evidencias formais.

## Sprint Inventario/Ativos 2 - CRUD administrativo backend

Area: Inventario/Ativos
Categoria: Infraestrutura

Status da implementacao: CRUD administrativo implementado
Status tecnico: Sprint 2 concluida apos validacao
Percentual: 45%

Objetivo:
Implementar o CRUD administrativo backend do modulo Inventario/Ativos com governanca de permissao, validacoes de negocio, auditoria e cobertura de testes.

Entregas consolidadas:
- contratos administrativos para filtro, listagem, detalhe, criacao e atualizacao;
- interface `IAdminInventarioAtivosUseCases`;
- implementacao `InventarioAtivosAdminUseCases` com listar, obter, criar, atualizar, inativar, reativar e listar tipos;
- controller `AdminInventarioAtivosController` e rotas em `/api/admin/inventario-ativos`;
- validacoes de unicidade para codigo, numero de patrimonio e numero de serie;
- validacoes de existencia/atividade para tipo, departamento, local e usuario responsavel;
- inativacao e reativacao com preservacao historica;
- auditoria completa de operacoes criticas;
- testes automatizados de use case e autorizacao HTTP.

Permissoes aplicadas:
- `InventarioAtivos.Visualizar` para leitura e tipos;
- `InventarioAtivos.Gerenciar` para criacao e atualizacao;
- `InventarioAtivos.Inativar` para inativacao e reativacao.

Pendencias evolutivas:
- modulo de movimentacao do ativo;
- integracao com fluxo de chamados por ativo;
- consolidacao de indicadores operacionais;
- homologacao institucional com evidencias formais.
## Sprint Inventario/Ativos 3 - Historico e movimentacao

Area: Inventario/Ativos
Categoria: Infraestrutura

Status da implementacao: Historico e movimentacao implementados
Status tecnico: Sprint 3 concluida apos validacao
Percentual: 62%

Objetivo:
Garantir rastreabilidade operacional dos ativos de inventario por meio de historico estruturado e movimentacao administrativa com trilha de alteracoes.

Entregas consolidadas:
- modelo `HistoricoInventarioAtivo` com tabela `historico_inventario_ativos`;
- `TipoMovimentacaoAtivo` para classificar eventos de ativo;
- listagem de historico paginada por ativo;
- movimentacao com atualizacao do cadastro e registro consolidado no historico;
- registro automatico de historico em criacao, edicao relevante, inativacao e reativacao;
- manutencao da auditoria existente como camada complementar;
- testes automatizados de comportamento e seguranca dos endpoints.

Permissoes aplicadas:
- `InventarioAtivos.Visualizar`;
- `InventarioAtivos.Movimentar`.

Pendencias evolutivas:
- integracao do ativo com fluxo completo de chamados;
- consolidacao de indicadores operacionais;
- homologacao institucional com evidencias formais.

## Sprint Inventario/Ativos 4 - Integrado aos chamados

Area: Inventario/Ativos
Categoria: Infraestrutura

Status da implementacao: Integrado aos chamados
Status tecnico: Sprint 4 concluida apos validacao
Percentual: 75%

Objetivo:
Integrar o modulo Inventario/Ativos ao fluxo de chamados para permitir vinculo operacional de ativo, consulta historica de chamados por ativo e rastreabilidade completa de vinculacao/remocao.

Entregas consolidadas:
- Chamado com campo opcional InventarioAtivoId;
- migration Sprint4InventarioAtivosChamados;
- abertura de chamado via portal com validacao de ativo quando informado;
- bloqueio de abertura/vinculo com ativo inativo;
- endpoints administrativos de vinculo e remocao de ativo no chamado;
- endpoint administrativo para consultar chamados relacionados ao ativo;
- historico do chamado em vinculacao/remocao (AtivoVinculado e AtivoRemovido);
- historico do ativo em vinculacao/remocao (VinculoChamado e RemocaoVinculoChamado);
- permissao InventarioAtivos.VincularChamado aplicada em vincular/remover;
- permissao InventarioAtivos.Visualizar aplicada na consulta de chamados por ativo;
- auditoria de chamados mantida no ciclo de vinculo/remocao;
- testes backend ampliados, com suite total validada.

Checklist Sprint 4:
- [x] Chamado com vinculacao opcional a ativo implementado.
- [x] Migration de relacionamento chamado x ativo criada.
- [x] Abertura portal com ativo valido implementada.
- [x] Bloqueio para ativo inativo implementado.
- [x] Endpoints administrativos de vincular/remover implementados.
- [x] Endpoint de chamados por ativo implementado.
- [x] Historico de chamado para vinculo/remocao implementado.
- [x] Historico de ativo para vinculo/remocao implementado.
- [x] Permissoes aplicadas e testadas.
- [x] Documentacao atualizada sem duplicacao de item.

Pendencias evolutivas:
- evoluir regra patrimonial para bloqueio condicional por status patrimonial;
- ampliar indicadores/dashboards de ativos;
- homologacao institucional com evidencias formais.

## Sprint Inventario/Ativos 5 - Frontend administrativo
Area: Inventario/Ativos
Categoria: Infraestrutura
Status da implementacao: Frontend administrativo implementado
Status tecnico: Sprint 5 concluida apos validacao
Percentual: 87%
Objetivo:
Concluir a experiencia administrativa do modulo Inventario/Ativos no frontend, cobrindo listagem, cadastro, edicao, detalhe, inativacao, reativacao, movimentacao e consulta de chamados relacionados.
Entregas consolidadas:
- types do modulo em src/SGX.SistemaChamado.Web/src/types/inventarioAtivos.ts;
- service administrativo inventarioAtivosAdminService;
- service de vinculo ativo/chamado chamadoInventarioAtivoService;
- testes unitarios dos services de inventario e vinculo;
- telas administrativas:
  - InventarioAtivosListPage.vue;
  - InventarioAtivosFormPage.vue;
  - InventarioAtivosDetalhePage.vue;
- rotas administrativas de inventario e edicao;
- menu administrativo atualizado no agrupamento Infraestrutura;
- listagem com filtros, tabela, paginacao e acoes de ciclo de vida;
- formulario com validacoes de negocio alinhadas ao backend;
- detalhe do ativo com historico/movimentacoes e chamados relacionados;
- modal de movimentacao com validacao de alteracao efetiva;
- detalhe administrativo do chamado com secao de ativo vinculado, vincular/remover e link para ativo.
Checklist Sprint 5:
- [x] Frontend administrativo de inventario implementado.
- [x] Services frontend do modulo implementados.
- [x] Rotas e menu administrativo atualizados.
- [x] Detalhe administrativo do chamado integrado ao vinculo de ativo.
- [x] Permissoes frontend aplicadas.
- [x] Testes unitarios frontend atualizados e aprovados.
- [x] Build frontend aprovado.
- [x] Build/testes backend mantidos sem regressao.
- [x] Documentacao e roadmaps atualizados sem duplicacao de item.
Pendencias evolutivas:
- avaliar seletor de ativo na abertura de chamado (portal/admin) em sprint futura;
- ampliar testes de views do modulo;
- homologacao institucional com evidencias formais.

## Sprint Inventario/Ativos 6 - Fechamento funcional
Area: Inventario/Ativos
Categoria: Infraestrutura
Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%
Situacao atual:
Inventario/Ativos implementado funcionalmente como modulo de infraestrutura. O modulo contempla cadastro de ativos, tipos de ativo, inativacao logica, validacoes de codigo/patrimonio/serie, filtros administrativos, auditoria, historico operacional, movimentacao, vinculo com chamados, consulta de chamados relacionados, frontend administrativo, integracao visual com detalhe administrativo do chamado, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras.
Entregas de fechamento da sprint:
- checklist de homologacao em docs/CHECKLIST-HOMOLOGACAO-INVENTARIO-ATIVOS.md;
- estrutura de evidencias em docs/evidencias/inventario-ativos/README.md;
- documentacao principal consolidada em docs/INVENTARIO-ATIVOS.md;
- revisao de UX das telas administrativas e do detalhe administrativo do chamado;
- revisao de seguranca com confirmacao de politicas de permissao e bloqueios de ativo inativo;
- validacoes automatizadas mantidas sem regressao.
Checklist Sprint 6:
- [x] Checklist de homologacao criado.
- [x] Estrutura de evidencias criada.
- [x] Documentacao principal atualizada.
- [x] Roadmaps atualizados para 90%.
- [x] Revisao UX documentada.
- [x] Revisao de seguranca documentada.
- [x] Nenhuma regra de seguranca relaxada.
- [x] Sem duplicacao de item no roadmap.
- [x] Backend build Release validado.
- [x] Testes backend validados.
- [x] Frontend unit tests validados.
- [x] Frontend build validado.
Pendencias evolutivas:
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- seletor de ativo na abertura de chamado do portal/admin;
- importacao em massa de ativos;
- exportacao de inventario;
- leitura por QR Code;
- etiquetas patrimoniais;
- anexos no ativo;
- garantia e alertas de vencimento;
- manutencao preventiva;
- integracao com patrimonio oficial;
- relatorios de ativos por departamento;
- dashboard de ativos criticos;
- indicadores de chamados por ativo;
- regra patrimonial avancada para descartado/extraviado;
- inventario por agente automatico futuramente.

## Sprint Aprovacao de Chamados 6 - Fechamento funcional e homologacao

Area: Aprovacao de chamados
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%

Situacao atual:
Aprovacao de chamados implementada funcionalmente. O modulo contempla fundacao tecnica, backend administrativo, aprovacao manual, aprovacao automatica por Catalogo de Servicos, bloqueios operacionais para chamados pendentes ou reprovados, frontend administrativo, acompanhamento no portal do solicitante, historico do chamado, auditoria, permissoes, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras como multiplos niveis de aprovacao, alcadas, delegacao, notificacoes avancadas e relatorios.

## Sprint Relatorios Avancados 6 - Fechamento funcional e preparacao de homologacao

Area: Relatorios avancados
Categoria: Relatorios

Status da implementacao: Implementado funcionalmente
Status tecnico: Homologacao funcional preparada
Percentual: 90%

Objetivo:
Preparar o fechamento funcional do modulo Relatorios Avancados com checklist de homologacao, estrutura de evidencias, revisoes de UX/seguranca/performance e documentacao final.

Situacao atual:
Relatorios Avancados implementado funcionalmente. O modulo contempla fundacao tecnica, permissoes, metadados, relatorios de chamados, atendimento, SLA, aprovacoes, catalogo de servicos, inventario/ativos, base de conhecimento e auditoria, alem de frontend administrativo com dashboard, filtros, cards, tabelas, controle por permissoes e exportacao CSV simples de dados carregados. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos, exportacoes avancadas, dashboards configuraveis, cache de indicadores e otimizacoes futuras.

Entregas consolidadas da Sprint 6:
- checklist formal de homologacao do modulo criado;
- estrutura de evidencias para validacao funcional/institucional criada;
- documentacao principal do modulo revisada e consolidada;
- revisao de UX documentada para dashboard e telas principais;
- revisao de seguranca documentada sem relaxamento de permissao;
- revisao basica de performance documentada (AsNoTracking, filtro no banco, projecao DTO);
- validacoes automatizadas de backend/frontend executadas e registradas.

Pendencias planejadas:
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- exportacoes avancadas e dashboards configuraveis;
- cache de indicadores, materialized views e otimizacoes para alto volume.

## Sprint Autenticacao 4 - Gestao administrativa dos metodos de login

Area: Seguranca
Categoria: Autenticacao

Status da implementacao: Implementado funcionalmente
Status tecnico: Sprint 4 concluida apos validacao tecnica
Percentual: 90%

Objetivo:
Permitir a gestao administrativa explicita dos metodos de login, com configuracao persistida e aplicacao no endpoint publico do login.

Entregas consolidadas:
- endpoints administrativos `GET/PUT /api/admin/autenticacao/provedores`;
- permissoes `AutenticacaoProvedores.Visualizar` e `AutenticacaoProvedores.Gerenciar`;
- leitura/escrita da configuracao administrativa de provedores;
- composicao de fallback seguro com configuracao tecnica quando nao ha estado salvo;
- aplicacao da configuracao efetiva em `GET /api/auth/provedores`;
- tela administrativa `Metodos de login` no frontend;
- validacoes de seguranca para impedir estado sem metodo viavel;
- bloqueio de atribuicao automatica de perfil `Administrador` por AD/Microsoft;
- testes automatizados de backend/integracao e validacao de build frontend/backend.

Checklist Sprint 4:
- [x] Provedor habilitado aparece em `/api/auth/provedores`.
- [x] Provedor desabilitado nao aparece em `/api/auth/provedores`.
- [x] `LocalDevelopment` nao aparece fora de `Development`.
- [x] Nao permite salvar sem metodo de login viavel.
- [x] Nao permite remover ultimo metodo de acesso administrativo sem alternativa.
- [x] Provedor principal unico validado.
- [x] Ordem de exibicao respeitada.
- [x] Auto provisionamento respeita perfil padrao configurado.
- [x] Usuario sem permissao nao altera metodos de login.
- [x] Usuario com permissao consulta/altera metodos de login.
- [x] Documentacao atualizada (`AUTENTICACAO-CORPORATIVA`, `CONFIGURACAO-ACTIVE-DIRECTORY`, `METODOS-DE-LOGIN`).

Pendencias evolutivas:
- homologacao institucional com politicas corporativas reais de identidade;
- governanca visual avancada de validacao de conectividade por provedor (sprint futura);
- trilha de auditoria dedicada para alteracoes administrativas de autenticacao.

## Sprint Autenticacao 5 - Auditoria de autenticacao e metodos de login

Area: Seguranca
Categoria: Autenticacao

Status da implementacao: Implementado funcionalmente
Status tecnico: Sprint 5 concluida apos validacao tecnica
Percentual: 92%

Objetivo:
Persistir trilha de auditoria para eventos de autenticacao e alteracoes administrativas dos metodos de login.

Entregas consolidadas:
- classificacao por `TipoEventoAutenticacao` e `ResultadoEventoAutenticacao`;
- auditoria persistida no banco via `EventoAuditoria` (modulo `Autenticacao`);
- eventos de login local, AD e Microsoft;
- eventos de usuario inativo, provedor desabilitado, falha de configuracao e credencial invalida;
- eventos de auto provisionamento e fluxos de senha local;
- auditoria administrativa das alteracoes de metodos de login com dados antes/depois;
- auditoria de tentativa negada por falta de permissao para alterar metodos de login;
- endpoint dedicado `GET /api/admin/auditoria/autenticacao` com permissao `AuditoriaAutenticacao.Visualizar`;
- testes automatizados cobrindo cenarios principais e resiliencia (falha de auditoria nao bloqueia login).

Checklist Sprint 5:
- [x] Login local bem-sucedido gera evento.
- [x] Login local negado gera evento.
- [x] Login AD bem-sucedido gera evento.
- [x] Login AD negado gera evento.
- [x] Usuario inativo gera evento bloqueado.
- [x] Alteracao de metodos de login gera evento administrativo.
- [x] Tentativa sem permissao gera evento negado.
- [x] Sem vazamento de senha/token nos registros.
- [x] Falha de auditoria nao impede autenticacao.
- [x] Documentacao atualizada (`AUTENTICACAO-CORPORATIVA`, `AUDITORIA-AUTENTICACAO`, `METODOS-DE-LOGIN`).

Pendencias evolutivas:
- filtros especializados por `TipoEventoAutenticacao` e `ResultadoEventoAutenticacao` na UI administrativa;
- dashboard executivo dedicado para autenticacao e metodos de login;
- homologacao institucional com politicas de retencao de auditoria por cliente.

## Atualizacao 2026-07-03 - Sprint 8 Catalogo de Servicos 2.0 - Item 48

- Percentual recalculado para `82%` com base em `62` itens concluidos e `14` pendentes.
- O item `48` foi concluido com a exibicao das respostas do formulario no portal do solicitante.
- A tela de detalhe do chamado no portal agora mostra `Rotulo`, `Tipo`, `Valor` e `Valores`, preservando a ordem recebida do backend e compatibilidade quando nao ha respostas.
- Nenhum backend funcional novo foi criado; a exibicao reutiliza o contrato de detalhe ja existente.
- O atendimento administrativo ainda nao possui exibicao dedicada dessas respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `49` (`Exibir respostas do formulario na area administrativa de atendimento`).

## Atualizacao 2026-07-03 - Sprint 8 Catalogo de Servicos 2.0 - Item 47

- Percentual recalculado para `80%` com base em `61` itens concluidos e `15` pendentes.
- O item `47` foi concluido com a exposicao das respostas do formulario no detalhe do chamado.
- O contrato de detalhe agora inclui dados minimos do campo e o conteudo da resposta, com desserializacao de `ValoresJson` para `Valores`.
- Chamados sem respostas continuam retornando colecao vazia, sem quebrar consumidores existentes.
- O portal do solicitante e o atendimento administrativo ainda nao possuem exibicao dedicada dessas respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `48` (`Exibir respostas do formulario no portal do solicitante`).

## Atualizacao 2026-07-02 - Sprint 8 Catalogo de Servicos 2.0 - Item 46

- Percentual recalculado para `79%` com base em `60` itens concluidos e `16` pendentes.
- O item `46` foi concluido com a persistencia real das respostas do formulario na abertura guiada.
- Cada resposta valida agora e gravada em `respostas_formulario_chamado`, vinculada ao chamado, a versao aplicavel do formulario e ao campo respondido.
- Respostas simples sao persistidas em `Valor` e respostas multiplas em `ValoresJson`.
- As respostas ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `47` (`Exibir respostas do formulario no detalhe do chamado`).

## Atualizacao 2026-07-02 - Sprint 8 Catalogo de Servicos 2.0 - Item 45

- Percentual recalculado para `78%` com base em `59` itens concluidos e `17` pendentes.
- O item `45` foi concluido com a migration estrutural da tabela `respostas_formulario_chamado`.
- Snapshot e metadata do EF Core ficaram consolidados, fechando o `pending model changes` da etapa anterior.
- As respostas ainda nao sao persistidas na abertura guiada e ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `46` (`Persistir respostas do formulario na abertura guiada`).
## Atualizacao 2026-07-03 - Sprint 8 Catalogo de Servicos 2.0 - Item 49

- Percentual recalculado para `83%` com base em `63` itens concluidos e `13` pendentes.
- O item `49` foi concluido com a exibicao das respostas persistidas do formulario na area administrativa de atendimento.
- A tela administrativa agora mostra `Rotulo`, `Tipo`, `Valor` e `Valores`, preservando a ordem recebida do contrato ja existente.
- Nenhum backend funcional novo foi criado nesta etapa e nao houve alteracao na persistencia das respostas.
- Auditoria e historico especifico dessas respostas ainda nao existem.
- O proximo item pendente da Sprint 8 passa a ser o item `50` (`Registrar historico da abertura com formulario preenchido`).
## Atualizacao 2026-07-03 - Sprint 8 Catalogo de Servicos 2.0 - Item 50

- Percentual recalculado para `84%` com base em `64` itens concluidos e `12` pendentes.
- O item `50` foi concluido com o registro do historico funcional e resumido da abertura guiada com formulario preenchido.
- Quando houver respostas persistidas, a abertura agora grava `Chamado aberto com formulario do servico preenchido.` sem expor valores das respostas.
- O historico padrao de criacao e o historico de criacao por catalogo permanecem preservados.
- Auditoria tecnica especifica dessas respostas ainda nao existe.
- O proximo item pendente da Sprint 8 passa a ser o item `51` (`Registrar auditoria tecnica das respostas persistidas`).
