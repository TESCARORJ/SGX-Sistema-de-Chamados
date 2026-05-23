using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1BaseConhecimentoFundacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "base_conhecimento_artigos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    slug = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    resumo = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    conteudo = table.Column<string>(type: "character varying(20000)", maxLength: 20000, nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    visibilidade = table.Column<int>(type: "integer", nullable: false),
                    tags = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    publicado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    publicado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    atualizado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    arquivado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    arquivado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_base_conhecimento_artigos", x => x.id);
                    table.ForeignKey(
                        name: "FK_base_conhecimento_artigos_categorias_chamado_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_base_conhecimento_artigos_usuarios_arquivado_por_usuario_id",
                        column: x => x.arquivado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_base_conhecimento_artigos_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_base_conhecimento_artigos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_base_conhecimento_artigos_usuarios_publicado_por_usuario_id",
                        column: x => x.publicado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chamados_artigos_conhecimento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    artigo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vinculado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vinculado_por_usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamados_artigos_conhecimento", x => x.id);
                    table.ForeignKey(
                        name: "FK_chamados_artigos_conhecimento_base_conhecimento_artigos_art~",
                        column: x => x.artigo_id,
                        principalTable: "base_conhecimento_artigos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_artigos_conhecimento_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_artigos_conhecimento_usuarios_vinculado_por_usuari~",
                        column: x => x.vinculado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999048"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999049"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999050"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999051"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999052"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999053"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999054"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999055"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999056"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999057"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999058"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999059"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999060"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999061"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999062"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999063"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999064"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999065"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888836") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999066"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999067"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999068"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999069"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999070"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999071"));

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888843"), "Visualizar", true, null, null, "BaseConhecimento.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "BaseConhecimento" },
                    { new Guid("88888888-8888-8888-8888-888888888844"), "Gerenciar", true, null, null, "BaseConhecimento.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "BaseConhecimento" },
                    { new Guid("88888888-8888-8888-8888-888888888845"), "Publicar", true, null, null, "BaseConhecimento.Publicar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "BaseConhecimento" },
                    { new Guid("88888888-8888-8888-8888-888888888846"), "Arquivar", true, null, null, "BaseConhecimento.Arquivar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "BaseConhecimento" },
                    { new Guid("88888888-8888-8888-8888-888888888847"), "VincularChamado", true, null, null, "BaseConhecimento.VincularChamado", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "BaseConhecimento" }
                });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888843"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999043") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888844"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999044") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888845"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999045") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888846"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999046") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888847"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999047") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_base_conhecimento_artigos_arquivado_por_usuario_id",
                table: "base_conhecimento_artigos",
                column: "arquivado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_base_conhecimento_artigos_atualizado_por_usuario_id",
                table: "base_conhecimento_artigos",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_conhecimento_artigos_categoria_id",
                table: "base_conhecimento_artigos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_base_conhecimento_artigos_criado_por_usuario_id",
                table: "base_conhecimento_artigos",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_conhecimento_artigos_publicado_em",
                table: "base_conhecimento_artigos",
                column: "publicado_em");

            migrationBuilder.CreateIndex(
                name: "IX_base_conhecimento_artigos_publicado_por_usuario_id",
                table: "base_conhecimento_artigos",
                column: "publicado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_conhecimento_artigos_status_ativo",
                table: "base_conhecimento_artigos",
                columns: new[] { "status", "ativo" });

            migrationBuilder.CreateIndex(
                name: "ux_base_conhecimento_artigos_slug",
                table: "base_conhecimento_artigos",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chamados_artigos_conhecimento_artigo_id",
                table: "chamados_artigos_conhecimento",
                column: "artigo_id");

            migrationBuilder.CreateIndex(
                name: "ix_chamados_artigos_conhecimento_chamado_id",
                table: "chamados_artigos_conhecimento",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_artigos_conhecimento_vinculado_por_usuario_id",
                table: "chamados_artigos_conhecimento",
                column: "vinculado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_chamados_artigos_conhecimento_chamado_artigo",
                table: "chamados_artigos_conhecimento",
                columns: new[] { "chamado_id", "artigo_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chamados_artigos_conhecimento");

            migrationBuilder.DropTable(
                name: "base_conhecimento_artigos");

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888843") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888844") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888845") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888846") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888847") });

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888843"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888844"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888845"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888846"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888847"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999043"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999044"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999045"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999046"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999047"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999048"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999049"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999050"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999051"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999052"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999053"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999054"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999055"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999056"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999057"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999058"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999059"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999060"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888836") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999061"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999062"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999063"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999064"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999065"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999066"));
        }
    }
}
