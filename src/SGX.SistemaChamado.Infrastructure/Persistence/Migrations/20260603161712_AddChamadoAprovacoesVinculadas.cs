using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChamadoAprovacoesVinculadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_aprovacoes_chamado_chamado_id_pendente_ativo",
                table: "aprovacoes_chamado");

            migrationBuilder.AddColumn<bool>(
                name: "bloqueia_avanco_atendimento",
                table: "aprovacoes_chamado",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancelado_em",
                table: "aprovacoes_chamado",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cancelado_por_usuario_id",
                table: "aprovacoes_chamado",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "aprovacoes_chamado",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "motivo_cancelamento",
                table: "aprovacoes_chamado",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "titulo",
                table: "aprovacoes_chamado",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "Aprovacao");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_ativo",
                table: "aprovacoes_chamado",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_cancelado_por_usuario_id",
                table: "aprovacoes_chamado",
                column: "cancelado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_chamado_id_ativo_status",
                table: "aprovacoes_chamado",
                columns: new[] { "chamado_id", "ativo", "status" });

            migrationBuilder.AddForeignKey(
                name: "FK_aprovacoes_chamado_usuarios_cancelado_por_usuario_id",
                table: "aprovacoes_chamado",
                column: "cancelado_por_usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aprovacoes_chamado_usuarios_cancelado_por_usuario_id",
                table: "aprovacoes_chamado");

            migrationBuilder.DropIndex(
                name: "ix_aprovacoes_chamado_ativo",
                table: "aprovacoes_chamado");

            migrationBuilder.DropIndex(
                name: "ix_aprovacoes_chamado_cancelado_por_usuario_id",
                table: "aprovacoes_chamado");

            migrationBuilder.DropIndex(
                name: "ix_aprovacoes_chamado_chamado_id_ativo_status",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "bloqueia_avanco_atendimento",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "cancelado_em",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "cancelado_por_usuario_id",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "motivo_cancelamento",
                table: "aprovacoes_chamado");

            migrationBuilder.DropColumn(
                name: "titulo",
                table: "aprovacoes_chamado");

            migrationBuilder.CreateIndex(
                name: "ux_aprovacoes_chamado_chamado_id_pendente_ativo",
                table: "aprovacoes_chamado",
                columns: new[] { "chamado_id", "ativo", "status" },
                unique: true,
                filter: "ativo = true AND status = 1");
        }
    }
}
