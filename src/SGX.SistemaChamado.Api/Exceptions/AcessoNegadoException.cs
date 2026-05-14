namespace SGX.SistemaChamado.Api.Exceptions;

public sealed class AcessoNegadoException(string message) : Exception(message)
{
}
