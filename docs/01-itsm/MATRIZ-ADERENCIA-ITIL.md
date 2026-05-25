# Matriz de Aderência ITIL / ITSM - SGX Sistema de Chamados

## 1. Objetivo

Este documento registra a aderência atual do SGX Sistema de Chamados a práticas ITIL / ITSM, separando o que já está implementado, o que está parcialmente atendido e o que deve entrar no roadmap de produto.

A matriz deve ser usada para:

- apresentação executiva;
- avaliação técnica por especialistas ITSM;
- definição de roadmap;
- priorização de evoluções;
- homologação institucional.

## 2. Classificação usada

| Status | Significado |
|---|---|
| Implementado funcionalmente | Já existe base funcional no sistema, mas pode depender de homologação final. |
| Parcialmente implementado | Existe parte da capacidade, mas falta formalização ou complemento funcional. |
| Planejado | Ainda não implementado, mas recomendado para o roadmap. |
| Dependente de ambiente | Depende de configuração externa, infraestrutura ou validação institucional. |
| Fora do escopo atual | Não faz parte do MVP atual. |

## 3. Matriz resumida

| Prática / Recurso | Status SGX | Evidência atual | Pendência / Próxima ação |
|---|---|---|---|
| Gerenciamento de Incidentes | Parcialmente implementado | Chamados, prioridade, SLA, histórico, comentários, anexos e atendimento administrativo | Formalizar tipo de chamado como Incidente e regras específicas de tratamento. |
| Gerenciamento de Requisições | Parcialmente implementado | Catálogo de Serviços, abertura orientada por serviço e portal do solicitante | Formalizar tipo de chamado como Requisição e separar fluxo de requisição x incidente. |
| Gerenciamento de Mudanças | Planejado | Roadmap ITSM registra evoluções futuras | Criar módulo de mudanças com aprovação, impacto, risco, janela, plano de rollback e histórico. |
| Catálogo de Serviços | Implementado funcionalmente | Módulo de Catálogo de Serviços, consulta no portal e abertura por serviço | Homologar com usuários reais e evoluir formulários dinâmicos por serviço. |
| Base de Conhecimento | Implementado funcionalmente | Módulo de artigos, publicação, consulta no portal e vínculo ao chamado | Homologar, criar evidências e evoluir workflow editorial. |
| SLA | Implementado funcionalmente | Políticas, metas, aplicação no chamado, alertas, dashboard e calendário corporativo | Homologar com dados reais, validar feriados, horários e regras por área. |
| Inventário / Ativos | Implementado funcionalmente | Cadastro de ativos, histórico, movimentação e vínculo com chamados | Homologar, evoluir importação, exportação, QR Code, manutenção e integração patrimonial. |
| CMDB | Parcial / futuro | Inventário/Ativos e vínculo com chamados já existem | Evoluir para relacionamentos entre ativos, serviços, dependências e análise de impacto. |
| Análise de Impacto | Planejado | Base de ativos e catálogo permitem evolução | Criar modelo de dependência entre serviços, ativos, áreas e chamados. |
| Abertura por Portal | Implementado funcionalmente | Portal do solicitante e criação de chamados | Homologar com usuário real e registrar evidências. |
| Abertura por E-mail | Implementado funcionalmente | Worker IMAP, criação de chamado, correlação e logs administrativos | Validar caixa IMAP real, OAuth Microsoft se necessário, retry/backoff e monitoramento. |
| Comentários e Anexos | Implementado funcionalmente | Comentários públicos/internos, upload/listagem/download de anexos | Homologar e manter regras de segurança de anexos. |
| Histórico / Linha do Tempo | Implementado funcionalmente | Consolidação de eventos, comentários e anexos | Homologar com usuários reais. |
| Auditoria | Implementado funcionalmente | Eventos de auditoria, filtros, detalhe e indicadores | Evoluir exportação, retenção, alertas e integração SIEM. |
| Perfis e Permissões | Implementado funcionalmente | Administrador, Atendente, Solicitante e permissões granulares | Homologar matriz de permissões com usuários-chave. |
| MFA | Dependente de ambiente | Arquitetura baseada em Microsoft Entra ID | Validar MFA e Conditional Access no tenant institucional. |
| Observador de Chamado | Planejado | Não consolidado no MVP atual | Criar modelo de observadores, notificações e permissão de visualização. |
| Grupo Técnico | Planejado / Parcial | Perfis e usuários existem | Criar grupos técnicos, filas por grupo, atribuição e regras de visibilidade. |
| Regras de Notificação | Parcialmente implementado | Central frontend/local e logs existem | Criar API persistente de notificações e regras configuráveis. |
| Pesquisa de Satisfação | Planejado | Não consolidado no MVP atual | Criar pesquisa após fechamento e indicadores de satisfação. |
| Regra de Fechamento | Planejado | Resolução/encerramento existem no fluxo de chamado | Criar regras configuráveis de fechamento, aceite do solicitante e encerramento automático. |
| Formulários Dinâmicos | Planejado | Catálogo de Serviços permite evolução | Criar campos dinâmicos por serviço, obrigatoriedade e validações. |
| Integração Zabbix | Planejado | Não consolidado no MVP atual | Criar integração para abertura/atualização automática de incidentes. |
| Gerenciamento de Projetos | Planejado | Fora do core atual de chamados | Avaliar se será módulo próprio ou integração com ferramenta externa. |
| Relatórios Exportáveis | Em evolução | Dashboard e consultas estruturadas existem | Criar exportação Excel/PDF e filtros gerenciais. |
| Dashboards Gerenciais | Implementado funcionalmente | Dashboard administrativo e indicadores | Homologar com massa real e refinar layout para diretoria. |

## 4. Gerenciamento de Incidentes

### Situação atual

O SGX já possui a base essencial para tratamento de incidentes:

- abertura de chamado;
- prioridade;
- categoria;
- subcategoria;
- departamento;
- SLA;
- comentários;
- anexos;
- histórico;
- auditoria;
- atendimento administrativo;
- dashboard.

### Lacuna

Ainda falta formalizar o tipo de chamado como Incidente e criar regras específicas para classificação, impacto, urgência, prioridade e encerramento.

### Recomendação

Criar uma evolução chamada **Tipos de Chamado / Natureza do Chamado**, contemplando no mínimo:

- Incidente;
- Requisição;
- Mudança;
- Dúvida;
- Acesso;
- Tarefa operacional.

## 5. Gerenciamento de Requisições

### Situação atual

O Catálogo de Serviços já permite estruturar solicitações por serviço e abrir chamados a partir de um serviço publicado.

### Lacuna

Ainda falta separar formalmente requisições de incidentes e permitir formulários específicos por serviço.

### Recomendação

Evoluir o Catálogo de Serviços para suportar:

- tipo de solicitação vinculado ao serviço;
- formulário dinâmico;
- aprovação por serviço;
- SLA por serviço;
- grupo técnico responsável;
- regras de notificação.

## 6. Gerenciamento de Mudanças

### Situação atual

Não há módulo específico de Gerenciamento de Mudanças.

### Recomendação

Criar módulo futuro com:

- abertura de mudança;
- tipo de mudança: normal, emergencial, padrão;
- justificativa;
- risco;
- impacto;
- ativos/serviços afetados;
- janela de execução;
- plano de rollback;
- aprovadores;
- histórico;
- anexos;
- relatório pós-implementação.

## 7. CMDB e Análise de Impacto

### Situação atual

O SGX já possui módulo de Inventário/Ativos e vínculo entre ativo e chamado.

### Lacuna

CMDB exige mais do que inventário. É necessário modelar relações e dependências.

### Recomendação

Evoluir Inventário/Ativos para CMDB com:

- item de configuração (CI);
- relacionamento entre ativos;
- relacionamento entre ativo e serviço;
- dependência entre sistemas;
- criticidade;
- responsável técnico;
- impacto por indisponibilidade;
- histórico de alterações;
- mapa de impacto.

## 8. Segurança e MFA

### Situação atual

A arquitetura usa Microsoft Entra ID para autenticação e SGX para autorização interna.

### Recomendação

MFA deve ser controlado pelo Microsoft Entra ID, via Conditional Access, evitando implementar MFA próprio no SGX.

Pendências:

- validar App Registration real;
- validar tenant institucional;
- validar usuários reais;
- validar MFA;
- validar Conditional Access;
- registrar evidências de homologação.

## 9. Roadmap recomendado de aderência ITIL

### Prioridade alta

1. Formalizar tipo/natureza do chamado.
2. Homologar abertura por portal e e-mail.
3. Homologar SLA com usuários reais.
4. Homologar Catálogo de Serviços.
5. Criar grupos técnicos.
6. Criar observadores.
7. Criar regras de notificação persistentes.
8. Criar pesquisa de satisfação.

### Prioridade média

1. Criar regras de fechamento.
2. Criar formulários dinâmicos por serviço.
3. Evoluir Inventário para CMDB.
4. Criar análise de impacto.
5. Criar relatórios exportáveis.
6. Integrar com Zabbix.

### Prioridade futura

1. Gerenciamento de Mudanças.
2. Gerenciamento de Projetos.
3. Agente automático de inventário.
4. Automações e sugestões inteligentes.
5. Busca semântica na Base de Conhecimento.

## 10. Conclusão

O SGX já possui aderência parcial e relevante a práticas ITSM, especialmente nos pilares de chamados, SLA, catálogo, conhecimento, auditoria e ativos.

Para ser apresentado como produto ITSM mais maduro, as próximas evoluções devem priorizar:

- classificação formal de incidente/requisição/mudança;
- homologação institucional;
- evidências formais;
- grupos técnicos;
- observadores;
- notificações;
- satisfação;
- regras de fechamento;
- CMDB e análise de impacto.