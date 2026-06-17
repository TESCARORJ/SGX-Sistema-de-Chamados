using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5RoadmapChecklistCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000801"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777724"), "Planejar escopo e criterios de aceite da Sprint 5" },
                    { new Guid("78787878-7878-7878-7878-000000000802"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777724"), "Mapear fluxo atual de encerramento e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000803"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777724"), "Validar compatibilidade com Fundacao ITSM do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000804"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 1, true, 4, new Guid("77777777-7777-7777-7777-777777777724"), "Validar compatibilidade com Sprint 4 Motor de Aprovacoes ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000805"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 4, true, 5, new Guid("77777777-7777-7777-7777-777777777724"), "Documentar modelo de ciclo de vida Resolvido/Fechado/Reaberto" },
                    { new Guid("78787878-7878-7878-7878-000000000806"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777724"), "Separar status Resolvido e Fechado no fluxo de negocio" },
                    { new Guid("78787878-7878-7878-7878-000000000807"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra para exigir solucao tecnica ao resolver chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000808"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra para exigir motivo ao cancelar chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000809"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de aceite do solicitante" },
                    { new Guid("78787878-7878-7878-7878-000000000810"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de rejeicao da solucao pelo solicitante" },
                    { new Guid("78787878-7878-7878-7878-000000000811"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de retorno ao atendimento apos rejeicao da solucao" },
                    { new Guid("78787878-7878-7878-7878-000000000812"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777724"), "Criar politica de fechamento automatico apos prazo de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000813"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777724"), "Criar configuracao administrativa do prazo de auto-fechamento" },
                    { new Guid("78787878-7878-7878-7878-000000000814"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777724"), "Criar regra de reabertura controlada por prazo/politica" },
                    { new Guid("78787878-7878-7878-7878-000000000815"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar auditoria de resolucao, aceite, rejeicao, fechamento e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000816"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777724"), "Preservar bloqueio por aprovacao pendente antes de fechamento definitivo" },
                    { new Guid("78787878-7878-7878-7878-000000000817"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777724"), "Ajustar endpoints de resolucao, fechamento, aceite e reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000818"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777724"), "Exibir dados de solucao, aceite e fechamento no detalhe do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000819"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777724"), "Permitir aceite/rejeicao pelo solicitante na interface" },
                    { new Guid("78787878-7878-7878-7878-000000000820"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777724"), "Exibir historico de fechamento e reabertura na interface administrativa" },
                    { new Guid("78787878-7878-7878-7878-000000000821"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 21, new Guid("77777777-7777-7777-7777-777777777724"), "Testar resolucao com solucao obrigatoria" },
                    { new Guid("78787878-7878-7878-7878-000000000822"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 22, new Guid("77777777-7777-7777-7777-777777777724"), "Testar cancelamento com motivo obrigatorio" },
                    { new Guid("78787878-7878-7878-7878-000000000823"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 23, new Guid("77777777-7777-7777-7777-777777777724"), "Testar aceite e fechamento definitivo" },
                    { new Guid("78787878-7878-7878-7878-000000000824"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 24, new Guid("77777777-7777-7777-7777-777777777724"), "Testar rejeicao da solucao e retorno ao atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000825"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 25, new Guid("77777777-7777-7777-7777-777777777724"), "Testar fechamento automatico por prazo" },
                    { new Guid("78787878-7878-7878-7878-000000000826"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 26, new Guid("77777777-7777-7777-7777-777777777724"), "Testar reabertura controlada e auditavel" },
                    { new Guid("78787878-7878-7878-7878-000000000827"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777724"), "Testar regressao de encerramento/reabertura existente" },
                    { new Guid("78787878-7878-7878-7878-000000000828"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 3, true, 28, new Guid("77777777-7777-7777-7777-777777777724"), "Testar integracao com aprovacao pendente bloqueante" },
                    { new Guid("78787878-7878-7878-7878-000000000829"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 4, true, 29, new Guid("77777777-7777-7777-7777-777777777724"), "Documentar impacto no fluxo atual de chamados" },
                    { new Guid("78787878-7878-7878-7878-000000000830"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 5, true, 30, new Guid("77777777-7777-7777-7777-777777777724"), "Preparar roteiro de homologacao da Sprint 5" },
                    { new Guid("78787878-7878-7878-7878-000000000831"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 5, true, 31, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar homologacao e aceite tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000832"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento", 9, true, 32, new Guid("77777777-7777-7777-7777-777777777724"), "Atualizar roadmap final da Sprint 5" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                column: "percentual_implementacao",
                value: 13);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000801"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000802"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000803"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000804"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000805"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000808"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000811"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000812"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000813"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000815"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000816"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000817"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000818"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000819"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000820"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000821"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000822"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000823"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000824"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000825"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000826"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000827"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000828"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000829"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000830"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"));

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000133"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento, aceite e reabertura", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777724"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000134"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento, aceite e reabertura", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777724"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000135"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento, aceite e reabertura", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777724"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000136"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 5 Regras de fechamento, aceite e reabertura", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777724"), "Registrar homologacao e aceite" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                column: "percentual_implementacao",
                value: 50);
        }
    }
}
