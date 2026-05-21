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

- Cadastros Administrativos (Sprints 1 a 6) implementado funcionalmente, com integracao aos chamados, seed inicial consolidado e documentacao de fechamento
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

## Sprint Cadastros Administrativos 1 - Base tecnica

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Entregas da sprint:
- estrutura de entidades administrativas consolidada;
- `SubcategoriaChamado` criada e vinculada em `CategoriaChamado`;
- `TipoSolicitacao` criado;
- `LocalUnidade` criado com `Endereco`;
- `PrioridadeChamado` evoluida com `Peso` e `Cor`;
- `DbSet` e mapeamentos Fluent API adicionados;
- migration `AddCadastrosAdministrativosSprint1` criada e aplicada.

Checklist Sprint 1:
- [x] Criar entidade Departamento
- [x] Criar entidade CategoriaChamado
- [x] Criar entidade SubcategoriaChamado
- [x] Criar entidade PrioridadeChamado
- [x] Criar entidade TipoSolicitacao
- [x] Criar entidade LocalUnidade
- [x] Adicionar DbSet no DbContext
- [x] Criar configuracoes Fluent API
- [x] Definir relacionamento CategoriaChamado x SubcategoriaChamado
- [x] Criar migration inicial dos cadastros administrativos
- [x] Atualizar banco de dados
- [x] Criar `docs/CADASTROS-ADMINISTRATIVOS.md`
- [x] Atualizar `docs/ROADMAP.md`
- [x] Atualizar `docs/ROADMAP-ITSM.md`

Pendencias evolutivas:
- criar CRUD administrativo para tipos de solicitacao e locais/unidades;
- vincular subcategoria/tipo/local no fluxo de abertura e tratamento de chamados;
- ampliar testes automatizados cobrindo novos cadastros.

## Sprint Cadastros Administrativos 2 - Backend CRUD (Departamentos, Categorias e Subcategorias)

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar CRUD administrativo de departamentos, categorias e subcategorias com validacoes de duplicidade, vinculo categoria/subcategoria, ativacao/inativacao e preservacao historica.

Entregas da sprint:
- CRUD de departamentos consolidado com listagem, busca e filtro de status;
- CRUD de categorias consolidado com listagem, busca e filtro de status;
- CRUD de subcategorias implementado com listagem geral e listagem por categoria;
- rotas administrativas expostas em `api/admin` e mantidas em `api/admin/cadastros` para compatibilidade;
- `DELETE` alinhado para inativacao logica;
- validacao de duplicidade por nome dentro da categoria para subcategorias;
- validacao obrigatoria de categoria existente para criar/atualizar subcategoria;
- testes automatizados de use case ampliados para cobrir sprint 2.

Checklist Sprint 2:
- [x] DTOs para subcategorias
- [x] Services/use cases para subcategorias
- [x] Endpoints administrativos para subcategorias
- [x] Endpoints `DELETE`/`PATCH` para ativacao e inativacao
- [x] Validacao de duplicidade para departamentos
- [x] Validacao de duplicidade para categorias
- [x] Validacao de vinculo categoria/subcategoria
- [x] Listagem com busca e filtro por status
- [x] Testes automatizados de departamentos/categorias/subcategorias
- [x] Documentacao de cadastros atualizada
- [x] Roadmaps atualizados

Pendencias evolutivas:
- CRUD administrativo de tipos de solicitacao;
- CRUD administrativo de locais/unidades;
- integracao desses cadastros no fluxo de abertura/gestao de chamados.

## Sprint Cadastros Administrativos 3 - Backend CRUD (Prioridades, Tipos de Solicitacao e Locais/Unidades)

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Implementar os CRUDs administrativos complementares para priorizacao, classificacao ITSM e localizacao dos chamados.

Entregas da sprint:
- CRUD de prioridades com regras de duplicidade por nome, peso obrigatorio maior que zero e cor opcional em formato hexadecimal;
- CRUD de tipos de solicitacao com duplicidade por nome, ativacao/inativacao e preservacao historica;
- CRUD de locais/unidades com duplicidade por nome, endereco opcional, ativacao/inativacao e preservacao historica;
- endpoints de `DELETE` convertidos para inativacao logica;
- aliases legados mantidos em `api/admin/cadastros/*`;
- testes automatizados de use case e HTTP ampliados para os tres cadastros.

Checklist Sprint 3:
- [x] DTOs de PrioridadeChamado
- [x] DTOs de TipoSolicitacao
- [x] DTOs de LocalUnidade
- [x] Use Cases / Services de PrioridadeChamado
- [x] Use Cases / Services de TipoSolicitacao
- [x] Use Cases / Services de LocalUnidade
- [x] Validators
- [x] Controllers administrativos
- [x] Endpoints REST
- [x] Validacao de duplicidade
- [x] Validacao de peso da prioridade
- [x] Ativacao e inativacao
- [x] Listagem com busca e filtro por status
- [x] Testes automatizados
- [x] Atualizacao da documentacao
- [x] Atualizacao dos roadmaps

Pendencias evolutivas:
- integrar `TipoSolicitacao` e `LocalUnidade` no fluxo de abertura/edicao de chamado;
- evoluir regras de ordenacao/priorizacao de SLA considerando `Peso` como fonte principal;
- homologacao funcional com usuarios-chave dos cadastros administrativos.

## Sprint Cadastros Administrativos 4 - Frontend Administrativo dos Cadastros

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Criar as telas administrativas para manutencao de departamentos, categorias, subcategorias, prioridades, tipos de solicitacao e locais/unidades no menu `Admin > Cadastros`.

Entregas da sprint:
- menu de cadastros expandido com subcategorias, tipos de solicitacao e locais/unidades;
- rotas frontend criadas para listagem e detalhe dos tres novos cadastros;
- telas de listagem com busca por nome, filtro por status e paginacao;
- acoes diretas de editar, inativar e reativar com confirmacao;
- formularios com validacao obrigatoria:
  - nome obrigatorio em todos os cadastros;
  - categoria obrigatoria para subcategoria;
  - peso obrigatorio e maior que zero para prioridade;
  - cor opcional de prioridade com validacao `#RRGGBB`;
- service de cadastros administrativos atualizado para consumir `api/admin/*` como base preferencial.

Checklist Sprint 4:
- [x] Menu Admin > Cadastros criado/atualizado
- [x] Tela Departamentos
- [x] Tela Categorias
- [x] Tela Subcategorias
- [x] Tela Prioridades
- [x] Tela Tipos de Solicitacao
- [x] Tela Locais / Unidades
- [x] Services de API frontend criados/atualizados
- [x] Rotas frontend criadas
- [x] Busca e filtro por status
- [x] Acoes de criar, editar, ativar e inativar
- [x] Feedback de sucesso e erro
- [x] Estados de carregamento e lista vazia
- [x] Documentacao atualizada

Pendencias evolutivas:
- homologacao funcional com usuarios administrativos;
- testes frontend E2E para o fluxo completo de cadastros;
- integracao de tipo/local/subcategoria no fluxo de abertura e edicao de chamados (fora desta sprint).

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

## Sprint Comentarios no Atendimento - Atualizacao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Entregas tecnicas confirmadas:
- endpoint unificado `GET /api/chamados/{chamadoId}/comentarios`;
- endpoint unificado `POST /api/chamados/{chamadoId}/comentarios`;
- regras por perfil para comentario publico/interno;
- bloqueio de comentario interno para `Solicitante`;
- visibilidade de comentario interno apenas para `Administrador` e `Atendente`;
- validacao de mensagem obrigatoria e limite de 4000 caracteres;
- ordenacao cronologica crescente de comentarios;
- frontend do detalhe do chamado com secao `Atendimento / Comentarios`, envio e marcacao de comentario interno por perfil;
- migration incremental com ajuste de `mensagem` para 4000 e indice por `criado_em`;
- testes automatizados da sprint adicionados e executados.

## Sprint Anexos no Atendimento - Atualizacao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Entregas tecnicas confirmadas:
- endpoints unificados de anexos em `/api/chamados` para listar, enviar e baixar;
- validacao de arquivo obrigatorio, nao vazio, tamanho maximo, extensao permitida e extensao bloqueada;
- nome fisico seguro com identificador aleatorio e extensao normalizada;
- download com validacao de acesso ao chamado antes da abertura de arquivo;
- retorno de `404` para arquivo fisico inexistente sem exposicao de caminho interno;
- listagem sem exposicao de `Caminho` e `NomeArquivoArmazenado`;
- frontend de detalhe do chamado atualizado para upload/listagem/download de anexos;
- nenhum endpoint DELETE de anexo publicado;
- nenhum botao de exclusao de anexo na interface.

## Sprint Historico e Linha do Tempo do Atendimento - Atualizacao

Area: Atendimento
Categoria: Atendimento

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo

Entregas tecnicas confirmadas:
- endpoint `GET /api/chamados/{chamadoId}/linha-do-tempo` criado;
- consolidacao de abertura, comentarios, anexos e historico em visao unica;
- filtro por perfil aplicado na linha do tempo (interno/publico);
- solicitante sem visualizacao de comentario interno e eventos internos administrativos;
- timeline atualizada apos envio de comentario e upload de anexo;
- evento de anexo na timeline com download e sem exposicao de caminho/nome fisico;
- sem criacao de endpoint DELETE de anexo;
- sem criacao de botao de exclusao de anexo.

## Item Roadmap ITSM - Comentarios e Anexos

Area: Atendimento
Nome do item: Comentarios e anexos

Status consolidado:
- StatusImplementacao: Implementado funcionalmente
- StatusTecnico: Completo
- PercentualImplementacao: 100%
- Situacao atual: Implementado
- Avaliacao: Aprovado

Pendencias:
- tecnicas: nenhuma pendencia bloqueante;
- homologacao: validar formalmente em ambiente de homologacao com usuarios reais, caso ainda nao exista evidencia formal.

Regras de negocio mantidas:
- comentarios internos restritos a Administrador/Atendente;
- anexos salvos no atendimento nao podem ser excluidos por nenhum perfil;
- nao existe endpoint DELETE de anexo;
- nao existe botao de exclusao de anexo.

## Sprint Cadastros Administrativos 5 - Integracao com Chamados

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Integrar departamentos, categorias, subcategorias, prioridades, tipos de solicitacao e locais/unidades ao fluxo real de chamados, com uso de ativos em novas operacoes e preservacao historica.

Checklist Sprint 5:
- [x] Modelo atual de `Chamado` analisado antes de alterar
- [x] Campos antigos preservados
- [x] Viculos faltantes adicionados (`SubcategoriaId`, `TipoSolicitacaoId`, `LocalUnidadeId`)
- [x] Migration incremental criada (`Sprint5IntegracaoCadastrosChamados`)
- [x] DTOs de criacao/edicao/detalhe/listagem atualizados
- [x] Use cases de abertura e administracao atualizados
- [x] Validacao de subcategoria por categoria aplicada
- [x] Regra de ativos para novas selecoes aplicada
- [x] Regras historicas para inativos preservadas
- [x] Endpoints operacionais de cadastros ativos publicados
- [x] Filtros administrativos de chamados ampliados
- [x] Frontend de abertura e detalhe atualizado
- [x] Testes backend atualizados

Comandos de validacao executados:
- `dotnet build SGX.SistemaChamado.sln -c Release`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj -c Release`
- `npm.cmd run build` (em `src/SGX.SistemaChamado.Web`)

Pendencias evolutivas:
- ampliar testes frontend automatizados para fluxo completo de abertura/triagem com novos campos;
- avaliar separacao futura entre `DepartamentoSolicitanteId` e `DepartamentoResponsavelId`.

## Sprint Cadastros Administrativos 6 - Seed Inicial, Testes e Fechamento

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar o modulo com seed inicial idempotente, reforco de testes e fechamento documental para validacao funcional.

Checklist Sprint 6:
- [x] seed inicial dos principais cadastros consolidado
- [x] comportamento sem duplicidade validado
- [x] testes backend revisados e ampliados
- [x] validacao de endpoints operacionais com somente ativos
- [x] validacao de subcategorias ativas filtradas por categoria
- [x] documentacao final atualizada
- [x] roadmap atualizado com fechamento da sprint

Decisao tecnica aplicada:
- seed inicial mantido no fluxo de `DevelopmentSeedService` para validacao funcional local/homologacao tecnica;
- prioridades consolidadas com peso/cor padrao da sprint (`Baixa` `#22C55E`, `Media` `#EAB308`, `Alta` `#F97316`, `Critica` `#EF4444`);
- subcategorias minimas por categoria adicionadas com vinculo consistente;
- cadastros existentes nao sao sobrescritos e nao ha reativacao automatica de inativos.

Pendencias evolutivas:
- evoluir para uma estrategia opcional/configuravel de seed institucional por ambiente;
- ampliar testes frontend automatizados para fluxos completos de abertura/triagem.

## Sprint Cadastros Administrativos 7 - Checklist Funcional e Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Concluir validacao funcional do modulo de cadastros administrativos no ciclo de chamados, com ajustes finos e registro de evidencias tecnicas.

Checklist Sprint 7:
- [x] validacao tecnica de CRUD administrativo por cadastro
- [x] validacao de busca por nome e filtros por status (`Ativo`, `Inativo`, `Todos`)
- [x] validacao de abertura de chamado com cadastros ativos
- [x] validacao de subcategoria por categoria e bloqueio de inconsistencias
- [x] validacao de bloqueio de registros inativos em novas operacoes
- [x] validacao de preservacao historica de chamados com cadastro inativo vinculado
- [x] validacao de filtros administrativos por cadastros no modulo de chamados
- [x] validacao de detalhe portal/admin com nomes de cadastros vinculados
- [x] ajustes finos em testes e validadores
- [x] documentacao de homologacao atualizada

Pendencias evolutivas:
- homologacao manual com evidencias visuais formais por tela em ambiente institucional;
- consolidacao de suite frontend automatizada/E2E para cobertura visual ponta a ponta.

## Sprint Cadastros Administrativos 8 - Consolidacao ITSM e Checklist de Homologacao

Area: Cadastros Administrativos
Categoria: Cadastros

Status da implementacao: Implementado funcionalmente
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar a governanca documental do modulo de cadastros administrativos e formalizar o checklist de homologacao para validacao institucional.

Checklist Sprint 8:
- [x] documento ITSM especifico dos cadastros administrativos criado
- [x] checklist de homologacao funcional criado
- [x] documentacao de cadastros atualizada com o fechamento da sprint
- [x] roadmap geral atualizado
- [x] roadmap ITSM atualizado

Evidencias documentais:
- `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md`
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`
- `docs/CADASTROS-ADMINISTRATIVOS.md`

Pendencias evolutivas:
- execucao manual do checklist em ambiente de homologacao com usuarios reais;
- formalizacao de aceite funcional e registro de evidencias visuais.

## Item de roadmap - Cadastros administrativos

Area:
- Cadastros administrativos

Categoria:
- Cadastros

Ordem:
- 8

Objetivo:
Disponibilizar cadastros administrativos parametrizaveis para apoiar a classificacao, priorizacao, organizacao, triagem, filtros, historico e evolucao ITSM do sistema de chamados. O modulo deve contemplar departamentos, categorias, subcategorias, prioridades, tipos de solicitacao, locais/unidades e demais cadastros estruturais necessarios para a operacao do service desk.

Situacao atual:
Modulo de Cadastros Administrativos implementado e validado funcionalmente em nivel tecnico. Backend, frontend administrativo, integracao com abertura/gestao de chamados, seed inicial e validacao funcional foram concluidos. A homologacao institucional/manual com evidencias formais permanece pendente.

Atencao tecnica:
Verificar se todos os cadastros permitirao ativacao/inativacao sem exclusao fisica, evitando perda de historico em chamados antigos. Validar quais cadastros serao parametrizaveis pela area administrativa e se o status do chamado permanecera como fluxo controlado do sistema ou se sera tratado futuramente como cadastro configuravel. Priorizar inativacao logica, validacao de duplicidade, uso apenas de registros ativos em novas operacoes e preservacao historica.

Status da implementacao:
- Fluxo funcional validado

Status tecnico:
- Aguardando homologacao institucional

Percentual (%):
- 90

Checklist:
- 7/8 concluidos
- [x] Criar documentacao ITSM.
- [x] Criar checklist de homologacao.
- [x] Implementar backend dos cadastros.
- [x] Implementar frontend administrativo.
- [x] Integrar cadastros com abertura de chamados.
- [x] Criar seed inicial.
- [x] Validar fluxo funcional.
- [ ] Homologar em ambiente institucional.

Pendencias tecnicas:
- Nao ha pendencias tecnicas bloqueantes identificadas para o modulo.
- Manter como evolucao futura a cobertura frontend E2E completa.
- Avaliar futuramente se status de chamado continuara como fluxo controlado ou se sera parametrizado em cadastro proprio.

Pendencias de homologacao:
- Executar homologacao institucional/manual.
- Coletar evidencias formais de tela.
- Registrar responsavel pela homologacao.
- Registrar data da homologacao.
- Registrar ambiente utilizado.
- Registrar resultado final: aprovado, aprovado com ressalvas ou reprovado.

Evidencia da implementacao:
Documentacao criada:
- `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md`
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`

Documentacao atualizada:
- `docs/CADASTROS-ADMINISTRATIVOS.md`
- `docs/ROADMAP.md`
- `docs/ROADMAP-ITSM.md`

Validacoes tecnicas:
- Backend dos cadastros implementado e validado.
- Frontend administrativo implementado e validado.
- Integracao com abertura e gestao de chamados validada.
- Seed inicial validado.
- Fluxo funcional validado.
- dotnet build OK.
- dotnet test OK com 420 testes aprovados.
- npm build OK.

Homologacao institucional (Item 8) - situacao real:
- roteiro formal de homologacao manual registrado em `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`;
- evidencias obrigatorias (prints, responsavel, data, ambiente e resultado) documentadas;
- sem evidencias institucionais anexadas nesta etapa, status mantido em `90%` e `7/8`.

Somente arquivos `docs/*.md` foram alterados nesta etapa.

Data de conclusao tecnica:
- (deixar em branco)

Data de homologacao:
- (deixar em branco)

Criterio de aceite:
- Documentacao ITSM criada.
- Checklist de homologacao criado.
- Backend dos cadastros implementado e validado.
- Frontend administrativo implementado e validado.
- Cadastros integrados ao fluxo de abertura e gestao de chamados.
- Seed inicial criado e validado.
- Fluxo funcional validado tecnicamente.
- Registros ativos usados em novas operacoes.
- Registros inativos preservados para historico.
- Homologacao institucional pendente como aceite formal final.

Proxima acao:
Executar homologacao institucional/manual com evidencias formais, incluindo prints das telas administrativas, abertura de chamado com cadastros, detalhe do chamado, filtros administrativos, responsavel, data, ambiente e resultado da validacao.

## Item de roadmap - Dashboard / Gestão

Area:
- Dashboard

Categoria:
- Gestão

Ordem:
- 9

Objetivo:
Disponibilizar uma visão gerencial da operação de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no período, chamados sem responsável, riscos de SLA, distribuição por status, prioridade, categoria, produtividade por atendente e situação da integração de e-mail.

Situacao atual:
Dashboard administrativo implementado funcionalmente no backend e frontend. A API disponibiliza indicadores consolidados, filtros por período e contexto administrativo. A interface apresenta cards gerenciais, gráficos/listagens por status, prioridade e categoria, indicadores de SLA, produtividade por atendente, fila de chamados e resumo da integração de e-mail. Pendente validação com usuários reais, refinamento visual final, testes frontend/e2e e homologação institucional.

Atencao tecnica:
Validar se os indicadores respeitam corretamente as permissões internas do usuário autenticado. Confirmar se administradores visualizam a operação completa e se atendentes visualizam apenas o escopo permitido, caso essa regra seja exigida. Verificar performance das consultas em bases maiores, principalmente filtros por período, produtividade por atendente e agrupamentos por status, prioridade e categoria. Garantir que chamados inativos, registros históricos e dados de SLA sejam tratados corretamente para não distorcer os indicadores.

Status da implementacao:
- Implementado funcionalmente

Status tecnico:
- Completo com pendências evolutivas

Percentual (%):
- 85

Pendencias tecnicas:
- Aplicar ou validar permissão granular `Dashboard.Visualizar` no backend, além da proteção por perfil.
- Validar performance com volume maior de chamados.
- Criar ou consolidar testes automatizados específicos do dashboard em nível HTTP.
- Criar testes frontend/e2e para `dashboardAdminService` e `AdminDashboardView`, se o projeto já tiver estrutura para isso.
- Avaliar cache ou otimização das consultas agregadas, caso necessário.
- Revisar regras de permissão dos indicadores por perfil.

Pendencias de homologacao:
- Validar com Administrador.
- Validar com Atendente.
- Conferir números do dashboard contra consultas reais no banco.
- Validar filtros por período, departamento, categoria e responsável.
- Confirmar se os indicadores atendem à necessidade de gestão da operação.
- Registrar evidências formais de homologação.

Evidencia da implementacao:
- `src/SGX.SistemaChamado.Api/Controllers/AdminDashboardController.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminIndicadoresUseCases.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminDashboardDtos.cs`
- `src/SGX.SistemaChamado.Web/src/services/dashboardAdminService.ts`
- `src/SGX.SistemaChamado.Web/src/types/dashboard.ts`
- `src/SGX.SistemaChamado.Web/src/views/AdminDashboardView.vue`
- `tests/SGX.SistemaChamado.Tests/DashboardAdminUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/IndicadoresUseCaseTests.cs`

Criterio de aceite:
O usuário autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da operação. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período. A tela deve permitir navegação para fila de chamados, gestão de chamados e integração de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.

Proxima acao:
Executar validação técnica e homologação funcional do dashboard com dados reais ou massa simulada mais próxima da operação institucional.

Checklist:

Planejamento:
- [x] Definir indicadores principais do dashboard.
- [x] Definir filtros gerenciais.
- [x] Definir visão para administrador e atendente.

Backend:
- [x] Criar endpoint de dashboard administrativo.
- [x] Criar endpoint de chamados por status.
- [x] Criar endpoint de chamados por prioridade.
- [x] Criar endpoint de chamados por categoria.
- [x] Criar endpoint de indicadores de SLA.
- [x] Criar endpoint de produtividade por atendente.
- [ ] Aplicar ou validar policy granular Dashboard.Visualizar no backend.
- [ ] Validar performance das consultas agregadas.
- [ ] Validar regras de permissão por perfil.

Frontend:
- [x] Criar tela administrativa de Dashboard.
- [x] Criar cards de indicadores principais.
- [x] Criar filtros do dashboard.
- [x] Exibir indicadores por status.
- [x] Exibir indicadores por prioridade.
- [x] Exibir indicadores por categoria.
- [x] Exibir indicadores de SLA.
- [x] Exibir produtividade por atendente.
- [x] Exibir fila resumida de chamados.
- [x] Exibir resumo da integração de e-mail.
- [ ] Refinar layout visual para apresentação gerencial.

Testes:
- [x] Criar testes de use case do dashboard.
- [x] Criar testes de use case dos indicadores.
- [ ] Criar testes HTTP de sucesso para /api/admin/dashboard.
- [ ] Criar testes HTTP de sucesso para /api/admin/indicadores/*.
- [ ] Testar bloqueio por ausência de permissão granular, se a policy for aplicada.
- [ ] Criar teste frontend/e2e, se aplicável.

Documentacao:
- [x] Registrar dashboard no roadmap geral.
- [x] Criar documentação funcional específica do Dashboard / Gestão.
- [ ] Registrar evidências de homologação.

Homologacao:
- [ ] Validar com administrador.
- [ ] Validar com atendente.
- [ ] Validar com massa real ou simulada.
- [ ] Registrar aceite funcional.
