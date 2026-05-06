using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailIntegrationLogSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "logs_integracao_email",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    fingerprint = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    remetente = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    nome_remetente = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    assunto = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    data_recebimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_processamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status_processamento = table.Column<int>(type: "integer", nullable: false),
                    erro = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tentativas = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_integracao_email", x => x.id);
                    table.ForeignKey(
                        name: "FK_logs_integracao_email_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_logs_integracao_email_chamado_id",
                table: "logs_integracao_email",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_logs_integracao_email_data_recebimento",
                table: "logs_integracao_email",
                column: "data_recebimento");

            migrationBuilder.CreateIndex(
                name: "ix_logs_integracao_email_status",
                table: "logs_integracao_email",
                column: "status_processamento");

            migrationBuilder.CreateIndex(
                name: "ux_logs_integracao_email_fingerprint",
                table: "logs_integracao_email",
                column: "fingerprint",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_logs_integracao_email_message_id",
                table: "logs_integracao_email",
                column: "message_id",
                unique: true,
                filter: "\"message_id\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs_integracao_email");
        }
    }
}
