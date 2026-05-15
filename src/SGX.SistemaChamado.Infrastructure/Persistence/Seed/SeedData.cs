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
    public static readonly Guid PermissaoSlaVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888836");
    public static readonly Guid PermissaoSlaCriarId = Guid.Parse("88888888-8888-8888-8888-888888888837");
    public static readonly Guid PermissaoSlaEditarId = Guid.Parse("88888888-8888-8888-8888-888888888838");
    public static readonly Guid PermissaoSlaExcluirId = Guid.Parse("88888888-8888-8888-8888-888888888839");
    public static readonly Guid PermissaoSlaAtivarDesativarId = Guid.Parse("88888888-8888-8888-8888-888888888840");
    public static readonly Guid PermissaoAuditoriaVisualizarId = Guid.Parse("88888888-8888-8888-8888-888888888841");
    public static readonly Guid PermissaoAuditoriaGerenciarId = Guid.Parse("88888888-8888-8888-8888-888888888842");

    public static readonly Guid StatusAbertoId = Guid.Parse("44444444-4444-4444-4444-444444444441");
    public static readonly Guid StatusEmAtendimentoId = Guid.Parse("44444444-4444-4444-4444-444444444442");
    public static readonly Guid StatusAguardandoSolicitanteId = Guid.Parse("44444444-4444-4444-4444-444444444443");
    public static readonly Guid StatusResolvidoId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid StatusEncerradoId = Guid.Parse("44444444-4444-4444-4444-444444444445");

    public static readonly Guid PrioridadeBaixaId = Guid.Parse("55555555-5555-5555-5555-555555555551");
    public static readonly Guid PrioridadeMediaId = Guid.Parse("55555555-5555-5555-5555-555555555552");
    public static readonly Guid PrioridadeAltaId = Guid.Parse("55555555-5555-5555-5555-555555555553");
    public static readonly Guid PrioridadeCriticaId = Guid.Parse("55555555-5555-5555-5555-555555555554");
    public static readonly Guid SlaPoliticaPadraoId = Guid.Parse("56565656-5656-5656-5656-565656565601");
    public static readonly Guid SlaMetaPadraoBaixaId = Guid.Parse("56565656-5656-5656-5656-565656565611");
    public static readonly Guid SlaMetaPadraoMediaId = Guid.Parse("56565656-5656-5656-5656-565656565612");
    public static readonly Guid SlaMetaPadraoAltaId = Guid.Parse("56565656-5656-5656-5656-565656565613");
    public static readonly Guid SlaMetaPadraoCriticaId = Guid.Parse("56565656-5656-5656-5656-565656565614");
    public static readonly Guid ConfiguracaoAlertaSlaPadraoId = Guid.Parse("56565656-5656-5656-5656-565656565621");
    public static readonly Guid CalendarioCorporativoPadraoId = Guid.Parse("56565656-5656-5656-5656-565656565701");
    public static readonly Guid HorarioCalendarioPadraoSegundaId = Guid.Parse("56565656-5656-5656-5656-565656565711");
    public static readonly Guid HorarioCalendarioPadraoTercaId = Guid.Parse("56565656-5656-5656-5656-565656565712");
    public static readonly Guid HorarioCalendarioPadraoQuartaId = Guid.Parse("56565656-5656-5656-5656-565656565713");
    public static readonly Guid HorarioCalendarioPadraoQuintaId = Guid.Parse("56565656-5656-5656-5656-565656565714");
    public static readonly Guid HorarioCalendarioPadraoSextaId = Guid.Parse("56565656-5656-5656-5656-565656565715");

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
    public static readonly Guid ChecklistSlaPoliticaCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707701");
    public static readonly Guid ChecklistSlaMetasCriadasId = Guid.Parse("70707070-7070-7070-7070-707070707702");
    public static readonly Guid ChecklistSlaMigrationCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707703");
    public static readonly Guid ChecklistSlaSeedPadraoCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707704");
    public static readonly Guid ChecklistSlaDtosCriadosId = Guid.Parse("70707070-7070-7070-7070-707070707705");
    public static readonly Guid ChecklistSlaServiceCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707706");
    public static readonly Guid ChecklistSlaEndpointsCriadosId = Guid.Parse("70707070-7070-7070-7070-707070707707");
    public static readonly Guid ChecklistSlaPermissoesCriadasId = Guid.Parse("70707070-7070-7070-7070-707070707708");
    public static readonly Guid ChecklistSlaTelaCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707709");
    public static readonly Guid ChecklistSlaValidacoesCriadasId = Guid.Parse("70707070-7070-7070-7070-707070707710");
    public static readonly Guid ChecklistSlaTestesServiceCriadosId = Guid.Parse("70707070-7070-7070-7070-707070707711");
    public static readonly Guid ChecklistSlaTestesEndpointsCriadosId = Guid.Parse("70707070-7070-7070-7070-707070707712");
    public static readonly Guid ChecklistSlaDocumentacaoCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707713");
    public static readonly Guid ChecklistSlaSprint2TabelaCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707714");
    public static readonly Guid ChecklistSlaSprint2RelacionamentoCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707715");
    public static readonly Guid ChecklistSlaSprint2ServiceCalculoCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707716");
    public static readonly Guid ChecklistSlaSprint2PoliticaAplicavelId = Guid.Parse("70707070-7070-7070-7070-707070707717");
    public static readonly Guid ChecklistSlaSprint2AplicacaoCriacaoId = Guid.Parse("70707070-7070-7070-7070-707070707718");
    public static readonly Guid ChecklistSlaSprint2PrazoPrimeiraRespostaId = Guid.Parse("70707070-7070-7070-7070-707070707719");
    public static readonly Guid ChecklistSlaSprint2PrazoResolucaoId = Guid.Parse("70707070-7070-7070-7070-707070707720");
    public static readonly Guid ChecklistSlaSprint2RegistroPrimeiraRespostaId = Guid.Parse("70707070-7070-7070-7070-707070707721");
    public static readonly Guid ChecklistSlaSprint2RegistroResolucaoId = Guid.Parse("70707070-7070-7070-7070-707070707722");
    public static readonly Guid ChecklistSlaSprint2PausaId = Guid.Parse("70707070-7070-7070-7070-707070707723");
    public static readonly Guid ChecklistSlaSprint2SituacaoAtualId = Guid.Parse("70707070-7070-7070-7070-707070707724");
    public static readonly Guid ChecklistSlaSprint2DetalheChamadoId = Guid.Parse("70707070-7070-7070-7070-707070707725");
    public static readonly Guid ChecklistSlaSprint2ListagemAdminId = Guid.Parse("70707070-7070-7070-7070-707070707726");
    public static readonly Guid ChecklistSlaSprint2FiltrosAdminId = Guid.Parse("70707070-7070-7070-7070-707070707727");
    public static readonly Guid ChecklistSlaSprint2DtosAtualizadosId = Guid.Parse("70707070-7070-7070-7070-707070707728");
    public static readonly Guid ChecklistSlaSprint2TestesId = Guid.Parse("70707070-7070-7070-7070-707070707729");
    public static readonly Guid ChecklistSlaSprint2DocumentacaoId = Guid.Parse("70707070-7070-7070-7070-707070707730");
    public static readonly Guid ChecklistSlaSprint4CalendarioCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707752");
    public static readonly Guid ChecklistSlaSprint4HorarioCriadoId = Guid.Parse("70707070-7070-7070-7070-707070707753");
    public static readonly Guid ChecklistSlaSprint4ExcecaoCriadaId = Guid.Parse("70707070-7070-7070-7070-707070707754");
    public static readonly Guid ChecklistSlaSprint4MigrationsId = Guid.Parse("70707070-7070-7070-7070-707070707755");
    public static readonly Guid ChecklistSlaSprint4SeedPadraoId = Guid.Parse("70707070-7070-7070-7070-707070707756");
    public static readonly Guid ChecklistSlaSprint4RelacionamentoPoliticaId = Guid.Parse("70707070-7070-7070-7070-707070707757");
    public static readonly Guid ChecklistSlaSprint4ServiceAdminId = Guid.Parse("70707070-7070-7070-7070-707070707758");
    public static readonly Guid ChecklistSlaSprint4ServiceTempoUtilId = Guid.Parse("70707070-7070-7070-7070-707070707759");
    public static readonly Guid ChecklistSlaSprint4PrazoPrimeiraRespostaId = Guid.Parse("70707070-7070-7070-7070-707070707760");
    public static readonly Guid ChecklistSlaSprint4PrazoResolucaoId = Guid.Parse("70707070-7070-7070-7070-707070707761");
    public static readonly Guid ChecklistSlaSprint4MinutosPrimeiraRespostaId = Guid.Parse("70707070-7070-7070-7070-707070707762");
    public static readonly Guid ChecklistSlaSprint4MinutosResolucaoId = Guid.Parse("70707070-7070-7070-7070-707070707763");
    public static readonly Guid ChecklistSlaSprint4EndpointsId = Guid.Parse("70707070-7070-7070-7070-707070707764");
    public static readonly Guid ChecklistSlaSprint4TelaCalendariosId = Guid.Parse("70707070-7070-7070-7070-707070707765");
    public static readonly Guid ChecklistSlaSprint4TelaPoliticaId = Guid.Parse("70707070-7070-7070-7070-707070707766");
    public static readonly Guid ChecklistSlaSprint4DetalheChamadoId = Guid.Parse("70707070-7070-7070-7070-707070707767");
    public static readonly Guid ChecklistSlaSprint4TestesId = Guid.Parse("70707070-7070-7070-7070-707070707768");
    public static readonly Guid ChecklistSlaSprint4DocumentacaoId = Guid.Parse("70707070-7070-7070-7070-707070707769");

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
        (PermissaoUsuariosRedefinirSenhaId, "Usuarios.RedefinirSenha"),
        (PermissaoSlaVisualizarId, "Sla.Visualizar"),
        (PermissaoSlaCriarId, "Sla.Criar"),
        (PermissaoSlaEditarId, "Sla.Editar"),
        (PermissaoSlaExcluirId, "Sla.Excluir"),
        (PermissaoSlaAtivarDesativarId, "Sla.AtivarDesativar"),
        (PermissaoAuditoriaVisualizarId, "Auditoria.Visualizar"),
        (PermissaoAuditoriaGerenciarId, "Auditoria.Gerenciar")
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
        "RoadmapImplementacoes.Visualizar",
        "Sla.Visualizar"
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

    public static readonly object[] SlaPoliticas =
    [
        new
        {
            Id = SlaPoliticaPadraoId,
            Nome = "SLA Padrão",
            Descricao = "Política inicial de SLA do SGX Sistema de Chamados, usada como base para controle de primeira resposta e resolução dos chamados.",
            Ordem = 1,
            CategoriaId = (Guid?)null,
            DepartamentoId = (Guid?)null,
            CalendarioCorporativoId = (Guid?)null,
            UsarHorarioComercial = false,
            PausarQuandoAguardandoSolicitante = true,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    public static readonly object[] CalendariosCorporativos =
    [
        new
        {
            Id = CalendarioCorporativoPadraoId,
            Nome = "Calendário Corporativo Padrão",
            Descricao = "Calendário inicial para cálculo de SLA em horário comercial.",
            Padrao = true,
            TimeZone = "America/Sao_Paulo",
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

    public static readonly object[] HorariosAtendimentoCalendario =
    [
        CriarHorarioCalendarioSeed(HorarioCalendarioPadraoSegundaId, DayOfWeek.Monday),
        CriarHorarioCalendarioSeed(HorarioCalendarioPadraoTercaId, DayOfWeek.Tuesday),
        CriarHorarioCalendarioSeed(HorarioCalendarioPadraoQuartaId, DayOfWeek.Wednesday),
        CriarHorarioCalendarioSeed(HorarioCalendarioPadraoQuintaId, DayOfWeek.Thursday),
        CriarHorarioCalendarioSeed(HorarioCalendarioPadraoSextaId, DayOfWeek.Friday)
    ];

    public static readonly object[] SlaMetas =
    [
        new
        {
            Id = SlaMetaPadraoBaixaId,
            PoliticaSlaId = SlaPoliticaPadraoId,
            PrioridadeId = PrioridadeBaixaId,
            TempoPrimeiraRespostaMinutos = 480,
            TempoResolucaoMinutos = 2880,
            TempoAtualizacaoMinutos = (int?)null,
            TempoRespostaSubsequenteMinutos = (int?)null,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = SlaMetaPadraoMediaId,
            PoliticaSlaId = SlaPoliticaPadraoId,
            PrioridadeId = PrioridadeMediaId,
            TempoPrimeiraRespostaMinutos = 240,
            TempoResolucaoMinutos = 1440,
            TempoAtualizacaoMinutos = (int?)null,
            TempoRespostaSubsequenteMinutos = (int?)null,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = SlaMetaPadraoAltaId,
            PoliticaSlaId = SlaPoliticaPadraoId,
            PrioridadeId = PrioridadeAltaId,
            TempoPrimeiraRespostaMinutos = 60,
            TempoResolucaoMinutos = 480,
            TempoAtualizacaoMinutos = (int?)null,
            TempoRespostaSubsequenteMinutos = (int?)null,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        },
        new
        {
            Id = SlaMetaPadraoCriticaId,
            PoliticaSlaId = SlaPoliticaPadraoId,
            PrioridadeId = PrioridadeCriticaId,
            TempoPrimeiraRespostaMinutos = 30,
            TempoResolucaoMinutos = 240,
            TempoAtualizacaoMinutos = (int?)null,
            TempoRespostaSubsequenteMinutos = (int?)null,
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        }
    ];

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
            Categoria = "SLA",
            Objetivo = "Permitir que o SGX Sistema de Chamados controle acordos de nível de serviço para chamados, definindo prazos de primeira resposta, atendimento e resolução conforme prioridade, categoria, departamento, tipo de solicitação e regras institucionais. O SLA deve apoiar gestão operacional, rastreabilidade, cobrança interna, indicadores e melhoria contínua do atendimento.",
            RoadmapCategoriaId = RoadmapCategoriaSlaId,
            SituacaoAtual = "Sprints 1, 2, 3 e 4 implementadas e validadas funcionalmente, com políticas/metas, SLA aplicado aos chamados, alertas, eventos, monitoramento, painel gerencial e calendário corporativo para horário comercial.",
            AtencaoTecnica = "O SLA não deve ser apenas um campo manual no chamado. Deve existir uma regra centralizada e auditável para cálculo de prazo. O sistema deve considerar prioridade, categoria, departamento responsável, horário útil, feriados, pausas/suspensões, reabertura de chamado e mudança de status. Evitar cálculo duplicado no frontend. A regra principal deve ficar no backend, com persistência dos marcos calculados no chamado para rastreabilidade.",
            Status = StatusRoadmapItsm.Pendente,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 100,
            PendenciasTecnicas = "- Validar cálculo de horário comercial em cenário real com volume institucional.\n- Evoluir calendário por departamento/time quando a governança estiver definida.\n- Evoluir importação automática de feriados nacionais/municipais.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar política de proximidade do vencimento por canal/time.\n- Implementar alertas/notificações operacionais por SLA, se aplicável.\n- Consolidar trilha de auditoria e relatórios gerenciais de cumprimento.",
            PendenciasHomologacao = "- Homologar cadastro de política de SLA.\n- Homologar abertura de chamado com cálculo automático de SLA.\n- Homologar SLA por prioridade.\n- Homologar SLA por categoria.\n- Homologar SLA por departamento responsável.\n- Homologar cálculo de vencimento com horário útil.\n- Homologar comportamento em chamado pausado ou aguardando solicitante.\n- Homologar comportamento em chamado reaberto.\n- Homologar exibição do SLA para atendente.\n- Homologar exibição do SLA para administrador/gestor.\n- Homologar filtros de chamados atrasados.\n- Homologar indicadores gerenciais.\n- Registrar evidências formais com prints, data, ambiente e usuário de teste.",
            EvidenciaImplementacao = "- docs/SLA.md\n- src/SGX.SistemaChamado.Domain/Entities/PoliticaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/MetaSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/ChamadoSla.cs\n- src/SGX.SistemaChamado.Domain/Entities/CalendarioCorporativo.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaService.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaCalculator.cs\n- src/SGX.SistemaChamado.Application/Services/Sla/SlaBusinessTimeCalculator.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaPoliciesController.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminSlaCalendarsController.cs\n- tests/SGX.SistemaChamado.Tests/SlaServiceTests.cs",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "O sistema deve permitir cadastrar políticas de SLA e aplicá-las automaticamente aos chamados conforme as regras configuradas. Ao abrir ou atualizar um chamado, o backend deve calcular e persistir os prazos de primeira resposta, atendimento e/ou resolução, considerando prioridade, categoria, departamento, horário útil e regras de pausa/reabertura quando aplicável. O detalhe do chamado deve exibir o status do SLA de forma clara: dentro do prazo, próximo do vencimento, vencido ou suspenso. Administradores e gestores devem conseguir filtrar e acompanhar chamados por situação de SLA. O cálculo deve ser testável, centralizado no backend e validado por testes automatizados.",
            ProximaAcao = "Executar homologação funcional de ponta a ponta com usuários reais e validar regras de SLA em ambiente publicado, incluindo casos de pausa, reabertura e governança operacional.",
            Observacao = "Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.",
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
            Area = "Histórico/Auditoria",
            Categoria = "Governança",
            Objetivo = "Criar trilha de auditoria para registrar ações relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governança, análise de alterações, auditoria operacional e apoio à homologação.",
            RoadmapCategoriaId = RoadmapCategoriaGovernancaId,
            SituacaoAtual = "Base técnica de auditoria criada, eventos de auditoria aplicados aos módulos críticos e tela administrativa de consulta implementada em Admin > Governança > Auditoria, com filtros, detalhe, indicadores e documentação em Gestão ITSM.",
            AtencaoTecnica = "Auditoria não é log técnico. ILogger continua para diagnóstico técnico; EventoAuditoria é governança/rastreabilidade. Não registrar senha, token JWT, refresh token, access token, client secret ou connection string. Dados sensíveis devem ser mascarados pelo AuditoriaDiffHelper.",
            Status = StatusRoadmapItsm.Parcial,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.AguardandoAvaliacao,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnico = StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            PercentualImplementacao = 100,
            PendenciasTecnicas = "- Exportação Excel/PDF.\n- Retenção configurável de auditoria.\n- Assinatura/hash da trilha de auditoria.\n- Alertas para eventos críticos.\n- Painel avançado de segurança.\n- Integração SIEM/Log Analytics.\n- Política de anonimização/LGPD para eventos antigos.\n- Fluxo backend controlado de logout, se vier a existir.\n- Auditoria de edição de documentação ITSM, caso a documentação deixe de ser estática.",
            PendenciasHomologacao = "- Executar homologação funcional com eventos reais em eventos_auditoria cobrindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM.\n- Validar filtros e consulta administrativa em Admin > Governança > Auditoria com evidências formais.",
            EvidenciaImplementacao = "- src/SGX.SistemaChamado.Domain/Entities/EventoAuditoria.cs\n- src/SGX.SistemaChamado.Domain/Enums/TipoAcaoAuditoria.cs\n- src/SGX.SistemaChamado.Domain/Enums/NivelAuditoria.cs\n- src/SGX.SistemaChamado.Application/Interfaces/Auditoria/IAuditoriaService.cs\n- src/SGX.SistemaChamado.Application/Interfaces/Auditoria/IAuditoriaContextProvider.cs\n- src/SGX.SistemaChamado.Infrastructure/Services/AuditoriaService.cs\n- src/SGX.SistemaChamado.Api/Services/AuditoriaContextProvider.cs\n- src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/EventoAuditoriaConfiguration.cs\n- src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260514215515_AddEventosAuditoriaGovernancaSprint1.cs\n- src/SGX.SistemaChamado.Application/Helpers/AuditoriaDiffHelper.cs\n- src/SGX.SistemaChamado.Api/Controllers/AdminAuditoriaController.cs\n- src/SGX.SistemaChamado.Web/src/views/AuditoriaAdminView.vue\n- src/SGX.SistemaChamado.Web/src/services/auditoriaService.ts\n- src/SGX.SistemaChamado.Web/src/types/auditoria.ts\n- src/SGX.SistemaChamado.Web/src/content/gestaoItsmDocs.ts\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md\n- README.md\n- tests/SGX.SistemaChamado.Tests/AuditoriaServiceTests.cs\n- tests/SGX.SistemaChamado.Tests/AuditoriaDiffHelperTests.cs\n- tests/SGX.SistemaChamado.Tests/AuditoriaModulosCriticosTests.cs",
            DataConclusaoTecnica = (DateTime?)null,
            DataHomologacao = (DateTime?)null,
            CriterioAceite = "O item Histórico/Auditoria deve exibir checklist ativo completo das Sprints 1, 2 e 3 com cálculo automático de percentual por checklist, status da implementação Implementado funcionalmente e status técnico Completo com pendências evolutivas, sem uso de percentual legado/manual.",
            ProximaAcao = "Executar homologação funcional com eventos reais em eventos_auditoria, incluindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM. Validar filtros e consulta administrativa em Admin > Governança > Auditoria.",
            Observacao = "Status legado mantido para compatibilidade; o status real deve considerar StatusImplementacao, StatusTecnico e checklist ativo.",
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
        new { Id = ChecklistAutenticacaoEvidenciaFormalHomologacaoId, RoadmapItemId = RoadmapItsmItem04Id, Titulo = "Registrar evidências formais de homologação.", Descricao = "Checklist pendente de homologação/governança", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 27, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaPoliticaCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Entidade de política de SLA criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaMetasCriadasId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Entidade de metas de SLA criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaMigrationCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Migration das tabelas de SLA criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSeedPadraoCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Seed inicial de SLA padrão criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaDtosCriadosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "DTOs de SLA criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaServiceCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Service de SLA criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaEndpointsCriadosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Endpoints administrativos criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaPermissoesCriadasId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Permissões administrativas de SLA criadas.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaTelaCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Tela administrativa básica criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaValidacoesCriadasId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Validações de duplicidade e campos obrigatórios criadas.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaTestesServiceCriadosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Testes automatizados da camada de service criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaTestesEndpointsCriadosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Testes de endpoints administrativos criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaDocumentacaoCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Documentação técnica inicial criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2TabelaCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Tabela de SLA aplicado ao chamado criada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2RelacionamentoCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Relacionamento entre chamado e SLA criado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2ServiceCalculoCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Service de cálculo de SLA criado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 16, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2PoliticaAplicavelId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Política aplicável identificada por prioridade/categoria/departamento.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 17, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2AplicacaoCriacaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "SLA aplicado na criação do chamado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 18, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2PrazoPrimeiraRespostaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Prazo de primeira resposta calculado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 19, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2PrazoResolucaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Prazo de resolução calculado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 20, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2RegistroPrimeiraRespostaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Primeira resposta registrada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 21, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2RegistroResolucaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Resolução registrada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 22, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2PausaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Pausa de SLA preparada ou implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 23, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2SituacaoAtualId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Situação atual do SLA calculada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 24, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2DetalheChamadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "SLA exibido no detalhe do chamado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 25, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2ListagemAdminId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "SLA exibido na listagem administrativa.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 26, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2FiltrosAdminId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Filtros administrativos de SLA criados.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 27, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2DtosAtualizadosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "DTOs de chamado atualizados com resumo de SLA.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 28, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2TestesId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Testes automatizados criados.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 29, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint2DocumentacaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Documentação atualizada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 30, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707731"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Configuração de alerta de SLA criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 31, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707732"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Tela administrativa de configuração de alerta criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 32, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707733"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Endpoints de configuração de alerta criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 33, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707734"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Job de verificação de SLA criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 34, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707735"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Periodicidade configurável por appsettings criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 35, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707736"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Controle contra notificações/eventos duplicados criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 36, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707737"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Histórico de eventos de SLA criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 37, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707738"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolução, pausa e retomada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 38, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707739"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Painel de indicadores de SLA criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 39, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707740"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicador de SLA vencido criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 40, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707741"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicador de SLA próximo do vencimento criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 41, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707742"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicador de percentual de cumprimento criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 42, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707743"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Métrica de tempo médio de primeira resposta criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 43, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707744"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Métrica de tempo médio de resolução criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 44, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707745"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicadores por prioridade criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 45, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707746"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicadores por categoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 46, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707747"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Indicadores por departamento criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 47, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707748"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Histórico de SLA exibido no detalhe administrativo do chamado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 48, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707749"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Estrutura preparada para exportação futura.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 49, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707750"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Documentação atualizada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 50, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("70707070-7070-7070-7070-707070707751"), RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Testes automatizados criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 51, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4CalendarioCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Entidade CalendarioCorporativo criada.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 52, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4HorarioCriadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Entidade HorarioAtendimentoCalendario criada.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 53, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4ExcecaoCriadaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Entidade ExcecaoCalendarioCorporativo criada.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 54, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4MigrationsId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Migrations de calendário criadas.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 55, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4SeedPadraoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Seed do calendário padrão criado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 56, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4RelacionamentoPoliticaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Relacionamento entre Política SLA e Calendário criado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 57, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4ServiceAdminId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Service administrativo de calendário criado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 58, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4ServiceTempoUtilId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Service de cálculo de tempo útil criado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 59, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4PrazoPrimeiraRespostaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Cálculo de prazo de primeira resposta usando horário comercial implementado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 60, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4PrazoResolucaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Cálculo de prazo de resolução usando horário comercial implementado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 61, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4MinutosPrimeiraRespostaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Cálculo de minutos úteis de primeira resposta implementado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 62, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4MinutosResolucaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Cálculo de minutos úteis de resolução implementado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 63, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4EndpointsId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Endpoints administrativos de calendário criados.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 64, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4TelaCalendariosId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Tela Admin > SLA > Calendários criada.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 65, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4TelaPoliticaId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Tela de política SLA atualizada com seleção de calendário.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 66, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4DetalheChamadoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Detalhe do chamado mostra tipo de cálculo e calendário usado.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 67, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4TestesId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Testes automatizados criados.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 68, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSlaSprint4DocumentacaoId, RoadmapItemId = RoadmapItsmItem05Id, Titulo = "Documentação atualizada.", Descricao = "Checklist da Sprint 4", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 69, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000001"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Entidade EventoAuditoria criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000002"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Enum de ação de auditoria criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000003"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Enum de nível de auditoria criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000004"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Migration da tabela eventos_auditoria criada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000005"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Índices de consulta criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000006"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Service centralizado de auditoria criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000007"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Context provider de auditoria criado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000008"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Captura de usuário atual integrada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000009"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Captura de IP e User-Agent integrada.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000a"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Registro de login integrado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000b"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Registro de logout avaliado e documentado como não aplicável enquanto não houver fluxo backend controlado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000c"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Registro de criação/edição/inativação de usuário integrado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000d"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Registro de perfis/permissões integrado.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000e"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "DTOs de auditoria criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000000f"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Testes automatizados criados.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000010"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Documentação atualizada em Gestão ITSM.", Descricao = "Checklist da Sprint 1", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 16, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000011"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Helper de diff antes/depois criado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 17, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000012"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Mascaramento de dados sensíveis implementado.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 18, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000013"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de abertura de chamado implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 19, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000014"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de alteração de status implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 20, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000015"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de alteração de prioridade implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 21, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000016"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de alteração de categoria implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 22, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000017"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de atribuição de responsável implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 23, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000018"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de assumir chamado implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 24, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000019"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de comentários administrativos implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 25, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001a"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de encerramento/resolução implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 26, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001b"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de reabertura implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 27, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001c"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de anexos preparada ou implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 28, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001d"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de usuários revisada e complementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 29, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001e"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de perfis revisada e complementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 30, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000001f"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de permissões revisada e complementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 31, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000020"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de políticas de SLA implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 32, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000021"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de metas de SLA implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 33, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000022"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de calendários de SLA implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 34, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000023"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de horários de calendário implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 35, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000024"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de exceções de calendário implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 36, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000025"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de alertas de SLA implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 37, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000026"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de autenticação corporativa implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 38, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000027"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de Roadmap ITSM implementada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 39, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000028"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Auditoria de documentação ITSM preparada conforme estrutura atual estática.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 40, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000029"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Testes automatizados de auditoria dos módulos críticos criados.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 41, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002a"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Documentação atualizada em Gestão ITSM.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 42, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002b"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Validação no banco confirmando eventos reais em eventos_auditoria preparada/executada.", Descricao = "Checklist da Sprint 2", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 43, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002c"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Endpoints administrativos de auditoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 44, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002d"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Use cases/services de consulta de auditoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 45, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002e"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Filtros de auditoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 46, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000002f"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Paginação de eventos criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 47, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000030"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Endpoint de detalhe de evento criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 48, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000031"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Endpoint de dashboard de auditoria criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 49, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000032"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Permissões de auditoria criadas ou integradas.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 50, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000033"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Menu Governança > Auditoria criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 51, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000034"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Rota /admin/governanca/auditoria criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 52, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000035"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Tela administrativa de auditoria criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 53, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000036"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Modal/drawer de detalhe criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 54, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000037"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Visualização de dados antes/depois criada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 55, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000038"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Indicadores básicos de auditoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 56, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-000000000039"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Service frontend de auditoria criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 57, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003a"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Tipos frontend de auditoria criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 58, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003b"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Link entre Auditoria e Gestão ITSM criado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 59, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003c"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Documentação em Gestão ITSM atualizada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Documentacao, Ordem = 60, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003d"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Testes automatizados backend criados.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 61, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003e"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Build frontend validado.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 62, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = Guid.Parse("71717171-7171-7171-7171-00000000003f"), RoadmapItemId = RoadmapItsmItem06Id, Titulo = "Validação com eventos reais em eventos_auditoria executada.", Descricao = "Checklist da Sprint 3", Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 63, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null }
    ];

    private static object CriarHorarioCalendarioSeed(Guid id, DayOfWeek diaSemana)
        => new
        {
            Id = id,
            CalendarioCorporativoId = CalendarioCorporativoPadraoId,
            DiaSemana = diaSemana,
            HoraInicio = new TimeOnly(9, 0),
            HoraFim = new TimeOnly(18, 0),
            Ativo = true,
            CriadoEm = DataBase,
            CriadoPor = UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null
        };

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




