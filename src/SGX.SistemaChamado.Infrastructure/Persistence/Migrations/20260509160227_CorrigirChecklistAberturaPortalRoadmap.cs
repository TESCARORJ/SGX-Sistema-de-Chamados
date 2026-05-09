using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirChecklistAberturaPortalRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM roadmap_checklist_itens
                 WHERE roadmap_item_id = '77777777-7777-7777-7777-777777777701';
                """);

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("67676767-6767-6767-6767-676767676711"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 1, new Guid("77777777-7777-7777-7777-777777777701"), "Endpoint de contexto do portal validado" },
                    { new Guid("67676767-6767-6767-6767-676767676712"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 2, new Guid("77777777-7777-7777-7777-777777777701"), "Endpoint de criação de chamado validado" },
                    { new Guid("67676767-6767-6767-6767-676767676713"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 3, new Guid("77777777-7777-7777-7777-777777777701"), "Validações obrigatórias implementadas" },
                    { new Guid("67676767-6767-6767-6767-676767676714"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 4, new Guid("77777777-7777-7777-7777-777777777701"), "Solicitante obtido pelo usuário autenticado" },
                    { new Guid("67676767-6767-6767-6767-676767676715"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 5, new Guid("77777777-7777-7777-7777-777777777701"), "Status inicial Aberto aplicado" },
                    { new Guid("67676767-6767-6767-6767-676767676716"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 6, new Guid("77777777-7777-7777-7777-777777777701"), "Histórico inicial criado" },
                    { new Guid("67676767-6767-6767-6767-676767676717"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 8, new Guid("77777777-7777-7777-7777-777777777701"), "Formulário com validação visual implementado" },
                    { new Guid("67676767-6767-6767-6767-676767676718"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 7, new Guid("77777777-7777-7777-7777-777777777701"), "Tela /portal/chamados/novo implementada" },
                    { new Guid("67676767-6767-6767-6767-676767676719"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 9, new Guid("77777777-7777-7777-7777-777777777701"), "Consumo de GET /api/portal/contexto implementado" },
                    { new Guid("67676767-6767-6767-6767-676767676720"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 2, true, 10, new Guid("77777777-7777-7777-7777-777777777701"), "Consumo de POST /api/portal/chamados implementado" },
                    { new Guid("67676767-6767-6767-6767-676767676721"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 11, new Guid("77777777-7777-7777-7777-777777777701"), "Redirecionamento para detalhe após abertura implementado" },
                    { new Guid("67676767-6767-6767-6767-676767676722"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 12, new Guid("77777777-7777-7777-7777-777777777701"), "Listagem /portal/chamados validada tecnicamente" },
                    { new Guid("67676767-6767-6767-6767-676767676723"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 13, new Guid("77777777-7777-7777-7777-777777777701"), "Detalhe /portal/chamados/:id validado tecnicamente" },
                    { new Guid("67676767-6767-6767-6767-676767676724"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 14, new Guid("77777777-7777-7777-7777-777777777701"), "Chamado visível na fila administrativa" },
                    { new Guid("67676767-6767-6767-6767-676767676725"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 15, new Guid("77777777-7777-7777-7777-777777777701"), "Detalhe administrativo do chamado validado tecnicamente" },
                    { new Guid("67676767-6767-6767-6767-676767676726"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 16, new Guid("77777777-7777-7777-7777-777777777701"), "Build backend validado" },
                    { new Guid("67676767-6767-6767-6767-676767676727"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 17, new Guid("77777777-7777-7777-7777-777777777701"), "Testes backend executados com sucesso" },
                    { new Guid("67676767-6767-6767-6767-676767676728"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 18, new Guid("77777777-7777-7777-7777-777777777701"), "Build frontend validado" },
                    { new Guid("67676767-6767-6767-6767-676767676729"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 19, new Guid("77777777-7777-7777-7777-777777777701"), "Homologação manual com usuário real" },
                    { new Guid("67676767-6767-6767-6767-676767676730"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 3, true, 20, new Guid("77777777-7777-7777-7777-777777777701"), "Testes E2E frontend do fluxo de abertura" },
                    { new Guid("67676767-6767-6767-6767-676767676731"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 21, new Guid("77777777-7777-7777-7777-777777777701"), "Validação real de anexos em ambiente de homologação" },
                    { new Guid("67676767-6767-6767-6767-676767676732"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 22, new Guid("77777777-7777-7777-7777-777777777701"), "Validação de anexo inválido com mensagem amigável" },
                    { new Guid("67676767-6767-6767-6767-676767676733"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 23, new Guid("77777777-7777-7777-7777-777777777701"), "Validação completa do fluxo abrir, anexar e acompanhar" },
                    { new Guid("67676767-6767-6767-6767-676767676734"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 24, new Guid("77777777-7777-7777-7777-777777777701"), "Validação com perfil Solicitante real do Microsoft Entra ID" },
                    { new Guid("67676767-6767-6767-6767-676767676735"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", null, 5, true, 25, new Guid("77777777-7777-7777-7777-777777777701"), "Validação com Atendente visualizando o chamado na fila" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Validar com usuario real e consolidar evidencias de homologacao", "Solicitante autenticado consegue abrir chamado pelo portal com título, descrição, categoria e prioridade, anexar arquivo permitido, visualizar o detalhe do chamado, acompanhar o status no portal e o chamado aparece na fila administrativa para atendimento.", 1, "GET /api/portal/contexto; POST /api/portal/chamados; tela /portal/chamados/novo; listagem /portal/chamados; detalhe /portal/chamados/:id; fila /admin/chamados; testes backend; build frontend.", "Implementado funcionalmente; nao homologado em usuario real nesta iteracao.", "Validar com usuário real o fluxo completo de abrir chamado, anexar arquivo, acompanhar no portal e visualizar na fila administrativa.", "Testes E2E frontend do fluxo de abertura, validação real de anexos em homologação e script lint frontend.", 72, "Executar homologação manual do fluxo completo com usuário real.", "Fluxo implementado no portal com abertura, anexos opcionais, listagem e detalhe", 1, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676711"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676712"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676713"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676714"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676715"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676716"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676717"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676718"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676719"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676720"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676721"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676722"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676723"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676724"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676725"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676726"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676727"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676728"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676729"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676730"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676731"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676732"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676733"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676734"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676735"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Demonstrar fluxo completo: abrir, anexar, acompanhar", null, 4, null, null, null, null, 0, null, "Prevista no portal /portal", 2, 0, 0 });
        }
    }
}
