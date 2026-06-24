using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEstadoLeituraNotificacaoSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "lida_em",
                table: "notificacoes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_notificacoes_destinatario_canal_status_criado_em",
                table: "notificacoes",
                columns: new[] { "destinatario_usuario_id", "canal", "status", "criado_em" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_notificacoes_lida_em_maior_ou_igual_enviada_em",
                table: "notificacoes",
                sql: "lida_em IS NULL OR enviada_em IS NULL OR lida_em >= enviada_em");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_notificacoes_destinatario_canal_status_criado_em",
                table: "notificacoes");

            migrationBuilder.DropCheckConstraint(
                name: "ck_notificacoes_lida_em_maior_ou_igual_enviada_em",
                table: "notificacoes");

            migrationBuilder.DropColumn(
                name: "lida_em",
                table: "notificacoes");
        }
    }
}
