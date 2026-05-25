# Relatorios Avancados

## Visao geral

O modulo de Relatorios Avancados do SGX Sistema de Chamados consolida visoes gerenciais e operacionais sobre atendimento, chamados, SLA, departamentos, catalogo de servicos, aprovacoes, inventario/ativos, base de conhecimento, auditoria e produtividade.

A proposta do modulo nao e reproduzir listagens de chamados, e sim estruturar consultas analiticas com filtros, agrupamentos e formatos de exportacao apropriados para operacao e gestao.

## Sprint 1 - Fundacao tecnica

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Fundacao tecnica implementada  
Status tecnico: Sprint 1 concluida apos validacao  
Percentual: 20%

### Entregas

- permissoes `RelatoriosAvancados.*` adicionadas no catalogo e no seed;
- constantes de permissao atualizadas no backend e frontend;
- DTOs, requests e enums iniciais criados para arquitetura de leitura;
- interface `IAdminRelatoriosAvancadosUseCases` criada;
- implementacao `RelatoriosAvancadosAdminUseCases` criada com retorno de metadados;
- controller `AdminRelatoriosAvancadosController` criado;
- endpoint `GET /api/admin/relatorios-avancados/metadados` publicado e protegido por permissao;
- testes automatizados de seed, autorizacao e metadados adicionados.

### Metadados do endpoint inicial

O endpoint inicial retorna:

- periodos suportados;
- tipos de relatorio disponiveis;
- agrupamentos suportados;
- filtros disponiveis;
- formatos de exportacao planejados;
- permissoes relevantes do modulo.

### Escopo intencional desta sprint

- sem criacao de novas tabelas;
- sem consultas pesadas;
- sem carga desnecessaria de entidades completas;
- base preparada para evolucoes incrementais nas proximas sprints.

## Sprint 2 - Relatorios de chamados e atendimento

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Relatorios de chamados implementados  
Status tecnico: Sprint 2 concluida apos validacao  
Percentual: 45%

### Entregas

- endpoints de relatorio publicados:
  - `GET /api/admin/relatorios-avancados/chamados/resumo`
  - `GET /api/admin/relatorios-avancados/chamados/serie-temporal`
  - `GET /api/admin/relatorios-avancados/chamados/distribuicao`
  - `GET /api/admin/relatorios-avancados/atendimento/produtividade`
- filtros por query string para periodo, departamento, categoria, subcategoria, prioridade, status, atendente, solicitante, catalogo, ativo vinculado e origem;
- validacao de filtros para intervalo de datas e agrupamento temporal (`Dia`, `Semana`, `Mes`);
- consultas agregadas com `AsNoTracking`, filtros no banco e projecao para DTOs de leitura;
- relatorio de resumo com totais consolidados, distribuicoes por prioridade/departamento/categoria e metricas de tempo quando confiaveis;
- serie temporal de chamados (abertos, encerrados e reabertos);
- distribuicao de chamados por status, prioridade, departamento, categoria, catalogo, atendente, solicitante e ativo vinculado;
- produtividade operacional por atendente com limite seguro de ranking;
- testes automatizados de autorizacao, validacao de filtros e agregacoes principais.

### Situacao atual do modulo

Fundacao tecnica e relatorios avancados de chamados/atendimento implementados.  
Pendencias planejadas: relatorios de SLA, aprovacoes, catalogo, inventario, base de conhecimento, auditoria, frontend de dashboards, exportacoes e homologacao institucional.

## Sprint 3 - SLA, aprovacoes e catalogo de servicos

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Relatorios de SLA, aprovacoes e catalogo implementados  
Status tecnico: Sprint 3 concluida apos validacao  
Percentual: 60%

### Entregas

- endpoints de SLA publicados:
  - `GET /api/admin/relatorios-avancados/sla/resumo`
  - `GET /api/admin/relatorios-avancados/sla/violacoes`
  - `GET /api/admin/relatorios-avancados/sla/por-departamento`
  - `GET /api/admin/relatorios-avancados/sla/por-prioridade`
- endpoints de aprovacoes publicados:
  - `GET /api/admin/relatorios-avancados/aprovacoes/resumo`
  - `GET /api/admin/relatorios-avancados/aprovacoes/tempo-medio`
  - `GET /api/admin/relatorios-avancados/aprovacoes/por-origem`
- endpoints de catalogo de servicos publicados:
  - `GET /api/admin/relatorios-avancados/catalogo-servicos/resumo`
  - `GET /api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados`
  - `GET /api/admin/relatorios-avancados/catalogo-servicos/por-departamento`
- filtros comuns reaproveitados com validacao de periodo, limites e enums;
- aplicacao de seguranca por permissao com combinacao de `Visualizar` + escopo `Gerencial` ou `Operacional`;
- consultas com `AsNoTracking`, filtro no banco e projecao para DTOs;
- testes automatizados cobrindo agregacoes, filtros, validacoes e autorizacao dos novos endpoints.

### Limitacoes documentadas

- `ChamadosProximosVencimento` e indicadores sem base confiavel permanecem como `null`;
- metricas dependentes de dados ambiguos nao sao inventadas;
- backlog do modulo segue com inventario, base de conhecimento, auditoria, frontend de dashboards, exportacoes e homologacao institucional.

### Situacao atual do modulo

Fundacao tecnica, relatorios de chamados/atendimento e relatorios de SLA, aprovacoes e Catalogo de Servicos implementados.  
O modulo possui permissoes, metadados, indicadores de chamados, series temporais, distribuicoes, produtividade operacional, cumprimento de SLA, violacoes, tempo medio de aprovacao, aprovacoes por origem, servicos mais solicitados e chamados por catalogo.

## Sprint 4 - Inventario, base de conhecimento e auditoria

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Relatorios institucionais implementados  
Status tecnico: Sprint 4 concluida apos validacao  
Percentual: 75%

### Entregas

- endpoints de inventario/ativos publicados:
  - `GET /api/admin/relatorios-avancados/inventario-ativos/resumo`
  - `GET /api/admin/relatorios-avancados/inventario-ativos/por-status`
  - `GET /api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes`
  - `GET /api/admin/relatorios-avancados/inventario-ativos/por-departamento`
- endpoints de base de conhecimento publicados:
  - `GET /api/admin/relatorios-avancados/base-conhecimento/resumo`
  - `GET /api/admin/relatorios-avancados/base-conhecimento/por-status`
  - `GET /api/admin/relatorios-avancados/base-conhecimento/vinculos-chamados`
- endpoints de auditoria publicados:
  - `GET /api/admin/relatorios-avancados/auditoria/resumo`
  - `GET /api/admin/relatorios-avancados/auditoria/por-usuario`
  - `GET /api/admin/relatorios-avancados/auditoria/por-entidade`
- novos filtros institucionais para inventario, base de conhecimento e auditoria;
- validacoes de periodo, limite, enums e termo de busca em auditoria;
- aplicacao de permissao por escopo (`Visualizar` + `Gerencial` para consolidacoes e `Visualizar` + `Auditoria` para trilha auditavel);
- consultas com `AsNoTracking`, filtro no banco e projecao para DTOs;
- testes automatizados para filtros, agregacoes, validacoes e autorizacao.

### Limitacoes documentadas

- metricas dependentes de campos inexistentes ou ambiguos permanecem `null` ou omitidas;
- nao foram criadas tabelas novas de auditoria; os relatorios usam a trilha existente;
- metricas de leitura/acesso de artigos nao foram inventadas e seguem como evolucao futura.

### Situacao atual do modulo

Fundacao tecnica, relatorios de chamados/atendimento, SLA, aprovacoes, catalogo, inventario, base de conhecimento e auditoria implementados.  
O modulo ja possui permissoes, metadados e indicadores operacionais, gerenciais e institucionais cobrindo volume de chamados, produtividade, SLA, aprovacoes, servicos, ativos, conhecimento e auditoria.  
Pendencias planejadas: frontend de dashboards, exportacoes avancadas e homologacao institucional.

## Sprint 5 - Frontend administrativo e dashboards

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Frontend e dashboards implementados  
Status tecnico: Sprint 5 concluida apos validacao  
Percentual: 85%

### Entregas

- types frontend dedicados para o modulo de relatorios avancados;
- service administrativo com metodos para chamados, SLA, aprovacoes, catalogo, inventario, base e auditoria;
- utilitario de exportacao CSV simples, limitado a dados ja carregados em tela;
- dashboard administrativo geral com metadados, cards e atalhos por area;
- telas administrativas principais:
  - relatorios avancados (dashboard)
  - chamados
  - SLA
  - aprovacoes
  - catalogo de servicos
  - inventario/ativos
  - base de conhecimento
  - auditoria
- rotas administrativas de relatorios adicionadas em `/admin/relatorios/*`;
- menu administrativo com secao dedicada de relatorios;
- controle visual por permissao (`Visualizar`, `Gerencial`, `Operacional`, `Auditoria`, `Exportar`);
- estados de loading, erro e vazio tratados nas telas;
- testes unitarios de service, utilitario CSV e specs basicas das views principais.

### Limitacoes documentadas

- exportacao CSV nesta sprint e apenas de dados ja carregados no frontend;
- nao ha exportacao massiva backend nesta entrega;
- dashboards configuraveis e exportacoes avancadas seguem como evolucao posterior.

### Situacao atual do modulo

Fundacao tecnica, backend de relatorios avancados e frontend administrativo de dashboards implementados.  
O modulo contempla metadados, relatorios de chamados, atendimento, SLA, aprovacoes, catalogo, inventario, base de conhecimento e auditoria, com filtros, cards, tabelas, controle por permissoes e exportacao CSV simples quando permitida.  
Pendencias planejadas: homologacao institucional, evidencias formais, exportacoes avancadas, dashboards configuraveis e otimizacoes futuras.

## Sprint 6 - Fechamento funcional e preparacao de homologacao

Area: Relatorios avancados  
Categoria: Relatorios  
Status da implementacao: Implementado funcionalmente  
Status tecnico: Homologacao funcional preparada  
Percentual: 90%

### Consolidado funcional do modulo

- visao geral do modulo consolidada para leitura gerencial e operacional;
- permissoes ativas:
  - `RelatoriosAvancados.Visualizar`
  - `RelatoriosAvancados.Exportar`
  - `RelatoriosAvancados.Gerencial`
  - `RelatoriosAvancados.Operacional`
  - `RelatoriosAvancados.Auditoria`
- endpoint de metadados:
  - `GET /api/admin/relatorios-avancados/metadados`
- relatorios backend implementados:
  - chamados e atendimento
  - SLA
  - aprovacoes
  - catalogo de servicos
  - inventario/ativos
  - base de conhecimento
  - auditoria
- frontend administrativo implementado:
  - dashboard geral
  - telas por area
  - filtros
  - cards
  - tabelas
  - controle por permissao
  - exportacao CSV simples de dados carregados

### Filtros disponiveis

- periodo: `DataInicial`, `DataFinal`, `PeriodoPreDefinido`;
- organizacao e catalogacao: `DepartamentoId`, `CategoriaId`, `SubcategoriaId`, `CatalogoServicoId`;
- fluxo de atendimento: `PrioridadeId`, `StatusId`, `Status`, `AtendenteId`, `SolicitanteId`, `Origem`;
- SLA e aprovacoes: `PoliticaSlaId`, `SituacaoSla`, `TipoOrigemAprovacao`, `StatusAprovacao`;
- inventario: `LocalUnidadeId`, `UsuarioResponsavelId`, `TipoAtivoInventarioId`, `StatusOperacional`, `StatusPatrimonial`, `Criticidade`, `Ativo`;
- base e auditoria: `StatusArtigo`, `VisibilidadeArtigo`, `UsuarioId`, `Entidade`, `TipoAcao`, `Termo`.

### Exportacao CSV simples

- exportacao limitada aos dados ja carregados na tela;
- sem processamento massivo e sem exportacao backend adicional nesta sprint;
- botao de exportacao visivel apenas para quem possui `RelatoriosAvancados.Exportar`;
- nomes de arquivo coerentes por dominio de relatorio.

### Ajuste tecnico de dashboard (pos Sprint 6)

- dashboard administrativo passa a carregar, no bootstrap:
  - `GET /api/admin/relatorios-avancados/metadados`
  - `GET /api/admin/relatorios-avancados/chamados/resumo`
  - `GET /api/admin/relatorios-avancados/aprovacoes/resumo`
  - `GET /api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados`
  - `GET /api/admin/relatorios-avancados/sla/resumo` (quando houver permissao gerencial)
  - `GET /api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes` (quando houver permissao gerencial)
  - `GET /api/admin/relatorios-avancados/auditoria/resumo` (somente com permissao de auditoria)
- periodo padrao do dashboard definido como ultimos 30 dias corridos:
  - `DataInicial = hoje - 30 dias`
  - `DataFinal = hoje`
  - formato enviado: `YYYY-MM-DDTHH:mm:ss.fffZ` (UTC, inicio/fim do dia)
- query string do dashboard com limpeza de filtros antes do envio:
  - remove `undefined`, `null`, string vazia, array vazio e `Guid.Empty`;
  - remove `Agrupamento` e `AgruparPor` de endpoints de resumo;
  - nao envia chaves fora do contrato de cada endpoint;
- card de "Servicos mais usados" passa a usar ranking de `mais-solicitados` e exibe servico lider com quantidade de chamados;
- card de "Ativos com chamados" passa a usar o ativo lider em recorrencia (`codigo + quantidade`);
- sem permissao: card exibe `Sem permissao`;
- sem permissao de auditoria: endpoint de auditoria nao e chamado e o card exibe `Sem permissao`;
- falha de endpoint:
  - `400`: `Filtro invalido`
  - `401/403`: `Sem permissao`
  - `404`: `Endpoint nao encontrado`
  - `500+`: `Erro interno ao carregar`
  - falha de rede: `API indisponivel`
  - fallback: `Erro ao carregar`
  sem derrubar o dashboard inteiro;
- resposta vazia de ranking: card exibe `Sem dados no periodo`;
- valor numerico `0` permanece apenas quando a API retorna total valido igual a zero.
- exibicao de metadados no dashboard:
  - endpoint de metadados permanece ativo e consumido pelo frontend;
  - tela principal prioriza cards e acesso rapido (visao gerencial);
  - a secao tecnica de metadados foi removida da visualizacao principal para usuario final;
  - metadados continuam disponiveis no backend em `GET /api/admin/relatorios-avancados/metadados`;
  - textos da interface foram revisados com ortografia e acentuacao para linguagem orientada ao usuario.

### Limitacoes conhecidas

- metricas sem base confiavel permanecem `null` ou omitidas;
- nao ha exportacao avancada (Excel/PDF) nesta etapa;
- nao ha agendamento ou envio automatico de relatorios nesta etapa;
- nao existe framework E2E dedicado no projeto frontend no momento.

### Revisao UX (Sprint 6)

Arquivos revisados:

- `RelatoriosAvancadosDashboardPage.vue`
- `RelatoriosChamadosPage.vue`
- `RelatoriosSlaPage.vue`
- `RelatoriosAprovacoesPage.vue`
- `RelatoriosCatalogoServicosPage.vue`
- `RelatoriosInventarioAtivosPage.vue`
- `RelatoriosBaseConhecimentoPage.vue`
- `RelatoriosAuditoriaPage.vue`

Resultado da revisao:

- loading, erro e vazio tratados nas telas principais;
- filtros, cards e tabelas presentes e com mensagens de apoio amigaveis;
- permissao visual aplicada por tipo de relatorio e por acao de exportacao;
- responsividade basica mantida pelo padrao de grid/cards/tabelas do projeto;
- sem `console.log`, `debugger`, `TODO` ou `FIXME` indevidos nos arquivos revisados.

### Revisao de seguranca (Sprint 6)

- endpoints backend seguem protegidos por policies de permissao;
- auditoria segue com exigencia de `RelatoriosAvancados.Auditoria`;
- exportacao visual no frontend segue condicionada a `RelatoriosAvancados.Exportar`;
- frontend nao e barreira principal; backend continua como barreira efetiva;
- usuarios sem permissao nao acessam relatorios restritos;
- nao houve relaxamento de permissao nesta sprint.

### Revisao basica de performance (Sprint 6)

- consultas de relatorios mantidas com `AsNoTracking`;
- filtros aplicados no banco;
- projecao para DTO nas consultas agregadas;
- sem carga desnecessaria de entidades completas;
- sem exportacao massiva nesta sprint;
- exportacao CSV restrita a dados ja carregados;
- metricas sem base confiavel permanecem nulas/omitidas;
- cache/materializacao ficam como evolucao futura.

### Checklist e evidencias

- checklist de homologacao criado em:
  - `docs/CHECKLIST-HOMOLOGACAO-RELATORIOS-AVANCADOS.md`
- estrutura de evidencias criada em:
  - `docs/evidencias/relatorios-avancados/README.md`

### Validacoes executadas (Sprint 6)

- `dotnet build` Release: OK;
- testes backend: OK (`785` aprovados);
- `npm run test:unit`: OK (`99` aprovados);
- `npm run build`: OK.

### Pendencias evolutivas

- construtor de relatorios dinamicos;
- selecao de fonte de dados;
- escolha de campos;
- agrupamentos configuraveis;
- exportacoes avancadas;
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- exportacao avancada Excel;
- exportacao PDF;
- relatorios agendados;
- envio automatico por e-mail;
- dashboards configuraveis por perfil;
- cache de indicadores;
- materialized views;
- integracao com BI externo;
- graficos avancados;
- comparativos mes a mes;
- metas por departamento;
- relatorios por centro de custo;
- trilha de auditoria exportavel;
- otimizacoes para alto volume.
