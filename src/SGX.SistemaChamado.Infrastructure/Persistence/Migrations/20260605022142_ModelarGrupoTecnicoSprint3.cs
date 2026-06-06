using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModelarGrupoTecnicoSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grupos_tecnicos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_grupos_tecnicos", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "grupos_tecnicos",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "nome" },
                values: new object[,]
                {
                    { new Guid("93939393-9393-9393-9393-939393939301"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo responsavel pela triagem e atendimento inicial de chamados.", "Service Desk" },
                    { new Guid("93939393-9393-9393-9393-939393939302"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo responsavel por atendimento tecnico de estacoes, perifericos e suporte operacional.", "Suporte Tecnico" },
                    { new Guid("93939393-9393-9393-9393-939393939303"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo responsavel por servidores, redes e componentes de infraestrutura corporativa.", "Infraestrutura" },
                    { new Guid("93939393-9393-9393-9393-939393939304"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo responsavel por sistemas corporativos, aplicacoes e sustentacao funcional.", "Sistemas" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000215"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 6);

            migrationBuilder.CreateIndex(
                name: "ix_grupos_tecnicos_ativo",
                table: "grupos_tecnicos",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ux_grupos_tecnicos_nome",
                table: "grupos_tecnicos",
                column: "nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grupos_tecnicos");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000215"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 4);
        }
    }
}
