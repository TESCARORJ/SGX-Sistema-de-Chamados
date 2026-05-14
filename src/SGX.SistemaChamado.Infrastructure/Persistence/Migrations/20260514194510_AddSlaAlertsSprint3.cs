using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaAlertsSprint3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracoes_alerta_sla",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    minutos_antes_vencimento_primeira_resposta = table.Column<int>(type: "integer", nullable: false),
                    minutos_antes_vencimento_resolucao = table.Column<int>(type: "integer", nullable: false),
                    notificar_atendente = table.Column<bool>(type: "boolean", nullable: false),
                    notificar_gestor = table.Column<bool>(type: "boolean", nullable: false),
                    notificar_departamento = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracoes_alerta_sla", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "eventos_sla",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_sla_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_evento = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    data_evento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    chave_idempotencia = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos_sla", x => x.id);
                    table.ForeignKey(
                        name: "FK_eventos_sla_chamado_slas_chamado_sla_id",
                        column: x => x.chamado_sla_id,
                        principalTable: "chamado_slas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_eventos_sla_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_eventos_sla_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "configuracoes_alerta_sla",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "minutos_antes_vencimento_primeira_resposta", "minutos_antes_vencimento_resolucao", "notificar_atendente", "notificar_departamento", "notificar_gestor" },
                values: new object[] { new Guid("56565656-5656-5656-5656-565656565621"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 30, 120, true, false, false });

            migrationBuilder.CreateIndex(
                name: "ix_eventos_sla_chamado_id",
                table: "eventos_sla",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_sla_chamado_sla_id",
                table: "eventos_sla",
                column: "chamado_sla_id");

            migrationBuilder.CreateIndex(
                name: "ix_eventos_sla_data_evento",
                table: "eventos_sla",
                column: "data_evento");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_sla_usuario_id",
                table: "eventos_sla",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_eventos_sla_chave_idempotencia",
                table: "eventos_sla",
                column: "chave_idempotencia",
                unique: true,
                filter: "chave_idempotencia IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracoes_alerta_sla");

            migrationBuilder.DropTable(
                name: "eventos_sla");
        }
    }
}
