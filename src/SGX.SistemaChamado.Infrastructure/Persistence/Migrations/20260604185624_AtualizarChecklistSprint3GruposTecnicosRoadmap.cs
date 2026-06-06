using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarChecklistSprint3GruposTecnicosRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM roadmap_checklist_itens
                WHERE roadmap_item_id = '77777777-7777-7777-7777-777777777720';
                """);

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000118"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000119"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000120"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000117"),
                column: "titulo",
                value: "Planejar escopo e criterios de aceite da Sprint 3");

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000117"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777720"), "Planejar escopo e criterios de aceite da Sprint 3" },
                    { new Guid("78787878-7878-7878-7878-000000000214"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777720"), "Mapear impacto do modelo atual de responsavel por chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000215"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777720"), "Modelar entidade GrupoTecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000216"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 4, new Guid("77777777-7777-7777-7777-777777777720"), "Modelar entidade MembroGrupoTecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000217"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 5, new Guid("77777777-7777-7777-7777-777777777720"), "Modelar entidade FilaAtendimento ou estrutura equivalente" },
                    { new Guid("78787878-7878-7878-7878-000000000218"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 6, new Guid("77777777-7777-7777-7777-777777777720"), "Definir vinculo entre chamado e grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000219"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 7, new Guid("77777777-7777-7777-7777-777777777720"), "Definir vinculo entre chamado e fila de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000220"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 8, new Guid("77777777-7777-7777-7777-777777777720"), "Definir regras de atribuicao individual sem quebrar o responsavel atual" },
                    { new Guid("78787878-7878-7878-7878-000000000221"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 9, new Guid("77777777-7777-7777-7777-777777777720"), "Definir regras de transferencia entre grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000222"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 1, true, 10, new Guid("77777777-7777-7777-7777-777777777720"), "Definir regras de auditoria para entrada, saida e transferencia de fila" },
                    { new Guid("78787878-7878-7878-7878-000000000223"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777720"), "Criar migration para grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000224"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777720"), "Criar migration para membros de grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000225"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777720"), "Criar migration para fila ou vinculo de fila do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000226"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777720"), "Criar indices necessarios para consulta por grupo, fila e responsavel" },
                    { new Guid("78787878-7878-7878-7878-000000000227"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777720"), "Garantir compatibilidade com chamados existentes" },
                    { new Guid("78787878-7878-7878-7878-000000000228"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777720"), "Criar contratos de grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000229"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777720"), "Criar servico de aplicacao para cadastro de grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000230"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777720"), "Criar servico de aplicacao para membros de grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000231"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777720"), "Criar regra para direcionar chamado a grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000232"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777720"), "Criar regra para assumir chamado da fila" },
                    { new Guid("78787878-7878-7878-7878-000000000233"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777720"), "Criar regra para transferir chamado entre grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000234"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777720"), "Criar regra para atribuir chamado a tecnico especifico" },
                    { new Guid("78787878-7878-7878-7878-000000000235"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 9, true, 23, new Guid("77777777-7777-7777-7777-777777777720"), "Criar historico/auditoria das movimentacoes" },
                    { new Guid("78787878-7878-7878-7878-000000000236"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777720"), "Ajustar consultas de chamados para considerar grupo tecnico e fila" },
                    { new Guid("78787878-7878-7878-7878-000000000237"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 7, true, 25, new Guid("77777777-7777-7777-7777-777777777720"), "Validar permissoes de acesso as operacoes de grupo e fila" },
                    { new Guid("78787878-7878-7878-7878-000000000238"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoints de cadastro de grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000239"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 27, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoints de membros de grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000240"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 28, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoints de direcionamento para grupo" },
                    { new Guid("78787878-7878-7878-7878-000000000241"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 29, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoint para assumir chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000242"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 30, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoint para transferencia de chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000243"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777720"), "Criar endpoint/listagem de fila por grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000244"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 32, new Guid("77777777-7777-7777-7777-777777777720"), "Criar tela ou secao de cadastro de grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000245"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 33, new Guid("77777777-7777-7777-7777-777777777720"), "Criar tela ou secao de membros por grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000246"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777720"), "Exibir grupo tecnico no detalhe do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000247"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 35, new Guid("77777777-7777-7777-7777-777777777720"), "Exibir fila de atendimento por grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000248"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 36, new Guid("77777777-7777-7777-7777-777777777720"), "Permitir assumir chamado pela fila" },
                    { new Guid("78787878-7878-7878-7878-000000000249"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777720"), "Permitir transferir chamado para outro grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000250"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777720"), "Ajustar listagem/filtros para grupo tecnico e fila" },
                    { new Guid("78787878-7878-7878-7878-000000000251"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 39, new Guid("77777777-7777-7777-7777-777777777720"), "Testar cadastro de grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000252"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 40, new Guid("77777777-7777-7777-7777-777777777720"), "Testar inclusao e remocao de membros" },
                    { new Guid("78787878-7878-7878-7878-000000000253"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 41, new Guid("77777777-7777-7777-7777-777777777720"), "Testar direcionamento de chamado para grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000254"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 42, new Guid("77777777-7777-7777-7777-777777777720"), "Testar assumir chamado da fila" },
                    { new Guid("78787878-7878-7878-7878-000000000255"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 43, new Guid("77777777-7777-7777-7777-777777777720"), "Testar transferencia entre grupos tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000256"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 44, new Guid("77777777-7777-7777-7777-777777777720"), "Testar preservacao do responsavel atual do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000257"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 45, new Guid("77777777-7777-7777-7777-777777777720"), "Testar auditoria das movimentacoes" },
                    { new Guid("78787878-7878-7878-7878-000000000258"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 46, new Guid("77777777-7777-7777-7777-777777777720"), "Testar filtros/listagens por grupo e fila" },
                    { new Guid("78787878-7878-7878-7878-000000000259"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 47, new Guid("77777777-7777-7777-7777-777777777720"), "Testar regressao do fluxo atual de abertura e atribuicao de chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000260"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 4, true, 48, new Guid("77777777-7777-7777-7777-777777777720"), "Documentar modelo de grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000261"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 4, true, 49, new Guid("77777777-7777-7777-7777-777777777720"), "Documentar regras de roteamento e transferencia" },
                    { new Guid("78787878-7878-7878-7878-000000000262"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 4, true, 50, new Guid("77777777-7777-7777-7777-777777777720"), "Documentar impacto no fluxo atual de chamados" },
                    { new Guid("78787878-7878-7878-7878-000000000263"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 4, true, 51, new Guid("77777777-7777-7777-7777-777777777720"), "Documentar criterios de testes tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000264"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 5, true, 52, new Guid("77777777-7777-7777-7777-777777777720"), "Preparar roteiro de homologacao de produtividade por grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000000265"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 5, true, 53, new Guid("77777777-7777-7777-7777-777777777720"), "Preparar roteiro de homologacao de visibilidade por fila" },
                    { new Guid("78787878-7878-7878-7878-000000000266"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 5, true, 54, new Guid("77777777-7777-7777-7777-777777777720"), "Registrar homologacao e aceite final" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000214"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000215"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000216"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000217"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000218"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000219"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000220"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000221"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000222"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000223"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000224"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000225"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000226"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000227"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000228"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000229"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000230"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000231"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000232"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000233"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000234"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000235"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000236"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000237"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000238"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000239"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000240"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000241"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000242"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000243"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000244"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000245"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000246"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000247"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000248"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000249"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000250"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000251"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000252"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000253"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000254"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000255"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000256"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000257"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000258"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000259"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000260"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000261"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000262"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000263"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000264"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000265"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000266"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000117"),
                column: "titulo",
                value: "Planejar escopo e criterios de aceite");

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000118"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777720"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000119"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777720"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000120"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 3 Grupos tecnicos, filas e atribuicao", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777720"), "Registrar homologacao e aceite" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                column: "percentual_implementacao",
                value: 25);
        }
    }
}
