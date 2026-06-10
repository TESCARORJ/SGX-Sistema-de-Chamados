using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarDecisaoAprovacaoChamadoSprint4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_etapas_aprovacao_chamado_id_instancia_aprovacao_chamado_id",
                table: "etapas_aprovacao_chamado",
                columns: new[] { "id", "instancia_aprovacao_chamado_id" });

            migrationBuilder.CreateTable(
                name: "decisoes_aprovacao_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    instancia_aprovacao_chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    etapa_aprovacao_chamado_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_decisao = table.Column<int>(type: "integer", nullable: false),
                    resultado = table.Column<int>(type: "integer", nullable: false),
                    data_decisao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    decisor_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    papel_decisor_snapshot = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    autoridade_decisor_snapshot = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    decisor_eh_aprovador_especifico = table.Column<bool>(type: "boolean", nullable: false),
                    decisor_eh_aprovador_padrao = table.Column<bool>(type: "boolean", nullable: false),
                    decisor_eh_membro_grupo = table.Column<bool>(type: "boolean", nullable: false),
                    decisor_por_delegacao = table.Column<bool>(type: "boolean", nullable: false),
                    grupo_aprovador_snapshot = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    quorum_esperado = table.Column<int>(type: "integer", nullable: true),
                    quorum_atingido = table.Column<int>(type: "integer", nullable: true),
                    justificativa = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    escopo_decidido_snapshot = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    efeito_operacional = table.Column<int>(type: "integer", nullable: false),
                    decisao_parcial = table.Column<bool>(type: "boolean", nullable: false),
                    decisao_final = table.Column<bool>(type: "boolean", nullable: false),
                    libera_avanco = table.Column<bool>(type: "boolean", nullable: false),
                    mantem_bloqueio = table.Column<bool>(type: "boolean", nullable: false),
                    exige_reavaliacao = table.Column<bool>(type: "boolean", nullable: false),
                    permite_nova_solicitacao = table.Column<bool>(type: "boolean", nullable: false),
                    cancela_fluxo = table.Column<bool>(type: "boolean", nullable: false),
                    status_instancia_anterior = table.Column<int>(type: "integer", nullable: false),
                    status_instancia_novo = table.Column<int>(type: "integer", nullable: false),
                    status_etapa_anterior = table.Column<int>(type: "integer", nullable: true),
                    status_etapa_novo = table.Column<int>(type: "integer", nullable: true),
                    status_chamado_anterior_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status_chamado_novo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nivel_etapa_snapshot = table.Column<int>(type: "integer", nullable: true),
                    ordem_etapa_snapshot = table.Column<int>(type: "integer", nullable: true),
                    ramo_etapa_snapshot = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
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
                    table.PrimaryKey("PK_decisoes_aprovacao_chamado", x => x.id);
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_bloqueio_liberacao", "NOT (libera_avanco AND mantem_bloqueio)");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_etapa_status", "(etapa_aprovacao_chamado_id IS NULL AND status_etapa_anterior IS NULL AND status_etapa_novo IS NULL AND nivel_etapa_snapshot IS NULL AND ordem_etapa_snapshot IS NULL AND ramo_etapa_snapshot IS NULL) OR (etapa_aprovacao_chamado_id IS NOT NULL AND status_etapa_anterior IS NOT NULL AND status_etapa_novo IS NOT NULL)");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_nivel_etapa_snapshot", "nivel_etapa_snapshot IS NULL OR nivel_etapa_snapshot > 0");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_ordem_etapa_snapshot", "ordem_etapa_snapshot IS NULL OR ordem_etapa_snapshot >= 0");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_quorum_atingido", "quorum_atingido IS NULL OR quorum_atingido > 0");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_quorum_dependencia", "quorum_atingido IS NULL OR quorum_esperado IS NOT NULL");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_quorum_esperado", "quorum_esperado IS NULL OR quorum_esperado > 0");
                    table.CheckConstraint("ck_decisoes_aprovacao_chamado_regra_versao_snapshot", "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_etapas_aprovacao_chamado_etapa_a~",
                        columns: x => new { x.etapa_aprovacao_chamado_id, x.instancia_aprovacao_chamado_id },
                        principalTable: "etapas_aprovacao_chamado",
                        principalColumns: new[] { "id", "instancia_aprovacao_chamado_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_instancias_aprovacao_chamado_ins~",
                        column: x => x.instancia_aprovacao_chamado_id,
                        principalTable: "instancias_aprovacao_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_status_chamado_status_chamado_an~",
                        column: x => x.status_chamado_anterior_id,
                        principalTable: "status_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_status_chamado_status_chamado_no~",
                        column: x => x.status_chamado_novo_id,
                        principalTable: "status_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_usuarios_atualizado_por_usuario_~",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_decisoes_aprovacao_chamado_usuarios_decisor_usuario_id",
                        column: x => x.decisor_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000327"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 47, "Criar migrations estruturais do motor de aprovacao." });

            migrationBuilder.CreateIndex(
                name: "ux_etapas_aprovacao_chamado_id_instancia",
                table: "etapas_aprovacao_chamado",
                columns: new[] { "id", "instancia_aprovacao_chamado_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_ativo",
                table: "decisoes_aprovacao_chamado",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_atualizado_por_usuario_id",
                table: "decisoes_aprovacao_chamado",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_criado_por_usuario_id",
                table: "decisoes_aprovacao_chamado",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_data_decisao",
                table: "decisoes_aprovacao_chamado",
                column: "data_decisao");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_decisor_usuario_id",
                table: "decisoes_aprovacao_chamado",
                column: "decisor_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_etapa_aprovacao_chamado_id",
                table: "decisoes_aprovacao_chamado",
                column: "etapa_aprovacao_chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_decisoes_aprovacao_chamado_etapa_aprovacao_chamado_id_insta~",
                table: "decisoes_aprovacao_chamado",
                columns: new[] { "etapa_aprovacao_chamado_id", "instancia_aprovacao_chamado_id" });

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_instancia_aprovacao_chamado_id",
                table: "decisoes_aprovacao_chamado",
                column: "instancia_aprovacao_chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_instancia_data_decisao",
                table: "decisoes_aprovacao_chamado",
                columns: new[] { "instancia_aprovacao_chamado_id", "data_decisao" });

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_instancia_etapa_tipo_decisao",
                table: "decisoes_aprovacao_chamado",
                columns: new[] { "instancia_aprovacao_chamado_id", "etapa_aprovacao_chamado_id", "tipo_decisao" });

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_resultado",
                table: "decisoes_aprovacao_chamado",
                column: "resultado");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_status_chamado_anterior_id",
                table: "decisoes_aprovacao_chamado",
                column: "status_chamado_anterior_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_status_chamado_novo_id",
                table: "decisoes_aprovacao_chamado",
                column: "status_chamado_novo_id");

            migrationBuilder.CreateIndex(
                name: "ix_decisoes_aprovacao_chamado_tipo_decisao",
                table: "decisoes_aprovacao_chamado",
                column: "tipo_decisao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "decisoes_aprovacao_chamado");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_etapas_aprovacao_chamado_id_instancia_aprovacao_chamado_id",
                table: "etapas_aprovacao_chamado");

            migrationBuilder.DropIndex(
                name: "ux_etapas_aprovacao_chamado_id_instancia",
                table: "etapas_aprovacao_chamado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000327"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 46, "Modelar decisao de aprovacao." });
        }
    }
}
