using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarEstruturaPreferenciaNotificacaoUsuarioSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "preferencias_notificacao_usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_evento = table.Column<int>(type: "integer", nullable: false),
                    canal = table.Column<int>(type: "integer", nullable: false),
                    habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preferencias_notificacao_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_preferencias_notificacao_usuario_usuarios_atualizado_por_us~",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_preferencias_notificacao_usuario_usuarios_criado_por_usuari~",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_preferencias_notificacao_usuario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_preferencias_notificacao_usuario_atualizado_por_usuario_id",
                table: "preferencias_notificacao_usuario",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_preferencias_notificacao_usuario_criado_por_usuario_id",
                table: "preferencias_notificacao_usuario",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_preferencias_notificacao_usuario_tipo_evento_canal",
                table: "preferencias_notificacao_usuario",
                columns: new[] { "tipo_evento", "canal" });

            migrationBuilder.CreateIndex(
                name: "ix_preferencias_notificacao_usuario_usuario_id",
                table: "preferencias_notificacao_usuario",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_preferencias_notificacao_usuario_chave",
                table: "preferencias_notificacao_usuario",
                columns: new[] { "usuario_id", "tipo_evento", "canal" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "preferencias_notificacao_usuario");
        }
    }
}
