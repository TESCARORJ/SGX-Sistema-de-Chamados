using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarEstruturaNotificacaoSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notificacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_evento = table.Column<int>(type: "integer", nullable: false),
                    canal = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    destinatario_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    destinatario_endereco = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    assunto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    conteudo = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    chave_correlacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    chave_idempotencia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    agendada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    processada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    enviada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    falhou_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    quantidade_tentativas = table.Column<int>(type: "integer", nullable: false),
                    ultimo_erro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    motivo_cancelamento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacoes", x => x.id);
                    table.CheckConstraint("ck_notificacoes_destinatario", "destinatario_usuario_id IS NOT NULL OR destinatario_endereco IS NOT NULL");
                    table.CheckConstraint("ck_notificacoes_quantidade_tentativas_nao_negativa", "quantidade_tentativas >= 0");
                    table.ForeignKey(
                        name: "FK_notificacoes_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notificacoes_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notificacoes_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notificacoes_usuarios_destinatario_usuario_id",
                        column: x => x.destinatario_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_agendada_em",
                table: "notificacoes",
                column: "agendada_em");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_atualizado_por_usuario_id",
                table: "notificacoes",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_canal",
                table: "notificacoes",
                column: "canal");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_chamado_id",
                table: "notificacoes",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_criado_em",
                table: "notificacoes",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "IX_notificacoes_criado_por_usuario_id",
                table: "notificacoes",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_destinatario_usuario_id",
                table: "notificacoes",
                column: "destinatario_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_status",
                table: "notificacoes",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_status_agendada_em",
                table: "notificacoes",
                columns: new[] { "status", "agendada_em" });

            migrationBuilder.CreateIndex(
                name: "ux_notificacoes_chave_idempotencia",
                table: "notificacoes",
                column: "chave_idempotencia",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notificacoes");
        }
    }
}
