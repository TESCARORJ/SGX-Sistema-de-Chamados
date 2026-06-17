using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData_Sprint5_FechamentoFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000133"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000134"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000135"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000136"),
                column: "ativo",
                value: false);

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000801"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777724"), "Planejar escopo e criterios de aceite da Sprint 5" },
                    { new Guid("78787878-7878-7878-7878-000000000802"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777724"), "Mapear fluxo atual de encerramento e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000803"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777724"), "Validar compatibilidade com Fundacao ITSM do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000804"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 4, new Guid("77777777-7777-7777-7777-777777777724"), "Validar compatibilidade com Sprint 4 Motor de Aprovacoes ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000805"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 4, true, 5, new Guid("77777777-7777-7777-7777-777777777724"), "Documentar modelo de ciclo de vida Resolvido/Fechado/Reaberto" },
                    { new Guid("78787878-7878-7878-7878-000000000806"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777724"), "Separar status Resolvido e Fechado no fluxo de negocio" },
                    { new Guid("78787878-7878-7878-7878-000000000807"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra para exigir solucao tecnica ao resolver chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000808"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra para exigir motivo ao cancelar chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000809"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de aceite do solicitante" },
                    { new Guid("78787878-7878-7878-7878-000000000810"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de rejeicao da solucao pelo solicitante" },
                    { new Guid("78787878-7878-7878-7878-000000000811"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de retorno ao atendimento apos rejeicao da solucao" },
                    { new Guid("78787878-7878-7878-7878-000000000812"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777724"), "Criar politica de fechamento automatico apos prazo de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000813"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777724"), "Criar configuracao administrativa do prazo de auto-fechamento" },
                    { new Guid("78787878-7878-7878-7878-000000000814"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de reabertura controlada por prazo/politica" },
                    { new Guid("78787878-7878-7878-7878-000000000815"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar auditoria de resolucao, aceite, rejeicao, fechamento e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000816"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777724"), "Preservar bloqueio por aprovacao pendente antes de fechamento definitivo" },
                    { new Guid("78787878-7878-7878-7878-000000000817"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777724"), "Ajustar endpoints de resolucao, fechamento, aceite e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000818"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777724"), "Exibir dados de solucao, aceite e fechamento no detalhe do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000819"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777724"), "Permitir aceite/rejeicao pelo solicitante na interface" },
                    { new Guid("78787878-7878-7878-7878-000000000820"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777724"), "Exibir historico de fechamento e reabertura na interface administrativa" },
                    { new Guid("78787878-7878-7878-7878-000000000821"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 21, new Guid("77777777-7777-7777-7777-777777777724"), "Testar resolucao com solucao obrigatoria" },
                    { new Guid("78787878-7878-7878-7878-000000000822"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 22, new Guid("77777777-7777-7777-7777-777777777724"), "Testar cancelamento com motivo obrigatorio" },
                    { new Guid("78787878-7878-7878-7878-000000000823"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 23, new Guid("77777777-7777-7777-7777-777777777724"), "Testar aceite e fechamento definitivo" },
                    { new Guid("78787878-7878-7878-7878-000000000824"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 24, new Guid("77777777-7777-7777-7777-777777777724"), "Testar rejeicao da solucao e retorno ao atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000825"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 25, new Guid("77777777-7777-7777-7777-777777777724"), "Testar fechamento automatico por prazo" },
                    { new Guid("78787878-7878-7878-7878-000000000826"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 26, new Guid("77777777-7777-7777-7777-777777777724"), "Testar reabertura controlada e auditavel" },
                    { new Guid("78787878-7878-7878-7878-000000000827"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777724"), "Testar regressao de encerramento/reabertura existente" },
                    { new Guid("78787878-7878-7878-7878-000000000828"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 28, new Guid("77777777-7777-7777-7777-777777777724"), "Testar integracao com aprovacao pendente bloqueante" },
                    { new Guid("78787878-7878-7878-7878-000000000829"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 4, true, 29, new Guid("77777777-7777-7777-7777-777777777724"), "Documentar impacto no fluxo atual de chamados" },
                    { new Guid("78787878-7878-7878-7878-000000000830"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 5, true, 30, new Guid("77777777-7777-7777-7777-777777777724"), "Preparar roteiro de homologacao da Sprint 5" },
                    { new Guid("78787878-7878-7878-7878-000000000831"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 5, true, 31, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar fechamento tecnico e homologacao posterior da Sprint 5" },
                    { new Guid("78787878-7878-7878-7878-000000000832"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 9, true, 32, new Guid("77777777-7777-7777-7777-777777777724"), "Atualizar roadmap final da Sprint 5" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Homologacao formal permanece pendente e sera executada posteriormente com usuarios reais, sem misturar implementacao funcional da Sprint 6 neste fechamento.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Consolidado em docs/roadmap/sprint-5-regras-fechamento-aceite-reabertura.md, docs/roadmap/sprint-5-impacto-fluxo-atual-chamados.md, docs/roadmap/sprint-5-roteiro-homologacao.md e docs/roadmap/sprint-5-fechamento-tecnico-final.md.", "Sprint 5 encerrada tecnicamente; homologacao formal permanece pendente e foi apenas roteirizada nesta consolidacao final.", "Executar posteriormente a homologacao institucional/manual com solicitantes, atendentes e administradores reais, registrando evidencias e aceite formal.", "Nao ha nova pendencia funcional aberta da Sprint 5 neste fechamento tecnico; permanecem apenas homologacao formal posterior e evolucoes futuras fora do escopo desta sprint.", 100, "Executar homologacao formal da Sprint 5 e iniciar a analise da Sprint 6 - Notificacoes ITSM, sem antecipar implementacao funcional.", "Fluxo de resolucao, aceite, rejeicao, fechamento automatico e reabertura foi consolidado tecnicamente com checklist final e documentacao de encerramento da Sprint 5.", 1, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000133"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000134"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000135"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000136"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000801"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000802"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000803"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000804"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000805"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000808"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000811"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000812"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000813"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000815"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000816"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000817"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000818"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000819"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000820"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000821"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000822"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000823"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000824"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000825"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000826"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000827"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000828"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000829"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000830"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Separar resolvido de fechado e exigir dados obrigatorios de solucao/cancelamento.", null, "Base de encerramento/reabertura existente reaproveitada.", null, "Validar regras com solicitantes e atendentes reais.", "Aceite, prazo de auto-fechamento, motivo de cancelamento e campo solucao obrigatorio.", 50, "Evoluir estados e regras de negocio de ciclo de vida.", "Encerrar e reabrir existem, mas faltam aceite do solicitante e politicas formais.", 3, 2, 1 });
        }
    }
}
