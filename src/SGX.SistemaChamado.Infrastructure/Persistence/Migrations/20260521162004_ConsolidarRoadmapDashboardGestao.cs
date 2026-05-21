using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidarRoadmapDashboardGestao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("74727272-7272-7272-7272-000000000001"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777709"), "Definir indicadores principais do dashboard." },
                    { new Guid("74727272-7272-7272-7272-000000000002"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777709"), "Definir filtros gerenciais." },
                    { new Guid("74727272-7272-7272-7272-000000000003"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777709"), "Definir visão para administrador e atendente." },
                    { new Guid("74727272-7272-7272-7272-000000000004"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 4, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de dashboard administrativo." },
                    { new Guid("74727272-7272-7272-7272-000000000005"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de chamados por status." },
                    { new Guid("74727272-7272-7272-7272-000000000006"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de chamados por prioridade." },
                    { new Guid("74727272-7272-7272-7272-000000000007"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de chamados por categoria." },
                    { new Guid("74727272-7272-7272-7272-000000000008"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de indicadores de SLA." },
                    { new Guid("74727272-7272-7272-7272-000000000009"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777709"), "Criar endpoint de produtividade por atendente." },
                    { new Guid("74727272-7272-7272-7272-000000000010"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777709"), "Aplicar ou validar policy granular Dashboard.Visualizar no backend." },
                    { new Guid("74727272-7272-7272-7272-000000000011"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777709"), "Validar performance das consultas agregadas." },
                    { new Guid("74727272-7272-7272-7272-000000000012"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777709"), "Validar regras de permissão por perfil." },
                    { new Guid("74727272-7272-7272-7272-000000000013"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777709"), "Criar tela administrativa de Dashboard." },
                    { new Guid("74727272-7272-7272-7272-000000000014"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777709"), "Criar cards de indicadores principais." },
                    { new Guid("74727272-7272-7272-7272-000000000015"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777709"), "Criar filtros do dashboard." },
                    { new Guid("74727272-7272-7272-7272-000000000016"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir indicadores por status." },
                    { new Guid("74727272-7272-7272-7272-000000000017"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir indicadores por prioridade." },
                    { new Guid("74727272-7272-7272-7272-000000000018"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir indicadores por categoria." },
                    { new Guid("74727272-7272-7272-7272-000000000019"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir indicadores de SLA." },
                    { new Guid("74727272-7272-7272-7272-000000000020"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir produtividade por atendente." },
                    { new Guid("74727272-7272-7272-7272-000000000021"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir fila resumida de chamados." },
                    { new Guid("74727272-7272-7272-7272-000000000022"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777709"), "Exibir resumo da integração de e-mail." },
                    { new Guid("74727272-7272-7272-7272-000000000023"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 23, new Guid("77777777-7777-7777-7777-777777777709"), "Refinar layout visual para apresentação gerencial." },
                    { new Guid("74727272-7272-7272-7272-000000000024"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 24, new Guid("77777777-7777-7777-7777-777777777709"), "Criar testes de use case do dashboard." },
                    { new Guid("74727272-7272-7272-7272-000000000025"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 25, new Guid("77777777-7777-7777-7777-777777777709"), "Criar testes de use case dos indicadores." },
                    { new Guid("74727272-7272-7272-7272-000000000026"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 26, new Guid("77777777-7777-7777-7777-777777777709"), "Criar testes HTTP de sucesso para /api/admin/dashboard." },
                    { new Guid("74727272-7272-7272-7272-000000000027"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777709"), "Criar testes HTTP de sucesso para /api/admin/indicadores/*." },
                    { new Guid("74727272-7272-7272-7272-000000000028"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 28, new Guid("77777777-7777-7777-7777-777777777709"), "Testar bloqueio por ausência de permissão granular, se a policy for aplicada." },
                    { new Guid("74727272-7272-7272-7272-000000000029"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 29, new Guid("77777777-7777-7777-7777-777777777709"), "Criar teste frontend/e2e, se aplicável." },
                    { new Guid("74727272-7272-7272-7272-000000000030"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 4, true, 30, new Guid("77777777-7777-7777-7777-777777777709"), "Registrar dashboard no roadmap geral." },
                    { new Guid("74727272-7272-7272-7272-000000000031"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 4, true, 31, new Guid("77777777-7777-7777-7777-777777777709"), "Criar documentação funcional específica do Dashboard / Gestão." },
                    { new Guid("74727272-7272-7272-7272-000000000032"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 4, true, 32, new Guid("77777777-7777-7777-7777-777777777709"), "Registrar evidências de homologação." },
                    { new Guid("74727272-7272-7272-7272-000000000033"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 5, true, 33, new Guid("77777777-7777-7777-7777-777777777709"), "Validar com administrador." },
                    { new Guid("74727272-7272-7272-7272-000000000034"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 5, true, 34, new Guid("77777777-7777-7777-7777-777777777709"), "Validar com atendente." },
                    { new Guid("74727272-7272-7272-7272-000000000035"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 5, true, 35, new Guid("77777777-7777-7777-7777-777777777709"), "Validar com massa real ou simulada." },
                    { new Guid("74727272-7272-7272-7272-000000000036"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 5, true, 36, new Guid("77777777-7777-7777-7777-777777777709"), "Registrar aceite funcional." },
                    { new Guid("74727272-7272-7272-7272-000000000037"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777709"), "Cards principais com abertos, atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período implementados." },
                    { new Guid("74727272-7272-7272-7272-000000000038"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777709"), "Navegação para fila de chamados, gestão de chamados e integração de e-mail implementada." },
                    { new Guid("74727272-7272-7272-7272-000000000039"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 2, true, 39, new Guid("77777777-7777-7777-7777-777777777709"), "Filtros por período, departamento, categoria e responsável implementados." },
                    { new Guid("74727272-7272-7272-7272-000000000040"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Dashboard / Gestão", 3, true, 40, new Guid("77777777-7777-7777-7777-777777777709"), "Dados consolidados coerentes com os registros persistidos em cenário funcional base." }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Validar se os indicadores respeitam corretamente as permissões internas do usuário autenticado. Confirmar se administradores visualizam a operação completa e se atendentes visualizam apenas o escopo permitido, caso essa regra seja exigida. Verificar performance das consultas em bases maiores, principalmente filtros por período, produtividade por atendente e agrupamentos por status, prioridade e categoria. Garantir que chamados inativos, registros históricos e dados de SLA sejam tratados corretamente para não distorcer os indicadores.", "Gestão", "O usuário autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da operação. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período. A tela deve permitir navegação para fila de chamados, gestão de chamados e integração de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.", "- src/SGX.SistemaChamado.Api/Controllers/AdminDashboardController.cs\n- src/SGX.SistemaChamado.Application/UseCases/Admin/AdminIndicadoresUseCases.cs\n- src/SGX.SistemaChamado.Application/DTOs/Admin/AdminDashboardDtos.cs\n- src/SGX.SistemaChamado.Web/src/services/dashboardAdminService.ts\n- src/SGX.SistemaChamado.Web/src/types/dashboard.ts\n- src/SGX.SistemaChamado.Web/src/views/AdminDashboardView.vue\n- tests/SGX.SistemaChamado.Tests/DashboardAdminUseCaseTests.cs\n- tests/SGX.SistemaChamado.Tests/IndicadoresUseCaseTests.cs", "Disponibilizar uma visão gerencial da operação de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no período, chamados sem responsável, riscos de SLA, distribuição por status, prioridade, categoria, produtividade por atendente e situação da integração de e-mail.", "Checklist ativo consolidado em 34/40 itens (85%), com pendências concentradas em policy granular, performance, testes HTTP/frontend e homologação.", "- Validar com Administrador.\n- Validar com Atendente.\n- Conferir números do dashboard contra consultas reais no banco.\n- Validar filtros por período, departamento, categoria e responsável.\n- Confirmar se os indicadores atendem à necessidade de gestão da operação.\n- Registrar evidências formais de homologação.", "- Aplicar ou validar permissão granular Dashboard.Visualizar no backend, além da proteção por perfil.\n- Validar performance com volume maior de chamados.\n- Criar ou consolidar testes automatizados específicos do dashboard em nível HTTP.\n- Criar testes frontend/e2e para dashboardAdminService e AdminDashboardView, se o projeto já tiver estrutura para isso.\n- Avaliar cache ou otimização das consultas agregadas, caso necessário.\n- Revisar regras de permissão dos indicadores por perfil.", 85, "Executar validação técnica e homologação funcional do dashboard com dados reais ou massa simulada mais próxima da operação institucional.", "Dashboard administrativo implementado funcionalmente no backend e frontend. A API disponibiliza indicadores consolidados, filtros por período e contexto administrativo. A interface apresenta cards gerenciais, gráficos/listagens por status, prioridade e categoria, indicadores de SLA, produtividade por atendente, fila de chamados e resumo da integração de e-mail. Pendente validação com usuários reais, refinamento visual final, testes frontend/e2e e homologação institucional.", 2, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000001"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000002"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000003"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000004"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000005"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000006"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000007"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000008"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000009"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000010"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000011"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000012"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000013"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000014"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000015"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000016"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000017"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000018"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000019"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000020"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000021"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000022"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000023"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000024"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000025"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000026"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000027"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000028"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000029"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000030"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000031"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000032"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000033"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000034"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000035"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000036"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000037"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000038"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000039"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000040"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Levar indicadores simples: abertos, vencidos, por status e por atendente", "Gestao", null, null, null, null, null, null, 0, null, "Previsto", 3, 0, 0 });
        }
    }
}
