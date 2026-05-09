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
        (PermissaoRoadmapImplementacoesGerenciarId, "RoadmapImplementacoes.Gerenciar")
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
            RoadmapCategoriaId = RoadmapCategoriaPortalId,
            SituacaoAtual = "Prevista no portal /portal",
            AtencaoTecnica = "Demonstrar fluxo completo: abrir, anexar, acompanhar",
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
            Categoria = "Integracao",
            RoadmapCategoriaId = RoadmapCategoriaIntegracoesId,
            SituacaoAtual = "Prevista via Worker IMAP",
            AtencaoTecnica = "Testar e mostrar correlacao por codigo, assunto e resposta",
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
            RoadmapCategoriaId = RoadmapCategoriaSegurancaId,
            SituacaoAtual = "Administrador, Atendente e Solicitante",
            AtencaoTecnica = "Validar permissoes finas por tela e acao",
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
            Area = "Autenticacao corporativa",
            Categoria = "Seguranca",
            RoadmapCategoriaId = RoadmapCategoriaSegurancaId,
            SituacaoAtual = "Entra ID/Azure AD previsto",
            AtencaoTecnica = "Preparar explicacao clara: Azure autentica, SGX autoriza",
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
        new { Id = ChecklistPerfisMacroId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Perfis macro criados", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Planejamento, Ordem = 1, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistCrudPerfisId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "CRUD de perfis criado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 2, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistPermissoesGranularesId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Permissões granulares criadas", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 3, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistMigrationId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Migration aplicada", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 4, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistSeedsId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Seeds criados", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistApiMePermissoesId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "/api/me com permissões", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Testes, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistAuthorizationHandlerId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "AuthorizationHandler criado", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistMatrizFrontendId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Matriz de permissões no frontend", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistControleVisualId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Controle visual por permissão", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null },
        new { Id = ChecklistHomologacaoUsuariosId, RoadmapItemId = RoadmapItsmItem03Id, Titulo = "Homologação com usuários reais", Descricao = (string?)null, Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 10, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = (DateTime?)null, AtualizadoPor = (string?)null }
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


