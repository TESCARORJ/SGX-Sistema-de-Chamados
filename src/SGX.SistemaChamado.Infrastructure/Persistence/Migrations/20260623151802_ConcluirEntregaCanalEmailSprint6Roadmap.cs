using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirEntregaCanalEmailSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000908"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Notificacao persistida com geracao, destinatarios, templates, preferencias, processamento, entrega Sistema e transporte outbound de Email com sucesso/falha, idempotencia e testes; sem alterar inbound.", "Criar API de consulta, leitura e marcacao como nao lida, depois frontend e integracao dos eventos ITSM sem misturar transporte outbound, inbox e eventos.", 75, "Criar API de consulta, leitura e marcacao como nao lida" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000908"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Notificacao persistida com geracao idempotente, destinatarios, templates, preferencias, processamento com tentativas e entrega pelo canal Sistema reutilizando a propria Notificacao, com idempotencia e testes PostgreSQL; sem e-mail.", "Implementar entrega pelo canal E-mail, API de consulta/leitura e integracao dos eventos ITSM sem misturar entrega interna, e-mail e frontend completo.", 69, "Implementar entrega pelo canal E-mail" });
        }
    }
}
