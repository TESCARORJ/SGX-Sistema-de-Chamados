using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class CalendarioCorporativoService(
    IRepository<CalendarioCorporativo> calendarioRepository,
    IRepository<HorarioAtendimentoCalendario> horarioRepository,
    IRepository<ExcecaoCalendarioCorporativo> excecaoRepository,
    IUnitOfWork unitOfWork) : ICalendarioCorporativoService
{
    public async Task<IReadOnlyCollection<CalendarioCorporativoResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var calendarios = await QueryCompleta()
            .AsNoTracking()
            .OrderByDescending(x => x.Padrao)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return calendarios.Select(Map).ToArray();
    }

    public async Task<CalendarioCorporativoResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        return Map(calendario);
    }

    public async Task<CalendarioCorporativoResponse> CriarAsync(CriarCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = new CalendarioCorporativo(request.Nome, request.Descricao, request.Padrao, request.TimeZone, usuarioLogin);
        if (!request.Ativo)
        {
            calendario.Desativar(usuarioLogin);
        }

        await calendarioRepository.AddAsync(calendario, cancellationToken);

        if (request.Padrao && request.Ativo)
        {
            await RemoverPadraoDosDemaisAsync(calendario.Id, usuarioLogin, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendario.Id, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> AtualizarAsync(Guid id, AtualizarCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        calendario.Atualizar(request.Nome, request.Descricao, request.TimeZone, usuarioLogin);
        calendarioRepository.Update(calendario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(calendario);
    }

    public async Task<CalendarioCorporativoResponse> AtualizarStatusAsync(Guid id, bool ativo, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        if (ativo)
        {
            if (calendario.Padrao)
            {
                await RemoverPadraoDosDemaisAsync(calendario.Id, usuarioLogin, cancellationToken);
            }

            calendario.Ativar(usuarioLogin);
        }
        else
        {
            calendario.Desativar(usuarioLogin);
            if (calendario.Padrao)
            {
                calendario.RemoverPadrao(usuarioLogin);
            }
        }

        calendarioRepository.Update(calendario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(calendario);
    }

    public async Task<CalendarioCorporativoResponse> DefinirPadraoAsync(Guid id, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        await RemoverPadraoDosDemaisAsync(calendario.Id, usuarioLogin, cancellationToken);
        calendario.DefinirComoPadrao(usuarioLogin);
        calendarioRepository.Update(calendario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(calendario);
    }

    public async Task<CalendarioCorporativoResponse> AdicionarHorarioAsync(Guid calendarioId, HorarioAtendimentoCalendarioRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        ValidarSobreposicaoHorario(calendario, request, null);
        var horario = new HorarioAtendimentoCalendario(calendarioId, request.DiaSemana, request.HoraInicio, request.HoraFim, request.Ativo, usuarioLogin);
        await horarioRepository.AddAsync(horario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> AtualizarHorarioAsync(Guid calendarioId, Guid horarioId, HorarioAtendimentoCalendarioRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");
        var horario = calendario.HorariosAtendimento.FirstOrDefault(x => x.Id == horarioId)
            ?? throw new KeyNotFoundException("Horario de atendimento nao encontrado.");

        ValidarSobreposicaoHorario(calendario, request, horarioId);
        horario.Atualizar(request.DiaSemana, request.HoraInicio, request.HoraFim, request.Ativo, usuarioLogin);
        horarioRepository.Update(horario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> ExcluirHorarioAsync(Guid calendarioId, Guid horarioId, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");
        var horario = calendario.HorariosAtendimento.FirstOrDefault(x => x.Id == horarioId)
            ?? throw new KeyNotFoundException("Horario de atendimento nao encontrado.");

        horario.Desativar(usuarioLogin);
        horarioRepository.Update(horario);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> AdicionarExcecaoAsync(Guid calendarioId, ExcecaoCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        _ = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");

        var excecao = new ExcecaoCalendarioCorporativo(
            calendarioId,
            request.Data,
            request.Tipo,
            request.Descricao,
            request.HoraInicio,
            request.HoraFim,
            request.Ativo,
            usuarioLogin);

        await excecaoRepository.AddAsync(excecao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> AtualizarExcecaoAsync(Guid calendarioId, Guid excecaoId, ExcecaoCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");
        var excecao = calendario.Excecoes.FirstOrDefault(x => x.Id == excecaoId)
            ?? throw new KeyNotFoundException("Excecao de calendario nao encontrada.");

        excecao.Atualizar(request.Data, request.Tipo, request.Descricao, request.HoraInicio, request.HoraFim, request.Ativo, usuarioLogin);
        excecaoRepository.Update(excecao);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    public async Task<CalendarioCorporativoResponse> ExcluirExcecaoAsync(Guid calendarioId, Guid excecaoId, string usuarioLogin, CancellationToken cancellationToken = default)
    {
        var calendario = await CarregarAsync(calendarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Calendario corporativo nao encontrado.");
        var excecao = calendario.Excecoes.FirstOrDefault(x => x.Id == excecaoId)
            ?? throw new KeyNotFoundException("Excecao de calendario nao encontrada.");

        excecao.Desativar(usuarioLogin);
        excecaoRepository.Update(excecao);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await ObterAsync(calendarioId, cancellationToken);
    }

    private IQueryable<CalendarioCorporativo> QueryCompleta()
        => calendarioRepository.Query()
            .Include(x => x.HorariosAtendimento)
            .Include(x => x.Excecoes);

    private Task<CalendarioCorporativo?> CarregarAsync(Guid id, CancellationToken cancellationToken)
        => QueryCompleta().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    private async Task RemoverPadraoDosDemaisAsync(Guid calendarioPadraoId, string usuarioLogin, CancellationToken cancellationToken)
    {
        var atuais = await calendarioRepository.Query()
            .Where(x => x.Id != calendarioPadraoId && x.Padrao)
            .ToListAsync(cancellationToken);

        foreach (var atual in atuais)
        {
            atual.RemoverPadrao(usuarioLogin);
            calendarioRepository.Update(atual);
        }
    }

    private static void ValidarSobreposicaoHorario(CalendarioCorporativo calendario, HorarioAtendimentoCalendarioRequest request, Guid? horarioIdIgnorado)
    {
        var existeSobreposicao = calendario.HorariosAtendimento
            .Where(x => x.Ativo && x.Id != horarioIdIgnorado && x.DiaSemana == request.DiaSemana)
            .Any(x => request.HoraInicio < x.HoraFim && request.HoraFim > x.HoraInicio);

        if (existeSobreposicao)
        {
            throw new InvalidOperationException("Ja existe horario de atendimento ativo sobreposto para este dia da semana.");
        }
    }

    public static CalendarioCorporativoResponse Map(CalendarioCorporativo calendario)
        => new(
            calendario.Id,
            calendario.Nome,
            calendario.Descricao,
            calendario.Ativo,
            calendario.Padrao,
            calendario.TimeZone,
            calendario.CriadoEm,
            calendario.CriadoPor,
            calendario.AtualizadoEm,
            calendario.AtualizadoPor,
            calendario.HorariosAtendimento
                .OrderBy(x => x.DiaSemana)
                .ThenBy(x => x.HoraInicio)
                .Select(x => new HorarioAtendimentoCalendarioResponse(
                    x.Id,
                    x.DiaSemana,
                    ObterNomeDiaSemana(x.DiaSemana),
                    x.HoraInicio,
                    x.HoraFim,
                    x.Ativo))
                .ToArray(),
            calendario.Excecoes
                .OrderBy(x => x.Data)
                .ThenBy(x => x.HoraInicio ?? TimeOnly.MinValue)
                .Select(x => new ExcecaoCalendarioCorporativoResponse(
                    x.Id,
                    x.Data,
                    x.Tipo,
                    x.Tipo.ToString(),
                    x.Descricao,
                    x.HoraInicio,
                    x.HoraFim,
                    x.Ativo))
                .ToArray());

    private static string ObterNomeDiaSemana(DayOfWeek diaSemana)
        => diaSemana switch
        {
            DayOfWeek.Sunday => "Domingo",
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terça-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            DayOfWeek.Saturday => "Sábado",
            _ => diaSemana.ToString()
        };
}
