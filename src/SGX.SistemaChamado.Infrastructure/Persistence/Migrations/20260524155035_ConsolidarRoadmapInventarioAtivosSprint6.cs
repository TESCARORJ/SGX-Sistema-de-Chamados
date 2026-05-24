using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidarRoadmapInventarioAtivosSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "impacto", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "prioridade", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Inventario/Ativos", "Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras planejadas.", "Infraestrutura", "A tela do roadmap deve exibir um unico item de Inventario/Ativos com categoria Infraestrutura, status da implementacao Implementado funcionalmente, status tecnico Homologacao funcional preparada e percentual 90.", 4, "- docs/INVENTARIO-ATIVOS.md\n- docs/CHECKLIST-HOMOLOGACAO-INVENTARIO-ATIVOS.md\n- docs/evidencias/inventario-ativos/README.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md", 1, "Sprints 1 a 6 concluidas tecnicamente; item consolidado para evitar duplicidades por variacao de nome.", "- Coletar evidencias com prints reais.\n- Registrar aceite funcional institucional.", "- Homologacao institucional com usuarios reais.\n- Testes E2E completos.\n- Evolucoes futuras: importacao em massa, exportacao, QR Code, etiquetas patrimoniais, anexos, alertas de garantia, manutencao preventiva e indicadores por ativo.", 90, 2, "Executar homologacao institucional com usuarios reais e anexar evidencias formais.", "Inventario/Ativos implementado funcionalmente como modulo de infraestrutura. O modulo contempla cadastro de ativos, tipos de ativo, inativacao logica, validacoes de codigo/patrimonio/serie, filtros administrativos, auditoria, historico operacional, movimentacao, vinculo com chamados, consulta de chamados relacionados, frontend administrativo, integracao visual com detalhe administrativo do chamado, testes backend/frontend e documentacao.", 2, 3, 4 });

            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    v_keeper uuid;
                    v_categoria_infra uuid := '66666666-6666-6666-6666-666666666607';
                    v_usuario_atualizacao text := 'migration.inventario-ativos.sprint6';
                BEGIN
                    SELECT id
                    INTO v_keeper
                    FROM roadmap_itsm_itens
                    WHERE id = '77777777-7777-7777-7777-777777777711';

                    IF v_keeper IS NULL THEN
                        SELECT id
                        INTO v_keeper
                        FROM roadmap_itsm_itens
                        WHERE (
                                regexp_replace(
                                    translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                    '[^a-z]',
                                    '',
                                    'g'
                                ) IN ('inventarioativos', 'inventariodeativos')
                                OR (
                                    regexp_replace(
                                        translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                        '[^a-z]',
                                        '',
                                        'g'
                                    ) IN ('inventario', 'ativos')
                                    AND (
                                        regexp_replace(
                                            translate(lower(trim(COALESCE(categoria, ''))), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                            '[^a-z]',
                                            '',
                                            'g'
                                        ) IN ('infraestrutura', 'ativos')
                                        OR roadmap_categoria_id = v_categoria_infra
                                    )
                                )
                            )
                        ORDER BY
                            CASE WHEN id = '77777777-7777-7777-7777-777777777711' THEN 0 ELSE 1 END,
                            CASE WHEN status_implementacao = 3 THEN 0 ELSE 1 END,
                            CASE WHEN status_tecnico = 4 THEN 0 ELSE 1 END,
                            percentual_implementacao DESC,
                            ordem ASC,
                            criado_em ASC
                        LIMIT 1;
                    END IF;

                    IF v_keeper IS NULL THEN
                        RETURN;
                    END IF;

                    UPDATE roadmap_checklist_itens
                    SET roadmap_item_id = v_keeper
                    WHERE roadmap_item_id <> v_keeper
                      AND roadmap_item_id IN (
                          SELECT id
                          FROM roadmap_itsm_itens
                          WHERE (
                                  regexp_replace(
                                      translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                      '[^a-z]',
                                      '',
                                      'g'
                                  ) IN ('inventarioativos', 'inventariodeativos')
                                  OR (
                                      regexp_replace(
                                          translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                          '[^a-z]',
                                          '',
                                          'g'
                                      ) IN ('inventario', 'ativos')
                                      AND (
                                          regexp_replace(
                                              translate(lower(trim(COALESCE(categoria, ''))), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                              '[^a-z]',
                                              '',
                                              'g'
                                          ) IN ('infraestrutura', 'ativos')
                                          OR roadmap_categoria_id = v_categoria_infra
                                      )
                                  )
                              )
                      );

                    UPDATE roadmap_implementacoes_futuras
                    SET roadmap_item_id = v_keeper
                    WHERE roadmap_item_id <> v_keeper
                      AND roadmap_item_id IN (
                          SELECT id
                          FROM roadmap_itsm_itens
                          WHERE (
                                  regexp_replace(
                                      translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                      '[^a-z]',
                                      '',
                                      'g'
                                  ) IN ('inventarioativos', 'inventariodeativos')
                                  OR (
                                      regexp_replace(
                                          translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                          '[^a-z]',
                                          '',
                                          'g'
                                      ) IN ('inventario', 'ativos')
                                      AND (
                                          regexp_replace(
                                              translate(lower(trim(COALESCE(categoria, ''))), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                              '[^a-z]',
                                              '',
                                              'g'
                                          ) IN ('infraestrutura', 'ativos')
                                          OR roadmap_categoria_id = v_categoria_infra
                                      )
                                  )
                              )
                      );

                    UPDATE roadmap_itsm_itens
                    SET
                        area = 'Inventario/Ativos',
                        categoria = 'Infraestrutura',
                        roadmap_categoria_id = v_categoria_infra,
                        situacao_atual = 'Inventario/Ativos implementado funcionalmente como modulo de infraestrutura. O modulo contempla cadastro de ativos, tipos de ativo, inativacao logica, validacoes de codigo/patrimonio/serie, filtros administrativos, auditoria, historico operacional, movimentacao, vinculo com chamados, consulta de chamados relacionados, frontend administrativo, integracao visual com detalhe administrativo do chamado, testes backend/frontend e documentacao.',
                        atencao_tecnica = 'Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras planejadas.',
                        status = 2,
                        prioridade = 2,
                        impacto = 1,
                        decisao = 4,
                        status_implementacao = 3,
                        status_tecnico = 4,
                        percentual_implementacao = 90,
                        pendencias_tecnicas = '- Homologacao institucional com usuarios reais.\n- Testes E2E completos.\n- Evolucoes futuras: importacao em massa, exportacao, QR Code, etiquetas patrimoniais, anexos, alertas de garantia, manutencao preventiva e indicadores por ativo.',
                        pendencias_homologacao = '- Coletar evidencias com prints reais.\n- Registrar aceite funcional institucional.',
                        evidencia_implementacao = '- docs/INVENTARIO-ATIVOS.md\n- docs/CHECKLIST-HOMOLOGACAO-INVENTARIO-ATIVOS.md\n- docs/evidencias/inventario-ativos/README.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md',
                        criterio_aceite = 'A tela do roadmap deve exibir um unico item de Inventario/Ativos com categoria Infraestrutura, status da implementacao Implementado funcionalmente, status tecnico Homologacao funcional preparada e percentual 90.',
                        proxima_acao = 'Executar homologacao institucional com usuarios reais e anexar evidencias formais.',
                        observacao = 'Sprints 1 a 6 concluidas tecnicamente; item consolidado para evitar duplicidades por variacao de nome.',
                        ativo = true,
                        atualizado_em = NOW(),
                        atualizado_por = v_usuario_atualizacao
                    WHERE id = v_keeper;

                    UPDATE roadmap_itsm_itens
                    SET
                        ativo = false,
                        atualizado_em = NOW(),
                        atualizado_por = v_usuario_atualizacao
                    WHERE id <> v_keeper
                      AND ativo = true
                      AND (
                              regexp_replace(
                                  translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                  '[^a-z]',
                                  '',
                                  'g'
                              ) IN ('inventarioativos', 'inventariodeativos')
                              OR (
                                  regexp_replace(
                                      translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                      '[^a-z]',
                                      '',
                                      'g'
                                  ) IN ('inventario', 'ativos')
                                  AND (
                                      regexp_replace(
                                          translate(lower(trim(COALESCE(categoria, ''))), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                          '[^a-z]',
                                          '',
                                          'g'
                                      ) IN ('infraestrutura', 'ativos')
                                      OR roadmap_categoria_id = v_categoria_infra
                                  )
                              )
                          );
                END $$;
                """);

            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS ux_roadmap_inventario_ativos_unico;

                CREATE UNIQUE INDEX IF NOT EXISTS ux_roadmap_inventario_ativos_unico
                ON roadmap_itsm_itens ((1))
                WHERE ativo = true
                  AND (
                        regexp_replace(
                            translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                            '[^a-z]',
                            '',
                            'g'
                        ) IN ('inventarioativos', 'inventariodeativos')
                        OR (
                            regexp_replace(
                                translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                '[^a-z]',
                                '',
                                'g'
                            ) IN ('inventario', 'ativos')
                            AND (
                                regexp_replace(
                                    translate(lower(trim(COALESCE(categoria, ''))), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc'),
                                    '[^a-z]',
                                    '',
                                    'g'
                                ) IN ('infraestrutura', 'ativos')
                                OR roadmap_categoria_id = '66666666-6666-6666-6666-666666666607'
                            )
                        )
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ux_roadmap_inventario_ativos_unico;");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "decisao", "evidencia_implementacao", "impacto", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "prioridade", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Inventario/ativos", "Nao prometer equivalencia com GLPI nesse ponto", "Ativos", null, 2, null, 2, null, null, null, 0, 3, null, "Nao ha evidencia forte", 4, 0, 0 });
        }
    }
}
