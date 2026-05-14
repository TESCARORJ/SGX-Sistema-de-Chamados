using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ExcecaoCalendarioCorporativo : AuditableEntity
{
    public Guid CalendarioCorporativoId { get; private set; }
    public DateOnly Data { get; private set; }
    public TipoExcecaoCalendarioCorporativo Tipo { get; private set; }
    public string? Descricao { get; private set; }
    public TimeOnly? HoraInicio { get; private set; }
    public TimeOnly? HoraFim { get; private set; }

    public CalendarioCorporativo CalendarioCorporativo { get; private set; } = default!;

    private ExcecaoCalendarioCorporativo()
    {
    }

    public ExcecaoCalendarioCorporativo(
        Guid calendarioCorporativoId,
        DateOnly data,
        TipoExcecaoCalendarioCorporativo tipo,
        string? descricao,
        TimeOnly? horaInicio,
        TimeOnly? horaFim,
        bool ativo,
        string criadoPor)
    {
        if (calendarioCorporativoId == Guid.Empty)
        {
            throw new ArgumentException("O calendario da excecao e obrigatorio.", nameof(calendarioCorporativoId));
        }

        CalendarioCorporativoId = calendarioCorporativoId;
        AtualizarDados(data, tipo, descricao, horaInicio, horaFim);
        DefinirCriacao(criadoPor);

        if (!ativo)
        {
            Desativar(criadoPor);
        }
    }

    public void Atualizar(
        DateOnly data,
        TipoExcecaoCalendarioCorporativo tipo,
        string? descricao,
        TimeOnly? horaInicio,
        TimeOnly? horaFim,
        bool ativo,
        string atualizadoPor)
    {
        AtualizarDados(data, tipo, descricao, horaInicio, horaFim);

        if (ativo)
        {
            Ativar(atualizadoPor);
        }
        else
        {
            Desativar(atualizadoPor);
        }
    }

    private void AtualizarDados(
        DateOnly data,
        TipoExcecaoCalendarioCorporativo tipo,
        string? descricao,
        TimeOnly? horaInicio,
        TimeOnly? horaFim)
    {
        if (tipo == TipoExcecaoCalendarioCorporativo.ExpedienteEspecial)
        {
            if (!horaInicio.HasValue || !horaFim.HasValue)
            {
                throw new ArgumentException("Expediente especial exige hora inicial e hora final.");
            }

            if (horaFim.Value <= horaInicio.Value)
            {
                throw new ArgumentException("A hora final da excecao deve ser maior que a hora inicial.");
            }
        }
        else if (horaInicio.HasValue != horaFim.HasValue)
        {
            throw new ArgumentException("Informe hora inicial e hora final juntas para excecoes com periodo.");
        }

        Data = data;
        Tipo = tipo;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }
}
