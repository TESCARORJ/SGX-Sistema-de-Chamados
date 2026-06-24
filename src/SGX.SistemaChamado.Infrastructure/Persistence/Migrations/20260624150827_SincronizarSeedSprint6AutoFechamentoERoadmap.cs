using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarSeedSprint6AutoFechamentoERoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO parametros_sistema (id, ativo, atualizado_em, atualizado_por, chave, criado_em, criado_por, descricao, sensivel, valor)
                VALUES ('e0000000-0000-0000-0000-000000000001', TRUE, NULL, NULL, 'chamados.fechamento_automatico.prazo_aceite_horas', TIMESTAMPTZ '2026-01-01T00:00:00Z', 'seed.sistema', 'Prazo em horas para fechamento automatico por falta de aceite', FALSE, '48')
                ON CONFLICT (id) DO UPDATE
                SET ativo = EXCLUDED.ativo,
                    atualizado_em = EXCLUDED.atualizado_em,
                    atualizado_por = EXCLUDED.atualizado_por,
                    chave = EXCLUDED.chave,
                    descricao = EXCLUDED.descricao,
                    sensivel = EXCLUDED.sensivel,
                    valor = EXCLUDED.valor;
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO parametros_sistema (id, ativo, atualizado_em, atualizado_por, chave, criado_em, criado_por, descricao, sensivel, valor)
                VALUES ('e0000000-0000-0000-0000-000000000002', TRUE, NULL, NULL, 'chamados.reabertura.prazo_maximo_horas', TIMESTAMPTZ '2026-01-01T00:00:00Z', 'seed.sistema', 'Prazo maximo em horas para reabertura de chamado encerrado', FALSE, '48')
                ON CONFLICT (id) DO UPDATE
                SET ativo = EXCLUDED.ativo,
                    atualizado_em = EXCLUDED.atualizado_em,
                    atualizado_por = EXCLUDED.atualizado_por,
                    chave = EXCLUDED.chave,
                    descricao = EXCLUDED.descricao,
                    sensivel = EXCLUDED.sensivel,
                    valor = EXCLUDED.valor;
                """);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 1, "Diagnosticar estruturas existentes de notificacoes e eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 2, "Modelar entidade Notificacao e contrato de eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 2, "Criar configuracao EF e migration estrutural de notificacoes" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000901"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 5, new Guid("77777777-7777-7777-7777-777777777725"), "Testar dominio e estrutura persistente de notificacoes" },
                    { new Guid("78787878-7878-7878-7878-000000000902"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777725"), "Criar servico de geracao idempotente de notificacoes" },
                    { new Guid("78787878-7878-7878-7878-000000000903"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar resolucao de destinatarios por participacao e perfil" },
                    { new Guid("78787878-7878-7878-7878-000000000904"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777725"), "Modelar templates e materializacao de conteudo" },
                    { new Guid("78787878-7878-7878-7878-000000000905"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 5, true, 9, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar preferencias de notificacao por usuario e evento" },
                    { new Guid("78787878-7878-7878-7878-000000000906"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar processamento e controle de tentativas de entrega" },
                    { new Guid("78787878-7878-7878-7878-000000000907"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal Sistema" },
                    { new Guid("78787878-7878-7878-7878-000000000908"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal E-mail" },
                    { new Guid("78787878-7878-7878-7878-000000000909"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 13, new Guid("77777777-7777-7777-7777-777777777725"), "Criar API de consulta, leitura e marcacao como nao lida" },
                    { new Guid("78787878-7878-7878-7878-000000000910"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 5, true, 14, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar central de notificacoes no frontend" },
                    { new Guid("78787878-7878-7878-7878-000000000911"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 15, new Guid("77777777-7777-7777-7777-777777777725"), "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao" },
                    { new Guid("78787878-7878-7878-7878-000000000912"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 5, true, 16, new Guid("77777777-7777-7777-7777-777777777725"), "Documentar, homologar e registrar aceite da Sprint 6" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Preservar a separacao entre fato de negocio, resolucao de destinatarios, materializacao de conteudo, geracao idempotente, processamento e entrega, mantendo fora do escopo aprovacao/SLA sem ponto estavel de notificacao nesta etapa.", "Eventos priorizados integrados ao pipeline de notificacoes via orquestrador interno, com pontos estaveis em abertura, atribuicao/assuncao, status relevante e encerramento; idempotencia por evento/destinatario/canal; testes unitarios, integracao e regressao; compatibilidade com frontend, processamento e canais Sistema/Email; sem SignalR, sem fila externa, sem outbox improvisada e sem alterar Worker.Email.", "Validar recebimento real por solicitante e responsavel, confirmar templates ativos no ambiente, revisar eventos adiados e registrar aceite institucional da Sprint 6.", "Executar homologacao funcional/manual da Sprint 6 com templates ativos no ambiente, cenarios reais por perfil e evidencias formais, sem antecipar item 16 nem ampliar escopo para todos os eventos, aprovacao completa ou SLA.", 94, "Documentar, homologar e registrar aceite da Sprint 6", "Notificacoes internas persistidas, inbox autenticada e central frontend concluida; eventos ITSM priorizados agora integram o pipeline de geracao idempotente sem entrega sincrona nem impacto indevido em abertura, atribuicao, status, encerramento ou fluxos legados.", 2, 6 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000901"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000902"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000903"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000904"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000905"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000906"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000907"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000908"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000909"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000910"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000911"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000912"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 2, "Implementar entregas centrais da sprint" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 3, "Executar testes funcionais e tecnicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 5, "Registrar homologacao e aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Definir modelo de notificacao sem acoplamento excessivo com canais externos.", "Escopo sprint definido.", "Validar recebimento por perfil, observador, aprovador e grupo tecnico.", "Tabela de notificacoes, API leitura/nao lida, preferencias e regras por evento.", 25, "Modelar entidade Notificacao e pipeline de eventos.", "Notificacoes ainda nao estao consolidadas como modulo persistente por evento ITSM.", 1, 0 });
        }
    }
}
