# Sprint 5 - Modelo de Ciclo de Vida: Resolvido, Fechado e Reaberto

## 1. Objetivo
Criar documentação técnica clara para o ciclo de vida Resolvido/Fechado/Reaberto, separando resolução técnica de fechamento definitivo, definindo regras de aceite, rejeição, auto-fechamento, cancelamento e reabertura controlada.

## 2. Contexto da Sprint 5
A Sprint 5 foca em adicionar governança ao processo de encerramento de chamados. O fluxo anterior não distinguia "Resolvido" de "Fechado" de maneira formal e a reabertura carecia de políticas rígidas de auditoria e controle de prazo.

## 3. Situação atual do sistema
Os fluxos atuais (`EncerrarChamadoUseCase` e `ReabrirChamadoUseCase`) encerram e reabrem os chamados de forma direta. Falta governança completa de aceite do solicitante, prazo formal de auto-fechamento, rejeição de solução, campos obrigatórios de solução/cancelamento e políticas auditáveis de reabertura.

## 4. Conceito de Resolvido
**Resolvido** não é Fechado. Resolvido significa que o atendente registrou uma solução técnica para o incidente ou requisição, mas o solicitante ou o sistema ainda precisam validar essa solução.

## 5. Conceito de Fechado
**Fechado** significa o encerramento definitivo do chamado após o aceite do solicitante, auto-fechamento por decurso de prazo ou decisão administrativa válida.

## 6. Diferença entre Resolvido e Fechado
Enquanto "Resolvido" é um estado transitório aguardando validação, "Fechado" é o estado terminal primário do ciclo de vida. O SLA de resolução é pausado/concluído no momento em que o chamado é "Resolvido", não "Fechado".

## 7. Conceito de Reaberto
**Reaberto** é o estado que o chamado assume quando um chamado Fechado ou Resolvido retorna ao atendimento devido à rejeição da solução pelo solicitante ou por ação de reabertura controlada por política, prazo e auditoria.

## 8. Papel do solicitante no aceite
O solicitante tem a responsabilidade e o direito de analisar a solução técnica fornecida e realizar o aceite (confirmando a eficácia) ou a rejeição (quando a solução for insatisfatória).

## 9. Papel do atendente na resolução
O atendente deve obrigatoriamente fornecer a solução técnica ou motivo da solução ao mover o chamado para o status "Resolvido".

## 10. Papel do administrador
O administrador configura as políticas gerais, como o prazo para o fechamento automático e possui poderes de auditoria e, quando configurado, poderes de reabertura ou encerramento administrativo.

## 11. Fluxo esperado de resolução
1. Atendente inicia a resolução do chamado.
2. Sistema exige o preenchimento de uma solução técnica obrigatória.
3. Status do chamado muda para "Resolvido".
4. Notificação é enviada ao solicitante aguardando o aceite.

## 12. Fluxo esperado de aceite
1. Solicitante revisa a solução registrada.
2. Solicitante confirma que o problema foi resolvido.
3. Status do chamado muda para "Fechado" de forma definitiva.

## 13. Fluxo esperado de rejeição da solução
1. Solicitante não concorda que a solução resolveu o problema.
2. Solicitante aciona a rejeição.
3. Chamado retorna ao atendimento (reaberto ou equivalente no fluxo) para nova análise pelo atendente.

## 14. Fluxo esperado de fechamento automático
1. Chamado permanece no status "Resolvido".
2. Ocorre o decurso do prazo configurado de aceite sem ação do solicitante.
3. O sistema realiza o fechamento automático, mudando o status para "Fechado" sem intervenção manual.

## 15. Fluxo esperado de cancelamento
1. Atendente ou sistema inicia o cancelamento do chamado.
2. Sistema exige o preenchimento de um motivo obrigatório.
3. Chamado transita para "Cancelado", sendo também um estado terminal.

## 16. Fluxo esperado de reabertura
1. Solicitação de reabertura de um chamado fechado.
2. Sistema valida a política, o prazo permitido para reabertura e as permissões.
3. Sistema exige o registro de quem reabriu, quando, motivo e contexto.
4. O chamado retorna ao atendimento, mas sem apagar o histórico anterior.

## 17. Regras de solução técnica obrigatória
Qualquer transição para o status "Resolvido" exige de forma estrita o preenchimento da solução técnica que fundamentou a resolução.

## 18. Regras de motivo obrigatório de cancelamento
O cancelamento de um chamado não pode ser silencioso. Deve exigir motivo obrigatório de cancelamento de forma irrevogável na transição de status.

## 19. Regras de prazo para auto-fechamento
O fechamento automático só pode ocorrer após prazo configurado de aceite (ex: 72 horas). Um worker ou rotina agendada deve validar esse prazo a partir da data de resolução.

## 20. Regras de reabertura controlada
A reabertura deve ser controlada por política e prazo. Após o prazo máximo (ex: 7 dias pós-fechamento), o chamado não pode mais ser reaberto, sendo necessária a criação de um novo chamado vinculado.

## 21. Auditoria e histórico obrigatório
Auditoria detalhada de resolução, aceite, rejeição, fechamento e reabertura é compulsória. A reabertura deve registrar quem reabriu, quando, o motivo e o contexto, sem jamais sobrescrever os registros anteriores.

## 22. Integração com SLA
SLA não deve ser alterado neste item. As regras de SLA devem continuar a contar ou parar conforme as transições existentes ou compatíveis com as premissas atuais. O marco de pausa de SLA para o cliente tipicamente se dá no momento do "Resolvido".

## 23. Integração com motor de aprovações da Sprint 4
A aprovação pendente bloqueante da Sprint 4 deve impedir o fechamento definitivo quando aplicável. O motor de aprovações não deve ser alterado neste item, devendo os bloqueios serem respeitados pelas novas regras do ciclo de vida.

## 24. Integração com permissões e perfis
Apenas os perfis adequados podem executar ações no ciclo de vida. Solicitantes fazem o aceite/rejeição de seus chamados; atendentes realizam a resolução técnica; cancelamentos e reaberturas dependem de permissões explícitas.

## 25. Impacto no fluxo legado de encerramento
O fluxo legado de `EncerrarChamadoUseCase` deve ser preservado até a implementação dos itens funcionais. Qualquer mudança funcional deve ficar para os próximos itens da Sprint 5.

## 26. Impacto no fluxo legado de reabertura
O fluxo legado de `ReabrirChamadoUseCase` também permanece intocado neste momento. Evoluções serão feitas iterativamente sem quebrar os endpoints atuais precipitadamente.

## 27. Ações permitidas por status
- **Resolvido:** Aceite, Rejeição, Fechamento Automático.
- **Fechado:** Reabertura (se dentro do prazo/política).
- **Em Atendimento:** Resolução, Cancelamento.

## 28. Ações bloqueadas por status
- Não se pode resolver um chamado já Fechado.
- Não se pode fechar um chamado sem estar Resolvido (a menos de regra de cancelamento).
- Não se pode cancelar um chamado já Resolvido ou Fechado.

## 29. Riscos técnicos
- Quebra de integrações legadas ou workers externos que ainda não reconhecem o novo status "Resolvido" vs "Fechado".
- Risco da lógica de bloqueio por aprovações da Sprint 4 não interceptar adequadamente uma transição de fechamento por timeout.

## 30. Decisões adiadas
A criação das tabelas/configurações exatas para a definição de "X dias para auto-fechamento" será definida no momento de sua implementação, não engessando o modelo de banco agora.

## 31. Critérios técnicos de aceite
- O documento deve listar todos os conceitos, regras obrigatórias, impactos e responsabilidades relativas ao ciclo de vida.
- O modelo documentado deve servir de fundação para os próximos passos de implementação sem a necessidade de reestruturar a arquitetura.

## 32. Próximos itens da Sprint 5
Separar status Resolvido e Fechado no fluxo de negocio, iniciando de fato a execução do design apresentado neste documento.
