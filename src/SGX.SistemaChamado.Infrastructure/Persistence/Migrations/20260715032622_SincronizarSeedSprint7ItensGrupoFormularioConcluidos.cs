using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarSeedSprint7ItensGrupoFormularioConcluidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000005"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010010"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010013"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010014"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777717"),
                column: "proxima_acao",
                value: "Modelar dados operacionais especificos do incidente: servico afetado, CI afetado, causa provavel e solucao de contorno.");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Nenhum debito tecnico pendente; falta apenas homologacao formal.", "Grupo tecnico do catalogo, formulario dinamico por servico e persistencia/validacao das respostas implementados e cobertos por testes comportamentais (ver AbrirChamadoUseCaseTests).", null, 100, "Registrar homologacao funcional e visual e aceite formal da Sprint 7.", "Fluxo de requisicao via catalogo implementado, incluindo grupo responsavel, formulario dinamico por servico e persistencia das respostas.", 3, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000005"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010010"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010013"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000010014"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777717"),
                column: "proxima_acao",
                value: "Modelar os dados operacionais especificos do incidente, incluindo servico afetado, CI afetado, causa provavel e solucao de contorno, sem quebrar o fluxo atual.");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Orquestrar formulario por servico, aprovacao e atendimento sem duplicar regras.", "Capacidades base existentes de catalogo e aprovacao aproveitadas.", "Fluxo de aprovacao por servico, status proprios, servicos relacionados e conclusao com aceite.", 92, "Planejar modelagem estrutural futura para grupo responsavel por catalogo, formulario por servico e persistencia das respostas.", "Catalogo e aprovacao existem, porem sem fluxo separado de requisicao.", 2, 1 });
        }
    }
}
