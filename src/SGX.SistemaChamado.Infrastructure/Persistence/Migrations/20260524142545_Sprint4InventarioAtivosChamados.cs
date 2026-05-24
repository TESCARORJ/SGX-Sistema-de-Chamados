using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4InventarioAtivosChamados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "inventario_ativo_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_chamados_inventario_ativo_id",
                table: "chamados",
                column: "inventario_ativo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_inventario_ativos_inventario_ativo_id",
                table: "chamados",
                column: "inventario_ativo_id",
                principalTable: "inventario_ativos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_inventario_ativos_inventario_ativo_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "ix_chamados_inventario_ativo_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "inventario_ativo_id",
                table: "chamados");
        }
    }
}
