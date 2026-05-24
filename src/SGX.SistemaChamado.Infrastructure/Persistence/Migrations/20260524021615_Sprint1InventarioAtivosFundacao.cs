using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1InventarioAtivosFundacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipos_ativo_inventario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_ativo_inventario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventario_ativos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    numero_patrimonio = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    numero_serie = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    tipo_ativo_inventario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fabricante = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    modelo = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    departamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    local_unidade_id = table.Column<Guid>(type: "uuid", nullable: true),
                    usuario_responsavel_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status_operacional = table.Column<int>(type: "integer", nullable: false),
                    status_patrimonial = table.Column<int>(type: "integer", nullable: false),
                    criticidade = table.Column<int>(type: "integer", nullable: false),
                    data_aquisicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_fim_garantia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valor_aquisicao = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    fornecedor = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inativado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    inativado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventario_ativos", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_locais_unidade_local_unidade_id",
                        column: x => x.local_unidade_id,
                        principalTable: "locais_unidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_tipos_ativo_inventario_tipo_ativo_inventa~",
                        column: x => x.tipo_ativo_inventario_id,
                        principalTable: "tipos_ativo_inventario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_usuarios_inativado_por_usuario_id",
                        column: x => x.inativado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inventario_ativos_usuarios_usuario_responsavel_id",
                        column: x => x.usuario_responsavel_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999057"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999058"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999059"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999060"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999061"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999062"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999063"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999064"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999065"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999066"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999067"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999068"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999069"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999070"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999071"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999072"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999073"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999074"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888836") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999075"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888843") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999076"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888847") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999077"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888848") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999078"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999082"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999083"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999084"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999085"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999086"));

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888852"), "Visualizar", true, null, null, "InventarioAtivos.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "InventarioAtivos" },
                    { new Guid("88888888-8888-8888-8888-888888888853"), "Gerenciar", true, null, null, "InventarioAtivos.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "InventarioAtivos" },
                    { new Guid("88888888-8888-8888-8888-888888888854"), "Inativar", true, null, null, "InventarioAtivos.Inativar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "InventarioAtivos" },
                    { new Guid("88888888-8888-8888-8888-888888888855"), "Movimentar", true, null, null, "InventarioAtivos.Movimentar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "InventarioAtivos" },
                    { new Guid("88888888-8888-8888-8888-888888888856"), "VincularChamado", true, null, null, "InventarioAtivos.VincularChamado", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "InventarioAtivos" }
                });

            migrationBuilder.InsertData(
                table: "tipos_ativo_inventario",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "nome" },
                values: new object[,]
                {
                    { new Guid("58585858-5858-5858-5858-585858585801"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Computador portatil corporativo.", "Notebook" },
                    { new Guid("58585858-5858-5858-5858-585858585802"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Computador de mesa corporativo.", "Desktop" },
                    { new Guid("58585858-5858-5858-5858-585858585803"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Impressora ou multifuncional.", "Impressora" },
                    { new Guid("58585858-5858-5858-5858-585858585804"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Monitor corporativo.", "Monitor" },
                    { new Guid("58585858-5858-5858-5858-585858585805"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Servidor fisico ou appliance.", "Servidor" },
                    { new Guid("58585858-5858-5858-5858-585858585806"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Equipamento de comutacao de rede.", "Switch" },
                    { new Guid("58585858-5858-5858-5858-585858585807"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Equipamento de roteamento de rede.", "Roteador" },
                    { new Guid("58585858-5858-5858-5858-585858585808"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Celular corporativo.", "Celular" },
                    { new Guid("58585858-5858-5858-5858-585858585809"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Tablet corporativo.", "Tablet" },
                    { new Guid("58585858-5858-5858-5858-585858585810"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Equipamento de energia ininterrupta.", "Nobreak" },
                    { new Guid("58585858-5858-5858-5858-585858585811"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Perifericos diversos.", "Periferico" },
                    { new Guid("58585858-5858-5858-5858-585858585812"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Demais tipos de ativo inventariavel.", "Outro" }
                });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888852"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999052") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888853"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999053") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888854"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999054") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888855"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999055") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888856"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999056") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888852"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999079") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888855"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999080") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888856"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999081") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventario_ativos_ativo",
                table: "inventario_ativos",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "IX_inventario_ativos_atualizado_por_usuario_id",
                table: "inventario_ativos",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventario_ativos_criado_por_usuario_id",
                table: "inventario_ativos",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventario_ativos_departamento_id",
                table: "inventario_ativos",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventario_ativos_inativado_por_usuario_id",
                table: "inventario_ativos",
                column: "inativado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventario_ativos_local_unidade_id",
                table: "inventario_ativos",
                column: "local_unidade_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventario_ativos_status_operacional",
                table: "inventario_ativos",
                column: "status_operacional");

            migrationBuilder.CreateIndex(
                name: "IX_inventario_ativos_tipo_ativo_inventario_id",
                table: "inventario_ativos",
                column: "tipo_ativo_inventario_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventario_ativos_usuario_responsavel_id",
                table: "inventario_ativos",
                column: "usuario_responsavel_id");

            migrationBuilder.CreateIndex(
                name: "ux_inventario_ativos_codigo",
                table: "inventario_ativos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_inventario_ativos_numero_patrimonio",
                table: "inventario_ativos",
                column: "numero_patrimonio",
                unique: true,
                filter: "numero_patrimonio IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_inventario_ativos_numero_serie",
                table: "inventario_ativos",
                column: "numero_serie",
                unique: true,
                filter: "numero_serie IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ux_tipos_ativo_inventario_nome",
                table: "tipos_ativo_inventario",
                column: "nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventario_ativos");

            migrationBuilder.DropTable(
                name: "tipos_ativo_inventario");

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888852") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888853") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888854") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888855") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888856") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888852") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888855") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888856") });

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888852"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888853"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888854"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888855"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888856"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999052"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999053"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999054"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999055"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999056"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999057"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999058"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999059"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999060"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999061"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999062"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999063"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999064"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999065"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999066"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999067"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999068"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999069"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888836") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999070"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888843") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999071"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888847") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999072"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888848") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999073"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999074"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999075"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999076"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999077"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999078"));
        }
    }
}
