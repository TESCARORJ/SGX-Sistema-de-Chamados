using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class CatalogoServicosPortalUseCases(
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<FormularioServico> formularioServicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IPortalCatalogoServicosUseCases
{
    public async Task<PortalListaCatalogoServicosResponse> ListarAsync(
        PortalFiltroCatalogoServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var query = catalogoServicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.DepartamentoResponsavel)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Where(x => x.Status == StatusCatalogoServico.Publicado && x.Ativo)
            .AsQueryable();

        query = PortalCatalogoServicosVisibilidadeHelper.FiltrarPorVisibilidade(query, usuarioAtual);

        if (request.DepartamentoResponsavelId.HasValue)
        {
            query = query.Where(x => x.DepartamentoResponsavelId == request.DepartamentoResponsavelId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PermiteAberturaChamado.HasValue)
        {
            query = query.Where(x => x.PermiteAberturaChamado == request.PermiteAberturaChamado.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Nome.ToLower().Contains(termo) ||
                ((x.Descricao ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.InstrucoesSolicitante ?? string.Empty).ToLower().Contains(termo)));
        }

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .OrderBy(x => x.DepartamentoResponsavel.Nome)
            .ThenBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PortalListaCatalogoServicosResponse
        {
            Items = itens.Select(PortalCatalogoServicoMapeamentos.MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<PortalCatalogoServicoDetalheDto> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var servico = await ObterServicoPublicadoAtivoPorSlugAsync(slug, cancellationToken);

        if (!PortalCatalogoServicosVisibilidadeHelper.PodeVisualizarServico(usuarioAtual, servico.Visibilidade))
        {
            throw new KeyNotFoundException("Servico do catalogo nao encontrado.");
        }

        return PortalCatalogoServicoMapeamentos.MapDetalhe(servico);
    }

    public async Task<PortalPrepararChamadoCatalogoServicoDto> PrepararAberturaChamadoAsync(string slug, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var servico = await ObterServicoPublicadoAtivoPorSlugAsync(slug, cancellationToken);

        if (!PortalCatalogoServicosVisibilidadeHelper.PodeVisualizarServico(usuarioAtual, servico.Visibilidade))
        {
            throw new KeyNotFoundException("Servico do catalogo nao encontrado.");
        }

        if (!servico.PermiteAberturaChamado)
        {
            throw new InvalidOperationException("Este servico esta disponivel apenas para consulta.");
        }

        var (formulario, versaoSelecionada) = await ObterFormularioPreparacaoAsync(servico.Id, cancellationToken);

        return PortalCatalogoServicoMapeamentos.MapPrepararChamado(servico, formulario, versaoSelecionada);
    }

    private async Task<CatalogoServico> ObterServicoPublicadoAtivoPorSlugAsync(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new KeyNotFoundException("Servico do catalogo nao encontrado.");
        }

        var slugNormalizado = slug.Trim().ToLowerInvariant();
        var servico = await catalogoServicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.DepartamentoResponsavel)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.PrioridadePadrao)
            .Include(x => x.SlaPadrao)
            .Include(x => x.ArtigoBaseConhecimento)
            .FirstOrDefaultAsync(
                x => x.Slug == slugNormalizado && x.Status == StatusCatalogoServico.Publicado && x.Ativo,
                cancellationToken);

        return servico ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");
    }

    private async Task<(FormularioServico? Formulario, FormularioServicoVersao? VersaoSelecionada)> ObterFormularioPreparacaoAsync(
        Guid catalogoServicoId,
        CancellationToken cancellationToken)
    {
        var formulario = await formularioServicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Versoes)
                .ThenInclude(x => x.Campos)
                    .ThenInclude(x => x.Opcoes)
            .FirstOrDefaultAsync(x => x.CatalogoServicoId == catalogoServicoId && x.Ativo, cancellationToken);

        if (formulario is null)
        {
            return (null, null);
        }

        var versoesAtivas = formulario.Versoes
            .Where(x => x.Ativo)
            .ToArray();

        if (versoesAtivas.Length == 0)
        {
            return (formulario, null);
        }

        var versaoSelecionada = versoesAtivas
            .Where(x => x.Publicada)
            .OrderByDescending(x => x.PublicadoEm ?? DateTime.MinValue)
            .ThenByDescending(x => x.Numero)
            .FirstOrDefault()
            ?? versoesAtivas
                .OrderByDescending(x => x.Numero)
                .First();

        return (formulario, versaoSelecionada);
    }
}

internal static class PortalCatalogoServicosVisibilidadeHelper
{
    public static IQueryable<CatalogoServico> FiltrarPorVisibilidade(
        IQueryable<CatalogoServico> query,
        UsuarioContextoAplicacao usuario)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return query;
        }

        if (usuario.PossuiPerfil("Atendente"))
        {
            return query.Where(x =>
                x.Visibilidade == VisibilidadeCatalogoServico.Solicitante ||
                x.Visibilidade == VisibilidadeCatalogoServico.Atendente ||
                x.Visibilidade == VisibilidadeCatalogoServico.Interno);
        }

        if (usuario.PossuiPerfil("Solicitante"))
        {
            return query.Where(x => x.Visibilidade == VisibilidadeCatalogoServico.Solicitante);
        }

        return query.Where(_ => false);
    }

    public static bool PodeVisualizarServico(UsuarioContextoAplicacao usuario, VisibilidadeCatalogoServico visibilidade)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return true;
        }

        return visibilidade switch
        {
            VisibilidadeCatalogoServico.Solicitante => usuario.PossuiPerfil("Solicitante") || usuario.PossuiPerfil("Atendente"),
            VisibilidadeCatalogoServico.Atendente => usuario.PossuiPerfil("Atendente"),
            VisibilidadeCatalogoServico.Administrador => false,
            VisibilidadeCatalogoServico.Interno => usuario.PossuiPerfil("Atendente"),
            _ => false
        };
    }
}

internal static class PortalCatalogoServicoMapeamentos
{
    public static PortalCatalogoServicoListagemDto MapListagem(CatalogoServico servico)
        => new()
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Slug = servico.Slug,
            Descricao = servico.Descricao,
            DepartamentoResponsavelId = servico.DepartamentoResponsavelId,
            DepartamentoResponsavelNome = servico.DepartamentoResponsavel?.Nome,
            CategoriaId = servico.CategoriaId,
            CategoriaNome = servico.Categoria?.Nome,
            SubcategoriaId = servico.SubcategoriaId,
            SubcategoriaNome = servico.Subcategoria?.Nome,
            PermiteAberturaChamado = servico.PermiteAberturaChamado,
            RequerAprovacao = servico.RequerAprovacao,
            Visibilidade = servico.Visibilidade,
            PublicadoEm = servico.PublicadoEm,
            Ordem = servico.Ordem
        };

    public static PortalCatalogoServicoDetalheDto MapDetalhe(CatalogoServico servico)
        => new()
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Slug = servico.Slug,
            Descricao = servico.Descricao,
            InstrucoesSolicitante = servico.InstrucoesSolicitante,
            DepartamentoResponsavelId = servico.DepartamentoResponsavelId,
            DepartamentoResponsavelNome = servico.DepartamentoResponsavel?.Nome,
            CategoriaId = servico.CategoriaId,
            CategoriaNome = servico.Categoria?.Nome,
            SubcategoriaId = servico.SubcategoriaId,
            SubcategoriaNome = servico.Subcategoria?.Nome,
            PrioridadePadraoId = servico.PrioridadePadraoId,
            PrioridadePadraoNome = servico.PrioridadePadrao?.Nome,
            SlaPadraoId = servico.SlaPadraoId,
            SlaPadraoNome = servico.SlaPadrao?.Nome,
            ArtigoBaseConhecimentoId = servico.ArtigoBaseConhecimentoId,
            ArtigoBaseConhecimentoTitulo = servico.ArtigoBaseConhecimento?.Titulo,
            ArtigoBaseConhecimentoSlug = servico.ArtigoBaseConhecimento?.Slug,
            PermiteAberturaChamado = servico.PermiteAberturaChamado,
            RequerAprovacao = servico.RequerAprovacao,
            Visibilidade = servico.Visibilidade,
            PublicadoEm = servico.PublicadoEm
        };

    public static PortalPrepararChamadoCatalogoServicoDto MapPrepararChamado(
        CatalogoServico servico,
        FormularioServico? formulario,
        FormularioServicoVersao? versaoSelecionada)
        => new()
        {
            CatalogoServicoId = servico.Id,
            Nome = servico.Nome,
            Slug = servico.Slug,
            Descricao = servico.Descricao,
            InstrucoesSolicitante = servico.InstrucoesSolicitante,
            DepartamentoResponsavelId = servico.DepartamentoResponsavelId,
            DepartamentoResponsavelNome = servico.DepartamentoResponsavel?.Nome,
            CategoriaId = servico.CategoriaId,
            CategoriaNome = servico.Categoria?.Nome,
            SubcategoriaId = servico.SubcategoriaId,
            SubcategoriaNome = servico.Subcategoria?.Nome,
            PrioridadePadraoId = servico.PrioridadePadraoId,
            PrioridadePadraoNome = servico.PrioridadePadrao?.Nome,
            SlaPadraoId = servico.SlaPadraoId,
            SlaPadraoNome = servico.SlaPadrao?.Nome,
            RequerAprovacao = servico.RequerAprovacao,
            PermiteAberturaChamado = servico.PermiteAberturaChamado,
            Formulario = MapFormularioPreparacao(formulario, versaoSelecionada)
        };

    private static PortalFormularioPreparacaoDto? MapFormularioPreparacao(
        FormularioServico? formulario,
        FormularioServicoVersao? versaoSelecionada)
    {
        if (formulario is null || versaoSelecionada is null)
        {
            return null;
        }

        return new PortalFormularioPreparacaoDto
        {
            Id = formulario.Id,
            Nome = formulario.Nome,
            Descricao = formulario.Descricao,
            Versao = new PortalFormularioPreparacaoVersaoDto
            {
                Id = versaoSelecionada.Id,
                Numero = versaoSelecionada.Numero,
                Publicada = versaoSelecionada.Publicada,
                PublicadoEm = versaoSelecionada.PublicadoEm,
                Campos = versaoSelecionada.Campos
                    .Where(x => x.Ativo && x.Visivel)
                    .OrderBy(x => x.Ordem)
                    .ThenBy(x => x.Nome)
                    .Select(MapCampoPreparacao)
                    .ToArray()
            }
        };
    }

    private static PortalFormularioPreparacaoCampoDto MapCampoPreparacao(CampoFormularioServico campo)
        => new()
        {
            Id = campo.Id,
            Nome = campo.Nome,
            Rotulo = campo.Rotulo,
            Tipo = campo.Tipo,
            Obrigatorio = campo.Obrigatorio,
            Ordem = campo.Ordem,
            TextoAjuda = campo.TextoAjuda,
            Opcoes = CampoAceitaOpcoes(campo)
                ? campo.Opcoes
                    .Where(x => x.Ativo)
                    .OrderBy(x => x.Ordem)
                    .ThenBy(x => x.Valor)
                    .Select(MapOpcaoPreparacao)
                    .ToArray()
                : []
        };

    private static PortalFormularioPreparacaoOpcaoDto MapOpcaoPreparacao(OpcaoCampoFormularioServico opcao)
        => new()
        {
            Id = opcao.Id,
            Valor = opcao.Valor,
            Rotulo = opcao.Rotulo,
            Ordem = opcao.Ordem
        };

    private static bool CampoAceitaOpcoes(CampoFormularioServico campo)
        => campo.Tipo is TipoCampoFormularioServico.SelecaoUnica or TipoCampoFormularioServico.SelecaoMultipla;
}
