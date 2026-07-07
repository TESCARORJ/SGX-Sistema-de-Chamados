using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarChecklistSprint8CatalogoServicos2Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000113"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000114"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000115"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000116"));

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000001001"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777719"), "Diagnosticar estado atual do Catalogo 2.0 e pendencias transferidas da Sprint 7" },
                    { new Guid("78787878-7878-7878-7878-000000001002"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 1, true, 2, new Guid("77777777-7777-7777-7777-777777777719"), "Confirmar escopo estrutural do Catalogo 2.0" },
                    { new Guid("78787878-7878-7878-7878-000000001003"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 1, true, 3, new Guid("77777777-7777-7777-7777-777777777719"), "Definir criterios de aceite para motor de abertura guiada por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001004"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 9, true, 4, new Guid("77777777-7777-7777-7777-777777777719"), "Documentar decisao de transferencia dos itens 10, 13 e 14 da Sprint 7" },
                    { new Guid("78787878-7878-7878-7878-000000001005"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel" },
                    { new Guid("78787878-7878-7878-7878-000000001006"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777719"), "Configurar EF Core para vinculo entre catalogo e grupo tecnico" },
                    { new Guid("78787878-7878-7878-7878-000000001007"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777719"), "Criar migration estrutural para grupo tecnico no catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001008"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar contratos administrativos do catalogo para grupo tecnico responsavel" },
                    { new Guid("78787878-7878-7878-7878-000000001009"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar validators administrativos do catalogo para grupo tecnico responsavel" },
                    { new Guid("78787878-7878-7878-7878-000000001010"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar use cases administrativos do catalogo para grupo tecnico responsavel" },
                    { new Guid("78787878-7878-7878-7878-000000001011"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777719"), "Expor grupo tecnico responsavel na consulta administrativa do catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001012"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777719"), "Aplicar grupo tecnico responsavel na abertura guiada por catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001013"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 13, new Guid("77777777-7777-7777-7777-777777777719"), "Preservar fallback de grupo quando servico nao possuir grupo configurado" },
                    { new Guid("78787878-7878-7878-7878-000000001014"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 14, new Guid("77777777-7777-7777-7777-777777777719"), "Testar aplicacao de grupo tecnico configurado no catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001015"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 15, new Guid("77777777-7777-7777-7777-777777777719"), "Testar fallback de grupo sem configuracao no catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001016"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar entidade de formulario por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001017"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar campos do formulario por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001018"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar tipos de campo permitidos" },
                    { new Guid("78787878-7878-7878-7878-000000001019"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar obrigatoriedade, ordem, ajuda e visibilidade dos campos" },
                    { new Guid("78787878-7878-7878-7878-000000001020"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar opcoes de campos enumerados, se aplicavel" },
                    { new Guid("78787878-7878-7878-7878-000000001021"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar versionamento de formulario por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001022"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777719"), "Configurar EF Core para formulario e campos" },
                    { new Guid("78787878-7878-7878-7878-000000001023"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 23, new Guid("77777777-7777-7777-7777-777777777719"), "Criar migration estrutural para formulario dinamico" },
                    { new Guid("78787878-7878-7878-7878-000000001024"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar contratos administrativos para manutencao de formulario do servico" },
                    { new Guid("78787878-7878-7878-7878-000000001025"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 25, new Guid("77777777-7777-7777-7777-777777777719"), "Criar validators administrativos para formulario do servico" },
                    { new Guid("78787878-7878-7878-7878-000000001026"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777719"), "Criar use cases administrativos para configurar formulario do servico" },
                    { new Guid("78787878-7878-7878-7878-000000001027"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 27, new Guid("77777777-7777-7777-7777-777777777719"), "Criar endpoints administrativos para formulario do servico" },
                    { new Guid("78787878-7878-7878-7878-000000001028"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 28, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar frontend administrativo do catalogo para configurar formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001029"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 29, new Guid("77777777-7777-7777-7777-777777777719"), "Testar configuracao administrativa de formulario por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001030"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 30, new Guid("77777777-7777-7777-7777-777777777719"), "Expor campos do formulario no endpoint de preparacao da abertura" },
                    { new Guid("78787878-7878-7878-7878-000000001031"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar contrato de abertura guiada para receber respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001032"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 32, new Guid("77777777-7777-7777-7777-777777777719"), "Criar validator de respostas do formulario na abertura guiada" },
                    { new Guid("78787878-7878-7878-7878-000000001033"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 33, new Guid("77777777-7777-7777-7777-777777777719"), "Validar obrigatoriedade dos campos no backend" },
                    { new Guid("78787878-7878-7878-7878-000000001034"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 34, new Guid("77777777-7777-7777-7777-777777777719"), "Validar tipos e formatos das respostas no backend" },
                    { new Guid("78787878-7878-7878-7878-000000001035"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 7, true, 35, new Guid("77777777-7777-7777-7777-777777777719"), "Impedir respostas de campos inexistentes ou de outro servico" },
                    { new Guid("78787878-7878-7878-7878-000000001036"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 36, new Guid("77777777-7777-7777-7777-777777777719"), "Preservar abertura guiada sem formulario configurado" },
                    { new Guid("78787878-7878-7878-7878-000000001037"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 37, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar frontend do portal para renderizar formulario dinamico" },
                    { new Guid("78787878-7878-7878-7878-000000001038"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 38, new Guid("77777777-7777-7777-7777-777777777719"), "Ajustar frontend do portal para enviar respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001039"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 39, new Guid("77777777-7777-7777-7777-777777777719"), "Testar abertura guiada com formulario valido" },
                    { new Guid("78787878-7878-7878-7878-000000001040"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 40, new Guid("77777777-7777-7777-7777-777777777719"), "Testar abertura guiada com campos obrigatorios ausentes" },
                    { new Guid("78787878-7878-7878-7878-000000001041"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 41, new Guid("77777777-7777-7777-7777-777777777719"), "Testar abertura guiada com respostas invalidas" },
                    { new Guid("78787878-7878-7878-7878-000000001042"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 42, new Guid("77777777-7777-7777-7777-777777777719"), "Testar abertura guiada sem formulario configurado" },
                    { new Guid("78787878-7878-7878-7878-000000001043"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 43, new Guid("77777777-7777-7777-7777-777777777719"), "Modelar persistencia das respostas do formulario no chamado" },
                    { new Guid("78787878-7878-7878-7878-000000001044"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 44, new Guid("77777777-7777-7777-7777-777777777719"), "Configurar EF Core para respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001045"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 45, new Guid("77777777-7777-7777-7777-777777777719"), "Criar migration estrutural para respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001046"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 46, new Guid("77777777-7777-7777-7777-777777777719"), "Persistir respostas do formulario na abertura guiada" },
                    { new Guid("78787878-7878-7878-7878-000000001047"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 47, new Guid("77777777-7777-7777-7777-777777777719"), "Exibir respostas do formulario no detalhe do chamado" },
                    { new Guid("78787878-7878-7878-7878-000000001048"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 48, new Guid("77777777-7777-7777-7777-777777777719"), "Exibir respostas do formulario no portal do solicitante" },
                    { new Guid("78787878-7878-7878-7878-000000001049"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 49, new Guid("77777777-7777-7777-7777-777777777719"), "Exibir respostas do formulario na area administrativa de atendimento" },
                    { new Guid("78787878-7878-7878-7878-000000001050"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 9, true, 50, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar historico da abertura com formulario preenchido" },
                    { new Guid("78787878-7878-7878-7878-000000001051"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 9, true, 51, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar auditoria tecnica das respostas persistidas" },
                    { new Guid("78787878-7878-7878-7878-000000001052"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 52, new Guid("77777777-7777-7777-7777-777777777719"), "Testar persistencia das respostas do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001053"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 53, new Guid("77777777-7777-7777-7777-777777777719"), "Testar exibicao das respostas no portal" },
                    { new Guid("78787878-7878-7878-7878-000000001054"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 54, new Guid("77777777-7777-7777-7777-777777777719"), "Testar exibicao das respostas no atendimento administrativo" },
                    { new Guid("78787878-7878-7878-7878-000000001055"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 55, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir aplicacao de tipo, categoria, subcategoria e prioridade do catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001056"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 56, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir aplicacao de SLA padrao do catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001057"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 57, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir aplicacao de aprovacao por servico" },
                    { new Guid("78787878-7878-7878-7878-000000001058"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 58, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir compatibilidade com abertura legada sem catalogo" },
                    { new Guid("78787878-7878-7878-7878-000000001059"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 59, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir compatibilidade com incidentes" },
                    { new Guid("78787878-7878-7878-7878-000000001060"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 60, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir compatibilidade com aprovacao legada e motor novo" },
                    { new Guid("78787878-7878-7878-7878-000000001061"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 61, new Guid("77777777-7777-7777-7777-777777777719"), "Testar regressao de abertura guiada com SLA, grupo e aprovacao" },
                    { new Guid("78787878-7878-7878-7878-000000001062"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 62, new Guid("77777777-7777-7777-7777-777777777719"), "Testar regressao de abertura legada e incidente" },
                    { new Guid("78787878-7878-7878-7878-000000001063"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 7, true, 63, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir autorizacao para manutencao administrativa do formulario" },
                    { new Guid("78787878-7878-7878-7878-000000001064"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 7, true, 64, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir que solicitante nao manipule grupo, SLA, aprovacao ou classificacao" },
                    { new Guid("78787878-7878-7878-7878-000000001065"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 7, true, 65, new Guid("77777777-7777-7777-7777-777777777719"), "Garantir que solicitante so envie respostas permitidas para o servico" },
                    { new Guid("78787878-7878-7878-7878-000000001066"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 66, new Guid("77777777-7777-7777-7777-777777777719"), "Testar seguranca do formulario e respostas" },
                    { new Guid("78787878-7878-7878-7878-000000001067"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 4, true, 67, new Guid("77777777-7777-7777-7777-777777777719"), "Atualizar documentacao tecnica da Sprint 8" },
                    { new Guid("78787878-7878-7878-7878-000000001068"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 4, true, 68, new Guid("77777777-7777-7777-7777-777777777719"), "Atualizar docs/ROADMAP.md e docs/ROADMAP-ITSM.md" },
                    { new Guid("78787878-7878-7878-7878-000000001069"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 9, true, 69, new Guid("77777777-7777-7777-7777-777777777719"), "Atualizar SeedData e testes de checklist da Sprint 8" },
                    { new Guid("78787878-7878-7878-7878-000000001070"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 70, new Guid("77777777-7777-7777-7777-777777777719"), "Criar migration de checklist da Sprint 8" },
                    { new Guid("78787878-7878-7878-7878-000000001071"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 71, new Guid("77777777-7777-7777-7777-777777777719"), "Executar build backend e testes direcionados" },
                    { new Guid("78787878-7878-7878-7878-000000001072"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 72, new Guid("77777777-7777-7777-7777-777777777719"), "Executar build frontend e validacao TypeScript" },
                    { new Guid("78787878-7878-7878-7878-000000001073"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 73, new Guid("77777777-7777-7777-7777-777777777719"), "Verificar EF pending model changes" },
                    { new Guid("78787878-7878-7878-7878-000000001074"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 5, true, 74, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar homologacao funcional" },
                    { new Guid("78787878-7878-7878-7878-000000001075"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 5, true, 75, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar homologacao visual responsiva" },
                    { new Guid("78787878-7878-7878-7878-000000001076"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 5, true, 76, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar aceite formal somente com evidencia" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Concentrar as pendencias transferidas da Sprint 7 (grupo responsavel do catalogo, formulario dinamico por servico e respostas persistidas) sem quebrar abertura legada, incidentes ou aprovacao atual.", "O catalogo 2.0 deve permitir abertura guiada por servico com grupo tecnico opcional, formulario dinamico versionado, validacao backend das respostas, persistencia rastreavel no chamado e compatibilidade com fluxos legados, incidentes e aprovacao atual.", "CatalogoServico atual com departamento/categoria/subcategoria/prioridade/SLA/aprovacao; GET /api/portal/catalogo-servicos/{slug}/preparar-chamado; POST /api/portal/catalogo-servicos/requisicoes; NovoChamadoView estatico; testes cobrindo SLA, aprovacao e compatibilidade da abertura atual.", "Itens 10, 13 e 14 da Sprint 7 foram absorvidos pela Sprint 8 como pendencias estruturais rastreadas.", "Nao iniciar homologacao funcional/visual nem aceite formal antes de concluir grupo tecnico no catalogo, formulario dinamico, persistencia de respostas e testes de regressao da abertura guiada.", "Modelar GrupoTecnico opcional no CatalogoServico; criar formulario dinamico versionado por servico; validar e persistir respostas; expor endpoints administrativos e de abertura guiada; renderizar e enviar respostas no frontend; fechar rastreabilidade, seguranca e regressao.", 24, "Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel.", "A base do catalogo e da abertura guiada existe, com consulta no portal, abertura por servico e aplicacao backend de categoria, subcategoria, prioridade, SLA e aprovacao. Ainda nao existe grupo tecnico por servico, formulario dinamico, versionamento de campos ou persistencia estruturada das respostas.", 2, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001001"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001002"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001003"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001004"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001005"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001006"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001007"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001008"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001009"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001010"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001011"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001012"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001013"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001014"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001015"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001016"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001017"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001018"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001019"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001020"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001021"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001022"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001023"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001024"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001025"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001026"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001027"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001028"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001029"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001030"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001031"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001032"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001033"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001034"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001035"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001036"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001037"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001038"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001039"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001040"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001041"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001042"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001043"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001044"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001045"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001046"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001047"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001048"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001049"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001050"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001051"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001052"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001053"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001054"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001055"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001056"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001057"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001058"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001059"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001060"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001061"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001062"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001063"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001064"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001065"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001066"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001067"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001068"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001069"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001070"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001071"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001072"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001073"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001074"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001075"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000001076"));

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000113"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 1, true, 1, new Guid("77777777-7777-7777-7777-777777777719"), "Planejar escopo e criterios de aceite" },
                    { new Guid("78787878-7878-7878-7878-000000000114"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777719"), "Implementar entregas centrais da sprint" },
                    { new Guid("78787878-7878-7878-7878-000000000115"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 3, true, 3, new Guid("77777777-7777-7777-7777-777777777719"), "Executar testes funcionais e tecnicos" },
                    { new Guid("78787878-7878-7878-7878-000000000116"), true, null, null, false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 8 Catalogo de Servicos 2.0", 5, true, 4, new Guid("77777777-7777-7777-7777-777777777719"), "Registrar homologacao e aceite" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777719"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Adicionar tipo padrao, SLA padrao, grupo tecnico e formulario dinamico por servico.", "Selecionar servico deve sugerir/preencher tipo, categoria, SLA, prioridade, grupo e aprovacao.", "Catalogo atual reaproveitado como base da evolucao 2.0.", null, "Homologar abertura guiada com servicos reais e validacao de aprovacoes.", "Campos obrigatorios dinamicos, sugestoes automaticas e visibilidade por perfil refinada.", 90, "Evoluir entidade de catalogo e contrato de abertura guiada.", "Modulo de catalogo esta implementado funcionalmente, com evolucoes ITIL pendentes.", 3, 3 });
        }
    }
}
