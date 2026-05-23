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

public sealed class ListarArtigosConhecimentoDoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ChamadoArtigoConhecimento> chamadoArtigoConhecimentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarArtigosConhecimentoDoChamadoUseCase
{
    public async Task<IReadOnlyCollection<ChamadoArtigoConhecimentoDto>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken);
        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var query = chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Artigo).ThenInclude(x => x.Categoria)
            .Include(x => x.VinculadoPorUsuario)
            .Where(x => x.ChamadoId == chamadoId)
            .AsQueryable();

        query = BaseConhecimentoChamadoVisibilidadeHelper.FiltrarVinculosPorVisibilidade(query, usuario);

        var itens = await query
            .OrderByDescending(x => x.VinculadoEm)
            .ToListAsync(cancellationToken);

        return itens.Select(BaseConhecimentoChamadoMapeamentos.MapVinculo).ToArray();
    }
}

public sealed class VincularArtigoConhecimentoAoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IRepository<ChamadoArtigoConhecimento> chamadoArtigoConhecimentoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IVincularArtigoConhecimentoAoChamadoUseCase
{
    public async Task<ChamadoArtigoConhecimentoDto> ExecutarAsync(
        Guid chamadoId,
        Guid artigoId,
        VincularArtigoChamadoRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (artigoId == Guid.Empty)
        {
            throw new ArgumentException("Id do artigo invalido.", nameof(artigoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == chamadoId && x.Ativo)
            .Select(x => new { x.Id, x.Codigo, x.Titulo })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var artigo = await artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == artigoId, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo da base de conhecimento nao encontrado.");

        if (!artigo.Ativo || artigo.Status != StatusArtigoConhecimento.Publicado)
        {
            throw new InvalidOperationException("Somente artigos publicados e ativos podem ser vinculados.");
        }

        if (!BaseConhecimentoChamadoVisibilidadeHelper.PodeVisualizarArtigo(usuario, artigo.Visibilidade))
        {
            throw new InvalidOperationException("Artigo indisponivel para vinculacao neste perfil.");
        }

        var jaVinculado = await chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.ChamadoId == chamadoId && x.ArtigoId == artigoId, cancellationToken);

        if (jaVinculado)
        {
            throw new InvalidOperationException("Este artigo ja esta vinculado ao chamado.");
        }

        var vinculo = new ChamadoArtigoConhecimento(
            chamadoId,
            artigoId,
            usuario.Id,
            request?.Observacao,
            usuario.Login);

        await chamadoArtigoConhecimentoRepository.AddAsync(vinculo, cancellationToken);

        var historico = new HistoricoChamado(
            chamadoId,
            TipoHistoricoChamado.ArtigoConhecimentoVinculado,
            $"Artigo da base de conhecimento vinculado: {artigo.Titulo}",
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var vinculoCriado = await chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Artigo).ThenInclude(x => x.Categoria)
            .Include(x => x.VinculadoPorUsuario)
            .FirstAsync(x => x.ChamadoId == chamadoId && x.ArtigoId == artigoId, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Base de Conhecimento",
                Entidade = "ChamadoArtigoConhecimento",
                EntidadeId = $"{chamadoId}:{artigoId}",
                Acao = TipoAcaoAuditoria.Criacao,
                Descricao = "Artigo vinculado ao chamado.",
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    ArtigoId = artigoId,
                    artigo.Titulo,
                    vinculo.VinculadoEm,
                    VinculadoPorUsuarioId = usuario.Id
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Base de Conhecimento",
                    entidade: "ChamadoArtigoConhecimento",
                    entidadeId: $"{chamadoId}:{artigoId}",
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: "VincularArtigoChamado",
                    resultado: "Sucesso",
                    observacao: $"Artigo: {artigo.Titulo}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
            }, cancellationToken);
        }

        return BaseConhecimentoChamadoMapeamentos.MapVinculo(vinculoCriado);
    }
}

public sealed class RemoverArtigoConhecimentoDoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ChamadoArtigoConhecimento> chamadoArtigoConhecimentoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IRemoverArtigoConhecimentoDoChamadoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid chamadoId, Guid artigoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (artigoId == Guid.Empty)
        {
            throw new ArgumentException("Id do artigo invalido.", nameof(artigoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == chamadoId && x.Ativo)
            .Select(x => new { x.Id, x.Codigo, x.Titulo })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var vinculo = await chamadoArtigoConhecimentoRepository.Query()
            .Include(x => x.Artigo)
            .FirstOrDefaultAsync(x => x.ChamadoId == chamadoId && x.ArtigoId == artigoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vinculo de artigo nao encontrado para o chamado.");

        chamadoArtigoConhecimentoRepository.Remove(vinculo);

        var historico = new HistoricoChamado(
            chamadoId,
            TipoHistoricoChamado.ArtigoConhecimentoDesvinculado,
            $"Artigo da base de conhecimento removido do chamado: {vinculo.Artigo.Titulo}",
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Base de Conhecimento",
                Entidade = "ChamadoArtigoConhecimento",
                EntidadeId = $"{chamadoId}:{artigoId}",
                Acao = TipoAcaoAuditoria.Edicao,
                Descricao = "Vinculo de artigo removido do chamado.",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    ArtigoId = artigoId,
                    vinculo.Artigo.Titulo,
                    vinculo.VinculadoEm,
                    vinculo.VinculadoPorUsuarioId
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Base de Conhecimento",
                    entidade: "ChamadoArtigoConhecimento",
                    entidadeId: $"{chamadoId}:{artigoId}",
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: "RemoverVinculoArtigoChamado",
                    resultado: "Sucesso",
                    observacao: $"Artigo: {vinculo.Artigo.Titulo}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
            }, cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(
            vinculo.ArtigoId,
            false,
            "Vinculo removido com sucesso.");
    }
}

public sealed class BuscarArtigosConhecimentoParaVinculoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<BaseConhecimentoArtigo> artigoRepository,
    IRepository<ChamadoArtigoConhecimento> chamadoArtigoConhecimentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IBuscarArtigosConhecimentoParaVinculoUseCase
{
    public async Task<PagedResultResponse<ArtigoConhecimentoDisponivelParaVinculoDto>> ExecutarAsync(
        Guid chamadoId,
        BuscarArtigosParaVinculoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken);
        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var query = artigoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .Where(x => x.Ativo && x.Status == StatusArtigoConhecimento.Publicado)
            .AsQueryable();

        query = BaseConhecimentoChamadoVisibilidadeHelper.FiltrarArtigosPorVisibilidade(query, usuario);

        var artigosJaVinculados = chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .Where(x => x.ChamadoId == chamadoId)
            .Select(x => x.ArtigoId);

        query = query.Where(x => !artigosJaVinculados.Contains(x.Id));

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
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<ArtigoConhecimentoDisponivelParaVinculoDto>
        {
            Items = itens.Select(BaseConhecimentoChamadoMapeamentos.MapDisponivelParaVinculo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}

internal static class BaseConhecimentoChamadoVisibilidadeHelper
{
    public static IQueryable<BaseConhecimentoArtigo> FiltrarArtigosPorVisibilidade(
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

        return query.Where(_ => false);
    }

    public static IQueryable<ChamadoArtigoConhecimento> FiltrarVinculosPorVisibilidade(
        IQueryable<ChamadoArtigoConhecimento> query,
        UsuarioContextoAplicacao usuario)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return query;
        }

        if (usuario.PossuiPerfil("Atendente"))
        {
            return query.Where(x =>
                x.Artigo.Visibilidade == VisibilidadeArtigoConhecimento.Solicitante ||
                x.Artigo.Visibilidade == VisibilidadeArtigoConhecimento.Atendente);
        }

        return query.Where(_ => false);
    }

    public static bool PodeVisualizarArtigo(UsuarioContextoAplicacao usuario, VisibilidadeArtigoConhecimento visibilidade)
    {
        if (usuario.PossuiPerfil("Administrador"))
        {
            return true;
        }

        if (usuario.PossuiPerfil("Atendente"))
        {
            return visibilidade is
                VisibilidadeArtigoConhecimento.Solicitante or
                VisibilidadeArtigoConhecimento.Atendente;
        }

        return false;
    }
}

internal static class BaseConhecimentoChamadoMapeamentos
{
    public static ChamadoArtigoConhecimentoDto MapVinculo(ChamadoArtigoConhecimento vinculo)
        => new(
            vinculo.ArtigoId,
            vinculo.Artigo.Titulo,
            vinculo.Artigo.Slug,
            vinculo.Artigo.Resumo,
            (int)vinculo.Artigo.Status,
            DescricaoStatus(vinculo.Artigo.Status),
            (int)vinculo.Artigo.Visibilidade,
            DescricaoVisibilidade(vinculo.Artigo.Visibilidade),
            vinculo.Artigo.CategoriaId,
            vinculo.Artigo.Categoria?.Nome,
            vinculo.VinculadoEm,
            vinculo.VinculadoPorUsuarioId,
            vinculo.VinculadoPorUsuario.Nome,
            vinculo.Observacao);

    public static ArtigoConhecimentoDisponivelParaVinculoDto MapDisponivelParaVinculo(BaseConhecimentoArtigo artigo)
        => new(
            artigo.Id,
            artigo.Titulo,
            artigo.Slug,
            artigo.Resumo,
            (int)artigo.Status,
            DescricaoStatus(artigo.Status),
            (int)artigo.Visibilidade,
            DescricaoVisibilidade(artigo.Visibilidade),
            artigo.CategoriaId,
            artigo.Categoria?.Nome,
            artigo.Tags,
            artigo.PublicadoEm);

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
