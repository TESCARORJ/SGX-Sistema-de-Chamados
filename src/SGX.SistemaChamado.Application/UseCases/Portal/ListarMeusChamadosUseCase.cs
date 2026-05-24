using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class ListarMeusChamadosUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarMeusChamadosUseCase
{
    public async Task<ListaChamadosPortalResponse> ExecutarAsync(FiltroChamadosPortalRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.InventarioAtivo)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .AsQueryable();

        var podeVisaoAmpliada = request.VisaoAmpliada && PortalUseCaseHelpers.PodeVisaoAmpliada(usuarioAtual);
        if (!podeVisaoAmpliada)
        {
            query = query.Where(x => x.SolicitanteId == usuarioAtual.Id);
        }

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

        if (request.DataInicial.HasValue)
        {
            query = query.Where(x => x.AbertoEm >= request.DataInicial.Value);
        }

        if (request.DataFinal.HasValue)
        {
            var dataFinal = request.DataFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.AbertoEm < dataFinal);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x =>
                x.Codigo.Contains(texto) ||
                x.Titulo.Contains(texto) ||
                x.Descricao.Contains(texto));
        }

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm)
            .ThenByDescending(x => x.AbertoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ListaChamadosPortalResponse
        {
            Items = items.Select(PortalUseCaseHelpers.MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}
