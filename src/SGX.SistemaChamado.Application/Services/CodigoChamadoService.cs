using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services;

public sealed class CodigoChamadoService(IRepository<Chamado> chamadoRepository) : ICodigoChamadoService
{
    public async Task<string> GerarAsync(CancellationToken cancellationToken = default)
    {
        var anoAtual = DateTime.UtcNow.Year;
        var prefixo = $"SGX-{anoAtual}-";

        var ultimoCodigoAno = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Codigo.StartsWith(prefixo))
            .OrderByDescending(x => x.Codigo)
            .Select(x => x.Codigo)
            .FirstOrDefaultAsync(cancellationToken);

        var sequencial = 1;
        if (!string.IsNullOrWhiteSpace(ultimoCodigoAno) &&
            ultimoCodigoAno.Length >= prefixo.Length + 6 &&
            int.TryParse(ultimoCodigoAno.AsSpan(prefixo.Length, 6), out var ultimoSequencial))
        {
            sequencial = ultimoSequencial + 1;
        }

        return $"{prefixo}{sequencial:D6}";
    }
}
