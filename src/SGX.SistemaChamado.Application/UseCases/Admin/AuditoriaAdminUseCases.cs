using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Auditoria;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using System.Linq.Expressions;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarEventosAuditoriaUseCase(
    IRepository<EventoAuditoria> eventoAuditoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarEventosAuditoriaUseCase
{
    public async Task<ListaEventosAuditoriaResponse> ExecutarAsync(
        FiltroEventosAuditoriaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var query = eventoAuditoriaRepository.Query()
            .AsNoTracking()
            .AsQueryable();

        query = AplicarFiltrosListagem(query, request);
        query = query.OrderByDescending(x => x.DataEvento);

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(MapResumoExpression())
            .ToListAsync(cancellationToken);

        return new ListaEventosAuditoriaResponse
        {
            Items = items,
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    internal static IQueryable<EventoAuditoria> AplicarFiltrosListagem(
        IQueryable<EventoAuditoria> query,
        FiltroEventosAuditoriaRequest request)
    {
        if (request.DataInicio.HasValue)
        {
            var dataInicioUtc = GarantirUtc(request.DataInicio.Value);
            query = query.Where(x => x.DataEvento >= dataInicioUtc);
        }

        if (request.DataFim.HasValue)
        {
            var dataFimUtc = GarantirUtc(request.DataFim.Value);
            var dataFimExclusiva = dataFimUtc.Date.AddDays(1);
            query = query.Where(x => x.DataEvento < dataFimExclusiva);
        }

        if (request.UsuarioId.HasValue)
        {
            query = query.Where(x => x.UsuarioId == request.UsuarioId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.UsuarioEmail))
        {
            var usuarioEmail = request.UsuarioEmail.Trim().ToLowerInvariant();
            query = query.Where(x => x.UsuarioEmail != null && x.UsuarioEmail.ToLower().Contains(usuarioEmail));
        }

        if (!string.IsNullOrWhiteSpace(request.Modulo))
        {
            var modulo = request.Modulo.Trim().ToLowerInvariant();
            query = query.Where(x => x.Modulo.ToLower().Contains(modulo));
        }

        if (!string.IsNullOrWhiteSpace(request.Entidade))
        {
            var entidade = request.Entidade.Trim().ToLowerInvariant();
            query = query.Where(x => x.Entidade.ToLower().Contains(entidade));
        }

        if (!string.IsNullOrWhiteSpace(request.EntidadeId))
        {
            var entidadeId = request.EntidadeId.Trim().ToLowerInvariant();
            query = query.Where(x => x.EntidadeId != null && x.EntidadeId.ToLower().Contains(entidadeId));
        }

        if (request.Acao.HasValue)
        {
            query = query.Where(x => x.Acao == request.Acao.Value);
        }

        if (request.Nivel.HasValue)
        {
            query = query.Where(x => x.Nivel == request.Nivel.Value);
        }

        if (request.Sucesso.HasValue)
        {
            query = query.Where(x => x.Sucesso == request.Sucesso.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.IpOrigem))
        {
            var ipOrigem = request.IpOrigem.Trim().ToLowerInvariant();
            query = query.Where(x => x.IpOrigem != null && x.IpOrigem.ToLower().Contains(ipOrigem));
        }

        if (!string.IsNullOrWhiteSpace(request.CorrelacaoId))
        {
            var correlacaoId = request.CorrelacaoId.Trim().ToLowerInvariant();
            query = query.Where(x => x.CorrelacaoId != null && x.CorrelacaoId.ToLower().Contains(correlacaoId));
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLowerInvariant();
            var filtroAcaoTexto = Enum.TryParse<TipoAcaoAuditoria>(request.Texto.Trim(), true, out var acaoTexto)
                ? (TipoAcaoAuditoria?)acaoTexto
                : null;

            query = query.Where(x =>
                x.Descricao.ToLower().Contains(texto) ||
                (x.UsuarioNome != null && x.UsuarioNome.ToLower().Contains(texto)) ||
                (x.UsuarioEmail != null && x.UsuarioEmail.ToLower().Contains(texto)) ||
                x.Modulo.ToLower().Contains(texto) ||
                x.Entidade.ToLower().Contains(texto) ||
                (x.CorrelacaoId != null && x.CorrelacaoId.ToLower().Contains(texto)) ||
                (x.Metadados != null && x.Metadados.ToLower().Contains(texto)) ||
                (filtroAcaoTexto.HasValue && x.Acao == filtroAcaoTexto.Value));
        }

        if (!string.IsNullOrWhiteSpace(request.Provedor))
        {
            var provedor = request.Provedor.Trim().ToLowerInvariant();
            var filtroProvedor = $"\"provedor\":\"{provedor}\"";
            query = query.Where(x => x.Metadados != null && x.Metadados.ToLower().Contains(filtroProvedor));
        }

        if (!string.IsNullOrWhiteSpace(request.TipoEventoAutenticacao))
        {
            var tipoEventoAutenticacao = request.TipoEventoAutenticacao.Trim().ToLowerInvariant();
            var filtroTipoEvento = $"\"tipoeventoautenticacao\":\"{tipoEventoAutenticacao}\"";
            query = query.Where(x => x.Metadados != null && x.Metadados.ToLower().Contains(filtroTipoEvento));
        }

        if (request.ResultadoAutenticacao.HasValue)
        {
            var resultadoAutenticacao = request.ResultadoAutenticacao.Value.ToString().ToLowerInvariant();
            var filtroResultado = $"\"resultadoautenticacao\":\"{resultadoAutenticacao}\"";
            query = query.Where(x => x.Metadados != null && x.Metadados.ToLower().Contains(filtroResultado));
        }

        return query;
    }

    internal static IQueryable<EventoAuditoria> AplicarFiltrosDashboard(
        IQueryable<EventoAuditoria> query,
        FiltroDashboardAuditoriaRequest request)
    {
        if (request.DataInicio.HasValue)
        {
            var dataInicioUtc = GarantirUtc(request.DataInicio.Value);
            query = query.Where(x => x.DataEvento >= dataInicioUtc);
        }

        if (request.DataFim.HasValue)
        {
            var dataFimUtc = GarantirUtc(request.DataFim.Value);
            var dataFimExclusiva = dataFimUtc.Date.AddDays(1);
            query = query.Where(x => x.DataEvento < dataFimExclusiva);
        }

        if (!string.IsNullOrWhiteSpace(request.Modulo))
        {
            var modulo = request.Modulo.Trim().ToLowerInvariant();
            query = query.Where(x => x.Modulo.ToLower().Contains(modulo));
        }

        if (!string.IsNullOrWhiteSpace(request.UsuarioEmail))
        {
            var usuarioEmail = request.UsuarioEmail.Trim().ToLowerInvariant();
            query = query.Where(x => x.UsuarioEmail != null && x.UsuarioEmail.ToLower().Contains(usuarioEmail));
        }

        if (request.Nivel.HasValue)
        {
            query = query.Where(x => x.Nivel == request.Nivel.Value);
        }

        if (request.Sucesso.HasValue)
        {
            query = query.Where(x => x.Sucesso == request.Sucesso.Value);
        }

        return query;
    }

    private static DateTime GarantirUtc(DateTime data)
        => data.Kind == DateTimeKind.Utc ? data : data.ToUniversalTime();

    private static Expression<Func<EventoAuditoria, EventoAuditoriaResumoResponse>> MapResumoExpression()
        => x => new EventoAuditoriaResumoResponse(
            x.Id,
            x.DataEvento,
            x.UsuarioNome,
            x.UsuarioEmail,
            x.Modulo,
            x.Entidade,
            x.EntidadeId,
            x.Acao,
            x.Descricao,
            x.Nivel,
            x.Sucesso,
            x.IpOrigem,
            x.CorrelacaoId)
        {
            Metadados = x.Metadados
        };

    internal static EventoAuditoriaResumoResponse MapResumo(EventoAuditoria x)
        => new(
            x.Id,
            x.DataEvento,
            x.UsuarioNome,
            x.UsuarioEmail,
            x.Modulo,
            x.Entidade,
            x.EntidadeId,
            x.Acao,
            x.Descricao,
            x.Nivel,
            x.Sucesso,
            x.IpOrigem,
            x.CorrelacaoId)
        {
            Metadados = x.Metadados
        };
}

public sealed class ObterEventoAuditoriaUseCase(
    IRepository<EventoAuditoria> eventoAuditoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterEventoAuditoriaUseCase
{
    public async Task<EventoAuditoriaDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id do evento de auditoria invalido.", nameof(id));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var evento = await eventoAuditoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Evento de auditoria nao encontrado.");

        return new EventoAuditoriaDetalheResponse(
            evento.Id,
            evento.DataEvento,
            evento.UsuarioId,
            evento.UsuarioNome,
            evento.UsuarioEmail,
            evento.UsuarioLogin,
            evento.IpOrigem,
            evento.UserAgent,
            evento.Modulo,
            evento.Entidade,
            evento.EntidadeId,
            evento.Acao,
            evento.Descricao,
            evento.DadosAntes,
            evento.DadosDepois,
            evento.Metadados,
            evento.Nivel,
            evento.Sucesso,
            evento.MensagemErro,
            evento.CorrelacaoId,
            evento.CriadoEm);
    }
}

public sealed class ObterDashboardAuditoriaUseCase(
    IRepository<EventoAuditoria> eventoAuditoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterDashboardAuditoriaUseCase
{
    public async Task<AuditoriaDashboardResponse> ExecutarAsync(
        FiltroDashboardAuditoriaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var query = eventoAuditoriaRepository.Query()
            .AsNoTracking()
            .AsQueryable();

        query = ListarEventosAuditoriaUseCase.AplicarFiltrosDashboard(query, request);

        var totalEventos = await query.CountAsync(cancellationToken);
        var totalEventosCriticos = await query.CountAsync(x => x.Nivel == NivelAuditoria.Critico, cancellationToken);
        var totalEventosAlerta = await query.CountAsync(x => x.Nivel == NivelAuditoria.Alerta, cancellationToken);
        var totalEventosInformacao = await query.CountAsync(x => x.Nivel == NivelAuditoria.Informacao, cancellationToken);
        var totalFalhas = await query.CountAsync(x => !x.Sucesso, cancellationToken);
        var totalSucessos = await query.CountAsync(x => x.Sucesso, cancellationToken);

        var eventosPorModuloRaw = await query
            .GroupBy(x => x.Modulo)
            .Select(x => new { Chave = x.Key, Total = x.Count() })
            .OrderByDescending(x => x.Total)
            .Take(20)
            .ToListAsync(cancellationToken);

        var eventosPorAcaoRaw = await query
            .GroupBy(x => x.Acao)
            .Select(x => new { Chave = x.Key, Total = x.Count() })
            .OrderByDescending(x => x.Total)
            .Take(20)
            .ToListAsync(cancellationToken);

        var eventosPorUsuarioRaw = await query
            .GroupBy(x => x.UsuarioEmail ?? "sem-usuario")
            .Select(x => new { Chave = x.Key, Total = x.Count() })
            .OrderByDescending(x => x.Total)
            .Take(20)
            .ToListAsync(cancellationToken);

        var eventosPorDiaRaw = await query
            .GroupBy(x => x.DataEvento.Date)
            .Select(x => new { Dia = x.Key, Total = x.Count() })
            .OrderBy(x => x.Dia)
            .Take(60)
            .ToListAsync(cancellationToken);

        var ultimosEventosCriticos = await query
            .Where(x => x.Nivel == NivelAuditoria.Critico)
            .OrderByDescending(x => x.DataEvento)
            .Take(10)
            .ToListAsync(cancellationToken);

        var ultimasFalhas = await query
            .Where(x => !x.Sucesso)
            .OrderByDescending(x => x.DataEvento)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new AuditoriaDashboardResponse
        {
            TotalEventos = totalEventos,
            TotalEventosCriticos = totalEventosCriticos,
            TotalEventosAlerta = totalEventosAlerta,
            TotalEventosInformacao = totalEventosInformacao,
            TotalFalhas = totalFalhas,
            TotalSucessos = totalSucessos,
            EventosPorModulo = eventosPorModuloRaw.Select(x => new AuditoriaAgrupamentoResponse(x.Chave, x.Total)).ToArray(),
            EventosPorAcao = eventosPorAcaoRaw.Select(x => new AuditoriaAgrupamentoResponse(x.Chave.ToString(), x.Total)).ToArray(),
            EventosPorUsuario = eventosPorUsuarioRaw.Select(x => new AuditoriaAgrupamentoResponse(x.Chave, x.Total)).ToArray(),
            EventosPorDia = eventosPorDiaRaw.Select(x => new AuditoriaAgrupamentoDiaResponse(x.Dia, x.Total)).ToArray(),
            UltimosEventosCriticos = ultimosEventosCriticos.Select(ListarEventosAuditoriaUseCase.MapResumo).ToArray(),
            UltimasFalhas = ultimasFalhas.Select(ListarEventosAuditoriaUseCase.MapResumo).ToArray()
        };
    }
}
