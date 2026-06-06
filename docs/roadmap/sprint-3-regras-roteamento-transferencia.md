# Regras de Roteamento e Transferência - Sprint 3

Este documento detalha as regras operacionais, comportamentos de negócio, matrizes de decisão e impactos sobre os chamados no trâmite entre **Grupos Técnicos**, **Filas de Atendimento** e **Responsáveis Individuais** estabelecidos na Sprint 3.

---

## 1. Objetivo do Documento

Este documento serve como especificação e referência técnica para o comportamento operacional das movimentações de chamados implementadas na Sprint 3 do SGX Sistema de Chamados. Ele orienta o desenvolvimento frontend, a homologação de negócio e a manutenção futura dos fluxos de trâmite de chamados.

---

## 2. Conceitos Fundamentais

*   **Grupo Técnico**: Equipe ou unidade corporativa responsável pelo atendimento (ex: Service Desk, Sistemas). Mapeado pela entidade [GrupoTecnico](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/GrupoTecnico.cs).
*   **Fila de Atendimento**: Segmentação interna ou caixa de entrada de trabalho do grupo técnico. Mapeada por [FilaAtendimento](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/FilaAtendimento.cs).
*   **Responsável Individual**: Técnico alocado para a resolução do chamado, mapeado por `ResponsavelId` em [Chamado](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/Chamado.cs).
*   **Direcionamento**: Operação de alocação inicial de um chamado para um grupo técnico (e opcionalmente fila).
*   **Transferência**: Ação de mover um chamado que já possui grupo técnico para outra equipe (novo grupo técnico).
*   **Assumir Fila**: Ação na qual um técnico pertencente a um grupo técnico assume a autoria de um chamado posicionado na fila desse grupo.
*   **Atribuição Técnica**: Designação (normalmente feita por um coordenador ou administrador) de um chamado para um técnico específico do grupo.
*   **Roteamento Manual/Assistido vs Roteamento Automático**:
    *   *Roteamento Manual/Assistido*: O usuário escolhe o grupo e fila de destino através da UI. O sistema valida regras de integridade e permissão.
    *   *Roteamento Automático*: Regras de inteligência do sistema que decidem o grupo/fila sem intervenção humana (não implementado nesta Sprint).

---

## 3. O que é Roteamento nesta Sprint

Na Sprint 3, todo o roteamento de chamados é **manual e assistido**.
*   Não existem regras automáticas de roteamento (ex: rotear por categoria do chamado, prioridade, SLA, fuso horário ou balanceamento automático de carga).
*   O backend atua estritamente validando a consistência das ações manuais (como impedir que um chamado seja direcionado a uma fila que pertence a outro grupo ou exigir que o técnico seja membro ativo do grupo). O sistema **não escolhe o grupo ou fila sozinho**.

---

## 4. Direcionamento Inicial para Grupo

O direcionamento aloca um chamado sem grupo técnico para uma equipe de atendimento corporativo.

*   **Quando usar**: Quando o chamado é criado ou classificado inicialmente e precisa ser associado a um grupo técnico responsável.
*   **Pré-condições**:
    *   O chamado deve existir no sistema.
    *   O chamado **não deve possuir** nenhum grupo técnico associado (campo `GrupoTecnicoId` deve ser nulo). Se já possuir grupo técnico, a ação falha e o sistema exige o uso da transferência.
    *   O grupo técnico de destino deve estar cadastrado e ativo.
*   **Fila Opcional**: O preenchimento da fila de atendimento é opcional. Se informada, a fila deve existir, estar ativa e pertencer obrigatoriamente ao grupo técnico selecionado.
*   **Efeito no Responsável**: O campo `ResponsavelId` é **preservado**. Se o chamado já possuir responsável individual, o vínculo é mantido.
*   **Histórico Gerado**: Grava os históricos de definição do grupo e de fila (se selecionada).

---

## 5. Ajuste de Fila dentro do mesmo Grupo

Representa a reclassificação ou movimentação operacional do chamado entre filas da mesma equipe.

*   **Quando ocorre**: Quando a equipe responsável decide mover o chamado internamente (ex: mover da fila de "Triagem" para a fila de "Nível 2" dentro do mesmo grupo).
*   **Funcionamento**: Realizado através do mesmo use case de direcionamento ([DirecionarChamadoGrupoTecnicoAdminUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/DirecionarChamadoGrupoTecnicoAdminUseCase.cs)) enviando o mesmo `GrupoTecnicoId` atual, mas um novo `FilaAtendimentoId`.
*   **Preservação**: O grupo técnico e o técnico responsável atual permanecem inalterados.
*   **Tipos de histórico**: Conforme a mudança de fila, gera registros de definição de fila, transferência de fila ou remoção de fila.

---

## 6. Transferência entre Grupos Técnicos

A transferência move a custódia do chamado de um grupo técnico de origem para um grupo técnico de destino diferente.

*   **Quando usar**: Quando o grupo técnico atual constata que a resolução do problema pertence a outra especialidade operacional.
*   **Exigência de grupo anterior**: O chamado **deve possuir** um grupo técnico de origem associado. Se o grupo atual for nulo, a operação é rejeitada (deve ser usado direcionamento inicial).
*   **Grupo destino ativo**: O grupo selecionado para destino deve existir e estar ativo.
*   **Fila destino opcional**: A fila de destino é opcional. Se fornecida, deve pertencer ao grupo destino e estar ativa.
*   **Limpeza do Responsável**: O `ResponsavelId` é **obrigatoriamente limpo** (definido como `null`) para que o chamado retorne à fila geral da nova equipe sem um dono pré-atribuído.
*   **Limpeza/Redefinição de fila**: Se nenhuma fila de destino for informada, o campo `FilaAtendimentoId` é limpo. Caso contrário, assume a nova fila.

---

## 7. Assumir Chamado da Fila

Ação pela qual o próprio técnico logado puxa para si a responsabilidade de resolução do chamado posicionado na fila de seu grupo.

*   **Quando usar**: Quando o atendente está livre para realizar atendimentos e decide processar um chamado da fila de sua equipe.
*   **Requisitos de Chamado**:
    *   O chamado deve estar associado a um grupo técnico ativo.
    *   O chamado deve estar associado a uma fila de atendimento ativa.
    *   O chamado deve estar **sem responsável individual** (`ResponsavelId == null`). Se já possuir responsável, a operação é rejeitada.
*   **Membro Ativo do Grupo**: O usuário executor deve ser o próprio usuário autenticado e deve possuir um vínculo ativo (`MembroGrupoTecnico.Ativo == true`) com o grupo associado ao chamado.
*   **Efeito nos Campos**: O `ResponsavelId` é atualizado para o ID do usuário executor. O grupo técnico e a fila de atendimento permanecem intactos.

---

## 8. Atribuição a Técnico Específico

A atribuição designa manualmente um chamado a um técnico específico.

*   **Atribuição Legada (Sem Grupo)**: Se o chamado não possui grupo técnico vinculado, a atribuição individual funciona de forma aberta, permitindo selecionar qualquer usuário com perfil de atendimento do sistema.
*   **Atribuição com Grupo**: Se o chamado possui grupo técnico vinculado, o técnico selecionado como responsável **deve ser um membro ativo** daquele grupo técnico.
*   **Preservação de contexto**: O grupo e a fila do chamado permanecem os mesmos ao atualizar o responsável individual.
*   **Diferença de Assumir Fila**: A atribuição é realizada por terceiros (ex: administradores ou coordenadores) para designar trabalho a um técnico qualquer da equipe, enquanto o "Assumir Fila" é uma autoatribuição do técnico logado.

---

## 9. Matriz de Decisão

| Necessidade Operacional | Ação a Executar | Endpoint Sugerido |
| :--- | :--- | :--- |
| Chamado recém-criado ou sem equipe precisa entrar em um grupo | **Direcionamento** | `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico` |
| Chamado já alocado na equipe A precisa ser enviado para a equipe B | **Transferência** | `POST /api/admin/chamados/{id}/transferir-grupo-tecnico` |
| Chamado posicionado na fila precisa ser assumido pelo técnico logado | **Assumir Fila** | `POST /api/admin/chamados/{id}/assumir-fila` |
| Administrador designa chamado para um técnico específico da equipe | **Atribuição** | `POST /api/admin/chamados/{id}/atribuir` |
| Técnico deseja apenas mudar a fila operacional do chamado dentro do mesmo grupo | **Ajuste de Fila (Direcionar)** | `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico` |
| Chamado sem grupo precisa ser alocado diretamente a um técnico | **Atribuição Legada** | `POST /api/admin/chamados/{id}/atribuir` |

---

## 10. Matriz de Efeitos nos Campos

A tabela abaixo descreve o comportamento de alteração nos campos do chamado para cada operação:

| Operação | Altera `GrupoTecnicoId` | Altera `FilaAtendimentoId` | Altera `ResponsavelId` | Gera Histórico | Observação |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Direcionamento** | Sim, define o grupo inicial | Sim, se fornecida | Não | Sim | Rejeita se o chamado já tiver outro grupo técnico. |
| **Ajuste de Fila** | Não, mantém o atual | Sim, altera ou limpa | Não | Sim | Executado via use case de direcionamento. |
| **Transferência** | Sim, define o novo grupo | Sim, limpa ou define nova | Sim, define como `null` | Sim | Rejeita se o chamado não possuir grupo anterior. |
| **Assumir Fila** | Não | Não | Sim, seta atendente logado | Sim | Exige atendente logado membro ativo do grupo. |
| **Atribuição** | Não | Não | Sim, seta técnico selecionado | Sim | Valida se o técnico é membro ativo do grupo do chamado. |
| **Cadastro de Grupo** | Não | Não | Não | Não | Operação de cadastro administrativo. |
| **Gestão de Membros** | Não | Não | Não | Não | Altera apenas vinculações de técnicos, não mexe no chamado. |
| **Listagem/Filtros** | Não | Não | Não | Não | Apenas consulta e visualização dos dados. |

---

## 11. Matriz de Histórico

Gravações realizadas em `HistoricoChamado` e seus respectivos textos gerados na auditoria:

| Operação / Trâmite | Tipo de Histórico (`TipoHistoricoChamado`) | Informação Mínima no Texto | Origem / Destino no Histórico |
| :--- | :--- | :--- | :--- |
| **Definir Grupo** | `GrupoTecnicoDefinido` | Nome do grupo técnico definido. | Apenas Destino (Grupo definido). |
| **Definir Fila** | `FilaAtendimentoDefinida` | Nome da fila definida. | Apenas Destino (Fila definida). |
| **Remover Fila** | `FilaAtendimentoRemovida` | Nome da fila que foi desvinculada. | Apenas Origem (Fila removida). |
| **Trocar Fila** | `FilaAtendimentoTransferida` | Nomes da fila de origem e de destino. | Origem e Destino informados no texto. |
| **Transferir Grupo** | `GrupoTecnicoTransferido` | Nomes do grupo de origem e grupo de destino. | Origem e Destino informados no texto. |
| **Limpeza de Técnico** | `ResponsavelRemovidoPorTransferenciaGrupo` | Nome do técnico que perdeu atribuição. | Técnico de origem no texto do histórico. |
| **Assumir Fila** | `ChamadoAssumidoDaFila` | Nome do técnico e da fila de onde assumiu. | Técnico destino e fila no texto do histórico. |
| **Atribuição/Reatribuição** | `ResponsavelAlterado` | Nome do técnico de destino (e origem se houver).| Técnico de origem e destino no texto do histórico. |

---

## 12. Permissões de Acesso

O SGX implementa validações de segurança em duas etapas para operações críticas de trâmite:

1.  **Direcionar Chamado**: Permitido a `Administrador` ou `Atendente`. Validado no controller via policy `Policies.AdminOuAtendente` e na camada de aplicação.
2.  **Transferir Chamado**: Permitido a `Administrador` ou `Atendente`. Validado no controller via policy `Policies.AdminOuAtendente` e na camada de aplicação.
3.  **Assumir Fila**: Permitido a `Administrador` ou `Atendente`. A aplicação exige adicionalmente que o usuário autenticado seja um membro ativo do grupo associado ao chamado.
4.  **Atribuir Técnico**: Restrito a perfil `Administrador` (Sprint 3) no controller através da policy `PermissionPolicies.ChamadosAtribuir`. A aplicação valida se o técnico destino pertence ao grupo do chamado.
5.  **Diferença UI vs Use Case**: O frontend apenas oculta/desabilita botões visualmente de acordo com as claims do token. A validação de negócio e consistência de segurança real é reexecutada de forma blindada no backend pelos Use Cases.

---

## 13. Endpoints Envolvidos

*   `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico`
    *   Usa use case [DirecionarChamadoGrupoTecnicoAdminUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/DirecionarChamadoGrupoTecnicoAdminUseCase.cs).
*   `POST /api/admin/chamados/{id}/transferir-grupo-tecnico`
    *   Usa use case [TransferirGrupoTecnicoChamadoUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/TransferirGrupoTecnicoChamadoUseCase.cs).
*   `POST /api/admin/chamados/{id}/assumir-fila`
    *   Usa use case [AssumirChamadoFilaAdminUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoFilaAdminUseCase.cs).
*   `POST /api/admin/chamados/{id}/atribuir`
    *   Usa use case [AtribuirChamadoUseCase](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Application/UseCases/Admin/AtribuirChamadoUseCase.cs).
*   **Endpoints Auxiliares**:
    *   `GET /api/admin/grupos-tecnicos` (listagem para preencher combos).
    *   `GET /api/admin/grupos-tecnicos/{grupoId}/filas` (listagem de filas válidas do grupo).

---

## 14. Frontend Envolvido

A interface administrativa (`src/SGX.SistemaChamado.Web`) foi ajustada nas seguintes áreas para comportar o roteamento:

*   **Detalhe do Chamado**: Exibe as tags identificadoras do grupo técnico e fila no card superior.
*   **Botão "Assumir Chamado"**: Fica visível apenas para usuários com perfil de atendimento que pertençam como membro ativo ao grupo técnico do chamado atual (quando o chamado não tem responsável).
*   **Botão "Transferir Grupo"**: Exibe modal interativo para seleção do novo grupo e listagem dinâmica de suas filas correspondentes.
*   **Filtros de Listagem**: Grid principal exibe as colunas e filtros de grupo e fila no cabeçalho.
*   **Telas Administrativas**: Telas exclusivas de Grupos Técnicos, Membros e Filas sob o menu `/admin/grupos-tecnicos`.
*   *Nota*: Listagens e filtros são operações de visualização, não disparando qualquer lógica operacional de alteração no chamado.

---

## 15. Regras Negativas Importantes

Para evitar bugs e inconsistências no trâmite de chamados, as seguintes restrições foram codificadas:

1.  **Direcionar não transfere**: Não é permitido usar o direcionamento para trocar o chamado de grupo técnico. Se o chamado possui grupo, deve-se usar explicitamente a transferência.
2.  **Transferência não direciona**: A transferência rejeita chamados que não possuem grupo técnico prévio alocado.
3.  **Assumir não delega**: O técnico não pode "assumir" o chamado em nome de outro usuário (o ID do técnico assumindo deve bater com as credenciais logadas).
4.  **Atribuição não transfere**: Atribuir um técnico não altera o grupo técnico nem a fila do chamado (apenas designa o responsável individual no grupo atual).
5.  **Gestão de membros não altera chamados**: Desativar ou ativar membros em um grupo não modifica os chamados atualmente atribuídos a eles ou em andamento.
6.  **Listagem não altera dados**: Consultar chamados por filtros de grupo ou fila não dispara gatilhos de alteração de campos.
7.  **Frontend não altera campos criticamente**: Nenhuma regra de direcionamento, limpeza de responsável ou validação de fila é executada no cliente. O frontend limita-se a exibir os campos e enviar requests; o backend revalida tudo.

---

## 16. Limitações Atuais

*   **Sem Automatização**: O roteamento depende integralmente da seleção manual do atendente ou triador. Não há suporte a atribuição round-robin ou fila automática por categoria.
*   **Fila sem CRUD Completo**: O cadastro de filas é feito por scripts ou migrations; o frontend possui apenas exibição e seeds das filas associadas aos grupos técnicos.
*   **SLA por Grupo (OLA)**: O cálculo de SLA de atendimento permanece unificado por chamado. Não há suporte a SLAs ou cronômetros específicos configurados para cada grupo técnico individual.
*   **Auditoria Textual**: A auditoria das movimentações ocorre por concatenação de texto na descrição do histórico (sem relacionamento referencial no banco para fins de estatística).

---

## 17. Riscos e Cuidados Futuros

1.  **Evitar Duplicidade de Operações**: O desenvolvedor não deve mesclar os use cases de direcionamento e transferência. Eles representam momentos de negócio distintos no ciclo de vida do chamado.
2.  **Consistência Grupo/Fila**: Toda alteração manual futura em fila ou grupo deve passar pela validação backend para garantir que `Chamado.FilaAtendimento.GrupoTecnicoId == Chamado.GrupoTecnicoId`.
3.  **Perda de Responsável**: Lembrar que a transferência de grupo limpa o responsável individual. Automações futuras que transferirem chamados devem prever essa limpeza de forma a alertar os novos times da fila.
4.  **Permissões por Grupo**: Futuras evoluções de segurança podem exigir que atendentes leiam apenas chamados do próprio grupo técnico. Isso exigirá filtros nas queries SQL principais.

---

## 18. Próximas Evoluções Recomendadas

1.  **Roteamento Automático**: Regras configuráveis no banco de dados para direcionar o chamado para grupos técnicos com base na categoria da solicitação (ex: chamados de "Redes" rotearem direto para Infraestrutura).
2.  **Gestão de Filas na UI**: Tela de CRUD completo de filas de atendimento para que gerentes cadastrem novas filas sem precisar rodar migrations de banco.
3.  **OLAs (Operational Level Agreements)**: SLA interno para medir o tempo de atendimento do grupo técnico antes de transferir ou resolver o chamado.
4.  **Auditoria Estruturada**: Inclusão de chaves estrangeiras (`GrupoTecnicoOrigemId`, `GrupoTecnicoDestinoId`) na tabela de histórico de chamados para permitir relatórios de trâmite de chamados precisos.
