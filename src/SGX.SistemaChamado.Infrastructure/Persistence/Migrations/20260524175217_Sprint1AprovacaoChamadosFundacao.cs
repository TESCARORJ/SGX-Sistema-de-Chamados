using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1AprovacaoChamadosFundacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aprovacoes_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tipo_origem = table.Column<int>(type: "integer", nullable: false),
                    origem_descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    solicitante_id = table.Column<Guid>(type: "uuid", nullable: true),
                    aprovador_id = table.Column<Guid>(type: "uuid", nullable: true),
                    justificativa_solicitacao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    justificativa_decisao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    solicitada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    decidida_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_aprovacoes_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_aprovacoes_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_aprovacoes_chamado_usuarios_aprovador_id",
                        column: x => x.aprovador_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aprovacoes_chamado_usuarios_atualizado_por_usuario_id",
                        column: x => x.atualizado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aprovacoes_chamado_usuarios_criado_por_usuario_id",
                        column: x => x.criado_por_usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aprovacoes_chamado_usuarios_solicitante_id",
                        column: x => x.solicitante_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888801") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999062"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999063"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888803") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999064"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999065"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999066"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888807") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999067"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888809") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999068"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888810") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999069"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888811") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999070"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888812") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999071"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888813") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999072"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888814") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999073"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888816") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999074"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888824") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999075"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999076"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888828") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999077"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888829") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999078"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888831") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999079"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888836") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999080"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888843") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999081"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888847") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999082"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888848") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999083"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888852") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999084"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888855") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999085"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888856") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999086"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888802") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999087"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888804") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999088"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888805") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999089"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888806") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999090"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("88888888-8888-8888-8888-888888888826") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999091"));

            migrationBuilder.InsertData(
                table: "permissoes_sistema",
                columns: new[] { "id", "acao", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "modulo" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888857"), "Visualizar", true, null, null, "AprovacaoChamados.Visualizar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "AprovacaoChamados" },
                    { new Guid("88888888-8888-8888-8888-888888888858"), "Gerenciar", true, null, null, "AprovacaoChamados.Gerenciar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "AprovacaoChamados" },
                    { new Guid("88888888-8888-8888-8888-888888888859"), "Aprovar", true, null, null, "AprovacaoChamados.Aprovar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "AprovacaoChamados" },
                    { new Guid("88888888-8888-8888-8888-888888888860"), "Reprovar", true, null, null, "AprovacaoChamados.Reprovar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "AprovacaoChamados" },
                    { new Guid("88888888-8888-8888-8888-888888888861"), "Cancelar", true, null, null, "AprovacaoChamados.Cancelar", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, "AprovacaoChamados" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Manter governanca do fluxo de aprovacao sem relaxar validacoes de backend, trilha de historico e auditoria, e controle de acesso por permissao nos endpoints administrativos e de portal.", "Atendimento", "A tela do roadmap deve exibir um unico item de Aprovacao de chamados com categoria Atendimento, status de implementacao Implementado funcionalmente, status tecnico Homologacao funcional preparada e percentual 90.", 4, "- docs/APROVACAO-CHAMADOS.md\n- docs/CHECKLIST-HOMOLOGACAO-APROVACAO-CHAMADOS.md\n- docs/evidencias/aprovacao-chamados/README.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md", "Sprints 1 a 6 concluidas com fechamento funcional e preparacao de homologacao.", "- Executar homologacao institucional com usuarios reais.\n- Coletar evidencias com prints reais e registrar aceite funcional.", "- Testes E2E completos quando houver framework institucional.\n- Evolucoes futuras: multiplos niveis de aprovacao, alcadas, delegacao, notificacoes avancadas e relatorios.", 90, "Executar homologacao institucional com usuarios reais e anexar evidencias formais da validacao funcional.", "Aprovacao de chamados implementada funcionalmente. O modulo contempla fundacao tecnica, backend administrativo, aprovacao manual, aprovacao automatica por Catalogo de Servicos, bloqueios operacionais para chamados pendentes ou reprovados, frontend administrativo, acompanhamento no portal do solicitante, historico do chamado, auditoria, permissoes, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras.", 2, 3, 4 });

            migrationBuilder.InsertData(
                table: "perfis_acesso_permissoes",
                columns: new[] { "perfil_acesso_id", "permissao_sistema_id", "criado_em", "criado_por", "id" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888857"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999057") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888858"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999058") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888859"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999059") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888860"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999060") },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888861"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", new Guid("99999999-9999-9999-9999-999999999061") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_aprovador_id",
                table: "aprovacoes_chamado",
                column: "aprovador_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_atualizado_por_usuario_id",
                table: "aprovacoes_chamado",
                column: "atualizado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_chamado_id",
                table: "aprovacoes_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_criado_por_usuario_id",
                table: "aprovacoes_chamado",
                column: "criado_por_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_solicitada_em",
                table: "aprovacoes_chamado",
                column: "solicitada_em");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_solicitante_id",
                table: "aprovacoes_chamado",
                column: "solicitante_id");

            migrationBuilder.CreateIndex(
                name: "ix_aprovacoes_chamado_status",
                table: "aprovacoes_chamado",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_aprovacoes_chamado_chamado_id_pendente_ativo",
                table: "aprovacoes_chamado",
                columns: new[] { "chamado_id", "ativo", "status" },
                unique: true,
                filter: "ativo = true AND status = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aprovacoes_chamado");

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888857") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888858") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888859") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888860") });

            migrationBuilder.DeleteData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("88888888-8888-8888-8888-888888888861") });

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888857"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888858"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888859"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888860"));

            migrationBuilder.DeleteData(
                table: "permissoes_sistema",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888861"));

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
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888852") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999079"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888855") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999080"));

            migrationBuilder.UpdateData(
                table: "perfis_acesso_permissoes",
                keyColumns: new[] { "perfil_acesso_id", "permissao_sistema_id" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("88888888-8888-8888-8888-888888888856") },
                column: "id",
                value: new Guid("99999999-9999-9999-9999-999999999081"));

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

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Tratar como melhoria futura se for exigencia", "Workflow", null, 2, null, null, null, null, 0, null, "Nao ha evidencia forte", 4, 0, 0 });
        }
    }
}
