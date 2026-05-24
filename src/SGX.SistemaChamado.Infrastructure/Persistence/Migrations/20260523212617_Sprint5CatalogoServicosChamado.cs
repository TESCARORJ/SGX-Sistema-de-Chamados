using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5CatalogoServicosChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "catalogo_servico_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_chamados_catalogo_servico_id",
                table: "chamados",
                column: "catalogo_servico_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_catalogo_servicos_catalogo_servico_id",
                table: "chamados",
                column: "catalogo_servico_id",
                principalTable: "catalogo_servicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_catalogo_servicos_catalogo_servico_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "ix_chamados_catalogo_servico_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "catalogo_servico_id",
                table: "chamados");
        }
    }
}
