using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirRegraSolucaoTecnicaObrigatoriaSprint5Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Aceite do solicitante, rejeição da solução, prazo de auto-fechamento, motivo de cancelamento, política formal de reabertura, auditoria do ciclo resolvido/fechado/reaberto e integração segura com bloqueios de aprovação pendente.", 22, "Criar regra para exigir motivo ao cancelar chamado." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Aceite do solicitante, rejeição da solução, prazo de auto-fechamento, motivo de cancelamento, campo solução obrigatório, política formal de reabertura, auditoria do ciclo resolvido/fechado/reaberto e integração segura com bloqueios de aprovação pendente.", 19, "Criar regra para exigir solucao tecnica ao resolver chamado." });
        }
    }
}
