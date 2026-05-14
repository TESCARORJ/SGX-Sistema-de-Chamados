using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarChamadosAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarChamadosAdminUseCase
{
    public async Task<ListaChamadosAdminResponse> ExecutarAsync(FiltroChamadosAdminRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Departamento)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .Where(x => x.Ativo)
            .AsQueryable();

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.ResponsavelId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == request.ResponsavelId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.DataInicio.HasValue)
        {
            query = query.Where(x => x.AbertoEm >= request.DataInicio.Value);
        }

        if (request.DataFim.HasValue)
        {
            var dataFinalExclusiva = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(x => x.AbertoEm < dataFinalExclusiva);
        }

        if (request.SlaVencido.HasValue)
        {
            query = request.SlaVencido.Value
                ? query.Where(x => x.ChamadoSla != null && (x.ChamadoSla.ResolucaoViolada || (x.ChamadoSla.DataResolucao == null && x.ChamadoSla.PrazoResolucao < DateTime.UtcNow)))
                : query.Where(x => x.ChamadoSla == null || (!x.ChamadoSla.ResolucaoViolada && (x.ChamadoSla.DataResolucao != null || x.ChamadoSla.PrazoResolucao >= DateTime.UtcNow)));
        }

        if (request.SlaSituacao.HasValue)
        {
            var agora = DateTime.UtcNow;
            var limiteProximo = agora.AddMinutes(60);
            query = request.SlaSituacao.Value switch
            {
                SituacaoSlaChamadoEnum.NaoAplicavel => query.Where(x => x.ChamadoSla == null),
                SituacaoSlaChamadoEnum.Pausado => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.Pausado),
                SituacaoSlaChamadoEnum.Cumprido => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.DataResolucao != null && !x.ChamadoSla.ResolucaoViolada),
                SituacaoSlaChamadoEnum.Violado => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.DataResolucao != null && x.ChamadoSla.ResolucaoViolada),
                SituacaoSlaChamadoEnum.Vencido => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.DataResolucao == null && x.ChamadoSla.PrazoResolucao < agora),
                SituacaoSlaChamadoEnum.ProximoDoVencimento => query.Where(x =>
                    x.ChamadoSla != null &&
                    !x.ChamadoSla.Pausado &&
                    x.ChamadoSla.DataResolucao == null &&
                    x.ChamadoSla.PrazoResolucao >= agora &&
                    x.ChamadoSla.PrazoResolucao <= limiteProximo),
                SituacaoSlaChamadoEnum.DentroDoPrazo => query.Where(x =>
                    x.ChamadoSla != null &&
                    !x.ChamadoSla.Pausado &&
                    x.ChamadoSla.DataResolucao == null &&
                    x.ChamadoSla.PrazoResolucao > agora),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x =>
                x.Codigo.Contains(texto) ||
                x.Titulo.Contains(texto) ||
                x.Descricao.Contains(texto) ||
                x.Solicitante.Nome.Contains(texto) ||
                x.Solicitante.Email.Contains(texto));
        }

        query = ApplyOrder(query, request.OrdenarPor, request.DirecaoOrdenacao);

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ListaChamadosAdminResponse
        {
            Items = items.Select(AdminUseCaseHelpers.MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    private static IQueryable<Chamado> ApplyOrder(IQueryable<Chamado> query, string? ordenarPor, string? direcao)
    {
        var desc = string.Equals(direcao, "desc", StringComparison.OrdinalIgnoreCase);
        var campo = (ordenarPor ?? string.Empty).Trim().ToLowerInvariant();

        return campo switch
        {
            "codigo" => desc ? query.OrderByDescending(x => x.Codigo) : query.OrderBy(x => x.Codigo),
            "titulo" => desc ? query.OrderByDescending(x => x.Titulo) : query.OrderBy(x => x.Titulo),
            "abertoem" => desc ? query.OrderByDescending(x => x.AbertoEm) : query.OrderBy(x => x.AbertoEm),
            "encerradoem" => desc ? query.OrderByDescending(x => x.EncerradoEm) : query.OrderBy(x => x.EncerradoEm),
            "status" => desc ? query.OrderByDescending(x => x.Status.Nome) : query.OrderBy(x => x.Status.Nome),
            "prioridade" => desc ? query.OrderByDescending(x => x.Prioridade.Nome) : query.OrderBy(x => x.Prioridade.Nome),
            _ => desc
                ? query.OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm).ThenByDescending(x => x.AbertoEm)
                : query.OrderBy(x => x.AtualizadoEm ?? x.CriadoEm).ThenBy(x => x.AbertoEm)
        };
    }
}
