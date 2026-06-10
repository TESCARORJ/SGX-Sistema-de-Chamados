# Sprint 4 - Impacto do Motor de Aprovação no Fluxo Atual de Chamados

## 1. Objetivo

Este documento tem como objetivo registrar e detalhar o impacto real do motor de aprovação ITSM sobre o fluxo operacional de chamados no **SGX Sistema de Chamados**. Ele busca mapear com precisão técnica e funcional quais processos foram afetados e quais permanecem inalterados, servindo como guia de governança e compatibilidade. O foco é evitar interpretações incorretas de que o motor bloqueia a operação geral do sistema, esclarecendo a coexistência com o SLA, status e fluxos legados.

---

## 2. Resumo executivo

* **Governança de Aprovação:** O motor de aprovações ITSM adiciona uma camada de conformidade configurável e automatizada que avalia o contexto do chamado (categoria, catálogo, custo, etc.) e dispara solicitações formais de aprovação.
* **Abertura Preservada:** O fluxo de abertura de chamados, tanto via portal quanto pela área administrativa, continua funcionando sem impedimentos ou bloqueios prévios.
* **Atendimento Livre:** O atendimento diário e o andamento comum dos chamados persistem normalmente. O técnico pode interagir com o ticket a qualquer momento.
* **Ações Consultivas e de Triagem:** Ações como visualizar, comentar, incluir anexos, triar e classificar o chamado continuam liberadas mesmo se houver pendências de aprovação.
* **Restrição Cirúrgica:** Apenas ações finais e sensíveis (como o encerramento do chamado) são bloqueadas, e somente quando houver uma `InstanciaAprovacaoChamado` ativa com status pendente e que esteja explicitamente configurada como bloqueante.
* **Fluxo Legado Mantido:** A rotina clássica de `AprovacaoChamado` legado continua coexistindo de forma independente.
* **SLA Intacto:** O SLA operacional do chamado não sofre pausas ou alterações automáticas causadas pelo motor nesta etapa.
* **Status do Chamado:** O status operacional do chamado (ex: *Em Atendimento*, *Aberto*) não é alterado automaticamente pelas transições da instância de aprovação, mantendo o controle operacional com os analistas.

---

## 3. O que mudou

Com a introdução do Motor de Aprovação ITSM na Sprint 4, as seguintes novidades foram implementadas no sistema:
* **Estrutura do Motor de Aprovação:** Introdução das entidades `ConfiguracaoRegraAprovacao`, `InstanciaAprovacaoChamado`, `EtapaAprovacaoChamado` e `DecisaoAprovacaoChamado` para guiar a lógica de governança.
* **Regras de Negócio Configuráveis:** Criação de uma interface e endpoints administrativos em `/admin` para parametrizar regras por natureza, tipo de solicitação, catálogo de serviço, impacto, urgência, custo e nível de risco.
* **Geração Automática de Instâncias:** O caso de uso `GerarAprovacaoObrigatoriaChamadoUseCase` avalia chamados sob demanda e gera solicitações de aprovação dinamicamente conforme as regras vigentes.
* **Bloqueio Operacional Inteligente:** Interceptação em momentos sensíveis de transição (ex: encerramento) utilizando o caso de uso `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase` para barrar a finalização de tickets com pendências bloqueantes.
* **Histórico e Trilha de Auditoria:** Registro detalhado de logs e decisões (`DecisaoAprovacaoChamado`) capturando a assinatura do decisor e snapshots das regras no momento da avaliação.
* **Painel Administrativo de Pendências:** Nova seção na área administrativa do sistema para exibição e filtragem de pendências ativas de aprovação para os respectivos decisores.

---

## 4. O que não mudou

A estabilidade e os fluxos diários do Service Desk foram resguardados através das seguintes premissas:
* **Abertura de Chamado:** Nenhuma regra de aprovação impede a criação de um ticket no banco de dados. O solicitante consegue registrar a demanda sem bloqueio prévio.
* **Fluxo de Atendimento Comum:** Ações de triagem, investigação e diagnóstico realizadas pelos técnicos continuam liberadas.
* **Cálculo e Controle do SLA:** O tempo de atendimento e solução (SLA) permanece correndo de forma contínua conforme o calendário corporativo do chamado.
* **Status do Chamado:** A transição de status do chamado continua sendo uma ação manual do técnico da fila (ex: mover para *Em Atendimento*), não havendo alteração invisível de status.
* **Ecossistema Legado:** A antiga entidade `AprovacaoChamado` e seus respectivos endpoints (`AdminAprovacaoChamadosController`) continuam plenamente operacionais.
* **Interações Gerais do Ticket:** O registro de comentários, anexos e triagem técnica permanece livre de travas.
* **Cenários sem Regra Aplicável:** Chamados que não ativem nenhuma regra compatível no motor ITSM seguem o fluxo operacional normal sem nenhuma solicitação ou validação de aprovação.

---

## 5. Impacto na abertura de chamado

A criação de chamados, seja no portal de solicitantes ou no console administrativo, não sofre nenhuma interrupção pelo motor de aprovações:
* **Abertura Livre:** O fluxo de gravação inicial do ticket no banco permanece direto e desimpedível.
* **Coleta de Contexto:** Dados informados na abertura (como natureza ITSM, catálogo de serviço, impacto e urgência informados pelo usuário) servem de base para que o motor, de forma subsequente ou reativa, avalie a necessidade de aprovação.
* **Geração Posteriore:** Se aplicável, a `InstanciaAprovacaoChamado` é gerada após a criação do contexto do chamado, sem pausar ou lançar erros durante a persistência inicial do ticket.
* **Nenhum Falso Positivo:** Se o chamado não ativar nenhuma política ativa e vigente, ele segue a jornada sem qualquer solicitação pendente no banco.

---

## 6. Impacto na triagem

A etapa de triagem e classificação técnica, realizada por líderes de equipe ou despachantes, funciona de forma contínua:
* **Diagnóstico Permitido:** Analistas de triagem podem ler o ticket, redefinir categorias, atribuir responsabilidade a grupos técnicos e registrar notas.
* **Aprovação não Impede Triagem:** O fato de haver uma aprovação pendente no motor não interfere na triagem consultiva ou no saneamento inicial da fila.
* **Gatilho de Reavaliação:** Se durante a triagem o analista alterar dados altamente sensíveis (como subir o impacto para Alto ou alterar o custo estimado), o caso de uso `ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase` será acionado, atualizando o status da instância para `EmReavaliacao` para garantir o compliance da nova informação.

---

## 7. Impacto no atendimento

O atendimento técnico e operacional é preservado para evitar a paralisação do Service Desk:
* **O Chamado não é Congelado:** O bloqueio gerado pelo motor não significa que o ticket está inoperante ou "congelado". 
* **Atividades Liberadas:** Técnicos podem prosseguir com testes, registrar diagnósticos, interagir com o solicitante, agendar tarefas operacionais e registrar progressos no atendimento.
* **Restrição Focada:** A trava atua exclusivamente em pontos de transição finalística. O técnico é livre para interagir com o chamado, sabendo apenas que não poderá fechá-lo até a aprovação formal.

---

## 8. Impacto em comentários, anexos e evidências

A comunicação e a documentação do ticket continuam sendo suportadas:
* **Comentários Livres:** A inserção de comentários (públicos ou internos) permanece liberada a qualquer momento tanto para solicitantes quanto para técnicos.
* **Evidências e Anexos:** A adição de arquivos, screenshots ou relatórios de diagnóstico não sofre qualquer bloqueio pelo motor.
* **Apoio à Decisão:** Essas evidências inseridas no chamado servem de subsídio para o aprovador analisar a demanda e tomar a decisão de aprovação com maior clareza.

---

## 9. Impacto em atribuição, assumir e encaminhar chamado

A distribuição de chamados e a gestão da fila permanecem inalteradas:
* **Ações de Atribuição:** Ações como *Assumir Chamado*, *Direcionar para Grupo Técnico* ou *Transferir entre Filas* não são impedidas pelo motor de aprovações.
* **Organização da Fila:** O fluxo de triagem e atribuição a técnicos específicos continua funcionando conforme as permissões de perfil do sistema.
* **Sem Avanço Final:** Mover a propriedade do chamado entre grupos ou analistas não constitui uma movimentação finalística, logo, o interceptor de bloqueio não interfere nessas ações.

---

## 10. Impacto na alteração de status

As transições de estados operacionais do chamado respeitam as seguintes diretrizes:
* **Sem Automação de Status:** O motor não altera o status operacional do chamado de forma autônoma após aprovações ou rejeições (ou seja, o chamado não passa a ser *Fechado* automaticamente após a aprovação; o analista deve concluir a ação manualmente).
* **Bloqueio de Status Sensíveis:** Se o chamado estiver sob pendência de aprovação bloqueante, transições para status finais (ex: *Resolvido*, *Fechado*, *Encerrado*) ou que impliquem avanço final do workflow são interceptadas e impedidas pelo sistema.
* **Status Intermediários:** Transições para estados como *Em Atendimento*, *Pendente com Fornecedor* ou *Aguardando Solicitante* continuam funcionando de acordo com as regras operacionais preexistentes.

---

## 11. Impacto no encerramento do chamado

O encerramento é o principal ponto de controle do motor de aprovações:
* **Ação Sensível por Excelência:** O encerramento de chamados (uso de `EncerrarChamadoUseCase` ou alteração para status final) é considerado a ação finalística chave.
* **Travamento por Pendência:** Se o chamado tiver qualquer `InstanciaAprovacaoChamado` com status `Pendente` ou `EmReavaliacao` associada a uma regra com flags `Bloqueante = true` e `ExigeAprovacao = true`, o encerramento é impedido e uma exceção de validação é exibida ao usuário.
* **Desbloqueio Pós-Aprovação:** Uma vez que o aprovador registre a decisão de aprovação (mudando o status da instância para `Aprovada`), a trava operacional é removida, permitindo que o analista de suporte encerre o ticket normalmente.
* **Comportamento na Rejeição:** A rejeição da aprovação resolve a pendência (a instância muda para `Reprovada`), mas **não encerra nem cancela o chamado automaticamente** a nível de domínio, cabendo ao fluxo operacional determinar a tratativa (retorno à triagem, redimensionamento ou cancelamento manual do chamado).

---

## 12. Impacto no SLA

A governança sobre o SLA do chamado foi mantida isolada nesta etapa:
* **Prazos do Motor:** O motor de aprovações possui controle sobre o prazo para a tomada de decisão (`PrazoDecisaoHoras` e `DeveExpirarEm`) da instância.
* **Sem Recálculo de SLA do Chamado:** Esse prazo de aprovação funciona apenas como um metadado de controle corporativo e **não interfere na contagem de SLA operacional do ticket**. 
* **SLA Contínuo:** O cronômetro de SLA de solução do chamado continua rodando mesmo enquanto o ticket aguarda aprovação de um gestor.
* **Integrações Futuras:** Evoluções no roadmap podem contemplar a pausa automática ou recálculo de SLAs em status de pendência de aprovação, mas essa lógica não está implementada funcionalmente na Sprint 4.

---

## 13. Impacto no fluxo legado de aprovação

Para garantir compatibilidade total e segurança na transição tecnológica:
* **Coexistência Pacífica:** As rotinas e tabelas de `AprovacaoChamado` (legado) continuam ativas e operacionais.
* **Isolamento de Interfaces:** Os controllers administrativos do motor novo (`AdminAprovacoesMotorController`) e do legado (`AdminAprovacaoChamadosController`) residem em escopos separados na API.
* **Anti-Duplicidade Lógica:** O motor novo verifica se já existe uma aprovação legada equivalente e pendente por catálogo associada ao ticket. Caso positivo, o motor **não gera** uma nova instância duplicada para evitar retrabalho operacional e conflitos de workflow.

---

## 14. Quando o chamado fica bloqueado

Um chamado será bloqueado para ações sensíveis/finais se, e somente se, atender cumulativamente aos seguintes critérios:
1. **Existência de Instância:** Estar associado a uma `InstanciaAprovacaoChamado` correspondente (filtrado por `ChamadoId`).
2. **Status Pendente:** A instância de aprovação deve estar no status `Pendente` ou `EmReavaliacao`.
3. **Exigência de Decisão:** O flag `ExigeAprovacao` da regra/instância deve ser `true`.
4. **Flag Bloqueante Ativo:** O flag `Bloqueante` da regra/instância deve ser `true` (em linha com o enum de efeito operacional `ExigirAprovacaoEBloquearAvanco`).
5. **Ação Final:** A movimentação solicitada pelo usuário for classificada como sensível/final pelo interceptor (ex: Encerramento do chamado).

---

## 15. When the chamado não fica bloqueado

O chamado seguirá fluxo operacional normal e não sofrerá bloqueios se:
* **Sem Instância Ativa:** Não houver nenhuma `InstanciaAprovacaoChamado` pendente associada ao chamado.
* **Instância Resolvida:** A instância de aprovação já tiver sido decidida (status `Aprovada`, `Reprovada`, `Cancelada`, `Expirada` ou `Substituida`).
* **Regra Informativa:** O flag `Bloqueante` da regra/instância for `false` (efeito operacional configurado como `Sinalizar` ou `ExigirAprovacao` sem bloqueio).
* **Ação Operacional Comum:** A ação solicitada for de caráter consultivo, colaborativo ou preparatório (ex: visualizar chamado, comentar, triar, assumir ou direcionar).
* **Pendência de Outro Chamado:** O chamado avaliado não possuir bloqueios próprios, mesmo que outros chamados da fila estejam travados.

---

## 16. Ações permitidas versus ações sensíveis

| Tipo de ação | Exemplo | Impacto esperado |
| :--- | :--- | :--- |
| **Consultiva** | Visualizar chamado e histórico | **Permitido** |
| **Colaborativa** | Inserir comentários públicos ou internos | **Permitido** |
| **Evidência** | Fazer upload de anexos e evidências técnicas | **Permitido** (se o fluxo atual permitir) |
| **Organização** | Assumir, atribuir ou encaminhar a outro grupo técnico | **Permitido** (se o fluxo atual permitir) |
| **Triagem** | Classificar categoria, subcategoria ou urgência | **Permitido** (desde que não resulte em avanço final) |
| **Sensível** | Alterar status para estados finais intermediários | **Pode bloquear** (conforme regras operacionais atuais) |
| **Final** | Encerrar ou resolver o chamado | **Bloqueado** (se houver pendência bloqueante no motor) |
| **Aprovação** | Registrar decisão de aprovação ou rejeição | **Permitido** (resolvido via endpoints do motor) |

---

## 17. Relação com reavaliação por dados sensíveis

* **Avaliação de Mudança de Dados:** Se um chamado sofrer alteração em campos chaves (como natureza, catálogo, prioridade, impacto, urgência, custo ou risco) após já ter sido aprovado ou estar pendente, o motor pode invocar a reavaliação.
* **Status EmReavaliacao:** A reavaliação coloca a `InstanciaAprovacaoChamado` sob o status `EmReavaliacao`.
* **Retorno do Bloqueio:** Se a instância reavaliada for marcada como bloqueante, a trava de encerramento volta a atuar imediatamente sobre o chamado até que uma nova decisão formal seja tomada.
* **Snapshot Preservado:** O motor reavalia os dados contra a mesma instância gerada anteriormente, não criando um novo registro de instância duplicada no banco de dados.

---

## 18. Riscos de interpretação

Para evitar atritos e falhas de comunicação operacional, as equipes devem estar cientes dos seguintes riscos:
* **Achar que o chamado está "congelado":** O bloqueio atua apenas no encerramento; técnicos e solicitantes podem continuar interagindo e registrando andamentos normalmente.
* **Confundir prazos:** Achar que o prazo limite do aprovador (`DeveExpirarEm`) interrompe ou altera o cronômetro do SLA operacional do chamado.
* **Expectativa de encerramento automático:** Achar que a aprovação do chamado realiza transição de status para *Resolvido* ou *Fechado* de forma automatizada.
* **Duplicidade de aprovação:** Criar uma regra no motor novo para um catálogo que já possui fluxo de aprovação legada ativo, gerando duplicidade operacional indesejada.
* **Bloqueio total da fila:** Cadastrar regras genéricas com flag `Bloqueante = true` para incidentes simples de baixo custo, paralisando a operação rotineira da central de serviços.

---

## 19. Validações já realizadas

A segurança nas interações do motor de aprovação no fluxo atual foi garantida pelos seguintes testes prévios de homologação:
* **Item 54 (Bloqueio por Pendência):** Validação de que transições de encerramento de chamado são bloqueadas quando há aprovações pendentes marcadas como bloqueantes.
* **Item 55 (Aprovação e Liberação):** Validação de que a aprovação formal libera o analista de suporte para encerrar o chamado normalmente.
* **Item 56 (Rejeição de Aprovação):** Validação do comportamento lógico após rejeição da instância.
* **Item 59 (Regressão do Fluxo Legado):** Garantia de que a criação de aprovações legadas de catálogo continua operando de forma íntegra.
* **Item 60 (Regressão de Abertura e Atendimento):** Homologação de que chamados sem regras aplicáveis podem ser abertos, triados, atendidos e resolvidos sem travas operacionais.
* **Item 61 (Modelo do Motor):** Validação da consistência conceitual e de integridade referencial das entidades.
* **Item 62 (Regras de Aprovação ITSM):** Homologação das regras configuráveis por contexto de risco, custo e catalogação.

---

## 20. Recomendações operacionais

* **Criação Gradual:** Comece implementando regras de aprovação bloqueantes apenas para serviços específicos e de altíssimo risco ou custo.
* **Evite Bloqueios em Fallback:** Mantenha regras genéricas ou de fallback como não bloqueantes (`Bloqueante = false`) para evitar gargalos acidentais na operação diária.
* **Treinamento e Divulgação:** Comunique à equipe de atendimento que chamados sob aprovação continuam aptos a receber triagem, investigação e comentários.
* **Acompanhamento de Prazos:** Monitore periodicamente as instâncias de aprovação próximas do vencimento para evitar que o SLA operacional do chamado expire por inércia do aprovador.
* **Homologação Prévia:** Sempre realize o ciclo de vida completo de testes de uma nova regra em ambiente de validação antes de ativá-la em produção.

---

## 21. Limitações atuais

* **Orquestração Passiva:** O avanço das etapas e instâncias depende de chamadas diretas de Use Cases no código; não existe processamento assíncrono automatizado em segundo plano (jobs).
* **Grupo Aprovador e Quórum:** As funcionalidades de aprovação por equipes (grupos) e limites de quórum de votos são suportadas de forma estrutural pelas entidades, mas não contam com lógica de distribuição de voto real implementada nesta fase.
* **Ausência de Integração de SLA:** O motor não possui recursos funcionais de suspensão temporária de cronômetro de SLA de chamado durante o período de aprovação pendente.
* **Ausência de Delegação Real:** Direcionamento de voto em caso de ausência do aprovador não está implementado na lógica da Sprint 4.

---

## 22. Conclusão

O impacto do motor de aprovações ITSM sobre o fluxo de chamados encontra-se plenamente documentado e validado. O design adotado no **SGX Sistema de Chamados** assegura que a governança de aprovação atue de forma precisa e cirúrgica. As atividades de abertura, triagem, atendimento diário, SLA e comentários permanecem preservadas, agindo como facilitadores do suporte técnico, enquanto o bloqueio operacional incide unicamente sobre ações finalísticas críticas quando houver pendências bloqueantes ativas.
