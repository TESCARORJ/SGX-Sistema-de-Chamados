namespace SGX.SistemaChamado.Application.Interfaces;

public interface ICodigoChamadoService
{
    Task<string> GerarAsync(CancellationToken cancellationToken = default);
}
