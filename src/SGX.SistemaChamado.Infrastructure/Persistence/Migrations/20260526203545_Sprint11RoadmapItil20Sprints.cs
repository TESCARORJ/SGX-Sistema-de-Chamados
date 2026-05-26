using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint11RoadmapItil20Sprints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roadmap_categorias",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "cor", "criado_em", "criado_por", "descricao", "icone", "nome", "ordem" },
                values: new object[] { new Guid("66666666-6666-6666-6666-666666666616"), true, null, null, "#0D47A1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Roadmap corporativo orientado a processos ITIL.", "route", "ITIL/ITSM", 16 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777714"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777715"),
                column: "ativo",
                value: false);

            migrationBuilder.InsertData(
                table: "roadmap_itsm_itens",
                columns: new[] { "id", "area", "atencao_tecnica", "ativo", "atualizado_em", "atualizado_por", "categoria", "criado_em", "criado_por", "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "decisao", "evidencia_implementacao", "impacto", "objetivo", "observacao", "ordem", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "prazo_alvo", "prioridade", "proxima_acao", "responsavel", "roadmap_categoria_id", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777716"), "Sprint 1 - Fundacao ITSM do chamado", "Definir natureza obrigatoria e migrar chamados legados sem perda de historico.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Todo chamado possui natureza ITSM e ela influencia status, SLA, campos obrigatorios, permissoes, relatorios e acoes.", null, null, 1, "Planejamento consolidado para Sprint 1 da trilha ITIL.", 1, "Criar a base para que todo chamado tenha natureza ITSM obrigatoria e regras operacionais por natureza.", "Sprint estruturante da trilha ITIL.", 101, "Validar fluxo completo com cada natureza ITSM em ambiente de homologacao.", "Natureza obrigatoria, impacto x urgencia, regras por tipo no portal, e-mail, atendimento, dashboard e relatorios.", 45, null, 1, "Modelar NaturezaChamado, matriz impacto x urgencia e migracao de dados legados.", "Time Produto/Arquitetura", new Guid("66666666-6666-6666-6666-666666666616"), "Modelo atual ainda e generico, com categoria, prioridade e SLA desacoplados da natureza ITSM.", 3, 2, 8 },
                    { new Guid("77777777-7777-7777-7777-777777777717"), "Sprint 2 - Gerenciamento de Incidentes", "Separar status, campos e SLA de incidente sem quebrar fluxo atual.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Incidente deve ser aberto, classificado, priorizado, atendido, resolvido, reaberto e fechado com rastreabilidade.", null, null, 1, "Fluxo alvo definido no novo roadmap ITIL.", 1, "Formalizar fluxo de Incidente para falha, indisponibilidade ou degradacao de servico.", null, 102, "Homologar ciclo abrir, triar, atender, resolver, reabrir e fechar.", "Servico afetado, CI afetado, causa provavel, solucao de contorno e regra de reabertura.", 40, null, 1, "Implementar estados de incidente e campos especificos no chamado.", "Time Atendimento", new Guid("66666666-6666-6666-6666-666666666616"), "Chamados operacionais existem, mas sem trilha completa de incidente com diagnostico e workaround.", 3, 2, 1 },
                    { new Guid("77777777-7777-7777-7777-777777777718"), "Sprint 3 - Gerenciamento de Requisicoes", "Orquestrar formulario por servico, aprovacao e atendimento sem duplicar regras.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Requisicao nasce do Catalogo e aplica formulario, aprovacao, SLA e grupo responsavel.", null, null, 1, "Capacidades base existentes de catalogo e aprovacao aproveitadas.", 1, "Formalizar fluxo de Requisicao de Servico com abertura preferencial via Catalogo.", null, 103, "Validar abertura guiada por catalogo com regras diferentes por servico.", "Fluxo de aprovacao por servico, status proprios, servicos relacionados e conclusao com aceite.", 45, null, 1, "Vincular fluxo de requisicao ao catalogo no backend e frontend.", "Time Atendimento", new Guid("66666666-6666-6666-6666-666666666616"), "Catalogo e aprovacao existem, porem sem fluxo separado de requisicao.", 3, 2, 1 },
                    { new Guid("77777777-7777-7777-7777-777777777719"), "Sprint 4 - Catalogo de Servicos 2.0", "Adicionar tipo padrao, SLA padrao, grupo tecnico e formulario dinamico por servico.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Selecionar servico deve sugerir/preencher tipo, categoria, SLA, prioridade, grupo e aprovacao.", null, null, 1, "Catalogo atual reaproveitado como base da evolucao 2.0.", 1, "Transformar catalogo em motor de abertura guiada com regras operacionais por servico.", null, 104, "Homologar abertura guiada com servicos reais e validacao de aprovacoes.", "Campos obrigatorios dinamicos, sugestoes automaticas e visibilidade por perfil refinada.", 60, null, 1, "Evoluir entidade de catalogo e contrato de abertura guiada.", "Time Catalogo", new Guid("66666666-6666-6666-6666-666666666616"), "Modulo de catalogo esta implementado funcionalmente, com evolucoes ITIL pendentes.", 3, 3, 3 },
                    { new Guid("77777777-7777-7777-7777-777777777720"), "Sprint 5 - Grupos tecnicos, filas e atribuicao", "Introduzir novas entidades sem regressao no fluxo de atribuicao atual.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamados podem ser direcionados, assumidos, transferidos e auditados por grupo tecnico.", null, null, 1, "Escopo sprint definido.", 1, "Criar filas corporativas por grupo tecnico com roteamento e transferencia.", null, 105, "Validar produtividade e visibilidade por grupo tecnico.", "Cadastro de grupos, membros, fila, roteamento, transferencia e escalonamento.", 10, null, 1, "Modelar GrupoTecnico e regras de roteamento inicial.", "Time Atendimento", new Guid("66666666-6666-6666-6666-666666666616"), "Existe responsavel por chamado, mas sem conceito formal de grupo tecnico e fila.", 4, 1, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777721"), "Sprint 6 - Observadores de chamados", "Garantir seguranca de visibilidade e evitar elevacao indevida de permissao.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado aceita observadores com regras claras de visualizacao, comentario e notificacao.", null, null, 1, "Escopo sprint definido.", 2, "Permitir acompanhamento controlado por observadores sem atribuir responsabilidade operacional.", null, 106, "Validar comportamento de observador solicitante, tecnico e gestor.", "Entidade ObservadorChamado, regras por tipo e notificacoes por evento.", 0, null, 2, "Criar modelo e contratos de API para observadores.", "Time Produto", new Guid("66666666-6666-6666-6666-666666666616"), "Nao ha entidade dedicada para observadores com regras de comentario e notificacao.", 4, 0, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777722"), "Sprint 7 - Motor de Aprovacoes ITSM", "Generalizar aprovacao preservando compatibilidade com fluxo atual.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamados com aprovacao obrigatoria ficam bloqueados ate decisao formal registrada.", null, null, 1, "Modulo de aprovacao existente reutilizado como base.", 1, "Evoluir aprovacao para motor reutilizavel por tipo de chamado e servico sensivel.", null, 107, "Homologar casos sensiveis como custo, acesso e mudanca emergencial.", "Aprovacao por grupo, multi-nivel, aprovador padrao e bloqueio por decisao pendente.", 55, null, 1, "Introduzir regra de aprovacao por servico e natureza ITSM.", "Time Atendimento", new Guid("66666666-6666-6666-6666-666666666616"), "Aprovacao atual cobre base funcional, sem motor multi-nivel completo.", 3, 3, 3 },
                    { new Guid("77777777-7777-7777-7777-777777777723"), "Sprint 8 - SLA 2.0, OLA e matriz impacto x urgencia", "Garantir consistencia entre calculo de prioridade, calendario e eventos SLA.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "SLA aplicado por tipo, servico, prioridade, calendario e grupo, com pausas e violacoes registradas.", null, null, 1, "Base SLA existente, com eventos e dashboards, reaproveitada.", 1, "Evoluir SLA para regras corporativas por tipo, servico, prioridade e grupo tecnico.", null, 108, "Validar SLA por tipo e servico com cenarios reais de violacao.", "Matriz impacto x urgencia, OLA por grupo, pausa com motivo e escalonamento automatico.", 60, null, 1, "Implementar matriz impacto x urgencia e politicas por natureza ITSM.", "Time SLA", new Guid("66666666-6666-6666-6666-666666666616"), "Modulo SLA atual esta implementado e precisa da matriz impacto x urgencia e OLA por grupo.", 3, 3, 3 },
                    { new Guid("77777777-7777-7777-7777-777777777724"), "Sprint 9 - Regras de fechamento, aceite e reabertura", "Separar resolvido de fechado e exigir dados obrigatorios de solucao/cancelamento.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fluxo contempla resolucao, aceite/rejeicao, fechamento automatico e reabertura auditavel.", null, null, 1, "Base de encerramento/reabertura existente reaproveitada.", 1, "Criar governanca de encerramento com aceite, fechamento automatico e reabertura controlada.", null, 109, "Validar regras com solicitantes e atendentes reais.", "Aceite, prazo de auto-fechamento, motivo de cancelamento e campo solucao obrigatorio.", 50, null, 1, "Evoluir estados e regras de negocio de ciclo de vida.", "Time Atendimento", new Guid("66666666-6666-6666-6666-666666666616"), "Encerrar e reabrir existem, mas faltam aceite do solicitante e politicas formais.", 3, 2, 1 },
                    { new Guid("77777777-7777-7777-7777-777777777725"), "Sprint 10 - Notificacoes ITSM", "Definir modelo de notificacao sem acoplamento excessivo com canais externos.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Usuarios recebem notificacoes persistentes conforme eventos e regras configuradas.", null, null, 1, "Escopo sprint definido.", 2, "Criar notificacoes persistentes e configuraveis por evento, perfil e participacao.", null, 110, "Validar recebimento por perfil, observador, aprovador e grupo tecnico.", "Tabela de notificacoes, API leitura/nao lida, preferencias e regras por evento.", 15, null, 2, "Modelar entidade Notificacao e pipeline de eventos.", "Time Produto", new Guid("66666666-6666-6666-6666-666666666616"), "Notificacoes ainda nao estao consolidadas como modulo persistente por evento ITSM.", 4, 1, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777726"), "Sprint 11 - Relatorios ITSM avancados", "Alinhar contratos existentes de relatorios com nova taxonomia de chamado.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Gestores autorizados consultam/exportam indicadores por tipo, servico, SLA, grupo, atendente e origem.", null, null, 1, "Base de relatorios avancados existente reaproveitada.", 1, "Criar relatorios por fluxo ITIL com filtros por tipo, grupo, servico, SLA e periodo.", null, 111, "Validar indicadores por perfil gestor e exportacao institucional.", "Filtros por tipo/grupo/servico e exportacoes alinhadas ao novo modelo ITSM.", 55, null, 1, "Evoluir filtros e agregacoes para natureza ITSM.", "Time Gestao", new Guid("66666666-6666-6666-6666-666666666616"), "Modulo de relatorios avancados existe e precisa consolidar visao por natureza ITSM.", 3, 3, 3 },
                    { new Guid("77777777-7777-7777-7777-777777777727"), "Sprint 12 - Gerenciamento de Mudancas", "Modelar RFC, risco, impacto, janela, plano de execucao e rollback com aprovacao.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Mudancas possuem fluxo proprio com analise, aprovacao, execucao, rollback e encerramento.", null, null, 1, "Sprint planejada no novo roadmap.", 1, "Criar fluxo completo de mudanca (padrao, normal e emergencial) separado de incidente e requisicao.", null, 112, "Executar ciclo completo RFC ate revisao pos-implementacao.", "Entidades de mudanca, status proprios e aprovacao especifica por tipo.", 0, null, 1, "Modelar dominio de Mudanca e contratos de API.", "Time ITSM", new Guid("66666666-6666-6666-6666-666666666616"), "Fluxo de mudanca ainda nao esta implementado como processo dedicado.", 4, 0, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777728"), "Sprint 13 - CMDB e Itens de Configuracao", "Migrar conceito sem perder dados de ativos existentes.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sistema permite registrar CIs, dependencias e vinculos com chamados, mudancas e servicos.", null, null, 1, "Inventario atual reutilizado como alicerce da CMDB.", 1, "Evoluir inventario/ativos para CMDB com relacionamentos entre CIs e vinculos com chamados.", null, 113, "Validar mapa basico de dependencias com cenario real.", "Tipos de CI, relacionamentos, vinculos CI-servico, CI-chamado e CI-mudanca.", 40, null, 1, "Planejar migracao de InventarioAtivo para ItemConfiguracao.", "Time Infraestrutura", new Guid("66666666-6666-6666-6666-666666666616"), "Inventario existe e funciona como base, mas sem malha de dependencias de CMDB.", 3, 2, 1 },
                    { new Guid("77777777-7777-7777-7777-777777777729"), "Sprint 14 - Analise de impacto", "Depende da maturidade de CMDB e relacionamento entre entidades.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Ao analisar incidente/mudanca, sistema mostra servicos e CIs potencialmente impactados.", null, null, 1, "Sprint planejada.", 2, "Permitir avaliacao de impacto para incidentes, mudancas, problemas e servicos.", null, 114, "Validar cenarios de incidente critico e mudanca de alto risco.", "Consultas de dependencias, criticidade e relatorio de impacto.", 10, null, 2, "Modelar visoes de impacto por servico e CI.", "Time Gestao", new Guid("66666666-6666-6666-6666-666666666616"), "Existem dados de ativos e servicos, porem sem visao consolidada de impacto.", 4, 1, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777730"), "Sprint 15 - Gerenciamento de Problemas", "Conectar problema com incidentes recorrentes e base de conhecimento.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sistema permite abrir problema, vincular incidentes e registrar causa raiz com rastreabilidade.", null, null, 1, "Sprint planejada.", 2, "Tratar causa raiz e recorrencia com vinculo entre problemas, incidentes e mudancas.", null, 115, "Validar ciclo investigacao ate encerramento com caso real recorrente.", "Entidade Problema, RCA, erro conhecido, workaround e vinculacao de incidentes.", 5, null, 2, "Definir modelo de Problema e integrações com incidente/mudanca.", "Time ITSM", new Guid("66666666-6666-6666-6666-666666666616"), "Nao existe registro dedicado de problema com erro conhecido e RCA.", 4, 1, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777731"), "Sprint 16 - Pesquisa de satisfacao", "Definir disparo, anonimato opcional e consolidacao de indicadores.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Solicitante avalia atendimento apos fechamento e gestao consulta indicadores consolidados.", null, null, 2, "Sprint planejada.", 2, "Medir qualidade de atendimento apos resolucao/fechamento com indicadores por atendente e grupo.", null, 116, "Validar taxa de resposta e consistencia de indicadores.", "Modelo de pesquisa, envio automatico e dashboard de satisfacao.", 0, null, 3, "Modelar entidade PesquisaSatisfacao e evento de disparo.", "Time Gestao", new Guid("66666666-6666-6666-6666-666666666616"), "Nao ha mecanismo persistente de pesquisa de satisfacao no fluxo atual.", 4, 0, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777732"), "Sprint 17 - Monitoramento, eventos e Zabbix", "Garantir idempotencia, autenticacao e correlacao segura de eventos.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Eventos externos criam/atualizam incidente automaticamente com rastreabilidade.", null, null, 1, "Sprint planejada com base no worker/integracoes atuais.", 2, "Permitir abertura e atualizacao automatica de incidentes por eventos externos de monitoramento.", null, 117, "Validar com eventos reais de monitoramento em ambiente controlado.", "Endpoint de integracao, token, mapeamento evento-servico/CI e anti-duplicidade.", 10, null, 2, "Definir contrato de webhook e camada de correlacao de alerta.", "Time Integracoes", new Guid("66666666-6666-6666-6666-666666666616"), "Integracao de e-mail existe, mas nao ha endpoint dedicado para eventos de monitoramento.", 4, 1, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777733"), "Sprint 18 - Base de Conhecimento 2.0", "Reaproveitar modulo atual sem romper buscas e relacionamentos existentes.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Base de conhecimento apoia incidentes, problemas e requisicoes com sugestoes relevantes.", null, null, 1, "Modulo de base de conhecimento atual reaproveitado para evolucao 2.0.", 2, "Conectar conhecimento aos fluxos ITIL com sugestao contextual e workflow editorial.", null, 118, "Validar ganho de produtividade no atendimento e autoatendimento.", "Sugestao por servico/erro conhecido, workflow editorial e avaliacao de utilidade.", 50, null, 2, "Implementar mecanismos de sugestao contextual.", "Time Conhecimento", new Guid("66666666-6666-6666-6666-666666666616"), "Base de conhecimento esta implementada e precisa evoluir para contexto por erro conhecido e servico.", 3, 3, 3 },
                    { new Guid("77777777-7777-7777-7777-777777777734"), "Sprint 19 - Homologacao institucional ITSM", "Consolidar evidencias sem perder rastreabilidade historica dos modulos ja entregues.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fluxos principais executados por usuarios reais com evidencias e aprovacao da gestao.", null, null, 1, "Estrutura de checklists e pastas de evidencias ja disponivel no repositorio.", 1, "Validar fluxos principais com usuarios reais e evidencias formais de aceite.", null, 119, "Executar homologacao formal com area gestora e registrar aceite/ressalvas.", "Pacote de cenario integrado incidente, requisicao, mudanca, SLA, catalogo e notificacoes.", 85, null, 1, "Montar bateria institucional de testes homologatorios por processo ITIL.", "Time Homologacao", new Guid("66666666-6666-6666-6666-666666666616"), "Checklists e evidencias por modulo existem; falta consolidacao institucional ponta a ponta por processo ITIL.", 2, 4, 4 },
                    { new Guid("77777777-7777-7777-7777-777777777735"), "Sprint 20 - Produto, implantacao e operacao", "Consolidar runbooks, suporte, monitoramento e postura de produto SaaS corporativo.", true, null, null, "ITIL/ITSM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "SGX deve estar documentado, implantavel, demonstravel e operavel com responsabilidades claras.", null, null, 2, "Documentacao existente de implantacao e execucao local como base.", 1, "Preparar SGX para implantacao institucional/comercial com operacao sustentavel.", null, 120, "Validar readiness operacional com time de sustentacao e negocio.", "Checklist de producao, backup, logs, monitoramento, suporte e materiais de treinamento.", 35, null, 2, "Consolidar plano de go-live e manuais por perfil de uso.", "Time Produto", new Guid("66666666-6666-6666-6666-666666666616"), "Ha documentacao tecnica dispersa, sem pacote unico de operacao e go-live corporativo.", 3, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000101"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 1 Fundacao ITSM do chamado", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777716"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000102"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 1 Fundacao ITSM do chamado", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777716"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000103"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 1 Fundacao ITSM do chamado", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777716"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000104"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 1 Fundacao ITSM do chamado", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777716"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000105"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Gerenciamento de Incidentes", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777717"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000106"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Gerenciamento de Incidentes", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777717"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000107"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Gerenciamento de Incidentes", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777717"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000108"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Gerenciamento de Incidentes", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000109"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Gerenciamento de Requisicoes", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777718"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000110"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Gerenciamento de Requisicoes", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000111"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Gerenciamento de Requisicoes", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777718"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000112"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Gerenciamento de Requisicoes", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777718"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000113"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 4 Catalogo de Servicos 2.0", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777719"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000114"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 4 Catalogo de Servicos 2.0", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777719"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000115"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 4 Catalogo de Servicos 2.0", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777719"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000116"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 4 Catalogo de Servicos 2.0", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000117"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Grupos tecnicos, filas e atribuicao", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777720"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000118"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Grupos tecnicos, filas e atribuicao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777720"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000119"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Grupos tecnicos, filas e atribuicao", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777720"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000120"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Grupos tecnicos, filas e atribuicao", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777720"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000121"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Observadores de chamados", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777721"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000122"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Observadores de chamados", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777721"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000123"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Observadores de chamados", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777721"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000124"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Observadores de chamados", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777721"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000125"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Motor de Aprovacoes ITSM", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777722"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000126"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Motor de Aprovacoes ITSM", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777722"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000127"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Motor de Aprovacoes ITSM", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777722"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000128"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Motor de Aprovacoes ITSM", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777722"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000129"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 SLA 2.0 OLA e matriz impacto x urgencia", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777723"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000130"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 SLA 2.0 OLA e matriz impacto x urgencia", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777723"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000131"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 SLA 2.0 OLA e matriz impacto x urgencia", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777723"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000132"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 SLA 2.0 OLA e matriz impacto x urgencia", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777723"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000133"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Regras de fechamento, aceite e reabertura", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777724"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000134"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Regras de fechamento, aceite e reabertura", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777724"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000135"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Regras de fechamento, aceite e reabertura", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777724"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000136"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Regras de fechamento, aceite e reabertura", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000137"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 10 Notificacoes ITSM", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777725"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000138"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 10 Notificacoes ITSM", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000139"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 10 Notificacoes ITSM", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777725"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000140"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 10 Notificacoes ITSM", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777725"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000141"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 11 Relatorios ITSM avancados", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777726"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000142"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 11 Relatorios ITSM avancados", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777726"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000143"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 11 Relatorios ITSM avancados", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777726"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000144"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 11 Relatorios ITSM avancados", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777726"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000145"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 12 Gerenciamento de Mudancas", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777727"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000146"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 12 Gerenciamento de Mudancas", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777727"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000147"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 12 Gerenciamento de Mudancas", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777727"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000148"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 12 Gerenciamento de Mudancas", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777727"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000149"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 13 CMDB e Itens de Configuracao", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777728"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000150"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 13 CMDB e Itens de Configuracao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777728"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000151"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 13 CMDB e Itens de Configuracao", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777728"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000152"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 13 CMDB e Itens de Configuracao", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777728"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000153"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 14 Analise de impacto", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777729"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000154"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 14 Analise de impacto", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777729"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000155"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 14 Analise de impacto", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777729"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000156"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 14 Analise de impacto", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777729"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000157"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 15 Gerenciamento de Problemas", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777730"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000158"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 15 Gerenciamento de Problemas", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777730"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000159"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 15 Gerenciamento de Problemas", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777730"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000160"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 15 Gerenciamento de Problemas", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777730"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000161"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 16 Pesquisa de satisfacao", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777731"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000162"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 16 Pesquisa de satisfacao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777731"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000163"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 16 Pesquisa de satisfacao", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777731"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000164"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 16 Pesquisa de satisfacao", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777731"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000165"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 17 Monitoramento, eventos e Zabbix", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777732"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000166"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 17 Monitoramento, eventos e Zabbix", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777732"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000167"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 17 Monitoramento, eventos e Zabbix", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777732"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000168"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 17 Monitoramento, eventos e Zabbix", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777732"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000169"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 18 Base de Conhecimento 2.0", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777733"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000170"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 18 Base de Conhecimento 2.0", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777733"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000171"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 18 Base de Conhecimento 2.0", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777733"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000172"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 18 Base de Conhecimento 2.0", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777733"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000173"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 19 Homologacao institucional ITSM", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777734"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000174"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 19 Homologacao institucional ITSM", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777734"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000175"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 19 Homologacao institucional ITSM", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777734"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000176"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 19 Homologacao institucional ITSM", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777734"), "Registrar homologacao e aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000177"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 20 Produto, implantacao e operacao", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777735"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000178"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 20 Produto, implantacao e operacao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777735"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000179"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 20 Produto, implantacao e operacao", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777735"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000180"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 20 Produto, implantacao e operacao", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777735"), "Registrar homologacao e aceite" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000101"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000102"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000103"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000104"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000105"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000106"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000107"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000108"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000109"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000110"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000111"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000112"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000113"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000114"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000115"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000116"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000117"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000118"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000119"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000120"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000121"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000122"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000123"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000124"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000125"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000126"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000127"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000128"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000129"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000130"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000131"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000132"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000133"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000134"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000135"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000136"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000141"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000142"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000143"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000144"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000145"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000146"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000147"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000148"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000149"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000150"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000151"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000152"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000153"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000154"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000155"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000156"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000157"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000158"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000159"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000160"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000161"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000162"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000163"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000164"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000165"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000166"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000167"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000168"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000169"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000170"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000171"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000172"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000173"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000174"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000175"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000176"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000177"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000178"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000179"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000180"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777716"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777717"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777719"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777721"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777723"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777726"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777727"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777728"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777729"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777730"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777731"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777732"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777733"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777734"));

            migrationBuilder.DeleteData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777735"));

            migrationBuilder.DeleteData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666616"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777714"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777715"),
                column: "ativo",
                value: true);
        }
    }
}
