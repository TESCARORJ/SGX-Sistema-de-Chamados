using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

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

        if (request.DataInicio.HasValue)
        {
            query = query.Where(x => x.DataRecebimento >= request.DataInicio.Value);
        }

        if (request.DataFim.HasValue)
        {
            var dataFinalExclusiva = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(x => x.DataRecebimento < dataFinalExclusiva);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.StatusProcessamento == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Remetente))
        {
            var remetente = request.Remetente.Trim();
            query = query.Where(x => x.Remetente.Contains(remetente));
        }

        if (request.ChamadoId.HasValue)
        {
            query = query.Where(x => x.ChamadoId == request.ChamadoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x =>
                (x.Assunto != null && x.Assunto.Contains(texto)) ||
                (x.Erro != null && x.Erro.Contains(texto)) ||
                x.Remetente.Contains(texto) ||
                x.Fingerprint.Contains(texto) ||
                (x.MessageId != null && x.MessageId.Contains(texto)) ||
                (x.Chamado != null && x.Chamado.Codigo.Contains(texto)));
        }

        query = query.OrderByDescending(x => x.DataRecebimento);

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 200);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(x => new LogIntegracaoEmailResumoResponse
            {
                Id = x.Id,
                DataRecebimento = x.DataRecebimento,
                DataProcessamento = x.DataProcessamento,
                Remetente = x.Remetente,
                Assunto = x.Assunto,
                StatusProcessamento = x.StatusProcessamento,
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
            Fingerprint = log.Fingerprint,
            Remetente = log.Remetente,
            NomeRemetente = log.NomeRemetente,
            Assunto = log.Assunto,
            DataRecebimento = log.DataRecebimento,
            DataProcessamento = log.DataProcessamento,
            StatusProcessamento = log.StatusProcessamento,
            Erro = log.Erro,
            ChamadoId = log.ChamadoId,
            ChamadoCodigo = log.Chamado?.Codigo,
            Tentativas = log.Tentativas,
            CriadoEm = log.CriadoEm,
            CriadoPor = log.CriadoPor,
            AtualizadoEm = log.AtualizadoEm,
            AtualizadoPor = log.AtualizadoPor
        };
    }
}
