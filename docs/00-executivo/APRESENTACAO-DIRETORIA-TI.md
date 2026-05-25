# SGX Sistema de Chamados - Apresentacao para Diretoria de TI

## 1. Objetivo do documento

Este documento consolida uma visao executiva do SGX Sistema de Chamados para avaliacao pela Diretoria de TI, com foco em transformar o MVP em um produto institucional de Service Desk / ITSM.

A proposta e apresentar:

- o problema que o sistema resolve;
- o que ja foi implementado;
- o nivel atual de maturidade;
- os riscos e pendencias;
- o plano para homologacao;
- o caminho para transformar o MVP em produto.

## 2. Contexto atual

Organizacoes que utilizam ferramentas de Service Desk precisam registrar, classificar, priorizar, acompanhar e medir chamados de forma rastreavel.

O SGX Sistema de Chamados nasceu como uma solucao propria para abertura, atendimento, acompanhamento, SLA, gestao administrativa e integracao de chamados por e-mail.

O sistema evoluiu de um MVP para uma base funcional mais ampla, com modulos de atendimento, seguranca, SLA, catalogo de servicos, base de conhecimento, inventario/ativos, auditoria, dashboard e gestao ITSM.

## 3. Proposta do SGX

O SGX propoe uma plataforma institucional de Service Desk com controle interno, integracao corporativa e possibilidade de evolucao gradual conforme a necessidade da organizacao.

A proposta central e:

- centralizar a abertura e acompanhamento de chamados;
- permitir atendimento administrativo estruturado;
- organizar classificacao, prioridade, departamento, categoria e localidade;
- controlar SLA e indicadores de gestao;
- permitir abertura por portal e por e-mail;
- manter historico, comentarios e anexos;
- oferecer base de conhecimento e catalogo de servicos;
- vincular chamados a ativos de inventario;
- manter auditoria de acoes relevantes;
- autenticar com Microsoft Entra ID e autorizar internamente por perfis e permissoes.

## 4. Status executivo do MVP

Status recomendado para comunicacao executiva:

**MVP avancado / Produto em preparacao para homologacao institucional.**

O sistema ja possui varios modulos implementados funcionalmente, mas ainda nao deve ser comunicado como produto final pronto para producao sem:

- homologacao institucional com usuarios reais;
- evidencias formais por modulo;
- validacao de ambiente real;
- validacao de integracao Microsoft Entra ID real;
- validacao de caixa IMAP real;
- hardening de seguranca;
- plano de suporte, operacao e monitoramento;
- definicao de modelo de implantacao.

## 5. Modulos ja implementados ou em fase avancada

### Atendimento e chamados

- Portal do solicitante.
- Area administrativa.
- Abertura e acompanhamento de chamados.
- Comentarios publicos e internos.
- Historico e linha do tempo.
- Upload e download de anexos.
- Regras de visibilidade por perfil.

### Integracao por e-mail

- Worker IMAP.
- Criacao de chamado a partir de e-mail novo.
- Correlacao de resposta por codigo do chamado e cabecalhos de e-mail.
- Comentario publico a partir de resposta por e-mail.
- Tratamento de anexos permitidos.
- Logs administrativos de processamento de e-mail.

### Seguranca e identidade

- Autenticacao com Microsoft Entra ID.
- Autorizacao interna por usuarios, perfis e permissoes.
- Perfis Administrador, Atendente e Solicitante.
- Permissoes granulares por modulo e acao.
- Login local para cenarios controlados.
- Bootstrap seguro de administrador inicial.

### SLA e gestao

- Politicas e metas de SLA.
- SLA aplicado ao chamado.
- Primeira resposta e resolucao.
- Pausa e retomada.
- Alertas e eventos de SLA.
- Dashboard de SLA.
- Calendario corporativo e horario comercial.

### Governanca

- Roadmap ITSM administrativo.
- Documentacao ITSM dentro do sistema.
- Auditoria de eventos relevantes.
- Consulta administrativa de auditoria.
- Checklist por item de roadmap.
- Pendencias tecnicas e de homologacao registradas.

### Conhecimento e servicos

- Base de Conhecimento.
- Publicacao e consulta de artigos.
- Visibilidade por perfil.
- Vinculo de artigo com chamado.
- Catalogo de Servicos.
- Abertura de chamado orientada por servico.
- Aplicacao automatica de classificacao operacional pelo servico.

### Infraestrutura / ativos

- Inventario de ativos.
- Tipos de ativos.
- Historico e movimentacao de ativos.
- Vinculo de ativo com chamado.
- Consulta de chamados por ativo.
- Frontend administrativo de inventario.

## 6. Pontos fortes para a Diretoria de TI

- Codigo e documentacao versionados no GitHub.
- Arquitetura organizada em camadas.
- Stack moderna: .NET, Vue, Quasar, PostgreSQL e EF Core.
- Separacao entre autenticacao corporativa e autorizacao interna.
- Base funcional superior a um MVP simples.
- Evolucao alinhada a praticas ITSM.
- Possibilidade de implantacao controlada em ambiente institucional.
- Capacidade de evoluir para produto proprio, com menor dependencia de ferramentas externas.

## 7. Pontos de atencao

- Ainda ha pendencias de homologacao institucional.
- Algumas funcionalidades estao implementadas funcionalmente, mas sem aceite formal.
- A integracao por e-mail precisa ser validada com caixa real.
- A autenticacao Microsoft precisa ser validada no tenant real.
- O modulo de inventario ainda nao e uma CMDB completa.
- Gerenciamento de mudancas ainda precisa de modulo especifico.
- Observadores, grupos tecnicos avancados, pesquisa de satisfacao, regras de fechamento e integracao com Zabbix devem entrar no roadmap de produto.
- E necessario formalizar ambiente de homologacao, producao, backup, monitoramento e suporte.

## 8. Aderencia ITSM / ITIL - resumo executivo

| Pratica / Capacidade | Situacao no SGX | Observacao executiva |
|---|---|---|
| Gerenciamento de Incidentes | Parcialmente implementado | Chamados, SLA, historico, atendimento e prioridade ja existem; falta formalizar tipo de chamado como incidente. |
| Gerenciamento de Requisicoes | Parcialmente implementado | Catalogo de Servicos e abertura orientada por servico ja existem; falta classificacao formal como requisicao. |
| Gerenciamento de Mudancas | Planejado | Deve ser criado como modulo especifico. |
| SLA | Implementado funcionalmente | Necessita homologacao institucional e validacao com dados reais. |
| Base de Conhecimento | Implementado funcionalmente | Necessita homologacao e evidencias reais. |
| Catalogo de Servicos | Implementado funcionalmente | Necessita homologacao e formularios dinamicos futuros. |
| Ativos / Inventario | Implementado funcionalmente | Base pronta para evoluir para CMDB. |
| CMDB | Parcial / futuro | Requer modelagem de relacionamentos, impacto e dependencias. |
| Auditoria | Implementado funcionalmente | Boa base para governanca e rastreabilidade. |
| Dashboard | Implementado funcionalmente | Necessita validacao com massa real e refinamento visual. |
| MFA | Dependente do Entra ID | MFA deve ser governado por Microsoft Entra ID / Conditional Access. |

## 9. Roadmap executivo sugerido

### 30 dias

- Consolidar documentacao executiva.
- Criar matriz de aderencia ITIL.
- Preparar ambiente de homologacao.
- Validar fluxo de chamados com usuarios reais.
- Validar autenticacao Microsoft Entra ID real.
- Validar abertura por e-mail com caixa institucional.
- Registrar evidencias de tela dos modulos principais.

### 60 dias

- Homologar SLA, dashboard, catalogo de servicos e base de conhecimento.
- Ajustar pendencias funcionais identificadas na homologacao.
- Definir modelo de implantacao.
- Definir plano de suporte e operacao.
- Criar material de treinamento para administrador, atendente e solicitante.
- Definir escopo minimo da primeira versao de produto.

### 90 dias

- Fechar aceite institucional da primeira versao.
- Preparar ambiente de producao.
- Executar hardening de seguranca.
- Implantar monitoramento, logs, backup e rotina de sustentacao.
- Definir roadmap pos-producao: mudancas, CMDB, Zabbix, satisfacao, observadores, grupos tecnicos e relatorios avancados.

## 10. Decisao esperada da Diretoria de TI

A decisao recomendada nao e aprovar uso imediato em producao sem validacao.

A decisao recomendada e:

1. Autorizar a criacao de ambiente formal de homologacao.
2. Autorizar validacao com usuarios reais.
3. Definir responsaveis de TI para homologacao funcional.
4. Validar integracoes corporativas: Microsoft Entra ID e e-mail.
5. Definir se o SGX seguira como produto institucional, produto comercial ou solucao interna sob governanca da TI.

## 11. Recomendacao final

O SGX deve ser tratado como um MVP avancado com potencial real de produto.

O proximo passo correto e executar uma homologacao institucional controlada, com evidencias, criterios de aceite e plano de implantacao. Somente apos essa etapa o sistema deve ser posicionado como produto pronto para producao.