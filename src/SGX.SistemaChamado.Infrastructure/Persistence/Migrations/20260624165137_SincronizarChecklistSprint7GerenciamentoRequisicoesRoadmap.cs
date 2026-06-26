using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarChecklistSprint7GerenciamentoRequisicoesRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000109"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Diagnosticar estado atual da Sprint 7 e inconsistencias do roadmap" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000110"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", 1, "Confirmar representacao da requisicao de servico como Chamado com NaturezaChamadoEnum.Requisicao" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000111"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 1, "Validar vinculo existente entre Chamado e Catalogo de Servicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000112"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 1, "Definir menor escopo seguro da abertura guiada por catalogo" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000913"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar ou ajustar contrato de consulta do servico para abertura" },
                    { new Guid("78787878-7878-7878-7878-000000000914"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar ou ajustar contrato de abertura guiada por catalogo com semantica de requisicao" },
                    { new Guid("78787878-7878-7878-7878-000000000915"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777718"), "Criar validator dedicado para abertura guiada por catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000000916"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar use case dedicado de abertura de requisicao de servico via catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000000917"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777718"), "Aplicar classificacao vinda do catalogo no backend" },
                    { new Guid("78787878-7878-7878-7878-000000000918"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777718"), "Aplicar grupo responsavel configurado no catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000000919"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777718"), "Aplicar SLA configurado ou fallback existente" },
                    { new Guid("78787878-7878-7878-7878-000000000920"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777718"), "Persistir vinculo entre chamado e servico do catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000000921"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar ou reutilizar formulario por servico" },
                    { new Guid("78787878-7878-7878-7878-000000000922"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777718"), "Validar e persistir respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000000923"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777718"), "Gerar aprovacao obrigatoria quando a regra aplicavel exigir" },
                    { new Guid("78787878-7878-7878-7878-000000000924"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777718"), "Preservar aprovacao legada sem duplicidade" },
                    { new Guid("78787878-7878-7878-7878-000000000925"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777718"), "Preservar abertura de incidentes e chamados sem catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000000926"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777718"), "Criar ou ajustar endpoints do portal para catalogo e abertura guiada" },
                    { new Guid("78787878-7878-7878-7878-000000000927"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar tela de catalogo no portal" },
                    { new Guid("78787878-7878-7878-7878-000000000928"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar detalhe do servico no portal" },
                    { new Guid("78787878-7878-7878-7878-000000000929"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar formulario guiado de abertura" },
                    { new Guid("78787878-7878-7878-7878-000000000930"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777718"), "Implementar confirmacao e acompanhamento da requisicao aberta" },
                    { new Guid("78787878-7878-7878-7878-000000000931"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 7, true, 23, new Guid("77777777-7777-7777-7777-777777777718"), "Garantir seguranca, autorizacao e ownership dos endpoints" },
                    { new Guid("78787878-7878-7878-7878-000000000932"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 9, true, 24, new Guid("77777777-7777-7777-7777-777777777718"), "Registrar historico e auditoria dos eventos relevantes" },
                    { new Guid("78787878-7878-7878-7878-000000000933"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 25, new Guid("77777777-7777-7777-7777-777777777718"), "Testar abertura por catalogo sem aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000934"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 26, new Guid("77777777-7777-7777-7777-777777777718"), "Testar abertura por catalogo com aprovacao obrigatoria" },
                    { new Guid("78787878-7878-7878-7878-000000000935"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777718"), "Testar formulario obrigatorio e respostas invalidas" },
                    { new Guid("78787878-7878-7878-7878-000000000936"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 28, new Guid("77777777-7777-7777-7777-777777777718"), "Testar grupo responsavel e SLA" },
                    { new Guid("78787878-7878-7878-7878-000000000937"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 29, new Guid("77777777-7777-7777-7777-777777777718"), "Testar regressao de abertura legada, incidente e atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000938"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 30, new Guid("77777777-7777-7777-7777-777777777718"), "Testar regressao de aprovacao legada e motor novo" },
                    { new Guid("78787878-7878-7878-7878-000000000939"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 31, new Guid("77777777-7777-7777-7777-777777777718"), "Executar build backend e testes direcionados" },
                    { new Guid("78787878-7878-7878-7878-000000000940"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 3, true, 32, new Guid("77777777-7777-7777-7777-777777777718"), "Executar build frontend e validacao TypeScript" },
                    { new Guid("78787878-7878-7878-7878-000000000941"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 9, true, 33, new Guid("77777777-7777-7777-7777-777777777718"), "Verificar EF pending model changes" },
                    { new Guid("78787878-7878-7878-7878-000000000942"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777718"), "Criar ou revisar migrations estruturais, se necessarias" },
                    { new Guid("78787878-7878-7878-7878-000000000943"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 2, true, 35, new Guid("77777777-7777-7777-7777-777777777718"), "Criar migration de dados ou checklist, se aplicavel" },
                    { new Guid("78787878-7878-7878-7878-000000000944"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 4, true, 36, new Guid("77777777-7777-7777-7777-777777777718"), "Atualizar documentacao principal da Sprint 7" },
                    { new Guid("78787878-7878-7878-7878-000000000945"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 5, true, 37, new Guid("77777777-7777-7777-7777-777777777718"), "Registrar homologacao funcional" },
                    { new Guid("78787878-7878-7878-7878-000000000946"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 5, true, 38, new Guid("77777777-7777-7777-7777-777777777718"), "Registrar homologacao visual responsiva" },
                    { new Guid("78787878-7878-7878-7878-000000000947"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 7 Gerenciamento de Requisicoes", 5, true, 39, new Guid("77777777-7777-7777-7777-777777777718"), "Registrar aceite formal somente com evidencia" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Reutilizar Chamado, Catalogo, SLA, Grupos Tecnicos e Aprovacao sem duplicar enums, status ou regras; backend deve permanecer fonte de verdade para classificacao, aprovacao, grupo responsavel e SLA do servico.", "CatalogoServicoId em Chamado; GET /api/portal/catalogo-servicos/{slug}/preparar-chamado; abertura por catalogo no AbrirChamadoUseCase; historico de abertura por catalogo; aprovacao automatica opcional por servico; tela de catalogo e detalhe do servico no portal.", "Validar abertura guiada de requisicao por catalogo em cenarios com e sem aprovacao, formulario obrigatorio, ownership dos endpoints e responsividade no portal.", "Criar fluxo guiado de requisicao sobre o chamado existente, com contrato e validator dedicados, aplicacao de grupo responsavel e SLA por servico, formulario por servico, persistencia de respostas, endpoints e telas guiadas sem romper incidentes e fluxos legados.", 49, "Implementar ou ajustar contrato de abertura guiada por catalogo com semantica explicita de requisicao.", "Abertura por catalogo ja existe no chamado comum, com consulta do servico, associacao CatalogoServicoId, aplicacao backend de classificacao e aprovacao automatica opcional por servico. Ainda nao existe fluxo separado e guiado de Requisicao de Servico com contrato, validator, use case e formulario dinamico dedicados." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000913"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000914"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000915"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000916"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000917"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000918"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000919"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000920"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000921"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000922"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000923"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000924"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000925"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000926"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000927"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000928"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000929"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000930"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000931"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000932"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000933"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000934"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000935"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000936"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000937"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000938"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000939"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000940"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000941"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000942"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000943"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000944"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000945"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000946"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000947"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000109"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { null, null, "Planejar escopo e criterios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000110"),
                columns: new[] { "atualizado_em", "atualizado_por", "grupo", "titulo" },
                values: new object[] { null, null, 2, "Implementar entregas centrais da sprint" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000111"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 3, "Executar testes funcionais e tecnicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000112"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 5, "Registrar homologacao e aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Orquestrar formulario por servico, aprovacao e atendimento sem duplicar regras.", "Capacidades base existentes de catalogo e aprovacao aproveitadas.", "Validar abertura guiada por catalogo com regras diferentes por servico.", "Fluxo de aprovacao por servico, status proprios, servicos relacionados e conclusao com aceite.", 51, "Vincular fluxo de requisicao ao catalogo no backend e frontend.", "Catalogo e aprovacao existem, porem sem fluxo separado de requisicao." });
        }
    }
}
