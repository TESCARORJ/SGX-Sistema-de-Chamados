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

public sealed class CatalogoServicosAdminUseCases(
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<PoliticaSla> politicaSlaRepository,
    IRepository<BaseConhecimentoArtigo> baseConhecimentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAdminCatalogoServicosUseCases
{
    public async Task<PagedResultResponse<CatalogoServicoListagemDto>> ListarAsync(
        FiltroCatalogoServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = catalogoServicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.DepartamentoResponsavel)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.PrioridadePadrao)
            .Include(x => x.SlaPadrao)
            .AsQueryable();

        var slaPadraoId = CatalogoServicoValidacoes.ResolverSlaPadraoId(request.SlaPadraoId, request.PoliticaSlaId);

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Nome.ToLower().Contains(termo) ||
                ((x.Descricao ?? string.Empty).ToLower().Contains(termo)) ||
                ((x.InstrucoesSolicitante ?? string.Empty).ToLower().Contains(termo)));
        }

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

        if (request.PrioridadePadraoId.HasValue)
        {
            query = query.Where(x => x.PrioridadePadraoId == request.PrioridadePadraoId.Value);
        }

        if (slaPadraoId.HasValue)
        {
            query = query.Where(x => x.SlaPadraoId == slaPadraoId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Visibilidade.HasValue)
        {
            query = query.Where(x => x.Visibilidade == request.Visibilidade.Value);
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.PermiteAberturaChamado.HasValue)
        {
            query = query.Where(x => x.PermiteAberturaChamado == request.PermiteAberturaChamado.Value);
        }

        if (request.RequerAprovacao.HasValue)
        {
            query = query.Where(x => x.RequerAprovacao == request.RequerAprovacao.Value);
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "atualizadoEm").Trim().ToLowerInvariant() switch
        {
            "nome" => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            "ordem" => desc ? query.OrderByDescending(x => x.Ordem) : query.OrderBy(x => x.Ordem),
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

        return new PagedResultResponse<CatalogoServicoListagemDto>
        {
            Items = itens.Select(CatalogoServicoMapeamentos.MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<CatalogoServicoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var servico = await ObterCatalogoCompletoPorIdAsync(id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");

        return CatalogoServicoMapeamentos.MapDetalhe(servico);
    }

    public async Task<CatalogoServicoDetalheDto> CriarAsync(
        CriarCatalogoServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        CatalogoServicoValidacoes.ValidarDadosBasicos(request.Nome, request.Descricao, request.DepartamentoResponsavelId);
        var slaPadraoId = CatalogoServicoValidacoes.ResolverSlaPadraoId(request.SlaPadraoId, request.PoliticaSlaId);

        await CatalogoServicoValidacoes.ValidarRelacionamentosAsync(
            departamentoRepository,
            categoriaRepository,
            subcategoriaRepository,
            prioridadeRepository,
            politicaSlaRepository,
            baseConhecimentoRepository,
            request.DepartamentoResponsavelId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.PrioridadePadraoId,
            slaPadraoId,
            request.ArtigoBaseConhecimentoId,
            cancellationToken);

        var slug = await CatalogoServicoSlugHelper.GerarSlugUnicoAsync(
            catalogoServicoRepository,
            request.Nome,
            null,
            cancellationToken);

        var servico = new CatalogoServico(
            request.Nome,
            slug,
            request.Descricao,
            request.InstrucoesSolicitante,
            request.DepartamentoResponsavelId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.PrioridadePadraoId,
            slaPadraoId,
            request.ArtigoBaseConhecimentoId,
            request.Visibilidade,
            request.PermiteAberturaChamado ?? true,
            request.RequerAprovacao,
            request.Ordem,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await catalogoServicoRepository.AddAsync(servico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterCatalogoCompletoPorIdAsync(servico.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado apos criacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Catalogo de Servicos",
                "CatalogoServico",
                servico.Id.ToString(),
                "Servico do catalogo criado.",
                dadosDepois: CatalogoServicoMapeamentos.SerializarAuditoria(completo),
                metadados: CatalogoServicoAuditoriaHelper.CriarMetadados(completo, "CriacaoCatalogoServico"),
                cancellationToken: cancellationToken);
        }

        return CatalogoServicoMapeamentos.MapDetalhe(completo);
    }

    public async Task<CatalogoServicoDetalheDto> AtualizarAsync(
        Guid id,
        AtualizarCatalogoServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        CatalogoServicoValidacoes.ValidarDadosBasicos(request.Nome, request.Descricao, request.DepartamentoResponsavelId);
        var slaPadraoId = CatalogoServicoValidacoes.ResolverSlaPadraoId(request.SlaPadraoId, request.PoliticaSlaId);

        var servico = await ObterCatalogoCompletoPorIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");

        if (servico.Status == StatusCatalogoServico.Arquivado)
        {
            throw new InvalidOperationException("Servico arquivado nao pode ser editado. Reative-o antes da edicao.");
        }

        await CatalogoServicoValidacoes.ValidarRelacionamentosAsync(
            departamentoRepository,
            categoriaRepository,
            subcategoriaRepository,
            prioridadeRepository,
            politicaSlaRepository,
            baseConhecimentoRepository,
            request.DepartamentoResponsavelId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.PrioridadePadraoId,
            slaPadraoId,
            request.ArtigoBaseConhecimentoId,
            cancellationToken);

        var slug = string.Equals(servico.Nome, request.Nome, StringComparison.Ordinal)
            ? servico.Slug
            : await CatalogoServicoSlugHelper.GerarSlugUnicoAsync(catalogoServicoRepository, request.Nome, servico.Id, cancellationToken);

        var dadosAntes = CatalogoServicoMapeamentos.SerializarAuditoria(servico);

        servico.Atualizar(
            request.Nome,
            slug,
            request.Descricao,
            request.InstrucoesSolicitante,
            request.DepartamentoResponsavelId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.PrioridadePadraoId,
            slaPadraoId,
            request.ArtigoBaseConhecimentoId,
            request.Visibilidade,
            request.PermiteAberturaChamado,
            request.RequerAprovacao,
            request.Ordem,
            usuarioAtual.Id,
            usuarioAtual.Login);

        if (!request.Ativo && servico.Ativo)
        {
            servico.Desativar(usuarioAtual.Login);
        }
        else if (request.Ativo && !servico.Ativo)
        {
            servico.Ativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterCatalogoCompletoPorIdAsync(servico.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado apos atualizacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Catalogo de Servicos",
                "CatalogoServico",
                servico.Id.ToString(),
                "Servico do catalogo atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: CatalogoServicoMapeamentos.SerializarAuditoria(completo),
                metadados: CatalogoServicoAuditoriaHelper.CriarMetadados(completo, "AtualizacaoCatalogoServico"),
                cancellationToken: cancellationToken);
        }

        return CatalogoServicoMapeamentos.MapDetalhe(completo);
    }

    public async Task<CatalogoServicoDetalheDto> PublicarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var servico = await ObterCatalogoCompletoPorIdAsync(id, asNoTracking: false, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");

        if (!servico.Ativo)
        {
            throw new InvalidOperationException("Somente servicos ativos podem ser publicados.");
        }

        if (servico.Status == StatusCatalogoServico.Arquivado)
        {
            throw new InvalidOperationException("Servico arquivado nao pode ser publicado diretamente.");
        }

        if (string.IsNullOrWhiteSpace(servico.Nome) ||
            string.IsNullOrWhiteSpace(servico.Descricao) ||
            servico.DepartamentoResponsavelId == Guid.Empty)
        {
            throw new InvalidOperationException("Nome, descricao e departamento responsavel devem estar preenchidos para publicacao.");
        }

        if (servico.PermiteAberturaChamado && !servico.CategoriaId.HasValue)
        {
            throw new InvalidOperationException("Categoria e obrigatoria para publicar servico que permite abertura de chamado.");
        }

        var dadosAntes = CatalogoServicoMapeamentos.SerializarAuditoria(servico);
        servico.Publicar(usuarioAtual.Id, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await ObterCatalogoCompletoPorIdAsync(servico.Id, asNoTracking: true, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado apos publicacao.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Catalogo de Servicos",
                "CatalogoServico",
                servico.Id.ToString(),
                "Servico do catalogo publicado.",
                dadosAntes: dadosAntes,
                dadosDepois: CatalogoServicoMapeamentos.SerializarAuditoria(completo),
                metadados: CatalogoServicoAuditoriaHelper.CriarMetadados(completo, "PublicacaoCatalogoServico"),
                cancellationToken: cancellationToken);
        }

        return CatalogoServicoMapeamentos.MapDetalhe(completo);
    }

    public async Task<AlterarSituacaoCadastroResponse> ArquivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var servico = await catalogoServicoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");

        if (servico.Status == StatusCatalogoServico.Arquivado && !servico.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(servico.Id, false, "Servico ja estava arquivado.");
        }

        servico.Arquivar(usuarioAtual.Id, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarInativacaoAsync(
                "Catalogo de Servicos",
                "CatalogoServico",
                servico.Id.ToString(),
                "Servico do catalogo arquivado.",
                CatalogoServicoAuditoriaHelper.CriarMetadados(servico, "ArquivamentoCatalogoServico"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(servico.Id, false, "Servico arquivado com sucesso.");
    }

    public async Task<AlterarSituacaoCadastroResponse> ReativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var servico = await catalogoServicoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Servico do catalogo nao encontrado.");

        if (servico.Status != StatusCatalogoServico.Arquivado)
        {
            throw new InvalidOperationException("Somente servicos arquivados podem ser reativados.");
        }

        servico.TornarRascunho(usuarioAtual.Id, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAtivacaoAsync(
                "Catalogo de Servicos",
                "CatalogoServico",
                servico.Id.ToString(),
                "Servico do catalogo reativado.",
                CatalogoServicoAuditoriaHelper.CriarMetadados(servico, "ReativacaoCatalogoServico"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(servico.Id, true, "Servico reativado com sucesso.");
    }

    private async Task<CatalogoServico?> ObterCatalogoCompletoPorIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken)
    {
        var query = catalogoServicoRepository.Query()
            .Include(x => x.DepartamentoResponsavel)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.PrioridadePadrao)
            .Include(x => x.SlaPadrao)
            .Include(x => x.ArtigoBaseConhecimento)
            .Where(x => x.Id == id);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}

internal static class CatalogoServicoValidacoes
{
    public static Guid? ResolverSlaPadraoId(Guid? slaPadraoId, Guid? politicaSlaId)
    {
        if (slaPadraoId.HasValue && politicaSlaId.HasValue && slaPadraoId.Value != politicaSlaId.Value)
        {
            throw new ArgumentException("SlaPadraoId e PoliticaSlaId nao podem ter valores diferentes na mesma requisicao.");
        }

        return politicaSlaId ?? slaPadraoId;
    }

    public static void ValidarDadosBasicos(string nome, string descricao, Guid departamentoResponsavelId)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do servico e obrigatorio.", nameof(nome));
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("Descricao do servico e obrigatoria.", nameof(descricao));
        }

        if (departamentoResponsavelId == Guid.Empty)
        {
            throw new ArgumentException("Departamento responsavel e obrigatorio.", nameof(departamentoResponsavelId));
        }
    }

    public static async Task ValidarRelacionamentosAsync(
        IRepository<Departamento> departamentoRepository,
        IRepository<CategoriaChamado> categoriaRepository,
        IRepository<SubcategoriaChamado> subcategoriaRepository,
        IRepository<PrioridadeChamado> prioridadeRepository,
        IRepository<PoliticaSla> politicaSlaRepository,
        IRepository<BaseConhecimentoArtigo> baseConhecimentoRepository,
        Guid departamentoResponsavelId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        Guid? prioridadePadraoId,
        Guid? slaPadraoId,
        Guid? artigoBaseConhecimentoId,
        CancellationToken cancellationToken)
    {
        var departamentoValido = await departamentoRepository.Query()
            .AnyAsync(x => x.Id == departamentoResponsavelId && x.Ativo, cancellationToken);
        if (!departamentoValido)
        {
            throw new InvalidOperationException("Departamento responsavel informado nao encontrado ou inativo.");
        }

        if (categoriaId.HasValue)
        {
            var categoriaValida = await categoriaRepository.Query()
                .AnyAsync(x => x.Id == categoriaId.Value && x.Ativo, cancellationToken);
            if (!categoriaValida)
            {
                throw new InvalidOperationException("Categoria informada nao encontrada ou inativa.");
            }
        }

        if (subcategoriaId.HasValue)
        {
            var subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == subcategoriaId.Value && x.Ativo, cancellationToken);

            if (subcategoria is null)
            {
                throw new InvalidOperationException("Subcategoria informada nao encontrada ou inativa.");
            }

            if (categoriaId.HasValue && subcategoria.CategoriaChamadoId != categoriaId.Value)
            {
                throw new InvalidOperationException("Subcategoria informada nao pertence a categoria selecionada.");
            }
        }

        if (prioridadePadraoId.HasValue)
        {
            var prioridadeValida = await prioridadeRepository.Query()
                .AnyAsync(x => x.Id == prioridadePadraoId.Value && x.Ativo, cancellationToken);
            if (!prioridadeValida)
            {
                throw new InvalidOperationException("Prioridade padrao informada nao encontrada ou inativa.");
            }
        }

        if (slaPadraoId.HasValue)
        {
            var slaValido = await politicaSlaRepository.Query()
                .AnyAsync(x => x.Id == slaPadraoId.Value && x.Ativo, cancellationToken);
            if (!slaValido)
            {
                throw new InvalidOperationException("Politica de SLA informada nao encontrada ou inativa.");
            }
        }

        if (artigoBaseConhecimentoId.HasValue)
        {
            var artigoValido = await baseConhecimentoRepository.Query()
                .AnyAsync(x => x.Id == artigoBaseConhecimentoId.Value && x.Ativo, cancellationToken);
            if (!artigoValido)
            {
                throw new InvalidOperationException("Artigo da base de conhecimento informado nao encontrado ou inativo.");
            }
        }
    }
}

internal static class CatalogoServicoMapeamentos
{
    public static CatalogoServicoListagemDto MapListagem(CatalogoServico servico)
        => new(
            servico.Id,
            servico.Nome,
            servico.Slug,
            servico.Descricao ?? string.Empty,
            servico.DepartamentoResponsavelId,
            servico.DepartamentoResponsavel?.Nome,
            servico.CategoriaId,
            servico.Categoria?.Nome,
            servico.SubcategoriaId,
            servico.Subcategoria?.Nome,
            servico.PrioridadePadraoId,
            servico.PrioridadePadrao?.Nome,
            servico.SlaPadraoId,
            servico.SlaPadrao?.Nome,
            servico.Status,
            DescricaoStatus(servico.Status),
            servico.Visibilidade,
            DescricaoVisibilidade(servico.Visibilidade),
            servico.PermiteAberturaChamado,
            servico.RequerAprovacao,
            servico.Ordem,
            servico.Ativo,
            servico.CriadoEm,
            servico.AtualizadoEm,
            servico.PublicadoEm,
            servico.ArquivadoEm);

    public static CatalogoServicoDetalheDto MapDetalhe(CatalogoServico servico)
        => new(
            servico.Id,
            servico.Nome,
            servico.Slug,
            servico.Descricao ?? string.Empty,
            servico.InstrucoesSolicitante,
            servico.DepartamentoResponsavelId,
            servico.DepartamentoResponsavel?.Nome,
            servico.CategoriaId,
            servico.Categoria?.Nome,
            servico.SubcategoriaId,
            servico.Subcategoria?.Nome,
            servico.PrioridadePadraoId,
            servico.PrioridadePadrao?.Nome,
            servico.SlaPadraoId,
            servico.SlaPadrao?.Nome,
            servico.ArtigoBaseConhecimentoId,
            servico.ArtigoBaseConhecimento?.Titulo,
            servico.Status,
            DescricaoStatus(servico.Status),
            servico.Visibilidade,
            DescricaoVisibilidade(servico.Visibilidade),
            servico.PermiteAberturaChamado,
            servico.RequerAprovacao,
            servico.Ordem,
            servico.Ativo,
            servico.CriadoEm,
            servico.CriadoPorUsuarioId,
            servico.AtualizadoEm,
            servico.AtualizadoPorUsuarioId,
            servico.PublicadoEm,
            servico.PublicadoPorUsuarioId,
            servico.ArquivadoEm,
            servico.ArquivadoPorUsuarioId);

    public static string? SerializarAuditoria(CatalogoServico servico)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            servico.Nome,
            servico.Slug,
            servico.Descricao,
            servico.InstrucoesSolicitante,
            servico.DepartamentoResponsavelId,
            servico.CategoriaId,
            servico.SubcategoriaId,
            servico.PrioridadePadraoId,
            servico.SlaPadraoId,
            servico.ArtigoBaseConhecimentoId,
            servico.Status,
            servico.Visibilidade,
            servico.PermiteAberturaChamado,
            servico.RequerAprovacao,
            servico.Ordem,
            servico.PublicadoEm,
            servico.PublicadoPorUsuarioId,
            servico.ArquivadoEm,
            servico.ArquivadoPorUsuarioId,
            servico.Ativo
        });

    private static string DescricaoStatus(StatusCatalogoServico value) => value switch
    {
        StatusCatalogoServico.Rascunho => "Rascunho",
        StatusCatalogoServico.Publicado => "Publicado",
        StatusCatalogoServico.Arquivado => "Arquivado",
        _ => value.ToString()
    };

    private static string DescricaoVisibilidade(VisibilidadeCatalogoServico value) => value switch
    {
        VisibilidadeCatalogoServico.Interno => "Interno",
        VisibilidadeCatalogoServico.Solicitante => "Solicitante",
        VisibilidadeCatalogoServico.Atendente => "Atendente",
        VisibilidadeCatalogoServico.Administrador => "Administrador",
        _ => value.ToString()
    };
}

internal static class CatalogoServicoAuditoriaHelper
{
    public static string CriarMetadados(CatalogoServico servico, string operacao, string? observacao = null)
        => AuditoriaDiffHelper.CriarMetadadosPadrao(
            origem: "api",
            modulo: "Catalogo de Servicos",
            entidade: "CatalogoServico",
            entidadeId: servico.Id.ToString(),
            codigo: servico.Slug,
            nome: servico.Nome,
            operacao: operacao,
            resultado: "Sucesso",
            observacao: observacao);
}

internal static class CatalogoServicoSlugHelper
{
    private static readonly Regex MultiHyphenRegex = new("-+", RegexOptions.Compiled);

    public static async Task<string> GerarSlugUnicoAsync(
        IRepository<CatalogoServico> catalogoServicoRepository,
        string nome,
        Guid? idIgnorado,
        CancellationToken cancellationToken)
    {
        var slugBase = NormalizarSlug(nome);
        if (string.IsNullOrWhiteSpace(slugBase))
        {
            slugBase = "servico";
        }

        var query = catalogoServicoRepository.Query().AsNoTracking();
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

    private static string NormalizarSlug(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return string.Empty;
        }

        var texto = RemoverAcentos(nome).ToLowerInvariant();
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
