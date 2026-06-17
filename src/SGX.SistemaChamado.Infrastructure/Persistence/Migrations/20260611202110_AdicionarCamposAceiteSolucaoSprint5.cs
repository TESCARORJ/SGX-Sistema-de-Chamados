using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposAceiteSolucaoSprint5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AceitoEm",
                table: "chamados",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AceitoPorUsuarioId",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacaoAceite",
                table: "chamados",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 28, "Criar regra de rejeição de solução." });

            migrationBuilder.CreateIndex(
                name: "IX_chamados_AceitoPorUsuarioId",
                table: "chamados",
                column: "AceitoPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_usuarios_AceitoPorUsuarioId",
                table: "chamados",
                column: "AceitoPorUsuarioId",
                principalTable: "usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_usuarios_AceitoPorUsuarioId",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_AceitoPorUsuarioId",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "AceitoEm",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "AceitoPorUsuarioId",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "ObservacaoAceite",
                table: "chamados");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 25, "Criar regra de aceite do solicitante." });
        }
    }
}
