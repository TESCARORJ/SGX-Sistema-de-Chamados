using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations;

[DbContext(typeof(SGXSistemaChamadoDbContext))]
[Migration("20260514190000_GarantirChecklistAutenticacaoCorporativa")]
public partial class GarantirChecklistAutenticacaoCorporativa : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
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
                '77777777-7777-7777-7777-777777777704',
                'Autenticação corporativa',
                'Segurança',
                'Permitir que usuários acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autorização interna no SGX por usuários, perfis e permissões. O Azure autentica a identidade; o SGX controla o que cada usuário pode acessar e executar dentro do sistema.',
                '66666666-6666-6666-6666-666666666601',
                'Fluxo de autenticação corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com suporte a validação de token JWT, modo Single Tenant, controle de domínio permitido, integração com GET /api/me, criação/identificação de usuário interno e autorização por perfis/permissões do SGX. Ainda depende de homologação com tenant institucional real.',
                'Manter a explicação clara: Microsoft Entra ID/Azure AD autentica; SGX autoriza. Não usar roles ou groups do Azure para conceder acesso administrativo automaticamente. Perfis e permissões continuam internos ao SGX. Validar MFA, Conditional Access, tenant real, redirect URI, API scope e ambiente publicado antes de considerar produção.',
                1, 1, 1, 1, 3, 3, 70,
                '- Homologar com tenant institucional real do Microsoft Entra ID.\n- Validar login com usuários corporativos reais.\n- Validar MFA.\n- Validar Conditional Access.\n- Validar logout corporativo.\n- Validar ambiente publicado/VPS.\n- Revisar configuração com a equipe responsável pelo Azure.\n- Registrar evidências formais de homologação.\n- Avaliar persistência opcional de identificadores corporativos oid/tid, se necessário.\n- Definir governança de ciclo de vida do usuário interno: bloqueio, reativação e auditoria.',
                '- Executar homologação ponta a ponta com usuário Administrador real.\n- Executar homologação ponta a ponta com usuário Atendente real.\n- Executar homologação ponta a ponta com usuário Solicitante real.\n- Validar comportamento com usuário interno inativo.\n- Validar bloqueio de domínio/tenant não permitido.\n- Validar mensagens de erro de login.\n- Validar redirecionamento por perfil/permissão após login.\n- Registrar evidências com prints, data, ambiente e usuário de teste.',
                'docs/AUTENTICACAO-CORPORATIVA.md; docs/CONFIGURACAO-AZURE-AD.md; docs/HOMOLOGACAO-CHECKLIST.md; docs/ROADMAP.md; docs/ROADMAP-ITSM.md; src/SGX.SistemaChamado.Api/Services/UsuarioAtualService.cs; src/SGX.SistemaChamado.Api/Extensions/ServiceCollectionExtensions.cs; src/SGX.SistemaChamado.Api/Options/AuthOptions.cs; src/SGX.SistemaChamado.Api/Options/AzureAdOptions.cs; src/SGX.SistemaChamado.Api/Options/AzureAdOptionsValidator.cs; src/SGX.SistemaChamado.Web/src/views/LoginView.vue; src/SGX.SistemaChamado.Web/src/services/authService.ts; src/SGX.SistemaChamado.Web/src/stores/authStore.ts; tests/SGX.SistemaChamado.Tests/UsuarioAtualServiceTests.cs; tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs; tests/SGX.SistemaChamado.Tests/AzureAdOptionsValidatorTests.cs',
                NULL, NULL,
                'O usuário corporativo autentica pelo Microsoft Entra ID/Azure AD no tenant configurado. A API valida token, issuer, audience, tenant, expiração e assinatura. O SGX identifica ou cria o usuário interno conforme configuração permitida, bloqueia usuários inativos ou fora do tenant/domínio permitido, retorna perfis e permissões efetivas em GET /api/me e aplica autorização interna nas rotas e ações. Usuários Solicitante, Atendente e Administrador devem acessar apenas o que seus perfis/permissões internos permitem.',
                'Executar homologação com tenant institucional real do Microsoft Entra ID, validar MFA/Conditional Access, revisar configuração com a equipe Azure, testar usuários reais por perfil e anexar evidências formais antes de promoção para produção.',
                'Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.',
                NULL, NULL, 4, TRUE, TIMESTAMPTZ '2026-01-01 00:00:00Z', 'seed.sistema', NULL, NULL
            )
            ON CONFLICT (id) DO UPDATE
            SET
                area = EXCLUDED.area,
                categoria = EXCLUDED.categoria,
                objetivo = EXCLUDED.objetivo,
                roadmap_categoria_id = EXCLUDED.roadmap_categoria_id,
                situacao_atual = EXCLUDED.situacao_atual,
                atencao_tecnica = EXCLUDED.atencao_tecnica,
                status = EXCLUDED.status,
                prioridade = EXCLUDED.prioridade,
                impacto = EXCLUDED.impacto,
                decisao = EXCLUDED.decisao,
                status_implementacao = EXCLUDED.status_implementacao,
                status_tecnico = EXCLUDED.status_tecnico,
                percentual_implementacao = EXCLUDED.percentual_implementacao,
                pendencias_tecnicas = EXCLUDED.pendencias_tecnicas,
                pendencias_homologacao = EXCLUDED.pendencias_homologacao,
                evidencia_implementacao = EXCLUDED.evidencia_implementacao,
                data_conclusao_tecnica = EXCLUDED.data_conclusao_tecnica,
                data_homologacao = EXCLUDED.data_homologacao,
                criterio_aceite = EXCLUDED.criterio_aceite,
                proxima_acao = EXCLUDED.proxima_acao,
                observacao = EXCLUDED.observacao,
                ordem = EXCLUDED.ordem,
                ativo = EXCLUDED.ativo;
            """);

        migrationBuilder.Sql(
            """
            INSERT INTO roadmap_checklist_itens
            (
                id, roadmap_item_id, titulo, descricao, grupo, ordem, concluido, obrigatorio, ativo, criado_em, criado_por, atualizado_em, atualizado_por
            )
            VALUES
            ('69696969-6969-6969-6969-696969696701','77777777-7777-7777-7777-777777777704','Decisão arquitetural documentada: Azure autentica, SGX autoriza.','Checklist técnico concluído',1,1,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696702','77777777-7777-7777-7777-777777777704','Login Microsoft revisado no frontend.','Checklist técnico concluído',2,2,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696703','77777777-7777-7777-7777-777777777704','Validação JWT/API revisada.','Checklist técnico concluído',2,3,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696704','77777777-7777-7777-7777-777777777704','GET /api/me revisado.','Checklist técnico concluído',2,4,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696705','77777777-7777-7777-7777-777777777704','httpClient e tratamento de 401/403 revisados.','Checklist técnico concluído',2,5,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696706','77777777-7777-7777-7777-777777777704','Router guards revisados.','Checklist técnico concluído',2,6,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696707','77777777-7777-7777-7777-777777777704','Login local Development preservado.','Checklist técnico concluído',1,7,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696708','77777777-7777-7777-7777-777777777704','Emulação de perfis em Development preservada.','Checklist técnico concluído',1,8,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696709','77777777-7777-7777-7777-777777777704','Documentação técnica consolidada.','Checklist técnico concluído',4,9,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696710','77777777-7777-7777-7777-777777777704','Authority, Issuer, Audience, expiração e assinatura validados.','Checklist técnico concluído',3,10,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696711','77777777-7777-7777-7777-777777777704','MetadataAddress opcional suportado.','Checklist técnico concluído',2,11,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696712','77777777-7777-7777-7777-777777777704','Domínios permitidos configuráveis.','Checklist técnico concluído',1,12,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696713','77777777-7777-7777-7777-777777777704','Criação automática de usuário interno configurável.','Checklist técnico concluído',2,13,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696714','77777777-7777-7777-7777-777777777704','Perfil padrão de usuário Microsoft configurável.','Checklist técnico concluído',2,14,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696715','77777777-7777-7777-7777-777777777704','Claims Microsoft mapeadas com fallback.','Checklist técnico concluído',2,15,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696716','77777777-7777-7777-7777-777777777704','Bloqueio por domínio não permitido.','Checklist técnico concluído',1,16,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696717','77777777-7777-7777-7777-777777777704','Bloqueio de usuário interno inativo.','Checklist técnico concluído',1,17,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696718','77777777-7777-7777-7777-777777777704','Roles/groups do Azure não concedem Administrador automaticamente.','Checklist técnico concluído',1,18,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696719','77777777-7777-7777-7777-777777777704','Testes automatizados atualizados.','Checklist técnico concluído',3,19,TRUE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696720','77777777-7777-7777-7777-777777777704','Configurar tenant institucional real.','Checklist pendente de homologação/governança',5,20,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696721','77777777-7777-7777-7777-777777777704','Validar login com usuários corporativos reais.','Checklist pendente de homologação/governança',5,21,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696722','77777777-7777-7777-7777-777777777704','Validar MFA.','Checklist pendente de homologação/governança',5,22,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696723','77777777-7777-7777-7777-777777777704','Validar Conditional Access.','Checklist pendente de homologação/governança',5,23,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696724','77777777-7777-7777-7777-777777777704','Validar logout corporativo.','Checklist pendente de homologação/governança',5,24,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696725','77777777-7777-7777-7777-777777777704','Validar ambiente publicado/VPS.','Checklist pendente de homologação/governança',5,25,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696726','77777777-7777-7777-7777-777777777704','Revisar configuração com equipe responsável pelo Azure.','Checklist pendente de homologação/governança',1,26,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL),
            ('69696969-6969-6969-6969-696969696727','77777777-7777-7777-7777-777777777704','Registrar evidências formais de homologação.','Checklist pendente de homologação/governança',4,27,FALSE,TRUE,TRUE,TIMESTAMPTZ '2026-01-01 00:00:00Z','seed.sistema',NULL,NULL)
            ON CONFLICT (id) DO UPDATE
            SET
                roadmap_item_id = EXCLUDED.roadmap_item_id,
                titulo = EXCLUDED.titulo,
                descricao = EXCLUDED.descricao,
                grupo = EXCLUDED.grupo,
                ordem = EXCLUDED.ordem,
                concluido = EXCLUDED.concluido,
                obrigatorio = EXCLUDED.obrigatorio,
                ativo = EXCLUDED.ativo;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Migração corretiva e idempotente de dados. Não remover registros no rollback.
    }
}
