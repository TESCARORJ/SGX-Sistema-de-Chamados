using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirChecklistAberturaEmailRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM roadmap_checklist_itens
                 WHERE roadmap_item_id = '77777777-7777-7777-7777-777777777702';
                """);

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("68686868-6868-6868-6868-686868686701"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Worker", 2, true, 1, new Guid("77777777-7777-7777-7777-777777777702"), "Projeto Worker.Email validado/criado" },
                    { new Guid("68686868-6868-6868-6868-686868686702"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Configuracao", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777702"), "Configuracoes IMAP definidas" },
                    { new Guid("68686868-6868-6868-6868-686868686703"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Worker", 2, true, 3, new Guid("77777777-7777-7777-7777-777777777702"), "Leitura IMAP implementada" },
                    { new Guid("68686868-6868-6868-6868-686868686704"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Worker", 2, true, 4, new Guid("77777777-7777-7777-7777-777777777702"), "Processamento em lote implementado" },
                    { new Guid("68686868-6868-6868-6868-686868686705"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Persistencia", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777702"), "LogIntegracaoEmail implementado" },
                    { new Guid("68686868-6868-6868-6868-686868686706"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Consistencia", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777702"), "Prevencao de duplicidade por MessageId implementada" },
                    { new Guid("68686868-6868-6868-6868-686868686707"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Chamado", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777702"), "E-mail novo cria chamado" },
                    { new Guid("68686868-6868-6868-6868-686868686708"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Chamado", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777702"), "Origem E-mail aplicada ao chamado" },
                    { new Guid("68686868-6868-6868-6868-686868686709"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Chamado", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777702"), "Status inicial Aberto aplicado" },
                    { new Guid("68686868-6868-6868-6868-686868686710"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Historico", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777702"), "Historico inicial criado" },
                    { new Guid("68686868-6868-6868-6868-686868686711"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Correlacao", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777702"), "Correlacao por codigo do chamado implementada" },
                    { new Guid("68686868-6868-6868-6868-686868686712"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Correlacao", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777702"), "Correlacao por Message-Id/In-Reply-To implementada" },
                    { new Guid("68686868-6868-6868-6868-686868686713"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Comentarios", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777702"), "Resposta por e-mail adiciona comentario" },
                    { new Guid("68686868-6868-6868-6868-686868686714"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Anexos", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777702"), "Anexos por e-mail validados" },
                    { new Guid("68686868-6868-6868-6868-686868686715"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Anexos", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777702"), "Anexos permitidos sao salvos" },
                    { new Guid("68686868-6868-6868-6868-686868686716"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Anexos", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777702"), "Anexos invalidos sao rejeitados e logados" },
                    { new Guid("68686868-6868-6868-6868-686868686717"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Admin", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777702"), "Endpoint de logs administrativos implementado" },
                    { new Guid("68686868-6868-6868-6868-686868686718"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Frontend", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777702"), "Tela /admin/integracoes/email validada" },
                    { new Guid("68686868-6868-6868-6868-686868686719"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Frontend", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777702"), "Filtros de logs implementados" },
                    { new Guid("68686868-6868-6868-6868-686868686720"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Frontend", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777702"), "Detalhe de log em dialog implementado" },
                    { new Guid("68686868-6868-6868-6868-686868686721"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Testes", 3, true, 21, new Guid("77777777-7777-7777-7777-777777777702"), "Testes unitarios de processamento criados" },
                    { new Guid("68686868-6868-6868-6868-686868686722"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Testes", 3, true, 22, new Guid("77777777-7777-7777-7777-777777777702"), "Testes de correlacao criados" },
                    { new Guid("68686868-6868-6868-6868-686868686723"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Testes", 3, true, 23, new Guid("77777777-7777-7777-7777-777777777702"), "Testes de anexos criados" },
                    { new Guid("68686868-6868-6868-6868-686868686724"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Validacao", 3, true, 24, new Guid("77777777-7777-7777-7777-777777777702"), "Build backend validado" },
                    { new Guid("68686868-6868-6868-6868-686868686725"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Validacao", 3, true, 25, new Guid("77777777-7777-7777-7777-777777777702"), "Testes backend executados" },
                    { new Guid("68686868-6868-6868-6868-686868686726"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Validacao", 3, true, 26, new Guid("77777777-7777-7777-7777-777777777702"), "Build Worker validado" },
                    { new Guid("68686868-6868-6868-6868-686868686727"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Validacao", 3, true, 27, new Guid("77777777-7777-7777-7777-777777777702"), "Build frontend validado" },
                    { new Guid("68686868-6868-6868-6868-686868686728"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Homologacao", 5, true, 28, new Guid("77777777-7777-7777-7777-777777777702"), "Validacao com caixa IMAP real" },
                    { new Guid("68686868-6868-6868-6868-686868686729"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Homologacao", 5, true, 29, new Guid("77777777-7777-7777-7777-777777777702"), "Homologacao com e-mails reais" },
                    { new Guid("68686868-6868-6868-6868-686868686730"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Homologacao", 5, true, 30, new Guid("77777777-7777-7777-7777-777777777702"), "Validacao com anexos reais" },
                    { new Guid("68686868-6868-6868-6868-686868686731"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Evolucao", 1, true, 31, new Guid("77777777-7777-7777-7777-777777777702"), "Autenticacao OAuth para caixa Microsoft, se exigido" },
                    { new Guid("68686868-6868-6868-6868-686868686732"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Evolucao", 1, true, 32, new Guid("77777777-7777-7777-7777-777777777702"), "Retry/backoff em falhas temporarias" },
                    { new Guid("68686868-6868-6868-6868-686868686733"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Evolucao", 1, true, 33, new Guid("77777777-7777-7777-7777-777777777702"), "Dead-letter ou fila de mensagens com erro" },
                    { new Guid("68686868-6868-6868-6868-686868686734"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Evolucao", 1, true, 34, new Guid("77777777-7777-7777-7777-777777777702"), "Monitoramento/health check do Worker" },
                    { new Guid("68686868-6868-6868-6868-686868686735"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Evolucao", 1, true, 35, new Guid("77777777-7777-7777-7777-777777777702"), "Painel de reprocessamento manual de e-mails com erro" },
                    { new Guid("68686868-6868-6868-6868-686868686736"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Seguranca", 1, true, 36, new Guid("77777777-7777-7777-7777-777777777702"), "Sanitizacao avancada de HTML" },
                    { new Guid("68686868-6868-6868-6868-686868686737"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Seguranca", 1, true, 37, new Guid("77777777-7777-7777-7777-777777777702"), "Antivirus/varredura de anexos" },
                    { new Guid("68686868-6868-6868-6868-686868686738"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Testes", 3, true, 38, new Guid("77777777-7777-7777-7777-777777777702"), "Teste E2E com IMAP real" },
                    { new Guid("68686868-6868-6868-6868-686868686739"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Observabilidade", 1, true, 39, new Guid("77777777-7777-7777-7777-777777777702"), "Metricas operacionais do Worker" },
                    { new Guid("68686868-6868-6868-6868-686868686740"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Grupo solicitado: Observabilidade", 1, true, 40, new Guid("77777777-7777-7777-7777-777777777702"), "Alertas de falha recorrente no processamento de e-mail" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Validar fluxo completo com caixa IMAP real e e-mails reais antes de homologar", "Integracoes", "E-mail recebido na caixa configurada e processado pelo Worker, criando chamado com origem E-mail, status inicial, historico e vinculo com remetente. Respostas correlacionadas adicionam comentario ao chamado existente. Anexos permitidos sao tratados conforme regras de seguranca. Logs tecnicos ficam disponiveis na area administrativa.", 1, "Worker.Email; EmailWorkerOptions; LogIntegracaoEmail; ProcessarEmailRecebidoUseCase; EmailParaChamadoService; correlacao por assunto e headers; anexos por e-mail; endpoints de logs; tela /admin/integracoes/email; testes automatizados; docs/INTEGRACAO-EMAIL.md.", "Implementado funcionalmente; nao homologado e nao em producao sem validacao IMAP real.", "Validacao com caixa IMAP real, homologacao com e-mails reais e validacao com anexos reais.", "OAuth Microsoft (se exigido), retry/backoff, dead-letter, monitoramento do Worker, reprocessamento manual, sanitizacao avancada de HTML, antivirus de anexos e metricas/alertas operacionais.", 68, "Validar com caixa IMAP real em homologacao.", "Worker.Email, abertura por e-mail, correlacao de respostas, anexos e logs administrativos implementados tecnicamente", 1, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686701"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686702"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686703"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686704"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686705"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686706"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686707"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686708"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686709"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686710"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686711"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686712"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686713"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686714"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686715"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686716"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686717"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686718"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686719"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686720"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686721"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686722"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686723"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686724"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686725"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686726"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686727"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686728"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686729"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686730"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686731"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686732"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686733"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686734"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686735"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686736"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686737"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686738"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686739"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("68686868-6868-6868-6868-686868686740"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Testar e mostrar correlacao por codigo, assunto e resposta", "Integracao", null, 4, null, null, null, null, 0, null, "Prevista via Worker IMAP", 2, 0, 0 });
        }
    }
}
