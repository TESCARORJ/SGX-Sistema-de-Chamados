using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations;

[DbContext(typeof(SGXSistemaChamadoDbContext))]
[Migration("20260514235959_ConsolidarRoadmapHistoricoAuditoria")]
public partial class ConsolidarRoadmapHistoricoAuditoria : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var checklist = ObterChecklist();

        var checklistValuesSql = string.Join(",\n", checklist.Select(c =>
            $"('{c.Id}'::uuid, {c.Ordem}, '{EscapeSql(c.Titulo)}', '{EscapeSql(c.Descricao)}', {c.Grupo}, {(c.Concluido ? "TRUE" : "FALSE")})"));

        var checklistIdsSql = string.Join(",\n", checklist.Select(c =>
            $"('{c.Id}'::uuid, {c.Ordem})"));

        var sql = $@"
DO $$
DECLARE
    v_seed_id uuid := '77777777-7777-7777-7777-777777777706';
    v_categoria_id uuid := '66666666-6666-6666-6666-666666666613';
    v_canonical_id uuid;
BEGIN
    SELECT id INTO v_canonical_id
      FROM roadmap_itsm_itens
     WHERE id = v_seed_id;

    IF v_canonical_id IS NULL THEN
        SELECT i.id INTO v_canonical_id
          FROM roadmap_itsm_itens i
         WHERE replace(replace(replace(lower(translate(coalesce(i.area, ''), 'ÁÀÂÃÄáàâãäÉÈÊËéèêëÍÌÎÏíìîïÓÒÔÕÖóòôõöÚÙÛÜúùûüÇç', 'AAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUUuuuuCc')), ' ', ''), '/', ''), '-', '') = 'historicoauditoria'
         ORDER BY CASE WHEN i.ativo THEN 0 ELSE 1 END, i.ordem, i.id
         LIMIT 1;
    END IF;

    IF v_canonical_id IS NULL THEN
        INSERT INTO roadmap_itsm_itens
        (
            id, area, categoria, objetivo, roadmap_categoria_id, situacao_atual, atencao_tecnica,
            status, prioridade, impacto, decisao, status_implementacao, status_tecnico, percentual_implementacao,
            pendencias_tecnicas, pendencias_homologacao, evidencia_implementacao,
            data_conclusao_tecnica, data_homologacao, criterio_aceite, proxima_acao, observacao,
            responsavel, prazo_alvo, ordem, ativo, criado_em, criado_por, atualizado_em, atualizado_por
        )
        VALUES
        (
            v_seed_id,
            'Histórico/Auditoria',
            'Governança',
            'Criar trilha de auditoria para registrar ações relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governança, análise de alterações, auditoria operacional e apoio à homologação.',
            v_categoria_id,
            'Base técnica de auditoria criada, eventos de auditoria aplicados aos módulos críticos e tela administrativa de consulta implementada em Admin > Governança > Auditoria, com filtros, detalhe, indicadores e documentação em Gestão ITSM.',
            'Auditoria não é log técnico. ILogger continua para diagnóstico técnico; EventoAuditoria é governança/rastreabilidade. Não registrar senha, token JWT, refresh token, access token, client secret ou connection string. Dados sensíveis devem ser mascarados pelo AuditoriaDiffHelper.',
            3, 1, 1, 4, 3, 3, 0,
            '- Exportação Excel/PDF; Retenção configurável de auditoria; Assinatura/hash da trilha de auditoria; Alertas para eventos críticos; Painel avançado de segurança; Integração SIEM/Log Analytics; Política de anonimização/LGPD para eventos antigos; Fluxo backend controlado de logout, se vier a existir; Auditoria de edição de documentação ITSM, caso a documentação deixe de ser estática.',
            '- Executar homologação funcional com eventos reais em eventos_auditoria cobrindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM; Validar filtros e consulta administrativa em Admin > Governança > Auditoria com evidências formais.',
            '- src/SGX.SistemaChamado.Domain/Entities/EventoAuditoria.cs; - src/SGX.SistemaChamado.Domain/Enums/TipoAcaoAuditoria.cs; - src/SGX.SistemaChamado.Domain/Enums/NivelAuditoria.cs; - src/SGX.SistemaChamado.Application/Interfaces/Auditoria/IAuditoriaService.cs; - src/SGX.SistemaChamado.Application/Interfaces/Auditoria/IAuditoriaContextProvider.cs; - src/SGX.SistemaChamado.Infrastructure/Services/AuditoriaService.cs; - src/SGX.SistemaChamado.Api/Services/AuditoriaContextProvider.cs; - src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/EventoAuditoriaConfiguration.cs; - src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260514215515_AddEventosAuditoriaGovernancaSprint1.cs; - src/SGX.SistemaChamado.Application/Helpers/AuditoriaDiffHelper.cs; - src/SGX.SistemaChamado.Api/Controllers/AdminAuditoriaController.cs; - src/SGX.SistemaChamado.Web/src/views/AuditoriaAdminView.vue; - src/SGX.SistemaChamado.Web/src/services/auditoriaService.ts; - src/SGX.SistemaChamado.Web/src/types/auditoria.ts; - src/SGX.SistemaChamado.Web/src/content/gestaoItsmDocs.ts; - docs/ROADMAP.md; - docs/ROADMAP-ITSM.md; - README.md; - tests/SGX.SistemaChamado.Tests/AuditoriaServiceTests.cs; - tests/SGX.SistemaChamado.Tests/AuditoriaDiffHelperTests.cs; - tests/SGX.SistemaChamado.Tests/AuditoriaModulosCriticosTests.cs',
            NULL, NULL,
            'O item Histórico/Auditoria deve exibir checklist ativo completo das Sprints 1, 2 e 3 com cálculo automático de percentual por checklist, status da implementação Implementado funcionalmente e status técnico Completo com pendências evolutivas, sem uso de percentual legado/manual.',
            'Executar homologação funcional com eventos reais em eventos_auditoria, incluindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM. Validar filtros e consulta administrativa em Admin > Governança > Auditoria.',
            'Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.',
            NULL, NULL, 6, TRUE, TIMESTAMPTZ '2026-01-01 00:00:00Z', 'seed.sistema', NULL, NULL
        );

        v_canonical_id := v_seed_id;
    END IF;

    WITH itens_auditoria AS
    (
        SELECT i.id
          FROM roadmap_itsm_itens i
         WHERE replace(replace(replace(lower(translate(coalesce(i.area, ''), 'ÁÀÂÃÄáàâãäÉÈÊËéèêëÍÌÎÏíìîïÓÒÔÕÖóòôõöÚÙÛÜúùûüÇç', 'AAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUUuuuuCc')), ' ', ''), '/', ''), '-', '') = 'historicoauditoria'
    )
    UPDATE roadmap_checklist_itens c
       SET roadmap_item_id = v_canonical_id,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
      FROM itens_auditoria i
     WHERE c.roadmap_item_id = i.id
       AND c.roadmap_item_id <> v_canonical_id;

    WITH itens_auditoria AS
    (
        SELECT i.id
          FROM roadmap_itsm_itens i
         WHERE replace(replace(replace(lower(translate(coalesce(i.area, ''), 'ÁÀÂÃÄáàâãäÉÈÊËéèêëÍÌÎÏíìîïÓÒÔÕÖóòôõöÚÙÛÜúùûüÇç', 'AAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUUuuuuCc')), ' ', ''), '/', ''), '-', '') = 'historicoauditoria'
    )
    UPDATE roadmap_implementacoes_futuras f
       SET roadmap_item_id = v_canonical_id,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
      FROM itens_auditoria i
     WHERE f.roadmap_item_id = i.id
       AND f.roadmap_item_id <> v_canonical_id;

    UPDATE roadmap_itsm_itens
       SET area = 'Histórico/Auditoria',
           categoria = 'Governança',
           roadmap_categoria_id = v_categoria_id,
           objetivo = 'Criar trilha de auditoria para registrar ações relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governança, análise de alterações, auditoria operacional e apoio à homologação.',
           situacao_atual = 'Base técnica de auditoria criada, eventos de auditoria aplicados aos módulos críticos e tela administrativa de consulta implementada em Admin > Governança > Auditoria, com filtros, detalhe, indicadores e documentação em Gestão ITSM.',
           atencao_tecnica = 'Auditoria não é log técnico. ILogger continua para diagnóstico técnico; EventoAuditoria é governança/rastreabilidade. Não registrar senha, token JWT, refresh token, access token, client secret ou connection string. Dados sensíveis devem ser mascarados pelo AuditoriaDiffHelper.',
           pendencias_tecnicas = '- Exportação Excel/PDF; Retenção configurável de auditoria; Assinatura/hash da trilha de auditoria; Alertas para eventos críticos; Painel avançado de segurança; Integração SIEM/Log Analytics; Política de anonimização/LGPD para eventos antigos; Fluxo backend controlado de logout, se vier a existir; Auditoria de edição de documentação ITSM, caso a documentação deixe de ser estática.',
           pendencias_homologacao = '- Executar homologação funcional com eventos reais em eventos_auditoria cobrindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM; Validar filtros e consulta administrativa em Admin > Governança > Auditoria com evidências formais.',
           criterio_aceite = 'O item Histórico/Auditoria deve exibir checklist ativo completo das Sprints 1, 2 e 3 com cálculo automático de percentual por checklist, status da implementação Implementado funcionalmente e status técnico Completo com pendências evolutivas, sem uso de percentual legado/manual.',
           proxima_acao = 'Executar homologação funcional com eventos reais em eventos_auditoria, incluindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM. Validar filtros e consulta administrativa em Admin > Governança > Auditoria.',
           observacao = 'Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.',
           status = 3,
           prioridade = 1,
           impacto = 1,
           decisao = 4,
           status_implementacao = 3,
           status_tecnico = 3,
           ordem = 6,
           ativo = TRUE,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
     WHERE id = v_canonical_id;

    UPDATE roadmap_itsm_itens
       SET ativo = FALSE,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
     WHERE id <> v_canonical_id
       AND replace(replace(replace(lower(translate(coalesce(area, ''), 'ÁÀÂÃÄáàâãäÉÈÊËéèêëÍÌÎÏíìîïÓÒÔÕÖóòôõöÚÙÛÜúùûüÇç', 'AAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUUuuuuCc')), ' ', ''), '/', ''), '-', '') = 'historicoauditoria';

    WITH dados(id, ordem, titulo, descricao, grupo, concluido) AS
    (
        VALUES
{checklistValuesSql}
    )
    INSERT INTO roadmap_checklist_itens
    (
        id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por, atualizado_em, atualizado_por
    )
    SELECT d.id, v_canonical_id, d.titulo, d.descricao, d.grupo, d.ordem, d.concluido, TRUE, TRUE, TIMESTAMPTZ '2026-01-01 00:00:00Z', 'seed.sistema', NOW(), 'migracao.roadmap.historico.auditoria'
      FROM dados d
    ON CONFLICT (id) DO UPDATE
    SET roadmap_item_id = EXCLUDED.roadmap_item_id,
        titulo = EXCLUDED.titulo,
        descricao = EXCLUDED.descricao,
        grupo = EXCLUDED.grupo,
        ordem = EXCLUDED.ordem,
        concluido = EXCLUDED.concluido,
        obrigatorio = TRUE,
        ativo = TRUE,
        atualizado_em = NOW(),
        atualizado_por = 'migracao.roadmap.historico.auditoria';

    WITH dados(id, ordem) AS
    (
        VALUES
{checklistIdsSql}
    ),
    duplicados AS
    (
        SELECT c.id,
               ROW_NUMBER() OVER
               (
                   PARTITION BY c.ordem
                   ORDER BY CASE WHEN c.id = d.id THEN 0 ELSE 1 END, c.criado_em, c.id
               ) AS rn
          FROM roadmap_checklist_itens c
          LEFT JOIN dados d ON d.ordem = c.ordem
         WHERE c.roadmap_item_id = v_canonical_id
           AND c.ativo = TRUE
    )
    UPDATE roadmap_checklist_itens c
       SET ativo = FALSE,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
      FROM duplicados x
     WHERE c.id = x.id
       AND x.rn > 1;

    WITH dados(id, ordem) AS
    (
        VALUES
{checklistIdsSql}
    )
    UPDATE roadmap_checklist_itens c
       SET ativo = FALSE,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
     WHERE c.roadmap_item_id = v_canonical_id
       AND c.ativo = TRUE
       AND NOT EXISTS (SELECT 1 FROM dados d WHERE d.id = c.id);

    UPDATE roadmap_checklist_itens
       SET concluido = TRUE,
           obrigatorio = TRUE,
           ativo = TRUE,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
     WHERE roadmap_item_id = v_canonical_id
       AND ordem BETWEEN 1 AND 63;

    UPDATE roadmap_itsm_itens i
       SET percentual_implementacao = calc.percentual,
           status_implementacao = 3,
           status_tecnico = 3,
           atualizado_em = NOW(),
           atualizado_por = 'migracao.roadmap.historico.auditoria'
      FROM
      (
            SELECT roadmap_item_id,
                   COALESCE((COUNT(*) FILTER (WHERE concluido) * 100) / NULLIF(COUNT(*), 0), 0) AS percentual
              FROM roadmap_checklist_itens
             WHERE roadmap_item_id = v_canonical_id
               AND ativo = TRUE
             GROUP BY roadmap_item_id
      ) calc
     WHERE i.id = calc.roadmap_item_id;
END $$;
";

        migrationBuilder.Sql(sql);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Migração de consolidação idempotente de dados. Down intencionalmente não remove dados históricos.
    }

    private static string EscapeSql(string valor) => valor.Replace("'", "''");

    private static ChecklistDef[] ObterChecklist() =>
    [
        new(Guid.Parse("71717171-7171-7171-7171-000000000001"), 1, "Entidade EventoAuditoria criada.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000002"), 2, "Enum de ação de auditoria criado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000003"), 3, "Enum de nível de auditoria criado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000004"), 4, "Migration da tabela eventos_auditoria criada.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000005"), 5, "Índices de consulta criados.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000006"), 6, "Service centralizado de auditoria criado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000007"), 7, "Context provider de auditoria criado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000008"), 8, "Captura de usuário atual integrada.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000009"), 9, "Captura de IP e User-Agent integrada.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000a"), 10, "Registro de login integrado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000b"), 11, "Registro de logout avaliado e documentado como não aplicável enquanto não houver fluxo backend controlado.", "Checklist da Sprint 1", 1, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000c"), 12, "Registro de criação/edição/inativação de usuário integrado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000d"), 13, "Registro de perfis/permissões integrado.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000e"), 14, "DTOs de auditoria criados.", "Checklist da Sprint 1", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000000f"), 15, "Testes automatizados criados.", "Checklist da Sprint 1", 3, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000010"), 16, "Documentação atualizada em Gestão ITSM.", "Checklist da Sprint 1", 4, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000011"), 17, "Helper de diff antes/depois criado.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000012"), 18, "Mascaramento de dados sensíveis implementado.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000013"), 19, "Auditoria de abertura de chamado implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000014"), 20, "Auditoria de alteração de status implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000015"), 21, "Auditoria de alteração de prioridade implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000016"), 22, "Auditoria de alteração de categoria implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000017"), 23, "Auditoria de atribuição de responsável implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000018"), 24, "Auditoria de assumir chamado implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000019"), 25, "Auditoria de comentários administrativos implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001a"), 26, "Auditoria de encerramento/resolução implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001b"), 27, "Auditoria de reabertura implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001c"), 28, "Auditoria de anexos preparada ou implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001d"), 29, "Auditoria de usuários revisada e complementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001e"), 30, "Auditoria de perfis revisada e complementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000001f"), 31, "Auditoria de permissões revisada e complementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000020"), 32, "Auditoria de políticas de SLA implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000021"), 33, "Auditoria de metas de SLA implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000022"), 34, "Auditoria de calendários de SLA implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000023"), 35, "Auditoria de horários de calendário implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000024"), 36, "Auditoria de exceções de calendário implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000025"), 37, "Auditoria de alertas de SLA implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000026"), 38, "Auditoria de autenticação corporativa implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000027"), 39, "Auditoria de Roadmap ITSM implementada.", "Checklist da Sprint 2", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000028"), 40, "Auditoria de documentação ITSM preparada conforme estrutura atual estática.", "Checklist da Sprint 2", 1, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000029"), 41, "Testes automatizados de auditoria dos módulos críticos criados.", "Checklist da Sprint 2", 3, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002a"), 42, "Documentação atualizada em Gestão ITSM.", "Checklist da Sprint 2", 4, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002b"), 43, "Validação no banco confirmando eventos reais em eventos_auditoria preparada/executada.", "Checklist da Sprint 2", 1, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002c"), 44, "Endpoints administrativos de auditoria criados.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002d"), 45, "Use cases/services de consulta de auditoria criados.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002e"), 46, "Filtros de auditoria criados.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000002f"), 47, "Paginação de eventos criada.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000030"), 48, "Endpoint de detalhe de evento criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000031"), 49, "Endpoint de dashboard de auditoria criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000032"), 50, "Permissões de auditoria criadas ou integradas.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000033"), 51, "Menu Governança > Auditoria criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000034"), 52, "Rota /admin/governanca/auditoria criada.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000035"), 53, "Tela administrativa de auditoria criada.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000036"), 54, "Modal/drawer de detalhe criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000037"), 55, "Visualização de dados antes/depois criada.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000038"), 56, "Indicadores básicos de auditoria criados.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-000000000039"), 57, "Service frontend de auditoria criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003a"), 58, "Tipos frontend de auditoria criados.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003b"), 59, "Link entre Auditoria e Gestão ITSM criado.", "Checklist da Sprint 3", 2, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003c"), 60, "Documentação em Gestão ITSM atualizada.", "Checklist da Sprint 3", 4, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003d"), 61, "Testes automatizados backend criados.", "Checklist da Sprint 3", 3, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003e"), 62, "Build frontend validado.", "Checklist da Sprint 3", 3, true),
        new(Guid.Parse("71717171-7171-7171-7171-00000000003f"), 63, "Validação com eventos reais em eventos_auditoria executada.", "Checklist da Sprint 3", 1, true)
    ];

    private readonly record struct ChecklistDef(Guid Id, int Ordem, string Titulo, string Descricao, int Grupo, bool Concluido);
}
