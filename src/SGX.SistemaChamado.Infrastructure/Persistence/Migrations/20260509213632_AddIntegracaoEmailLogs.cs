using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIntegracaoEmailLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "destinatario",
                table: "logs_integracao_email",
                type: "character varying(1200)",
                maxLength: 1200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "in_reply_to",
                table: "logs_integracao_email",
                type: "character varying(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "references",
                table: "logs_integracao_email",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "destinatario",
                table: "logs_integracao_email");

            migrationBuilder.DropColumn(
                name: "in_reply_to",
                table: "logs_integracao_email");

            migrationBuilder.DropColumn(
                name: "references",
                table: "logs_integracao_email");
        }
    }
}
