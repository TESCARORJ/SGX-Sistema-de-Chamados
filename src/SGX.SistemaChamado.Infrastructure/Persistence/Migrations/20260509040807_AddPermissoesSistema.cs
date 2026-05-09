using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissoesSistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissoes_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    modulo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    acao = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissoes_sistema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perfis_acesso_permissoes",
                columns: table => new
                {
                    perfil_acesso_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permissao_sistema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_acesso_permissoes", x => new { x.perfil_acesso_id, x.permissao_sistema_id });
                    table.ForeignKey(
                        name: "FK_perfis_acesso_permissoes_perfis_acesso_perfil_acesso_id",
                        column: x => x.perfil_acesso_id,
                        principalTable: "perfis_acesso",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_perfis_acesso_permissoes_permissoes_sistema_permissao_siste~",
                        column: x => x.permissao_sistema_id,
                        principalTable: "permissoes_sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888801"), "Visualizar", true, null, null, "Dashboard.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Dashboard" },
                    { new Guid("88888888-8888-8888-8888-888888888802"), "Visualizar", true, null, null, "Chamados.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888803"), "VisualizarTodos", true, null, null, "Chamados.VisualizarTodos", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888804"), "Abrir", true, null, null, "Chamados.Abrir", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888805"), "Comentar", true, null, null, "Chamados.Comentar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888806"), "Anexar", true, null, null, "Chamados.Anexar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888807"), "Assumir", true, null, null, "Chamados.Assumir", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888808"), "Atribuir", true, null, null, "Chamados.Atribuir", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888809"), "AlterarStatus", true, null, null, "Chamados.AlterarStatus", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888810"), "AlterarPrioridade", true, null, null, "Chamados.AlterarPrioridade", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888811"), "AlterarCategoria", true, null, null, "Chamados.AlterarCategoria", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888812"), "Encerrar", true, null, null, "Chamados.Encerrar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888813"), "Reabrir", true, null, null, "Chamados.Reabrir", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Chamados" },
                    { new Guid("88888888-8888-8888-8888-888888888814"), "Visualizar", true, null, null, "Cadastros.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Cadastros" },
                    { new Guid("88888888-8888-8888-8888-888888888815"), "Gerenciar", true, null, null, "Cadastros.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Cadastros" },
                    { new Guid("88888888-8888-8888-8888-888888888816"), "Visualizar", true, null, null, "Usuarios.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Usuarios" },
                    { new Guid("88888888-8888-8888-8888-888888888817"), "Gerenciar", true, null, null, "Usuarios.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Usuarios" },
                    { new Guid("88888888-8888-8888-8888-888888888818"), "AlterarPerfis", true, null, null, "Usuarios.AlterarPerfis", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Usuarios" },
                    { new Guid("88888888-8888-8888-8888-888888888819"), "Visualizar", true, null, null, "Perfis.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Perfis" },
                    { new Guid("88888888-8888-8888-8888-888888888820"), "Gerenciar", true, null, null, "Perfis.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Perfis" },
                    { new Guid("88888888-8888-8888-8888-888888888821"), "AlterarPermissoes", true, null, null, "Perfis.AlterarPermissoes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Perfis" },
                    { new Guid("88888888-8888-8888-8888-888888888822"), "Visualizar", true, null, null, "Parametros.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Parametros" },
                    { new Guid("88888888-8888-8888-8888-888888888823"), "Gerenciar", true, null, null, "Parametros.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Parametros" },
                    { new Guid("88888888-8888-8888-8888-888888888824"), "Visualizar", true, null, null, "IntegracoesEmail.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "IntegracoesEmail" },
                    { new Guid("88888888-8888-8888-8888-888888888825"), "Gerenciar", true, null, null, "IntegracoesEmail.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "IntegracoesEmail" },
                    { new Guid("88888888-8888-8888-8888-888888888826"), "Visualizar", true, null, null, "Notificacoes.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Notificacoes" },
                    { new Guid("88888888-8888-8888-8888-888888888827"), "Gerenciar", true, null, null, "Notificacoes.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Notificacoes" },
                    { new Guid("88888888-8888-8888-8888-888888888828"), "Visualizar", true, null, null, "Indicadores.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "Indicadores" }
                });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888801"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999001") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888802"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999002") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888803"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999003") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888804"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999004") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888805"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999005") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888806"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999006") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888807"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999007") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888808"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999008") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888809"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999009") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888810"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999010") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888811"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999011") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888812"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999012") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888813"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999013") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888814"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999014") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888815"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999015") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888816"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999016") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888817"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999017") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888818"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999018") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888819"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999019") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888820"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999020") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888821"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999021") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888822"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999022") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888823"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999023") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888824"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999024") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888825"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999025") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888826"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999026") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888827"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999027") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888828"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999028") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999029") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999030") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999031") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999032") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999033") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999034") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999035") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999036") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999037") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999038") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999039") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999040") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999041") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999042") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999043") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999044") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999045") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999046") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999047") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999048") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999049") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_perfis_acesso_permissoes_permissao_sistema_id",
                table: "perfis_acesso_permissoes",
                column: "permissao_sistema_id");

            migrationBuilder.CreateIndex(
                name: "ux_permissoes_sistema_codigo",
                table: "permissoes_sistema",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_permissoes_sistema_modulo_acao",
                table: "permissoes_sistema",
                columns: new[] { "modulo", "acao" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "perfis_acesso_permissoes");

            migrationBuilder.DropTable(
                name: "permissoes_sistema");
        }
    }
}
