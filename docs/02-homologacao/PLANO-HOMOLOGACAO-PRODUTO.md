# Plano de Homologação do Produto - SGX Sistema de Chamados

## 1. Objetivo

Este documento define o plano de homologação institucional do SGX Sistema de Chamados, com foco em validar se o MVP avançado está apto a evoluir para produto institucional ou comercial.

A homologação deve comprovar, com evidências formais, que os principais fluxos funcionais, técnicos e de segurança atendem aos critérios mínimos definidos para a primeira versão do produto.

## 2. Escopo da homologação

A homologação deve cobrir os seguintes blocos:

- autenticação e acesso;
- perfis e permissões;
- portal do solicitante;
- abertura de chamado;
- atendimento administrativo;
- comentários, anexos e histórico;
- abertura por e-mail;
- SLA;
- dashboard de gestão;
- cadastros administrativos;
- base de conhecimento;
- catálogo de serviços;
- inventário/ativos;
- auditoria;
- documentação ITSM;
- segurança básica;
- implantação em ambiente de homologação.

## 3. Fora do escopo da primeira homologação

Os itens abaixo podem permanecer como roadmap futuro, desde que documentados:

- Gerenciamento de Mudanças completo;
- CMDB completa;
- agente automático de inventário;
- integração Zabbix;
- pesquisa de satisfação;
- formulários dinâmicos avançados;
- grupos técnicos avançados;
- observadores;
- regras de fechamento configuráveis;
- relatórios executivos exportáveis completos;
- integração SIEM;
- automações com IA.

## 4. Ambientes

### Ambiente local de desenvolvimento

Usado para desenvolvimento, validação técnica e testes automatizados.

### Ambiente de homologação

Ambiente obrigatório para aceite institucional.

Deve conter:

- URL definida;
- banco PostgreSQL dedicado;
- API publicada;
- frontend publicado;
- Worker de e-mail configurado;
- logs acessíveis;
- backup básico definido;
- usuários reais ou massa representativa;
- configuração Microsoft Entra ID real ou ambiente controlado equivalente.

### Ambiente de produção

Somente deve ser preparado após homologação aprovada ou aprovada com ressalvas controladas.

## 5. Perfis envolvidos

A homologação deve envolver pelo menos:

| Perfil | Objetivo da validação |
|---|---|
| Administrador | Validar configurações, cadastros, permissões, SLA, catálogo, conhecimento, inventário, auditoria e dashboard. |
| Atendente | Validar triagem, atendimento, comentários, anexos, status, SLA, base de conhecimento e vínculo de ativos. |
| Solicitante | Validar abertura, acompanhamento, comentários públicos, anexos permitidos, catálogo e base de conhecimento. |
| Gestor de TI | Validar dashboard, indicadores, governança e aderência ao processo. |
| Responsável por infraestrutura / identidade | Validar Microsoft Entra ID, e-mail, ambiente, logs e segurança. |

## 6. Critérios gerais de aceite

A homologação será considerada aprovada quando:

- os fluxos principais forem executados com sucesso;
- as regras de perfil e permissão forem respeitadas;
- os dados exibidos estiverem coerentes com o banco;
- SLA for calculado e exibido corretamente nos cenários principais;
- abertura por e-mail funcionar em caixa real ou ambiente equivalente validado;
- autenticação Microsoft for validada no tenant definido;
- evidências forem registradas;
- pendências críticas forem corrigidas ou formalmente aceitas como ressalvas;
- houver registro de responsável, data, ambiente e resultado.

## 7. Classificação de resultado

| Resultado | Significado |
|---|---|
| Aprovado | Funcionalidade validada sem pendências impeditivas. |
| Aprovado com ressalvas | Funcionalidade atende ao uso principal, mas possui pendências controladas. |
| Reprovado | Funcionalidade possui falha impeditiva ou risco não aceito. |
| Não testado | Cenário não foi executado. |

## 8. Evidências obrigatórias

Para cada módulo homologado, registrar:

- nome do módulo;
- data da validação;
- responsável pela validação;
- ambiente utilizado;
- usuário/perfil utilizado;
- roteiro executado;
- resultado;
- prints das telas principais;
- observações;
- pendências;
- decisão final.

Sugestão de pasta:

```txt
docs/02-homologacao/evidencias/
  autenticacao/
  chamados/
  email/
  sla/
  dashboard/
  cadastros/
  base-conhecimento/
  catalogo-servicos/
  inventario-ativos/
  auditoria/
```

## 9. Roteiro de homologação por módulo

### 9.1 Autenticação e acesso

| Cenário | Perfil | Resultado |
|---|---|---|
| Login com Microsoft Entra ID | Todos | Pendente |
| Validação de MFA/Conditional Access | Todos | Pendente |
| Bloqueio de usuário não autorizado | Usuário externo | Pendente |
| GET /api/me retorna perfis e permissões | Todos | Pendente |
| Logout e expiração de sessão | Todos | Pendente |
| Login local controlado, quando aplicável | Administrador | Pendente |

### 9.2 Perfis e permissões

| Cenário | Perfil | Resultado |
|---|---|---|
| Administrador acessa área administrativa | Administrador | Pendente |
| Solicitante não acessa área administrativa | Solicitante | Pendente |
| Atendente acessa apenas recursos permitidos | Atendente | Pendente |
| Botão/ação sem permissão não aparece ou é bloqueado | Todos | Pendente |
| Backend retorna 403 quando permissão ausente | Todos | Pendente |

### 9.3 Portal do solicitante

| Cenário | Resultado |
|---|---|
| Abrir chamado pelo portal | Pendente |
| Anexar arquivo permitido | Pendente |
| Bloquear arquivo inválido | Pendente |
| Listar chamados do solicitante | Pendente |
| Abrir detalhe do chamado | Pendente |
| Enviar comentário público | Pendente |
| Visualizar histórico público | Pendente |

### 9.4 Atendimento administrativo

| Cenário | Resultado |
|---|---|
| Listar fila de chamados | Pendente |
| Filtrar por status, prioridade, categoria e responsável | Pendente |
| Assumir chamado | Pendente |
| Alterar status | Pendente |
| Alterar prioridade/categoria quando permitido | Pendente |
| Enviar comentário interno | Pendente |
| Enviar comentário público | Pendente |
| Anexar arquivo | Pendente |
| Resolver ou encerrar chamado | Pendente |
| Reabrir chamado, quando aplicável | Pendente |

### 9.5 Abertura por e-mail

| Cenário | Resultado |
|---|---|
| E-mail novo cria chamado | Pendente |
| E-mail duplicado não cria chamado duplicado | Pendente |
| Resposta por e-mail adiciona comentário | Pendente |
| Anexo permitido por e-mail é salvo | Pendente |
| Anexo inválido é rejeitado/logado | Pendente |
| Log administrativo de e-mail é exibido | Pendente |
| Falha de processamento é registrada | Pendente |

### 9.6 SLA

| Cenário | Resultado |
|---|---|
| SLA aplicado na abertura | Pendente |
| Prazo de primeira resposta calculado | Pendente |
| Prazo de resolução calculado | Pendente |
| Primeira resposta registrada | Pendente |
| Resolução registrada | Pendente |
| Pausa e retomada funcionando | Pendente |
| Indicador de vencido/próximo do vencimento correto | Pendente |
| Calendário corporativo aplicado | Pendente |

### 9.7 Dashboard

| Cenário | Resultado |
|---|---|
| Cards principais carregam corretamente | Pendente |
| Filtro por período altera indicadores | Pendente |
| Chamados por status coerentes | Pendente |
| Chamados por prioridade coerentes | Pendente |
| SLA vencido/próximo coerente | Pendente |
| Produtividade por atendente coerente | Pendente |
| Resumo da integração por e-mail coerente | Pendente |

### 9.8 Cadastros administrativos

| Cenário | Resultado |
|---|---|
| Criar departamento | Pendente |
| Criar categoria | Pendente |
| Criar subcategoria vinculada à categoria | Pendente |
| Criar prioridade | Pendente |
| Criar tipo de solicitação | Pendente |
| Criar local/unidade | Pendente |
| Inativar cadastro sem excluir histórico | Pendente |
| Bloquear uso de cadastro inativo em nova operação | Pendente |

### 9.9 Base de Conhecimento

| Cenário | Resultado |
|---|---|
| Criar artigo | Pendente |
| Publicar artigo | Pendente |
| Consultar artigo no portal | Pendente |
| Bloquear artigo não publicado | Pendente |
| Vincular artigo ao chamado | Pendente |
| Remover vínculo do artigo | Pendente |
| Validar visibilidade por perfil | Pendente |

### 9.10 Catálogo de Serviços

| Cenário | Resultado |
|---|---|
| Criar serviço | Pendente |
| Publicar serviço | Pendente |
| Consultar serviço no portal | Pendente |
| Abrir chamado a partir do serviço | Pendente |
| Aplicar categoria/subcategoria/prioridade/SLA pelo backend | Pendente |
| Bloquear serviço arquivado/inativo | Pendente |

### 9.11 Inventário / Ativos

| Cenário | Resultado |
|---|---|
| Criar ativo | Pendente |
| Editar ativo | Pendente |
| Inativar e reativar ativo | Pendente |
| Movimentar ativo | Pendente |
| Visualizar histórico do ativo | Pendente |
| Vincular ativo ao chamado | Pendente |
| Remover vínculo do ativo | Pendente |
| Consultar chamados relacionados ao ativo | Pendente |

### 9.12 Auditoria

| Cenário | Resultado |
|---|---|
| Ações relevantes geram evento de auditoria | Pendente |
| Consulta administrativa lista eventos | Pendente |
| Filtros funcionam | Pendente |
| Detalhe mostra antes/depois quando aplicável | Pendente |
| Usuário, IP e User-Agent são registrados quando disponíveis | Pendente |

## 10. Registro consolidado de homologação

| Módulo | Responsável | Data | Ambiente | Resultado | Observações |
|---|---|---|---|---|---|
| Autenticação |  |  |  | Não testado |  |
| Perfis e permissões |  |  |  | Não testado |  |
| Portal |  |  |  | Não testado |  |
| Atendimento |  |  |  | Não testado |  |
| E-mail |  |  |  | Não testado |  |
| SLA |  |  |  | Não testado |  |
| Dashboard |  |  |  | Não testado |  |
| Cadastros |  |  |  | Não testado |  |
| Base de Conhecimento |  |  |  | Não testado |  |
| Catálogo de Serviços |  |  |  | Não testado |  |
| Inventário/Ativos |  |  |  | Não testado |  |
| Auditoria |  |  |  | Não testado |  |

## 11. Critério para avançar para produção

O SGX somente deve avançar para produção quando:

- os módulos essenciais estiverem aprovados ou aprovados com ressalvas controladas;
- não houver falha crítica de segurança;
- autenticação e autorização estiverem validadas;
- e-mail e SLA estiverem testados;
- backup e logs estiverem definidos;
- responsáveis de suporte estiverem nomeados;
- plano de rollback estiver definido;
- a diretoria ou gestor responsável aprovar formalmente.

## 12. Conclusão

A homologação deve ser tratada como etapa obrigatória entre MVP avançado e produto.

O objetivo não é apenas testar telas, mas comprovar que o SGX atende aos fluxos principais, respeita segurança, gera rastreabilidade e pode ser sustentado em ambiente institucional.