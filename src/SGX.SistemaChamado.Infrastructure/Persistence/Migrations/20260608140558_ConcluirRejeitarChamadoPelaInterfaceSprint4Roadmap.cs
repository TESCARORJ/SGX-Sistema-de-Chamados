using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirRejeitarChamadoPelaInterfaceSprint4Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000345"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 74, "Ajustar listagem/filtros para aprovacao pendente." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000345"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 69, "Criar tela ou secao de configuracao de regras de aprovacao." });
        }
    }
}
