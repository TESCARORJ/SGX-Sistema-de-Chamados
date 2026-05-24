using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3InventarioAtivosHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "historico_inventario_ativos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventario_ativo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_movimentacao = table.Column<int>(type: "integer", nullable: false),
                    departamento_origem_id = table.Column<Guid>(type: "uuid", nullable: true),
                    departamento_destino_id = table.Column<Guid>(type: "uuid", nullable: true),
                    local_unidade_origem_id = table.Column<Guid>(type: "uuid", nullable: true),
                    local_unidade_destino_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usuario_responsavel_origem_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usuario_responsavel_destino_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status_operacional_anterior = table.Column<int>(type: "integer", nullable: true),
                    status_operacional_novo = table.Column<int>(type: "integer", nullable: true),
                    status_patrimonial_anterior = table.Column<int>(type: "integer", nullable: true),
                    status_patrimonial_novo = table.Column<int>(type: "integer", nullable: true),
                    observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_inventario_ativos", x => x.id);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_departamentos_departamento_dest~",
                        column: x => x.departamento_destino_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_departamentos_departamento_orig~",
                        column: x => x.departamento_origem_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_inventario_ativos_inventario_at~",
                        column: x => x.inventario_ativo_id,
                        principalTable: "inventario_ativos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_locais_unidade_local_unidade_de~",
                        column: x => x.local_unidade_destino_id,
                        principalTable: "locais_unidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_locais_unidade_local_unidade_or~",
                        column: x => x.local_unidade_origem_id,
                        principalTable: "locais_unidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_usuarios_usuario_responsavel_de~",
                        column: x => x.usuario_responsavel_destino_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historico_inventario_ativos_usuarios_usuario_responsavel_or~",
                        column: x => x.usuario_responsavel_origem_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_historico_inventario_ativos_criado_em",
                table: "historico_inventario_ativos",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_criado_por_usuario_id",
                table: "historico_inventario_ativos",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_departamento_destino_id",
                table: "historico_inventario_ativos",
                column: "departamento_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_departamento_origem_id",
                table: "historico_inventario_ativos",
                column: "departamento_origem_id");

            migrationBuilder.CreateIndex(
                name: "ix_historico_inventario_ativos_inventario_ativo_id",
                table: "historico_inventario_ativos",
                column: "inventario_ativo_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_local_unidade_destino_id",
                table: "historico_inventario_ativos",
                column: "local_unidade_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_local_unidade_origem_id",
                table: "historico_inventario_ativos",
                column: "local_unidade_origem_id");

            migrationBuilder.CreateIndex(
                name: "ix_historico_inventario_ativos_tipo_movimentacao",
                table: "historico_inventario_ativos",
                column: "tipo_movimentacao");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_usuario_responsavel_destino_id",
                table: "historico_inventario_ativos",
                column: "usuario_responsavel_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_inventario_ativos_usuario_responsavel_origem_id",
                table: "historico_inventario_ativos",
                column: "usuario_responsavel_origem_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historico_inventario_ativos");
        }
    }
}
