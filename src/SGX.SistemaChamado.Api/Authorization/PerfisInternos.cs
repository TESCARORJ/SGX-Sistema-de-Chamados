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
        Solicitante
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

        return Todos.Contains(perfil.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
