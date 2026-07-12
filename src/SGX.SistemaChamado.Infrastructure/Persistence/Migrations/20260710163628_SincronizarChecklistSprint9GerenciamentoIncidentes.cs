using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarChecklistSprint9GerenciamentoIncidentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000051"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000052"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000053"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000054"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000055"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000056"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000057"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000058"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000059"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000060"));

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000001"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777717"), "Diagnosticar estado atual dos chamados operacionais" },
                    { new Guid("78787878-7878-7878-7878-000000000002"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777717"), "Confirmar escopo funcional da Sprint 9" },
                    { new Guid("78787878-7878-7878-7878-000000000003"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777717"), "Definir criterios de aceite para Gerenciamento de Incidentes" },
                    { new Guid("78787878-7878-7878-7878-000000000004"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 4, new Guid("77777777-7777-7777-7777-777777777717"), "Documentar diferenca entre incidente, requisicao e chamado legado" },
                    { new Guid("78787878-7878-7878-7878-000000000005"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 5, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar limites atuais do fluxo de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000006"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 6, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar dependencias e riscos da Sprint 9, incluindo CMDB, SLA e autorizacao" },
                    { new Guid("78787878-7878-7878-7878-000000000007"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777717"), "Confirmar existencia da natureza Incidente no modelo ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000008"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777717"), "Validar matriz de status permitidos para Incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000009"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777717"), "Exigir impacto e urgencia na criacao de incidentes" },
                    { new Guid("78787878-7878-7878-7878-000000000010"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar classificacao de incidente por e-mail" },
                    { new Guid("78787878-7878-7878-7878-000000000011"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar compatibilidade de Incidente nos filtros do dashboard administrativo" },
                    { new Guid("78787878-7878-7878-7878-000000000012"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar compatibilidade de Incidente nos relatorios administrativos" },
                    { new Guid("78787878-7878-7878-7878-000000000013"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar compatibilidade de Incidente nas acoes disponiveis do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000014"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar compatibilidade de Incidente na abertura legada do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000015"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777717"), "Sincronizar SeedData, teste, migration e documentacao da Sprint 9" },
                    { new Guid("78787878-7878-7878-7878-000000000016"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar regra de fechamento" },
                    { new Guid("78787878-7878-7878-7878-000000000017"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar compatibilidade com status atual do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000018"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar limitacao de SLA se ainda reutilizar SLA existente" },
                    { new Guid("78787878-7878-7878-7878-000000000019"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar prioridade por impacto e urgencia" },
                    { new Guid("78787878-7878-7878-7878-000000000020"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para DTOs de abertura de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000021"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para validators de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000022"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de abertura" },
                    { new Guid("78787878-7878-7878-7878-000000000023"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 23, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de triagem" },
                    { new Guid("78787878-7878-7878-7878-000000000024"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000025"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 25, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de diagnostico" },
                    { new Guid("78787878-7878-7878-7878-000000000026"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de workaround" },
                    { new Guid("78787878-7878-7878-7878-000000000027"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 27, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de resolucao" },
                    { new Guid("78787878-7878-7878-7878-000000000028"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 28, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000029"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 29, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para use case de fechamento" },
                    { new Guid("78787878-7878-7878-7878-000000000030"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 30, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para historico de diagnostico, workaround e resolucao" },
                    { new Guid("78787878-7878-7878-7878-000000000031"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para auditoria minima" },
                    { new Guid("78787878-7878-7878-7878-000000000032"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 32, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para endpoints de abertura/consulta de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000033"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 33, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para endpoints de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000034"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para endpoints de resolucao" },
                    { new Guid("78787878-7878-7878-7878-000000000035"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 35, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para endpoints de reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000036"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 36, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para endpoints de fechamento" },
                    { new Guid("78787878-7878-7878-7878-000000000037"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para contratos sem expor detalhes internos do dominio" },
                    { new Guid("78787878-7878-7878-7878-000000000038"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para abertura de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000039"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 39, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para tela de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000040"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 40, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para diagnostico e workaround" },
                    { new Guid("78787878-7878-7878-7878-000000000041"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 41, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para resolucao" },
                    { new Guid("78787878-7878-7878-7878-000000000042"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 42, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar pendencia para reabertura" },
                    { new Guid("78787878-7878-7878-7878-000000000043"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 7, true, 43, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar autorizacao por acao operacional de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000044"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 7, true, 44, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar protecao de payload e integridade de metadados" },
                    { new Guid("78787878-7878-7878-7878-000000000045"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 3, true, 45, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar testes de abertura e triagem de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000046"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 3, true, 46, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar testes de atendimento e diagnostico de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000047"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 3, true, 47, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar testes de workaround e resolucao de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000048"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 3, true, 48, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar testes de reabertura e fechamento de incidente" },
                    { new Guid("78787878-7878-7878-7878-000000000049"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 9, true, 49, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar documentacao tecnica e rastreabilidade da Sprint 9" },
                    { new Guid("78787878-7878-7878-7878-000000000050"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 5, true, 50, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar homologacao funcional, visual e aceite formal" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777717"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar status, campos, API, frontend e SLA de incidente sem quebrar o fluxo atual ou a Sprint 8.", "O checklist tecnico da Sprint 9 deve refletir o estado real da trilha ITSM, com o que ja existe evidenciado em codigo, teste, documentacao e migration, sem implementar o fluxo de incidente nesta entrega.", "Checklist tecnico consolidado em 50 itens, com 18 concluidos e 32 pendentes, com evidencias em dominio, servicos, UI, seed, teste, migration e documentacao.", "Formalizar o backlog tecnico de Incidente com rastreabilidade, compatibilidade ITSM e preparo incremental do fluxo funcional.", "Checklist tecnico consolidado em 50 itens, com 18 concluidos e 32 pendentes.", "Homologacao funcional, visual, de permissao e aceite formal permanecem pendentes ate a implementacao do fluxo de incidente.", "Modelagem de incidente, contratos, DTOs, validators, use cases, endpoints, telas, seguranca, homologacao e CI afetado sem CMDB.", 36, "Implementar os itens pendentes de modelagem, backend, API, frontend, testes, seguranca, governanca e homologacao do fluxo de incidente.", "Chamados operacionais existem, mas o fluxo de incidente ainda depende de modelagem, contratos, telas, seguranca e homologacao funcional." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000001"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000002"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000003"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000004"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000005"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000006"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000007"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000008"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000009"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000010"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000011"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000012"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000013"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000014"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000015"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000016"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000017"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000018"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000019"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000020"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000021"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000022"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000023"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000024"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000025"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000026"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000027"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000028"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000029"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000030"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000031"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000032"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000033"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000034"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000035"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000036"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000037"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000038"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000039"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000040"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000041"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000042"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000043"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000044"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000045"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000046"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000047"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000048"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000049"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000050"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000051"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000052"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000053"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000054"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000055"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000056"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000057"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000058"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000059"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000060"));

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000105"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777717"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000106"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777717"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000107"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777717"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000108"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 9 Gerenciamento de Incidentes", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777717"), "Registrar homologacao e aceite" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777717"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar status, campos e SLA de incidente sem quebrar fluxo atual.", "Incidente deve ser aberto, classificado, priorizado, atendido, resolvido, reaberto e fechado com rastreabilidade.", "Fluxo alvo definido no novo roadmap ITIL.", "Formalizar fluxo de Incidente para falha, indisponibilidade ou degradacao de servico.", null, "Homologar ciclo abrir, triar, atender, resolver, reabrir e fechar.", "Servico afetado, CI afetado, causa provavel, solucao de contorno e regra de reabertura.", 90, "Implementar estados de incidente e campos especificos no chamado.", "Chamados operacionais existem, mas sem trilha completa de incidente com diagnostico e workaround." });
        }
    }
}
