# Sprint 4 - Critérios de Testes Técnicos do Motor de Aprovação ITSM

## 1. Objetivo

Este documento define os critérios técnicos mínimos e as suítes de validação utilizadas para testar, homologar e proteger o motor de aprovações ITSM do **SGX Sistema de Chamados** contra regressões de software. O foco é estabelecer bases claras para que novos desenvolvimentos ou refatorações futuras não corrompam a integridade lógica de controle do fluxo de tickets.

---

## 2. Escopo da validação

A cobertura de testes técnicos e funcionais da Sprint 4 engloba as seguintes áreas:
* **Domínio:** Consistência das entidades core (`ConfiguracaoRegraAprovacao`, `InstanciaAprovacaoChamado`, `EtapaAprovacaoChamado` e `DecisaoAprovacaoChamado`).
* **Contratos:** Validação de DTOs e contratos de requests de controle e configuração.
* **Validators:** Regras de preenchimento obrigatório e consistência lógica de inputs.
* **Use Cases:** Execução isolada de fluxos de negócio.
* **Geração de Aprovação:** Escolha determinística de políticas de aprovação com base no contexto.
* **Bloqueio:** Interceptadores de movimentações sensíveis e impedimento de encerramento de tickets.
* **Aprovação e Rejeição:** Registro de decisões formais, transições de status da instância e impacto no ticket.
* **Reavaliação por Dados Sensíveis:** Recálculo e reengajamento de trancas operacionais após modificações estruturais do chamado.
* **Grupo Aprovador Futuro:** Compatibilidade e preenchimento de snapshots de aprovação difusa por equipes.
* **Multi-nível Estrutural:** Geração e processamento de etapas de aprovação sequenciais e paralelas.
* **Regressão do Fluxo Legado:** Proteção à integridade do módulo antigo `AprovacaoChamado`.
* **Regressão de Abertura e Atendimento:** Garantia de que fluxos não impactados continuem limpos.
* **Roadmap/Checklist:** Testes de auditoria de progresso da sprint.
* **EF Pending Model Changes:** Verificação de integridade e sincronismo de modelos com o banco de dados.
* **Build:** Sucesso na compilação integral da solução.

---

## 3. Critérios gerais de aceite técnico

Para que qualquer alteração no motor de aprovações ITSM seja considerada aceita, os seguintes critérios devem ser cumpridos:
* **Compilação Sem Erros:** Toda a solução (Backend em .NET 9 e Frontend em Vue 3) deve compilar com êxito.
* **Justificativa de Warnings:** Novos avisos de compilação (warnings) não devem ser introduzidos, a menos que tecnicamente justificados.
* **Execução das Suítes:** Todos os testes de unidade e integração associados ao motor de aprovações devem passar sem falhas.
* **Passagem do Teste de Roadmap:** O teste `RoadmapSprint4MotorAprovacoesChecklistTests` deve ser executado e obter sucesso completo com base nos percentuais corretos.
* **Sem Mudanças Pendentes no EF:** O comando `has-pending-model-changes` do EF Core deve retornar limpo (sem alterações pendentes).
* **Ausência de Migrations Estruturais:** Apenas migrations de dados/checklist são permitidas para itens de documentação ou seeds.
* **Preservação de Comportamento:** Itens documentais não podem, sob qualquer pretexto, alterar o comportamento funcional e lógico do sistema.
* **Frontend Intacto:** Alterações no frontend não devem ocorrer em entregas de cunho puramente documental/backend, a menos que explicitamente solicitado.
* **Compatibilidade Legada:** O fluxo legado `AprovacaoChamado` deve manter comportamento original validado.
* **Proteção contra Duplicidade:** O motor de regras não deve criar solicitações redundantes no chamado se já existir uma pendência idêntica (legada ou nova).

---

## 4. Testes de domínio

Os testes a nível de domínio validam a integridade interna de cada entidade de negócio:
* **`ConfiguracaoRegraAprovacao`:** Validação do construtor, obrigatoriedade do criador, limites de caracteres em propriedades de texto (Nome com máximo de 180 e Descrição com 4000) e integridade dos critérios ITSM.
* **`InstanciaAprovacaoChamado`:** Garantia de que a instância herde corretamente as propriedades da regra no momento da criação, mantendo as informações imutáveis mesmo se a regra for editada posteriormente.
* **`EtapaAprovacaoChamado`:** Validação estrutural de níveis, ordem de avaliação e ramos de aprovação.
* **`DecisaoAprovacaoChamado`:** Integridade dos registros de aprovação, rejeição, cancelamento ou expiração.
* **Coerência de Status:** Impedir estados inconsistentes (ex: uma etapa aprovada em uma instância cancelada).
* **Coerência de Bloqueios:** Validação lógica de que regras marcadas como bloqueantes obrigatoriamente exijam aprovação (`ExigeAprovacao = true` e `Bloqueante = true`).
* **Snapshots de Auditoria:** Verificação de que nomes de regras, versões e descrições de critérios sejam gravados textualmente no banco para rastreabilidade permanente.
* **Custo/Risco/Prazo:** Garantia de que valores numéricos negativos não sejam aceitos em custo mínimo ou prazos de decisão.
* **Categoria/Subcategoria:** Validação de que, se uma subcategoria for associada a uma regra, a categoria pai correspondente deve ser obrigatoriamente informada.
* **Versionamento:** Validação de que a versão inicial da regra seja maior que zero.
* **Nível/Ordem/Ramo:** Validação estrutural das etapas para evitar loops ou deadlocks de workflow.

---

## 5. Testes de contratos e validators

Os testes de contratos e validadores garantem o saneamento básico de dados recebidos pelas APIs:
* **Requests Administrativos de Regras:** Validators associados ao cadastro de regras garantem que o nome não seja nulo, que a prioridade e ordem sejam positivas e que a vigência inicial não seja posterior à vigência final.
* **Requests de Decisão:** Validators para aprovar/reprovar instâncias e etapas exigem justificativas textuais quando aplicável e IDs válidos.
* **Validações de Nome, Versão, Ordem, Prioridade:** Limites de preenchimento impostos para evitar estouro de buffers no banco de dados.
* **Coerência Bloqueante x Exige Aprovação:** O validador rejeita regras de efeito operacional de permitir/sinalizar caso as mesmas tentem forçar o flag de exigência ou bloqueio de chamado.
* **Coerência de Aprovadores:** Validação de que regras que utilizam aprovador específico forneçam o ID do usuário correspondente, o mesmo valendo para aprovadores padrão e grupos de aprovação.
* **Justificativa Obrigatória:** A rejeição de uma aprovação exige justificativa obrigatória com detalhes do motivo no DTO de entrada.
* **Quórum Esperado/Atingido:** Validação de que o quórum atingido não pode ser cadastrado sem um quórum mínimo correspondente na regra.
* **Vigência Temporal:** O validador do request impede a gravação de vigências temporais sobrepostas de regras idênticas.
* **Filtros e Paginação:** Garantia de que requests de consulta suportem paginação padrão e filtros por ChamadoId e Decisor.

---

## 6. Testes de geração obrigatória de aprovação

Testados no caso de uso `GerarAprovacaoObrigatoriaChamadoUseCaseTests`:
* **Regra Ativa e Vigente:** O motor só deve selecionar regras que estejam ativas e no período correto de vigência.
* **Filtro por Natureza:** Validação de que chamados de uma determinada natureza (ex: Mudança) ativem a regra correspondente.
* **Catálogo e Serviço Sensível:** Validação da geração de instâncias de aprovação ao abrir chamados para itens de catálogo configurados como sensíveis.
* **Categoria/Subcategoria:** Avaliação da aplicação de regras vinculadas a subcategorias específicas de infraestrutura.
* **Impacto, Urgência e Prioridade Mínima:** Teste de piso de gravidade (ex: regras que só disparam se o chamado tiver impacto maior ou igual a Alto).
* **Custo e Risco:** Avaliação de regras financeiras (custo estimado maior que o limite configurado).
* **Algoritmo de Desempate:** Garantir que, havendo concorrência de regras compatíveis, o motor selecione a de maior Prioridade, maior Especificidade (quem filtra mais campos), menor Ordem e maior Versão.
* **Regra Genérica vs. Específica:** Validação de que uma regra específica de catálogo vença uma genérica de categoria.
* **Anti-Duplicidade de Instância:** Garantia de que o motor não gere uma nova solicitação caso já exista uma instância pendente equivalente para o mesmo chamado.
* **Compatibilidade com Fluxo Legado:** O motor previne duplicidade caso já exista uma aprovação legada pendente por catálogo associada ao ticket.

---

## 7. Testes de bloqueio por aprovação pendente

Testados no caso de uso `BloquearMovimentacaoAprovacaoPendenteUseCaseTests`:
* **Pendente ou Em Reavaliação:** O bloqueio deve atuar de forma ativa se a instância estiver pendente ou em reavaliação.
* **Flags Ativos:** O bloqueio só deve ocorrer se `ExigeAprovacao = true` e `Bloqueante = true` (efeito `ExigirAprovacaoEBloquearAvanco`).
* **Ignora Não-Bloqueantes:** Instâncias informativas (com `Bloqueante = false`) ou com efeito `Sinalizar` não devem travar a transição de status do chamado.
* **Ignora Resolvidos:** Instâncias que já mudaram de status para `Aprovada`, `Reprovada`, `Cancelada`, `Expirada` ou `Substituida` não podem gerar bloqueio.
* **Isolamento de Chamado:** A validação é isolada por `ChamadoId`, garantindo que bloqueios de um chamado não interfiram nas transições de outros.
* **Ações Sensíveis vs. Permitidas:** O interceptador de transição deve permitir a escrita de comentários e triagem intermediária, bloqueando apenas ações sensíveis como encerramento de chamados.

---

## 8. Testes de aprovação e liberação

Testados no caso de uso `AprovarAprovacaoChamadoUseCaseTests`:
* **Bloqueio Prévio:** Garantia de que o chamado esteja travado antes da execução do use case.
* **Transição de Status:** A aprovação bem-sucedida deve transicionar a instância para o status `Aprovada`.
* **Registro de Decisão:** Uma entidade `DecisaoAprovacaoChamado` deve ser registrada contendo a assinatura do decisor, justificativa e snapshot estrutural.
* **Liberação de Avanço:** O caso de uso de bloqueio deve retornar `Bloqueado = false` para o chamado após a aprovação da instância pendente.
* **Múltiplas Instâncias:** O chamado não deve ser liberado se restar qualquer outra instância bloqueante pendente.
* **Preservação de Etapas:** A aprovação de etapas em fluxos sequenciais ou paralelos deve atualizar o status individual da etapa antes de consolidar a instância principal.
* **Independência Operacional:** A aprovação não pode mover o status do chamado ou alterar o SLA de forma invisível.

---

## 9. Testes de rejeição/reprovação

Testados no caso de uso `ReprovarAprovacaoChamadoUseCaseTests`:
* **Status Reprovada:** A rejeição deve transicionar o status da instância para `Reprovada`.
* **Registro Formal:** Gravação de `DecisaoAprovacaoChamado` com tipo de decisão de rejeição.
* **Justificativa Preservada:** A mensagem justificando a rejeição é gravada de forma obrigatória no banco de dados.
* **Resolução de Pendência:** A reprovação extingue o status de pendente da instância (removendo o bloqueio causado por ela no encerramento).
* **Fluxo de Atendimento:** A rejeição não pode forçar o cancelamento ou arquivamento automático do chamado no domínio, mantendo o ticket ativo para tratativa manual.
* **Isolamento:** A reprovação afeta apenas a instância selecionada e o chamado vinculado.

---

## 10. Testes de reavaliação por dados sensíveis

Testados no caso de uso `ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCaseTests`:
* **Gatilho de Mudança:** Alterações em campos sensíveis (como natureza, catálogo, prioridade, impacto, urgência, custo ou risco) acionam a reavaliação.
* **Status EmReavaliacao:** A instância já aprovada ou pendente muda seu status para `EmReavaliacao`.
* **Retorno do Bloqueio:** Se a instância for de caráter bloqueante, o encerramento do chamado volta a ficar travado imediatamente.
* **Sem Duplicidade:** O use case não cria uma nova instância no banco, operando de forma idempotente sobre o registro original.
* **Mudanças Não Sensíveis:** Alterações simples em campos normais (ex: descrição textual do chamado) não disparam a reavaliação, mantendo o status da aprovação.

---

## 11. Testes de grupo aprovador futuro

* **Conceito Estrutural:** Validação de que a infraestrutura aceite regras parametrizadas com `TipoResolucaoAprovador = GrupoAprovadorFuturo`.
* **Snapshots de Grupo:** Verificação de que o campo `GrupoAprovadorSnapshot` e o flag `DecisorEhMembroGrupo` sejam gravados corretamente na decisão para auditoria futura.
* **Sem Automações:** Garantia de que a persistência não tente invocar rotinas de distribuição de e-mail ou quórum ativo para grupos, visto que essas regras são apenas estruturais na Sprint 4.
* **Sem Tabelas:** Confirmação de que nenhuma tabela de relacionamento de grupos foi criada acidentalmente nas migrations.

---

## 12. Testes de multi-nível estrutural

* **Consistência de Etapas:** Validação de que a geração do chamado para regras de múltiplos níveis gere registros de `EtapaAprovacaoChamado` ordenados por nível e ordem.
* **Snapshots de Nível:** Garantia de que cada aprovação de etapa grave no banco de dados os snapshots corretos de nível, ordem e ramo (ex: Nível 1, Ordem 1, Ramo "Financeiro").
* **Múltiplas Etapas Coexistentes:** Validação de que a resolução de uma etapa não interfira negativamente em outras etapas pendentes da mesma instância.
* **Falta de Orquestrador:** Confirmação de que o avanço de etapas dependa de chamadas explícitas e que a ausência de um processador em background não cause erros lógicos no fluxo simples.

---

## 13. Testes de regressão do fluxo legado de aprovação

Testados no caso de uso `ChamadoAprovacaoUseCaseTests`:
* **Preservação do Legado:** Validação de que a criação, aprovação, reprovação e cancelamento manuais de `AprovacaoChamado` continuem funcionando sem falhas.
* **Controllers Separados:** Confirmação de que requisições legadas passem pelo controller correto (`AdminAprovacaoChamadosController`) e não pelo novo motor.
* **Validação Cruzada:** O interceptador de bloqueio de transição avalia de forma combinada instâncias novas e aprovações legadas pendentes.

---

## 14. Testes de regressão de abertura e atendimento

* **Abertura Limpa:** Chamados sem correspondência com nenhuma regra são criados de forma limpa.
* **Atributos ITSM:** Validação de que a atribuição de natureza ou catálogo na abertura não quebre a rotina de gravação.
* **Sem Falso Bloqueio:** Chamados sem pendências bloqueantes ativas podem ser editados, triados e encerrados sem interceptação.
* **Andamento Livre:** Garantia de que analistas possam adicionar comentários e registrar atendimentos no chamado mesmo com aprovações pendentes.
* **SLA Estável:** Verificação de que o SLA operacional do chamado não seja alterado de forma invisível.

---

## 15. Testes de endpoints e frontend, se aplicável

* **Endpoints Administrativos:** Testes de integração do CRUD de regras no controller `AdminConfiguracoesRegrasAprovacaoController`.
* **Endpoints de Pendências e Decisões:** Validação de rotas de consulta de pendências e endpoints de aprovação no controller `AdminAprovacoesMotorController`.
* **Interface Administrativa (Vue):** Garantir que a listagem (`AdminConfiguracoesRegrasAprovacaoListPage.vue`) e o formulário (`AdminConfiguracoesRegrasAprovacaoFormPage.vue`) exibam e salvem todas as configurações de forma consistente.
* **Tela de Detalhe de Chamados:** Verificação da exibição correta das pendências na barra lateral ou seção dedicada no console do técnico.
* **Build Frontend:** Em caso de alterações na UI, deve ser verificado o build de produção do frontend para evitar falhas de assets ou dependências quebradas.

---

## 16. Teste de roadmap/checklist

A integridade do progresso da entrega é auditada pelo teste `RoadmapSprint4MotorAprovacoesChecklistTests`:
* **Conformidade do Checklist:** O teste valida se o banco de dados de seeds (`SeedData.cs`) possui os registros corretos de itens e se eles estão marcados de acordo com o progresso real.
* **Verificação do Percentual:** O teste calcula de forma autônoma a taxa de conclusão (itens concluídos / total de itens ativos) e confronta com o valor estático no item do roadmap, exigindo igualdade exata (ex: 94%).
* **Próxima Ação:** O teste audita a consistência da propriedade `ProximaAcao` para garantir que o roadmap indique a etapa subsequente correta.

---

## 17. Critério de migration e EF

* **Migrations de Dados:** Modificações de seeds de checklist e roadmaps não alteram o modelo de dados físico, devendo gerar migrations de dados simples (`UpdateData`).
* **Migrations Estruturais:** Alterações em colunas, tabelas ou chaves estrangeiras são estritamente proibidas para tarefas puramente documentais.
* **EF Model Snapshot:** Qualquer alteração no banco deve ser refletida no snapshot do DbContext. O comando `has-pending-model-changes` é a validação soberana para garantir que o modelo em C# e o banco estejam em sincronia completa.

---

## 18. Comandos padrão de validação

Os seguintes comandos devem ser executados em sequência para certificar a qualidade da entrega:

### 18.1 Build da Solução
```powershell
dotnet build SGX.SistemaChamado.sln
```
*Garante que não há nenhum erro de compilação ou sintaxe no backend.*

### 18.2 Execução do Teste de Roadmap
```powershell
dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --filter "FullyQualifiedName~RoadmapSprint4MotorAprovacoesChecklistTests"
```
*Verifica o percentual do checklist da sprint e a próxima ação cadastrada.*

### 18.3 Verificação de Pendências de Migração no EF Core
```powershell
dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure/SGX.SistemaChamado.Infrastructure.csproj --startup-project src/SGX.SistemaChamado.Api/SGX.SistemaChamado.Api.csproj
```
*Confirma que não existem alterações pendentes que necessitem de migrations estruturais.*

### 18.4 Suítes de Testes Unitários e Integração
Para rodar suítes específicas do motor:
* **Regras Administrativas:** `--filter "FullyQualifiedName~ConfiguracaoRegraAprovacao"`
* **Geração Obrigatória:** `--filter "FullyQualifiedName~GerarAprovacaoObrigatoria"`
* **Bloqueio de Chamado:** `--filter "FullyQualifiedName~BloquearMovimentacaoAprovacaoPendente"`
* **Aprovação:** `--filter "FullyQualifiedName~AprovarAprovacaoChamado"`
* **Rejeição:** `--filter "FullyQualifiedName~ReprovarAprovacaoChamado"`
* **Instância:** `--filter "FullyQualifiedName~InstanciaAprovacaoChamado"`
* **Etapas:** `--filter "FullyQualifiedName~EtapaAprovacaoChamado"`
* **Decisão:** `--filter "FullyQualifiedName~DecisaoAprovacaoChamado"`
* **Legado:** `--filter "FullyQualifiedName~ChamadoAprovacao"`
* **Abertura:** `--filter "FullyQualifiedName~AbrirChamado"`
* **Atendimento:** `--filter "FullyQualifiedName~Atendimento"`
* **Assumir Chamado:** `--filter "FullyQualifiedName~AssumirChamado"`
* **Encerramento:** `--filter "FullyQualifiedName~EncerrarChamado"`

---

## 19. Matriz de rastreabilidade

| Área validada | Teste/filtro sugerido | Critério de sucesso |
| :--- | :--- | :--- |
| **Regra administrativa** | `ConfiguracaoRegraAprovacao` | Regras coerentes cadastradas com consistência de vigência |
| **Geração obrigatória** | `GerarAprovacaoObrigatoria` | Geração de instância com base nos critérios ITSM do chamado |
| **Bloqueio** | `BloquearMovimentacaoAprovacaoPendente` | Bloqueio atua apenas no encerramento quando a pendência for bloqueante |
| **Aprovação** | `AprovarAprovacaoChamado` | Registro formal da decisão, transição para Aprovada e liberação do chamado |
| **Rejeição** | `ReprovarAprovacaoChamado` | Registro formal, transição para Reprovada e manutenção do chamado ativo |
| **Instância** | `InstanciaAprovacaoChamado` | Validação de snapshots de regras e imutabilidade pós-criação |
| **Etapa** | `EtapaAprovacaoChamado` | Sequenciamento correto de níveis, ordens e ramos de aprovação |
| **Decisão** | `DecisaoAprovacaoChamado` | Logs de auditoria íntegros com assinatura e timestamps corretos |
| **Legado** | `ChamadoAprovacao` | Aprovacoes legadas manuais coexistem e barram encerramentos |
| **Abertura** | `AbrirChamado` | Chamados sem correspondência seguem jornada operacional normal |
| **Roadmap** | `RoadmapSprint4MotorAprovacoesChecklistTests` | Cálculo do percentual de conclusão e próxima ação coerentes |
| **EF Core** | `has-pending-model-changes` | Nenhuma divergência de mapeamento de banco detectada |

---

## 20. Riscos de teste insuficiente

A negligência nas suítes de validação pode acarretar as seguintes falhas operacionais:
* **Concorrência Incorreta:** Regras específicas perdendo prioridade para genéricas por má ordenação no algoritmo de desempate.
* **Falha de Escopo de Bloqueio:** Bloqueio de encerramento de chamado indevido (ex: um chamado bloqueado por causa de pendência pertencente a outro ticket).
* **Fuga de Governança:** Aprovação manual indevida liberando o chamado sem registrar a respectiva `DecisaoAprovacaoChamado`.
* **Cancelamento Incorreto:** Rejeição de uma etapa cancelar ou encerrar o chamado de forma automatizada a nível de domínio sem intervenção do técnico.
* **Regressão Legada:** A introdução do motor novo corromper a leitura das aprovações legadas de catálogo na interface do usuário.
* **Falso Avanço Multi-nível:** Assumir que as etapas multi-nível avançam sem a chamada do use case, travando fluxos de aprovação na fila.
* **Expectativa Incorreta sobre Grupos:** Permitir a configuração de grupos aprovadores reais em produção antes do desenvolvimento das tabelas relacionais de equipes.
* **Migration Inválida:** Quebra de banco de dados por geração de migration estrutural com perda de dados sob migrações de dados simplificadas.
* **Assets Quebrados no Vue:** Alterar serviços no frontend sem rodar o build de produção do Quasar, ocultando erros de Typescript em tempo de compilação.
* **Estouro de SLAs:** Achar que as travas de aprovação suspendem SLAs de chamados sem ter testado a integração correspondente.

---

## 21. Critério para fechar item da Sprint 4

Ao finalizar qualquer item de roadmap na Sprint 4, garanta o cumprimento da seguinte checklist de entrega:
* [ ] **Documentação:** Criar ou atualizar o respectivo arquivo markdown em `docs/roadmap/` com detalhes funcionais e técnicos.
* [ ] **SeedData:** Atualizar `SeedData.cs` marcando o checklist correspondente como `Concluido = true` e atualizar a porcentagem de entrega da sprint.
* [ ] **Testes de Roadmap:** Atualizar a classe `RoadmapSprint4MotorAprovacoesChecklistTests.cs` com o novo percentual consolidado e quantidade de itens.
* [ ] **Migration de Dados:** Gerar a migration do Entity Framework para aplicar as alterações de seeds.
* [ ] **Compilação Geral:** Executar build completo da solução garantindo que nada foi quebrado.
* [ ] **Homologação:** Executar os testes automatizados filtrados da respectiva área modificada.
* [ ] **Modelo EF Core:** Confirmar que não existem mudanças pendentes no modelo do EF Core.
* [ ] **Relatório Final:** Registrar as alterações feitas, comandos executados e decisões técnicas no log de encerramento.
* [ ] **Limitações:** Deixar claro restrições operacionais e pontos de evolução futura.
* [ ] **Planejamento:** Apontar qual o próximo passo planejado no roadmap.

---

## 22. Conclusão

Os critérios técnicos de testes e homologação para o motor de aprovações ITSM encontram-se plenamente consolidados e documentados. O uso estrito das diretrizes descritas neste manual blinda o ecossistema do **SGX** contra regressões lógicas, garante o funcionamento simultâneo dos fluxos novos e legados e estabelece um processo seguro de evolução contínua para o gerenciamento de serviços de TI corporativo.
