using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaCorporateCalendarSprint4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "calendario_corporativo_id",
                table: "sla_politicas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "calendario_corporativo_id",
                table: "chamado_slas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "usar_horario_comercial",
                table: "chamado_slas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "calendarios_corporativos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    padrao = table.Column<bool>(type: "boolean", nullable: false),
                    time_zone = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendarios_corporativos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "excecoes_calendario_corporativo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    calendario_corporativo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    hora_fim = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_excecoes_calendario_corporativo", x => x.id);
                    table.ForeignKey(
                        name: "FK_excecoes_calendario_corporativo_calendarios_corporativos_ca~",
                        column: x => x.calendario_corporativo_id,
                        principalTable: "calendarios_corporativos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horarios_atendimento_calendario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    calendario_corporativo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dia_semana = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hora_fim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios_atendimento_calendario", x => x.id);
                    table.ForeignKey(
                        name: "FK_horarios_atendimento_calendario_calendarios_corporativos_ca~",
                        column: x => x.calendario_corporativo_id,
                        principalTable: "calendarios_corporativos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "calendarios_corporativos",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "criado_em", "criado_por", "descricao", "nome", "padrao", "time_zone" },
                values: new object[] { new Guid("56565656-5656-5656-5656-565656565701"), true, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Calendário inicial para cálculo de SLA em horário comercial.", "Calendário Corporativo Padrão", true, "America/Sao_Paulo" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("70707070-7070-7070-7070-707070707752"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 52, new Guid("77777777-7777-7777-7777-777777777705"), "Entidade CalendarioCorporativo criada." },
                    { new Guid("70707070-7070-7070-7070-707070707753"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 53, new Guid("77777777-7777-7777-7777-777777777705"), "Entidade HorarioAtendimentoCalendario criada." },
                    { new Guid("70707070-7070-7070-7070-707070707754"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 54, new Guid("77777777-7777-7777-7777-777777777705"), "Entidade ExcecaoCalendarioCorporativo criada." },
                    { new Guid("70707070-7070-7070-7070-707070707755"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 55, new Guid("77777777-7777-7777-7777-777777777705"), "Migrations de calendário criadas." },
                    { new Guid("70707070-7070-7070-7070-707070707756"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 56, new Guid("77777777-7777-7777-7777-777777777705"), "Seed do calendário padrão criado." },
                    { new Guid("70707070-7070-7070-7070-707070707757"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 57, new Guid("77777777-7777-7777-7777-777777777705"), "Relacionamento entre Política SLA e Calendário criado." },
                    { new Guid("70707070-7070-7070-7070-707070707758"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 58, new Guid("77777777-7777-7777-7777-777777777705"), "Service administrativo de calendário criado." },
                    { new Guid("70707070-7070-7070-7070-707070707759"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 59, new Guid("77777777-7777-7777-7777-777777777705"), "Service de cálculo de tempo útil criado." },
                    { new Guid("70707070-7070-7070-7070-707070707760"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 60, new Guid("77777777-7777-7777-7777-777777777705"), "Cálculo de prazo de primeira resposta usando horário comercial implementado." },
                    { new Guid("70707070-7070-7070-7070-707070707761"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 61, new Guid("77777777-7777-7777-7777-777777777705"), "Cálculo de prazo de resolução usando horário comercial implementado." },
                    { new Guid("70707070-7070-7070-7070-707070707762"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 62, new Guid("77777777-7777-7777-7777-777777777705"), "Cálculo de minutos úteis de primeira resposta implementado." },
                    { new Guid("70707070-7070-7070-7070-707070707763"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 63, new Guid("77777777-7777-7777-7777-777777777705"), "Cálculo de minutos úteis de resolução implementado." },
                    { new Guid("70707070-7070-7070-7070-707070707764"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 64, new Guid("77777777-7777-7777-7777-777777777705"), "Endpoints administrativos de calendário criados." },
                    { new Guid("70707070-7070-7070-7070-707070707765"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 65, new Guid("77777777-7777-7777-7777-777777777705"), "Tela Admin > SLA > Calendários criada." },
                    { new Guid("70707070-7070-7070-7070-707070707766"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 66, new Guid("77777777-7777-7777-7777-777777777705"), "Tela de política SLA atualizada com seleção de calendário." },
                    { new Guid("70707070-7070-7070-7070-707070707767"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 2, true, 67, new Guid("77777777-7777-7777-7777-777777777705"), "Detalhe do chamado mostra tipo de cálculo e calendário usado." },
                    { new Guid("70707070-7070-7070-7070-707070707768"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 3, true, 68, new Guid("77777777-7777-7777-7777-777777777705"), "Testes automatizados criados." },
                    { new Guid("70707070-7070-7070-7070-707070707769"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 4", 4, true, 69, new Guid("77777777-7777-7777-7777-777777777705"), "Documentação atualizada." }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "situacao_atual", "status_tecnico" },
                values: new object[] { "- docs/SLA.md\n- src/SGX.SistemaChamado.Domain/Entities/PoliticaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/MetaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/ChamadoSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/CalendarioCorporativo.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaService.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaCalculator.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaBusinessTimeCalculator.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaPoliciesController.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaCalendarsController.cs\n- tests/SGX.SistemaChamado.Tests/SlaServiceTests.cs", "- Validar cálculo de horário comercial em cenário real com volume institucional.\n- Evoluir calendário por departamento/time quando a governança estiver definida.\n- Evoluir importação automática de feriados nacionais/municipais.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar política de proximidade do vencimento por canal/time.\n- Implementar alertas/notificações operacionais por SLA, se aplicável.\n- Consolidar trilha de auditoria e relatórios gerenciais de cumprimento.", "Sprints 1, 2 e 3 implementadas com políticas/metas, SLA aplicado aos chamados, alertas, eventos, monitoramento e painel gerencial. Sprint 4 em implementação para cálculo real em horário comercial com calendário corporativo, expediente, fins de semana e exceções.", 10 });

            migrationBuilder.UpdateData(
                table: "sla_politicas",
                keyColumn: "id",
                keyValue: new Guid("56565656-5656-5656-5656-565656565601"),
                column: "calendario_corporativo_id",
                value: null);

            migrationBuilder.InsertData(
                table: "horarios_atendimento_calendario",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "calendario_corporativo_id", "criado_em", "criado_por", "dia_semana", "hora_fim", "hora_inicio" },
                values: new object[,]
                {
                    { new Guid("56565656-5656-5656-5656-565656565711"), true, null, null, new Guid("56565656-5656-5656-5656-565656565701"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 1, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) },
                    { new Guid("56565656-5656-5656-5656-565656565712"), true, null, null, new Guid("56565656-5656-5656-5656-565656565701"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 2, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) },
                    { new Guid("56565656-5656-5656-5656-565656565713"), true, null, null, new Guid("56565656-5656-5656-5656-565656565701"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 3, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) },
                    { new Guid("56565656-5656-5656-5656-565656565714"), true, null, null, new Guid("56565656-5656-5656-5656-565656565701"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 4, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) },
                    { new Guid("56565656-5656-5656-5656-565656565715"), true, null, null, new Guid("56565656-5656-5656-5656-565656565701"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 5, new TimeOnly(18, 0, 0), new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_sla_politicas_calendario_corporativo_id",
                table: "sla_politicas",
                column: "calendario_corporativo_id");

            migrationBuilder.CreateIndex(
                name: "IX_chamado_slas_calendario_corporativo_id",
                table: "chamado_slas",
                column: "calendario_corporativo_id");

            migrationBuilder.CreateIndex(
                name: "ix_calendarios_corporativos_nome",
                table: "calendarios_corporativos",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ux_calendarios_corporativos_padrao_ativo",
                table: "calendarios_corporativos",
                column: "padrao",
                unique: true,
                filter: "padrao = true AND ativo = true");

            migrationBuilder.CreateIndex(
                name: "ix_excecoes_calendario_corporativo_data",
                table: "excecoes_calendario_corporativo",
                columns: new[] { "calendario_corporativo_id", "data" });

            migrationBuilder.CreateIndex(
                name: "ix_horarios_atendimento_calendario_dia",
                table: "horarios_atendimento_calendario",
                columns: new[] { "calendario_corporativo_id", "dia_semana" });

            migrationBuilder.AddForeignKey(
                name: "FK_chamado_slas_calendarios_corporativos_calendario_corporativ~",
                table: "chamado_slas",
                column: "calendario_corporativo_id",
                principalTable: "calendarios_corporativos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sla_politicas_calendarios_corporativos_calendario_corporati~",
                table: "sla_politicas",
                column: "calendario_corporativo_id",
                principalTable: "calendarios_corporativos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamado_slas_calendarios_corporativos_calendario_corporativ~",
                table: "chamado_slas");

            migrationBuilder.DropForeignKey(
                name: "FK_sla_politicas_calendarios_corporativos_calendario_corporati~",
                table: "sla_politicas");

            migrationBuilder.DropTable(
                name: "excecoes_calendario_corporativo");

            migrationBuilder.DropTable(
                name: "horarios_atendimento_calendario");

            migrationBuilder.DropTable(
                name: "calendarios_corporativos");

            migrationBuilder.DropIndex(
                name: "IX_sla_politicas_calendario_corporativo_id",
                table: "sla_politicas");

            migrationBuilder.DropIndex(
                name: "IX_chamado_slas_calendario_corporativo_id",
                table: "chamado_slas");

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707752"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707753"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707754"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707755"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707756"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707757"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707758"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707759"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707760"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707761"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707762"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707763"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707764"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707765"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707766"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707767"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707768"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707769"));

            migrationBuilder.DropColumn(
                name: "calendario_corporativo_id",
                table: "sla_politicas");

            migrationBuilder.DropColumn(
                name: "calendario_corporativo_id",
                table: "chamado_slas");

            migrationBuilder.DropColumn(
                name: "usar_horario_comercial",
                table: "chamado_slas");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "situacao_atual", "status_tecnico" },
                values: new object[] { "- docs/SLA.md\n- src/SGX.SistemaChamado.Domain/Entities/PoliticaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/MetaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/ChamadoSla.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaService.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaCalculator.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaPoliciesController.cs\n- tests/SGX.SistemaChamado.Tests/SlaServiceTests.cs", "- Validar aplicação de SLA em cenário real com volume institucional.\n- Evoluir cálculo de horário comercial com calendário corporativo e feriados.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar política de proximidade do vencimento por canal/time.\n- Implementar alertas/notificações operacionais por SLA, se aplicável.\n- Consolidar trilha de auditoria e relatórios gerenciais de cumprimento.", "Sprint 1 concluída com modelagem e cadastro administrativo de políticas/metas de SLA. Sprint 2 em implementação para aplicar políticas nos chamados, calcular marcos de primeira resposta e resolução, registrar violações/cumprimento, exibir situação operacional e habilitar filtros administrativos.", 8 });
        }
    }
}
