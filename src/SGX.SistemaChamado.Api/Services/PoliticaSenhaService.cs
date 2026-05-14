using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Api.Services;

public readonly record struct ValidacaoSenhaResult(bool Valida, string? Motivo);

public interface IPoliticaSenhaService
{
    ValidacaoSenhaResult ValidarSenha(string senha);
    ValidacaoSenhaResult ValidarNovaSenha(Usuario usuario, string novaSenha);
}

public sealed class PoliticaSenhaService(
    IOptions<AuthOptions> authOptions,
    IPasswordHasher<Usuario> passwordHasher) : IPoliticaSenhaService
{
    private static readonly HashSet<string> SenhasBloqueadas = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin@123456",
        "123456",
        "password",
        "senha",
        "admin",
        "qwerty",
        "111111"
    };

    public ValidacaoSenhaResult ValidarSenha(string senha)
    {
        var opcoes = authOptions.Value.PoliticaSenha;
        var senhaNormalizada = (senha ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(senhaNormalizada))
        {
            return new ValidacaoSenhaResult(false, "Senha obrigatoria.");
        }

        if (senhaNormalizada.Length < opcoes.TamanhoMinimo)
        {
            return new ValidacaoSenhaResult(false, $"Senha precisa ter no minimo {opcoes.TamanhoMinimo} caracteres.");
        }

        if (SenhasBloqueadas.Contains(senhaNormalizada))
        {
            return new ValidacaoSenhaResult(false, "Senha fraca bloqueada por politica de seguranca.");
        }

        if (opcoes.ExigirMaiuscula && !senhaNormalizada.Any(char.IsUpper))
        {
            return new ValidacaoSenhaResult(false, "Senha precisa conter letra maiuscula.");
        }

        if (opcoes.ExigirMinuscula && !senhaNormalizada.Any(char.IsLower))
        {
            return new ValidacaoSenhaResult(false, "Senha precisa conter letra minuscula.");
        }

        if (opcoes.ExigirNumero && !senhaNormalizada.Any(char.IsDigit))
        {
            return new ValidacaoSenhaResult(false, "Senha precisa conter numero.");
        }

        if (opcoes.ExigirEspecial && !senhaNormalizada.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            return new ValidacaoSenhaResult(false, "Senha precisa conter caractere especial.");
        }

        return new ValidacaoSenhaResult(true, null);
    }

    public ValidacaoSenhaResult ValidarNovaSenha(Usuario usuario, string novaSenha)
    {
        var validacaoBase = ValidarSenha(novaSenha);
        if (!validacaoBase.Valida)
        {
            return validacaoBase;
        }

        var bloquearSenhaAnterior = authOptions.Value.PoliticaSenha.BloquearSenhaAnterior;
        if (!bloquearSenhaAnterior || string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            return validacaoBase;
        }

        var verificacao = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHashLocal, novaSenha);
        if (verificacao != PasswordVerificationResult.Failed)
        {
            return new ValidacaoSenhaResult(false, "Nova senha deve ser diferente da senha atual.");
        }

        return validacaoBase;
    }
}
