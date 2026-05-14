using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static readonly DateTime DataBase = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public const string UsuarioSistema = "seed.sistema";

    public static readonly Guid PerfilAdministradorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid PerfilAtendenteId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PerfilSolicitanteId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid PermissaoDashboardVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888801");
    public static readonly Guid PermissaoChamadosVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888802");
    public static readonly Guid PermissaoChamadosVisualizarTodosId = Guid.Parse("88888888-8888-8888-8888-888888888803");
    public static readonly Guid PermissaoChamadosAbrirId = Guid.Parse("88888888-8888-8888-8888-888888888804");
    public static readonly Guid PermissaoChamadosComentarId = Guid.Parse("88888888-8888-8888-8888-888888888805");
    public static readonly Guid PermissaoChamadosAnexarId = Guid.Parse("88888888-8888-8888-8888-888888888806");
    public static readonly Guid PermissaoChamadosAssumirId = Guid.Parse("88888888-8888-8888-8888-888888888807");
    public static readonly Guid PermissaoChamadosAtribuirId = Guid.Parse("88888888-8888-8888-8888-888888888808");
    public static readonly Guid PermissaoChamadosAlterarStatusId = Guid.Parse("88888888-8888-8888-8888-888888888809");
    public static readonly Guid PermissaoChamadosAlterarPrioridadeId = Guid.Parse("88888888-8888-8888-8888-888888888810");
    public static readonly Guid PermissaoChamadosAlterarCategoriaId = Guid.Parse("88888888-8888-8888-8888-888888888811");
    public static readonly Guid PermissaoChamadosEncerrarId = Guid.Parse("88888888-8888-8888-8888-888888888812");
    public static readonly Guid PermissaoChamadosReabrirId = Guid.Parse("88888888-8888-8888-8888-888888888813");
    public static readonly Guid PermissaoCadastrosVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888814");
    public static readonly Guid PermissaoCadastrosGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888815");
    public static readonly Guid PermissaoUsuariosVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888816");
    public static readonly Guid PermissaoUsuariosGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888817");
    public static readonly Guid PermissaoUsuariosAlterarPerfisId = Guid.Parse("88888888-8888-8888-8888-888888888818");
    public static readonly Guid PermissaoPerfisVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888819");
    public static readonly Guid PermissaoPerfisGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888820");
    public static readonly Guid PermissaoPerfisAlterarPermissoesId = Guid.Parse("88888888-8888-8888-8888-888888888821");
    public static readonly Guid PermissaoParametrosVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888822");
    public static readonly Guid PermissaoParametrosGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888823");
    public static readonly Guid PermissaoIntegracoesEmailVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888824");
    public static readonly Guid PermissaoIntegracoesEmailGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888825");
    public static readonly Guid PermissaoNotificacoesVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888826");
    public static readonly Guid PermissaoNotificacoesGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888827");
    public static readonly Guid PermissaoIndicadoresVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888828");
    public static readonly Guid PermissaoRoadmapVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888829");
    public static readonly Guid PermissaoRoadmapGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888830");
    public static readonly Guid PermissaoRoadmapImplementacoesVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888831");
    public static readonly Guid PermissaoRoadmapImplementacoesGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888832");
    public static readonly Guid PermissaoIntegracoesMicrosoftVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888833");
    public static readonly Guid PermissaoIntegracoesMicrosoftGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888834");
    public static readonly Guid PermissaoUsuariosRedefinirSenhaId = Guid.Parse("88888888-8888-8888-8888-888888888835");

    public static readonly Guid StatusAbertoId = Guid.Parse("44444444-4444-4444-4444-444444444441");
    public static readonly Guid StatusEmAtendimentoId = Guid.Parse("44444444-4444-4444-4444-444444444442");
    public static readonly Guid StatusAguardandoSolicitanteId = Guid.Parse("44444444-4444-4444-4444-444444444443");
    public static readonly Guid StatusResolvidoId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid StatusEncerradoId = Guid.Parse("44444444-4444-4444-4444-444444444445");

    public static readonly Guid PrioridadeBaixaId = Guid.Parse("55555555-5555-5555-5555-555555555551");
    public static readonly Guid PrioridadeMediaId = Guid.Parse("55555555-5555-5555-5555-555555555552");
    public static readonly Guid PrioridadeAltaId = Guid.Parse("55555555-5555-5555-5555-555555555553");
    public static readonly Guid PrioridadeCriticaId = Guid.Parse("55555555-5555-5555-5555-555555555554");

    public static readonly Guid RoadmapItsmItem01Id = Guid.Parse("77777777-7777-7777-7777-777777777701");
    public static readonly Guid RoadmapItsmItem02Id = Guid.Parse("77777777-7777-7777-7777-777777777702");
    public static readonly Guid RoadmapItsmItem03Id = Guid.Parse("77777777-7777-7777-7777-777777777703");
    public static readonly Guid RoadmapItsmItem04Id = Guid.Parse("77777777-7777-7777-7777-777777777704");
    public static readonly Guid RoadmapItsmItem05Id = Guid.Parse("77777777-7777-7777-7777-777777777705");
    public static readonly Guid RoadmapItsmItem06Id = Guid.Parse("77777777-7777-7777-7777-777777777706");
    public static readonly Guid RoadmapItsmItem07Id = Guid.Parse("77777777-7777-7777-7777-777777777707");
    public static readonly Guid RoadmapItsmItem08Id = Guid.Parse("77777777-7777-7777-7777-777777777708");
    public static readonly Guid RoadmapItsmItem09Id = Guid.Parse("77777777-7777-7777-7777-777777777709");
    public static readonly Guid RoadmapItsmItem10Id = Guid.Parse("77777777-7777-7777-7777-777777777710");
    public static readonly Guid RoadmapItsmItem11Id = Guid.Parse("77777777-7777-7777-7777-777777777711");
    public static readonly Guid RoadmapItsmItem12Id = Guid.Parse("77777777-7777-7777-7777-777777777712");
    public static readonly Guid RoadmapItsmItem13Id = Guid.Parse("77777777-7777-7777-7777-777777777713");
    public static readonly Guid RoadmapItsmItem14Id = Guid.Parse("77777777-7777-7777-7777-777777777714");
    public static readonly Guid RoadmapItsmItem15Id = Guid.Parse("77777777-7777-7777-7777-777777777715");

    public static readonly Guid RoadmapCategoriaSegurancaId = Guid.Parse("66666666-6666-6666-6666-666666666601");
    public static readonly Guid RoadmapCategoriaAtendimentoId = Guid.Parse("66666666-6666-6666-6666-666666666602");
    public static readonly Guid RoadmapCategoriaSlaId = Guid.Parse("66666666-6666-6666-6666-666666666603");
    public static readonly Guid RoadmapCategoriaIntegracoesId = Guid.Parse("66666666-6666-6666-6666-666666666604");
    public static readonly Guid RoadmapCategoriaCadastrosId = Guid.Parse("66666666-6666-6666-6666-666666666605");
    public static readonly Guid RoadmapCategoriaNotificacoesId = Guid.Parse("66666666-6666-6666-6666-666666666606");
    public static readonly Guid RoadmapCategoriaInfraestruturaId = Guid.Parse("66666666-6666-6666-6666-666666666607");
    public static readonly Guid RoadmapCategoriaUxId = Guid.Parse("66666666-6666-6666-6666-666666666608");
    public static readonly Guid RoadmapCategoriaRelatoriosId = Guid.Parse("66666666-6666-6666-6666-666666666609");
    public static readonly Guid RoadmapCategoriaHomologacaoId = Guid.Parse("66666666-6666-6666-6666-666666666610");
    public static readonly Guid RoadmapCategoriaDocumentacaoId = Guid.Parse("66666666-6666-6666-6666-666666666611");
    public static readonly Guid RoadmapCategoriaGestaoId = Guid.Parse("66666666-6666-6666-6666-666666666612");
    public static readonly Guid RoadmapCategoriaGovernancaId = Guid.Parse("66666666-6666-6666-6666-666666666613");
    public static readonly Guid RoadmapCategoriaConhecimentoId = Guid.Parse("66666666-6666-6666-6666-666666666614");
    public static readonly Guid RoadmapCategoriaPortalId = Guid.Parse("66666666-6666-6666-6666-666666666615");

    public static readonly Guid ChecklistPerfisMacroId = Guid.Parse("67676767-6767-6767-6767-676767676701");
    public static readonly Guid ChecklistCrudPerfisId = Guid.Parse("67676767-6767-6767-6767-676767676702");
    public static readonly Guid ChecklistPermissoesGranularesId = Guid.Parse("67676767-6767-6767-6767-676767676703");
    public static readonly Guid ChecklistMigrationId = Guid.Parse("67676767-6767-6767-6767-676767676704");
    public static readonly Guid ChecklistSeedsId = Guid.Parse("67676767-6767-6767-6767-676767676705");
    public static readonly Guid ChecklistApiMePermissoesId = Guid.Parse("67676767-6767-6767-6767-676767676706");
    public static readonly Guid ChecklistAuthorizationHandlerId = Guid.Parse("67676767-6767-6767-6767-676767676707");
    public static readonly Guid ChecklistMatrizFrontendId = Guid.Parse("67676767-6767-6767-6767-676767676708");
    public static readonly Guid ChecklistControleVisualId = Guid.Parse("67676767-6767-6767-6767-676767676709");
    public static readonly Guid ChecklistHomologacaoUsuariosId = Guid.Parse("67676767-6767-6767-6767-676767676710");
    public static readonly Guid ChecklistPortalContextoValidadoId = Guid.Parse("67676767-6767-6767-6767-676767676711");
    public static readonly Guid ChecklistPortalCriacaoValidadaId = Guid.Parse("67676767-6767-6767-6767-676767676712");
    public static readonly Guid ChecklistPortalValidacoesObrigatoriasId = Guid.Parse("67676767-6767-6767-6767-676767676713");
    public static readonly Guid ChecklistPortalSolicitanteAutenticadoId = Guid.Parse("67676767-6767-6767-6767-676767676714");
    public static readonly Guid ChecklistPortalStatusAbertoId = Guid.Parse("67676767-6767-6767-6767-676767676715");
    public static readonly Guid ChecklistPortalHistoricoInicialId = Guid.Parse("67676767-6767-6767-6767-676767676716");
    public static readonly Guid ChecklistPortalSlaInicialId = Guid.Parse("67676767-6767-6767-6767-676767676717");
    public static readonly Guid ChecklistPortalTelaNovoId = Guid.Parse("67676767-6767-6767-6767-676767676718");
    public static readonly Guid ChecklistPortalUploadAnexoId = Guid.Parse("67676767-6767-6767-6767-676767676719");
    public static readonly Guid ChecklistPortalRedirectDetalheId = Guid.Parse("67676767-6767-6767-6767-676767676720");
    public static readonly Guid ChecklistPortalListagemPortalId = Guid.Parse("67676767-6767-6767-6767-676767676721");
    public static readonly Guid ChecklistPortalVisivelAdminId = Guid.Parse("67676767-6767-6767-6767-676767676722");
    public static readonly Guid ChecklistPortalDetalheValidadoId = Guid.Parse("67676767-6767-6767-6767-676767676723");
    public static readonly Guid ChecklistPortalHistoricoVisivelId = Guid.Parse("67676767-6767-6767-6767-676767676724");
    public static readonly Guid ChecklistPortalAnexoVisivelPortalId = Guid.Parse("67676767-6767-6767-6767-676767676725");
    public static readonly Guid ChecklistPortalAnexoVisivelAdminId = Guid.Parse("67676767-6767-6767-6767-676767676726");
    public static readonly Guid ChecklistPortalTestesBackendId = Guid.Parse("67676767-6767-6767-6767-676767676727");
    public static readonly Guid ChecklistPortalBuildFrontendId = Guid.Parse("67676767-6767-6767-6767-676767676728");
    public static readonly Guid ChecklistPortalHomologacaoUsuarioRealId = Guid.Parse("67676767-6767-6767-6767-676767676729");
    public static readonly Guid ChecklistPortalE2eFrontendId = Guid.Parse("67676767-6767-6767-6767-676767676730");
    public static readonly Guid ChecklistPortalValidacaoAnexosHomologacaoId = Guid.Parse("67676767-6767-6767-6767-676767676731");
    public static readonly Guid ChecklistPortalAnexoInvalidoAmigavelId = Guid.Parse("67676767-6767-6767-6767-676767676732");
    public static readonly Guid ChecklistPortalFluxoCompletoAcompanharId = Guid.Parse("67676767-6767-6767-6767-676767676733");
    public static readonly Guid ChecklistPortalSolicitanteEntraId = Guid.Parse("67676767-6767-6767-6767-676767676734");
    public static readonly Guid ChecklistPortalAtendenteFilaId = Guid.Parse("67676767-6767-6767-6767-676767676735");
    public static readonly Guid ChecklistEmailWorkerProjetoValidadoId = Guid.Parse("68686868-6868-6868-6868-686868686701");
    public static readonly Guid ChecklistEmailConfiguracoesImapDefinidasId = Guid.Parse("68686868-6868-6868-6868-686868686702");
    public static readonly Guid ChecklistEmailLeituraImapImplementadaId = Guid.Parse("68686868-6868-6868-6868-686868686703");
    public static readonly Guid ChecklistEmailProcessamentoLoteImplementadoId = Guid.Parse("68686868-6868-6868-6868-686868686704");
    public static readonly Guid ChecklistEmailLogIntegracaoImplementadoId = Guid.Parse("68686868-6868-6868-6868-686868686705");
    public static readonly Guid ChecklistEmailDeduplicacaoMessageIdId = Guid.Parse("68686868-6868-6868-6868-686868686706");
    public static readonly Guid ChecklistEmailNovoCriaChamadoId = Guid.Parse("68686868-6868-6868-6868-686868686707");
    public static readonly Guid ChecklistEmailOrigemAplicadaId = Guid.Parse("68686868-6868-6868-6868-686868686708");
    public static readonly Guid ChecklistEmailStatusAbertoAplicadoId = Guid.Parse("68686868-6868-6868-6868-686868686709");
    public static readonly Guid ChecklistEmailHistoricoInicialCriadoId = Guid.Parse("68686868-6868-6868-6868-686868686710");
    public static readonly Guid ChecklistEmailCorrelacaoCodigoAssuntoId = Guid.Parse("68686868-6868-6868-6868-686868686711");
    public static readonly Guid ChecklistEmailCorrelacaoHeadersId = Guid.Parse("68686868-6868-6868-6868-686868686712");
    public static readonly Guid ChecklistEmailRespostaAdicionaComentarioId = Guid.Parse("68686868-6868-6868-6868-686868686713");
    public static readonly Guid ChecklistEmailAnexosValidadosId = Guid.Parse("68686868-6868-6868-6868-686868686714");
    public static readonly Guid ChecklistEmailAnexosPermitidosSalvosId = Guid.Parse("68686868-6868-6868-6868-686868686715");
    public static readonly Guid ChecklistEmailAnexosInvalidosRejeitadosId = Guid.Parse("68686868-6868-6868-6868-686868686716");
    public static readonly Guid ChecklistEmailEndpointLogsAdminId = Guid.Parse("68686868-6868-6868-6868-686868686717");
    public static readonly Guid ChecklistEmailTelaAdminValidadaId = Guid.Parse("68686868-6868-6868-6868-686868686718");
    public static readonly Guid ChecklistEmailFiltrosLogsImplementadosId = Guid.Parse("68686868-6868-6868-6868-686868686719");
    public static readonly Guid ChecklistEmailDetalheDialogImplementadoId = Guid.Parse("68686868-6868-6868-6868-686868686720");
    public static readonly Guid ChecklistEmailTestesProcessamentoId = Guid.Parse("68686868-6868-6868-6868-686868686721");
    public static readonly Guid ChecklistEmailTestesCorrelacaoId = Guid.Parse("68686868-6868-6868-6868-686868686722");
    public static readonly Guid ChecklistEmailTestesAnexosId = Guid.Parse("68686868-6868-6868-6868-686868686723");
    public static readonly Guid ChecklistEmailBuildBackendValidadoId = Guid.Parse("68686868-6868-6868-6868-686868686724");
    public static readonly Guid ChecklistEmailTestesBackendExecutadosId = Guid.Parse("68686868-6868-6868-6868-686868686725");
    public static readonly Guid ChecklistEmailBuildWorkerValidadoId = Guid.Parse("68686868-6868-6868-6868-686868686726");
    public static readonly Guid ChecklistEmailBuildFrontendValidadoId = Guid.Parse("68686868-6868-6868-6868-686868686727");
    public static readonly Guid ChecklistEmailValidacaoCaixaImapRealId = Guid.Parse("68686868-6868-6868-6868-686868686728");
    public static readonly Guid ChecklistEmailHomologacaoEmailsReaisId = Guid.Parse("68686868-6868-6868-6868-686868686729");
    public static readonly Guid ChecklistEmailValidacaoAnexosReaisId = Guid.Parse("68686868-6868-6868-6868-686868686730");
    public static readonly Guid ChecklistEmailOauthMicrosoftId = Guid.Parse("68686868-6868-6868-6868-686868686731");
    public static readonly Guid ChecklistEmailRetryBackoffId = Guid.Parse("68686868-6868-6868-6868-686868686732");
    public static readonly Guid ChecklistEmailDeadLetterId = Guid.Parse("68686868-6868-6868-6868-686868686733");
    public static readonly Guid ChecklistEmailMonitoramentoWorkerId = Guid.Parse("68686868-6868-6868-6868-686868686734");
    public static readonly Guid ChecklistEmailReprocessamentoManualId = Guid.Parse("68686868-6868-6868-6868-686868686735");
    public static readonly Guid ChecklistEmailSanitizacaoHtmlAvancadaId = Guid.Parse("68686868-6868-6868-6868-686868686736");
    public static readonly Guid ChecklistEmailAntivirusAnexosId = Guid.Parse("68686868-6868-6868-6868-686868686737");
    public static readonly Guid ChecklistEmailTesteE2eImapRealId = Guid.Parse("68686868-6868-6868-6868-686868686738");
    public static readonly Guid ChecklistEmailMetricasOperacionaisId = Guid.Parse("68686868-6868-6868-6868-686868686739");
    public static readonly Guid ChecklistEmailAlertasFalhaRecorrenteId = Guid.Parse("68686868-6868-6868-6868-686868686740");
    public static readonly Guid ChecklistAutenticacaoArquiteturaDefinidaId = Guid.Parse("69696969-6969-6969-6969-696969696701");
    public static readonly Guid ChecklistAutenticacaoAppRegistrationDocumentadoId = Guid.Parse("69696969-6969-6969-6969-696969696702");
    public static readonly Guid ChecklistAutenticacaoVariaveisBackendDocumentadasId = Guid.Parse("69696969-6969-6969-6969-696969696703");
    public static readonly Guid ChecklistAutenticacaoVariaveisFrontendDocumentadasId = Guid.Parse("69696969-6969-6969-6969-696969696704");
    public static readonly Guid ChecklistAutenticacaoJwtBearerConfiguradoId = Guid.Parse("69696969-6969-6969-6969-696969696705");
    public static readonly Guid ChecklistAutenticacaoClaimsMicrosoftMapeadasId = Guid.Parse("69696969-6969-6969-6969-696969696706");
    public static readonly Guid ChecklistAutenticacaoUsuarioLocalizadoPorEmailLoginId = Guid.Parse("69696969-6969-6969-6969-696969696707");
    public static readonly Guid ChecklistAutenticacaoUsuarioNovoTratadoRegraSgxId = Guid.Parse("69696969-6969-6969-6969-696969696708");
    public static readonly Guid ChecklistAutenticacaoApiMePerfisPermissoesId = Guid.Parse("69696969-6969-6969-6969-696969696709");
    public static readonly Guid ChecklistAutenticacaoLoginMicrosoftFrontendId = Guid.Parse("69696969-6969-6969-6969-696969696710");
    public static readonly Guid ChecklistAutenticacaoLoginLocalApenasDevelopmentId = Guid.Parse("69696969-6969-6969-6969-696969696711");
    public static readonly Guid ChecklistAutenticacaoGuardsFuncionandoId = Guid.Parse("69696969-6969-6969-6969-696969696712");
    public static readonly Guid ChecklistAutenticacaoPermissoesInternasPreservadasId = Guid.Parse("69696969-6969-6969-6969-696969696713");
    public static readonly Guid ChecklistAutenticacaoRefreshSemLogoffIndevidoId = Guid.Parse("69696969-6969-6969-6969-696969696714");
    public static readonly Guid ChecklistAutenticacaoUsuarioInativoBloqueadoId = Guid.Parse("69696969-6969-6969-6969-696969696715");
    public static readonly Guid ChecklistAutenticacaoDocumentacaoAtualizadaId = Guid.Parse("69696969-6969-6969-6969-696969696716");
    public static readonly Guid ChecklistAutenticacaoBuildBackendValidadoId = Guid.Parse("69696969-6969-6969-6969-696969696717");
    public static readonly Guid ChecklistAutenticacaoTestesBackendExecutadosId = Guid.Parse("69696969-6969-6969-6969-696969696718");
    public static readonly Guid ChecklistAutenticacaoBuildFrontendValidadoId = Guid.Parse("69696969-6969-6969-6969-696969696719");
    public static readonly Guid ChecklistAutenticacaoHomologacaoTenantRealId = Guid.Parse("69696969-6969-6969-6969-696969696720");
    public static readonly Guid ChecklistAutenticacaoTesteUsuarioRealDominioId = Guid.Parse("69696969-6969-6969-6969-696969696721");
    public static readonly Guid ChecklistAutenticacaoTesteMfaId = Guid.Parse("69696969-6969-6969-6969-696969696722");
    public static readonly Guid ChecklistAutenticacaoTesteConditionalAccessId = Guid.Parse("69696969-6969-6969-6969-696969696723");
    public static readonly Guid ChecklistAutenticacaoTesteLogoutCorporativoId = Guid.Parse("69696969-6969-6969-6969-696969696724");
    public static readonly Guid ChecklistAutenticacaoTesteAmbientePublicadoId = Guid.Parse("69696969-6969-6969-6969-696969696725");
    public static readonly Guid ChecklistAutenticacaoRevisaoEquipeAzureId = Guid.Parse("69696969-6969-6969-6969-696969696726");
    public static readonly Guid ChecklistAutenticacaoEvidenciaFormalHomologacaoId = Guid.Parse("69696969-6969-6969-6969-696969696727");

    public static readonly object[] PerfisAcesso =
    [
        new
        {
            Id = PerfilAdministradorId,
            Nome = "Administrador",
            TipoPerfil = TipoPerfil.Administrador,
            Descricao = "Perfil com acesso total ao SGX Sistema de Chamados.",
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = PerfilAtendenteId,
            Nome = "Atendente",
            TipoPerfil = TipoPerfil.Atendente,
            Descricao = "Perfil responsavel por atendimento e resolucao dos chamados.",
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = PerfilSolicitanteId,
            Nome = "Solicitante",
            TipoPerfil = TipoPerfil.Solicitante,
            Descricao = "Perfil de abertura e acompanhamento de chamados.",
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    private static readonly (Guid Id, string Codigo)[] CatalogoPermissoesSistema =
    [
        (PermissaoDashboardVisualizarId, "Dashboard.Visualizar"),
        (PermissaoChamadosVisualizarId, "Chamados.Visualizar"),
        (PermissaoChamadosVisualizarTodosId, "Chamados.VisualizarTodos"),
        (PermissaoChamadosAbrirId, "Chamados.Abrir"),
        (PermissaoChamadosComentarId, "Chamados.Comentar"),
        (PermissaoChamadosAnexarId, "Chamados.Anexar"),
        (PermissaoChamadosAssumirId, "Chamados.Assumir"),
        (PermissaoChamadosAtribuirId, "Chamados.Atribuir"),
        (PermissaoChamadosAlterarStatusId, "Chamados.AlterarStatus"),
        (PermissaoChamadosAlterarPrioridadeId, "Chamados.AlterarPrioridade"),
        (PermissaoChamadosAlterarCategoriaId, "Chamados.AlterarCategoria"),
        (PermissaoChamadosEncerrarId, "Chamados.Encerrar"),
        (PermissaoChamadosReabrirId, "Chamados.Reabrir"),
        (PermissaoCadastrosVisualizarId, "Cadastros.Visualizar"),
        (PermissaoCadastrosGerenciarId, "Cadastros.Gerenciar"),
        (PermissaoUsuariosVisualizarId, "Usuarios.Visualizar"),
        (PermissaoUsuariosGerenciarId, "Usuarios.Gerenciar"),
        (PermissaoUsuariosAlterarPerfisId, "Usuarios.AlterarPerfis"),
        (PermissaoPerfisVisualizarId, "Perfis.Visualizar"),
        (PermissaoPerfisGerenciarId, "Perfis.Gerenciar"),
        (PermissaoPerfisAlterarPermissoesId, "Perfis.AlterarPermissoes"),
        (PermissaoParametrosVisualizarId, "Parametros.Visualizar"),
        (PermissaoParametrosGerenciarId, "Parametros.Gerenciar"),
        (PermissaoIntegracoesEmailVisualizarId, "IntegracoesEmail.Visualizar"),
        (PermissaoIntegracoesEmailGerenciarId, "IntegracoesEmail.Gerenciar"),
        (PermissaoNotificacoesVisualizarId, "Notificacoes.Visualizar"),
        (PermissaoNotificacoesGerenciarId, "Notificacoes.Gerenciar"),
        (PermissaoIndicadoresVisualizarId, "Indicadores.Visualizar"),
        (PermissaoRoadmapVisualizarId, "Roadmap.Visualizar"),
        (PermissaoRoadmapGerenciarId, "Roadmap.Gerenciar"),
        (PermissaoRoadmapImplementacoesVisualizarId, "RoadmapImplementacoes.Visualizar"),
        (PermissaoRoadmapImplementacoesGerenciarId, "RoadmapImplementacoes.Gerenciar"),
        (PermissaoIntegracoesMicrosoftVisualizarId, "IntegracoesMicrosoft.Visualizar"),
        (PermissaoIntegracoesMicrosoftGerenciarId, "IntegracoesMicrosoft.Gerenciar"),
        (PermissaoUsuariosRedefinirSenhaId, "Usuarios.RedefinirSenha")
    ];

    private static readonly IReadOnlyDictionary<string, Guid> PermissoesSistemaPorCodigo = CatalogoPermissoesSistema
        .ToDictionary(x => x.Codigo, x => x.Id, StringComparer.Ordinal);

    private static readonly string[] CodigosPermissoesAtendente =
    [
        "Dashboard.Visualizar",
        "Chamados.Visualizar",
        "Chamados.VisualizarTodos",
        "Chamados.Comentar",
        "Chamados.Anexar",
        "Chamados.Assumir",
        "Chamados.AlterarStatus",
        "Chamados.AlterarPrioridade",
        "Chamados.AlterarCategoria",
        "Chamados.Encerrar",
        "Chamados.Reabrir",
        "Cadastros.Visualizar",
        "Usuarios.Visualizar",
        "IntegracoesEmail.Visualizar",
        "Notificacoes.Visualizar",
        "Indicadores.Visualizar",
        "Roadmap.Visualizar",
        "RoadmapImplementacoes.Visualizar"
    ];

    private static readonly string[] CodigosPermissoesSolicitante =
    [
        "Chamados.Visualizar",
        "Chamados.Abrir",
        "Chamados.Comentar",
        "Chamados.Anexar",
        "Notificacoes.Visualizar"
    ];

    public static readonly object[] PermissoesSistema = CatalogoPermissoesSistema
        .Select(x =>
        {
            var partes = x.Codigo.Split('.', 2);
            return new
            {
                Id = x.Id,
                Codigo = x.Codigo,
                Modulo = partes[0],
                Acao = partes[1],
                Descricao = (string?)null,
                Ativo = true,
                CriadoEm = DataBase,
                CriadoPor = UsuarioSistema,
                AtualizadoEm = (DateTime?)null,
                AtualizadoPor = (string?)null
            };
        })
        .ToArray();

    public static readonly object[] PerfisAcessoPermissoes = CriarPerfisAcessoPermissoes();

    public static readonly object[] StatusChamado =
    [
        new
        {
            Id = StatusAbertoId,
            Nome = "Aberto",
            Codigo = StatusChamadoEnum.Aberto,
            Descricao = "Chamado aberto e aguardando atendimento.",
            EhStatusFinal = false,
            PausaSla = false,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = StatusEmAtendimentoId,
            Nome = "Em Atendimento",
            Codigo = StatusChamadoEnum.EmAtendimento,
            Descricao = "Chamado em atendimento pela equipe.",
            EhStatusFinal = false,
            PausaSla = false,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = StatusAguardandoSolicitanteId,
            Nome = "Aguardando Solicitante",
            Codigo = StatusChamadoEnum.AguardandoSolicitante,
            Descricao = "Chamado aguardando retorno do solicitante.",
            EhStatusFinal = false,
            PausaSla = true,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = StatusResolvidoId,
            Nome = "Resolvido",
            Codigo = StatusChamadoEnum.Resolvido,
            Descricao = "Chamado resolvido e aguardando encerramento.",
            EhStatusFinal = false,
            PausaSla = false,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = StatusEncerradoId,
            Nome = "Encerrado",
            Codigo = StatusChamadoEnum.Encerrado,
            Descricao = "Chamado encerrado.",
            EhStatusFinal = true,
            PausaSla = false,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    public static readonly object[] PrioridadesChamado =
    [
        new
        {
            Id = PrioridadeBaixaId,
            Nome = "Baixa",
            Nivel = PrioridadeChamadoEnum.Baixa,
            Descricao = "Impacto baixo.",
            PrazoPrimeiraRespostaHoras = 8,
            PrazoResolucaoHoras = 48,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = PrioridadeMediaId,
            Nome = "Media",
            Nivel = PrioridadeChamadoEnum.Media,
            Descricao = "Impacto moderado.",
            PrazoPrimeiraRespostaHoras = 4,
            PrazoResolucaoHoras = 24,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = PrioridadeAltaId,
            Nome = "Alta",
            Nivel = PrioridadeChamadoEnum.Alta,
            Descricao = "Impacto alto.",
            PrazoPrimeiraRespostaHoras = 2,
            PrazoResolucaoHoras = 8,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = PrioridadeCriticaId,
            Nome = "Critica",
            Nivel = PrioridadeChamadoEnum.Critica,
            Descricao = "Impacto critico.",
            PrazoPrimeiraRespostaHoras = 1,
            PrazoResolucaoHoras = 4,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    public static readonly object[] RoadmapCategorias =
    [
        new { Id = RoadmapCategoriaSegurancaId, Nome = "Segurança", Descricao = "Segurança e controle de acesso.", Cor = "#D32F2F", Icone = "shield", Ordem = 1, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaAtendimentoId, Nome = "Atendimento", Descricao = "Fluxos operacionais de atendimento.", Cor = "#1976D2", Icone = "support_agent", Ordem = 2, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaSlaId, Nome = "SLA", Descricao = "Metas e acompanhamento de SLA.", Cor = "#5D4037", Icone = "schedule", Ordem = 3, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaIntegracoesId, Nome = "Integrações", Descricao = "Integrações com canais e sistemas.", Cor = "#00897B", Icone = "hub", Ordem = 4, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaCadastrosId, Nome = "Cadastros", Descricao = "Cadastros e parametrizações administrativas.", Cor = "#7B1FA2", Icone = "inventory_2", Ordem = 5, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaNotificacoesId, Nome = "Notificações", Descricao = "Notificações e comunicação.", Cor = "#F57C00", Icone = "notifications", Ordem = 6, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaInfraestruturaId, Nome = "Infraestrutura", Descricao = "Infraestrutura e sustentação.", Cor = "#455A64", Icone = "dns", Ordem = 7, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaUxId, Nome = "UX", Descricao = "Experiência de uso e interface.", Cor = "#C2185B", Icone = "palette", Ordem = 8, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaRelatoriosId, Nome = "Relatórios", Descricao = "Relatórios e exportações.", Cor = "#6D4C41", Icone = "assessment", Ordem = 9, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaHomologacaoId, Nome = "Homologação", Descricao = "Validações e aceite com usuários.", Cor = "#388E3C", Icone = "fact_check", Ordem = 10, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaDocumentacaoId, Nome = "Documentação", Descricao = "Documentação técnica e funcional.", Cor = "#303F9F", Icone = "description", Ordem = 11, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaGestaoId, Nome = "Gestão", Descricao = "Indicadores e governança gerencial.", Cor = "#3949AB", Icone = "insights", Ordem = 12, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaGovernancaId, Nome = "Governança", Descricao = "Rastreabilidade e governança.", Cor = "#546E7A", Icone = "gavel", Ordem = 13, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaConhecimentoId, Nome = "Conhecimento", Descricao = "Base de conhecimento e catálogo.", Cor = "#8D6E63", Icone = "menu_book", Ordem = 14, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = RoadmapCategoriaPortalId, Nome = "Portal", Descricao = "Fluxos e experiência do portal.", Cor = "#1E88E5", Icone = "language", Ordem = 15, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null }
    ];

    public static readonly object[] RoadmapItsmItens =
    [
        new
        {
            Id = RoadmapItsmItem01Id,
            Area = "Abertura de chamado pelo portal",
            Categoria = "Portal",
            Objetivo = "Permitir que o solicitante autenticado registre chamados diretamente pelo portal, informando titulo, descricao, categoria, prioridade e anexos opcionais, acompanhando depois o andamento, historico e status do atendimento.",
            RoadmapCategoriaId = RoadmapCategoriaPortalId,
            SituacaoAtual = "Fluxo implementado no portal com abertura, anexos opcionais, listagem e detalhe",
            AtencaoTecnica = "Validar com usuario real e consolidar evidencias de homologacao",
            Status = StatusRoadmapItsm.Implementado,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.DesenvolverAgora,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 72,
            PendenciasTecnicas = "Testes E2E frontend do fluxo de abertura, validação real de anexos em homologação e script lint frontend.",
            PendenciasHomologacao = "Validar com usuário real o fluxo completo de abrir chamado, anexar arquivo, acompanhar no portal e visualizar na fila administrativa.",
            EvidenciaImplementacao = "GET /api/portal/contexto; POST /api/portal/chamados; tela /portal/chamados/novo; listagem /portal/chamados; detalhe /portal/chamados/:id; fila /admin/chamados; testes backend; build frontend.",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "Solicitante autenticado consegue abrir chamado pelo portal com título, descrição, categoria e prioridade, anexar arquivo permitido, visualizar o detalhe do chamado, acompanhar o status no portal e o chamado aparece na fila administrativa para atendimento.",
            ProximaAcao = "Executar homologação manual do fluxo completo com usuário real.",
            Observacao = "Implementado funcionalmente; nao homologado em usuario real nesta iteracao.",
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 1,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem02Id,
            Area = "Abertura por e-mail",
            Categoria = "Integracoes",
            Objetivo = "Permitir que e-mails recebidos em uma caixa configurada sejam processados pelo Worker IMAP para criar chamados automaticamente, correlacionar respostas com chamados existentes, registrar anexos permitidos e manter logs tecnicos de processamento.",
            RoadmapCategoriaId = RoadmapCategoriaIntegracoesId,
            SituacaoAtual = "Fluxo implementado tecnicamente via Worker.Email, com criacao de chamado por e-mail, correlacao de respostas, tratamento de anexos e logs administrativos.",
            AtencaoTecnica = "Validar com caixa IMAP real, e-mails reais, anexos reais e regras de autenticacao exigidas pelo ambiente.",
            Status = StatusRoadmapItsm.Implementado,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.DesenvolverAgora,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 68,
            PendenciasTecnicas = "OAuth Microsoft (se exigido), retry/backoff, dead-letter, monitoramento do Worker, reprocessamento manual, sanitizacao avancada de HTML, antivirus de anexos e metricas/alertas operacionais.",
            PendenciasHomologacao = "Validacao com caixa IMAP real, homologacao com e-mails reais e validacao com anexos reais.",
            EvidenciaImplementacao = "Worker.Email; EmailWorkerOptions; LogIntegracaoEmail; ProcessarEmailRecebidoUseCase; EmailParaChamadoService; correlacao por assunto e headers; anexos por e-mail; endpoints de logs; tela /admin/integracoes/email; testes automatizados; docs/INTEGRACAO-EMAIL.md.",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "E-mail recebido na caixa configurada e processado pelo Worker, criando chamado com origem E-mail, status inicial, historico e vinculo com remetente. Respostas correlacionadas adicionam comentario ao chamado existente. Anexos permitidos sao tratados conforme regras de seguranca. Logs tecnicos ficam disponiveis na area administrativa.",
            ProximaAcao = "Validar com caixa IMAP real em homologacao.",
            Observacao = "Implementado funcionalmente; nao homologado e nao em producao sem validacao IMAP real.",
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 2,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem03Id,
            Area = "Perfis de acesso",
            Categoria = "Seguranca",
            Objetivo = "Controlar o acesso ao sistema por perfis e permissoes granulares, permitindo definir o que Administradores, Atendentes e Solicitantes podem visualizar, executar e administrar sem necessidade de alteracao de codigo.",
            RoadmapCategoriaId = RoadmapCategoriaSegurancaId,
            SituacaoAtual = "Perfis macro, permissoes granulares, matriz de permissoes, /api/me com permissoes efetivas e controle visual por permissao implementados.",
            AtencaoTecnica = "Validar permissoes finas por tela e acao durante homologacao e evoluir auditoria detalhada de alteracoes de permissoes.",
            Status = StatusRoadmapItsm.Implementado,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.DesenvolverAgora,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 90,
            PendenciasTecnicas = "Auditoria detalhada de alteracoes de permissoes, testes frontend/e2e da matriz de permissoes e validacao de permissoes finas em homologacao.",
            PendenciasHomologacao = "Validar com usuarios reais os perfis Administrador, Atendente e Solicitante, incluindo acoes permitidas e bloqueadas.",
            EvidenciaImplementacao = "docs/SEGURANCA-PERFIS-PERMISSOES.md; docs/ROADMAP.md; testes backend com permissoes; matriz de permissoes no frontend.",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "Administrador consegue gerenciar permissoes por perfil. Atendente visualiza apenas acoes permitidas. Solicitante nao acessa administracao. Backend bloqueia acoes sem permissao.",
            ProximaAcao = "Executar homologacao com usuarios reais e priorizar auditoria detalhada.",
            Observacao = "Status legado mantido para compatibilidade; usar StatusImplementacao como referencia principal.",
            Responsavel = "Thiago Tescaro",
            PrazoAlvo = new DateTime(2026, 5, 9, 0, 0, 0, DateTimeKind.Utc),
            Ordem = 3,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem04Id,
            Area = "Autenticação corporativa",
            Categoria = "Segurança",
            Objetivo = "Permitir que usuários acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autorização interna no SGX por usuários, perfis e permissões. O Azure autentica a identidade; o SGX controla o que cada usuário pode acessar e executar dentro do sistema.",
            RoadmapCategoriaId = RoadmapCategoriaSegurancaId,
            SituacaoAtual = "Fluxo de autenticação corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com suporte a validação de token JWT, modo Single Tenant, controle de domínio permitido, integração com GET /api/me, criação/identificação de usuário interno e autorização por perfis/permissões do SGX. Ainda depende de homologação com tenant institucional real.",
            AtencaoTecnica = "Manter a explicação clara: Microsoft Entra ID/Azure AD autentica; SGX autoriza. Não usar roles ou groups do Azure para conceder acesso administrativo automaticamente. Perfis e permissões continuam internos ao SGX. Validar MFA, Conditional Access, tenant real, redirect URI, API scope e ambiente publicado antes de considerar produção.",
            Status = StatusRoadmapItsm.Implementado,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.DesenvolverAgora,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 70,
            PendenciasTecnicas = "- Homologar com tenant institucional real do Microsoft Entra ID.\n- Validar login com usuários corporativos reais.\n- Validar MFA.\n- Validar Conditional Access.\n- Validar logout corporativo.\n- Validar ambiente publicado/VPS.\n- Revisar configuração com a equipe responsável pelo Azure.\n- Registrar evidências formais de homologação.\n- Avaliar persistência opcional de identificadores corporativos oid/tid, se necessário.\n- Definir governança de ciclo de vida do usuário interno: bloqueio, reativação e auditoria.",
            PendenciasHomologacao = "- Executar homologação ponta a ponta com usuário Administrador real.\n- Executar homologação ponta a ponta com usuário Atendente real.\n- Executar homologação ponta a ponta com usuário Solicitante real.\n- Validar comportamento com usuário interno inativo.\n- Validar bloqueio de domínio/tenant não permitido.\n- Validar mensagens de erro de login.\n- Validar redirecionamento por perfil/permissão após login.\n- Registrar evidências com prints, data, ambiente e usuário de teste.",
            EvidenciaImplementacao = "docs/AUTENTICACAO-CORPORATIVA.md; docs/CONFIGURACAO-AZURE-AD.md; docs/HOMOLOGACAO-CHECKLIST.md; docs/ROADMAP.md; docs/ROADMAP-ITSM.md; src/SGX.SistemaChamado.Api/Services/UsuarioAtualService.cs; src/SGX.SistemaChamado.Api/Extensions/ServiceCollectionExtensions.cs; src/SGX.SistemaChamado.Api/Options/AuthOptions.cs; src/SGX.SistemaChamado.Api/Options/AzureAdOptions.cs; src/SGX.SistemaChamado.Api/Options/AzureAdOptionsValidator.cs; src/SGX.SistemaChamado.Web/src/views/LoginView.vue; src/SGX.SistemaChamado.Web/src/services/authService.ts; src/SGX.SistemaChamado.Web/src/stores/authStore.ts; tests/SGX.SistemaChamado.Tests/UsuarioAtualServiceTests.cs; tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs; tests/SGX.SistemaChamado.Tests/AzureAdOptionsValidatorTests.cs",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "O usuário corporativo autentica pelo Microsoft Entra ID/Azure AD no tenant configurado. A API valida token, issuer, audience, tenant, expiração e assinatura. O SGX identifica ou cria o usuário interno conforme configuração permitida, bloqueia usuários inativos ou fora do tenant/domínio permitido, retorna perfis e permissões efetivas em GET /api/me e aplica autorização interna nas rotas e ações. Usuários Solicitante, Atendente e Administrador devem acessar apenas o que seus perfis/permissões internos permitem.",
            ProximaAcao = "Executar homologação com tenant institucional real do Microsoft Entra ID, validar MFA/Conditional Access, revisar configuração com a equipe Azure, testar usuários reais por perfil e anexar evidências formais antes de promoção para produção.",
            Observacao = "Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.",
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 4,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem05Id,
            Area = "SLA",
            Categoria = "Operacao",
            RoadmapCategoriaId = RoadmapCategoriaSlaId,
            SituacaoAtual = "Estrutura prevista com controle e configuracao",
            AtencaoTecnica = "Mostrar regra de prazo, pausa, resposta e encerramento",
            Status = StatusRoadmapItsm.Parcial,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 5,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem06Id,
            Area = "Historico/auditoria",
            Categoria = "Governanca",
            RoadmapCategoriaId = RoadmapCategoriaGovernancaId,
            SituacaoAtual = "Previsto com historico do chamado",
            AtencaoTecnica = "Garantir que mudancas relevantes sejam registradas",
            Status = StatusRoadmapItsm.Parcial,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 6,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem07Id,
            Area = "Comentarios e anexos",
            Categoria = "Operacao",
            RoadmapCategoriaId = RoadmapCategoriaAtendimentoId,
            SituacaoAtual = "Previsto",
            AtencaoTecnica = "Testar upload, download, visibilidade publica/interna",
            Status = StatusRoadmapItsm.EmValidacao,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 7,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem08Id,
            Area = "Cadastros administrativos",
            Categoria = "Administracao",
            RoadmapCategoriaId = RoadmapCategoriaCadastrosId,
            SituacaoAtual = "Categorias, prioridades, status e departamentos",
            AtencaoTecnica = "Verificar se permitem inativacao e parametrizacao",
            Status = StatusRoadmapItsm.EmValidacao,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Medio,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 8,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem09Id,
            Area = "Dashboard",
            Categoria = "Gestao",
            RoadmapCategoriaId = RoadmapCategoriaGestaoId,
            SituacaoAtual = "Previsto",
            AtencaoTecnica = "Levar indicadores simples: abertos, vencidos, por status e por atendente",
            Status = StatusRoadmapItsm.Parcial,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 9,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem10Id,
            Area = "Base de conhecimento",
            Categoria = "Conhecimento",
            RoadmapCategoriaId = RoadmapCategoriaConhecimentoId,
            SituacaoAtual = "Nao ha evidencia forte",
            AtencaoTecnica = "Pode ser GAP assumido para evolucao",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Medio,
            Decisao = DecisaoRoadmapItsm.PosValidacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 10,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem11Id,
            Area = "Inventario/ativos",
            Categoria = "Ativos",
            RoadmapCategoriaId = RoadmapCategoriaInfraestruturaId,
            SituacaoAtual = "Nao ha evidencia forte",
            AtencaoTecnica = "Nao prometer equivalencia com GLPI nesse ponto",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Baixa,
            Impacto = ImpactoRoadmapItsm.Medio,
            Decisao = DecisaoRoadmapItsm.PosValidacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 11,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem12Id,
            Area = "Catalogo de servicos",
            Categoria = "Catalogo",
            RoadmapCategoriaId = RoadmapCategoriaConhecimentoId,
            SituacaoAtual = "Parcial, via categorias/departamentos",
            AtencaoTecnica = "Pode precisar virar recurso mais formal",
            Status = StatusRoadmapItsm.Parcial,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.PosValidacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 12,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem13Id,
            Area = "Aprovacao de chamados",
            Categoria = "Workflow",
            RoadmapCategoriaId = RoadmapCategoriaAtendimentoId,
            SituacaoAtual = "Nao ha evidencia forte",
            AtencaoTecnica = "Tratar como melhoria futura se for exigencia",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Medio,
            Decisao = DecisaoRoadmapItsm.PosValidacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 13,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem14Id,
            Area = "Notificacoes",
            Categoria = "Comunicacao",
            RoadmapCategoriaId = RoadmapCategoriaNotificacoesId,
            SituacaoAtual = "Nao ficou suficientemente evidente",
            AtencaoTecnica = "Validar e/ou planejar e-mail/notificacao por evento",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 14,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = RoadmapItsmItem15Id,
            Area = "Relatorios avancados",
            Categoria = "Gestao",
            RoadmapCategoriaId = RoadmapCategoriaRelatoriosId,
            SituacaoAtual = "Nao ficou suficientemente evidente",
            AtencaoTecnica = "Planejar exportacao/filtros gerenciais",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Media,
            Impacto = ImpactoRoadmapItsm.Medio,
            Decisao = DecisaoRoadmapItsm.PosValidacao,
                        StatusImplementacao = StatusImplementacaoRoadmapItsm.NaoIniciado,
            StatusTecnico = StatusTecnicoRoadmapItsm.NaoAvaliado,
            PercentualImplementacao = 0,
            PendenciasTecnicas = (string?)null,
            PendenciasHomologacao = (string?)null,
            EvidenciaImplementacao = (string?)null,
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = (string?)null,
            ProximaAcao = (string?)null,
            Observacao = (string?)null,
            Responsavel = (string?)null,
            PrazoAlvo = (DateTime?)null,
            Ordem = 15,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    public static readonly object[] RoadmapChecklistItens =
    [
        new { Id = ChecklistPortalContextoValidadoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Endpoint de contexto do portal validado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalCriacaoValidadaId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Endpoint de criação de chamado validado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalValidacoesObrigatoriasId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validações obrigatórias implementadas", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalSolicitanteAutenticadoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Solicitante obtido pelo usuário autenticado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalStatusAbertoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Status inicial Aberto aplicado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalHistoricoInicialId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Histórico inicial criado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalTelaNovoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Tela /portal/chamados/novo implementada", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalSlaInicialId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Formulário com validação visual implementado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalUploadAnexoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Consumo de GET /api/portal/contexto implementado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalRedirectDetalheId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Consumo de POST /api/portal/chamados implementado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalListagemPortalId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Redirecionamento para detalhe após abertura implementado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalVisivelAdminId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Listagem /portal/chamados validada tecnicamente", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalDetalheValidadoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Detalhe /portal/chamados/:id validado tecnicamente", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalHistoricoVisivelId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Chamado visível na fila administrativa", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalAnexoVisivelPortalId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Detalhe administrativo do chamado validado tecnicamente", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalAnexoVisivelAdminId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Build backend validado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 16, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalTestesBackendId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Testes backend executados com sucesso", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 17, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalBuildFrontendId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Build frontend validado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 18, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalHomologacaoUsuarioRealId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Homologação manual com usuário real", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 19, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalE2eFrontendId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Testes E2E frontend do fluxo de abertura", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 20, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalValidacaoAnexosHomologacaoId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validação real de anexos em ambiente de homologação", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 21, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalAnexoInvalidoAmigavelId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validação de anexo inválido com mensagem amigável", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 22, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalFluxoCompletoAcompanharId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validação completa do fluxo abrir, anexar e acompanhar", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 23, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalSolicitanteEntraId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validação com perfil Solicitante real do Microsoft Entra ID", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 24, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPortalAtendenteFilaId, RoadmapItemId = RoadmapItsmItem01Id, Titulo = "Validação com Atendente visualizando o chamado na fila", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 25, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailWorkerProjetoValidadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Projeto Worker.Email validado/criado", Descricao = "Grupo solicitado: Worker", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailConfiguracoesImapDefinidasId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Configuracoes IMAP definidas", Descricao = "Grupo solicitado: Configuracao", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailLeituraImapImplementadaId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Leitura IMAP implementada", Descricao = "Grupo solicitado: Worker", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailProcessamentoLoteImplementadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Processamento em lote implementado", Descricao = "Grupo solicitado: Worker", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailLogIntegracaoImplementadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "LogIntegracaoEmail implementado", Descricao = "Grupo solicitado: Persistencia", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailDeduplicacaoMessageIdId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Prevencao de duplicidade por MessageId implementada", Descricao = "Grupo solicitado: Consistencia", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailNovoCriaChamadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "E-mail novo cria chamado", Descricao = "Grupo solicitado: Chamado", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailOrigemAplicadaId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Origem E-mail aplicada ao chamado", Descricao = "Grupo solicitado: Chamado", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailStatusAbertoAplicadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Status inicial Aberto aplicado", Descricao = "Grupo solicitado: Chamado", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailHistoricoInicialCriadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Historico inicial criado", Descricao = "Grupo solicitado: Historico", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailCorrelacaoCodigoAssuntoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Correlacao por codigo do chamado implementada", Descricao = "Grupo solicitado: Correlacao", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailCorrelacaoHeadersId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Correlacao por Message-Id/In-Reply-To implementada", Descricao = "Grupo solicitado: Correlacao", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailRespostaAdicionaComentarioId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Resposta por e-mail adiciona comentario", Descricao = "Grupo solicitado: Comentarios", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailAnexosValidadosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Anexos por e-mail validados", Descricao = "Grupo solicitado: Anexos", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailAnexosPermitidosSalvosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Anexos permitidos sao salvos", Descricao = "Grupo solicitado: Anexos", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailAnexosInvalidosRejeitadosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Anexos invalidos sao rejeitados e logados", Descricao = "Grupo solicitado: Anexos", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 16, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailEndpointLogsAdminId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Endpoint de logs administrativos implementado", Descricao = "Grupo solicitado: Admin", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 17, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTelaAdminValidadaId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Tela /admin/integracoes/email validada", Descricao = "Grupo solicitado: Frontend", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 18, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailFiltrosLogsImplementadosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Filtros de logs implementados", Descricao = "Grupo solicitado: Frontend", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 19, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailDetalheDialogImplementadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Detalhe de log em dialog implementado", Descricao = "Grupo solicitado: Frontend", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 20, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTestesProcessamentoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Testes unitarios de processamento criados", Descricao = "Grupo solicitado: Testes", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 21, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTestesCorrelacaoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Testes de correlacao criados", Descricao = "Grupo solicitado: Testes", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 22, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTestesAnexosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Testes de anexos criados", Descricao = "Grupo solicitado: Testes", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 23, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailBuildBackendValidadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Build backend validado", Descricao = "Grupo solicitado: Validacao", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 24, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTestesBackendExecutadosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Testes backend executados", Descricao = "Grupo solicitado: Validacao", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 25, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailBuildWorkerValidadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Build Worker validado", Descricao = "Grupo solicitado: Validacao", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 26, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailBuildFrontendValidadoId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Build frontend validado", Descricao = "Grupo solicitado: Validacao", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 27, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailValidacaoCaixaImapRealId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Validacao com caixa IMAP real", Descricao = "Grupo solicitado: Homologacao", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 28, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailHomologacaoEmailsReaisId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Homologacao com e-mails reais", Descricao = "Grupo solicitado: Homologacao", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 29, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailValidacaoAnexosReaisId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Validacao com anexos reais", Descricao = "Grupo solicitado: Homologacao", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 30, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailOauthMicrosoftId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Autenticacao OAuth para caixa Microsoft, se exigido", Descricao = "Grupo solicitado: Evolucao", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 31, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailRetryBackoffId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Retry/backoff em falhas temporarias", Descricao = "Grupo solicitado: Evolucao", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 32, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailDeadLetterId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Dead-letter ou fila de mensagens com erro", Descricao = "Grupo solicitado: Evolucao", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 33, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailMonitoramentoWorkerId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Monitoramento/health check do Worker", Descricao = "Grupo solicitado: Evolucao", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 34, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailReprocessamentoManualId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Painel de reprocessamento manual de e-mails com erro", Descricao = "Grupo solicitado: Evolucao", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 35, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailSanitizacaoHtmlAvancadaId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Sanitizacao avancada de HTML", Descricao = "Grupo solicitado: Seguranca", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 36, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailAntivirusAnexosId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Antivirus/varredura de anexos", Descricao = "Grupo solicitado: Seguranca", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 37, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailTesteE2eImapRealId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Teste E2E com IMAP real", Descricao = "Grupo solicitado: Testes", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 38, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailMetricasOperacionaisId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Metricas operacionais do Worker", Descricao = "Grupo solicitado: Observabilidade", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 39, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistEmailAlertasFalhaRecorrenteId, RoadmapItemId = RoadmapItsmItem02Id, Titulo = "Alertas de falha recorrente no processamento de e-mail", Descricao = "Grupo solicitado: Observabilidade", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 40, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPerfisMacroId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Perfis macro criados", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistCrudPerfisId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "CRUD de perfis criado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPermissoesGranularesId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Permissões granulares criadas", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistMigrationId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Migration aplicada", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSeedsId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Seeds criados", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistApiMePermissoesId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "/api/me com permissões", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAuthorizationHandlerId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "AuthorizationHandler criado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistMatrizFrontendId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Matriz de permissões no frontend", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistControleVisualId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Controle visual por permissão", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistHomologacaoUsuariosId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Homologação com usuários reais", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 10, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoArquiteturaDefinidaId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Decisão arquitetural documentada: Azure autentica, SGX autoriza.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoAppRegistrationDocumentadoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Login Microsoft revisado no frontend.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoVariaveisBackendDocumentadasId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validação JWT/API revisada.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoVariaveisFrontendDocumentadasId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "GET /api/me revisado.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoJwtBearerConfiguradoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "httpClient e tratamento de 401/403 revisados.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoClaimsMicrosoftMapeadasId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Router guards revisados.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoUsuarioLocalizadoPorEmailLoginId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Login local Development preservado.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoUsuarioNovoTratadoRegraSgxId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Emulação de perfis em Development preservada.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoApiMePerfisPermissoesId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Documentação técnica consolidada.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoLoginMicrosoftFrontendId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Authority, Issuer, Audience, expiração e assinatura validados.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoLoginLocalApenasDevelopmentId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "MetadataAddress opcional suportado.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoGuardsFuncionandoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Domínios permitidos configuráveis.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoPermissoesInternasPreservadasId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Criação automática de usuário interno configurável.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoRefreshSemLogoffIndevidoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Perfil padrão de usuário Microsoft configurável.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoUsuarioInativoBloqueadoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Claims Microsoft mapeadas com fallback.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoDocumentacaoAtualizadaId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Bloqueio por domínio não permitido.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 16, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoBuildBackendValidadoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Bloqueio de usuário interno inativo.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 17, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTestesBackendExecutadosId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Roles/groups do Azure não concedem Administrador automaticamente.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 18, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoBuildFrontendValidadoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Testes automatizados atualizados.", Descricao = "Checklist técnico concluído", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 19, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoHomologacaoTenantRealId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Configurar tenant institucional real.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 20, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTesteUsuarioRealDominioId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validar login com usuários corporativos reais.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 21, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTesteMfaId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validar MFA.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 22, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTesteConditionalAccessId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validar Conditional Access.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 23, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTesteLogoutCorporativoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validar logout corporativo.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 24, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoTesteAmbientePublicadoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Validar ambiente publicado/VPS.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 25, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoRevisaoEquipeAzureId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Revisar configuração com equipe responsável pelo Azure.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 26, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAutenticacaoEvidenciaFormalHomologacaoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Registrar evidências formais de homologação.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 27, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null }
    ];

    private static object[] CriarPerfisAcessoPermissoes()
    {
        var vinculos = new List<(Guid PerfilAcessoId, string CodigoPermissao)>();

        vinculos.AddRange(CatalogoPermissoesSistema.Select(x => (PerfilAdministradorId, x.Codigo)));
        vinculos.AddRange(CodigosPermissoesAtendente.Select(codigo => (PerfilAtendenteId, codigo)));
        vinculos.AddRange(CodigosPermissoesSolicitante.Select(codigo => (PerfilSolicitanteId, codigo)));

        return vinculos
            .Distinct()
            .Select((vinculo, indice) => new
            {
                Id = GerarIdPerfilPermissao(indice + 1),
                PerfilAcessoId = vinculo.PerfilAcessoId,
                PermissaoSistemaId = PermissoesSistemaPorCodigo[vinculo.CodigoPermissao],
                CriadoEm = DataBase,
                CriadoPor = UsuarioSistema
            })
            .ToArray();
    }

    private static Guid GerarIdPerfilPermissao(int indice)
        => Guid.Parse($"99999999-9999-9999-9999-999999999{indice:000}");
}




