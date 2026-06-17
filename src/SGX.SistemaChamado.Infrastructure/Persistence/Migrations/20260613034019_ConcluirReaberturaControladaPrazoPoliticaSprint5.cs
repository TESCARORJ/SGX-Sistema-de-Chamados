using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirReaberturaControladaPrazoPoliticaSprint5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "parametros_sistema",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "chave", "criado_em", "criado_por", "descricao", "sensivel", "valor" },
                values: new object[] { new Guid("57575757-5757-5757-5757-575757575702"), true, null, null, "chamados.reabertura.prazo_maximo_horas", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Prazo maximo em horas para reabertura controlada de chamados encerrados.", false, "168" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 44, "Registrar auditoria de resolucao, aceite, rejeicao, fechamento e reabertura." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "parametros_sistema",
                keyColumn: "id",
                keyValue: new Guid("57575757-5757-5757-5757-575757575702"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "percentual_implementacao", "proxima_acao" },
                values: new object[] { 41, "Criar regra de reabertura controlada por prazo/politica." });
        }
    }
}
