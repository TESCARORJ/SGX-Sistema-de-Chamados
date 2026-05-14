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

        if (options.UsaLoginLocalSgxComoPrincipalOuHibrido() && !options.LoginLocalHabilitado)
        {
            erros.Add("Authentication:LoginLocalHabilitado deve ser true quando ProvedorPrincipal for Local ou Hibrido.");
        }

        if (!environment.IsDevelopment() && options.ModoLocalHabilitado)
        {
            erros.Add("Authentication:ModoLocalHabilitado deve ser false fora do ambiente Development.");
        }

        if (options.LoginLocalHabilitado)
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
}
