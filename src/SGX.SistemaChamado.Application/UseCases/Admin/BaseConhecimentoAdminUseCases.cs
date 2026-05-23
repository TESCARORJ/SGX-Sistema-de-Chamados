using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarArtigosBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarArtigosBaseConhecimentoUseCase
{
    public async Task<PagedResultResponse<BaseConhecimentoArtigoListagemDto>> ExecutarAsync(
        FiltroBaseConhecimentoArtigoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Titulo.ToLower().Contains(termo) ||
                ((x.Resumo ?? string.Empty).ToLower().Contains(termo)) ||
                x.Conteudo.ToLower().Contains(termo) ||
                ((x.Tags ?? string.Empty).ToLower().Contains(termo)));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Visibilidade.HasValue)
        {
            query = query.Where(x => x.Visibilidade == request.Visibilidade.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "atualizadoEm").Trim().ToLowerInvariant() switch
        {
            "criadoem" => desc ? query.OrderByDescending(x => x.CriadoEm) : query.OrderBy(x => x.CriadoEm),
            _ => desc
                ? query.OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm)
                : query.OrderBy(x => x.AtualizadoEm ?? x.CriadoEm)
        };

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<BaseConhecimentoArtigoListagemDto>
        {
            Items = itens.Select(BaseConhecimentoArtigoMapeamentos.MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}

public sealed class ObterArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterArtigoBaseConhecimentoUseCase
{
    public async Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var artigo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        return BaseConhecimentoArtigoMapeamentos.MapDetalhe(artigo);
    }
}

public sealed class CriarArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarArtigoBaseConhecimentoUseCase
{
    public async Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(
        CriarBaseConhecimentoArtigoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        if (request.CategoriaId.HasValue)
        {
            var categoriaValida = await categoriaRepository.Query()
                .AnyAsync(x => x.Id == request.CategoriaId.Value && x.Ativo, cancellationToken);

            if (!categoriaValida)
            {
                throw new InvalidOperationException("Categoria informada nao encontrada ou inativa.");
            }
        }

        var slug = await BaseConhecimentoSlugHelper.GerarSlugUnicoAsync(
            artigoRepository,
            request.Titulo,
            null,
            cancellationToken);

        var artigo = new BaseConhecimentoArtigo(
            request.Titulo,
            slug,
            request.Resumo,
            request.Conteudo,
            request.CategoriaId,
            StatusArtigoConhecimento.Rascunho,
            request.Visibilidade,
            request.Tags,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await artigoRepository.AddAsync(artigo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstAsync(x => x.Id == artigo.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Base de Conhecimento",
                "BaseConhecimentoArtigo",
                artigo.Id.ToString(),
                "Artigo da base de conhecimento criado.",
                dadosDepois: BaseConhecimentoArtigoMapeamentos.SerializarAuditoria(completo),
                metadados: BaseConhecimentoAuditoriaHelper.CriarMetadados(completo, "CriacaoArtigoBaseConhecimento"),
                cancellationToken: cancellationToken);
        }

        return BaseConhecimentoArtigoMapeamentos.MapDetalhe(completo);
    }
}

public sealed class AtualizarArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarArtigoBaseConhecimentoUseCase
{
    public async Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(
        Guid id,
        AtualizarBaseConhecimentoArtigoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var artigo = await artigoRepository.Query()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (artigo.Status == StatusArtigoConhecimento.Arquivado)
        {
            throw new InvalidOperationException("Artigo arquivado nao pode ser editado. Reative-o antes da edicao.");
        }

        if (request.CategoriaId.HasValue)
        {
            var categoriaValida = await categoriaRepository.Query()
                .AnyAsync(x => x.Id == request.CategoriaId.Value && x.Ativo, cancellationToken);

            if (!categoriaValida)
            {
                throw new InvalidOperationException("Categoria informada nao encontrada ou inativa.");
            }
        }

        var slug = string.Equals(artigo.Titulo, request.Titulo, StringComparison.Ordinal)
            ? artigo.Slug
            : await BaseConhecimentoSlugHelper.GerarSlugUnicoAsync(artigoRepository, request.Titulo, artigo.Id, cancellationToken);

        var dadosAntes = BaseConhecimentoArtigoMapeamentos.SerializarAuditoria(artigo);

        artigo.AtualizarDados(
            request.Titulo,
            slug,
            request.Resumo,
            request.Conteudo,
            request.CategoriaId,
            request.Visibilidade,
            request.Tags,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstAsync(x => x.Id == artigo.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Base de Conhecimento",
                "BaseConhecimentoArtigo",
                artigo.Id.ToString(),
                "Artigo da base de conhecimento atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: BaseConhecimentoArtigoMapeamentos.SerializarAuditoria(completo),
                metadados: BaseConhecimentoAuditoriaHelper.CriarMetadados(completo, "AtualizacaoArtigoBaseConhecimento"),
                cancellationToken: cancellationToken);
        }

        return BaseConhecimentoArtigoMapeamentos.MapDetalhe(completo);
    }
}

public sealed class PublicarArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IPublicarArtigoBaseConhecimentoUseCase
{
    public async Task<BaseConhecimentoArtigoDetalheDto> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var artigo = await artigoRepository.Query()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (!artigo.Ativo)
        {
            throw new InvalidOperationException("Somente artigos ativos podem ser publicados.");
        }

        if (artigo.Status == StatusArtigoConhecimento.Arquivado)
        {
            throw new InvalidOperationException("Artigo arquivado nao pode ser publicado diretamente.");
        }

        if (string.IsNullOrWhiteSpace(artigo.Titulo) || string.IsNullOrWhiteSpace(artigo.Conteudo))
        {
            throw new InvalidOperationException("Titulo e conteudo devem estar preenchidos para publicacao.");
        }

        var dadosAntes = BaseConhecimentoArtigoMapeamentos.SerializarAuditoria(artigo);
        artigo.Publicar(usuarioAtual.Id, usuarioAtual.Login);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstAsync(x => x.Id == artigo.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Base de Conhecimento",
                "BaseConhecimentoArtigo",
                artigo.Id.ToString(),
                "Artigo da base de conhecimento publicado.",
                dadosAntes: dadosAntes,
                dadosDepois: BaseConhecimentoArtigoMapeamentos.SerializarAuditoria(completo),
                metadados: BaseConhecimentoAuditoriaHelper.CriarMetadados(completo, "PublicacaoArtigoBaseConhecimento"),
                cancellationToken: cancellationToken);
        }

        return BaseConhecimentoArtigoMapeamentos.MapDetalhe(completo);
    }
}

public sealed class ArquivarArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IArquivarArtigoBaseConhecimentoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var artigo = await artigoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (artigo.Status == StatusArtigoConhecimento.Arquivado && !artigo.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(artigo.Id, false, "Artigo ja estava arquivado.");
        }

        artigo.Arquivar(usuarioAtual.Id, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarInativacaoAsync(
                "Base de Conhecimento",
                "BaseConhecimentoArtigo",
                artigo.Id.ToString(),
                "Artigo da base de conhecimento arquivado.",
                BaseConhecimentoAuditoriaHelper.CriarMetadados(artigo, "ArquivamentoArtigoBaseConhecimento"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(artigo.Id, false, "Artigo arquivado com sucesso.");
    }
}

public sealed class ReativarArtigoBaseConhecimentoUseCase(
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReativarArtigoBaseConhecimentoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var artigo = await artigoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (artigo.Status != StatusArtigoConhecimento.Arquivado)
        {
            throw new InvalidOperationException("Somente artigos arquivados podem ser reativados.");
        }

        artigo.TornarRascunho(usuarioAtual.Id, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAtivacaoAsync(
                "Base de Conhecimento",
                "BaseConhecimentoArtigo",
                artigo.Id.ToString(),
                "Artigo da base de conhecimento reativado.",
                BaseConhecimentoAuditoriaHelper.CriarMetadados(artigo, "ReativacaoArtigoBaseConhecimento"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(artigo.Id, true, "Artigo reativado com sucesso.");
    }
}

internal static class BaseConhecimentoArtigoMapeamentos
{
    public static BaseConhecimentoArtigoListagemDto MapListagem(BaseConhecimentoArtigo artigo)
        => new(
            artigo.Id,
            artigo.Titulo,
            artigo.Slug,
            artigo.Resumo,
            artigo.Status,
            DescricaoStatus(artigo.Status),
            artigo.Visibilidade,
            DescricaoVisibilidade(artigo.Visibilidade),
            artigo.CategoriaId,
            artigo.Categoria?.Nome,
            artigo.Tags,
            artigo.PublicadoEm,
            artigo.Ativo,
            artigo.CriadoEm,
            artigo.AtualizadoEm);

    public static BaseConhecimentoArtigoDetalheDto MapDetalhe(BaseConhecimentoArtigo artigo)
        => new(
            artigo.Id,
            artigo.Titulo,
            artigo.Slug,
            artigo.Resumo,
            artigo.Conteudo,
            artigo.CategoriaId,
            artigo.Categoria?.Nome,
            artigo.Status,
            DescricaoStatus(artigo.Status),
            artigo.Visibilidade,
            DescricaoVisibilidade(artigo.Visibilidade),
            artigo.Tags,
            artigo.PublicadoEm,
            artigo.PublicadoPorUsuarioId,
            artigo.CriadoEm,
            artigo.CriadoPorUsuarioId,
            artigo.AtualizadoEm,
            artigo.AtualizadoPorUsuarioId,
            artigo.ArquivadoEm,
            artigo.ArquivadoPorUsuarioId,
            artigo.Ativo);

    public static string? SerializarAuditoria(BaseConhecimentoArtigo artigo)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            artigo.Titulo,
            artigo.Slug,
            artigo.Resumo,
            artigo.Conteudo,
            artigo.CategoriaId,
            artigo.Status,
            artigo.Visibilidade,
            artigo.Tags,
            artigo.PublicadoEm,
            artigo.PublicadoPorUsuarioId,
            artigo.ArquivadoEm,
            artigo.ArquivadoPorUsuarioId,
            artigo.Ativo
        });

    private static string DescricaoStatus(StatusArtigoConhecimento value) => value switch
    {
        StatusArtigoConhecimento.Rascunho => "Rascunho",
        StatusArtigoConhecimento.EmRevisao => "Em revisao",
        StatusArtigoConhecimento.Publicado => "Publicado",
        StatusArtigoConhecimento.Arquivado => "Arquivado",
        _ => value.ToString()
    };

    private static string DescricaoVisibilidade(VisibilidadeArtigoConhecimento value) => value switch
    {
        VisibilidadeArtigoConhecimento.Solicitante => "Solicitante",
        VisibilidadeArtigoConhecimento.Atendente => "Atendente",
        VisibilidadeArtigoConhecimento.Administrador => "Administrador",
        _ => value.ToString()
    };
}

internal static class BaseConhecimentoAuditoriaHelper
{
    public static string CriarMetadados(BaseConhecimentoArtigo artigo, string operacao, string? observacao = null)
        => AuditoriaDiffHelper.CriarMetadadosPadrao(
            origem: "api",
            modulo: "Base de Conhecimento",
            entidade: "BaseConhecimentoArtigo",
            entidadeId: artigo.Id.ToString(),
            codigo: artigo.Slug,
            nome: artigo.Titulo,
            operacao: operacao,
            resultado: "Sucesso",
            observacao: observacao);
}

internal static class BaseConhecimentoSlugHelper
{
    private static readonly Regex MultiHyphenRegex = new("-+", RegexOptions.Compiled);

    public static async Task<string> GerarSlugUnicoAsync(
        IRepository<BaseConhecimentoArtigo> artigoRepository,
        string titulo,
        Guid? idIgnorado,
        CancellationToken cancellationToken)
    {
        var slugBase = NormalizarSlug(titulo);
        if (string.IsNullOrWhiteSpace(slugBase))
        {
            slugBase = "artigo";
        }

        var query = artigoRepository.Query().AsNoTracking();
        if (idIgnorado.HasValue)
        {
            query = query.Where(x => x.Id != idIgnorado.Value);
        }

        var slugsExistentes = await query
            .Where(x => x.Slug == slugBase || x.Slug.StartsWith(slugBase + "-"))
            .Select(x => x.Slug)
            .ToListAsync(cancellationToken);

        if (!slugsExistentes.Contains(slugBase, StringComparer.OrdinalIgnoreCase))
        {
            return slugBase;
        }

        var sufixo = 2;
        while (slugsExistentes.Contains($"{slugBase}-{sufixo}", StringComparer.OrdinalIgnoreCase))
        {
            sufixo++;
        }

        return $"{slugBase}-{sufixo}";
    }

    private static string NormalizarSlug(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            return string.Empty;
        }

        var texto = RemoverAcentos(titulo).ToLowerInvariant();
        var builder = new StringBuilder(texto.Length);

        foreach (var ch in texto)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_')
            {
                builder.Append('-');
            }
        }

        var slug = MultiHyphenRegex.Replace(builder.ToString(), "-").Trim('-');
        return slug;
    }

    private static string RemoverAcentos(string valor)
    {
        var normalized = valor.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}