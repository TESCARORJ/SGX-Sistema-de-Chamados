# Impacto no Fluxo de Chamados - Sprint 5

Este documento detalha o impacto técnico e de negócios no fluxo atual de chamados do SGX, com foco nas entregas da **Sprint 5**, que introduziu regras avançadas de fechamento, aceite e auditoria de chamados.

## 1. O que Mudou (Novos Comportamentos)

O ciclo de vida de um chamado recebeu uma nova etapa intermediária ("Resolvido") e regras de auto-fechamento:

- **Separação Resolvido vs Encerrado**: O fluxo agora possui um status intermediário obrigatório. Quando a equipe técnica propõe uma solução, o chamado transita para **Resolvido**. O status **Encerrado** (fechamento definitivo) é atingido apenas quando ocorre o aceite.
- **Aceite/Rejeição pelo Solicitante**: Solicitantes agora podem aceitar ou rejeitar a solução proposta no portal.
    - Se **Aceita**: O chamado transita imediatamente para `Encerrado`.
    - Se **Rejeitada**: O chamado retorna para `Em Atendimento` (e o SLA é reativado/pausado de acordo com a política vigente).
- **Fechamento Automático (Timeout)**: Caso o solicitante não realize nenhuma ação no chamado `Resolvido` dentro de um prazo específico, uma política de fundo (Worker) move automaticamente o chamado para `Encerrado`.
- **Configuração Administrativa**: O prazo de auto-fechamento não é fixo em código. Administradores podem configurar, em horas, o limite de espera pelo aceite através dos `ParametrosSistema`.
- **Obrigatoriedades (Solução/Cancelamento)**: Resoluções exigem preenchimento formal da *Solução Técnica*, e cancelamentos exigem *Motivo de Cancelamento*.
- **Auditoria Avançada**: Transições críticas (Resolver, Aceitar, Rejeitar, Fechar Automaticamente, Reabrir) geram registros detalhados de auditoria (via `IAuditoriaService` e `HistoricoChamado`).

## 2. O que foi Preservado (Fluxos Legados Compatíveis)

- **Fluxo Inicial (Aberto -> Em Atendimento)**: Permanece inalterado.
- **SLA de Resposta e Solução**: A contagem do SLA continua funcional. Para o SLA de Solução, o marco de parada ocorre no momento da transição para `Resolvido`.
- **Painéis e Consultas Existentes**: Como os relacionamentos de banco foram introduzidos mantendo compatibilidade com as estruturas existentes (os novos campos, como `AceitoPorUsuarioId` e `ResolvidoEm`, permitem nullability), relatórios antigos não foram quebrados.
- **Aprovações Pendentes**: O bloqueio de encerramento em caso de aprovações ativas foi preservado e integrado nativamente à nova separação `Resolvido`/`Encerrado`. Não é possível resolver ou fechar um chamado se existirem fluxos de aprovação abertos.

## 3. Reabertura Controlada

- Foi introduzida uma regra sistêmica para **Reabertura Controlada**.
- Um chamado só pode ser reaberto a partir de status finais (ou do recém-introduzido status `Encerrado`) se estiver dentro do **prazo máximo de reabertura** configurado pelos administradores.
- Tentativas de reabertura fora desse prazo resultam em bloqueio, garantindo o versionamento correto das resoluções (forçando a abertura de um *novo* chamado em vez de reabrir um muito antigo).
- Todo fluxo de reabertura registra histórico detalhado e logs de auditoria.

## 4. Limitações e Decisões Futuras (Abertas)

- **Notificações**: Apesar de o fluxo técnico estar pronto (rejeição, auto-fechamento, aceite), o envio assíncrono de notificações transacionais (e-mail/SMS) informando o solicitante que o chamado está aguardando aceite foi mapeado mas dependerá das sprints focadas no `Worker.Email` e mensageria.
- **Painel de Chamados "Aguardando Aceite"**: O solicitante pode ver o chamado no portal, mas a criação de uma *widget* específica (dashboard) destacando os chamados pendentes de aceite do usuário poderá ser refinada futuramente no Frontend.
- **Métricas de Re-trabalho (Rejeições)**: Como os eventos de rejeição estão sendo auditados e salvos no `HistoricoChamado`, abre-se espaço no futuro para a criação de painéis analíticos no Admin para acompanhar taxas de rejeição ("First Contact Resolution" e re-trabalho por equipe).

## Resumo Arquitetural

As regras foram adicionadas estritamente na camada `Application` (Use Cases) e as invariantes validadas nos agregados da camada `Domain`. A persistência no `Infrastructure` e mapeamentos via `EF Core` sofreram adições progressivas (`Migrations`), sendo concluídas sem romper as tabelas de `chamados` e mantendo retrocompatibilidade com chamados em andamento (que podem ser resolvidos pelas novas lógicas imediatamente).
