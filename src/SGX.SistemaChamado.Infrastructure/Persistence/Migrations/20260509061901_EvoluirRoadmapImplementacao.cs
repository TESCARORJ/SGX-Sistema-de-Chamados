using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EvoluirRoadmapImplementacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "criterio_aceite",
                table: "roadmap_itsm_itens",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_conclusao_tecnica",
                table: "roadmap_itsm_itens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_homologacao",
                table: "roadmap_itsm_itens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidencia_implementacao",
                table: "roadmap_itsm_itens",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pendencias_homologacao",
                table: "roadmap_itsm_itens",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pendencias_tecnicas",
                table: "roadmap_itsm_itens",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "percentual_implementacao",
                table: "roadmap_itsm_itens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "proxima_acao",
                table: "roadmap_itsm_itens",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status_implementacao",
                table: "roadmap_itsm_itens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status_tecnico",
                table: "roadmap_itsm_itens",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "roadmap_implementacoes_futuras",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roadmap_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    responsavel = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    prazo_alvo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_implementacoes_futuras", x => x.id);
                    table.ForeignKey(
                        name: "FK_roadmap_implementacoes_futuras_roadmap_itsm_itens_roadmap_i~",
                        column: x => x.roadmap_item_id,
                        principalTable: "roadmap_itsm_itens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999033"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999034"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999035"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999036"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999037"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999038"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999039"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999040"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999041"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999042"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999043"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999044"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999045"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999046"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999047"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999048"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999051"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999052"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999053"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999054"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999055"));

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888829"), "Visualizar", true, null, null, "Roadmap.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Roadmap" },
                    { new Guid("88888888-8888-8888-8888-888888888830"), "Gerenciar", true, null, null, "Roadmap.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Roadmap" },
                    { new Guid("88888888-8888-8888-8888-888888888831"), "Visualizar", true, null, null, "RoadmapImplementacoes.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "RoadmapImplementacoes" },
                    { new Guid("88888888-8888-8888-8888-888888888832"), "Gerenciar", true, null, null, "RoadmapImplementacoes.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "RoadmapImplementacoes" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "prazo_alvo", "proxima_acao", "responsavel", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Administrador consegue gerenciar permissoes por perfil. Atendente visualiza apenas acoes permitidas. Solicitante nao acessa administracao. Backend bloqueia acoes sem permissao.", null, null, 1, "docs/SEGURANCA-PERFIS-PERMISSOES.md; docs/ROADMAP.md; testes backend com permissoes; matriz de permissoes no frontend.", "Status legado mantido para compatibilidade; usar StatusImplementacao como referencia principal.", "Validar com usuarios reais os perfis Administrador, Atendente e Solicitante, incluindo acoes permitidas e bloqueadas.", "Auditoria detalhada de alteracoes de permissoes, testes frontend/e2e da matriz de permissoes e validacao de permissoes finas em homologacao.", 90, new DateTime(2026, 5, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Executar homologacao com usuarios reais e priorizar auditoria detalhada.", "Thiago Tescaro", 1, 3, 3 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777714"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777715"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "data_homologacao", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { null, null, null, null, null, null, 0, null, 0, 0 });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888829"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999029") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888830"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999030") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888831"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999031") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888832"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999032") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999049") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999050") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_impl_futuras_ativo",
                table: "roadmap_implementacoes_futuras",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_impl_futuras_prioridade",
                table: "roadmap_implementacoes_futuras",
                column: "prioridade");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_impl_futuras_roadmap_item_id",
                table: "roadmap_implementacoes_futuras",
                column: "roadmap_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_impl_futuras_status",
                table: "roadmap_implementacoes_futuras",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_impl_futuras_tipo",
                table: "roadmap_implementacoes_futuras",
                column: "tipo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadmap_implementacoes_futuras");

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888829") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888830") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888831") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888832") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") });

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888829"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888830"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888831"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888832"));

            migrationBuilder.DropColumn(
                name: "criterio_aceite",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "data_conclusao_tecnica",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "data_homologacao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "evidencia_implementacao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "pendencias_homologacao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "pendencias_tecnicas",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "percentual_implementacao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "proxima_acao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "status_implementacao",
                table: "roadmap_itsm_itens");

            migrationBuilder.DropColumn(
                name: "status_tecnico",
                table: "roadmap_itsm_itens");

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999029"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999030"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999031"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999032"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999033"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999034"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999035"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999036"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999037"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999038"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999039"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999040"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999041"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999042"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999043"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999044"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999045"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999046"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999047"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999048"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999049"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                columns: new[] { "decisao", "observacao", "prazo_alvo", "responsavel", "status" },
                values: new object[] { 4, null, null, null, 3 });
        }
    }
}
