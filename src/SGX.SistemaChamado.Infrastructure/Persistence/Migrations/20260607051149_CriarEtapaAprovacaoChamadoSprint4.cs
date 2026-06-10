using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarEtapaAprovacaoChamadoSprint4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "etapas_aprovacao_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    instancia_aprovacao_chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tipo_etapa = table.Column<int>(type: "integer", nullable: false),
                    tipo_fluxo_aprovacao = table.Column<int>(type: "integer", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    ramo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    obrigatoria = table.Column<bool>(type: "boolean", nullable: false),
                    critica_para_consolidacao = table.Column<bool>(type: "boolean", nullable: false),
                    permite_reenvio = table.Column<bool>(type: "boolean", nullable: false),
                    permite_fallback = table.Column<bool>(type: "boolean", nullable: false),
                    permite_delegacao = table.Column<bool>(type: "boolean", nullable: false),
                    tipo_resolucao_aprovador = table.Column<int>(type: "integer", nullable: false),
                    aprovador_especifico_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_padrao_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_resolvido_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    grupo_aprovador_snapshot = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    quorum_minimo = table.Column<int>(type: "integer", nullable: true),
                    quantidade_aprovacoes_necessarias = table.Column<int>(type: "integer", nullable: true),
                    solicitante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    solicitada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    prazo_decisao_horas = table.Column<int>(type: "integer", nullable: true),
                    deve_expirar_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expirada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelada_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    motivo_cancelamento = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    decidida_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    escopo_resumo_snapshot = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_etapas_aprovacao_chamado", x => x.id);
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_expiracao_planejada", "deve_expirar_em IS NULL OR deve_expirar_em >= solicitada_em");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_nivel", "nivel > 0");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_ordem", "ordem >= 0");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_prazo_decisao", "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_qtd_aprovacoes_necessarias", "quantidade_aprovacoes_necessarias IS NULL OR quantidade_aprovacoes_necessarias > 0");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_quorum_minimo", "quorum_minimo IS NULL OR quorum_minimo > 0");
                    table.CheckConstraint("ck_etapas_aprovacao_chamado_regra_versao_snapshot", "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_instancias_aprovacao_chamado_insta~",
                        column: x => x.instancia_aprovacao_chamado_id,
                        principalTable: "instancias_aprovacao_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_aprovador_especifico_usua~",
                        column: x => x.aprovador_especifico_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_aprovador_padrao_usuario_~",
                        column: x => x.aprovador_padrao_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_aprovador_resolvido_usuar~",
                        column: x => x.aprovador_resolvido_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_cancelada_por_usuario_id",
                        column: x => x.cancelada_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etapas_aprovacao_chamado_usuarios_solicitante_id",
                        column: x => x.solicitante_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000326"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 46, "Modelar decisao de aprovacao." });

            migrationBuilder.CreateIndex(
                name: "IX_etapas_aprovacao_chamado_aprovador_especifico_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "aprovador_especifico_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_aprovacao_chamado_aprovador_padrao_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "aprovador_padrao_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_aprovador_resolvido_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "aprovador_resolvido_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_atualizado_por_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_etapas_aprovacao_chamado_cancelada_por_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "cancelada_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_criado_por_usuario_id",
                table: "etapas_aprovacao_chamado",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_deve_expirar_em",
                table: "etapas_aprovacao_chamado",
                column: "deve_expirar_em");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_instancia_aprovacao_chamado_id",
                table: "etapas_aprovacao_chamado",
                column: "instancia_aprovacao_chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_instancia_nivel_ordem_ramo",
                table: "etapas_aprovacao_chamado",
                columns: new[] { "instancia_aprovacao_chamado_id", "nivel", "ordem", "ramo" });

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_solicitada_em",
                table: "etapas_aprovacao_chamado",
                column: "solicitada_em");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_solicitante_id",
                table: "etapas_aprovacao_chamado",
                column: "solicitante_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_status",
                table: "etapas_aprovacao_chamado",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_tipo_etapa",
                table: "etapas_aprovacao_chamado",
                column: "tipo_etapa");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_aprovacao_chamado_tipo_fluxo_aprovacao",
                table: "etapas_aprovacao_chamado",
                column: "tipo_fluxo_aprovacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etapas_aprovacao_chamado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000326"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 44, "Modelar etapa de aprovacao." });
        }
    }
}
