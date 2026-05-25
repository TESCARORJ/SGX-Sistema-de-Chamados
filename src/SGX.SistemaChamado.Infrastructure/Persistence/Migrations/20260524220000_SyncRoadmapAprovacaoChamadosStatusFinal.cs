using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SGXSistemaChamadoDbContext))]
    [Migration("20260524220000_SyncRoadmapAprovacaoChamadosStatusFinal")]
    public partial class SyncRoadmapAprovacaoChamadosStatusFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    v_item_canonico uuid;
                    v_categoria_atendimento constant uuid := '66666666-6666-6666-6666-666666666602';
                    v_item_preferencial constant uuid := '77777777-7777-7777-7777-777777777713';
                BEGIN
                    WITH candidatos AS (
                        SELECT
                            i.id,
                            i.criado_em,
                            regexp_replace(
                                lower(
                                    translate(
                                        coalesce(i.area, ''),
                                        'áàâãäåÁÀÂÃÄÅéèêëÉÈÊËíìîïÍÌÎÏóòôõöÓÒÔÕÖúùûüÚÙÛÜçÇñÑ',
                                        'aaaaaaAAAAAAeeeeEEEEiiiiIIIIoooooOOOOOuuuuUUUUcCnN'
                                    )
                                ),
                                '[^a-z0-9]+',
                                '',
                                'g'
                            ) AS area_norm,
                            regexp_replace(
                                lower(
                                    translate(
                                        coalesce(i.categoria, ''),
                                        'áàâãäåÁÀÂÃÄÅéèêëÉÈÊËíìîïÍÌÎÏóòôõöÓÒÔÕÖúùûüÚÙÛÜçÇñÑ',
                                        'aaaaaaAAAAAAeeeeEEEEiiiiIIIIoooooOOOOOuuuuUUUUcCnN'
                                    )
                                ),
                                '[^a-z0-9]+',
                                '',
                                'g'
                            ) AS categoria_norm
                        FROM roadmap_itsm_itens i
                    )
                    SELECT c.id
                    INTO v_item_canonico
                    FROM candidatos c
                    WHERE c.area_norm IN ('aprovacaodechamados', 'aprovacaochamados', 'aprovacaochamado')
                    ORDER BY
                        CASE WHEN c.id = v_item_preferencial THEN 0 ELSE 1 END,
                        c.criado_em NULLS LAST,
                        c.id
                    LIMIT 1;

                    IF v_item_canonico IS NULL THEN
                        IF EXISTS (SELECT 1 FROM roadmap_itsm_itens WHERE id = v_item_preferencial) THEN
                            v_item_canonico := v_item_preferencial;
                        ELSE
                            RETURN;
                        END IF;
                    END IF;

                    UPDATE roadmap_itsm_itens
                    SET
                        area = 'Aprovacao de chamados',
                        categoria = 'Atendimento',
                        roadmap_categoria_id = v_categoria_atendimento,
                        situacao_atual = 'Aprovacao de chamados implementada funcionalmente. O modulo contempla fundacao tecnica, backend administrativo, aprovacao manual, aprovacao automatica por Catalogo de Servicos, bloqueios operacionais para chamados pendentes ou reprovados, frontend administrativo, acompanhamento no portal do solicitante, historico do chamado, auditoria, permissoes, testes backend/frontend e documentacao. Homologacao funcional preparada com checklist e estrutura de evidencias. Pendem homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras.',
                        atencao_tecnica = 'Manter governanca do fluxo de aprovacao sem relaxar validacoes de backend, trilha de historico e auditoria, e controle de acesso por permissao nos endpoints administrativos e de portal.',
                        status = 2,
                        prioridade = 2,
                        impacto = 1,
                        decisao = 4,
                        status_implementacao = 3,
                        status_tecnico = 4,
                        percentual_implementacao = 90,
                        pendencias_tecnicas = '- Testes E2E completos quando houver framework institucional.\n- Evolucoes futuras: multiplos niveis de aprovacao, alcadas, delegacao, notificacoes avancadas e relatorios.',
                        pendencias_homologacao = '- Executar homologacao institucional com usuarios reais.\n- Coletar evidencias com prints reais e registrar aceite funcional.',
                        evidencia_implementacao = '- docs/APROVACAO-CHAMADOS.md\n- docs/CHECKLIST-HOMOLOGACAO-APROVACAO-CHAMADOS.md\n- docs/evidencias/aprovacao-chamados/README.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md',
                        criterio_aceite = 'A tela do roadmap deve exibir um unico item de Aprovacao de chamados com categoria Atendimento, status de implementacao Implementado funcionalmente, status tecnico Homologacao funcional preparada e percentual 90.',
                        proxima_acao = 'Executar homologacao institucional com usuarios reais e anexar evidencias formais da validacao funcional.',
                        observacao = 'Sprints 1 a 6 concluidas com fechamento funcional e preparacao de homologacao.',
                        data_conclusao_tecnica = COALESCE(data_conclusao_tecnica, NOW()),
                        atualizado_em = NOW(),
                        atualizado_por = 'migration.20260524220000'
                    WHERE id = v_item_canonico;

                    UPDATE roadmap_itsm_itens i
                    SET
                        ativo = FALSE,
                        atualizado_em = NOW(),
                        atualizado_por = 'migration.20260524220000'
                    WHERE i.id <> v_item_canonico
                      AND i.ativo = TRUE
                      AND regexp_replace(
                            lower(
                                translate(
                                    coalesce(i.area, ''),
                                    'áàâãäåÁÀÂÃÄÅéèêëÉÈÊËíìîïÍÌÎÏóòôõöÓÒÔÕÖúùûüÚÙÛÜçÇñÑ',
                                    'aaaaaaAAAAAAeeeeEEEEiiiiIIIIoooooOOOOOuuuuUUUUcCnN'
                                )
                            ),
                            '[^a-z0-9]+',
                            '',
                            'g'
                        ) IN ('aprovacaodechamados', 'aprovacaochamados', 'aprovacaochamado');
                END $$;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE roadmap_itsm_itens
                SET
                    area = 'Aprovacao de chamados',
                    categoria = 'Atendimento',
                    situacao_atual = 'Fundacao tecnica da aprovacao de chamados implementada com modelagem inicial, persistencia, permissoes e historico preparados para evolucao das proximas sprints.',
                    atencao_tecnica = 'A aprovacao deve permanecer institucional e multiarea, sem restricao ao contexto de TI, preservando rastreabilidade de solicitacao/decisao e sem exclusao fisica de aprovacoes.',
                    status = 3,
                    impacto = 2,
                    decisao = 1,
                    status_implementacao = 2,
                    status_tecnico = 2,
                    percentual_implementacao = 20,
                    pendencias_tecnicas = '- Implementar regra de aplicacao automatica de aprovacao por catalogo/categoria/departamento.\n- Implementar casos de uso e endpoints administrativos/operacionais de solicitacao, aprovacao, reprovacao e cancelamento.\n- Integrar fluxo de aprovacao ao ciclo de atendimento sem alterar de forma destrutiva o status principal do chamado.',
                    pendencias_homologacao = '- Validar fluxo multiarea com departamentos institucionais (TI, RH, Financeiro, Patrimonio, Compras e Juridico).\n- Coletar evidencias formais de homologacao da trilha de aprovacao.',
                    evidencia_implementacao = '- docs/APROVACAO-CHAMADOS.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md',
                    criterio_aceite = 'Fundacao tecnica do modulo de aprovacao implementada com entidade, enums, migration, permissoes, seed e historico preparados para evolucao funcional.',
                    proxima_acao = 'Iniciar Sprint 2 com casos de uso e endpoints para orquestrar a aprovacao antes do atendimento quando aplicavel.',
                    observacao = 'Sprint 1 concluida com foco em base tecnica e preservacao de historico.',
                    atualizado_em = NOW(),
                    atualizado_por = 'migration.20260524220000.down'
                WHERE id = '77777777-7777-7777-7777-777777777713';
                """);
        }
    }
}
