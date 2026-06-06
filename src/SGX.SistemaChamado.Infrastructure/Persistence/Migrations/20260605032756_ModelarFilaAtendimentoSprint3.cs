using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelarFilaAtendimentoSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "filas_atendimento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_tecnico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filas_atendimento", x => x.id);
                    table.ForeignKey(
                        name: "FK_filas_atendimento_grupos_tecnicos_grupo_tecnico_id",
                        column: x => x.grupo_tecnico_id,
                        principalTable: "grupos_tecnicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "filas_atendimento",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "grupo_tecnico_id", "nome" },
                values: new object[,]
                {
                    { new Guid("94949494-9494-9494-9494-949494949401"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fila operacional para triagem e atendimento inicial.", new Guid("93939393-9393-9393-9393-939393939301"), "Fila Service Desk" },
                    { new Guid("94949494-9494-9494-9494-949494949402"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fila operacional para suporte tecnico de usuarios, estacoes e perifericos.", new Guid("93939393-9393-9393-9393-939393939302"), "Fila Suporte Tecnico" },
                    { new Guid("94949494-9494-9494-9494-949494949403"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fila operacional para infraestrutura corporativa.", new Guid("93939393-9393-9393-9393-939393939303"), "Fila Infraestrutura" },
                    { new Guid("94949494-9494-9494-9494-949494949404"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fila operacional para sistemas corporativos e aplicacoes.", new Guid("93939393-9393-9393-9393-939393939304"), "Fila Sistemas" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000217"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 9);

            migrationBuilder.CreateIndex(
                name: "ix_filas_atendimento_ativo",
                table: "filas_atendimento",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_filas_atendimento_grupo_tecnico_id",
                table: "filas_atendimento",
                column: "grupo_tecnico_id");

            migrationBuilder.CreateIndex(
                name: "ux_filas_atendimento_grupo_nome",
                table: "filas_atendimento",
                columns: new[] { "grupo_tecnico_id", "nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filas_atendimento");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000217"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 7);
        }
    }
}
