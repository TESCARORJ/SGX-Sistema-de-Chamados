using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarMetadadosCampoFormularioServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "obrigatorio",
                table: "campos_formulario_servico",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ordem",
                table: "campos_formulario_servico",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "texto_ajuda",
                table: "campos_formulario_servico",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "visivel",
                table: "campos_formulario_servico",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "ux_campos_formulario_servico_formulario_servico_id_ordem",
                table: "campos_formulario_servico",
                columns: new[] { "formulario_servico_id", "ordem" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_campos_formulario_servico_formulario_servico_id_ordem",
                table: "campos_formulario_servico");

            migrationBuilder.DropColumn(
                name: "obrigatorio",
                table: "campos_formulario_servico");

            migrationBuilder.DropColumn(
                name: "ordem",
                table: "campos_formulario_servico");

            migrationBuilder.DropColumn(
                name: "texto_ajuda",
                table: "campos_formulario_servico");

            migrationBuilder.DropColumn(
                name: "visivel",
                table: "campos_formulario_servico");
        }
    }
}
