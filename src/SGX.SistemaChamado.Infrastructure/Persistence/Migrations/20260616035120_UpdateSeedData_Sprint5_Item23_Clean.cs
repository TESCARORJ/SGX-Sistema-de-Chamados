using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData_Sprint5_Item23_Clean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "parametros_sistema",
                keyColumn: "id",
                keyValue: new Guid("57575757-5757-5757-5757-575757575701"));

            migrationBuilder.DeleteData(
                table: "parametros_sistema",
                keyColumn: "id",
                keyValue: new Guid("57575757-5757-5757-5757-575757575702"));

            migrationBuilder.InsertData(
                table: "parametros_sistema",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "chave", "criado_em", "criado_por", "descricao", "sensivel", "valor" },
                values: new object[,]
                {
                    { new Guid("e0000000-0000-0000-0000-000000000001"), true, null, null, "chamados.fechamento_automatico.prazo_aceite_horas", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Prazo em horas para fechamento automatico por falta de aceite", false, "48" },
                    { new Guid("e0000000-0000-0000-0000-000000000002"), true, null, null, "chamados.reabertura.prazo_maximo_horas", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Prazo maximo em horas para reabertura de chamado encerrado", false, "48" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676710"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676729"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676730"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676731"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676732"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676733"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676734"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676735"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686728"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686729"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686730"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686731"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686732"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686733"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686734"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686735"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686736"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686737"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686738"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686739"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686740"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696720"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696721"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696722"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696723"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696724"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696725"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696726"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696727"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000801"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000802"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000803"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000804"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000805"),
                column: "grupo",
                value: 5);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000808"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000811"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000812"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000813"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000815"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000816"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000817"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000818"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000819"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000820"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000821"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000822"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000823"),
                columns: new[] { "concluido", "grupo" },
                values: new object[] { true, 4 });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000824"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000825"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000826"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000827"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000828"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000829"),
                column: "grupo",
                value: 5);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000830"),
                column: "grupo",
                value: 9);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"),
                column: "grupo",
                value: 9);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar resolvido de fechado e exigir dados obrigatorios de solucao/cancelamento.", "Fluxo contempla resolucao, aceite/rejeicao, fechamento automatico e reabertura auditavel.", "Base de encerramento/reabertura existente reaproveitada.", "Criar governanca de encerramento com aceite, fechamento automatico e reabertura controlada.", "Validar regras com solicitantes e atendentes reais.", "Aceite, prazo de auto-fechamento, motivo de cancelamento e campo solucao obrigatorio.", 72, "Testar rejeicao da solucao e retorno ao atendimento", "Encerrar e reabrir existem, mas faltam aceite do solicitante e politicas formais." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "parametros_sistema",
                keyColumn: "id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "parametros_sistema",
                keyColumn: "id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000002"));

            migrationBuilder.InsertData(
                table: "parametros_sistema",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "chave", "criado_em", "criado_por", "descricao", "sensivel", "valor" },
                values: new object[,]
                {
                    { new Guid("57575757-5757-5757-5757-575757575701"), true, null, null, "chamados.fechamento_automatico.prazo_aceite_horas", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Prazo em horas para fechamento automatico de chamados resolvidos sem manifestacao do solicitante.", false, "72" },
                    { new Guid("57575757-5757-5757-5757-575757575702"), true, null, null, "chamados.reabertura.prazo_maximo_horas", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Prazo maximo em horas para reabertura controlada de chamados encerrados.", false, "168" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676710"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676729"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676730"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676731"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676732"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676733"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676734"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676735"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686728"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686729"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686730"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686731"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686732"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686733"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686734"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686735"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686736"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686737"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686738"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686739"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686740"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696720"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696721"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696722"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696723"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696724"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696725"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696726"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696727"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000801"),
                column: "grupo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000802"),
                column: "grupo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000803"),
                column: "grupo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000804"),
                column: "grupo",
                value: 1);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000805"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000806"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000807"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000808"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000809"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000810"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000811"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000812"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000813"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000814"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000815"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000816"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000817"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000818"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000819"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000820"),
                column: "grupo",
                value: 2);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000821"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000822"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000823"),
                columns: new[] { "concluido", "grupo" },
                values: new object[] { false, 3 });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000824"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000825"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000826"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000827"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000828"),
                column: "grupo",
                value: 3);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000829"),
                column: "grupo",
                value: 4);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000830"),
                column: "grupo",
                value: 5);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"),
                column: "grupo",
                value: 5);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"),
                column: "grupo",
                value: 9);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar conceitualmente e funcionalmente os estados Resolvido e Fechado. Não tratar resolução como encerramento definitivo. Exigir dados obrigatórios de solução para resolução e motivo obrigatório para cancelamento. Preservar compatibilidade com o fluxo legado, com SLA, com atendimento, com histórico, com permissões administrativas e com o motor de aprovações da Sprint 4. Nenhuma regra de fechamento deve ignorar aprovação pendente bloqueante.", "Fluxo contempla resolução, aceite/rejeição pelo solicitante, fechamento automático por prazo, cancelamento com motivo obrigatório, reabertura auditável e compatibilidade com SLA, histórico, permissões e aprovações pendentes bloqueantes.", "Base de encerramento/reabertura existente reaproveitada. Fluxos atuais de EncerrarChamadoUseCase e ReabrirChamadoUseCase devem ser preservados como base evolutiva.", "Criar governança de encerramento com aceite, fechamento automático e reabertura controlada.", "Validar regras com solicitantes, atendentes e administradores reais, incluindo resolução, aceite, rejeição, fechamento automático, cancelamento e reabertura controlada.", "Aceite do solicitante, rejeição da solução, prazo de auto-fechamento, motivo de cancelamento, política formal de reabertura, auditoria do ciclo resolvido/fechado/reaberto e integração segura com bloqueios de aprovação pendente.", 69, "Testar aceite e fechamento definitivo.", "Encerrar e reabrir chamados já existem no sistema, mas o fluxo ainda não possui governança completa de aceite do solicitante, prazo formal de auto-fechamento, rejeição de solução, campos obrigatórios de solução/cancelamento e políticas auditáveis de reabertura." });
        }
    }
}
