using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoadmapComentariosAnexosChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("72727272-7272-7272-7272-000000000001"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 1, new Guid("77777777-7777-7777-7777-777777777707"), "Endpoint GET de comentarios criado." },
                    { new Guid("72727272-7272-7272-7272-000000000002"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 2, new Guid("77777777-7777-7777-7777-777777777707"), "Endpoint POST de comentarios criado." },
                    { new Guid("72727272-7272-7272-7272-000000000003"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 3, new Guid("77777777-7777-7777-7777-777777777707"), "Comentario publico permitido para Administrador, Atendente e Solicitante." },
                    { new Guid("72727272-7272-7272-7272-000000000004"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 4, new Guid("77777777-7777-7777-7777-777777777707"), "Comentario interno permitido somente para Administrador e Atendente." },
                    { new Guid("72727272-7272-7272-7272-000000000005"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 5, new Guid("77777777-7777-7777-7777-777777777707"), "Solicitante impedido de criar comentario interno." },
                    { new Guid("72727272-7272-7272-7272-000000000006"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777707"), "Solicitante impedido de visualizar comentario interno." },
                    { new Guid("72727272-7272-7272-7272-000000000007"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777707"), "Validacao de mensagem obrigatoria implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000008"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777707"), "Limite de 4000 caracteres implementado." },
                    { new Guid("72727272-7272-7272-7272-000000000009"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 9, new Guid("77777777-7777-7777-7777-777777777707"), "Ordenacao cronologica implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000010"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777707"), "Tela de detalhe do chamado atualizada com comentarios." },
                    { new Guid("72727272-7272-7272-7272-000000000011"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 3, true, 11, new Guid("77777777-7777-7777-7777-777777777707"), "Testes backend de comentarios aprovados." },
                    { new Guid("72727272-7272-7272-7272-000000000012"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 3, true, 12, new Guid("77777777-7777-7777-7777-777777777707"), "Testes frontend/build de comentarios aprovados." },
                    { new Guid("72727272-7272-7272-7272-000000000013"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 13, new Guid("77777777-7777-7777-7777-777777777707"), "Documentacao de comentarios atualizada." },
                    { new Guid("72727272-7272-7272-7272-000000000014"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 14, new Guid("77777777-7777-7777-7777-777777777707"), "Endpoint GET de anexos criado." },
                    { new Guid("72727272-7272-7272-7272-000000000015"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 15, new Guid("77777777-7777-7777-7777-777777777707"), "Endpoint POST de anexos criado." },
                    { new Guid("72727272-7272-7272-7272-000000000016"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 16, new Guid("77777777-7777-7777-7777-777777777707"), "Endpoint de download de anexo criado." },
                    { new Guid("72727272-7272-7272-7272-000000000017"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 17, new Guid("77777777-7777-7777-7777-777777777707"), "Upload de anexo por perfil implementado." },
                    { new Guid("72727272-7272-7272-7272-000000000018"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 18, new Guid("77777777-7777-7777-7777-777777777707"), "Listagem de anexo por perfil implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000019"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 19, new Guid("77777777-7777-7777-7777-777777777707"), "Download de anexo por perfil implementado." },
                    { new Guid("72727272-7272-7272-7272-000000000020"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 20, new Guid("77777777-7777-7777-7777-777777777707"), "Validacao de arquivo vazio implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000021"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 21, new Guid("77777777-7777-7777-7777-777777777707"), "Validacao de tamanho maximo implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000022"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 22, new Guid("77777777-7777-7777-7777-777777777707"), "Validacao de extensao permitida implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000023"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 23, new Guid("77777777-7777-7777-7777-777777777707"), "Bloqueio de extensoes perigosas implementado." },
                    { new Guid("72727272-7272-7272-7272-000000000024"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 24, new Guid("77777777-7777-7777-7777-777777777707"), "Storage seguro implementado." },
                    { new Guid("72727272-7272-7272-7272-000000000025"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 25, new Guid("77777777-7777-7777-7777-777777777707"), "Protecao contra path traversal implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000026"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 26, new Guid("77777777-7777-7777-7777-777777777707"), "API nao expoe caminho fisico." },
                    { new Guid("72727272-7272-7272-7272-000000000027"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 27, new Guid("77777777-7777-7777-7777-777777777707"), "API nao expoe nome fisico armazenado." },
                    { new Guid("72727272-7272-7272-7272-000000000028"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 28, new Guid("77777777-7777-7777-7777-777777777707"), "Nenhum endpoint DELETE de anexo foi criado." },
                    { new Guid("72727272-7272-7272-7272-000000000029"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 29, new Guid("77777777-7777-7777-7777-777777777707"), "Nenhum botao de exclusao de anexo foi criado." },
                    { new Guid("72727272-7272-7272-7272-000000000030"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 30, new Guid("77777777-7777-7777-7777-777777777707"), "Exclusao logica/fisica de anexos nao foi implementada." },
                    { new Guid("72727272-7272-7272-7272-000000000031"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 2, true, 31, new Guid("77777777-7777-7777-7777-777777777707"), "Tela de detalhe do chamado atualizada com anexos." },
                    { new Guid("72727272-7272-7272-7272-000000000032"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 3, true, 32, new Guid("77777777-7777-7777-7777-777777777707"), "Testes backend de anexos aprovados." },
                    { new Guid("72727272-7272-7272-7272-000000000033"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 3, true, 33, new Guid("77777777-7777-7777-7777-777777777707"), "Testes frontend/build de anexos aprovados." },
                    { new Guid("72727272-7272-7272-7272-000000000034"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 34, new Guid("77777777-7777-7777-7777-777777777707"), "Documentacao de anexos atualizada." },
                    { new Guid("72727272-7272-7272-7272-000000000035"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 35, new Guid("77777777-7777-7777-7777-777777777707"), "docs/ATENDIMENTO.md atualizado." },
                    { new Guid("72727272-7272-7272-7272-000000000036"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 36, new Guid("77777777-7777-7777-7777-777777777707"), "docs/ROADMAP.md atualizado." },
                    { new Guid("72727272-7272-7272-7272-000000000037"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 37, new Guid("77777777-7777-7777-7777-777777777707"), "docs/ROADMAP-ITSM.md atualizado." },
                    { new Guid("72727272-7272-7272-7272-000000000038"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 4, true, 38, new Guid("77777777-7777-7777-7777-777777777707"), "Migrations de comentarios e anexos registradas." },
                    { new Guid("72727272-7272-7272-7272-000000000039"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 3, true, 39, new Guid("77777777-7777-7777-7777-777777777707"), "Evidencias de testes registradas." },
                    { new Guid("72727272-7272-7272-7272-000000000040"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 1, true, 40, new Guid("77777777-7777-7777-7777-777777777707"), "Percentual atualizado para 100%." },
                    { new Guid("72727272-7272-7272-7272-000000000041"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 1, true, 41, new Guid("77777777-7777-7777-7777-777777777707"), "Status final ajustado para Implementado funcionalmente." },
                    { new Guid("72727272-7272-7272-7272-000000000042"), true, null, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Checklist de Comentarios e Anexos", 5, true, 42, new Guid("77777777-7777-7777-7777-777777777707"), "Avaliacao final ajustada para Aprovado." }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Comentarios internos restritos a Administrador/Atendente. Anexos devem permanecer como evidencia permanente, sem endpoint DELETE e sem exclusao logica/fisica.", "Atendimento", "Administrador, Atendente e Solicitante conseguem interagir no atendimento conforme regras de perfil; comentario interno fica restrito; anexos sao enviados/listados/baixados com seguranca; anexos nao podem ser excluidos por nenhum perfil apos upload.", 2, "- docs/ATENDIMENTO.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md\n- GET /api/chamados/{chamadoId}/comentarios\n- POST /api/chamados/{chamadoId}/comentarios\n- GET /api/chamados/{chamadoId}/anexos\n- POST /api/chamados/{chamadoId}/anexos\n- GET /api/chamados/{chamadoId}/anexos/{anexoId}/download\n- tests/SGX.SistemaChamado.Tests/ComentariosChamadoUseCasesTests.cs\n- tests/SGX.SistemaChamado.Tests/AnexosChamadoUseCasesTests.cs\n- src/SGX.SistemaChamado.Web/src/services/chamadosService.spec.ts\n- src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.anexos.spec.ts", "Permitir comentarios e anexos no atendimento com regras por perfil, seguranca no upload/download e rastreabilidade, mantendo anexos como evidencia permanente sem exclusao.", "Checklist consolidado em 100% para comentarios e anexos. Regra de negocio mantida: anexos salvos nao podem ser excluidos.", "Validar em ambiente de homologacao com usuarios reais, caso ainda nao exista validacao formal registrada.", "Nenhuma pendencia bloqueante.", 100, "Consolidar evidencias formais de homologacao com usuarios reais por perfil.", "Implementado", 1, 3, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000001"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000002"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000003"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000004"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000005"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000006"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000007"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000008"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000009"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000010"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000011"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000012"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000013"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000014"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000015"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000016"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000017"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000018"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000019"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000020"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000021"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000022"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000023"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000024"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000025"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000026"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000027"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000028"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000029"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000030"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000031"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000032"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000033"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000034"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000035"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000036"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000037"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000038"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000039"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000040"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000041"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("72727272-7272-7272-7272-000000000042"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Testar upload, download, visibilidade publica/interna", "Operacao", null, 4, null, null, null, null, null, 0, null, "Previsto", 2, 0, 0 });
        }
    }
}
