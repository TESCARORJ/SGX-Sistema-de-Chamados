using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCadastrosAdministrativosSprint1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cor",
                table: "prioridades_chamado",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#1976D2");

            migrationBuilder.AddColumn<int>(
                name: "peso",
                table: "prioridades_chamado",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "locais_unidade",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locais_unidade", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subcategorias_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_subcategorias_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_subcategorias_chamado_categorias_chamado_categoria_chamado_~",
                        column: x => x.categoria_chamado_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tipos_solicitacao",
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
                    table.PrimaryKey("PK_tipos_solicitacao", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "prioridades_chamado",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"),
                columns: new[] { "cor", "peso" },
                values: new object[] { "#2E7D32", 1 });

            migrationBuilder.UpdateData(
                table: "prioridades_chamado",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"),
                columns: new[] { "cor", "peso" },
                values: new object[] { "#F9A825", 2 });

            migrationBuilder.UpdateData(
                table: "prioridades_chamado",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"),
                columns: new[] { "cor", "peso" },
                values: new object[] { "#EF6C00", 3 });

            migrationBuilder.UpdateData(
                table: "prioridades_chamado",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555554"),
                columns: new[] { "cor", "peso" },
                values: new object[] { "#C62828", 4 });

            migrationBuilder.CreateIndex(
                name: "ux_locais_unidade_nome",
                table: "locais_unidade",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_subcategorias_chamado_categoria_nome",
                table: "subcategorias_chamado",
                columns: new[] { "categoria_chamado_id", "nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_tipos_solicitacao_nome",
                table: "tipos_solicitacao",
                column: "nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locais_unidade");

            migrationBuilder.DropTable(
                name: "subcategorias_chamado");

            migrationBuilder.DropTable(
                name: "tipos_solicitacao");

            migrationBuilder.DropColumn(
                name: "cor",
                table: "prioridades_chamado");

            migrationBuilder.DropColumn(
                name: "peso",
                table: "prioridades_chamado");
        }
    }
}
