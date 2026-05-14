using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public interface ISlaEventService
{
    Task RegistrarAsync(
        ChamadoSla chamadoSla,
        TipoEventoSla tipoEvento,
        string descricao,
        DateTime dataEventoUtc,
        string usuarioLogin,
        Guid? usuarioId = null,
        string? chaveIdempotencia = null,
        CancellationToken cancellationToken = default);
}
