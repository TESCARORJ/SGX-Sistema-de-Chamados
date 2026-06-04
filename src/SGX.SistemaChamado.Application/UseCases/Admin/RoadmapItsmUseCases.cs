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

public sealed class ListarRoadmapItsmUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRoadmapItsmUseCase
{
    public async Task<IReadOnlyCollection<RoadmapItsmResumoResponse>> ExecutarAsync(FiltroRoadmapItsmRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = roadmapRepository.Query()
            .AsNoTracking()
            .Include(x => x.RoadmapCategoria)
            .Include(x => x.ChecklistItens)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Prioridade.HasValue)
        {
            query = query.Where(x => x.Prioridade == request.Prioridade.Value);
        }

        if (request.Impacto.HasValue)
        {
            query = query.Where(x => x.Impacto == request.Impacto.Value);
        }

        if (request.RoadmapCategoriaId.HasValue && request.RoadmapCategoriaId.Value != Guid.Empty)
        {
            query = query.Where(x => x.RoadmapCategoriaId == request.RoadmapCategoriaId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(request.Categoria))
        {
            var categoria = request.Categoria.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Categoria.ToLower().Contains(categoria) ||
                (x.RoadmapCategoria != null && x.RoadmapCategoria.Nome.ToLower().Contains(categoria)));
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        var itens = await query
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Area)
            .ToListAsync(cancellationToken);

        return itens.Select(RoadmapItsmMaps.MapResumo).ToArray();
    }
}

public sealed class ObterRoadmapItsmItemUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterRoadmapItsmItemUseCase
{
    public async Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var item = await roadmapRepository.Query()
            .AsNoTracking()
            .Include(x => x.RoadmapCategoria)
            .Include(x => x.ChecklistItens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");

        return RoadmapItsmMaps.MapDetalhe(item);
    }
}

public sealed class CriarRoadmapItsmItemUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IRepository<RoadmapCategoria> roadmapCategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarRoadmapItsmItemUseCase
{
    public async Task<RoadmapItsmDetalheResponse> ExecutarAsync(CriarRoadmapItsmItemRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await RoadmapCategoriaHelper.ObterCategoriaValidaAsync(roadmapCategoriaRepository, request.RoadmapCategoriaId, cancellationToken);
        var categoriaLegado = string.IsNullOrWhiteSpace(request.Categoria)
            ? categoria?.Nome ?? string.Empty
            : request.Categoria.Trim();

        var item = new RoadmapItsmItem(
            request.Area,
            categoriaLegado,
            request.Objetivo,
            categoria?.Id,
            request.SituacaoAtual,
            request.AtencaoTecnica,
            request.Status,
            request.Prioridade,
            request.Impacto,
            request.Decisao,
            request.Observacao,
            request.Responsavel,
            request.PrazoAlvo,
            request.Ordem,
            request.StatusImplementacao,
            request.StatusTecnico,
            request.PercentualImplementacao ?? 0,
            request.PendenciasTecnicas,
            request.PendenciasHomologacao,
            request.EvidenciaImplementacao,
            request.DataConclusaoTecnica,
            request.DataHomologacao,
            request.CriterioAceite,
            request.ProximaAcao,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            item.Desativar(usuarioAtual.Login);
        }

        await roadmapRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await roadmapRepository.Query()
            .AsNoTracking()
            .Include(x => x.RoadmapCategoria)
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Roadmap ITSM",
                "RoadmapItsmItem",
                item.Id.ToString(),
                "Item de roadmap ITSM criado.",
                dadosDepois: RoadmapItsmMaps.SerializarItemAuditoria(completo),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosItem(completo, "CriacaoItemRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapDetalhe(completo);
    }
}

public sealed class AtualizarRoadmapItsmItemUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IRepository<RoadmapCategoria> roadmapCategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarRoadmapItsmItemUseCase
{
    public async Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, AtualizarRoadmapItsmItemRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await RoadmapCategoriaHelper.ObterCategoriaValidaAsync(roadmapCategoriaRepository, request.RoadmapCategoriaId, cancellationToken);

        var item = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .Include(x => x.RoadmapCategoria)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarItemAuditoria(item);

        var categoriaLegado = string.IsNullOrWhiteSpace(request.Categoria)
            ? categoria?.Nome ?? item.Categoria
            : request.Categoria.Trim();

        item.Atualizar(
            request.Area,
            categoriaLegado,
            request.Objetivo,
            categoria?.Id,
            request.SituacaoAtual,
            request.AtencaoTecnica,
            request.Status,
            request.Prioridade,
            request.Impacto,
            request.Decisao,
            request.Observacao,
            request.Responsavel,
            request.PrazoAlvo,
            request.Ordem,
            request.StatusImplementacao,
            request.StatusTecnico,
            request.PercentualImplementacao ?? item.PercentualImplementacao,
            request.PendenciasTecnicas,
            request.PendenciasHomologacao,
            request.EvidenciaImplementacao,
            request.DataConclusaoTecnica,
            request.DataHomologacao,
            request.CriterioAceite,
            request.ProximaAcao,
            usuarioAtual.Login);

        item.RecalcularPercentualImplementacao(item.ChecklistItens);

        if (request.Ativo && !item.Ativo)
        {
            item.Ativar(usuarioAtual.Login);
        }
        else if (!request.Ativo && item.Ativo)
        {
            item.Desativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var completo = await roadmapRepository.Query()
            .AsNoTracking()
            .Include(x => x.RoadmapCategoria)
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Roadmap ITSM",
                "RoadmapItsmItem",
                item.Id.ToString(),
                "Item de roadmap ITSM atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: RoadmapItsmMaps.SerializarItemAuditoria(completo),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosItem(completo, "AtualizacaoItemRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapDetalhe(completo);
    }
}

public sealed class AtualizarStatusRoadmapItsmUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarStatusRoadmapItsmUseCase
{
    public async Task<RoadmapItsmDetalheResponse> ExecutarAsync(Guid id, AtualizarStatusRoadmapItsmRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await roadmapRepository.Query()
            .Include(x => x.RoadmapCategoria)
            .Include(x => x.ChecklistItens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");
        var dadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
        {
            item.Status,
            item.Prioridade,
            item.Decisao,
            item.Responsavel,
            item.PrazoAlvo,
            item.Observacao
        });

        item.AtualizarClassificacao(
            request.Status,
            request.Prioridade,
            request.Decisao,
            request.Responsavel,
            request.PrazoAlvo,
            request.Observacao,
            usuarioAtual.Login);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Roadmap ITSM",
                "RoadmapItsmItem",
                item.Id.ToString(),
                "Status/classificacao do item de roadmap ITSM alterado.",
                dadosAntes: dadosAntes,
                dadosDepois: AuditoriaDiffHelper.SerializarSeguro(new
                {
                    item.Status,
                    item.Prioridade,
                    item.Decisao,
                    item.Responsavel,
                    item.PrazoAlvo,
                    item.Observacao
                }),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosItem(item, "AtualizacaoStatusRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapDetalhe(item);
    }
}

public sealed class InativarRoadmapItsmItemUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IInativarRoadmapItsmItemUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await roadmapRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");

        item.Desativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarInativacaoAsync(
                "Roadmap ITSM",
                "RoadmapItsmItem",
                item.Id.ToString(),
                "Item de roadmap ITSM inativado.",
                RoadmapItsmAuditoriaHelper.CriarMetadadosItem(item, "InativacaoItemRoadmap"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(item.Id, false, "Item do roadmap ITSM inativado com sucesso.");
    }
}

public sealed class ReativarRoadmapItsmItemUseCase(
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReativarRoadmapItsmItemUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await roadmapRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");

        item.Ativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAtivacaoAsync(
                "Roadmap ITSM",
                "RoadmapItsmItem",
                item.Id.ToString(),
                "Item de roadmap ITSM reativado.",
                RoadmapItsmAuditoriaHelper.CriarMetadadosItem(item, "ReativacaoItemRoadmap"),
                cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(item.Id, true, "Item do roadmap ITSM reativado com sucesso.");
    }
}

public sealed class ListarRoadmapImplementacoesFuturasUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRoadmapImplementacoesFuturasUseCase
{
    public async Task<PagedResultResponse<RoadmapImplementacaoFuturaResponse>> ExecutarAsync(
        FiltroRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = implementacaoRepository.Query().AsNoTracking().AsQueryable();

        if (request.RoadmapItemId.HasValue && request.RoadmapItemId.Value != Guid.Empty)
        {
            query = query.Where(x => x.RoadmapItemId == request.RoadmapItemId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Titulo.ToLower().Contains(texto) ||
                (x.Descricao != null && x.Descricao.ToLower().Contains(texto)));
        }

        if (request.Tipo.HasValue)
        {
            query = query.Where(x => x.Tipo == request.Tipo.Value);
        }

        if (request.Prioridade.HasValue)
        {
            query = query.Where(x => x.Prioridade == request.Prioridade.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Responsavel))
        {
            var responsavel = request.Responsavel.Trim().ToLowerInvariant();
            query = query.Where(x => x.Responsavel != null && x.Responsavel.ToLower().Contains(responsavel));
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.Prioridade)
            .ThenBy(x => x.PrazoAlvo)
            .ThenBy(x => x.Titulo)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<RoadmapImplementacaoFuturaResponse>
        {
            Items = itens.Select(RoadmapItsmMaps.MapImplementacao).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }
}

public sealed class ListarRoadmapImplementacoesPorItemUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRoadmapImplementacoesPorItemUseCase
{
    public async Task<IReadOnlyCollection<RoadmapImplementacaoFuturaResponse>> ExecutarAsync(
        Guid roadmapItemId,
        CancellationToken cancellationToken = default)
    {
        if (roadmapItemId == Guid.Empty)
        {
            throw new ArgumentException("RoadmapItemId inválido.", nameof(roadmapItemId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var roadmapExiste = await roadmapRepository.Query()
            .AnyAsync(x => x.Id == roadmapItemId, cancellationToken);

        if (!roadmapExiste)
        {
            throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");
        }

        var itens = await implementacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.RoadmapItemId == roadmapItemId)
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.Prioridade)
            .ThenBy(x => x.PrazoAlvo)
            .ThenBy(x => x.Titulo)
            .ToListAsync(cancellationToken);

        return itens.Select(RoadmapItsmMaps.MapImplementacao).ToArray();
    }
}

public sealed class ObterRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterRoadmapImplementacaoFuturaUseCase
{
    public async Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var item = await implementacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Implementacao futura nao encontrada.");

        return RoadmapItsmMaps.MapImplementacao(item);
    }
}

public sealed class CriarRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarRoadmapImplementacaoFuturaUseCase
{
    public async Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(
        CriarRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var roadmapExiste = await roadmapRepository.Query()
            .AnyAsync(x => x.Id == request.RoadmapItemId, cancellationToken);

        if (!roadmapExiste)
        {
            throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");
        }

        var item = new RoadmapImplementacaoFutura(
            request.RoadmapItemId,
            request.Titulo,
            request.Descricao,
            request.Tipo,
            request.Prioridade,
            request.Status,
            request.Responsavel,
            request.PrazoAlvo,
            dataConclusao: null,
            request.Observacao,
            usuarioAtual.Login);

        if (request.Status == StatusRoadmapImplementacaoFutura.Concluido)
        {
            item.Concluir(null, usuarioAtual.Login);
        }

        await implementacaoRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RoadmapItsmMaps.MapImplementacao(item);
    }
}

public sealed class AtualizarRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarRoadmapImplementacaoFuturaUseCase
{
    public async Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(
        Guid id,
        AtualizarRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await implementacaoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Implementacao futura nao encontrada.");

        var dataConclusao = request.Status == StatusRoadmapImplementacaoFutura.Concluido
            ? request.DataConclusao ?? DateTime.UtcNow
            : request.DataConclusao;

        item.Atualizar(
            request.Titulo,
            request.Descricao,
            request.Tipo,
            request.Prioridade,
            request.Status,
            request.Responsavel,
            request.PrazoAlvo,
            dataConclusao,
            request.Observacao,
            usuarioAtual.Login);

        if (request.Ativo && !item.Ativo)
        {
            item.Ativar(usuarioAtual.Login);
        }
        else if (!request.Ativo && item.Ativo)
        {
            item.Desativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return RoadmapItsmMaps.MapImplementacao(item);
    }
}

public sealed class ConcluirRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IConcluirRoadmapImplementacaoFuturaUseCase
{
    public async Task<RoadmapImplementacaoFuturaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await implementacaoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Implementacao futura nao encontrada.");

        item.Concluir(null, usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RoadmapItsmMaps.MapImplementacao(item);
    }
}

public sealed class InativarRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarRoadmapImplementacaoFuturaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await implementacaoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Implementacao futura nao encontrada.");

        item.Desativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(item.Id, false, "Implementacao futura inativada com sucesso.");
    }
}

public sealed class ReativarRoadmapImplementacaoFuturaUseCase(
    IRepository<RoadmapImplementacaoFutura> implementacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarRoadmapImplementacaoFuturaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await implementacaoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Implementacao futura nao encontrada.");

        item.Ativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(item.Id, true, "Implementacao futura reativada com sucesso.");
    }
}

public sealed class ListarRoadmapCategoriasUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRoadmapCategoriasUseCase
{
    public async Task<IReadOnlyCollection<RoadmapCategoriaResponse>> ExecutarAsync(FiltroRoadmapCategoriaRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = categoriaRepository.Query().AsNoTracking().AsQueryable();
        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLowerInvariant();
            query = query.Where(x => x.Nome.ToLower().Contains(texto) || (x.Descricao != null && x.Descricao.ToLower().Contains(texto)));
        }

        var itens = await query
            .OrderBy(x => x.Ordem ?? int.MaxValue)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return itens.Select(RoadmapItsmMaps.MapCategoria).ToArray();
    }
}

public sealed class ObterRoadmapCategoriaUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterRoadmapCategoriaUseCase
{
    public async Task<RoadmapCategoriaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var item = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de roadmap nao encontrada.");

        return RoadmapItsmMaps.MapCategoria(item);
    }
}

public sealed class CriarRoadmapCategoriaUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarRoadmapCategoriaUseCase
{
    public async Task<RoadmapCategoriaResponse> ExecutarAsync(CriarRoadmapCategoriaRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        var existe = await categoriaRepository.Query()
            .AnyAsync(x => x.Nome.ToLower() == nome.ToLower(), cancellationToken);

        if (existe)
        {
            throw new InvalidOperationException("Ja existe categoria de roadmap com o mesmo nome.");
        }

        var item = new RoadmapCategoria(request.Nome, request.Descricao, request.Cor, request.Icone, request.Ordem, usuarioAtual.Login);
        await categoriaRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RoadmapItsmMaps.MapCategoria(item);
    }
}

public sealed class AtualizarRoadmapCategoriaUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarRoadmapCategoriaUseCase
{
    public async Task<RoadmapCategoriaResponse> ExecutarAsync(Guid id, AtualizarRoadmapCategoriaRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await categoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de roadmap nao encontrada.");

        var nome = request.Nome.Trim();
        var existeOutro = await categoriaRepository.Query()
            .AnyAsync(x => x.Id != id && x.Nome.ToLower() == nome.ToLower(), cancellationToken);
        if (existeOutro)
        {
            throw new InvalidOperationException("Ja existe categoria de roadmap com o mesmo nome.");
        }

        item.Atualizar(request.Nome, request.Descricao, request.Cor, request.Icone, request.Ordem, usuarioAtual.Login);

        if (request.Ativo && !item.Ativo)
        {
            item.Ativar(usuarioAtual.Login);
        }
        else if (!request.Ativo && item.Ativo)
        {
            item.Desativar(usuarioAtual.Login);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return RoadmapItsmMaps.MapCategoria(item);
    }
}

public sealed class InativarRoadmapCategoriaUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarRoadmapCategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await categoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de roadmap nao encontrada.");

        item.Desativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(item.Id, false, "Categoria de roadmap inativada com sucesso.");
    }
}

public sealed class ReativarRoadmapCategoriaUseCase(
    IRepository<RoadmapCategoria> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarRoadmapCategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await categoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de roadmap nao encontrada.");

        item.Ativar(usuarioAtual.Login);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlterarSituacaoCadastroResponse(item.Id, true, "Categoria de roadmap reativada com sucesso.");
    }
}

public sealed class ListarRoadmapChecklistPorItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRoadmapChecklistPorItemUseCase
{
    public async Task<IReadOnlyCollection<RoadmapChecklistItemResponse>> ExecutarAsync(Guid roadmapItemId, CancellationToken cancellationToken = default)
    {
        if (roadmapItemId == Guid.Empty)
        {
            throw new ArgumentException("RoadmapItemId inválido.", nameof(roadmapItemId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var roadmapExiste = await roadmapRepository.Query().AnyAsync(x => x.Id == roadmapItemId, cancellationToken);
        if (!roadmapExiste)
        {
            throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");
        }

        var itens = await checklistRepository.Query()
            .AsNoTracking()
            .Where(x => x.RoadmapItemId == roadmapItemId)
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Grupo)
            .ThenBy(x => x.Titulo)
            .ToListAsync(cancellationToken);

        return itens.Select(RoadmapItsmMaps.MapChecklistItem).ToArray();
    }
}

public sealed class CriarRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarRoadmapChecklistItemUseCase
{
    public async Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid roadmapItemId, CriarRoadmapChecklistItemRequest request, CancellationToken cancellationToken = default)
    {
        if (roadmapItemId == Guid.Empty)
        {
            throw new ArgumentException("RoadmapItemId inválido.", nameof(roadmapItemId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstOrDefaultAsync(x => x.Id == roadmapItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Item do roadmap ITSM nao encontrado.");

        var item = new RoadmapChecklistItem(
            roadmapItemId,
            request.Titulo,
            request.Descricao,
            request.Grupo,
            request.Ordem,
            request.Concluido,
            request.Obrigatorio,
            usuarioAtual.Login);

        await checklistRepository.AddAsync(item, cancellationToken);
        roadmap.ChecklistItens.Add(item);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Roadmap ITSM",
                "RoadmapChecklistItem",
                item.Id.ToString(),
                "Checklist do roadmap criado.",
                dadosDepois: RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "CriacaoChecklistRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapChecklistItem(item);
    }
}

public sealed class AtualizarRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarRoadmapChecklistItemUseCase
{
    public async Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid id, AtualizarRoadmapChecklistItemRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarChecklistAuditoria(item);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        item.Atualizar(
            request.Titulo,
            request.Descricao,
            request.Grupo,
            request.Ordem,
            request.Concluido,
            request.Obrigatorio,
            usuarioAtual.Login);

        if (request.Ativo && !item.Ativo)
        {
            item.Ativar(usuarioAtual.Login);
        }
        else if (!request.Ativo && item.Ativo)
        {
            item.Desativar(usuarioAtual.Login);
        }

        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Roadmap ITSM",
                "RoadmapChecklistItem",
                item.Id.ToString(),
                "Checklist do roadmap atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "AtualizacaoChecklistRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapChecklistItem(item);
    }
}

public sealed class ConcluirRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IConcluirRoadmapChecklistItemUseCase
{
    public async Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarChecklistAuditoria(item);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        item.Concluir(usuarioAtual.Login);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Roadmap ITSM",
                "RoadmapChecklistItem",
                item.Id.ToString(),
                "Checklist do roadmap concluido.",
                dadosAntes: dadosAntes,
                dadosDepois: RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "ConclusaoChecklistRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapChecklistItem(item);
    }
}

public sealed class ReabrirRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReabrirRoadmapChecklistItemUseCase
{
    public async Task<RoadmapChecklistItemResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarChecklistAuditoria(item);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        item.Reabrir(usuarioAtual.Login);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Roadmap ITSM",
                "RoadmapChecklistItem",
                item.Id.ToString(),
                "Checklist do roadmap reaberto.",
                dadosAntes: dadosAntes,
                dadosDepois: RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "ReaberturaChecklistRoadmap"),
                cancellationToken: cancellationToken);
        }

        return RoadmapItsmMaps.MapChecklistItem(item);
    }
}

public sealed class InativarRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IInativarRoadmapChecklistItemUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarChecklistAuditoria(item);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        item.Desativar(usuarioAtual.Login);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Roadmap ITSM",
                Entidade = "RoadmapChecklistItem",
                EntidadeId = item.Id.ToString(),
                Acao = TipoAcaoAuditoria.Inativacao,
                Descricao = "Checklist do roadmap inativado.",
                DadosAntes = dadosAntes,
                DadosDepois = RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                Metadados = RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "InativacaoChecklistRoadmap")
            }, cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(item.Id, false, "Checklist do roadmap inativado com sucesso.");
    }
}

public sealed class ExcluirRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IExcluirRoadmapChecklistItemUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        checklistRepository.Remove(item);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens.Where(x => x.Id != item.Id));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarExclusaoLogicaAsync(
                "Roadmap ITSM",
                "RoadmapChecklistItem",
                item.Id.ToString(),
                "Checklist do roadmap removido.",
                dadosAntes: RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                metadados: RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "ExclusaoChecklistRoadmap"),
                cancellationToken: cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(id, false, "Checklist do roadmap excluido com sucesso.");
    }
}

public sealed class ReativarRoadmapChecklistItemUseCase(
    IRepository<RoadmapChecklistItem> checklistRepository,
    IRepository<RoadmapItsmItem> roadmapRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReativarRoadmapChecklistItemUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id inválido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var item = await checklistRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist do roadmap nao encontrado.");
        var dadosAntes = RoadmapItsmMaps.SerializarChecklistAuditoria(item);

        var roadmap = await roadmapRepository.Query()
            .Include(x => x.ChecklistItens)
            .FirstAsync(x => x.Id == item.RoadmapItemId, cancellationToken);

        item.Ativar(usuarioAtual.Login);
        roadmap.RecalcularPercentualImplementacao(roadmap.ChecklistItens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Roadmap ITSM",
                Entidade = "RoadmapChecklistItem",
                EntidadeId = item.Id.ToString(),
                Acao = TipoAcaoAuditoria.Ativacao,
                Descricao = "Checklist do roadmap reativado.",
                DadosAntes = dadosAntes,
                DadosDepois = RoadmapItsmMaps.SerializarChecklistAuditoria(item),
                Metadados = RoadmapItsmAuditoriaHelper.CriarMetadadosChecklist(item, roadmap, "ReativacaoChecklistRoadmap")
            }, cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(item.Id, true, "Checklist do roadmap reativado com sucesso.");
    }
}
internal static class RoadmapCategoriaHelper
{
    public static async Task<RoadmapCategoria?> ObterCategoriaValidaAsync(
        IRepository<RoadmapCategoria> roadmapCategoriaRepository,
        Guid? roadmapCategoriaId,
        CancellationToken cancellationToken)
    {
        if (!roadmapCategoriaId.HasValue || roadmapCategoriaId.Value == Guid.Empty)
        {
            return null;
        }

        var categoria = await roadmapCategoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == roadmapCategoriaId.Value, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de roadmap nao encontrada.");

        if (!categoria.Ativo)
        {
            throw new InvalidOperationException("Categoria de roadmap inativa nao pode ser usada em novos itens.");
        }

        return categoria;
    }
}

internal static class RoadmapItsmAuditoriaHelper
{
    public static string CriarMetadadosItem(RoadmapItsmItem item, string operacao, string? observacao = null)
        => AuditoriaDiffHelper.CriarMetadadosPadrao(
            origem: "api",
            modulo: "Roadmap ITSM",
            entidade: "RoadmapItsmItem",
            entidadeId: item.Id.ToString(),
            codigo: item.Area,
            nome: item.Objetivo,
            operacao: operacao,
            resultado: "Sucesso",
            observacao: observacao);

    public static string CriarMetadadosChecklist(RoadmapChecklistItem checklist, RoadmapItsmItem item, string operacao, string? observacao = null)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            origem = "api",
            modulo = "Roadmap ITSM",
            entidade = "RoadmapChecklistItem",
            entidadeId = checklist.Id,
            roadmapItemId = item.Id,
            areaRoadmap = item.Area,
            tituloChecklist = checklist.Titulo,
            operacao,
            resultado = "Sucesso",
            observacao
        }) ?? "{}";
}

internal static class RoadmapItsmMaps
{
    public static string? SerializarItemAuditoria(RoadmapItsmItem item)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            item.Area,
            item.Categoria,
            item.Objetivo,
            item.RoadmapCategoriaId,
            item.Status,
            item.Prioridade,
            item.Impacto,
            item.Decisao,
            item.Responsavel,
            item.PrazoAlvo,
            item.Ativo,
            item.StatusImplementacao,
            item.StatusTecnico,
            item.PercentualImplementacao,
            item.PendenciasTecnicas,
            item.PendenciasHomologacao,
            item.EvidenciaImplementacao,
            item.DataConclusaoTecnica,
            item.DataHomologacao,
            item.CriterioAceite,
            item.ProximaAcao
        });

    public static string? SerializarChecklistAuditoria(RoadmapChecklistItem item)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            item.RoadmapItemId,
            item.Titulo,
            item.Descricao,
            item.Grupo,
            item.Ordem,
            item.Concluido,
            item.Obrigatorio,
            item.Ativo
        });

    public static RoadmapItsmResumoResponse MapResumo(RoadmapItsmItem item)
    {
        var (percentual, calculado, totalAtivos, totalConcluidos) = CalcularPercentual(item);
        var nomeCategoria = item.RoadmapCategoria?.Nome ?? item.Categoria;

        return new RoadmapItsmResumoResponse(
            item.Id,
            item.Area,
            nomeCategoria,
            item.Objetivo,
            item.RoadmapCategoriaId,
            item.RoadmapCategoria?.Nome,
            item.RoadmapCategoria?.Cor,
            item.RoadmapCategoria?.Icone,
            item.SituacaoAtual,
            item.Status,
            DescricaoStatus(item.Status),
            item.Prioridade,
            DescricaoPrioridade(item.Prioridade),
            item.Impacto,
            DescricaoImpacto(item.Impacto),
            item.Decisao,
            DescricaoDecisao(item.Decisao),
            item.Responsavel,
            item.PrazoAlvo,
            item.Ordem,
            item.Ativo,
            item.StatusImplementacao,
            DescricaoStatusImplementacao(item.StatusImplementacao),
            item.StatusTecnico,
            DescricaoStatusTecnico(item.StatusTecnico),
            percentual,
            calculado,
            totalAtivos,
            totalConcluidos,
            item.PendenciasTecnicas,
            item.PendenciasHomologacao,
            item.EvidenciaImplementacao,
            item.DataConclusaoTecnica,
            item.DataHomologacao,
            item.CriterioAceite,
            item.ProximaAcao);
    }

    public static RoadmapItsmDetalheResponse MapDetalhe(RoadmapItsmItem item)
    {
        var (percentual, calculado, totalAtivos, totalConcluidos) = CalcularPercentual(item);
        var nomeCategoria = item.RoadmapCategoria?.Nome ?? item.Categoria;

        return new RoadmapItsmDetalheResponse(
            item.Id,
            item.Area,
            nomeCategoria,
            item.Objetivo,
            item.RoadmapCategoriaId,
            item.RoadmapCategoria?.Nome,
            item.RoadmapCategoria?.Cor,
            item.RoadmapCategoria?.Icone,
            item.SituacaoAtual,
            item.AtencaoTecnica,
            item.Status,
            DescricaoStatus(item.Status),
            item.Prioridade,
            DescricaoPrioridade(item.Prioridade),
            item.Impacto,
            DescricaoImpacto(item.Impacto),
            item.Decisao,
            DescricaoDecisao(item.Decisao),
            item.Observacao,
            item.Responsavel,
            item.PrazoAlvo,
            item.Ordem,
            item.Ativo,
            item.CriadoEm,
            item.CriadoPor,
            item.AtualizadoEm,
            item.AtualizadoPor,
            item.StatusImplementacao,
            DescricaoStatusImplementacao(item.StatusImplementacao),
            item.StatusTecnico,
            DescricaoStatusTecnico(item.StatusTecnico),
            percentual,
            calculado,
            totalAtivos,
            totalConcluidos,
            item.PendenciasTecnicas,
            item.PendenciasHomologacao,
            item.EvidenciaImplementacao,
            item.DataConclusaoTecnica,
            item.DataHomologacao,
            item.CriterioAceite,
            item.ProximaAcao);
    }

    public static RoadmapImplementacaoFuturaResponse MapImplementacao(RoadmapImplementacaoFutura item)
        => new(
            item.Id,
            item.RoadmapItemId,
            item.Titulo,
            item.Descricao,
            item.Tipo,
            DescricaoTipoImplementacao(item.Tipo),
            item.Prioridade,
            DescricaoPrioridadeImplementacao(item.Prioridade),
            item.Status,
            DescricaoStatusImplementacaoFutura(item.Status),
            item.Responsavel,
            item.PrazoAlvo,
            item.DataConclusao,
            item.Observacao,
            item.Ativo,
            item.CriadoEm,
            item.CriadoPor,
            item.AtualizadoEm,
            item.AtualizadoPor);

    public static RoadmapCategoriaResponse MapCategoria(RoadmapCategoria item)
        => new(item.Id, item.Nome, item.Descricao, item.Cor, item.Icone, item.Ordem, item.Ativo, item.CriadoEm, item.CriadoPor, item.AtualizadoEm, item.AtualizadoPor);

    public static RoadmapChecklistItemResponse MapChecklistItem(RoadmapChecklistItem item)
        => new(
            item.Id,
            item.RoadmapItemId,
            item.Titulo,
            item.Descricao,
            item.Grupo,
            DescricaoGrupoChecklist(item.Grupo),
            item.Ordem,
            item.Concluido,
            item.Obrigatorio,
            item.Ativo,
            item.CriadoEm,
            item.CriadoPor,
            item.AtualizadoEm,
            item.AtualizadoPor);

    private static (int percentual, bool calculado, int ativos, int concluidos) CalcularPercentual(RoadmapItsmItem item)
    {
        var checklistAtivo = item.ChecklistItens.Where(x => x.Ativo).ToArray();
        if (checklistAtivo.Length == 0)
        {
            return (0, false, 0, 0);
        }

        var concluidos = checklistAtivo.Count(x => x.Concluido);
        var percentual = (int)Math.Round((concluidos * 100.0) / checklistAtivo.Length, MidpointRounding.AwayFromZero);
        return (percentual, true, checklistAtivo.Length, concluidos);
    }

    private static string DescricaoStatus(StatusRoadmapItsm value) => value switch
    {
        StatusRoadmapItsm.Implementado => "Implementado",
        StatusRoadmapItsm.EmValidacao => "Em validação",
        StatusRoadmapItsm.Parcial => "Parcial",
        StatusRoadmapItsm.Pendente => "Pendente",
        StatusRoadmapItsm.PosValidacao => "Pós-validação",
        StatusRoadmapItsm.NaoAplicavel => "Não aplicável",
        _ => value.ToString()
    };

    private static string DescricaoPrioridade(PrioridadeRoadmapItsm value) => value switch
    {
        PrioridadeRoadmapItsm.Alta => "Alta",
        PrioridadeRoadmapItsm.Media => "Média",
        PrioridadeRoadmapItsm.Baixa => "Baixa",
        _ => value.ToString()
    };

    private static string DescricaoImpacto(ImpactoRoadmapItsm value) => value switch
    {
        ImpactoRoadmapItsm.Alto => "Alto",
        ImpactoRoadmapItsm.Medio => "Médio",
        ImpactoRoadmapItsm.Baixo => "Baixo",
        _ => value.ToString()
    };

    private static string DescricaoDecisao(DecisaoRoadmapItsm value) => value switch
    {
        DecisaoRoadmapItsm.DesenvolverAgora => "Desenvolver agora",
        DecisaoRoadmapItsm.PosValidacao => "Pós-validação",
        DecisaoRoadmapItsm.NaoAplicavel => "Não aplicável",
        DecisaoRoadmapItsm.AguardandoAvaliacao => "Aguardando avaliação",
        _ => value.ToString()
    };

    private static string DescricaoStatusImplementacao(StatusImplementacaoRoadmapItsm value) => value switch
    {
        StatusImplementacaoRoadmapItsm.NaoIniciado => "Não iniciado",
        StatusImplementacaoRoadmapItsm.Planejado => "Planejado",
        StatusImplementacaoRoadmapItsm.EmDesenvolvimento => "Em desenvolvimento",
        StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente => "Implementado funcionalmente",
        StatusImplementacaoRoadmapItsm.EmHomologacao => "Em homologação",
        StatusImplementacaoRoadmapItsm.Homologado => "Homologado",
        StatusImplementacaoRoadmapItsm.EmProducao => "Em produção",
        StatusImplementacaoRoadmapItsm.Suspenso => "Suspenso",
        StatusImplementacaoRoadmapItsm.Cancelado => "Cancelado",
        _ => value.ToString()
    };

    private static string DescricaoStatusTecnico(StatusTecnicoRoadmapItsm value) => value switch
    {
        StatusTecnicoRoadmapItsm.NaoAvaliado => "Não avaliado",
        StatusTecnicoRoadmapItsm.Parcial => "Parcial",
        StatusTecnicoRoadmapItsm.Completo => "Completo",
        StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas => "Completo com pendências evolutivas",
        StatusTecnicoRoadmapItsm.RequerValidacao => "Homologação funcional preparada",
        StatusTecnicoRoadmapItsm.RequerCorrecao => "Requer correção",
        StatusTecnicoRoadmapItsm.Bloqueado => "Bloqueado",
        StatusTecnicoRoadmapItsm.ModelagemInicialEmImplementacao => "Modelagem inicial em implementação",
        StatusTecnicoRoadmapItsm.AplicacaoEmChamadosEmImplementacao => "Aplicação em chamados em implementação",
        _ => value.ToString()
    };

    private static string DescricaoTipoImplementacao(TipoRoadmapImplementacaoFutura value) => value switch
    {
        TipoRoadmapImplementacaoFutura.Melhoria => "Melhoria",
        TipoRoadmapImplementacaoFutura.Correcao => "Correção",
        TipoRoadmapImplementacaoFutura.Seguranca => "Segurança",
        TipoRoadmapImplementacaoFutura.Ux => "UX",
        TipoRoadmapImplementacaoFutura.Integracao => "Integração",
        TipoRoadmapImplementacaoFutura.Infraestrutura => "Infraestrutura",
        TipoRoadmapImplementacaoFutura.Homologacao => "Homologação",
        TipoRoadmapImplementacaoFutura.Documentacao => "Documentação",
        TipoRoadmapImplementacaoFutura.Outro => "Outro",
        _ => value.ToString()
    };

    private static string DescricaoPrioridadeImplementacao(PrioridadeRoadmapImplementacaoFutura value) => value switch
    {
        PrioridadeRoadmapImplementacaoFutura.Baixa => "Baixa",
        PrioridadeRoadmapImplementacaoFutura.Media => "Média",
        PrioridadeRoadmapImplementacaoFutura.Alta => "Alta",
        PrioridadeRoadmapImplementacaoFutura.Critica => "Crítica",
        _ => value.ToString()
    };

    private static string DescricaoStatusImplementacaoFutura(StatusRoadmapImplementacaoFutura value) => value switch
    {
        StatusRoadmapImplementacaoFutura.Backlog => "Backlog",
        StatusRoadmapImplementacaoFutura.Planejado => "Planejado",
        StatusRoadmapImplementacaoFutura.EmAnalise => "Em análise",
        StatusRoadmapImplementacaoFutura.EmDesenvolvimento => "Em desenvolvimento",
        StatusRoadmapImplementacaoFutura.EmHomologacao => "Em homologação",
        StatusRoadmapImplementacaoFutura.Concluido => "Concluído",
        StatusRoadmapImplementacaoFutura.Cancelado => "Cancelado",
        StatusRoadmapImplementacaoFutura.Suspenso => "Suspenso",
        _ => value.ToString()
    };

    private static string DescricaoGrupoChecklist(GrupoRoadmapChecklist value) => value switch
    {
        GrupoRoadmapChecklist.Planejamento => "Planejamento",
        GrupoRoadmapChecklist.Desenvolvimento => "Desenvolvimento",
        GrupoRoadmapChecklist.Testes => "Testes",
        GrupoRoadmapChecklist.Documentacao => "Documentação",
        GrupoRoadmapChecklist.Homologacao => "Homologação",
        GrupoRoadmapChecklist.Producao => "Produção",
        GrupoRoadmapChecklist.Seguranca => "Segurança",
        GrupoRoadmapChecklist.Implantacao => "Implantação",
        GrupoRoadmapChecklist.Governanca => "Governança",
        _ => value.ToString()
    };

}




