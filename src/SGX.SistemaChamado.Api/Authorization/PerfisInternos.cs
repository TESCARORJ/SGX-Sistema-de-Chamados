namespace SGX.SistemaChamado.Api.Authorization;

public static class PerfisInternos
{
    public const string Administrador = "Administrador";
    public const string Atendente = "Atendente";
    public const string Solicitante = "Solicitante";

    private static readonly Dictionary<string, string[]> PermissoesPorPerfil = new(StringComparer.OrdinalIgnoreCase)
    {
        [Administrador] =
        [
            PermissoesInternas.AdminAcessar,
            PermissoesInternas.ChamadosCriar,
            PermissoesInternas.ChamadosVisualizarProprios,
            PermissoesInternas.ChamadosVisualizarTodos,
            PermissoesInternas.ChamadosAtender,
            PermissoesInternas.CadastrosGerenciar,
            PermissoesInternas.UsuariosGerenciar
        ],
        [Atendente] =
        [
            PermissoesInternas.AdminAcessar,
            PermissoesInternas.ChamadosVisualizarTodos,
            PermissoesInternas.ChamadosAtender
        ],
        [Solicitante] =
        [
            PermissoesInternas.ChamadosCriar,
            PermissoesInternas.ChamadosVisualizarProprios
        ]
    };

    public static IReadOnlyCollection<string> Todos =>
    [
        Administrador,
        Atendente,
        Solicitante,
        "Atendente N1",
        "Técnico N2",
        "Coordenador Service Desk",
        "Gestor TI",
        "Auditor Governança"
    ];

    public static IReadOnlyCollection<string> ObterPermissoes(IEnumerable<string> perfis)
    {
        var permissaoSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var perfil in perfis)
        {
            if (PermissoesPorPerfil.TryGetValue(perfil, out var permissaoPerfil))
            {
                permissaoSet.UnionWith(permissaoPerfil);
            }
        }

        return permissaoSet.ToArray();
    }

    public static bool EhPerfilValido(string? perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
        {
            return false;
        }

        var normalized = perfil.Trim().Replace(" ", "").ToLowerInvariant();
        if (normalized == "administrador" ||
            normalized == "atendente" ||
            normalized == "solicitante" ||
            normalized == "atendenten1" ||
            normalized == "tecnicon2" ||
            normalized == "coordenadorservicedesk" ||
            normalized == "gestorti" ||
            normalized == "auditorgovernanca")
        {
            return true;
        }

        return Todos.Contains(perfil.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
