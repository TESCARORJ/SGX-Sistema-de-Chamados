using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarLogsIntegracaoEmailUseCase(
    IRepository<LogIntegracaoEmail> logIntegracaoEmailRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarLogsIntegracaoEmailUseCase
{
    public async Task<ListaLogsIntegracaoEmailResponse> ExecutarAsync(FiltroLogsEmailRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var query = logIntegracaoEmailRepository.Query()
            .AsNoTracking()
            .Include(x => x.Chamado)
            .Where(x => x.Ativo)
            .AsQueryable();

        var dataInicial = request.DataInicialEfetiva;
        if (dataInicial.HasValue)
        {
            query = query.Where(x => x.DataRecebimento >= dataInicial.Value);
        }

        var dataFinal = request.DataFinalEfetiva;
        if (dataFinal.HasValue)
        {
            var dataFinalExclusiva = dataFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.DataRecebimento < dataFinalExclusiva);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.StatusProcessamento == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Remetente))
        {
            var remetente = request.Remetente.Trim().ToLowerInvariant();
            query = query.Where(x => x.Remetente.ToLower().Contains(remetente));
        }

        if (request.ChamadoId.HasValue)
        {
            query = query.Where(x => x.ChamadoId == request.ChamadoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoChamado))
        {
            var codigo = request.CodigoChamado.Trim().ToLowerInvariant();
            query = query.Where(x => x.Chamado != null && x.Chamado.Codigo.ToLower().Contains(codigo));
        }

        if (!string.IsNullOrWhiteSpace(request.Assunto))
        {
            var assunto = request.Assunto.Trim().ToLowerInvariant();
            query = query.Where(x => x.Assunto != null && x.Assunto.ToLower().Contains(assunto));
        }

        if (!string.IsNullOrWhiteSpace(request.MessageId))
        {
            var messageId = request.MessageId.Trim().ToLowerInvariant();
            query = query.Where(x => x.MessageId != null && x.MessageId.ToLower().Contains(messageId));
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLowerInvariant();
            query = query.Where(x =>
                (x.Assunto != null && x.Assunto.ToLower().Contains(texto)) ||
                (x.Erro != null && x.Erro.ToLower().Contains(texto)) ||
                x.Remetente.ToLower().Contains(texto) ||
                x.Fingerprint.ToLower().Contains(texto) ||
                (x.MessageId != null && x.MessageId.ToLower().Contains(texto)) ||
                (x.Chamado != null && x.Chamado.Codigo.ToLower().Contains(texto)));
        }

        query = AplicarOrdenacao(query, request.OrdenarPor, request.Direcao);

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 200);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(x => new LogIntegracaoEmailResumoResponse
            {
                Id = x.Id,
                MessageId = x.MessageId,
                DataRecebimento = x.DataRecebimento,
                DataProcessamento = x.DataProcessamento,
                Remetente = x.Remetente,
                Destinatario = x.Destinatario,
                Assunto = x.Assunto,
                StatusProcessamento = x.StatusProcessamento,
                StatusProcessamentoLabel = ObterStatusLabel(x.StatusProcessamento),
                TemErro = !string.IsNullOrWhiteSpace(x.Erro),
                ChamadoId = x.ChamadoId,
                ChamadoCodigo = x.Chamado != null ? x.Chamado.Codigo : null,
                ErroResumido = x.Erro == null
                    ? null
                    : (x.Erro.Length <= 240 ? x.Erro : x.Erro.Substring(0, 240) + "...")
            })
            .ToListAsync(cancellationToken);

        return new ListaLogsIntegracaoEmailResponse
        {
            Items = items,
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    private static IQueryable<LogIntegracaoEmail> AplicarOrdenacao(IQueryable<LogIntegracaoEmail> query, string? ordenarPor, string? direcao)
    {
        var asc = string.Equals(direcao, "asc", StringComparison.OrdinalIgnoreCase);
        return (ordenarPor ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "datarecebimento" => asc ? query.OrderBy(x => x.DataRecebimento) : query.OrderByDescending(x => x.DataRecebimento),
            "dataprocessamento" => asc ? query.OrderBy(x => x.DataProcessamento) : query.OrderByDescending(x => x.DataProcessamento),
            "status" => asc ? query.OrderBy(x => x.StatusProcessamento) : query.OrderByDescending(x => x.StatusProcessamento),
            "remetente" => asc ? query.OrderBy(x => x.Remetente) : query.OrderByDescending(x => x.Remetente),
            _ => query.OrderByDescending(x => x.DataProcessamento ?? x.DataRecebimento)
        };
    }

    private static string ObterStatusLabel(StatusProcessamentoEmail status)
    {
        return status switch
        {
            StatusProcessamentoEmail.Pendente => "Pendente",
            StatusProcessamentoEmail.Processado => "Processado",
            StatusProcessamentoEmail.Ignorado => "Ignorado",
            StatusProcessamentoEmail.Erro => "Erro",
            StatusProcessamentoEmail.Duplicado => "Duplicado",
            StatusProcessamentoEmail.NaoCorrelacionado => "Não correlacionado",
            _ => status.ToString()
        };
    }
}

public sealed class ObterLogIntegracaoEmailUseCase(
    IRepository<LogIntegracaoEmail> logIntegracaoEmailRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterLogIntegracaoEmailUseCase
{
    public async Task<LogIntegracaoEmailDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id do log invalido.", nameof(id));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var log = await logIntegracaoEmailRepository.Query()
            .AsNoTracking()
            .Include(x => x.Chamado)
            .FirstOrDefaultAsync(x => x.Id == id && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Log de integracao de e-mail nao encontrado.");

        return new LogIntegracaoEmailDetalheResponse
        {
            Id = log.Id,
            MessageId = log.MessageId,
            InReplyTo = log.InReplyTo,
            References = log.References,
            Fingerprint = log.Fingerprint,
            Remetente = log.Remetente,
            Destinatario = log.Destinatario,
            NomeRemetente = log.NomeRemetente,
            Assunto = log.Assunto,
            DataRecebimento = log.DataRecebimento,
            DataProcessamento = log.DataProcessamento,
            StatusProcessamento = log.StatusProcessamento,
            Erro = log.Erro,
            ChamadoId = log.ChamadoId,
            ChamadoCodigo = log.Chamado?.Codigo,
            ChamadoTitulo = log.Chamado?.Titulo,
            Tentativas = log.Tentativas,
            CriadoEm = log.CriadoEm,
            CriadoPor = log.CriadoPor,
            AtualizadoEm = log.AtualizadoEm,
            AtualizadoPor = log.AtualizadoPor
        };
    }
}
