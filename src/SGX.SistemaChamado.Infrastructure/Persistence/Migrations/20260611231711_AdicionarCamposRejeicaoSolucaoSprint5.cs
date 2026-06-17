using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposRejeicaoSolucaoSprint5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoRejeicaoSolucao",
                table: "chamados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SolucaoRejeitadaEm",
                table: "chamados",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SolucaoRejeitadaPorUsuarioId",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 31, "Criar regra de retorno ao atendimento apos rejeicao da solucao." });

            migrationBuilder.CreateIndex(
                name: "IX_chamados_SolucaoRejeitadaPorUsuarioId",
                table: "chamados",
                column: "SolucaoRejeitadaPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_usuarios_SolucaoRejeitadaPorUsuarioId",
                table: "chamados",
                column: "SolucaoRejeitadaPorUsuarioId",
                principalTable: "usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_usuarios_SolucaoRejeitadaPorUsuarioId",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_SolucaoRejeitadaPorUsuarioId",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "MotivoRejeicaoSolucao",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "SolucaoRejeitadaEm",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "SolucaoRejeitadaPorUsuarioId",
                table: "chamados");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 28, "Criar regra de rejeição de solução." });
        }
    }
}
