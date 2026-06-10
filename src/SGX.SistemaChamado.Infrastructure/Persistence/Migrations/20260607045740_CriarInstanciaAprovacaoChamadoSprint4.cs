using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarInstanciaAprovacaoChamadoSprint4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "instancias_aprovacao_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    configuracao_regra_aprovacao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovacao_chamado_legada_id = table.Column<Guid>(type: "uuid", nullable: true),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    origem = table.Column<int>(type: "integer", nullable: false),
                    tipo_fluxo_aprovacao = table.Column<int>(type: "integer", nullable: false),
                    efeito_operacional = table.Column<int>(type: "integer", nullable: false),
                    escopo_regra = table.Column<int>(type: "integer", nullable: false),
                    tipo_regra = table.Column<int>(type: "integer", nullable: false),
                    natureza_chamado = table.Column<int>(type: "integer", nullable: true),
                    tipo_solicitacao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    catalogo_servico_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    impacto_avaliado = table.Column<int>(type: "integer", nullable: true),
                    urgencia_avaliada = table.Column<int>(type: "integer", nullable: true),
                    prioridade_avaliada = table.Column<int>(type: "integer", nullable: true),
                    custo_avaliado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    nivel_risco_avaliado = table.Column<int>(type: "integer", nullable: true),
                    exige_aprovacao = table.Column<bool>(type: "boolean", nullable: false),
                    bloqueante = table.Column<bool>(type: "boolean", nullable: false),
                    permite_reenvio = table.Column<bool>(type: "boolean", nullable: false),
                    permite_fallback = table.Column<bool>(type: "boolean", nullable: false),
                    tipo_resolucao_aprovador = table.Column<int>(type: "integer", nullable: false),
                    aprovador_especifico_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_padrao_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_resolvido_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    solicitante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    solicitada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prazo_decisao_horas = table.Column<int>(type: "integer", nullable: true),
                    deve_expirar_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expirada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelada_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_cancelamento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    decidida_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    regra_nome_snapshot = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    regra_versao_snapshot = table.Column<int>(type: "integer", nullable: true),
                    regra_criterio_snapshot = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instancias_aprovacao_chamado", x => x.id);
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_custo_avaliado", "custo_avaliado IS NULL OR custo_avaliado >= 0");
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_expiracao_planejada", "deve_expirar_em IS NULL OR deve_expirar_em >= solicitada_em");
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_nivel_risco_avaliado", "nivel_risco_avaliado IS NULL OR nivel_risco_avaliado > 0");
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_prazo_decisao", "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_regra_versao_snapshot", "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
                    table.CheckConstraint("ck_instancias_aprovacao_chamado_subcategoria_categoria", "subcategoria_id IS NULL OR categoria_id IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_aprovacoes_chamado_aprovacao_c~",
                        column: x => x.aprovacao_chamado_legada_id,
                        principalTable: "aprovacoes_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_catalogo_servicos_catalogo_ser~",
                        column: x => x.catalogo_servico_id,
                        principalTable: "catalogo_servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_categorias_chamado_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_configuracoes_regras_aprovacao~",
                        column: x => x.configuracao_regra_aprovacao_id,
                        principalTable: "configuracoes_regras_aprovacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_subcategorias_chamado_subcateg~",
                        column: x => x.subcategoria_id,
                        principalTable: "subcategorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_tipos_solicitacao_tipo_solicit~",
                        column: x => x.tipo_solicitacao_id,
                        principalTable: "tipos_solicitacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_aprovador_especifico_~",
                        column: x => x.aprovador_especifico_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_aprovador_padrao_usua~",
                        column: x => x.aprovador_padrao_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_aprovador_resolvido_u~",
                        column: x => x.aprovador_resolvido_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_atualizado_por_usuari~",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_cancelada_por_usuario~",
                        column: x => x.cancelada_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instancias_aprovacao_chamado_usuarios_solicitante_id",
                        column: x => x.solicitante_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000325"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 44, "Modelar etapa de aprovacao." });

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_aprovador_especifico_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "aprovador_especifico_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_aprovador_padrao_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "aprovador_padrao_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_aprovador_resolvido_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "aprovador_resolvido_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_ativo",
                table: "instancias_aprovacao_chamado",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_atualizado_por_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_cancelada_por_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "cancelada_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_catalogo_servico_id",
                table: "instancias_aprovacao_chamado",
                column: "catalogo_servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_categoria_id",
                table: "instancias_aprovacao_chamado",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_chamado_id",
                table: "instancias_aprovacao_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_chamado_id_ativo_status",
                table: "instancias_aprovacao_chamado",
                columns: new[] { "chamado_id", "ativo", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_configuracao_regra_aprovacao_id",
                table: "instancias_aprovacao_chamado",
                column: "configuracao_regra_aprovacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_criado_por_usuario_id",
                table: "instancias_aprovacao_chamado",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_deve_expirar_em",
                table: "instancias_aprovacao_chamado",
                column: "deve_expirar_em");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_origem",
                table: "instancias_aprovacao_chamado",
                column: "origem");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_solicitada_em",
                table: "instancias_aprovacao_chamado",
                column: "solicitada_em");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_solicitante_id",
                table: "instancias_aprovacao_chamado",
                column: "solicitante_id");

            migrationBuilder.CreateIndex(
                name: "ix_instancias_aprovacao_chamado_status",
                table: "instancias_aprovacao_chamado",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_subcategoria_id",
                table: "instancias_aprovacao_chamado",
                column: "subcategoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_instancias_aprovacao_chamado_tipo_solicitacao_id",
                table: "instancias_aprovacao_chamado",
                column: "tipo_solicitacao_id");

            migrationBuilder.CreateIndex(
                name: "ux_instancias_aprovacao_chamado_aprovacao_chamado_legada_id",
                table: "instancias_aprovacao_chamado",
                column: "aprovacao_chamado_legada_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instancias_aprovacao_chamado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000325"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 43, "Modelar instancia de aprovacao do chamado." });
        }
    }
}
