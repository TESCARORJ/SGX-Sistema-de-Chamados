# Impacto no Fluxo Atual de Chamados - Sprint 3

Este documento detalha como a introdução de **Grupos Técnicos** e **Filas de Atendimento** na Sprint 3 impacta os fluxos operacionais, APIs, frontend e regras existentes do SGX Sistema de Chamados, bem como os limites das mudanças.

---

## 1. Objetivo do Documento

O objetivo deste documento é mapear os impactos funcionais e técnicos causados pelas implementações da Sprint 3 sobre o ciclo de vida e operações dos chamados legados e novos no SGX. Ele registra a coexistência harmoniosa entre o modelo tradicional (centrado em responsável individual) e o novo modelo (estruturado por grupos e filas).

---

## 2. Visão Antes da Sprint 3

Antes da Sprint 3, o fluxo de atendimento era estritamente individual:
*   **Responsabilidade**: Toda a designação de atendimento ocorria no nível do técnico individual através de `ResponsavelId` na entidade [Chamado](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/Chamado.cs).
*   **Fila Operacional Implícita**: Um chamado era considerado "em fila" de forma genérica quando estava aberto e sem técnico associado (`ResponsavelId == null`).
*   **Sem Grupos ou Equipes**: Não havia suporte para equipes técnicas (ex: Infraestrutura, Sistemas), impedindo a atribuição a um setor em vez de um indivíduo.
*   **Sem Filas Físicas/Explícitas**: Não existia o conceito de "fila de atendimento" formal no banco de dados.

---

## 3. Visão Após a Sprint 3

Com as modificações da Sprint 3:
*   **Estrutura de Equipe**: O chamado pode estar formalmente sob a custódia de um **Grupo Técnico** e posicionado em uma **Fila de Atendimento** específica.
*   **Responsabilidade Convivente**: O técnico responsável (`ResponsavelId`) continua existindo de forma independente de grupo/fila. O chamado pode estar em grupo/fila sem técnico (aguardando atendimento) ou com técnico atribuído (em andamento).
*   **Opcionalidade**: O preenchimento de grupo técnico e fila no chamado é **totalmente opcional**, o que significa que os fluxos legados e chamados pré-existentes permanecem 100% funcionais e íntegros.

---

## 4. Impacto na Abertura de Chamado

A abertura de chamados, seja pelo portal do solicitante ou de forma administrativa:
*   **Campos de Equipe**: Não exige o preenchimento de `GrupoTecnicoId` nem de `FilaAtendimentoId`. Na criação, ambos permanecem como `null` por padrão.
*   **Metadados Preservados**: Campos existentes (como natureza do chamado, impacto, urgência, categoria, descrição e solicitante) continuam operando sem qualquer alteração.
*   **SLA**: O cronômetro de SLA inicial do chamado é iniciado normalmente na abertura, sem alteração de regras de prazo.
*   **Auditoria**: A abertura continua registrando o histórico de criação conforme o padrão legado.

---

## 5. Impacto na Listagem Administrativa

A grid e filtros de chamados administrativamente:
*   **Exibição Visual**: A listagem agora exibe colunas com os nomes do **Grupo Técnico** e da **Fila de Atendimento**.
*   **Fallback Amigável**: Registros legados ou novos sem grupo/fila exibem um texto de fallback amigável (ex: *Sem grupo técnico* e *Sem fila*).
*   **Filtros Avançados**: Foram adicionados filtros para permitir aos operadores segmentar a lista por grupo técnico e fila de atendimento específica.
*   **Preservação**: A paginação, ordenações e filtros pré-existentes (por responsável, status, solicitante, etc.) funcionam de forma idêntica.
*   **Operacional**: A listagem administrativa limita-se a exibir e filtrar registros, sem disparar ações automáticas ou mutações sobre os chamados.

---

## 6. Impacto no Detalhe do Chamado

A tela de visualização de detalhes do chamado:
*   **Tags no Topo**: Cartões visuais exibem o grupo técnico e a fila aos quais o chamado pertence.
*   **Responsável**: Exibe o técnico responsável individual, se já atribuído.
*   **Null-Safety**: O carregamento da tela é imune a valores nulos (chamados sem grupo ou fila continuam renderizando perfeitamente).
*   **Linha do Tempo**: A linha do tempo integrada de histórico exibe os novos eventos de movimentação de grupo/fila de forma formatada.
*   **Ações Operacionais**: As ações de direcionar para grupo, assumir chamado da fila e transferir grupo ficam centralizadas nesta tela.

---

## 7. Impacto no Fluxo de Assumir Chamado Legado

O caso de uso de assumir chamado existente ([AssumirChamadoUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoUseCase.cs)):
*   **Coexistência**: Continua ativo e atende chamados sem grupo/fila. O atendente simplesmente se define como responsável do chamado.
*   **Sem Bloqueio de Grupo**: O fluxo legado não exige que o usuário seja membro ativo de algum grupo para assumir chamados que não possuem grupo definido.
*   **Preservação**: Se o chamado já possuir grupo/fila (via classificação anterior), o caso de uso legado os mantém inalterados, apenas preenchendo o responsável.
*   **Diferença**: O fluxo legado baseia-se apenas no perfil técnico geral do atendente, enquanto o novo fluxo `Assumir da Fila` exige pertencimento e vínculo ativo de equipe.

---

## 8. Impacto na Atribuição de Chamado

A designação de chamado para outro técnico realizada via [AtribuirChamadoUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/AtribuirChamadoUseCase.cs):
*   **Atribuição sem Grupo**: Se o chamado não possui grupo, a regra permanece a mesma do legado (permite atribuir a qualquer técnico de atendimento).
*   **Atribuição com Grupo**: Se o chamado possui grupo técnico, o sistema valida e bloqueia a atribuição se o técnico de destino não for um membro ativo do grupo técnico associado ao chamado.
*   **Preservação**: O grupo técnico e a fila do chamado não são modificados.
*   **Histórico**: Registra a troca com `TipoHistoricoChamado.ResponsavelAlterado` identificando o técnico de origem (se houver) e o de destino.

---

## 9. Impacto no Novo Fluxo de Grupo/Fila

A Sprint 3 introduziu fluxos de transição específicos:
*   **Direcionamento Inicial**: Associa um chamado sem equipe a um grupo e fila pela primeira vez. Mantém o responsável atual.
*   **Ajuste de Fila**: Altera a fila mantendo o chamado na mesma equipe.
*   **Transferência**: Move o chamado para outra equipe (novo grupo técnico). Limpa o responsável individual.
*   **Assumir Fila**: O atendente da equipe se autoatribui o chamado da fila, definindo o `ResponsavelId`.

---

## 10. Impacto no `ResponsavelId`

A tabela abaixo descreve como o responsável individual do chamado é afetado por cada transação:

| Operação | Efeito no `ResponsavelId` | Justificativa |
| :--- | :--- | :--- |
| **Abertura de Chamado** | Mantido como `null` | Chamados iniciam sem técnico individual atribuído por padrão. |
| **Direcionamento** | **Preserva** o valor atual | O chamado apenas recebe a definição de grupo, sem impactar quem está trabalhando nele. |
| **Ajuste de Fila** | **Preserva** o valor atual | Mudança de fila interna na equipe não desfaz a atribuição do técnico responsável. |
| **Assumir Fila** | **Preenche** com o usuário logado | O técnico logado assume a autoria e responsabilidade individual do atendimento. |
| **Atribuição** | **Altera** para o técnico destino | Designação direta do operador de triagem para um técnico da equipe. |
| **Reatribuição** | **Altera** para o novo técnico | Substituição de técnico responsável do chamado por outro. |
| **Transferência** | **Limpa** (seta para `null`) | Ao mudar de grupo técnico, o técnico do grupo anterior não pode continuar como responsável. |
| **Cadastro de Grupos** | Sem impacto | Operação meramente cadastral que não toca em chamados. |
| **Gestão de Membros** | Sem impacto | Ativar/inativar membros não altera chamados atribuídos retroativamente. |
| **Listagem/Detalhe** | **Preserva** (apenas leitura) | Apenas exibe o estado atual das tabelas do banco de dados. |

---

## 11. Impacto no SLA

*   **Regra de Abertura**: O início do SLA não sofreu alterações. A criação do chamado inicia o SLA base do atendimento.
*   **Preservação Operacional**: Operações de direcionamento, assumir da fila, transferência e atribuição individual não alteram, pausam ou zeram os prazos de SLA existentes.
*   **Regra de Primeira Resposta**: Se o chamado já possuía uma regra de SLA de primeira resposta ativada (ex: ao interagir ou assumir o chamado parar o SLA de primeira resposta), o comportamento é preservado. O fato de direcionar para um grupo/fila não é considerado um "aceite de atendimento" individual, logo não interfere no SLA de primeira resposta.
*   **Limitação (OLA)**: A Sprint 3 **não** implementa SLAs de grupo técnico (Operational Level Agreements). O cálculo do SLA de atendimento permanece no nível global do chamado.

---

## 12. Impacto no Histórico e Auditoria

*   **Novos Tipos de Histórico**: Foram acopladas movimentações específicas ao enum `TipoHistoricoChamado` (como `GrupoTecnicoDefinido`, `GrupoTecnicoTransferido`, `ChamadoAssumidoDaFila`, etc.).
*   **Linha do Tempo**: O visual do histórico no frontend reconhece e exibe as novas movimentações com ícones e cores adequados.
*   **Histórico Antigo**: Eventos gravados antes da Sprint 3 continuam legíveis, pois os novos enums foram adicionados ao final da lista, preservando os códigos numéricos anteriores.
*   **Enum Travado**: Testes de integridade travam o enum de histórico contra remoções ou reordenações acidentais.
*   **Textual**: Os detalhes de origem e destino da transferência são descritos de forma puramente textual no corpo do histórico.

---

## 13. Impacto nas Permissões

*   **Administrador e Atendente**: Possuem acesso operacional completo para realizar direcionamento, transferência e visualização geral de grupos/filas.
*   **Guarda em Assumir Fila**: A permissão do use case exige validação em runtime (usuário logado deve ser membro ativo do grupo do chamado).
*   **Escrita de Cadastros**: Restrito apenas a administradores (criar e editar grupos, inativar membros).
*   **Solicitante**: Possui bloqueio total a qualquer operação de trâmite de grupo ou fila (solicitantes apenas interagem com seus chamados no portal, sem visão técnica).
*   **Fonte da Verdade**: O backend valida as permissões e claims rigidamente nos Use Cases; a UI apenas oculta controles.

---

## 14. Impacto no Frontend

*   **Visualizadores**: Cards de exibição e grid de chamados agora expõem de forma amigável as tags de grupo e fila.
*   **Controles de Ação**: Adicionados botões no detalhe do chamado para permitir transferir (abrir diálogo) e assumir da fila (se o técnico for elegível).
*   **Cadastro**: Adicionadas telas na área `/admin` para gestão de grupos técnicos, membros e listagem de filas por grupo.
*   *Restrição*: Nenhuma ação mutável foi adicionada à grid de listagem de chamados, mantendo as atualizações restritas à tela de detalhe.

---

## 15. Impacto na API

*   **Endpoints Novos**: Publicados novos endpoints administrativos específicos de grupos, membros, filas e trâmite do chamado (POST `/direcionar-grupo-tecnico`, `/transferir-grupo-tecnico`, `/assumir-fila`).
*   **Endpoints Legados**: Endpoints de criação de chamado, assumir chamado legado e atribuir individual continuam funcionais.
*   **Payload de Criação**: Os payloads de abertura continuam sem exigir grupo/fila, aceitando dados sem modificação estrutural.

---

## 16. Compatibilidade com Chamados Existentes

*   **Tabelas**: Colunas `grupo_tecnico_id` e `fila_atendimento_id` criadas como opcionais (nullable) nas migrations.
*   **Null Safety**: O backend implementa validações robustas contra nulos na busca, listagem e auditoria.
*   **Visual**: Quando o chamado não possui grupo ou fila, o frontend renderiza placeholders como "Sem grupo técnico" ou "Sem fila" em vez de quebrar a página.
*   **Regressão**: Testes de regressão garantem que as ações de negócio legadas continuam funcionando com dados parciais.

---

## 17. O Que Não Mudou

Para fins de clareza do escopo da Sprint 3, fica registrado o que **não foi modificado**:
1.  A abertura de chamados pelo portal continua sem requerer grupo ou fila.
2.  As regras de cálculo de prazos de SLA globais do chamado permanecem inalteradas.
3.  Os dashboards administrativos não exibem estatísticas de grupos/filas nesta sprint.
4.  Os relatórios administrativos legados não sofreram alterações.
5.  A fila de atendimento não possui CRUD de manutenção via interface.
6.  Não há roteamento de chamados automático.
7.  Não há distribuição automática de carga de chamados.

---

## 18. Riscos e Cuidados Operacionais

*   **Limpeza do Responsável**: A transferência de grupo técnico zera o responsável do chamado. Os operadores devem ser instruídos de que a transferência exige que o novo time assuma o chamado novamente.
*   **Triagem Manual**: A ausência de roteamento automático exige que operadores de Service Desk façam a triagem e direcionamento manual dos chamados sem grupo.
*   **Uso de CLI-Home**: Lembrar que diretórios de compilação como `.dotnet-cli-home` são gerados na execução de testes locais e não devem ser versionados no Git.

---

## 19. Evidências de Validação

Os seguintes conjuntos de testes automáticos validaram a integridade do fluxo de chamados:

*   `GruposTecnicosAdminUseCaseTests.cs`: Cadastro e manutenção de grupos.
*   `MembrosGruposTecnicosAdminUseCaseTests.cs`: Inserção e regras de ativação de membros.
*   `DirecionarChamadoGrupoTecnicoAdminUseCaseTests.cs`: Direcionamento inicial e validações de fila do grupo.
*   `AssumirChamadoFilaAdminUseCaseTests.cs`: Preenchimento de responsável e regras de membro ativo.
*   `TransferirGrupoTecnicoChamadoUseCaseTests.cs`: Troca de grupo, limpeza de responsável e reclassificação de fila.
*   `AssumirChamadoUseCaseTests.cs` e `AtribuirChamadoUseCaseTests.cs`: Regressão dos fluxos de atribuição individual legados.
*   `AuditoriaModulosCriticosTests.cs` e `LinhaTempoChamadoUseCasesTests.cs`: Registro de históricos de auditoria textuais coerentes na linha do tempo.

---

## 20. Próximas Evoluções Recomendadas

Como desdobramento da implantação da Sprint 3, sugere-se planejar:
1.  **CRUD de Filas**: Interface administrativa para inclusão de novas filas operacionais.
2.  **Roteamento Automático**: Roteamento inicial baseado em categoria do chamado.
3.  **OLAs de SLA**: Controle de SLA por etapa e por equipe.
4.  **Dashboards de Equipes**: Indicadores de volumetria e tempo médio de atendimento por grupo técnico.
5.  **Auditoria Estruturada**: Relacionamento de chaves nas tabelas de histórico para obter dados estatísticos de trâmite precisos.
