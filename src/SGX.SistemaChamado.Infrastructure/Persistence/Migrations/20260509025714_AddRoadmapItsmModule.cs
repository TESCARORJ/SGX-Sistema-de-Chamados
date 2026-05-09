using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadmapItsmModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roadmap_itsm_itens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    area = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    categoria = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    situacao_atual = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    atencao_tecnica = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    impacto = table.Column<int>(type: "integer", nullable: false),
                    decisao = table.Column<int>(type: "integer", nullable: false),
                    observacao = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    responsavel = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    prazo_alvo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roadmap_itsm_itens", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "roadmap_itsm_itens",
                columns: new[] { "id", "area", "atencao_tecnica", "ativo", "atualizado_em", "atualizado_por", "categoria", "criado_em", "criado_por", "decisao", "impacto", "observacao", "ordem", "prazo_alvo", "prioridade", "responsavel", "situacao_atual", "status" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777701"), "Abertura de chamado pelo portal", "Demonstrar fluxo completo: abrir, anexar, acompanhar", true, null, null, "Portal", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 1, null, 1, null, "Prevista no portal /portal", 2 },
                    { new Guid("77777777-7777-7777-7777-777777777702"), "Abertura por e-mail", "Testar e mostrar correlacao por codigo, assunto e resposta", true, null, null, "Integracao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 2, null, 1, null, "Prevista via Worker IMAP", 2 },
                    { new Guid("77777777-7777-7777-7777-777777777703"), "Perfis de acesso", "Validar permissoes finas por tela e acao", true, null, null, "Seguranca", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 3, null, 1, null, "Administrador, Atendente e Solicitante", 3 },
                    { new Guid("77777777-7777-7777-7777-777777777704"), "Autenticacao corporativa", "Preparar explicacao clara: Azure autentica, SGX autoriza", true, null, null, "Seguranca", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 4, null, 1, null, "Entra ID/Azure AD previsto", 4 },
                    { new Guid("77777777-7777-7777-7777-777777777705"), "SLA", "Mostrar regra de prazo, pausa, resposta e encerramento", true, null, null, "Operacao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 5, null, 1, null, "Estrutura prevista com controle e configuracao", 3 },
                    { new Guid("77777777-7777-7777-7777-777777777706"), "Historico/auditoria", "Garantir que mudancas relevantes sejam registradas", true, null, null, "Governanca", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 6, null, 1, null, "Previsto com historico do chamado", 3 },
                    { new Guid("77777777-7777-7777-7777-777777777707"), "Comentarios e anexos", "Testar upload, download, visibilidade publica/interna", true, null, null, "Operacao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 7, null, 1, null, "Previsto", 2 },
                    { new Guid("77777777-7777-7777-7777-777777777708"), "Cadastros administrativos", "Verificar se permitem inativacao e parametrizacao", true, null, null, "Administracao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 2, null, 8, null, 2, null, "Categorias, prioridades, status e departamentos", 2 },
                    { new Guid("77777777-7777-7777-7777-777777777709"), "Dashboard", "Levar indicadores simples: abertos, vencidos, por status e por atendente", true, null, null, "Gestao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 9, null, 2, null, "Previsto", 3 },
                    { new Guid("77777777-7777-7777-7777-777777777710"), "Base de conhecimento", "Pode ser GAP assumido para evolucao", true, null, null, "Conhecimento", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, 2, null, 10, null, 2, null, "Nao ha evidencia forte", 4 },
                    { new Guid("77777777-7777-7777-7777-777777777711"), "Inventario/ativos", "Nao prometer equivalencia com GLPI nesse ponto", true, null, null, "Ativos", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, 2, null, 11, null, 3, null, "Nao ha evidencia forte", 4 },
                    { new Guid("77777777-7777-7777-7777-777777777712"), "Catalogo de servicos", "Pode precisar virar recurso mais formal", true, null, null, "Catalogo", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, 1, null, 12, null, 2, null, "Parcial, via categorias/departamentos", 3 },
                    { new Guid("77777777-7777-7777-7777-777777777713"), "Aprovacao de chamados", "Tratar como melhoria futura se for exigencia", true, null, null, "Workflow", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, 2, null, 13, null, 2, null, "Nao ha evidencia forte", 4 },
                    { new Guid("77777777-7777-7777-7777-777777777714"), "Notificacoes", "Validar e/ou planejar e-mail/notificacao por evento", true, null, null, "Comunicacao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, 1, null, 14, null, 1, null, "Nao ficou suficientemente evidente", 4 },
                    { new Guid("77777777-7777-7777-7777-777777777715"), "Relatorios avancados", "Planejar exportacao/filtros gerenciais", true, null, null, "Gestao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, 2, null, 15, null, 2, null, "Nao ficou suficientemente evidente", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_roadmap_itsm_itens_ordem_categoria",
                table: "roadmap_itsm_itens",
                columns: new[] { "ordem", "categoria" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roadmap_itsm_itens");
        }
    }
}
