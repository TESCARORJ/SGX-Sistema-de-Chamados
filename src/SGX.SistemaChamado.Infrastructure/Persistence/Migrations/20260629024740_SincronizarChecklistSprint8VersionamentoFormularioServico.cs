using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarChecklistSprint8VersionamentoFormularioServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001021"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 46, "Configurar EF Core para formulario e campos." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001021"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 45, "Modelar versionamento de formulario por servico." });
        }
    }
}
