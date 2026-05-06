using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departamentos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    sigla = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departamentos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parametros_sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chave = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    valor = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sensivel = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametros_sistema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perfis_acesso",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tipo_perfil = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfis_acesso", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prioridades_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    prazo_primeira_resposta_horas = table.Column<int>(type: "integer", nullable: false),
                    prazo_resolucao_horas = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prioridades_chamado", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    codigo = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    eh_status_final = table.Column<bool>(type: "boolean", nullable: false),
                    pausa_sla = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_chamado", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    departamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_categorias_chamado_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    login = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    ultimo_acesso_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    departamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sla_configuracoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    departamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prioridade_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prazo_primeira_resposta_horas = table.Column<int>(type: "integer", nullable: false),
                    prazo_resolucao_horas = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sla_configuracoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_sla_configuracoes_categorias_chamado_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sla_configuracoes_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sla_configuracoes_prioridades_chamado_prioridade_id",
                        column: x => x.prioridade_id,
                        principalTable: "prioridades_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chamados",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    titulo = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    descricao = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    solicitante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    responsavel_id = table.Column<Guid>(type: "uuid", nullable: true),
                    departamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prioridade_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origem = table.Column<int>(type: "integer", nullable: false),
                    aberto_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    encerrado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamados", x => x.id);
                    table.ForeignKey(
                        name: "FK_chamados_categorias_chamado_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_departamentos_departamento_id",
                        column: x => x.departamento_id,
                        principalTable: "departamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_prioridades_chamado_prioridade_id",
                        column: x => x.prioridade_id,
                        principalTable: "prioridades_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_status_chamado_status_id",
                        column: x => x.status_id,
                        principalTable: "status_chamado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_usuarios_responsavel_id",
                        column: x => x.responsavel_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_usuarios_solicitante_id",
                        column: x => x.solicitante_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios_perfis_acesso",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    perfil_acesso_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_perfis_acesso", x => new { x.usuario_id, x.perfil_acesso_id });
                    table.ForeignKey(
                        name: "FK_usuarios_perfis_acesso_perfis_acesso_perfil_acesso_id",
                        column: x => x.perfil_acesso_id,
                        principalTable: "perfis_acesso",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuarios_perfis_acesso_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "anexos_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_arquivo = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    nome_arquivo_armazenado = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tamanho_bytes = table.Column<long>(type: "bigint", nullable: false),
                    caminho = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexos_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_anexos_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_anexos_chamado_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comentarios_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mensagem = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    interno = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_comentarios_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comentarios_chamado_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historicos_chamado",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historicos_chamado", x => x.id);
                    table.ForeignKey(
                        name: "FK_historicos_chamado_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_historicos_chamado_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sla_controles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chamado_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prazo_primeira_resposta_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    primeira_resposta_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    prazo_resolucao_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolvido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    esta_vencido = table.Column<bool>(type: "boolean", nullable: false),
                    esta_pausado = table.Column<bool>(type: "boolean", nullable: false),
                    pausado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_minutos_pausado = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sla_controles", x => x.id);
                    table.ForeignKey(
                        name: "FK_sla_controles_chamados_chamado_id",
                        column: x => x.chamado_id,
                        principalTable: "chamados",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "perfis_acesso",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "nome", "tipo_perfil" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Perfil com acesso total ao SGX Sistema de Chamados.", "Administrador", 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Perfil responsavel por atendimento e resolucao dos chamados.", "Atendente", 2 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Perfil de abertura e acompanhamento de chamados.", "Solicitante", 3 }
                });

            migrationBuilder.InsertData(
                table: "prioridades_chamado",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "nivel", "nome", "prazo_primeira_resposta_horas", "prazo_resolucao_horas" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Impacto baixo.", 1, "Baixa", 8, 48 },
                    { new Guid("55555555-5555-5555-5555-555555555552"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Impacto moderado.", 2, "Media", 4, 24 },
                    { new Guid("55555555-5555-5555-5555-555555555553"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Impacto alto.", 3, "Alta", 2, 8 },
                    { new Guid("55555555-5555-5555-5555-555555555554"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Impacto critico.", 4, "Critica", 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "status_chamado",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "eh_status_final", "nome", "pausa_sla" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), true, null, null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado aberto e aguardando atendimento.", false, "Aberto", false },
                    { new Guid("44444444-4444-4444-4444-444444444442"), true, null, null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado em atendimento pela equipe.", false, "Em Atendimento", false },
                    { new Guid("44444444-4444-4444-4444-444444444443"), true, null, null, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado aguardando retorno do solicitante.", false, "Aguardando Solicitante", true },
                    { new Guid("44444444-4444-4444-4444-444444444444"), true, null, null, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado resolvido e aguardando encerramento.", false, "Resolvido", false },
                    { new Guid("44444444-4444-4444-4444-444444444445"), true, null, null, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado encerrado.", true, "Encerrado", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_anexos_chamado_chamado_id",
                table: "anexos_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_anexos_chamado_usuario_id",
                table: "anexos_chamado",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_chamado_departamento_id",
                table: "categorias_chamado",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_categoria_id",
                table: "chamados",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_departamento_id",
                table: "chamados",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_prioridade_id",
                table: "chamados",
                column: "prioridade_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_responsavel_id",
                table: "chamados",
                column: "responsavel_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_solicitante_id",
                table: "chamados",
                column: "solicitante_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_status_id",
                table: "chamados",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ux_chamados_codigo",
                table: "chamados",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_chamado_chamado_id",
                table: "comentarios_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_chamado_usuario_id",
                table: "comentarios_chamado",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_departamentos_sigla",
                table: "departamentos",
                column: "sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_historicos_chamado_chamado_id",
                table: "historicos_chamado",
                column: "chamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_historicos_chamado_usuario_id",
                table: "historicos_chamado",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ux_parametros_sistema_chave",
                table: "parametros_sistema",
                column: "chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_perfis_acesso_nome",
                table: "perfis_acesso",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_prioridades_chamado_nivel",
                table: "prioridades_chamado",
                column: "nivel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sla_configuracoes_categoria_id",
                table: "sla_configuracoes",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_sla_configuracoes_departamento_id",
                table: "sla_configuracoes",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_sla_configuracoes_prioridade_id",
                table: "sla_configuracoes",
                column: "prioridade_id");

            migrationBuilder.CreateIndex(
                name: "ux_sla_controles_chamado_id",
                table: "sla_controles",
                column: "chamado_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_status_chamado_codigo",
                table: "status_chamado",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_departamento_id",
                table: "usuarios",
                column: "departamento_id");

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_login",
                table: "usuarios",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_perfis_acesso_perfil_acesso_id",
                table: "usuarios_perfis_acesso",
                column: "perfil_acesso_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "anexos_chamado");

            migrationBuilder.DropTable(
                name: "comentarios_chamado");

            migrationBuilder.DropTable(
                name: "historicos_chamado");

            migrationBuilder.DropTable(
                name: "parametros_sistema");

            migrationBuilder.DropTable(
                name: "sla_configuracoes");

            migrationBuilder.DropTable(
                name: "sla_controles");

            migrationBuilder.DropTable(
                name: "usuarios_perfis_acesso");

            migrationBuilder.DropTable(
                name: "chamados");

            migrationBuilder.DropTable(
                name: "perfis_acesso");

            migrationBuilder.DropTable(
                name: "categorias_chamado");

            migrationBuilder.DropTable(
                name: "prioridades_chamado");

            migrationBuilder.DropTable(
                name: "status_chamado");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "departamentos");
        }
    }
}
