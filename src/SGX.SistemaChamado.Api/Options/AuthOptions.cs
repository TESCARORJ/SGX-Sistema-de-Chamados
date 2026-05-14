namespace SGX.SistemaChamado.Api.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Authentication";

    public string ProvedorPrincipal { get; init; } = ProvedorAutenticacao.MicrosoftEntraId;
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
