using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoadmapSlaSprint3Checklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("70707070-7070-7070-7070-707070707731"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777705"), "Configuração de alerta de SLA criada." },
                    { new Guid("70707070-7070-7070-7070-707070707732"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 32, new Guid("77777777-7777-7777-7777-777777777705"), "Tela administrativa de configuração de alerta criada." },
                    { new Guid("70707070-7070-7070-7070-707070707733"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 33, new Guid("77777777-7777-7777-7777-777777777705"), "Endpoints de configuração de alerta criados." },
                    { new Guid("70707070-7070-7070-7070-707070707734"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777705"), "Job de verificação de SLA criado." },
                    { new Guid("70707070-7070-7070-7070-707070707735"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 35, new Guid("77777777-7777-7777-7777-777777777705"), "Periodicidade configurável por appsettings criada." },
                    { new Guid("70707070-7070-7070-7070-707070707736"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 36, new Guid("77777777-7777-7777-7777-777777777705"), "Controle contra notificações/eventos duplicados criado." },
                    { new Guid("70707070-7070-7070-7070-707070707737"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777705"), "Histórico de eventos de SLA criado." },
                    { new Guid("70707070-7070-7070-7070-707070707738"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777705"), "Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolução, pausa e retomada." },
                    { new Guid("70707070-7070-7070-7070-707070707739"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 39, new Guid("77777777-7777-7777-7777-777777777705"), "Painel de indicadores de SLA criado." },
                    { new Guid("70707070-7070-7070-7070-707070707740"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 40, new Guid("77777777-7777-7777-7777-777777777705"), "Indicador de SLA vencido criado." },
                    { new Guid("70707070-7070-7070-7070-707070707741"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 41, new Guid("77777777-7777-7777-7777-777777777705"), "Indicador de SLA próximo do vencimento criado." },
                    { new Guid("70707070-7070-7070-7070-707070707742"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 42, new Guid("77777777-7777-7777-7777-777777777705"), "Indicador de percentual de cumprimento criado." },
                    { new Guid("70707070-7070-7070-7070-707070707743"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 43, new Guid("77777777-7777-7777-7777-777777777705"), "Métrica de tempo médio de primeira resposta criada." },
                    { new Guid("70707070-7070-7070-7070-707070707744"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 44, new Guid("77777777-7777-7777-7777-777777777705"), "Métrica de tempo médio de resolução criada." },
                    { new Guid("70707070-7070-7070-7070-707070707745"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 45, new Guid("77777777-7777-7777-7777-777777777705"), "Indicadores por prioridade criados." },
                    { new Guid("70707070-7070-7070-7070-707070707746"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 46, new Guid("77777777-7777-7777-7777-777777777705"), "Indicadores por categoria criados." },
                    { new Guid("70707070-7070-7070-7070-707070707747"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 47, new Guid("77777777-7777-7777-7777-777777777705"), "Indicadores por departamento criados." },
                    { new Guid("70707070-7070-7070-7070-707070707748"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 48, new Guid("77777777-7777-7777-7777-777777777705"), "Histórico de SLA exibido no detalhe administrativo do chamado." },
                    { new Guid("70707070-7070-7070-7070-707070707749"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 2, true, 49, new Guid("77777777-7777-7777-7777-777777777705"), "Estrutura preparada para exportação futura." },
                    { new Guid("70707070-7070-7070-7070-707070707750"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 4, true, 50, new Guid("77777777-7777-7777-7777-777777777705"), "Documentação atualizada." },
                    { new Guid("70707070-7070-7070-7070-707070707751"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 3", 3, true, 51, new Guid("77777777-7777-7777-7777-777777777705"), "Testes automatizados criados." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707731"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707732"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707733"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707734"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707735"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707736"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707737"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707738"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707739"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707740"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707741"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707742"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707743"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707744"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707745"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707746"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707747"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707748"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707749"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707750"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707751"));
        }
    }
}
