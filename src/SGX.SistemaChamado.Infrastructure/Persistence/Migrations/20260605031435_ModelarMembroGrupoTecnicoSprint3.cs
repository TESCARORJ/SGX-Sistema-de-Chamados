using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelarMembroGrupoTecnicoSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "membros_grupos_tecnicos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_tecnico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membros_grupos_tecnicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_membros_grupos_tecnicos_grupos_tecnicos_grupo_tecnico_id",
                        column: x => x.grupo_tecnico_id,
                        principalTable: "grupos_tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membros_grupos_tecnicos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000216"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 7);

            migrationBuilder.CreateIndex(
                name: "ix_membros_grupos_tecnicos_ativo",
                table: "membros_grupos_tecnicos",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_membros_grupos_tecnicos_grupo_tecnico_id",
                table: "membros_grupos_tecnicos",
                column: "grupo_tecnico_id");

            migrationBuilder.CreateIndex(
                name: "ix_membros_grupos_tecnicos_usuario_id",
                table: "membros_grupos_tecnicos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_membros_grupos_tecnicos_grupo_usuario",
                table: "membros_grupos_tecnicos",
                columns: new[] { "grupo_tecnico_id", "usuario_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "membros_grupos_tecnicos");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000216"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 6);
        }
    }
}
