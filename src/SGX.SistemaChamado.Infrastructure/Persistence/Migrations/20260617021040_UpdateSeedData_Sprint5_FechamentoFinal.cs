using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData_Sprint5_FechamentoFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000133"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000134"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000135"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000136"),
                column: "ativo",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, "Registrar fechamento tecnico e homologacao posterior da Sprint 5" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Homologacao formal permanece pendente e sera executada posteriormente com usuarios reais, sem misturar implementacao funcional da Sprint 6 neste fechamento.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Consolidado em docs/roadmap/sprint-5-regras-fechamento-aceite-reabertura.md, docs/roadmap/sprint-5-impacto-fluxo-atual-chamados.md, docs/roadmap/sprint-5-roteiro-homologacao.md e docs/roadmap/sprint-5-fechamento-tecnico-final.md.", "Sprint 5 encerrada tecnicamente; homologacao formal permanece pendente e foi apenas roteirizada nesta consolidacao final.", "Executar posteriormente a homologacao institucional/manual com solicitantes, atendentes e administradores reais, registrando evidencias e aceite formal.", "Nao ha nova pendencia funcional aberta da Sprint 5 neste fechamento tecnico; permanecem apenas homologacao formal posterior e evolucoes futuras fora do escopo desta sprint.", 100, "Executar homologacao formal da Sprint 5 e iniciar a analise da Sprint 6 - Notificacoes ITSM, sem antecipar implementacao funcional.", "Fluxo de resolucao, aceite, rejeicao, fechamento automatico e reabertura foi consolidado tecnicamente com checklist final e documentacao de encerramento da Sprint 5.", 1, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000133"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000134"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000135"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000136"),
                column: "ativo",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000831"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "titulo" },
                values: new object[] { null, null, false, "Registrar homologacao e aceite tecnico" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000832"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Separar resolvido de fechado e exigir dados obrigatorios de solucao/cancelamento.", null, "Base de encerramento/reabertura existente reaproveitada.", null, "Validar regras com solicitantes e atendentes reais.", "Aceite, prazo de auto-fechamento, motivo de cancelamento e campo solucao obrigatorio.", 50, "Evoluir estados e regras de negocio de ciclo de vida.", "Encerrar e reabrir existem, mas faltam aceite do solicitante e politicas formais.", 3, 2, 1 });
        }
    }
}
