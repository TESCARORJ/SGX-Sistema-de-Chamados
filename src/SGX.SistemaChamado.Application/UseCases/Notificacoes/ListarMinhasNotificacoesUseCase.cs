using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ListarMinhasNotificacoesUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarMinhasNotificacoesUseCase
{
    private const int TamanhoResumoConteudo = 200;

    public async Task<ListarMinhasNotificacoesResponse> ExecutarAsync(
        ListarMinhasNotificacoesRequest request,
        CancellationToken cancellationToken = default)
    {
        var validator = new ListarMinhasNotificacoesRequestValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var query = CriarQueryBase(notificacaoRepository.Query(), usuarioAtual.Id);
        if (request.Lida.HasValue)
        {
            query = request.Lida.Value
                ? query.Where(x => x.LidaEm.HasValue)
                : query.Where(x => !x.LidaEm.HasValue);
        }

        var total = await query.CountAsync(cancellationToken);
        var totalNaoLidas = await CriarQueryBase(notificacaoRepository.Query(), usuarioAtual.Id)
            .CountAsync(x => !x.LidaEm.HasValue, cancellationToken);

        var itens = await query
            .OrderByDescending(x => x.EnviadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .ThenByDescending(x => x.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(x => new MinhaNotificacaoResumoResponse(
                x.Id,
                x.TipoEvento,
                x.Assunto,
                x.Conteudo.Length <= TamanhoResumoConteudo
                    ? x.Conteudo
                    : x.Conteudo.Substring(0, TamanhoResumoConteudo - 3) + "...",
                x.EnviadaEm!.Value,
                x.LidaEm.HasValue,
                x.LidaEm,
                x.ChamadoId))
            .ToArrayAsync(cancellationToken);

        var totalPaginas = total == 0 ? 0 : (int)Math.Ceiling(total / (double)tamanhoPagina);

        return new ListarMinhasNotificacoesResponse(
            itens,
            pagina,
            tamanhoPagina,
            total,
            totalPaginas,
            totalNaoLidas);
    }

    private static IQueryable<Notificacao> CriarQueryBase(IQueryable<Notificacao> query, Guid usuarioId)
        => query
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => x.Canal == CanalNotificacao.Sistema)
            .Where(x => x.Status == StatusNotificacao.Enviada)
            .Where(x => x.DestinatarioUsuarioId == usuarioId);
}
