# Critérios de Testes Técnicos - Sprint 3

Este documento consolida os critérios técnicos objetivos de teste e as diretrizes de validação para a Sprint 3 (Grupos Técnicos, Filas e Atribuição) do SGX Sistema de Chamados. Ele define os requisitos técnicos mínimos de aceitação por camada e a cobertura de testes necessária para garantir que as novas funcionalidades coexistam harmonicamente com os fluxos legados e chamados pré-existentes.

---

## 1. Objetivo do Documento

O objetivo deste documento é unificar os critérios técnicos de validação de banco de dados, regras de domínio, casos de uso, APIs, interface do usuário (frontend), regras de permissão, registros de auditoria e testes de não regressão que fundamentam a homologação da Sprint 3. Ele serve como roteiro formal para auditoria técnica e controle de qualidade do SGX.

---

## 2. Escopo Técnico Validado

As implementações da Sprint 3 compreendem validações estruturadas nas seguintes camadas da arquitetura DDD:

*   **Domínio**: Novas entidades e métodos de comportamento seguro.
*   **Banco de Dados**: Migrations incrementais do EF Core, chaves estrangeiras restritivas e índices de performance.
*   **Aplicação / Use Cases**: Casos de uso de administração de grupos, gerenciamento de membros e movimentação de chamados.
*   **API**: Endpoints protegidos, payloads tipados e autenticação baseada em claims de perfil.
*   **Frontend**: Telas administrativas em `/admin`, controles de ação null-safe na visualização do chamado e tratamento local seguro de estado.
*   **Permissões**: Runtime guards e controle estrito de acessos baseado em perfis (Administrador, Atendente, Solicitante).
*   **Auditoria / Histórico**: Rastreabilidade cronológica com novas ações de trâmite injetadas no histórico sem desorganização de chaves pré-existentes.
*   **Regressão**: Validação de compatibilidade retrógrada com dados legados e fluxos tradicionais de abertura e atribuição direta.

---

## 3. Critérios de Banco de Dados

Para garantir a integridade estrutural e a performance do PostgreSQL:

1.  **Tabelas de Grupos e Filas**:
    *   `grupos_tecnicos`: tabela para cadastro de equipes.
    *   `membros_grupos_tecnicos`: tabela associativa para membros da equipe técnica.
    *   `filas_atendimento`: tabela contendo filas de direcionamento atreladas a grupos.
2.  **Colunas Opcionais (Nullable)**:
    *   As colunas `grupo_tecnico_id` e `fila_atendimento_id` na tabela `chamados` devem ser definidas como opcionais (nullable) para acomodar dados legados e a opcionalidade do novo fluxo.
3.  **Integridade Referencial (Foreign Keys)**:
    *   Devem possuir comportamento restrito de deleção (`DeleteBehavior.Restrict`). A exclusão de um grupo técnico ou membro não pode desencadear deleções em cascata que afetem chamados existentes.
4.  **Índices de Banco de Dados**:
    *   Índices específicos criados nas chaves estrangeiras: `ix_chamados_grupo_tecnico_id` e `ix_chamados_fila_atendimento_id`.
    *   Índice exclusivo composto para integridade relacional de membros: `ux_membros_grupos_tecnicos_grupo_usuario` (nas colunas `grupo_tecnico_id` e `usuario_id`).
5.  **Seeds Mínimos**:
    *   Seeding estático em `SeedData.cs` contendo grupos técnicos (ex: *Service Desk*, *Infraestrutura*, *Sistemas*) e suas filas correspondentes para viabilizar testes.
6.  **Entity Framework Sem Alterações Pendentes**:
    *   A execução de `dotnet ef migrations has-pending-model-changes` deve resultar em sucesso absoluto, indicando que as classes de mapeamento e o modelo do DbContext estão em perfeita sincronia com as migrations.
7.  **Isolamento de Migrations**:
    *   Migrations estruturais de banco de dados devem ocorrer em etapas separadas das migrations que alteram apenas os dados do checklist do roadmap no banco de dados.

---

## 4. Critérios de Domínio

As regras e comportamentos expressos no coração do sistema em `src/SGX.SistemaChamado.Domain` devem satisfazer:

1.  **Estados Ativo/Inativo**:
    *   As entidades [GrupoTecnico](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/GrupoTecnico.cs) e [MembroGrupoTecnico](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/MembroGrupoTecnico.cs) devem suportar ativação e inativação explícita.
2.  **Hierarquia de Fila**:
    *   [FilaAtendimento](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/FilaAtendimento.cs) deve obrigatoriamente estar associada a um grupo técnico válido.
3.  **Null-Safety do Chamado**:
    *   A classe [Chamado](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Entities/Chamado.cs) deve permitir atribuições nulas para `GrupoTecnicoId` e `FilaAtendimentoId` sem estourar exceções em tempo de execução.
4.  **Preservação e Limpeza de `ResponsavelId`**:
    *   O comportamento do `ResponsavelId` deve aderir estritamente à matriz operacional de responsabilidade.
5.  **Compatibilidade de Carga Legada**:
    *   Nenhum método construtor ou método de validação do domínio pode forçar a existência de um grupo técnico para chamados criados antes da implantação da Sprint 3.

---

## 5. Critérios de Use Cases

As operações encapsuladas em `src/SGX.SistemaChamado.Application` devem cumprir:

*   **Cadastro de Grupos Técnicos**:
    *   Validação de nomes únicos para grupos ativos.
    *   Regras para inativação que impedem a inativação de grupos com chamados abertos em andamento (se configurado).
*   **Gestão de Membros de Grupos**:
    *   Verificação de perfil de usuário (apenas usuários com perfil de atendimento podem ser membros).
    *   Bloqueio contra a duplicidade de vínculo ativo para o mesmo usuário no mesmo grupo técnico.
*   **Direcionamento de Chamado**:
    *   Permitir direcionar chamados sem grupo técnico para uma fila de atendimento válida.
    *   Garantir a consistência de que a fila informada pertence ao grupo técnico escolhido.
*   **Assumir Fila de Atendimento**:
    *   Validar que o usuário que está assumindo o chamado possui vínculo de membro ativo no grupo técnico ao qual o chamado está direcionado.
    *   Lançar erro se o chamado já possuir responsável individual ativo atribuído.
    *   Exigir que o ID do usuário no payload corresponda exatamente ao ID do usuário autenticado no contexto.
*   **Transferência Entre Grupos Técnicos**:
    *   Alterar o grupo técnico e limpar obrigatoriamente o responsável atual (`ResponsavelId = null`), permitindo a triagem do novo grupo.
*   **Atribuição Técnica Individual**:
    *   Se o chamado possuir um grupo técnico, validar obrigatoriamente que o técnico de destino é membro ativo daquele grupo técnico.
*   **Listagem / Detalhe / Linha do Tempo**:
    *   As consultas devem incluir os relacionamentos do grupo e fila e montar o histórico correspondente formatado.

---

## 6. Critérios de API

A borda HTTP em `src/SGX.SistemaChamado.Api` deve obedecer ao seguinte design de endpoints em [AdminGruposTecnicosController](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Api/Controllers/AdminGruposTecnicosController.cs) e [AdminChamadosController](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Api/Controllers/AdminChamadosController.cs):

*   **Listagem de Grupos Técnicos**: `GET /api/admin/grupos-tecnicos` (Permissão: Administrador ou Atendente)
*   **Detalhes de Grupo**: `GET /api/admin/grupos-tecnicos/{id}` (Permissão: Administrador ou Atendente)
*   **Cadastro de Grupo**: `POST /api/admin/grupos-tecnicos` (Permissão: Administrador)
*   **Atualização de Grupo**: `PUT /api/admin/grupos-tecnicos/{id}` (Permissão: Administrador)
*   **Inativação/Ativação de Grupo**: `PATCH /api/admin/grupos-tecnicos/{id}/status` (Permissão: Administrador)
*   **Listar Filas do Grupo**: `GET /api/admin/grupos-tecnicos/{grupoTecnicoId}/filas` (Permissão: Administrador ou Atendente)
*   **Listar Membros do Grupo**: `GET /api/admin/grupos-tecnicos/{grupoTecnicoId}/membros` (Permissão: Administrador ou Atendente)
*   **Adicionar Membro**: `POST /api/admin/grupos-tecnicos/{grupoTecnicoId}/membros` (Permissão: Administrador)
*   **Alterar Status de Membro**: `PATCH /api/admin/grupos-tecnicos/{grupoTecnicoId}/membros/{membroId}/status` (Permissão: Administrador)
*   **Direcionar para Grupo**: `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico` (Permissão: Administrador ou Atendente)
*   **Assumir Fila**: `POST /api/admin/chamados/{id}/assumir-fila` (Permissão: Administrador ou Atendente)
*   **Transferir Grupo**: `POST /api/admin/chamados/{id}/transferir-grupo-tecnico` (Permissão: Administrador ou Atendente)
*   **Endpoints Legados Preservados**:
    *   Abertura de Chamado (`POST /api/portal/chamados` ou `POST /api/admin/chamados`)
    *   Assumir Chamado Legado (`POST /api/admin/chamados/{id}/assumir`)
    *   Atribuir Chamado Legado (`POST /api/admin/chamados/{id}/atribuir`)

---

## 7. Critérios de Frontend

A interface do usuário desenvolvida em Vue 3 + Quasar no projeto `src/SGX.SistemaChamado.Web` deve seguir as seguintes diretrizes:

*   **Telas Administrativas**:
    *   Telas em `/admin/cadastros/grupos-tecnicos` para CRUD de grupos, membros ativos e inativos, e visualização de filas por grupo técnico.
*   **Null-Safety Visual**:
    *   Exibir fallbacks visuais adequados (ex: *"Sem grupo técnico"*, *"Sem fila"*) para campos nulos de chamados legados.
*   **Controles no Detalhe**:
    *   Exibir botão **"Assumir da Fila"** apenas quando o chamado estiver direcionado para um grupo/fila sem responsável atribuído e se o usuário autenticado for membro ativo daquele grupo.
    *   Exibir botão **"Transferir de Grupo"** para atendentes/administradores efetuarem trâmite de equipe.
*   **Segurança e Tratamento de Estado**:
    *   A interface não deve alterar campos mutáveis do chamado localmente na listagem geral. Modificações e trâmites de negócio devem obrigatoriamente passar por requisições HTTP que invocam as regras de negócio no backend.

---

## 8. Critérios de Permissões

A matriz de direitos de acesso operacional em runtime deve validar:

1.  **Administrador**: Gerenciamento irrestrito de cadastros de grupos técnicos, inclusão e status de membros, além de transições de chamados.
2.  **Atendente (Técnico)**: Visualização de grupos e membros, execução de direcionamentos, transferência e assunção de chamados da fila.
3.  **Solicitante**: Bloqueio completo contra a visualização ou alteração de grupos técnicos, membros ou filas de chamados.
4.  **Restrição em Assumir Fila**: É obrigatório que o atendente autenticado possua vínculo ativo na tabela de membros do grupo para o qual o chamado está posicionado.
5.  **Restrição em Atribuição**: Se o chamado estiver sob a custódia de um grupo técnico, o técnico atribuído deve pertencer ativamente a esse grupo.

---

## 9. Critérios de `ResponsavelId`

A tabela a seguir consolida o comportamento esperado do campo `ResponsavelId` em resposta a cada operação:

| Operação | Comportamento Esperado | Teste ou Evidência |
| :--- | :--- | :--- |
| **Abertura de Chamado** | Iniciado como `null` | Testes de regressão de abertura |
| **Direcionamento Técnico** | Preserva o `ResponsavelId` atual (se houver) | `DirecionarChamadoGrupoTecnicoAdminUseCaseTests.cs` |
| **Ajuste de Fila** | Preserva o `ResponsavelId` atual | Mapeamento no use case de direcionamento |
| **Assumir Fila** | Preenche com o ID do atendente autenticado | `AssumirChamadoFilaAdminUseCaseTests.cs` |
| **Atribuição Direta** | Altera para o técnico destino (deve ser membro ativo) | `AtribuirChamadoUseCaseTests.cs` |
| **Reatribuição Direta** | Altera para o novo técnico (deve ser membro ativo) | `AtribuirChamadoUseCaseTests.cs` |
| **Transferência de Grupo** | **Limpa** (setado para `null`) | `TransferirGrupoTecnicoChamadoUseCaseTests.cs` |
| **Cadastro de Grupos** | Sem alteração em chamados | `GruposTecnicosAdminUseCaseTests.cs` |
| **Gestão de Membros** | Sem alteração retroativa em chamados atribuídos | `MembrosGruposTecnicosAdminUseCaseTests.cs` |
| **Listagem / Detalhe** | Preserva o estado original do banco | `ListarChamadosAdminUseCaseTests.cs` |

---

## 10. Critérios de Auditoria

A rastreabilidade dos trâmites de chamados na linha do tempo deve cobrir:

*   **Identificação de Eventos**:
    *   Uso estrito de novos tipos de histórico injetados no enum [TipoHistoricoChamado](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/src/SGX.SistemaChamado.Domain/Enums/TipoHistoricoChamado.cs): `GrupoTecnicoDefinido` (34), `GrupoTecnicoTransferido` (33), `FilaAtendimentoDefinida` (35), `FilaAtendimentoRemovida` (36), `FilaAtendimentoTransferida` (37), `ResponsavelRemovidoPorTransferenciaGrupo` (38) e `ChamadoAssumidoDaFila` (39).
*   **Preservação de Ordem de Enums**:
    *   Os novos tipos de histórico devem ser adicionados estritamente ao final do enum, para garantir que as movimentações antigas não sofram desordenação ou desserialização incorreta.
*   **Descrição Textual Coerente**:
    *   A descrição salva no log de auditoria deve expor claramente a origem e o destino do trâmite (ex: *"Grupo técnico transferido de Infraestrutura para Sistemas"*).
*   **Impedimento de Fabriação na UI**:
    *   Eventos de auditoria são criados e registrados em transação de banco de dados pelo backend nos Use Cases. O frontend apenas exibe os dados consumidos de forma segura.

---

## 11. Critérios de Regressão

Comportamentos que **não devem ter sido alterados** e exigem verificação contínua:

1.  **Abertura de Chamado Legada**: Deve continuar funcionando sem requerer informações de grupo técnico ou fila.
2.  **Políticas de SLA Globais**: Os prazos globais de SLA, cálculos de datas limite e alertas iniciados no momento de abertura do chamado devem ser preservados sem alteração de lógica.
3.  **Fluxo de Assumir Tradicional**: O atendente geral deve conseguir assumir chamados sem grupo/fila que estejam em triagem geral.
4.  **Dashboards e Relatórios**: Os dashboards preexistentes não devem ter tido suas consultas alteradas ou quebradas por conta das colunas opcionais nulas.

---

## 12. Critérios de Execução de Testes Automatizados

A suíte de testes deve ser executada localmente utilizando os seguintes comandos no terminal:

*   **Executar Testes do Backend Filtrados (Sprint 3)**:
    ```powershell
    dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --filter "FullyQualifiedName~GruposTecnicos|FullyQualifiedName~MembrosGrupos|FullyQualifiedName~DirecionarChamado|FullyQualifiedName~AssumirChamadoFila|FullyQualifiedName~TransferirGrupoTecnico"
    ```
*   **Executar Suite Backend Completa**:
    ```powershell
    dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj --no-build
    ```
*   **Executar Suite Frontend**:
    ```powershell
    powershell -ExecutionPolicy Bypass -Command "npm run test:unit"
    ```
*   **Executar Compilação Frontend**:
    ```powershell
    powershell -ExecutionPolicy Bypass -Command "npm run build"
    ```
*   **Executar Compilação Backend (Debug)**:
    ```powershell
    dotnet build SGX.SistemaChamado.sln --no-restore
    ```
*   **Executar Compilação Backend (Release)**:
    ```powershell
    dotnet build SGX.SistemaChamado.sln --configuration Release --no-restore
    ```
*   **Validar Pending Model Changes**:
    ```powershell
    dotnet ef migrations has-pending-model-changes --project src/SGX.SistemaChamado.Infrastructure --startup-project src/SGX.SistemaChamado.Api
    ```

---

## 13. Critérios de Aceite por Camada

A tabela a seguir especifica as evidências e falhas bloqueantes esperadas para cada camada arquitetural:

| Camada | Critério de Sucesso | Evidência Esperada | Falha Bloqueante |
| :--- | :--- | :--- | :--- |
| **Banco de Dados** | Estruturas nullable e chaves estrangeiras restritas | migrations aplicadas na lista do EF e schema atualizado | Migration ausente ou nulos rejeitados em campos novos |
| **Domínio** | Métodos seguros de definição e trâmite sem quebras | Testes unitários de domínio aprovados | Exceção não tratada ao passar valores nulos |
| **Aplicação** | Restrições de negócio aplicadas no trâmite | Testes de casos de uso com validação de membro ativo | Atribuição permitida a técnico fora do grupo |
| **API** | Endpoints expostos com as claims corretas | Testes de endpoints de integração aprovados | Acesso livre a endpoints administrativos por Solicitantes |
| **Frontend** | Renderização sem quebras e ações contextuais seguras | Build bem-sucedido e Vitest cobrindo views | Tela em branco ao carregar chamado legado sem grupo |

---

## 14. Critérios de Não Regressão (Falhas Bloqueantes)

O avanço da homologação da Sprint 3 deve ser **bloqueado imediatamente** se qualquer um dos seguintes comportamentos for identificado:

*   **Abertura Bloqueada**: Abertura de chamados exigindo grupo ou fila como campo obrigatório.
*   **Transferência Incompleta**: Transferência de grupo técnico concluída sem limpar o responsável individual anterior.
*   **Assunção Indevida**: Permissão para atendente assumir chamado de fila pertencente a grupo técnico do qual ele não é membro ativo.
*   **Atribuição Indevida**: Atribuição individual de responsável para técnico que não faz parte do grupo técnico do chamado.
*   **Fila Cruzada**: Aceite de associação de fila que não pertence ao grupo técnico do chamado.
*   **Histórico Reordenado**: Enum de histórico reordenado causando leitura incorreta de logs legados de auditoria.
*   **Mutações na UI**: Controles na interface do usuário simulando ou modificando dados diretamente na listagem sem passar pelo crivo do backend.
*   **Modelo EF Desatualizado**: Presença de alterações de modelo pendentes no Entity Framework.
*   **Falhas de Compilação/Testes**: Qualquer erro ou quebra na compilação ou na execução das suítes de teste de backend ou frontend.

---

## 15. Critérios de Documentação de Evidência

Qualquer registro de testes deve conter:
*   Os comandos executados de compilação, testes e migração.
*   A contagem final de testes aprovados (ex: *1315 testes backend e 198 testes frontend aprovados*).
*   Warnings preexistentes documentados que não impactam a entrega.
*   Declaração expressa de que não foram criados endpoints, componentes visuais ou regras fora do planejado originalmente na Sprint 3.
*   *Nota sobre versionamento*: Arquivos temporários locais como `.dotnet-cli-home` gerados pela CLI do .NET não devem ser versionados.

---

## 16. Warnings e Ruídos Conhecidos

Os seguintes ruídos são classificados como conhecidos e não impedem a conclusão da tarefa:

*   **Vite Chunk Size Warning**: Alerta informando que o bundle `dist/assets/index-CZ9b5RP0.js` excede 500 kB. Esse ruído é decorrente da agregação de dependências pesadas como o Quasar e Pinia e deve ser mitigado em sprints futuras.
*   **Telemetry local do .NET**: Criação ocasional do diretório `.dotnet-cli-home` no workspace local em ambiente Windows devido às execuções de testes. Esses arquivos devem ser limpos ou ignorados e não versionados.
*   **Warnings preexistentes de autenticação**: Avisos de testes ou logs preexistentes de AD/Microsoft Entra ID.

---

## 17. Matriz de Rastreabilidade

A matriz a seguir mapeia as áreas funcionais testadas na Sprint 3 até suas respectivas classes de teste e objetivos de negócio:

| Área Testada | Classe de Teste Principal | Endpoint / UseCase Relacionado | Objetivo da Validação |
| :--- | :--- | :--- | :--- |
| **Cadastro de Grupo** | [GruposTecnicosAdminUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/GruposTecnicosAdminUseCaseTests.cs) | `POST /api/admin/grupos-tecnicos` | Validar criação de equipes com nomes únicos e status ativos |
| **Membros do Grupo** | [MembrosGruposTecnicosAdminUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/MembrosGruposTecnicosAdminUseCaseTests.cs) | `POST /api/admin/grupos-tecnicos/{id}/membros` | Garantir que apenas usuários elegíveis com vínculo ativo possam compor o grupo |
| **Direcionamento** | [DirecionarChamadoGrupoTecnicoAdminUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/DirecionarChamadoGrupoTecnicoAdminUseCaseTests.cs) | `POST /api/admin/chamados/{id}/direcionar-grupo-tecnico` | Validar alocação em fila e grupo consistentes preservando o responsável atual |
| **Assumir Fila** | [AssumirChamadoFilaAdminUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/AssumirChamadoFilaAdminUseCaseTests.cs) | `POST /api/admin/chamados/{id}/assumir-fila` | Bloquear atendentes de fora do grupo e atribuir o responsável logado |
| **Transferência** | [TransferirGrupoTecnicoChamadoUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/TransferirGrupoTecnicoChamadoUseCaseTests.cs) | `POST /api/admin/chamados/{id}/transferir-grupo-tecnico` | Garantir a reclassificação de equipe e a remoção/limpeza obrigatória do responsável |
| **Auditoria** | [AuditoriaModulosCriticosTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/AuditoriaModulosCriticosTests.cs) | Linha do Tempo e Historico | Verificar gravação textual precisa dos trâmites de transferência e filas |
| **Regressão Atribuição** | [AtribuirChamadoUseCaseTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/AtribuirChamadoUseCaseTests.cs) | `POST /api/admin/chamados/{id}/atribuir` | Validar que técnicos só podem ser atribuídos a chamados com grupo se pertencerem a ele |
| **Regressão Abertura** | [RegressaoAberturaAtribuicaoChamadoEndpointsIntegrationTests.cs](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/tests/SGX.SistemaChamado.Tests/RegressaoAberturaAtribuicaoChamadoEndpointsIntegrationTests.cs) | endpoints de abertura de chamados | Garantir que chamados novos sem grupo ou fila continuam sendo abertos normalmente |

---

## 18. Próximas Validações

Com a documentação de critérios técnicos concluída, a Sprint 3 deve avançar para as etapas finais:

1.  **Roteiro de Homologação de Produtividade por Grupo Técnico**: Roteiro funcional focado em validar tempos de resposta e atendimento por equipe técnica.
2.  **Roteiro de Homologação de Visibilidade por Fila**: Roteiro focado em assegurar que cada equipe filtre e acesse exclusivamente sua fila de chamados posicionados.
3.  **Aceite Final**: Consolidação das assinaturas e homologações de negócio.
