using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VincularChamadoGrupoTecnicoSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "grupo_tecnico_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000218"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 11);

            migrationBuilder.CreateIndex(
                name: "ix_chamados_grupo_tecnico_id",
                table: "chamados",
                column: "grupo_tecnico_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_grupos_tecnicos_grupo_tecnico_id",
                table: "chamados",
                column: "grupo_tecnico_id",
                principalTable: "grupos_tecnicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_grupos_tecnicos_grupo_tecnico_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "ix_chamados_grupo_tecnico_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "grupo_tecnico_id",
                table: "chamados");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000218"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 9);
        }
    }
}
