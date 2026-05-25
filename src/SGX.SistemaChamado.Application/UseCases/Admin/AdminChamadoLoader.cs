using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

internal static class AdminChamadoLoader
{
    public static IQueryable<Chamado> QueryDetalhe(IQueryable<Chamado> query)
    {
        return query
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.InventarioAtivo)
            .Include(x => x.Aprovacoes)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.EventosSla).ThenInclude(x => x.Usuario)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo);
    }
}
