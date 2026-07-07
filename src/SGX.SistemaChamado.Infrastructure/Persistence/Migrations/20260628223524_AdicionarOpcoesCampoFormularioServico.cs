using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarOpcoesCampoFormularioServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "opcoes_campos_formulario_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    campo_formulario_servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valor = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    rotulo = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opcoes_campos_formulario_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_opcoes_campos_formulario_servico_campos_formulario_servico_~",
                        column: x => x.campo_formulario_servico_id,
                        principalTable: "campos_formulario_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_opcao_form_serv_ativo",
                table: "opcoes_campos_formulario_servico",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_opcao_form_serv_campo",
                table: "opcoes_campos_formulario_servico",
                column: "campo_formulario_servico_id");

            migrationBuilder.CreateIndex(
                name: "ux_opcao_form_serv_ordem",
                table: "opcoes_campos_formulario_servico",
                columns: new[] { "campo_formulario_servico_id", "ordem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_opcao_form_serv_valor",
                table: "opcoes_campos_formulario_servico",
                columns: new[] { "campo_formulario_servico_id", "valor" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opcoes_campos_formulario_servico");
        }
    }
}
