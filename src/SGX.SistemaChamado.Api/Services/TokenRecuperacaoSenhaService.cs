using System.Security.Cryptography;
using System.Text;

namespace SGX.SistemaChamado.Api.Services;

public interface ITokenRecuperacaoSenhaService
{
    string GerarToken();
    string CalcularHash(string valor);
}

public sealed class TokenRecuperacaoSenhaService : ITokenRecuperacaoSenhaService
{
    public string GerarToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes);
    }

    public string CalcularHash(string valor)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor ?? string.Empty));
        return Convert.ToHexString(bytes);
    }
}
