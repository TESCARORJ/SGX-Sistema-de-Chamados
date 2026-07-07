using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposFormularioServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campos_formulario_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    formulario_servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    rotulo = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campos_formulario_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_campos_formulario_servico_formularios_servico_formulario_se~",
                        column: x => x.formulario_servico_id,
                        principalTable: "formularios_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_campos_formulario_servico_ativo",
                table: "campos_formulario_servico",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_campos_formulario_servico_formulario_servico_id",
                table: "campos_formulario_servico",
                column: "formulario_servico_id");

            migrationBuilder.CreateIndex(
                name: "ux_campos_formulario_servico_formulario_servico_id_nome",
                table: "campos_formulario_servico",
                columns: new[] { "formulario_servico_id", "nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campos_formulario_servico");
        }
    }
}
