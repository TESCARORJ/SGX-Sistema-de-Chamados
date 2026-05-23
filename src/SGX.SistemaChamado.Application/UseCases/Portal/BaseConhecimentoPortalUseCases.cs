using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class ListarArtigosPortalBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarArtigosPortalBaseConhecimentoUseCase
{
    public async Task<PortalListaBaseConhecimentoArtigosResponse> ExecutarAsync(
        PortalFiltroBaseConhecimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var query = artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .Where(x => x.Status == StatusArtigoConhecimento.Publicado && x.Ativo)
            .AsQueryable();

        query = PortalBaseConhecimentoVisibilidadeHelper.FiltrarPorVisibilidade(query, usuarioAtual);

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Titulo.ToLower().Contains(termo) ||
                ((x.Resumo ?? string.Empty).ToLower().Contains(termo)) ||
                x.Conteudo.ToLower().Contains(termo) ||
                ((x.Tags ?? string.Empty).ToLower().Contains(termo)));
        }

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .OrderByDescending(x => x.PublicadoEm ?? x.AtualizadoEm ?? x.CriadoEm)
            .ThenByDescending(x => x.AtualizadoEm ?? x.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PortalListaBaseConhecimentoArtigosResponse
        {
            Items = itens.Select(PortalBaseConhecimentoMapeamentos.MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}

public sealed class ObterArtigoPortalBaseConhecimentoPorSlugUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterArtigoPortalBaseConhecimentoPorSlugUseCase
{
    public async Task<PortalBaseConhecimentoArtigoDetalheDto> ExecutarAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var slugNormalizado = slug.Trim().ToLowerInvariant();

        var artigo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(
                x => x.Slug == slugNormalizado && x.Status == StatusArtigoConhecimento.Publicado && x.Ativo,
                cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (!PortalBaseConhecimentoVisibilidadeHelper.PodeVisualizarArtigo(usuarioAtual, artigo.Visibilidade))
        {
            throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");
        }

        return PortalBaseConhecimentoMapeamentos.MapDetalhe(artigo);
    }
}

internal static class PortalBaseConhecimentoVisibilidadeHelper
{
    public static IQueryable<BaseConhecimentoArtigo> FiltrarPorVisibilidade(
        IQueryable<BaseConhecimentoArtigo> query,
        UsuarioContextoAplicacao usuario)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return query;
        }

        if (usuario.PossuiPerfil("Atendente"))
        {
            return query.Where(x =>
                x.Visibilidade == VisibilidadeArtigoConhecimento.Solicitante ||
                x.Visibilidade == VisibilidadeArtigoConhecimento.Atendente);
        }

        if (usuario.PossuiPerfil("Solicitante"))
        {
            return query.Where(x => x.Visibilidade == VisibilidadeArtigoConhecimento.Solicitante);
        }

        return query.Where(_ => false);
    }

    public static bool PodeVisualizarArtigo(UsuarioContextoAplicacao usuario, VisibilidadeArtigoConhecimento visibilidade)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return true;
        }

        return visibilidade switch
        {
            VisibilidadeArtigoConhecimento.Solicitante => usuario.PossuiPerfil("Solicitante") || usuario.PossuiPerfil("Atendente"),
            VisibilidadeArtigoConhecimento.Atendente => usuario.PossuiPerfil("Atendente"),
            VisibilidadeArtigoConhecimento.Administrador => false,
            _ => false
        };
    }
}

internal static class PortalBaseConhecimentoMapeamentos
{
    public static PortalBaseConhecimentoArtigoListagemDto MapListagem(BaseConhecimentoArtigo artigo)
        => new()
        {
            Id = artigo.Id,
            Titulo = artigo.Titulo,
            Slug = artigo.Slug,
            Resumo = artigo.Resumo,
            CategoriaId = artigo.CategoriaId,
            CategoriaNome = artigo.Categoria?.Nome,
            Tags = artigo.Tags,
            PublicadoEm = artigo.PublicadoEm,
            CriadoEm = artigo.CriadoEm,
            AtualizadoEm = artigo.AtualizadoEm
        };

    public static PortalBaseConhecimentoArtigoDetalheDto MapDetalhe(BaseConhecimentoArtigo artigo)
        => new()
        {
            Id = artigo.Id,
            Titulo = artigo.Titulo,
            Slug = artigo.Slug,
            Resumo = artigo.Resumo,
            Conteudo = artigo.Conteudo,
            CategoriaId = artigo.CategoriaId,
            CategoriaNome = artigo.Categoria?.Nome,
            Tags = artigo.Tags,
            PublicadoEm = artigo.PublicadoEm,
            CriadoEm = artigo.CriadoEm,
            AtualizadoEm = artigo.AtualizadoEm
        };
}