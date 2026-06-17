# Sprint 5: Impacto no Fluxo Atual de Chamados

## 1. Objetivo
Documentar, de forma técnica e objetiva, o impacto das novas regras da Sprint 5 (Regras de Fechamento, Aceite e Reabertura) no fluxo atual de chamados, explicitando o ciclo de vida, novos comportamentos, comportamentos preservados e limitações da versão atual.

## 2. Escopo
- Alterações no ciclo de vida do chamado (status Resolvido vs. Fechado/Encerrado).
- Obrigatoriedade de solução técnica e motivo de cancelamento.
- Aceite e rejeição pelo solicitante com retorno ao atendimento.
- Fechamento automático baseado em prazo configurável.
- Reabertura controlada (auditada e restrita a políticas de prazo).
- Integração de segurança com o Motor de Aprovação (Sprint 4).

## 3. Fora de Escopo
- Recriação de SLAs (preservados integralmente).
- Workflow estrutural do Motor de Aprovações ITSM (mantido).
- Reestruturação de catálogos de requisição ou filas (tratados em outras Sprints).

## 4. Estado Anterior do Fluxo (Antes da Sprint 5)
1. **Atendimento Finalizado:** O analista "Encerrava" o chamado diretamente. Não havia transição intermediária aguardando a concordância do solicitante.
2. **Reabertura Ilimitada:** Qualquer chamado encerrado podia ser reaberto a qualquer momento, dificultando medição de SLA final e qualidade técnica.
3. **Cancelamento Permissivo:** Cancelamentos não possuíam exigência estrutural de um motivo rastreável na resolução.
4. **Resolução Técnica Opcional:** Era possível fechar chamados sem preencher uma "Solução Técnica" clara e dedicada.

## 5. Estado Atual Após a Sprint 5
O fluxo tornou-se alinhado às boas práticas do ITIL/ITSM, introduzindo uma governança de encerramento baseada em aceite (resolução versus encerramento definitivo).

### 5.1 Ciclo de Vida do Chamado
As diferenças essenciais entre os novos status são:
- **Aberto:** Aguardando primeira atribuição/início. (Inalterado)
- **Em Atendimento:** Analista atuando ativamente. (Inalterado)
- **Resolvido (Novo Status Operacional):** A solução foi entregue ao solicitante. O chamado entra em *período de carência* aguardando aceite. SLA de atendimento é pausado/finalizado.
- **Encerrado/Fechado:** Definitivamente encerrado, seja por aceite explícito do solicitante, seja por decurso de prazo (Fechamento Automático). Ficam limitados apenas à visualização.
- **Reaberto:** O chamado retornou para atendimento com registro de auditoria, restabelecendo metas, contadores e filas.
- **Cancelado:** Encerrado de forma precoce sem solução técnica, mas agora exigindo motivo obrigatório.

### 5.2 Resolução Técnica
A transição para **Resolvido** exige obrigatoriamente que a equipe técnica preencha a string da *Solução Técnica*. Esta solução será auditada e copiada na auditoria (exibida no Frontend e Portal).

### 5.3 Aceite do Solicitante
Apenas chamados *Resolvidos* são expostos no Portal para o aceite do solicitante.
- O aceite transita o chamado de `Resolvido` para `Encerrado`.
- É o gatilho para preenchimento oficial do `AceitoEm`.

### 5.4 Rejeição da Solução e Retorno ao Atendimento
Caso o solicitante não concorde com a solução:
- A rejeição exige, no Portal, um motivo textualmente obrigatório.
- O chamado **retorna para Em Atendimento**, não reabrindo um ciclo do zero, mas dando continuidade à solicitação devolvida (o `ResolvidoEm` anterior é apagado para um novo ciclo de resolução).
- **Importante:** Rejeição (solução inválida) **não é o mesmo** que Reabertura (novo problema no serviço).

### 5.5 Fechamento Automático
Para contornar o esquecimento dos solicitantes, o Administrador pode configurar globalmente o prazo de *Fechamento Automático*. 
- Um *worker/job* ou fluxo processual avalia a idade da resolução.
- Se ultrapassado o prazo (em dias ou horas corridas, configurável), transita de `Resolvido` para `Encerrado`.
- **Importante:** Fechamento automático **não é tratado** computacionalmente como "Aceite manual". É um fechamento administrativo (via sistema), distinguindo métricas de SLA passivo e proativo.

### 5.6 Reabertura Controlada
A ação explícita de "Reabrir" (seja pelo Admin ou Solicitante) passou a exigir uma avaliação de política. A Sprint 5 introduziu auditoria base (Motivo de Reabertura obrigatório). Além do mais, a reabertura reseta o status para Em Atendimento de modo rastreado.

### 5.7 Cancelamento
Torna-se proibitivo efetuar o `CancelarChamadoUseCase` sem a fundamentação (Motivo). Diferencia os dados nulos na coluna Solução, populando o Cancelamento.

## 6. Histórico e Auditoria
Novos eventos de banco de dados foram anexados no histórico da linha do tempo do chamado:
- `ChamadoResolvidoEvento` (Armazena a cópia da Solução)
- `SolucaoChamadoRejeitadaEvento` (Armazena o Motivo)
- `ChamadoReabertoEvento` (Armazena o Motivo e o Solicitante)
- `ChamadoAceitoEvento` 

## 7. Aprovação Pendente Bloqueante (Ação Preservada da Sprint 4)
- **Bloqueio a Fechamento Final:** A integração assegura que se há uma Instância de Aprovação com a diretriz bloqueante não decidida (Status "Pendente"), **nenhum encerramento ou aceite pode ser finalizado**. Apenas acompanhamentos comuns fluem.
- O fechamento automático também reconhece a bloqueante e aborta a execução silenciosamente até o parecer formal.

## 8. Matriz de Impactos

| Área de Negócio / Domínio | Impacto Sistêmico |
| ------------------------- | ----------------- |
| **Abertura de Chamados**  | **Nulo:** O fluxo e endpoints de submissão permaneceram idênticos. |
| **Atendimento Diário** | **Moderado:** Analistas não encerram mais. Eles *resolvem*, preenchendo obrigatoriamente a solução. |
| **Portal do Solicitante** | **Alto:** A área de visualização do chamado habilitou as ações primárias de "Aceitar" ou "Rejeitar", condicionadas ao status Resolvido. |
| **Motor de Aprovações** | **Leve/Nulo:** O motor segue inalterado, apenas as regras de validação de fluxo da Sprint 5 estendem sua proteção à arquitetura. |
| **SLA** | **Leve:** O SLA não foi reescrito, porém o impacto da rejeição e reabertura altera os gatilhos das datas limite passivas. |

## 9. Compatibilidade com Fluxo Legado
Para retrocompatibilidade (scripts e integrações antigas):
- O fechamento administrativo via `EncerrarChamadoUseCase` foi **preservado** (não é exigido que a solução seja preenchida caso seja um encerramento forçado do Legado sem resolução).
- As restrições se aplicam de forma pesada nos novos UseCases da API/Portal, blindando a integridade das regras a partir desta Sprint.

## 10. Riscos e Cuidados Operacionais
- O Motor de Fechamento Automático exige agendamento ativo via `Worker Service` ou trigger do CRON. O não agendamento dessa base de domínio gera represamento de chamados na caixa de "Resolvidos".
- Erro no preenchimento do prazo de configuração administrativa pode encerrar chamados prematuramente.
- Solicitações em lote com encerramento forçado não podem ignorar as novas validações da Sprint 4 (Aprovação bloqueante).

## 11. Limitações e Decisões Futuras
1. **Recalculadora de SLA:** O tempo suspenso no status "Resolvido" ainda necessita de regras avançadas em relatórios que considerem apenas as horas úteis em atendimento ativo, algo a ser refinado em uma Sprint futura de métricas.
2. O Fechamento Automático ocorre de ponta a ponta sem disparar E-mail no momento. (Sprint 6).
3. Avaliar se "Cancelar" também deve respeitar bloqueio de aprovação no futuro, se isso configurar quebra de segurança de dados.

## 12. Checklist de Aceite
- [x] Detalhar fluxo antigo x atual.
- [x] Diferenciar "Resolver" de "Encerrar".
- [x] Diferenciar "Rejeitar" de "Reabrir".
- [x] Explicar blindagem da Aprovação Pendente Bloqueante.
- [x] Mapeamento de Riscos.
- [x] Compatibilidade Legada.

## 13. Conclusão
A implantação da governança de ciclo de vida atende perfeitamente ao desenho ITSM proposto na Sprint 5. O acoplamento técnico das validações baseadas na Clean Architecture e Domain-Driven Design manteve estabilidade funcional sem regredir cenários construídos nas quatro sprints anteriores.
