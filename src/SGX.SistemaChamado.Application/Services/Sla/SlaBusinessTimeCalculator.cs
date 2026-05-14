using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaBusinessTimeCalculator : ISlaBusinessTimeCalculator
{
    private const int MaxDiasBusca = 3660;

    public DateTimeOffset AddBusinessMinutes(DateTimeOffset inicio, int minutos, CalendarioCorporativo calendario)
    {
        if (minutos <= 0)
        {
            return IsBusinessTime(inicio, calendario) ? inicio : NextBusinessTime(inicio, calendario);
        }

        var timeZone = ObterTimeZone(calendario);
        var atualLocal = ParaLocal(inicio, timeZone);
        var restante = minutos;

        for (var dias = 0; dias < MaxDiasBusca; dias++)
        {
            var periodos = ObterPeriodosDoDia(DateOnly.FromDateTime(atualLocal), calendario).ToArray();
            foreach (var periodo in periodos)
            {
                var inicioPeriodo = Combinar(DateOnly.FromDateTime(atualLocal), periodo.Inicio);
                var fimPeriodo = Combinar(DateOnly.FromDateTime(atualLocal), periodo.Fim);

                if (atualLocal >= fimPeriodo)
                {
                    continue;
                }

                var ponto = atualLocal <= inicioPeriodo ? inicioPeriodo : atualLocal;
                var disponivel = (int)Math.Floor((fimPeriodo - ponto).TotalMinutes);
                if (disponivel <= 0)
                {
                    continue;
                }

                if (restante <= disponivel)
                {
                    return ParaUtcOffset(ponto.AddMinutes(restante), timeZone);
                }

                restante -= disponivel;
                atualLocal = fimPeriodo;
            }

            atualLocal = DateOnly.FromDateTime(atualLocal).AddDays(1).ToDateTime(TimeOnly.MinValue);
        }

        throw new InvalidOperationException("Nao foi possivel calcular prazo em horario comercial para o calendario informado.");
    }

    public int CountBusinessMinutes(DateTimeOffset inicio, DateTimeOffset fim, CalendarioCorporativo calendario)
    {
        if (fim <= inicio)
        {
            return 0;
        }

        var timeZone = ObterTimeZone(calendario);
        var inicioLocal = ParaLocal(inicio, timeZone);
        var fimLocal = ParaLocal(fim, timeZone);
        var total = 0;
        var data = DateOnly.FromDateTime(inicioLocal);
        var dataFim = DateOnly.FromDateTime(fimLocal);

        while (data <= dataFim)
        {
            foreach (var periodo in ObterPeriodosDoDia(data, calendario))
            {
                var inicioPeriodo = Combinar(data, periodo.Inicio);
                var fimPeriodo = Combinar(data, periodo.Fim);
                var inicioIntersecao = inicioPeriodo > inicioLocal ? inicioPeriodo : inicioLocal;
                var fimIntersecao = fimPeriodo < fimLocal ? fimPeriodo : fimLocal;

                if (fimIntersecao > inicioIntersecao)
                {
                    total += (int)Math.Floor((fimIntersecao - inicioIntersecao).TotalMinutes);
                }
            }

            data = data.AddDays(1);
        }

        return Math.Max(0, total);
    }

    public bool IsBusinessTime(DateTimeOffset dataHora, CalendarioCorporativo calendario)
    {
        var timeZone = ObterTimeZone(calendario);
        var local = ParaLocal(dataHora, timeZone);
        var data = DateOnly.FromDateTime(local);
        var hora = TimeOnly.FromDateTime(local);

        return ObterPeriodosDoDia(data, calendario)
            .Any(periodo => hora >= periodo.Inicio && hora < periodo.Fim);
    }

    public DateTimeOffset NextBusinessTime(DateTimeOffset dataHora, CalendarioCorporativo calendario)
    {
        var timeZone = ObterTimeZone(calendario);
        var local = ParaLocal(dataHora, timeZone);

        for (var dias = 0; dias < MaxDiasBusca; dias++)
        {
            var data = DateOnly.FromDateTime(local);
            var hora = TimeOnly.FromDateTime(local);

            foreach (var periodo in ObterPeriodosDoDia(data, calendario))
            {
                if (hora >= periodo.Inicio && hora < periodo.Fim)
                {
                    return ParaUtcOffset(local, timeZone);
                }

                if (hora < periodo.Inicio)
                {
                    return ParaUtcOffset(Combinar(data, periodo.Inicio), timeZone);
                }
            }

            local = data.AddDays(1).ToDateTime(TimeOnly.MinValue);
        }

        throw new InvalidOperationException("Nao foi encontrado proximo periodo util para o calendario informado.");
    }

    private static IEnumerable<PeriodoAtendimento> ObterPeriodosDoDia(DateOnly data, CalendarioCorporativo calendario)
    {
        var excecoes = calendario.Excecoes
            .Where(x => x.Ativo && x.Data == data)
            .OrderBy(x => x.HoraInicio ?? TimeOnly.MinValue)
            .ToArray();

        if (excecoes.Any(x => x.Tipo is TipoExcecaoCalendarioCorporativo.Feriado
                or TipoExcecaoCalendarioCorporativo.Recesso
                or TipoExcecaoCalendarioCorporativo.SemExpediente))
        {
            return [];
        }

        var expedientesEspeciais = excecoes
            .Where(x => x.Tipo == TipoExcecaoCalendarioCorporativo.ExpedienteEspecial && x.HoraInicio.HasValue && x.HoraFim.HasValue)
            .Select(x => new PeriodoAtendimento(x.HoraInicio!.Value, x.HoraFim!.Value))
            .OrderBy(x => x.Inicio)
            .ToArray();

        if (expedientesEspeciais.Length > 0)
        {
            return expedientesEspeciais;
        }

        return calendario.HorariosAtendimento
            .Where(x => x.Ativo && x.DiaSemana == data.DayOfWeek)
            .Select(x => new PeriodoAtendimento(x.HoraInicio, x.HoraFim))
            .OrderBy(x => x.Inicio)
            .ToArray();
    }

    private static TimeZoneInfo ObterTimeZone(CalendarioCorporativo calendario)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(calendario.TimeZone);
        }
        catch (TimeZoneNotFoundException) when (calendario.TimeZone == "America/Sao_Paulo")
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
        catch (InvalidTimeZoneException) when (calendario.TimeZone == "America/Sao_Paulo")
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }

    private static DateTime ParaLocal(DateTimeOffset dataHora, TimeZoneInfo timeZone)
        => TimeZoneInfo.ConvertTime(dataHora, timeZone).DateTime;

    private static DateTimeOffset ParaUtcOffset(DateTime local, TimeZoneInfo timeZone)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), timeZone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }

    private static DateTime Combinar(DateOnly data, TimeOnly hora)
        => data.ToDateTime(hora);

    private sealed record PeriodoAtendimento(TimeOnly Inicio, TimeOnly Fim);
}
