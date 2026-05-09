using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EvoluirRoadmapCategoriaChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "roadmap_categoria_id",
                table: "roadmap_itsm_itens",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "roadmap_categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    cor = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    icone = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roadmap_checklist_itens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roadmap_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    grupo = table.Column<int>(type: "integer", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    concluido = table.Column<bool>(type: "boolean", nullable: false),
                    obrigatorio = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_checklist_itens", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_checklist_itens_roadmap_itsm_itens_roadmap_item_id",
                        column: x => x.roadmap_item_id,
                        principalTable: "roadmap_itsm_itens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "roadmap_categorias",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "cor", "criado_em", "criado_por", "descricao", "icone", "nome", "ordem" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666601"), true, null, null, "#D32F2F", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Segurança e controle de acesso.", "shield", "Segurança", 1 },
                    { new Guid("66666666-6666-6666-6666-666666666602"), true, null, null, "#1976D2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fluxos operacionais de atendimento.", "support_agent", "Atendimento", 2 },
                    { new Guid("66666666-6666-6666-6666-666666666603"), true, null, null, "#5D4037", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Metas e acompanhamento de SLA.", "schedule", "SLA", 3 },
                    { new Guid("66666666-6666-6666-6666-666666666604"), true, null, null, "#00897B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Integrações com canais e sistemas.", "hub", "Integrações", 4 },
                    { new Guid("66666666-6666-6666-6666-666666666605"), true, null, null, "#7B1FA2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Cadastros e parametrizações administrativas.", "inventory_2", "Cadastros", 5 },
                    { new Guid("66666666-6666-6666-6666-666666666606"), true, null, null, "#F57C00", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Notificações e comunicação.", "notifications", "Notificações", 6 },
                    { new Guid("66666666-6666-6666-6666-666666666607"), true, null, null, "#455A64", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Infraestrutura e sustentação.", "dns", "Infraestrutura", 7 },
                    { new Guid("66666666-6666-6666-6666-666666666608"), true, null, null, "#C2185B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Experiência de uso e interface.", "palette", "UX", 8 },
                    { new Guid("66666666-6666-6666-6666-666666666609"), true, null, null, "#6D4C41", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Relatórios e exportações.", "assessment", "Relatórios", 9 },
                    { new Guid("66666666-6666-6666-6666-666666666610"), true, null, null, "#388E3C", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Validações e aceite com usuários.", "fact_check", "Homologação", 10 },
                    { new Guid("66666666-6666-6666-6666-666666666611"), true, null, null, "#303F9F", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Documentação técnica e funcional.", "description", "Documentação", 11 },
                    { new Guid("66666666-6666-6666-6666-666666666612"), true, null, null, "#3949AB", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Indicadores e governança gerencial.", "insights", "Gestão", 12 },
                    { new Guid("66666666-6666-6666-6666-666666666613"), true, null, null, "#546E7A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Rastreabilidade e governança.", "gavel", "Governança", 13 },
                    { new Guid("66666666-6666-6666-6666-666666666614"), true, null, null, "#8D6E63", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Base de conhecimento e catálogo.", "menu_book", "Conhecimento", 14 },
                    { new Guid("66666666-6666-6666-6666-666666666615"), true, null, null, "#1E88E5", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Fluxos e experiência do portal.", "language", "Portal", 15 }
                });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("67676767-6767-6767-6767-676767676701"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 1, true, 1, new Guid("77777777-7777-7777-7777-777777777703"), "Perfis macro criados" },
                    { new Guid("67676767-6767-6767-6767-676767676702"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 2, new Guid("77777777-7777-7777-7777-777777777703"), "CRUD de perfis criado" },
                    { new Guid("67676767-6767-6767-6767-676767676703"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 3, new Guid("77777777-7777-7777-7777-777777777703"), "Permissões granulares criadas" },
                    { new Guid("67676767-6767-6767-6767-676767676704"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 4, new Guid("77777777-7777-7777-7777-777777777703"), "Migration aplicada" },
                    { new Guid("67676767-6767-6767-6767-676767676705"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 5, new Guid("77777777-7777-7777-7777-777777777703"), "Seeds criados" },
                    { new Guid("67676767-6767-6767-6767-676767676706"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 6, new Guid("77777777-7777-7777-7777-777777777703"), "/api/me com permissões" },
                    { new Guid("67676767-6767-6767-6767-676767676707"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 7, new Guid("77777777-7777-7777-7777-777777777703"), "AuthorizationHandler criado" },
                    { new Guid("67676767-6767-6767-6767-676767676708"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 8, new Guid("77777777-7777-7777-7777-777777777703"), "Matriz de permissões no frontend" },
                    { new Guid("67676767-6767-6767-6767-676767676709"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 9, new Guid("77777777-7777-7777-7777-777777777703"), "Controle visual por permissão" },
                    { new Guid("67676767-6767-6767-6767-676767676710"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 10, new Guid("77777777-7777-7777-7777-777777777703"), "Homologação com usuários reais" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666615"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666604"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666601"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666601"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666603"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666613"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666602"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666605"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666612"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666614"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666607"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666614"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666602"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777714"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666606"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777715"),
                column: "roadmap_categoria_id",
                value: new Guid("66666666-6666-6666-6666-666666666609"));

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666601'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Seguranca' OR
                     categoria ILIKE 'Segurança' OR
                     categoria ILIKE '%seguran%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666602'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Atendimento' OR
                     categoria ILIKE '%atend%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666603'
                 WHERE roadmap_categoria_id IS NULL
                   AND categoria ILIKE 'SLA';
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666604'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Integracoes' OR
                     categoria ILIKE 'Integrações' OR
                     categoria ILIKE '%integr%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666605'
                 WHERE roadmap_categoria_id IS NULL
                   AND categoria ILIKE '%cadastro%';
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666606'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Notificacoes' OR
                     categoria ILIKE 'Notificações' OR
                     categoria ILIKE '%notific%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666607'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Infraestrutura' OR
                     categoria ILIKE '%infra%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666608'
                 WHERE roadmap_categoria_id IS NULL
                   AND categoria ILIKE 'UX';
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666609'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Relatorios' OR
                     categoria ILIKE 'Relatórios' OR
                     categoria ILIKE '%relat%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666610'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Homologacao' OR
                     categoria ILIKE 'Homologação' OR
                     categoria ILIKE '%homolog%'
                   );
                """);

            migrationBuilder.Sql("""
                UPDATE roadmap_itsm_itens
                   SET roadmap_categoria_id = '66666666-6666-6666-6666-666666666611'
                 WHERE roadmap_categoria_id IS NULL
                   AND (
                     categoria ILIKE 'Documentacao' OR
                     categoria ILIKE 'Documentação' OR
                     categoria ILIKE '%document%'
                   );
                """);

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_itsm_itens_roadmap_categoria_id",
                table: "roadmap_itsm_itens",
                column: "roadmap_categoria_id");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_categorias_ativo",
                table: "roadmap_categorias",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_categorias_ordem",
                table: "roadmap_categorias",
                column: "ordem");

            migrationBuilder.CreateIndex(
                name: "ux_roadmap_categorias_nome",
                table: "roadmap_categorias",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_checklist_itens_ativo",
                table: "roadmap_checklist_itens",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_checklist_itens_grupo",
                table: "roadmap_checklist_itens",
                column: "grupo");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_checklist_itens_item_ordem",
                table: "roadmap_checklist_itens",
                columns: new[] { "roadmap_item_id", "ordem" });

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_checklist_itens_roadmap_item_id",
                table: "roadmap_checklist_itens",
                column: "roadmap_item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_roadmap_itsm_itens_roadmap_categorias_roadmap_categoria_id",
                table: "roadmap_itsm_itens",
                column: "roadmap_categoria_id",
                principalTable: "roadmap_categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roadmap_itsm_itens_roadmap_categorias_roadmap_categoria_id",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropTable(
                name: "roadmap_categorias");

            migrationBuilder.DropTable(
                name: "roadmap_checklist_itens");

            migrationBuilder.DropIndex(
                name: "ix_roadmap_itsm_itens_roadmap_categoria_id",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "roadmap_categoria_id",
                table: "roadmap_itsm_itens");
        }
    }
}
