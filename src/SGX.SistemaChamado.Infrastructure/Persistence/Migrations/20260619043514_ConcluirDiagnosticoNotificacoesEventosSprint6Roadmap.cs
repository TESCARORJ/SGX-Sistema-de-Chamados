using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirDiagnosticoNotificacoesEventosSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao" },
                values: new object[] { "Diagnostico consolidado em docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md, com analise de CanalNotificacao, HistoricoChamado, EventoAuditoria, EventoSla, LogIntegracaoEmail, Worker.Email, permissoes e estruturas frontend locais.", "Validar recebimento por perfil, observador, aprovador e grupo tecnico apos existir modulo persistente e integracao real aos eventos ITSM.", "Consolidar a modelagem estrutural do modulo sem misturar historico, auditoria, eventos de origem, inbound de e-mail e notificacao persistente futura.", 13 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao" },
                values: new object[] { "Escopo sprint definido.", "Validar recebimento por perfil, observador, aprovador e grupo tecnico.", "Tabela de notificacoes, API leitura/nao lida, preferencias e regras por evento.", 6 });
        }
    }
}
