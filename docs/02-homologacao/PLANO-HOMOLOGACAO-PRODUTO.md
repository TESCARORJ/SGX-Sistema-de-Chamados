# Plano de Homologacao do Produto - SGX Sistema de Chamados

## 1. Objetivo

Este documento define o plano de homologacao institucional do SGX Sistema de Chamados, com foco em validar se o MVP avancado esta apto a evoluir para produto institucional ou comercial.

A homologacao deve comprovar, com evidencias formais, que os principais fluxos funcionais, tecnicos e de seguranca atendem aos criterios minimos definidos para a primeira versao do produto.

## 2. Escopo da homologacao

A homologacao deve cobrir os seguintes blocos:

- autenticacao e acesso;
- perfis e permissoes;
- portal do solicitante;
- abertura de chamado;
- atendimento administrativo;
- comentarios, anexos e historico;
- abertura por e-mail;
- SLA;
- dashboard de gestao;
- cadastros administrativos;
- base de conhecimento;
- catalogo de servicos;
- inventario/ativos;
- auditoria;
- documentacao ITSM;
- seguranca basica;
- implantacao em ambiente de homologacao.

## 3. Fora do escopo da primeira homologacao

Os itens abaixo podem permanecer como roadmap futuro, desde que documentados:

- Gerenciamento de Mudancas completo;
- CMDB completa;
- agente automatico de inventario;
- integracao Zabbix;
- pesquisa de satisfacao;
- formularios dinamicos avancados;
- grupos tecnicos avancados;
- observadores;
- regras de fechamento configuraveis;
- relatorios executivos exportaveis completos;
- integracao SIEM;
- automacoes com IA.

## 4. Ambientes

### Ambiente local de desenvolvimento

Usado para desenvolvimento, validacao tecnica e testes automatizados.

### Ambiente de homologacao

Ambiente obrigatorio para aceite institucional.

Deve conter:

- URL definida;
- banco PostgreSQL dedicado;
- API publicada;
- frontend publicado;
- Worker de e-mail configurado;
- logs acessiveis;
- backup basico definido;
- usuarios reais ou massa representativa;
- configuracao Microsoft Entra ID real ou ambiente controlado equivalente.

### Ambiente de producao

Somente deve ser preparado apos homologacao aprovada ou aprovada com ressalvas controladas.

## 5. Perfis envolvidos

A homologacao deve envolver pelo menos:

| Perfil | Objetivo da validacao |
|---|---|
| Administrador | Validar configuracoes, cadastros, permissoes, SLA, catalogo, conhecimento, inventario, auditoria e dashboard. |
| Atendente | Validar triagem, atendimento, comentarios, anexos, status, SLA, base de conhecimento e vinculo de ativos. |
| Solicitante | Validar abertura, acompanhamento, comentarios publicos, anexos permitidos, catalogo e base de conhecimento. |
| Gestor de TI | Validar dashboard, indicadores, governanca e aderencia ao processo. |
| Responsavel por infraestrutura / identidade | Validar Microsoft Entra ID, e-mail, ambiente, logs e seguranca. |

## 6. Criterios gerais de aceite

A homologacao sera considerada aprovada quando:

- os fluxos principais forem executados com sucesso;
- as regras de perfil e permissao forem respeitadas;
- os dados exibidos estiverem coerentes com o banco;
- SLA for calculado e exibido corretamente nos cenarios principais;
- abertura por e-mail funcionar em caixa real ou ambiente equivalente validado;
- autenticacao Microsoft for validada no tenant definido;
- evidencias forem registradas;
- pendencias criticas forem corrigidas ou formalmente aceitas como ressalvas;
- houver registro de responsavel, data, ambiente e resultado.

## 7. Classificacao de resultado

| Resultado | Significado |
|---|---|
| Aprovado | Funcionalidade validada sem pendencias impeditivas. |
| Aprovado com ressalvas | Funcionalidade atende ao uso principal, mas possui pendencias controladas. |
| Reprovado | Funcionalidade possui falha impeditiva ou risco nao aceito. |
| Nao testado | Cenario nao foi executado. |

## 8. Evidencias obrigatorias

Para cada modulo homologado, registrar:

- nome do modulo;
- data da validacao;
- responsavel pela validacao;
- ambiente utilizado;
- usuario/perfil utilizado;
- roteiro executado;
- resultado;
- prints das telas principais;
- observacoes;
- pendencias;
- decisao final.

Sugestao de pasta:

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

## 9. Roteiro de homologacao por modulo

### 9.1 Autenticacao e acesso

| Cenario | Perfil | Resultado |
|---|---|---|
| Login com Microsoft Entra ID | Todos | Pendente |
| Validacao de MFA/Conditional Access | Todos | Pendente |
| Bloqueio de usuario nao autorizado | Usuario externo | Pendente |
| GET /api/me retorna perfis e permissoes | Todos | Pendente |
| Logout e expiracao de sessao | Todos | Pendente |
| Login local controlado, quando aplicavel | Administrador | Pendente |

### 9.2 Perfis e permissoes

| Cenario | Perfil | Resultado |
|---|---|---|
| Administrador acessa area administrativa | Administrador | Pendente |
| Solicitante nao acessa area administrativa | Solicitante | Pendente |
| Atendente acessa apenas recursos permitidos | Atendente | Pendente |
| Botao/acao sem permissao nao aparece ou e bloqueado | Todos | Pendente |
| Backend retorna 403 quando permissao ausente | Todos | Pendente |

### 9.3 Portal do solicitante

| Cenario | Resultado |
|---|---|
| Abrir chamado pelo portal | Pendente |
| Anexar arquivo permitido | Pendente |
| Bloquear arquivo invalido | Pendente |
| Listar chamados do solicitante | Pendente |
| Abrir detalhe do chamado | Pendente |
| Enviar comentario publico | Pendente |
| Visualizar historico publico | Pendente |

### 9.4 Atendimento administrativo

| Cenario | Resultado |
|---|---|
| Listar fila de chamados | Pendente |
| Filtrar por status, prioridade, categoria e responsavel | Pendente |
| Assumir chamado | Pendente |
| Alterar status | Pendente |
| Alterar prioridade/categoria quando permitido | Pendente |
| Enviar comentario interno | Pendente |
| Enviar comentario publico | Pendente |
| Anexar arquivo | Pendente |
| Resolver ou encerrar chamado | Pendente |
| Reabrir chamado, quando aplicavel | Pendente |

### 9.5 Abertura por e-mail

| Cenario | Resultado |
|---|---|
| E-mail novo cria chamado | Pendente |
| E-mail duplicado nao cria chamado duplicado | Pendente |
| Resposta por e-mail adiciona comentario | Pendente |
| Anexo permitido por e-mail e salvo | Pendente |
| Anexo invalido e rejeitado/logado | Pendente |
| Log administrativo de e-mail e exibido | Pendente |
| Falha de processamento e registrada | Pendente |

### 9.6 SLA

| Cenario | Resultado |
|---|---|
| SLA aplicado na abertura | Pendente |
| Prazo de primeira resposta calculado | Pendente |
| Prazo de resolucao calculado | Pendente |
| Primeira resposta registrada | Pendente |
| Resolucao registrada | Pendente |
| Pausa e retomada funcionando | Pendente |
| Indicador de vencido/proximo do vencimento correto | Pendente |
| Calendario corporativo aplicado | Pendente |

### 9.7 Dashboard

| Cenario | Resultado |
|---|---|
| Cards principais carregam corretamente | Pendente |
| Filtro por periodo altera indicadores | Pendente |
| Chamados por status coerentes | Pendente |
| Chamados por prioridade coerentes | Pendente |
| SLA vencido/proximo coerente | Pendente |
| Produtividade por atendente coerente | Pendente |
| Resumo da integracao por e-mail coerente | Pendente |

### 9.8 Cadastros administrativos

| Cenario | Resultado |
|---|---|
| Criar departamento | Pendente |
| Criar categoria | Pendente |
| Criar subcategoria vinculada a categoria | Pendente |
| Criar prioridade | Pendente |
| Criar tipo de solicitacao | Pendente |
| Criar local/unidade | Pendente |
| Inativar cadastro sem excluir historico | Pendente |
| Bloquear uso de cadastro inativo em nova operacao | Pendente |

### 9.9 Base de Conhecimento

| Cenario | Resultado |
|---|---|
| Criar artigo | Pendente |
| Publicar artigo | Pendente |
| Consultar artigo no portal | Pendente |
| Bloquear artigo nao publicado | Pendente |
| Vincular artigo ao chamado | Pendente |
| Remover vinculo do artigo | Pendente |
| Validar visibilidade por perfil | Pendente |

### 9.10 Catalogo de Servicos

| Cenario | Resultado |
|---|---|
| Criar servico | Pendente |
| Publicar servico | Pendente |
| Consultar servico no portal | Pendente |
| Abrir chamado a partir do servico | Pendente |
| Aplicar categoria/subcategoria/prioridade/SLA pelo backend | Pendente |
| Bloquear servico arquivado/inativo | Pendente |

### 9.11 Inventario / Ativos

| Cenario | Resultado |
|---|---|
| Criar ativo | Pendente |
| Editar ativo | Pendente |
| Inativar e reativar ativo | Pendente |
| Movimentar ativo | Pendente |
| Visualizar historico do ativo | Pendente |
| Vincular ativo ao chamado | Pendente |
| Remover vinculo do ativo | Pendente |
| Consultar chamados relacionados ao ativo | Pendente |

### 9.12 Auditoria

| Cenario | Resultado |
|---|---|
| Acoes relevantes geram evento de auditoria | Pendente |
| Consulta administrativa lista eventos | Pendente |
| Filtros funcionam | Pendente |
| Detalhe mostra antes/depois quando aplicavel | Pendente |
| Usuario, IP e User-Agent sao registrados quando disponiveis | Pendente |

## 10. Registro consolidado de homologacao

| Modulo | Responsavel | Data | Ambiente | Resultado | Observacoes |
|---|---|---|---|---|---|
| Autenticacao |  |  |  | Nao testado |  |
| Perfis e permissoes |  |  |  | Nao testado |  |
| Portal |  |  |  | Nao testado |  |
| Atendimento |  |  |  | Nao testado |  |
| E-mail |  |  |  | Nao testado |  |
| SLA |  |  |  | Nao testado |  |
| Dashboard |  |  |  | Nao testado |  |
| Cadastros |  |  |  | Nao testado |  |
| Base de Conhecimento |  |  |  | Nao testado |  |
| Catalogo de Servicos |  |  |  | Nao testado |  |
| Inventario/Ativos |  |  |  | Nao testado |  |
| Auditoria |  |  |  | Nao testado |  |

## 11. Criterio para avancar para producao

O SGX somente deve avancar para producao quando:

- os modulos essenciais estiverem aprovados ou aprovados com ressalvas controladas;
- nao houver falha critica de seguranca;
- autenticacao e autorizacao estiverem validadas;
- e-mail e SLA estiverem testados;
- backup e logs estiverem definidos;
- responsaveis de suporte estiverem nomeados;
- plano de rollback estiver definido;
- a diretoria ou gestor responsavel aprovar formalmente.

## 12. Conclusao

A homologacao deve ser tratada como etapa obrigatoria entre MVP avancado e produto.

O objetivo nao e apenas testar telas, mas comprovar que o SGX atende aos fluxos principais, respeita seguranca, gera rastreabilidade e pode ser sustentado em ambiente institucional.