using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecuperacaoSenhaLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "bloqueado_ate",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "deve_alterar_senha",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "tentativas_invalidas",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ultima_alteracao_senha_em",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ultimo_login_em",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tokens_recuperacao_senha",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    utilizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip_solicitacao = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_agent_solicitacao = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens_recuperacao_senha", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_recuperacao_senha_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tokens_recuperacao_senha_expira_em",
                table: "tokens_recuperacao_senha",
                column: "expira_em");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_recuperacao_senha_token_hash",
                table: "tokens_recuperacao_senha",
                column: "token_hash");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_recuperacao_senha_usuario_id",
                table: "tokens_recuperacao_senha",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tokens_recuperacao_senha");

            migrationBuilder.DropColumn(
                name: "bloqueado_ate",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "deve_alterar_senha",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "tentativas_invalidas",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "ultima_alteracao_senha_em",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "ultimo_login_em",
                table: "usuarios");
        }
    }
}
