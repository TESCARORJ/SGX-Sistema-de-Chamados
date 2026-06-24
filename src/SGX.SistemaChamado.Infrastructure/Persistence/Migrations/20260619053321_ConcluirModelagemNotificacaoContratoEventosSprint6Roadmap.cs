using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirModelagemNotificacaoContratoEventosSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao" },
                values: new object[] { "Criar mapeamento EF, unicidade de idempotencia e base persistente sem acoplar envio, templates e preferencias antes da hora.", "Diagnostico consolidado em docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md; entidade de dominio Notificacao; contrato EventoCandidatoNotificacao; testes de dominio/contrato; documentacao em docs/roadmap/sprint-6-modelagem-notificacao-contrato-eventos.md; sem persistencia estrutural nesta etapa.", "Criar configuracao EF e migration estrutural da notificacao, preservando separacao entre dominio, persistencia, processamento, envio, templates, preferencias e resolucao de destinatarios.", 19, "Criar configuracao EF e migration estrutural de notificacoes.", "Nucleo de dominio da notificacao e contrato interno de evento modelados, ainda sem persistencia estrutural no EF.", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao" },
                values: new object[] { "Definir modelo de notificacao sem acoplamento excessivo com canais externos.", "Diagnostico consolidado em docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md, com analise de CanalNotificacao, HistoricoChamado, EventoAuditoria, EventoSla, LogIntegracaoEmail, Worker.Email, permissoes e estruturas frontend locais.", "Consolidar a modelagem estrutural do modulo sem misturar historico, auditoria, eventos de origem, inbound de e-mail e notificacao persistente futura.", 13, "Modelar entidade Notificacao e contrato de eventos.", "Notificacoes ainda nao estao consolidadas como modulo persistente por evento ITSM.", 1 });
        }
    }
}
