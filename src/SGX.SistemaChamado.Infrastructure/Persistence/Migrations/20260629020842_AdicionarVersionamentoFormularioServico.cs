using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarVersionamentoFormularioServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campos_formulario_servico_formularios_servico_formulario_se~",
                table: "campos_formulario_servico");

            migrationBuilder.RenameColumn(
                name: "formulario_servico_id",
                table: "campos_formulario_servico",
                newName: "formulario_servico_versao_id");

            migrationBuilder.RenameIndex(
                name: "ux_campos_formulario_servico_formulario_servico_id_ordem",
                table: "campos_formulario_servico",
                newName: "ux_campo_form_serv_ordem");

            migrationBuilder.RenameIndex(
                name: "ux_campos_formulario_servico_formulario_servico_id_nome",
                table: "campos_formulario_servico",
                newName: "ux_campo_form_serv_nome");

            migrationBuilder.RenameIndex(
                name: "ix_campos_formulario_servico_formulario_servico_id",
                table: "campos_formulario_servico",
                newName: "ix_campo_form_serv_versao");

            migrationBuilder.CreateTable(
                name: "formularios_servico_versoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    formulario_servico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    publicada = table.Column<bool>(type: "boolean", nullable: false),
                    publicado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formularios_servico_versoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_formularios_servico_versoes_formularios_servico_formulario_~",
                        column: x => x.formulario_servico_id,
                        principalTable: "formularios_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_form_serv_versao_ativo",
                table: "formularios_servico_versoes",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_form_serv_versao_form",
                table: "formularios_servico_versoes",
                column: "formulario_servico_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_serv_versao_pub",
                table: "formularios_servico_versoes",
                column: "publicada");

            migrationBuilder.CreateIndex(
                name: "ux_form_serv_versao_num",
                table: "formularios_servico_versoes",
                columns: new[] { "formulario_servico_id", "numero" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO formularios_servico_versoes (
                    id,
                    formulario_servico_id,
                    numero,
                    publicada,
                    publicado_em,
                    criado_em,
                    criado_por,
                    atualizado_em,
                    atualizado_por,
                    ativo
                )
                SELECT
                    id,
                    id,
                    1,
                    false,
                    NULL,
                    criado_em,
                    criado_por,
                    atualizado_em,
                    atualizado_por,
                    ativo
                FROM formularios_servico;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_campos_formulario_servico_formularios_servico_versoes_formu~",
                table: "campos_formulario_servico",
                column: "formulario_servico_versao_id",
                principalTable: "formularios_servico_versoes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campos_formulario_servico_formularios_servico_versoes_formu~",
                table: "campos_formulario_servico");

            migrationBuilder.DropTable(
                name: "formularios_servico_versoes");

            migrationBuilder.RenameColumn(
                name: "formulario_servico_versao_id",
                table: "campos_formulario_servico",
                newName: "formulario_servico_id");

            migrationBuilder.RenameIndex(
                name: "ux_campo_form_serv_ordem",
                table: "campos_formulario_servico",
                newName: "ux_campos_formulario_servico_formulario_servico_id_ordem");

            migrationBuilder.RenameIndex(
                name: "ux_campo_form_serv_nome",
                table: "campos_formulario_servico",
                newName: "ux_campos_formulario_servico_formulario_servico_id_nome");

            migrationBuilder.RenameIndex(
                name: "ix_campo_form_serv_versao",
                table: "campos_formulario_servico",
                newName: "ix_campos_formulario_servico_formulario_servico_id");

            migrationBuilder.AddForeignKey(
                name: "FK_campos_formulario_servico_formularios_servico_formulario_se~",
                table: "campos_formulario_servico",
                column: "formulario_servico_id",
                principalTable: "formularios_servico",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
