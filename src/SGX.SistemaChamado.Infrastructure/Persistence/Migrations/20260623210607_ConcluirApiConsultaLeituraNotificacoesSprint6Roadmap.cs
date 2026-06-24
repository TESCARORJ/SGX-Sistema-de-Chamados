using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirApiConsultaLeituraNotificacoesSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000909"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao com LidaEm, API autenticada de caixa propria, ownership, paginacao, contagem de nao lidas e marcacao lida/nao lida com testes; sem frontend e sem alterar Worker.Email.", "Implementar central de notificacoes no frontend e depois integrar eventos ITSM priorizados sem misturar inbox, transporte e processamento.", 81, "Implementar central de notificacoes no frontend", "Notificacoes internas persistidas com API autenticada para consulta propria, contagem de nao lidas e marcacao de leitura sem alterar entrega, transporte ou processamento." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000909"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao persistida com geracao, destinatarios, templates, preferencias, processamento, entrega Sistema e transporte outbound de Email com sucesso/falha, idempotencia e testes; sem alterar inbound.", "Criar API de consulta, leitura e marcacao como nao lida, depois frontend e integracao dos eventos ITSM sem misturar transporte outbound, inbox e eventos.", 75, "Criar API de consulta, leitura e marcacao como nao lida", "Processamento de notificacoes persistidas implementado com selecao, inicio seguro, controle de tentativas, backoff, reagendamento e encerramento de falha, ainda sem transporte real por canal." });
        }
    }
}
