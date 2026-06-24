using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirProcessamentoControleTentativasEntregaSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000906"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao persistida com geracao idempotente, destinatarios, templates, preferencias e ciclo de processamento com inicio seguro, tentativas, backoff, reagendamento, falha/sucesso e testes PostgreSQL; sem envio real.", "Implementar entrega pelo canal Sistema, depois entrega por E-mail e API de consulta sem misturar preferencia, geracao, processamento e envio.", 63, "Implementar entrega pelo canal Sistema", "Processamento de notificacoes persistidas implementado com selecao, inicio seguro, controle de tentativas, backoff, reagendamento e encerramento de falha, ainda sem transporte real por canal." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000906"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao persistida, geracao idempotente, resolucao de destinatarios, templates com materializacao segura e PreferenciaNotificacaoUsuario persistente com definicao, avaliacao, fallback permissivo, elegibilidade e testes de dominio/aplicacao/persistencia; sem envio funcional.", "Implementar processamento e controle de tentativas de entrega, depois entrega por canal e API de consulta sem misturar preferencia, geracao, processamento e envio.", 56, "Implementar processamento e controle de tentativas de entrega", "Preferencias de notificacao por usuario, evento e canal implementadas com avaliacao, fallback permissivo, elegibilidade, definicao explicita e testes de dominio/aplicacao/persistencia, ainda sem entrega por canal." });
        }
    }
}
