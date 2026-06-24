using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarChecklistDetalhadoSprint6Notificacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Planejar escopo e critérios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 1, "Diagnosticar estruturas existentes de notificações e eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, "Modelar entidade Notificacao e contrato de eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, "Criar configuração EF e migration estrutural de notificações" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000901"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 5, new Guid("77777777-7777-7777-7777-777777777725"), "Testar domínio e estrutura persistente de notificações" },
                    { new Guid("78787878-7878-7878-7878-000000000902"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777725"), "Criar serviço de geração idempotente de notificações" },
                    { new Guid("78787878-7878-7878-7878-000000000903"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar resolução de destinatários por participação e perfil" },
                    { new Guid("78787878-7878-7878-7878-000000000904"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777725"), "Modelar templates e materialização de conteúdo" },
                    { new Guid("78787878-7878-7878-7878-000000000905"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 4, true, 9, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar preferências de notificação por usuário e evento" },
                    { new Guid("78787878-7878-7878-7878-000000000906"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar processamento e controle de tentativas de entrega" },
                    { new Guid("78787878-7878-7878-7878-000000000907"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal Sistema" },
                    { new Guid("78787878-7878-7878-7878-000000000908"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal E-mail" },
                    { new Guid("78787878-7878-7878-7878-000000000909"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 13, new Guid("77777777-7777-7777-7777-777777777725"), "Criar API de consulta, leitura e marcação como não lida" },
                    { new Guid("78787878-7878-7878-7878-000000000910"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 4, true, 14, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar central de notificações no frontend" },
                    { new Guid("78787878-7878-7878-7878-000000000911"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 15, new Guid("77777777-7777-7777-7777-777777777725"), "Integrar notificações aos eventos ITSM priorizados e executar testes de regressão" },
                    { new Guid("78787878-7878-7878-7878-000000000912"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 5, true, 16, new Guid("77777777-7777-7777-7777-777777777725"), "Documentar, homologar e registrar aceite da Sprint 6" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                column: "proxima_acao",
                value: "Modelar entidade Notificacao e contrato de eventos.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000901"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000902"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000903"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000904"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000905"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000906"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000907"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000908"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000909"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000910"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000911"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000912"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { null, null, "Planejar escopo e criterios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { null, null, 2, "Implementar entregas centrais da sprint" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { null, null, 3, "Executar testes funcionais e tecnicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { null, null, 5, "Registrar homologacao e aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                column: "proxima_acao",
                value: "Modelar entidade Notificacao e pipeline de eventos.");
        }
    }
}
