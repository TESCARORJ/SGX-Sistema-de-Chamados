$roadmap = 'docs/ROADMAP.md'
$text = Get-Content $roadmap -Raw
$text = $text.Replace('5. Sprint 5 - Regras de fechamento, aceite e reabertura (41% - Em desenvolvimento)', '5. Sprint 5 - Regras de fechamento, aceite e reabertura (44% - Em desenvolvimento)')
Set-Content $roadmap $text

$roadmapItsm = 'docs/ROADMAP-ITSM.md'
$textItsm = Get-Content $roadmapItsm -Raw
$textItsm = $textItsm.Replace('5. Sprint 5 - Regras de fechamento, aceite e reabertura (41% - Em desenvolvimento)', '5. Sprint 5 - Regras de fechamento, aceite e reabertura (44% - Em desenvolvimento)')
$textItsm = $textItsm.Replace('| Sprint 5 - Regras de fechamento, aceite e reabertura | ITIL/ITSM | Em desenvolvimento | Parcial | 41% |', '| Sprint 5 - Regras de fechamento, aceite e reabertura | ITIL/ITSM | Em desenvolvimento | Parcial | 44% |')
$marker = '## Sprint 5 - Regras de fechamento, aceite e reabertura'
$idx = $textItsm.IndexOf($marker)
if ($idx -lt 0) { throw 'Secao da sprint 5 nao encontrada em ROADMAP-ITSM.' }
$prefix = $textItsm.Substring(0, $idx).TrimEnd()
$section = @"
## Sprint 5 - Regras de fechamento, aceite e reabertura

Area: Sprint 5 - Regras de fechamento, aceite e reabertura
Categoria: ITIL/ITSM

Status da implementacao: Em desenvolvimento
Status tecnico: Parcial
Percentual: 44%

Objetivo:
Criar governanca de encerramento com aceite, fechamento automatico e reabertura controlada.

Descricao do objetivo:
Evoluir o ciclo de vida do chamado para separar corretamente os estados de resolucao, aceite, fechamento e reabertura, garantindo rastreabilidade, auditoria e aderencia as praticas ITIL/ITSM.

Atencao tecnica:
Separar conceitualmente e funcionalmente os estados Resolvido e Fechado. Nao tratar resolucao como encerramento definitivo. Preservar compatibilidade com SLA, atendimento, historico, permissoes e com o motor de aprovacoes da Sprint 4.

Situacao atual:
Itens 1 a 14 concluidos. O Item 15 permanece pendente e trata do reforco de auditoria do ciclo de resolucao, aceite, rejeicao, fechamento e reabertura.

Pendencias tecnicas:
Auditoria consolidada da sprint, exibicao frontend e consolidacao final do roadmap da sprint.

Pendencias de homologacao:
Validar o ciclo com solicitantes, atendentes e administradores reais.

Evidencia da implementacao:
Base de encerramento/reabertura existente reaproveitada, com separacao entre resolucao, aceite, rejeicao, fechamento automatico por prazo, configuracao administrativa governada e reabertura controlada por politica de prazo.

Criterio de aceite:
Fluxo contempla resolucao, aceite/rejeicao pelo solicitante, fechamento automatico por prazo governado, reabertura controlada por prazo/politica, cancelamento com motivo obrigatorio e compatibilidade com SLA, historico, permissoes e aprovacoes pendentes bloqueantes.

Proxima acao:
Registrar auditoria de resolucao, aceite, rejeicao, fechamento e reabertura.

Checklist Sprint 5:
- [x] 1. Planejar escopo e criterios de aceite da Sprint 5 - Planejamento
- [x] 2. Mapear fluxo atual de encerramento e reabertura - Planejamento
- [x] 3. Validar compatibilidade com Fundacao ITSM do chamado - Planejamento
- [x] 4. Validar compatibilidade com Sprint 4 Motor de Aprovacoes ITSM - Planejamento
- [x] 5. Documentar modelo de ciclo de vida Resolvido/Fechado/Reaberto - Documentacao
- [x] 6. Separar status Resolvido e Fechado no fluxo de negocio - Desenvolvimento
- [x] 7. Criar regra para exigir solucao tecnica ao resolver chamado - Desenvolvimento
- [x] 8. Criar regra para exigir motivo ao cancelar chamado - Desenvolvimento
- [x] 9. Criar regra de aceite do solicitante - Desenvolvimento
- [x] 10. Criar regra de rejeicao da solucao pelo solicitante - Desenvolvimento
- [x] 11. Criar regra de retorno ao atendimento apos rejeicao da solucao - Desenvolvimento
- [x] 12. Criar politica de fechamento automatico apos prazo de aceite - Desenvolvimento
- [x] 13. Criar configuracao administrativa do prazo de auto-fechamento - Desenvolvimento
- [x] 14. Criar regra de reabertura controlada por prazo/politica - Desenvolvimento
- [ ] 15. Registrar auditoria de resolucao, aceite, rejeicao, fechamento e reabertura - Desenvolvimento
- [ ] 16. Preservar bloqueio por aprovacao pendente antes de fechamento definitivo - Desenvolvimento
- [ ] 17. Ajustar endpoints de resolucao, fechamento, aceite e reabertura - API
- [ ] 18. Exibir dados de solucao, aceite e fechamento no detalhe do chamado - Frontend
- [ ] 19. Permitir aceite/rejeicao pelo solicitante na interface - Frontend
- [ ] 20. Exibir historico de fechamento e reabertura na interface administrativa - Frontend
- [ ] 21. Testar resolucao com solucao obrigatoria - Testes
- [ ] 22. Testar cancelamento com motivo obrigatorio - Testes
- [ ] 23. Testar aceite e fechamento definitivo - Testes
- [ ] 24. Testar rejeicao da solucao e retorno ao atendimento - Testes
- [ ] 25. Testar fechamento automatico por prazo - Testes
- [ ] 26. Testar reabertura controlada e auditavel - Testes
- [ ] 27. Testar regressao de encerramento/reabertura existente - Testes
- [ ] 28. Testar integracao com aprovacao pendente bloqueante - Testes
- [ ] 29. Documentar impacto no fluxo atual de chamados - Documentacao
- [ ] 30. Preparar roteiro de homologacao da Sprint 5 - Homologacao
- [ ] 31. Registrar homologacao e aceite tecnico - Homologacao
- [ ] 32. Atualizar roadmap final da Sprint 5 - Governanca
"@
Set-Content $roadmapItsm ($prefix + "`r`n`r`n" + $section)

@'
# Sprint 5 - Reabertura controlada por prazo/politica

## Objetivo
Concluir o Item 14 com a menor mudanca segura para permitir reabertura apenas de chamados encerrados, dentro de prazo governado e com motivo obrigatorio.

## Decisao de arquitetura
- O fluxo legado de `ReabrirChamadoUseCase` foi evoluido em vez de criar um segundo fluxo paralelo.
- O prazo maximo de reabertura passou a ser governado pela chave `chamados.reabertura.prazo_maximo_horas` em `ParametroSistema`.
- Nao foi criada tabela nova, endpoint novo nem tela administrativa.
- O valor padrao inicial foi seedado em `168` horas.

## O que foi entregue
1. Regra de dominio para reabertura controlada apenas quando o chamado esta em `Encerrado`.
2. Motivo obrigatorio de reabertura, reaproveitando o campo `Mensagem` do request legado.
3. Validacao do prazo maximo de reabertura a partir de `EncerradoEm`.
4. Integracao com `ParametroSistema` para carregar o prazo governado.
5. Registro de historico e auditoria especificos da reabertura controlada.
6. Preservacao de `ResolvidoEm`, dados de aceite, rejeicao e fechamento automatico.
7. Sem criar armazenamento novo para solucao tecnica, porque a entidade `Chamado` nao expoe campo persistido proprio para isso no estado atual do projeto.
8. Preservacao do comportamento legado de limpar `EncerradoEm` ao reabrir, por compatibilidade com o fluxo existente.

## Regras aplicadas
- Reabrir somente chamados em `StatusChamadoEnum.Encerrado`.
- Bloquear reabertura de chamados cancelados, resolvidos, abertos, em atendimento e demais estados nao definitivos.
- Prazo governado entre `1` e `2160` horas.
- Fallback tecnico documentado para `168` horas quando o parametro nao estiver ativo/indisponivel.
- Configuracao invalida do parametro gera falha segura.
- O chamado retorna ao status operacional `EmAtendimento`, com fallback por natureza mantido pelo fluxo legado.
- O fluxo continua respeitando bloqueio por aprovacao pendente.

## Compatibilidade e SLA
- A reabertura continua usando o mesmo endpoint administrativo existente.
- Nao foi criado fluxo de portal novo.
- O comportamento legado de SLA foi preservado sem recalculo adicional neste item.

## Fora de escopo
- Item 15 de auditoria ampla da sprint.
- Tela/frontend.
- Scheduler.
- Politica de excecao administrativa fora do prazo.
