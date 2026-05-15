using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventosAuditoriaGovernancaSprint1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "eventos_auditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_evento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usuario_nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    usuario_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    usuario_login = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    ip_origem = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    modulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    entidade = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    entidade_id = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    acao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    dados_antes = table.Column<string>(type: "text", nullable: true),
                    dados_depois = table.Column<string>(type: "text", nullable: true),
                    metadados = table.Column<string>(type: "text", nullable: true),
                    nivel = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    mensagem_erro = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    correlacao_id = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos_auditoria", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_acao",
                table: "eventos_auditoria",
                column: "acao");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_correlacao_id",
                table: "eventos_auditoria",
                column: "correlacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_data_evento",
                table: "eventos_auditoria",
                column: "data_evento");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_entidade",
                table: "eventos_auditoria",
                column: "entidade");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_entidade_id",
                table: "eventos_auditoria",
                column: "entidade_id");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_modulo",
                table: "eventos_auditoria",
                column: "modulo");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_usuario_email",
                table: "eventos_auditoria",
                column: "usuario_email");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_auditoria_usuario_id",
                table: "eventos_auditoria",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventos_auditoria");
        }
    }
}
