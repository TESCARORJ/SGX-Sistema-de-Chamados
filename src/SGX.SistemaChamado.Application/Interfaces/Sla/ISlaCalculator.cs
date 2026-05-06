using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public sealed record SlaPrazos(int PrazoPrimeiraRespostaHoras, int PrazoResolucaoHoras, string Fonte);

public interface ISlaCalculator
{
    Task<SlaPrazos> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        CancellationToken cancellationToken = default);
}
