using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoadmapSlaSprint2Checklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707701"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707702"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707703"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707704"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707705"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707706"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707707"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707708"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707709"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707710"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707711"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707712"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707713"),
                column: "concluido",
                value: true);

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("70707070-7070-7070-7070-707070707714"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777705"), "Tabela de SLA aplicado ao chamado criada." },
                    { new Guid("70707070-7070-7070-7070-707070707715"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777705"), "Relacionamento entre chamado e SLA criado." },
                    { new Guid("70707070-7070-7070-7070-707070707716"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777705"), "Service de cálculo de SLA criado." },
                    { new Guid("70707070-7070-7070-7070-707070707717"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777705"), "Política aplicável identificada por prioridade/categoria/departamento." },
                    { new Guid("70707070-7070-7070-7070-707070707718"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777705"), "SLA aplicado na criação do chamado." },
                    { new Guid("70707070-7070-7070-7070-707070707719"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777705"), "Prazo de primeira resposta calculado." },
                    { new Guid("70707070-7070-7070-7070-707070707720"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777705"), "Prazo de resolução calculado." },
                    { new Guid("70707070-7070-7070-7070-707070707721"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777705"), "Primeira resposta registrada." },
                    { new Guid("70707070-7070-7070-7070-707070707722"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777705"), "Resolução registrada." },
                    { new Guid("70707070-7070-7070-7070-707070707723"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 23, new Guid("77777777-7777-7777-7777-777777777705"), "Pausa de SLA preparada ou implementada." },
                    { new Guid("70707070-7070-7070-7070-707070707724"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777705"), "Situação atual do SLA calculada." },
                    { new Guid("70707070-7070-7070-7070-707070707725"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 25, new Guid("77777777-7777-7777-7777-777777777705"), "SLA exibido no detalhe do chamado." },
                    { new Guid("70707070-7070-7070-7070-707070707726"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777705"), "SLA exibido na listagem administrativa." },
                    { new Guid("70707070-7070-7070-7070-707070707727"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 27, new Guid("77777777-7777-7777-7777-777777777705"), "Filtros administrativos de SLA criados." },
                    { new Guid("70707070-7070-7070-7070-707070707728"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 2, true, 28, new Guid("77777777-7777-7777-7777-777777777705"), "DTOs de chamado atualizados com resumo de SLA." },
                    { new Guid("70707070-7070-7070-7070-707070707729"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 3, true, 29, new Guid("77777777-7777-7777-7777-777777777705"), "Testes automatizados criados." },
                    { new Guid("70707070-7070-7070-7070-707070707730"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist da Sprint 2", 4, true, 30, new Guid("77777777-7777-7777-7777-777777777705"), "Documentação atualizada." }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_tecnico" },
                values: new object[] { "- docs/SLA.md\n- src/SGX.SistemaChamado.Domain/Entities/PoliticaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/MetaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/ChamadoSla.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaService.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaCalculator.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaPoliciesController.cs\n- tests/SGX.SistemaChamado.Tests/SlaServiceTests.cs", "- Validar aplicação de SLA em cenário real com volume institucional.\n- Evoluir cálculo de horário comercial com calendário corporativo e feriados.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar política de proximidade do vencimento por canal/time.\n- Implementar alertas/notificações operacionais por SLA, se aplicável.\n- Consolidar trilha de auditoria e relatórios gerenciais de cumprimento.", 43, "Executar homologação funcional de ponta a ponta com usuários reais e validar regras de SLA em ambiente publicado, incluindo casos de pausa, reabertura e governança operacional.", "Sprint 1 concluída com modelagem e cadastro administrativo de políticas/metas de SLA. Sprint 2 em implementação para aplicar políticas nos chamados, calcular marcos de primeira resposta e resolução, registrar violações/cumprimento, exibir situação operacional e habilitar filtros administrativos.", 8 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707714"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707715"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707716"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707717"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707718"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707719"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707720"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707721"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707722"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707723"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707724"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707725"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707726"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707727"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707728"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707729"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707730"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707701"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707702"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707703"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707704"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707705"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707706"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707707"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707708"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707709"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707710"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707711"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707712"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707713"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_tecnico" },
                values: new object[] { "Ainda não há evidência técnica suficiente de implementação funcional do SLA. Preencher após implementação com entidades de SLA, migrations, serviços de cálculo, endpoints, telas, testes e documentação atualizada.", "- Definir modelo de dados para políticas de SLA.\n- Definir regras de primeira resposta, atendimento e resolução.\n- Definir se o SLA será por prioridade, categoria, departamento, tipo de chamado ou combinação de critérios.\n- Definir cálculo com horário útil.\n- Definir tratamento de feriados e dias não úteis.\n- Definir comportamento quando o chamado for pausado, suspenso ou aguardando solicitante.\n- Definir comportamento quando o chamado for reaberto.\n- Definir comportamento quando prioridade, categoria ou departamento forem alterados.\n- Implementar persistência dos marcos de SLA no chamado.\n- Implementar serviço backend centralizado para cálculo de SLA.\n- Implementar endpoints administrativos para cadastro e manutenção das políticas de SLA.\n- Implementar exibição do SLA no detalhe do chamado.\n- Implementar indicadores de chamados dentro do prazo, próximos do vencimento e atrasados.\n- Implementar filtros por status de SLA.\n- Implementar alertas/notificações de proximidade de vencimento, se aplicável.\n- Criar testes automatizados para cálculo de SLA.", 0, "Definir o modelo funcional do SLA, criar entidades/migrations, implementar o serviço centralizado de cálculo, criar tela administrativa para políticas de SLA e exibir o status de SLA nos chamados.", "Item previsto no roadmap, mas ainda sem implementação funcional confirmada. O sistema precisa evoluir para permitir cadastro de políticas de SLA, associação com chamados, cálculo de vencimento, identificação de chamados dentro do prazo, próximos do vencimento e atrasados, além de exibição desses dados para solicitantes, atendentes, gestores e administradores.", 7 });
        }
    }
}
