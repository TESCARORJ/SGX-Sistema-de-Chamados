using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChamadoSlaSprint2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chamado_slas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    politica_sla_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prioridade_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prazo_primeira_resposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prazo_resolucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_primeira_resposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_resolucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    primeira_resposta_cumprida = table.Column<bool>(type: "boolean", nullable: true),
                    resolucao_cumprida = table.Column<bool>(type: "boolean", nullable: true),
                    primeira_resposta_violada = table.Column<bool>(type: "boolean", nullable: false),
                    resolucao_violada = table.Column<bool>(type: "boolean", nullable: false),
                    minutos_primeira_resposta = table.Column<int>(type: "integer", nullable: true),
                    minutos_resolucao = table.Column<int>(type: "integer", nullable: true),
                    pausado = table.Column<bool>(type: "boolean", nullable: false),
                    data_pausa = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    minutos_pausados = table.Column<int>(type: "integer", nullable: false),
                    pausar_quando_aguardando_solicitante = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamado_slas", x => x.id);
                    table.ForeignKey(
                        name: "FK_chamado_slas_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_chamado_slas_prioridades_chamado_prioridade_id",
                        column: x => x.prioridade_id,
                        principalTable: "prioridades_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamado_slas_sla_politicas_politica_sla_id",
                        column: x => x.politica_sla_id,
                        principalTable: "sla_politicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chamado_slas_politica_sla_id",
                table: "chamado_slas",
                column: "politica_sla_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamado_slas_prazo_resolucao",
                table: "chamado_slas",
                column: "prazo_resolucao");

            migrationBuilder.CreateIndex(
                name: "IX_chamado_slas_prioridade_id",
                table: "chamado_slas",
                column: "prioridade_id");

            migrationBuilder.CreateIndex(
                name: "ux_chamado_slas_chamado_id",
                table: "chamado_slas",
                column: "chamado_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chamado_slas");
        }
    }
}
