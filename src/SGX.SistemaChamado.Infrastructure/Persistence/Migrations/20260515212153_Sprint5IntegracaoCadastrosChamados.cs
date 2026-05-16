using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5IntegracaoCadastrosChamados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "local_unidade_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "subcategoria_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "tipo_solicitacao_id",
                table: "chamados",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chamados_local_unidade_id",
                table: "chamados",
                column: "local_unidade_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_subcategoria_id",
                table: "chamados",
                column: "subcategoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_tipo_solicitacao_id",
                table: "chamados",
                column: "tipo_solicitacao_id");

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_locais_unidade_local_unidade_id",
                table: "chamados",
                column: "local_unidade_id",
                principalTable: "locais_unidade",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_subcategorias_chamado_subcategoria_id",
                table: "chamados",
                column: "subcategoria_id",
                principalTable: "subcategorias_chamado",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_tipos_solicitacao_tipo_solicitacao_id",
                table: "chamados",
                column: "tipo_solicitacao_id",
                principalTable: "tipos_solicitacao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_locais_unidade_local_unidade_id",
                table: "chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_chamados_subcategorias_chamado_subcategoria_id",
                table: "chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_chamados_tipos_solicitacao_tipo_solicitacao_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_local_unidade_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_subcategoria_id",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_tipo_solicitacao_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "local_unidade_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "subcategoria_id",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "tipo_solicitacao_id",
                table: "chamados");
        }
    }
}
