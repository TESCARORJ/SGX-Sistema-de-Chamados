using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class HorarioAtendimentoCalendario : AuditableEntity
{
    public Guid CalendarioCorporativoId { get; private set; }
    public DayOfWeek DiaSemana { get; private set; }
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFim { get; private set; }

    public CalendarioCorporativo CalendarioCorporativo { get; private set; } = default!;

    private HorarioAtendimentoCalendario()
    {
    }

    public HorarioAtendimentoCalendario(
        Guid calendarioCorporativoId,
        DayOfWeek diaSemana,
        TimeOnly horaInicio,
        TimeOnly horaFim,
        bool ativo,
        string criadoPor)
    {
        if (calendarioCorporativoId == Guid.Empty)
        {
            throw new ArgumentException("O calendario do horario de atendimento e obrigatorio.", nameof(calendarioCorporativoId));
        }

        CalendarioCorporativoId = calendarioCorporativoId;
        DiaSemana = diaSemana;
        DefinirPeriodo(horaInicio, horaFim);
        DefinirCriacao(criadoPor);

        if (!ativo)
        {
            Desativar(criadoPor);
        }
    }

    public void Atualizar(DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, bool ativo, string atualizadoPor)
    {
        DiaSemana = diaSemana;
        DefinirPeriodo(horaInicio, horaFim);

        if (ativo)
        {
            Ativar(atualizadoPor);
        }
        else
        {
            Desativar(atualizadoPor);
        }
    }

    private void DefinirPeriodo(TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (horaFim <= horaInicio)
        {
            throw new ArgumentException("A hora final do horario de atendimento deve ser maior que a hora inicial.");
        }

        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }
}
