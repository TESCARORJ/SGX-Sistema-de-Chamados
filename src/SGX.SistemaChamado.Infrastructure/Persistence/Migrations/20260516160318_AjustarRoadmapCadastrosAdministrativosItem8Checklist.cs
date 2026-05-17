using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjustarRoadmapCadastrosAdministrativosItem8Checklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    v_item_id uuid := '77777777-7777-7777-7777-777777777708';
                    v_id uuid;
                BEGIN
                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 1
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000001', v_item_id, 'Criar documentação ITSM.', 'Checklist de Cadastros Administrativos', 4, 1, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Criar documentação ITSM.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 4,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 2
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000002', v_item_id, 'Criar checklist de homologação.', 'Checklist de Cadastros Administrativos', 4, 2, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Criar checklist de homologação.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 4,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 3
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000003', v_item_id, 'Implementar backend dos cadastros.', 'Checklist de Cadastros Administrativos', 2, 3, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Implementar backend dos cadastros.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 2,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 4
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000004', v_item_id, 'Implementar frontend administrativo.', 'Checklist de Cadastros Administrativos', 2, 4, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Implementar frontend administrativo.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 2,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 5
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000005', v_item_id, 'Integrar cadastros com abertura de chamados.', 'Checklist de Cadastros Administrativos', 2, 5, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Integrar cadastros com abertura de chamados.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 2,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 6
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000006', v_item_id, 'Criar seed inicial.', 'Checklist de Cadastros Administrativos', 2, 6, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Criar seed inicial.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 2,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 7
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000007', v_item_id, 'Validar fluxo funcional.', 'Checklist de Cadastros Administrativos', 3, 7, true, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Validar fluxo funcional.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 3,
                            concluido = true,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;

                    SELECT id INTO v_id
                    FROM roadmap_checklist_itens
                    WHERE roadmap_item_id = v_item_id AND ordem = 8
                    ORDER BY criado_em
                    LIMIT 1;
                    IF v_id IS NULL THEN
                        INSERT INTO roadmap_checklist_itens (id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por)
                        VALUES ('73727272-7272-7272-7272-000000000008', v_item_id, 'Homologar em ambiente institucional.', 'Checklist de Cadastros Administrativos', 5, 8, false, true, true, '2026-01-01 00:00:00+00', 'seed.sistema');
                    ELSE
                        UPDATE roadmap_checklist_itens
                        SET titulo = 'Homologar em ambiente institucional.',
                            descricao = 'Checklist de Cadastros Administrativos',
                            grupo = 5,
                            concluido = false,
                            obrigatorio = true,
                            ativo = true
                        WHERE id = v_id;
                    END IF;
                END $$;
                """);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Verificar se todos os cadastros permitirao ativacao/inativacao sem exclusao fisica, evitando perda de historico em chamados antigos. Priorizar inativacao logica, validacao de duplicidade, uso apenas de registros ativos em novas operacoes e preservacao historica.", "Cadastros", "- Documentacao ITSM criada.\n- Checklist de homologacao criado.\n- Backend dos cadastros implementado e validado.\n- Frontend administrativo implementado e validado.\n- Cadastros integrados ao fluxo de abertura e gestao de chamados.\n- Seed inicial criado e validado.\n- Fluxo funcional validado tecnicamente.\n- Registros ativos usados em novas operacoes.\n- Registros inativos preservados para historico.\n- Homologacao institucional pendente como aceite formal final.", "Documentacao criada:\n- docs/ITSM-CADASTROS-ADMINISTRATIVOS.md\n- docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md\n\nDocumentacao atualizada:\n- docs/CADASTROS-ADMINISTRATIVOS.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md\n\nValidacoes tecnicas:\n- Backend dos cadastros implementado e validado.\n- Frontend administrativo implementado e validado.\n- Integracao com abertura e gestao de chamados validada.\n- Seed inicial validado.\n- Fluxo funcional validado.\n- dotnet build OK.\n- dotnet test OK com 420 testes aprovados.\n- npm build OK.", "Checklist ativo consolidado em 7/8; homologacao institucional permanece pendente.", "- Executar homologacao institucional/manual.\n- Coletar evidencias formais de tela.\n- Registrar responsavel pela homologacao.\n- Registrar data da homologacao.\n- Registrar ambiente utilizado.\n- Registrar resultado final: aprovado, aprovado com ressalvas ou reprovado.", "- Nao ha pendencias tecnicas bloqueantes identificadas para o modulo.\n- Manter como evolucao futura a cobertura frontend E2E completa.\n- Avaliar futuramente se status de chamado continuara como fluxo controlado ou se sera parametrizado em cadastro proprio.", 90, "Executar homologacao institucional/manual com evidencias formais, incluindo prints das telas administrativas, abertura de chamado com cadastros, detalhe do chamado, filtros administrativos, responsavel, data, ambiente e resultado da validacao.", "Modulo de Cadastros Administrativos implementado e validado funcionalmente em nivel tecnico. Backend, frontend administrativo, integracao com abertura/gestao de chamados, seed inicial e validacao funcional foram concluidos. A homologacao institucional/manual com evidencias formais permanece pendente.", 3, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000001"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000002"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000003"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000004"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000005"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000006"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000007"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000008"));

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "evidencia_implementacao", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao", "status_tecnico" },
                values: new object[] { "Verificar se permitem inativacao e parametrizacao", "Administracao", null, null, null, null, null, 0, null, "Categorias, prioridades, status e departamentos", 0, 0 });
        }
    }
}
