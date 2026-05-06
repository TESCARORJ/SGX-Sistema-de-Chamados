using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static readonly DateTime DataBase = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public const string UsuarioSistema = "seed.sistema";

    public static readonly Guid PerfilAdministradorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid PerfilAtendenteId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid PerfilSolicitanteId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid StatusAbertoId = Guid.Parse("44444444-4444-4444-4444-444444444441");
    public static readonly Guid StatusEmAtendimentoId = Guid.Parse("44444444-4444-4444-4444-444444444442");
    public static readonly Guid StatusAguardandoSolicitanteId = Guid.Parse("44444444-4444-4444-4444-444444444443");
    public static readonly Guid StatusResolvidoId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid StatusEncerradoId = Guid.Parse("44444444-4444-4444-4444-444444444445");

    public static readonly Guid PrioridadeBaixaId = Guid.Parse("55555555-5555-5555-5555-555555555551");
    public static readonly Guid PrioridadeMediaId = Guid.Parse("55555555-5555-5555-5555-555555555552");
    public static readonly Guid PrioridadeAltaId = Guid.Parse("55555555-5555-5555-5555-555555555553");
    public static readonly Guid PrioridadeCriticaId = Guid.Parse("55555555-5555-5555-5555-555555555554");

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
}
