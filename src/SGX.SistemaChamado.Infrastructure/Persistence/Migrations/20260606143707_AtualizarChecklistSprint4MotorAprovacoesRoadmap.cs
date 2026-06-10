using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarChecklistSprint4MotorAprovacoesRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000125"),
                columns: new[] { "atualizado_em", "atualizado_por", "descricao", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Planejamento", "Planejar escopo e criterios de aceite da Sprint 4" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000126"),
                columns: new[] { "atualizado_em", "atualizado_por", "descricao", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Planejamento", 1, "Mapear modulo de aprovacao existente" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000127"),
                columns: new[] { "descricao", "grupo", "titulo" },
                values: new object[] { "Categoria: Planejamento", 1, "Mapear fluxo atual de aprovacao em chamados" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000128"),
                columns: new[] { "descricao", "grupo", "titulo" },
                values: new object[] { "Categoria: Planejamento", 1, "Mapear pontos onde chamado deve ficar bloqueado por aprovacao pendente" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000300"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 5, new Guid("77777777-7777-7777-7777-777777777722"), "Definir conceito de motor de aprovacao ITSM reutilizavel" },
                    { new Guid("78787878-7878-7878-7878-000000000301"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 6, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de aprovacao por natureza ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000302"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 7, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de aprovacao por tipo de chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000303"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 8, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de aprovacao por servico sensivel" },
                    { new Guid("78787878-7878-7878-7878-000000000304"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 9, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de aprovacao por impacto e urgencia" },
                    { new Guid("78787878-7878-7878-7878-000000000305"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 10, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de aprovacao por custo ou risco" },
                    { new Guid("78787878-7878-7878-7878-000000000306"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 11, new Guid("77777777-7777-7777-7777-777777777722"), "Definir conceito de aprovador padrao" },
                    { new Guid("78787878-7878-7878-7878-000000000307"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 12, new Guid("77777777-7777-7777-7777-777777777722"), "Definir conceito de grupo aprovador" },
                    { new Guid("78787878-7878-7878-7878-000000000308"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 13, new Guid("77777777-7777-7777-7777-777777777722"), "Definir conceito de aprovacao multi-nivel" },
                    { new Guid("78787878-7878-7878-7878-000000000309"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 14, new Guid("77777777-7777-7777-7777-777777777722"), "Definir comportamento de aprovacao sequencial" },
                    { new Guid("78787878-7878-7878-7878-000000000310"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Arquitetura", 9, true, 15, new Guid("77777777-7777-7777-7777-777777777722"), "Definir comportamento de aprovacao paralela" },
                    { new Guid("78787878-7878-7878-7878-000000000311"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Regra de Negocio", 9, true, 16, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de bloqueio por decisao pendente" },
                    { new Guid("78787878-7878-7878-7878-000000000312"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Regra de Negocio", 9, true, 17, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de liberacao apos aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000313"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Regra de Negocio", 9, true, 18, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de rejeicao e encerramento ou retorno do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000314"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Regra de Negocio", 9, true, 19, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de cancelamento de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000315"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Regra de Negocio", 9, true, 20, new Guid("77777777-7777-7777-7777-777777777722"), "Definir regra de expiracao de aprovacao pendente" },
                    { new Guid("78787878-7878-7878-7878-000000000316"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Auditoria", 9, true, 21, new Guid("77777777-7777-7777-7777-777777777722"), "Definir historico/auditoria de solicitacao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000317"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Auditoria", 9, true, 22, new Guid("77777777-7777-7777-7777-777777777722"), "Definir historico/auditoria de decisao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000318"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Auditoria", 9, true, 23, new Guid("77777777-7777-7777-7777-777777777722"), "Definir historico/auditoria de rejeicao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000319"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Auditoria", 9, true, 24, new Guid("77777777-7777-7777-7777-777777777722"), "Definir historico/auditoria de aprovacao expirada ou cancelada" },
                    { new Guid("78787878-7878-7878-7878-000000000320"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Compatibilidade", 9, true, 25, new Guid("77777777-7777-7777-7777-777777777722"), "Avaliar compatibilidade com chamados existentes" },
                    { new Guid("78787878-7878-7878-7878-000000000321"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Compatibilidade", 9, true, 26, new Guid("77777777-7777-7777-7777-777777777722"), "Avaliar compatibilidade com fluxo atual de abertura de chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000322"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Compatibilidade", 9, true, 27, new Guid("77777777-7777-7777-7777-777777777722"), "Avaliar compatibilidade com fluxo atual de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000000323"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Compatibilidade", 9, true, 28, new Guid("77777777-7777-7777-7777-777777777722"), "Avaliar compatibilidade com SLA atual" },
                    { new Guid("78787878-7878-7878-7878-000000000324"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 29, new Guid("77777777-7777-7777-7777-777777777722"), "Modelar configuracao de regra de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000325"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 30, new Guid("77777777-7777-7777-7777-777777777722"), "Modelar instancia de aprovacao do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000326"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777722"), "Modelar etapa de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000327"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 32, new Guid("77777777-7777-7777-7777-777777777722"), "Modelar decisao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000328"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 33, new Guid("77777777-7777-7777-7777-777777777722"), "Criar migrations estruturais do motor de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000329"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777722"), "Criar contratos de configuracao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000330"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 35, new Guid("77777777-7777-7777-7777-777777777722"), "Criar contratos de decisao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000331"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 36, new Guid("77777777-7777-7777-7777-777777777722"), "Criar servico de aplicacao para regras de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000332"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777722"), "Criar servico de aplicacao para instancia de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000333"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777722"), "Criar regra para gerar aprovacao obrigatoria no chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000334"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 39, new Guid("77777777-7777-7777-7777-777777777722"), "Criar regra para bloquear movimentacao com aprovacao pendente" },
                    { new Guid("78787878-7878-7878-7878-000000000335"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 40, new Guid("77777777-7777-7777-7777-777777777722"), "Criar regra para aprovar chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000336"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 41, new Guid("77777777-7777-7777-7777-777777777722"), "Criar regra para rejeitar chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000337"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Desenvolvimento", 2, true, 42, new Guid("77777777-7777-7777-7777-777777777722"), "Criar regra para reavaliar aprovacao apos mudanca de dados sensiveis" },
                    { new Guid("78787878-7878-7878-7878-000000000338"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: API", 2, true, 43, new Guid("77777777-7777-7777-7777-777777777722"), "Criar endpoints administrativos de regras de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000339"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: API", 2, true, 44, new Guid("77777777-7777-7777-7777-777777777722"), "Criar endpoints de aprovacao e rejeicao" },
                    { new Guid("78787878-7878-7878-7878-000000000340"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: API", 2, true, 45, new Guid("77777777-7777-7777-7777-777777777722"), "Criar endpoints de consulta de pendencias de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000341"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 46, new Guid("77777777-7777-7777-7777-777777777722"), "Exibir status de aprovacao no detalhe do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000342"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 47, new Guid("77777777-7777-7777-7777-777777777722"), "Exibir pendencias de aprovacao para aprovador" },
                    { new Guid("78787878-7878-7878-7878-000000000343"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 48, new Guid("77777777-7777-7777-7777-777777777722"), "Criar tela ou secao de configuracao de regras de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000344"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 49, new Guid("77777777-7777-7777-7777-777777777722"), "Permitir aprovar chamado pela interface" },
                    { new Guid("78787878-7878-7878-7878-000000000345"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 50, new Guid("77777777-7777-7777-7777-777777777722"), "Permitir rejeitar chamado pela interface" },
                    { new Guid("78787878-7878-7878-7878-000000000346"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Frontend", 2, true, 51, new Guid("77777777-7777-7777-7777-777777777722"), "Ajustar listagem/filtros para aprovacao pendente" },
                    { new Guid("78787878-7878-7878-7878-000000000347"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 52, new Guid("77777777-7777-7777-7777-777777777722"), "Testar regra de aprovacao por natureza ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000348"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 53, new Guid("77777777-7777-7777-7777-777777777722"), "Testar regra de aprovacao por servico sensivel" },
                    { new Guid("78787878-7878-7878-7878-000000000349"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 54, new Guid("77777777-7777-7777-7777-777777777722"), "Testar bloqueio por aprovacao pendente" },
                    { new Guid("78787878-7878-7878-7878-000000000350"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 55, new Guid("77777777-7777-7777-7777-777777777722"), "Testar aprovacao e liberacao do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000351"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 56, new Guid("77777777-7777-7777-7777-777777777722"), "Testar rejeicao de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000352"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 57, new Guid("77777777-7777-7777-7777-777777777722"), "Testar aprovacao por grupo aprovador" },
                    { new Guid("78787878-7878-7878-7878-000000000353"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 58, new Guid("77777777-7777-7777-7777-777777777722"), "Testar aprovacao multi-nivel" },
                    { new Guid("78787878-7878-7878-7878-000000000354"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 59, new Guid("77777777-7777-7777-7777-777777777722"), "Testar regressao do fluxo atual de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000355"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Testes", 3, true, 60, new Guid("77777777-7777-7777-7777-777777777722"), "Testar regressao de abertura e atendimento de chamado" },
                    { new Guid("78787878-7878-7878-7878-000000000356"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Documentacao", 4, true, 61, new Guid("77777777-7777-7777-7777-777777777722"), "Documentar modelo do motor de aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000000357"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Documentacao", 4, true, 62, new Guid("77777777-7777-7777-7777-777777777722"), "Documentar regras de aprovacao ITSM" },
                    { new Guid("78787878-7878-7878-7878-000000000358"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Documentacao", 4, true, 63, new Guid("77777777-7777-7777-7777-777777777722"), "Documentar impacto no fluxo atual de chamados" },
                    { new Guid("78787878-7878-7878-7878-000000000359"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Documentacao", 4, true, 64, new Guid("77777777-7777-7777-7777-777777777722"), "Documentar criterios de testes tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000360"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Homologacao", 5, true, 65, new Guid("77777777-7777-7777-7777-777777777722"), "Preparar roteiro de homologacao de casos sensiveis" },
                    { new Guid("78787878-7878-7878-7878-000000000361"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Homologacao", 5, true, 66, new Guid("77777777-7777-7777-7777-777777777722"), "Preparar roteiro de homologacao de aprovacao por grupo" },
                    { new Guid("78787878-7878-7878-7878-000000000362"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Homologacao", 5, true, 67, new Guid("77777777-7777-7777-7777-777777777722"), "Preparar roteiro de homologacao de aprovacao multi-nivel" },
                    { new Guid("78787878-7878-7878-7878-000000000363"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Categoria: Homologacao", 5, true, 68, new Guid("77777777-7777-7777-7777-777777777722"), "Registrar homologacao e aceite final" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "observacao", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Evoluir o mecanismo atual de aprovacao do SGX Sistema de Chamados para um motor reutilizavel de aprovacoes ITSM, capaz de bloquear chamados sensiveis ate decisao formal, respeitando tipo de chamado, natureza ITSM, servico solicitado, grupo aprovador e regras futuras de multiplos niveis.", 3, "Mapear fluxo atual de aprovacao em chamados." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000300"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000301"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000302"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000303"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000304"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000305"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000306"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000307"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000308"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000309"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000310"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000311"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000312"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000313"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000314"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000315"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000316"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000317"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000318"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000319"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000320"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000321"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000322"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000323"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000324"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000325"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000326"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000327"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000328"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000329"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000330"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000331"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000332"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000333"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000334"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000335"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000336"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000337"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000338"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000339"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000340"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000341"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000342"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000343"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000344"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000345"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000346"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000347"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000348"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000349"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000350"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000351"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000352"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000353"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000354"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000355"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000356"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000357"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000358"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000359"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000360"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000361"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000362"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000363"));

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000125"),
                columns: new[] { "atualizado_em", "atualizado_por", "descricao", "titulo" },
                values: new object[] { null, null, "Sprint 4 Motor de Aprovacoes ITSM", "Planejar escopo e criterios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000126"),
                columns: new[] { "atualizado_em", "atualizado_por", "descricao", "grupo", "titulo" },
                values: new object[] { null, null, "Sprint 4 Motor de Aprovacoes ITSM", 2, "Implementar entregas centrais da sprint" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000127"),
                columns: new[] { "descricao", "grupo", "titulo" },
                values: new object[] { "Sprint 4 Motor de Aprovacoes ITSM", 3, "Executar testes funcionais e tecnicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000128"),
                columns: new[] { "descricao", "grupo", "titulo" },
                values: new object[] { "Sprint 4 Motor de Aprovacoes ITSM", 5, "Registrar homologacao e aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777722"),
                columns: new[] { "observacao", "percentual_implementacao", "proxima_acao" },
                values: new object[] { null, 50, "Introduzir regra de aprovacao por servico e natureza ITSM." });
        }
    }
}
