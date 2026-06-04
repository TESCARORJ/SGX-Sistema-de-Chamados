using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChamadoRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chamados_relacionamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_origem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_destino_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_relacionamento = table.Column<int>(type: "integer", nullable: false),
                    justificativa = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    removido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    removido_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_remocao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamados_relacionamentos", x => x.id);
                    table.CheckConstraint("ck_chamados_relacionamentos_origem_destino_diferentes", "chamado_origem_id <> chamado_destino_id");
                    table.ForeignKey(
                        name: "FK_chamados_relacionamentos_chamados_chamado_destino_id",
                        column: x => x.chamado_destino_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_relacionamentos_chamados_chamado_origem_id",
                        column: x => x.chamado_origem_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_relacionamentos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_relacionamentos_usuarios_removido_por_usuario_id",
                        column: x => x.removido_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_chamado_destino_id",
                table: "chamados_relacionamentos",
                column: "chamado_destino_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_chamado_origem_id",
                table: "chamados_relacionamentos",
                column: "chamado_origem_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_criado_por_usuario_id",
                table: "chamados_relacionamentos",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_origem_destino_tipo_ativo",
                table: "chamados_relacionamentos",
                columns: new[] { "chamado_origem_id", "chamado_destino_id", "tipo_relacionamento", "ativo" });

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_removido_por_usuario_id",
                table: "chamados_relacionamentos",
                column: "removido_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_relacionamentos_tipo_relacionamento",
                table: "chamados_relacionamentos",
                column: "tipo_relacionamento");

            migrationBuilder.CreateIndex(
                name: "ux_chamados_relacionamentos_origem_destino_tipo_ativo",
                table: "chamados_relacionamentos",
                columns: new[] { "chamado_origem_id", "chamado_destino_id", "tipo_relacionamento" },
                unique: true,
                filter: "ativo = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chamados_relacionamentos");
        }
    }
}
