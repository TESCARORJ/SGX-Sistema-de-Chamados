# Roadmap SGX Sistema de Chamados

## Visao geral

O SGX Sistema de Chamados e uma solucao institucional para abertura, acompanhamento, atendimento, SLA, gestao administrativa e integracao de chamados por e-mail.

Diretriz de seguranca:
- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissoes.

## Entregue

- Migracao para .NET
- PostgreSQL + EF Core
- Estrutura Domain, Application, Infrastructure, Api, Worker.Email e Web
- Autenticacao Microsoft Entra ID
- Login local Development
- Autorizacao interna por perfis
- Perfis Administrador, Atendente e Solicitante
- Portal do solicitante
- Area administrativa
- Abertura e acompanhamento de chamados
- Comentarios, historico e anexos
- Cadastros administrativos
- SLA e dashboard
- Worker IMAP
- Logs de integracao de e-mail
- Frontend Vue 3 + Quasar
- Central de notificacoes frontend/local
- Perfis e permissoes granulares
- Matriz de permissoes por perfil
- GET /api/me com permissoes efetivas
- Controle visual por permissao

## Em evolucao

- Historico/Auditoria (Governanca) implementado funcionalmente - Sprints 1, 2 e 3 consolidadas com checklist 63/63
- API real de notificacoes
- Relatorios exportaveis
- Testes automatizados frontend/e2e
- Homologacao com usuarios reais
- Integracao Microsoft Entra ID real em ambiente institucional
- Validacao IMAP real
- Deploy em VPS/Docker Compose
- Hardening final de producao
- Observabilidade, logs e monitoramento

## Proximas etapas recomendadas

1. Validar matriz de permissoes com usuarios-chave.
2. Validar fluxo real de abertura/atendimento de chamados.
3. Validar integracao Microsoft Entra ID real.
4. Validar leitura IMAP real.
5. Criar API persistente de notificacoes.
6. Criar auditoria detalhada de alteracao de permissoes.
7. Criar relatorios exportaveis.
8. Preparar ambiente de homologacao.
9. Preparar deploy em VPS.
10. Executar checklist final de seguranca.

## Pontos para reuniao

- Azure/Microsoft Entra ID autentica; SGX autoriza.
- Perfis atuais: Administrador, Atendente e Solicitante.
- Evolucao implementada: permissoes granulares configuraveis por modulo e acao.
- Beneficio: ajustar acessos sem alteracao de codigo.
- Beneficio: seguranca e rastreabilidade.
- Beneficio: aderencia a operacao institucional.
- Beneficio: possibilidade de criar perfis derivados no futuro, como Atendente N1, Atendente N2, Supervisor e Auditor.
- Beneficio: separacao clara entre autenticacao corporativa e regra interna do sistema.

## Status do item Perfis de Acesso

| Item | Status | Observacao |
|---|---|---|
| Perfis macro | Concluido | Administrador, Atendente e Solicitante |
| CRUD de perfis | Concluido | Area administrativa |
| Associacao usuario-perfil | Concluido | Usuario pode ter perfis internos |
| Permissoes granulares | Concluido | Catalogo por modulo e acao |
| Matriz de permissoes | Concluido | Perfil tem permissoes configuraveis |
| GET /api/me com permissoes | Concluido | Frontend recebe permissoes efetivas |
| Policies por permissao | Concluido | Backend suporta autorizacao granular |
| Controle visual por permissao | Concluido | Acoes sao exibidas conforme permissao |
| Auditoria detalhada | Implementado funcionalmente | Sprints 1, 2 e 3 consolidadas com consulta administrativa e checklist 63/63 |

## Sprint Historico/Auditoria 1 - Base tecnica

Area: Historico/Auditoria
Categoria: Governanca

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual (checklist consolidado Sprints 1-3): 100% (63 de 63 itens)

Objetivo:
Criar trilha de auditoria para registrar acoes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanca, analise de alteracoes e apoio a homologacao.

Situacao atual:
Modulo de auditoria iniciado com estrutura central de eventos auditaveis, service de registro, tabela propria e primeiros eventos do sistema.

Checklist Sprint 1:
- [x] Entidade EventoAuditoria criada.
- [x] Enum de acao de auditoria criado.
- [x] Enum de nivel de auditoria criado.
- [x] Migration da tabela eventos_auditoria criada.
- [x] Indices de consulta criados.
- [x] Service centralizado de auditoria criado.
- [x] Context provider de auditoria criado.
- [x] Captura de usuario atual integrada.
- [x] Captura de IP e User-Agent integrada.
- [x] Registro de login integrado.
- [x] Registro de logout avaliado e documentado como nao aplicavel enquanto nao houver fluxo backend controlado.
- [x] Registro de criacao/edicao/inativacao de usuario integrado.
- [x] Registro de perfis/permissoes integrado.
- [x] DTOs de auditoria criados.
- [x] Testes automatizados criados.
- [x] Documentacao atualizada em Gestao ITSM.

## Roadmap ITSM - status real

- `Status` permanece como classificacao geral/legado para compatibilidade.
- `StatusImplementacao` e a referencia principal para maturidade real da entrega.
- `Objetivo` descreve a finalidade de cada item e a necessidade de negocio/operacao que ele resolve.
- `Implementado funcionalmente` significa entrega tecnica feita, mas ainda nao implica homologacao final ou producao.
- Pendencias evolutivas devem ser registradas em `PendenciasTecnicas`, `PendenciasHomologacao` e `ProximaAcao`.

Exemplos de objetivo preenchido:
- Abertura de chamado pelo portal
- Abertura por e-mail
- Perfis de acesso

## Roadmap ITSM - futuras implementacoes

- Cada item do roadmap pode registrar evolucoes no CRUD de futuras implementacoes.
- O registro suporta backlog, planejamento, desenvolvimento, homologacao, conclusao, inativacao e reativacao.
- Objetivo: dar governanca ao que falta sem perder rastreabilidade do que ja foi entregue.

## Roadmap ITSM - governanca de categoria e checklist

- Categoria agora e cadastro controlado em tabela propria (`RoadmapCategoria`) com selecao por dropdown.
- Itens antigos continuam com fallback no campo legado `Categoria` para compatibilidade.
- Percentual de implementacao passa a ser calculado automaticamente pelo checklist ativo do item.
- Regra de calculo: concluidos ativos / ativos * 100.
- UI exibe labels amigaveis para status tecnico/implementacao, sem mostrar enums crus.

## Gestao ITSM e documentacao administrativa

Nova secao administrativa disponivel:
- `Admin > Gestao ITSM > Roadmap`
- `Admin > Gestao ITSM > Documentacao`

Rotas:
- `/admin/gestao-itsm/roadmap`
- `/admin/gestao-itsm/documentacao`

A rota legada `/admin/roadmap-itsm` permanece funcional para compatibilidade com links existentes.

Objetivo:
Centralizar no painel administrativo a consulta ao Roadmap ITSM e a documentacao funcional/tecnica do SGX Sistema de Chamados, facilitando apresentacao, governanca, homologacao e acompanhamento da evolucao do sistema.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Checklist:
- [x] Grupo Gestao ITSM criado no menu administrativo.
- [x] Roadmap espelhado para Gestao ITSM.
- [x] Tela de Documentacao ITSM criada.
- [x] Documentos iniciais adicionados.
- [x] Busca de documentos criada.
- [x] Filtro por categoria criado.
- [x] Link entre Roadmap e Documentacao criado.
- [x] Permissoes integradas.
- [x] Documentacao do repositorio atualizada.
- [x] Testes ou validacao tecnica criados.

Pendencias evolutivas:
- Permitir edicao da documentacao pelo proprio sistema.
- Versionar documentacao por release.
- Anexar evidencias de homologacao.
- Exportar documentacao em PDF.
- Vincular documentos diretamente aos itens do roadmap.

## Atualizacao Sprint Portal 3

Entregas consolidadas:
- upload de anexos na abertura (`/portal/chamados/novo`) com envio sequencial apos criacao
- tratamento de falha parcial de anexos sem perder abertura do chamado
- validacao visual de tipo/tamanho de anexo com base no contexto do portal
- listagem de chamados no portal validada com exibicao de chamados recem-criados
- detalhe do portal validado com historico, anexos, comentarios publicos e SLA
- integracao portal/admin validada em testes de listagem e detalhe administrativo

Pendencias evolutivas:
- homologacao manual com usuario real
- testes E2E do fluxo portal->admin

## Sprint Portal 4 - Fechamento tecnico

Item: Abertura de chamado pelo portal
- Situacao: Implementado funcionalmente
- Status tecnico: Completo com pendencias evolutivas

Entregas confirmadas:
- backend da abertura validado
- frontend /portal/chamados/novo validado
- anexos, listagem e detalhe do portal validados
- visibilidade na fila administrativa validada
- historico inicial validado

Pendencias evolutivas reais:
- homologacao manual com usuario real
- testes E2E frontend
- validacao de anexos em ambiente real

## Sprint Integracoes E-mail 2 - Atualizacao

Entregas tecnicas confirmadas:
- abertura automatica de chamado para e-mail novo;
- origem `Email` aplicada ao chamado;
- status inicial `Aberto` aplicado;
- historico inicial `Chamado criado a partir de e-mail`;
- deduplicacao por `MessageId` mantida;
- configuracoes de categoria/prioridade/departamento/dominios no `EmailWorker`.

Pendencias para evolucao:
- correlacao avancada de respostas;
- escopo completo de anexos por e-mail;
- validacao IMAP real com caixa institucional;
- homologacao com e-mails reais.

## Sprint Integracoes E-mail 3 - Atualizacao

Entregas tecnicas confirmadas:
- correlacao de resposta por codigo no assunto (`SGX`/`CHM`);
- correlacao por `InReplyTo` e `References` via `LogIntegracaoEmail`;
- resposta correlacionada gera comentario publico e historico de resposta;
- anexos validados por extensao, tamanho e MIME;
- bloqueio explicito de extensoes perigosas e tentativa de path traversal;
- sucesso parcial de anexos sem derrubar processamento do Worker.

Pendencias evolutivas:
- validacao IMAP real e homologacao com e-mails reais;
- OAuth para caixa Microsoft (se exigido pelo ambiente);
- retry/backoff, dead-letter e reprocessamento manual;
- varredura antivirus e sanitizacao HTML avancada;
- monitoramento operacional e E2E com IMAP real.

## Sprint Integracoes E-mail 4 - Atualizacao

Entregas tecnicas confirmadas:
- endpoints administrativos de logs de e-mail com listagem, detalhe, filtros, ordenacao e paginacao;
- tela administrativa `/admin/integracoes/email` com filtros, estado vazio/erro/loading e dialog tecnico;
- labels amigaveis de status (`Nao correlacionado` sem enum cru na UI);
- acao de abrir chamado vinculado a partir da tabela e do detalhe do log;
- bloqueio de acesso para `Solicitante` mantido em backend e frontend.

Pendencias evolutivas:
- validacao com caixa IMAP real e homologacao com e-mails reais;
- reprocessamento manual, retry/backoff e dead-letter;
- monitoramento operacional do Worker e E2E com IMAP real.

## Sprint Integracoes E-mail 5 - Atualizacao

Fechamento tecnico realizado:
- item `Abertura por e-mail` atualizado para `Implementado funcionalmente`;
- status tecnico atualizado para `Completo com pendencias evolutivas`;
- checklist tecnico/evolutivo vinculado ao roadmap com percentual automatico por itens ativos;
- documentacao consolidada de configuracao, processamento, correlacao, anexos e logs administrativos.

Pendencias reais mantidas:
- validacao IMAP real e homologacao com e-mails/anexos reais;
- OAuth Microsoft (se exigido), retry/backoff, dead-letter;
- monitoramento/metricas/alertas do Worker;
- reprocessamento manual de mensagens com erro;
- sanitizacao HTML avancada e antivirus de anexos.

## Sprint Autenticacao 5 - Atualizacao

Fechamento do item `Autenticacao corporativa` no Roadmap ITSM:
- status da implementacao consolidado em `Implementado funcionalmente`;
- status tecnico consolidado em `Completo com pendencias evolutivas`;
- checklist do item consolidado em `27` itens (`19` concluidos e `8` pendentes);
- percentual de implementacao calculado automaticamente pelo checklist ativo (aprox. `70%`).
- regra arquitetural reforcada: Microsoft Entra ID/Azure AD autentica e SGX autoriza por usuarios, perfis e permissoes internas.

Pendencias reais mantidas:
- homologacao com tenant institucional real do Microsoft Entra ID;
- validacao com usuarios corporativos reais;
- validacao de MFA e Conditional Access;
- validacao de logout corporativo;
- validacao em ambiente publicado/VPS;
- revisao com equipe responsavel pelo Azure;
- evidencia formal de homologacao;
- governanca de ciclo de vida do usuario interno (bloqueio, reativacao e auditoria).

## Sprint SLA 1 - Base tecnica e administrativa

Area: SLA
Categoria: SLA

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Entregas da sprint:
- modelagem inicial com entidades de politica e meta de SLA;
- tabelas dedicadas para politicas e metas (`sla_politicas`, `sla_metas`);
- seed idempotente da politica `SLA Padrao` com metas por prioridade;
- endpoints administrativos para cadastro e manutencao;
- permissoes granulares de SLA;
- tela administrativa inicial em `/admin/sla/policies`.

Pendencias evolutivas:
- aplicacao automatica completa do SLA no ciclo do chamado (Sprint 2);
- homologacao funcional com usuarios reais e evidencias formais.

## Sprint SLA 2 - Aplicacao pratica nos chamados

Area: SLA
Categoria: SLA

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Entregas da sprint:
- tabela `chamado_slas` criada para registrar o SLA aplicado ao chamado;
- relacionamento 1:1 entre chamado e SLA aplicado;
- servico centralizado para escolha de politica, calculo de prazos, primeira resposta, resolucao, pausa e reabertura;
- SLA aplicado na abertura do chamado sem bloquear o fluxo quando nao houver politica/meta ativa;
- DTOs de detalhe/listagem atualizados com resumo de SLA;
- listagem administrativa com situacao de SLA e filtros por situacao;
- detalhe administrativo e portal exibindo politica, prazos e situacao de SLA;
- testes automatizados cobrindo prioridades, resposta, resolucao, pausa, politica especifica e ausencia de politica/meta;
- documentacao `docs/SLA.md` atualizada.

Checklist Sprint 2:
- [x] Tabela de SLA aplicado ao chamado criada.
- [x] Relacionamento entre chamado e SLA criado.
- [x] Service de calculo de SLA criado.
- [x] Politica aplicavel identificada por prioridade/categoria/departamento.
- [x] SLA aplicado na criacao do chamado.
- [x] Prazo de primeira resposta calculado.
- [x] Prazo de resolucao calculado.
- [x] Primeira resposta registrada.
- [x] Resolucao registrada.
- [x] Pausa de SLA preparada e implementada para `AguardandoSolicitante`.
- [x] Situacao atual do SLA calculada.
- [x] SLA exibido no detalhe do chamado.
- [x] SLA exibido na listagem administrativa.
- [x] Filtros administrativos de SLA criados.
- [x] DTOs de chamado atualizados com resumo de SLA.
- [x] Testes automatizados criados.
- [x] Documentacao atualizada.

Pendencias evolutivas:
- calendario de horario comercial configuravel;
- homologacao funcional em PostgreSQL real;
- evidencias formais de UX em ambiente publicado.

## Sprint SLA 3 - Alertas, vencimentos e painel

Area: SLA
Categoria: SLA

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Entregas da sprint:
- configuracao administrativa de alertas de SLA;
- historico de eventos de SLA com chave de idempotencia;
- job periodico de monitoramento configuravel por `SlaMonitoring`;
- eventos integrados ao ciclo de SLA aplicado, resposta, resolucao, pausa e retomada;
- painel administrativo de SLA com indicadores e agrupamentos;
- consulta estruturada para relatorio/exportacao futura;
- historico de SLA no detalhe administrativo do chamado.

Checklist Sprint 3:
- [x] Configuração de alerta de SLA criada.
- [x] Tela administrativa de configuração de alerta criada.
- [x] Endpoints de configuração de alerta criados.
- [x] Job de verificação de SLA criado.
- [x] Periodicidade configurável por appsettings criada.
- [x] Controle contra notificações/eventos duplicados criado.
- [x] Histórico de eventos de SLA criado.
- [x] Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolução, pausa e retomada.
- [x] Painel de indicadores de SLA criado.
- [x] Indicador de SLA vencido criado.
- [x] Indicador de SLA próximo do vencimento criado.
- [x] Indicador de percentual de cumprimento criado.
- [x] Métrica de tempo médio de primeira resposta criada.
- [x] Métrica de tempo médio de resolução criada.
- [x] Indicadores por prioridade criados.
- [x] Indicadores por categoria criados.
- [x] Indicadores por departamento criados.
- [x] Histórico de SLA exibido no detalhe administrativo do chamado.
- [x] Estrutura preparada para exportação futura.
- [x] Documentação atualizada.
- [x] Testes automatizados criados.

Pendencias evolutivas:
- integracao com servico oficial de notificacoes;
- exportacao Excel/PDF;
- dashboards historicos por periodo de violacao;
- calendario por departamento/time;
- importacao automatica de feriados.

## Sprint SLA 4 - Calendario corporativo e horario comercial

Area: SLA
Categoria: SLA

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Entregas tecnicas:

- entidades `CalendarioCorporativo`, `HorarioAtendimentoCalendario` e `ExcecaoCalendarioCorporativo`;
- tabelas `calendarios_corporativos`, `horarios_atendimento_calendario` e `excecoes_calendario_corporativo`;
- seed do calendario corporativo padrao em `America/Sao_Paulo`, segunda a sexta das 09:00 as 18:00;
- politica de SLA com `CalendarioCorporativoId` opcional;
- calculo de prazo e minutos decorridos com `SlaBusinessTimeCalculator`;
- tela `Admin > SLA > Calendarios`;
- tela de politicas atualizada com selecao de calendario;
- detalhe do chamado exibindo tipo de calculo e calendario usado.

Checklist Sprint 4:

- [x] Entidade CalendarioCorporativo criada.
- [x] Entidade HorarioAtendimentoCalendario criada.
- [x] Entidade ExcecaoCalendarioCorporativo criada.
- [x] Migrations de calendario criadas.
- [x] Seed do calendario padrao criado.
- [x] Relacionamento entre Politica SLA e Calendario criado.
- [x] Service administrativo de calendario criado.
- [x] Service de calculo de tempo util criado.
- [x] Calculo de prazo de primeira resposta usando horario comercial implementado.
- [x] Calculo de prazo de resolucao usando horario comercial implementado.
- [x] Calculo de minutos uteis de primeira resposta implementado.
- [x] Calculo de minutos uteis de resolucao implementado.
- [x] Endpoints administrativos de calendario criados.
- [x] Tela Admin > SLA > Calendarios criada.
- [x] Tela de politica SLA atualizada com selecao de calendario.
- [x] Detalhe do chamado mostra tipo de calculo e calendario usado.
- [x] Testes automatizados criados.
- [x] Documentacao atualizada.

Pendencias evolutivas:

- calendario especifico por departamento/time;
- importacao automatica de feriados nacionais, estaduais e municipais;
- excecoes recorrentes;
- refinamento de prazo remanescente em reabertura.

## Sprint Historico/Auditoria 2 - Modulos criticos

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Aplicar auditoria de governanca aos modulos criticos do SGX para registrar acoes relevantes com contexto, antes/depois e mascaramento de dados sensiveis.

Situacao atual:
Base tecnica da Sprint 1 mantida e cobertura expandida para chamados, usuarios, perfis/permissoes, SLA administrativo, autenticacao corporativa e roadmap ITSM.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist

Checklist Sprint 2:
- [x] Helper de diff antes/depois criado.
- [x] Mascaramento de dados sensiveis implementado.
- [x] Auditoria de abertura de chamado implementada.
- [x] Auditoria de alteracao de status implementada.
- [x] Auditoria de alteracao de prioridade implementada.
- [x] Auditoria de alteracao de categoria implementada.
- [x] Auditoria de atribuicao de responsavel implementada.
- [x] Auditoria de assumir chamado implementada.
- [x] Auditoria de comentarios administrativos implementada.
- [x] Auditoria de encerramento/resolucao implementada.
- [x] Auditoria de reabertura implementada.
- [x] Auditoria de anexos preparada ou implementada.
- [x] Auditoria de usuarios revisada e complementada.
- [x] Auditoria de perfis revisada e complementada.
- [x] Auditoria de permissoes revisada e complementada.
- [x] Auditoria de politicas de SLA implementada.
- [x] Auditoria de metas de SLA implementada.
- [x] Auditoria de calendarios de SLA implementada.
- [x] Auditoria de horarios de calendario implementada.
- [x] Auditoria de excecoes de calendario implementada.
- [x] Auditoria de alertas de SLA implementada.
- [x] Auditoria de autenticacao corporativa implementada.
- [x] Auditoria de Roadmap ITSM implementada.
- [x] Auditoria de documentacao ITSM preparada conforme estrutura atual.
- [x] Testes automatizados de auditoria dos modulos criticos criados.
- [x] Documentacao atualizada em Gestao ITSM.
- [x] Validacao no banco com eventos reais em eventos_auditoria preparada/executada.

Pendencias planejadas para Sprint 3:
- tela administrativa de consulta de auditoria;
- listagem paginada e filtros avancados;
- dashboard e exportacao de auditoria.

## Sprint Historico/Auditoria 3 - Consulta administrativa

Area: Historico/Auditoria
Categoria: Governanca

Objetivo:
Permitir consulta administrativa dos eventos de auditoria com filtros avancados, paginacao, detalhe do evento e indicadores de governanca.

Situacao atual:
Base tecnica da Sprint 1 e cobertura de modulos criticos da Sprint 2 foram consolidadas com camada de consulta e visualizacao administrativa.

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas
Percentual: calculado por checklist (63/63 = 100%)

Checklist Sprint 3:
- [x] Endpoints administrativos de auditoria criados.
- [x] Use cases/services de consulta de auditoria criados.
- [x] Filtros de auditoria criados.
- [x] Paginacao de eventos criada.
- [x] Endpoint de detalhe de evento criado.
- [x] Endpoint de dashboard de auditoria criado.
- [x] Permissoes de auditoria criadas e integradas.
- [x] Menu Governanca > Auditoria criado.
- [x] Rota /admin/governanca/auditoria criada.
- [x] Tela administrativa de auditoria criada.
- [x] Drawer de detalhe criado.
- [x] Visualizacao de dados antes/depois criada.
- [x] Indicadores basicos de auditoria criados.
- [x] Service frontend de auditoria criado.
- [x] Tipos frontend de auditoria criados.
- [x] Link entre Auditoria e Gestao ITSM criado.
- [x] Documentacao em Gestao ITSM atualizada.
- [x] Testes automatizados backend criados.
- [x] Build frontend validado.
- [x] Validacao com eventos reais em eventos_auditoria executada.

Pendencias evolutivas:
- exportacao Excel/PDF;
- retencao configuravel de auditoria;
- assinatura/hash da trilha de auditoria;
- alertas para eventos criticos;
- painel avancado de seguranca;
- integracao com SIEM/Log Analytics;
- politica de anonimizaçao/LGPD para eventos antigos.
