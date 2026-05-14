using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirChecklistRoadmapSla : Migration
    {
        private static readonly Guid RoadmapSlaId = new("77777777-7777-7777-7777-777777777705");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: RoadmapSlaId,
                columns: new[]
                {
                    "status_implementacao",
                    "status_tecnico",
                    "percentual_implementacao",
                    "situacao_atual"
                },
                values: new object[]
                {
                    3,
                    3,
                    100,
                    "Sprints 1, 2, 3 e 4 do módulo de SLA implementadas e validadas funcionalmente. O item de roadmap passa a refletir checklist técnico concluído, com evolução futura registrada em pendências."
                });

            migrationBuilder.Sql(
                """
                UPDATE roadmap_checklist_itens
                   SET concluido = TRUE
                 WHERE roadmap_item_id = '77777777-7777-7777-7777-777777777705'
                   AND ordem BETWEEN 1 AND 69;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: RoadmapSlaId,
                columns: new[]
                {
                    "status_implementacao",
                    "status_tecnico",
                    "percentual_implementacao",
                    "situacao_atual"
                },
                values: new object[]
                {
                    2,
                    10,
                    19,
                    "Sprints 1, 2, 3 e 4 concluídas no código, porém checklist técnico consolidado ainda pendente de alinhamento final no roadmap."
                });

            migrationBuilder.Sql(
                """
                UPDATE roadmap_checklist_itens
                   SET concluido = CASE WHEN ordem BETWEEN 1 AND 13 THEN TRUE ELSE FALSE END
                 WHERE roadmap_item_id = '77777777-7777-7777-7777-777777777705'
                   AND ordem BETWEEN 1 AND 69;
                """);
        }
    }
}
