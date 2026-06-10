using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CriarConfiguracaoRegraAprovacaoSprint4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracoes_regras_aprovacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    tipo_regra = table.Column<int>(type: "integer", nullable: false),
                    escopo_regra = table.Column<int>(type: "integer", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    versao = table.Column<int>(type: "integer", nullable: false),
                    natureza_chamado = table.Column<int>(type: "integer", nullable: true),
                    tipo_solicitacao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    catalogo_servico_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    impacto_minimo = table.Column<int>(type: "integer", nullable: true),
                    urgencia_minima = table.Column<int>(type: "integer", nullable: true),
                    prioridade_minima = table.Column<int>(type: "integer", nullable: true),
                    custo_minimo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    nivel_risco_minimo = table.Column<int>(type: "integer", nullable: true),
                    exige_aprovacao = table.Column<bool>(type: "boolean", nullable: false),
                    bloqueante = table.Column<bool>(type: "boolean", nullable: false),
                    permite_reenvio = table.Column<bool>(type: "boolean", nullable: false),
                    permite_fallback = table.Column<bool>(type: "boolean", nullable: false),
                    efeito_operacional = table.Column<int>(type: "integer", nullable: false),
                    tipo_fluxo_aprovacao = table.Column<int>(type: "integer", nullable: false),
                    tipo_resolucao_aprovador = table.Column<int>(type: "integer", nullable: false),
                    aprovador_especifico_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_padrao_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prazo_decisao_horas = table.Column<int>(type: "integer", nullable: true),
                    vigente_de = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    vigente_ate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_configuracoes_regras_aprovacao", x => x.id);
                    table.CheckConstraint("ck_configuracoes_regras_aprovacao_custo_minimo", "custo_minimo IS NULL OR custo_minimo >= 0");
                    table.CheckConstraint("ck_configuracoes_regras_aprovacao_nivel_risco", "nivel_risco_minimo IS NULL OR nivel_risco_minimo > 0");
                    table.CheckConstraint("ck_configuracoes_regras_aprovacao_prazo_decisao", "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
                    table.CheckConstraint("ck_configuracoes_regras_aprovacao_subcategoria_categoria", "subcategoria_id IS NULL OR categoria_id IS NOT NULL");
                    table.CheckConstraint("ck_configuracoes_regras_aprovacao_vigencia", "vigente_ate IS NULL OR vigente_de IS NULL OR vigente_ate >= vigente_de");
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_catalogo_servicos_catalogo_s~",
                        column: x => x.catalogo_servico_id,
                        principalTable: "catalogo_servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_categorias_chamado_categoria~",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_subcategorias_chamado_subcat~",
                        column: x => x.subcategoria_id,
                        principalTable: "subcategorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_tipos_solicitacao_tipo_solic~",
                        column: x => x.tipo_solicitacao_id,
                        principalTable: "tipos_solicitacao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_usuarios_aprovador_especific~",
                        column: x => x.aprovador_especifico_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_usuarios_aprovador_padrao_us~",
                        column: x => x.aprovador_padrao_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_usuarios_atualizado_por_usua~",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_configuracoes_regras_aprovacao_usuarios_criado_por_usuario_~",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000324"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 43, "Modelar instancia de aprovacao do chamado." });

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_aprovador_especifico_usuario_id",
                table: "configuracoes_regras_aprovacao",
                column: "aprovador_especifico_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_aprovador_padrao_usuario_id",
                table: "configuracoes_regras_aprovacao",
                column: "aprovador_padrao_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_ativo",
                table: "configuracoes_regras_aprovacao",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_atualizado_por_usuario_id",
                table: "configuracoes_regras_aprovacao",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_catalogo_servico_id",
                table: "configuracoes_regras_aprovacao",
                column: "catalogo_servico_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_categoria_id",
                table: "configuracoes_regras_aprovacao",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_criado_por_usuario_id",
                table: "configuracoes_regras_aprovacao",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_escopo_ordem_prioridade",
                table: "configuracoes_regras_aprovacao",
                columns: new[] { "ativo", "escopo_regra", "ordem", "prioridade" });

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_impacto_minimo",
                table: "configuracoes_regras_aprovacao",
                column: "impacto_minimo");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_natureza",
                table: "configuracoes_regras_aprovacao",
                column: "natureza_chamado");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_prioridade_minima",
                table: "configuracoes_regras_aprovacao",
                column: "prioridade_minima");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_subcategoria_id",
                table: "configuracoes_regras_aprovacao",
                column: "subcategoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_tipo_regra",
                table: "configuracoes_regras_aprovacao",
                column: "tipo_regra");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_tipo_solicitacao_id",
                table: "configuracoes_regras_aprovacao",
                column: "tipo_solicitacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_urgencia_minima",
                table: "configuracoes_regras_aprovacao",
                column: "urgencia_minima");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_regras_aprovacao_vigencia",
                table: "configuracoes_regras_aprovacao",
                columns: new[] { "vigente_de", "vigente_ate" });

            migrationBuilder.CreateIndex(
                name: "ux_configuracoes_regras_aprovacao_nome_versao",
                table: "configuracoes_regras_aprovacao",
                columns: new[] { "nome", "versao" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracoes_regras_aprovacao");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000324"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 41, "Modelar configuracao de regra de aprovacao." });
        }
    }
}
