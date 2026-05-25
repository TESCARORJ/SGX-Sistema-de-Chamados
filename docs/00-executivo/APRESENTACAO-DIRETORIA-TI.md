# SGX Sistema de Chamados - Apresentação para Diretoria de TI

## 1. Objetivo do documento

Este documento consolida uma visão executiva do SGX Sistema de Chamados para avaliação pela Diretoria de TI, com foco em transformar o MVP em um produto institucional de Service Desk / ITSM.

A proposta é apresentar:

- o problema que o sistema resolve;
- o que já foi implementado;
- o nível atual de maturidade;
- os riscos e pendências;
- o plano para homologação;
- o caminho para transformar o MVP em produto.

## 2. Contexto atual

Organizações que utilizam ferramentas de Service Desk precisam registrar, classificar, priorizar, acompanhar e medir chamados de forma rastreável.

O SGX Sistema de Chamados nasceu como uma solução própria para abertura, atendimento, acompanhamento, SLA, gestão administrativa e integração de chamados por e-mail.

O sistema evoluiu de um MVP para uma base funcional mais ampla, com módulos de atendimento, segurança, SLA, catálogo de serviços, base de conhecimento, inventário/ativos, auditoria, dashboard e gestão ITSM.

## 3. Proposta do SGX

O SGX propõe uma plataforma institucional de Service Desk com controle interno, integração corporativa e possibilidade de evolução gradual conforme a necessidade da organização.

A proposta central é:

- centralizar a abertura e o acompanhamento de chamados;
- permitir atendimento administrativo estruturado;
- organizar classificação, prioridade, departamento, categoria e localidade;
- controlar SLA e indicadores de gestão;
- permitir abertura por portal e por e-mail;
- manter histórico, comentários e anexos;
- oferecer base de conhecimento e catálogo de serviços;
- vincular chamados a ativos de inventário;
- manter auditoria de ações relevantes;
- autenticar com Microsoft Entra ID e autorizar internamente por perfis e permissões.

## 4. Status executivo do MVP

Status recomendado para comunicação executiva:

**MVP avançado / Produto em preparação para homologação institucional.**

O sistema já possui vários módulos implementados funcionalmente, mas ainda não deve ser comunicado como produto final pronto para produção sem:

- homologação institucional com usuários reais;
- evidências formais por módulo;
- validação de ambiente real;
- validação de integração Microsoft Entra ID real;
- validação de caixa IMAP real;
- hardening de segurança;
- plano de suporte, operação e monitoramento;
- definição de modelo de implantação.

## 5. Módulos já implementados ou em fase avançada

### Atendimento e chamados

- Portal do solicitante.
- Área administrativa.
- Abertura e acompanhamento de chamados.
- Comentários públicos e internos.
- Histórico e linha do tempo.
- Upload e download de anexos.
- Regras de visibilidade por perfil.

### Integração por e-mail

- Worker IMAP.
- Criação de chamado a partir de e-mail novo.
- Correlação de resposta por código do chamado e cabeçalhos de e-mail.
- Comentário público a partir de resposta por e-mail.
- Tratamento de anexos permitidos.
- Logs administrativos de processamento de e-mail.

### Segurança e identidade

- Autenticação com Microsoft Entra ID.
- Autorização interna por usuários, perfis e permissões.
- Perfis Administrador, Atendente e Solicitante.
- Permissões granulares por módulo e ação.
- Login local para cenários controlados.
- Bootstrap seguro de administrador inicial.

### SLA e gestão

- Políticas e metas de SLA.
- SLA aplicado ao chamado.
- Primeira resposta e resolução.
- Pausa e retomada.
- Alertas e eventos de SLA.
- Dashboard de SLA.
- Calendário corporativo e horário comercial.

### Governança

- Roadmap ITSM administrativo.
- Documentação ITSM dentro do sistema.
- Auditoria de eventos relevantes.
- Consulta administrativa de auditoria.
- Checklist por item de roadmap.
- Pendências técnicas e de homologação registradas.

### Conhecimento e serviços

- Base de Conhecimento.
- Publicação e consulta de artigos.
- Visibilidade por perfil.
- Vínculo de artigo com chamado.
- Catálogo de Serviços.
- Abertura de chamado orientada por serviço.
- Aplicação automática de classificação operacional pelo serviço.

### Infraestrutura / ativos

- Inventário de ativos.
- Tipos de ativos.
- Histórico e movimentação de ativos.
- Vínculo de ativo com chamado.
- Consulta de chamados por ativo.
- Frontend administrativo de inventário.

## 6. Pontos fortes para a Diretoria de TI

- Código e documentação versionados no GitHub.
- Arquitetura organizada em camadas.
- Stack moderna: .NET, Vue, Quasar, PostgreSQL e EF Core.
- Separação entre autenticação corporativa e autorização interna.
- Base funcional superior a um MVP simples.
- Evolução alinhada a práticas ITSM.
- Possibilidade de implantação controlada em ambiente institucional.
- Capacidade de evoluir para produto próprio, com menor dependência de ferramentas externas.

## 7. Pontos de atenção

- Ainda há pendências de homologação institucional.
- Algumas funcionalidades estão implementadas funcionalmente, mas sem aceite formal.
- A integração por e-mail precisa ser validada com caixa real.
- A autenticação Microsoft precisa ser validada no tenant real.
- O módulo de inventário ainda não é uma CMDB completa.
- O Gerenciamento de Mudanças ainda precisa de módulo específico.
- Observadores, grupos técnicos avançados, pesquisa de satisfação, regras de fechamento e integração com Zabbix devem entrar no roadmap de produto.
- É necessário formalizar ambiente de homologação, produção, backup, monitoramento e suporte.

## 8. Aderência ITSM / ITIL - resumo executivo

| Prática / Capacidade | Situação no SGX | Observação executiva |
|---|---|---|
| Gerenciamento de Incidentes | Parcialmente implementado | Chamados, SLA, histórico, atendimento e prioridade já existem; falta formalizar tipo de chamado como incidente. |
| Gerenciamento de Requisições | Parcialmente implementado | Catálogo de Serviços e abertura orientada por serviço já existem; falta classificação formal como requisição. |
| Gerenciamento de Mudanças | Planejado | Deve ser criado como módulo específico. |
| SLA | Implementado funcionalmente | Necessita homologação institucional e validação com dados reais. |
| Base de Conhecimento | Implementado funcionalmente | Necessita homologação e evidências reais. |
| Catálogo de Serviços | Implementado funcionalmente | Necessita homologação e formulários dinâmicos futuros. |
| Ativos / Inventário | Implementado funcionalmente | Base pronta para evoluir para CMDB. |
| CMDB | Parcial / futuro | Requer modelagem de relacionamentos, impacto e dependências. |
| Auditoria | Implementado funcionalmente | Boa base para governança e rastreabilidade. |
| Dashboard | Implementado funcionalmente | Necessita validação com massa real e refinamento visual. |
| MFA | Dependente do Entra ID | MFA deve ser governado por Microsoft Entra ID / Conditional Access. |

## 9. Roadmap executivo sugerido

### 30 dias

- Consolidar documentação executiva.
- Criar matriz de aderência ITIL.
- Preparar ambiente de homologação.
- Validar fluxo de chamados com usuários reais.
- Validar autenticação Microsoft Entra ID real.
- Validar abertura por e-mail com caixa institucional.
- Registrar evidências de tela dos módulos principais.

### 60 dias

- Homologar SLA, dashboard, catálogo de serviços e base de conhecimento.
- Ajustar pendências funcionais identificadas na homologação.
- Definir modelo de implantação.
- Definir plano de suporte e operação.
- Criar material de treinamento para administrador, atendente e solicitante.
- Definir escopo mínimo da primeira versão de produto.

### 90 dias

- Fechar aceite institucional da primeira versão.
- Preparar ambiente de produção.
- Executar hardening de segurança.
- Implantar monitoramento, logs, backup e rotina de sustentação.
- Definir roadmap pós-produção: mudanças, CMDB, Zabbix, satisfação, observadores, grupos técnicos e relatórios avançados.

## 10. Decisão esperada da Diretoria de TI

A decisão recomendada não é aprovar uso imediato em produção sem validação.

A decisão recomendada é:

1. Autorizar a criação de ambiente formal de homologação.
2. Autorizar validação com usuários reais.
3. Definir responsáveis de TI para homologação funcional.
4. Validar integrações corporativas: Microsoft Entra ID e e-mail.
5. Definir se o SGX seguirá como produto institucional, produto comercial ou solução interna sob governança da TI.

## 11. Recomendação final

O SGX deve ser tratado como um MVP avançado com potencial real de produto.

O próximo passo correto é executar uma homologação institucional controlada, com evidências, critérios de aceite e plano de implantação. Somente após essa etapa o sistema deve ser posicionado como produto pronto para produção.