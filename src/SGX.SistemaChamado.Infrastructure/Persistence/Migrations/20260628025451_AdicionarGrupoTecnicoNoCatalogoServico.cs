using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarGrupoTecnicoNoCatalogoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "grupo_tecnico_id",
                table: "catalogo_servicos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_catalogo_servicos_grupo_tecnico_id",
                table: "catalogo_servicos",
                column: "grupo_tecnico_id");

            migrationBuilder.AddForeignKey(
                name: "FK_catalogo_servicos_grupos_tecnicos_grupo_tecnico_id",
                table: "catalogo_servicos",
                column: "grupo_tecnico_id",
                principalTable: "grupos_tecnicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_catalogo_servicos_grupos_tecnicos_grupo_tecnico_id",
                table: "catalogo_servicos");

            migrationBuilder.DropIndex(
                name: "ix_catalogo_servicos_grupo_tecnico_id",
                table: "catalogo_servicos");

            migrationBuilder.DropColumn(
                name: "grupo_tecnico_id",
                table: "catalogo_servicos");
        }
    }
}
