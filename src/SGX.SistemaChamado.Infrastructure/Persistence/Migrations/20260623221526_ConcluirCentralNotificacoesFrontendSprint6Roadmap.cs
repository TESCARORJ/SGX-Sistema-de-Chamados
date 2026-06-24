using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirCentralNotificacoesFrontendSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000910"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Central frontend autenticada com service, types, contador de nao lidas, filtros, paginacao, detalhe, marcacao lida/nao lida, testes e responsividade; sem polling agressivo, sem tempo real e sem alterar Worker.Email.", "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao sem misturar inbox, transporte, processamento e frontend da caixa propria.", 88, "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao", "Notificacoes internas persistidas com API autenticada e central frontend responsiva para consulta propria, detalhe e marcacao lida/nao lida sem alterar entrega, transporte ou processamento." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000910"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao com LidaEm, API autenticada de caixa propria, ownership, paginacao, contagem de nao lidas e marcacao lida/nao lida com testes; sem frontend e sem alterar Worker.Email.", "Implementar central de notificacoes no frontend e depois integrar eventos ITSM priorizados sem misturar inbox, transporte e processamento.", 81, "Implementar central de notificacoes no frontend", "Notificacoes internas persistidas com API autenticada para consulta propria, contagem de nao lidas e marcacao de leitura sem alterar entrega, transporte ou processamento." });
        }
    }
}
