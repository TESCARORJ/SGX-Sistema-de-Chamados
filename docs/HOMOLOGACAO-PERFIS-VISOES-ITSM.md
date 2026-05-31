# Perfis de Homologação e Visões ITSM (ITIL v4)

Este documento estabelece a modelagem conceitual para os novos perfis operacionais da homologação ITSM, detalhando seus papéis, as visões/menus correspondentes e o mapeamento de segurança que guiará a implementação de perfis configuráveis na Sprint H0.3.

---

## 1. Contexto e Objetivos

O SGX Sistema de Chamados está evoluindo para suportar plenamente as práticas recomendadas pelo **ITIL v4** (Gerenciamento de Incidentes, Gerenciamento de Problemas, Gerenciamento de Mudanças e Central de Serviços). 

Para suportar essas práticas sem introduzir complexidade desnecessária no motor de autorização, mapeamos conceitualmente **5 novos perfis operacionais corporativos**. Na Sprint H0.3, esses perfis serão cadastrados no banco de dados e suas permissões poderão ser editadas dinamicamente.

---

## 2. Perfis de Homologação — Definições Conceituais

### A. Solicitante (Client/User)
* **Propósito:** Usuário final corporativo que consome os serviços da central.
* **Ações Principais:** Abrir chamados via portal, interagir nos chamados próprios, anexar evidências de erro e consultar a Base de Conhecimento pública.
* **Escopo de Visibilidade:** Estritamente limitado aos seus próprios chamados. Não possui acesso à área administrativa `/admin`.

### B. Atendente N1 (Service Desk Agent / Triador)
* **Propósito:** Primeiro ponto de contato corporativo (Central de Serviços).
* **Ações Principais:** Triagem inicial de chamados na fila geral, saneamento de dados (verificar preenchimento de campos obrigatórios), classificação de prioridades operacionais, escalonamento e resolução de chamados simples de baixa complexidade.
* **Menus Focados:** Fila de atendimento, Meus atendimentos, Triagem, Abertura rápida.
* **Permissões Críticas:** `Chamados.Visualizar`, `Chamados.Assumir`, `Chamados.Atribuir`, `Chamados.AlterarStatus`.

### C. Técnico N2 (Technical Specialist / Analista de Suporte)
* **Propósito:** Suporte técnico especializado. Resolve incidentes complexos, investiga causas-raiz (Gerenciamento de Problemas) e executa tarefas operacionais.
* **Ações Principais:** Assumir chamados de filas técnicas específicas, interagir com solicitantes, reclassificar chamados técnicos, criar/vincular artigos na Base de Conhecimento, diagnosticar Problemas e executar Tarefas Vinculadas.
* **Menus Focados:** Minha fila técnica, Meus atendimentos, Problemas, Mudanças, Tarefas operacionais, Base de conhecimento.
* **Permissões Críticas:** `Chamados.Visualizar`, `Chamados.Assumir`, `Chamados.Comentar`, `BaseConhecimento.Gerenciar`, `Problemas.Visualizar`, `Mudancas.Visualizar`, `Tarefas.Visualizar`.

### D. Coordenador (Service Desk Coordinator / Incident Manager)
* **Propósito:** Gestão operacional das equipes de atendimento e garantia de cumprimento dos SLAs de suporte.
* **Ações Principais:** Balanceamento de carga de trabalho (atribuir chamados manualmente), monitoramento em tempo real do painel de SLA, aprovação de chamados especiais que exigem liberação financeira ou técnica, e análise do Roadmap operacional.
* **Menus Focados:** Fila geral, SLA (Painel e Políticas), Aprovações, Dashboard operacional, Atribuições, Chamados escalados.
* **Permissões Críticas:** `Chamados.Atribuir`, `Sla.Visualizar`, `AprovacaoChamados.Gerenciar`, `AprovacaoChamados.Aprovar`, `Roadmap.Visualizar`.

### E. Gestor (IT Service Manager / Diretor de TI)
* **Propósito:** Supervisão estratégica do desempenho da TI, contratos de SLA, conformidade e melhoria contínua de processos.
* **Ações Principais:** Visualização de dashboards executivos complexos, relatórios gerenciais consolidados de desempenho, auditoria histórica de chamados e aprovação estratégica de mudanças complexas.
* **Menus Focados:** Dashboard executivo, Indicadores ITSM, Relatórios operacionais, SLA (Painel de Métricas).
* **Permissões Críticas:** `RelatoriosAvancados.Visualizar`, `RelatoriosAvancados.Gerencial`, `RelatoriosAvancados.Exportar`, `Dashboard.Visualizar`.

### F. Auditor (Compliance / Security Auditor)
* **Propósito:** Garantia de segurança, conformidade regulatória e transparência total de ações e alterações estruturais.
* **Ações Principais:** Visualização pura e integral de todos os históricos, trilhas de auditoria do sistema, logs de autenticação e relatórios de conformidade. Não executa ações de atendimento de chamados.
* **Menus Focados:** Histórico e auditoria, Logs/Auditoria, Relatórios de conformidade.
* **Permissões Críticas:** `Auditoria.Visualizar`, `AuditoriaAutenticacao.Visualizar`, `RelatoriosAvancados.Auditoria`.

---

## 3. Matriz de Mapeamento Conceitual de Permissões

Esta tabela orientará os scripts de importação e a configuração padrão a ser exibida na UI do Administrador na Sprint H0.3:

| Módulo / Permissão | Solicitante | Atendente N1 | Técnico N2 | Coordenador | Gestor | Auditor |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| `Dashboard.Visualizar` | | Sim | Sim | Sim | Sim | |
| `Chamados.Visualizar` | Sim | Sim | Sim | Sim | Sim | Sim |
| `Chamados.VisualizarTodos` | | Sim | Sim | Sim | Sim | Sim |
| `Chamados.Abrir` | Sim | Sim | Sim | Sim | | |
| `Chamados.Assumir` | | Sim | Sim | Sim | | |
| `Chamados.Atribuir` | | Sim | | Sim | | |
| `Chamados.AlterarStatus` | | Sim | Sim | Sim | | |
| `Chamados.Comentar` | Sim | Sim | Sim | Sim | | |
| `Chamados.Anexar` | Sim | Sim | Sim | Sim | | |
| `Chamados.Encerrar` | | Sim | Sim | Sim | | |
| `Chamados.Reabrir` | | Sim | Sim | Sim | | |
| `Cadastros.Visualizar` | | Sim | Sim | Sim | Sim | Sim |
| `BaseConhecimento.Visualizar` | Sim | Sim | Sim | Sim | Sim | Sim |
| `BaseConhecimento.Gerenciar` | | | Sim | Sim | | |
| `Sla.Visualizar` | | Sim | Sim | Sim | Sim | |
| `AprovacaoChamados.Visualizar` | | Sim | Sim | Sim | Sim | |
| `AprovacaoChamados.Aprovar` | | | | Sim | Sim | |
| `Auditoria.Visualizar` | | | | | | Sim |
| `AuditoriaAutenticacao.Visualizar`| | | | | | Sim |
| `Problemas.Visualizar` | | Sim | Sim | Sim | Sim | Sim |
| `Mudancas.Visualizar` | | Sim | Sim | Sim | Sim | Sim |
| `Tarefas.Visualizar` | | Sim | Sim | Sim | Sim | Sim |
| `RelatoriosAvancados.Visualizar` | | | | | Sim | Sim |
| `RelatoriosAvancados.Gerencial` | | | | | Sim | |
| `RelatoriosAvancados.Operacional` | | Sim | Sim | Sim | Sim | |

---

## 4. Próximos Passos para a Sprint H0.3

Com o catálogo de menus, rotas e ações mapeado estruturalmente na base H0.2:
1. **Interface de Perfis:** Criamos a View de gerenciamento de perfis baseando-se exatamente nos agrupamentos de módulos exibidos acima.
2. **Carga Inicial de Homologação:** No seed de produção da H0.3, os perfis "Atendente N1", "Técnico N2", "Coordenador", "Gestor" e "Auditor" são criados automaticamente, associando cada um às permissões padrão estabelecidas acima.
3. **Validação Instantânea:** O frontend lê de forma reativa a união dos perfis no `authStore.ts` e aplica a renderização condicional dinâmica sobre todos os 32 menus operacionais descritos na H0.2.

---

## 5. Conclusão da Sprint H0.5 — Massa de Homologação e Roteiros por Ator

A Sprint H0.5 concluiu com sucesso a preparação para a homologação visual e operacional:
- **Usuários Fixos Corporativos:** Criação automática dos 7 usuários de homologação corporativa dev (ex: `solicitante.hml@sgx.local`, `atendente.n1.hml@sgx.local`, etc.) vinculados de forma rigorosa e unificada aos perfis estabelecidos.
- **Massa de Dados de Homologação:** Geração de 6 chamados corporativos de homologação cobrindo as 6 naturezas do chamado (Incidente, Requisição, Mudança, Problema, Evento/Alerta e Tarefa Operacional) com variações de estados, impactos e urgências calculadas.
- **Roteiro Completo por Ator:** O roteiro oficial passo a passo de homologação e o checklist final estão oficialmente localizados em [HOMOLOGACAO-FUNDACAO-ITSM-CHAMADO.md](file:///c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/docs/HOMOLOGACAO-FUNDACAO-ITSM-CHAMADO.md).

---

## 6. Droplist de Visão de Homologação (Homologação Dinâmica)

Para facilitar o processo de homologação e a validação rápida dos menus por ator de negócios, a lateral administrativa (`AdminLayout.vue`) foi equipada com um componente de seleção de visão de homologação reativo (`q-select`).

### Como usar o Seletor de Visão
1. Certifique-se de que o sistema está executando em **ambiente local de desenvolvimento** (`modoLocal` ativado no `authStore`).
2. Faça login com o usuário administrador padrão (`admin@sgxdigital.com`).
3. No menu lateral esquerdo, logo abaixo da marca SGX, localize o campo **"Visão de homologação"**.
4. Clique e selecione qualquer um dos 8 perfis disponíveis na droplist:
   - **Administrador**: Visão completa com menus de gestão, usuários, perfis, etc.
   - **Solicitante**: Redireciona para o Portal do Solicitante (`/portal`) exibindo apenas chamados próprios.
   - **Atendente**: Fila de atendimento padrão.
   - **Atendente N1**: Visão operacional voltada para triagem de chamados.
   - **Técnico N2**: Visão voltada a suporte especializado com abas de Mudanças, Problemas e Tarefas.
   - **Coordenador Service Desk**: Visão de coordenação com métricas de SLA e atribuições.
   - **Gestor TI**: Visão gerencial estratégica com foco em relatórios e dashboards.
   - **Auditor Governança**: Visão de conformidade com acesso a logs de auditoria e relatórios.
5. Para retornar à sua conta e privilégios originais, selecione a opção **"Perfil original"** (ou clique no botão "Voltar para Perfil original" no banner reativo que aparece no topo da página).

### Diferença entre Emulação e Autorização Real
* **Emulação Visual:** O seletor de homologação emula e reescreve localmente a visão do usuário no frontend. O menu lateral e as permissões visuais se reconfiguram instantaneamente com base no perfil selecionado.
* **Segurança do Backend (Autoridade Final):** Toda requisição enviada aos endpoints da API passa por cabeçalhos de homologação de desenvolvimento (`X-Dev-User-Email` e `X-Dev-User-Role`) sincronizados dinamicamente pelo `authStore`. Como o banco de dados local possui usuários seeded correspondentes e com permissões reais mapeadas, o backend valida as requisições com base nas permissões efetivas do usuário associado ao e-mail emulado.
* **Limitação Importante:** Essa funcionalidade de alternância dinâmica é estritamente projetada para fins de homologação e testes de menus/layouts em ambiente local (`Development`). Ela **não** contorna as regras de segurança reais de produção, onde a autenticação integrada com o Microsoft Entra ID é a autoridade absoluta e final.
