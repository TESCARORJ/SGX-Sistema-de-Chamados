# Matriz de Aderencia ITIL / ITSM - SGX Sistema de Chamados

## 1. Objetivo

Este documento registra a aderencia atual do SGX Sistema de Chamados a praticas ITIL / ITSM, separando o que ja esta implementado, o que esta parcialmente atendido e o que deve entrar no roadmap de produto.

A matriz deve ser usada para:

- apresentacao executiva;
- avaliacao tecnica por especialistas ITSM;
- definicao de roadmap;
- priorizacao de evolucoes;
- homologacao institucional.

## 2. Classificacao usada

| Status | Significado |
|---|---|
| Implementado funcionalmente | Ja existe base funcional no sistema, mas pode depender de homologacao final. |
| Parcialmente implementado | Existe parte da capacidade, mas falta formalizacao ou complemento funcional. |
| Planejado | Ainda nao implementado, mas recomendado para o roadmap. |
| Dependente de ambiente | Depende de configuracao externa, infraestrutura ou validacao institucional. |
| Fora do escopo atual | Nao faz parte do MVP atual. |

## 3. Matriz resumida

| Pratica / Recurso | Status SGX | Evidencia atual | Pendencia / Proxima acao |
|---|---|---|---|
| Gerenciamento de Incidentes | Parcialmente implementado | Chamados, prioridade, SLA, historico, comentarios, anexos, atendimento administrativo | Formalizar tipo de chamado como Incidente e regras especificas de tratamento. |
| Gerenciamento de Requisicoes | Parcialmente implementado | Catalogo de Servicos, abertura orientada por servico, portal do solicitante | Formalizar tipo de chamado como Requisicao e separar fluxo de requisicao x incidente. |
| Gerenciamento de Mudancas | Planejado | Roadmap ITSM registra evolucoes futuras | Criar modulo de mudancas com aprovacao, impacto, risco, janela, plano de rollback e historico. |
| Catalogo de Servicos | Implementado funcionalmente | Modulo de Catalogo de Servicos, consulta no portal, abertura por servico | Homologar com usuarios reais e evoluir formularios dinamicos por servico. |
| Base de Conhecimento | Implementado funcionalmente | Modulo de artigos, publicacao, consulta no portal e vinculo ao chamado | Homologar, criar evidencias e evoluir workflow editorial. |
| SLA | Implementado funcionalmente | Politicas, metas, aplicacao no chamado, alertas, dashboard e calendario corporativo | Homologar com dados reais, validar feriados, horarios e regras por area. |
| Inventario / Ativos | Implementado funcionalmente | Cadastro de ativos, historico, movimentacao e vinculo com chamados | Homologar, evoluir importacao, exportacao, QR Code, manutencao e integracao patrimonial. |
| CMDB | Parcial / futuro | Inventario/Ativos e vinculo com chamados ja existem | Evoluir para relacionamentos entre ativos, servicos, dependencias e analise de impacto. |
| Analise de Impacto | Planejado | Base de ativos e catalogo permitem evolucao | Criar modelo de dependencia entre servicos, ativos, areas e chamados. |
| Abertura por Portal | Implementado funcionalmente | Portal do solicitante e criacao de chamados | Homologar com usuario real e registrar evidencias. |
| Abertura por E-mail | Implementado funcionalmente | Worker IMAP, criacao de chamado, correlacao e logs administrativos | Validar caixa IMAP real, OAuth Microsoft se necessario, retry/backoff e monitoramento. |
| Comentarios e Anexos | Implementado funcionalmente | Comentarios publicos/internos, upload/listagem/download de anexos | Homologar e manter regras de seguranca de anexos. |
| Historico / Linha do Tempo | Implementado funcionalmente | Consolidacao de eventos, comentarios e anexos | Homologar com usuarios reais. |
| Auditoria | Implementado funcionalmente | Eventos de auditoria, filtros, detalhe e indicadores | Evoluir exportacao, retencao, alertas e integracao SIEM. |
| Perfis e Permissoes | Implementado funcionalmente | Administrador, Atendente, Solicitante e permissoes granulares | Homologar matriz de permissoes com usuarios-chave. |
| MFA | Dependente de ambiente | Arquitetura baseada em Microsoft Entra ID | Validar MFA e Conditional Access no tenant institucional. |
| Observador de Chamado | Planejado | Nao consolidado no MVP atual | Criar modelo de observadores, notificacoes e permissao de visualizacao. |
| Grupo Tecnico | Planejado / Parcial | Perfis e usuarios existem | Criar grupos tecnicos, filas por grupo, atribuicao e regras de visibilidade. |
| Regras de Notificacao | Parcialmente implementado | Central frontend/local e logs existem | Criar API persistente de notificacoes e regras configuraveis. |
| Pesquisa de Satisfacao | Planejado | Nao consolidado no MVP atual | Criar pesquisa apos fechamento e indicadores de satisfacao. |
| Regra de Fechamento | Planejado | Resolucao/encerramento existem no fluxo de chamado | Criar regras configuraveis de fechamento, aceite do solicitante e encerramento automatico. |
| Formularios Dinamicos | Planejado | Catalogo de Servicos permite evolucao | Criar campos dinamicos por servico, obrigatoriedade e validacoes. |
| Integracao Zabbix | Planejado | Nao consolidado no MVP atual | Criar integracao para abertura/atualizacao automatica de incidentes. |
| Gerenciamento de Projetos | Planejado | Fora do core atual de chamados | Avaliar se sera modulo proprio ou integracao com ferramenta externa. |
| Relatorios Exportaveis | Em evolucao | Dashboard e consultas estruturadas existem | Criar exportacao Excel/PDF e filtros gerenciais. |
| Dashboards Gerenciais | Implementado funcionalmente | Dashboard administrativo e indicadores | Homologar com massa real e refinar layout para diretoria. |

## 4. Gerenciamento de Incidentes

### Situacao atual

O SGX ja possui a base essencial para tratamento de incidentes:

- abertura de chamado;
- prioridade;
- categoria;
- subcategoria;
- departamento;
- SLA;
- comentarios;
- anexos;
- historico;
- auditoria;
- atendimento administrativo;
- dashboard.

### Lacuna

Ainda falta formalizar o tipo de chamado como Incidente e criar regras especificas para classificacao, impacto, urgencia, prioridade e encerramento.

### Recomendacao

Criar uma evolucao chamada **Tipos de Chamado / Natureza do Chamado**, contemplando no minimo:

- Incidente;
- Requisicao;
- Mudanca;
- Duvida;
- Acesso;
- Tarefa operacional.

## 5. Gerenciamento de Requisicoes

### Situacao atual

O Catalogo de Servicos ja permite estruturar solicitacoes por servico e abrir chamados a partir de um servico publicado.

### Lacuna

Ainda falta separar formalmente requisicoes de incidentes e permitir formularios especificos por servico.

### Recomendacao

Evoluir o Catalogo de Servicos para suportar:

- tipo de solicitacao vinculado ao servico;
- formulario dinamico;
- aprovacao por servico;
- SLA por servico;
- grupo tecnico responsavel;
- regras de notificacao.

## 6. Gerenciamento de Mudancas

### Situacao atual

Nao ha modulo especifico de Gerenciamento de Mudancas.

### Recomendacao

Criar modulo futuro com:

- abertura de mudanca;
- tipo de mudanca: normal, emergencial, padrao;
- justificativa;
- risco;
- impacto;
- ativos/servicos afetados;
- janela de execucao;
- plano de rollback;
- aprovadores;
- historico;
- anexos;
- relatorio pos-implementacao.

## 7. CMDB e Analise de Impacto

### Situacao atual

O SGX ja possui modulo de Inventario/Ativos e vinculo entre ativo e chamado.

### Lacuna

CMDB exige mais do que inventario. E necessario modelar relacoes e dependencias.

### Recomendacao

Evoluir Inventario/Ativos para CMDB com:

- item de configuracao (CI);
- relacionamento entre ativos;
- relacionamento entre ativo e servico;
- dependencia entre sistemas;
- criticidade;
- responsavel tecnico;
- impacto por indisponibilidade;
- historico de alteracoes;
- mapa de impacto.

## 8. Seguranca e MFA

### Situacao atual

A arquitetura usa Microsoft Entra ID para autenticacao e SGX para autorizacao interna.

### Recomendacao

MFA deve ser controlado pelo Microsoft Entra ID, via Conditional Access, evitando implementar MFA proprio no SGX.

Pendencias:

- validar App Registration real;
- validar tenant institucional;
- validar usuarios reais;
- validar MFA;
- validar Conditional Access;
- registrar evidencias de homologacao.

## 9. Roadmap recomendado de aderencia ITIL

### Prioridade alta

1. Formalizar tipo/natureza do chamado.
2. Homologar abertura por portal e e-mail.
3. Homologar SLA com usuarios reais.
4. Homologar Catalogo de Servicos.
5. Criar grupos tecnicos.
6. Criar observadores.
7. Criar regras de notificacao persistentes.
8. Criar pesquisa de satisfacao.

### Prioridade media

1. Criar regras de fechamento.
2. Criar formularios dinamicos por servico.
3. Evoluir Inventario para CMDB.
4. Criar analise de impacto.
5. Criar relatorios exportaveis.
6. Integrar com Zabbix.

### Prioridade futura

1. Gerenciamento de Mudancas.
2. Gerenciamento de Projetos.
3. Agente automatico de inventario.
4. Automacoes e sugestoes inteligentes.
5. Busca semantica na Base de Conhecimento.

## 10. Conclusao

O SGX ja possui aderencia parcial e relevante a praticas ITSM, especialmente nos pilares de chamados, SLA, catalogo, conhecimento, auditoria e ativos.

Para ser apresentado como produto ITSM mais maduro, as proximas evolucoes devem priorizar:

- classificacao formal de incidente/requisicao/mudanca;
- homologacao institucional;
- evidencias formais;
- grupos tecnicos;
- observadores;
- notificacoes;
- satisfacao;
- regras de fechamento;
- CMDB e analise de impacto.