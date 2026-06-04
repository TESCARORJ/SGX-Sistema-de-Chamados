using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistSprint2RelacionamentosItsm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000181"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777736"), "Modelar entidade de relacionamento entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000182"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777736"), "Criar enum de tipos de vinculo entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000183"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 3, new Guid("77777777-7777-7777-7777-777777777736"), "Definir vinculos iniciais: Relacionado, Pai, Filho, Duplicado, Bloqueia, BloqueadoPor, DerivadoDe e Origina." },
                    { new Guid("78787878-7878-7878-7878-000000000184"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 4, new Guid("77777777-7777-7777-7777-777777777736"), "Criar configuracao ORM da entidade de relacionamento." },
                    { new Guid("78787878-7878-7878-7878-000000000185"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777736"), "Criar migration ou ajuste de persistencia necessario." },
                    { new Guid("78787878-7878-7878-7878-000000000186"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777736"), "Implementar validacao contra vinculo duplicado." },
                    { new Guid("78787878-7878-7878-7878-000000000187"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777736"), "Implementar validacao contra vinculo circular indevido." },
                    { new Guid("78787878-7878-7878-7878-000000000188"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 9, true, 8, new Guid("77777777-7777-7777-7777-777777777736"), "Registrar usuario, data, tipo de vinculo e justificativa." },
                    { new Guid("78787878-7878-7878-7878-000000000189"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 9, true, 9, new Guid("77777777-7777-7777-7777-777777777736"), "Registrar historico de vinculo criado." },
                    { new Guid("78787878-7878-7878-7878-000000000190"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 9, true, 10, new Guid("77777777-7777-7777-7777-777777777736"), "Registrar historico de vinculo removido." },
                    { new Guid("78787878-7878-7878-7878-000000000191"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777736"), "Criar servico de aplicacao para gerenciar vinculos entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000192"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777736"), "Criar endpoint para listar vinculos de um chamado." },
                    { new Guid("78787878-7878-7878-7878-000000000193"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777736"), "Criar endpoint para criar vinculo entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000194"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777736"), "Criar endpoint para remover ou inativar vinculo." },
                    { new Guid("78787878-7878-7878-7878-000000000195"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 7, true, 15, new Guid("77777777-7777-7777-7777-777777777736"), "Validar permissoes para criacao e remocao de vinculos." },
                    { new Guid("78787878-7878-7878-7878-000000000196"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777736"), "Implementar regra de dependencia entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000197"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777736"), "Implementar regra de bloqueio entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000198"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777736"), "Impedir fechamento indevido de chamado com dependencia obrigatoria ativa." },
                    { new Guid("78787878-7878-7878-7878-000000000199"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777736"), "Criar fluxo de chamado derivado." },
                    { new Guid("78787878-7878-7878-7878-000000000200"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777736"), "Criar vinculo automatico entre chamado origem e chamado derivado." },
                    { new Guid("78787878-7878-7878-7878-000000000201"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777736"), "Criar suporte a tarefas vinculadas ao chamado." },
                    { new Guid("78787878-7878-7878-7878-000000000202"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777736"), "Criar suporte a aprovacao vinculada ao chamado." },
                    { new Guid("78787878-7878-7878-7878-000000000203"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 9, true, 23, new Guid("77777777-7777-7777-7777-777777777736"), "Bloquear avanco de chamado com aprovacao pendente, quando aplicavel." },
                    { new Guid("78787878-7878-7878-7878-000000000204"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777736"), "Criar secao ou aba de relacionamentos no detalhe do chamado." },
                    { new Guid("78787878-7878-7878-7878-000000000205"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 25, new Guid("77777777-7777-7777-7777-777777777736"), "Exibir chamados vinculados no frontend." },
                    { new Guid("78787878-7878-7878-7878-000000000206"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777736"), "Exibir bloqueios, dependencias, derivacoes, tarefas e aprovacoes pendentes." },
                    { new Guid("78787878-7878-7878-7878-000000000207"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777736"), "Criar testes de dominio para relacionamentos entre chamados." },
                    { new Guid("78787878-7878-7878-7878-000000000208"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 3, true, 28, new Guid("77777777-7777-7777-7777-777777777736"), "Criar testes de integracao dos endpoints." },
                    { new Guid("78787878-7878-7878-7878-000000000209"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 5, true, 29, new Guid("77777777-7777-7777-7777-777777777736"), "Homologar cenario incidente-problema." },
                    { new Guid("78787878-7878-7878-7878-000000000210"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 5, true, 30, new Guid("77777777-7777-7777-7777-777777777736"), "Homologar cenario problema-mudanca." },
                    { new Guid("78787878-7878-7878-7878-000000000211"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 5, true, 31, new Guid("77777777-7777-7777-7777-777777777736"), "Homologar cenario requisicao com aprovacao." },
                    { new Guid("78787878-7878-7878-7878-000000000212"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 5, true, 32, new Guid("77777777-7777-7777-7777-777777777736"), "Homologar historico completo dos vinculos." },
                    { new Guid("78787878-7878-7878-7878-000000000213"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 2 Relacionamentos, dependencias e orquestracao ITSM", 4, true, 33, new Guid("77777777-7777-7777-7777-777777777736"), "Atualizar documentacao tecnica e funcional da Sprint 2." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000181"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000182"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000183"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000184"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000185"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000186"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000187"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000188"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000189"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000190"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000191"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000192"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000193"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000194"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000195"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000196"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000197"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000198"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000199"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000200"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000201"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000202"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000203"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000204"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000205"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000206"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000207"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000208"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000209"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000210"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000211"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000212"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000213"));
        }
    }
}
