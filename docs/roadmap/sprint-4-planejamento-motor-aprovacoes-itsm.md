# Sprint 4 - Planejamento do Motor de AprovaÃ§Ãµes ITSM

## SituaÃ§Ã£o atual da Sprint 4

- Item de roadmap: `Sprint 4 - Motor de Aprovacoes ITSM` (`ordem 104`).
- Status da implementaÃ§Ã£o: `Implementado funcionalmente`.
- Status tÃ©cnico: `Completo com pendÃªncias evolutivas`.
- SituaÃ§Ã£o atual: a aprovaÃ§Ã£o existente cobre a base funcional, mas ainda nÃ£o opera como motor reutilizÃ¡vel multinÃ­vel.
- Percentual anterior: `50%`, calculado pelo checklist genÃ©rico legado com `2/4` itens concluÃ­dos.
- Percentual esperado apos a normalizacao: `21%`, calculado pelo checklist ativo detalhado com `14/68` itens concluidos.

## O que ja existe no modulo de aprovacao

- Modulo funcional de aprovacao ja implementado e reutilizado como base.
- Aprovacao manual e automatica por catalogo de servicos.
- Bloqueios operacionais quando ha aprovacao pendente, conforme regras ja existentes.
- Historico, auditoria, permissoes e endpoints do fluxo atual preservados nesta etapa.

## O que sera evoluido

- Generalizacao da aprovacao por tipo de chamado, natureza ITSM e servico sensivel.
- Definicao de aprovador padrao, grupo aprovador e multiplos niveis.
- Regras de bloqueio, liberacao, rejeicao, cancelamento e expiracao.
- Trilhas de auditoria especificas do motor reutilizavel.
- Compatibilidade com abertura, atendimento, SLA e chamados ja existentes.

## Limites desta etapa

- Esta etapa atualiza apenas roadmap, checklist e documentacao inicial de planejamento.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao foram criados endpoints, controllers, telas ou services frontend.
- Nao houve implementacao funcional nova no fluxo de aprovacao.
- Nao houve homologacao nem registro de aceite final.

## Checklist oficial detalhado

| Ordem | Item | Categoria detalhada | Grupo roadmap | Concluido |
|---|---|---|---|---|
| 1 | Planejar escopo e criterios de aceite da Sprint 4 | Planejamento | Planejamento | Sim |
| 2 | Mapear modulo de aprovacao existente | Planejamento | Planejamento | Sim |
| 3 | Mapear fluxo atual de aprovacao em chamados | Planejamento | Planejamento | Sim |
| 4 | Mapear pontos onde chamado deve ficar bloqueado por aprovacao pendente | Planejamento | Planejamento | Sim |
| 5 | Definir conceito de motor de aprovacao ITSM reutilizavel | Arquitetura | Governanca | Sim |
| 6 | Definir regra de aprovacao por natureza ITSM | Arquitetura | Governanca | Sim |
| 7 | Definir regra de aprovacao por tipo de chamado | Arquitetura | Governanca | Sim |
| 8 | Definir regra de aprovacao por servico sensivel | Arquitetura | Governanca | Sim |
| 9 | Definir regra de aprovacao por impacto e urgencia | Arquitetura | Governanca | Sim |
| 10 | Definir regra de aprovacao por custo ou risco | Arquitetura | Governanca | Sim |
| 11 | Definir conceito de aprovador padrao | Arquitetura | Governanca | Sim |
| 12 | Definir conceito de grupo aprovador | Arquitetura | Governanca | Sim |
| 13 | Definir conceito de aprovacao multi-nivel | Arquitetura | Governanca | Sim |
| 14 | Definir comportamento de aprovacao sequencial | Arquitetura | Governanca | Sim |
| 15 | Definir comportamento de aprovacao paralela | Arquitetura | Governanca | Nao |
| 16 | Definir regra de bloqueio por decisao pendente | Regra de Negocio | Governanca | Nao |
| 17 | Definir regra de liberacao apos aprovacao | Regra de Negocio | Governanca | Nao |
| 18 | Definir regra de rejeicao e encerramento ou retorno do chamado | Regra de Negocio | Governanca | Nao |
| 19 | Definir regra de cancelamento de aprovacao | Regra de Negocio | Governanca | Nao |
| 20 | Definir regra de expiracao de aprovacao pendente | Regra de Negocio | Governanca | Nao |
| 21 | Definir historico/auditoria de solicitacao de aprovacao | Auditoria | Governanca | Nao |
| 22 | Definir historico/auditoria de decisao de aprovacao | Auditoria | Governanca | Nao |
| 23 | Definir historico/auditoria de rejeicao de aprovacao | Auditoria | Governanca | Nao |
| 24 | Definir historico/auditoria de aprovacao expirada ou cancelada | Auditoria | Governanca | Nao |
| 25 | Avaliar compatibilidade com chamados existentes | Compatibilidade | Governanca | Nao |
| 26 | Avaliar compatibilidade com fluxo atual de abertura de chamado | Compatibilidade | Governanca | Nao |
| 27 | Avaliar compatibilidade com fluxo atual de atendimento | Compatibilidade | Governanca | Nao |
| 28 | Avaliar compatibilidade com SLA atual | Compatibilidade | Governanca | Nao |
| 29 | Modelar configuracao de regra de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 30 | Modelar instancia de aprovacao do chamado | Desenvolvimento | Desenvolvimento | Nao |
| 31 | Modelar etapa de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 32 | Modelar decisao de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 33 | Criar migrations estruturais do motor de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 34 | Criar contratos de configuracao de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 35 | Criar contratos de decisao de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 36 | Criar servico de aplicacao para regras de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 37 | Criar servico de aplicacao para instancia de aprovacao | Desenvolvimento | Desenvolvimento | Nao |
| 38 | Criar regra para gerar aprovacao obrigatoria no chamado | Desenvolvimento | Desenvolvimento | Nao |
| 39 | Criar regra para bloquear movimentacao com aprovacao pendente | Desenvolvimento | Desenvolvimento | Nao |
| 40 | Criar regra para aprovar chamado | Desenvolvimento | Desenvolvimento | Nao |
| 41 | Criar regra para rejeitar chamado | Desenvolvimento | Desenvolvimento | Nao |
| 42 | Criar regra para reavaliar aprovacao apos mudanca de dados sensiveis | Desenvolvimento | Desenvolvimento | Nao |
| 43 | Criar endpoints administrativos de regras de aprovacao | API | Desenvolvimento | Nao |
| 44 | Criar endpoints de aprovacao e rejeicao | API | Desenvolvimento | Nao |
| 45 | Criar endpoints de consulta de pendencias de aprovacao | API | Desenvolvimento | Nao |
| 46 | Exibir status de aprovacao no detalhe do chamado | Frontend | Desenvolvimento | Nao |
| 47 | Exibir pendencias de aprovacao para aprovador | Frontend | Desenvolvimento | Nao |
| 48 | Criar tela ou secao de configuracao de regras de aprovacao | Frontend | Desenvolvimento | Nao |
| 49 | Permitir aprovar chamado pela interface | Frontend | Desenvolvimento | Nao |
| 50 | Permitir rejeitar chamado pela interface | Frontend | Desenvolvimento | Nao |
| 51 | Ajustar listagem/filtros para aprovacao pendente | Frontend | Desenvolvimento | Nao |
| 52 | Testar regra de aprovacao por natureza ITSM | Testes | Testes | Nao |
| 53 | Testar regra de aprovacao por servico sensivel | Testes | Testes | Nao |
| 54 | Testar bloqueio por aprovacao pendente | Testes | Testes | Nao |
| 55 | Testar aprovacao e liberacao do chamado | Testes | Testes | Nao |
| 56 | Testar rejeicao de aprovacao | Testes | Testes | Nao |
| 57 | Testar aprovacao por grupo aprovador | Testes | Testes | Nao |
| 58 | Testar aprovacao multi-nivel | Testes | Testes | Nao |
| 59 | Testar regressao do fluxo atual de aprovacao | Testes | Testes | Nao |
| 60 | Testar regressao de abertura e atendimento de chamado | Testes | Testes | Nao |
| 61 | Documentar modelo do motor de aprovacao | Documentacao | Documentacao | Nao |
| 62 | Documentar regras de aprovacao ITSM | Documentacao | Documentacao | Nao |
| 63 | Documentar impacto no fluxo atual de chamados | Documentacao | Documentacao | Nao |
| 64 | Documentar criterios de testes tecnicos | Documentacao | Documentacao | Nao |
| 65 | Preparar roteiro de homologacao de casos sensiveis | Homologacao | Homologacao | Nao |
| 66 | Preparar roteiro de homologacao de aprovacao por grupo | Homologacao | Homologacao | Nao |
| 67 | Preparar roteiro de homologacao de aprovacao multi-nivel | Homologacao | Homologacao | Nao |
| 68 | Registrar homologacao e aceite final | Homologacao | Homologacao | Nao |

## Itens marcados como concluidos

- Ordem 1: Planejar escopo e criterios de aceite da Sprint 4.
- Ordem 2: Mapear modulo de aprovacao existente.
- Ordem 3: Mapear fluxo atual de aprovacao em chamados.
- Ordem 4: Mapear pontos onde chamado deve ficar bloqueado por aprovacao pendente.
- Ordem 5: Definir conceito de motor de aprovacao ITSM reutilizavel.
- Ordem 6: Definir regra de aprovacao por natureza ITSM.
- Ordem 7: Definir regra de aprovacao por tipo de chamado.
- Ordem 8: Definir regra de aprovacao por servico sensivel.
- Ordem 9: Definir regra de aprovacao por impacto e urgencia.
- Ordem 10: Definir regra de aprovacao por custo ou risco.
- Ordem 11: Definir conceito de aprovador padrao.
- Ordem 12: Definir conceito de grupo aprovador.
- Ordem 13: Definir conceito de aprovacao multi-nivel.
- Ordem 14: Definir comportamento de aprovacao sequencial.

## Itens pendentes

- Permanecem pendentes `54` itens do checklist oficial.
- Nenhum item de homologacao foi marcado como concluido.
- A proxima acao objetiva passa a ser definir comportamento de aprovacao paralela.

## Percentual esperado apos a atualizacao

- Itens ativos: `68`.
- Itens concluidos: `14`.
- Percentual recalculado por checklist ativo: `21%`.
- Formula aplicada: `14 / 68 = 20,58%`, arredondado para `21%`.

## Proxima etapa objetiva

- Definir comportamento de aprovacao paralela, preservando independencia entre niveis, quorum proprio e bloqueio operacional.

