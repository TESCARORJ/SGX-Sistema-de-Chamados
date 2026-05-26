namespace SGX.SistemaChamado.Api.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Authentication";

    public string ProvedorPrincipal { get; init; } = ProvedorAutenticacao.MicrosoftEntraId;
    public ProvedoresAutenticacaoOptions Provedores { get; init; } = new();
    public bool LoginLocalHabilitado { get; init; }
    public bool ModoLocalHabilitado { get; init; }
    public string AdminLocalEmail { get; init; } = "admin@sgxdigital.com";
    public string AdminLocalNome { get; init; } = "Administrador SGX";
    public string? AdminLocalSenha { get; init; }
    public string[] DominiosPermitidos { get; init; } = [];
    public bool CriarUsuarioAutomaticamente { get; init; } = true;
    public string PerfilPadraoUsuarioMicrosoft { get; init; } = "Solicitante";
    public string JwtLocalIssuer { get; init; } = "SGX.Local";
    public string JwtLocalAudience { get; init; } = "SGX.SistemaChamado.Api";
    public string JwtLocalChaveAssinatura { get; init; } = string.Empty;
    public int JwtLocalExpiracaoMinutos { get; init; } = 120;
    public PoliticaSenhaOptions PoliticaSenha { get; init; } = new();
    public LockoutOptions Lockout { get; init; } = new();
    public RecuperacaoSenhaOptions RecuperacaoSenha { get; init; } = new();

    public bool PossuiConfiguracaoExplicitaProvedoresConfigurados()
        => (Provedores.Configurados?.Length ?? 0) > 0;

    public bool PossuiConfiguracaoExplicitaProvedoresHabilitados()
        => (Provedores.Habilitados?.Length ?? 0) > 0;

    public string[] ObterCodigosProvedoresConfiguradosNormalizados()
    {
        if (PossuiConfiguracaoExplicitaProvedoresConfigurados())
        {
            return NormalizarProvedores(Provedores.Configurados);
        }

        var legado = new List<string>();
        if (UsaMicrosoftComoPrincipalOuHibrido())
        {
            legado.Add(CodigoProvedorAutenticacao.MicrosoftEntraId);
        }

        if (UsaLoginLocalSgxComoPrincipalOuHibrido())
        {
            legado.Add(CodigoProvedorAutenticacao.LocalSgx);
        }

        if (ModoLocalHabilitado)
        {
            legado.Add(CodigoProvedorAutenticacao.LocalDevelopment);
        }

        return legado
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public string[] ObterCodigosProvedoresHabilitadosNormalizados()
    {
        if (PossuiConfiguracaoExplicitaProvedoresHabilitados())
        {
            return NormalizarProvedores(Provedores.Habilitados);
        }

        var legado = new List<string>();
        if (UsaMicrosoftComoPrincipalOuHibrido())
        {
            legado.Add(CodigoProvedorAutenticacao.MicrosoftEntraId);
        }

        if (UsaLoginLocalSgxComoPrincipalOuHibrido() && LoginLocalHabilitado)
        {
            legado.Add(CodigoProvedorAutenticacao.LocalSgx);
        }

        if (ModoLocalHabilitado)
        {
            legado.Add(CodigoProvedorAutenticacao.LocalDevelopment);
        }

        return legado
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public string ObterCodigoProvedorPrincipalNormalizado()
    {
        var principalExplicito = NormalizarCodigoProvedor(Provedores.Principal);
        if (!string.IsNullOrWhiteSpace(principalExplicito))
        {
            return principalExplicito;
        }

        var legado = ObterProvedorPrincipalNormalizado();
        if (string.Equals(legado, ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.LocalSgx;
        }

        return CodigoProvedorAutenticacao.MicrosoftEntraId;
    }

    public int ObterOrdemProvedor(string codigo, int ordemPadrao)
    {
        if (Provedores.Ordem is null || Provedores.Ordem.Count == 0)
        {
            return ordemPadrao;
        }

        foreach (var item in Provedores.Ordem)
        {
            var codigoItem = NormalizarCodigoProvedor(item.Key);
            if (!string.Equals(codigoItem, codigo, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return item.Value > 0 ? item.Value : ordemPadrao;
        }

        return ordemPadrao;
    }

    public string ObterProvedorPrincipalNormalizado()
    {
        var valor = (ProvedorPrincipal ?? string.Empty).Trim();
        if (valor.Equals(ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase))
        {
            return ProvedorAutenticacao.Local;
        }

        if (valor.Equals(ProvedorAutenticacao.Hibrido, StringComparison.OrdinalIgnoreCase))
        {
            return ProvedorAutenticacao.Hibrido;
        }

        return ProvedorAutenticacao.MicrosoftEntraId;
    }

    public bool UsaMicrosoftComoPrincipalOuHibrido()
    {
        var provedor = ObterProvedorPrincipalNormalizado();
        return provedor is ProvedorAutenticacao.MicrosoftEntraId or ProvedorAutenticacao.Hibrido;
    }

    public bool UsaLoginLocalSgxComoPrincipalOuHibrido()
    {
        var provedor = ObterProvedorPrincipalNormalizado();
        return provedor is ProvedorAutenticacao.Local or ProvedorAutenticacao.Hibrido;
    }

    private static string[] NormalizarProvedores(IEnumerable<string>? codigos)
    {
        if (codigos is null)
        {
            return [];
        }

        return codigos
            .Select(NormalizarCodigoProvedor)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()!;
    }

    private static string? NormalizarCodigoProvedor(string? codigo)
    {
        var valor = (codigo ?? string.Empty).Trim();
        if (string.Equals(valor, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.MicrosoftEntraId;
        }

        if (string.Equals(valor, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.ActiveDirectory;
        }

        if (string.Equals(valor, CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.LocalSgx;
        }

        if (string.Equals(valor, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.LocalDevelopment;
        }

        return null;
    }
}

public sealed class ProvedoresAutenticacaoOptions
{
    public string[] Configurados { get; init; } = [];
    public string[] Habilitados { get; init; } = [];
    public string Principal { get; init; } = string.Empty;
    public Dictionary<string, int> Ordem { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PoliticaSenhaOptions
{
    public int TamanhoMinimo { get; init; } = 12;
    public bool ExigirMaiuscula { get; init; } = true;
    public bool ExigirMinuscula { get; init; } = true;
    public bool ExigirNumero { get; init; } = true;
    public bool ExigirEspecial { get; init; } = true;
    public bool BloquearSenhaAnterior { get; init; } = true;
}

public sealed class LockoutOptions
{
    public int TentativasMaximas { get; init; } = 5;
    public int MinutosBloqueio { get; init; } = 15;
}

public sealed class RecuperacaoSenhaOptions
{
    public int ExpiracaoMinutos { get; init; } = 30;
}

public static class ProvedorAutenticacao
{
    public const string MicrosoftEntraId = "MicrosoftEntraId";
    public const string Local = "Local";
    public const string Hibrido = "Hibrido";
}

public enum CodigoProvedorAutenticacaoEnum
{
    MicrosoftEntraId = 1,
    ActiveDirectory = 2,
    LocalSgx = 3,
    LocalDevelopment = 4
}

public static class CodigoProvedorAutenticacao
{
    public const string MicrosoftEntraId = nameof(CodigoProvedorAutenticacaoEnum.MicrosoftEntraId);
    public const string ActiveDirectory = nameof(CodigoProvedorAutenticacaoEnum.ActiveDirectory);
    public const string LocalSgx = nameof(CodigoProvedorAutenticacaoEnum.LocalSgx);
    public const string LocalDevelopment = nameof(CodigoProvedorAutenticacaoEnum.LocalDevelopment);
}
