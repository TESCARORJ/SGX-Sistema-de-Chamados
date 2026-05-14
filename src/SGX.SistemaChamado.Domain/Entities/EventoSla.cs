using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class EventoSla : CreationAuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public Guid ChamadoSlaId { get; private set; }
    public TipoEventoSla TipoEvento { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataEvento { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public string? ChaveIdempotencia { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public ChamadoSla ChamadoSla { get; private set; } = default!;
    public Usuario? Usuario { get; private set; }

    private EventoSla()
    {
    }

    public EventoSla(
        Guid chamadoId,
        Guid chamadoSlaId,
        TipoEventoSla tipoEvento,
        string descricao,
        DateTime dataEvento,
        Guid? usuarioId,
        string? chaveIdempotencia,
        string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado do evento de SLA e obrigatorio.", nameof(chamadoId));
        }

        if (chamadoSlaId == Guid.Empty)
        {
            throw new ArgumentException("O SLA do chamado e obrigatorio.", nameof(chamadoSlaId));
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descricao do evento de SLA e obrigatoria.", nameof(descricao));
        }

        ChamadoId = chamadoId;
        ChamadoSlaId = chamadoSlaId;
        TipoEvento = tipoEvento;
        Descricao = descricao.Trim();
        DataEvento = dataEvento;
        UsuarioId = usuarioId;
        ChaveIdempotencia = string.IsNullOrWhiteSpace(chaveIdempotencia) ? null : chaveIdempotencia.Trim();
        DefinirCriacao(criadoPor);
    }
}
