using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VincularChamadoFilaAtendimentoSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "fila_atendimento_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000219"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 13);

            migrationBuilder.CreateIndex(
                name: "ix_chamados_fila_atendimento_id",
                table: "chamados",
                column: "fila_atendimento_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_filas_atendimento_fila_atendimento_id",
                table: "chamados",
                column: "fila_atendimento_id",
                principalTable: "filas_atendimento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_filas_atendimento_fila_atendimento_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "ix_chamados_fila_atendimento_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "fila_atendimento_id",
                table: "chamados");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000219"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 11);
        }
    }
}
