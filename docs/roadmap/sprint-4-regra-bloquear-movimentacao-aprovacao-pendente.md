# Sprint 4 - Regra para bloquear movimentacao com aprovacao pendente

## 1. Objetivo da regra
Criar uma regra de aplicacao que responda se uma movimentacao sensivel ou final do chamado deve ser bloqueada quando houver aprovacao pendente bloqueante, preservando o legado `AprovacaoChamado` e incorporando a nova `InstanciaAprovacaoChamado`.

## 2. Limites desta etapa
- Nao aprova, reprova, cancela ou expira aprovacoes.
- Nao altera status do chamado por conta propria.
- Nao cria etapa, decisao, endpoint, controller, tela ou frontend.
- Nao altera SLA.

## 3. Contexto dos itens anteriores
- O item 38 passou a gerar `InstanciaAprovacaoChamado` pendente quando uma regra aplicavel exige aprovacao.
- O legado continua usando `AprovacaoChamado` e `BloqueiaAvancoAtendimento`.
- O item 39 precisava unificar a leitura dessas duas fontes sem travar comentario, anexo, consulta ou triagem.

## 4. Diferenca entre gerar aprovacao e bloquear movimentacao
- Gerar aprovacao: cria a pendencia.
- Bloquear movimentacao: avalia se a acao atual pode prosseguir diante dessa pendencia.

## 5. Comportamento legado atual
O legado ja bloqueava `assumir`, `assumir fila`, `reabrir`, `encerrar` e avancos finais via `AprovacaoChamadoHelper` e `BloqueiaAvancoAtendimento`.

## 6. Regra/use case/helper criado
- `IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`
- `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase`

## 7. Contratos internos criados
- `ValidarBloqueioMovimentacaoAprovacaoPendenteRequest`
- `ValidarBloqueioMovimentacaoAprovacaoPendenteResponse`
- `TipoAcaoMovimentacaoChamado`

## 8. Tipos de acao/movimentacao avaliados
- `Consultar`
- `Comentar`
- `AnexarEvidencia`
- `Triagem`
- `Assumir`
- `Atribuir`
- `Encaminhar`
- `AlterarStatus`
- `Resolver`
- `Encerrar`
- `Reabrir`
- `ExecutarServicoSensivel`
- `AplicarMudanca`
- `LiberarAcesso`
- `Cancelar`

## 9. Criterios para bloquear
- Existe `AprovacaoChamado` pendente com `BloqueiaAvancoAtendimento = true`.
- Ou existe `InstanciaAprovacaoChamado` ativa em `Pendente` ou `EmReavaliacao`, que exige aprovacao e e bloqueante.
- A acao pedida e sensivel ou final.

## 10. Criterios para permitir
- Nao existe pendencia bloqueante.
- A acao e apenas consultiva, de comentario, anexo, evidencia, triagem, atribuicao ou encaminhamento.
- A instancia nova e apenas nao bloqueante.
- A aprovacao/instancia ja foi decidida, cancelada, expirada ou substituida.

## 11. Relacao com `AprovacaoChamado`
Continua sendo consultada como primeira fonte de bloqueio para preservar compatibilidade.

## 12. Relacao com `AprovacaoChamadoHelper`
Foi composta, nao substituida. A nova regra reutiliza o helper para o legado e so adiciona a consulta da nova estrutura.

## 13. Relacao com `InstanciaAprovacaoChamado`
Passou a ser consultada para detectar pendencias bloqueantes e pendencias nao bloqueantes da estrutura nova.

## 14. Relacao com `BloqueiaAvancoAtendimento`
Segue valendo integralmente para o legado. Na estrutura nova, o equivalente pratico e `Bloqueante` com `EfeitoOperacional = ExigirAprovacaoEBloquearAvanco`.

## 15. Relacao com `AguardandoAprovacao`
O status nao e usado como unica fonte de bloqueio. O bloqueio depende da pendencia real.

## 16. Relacao com status do chamado
Nao altera status. Apenas barra a movimentacao quando necessario.

## 17. Relacao com abertura de chamado
Nenhuma alteracao ampla foi feita na abertura.

## 18. Relacao com atendimento
O bloqueio foi mantido cirurgico: impede avancos sensiveis/finais e preserva atendimento comum.

## 19. Relacao com comentarios, anexos e evidencias
Essas acoes continuam permitidas, mesmo com pendencia bloqueante, recebendo no maximo sinalizacao.

## 20. Relacao com alteracao de status
`AlterarStatusChamadoUseCase` passou a consultar a nova regra para impedir apenas avancos finais quando houver pendencia bloqueante.

## 21. Relacao com encerramento
`EncerrarChamadoUseCase` passou a consultar a nova regra e continua respeitando o legado.

## 22. Relacao com reabertura
`ReabrirChamadoUseCase` passou a consultar a nova regra, preservando o bloqueio ja existente para reabertura sensivel.

## 23. Relacao com SLA
Nenhuma alteracao. O item nao pausa nem recalcula SLA.

## 24. Tratamento de aprovacao informativa
Nao bloqueia movimentacao. Quando cabivel, a resposta pode vir apenas com sinalizacao.

## 25. Tratamento de aprovacao pendente bloqueante
Bloqueia acoes sensiveis/finais e retorna motivo, origem e identificadores da pendencia.

## 26. Tratamento de aprovacao aprovada
Nao bloqueia.

## 27. Tratamento de aprovacao reprovada
Nao bloqueia nesta etapa. Rejeicao funcional ficara para item posterior.

## 28. Tratamento de aprovacao cancelada ou expirada
Nao bloqueia.

## 29. Compatibilidade com chamados legados
Mantida. Nao houve migracao em massa nem substituicao destrutiva do legado.

## 30. Garantias de ausencia de aprovacao/rejeicao funcional
A regra so avalia bloqueio. Nao decide aprovacao e nao altera workflow.

## 31. Testes criados
- `BloquearMovimentacaoAprovacaoPendenteUseCaseTests`
- Cobertura de legado bloqueante
- Cobertura de `InstanciaAprovacaoChamado` bloqueante
- Cobertura de comentario, anexo e triagem permitidos
- Cobertura de integracao com `AlterarStatus` e `Assumir`

## 32. Riscos de seguranca e governanca
- Confundir sinalizacao com permissao irrestrita.
- Assumir que toda pendencia nova bloqueia, mesmo quando a regra e apenas nao bloqueante.
- Ampliar demais o conceito de acao sensivel em futuras integracoes.

## 33. Decisoes adiadas para proximos itens
- Aprovacao funcional.
- Rejeicao funcional.
- Cancelamento/expiracao funcionais.
- Workflow sequencial, paralelo e multinivel.
- Endpoints e frontend.

## 34. Conclusao tecnica
O item 39 consolidou a leitura de pendencia bloqueante entre legado e nova estrutura, preservando o comportamento atual e adicionando bloqueio operacional seletivo para instancias do novo motor.

## 35. Proxima etapa recomendada
Executar o item 40: criar regra para aprovar chamado.
