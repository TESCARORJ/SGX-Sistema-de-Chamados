using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaEventService(
    IRepository<EventoSla> eventoSlaRepository) : ISlaEventService
{
    public async Task RegistrarAsync(
        ChamadoSla chamadoSla,
        TipoEventoSla tipoEvento,
        string descricao,
        DateTime dataEventoUtc,
        string usuarioLogin,
        Guid? usuarioId = null,
        string? chaveIdempotencia = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(chamadoSla);

        var chave = string.IsNullOrWhiteSpace(chaveIdempotencia)
            ? CriarChavePadrao(chamadoSla.Id, tipoEvento)
            : chaveIdempotencia.Trim();

        var jaExiste = await eventoSlaRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.ChaveIdempotencia == chave, cancellationToken);

        if (jaExiste)
        {
            return;
        }

        var evento = new EventoSla(
            chamadoSla.ChamadoId,
            chamadoSla.Id,
            tipoEvento,
            descricao,
            dataEventoUtc,
            usuarioId,
            chave,
            usuarioLogin);

        await eventoSlaRepository.AddAsync(evento, cancellationToken);
    }

    private static string CriarChavePadrao(Guid chamadoSlaId, TipoEventoSla tipoEvento)
        => $"chamado-sla:{chamadoSlaId}:{tipoEvento.ToString().ToLowerInvariant()}";
}
