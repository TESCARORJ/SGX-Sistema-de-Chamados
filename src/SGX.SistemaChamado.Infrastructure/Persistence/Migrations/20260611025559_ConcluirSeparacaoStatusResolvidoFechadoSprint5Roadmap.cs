using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirSeparacaoStatusResolvidoFechadoSprint5Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvidoEm",
                table: "chamados",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 19, "Criar regra para exigir solucao tecnica ao resolver chamado." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvidoEm",
                table: "chamados");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 16, "Separar status Resolvido e Fechado no fluxo de negocio." });
        }
    }
}
