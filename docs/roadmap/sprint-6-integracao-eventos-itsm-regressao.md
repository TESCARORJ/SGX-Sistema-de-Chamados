# Sprint 6 - Integracao de eventos ITSM e regressao

## 1. Objetivo
Integrar o pipeline de notificacoes a um conjunto pequeno e estavel de eventos ITSM reais, com idempotencia, destinatarios corretos e regressao dos fluxos operacionais.

## 2. Estado anterior
Dominio, persistencia, preferencias, materializacao, processamento, entrega, API autenticada e central frontend ja estavam concluídos antes deste item.

## 3. Criterios de priorizacao
Foram priorizados eventos com fato persistido, identificador estavel, destinatario claro, baixo risco de duplicidade e cobertura de testes existente.

## 4. Eventos selecionados
- chamado aberto
- responsavel alterado por atribuicao
- chamado assumido
- status alterado para `EmAtendimento`, `AguardandoSolicitante` ou `Resolvido`
- chamado encerrado

## 5. Eventos adiados
- reabertura
- comentarios e anexos
- aprovacao pendente/decidida
- eventos de SLA
- broadcasts administrativos

## 6. Pontos de integracao
Os pontos reais ficaram em `AbrirChamadoUseCase`, `AtribuirChamadoUseCase`, `AssumirChamadoUseCase`, `AlterarStatusChamadoUseCase` e `EncerrarChamadoUseCase`, sempre apos `SaveChangesAsync`.

## 7. Orquestrador
Foi criado `ProcessarEventoCandidatoNotificacaoUseCase` para orquestrar resolucao de destinatarios, preferencia, materializacao e geracao idempotente sem acoplar negocio a transporte.

## 8. Evento candidato
O fluxo reutiliza `EventoCandidatoNotificacao` como contrato interno base, complementado por request proprio com variaveis, canais e participacoes.

## 9. Identificador do evento
Os eventos usam chaves estaveis como:
- `chamado-aberto:{chamadoId}`
- `responsavel-alterado:{historicoId}`
- `chamado-assumido:{historicoId}`
- `status-alterado:{historicoId}`
- `chamado-encerrado:{historicoId}`

## 10. Chave de idempotencia
A notificacao final deriva da chave do evento com hash estavel por destinatario e canal, evitando GUID aleatorio e evitando colisao entre multiplos destinatarios.

## 11. Destinatarios
Foi reutilizado `ResolverDestinatariosNotificacaoUseCase`.
- abertura: `Solicitante`
- atribuicao: `ResponsavelAtual`
- assuncao: `ResponsavelAtual` com exclusao do originador
- status relevante: `Solicitante`
- encerramento: `Solicitante`

## 12. Preferencias
Foi reutilizado `AvaliarPreferenciaNotificacaoUseCase`. Preferencia explicita desabilitada bloqueia geracao; ausencia de preferencia mantem fallback permitido.

## 13. Templates
O item reutiliza a materializacao existente por `TipoEventoNotificacao` e canal. Nos testes foram usados templates genericos de `EventoChamado` para provar a integracao ponta a ponta.

## 14. Variaveis
As variaveis enviadas sao explicitas e pequenas, por exemplo:
- `chamado.codigo`
- `chamado.titulo`
- `chamado.status`
- `evento.nome`
- `evento.descricao`
- `responsavel.nome`
- `solicitante.nome`
- `solucao.resumo`

## 15. Canais
Foram geradas notificacoes para `Sistema` e `Email`, sempre como persistencia do pipeline, sem entrega sincrona dentro do fluxo do chamado.

## 16. Transacao
O fato de negocio continua sendo persistido primeiro. A integracao e tentada somente depois do commit da operacao principal.

## 17. Comportamento pos-commit
Falhas de notificacao sao capturadas e registradas no ponto de integracao; nao revertem abertura, atribuicao, status ou encerramento.

## 18. Falhas
Ausencia de template, bloqueio por preferencia e inelegibilidade de destinatario geram avisos/ignoradas, nao excecao funcional do chamado.

## 19. Logs
O orquestrador registra `EventoId`, `ChamadoId`, quantidades resolvidas, permitidas, criadas, duplicadas e ignoradas.

## 20. Chamado aberto
Integrado com notificacao ao solicitante usando o chamado ja persistido como fato estavel.

## 21. Atribuicao
Integrada com notificacao ao novo responsavel quando a atribuicao e concluida.

## 22. Status
A integracao foi limitada a estados relevantes `EmAtendimento`, `AguardandoSolicitante` e `Resolvido`. Estados internos ou nao priorizados ficam sem notificacao nesta etapa.

## 23. Encerramento
Integrado com notificacao ao solicitante apos o encerramento persistido.

## 24. Aprovacao, se integrada
Nao integrada neste item para evitar duplicidade e porque a trilha de aprovacao ainda exige definicao funcional mais especifica de eventos de notificacao.

## 25. SLA, se integrado
Nao integrado. Nao foi identificado evento de notificacao de SLA suficientemente estavel para esta entrega incremental.

## 26. Compatibilidade com legado
Fluxos legados de chamado, aprovacao e Worker.Email foram preservados.

## 27. Compatibilidade com motor de aprovacao
O motor de aprovacao nao foi alterado; apenas permaneceu elegivel para integracao futura.

## 28. Compatibilidade com frontend
Nenhuma mudanca funcional foi necessaria na central frontend alem do consumo da caixa ja entregue no item 14.

## 29. Testes unitarios
`ProcessarEventoCandidatoNotificacaoUseCaseTests` cobre geracao por canal, preferencia, idempotencia, resultado parcial e cancelamento.

## 30. Testes de integracao
Foram criadas suites para abertura, atribuicao/assuncao, status e encerramento, comprovando persistencia real de `Notificacao`.

## 31. Testes de regressao
`RegressaoNotificacoesFluxosItsmTests` cobre ausencia de notificacao em reabertura e garantia de que falha de template nao desfaz mudanca operacional.

## 32. O que nao foi implementado
- todos os eventos do sistema
- aprovacao completa
- SLA
- outbox generica
- fila externa
- push, SignalR, WebSocket

## 33. Riscos
Ambientes sem templates ativos para `EventoChamado` por canal nao gerarao notificacoes reais ate a configuracao correspondente.

## 34. Decisoes adiadas
Ficaram para a proxima etapa a homologacao manual completa, eventual especializacao de templates por subtipo de evento e a expansao para aprovacao/SLA.

## 35. Criterios de aceite
Eventos priorizados integrados, idempotencia preservada, regressao executada, nenhuma entrega sincrona, nenhum impacto indevido em SLA/aprovacao/Worker.Email.

## 36. Proxima etapa
Documentar, homologar e registrar aceite da Sprint 6.
