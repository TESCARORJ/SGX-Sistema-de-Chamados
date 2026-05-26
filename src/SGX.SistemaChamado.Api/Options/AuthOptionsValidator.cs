using Microsoft.Extensions.Options;

namespace SGX.SistemaChamado.Api.Options;

public sealed class AuthOptionsValidator(IHostEnvironment environment) : IValidateOptions<AuthOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthOptions options)
    {
        var erros = new List<string>();
        var provedorBruto = (options.ProvedorPrincipal ?? string.Empty).Trim();
        var provedor = options.ObterProvedorPrincipalNormalizado();

        var provedorValido = provedorBruto.Equals(ProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase)
            || provedorBruto.Equals(ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase)
            || provedorBruto.Equals(ProvedorAutenticacao.Hibrido, StringComparison.OrdinalIgnoreCase);
        if (!provedorValido)
        {
            erros.Add("Authentication:ProvedorPrincipal inválido. Use MicrosoftEntraId, Local ou Hibrido.");
        }

        var configurados = options.ObterCodigosProvedoresConfiguradosNormalizados();
        var habilitados = options.ObterCodigosProvedoresHabilitadosNormalizados();
        var principal = options.ObterCodigoProvedorPrincipalNormalizado();

        ValidarCodigosProvedoresBrutos(options.Provedores.Configurados, "Authentication:Provedores:Configurados", erros);
        ValidarCodigosProvedoresBrutos(options.Provedores.Habilitados, "Authentication:Provedores:Habilitados", erros);

        if (configurados.Length > 0 && !configurados.Contains(principal, StringComparer.OrdinalIgnoreCase))
        {
            erros.Add("Authentication:Provedores:Principal deve existir em Authentication:Provedores:Configurados.");
        }

        if (habilitados.Length > 0 && !habilitados.Contains(principal, StringComparer.OrdinalIgnoreCase))
        {
            erros.Add("Authentication:Provedores:Principal deve existir em Authentication:Provedores:Habilitados.");
        }

        var habilitadosForaConfigurados = habilitados
            .Where(x => !configurados.Contains(x, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (habilitadosForaConfigurados.Length > 0)
        {
            erros.Add(
                $"Authentication:Provedores:Habilitados contém códigos não configurados: {string.Join(", ", habilitadosForaConfigurados)}.");
        }

        var localSgxHabilitado = habilitados.Contains(CodigoProvedorAutenticacao.LocalSgx, StringComparer.OrdinalIgnoreCase);

        if (localSgxHabilitado && !options.LoginLocalHabilitado)
        {
            erros.Add("Authentication:LoginLocalHabilitado deve ser true quando LocalSgx estiver habilitado.");
        }

        if (options.UsaLoginLocalSgxComoPrincipalOuHibrido() && !options.LoginLocalHabilitado && !localSgxHabilitado)
        {
            erros.Add("Authentication:LoginLocalHabilitado deve ser true quando ProvedorPrincipal for Local ou Hibrido.");
        }

        var localDevelopmentHabilitado = habilitados.Contains(CodigoProvedorAutenticacao.LocalDevelopment, StringComparer.OrdinalIgnoreCase);
        if (!environment.IsDevelopment() && (options.ModoLocalHabilitado || localDevelopmentHabilitado))
        {
            erros.Add("Authentication:ModoLocalHabilitado e Provedor LocalDevelopment devem ser false fora do ambiente Development.");
        }

        if (options.LoginLocalHabilitado || localSgxHabilitado)
        {
            if (string.IsNullOrWhiteSpace(options.JwtLocalIssuer))
            {
                erros.Add("Authentication:JwtLocalIssuer não configurado.");
            }

            if (string.IsNullOrWhiteSpace(options.JwtLocalAudience))
            {
                erros.Add("Authentication:JwtLocalAudience não configurado.");
            }

            var chave = (options.JwtLocalChaveAssinatura ?? string.Empty).Trim();
            if (chave.Length < 32)
            {
                erros.Add("Authentication:JwtLocalChaveAssinatura deve possuir ao menos 32 caracteres.");
            }

            if (options.JwtLocalExpiracaoMinutos <= 0)
            {
                erros.Add("Authentication:JwtLocalExpiracaoMinutos deve ser maior que zero.");
            }

            if (options.PoliticaSenha.TamanhoMinimo < 8)
            {
                erros.Add("Authentication:PoliticaSenha:TamanhoMinimo deve ser maior ou igual a 8.");
            }

            if (options.Lockout.TentativasMaximas <= 0)
            {
                erros.Add("Authentication:Lockout:TentativasMaximas deve ser maior que zero.");
            }

            if (options.Lockout.MinutosBloqueio <= 0)
            {
                erros.Add("Authentication:Lockout:MinutosBloqueio deve ser maior que zero.");
            }

            if (options.RecuperacaoSenha.ExpiracaoMinutos <= 0)
            {
                erros.Add("Authentication:RecuperacaoSenha:ExpiracaoMinutos deve ser maior que zero.");
            }
        }

        return erros.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(erros);
    }

    private static void ValidarCodigosProvedoresBrutos(IEnumerable<string>? codigos, string caminho, ICollection<string> erros)
    {
        if (codigos is null)
        {
            return;
        }

        foreach (var codigo in codigos)
        {
            var valor = (codigo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(valor))
            {
                continue;
            }

            var valido =
                string.Equals(valor, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(valor, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(valor, CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(valor, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase);

            if (!valido)
            {
                erros.Add($"{caminho} contém código inválido: {valor}.");
            }
        }
    }
}
