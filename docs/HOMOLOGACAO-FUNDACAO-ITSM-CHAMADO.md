# Roteiro de Homologação da Fundação ITSM (ITIL v4) — SGX Sistema de Chamados

Este documento detalha o guia oficial de homologação corporativa para a **Fundação ITSM do Chamado** no SGX, contendo os roteiros de validação passo a passo por ator de negócios, cenários de sucesso, bloqueios de segurança e o checklist final de aceite.

---

## 1. Introdução e Credenciais Locais (Dev/Hml)

> [!NOTE]
> Os usuários de homologação corporativa são criados automaticamente no ambiente de desenvolvimento local através da classe `DevelopmentSeedService.cs`. As senhas são tratadas de forma criptográfica usando `IPasswordHasher<Usuario>` para garantir a segurança e não expor credenciais reais.
> 
> **Senha Padrão Dev/Hml:** A senha padrão local para todos os usuários abaixo é a senha administrativa regulada pelo arquivo `appsettings.Development.json` (geralmente `SgxDev123!`). Todos os atores devem trocar a sua senha corporativa após o primeiro login.

---

## 2. Roteiros de Homologação por Ator

### 1. Solicitante / Usuário Final
* **Perfil de Acesso:** `Solicitante`
* **Usuário Sugerido:** `solicitante.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Solicitante deve visualizar **exclusivamente** os menus do Portal do Solicitante:
- [x] **Meus chamados** (`list_alt`)
- [x] **Abrir chamado** (`add_circle`)
- [x] **Base de conhecimento** (`menu_book`)
- [x] **Minha conta** (`person`)

#### B. Ações Permitidas (Sucesso)
- Acessar o Portal do Solicitante (`/portal/chamados`).
- Abrir um chamado preenchendo o título, descrição, selecionando a Natureza (Incidente, Requisição), Impacto (Baixo, Médio, Alto) e Urgência (Baixa, Média, Alta).
- Visualizar e filtrar apenas os seus próprios chamados abertos.
- Adicionar comentários e fazer upload de arquivos de evidências no próprio chamado.
- Ler artigos públicos na Base de Conhecimento.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** acessar o painel administrativo (`/admin`). Qualquer tentativa de forçar a URL deve resultar em redirecionamento imediato para a tela de **Acesso Negado** (`/acesso-negado`).
- **NÃO** visualizar ou interagir com chamados de outros colaboradores.
- **NÃO** assumir, atribuir responsabilidades, reclassificar categorias ou alterar a prioridade calculada dos chamados.

#### D. Cenário de Teste Prático
1. Faça login como `solicitante.hml@sgx.local`.
2. Vá em **Abrir chamado**, crie uma nova solicitação:
   - Título: "Erro de Acesso ao ERP Corporativo"
   - Descrição: "Ao clicar no botão de faturamento, a página trava carregando infinitamente."
   - Natureza: Incidente
   - Impacto: Médio | Urgência: Alta (Prioridade Calculada: Alta)
3. Anexe uma imagem de erro e envie.
4. Tente digitar `/admin` na barra de endereços do navegador e confirme se foi bloqueado.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 2. Atendente N1
* **Perfil de Acesso:** `Atendente N1`
* **Usuário Sugerido:** `atendente.n1.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Atendente N1 deve visualizar os menus operacionais básicos:
- [x] **Fila de atendimento** (`list_alt`)
- [x] **Meus atendimentos** (`assignment_ind`)
- [x] **Triagem** (`assignment`)
- [x] **Base de conhecimento** (`menu_book`)

#### B. Ações Permitidas (Sucesso)
- Visualizar todos os chamados da fila corporativa operacional.
- Triar e saneador dados básicos do chamado (verificar integridade do preenchimento).
- Assumir a responsabilidade por chamados de menor complexidade.
- Alterar status permitidos (ex: mover de "Aberto" para "Em Atendimento").
- Direcionar/atribuir o chamado para outras filas técnicas (N2) ou responsáveis específicos.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** acessar as telas administrativas de Usuários, Perfis, Integrações ou Parâmetros globais do sistema.
- **NÃO** gerenciar políticas estruturais de metas ou calendários de SLA.
- **NÃO** inativar ou alterar configurações críticas de segurança.

#### D. Cenário de Teste Prático
1. Faça login como `atendente.n1.hml@sgx.local`.
2. Vá em **Fila de atendimento**, localize um chamado em aberto da fila geral.
3. Clique em **Assumir chamado** e verifique se o Responsável mudou para o seu usuário.
4. Mude o status para **Em Atendimento**.
5. No menu esquerdo, tente acessar a tela de **Perfis e permissões** e confirme que ela está invisível e inacessível.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 3. Técnico N2 / Especialista
* **Perfil de Acesso:** `Técnico N2`
* **Usuário Sugerido:** `tecnico.n2.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Técnico N2 deve visualizar menus técnicos avançados:
- [x] **Minha fila técnica** (`engineering`)
- [x] **Chamados escalados** (`arrow_upward`)
- [x] **Problemas** (`report_problem`)
- [x] **Mudanças** (`published_with_changes`)
- [x] **Tarefas operacionais** (`playlist_add_check`)
- [x] **Base de conhecimento** (`menu_book`)

#### B. Ações Permitidas (Sucesso)
- Acessar a fila de chamados complexos direcionados ao N2.
- Registrar e gerenciar **Problemas** (identificação de causa raiz e soluções de contorno).
- Visualizar e gerenciar **Mudanças** planejadas e autorizadas no ambiente técnico.
- Executar e dar baixa em **Tarefas operacionais** vinculadas a mudanças ou rotinas.
- Escrever e publicar artigos técnicos de causa raiz e solução de contorno na Base de Conhecimento.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** acessar a tela de administração de usuários ou de atribuição de permissões a perfis.
- **NÃO** redefinir senhas de outros colaboradores.
- **NÃO** alterar parametrizações de integrações com Microsoft AD ou provedores de e-mail.

#### D. Cenário de Teste Prático
1. Faça login como `tecnico.n2.hml@sgx.local`.
2. Acesse o menu **Problemas**, clique em criar ou visualizar o problema associado a quedas recorrentes.
3. Vincule o incidente `HML-INC-001` ao problema técnico correspondente.
4. Escreva uma solução de contorno na aba de anotações técnicas.
5. Tente abrir a página `/admin/cadastros/usuarios` via URL direta e confirme o bloqueio.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 4. Coordenador Service Desk
* **Perfil de Acesso:** `Coordenador Service Desk`
* **Usuário Sugerido:** `coordenador.service.desk.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Coordenador deve visualizar o painel geral de controle operacional:
- [x] **Dashboard operacional** (`space_dashboard`)
- [x] **Fila geral** (`toc`)
- [x] **Chamados críticos** (`priority_high`)
- [x] **SLA** (`schedule`)
- [x] **Atribuições** (`group_add`)
- [x] **Aprovações** (`fact_check`)
- [x] **Relatórios operacionais** (`analytics`)

#### B. Ações Permitidas (Sucesso)
- Visualizar métricas operacionais consolidadas (chamados em aberto, resolvidos, atrasados).
- Executar atribuição em massa de chamados (balanceamento de carga dos atendentes).
- Aprovar ou rejeitar requisições de acesso ou mudanças operacionais pendentes.
- Monitorar a fila geral e intervir em chamados escalados ou críticos.
- Visualizar políticas de SLA e prazos de resposta/resolução vigentes.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** editar as políticas ou metas do SLA de forma direta (apenas visualizar).
- **NÃO** alterar as chaves de integração, logs de auditoria e configurações gerais do SGX.
- **NÃO** gerenciar perfis, permissões e cadastro de novos usuários administradores.

#### D. Cenário de Teste Prático
1. Faça login como `coordenador.service.desk.hml@sgx.local`.
2. Abra o **Dashboard operacional** e verifique os painéis de tempo útil de atendimento.
3. Acesse a **Fila geral**, localize um chamado sem atendente e atribua-o ao `tecnico.n2.hml@sgx.local`.
4. Vá na tela de **Aprovações** e verifique as solicitações na fila.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 5. Gestor TI
* **Perfil de Acesso:** `Gestor TI`
* **Usuário Sugerido:** `gestor.ti.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Gestor de TI deve visualizar a central analítica de alto nível:
- [x] **Dashboard executivo** (`dashboard`)
- [x] **Indicadores ITSM** (`trending_up`)
- [x] **Relatórios** (`analytics`)
- [x] **SLA** (`schedule`)
- [x] **Problemas recorrentes** (`sync_problem`)
- [x] **Mudanças** (`published_with_changes`)

#### B. Ações Permitidas (Sucesso)
- Acessar o Dashboard Executivo de métricas de desempenho e SLA global.
- Exportar relatórios gerenciais avançados (Excel/CSV) para apresentações de diretoria.
- Acompanhar indicadores de satisfação do usuário e tempo médio de atendimento.
- Visualizar problemas graves e o andamento de mudanças estruturais planejadas.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** realizar ações operacionais nos chamados (assumir, atribuir, comentar ou dar baixa em tarefas).
- **NÃO** alterar parametrizações de infraestrutura ou permissões administrativas de sistema.

#### D. Cenário de Teste Prático
1. Faça login como `gestor.ti.hml@sgx.local`.
2. Acesse o **Dashboard executivo** e confirme a exibição de gráficos gerenciais consolidando incidentes vs problemas.
3. Abra a listagem de chamados, entre em um chamado aberto e tente clicar em "Assumir" — certifique-se de que a opção está indisponível para o seu perfil.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 6. Administrador
* **Perfil de Acesso:** `Administrador`
* **Usuário Sugerido:** `administrador.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Administrador do Sistema deve visualizar a área de controle global:
- [x] **Usuários** (`group`)
- [x] **Perfis e permissões** (`badge`)
- [x] **Cadastros administrativos** (`dataset`)
- [x] **SLA** (`schedule`)
- [x] **Integrações** (`hub`)
- [x] **Configurações** (`settings`)
- [x] **Roadmap** (`insights`)

#### B. Ações Permitidas (Sucesso)
- Gerenciar usuários corporativos (criar, ativar, inativar, resetar senhas).
- Configurar perfis e permissões interativas, marcando/desmarcando privilégios.
- Cadastrar e manter prioridades, localizações, categorias e departamentos operacionais.
- Parametrizar conexões de e-mail, conexões de AD e regras corporativas.
- Visualizar e gerenciar o Roadmap de implantações e atualizações do chamado.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** expor chaves produtivas em logs públicos.
- **NÃO** alterar regras básicas do ITIL v4 ou a matriz fundamental de SLAs sem controle de auditoria de segurança.

#### D. Cenário de Teste Prático
1. Faça login como `administrador.hml@sgx.local`.
2. Vá em **Perfis e permissões**, clique no perfil `Gestor TI` e tente desmarcar temporariamente uma permissão. Salve e verifique a mensagem de sucesso.
3. Acesse a tela de **Integrações** e revise as configurações parametrizadas.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

### 7. Auditor Governança
* **Perfil de Acesso:** `Auditor Governança`
* **Usuário Sugerido:** `auditor.governanca.hml@sgx.local`

#### A. Visibilidade de Menus (Sidebar)
O Auditor Governança deve visualizar a trilha de segurança e conformidade:
- [x] **Consulta de chamados** (`search`)
- [x] **Histórico e auditoria** (`manage_search`)
- [x] **Relatórios** (`analytics`)
- [x] **SLA** (`schedule`)
- [x] **Aprovações** (`fact_check`)
- [x] **Logs/Auditoria** (`security`)

#### B. Ações Permitidas (Sucesso)
- Visualizar de forma integral e irrestrita todas as trilhas de auditoria operacional de chamados (quem alterou, o que alterou, quando alterou).
- Consultar logs detalhados de login e segurança de autenticação corporativa.
- Visualizar painéis e relatórios analíticos de SLA e conformidade legal de dados.
- Consultar fluxos de aprovação executados com as respectivas assinaturas eletrônicas.

#### C. Bloqueios de Segurança (Não deve conseguir)
- **NÃO** realizar qualquer alteração cadastral ou operacional (criar chamados, alterar status, prioridades, categorias, comentar ou assumir chamados).
- **NÃO** alterar regras de segurança ou parâmetros do sistema.

#### D. Cenário de Teste Prático
1. Faça login como `auditor.governanca.hml@sgx.local`.
2. Vá em **Logs/Auditoria** e confirme a visualização dos registros de login efetuados no sistema.
3. Acesse **Consulta de chamados**, abra o chamado `HML-INC-001` e certifique-se de que todos os campos estão travados no modo de leitura (Read-Only) e que não há botões operacionais disponíveis.

---
#### E. Registro de Validação
* **Responsável:** ________________________
* **Data:** ____/____/______
* **Resultado:** [  ] Aprovado com Sucesso  |  [  ] Pendente/Falhou
* **Observações:** __________________________________________________________________

---

## 3. Checklist Final de Homologação ITSM

Este checklist consolida a validação corporativa completa para homologar e autorizar a subida da Fundação ITSM em produção.

| Item | Descrição / Critério de Aceite | Validador | Status | Data |
| :--- | :--- | :---: | :---: | :---: |
| **01** | Todos os 7 usuários de homologação corporativa dev estão disponíveis para login. | Operação | [  ] Pendente | ____/____/____ |
| **02** | Os perfis de homologação estão vinculados correta e estritamente a cada usuário. | Segurança | [  ] Pendente | ____/____/____ |
| **03** | O menu dinâmico renderiza estrita e reativamente conforme o perfil do usuário logado. | UX/Front | [  ] Pendente | ____/____/____ |
| **04** | Tentativas de acesso direto por URL a rotas protegidas barram com "Acesso Negado". | Segurança | [  ] Pendente | ____/____/____ |
| **05** | A massa mínima de 6 chamados cobrindo as 6 naturezas do ITIL v4 está disponível no sistema. | ITSM | [  ] Pendente | ____/____/____ |
| **06** | O cálculo de prioridades via matriz Impacto x Urgência está 100% correto. | Backend | [  ] Pendente | ____/____/____ |
| **07** | O Solicitante possui acesso exclusivo aos seus chamados e portal e não vê painel `/admin`. | Solicitante | [  ] Pendente | ____/____/____ |
| **08** | O Atendente N1 consegue triar, assumir, atribuir e interagir com chamados com sucesso. | N1 | [  ] Pendente | ____/____/____ |
| **09** | O Técnico N2 visualiza chamados complexos, e acessa Problemas, Mudanças e Tarefas. | N2 | [  ] Pendente | ____/____/____ |
| **10** | O Coordenador acessa painéis, relatórios operacionais, atribui e realiza aprovações. | Coordenador| [  ] Pendente | ____/____/____ |
| **11** | O Gestor TI acessa dashboard de performance corporativa e não executa ações operacionais. | Gestor | [  ] Pendente | ____/____/____ |
| **12** | O Auditor visualiza todos os históricos, relatórios e logs de segurança e não altera nada. | Auditor | [  ] Pendente | ____/____/____ |
| **13** | O Administrador mantém controle total de usuários, perfis, cadastros e parametrizações. | Admin | [  ] Pendente | ____/____/____ |
| **14** | Nenhuma senha real ou credencial corporativa confidencial foi exposta em logs ou código. | Segurança | [  ] Pendente | ____/____/____ |

---

## 4. Assinatura e Liberação Final

A equipe técnica e de negócios abaixo-assinada declara que os testes de homologação foram executados e homologados sob os critérios descritos acima:

- **Coordenador do Projeto SGX:** _______________________________  Data: ____/____/______
- **Líder Técnico de Desenvolvimento:** __________________________  Data: ____/____/______
- **Gestor ITSM do Cliente:** __________________________________  Data: ____/____/______
