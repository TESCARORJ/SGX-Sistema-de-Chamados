using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SGX.SistemaChamado.Infrastructure.Persistence;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(SGXSistemaChamadoDbContext))]
    [Migration("20260523235900_ConsolidarRoadmapCatalogoServicosSprint6")]
    public partial class ConsolidarRoadmapCatalogoServicosSprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    v_keeper uuid;
                    v_categoria_conhecimento uuid := '66666666-6666-6666-6666-666666666614';
                    v_usuario_atualizacao text := 'migration.catalogo-servicos.sprint6';
                BEGIN
                    SELECT id
                    INTO v_keeper
                    FROM roadmap_itsm_itens
                    WHERE id = '77777777-7777-7777-7777-777777777712';

                    IF v_keeper IS NULL THEN
                        SELECT id
                        INTO v_keeper
                        FROM roadmap_itsm_itens
                        WHERE translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc') = 'catalogo de servicos'
                        ORDER BY
                            CASE WHEN status_implementacao = 3 THEN 0 ELSE 1 END,
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
                          WHERE translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc') = 'catalogo de servicos'
                      );

                    UPDATE roadmap_implementacoes_futuras
                    SET roadmap_item_id = v_keeper
                    WHERE roadmap_item_id <> v_keeper
                      AND roadmap_item_id IN (
                          SELECT id
                          FROM roadmap_itsm_itens
                          WHERE translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc') = 'catalogo de servicos'
                      );

                    UPDATE roadmap_itsm_itens
                    SET
                        area = 'Catalogo de Servicos',
                        categoria = 'Conhecimento',
                        roadmap_categoria_id = v_categoria_conhecimento,
                        situacao_atual = 'Catalogo de Servicos implementado funcionalmente como modulo institucional multiarea. O modulo contempla fundacao tecnica, CRUD administrativo, frontend administrativo, consulta no portal, frontend do portal, controle de permissoes, visibilidade por perfil, integracao com abertura de chamados, associacao CatalogoServicoId ao chamado, aplicacao backend dos dados oficiais do servico, historico de abertura por catalogo, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras.',
                        atencao_tecnica = 'Manter consolidacao de um unico item canonico do Catalogo de Servicos no roadmap e preservar as regras de seguranca backend do modulo sem relaxamento.',
                        status = 2,
                        prioridade = 2,
                        impacto = 1,
                        decisao = 4,
                        status_implementacao = 3,
                        status_tecnico = 4,
                        percentual_implementacao = 90,
                        pendencias_tecnicas = '- Testes E2E completos.\n- Evolucoes futuras: formularios dinamicos por servico, campos obrigatorios por servico, workflow de aprovacao por servico e melhorias de indicadores/relatorios.',
                        pendencias_homologacao = '- Homologacao institucional com usuarios reais.\n- Evidencias formais com prints reais.',
                        evidencia_implementacao = '- docs/CATALOGO-SERVICOS.md\n- docs/CHECKLIST-HOMOLOGACAO-CATALOGO-SERVICOS.md\n- docs/evidencias/catalogo-servicos/README.md',
                        criterio_aceite = 'A tela do roadmap deve exibir um unico item de Catalogo de Servicos com categoria Conhecimento, status de implementacao Implementado funcionalmente, status tecnico Homologacao funcional preparada e percentual 90.',
                        proxima_acao = 'Executar homologacao institucional com usuarios reais e anexar evidencias formais.',
                        observacao = 'Sprint 6 consolidada com checklist e estrutura de evidencias preparados.',
                        atualizado_em = NOW(),
                        atualizado_por = v_usuario_atualizacao
                    WHERE id = v_keeper;

                    DELETE FROM roadmap_itsm_itens
                    WHERE id <> v_keeper
                      AND translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc') = 'catalogo de servicos';
                END $$;
                """);

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS ux_roadmap_catalogo_servicos_unico
                ON roadmap_itsm_itens ((translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc')))
                WHERE translate(lower(trim(area)), 'áàãâäéèêëíìîïóòõôöúùûüç', 'aaaaaeeeeiiiiooooouuuuc') = 'catalogo de servicos';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS ux_roadmap_catalogo_servicos_unico;");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                columns: new[]
                {
                    "area",
                    "categoria",
                    "situacao_atual",
                    "atencao_tecnica",
                    "status",
                    "prioridade",
                    "impacto",
                    "decisao",
                    "status_implementacao",
                    "status_tecnico",
                    "percentual_implementacao",
                    "pendencias_tecnicas",
                    "pendencias_homologacao",
                    "evidencia_implementacao",
                    "criterio_aceite",
                    "proxima_acao",
                    "observacao"
                },
                values: new object[]
                {
                    "Catalogo de servicos",
                    "Conhecimento",
                    "Fundacao tecnica implementada",
                    "Catalogo institucional e multiarea com departamento responsavel obrigatorio e relacionamentos opcionais para nao bloquear evolucao",
                    3,
                    2,
                    1,
                    2,
                    3,
                    4,
                    20,
                    "Implementar CRUD administrativo, publicacao/arquivamento operacional e regras de exposicao para solicitantes",
                    "Validar fluxos com areas nao-TI apos entrega funcional das proximas sprints",
                    "docs/CATALOGO-SERVICOS.md; src/SGX.SistemaChamado.Domain/Entities/CatalogoServico.cs",
                    "Fundacao tecnica do catalogo implementada com entidade, enums, mapeamento EF Core, migration, permissoes e documentacao inicial",
                    "Implementar sprint 2 com casos de uso e endpoints administrativos",
                    null
                });
        }
    }
}
