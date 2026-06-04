using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChamadoTarefas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chamados_tarefas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    responsavel_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prazo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    concluido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    concluido_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancelado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_cancelamento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamados_tarefas", x => x.id);
                    table.ForeignKey(
                        name: "FK_chamados_tarefas_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_tarefas_usuarios_cancelado_por_usuario_id",
                        column: x => x.cancelado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_tarefas_usuarios_concluido_por_usuario_id",
                        column: x => x.concluido_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_tarefas_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_tarefas_usuarios_responsavel_usuario_id",
                        column: x => x.responsavel_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_ativo",
                table: "chamados_tarefas",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_tarefas_cancelado_por_usuario_id",
                table: "chamados_tarefas",
                column: "cancelado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_chamado_id",
                table: "chamados_tarefas",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_tarefas_concluido_por_usuario_id",
                table: "chamados_tarefas",
                column: "concluido_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_criado_por_usuario_id",
                table: "chamados_tarefas",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_prazo",
                table: "chamados_tarefas",
                column: "prazo");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_responsavel_usuario_id",
                table: "chamados_tarefas",
                column: "responsavel_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_tarefas_status",
                table: "chamados_tarefas",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chamados_tarefas");
        }
    }
}
