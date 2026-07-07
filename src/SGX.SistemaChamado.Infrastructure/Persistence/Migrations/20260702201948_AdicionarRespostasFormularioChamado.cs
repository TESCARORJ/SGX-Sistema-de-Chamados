using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRespostasFormularioChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "respostas_formulario_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    formulario_servico_versao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campo_formulario_servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valor = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    valores_json = table.Column<string>(type: "character varying(16000)", maxLength: 16000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas_formulario_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_respostas_formulario_chamado_campos_formulario_servico_camp~",
                        column: x => x.campo_formulario_servico_id,
                        principalTable: "campos_formulario_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_respostas_formulario_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_respostas_formulario_chamado_formularios_servico_versoes_fo~",
                        column: x => x.formulario_servico_versao_id,
                        principalTable: "formularios_servico_versoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_resp_form_campo",
                table: "respostas_formulario_chamado",
                column: "campo_formulario_servico_id");

            migrationBuilder.CreateIndex(
                name: "ix_resp_form_chamado",
                table: "respostas_formulario_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_resp_form_chamado_ver",
                table: "respostas_formulario_chamado",
                columns: new[] { "chamado_id", "formulario_servico_versao_id" });

            migrationBuilder.CreateIndex(
                name: "ix_resp_form_versao",
                table: "respostas_formulario_chamado",
                column: "formulario_servico_versao_id");

            migrationBuilder.CreateIndex(
                name: "ux_resp_form_chamado_cmp",
                table: "respostas_formulario_chamado",
                columns: new[] { "chamado_id", "campo_formulario_servico_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respostas_formulario_chamado");
        }
    }
}
