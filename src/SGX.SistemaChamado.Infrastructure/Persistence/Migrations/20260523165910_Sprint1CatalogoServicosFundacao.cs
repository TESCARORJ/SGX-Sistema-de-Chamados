using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1CatalogoServicosFundacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "catalogo_servicos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    slug = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    instrucoes_solicitante = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    departamento_responsavel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prioridade_padrao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sla_padrao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    artigo_base_conhecimento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    visibilidade = table.Column<int>(type: "integer", nullable: false),
                    permite_abertura_chamado = table.Column<bool>(type: "boolean", nullable: false),
                    requer_aprovacao = table.Column<bool>(type: "boolean", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_catalogo_servicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_base_conhecimento_artigos_artigo_base_con~",
                        column: x => x.artigo_base_conhecimento_id,
                        principalTable: "base_conhecimento_artigos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_categorias_chamado_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_departamentos_departamento_responsavel_id",
                        column: x => x.departamento_responsavel_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_prioridades_chamado_prioridade_padrao_id",
                        column: x => x.prioridade_padrao_id,
                        principalTable: "prioridades_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_sla_politicas_sla_padrao_id",
                        column: x => x.sla_padrao_id,
                        principalTable: "sla_politicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_subcategorias_chamado_subcategoria_id",
                        column: x => x.subcategoria_id,
                        principalTable: "subcategorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_usuarios_arquivado_por_usuario_id",
                        column: x => x.arquivado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogo_servicos_usuarios_publicado_por_usuario_id",
                        column: x => x.publicado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888848"), "Visualizar", true, null, null, "CatalogoServicos.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "CatalogoServicos" },
                    { new Guid("88888888-8888-8888-8888-888888888849"), "Gerenciar", true, null, null, "CatalogoServicos.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "CatalogoServicos" },
                    { new Guid("88888888-8888-8888-8888-888888888850"), "Publicar", true, null, null, "CatalogoServicos.Publicar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "CatalogoServicos" },
                    { new Guid("88888888-8888-8888-8888-888888888851"), "Arquivar", true, null, null, "CatalogoServicos.Arquivar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "CatalogoServicos" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "data_conclusao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Catalogo institucional e multiarea com departamento responsavel obrigatorio e relacionamentos opcionais para nao bloquear evolucao", "Conhecimento", "Fundacao tecnica do catalogo implementada com entidade, enums, mapeamento EF Core, migration, permissoes e documentacao inicial", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "docs/CATALOGO-SERVICOS.md; src/SGX.SistemaChamado.Domain/Entities/CatalogoServico.cs", "Validar fluxos com areas nao-TI apos entrega funcional das proximas sprints", "Implementar CRUD administrativo, publicacao/arquivamento operacional e regras de exposicao para solicitantes", 20, "Implementar sprint 2 com casos de uso e endpoints administrativos", "Fundacao tecnica implementada", 3, 4 });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888848"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999048") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888849"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999049") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888850"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999050") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888851"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999051") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888848"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999073") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_arquivado_por_usuario_id",
                table: "catalogo_servicos",
                column: "arquivado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_artigo_base_conhecimento_id",
                table: "catalogo_servicos",
                column: "artigo_base_conhecimento_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalogo_servicos_ativo",
                table: "catalogo_servicos",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_atualizado_por_usuario_id",
                table: "catalogo_servicos",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_categoria_id",
                table: "catalogo_servicos",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_criado_por_usuario_id",
                table: "catalogo_servicos",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalogo_servicos_departamento_responsavel_id",
                table: "catalogo_servicos",
                column: "departamento_responsavel_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_prioridade_padrao_id",
                table: "catalogo_servicos",
                column: "prioridade_padrao_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_publicado_por_usuario_id",
                table: "catalogo_servicos",
                column: "publicado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_sla_padrao_id",
                table: "catalogo_servicos",
                column: "sla_padrao_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalogo_servicos_status",
                table: "catalogo_servicos",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_catalogo_servicos_status_ativo",
                table: "catalogo_servicos",
                columns: new[] { "status", "ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_catalogo_servicos_subcategoria_id",
                table: "catalogo_servicos",
                column: "subcategoria_id");

            migrationBuilder.CreateIndex(
                name: "ux_catalogo_servicos_slug",
                table: "catalogo_servicos",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalogo_servicos");

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888848") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888849") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888850") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888851") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888848") });

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888848"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888849"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888850"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888851"));

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
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888843") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999067"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888847") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999068"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999069"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999070"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999071"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999072"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999073"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "data_conclusao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Pode precisar virar recurso mais formal", "Catalogo", null, null, null, null, null, 0, null, "Parcial, via categorias/departamentos", 0, 0 });
        }
    }
}
