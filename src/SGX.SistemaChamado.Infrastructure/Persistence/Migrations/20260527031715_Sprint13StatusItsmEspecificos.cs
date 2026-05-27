using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint13StatusItsmEspecificos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "eh_status_final",
                value: true);

            migrationBuilder.InsertData(
                table: "status_chamado",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "codigo", "criado_em", "criado_por", "descricao", "eh_status_final", "nome", "pausa_sla" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444446"), true, null, null, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado cancelado.", true, "Cancelado", false },
                    { new Guid("44444444-4444-4444-4444-444444444447"), true, null, null, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado em analise tecnica.", false, "Em Analise", false },
                    { new Guid("44444444-4444-4444-4444-444444444448"), true, null, null, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Chamado aguardando aprovacao para avancar.", false, "Aguardando Aprovacao", true },
                    { new Guid("44444444-4444-4444-4444-444444444449"), true, null, null, 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Mudanca aprovada para execucao.", false, "Aprovada", false },
                    { new Guid("44444444-4444-4444-4444-444444444450"), true, null, null, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Mudanca reprovada.", true, "Reprovada", false },
                    { new Guid("44444444-4444-4444-4444-444444444451"), true, null, null, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Atividade em execucao.", false, "Em Execucao", false },
                    { new Guid("44444444-4444-4444-4444-444444444452"), true, null, null, 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Atividade concluida.", true, "Concluida", false },
                    { new Guid("44444444-4444-4444-4444-444444444453"), true, null, null, 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Causa raiz do problema identificada.", false, "Causa Raiz Identificada", false },
                    { new Guid("44444444-4444-4444-4444-444444444454"), true, null, null, 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Solucao de contorno registrada.", false, "Solucao de Contorno", false },
                    { new Guid("44444444-4444-4444-4444-444444444455"), true, null, null, 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Evento correlacionado.", false, "Correlacionado", false },
                    { new Guid("44444444-4444-4444-4444-444444444456"), true, null, null, 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Evento tratado.", true, "Tratado", false },
                    { new Guid("44444444-4444-4444-4444-444444444457"), true, null, null, 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Tarefa operacional planejada.", false, "Planejada", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444446"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444447"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444448"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444449"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444450"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444451"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444452"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444453"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444454"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444455"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444456"));

            migrationBuilder.DeleteData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444457"));

            migrationBuilder.UpdateData(
                table: "status_chamado",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "eh_status_final",
                value: false);
        }
    }
}
