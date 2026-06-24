using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarEstruturaTemplateNotificacaoSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "templates_notificacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    tipo_evento = table.Column<int>(type: "integer", nullable: false),
                    canal = table.Column<int>(type: "integer", nullable: false),
                    versao = table.Column<int>(type: "integer", nullable: false),
                    assunto_template = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    conteudo_template = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    vigente_de = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    vigente_ate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    variaveis_permitidas = table.Column<string>(type: "text", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templates_notificacao", x => x.id);
                    table.CheckConstraint("ck_templates_notificacao_versao_positiva", "versao > 0");
                    table.CheckConstraint("ck_templates_notificacao_vigencia", "vigente_ate IS NULL OR vigente_de IS NULL OR vigente_ate >= vigente_de");
                    table.ForeignKey(
                        name: "FK_templates_notificacao_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_templates_notificacao_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_templates_notificacao_atualizado_por_usuario_id",
                table: "templates_notificacao",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_templates_notificacao_criado_por_usuario_id",
                table: "templates_notificacao",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_templates_notificacao_tipo_evento_canal_ativo",
                table: "templates_notificacao",
                columns: new[] { "tipo_evento", "canal", "ativo" });

            migrationBuilder.CreateIndex(
                name: "ix_templates_notificacao_vigencia",
                table: "templates_notificacao",
                columns: new[] { "vigente_de", "vigente_ate" });

            migrationBuilder.CreateIndex(
                name: "ux_templates_notificacao_nome_versao",
                table: "templates_notificacao",
                columns: new[] { "nome", "versao" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "templates_notificacao");
        }
    }
}
