using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public interface ISlaBusinessTimeCalculator
{
    DateTimeOffset AddBusinessMinutes(DateTimeOffset inicio, int minutos, CalendarioCorporativo calendario);
    int CountBusinessMinutes(DateTimeOffset inicio, DateTimeOffset fim, CalendarioCorporativo calendario);
    bool IsBusinessTime(DateTimeOffset dataHora, CalendarioCorporativo calendario);
    DateTimeOffset NextBusinessTime(DateTimeOffset dataHora, CalendarioCorporativo calendario);
}
