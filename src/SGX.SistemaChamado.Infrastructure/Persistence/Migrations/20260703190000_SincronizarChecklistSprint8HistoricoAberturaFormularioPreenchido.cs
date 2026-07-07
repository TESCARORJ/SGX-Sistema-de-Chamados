using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations;

public partial class SincronizarChecklistSprint8HistoricoAberturaFormularioPreenchido : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "roadmap_checklist_itens",
            keyColumn: "id",
            keyValue: new Guid("78787878-7878-7878-7878-000000001050"),
            columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
            values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

        migrationBuilder.UpdateData(
            table: "roadmap_itsm_itens",
            keyColumn: "id",
            keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
            columns: new[] { "percentual_implementacao", "proxima_acao" },
            values: new object[] { 84, "Registrar auditoria tecnica das respostas persistidas." });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "roadmap_checklist_itens",
            keyColumn: "id",
            keyValue: new Guid("78787878-7878-7878-7878-000000001050"),
            columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
            values: new object[] { null, null, false });

        migrationBuilder.UpdateData(
            table: "roadmap_itsm_itens",
            keyColumn: "id",
            keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
            columns: new[] { "percentual_implementacao", "proxima_acao" },
            values: new object[] { 83, "Registrar historico da abertura com formulario preenchido." });
    }
}
