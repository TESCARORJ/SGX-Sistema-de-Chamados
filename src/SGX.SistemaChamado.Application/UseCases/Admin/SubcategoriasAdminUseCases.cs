using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarSubcategoriasAdminUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarSubcategoriasAdminUseCase
{
    public async Task<PagedResultResponse<SubcategoriaChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = subcategoriaRepository.Query()
            .AsNoTracking()
            .Include(x => x.CategoriaChamado)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x =>
                x.Nome.Contains(texto) ||
                (x.Descricao ?? string.Empty).Contains(texto) ||
                x.CategoriaChamado.Nome.Contains(texto));
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "nome").Trim().ToLowerInvariant() switch
        {
            "categoria" => desc ? query.OrderByDescending(x => x.CategoriaChamado.Nome) : query.OrderBy(x => x.CategoriaChamado.Nome),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<SubcategoriaChamadoResumoResponse>
        {
            Items = items.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    private static SubcategoriaChamadoResumoResponse MapResumo(SubcategoriaChamado entidade)
        => new(entidade.Id, entidade.CategoriaChamadoId, entidade.CategoriaChamado.Nome, entidade.Nome, entidade.Ativo);
}

public sealed class ListarSubcategoriasPorCategoriaUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarSubcategoriasPorCategoriaUseCase
{
    public async Task<IReadOnlyCollection<SubcategoriaChamadoResumoResponse>> ExecutarAsync(Guid categoriaId, bool? ativo = true, CancellationToken cancellationToken = default)
    {
        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("CategoriaId invalido.", nameof(categoriaId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = subcategoriaRepository.Query()
            .AsNoTracking()
            .Include(x => x.CategoriaChamado)
            .Where(x => x.CategoriaChamadoId == categoriaId);

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        var items = await query
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return items.Select(x => new SubcategoriaChamadoResumoResponse(
            x.Id,
            x.CategoriaChamadoId,
            x.CategoriaChamado.Nome,
            x.Nome,
            x.Ativo)).ToArray();
    }
}

public sealed class ObterSubcategoriaAdminUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterSubcategoriaAdminUseCase
{
    public async Task<SubcategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var subcategoria = await subcategoriaRepository.Query()
            .AsNoTracking()
            .Include(x => x.CategoriaChamado)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Subcategoria nao encontrada.");

        return new SubcategoriaChamadoDetalheResponse(
            subcategoria.Id,
            subcategoria.CategoriaChamadoId,
            subcategoria.CategoriaChamado.Nome,
            subcategoria.Nome,
            subcategoria.Descricao,
            subcategoria.Ativo);
    }
}

public sealed class CriarSubcategoriaUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarSubcategoriaUseCase
{
    public async Task<SubcategoriaChamadoDetalheResponse> ExecutarAsync(CriarSubcategoriaChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CategoriaChamadoId == Guid.Empty)
        {
            throw new ArgumentException("CategoriaChamadoId invalido.", nameof(request.CategoriaChamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CategoriaChamadoId, cancellationToken)
            ?? throw new InvalidOperationException("Categoria informada nao encontrada.");

        var nome = request.Nome.Trim();
        var duplicado = await subcategoriaRepository.Query()
            .AnyAsync(x => x.CategoriaChamadoId == request.CategoriaChamadoId && x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe subcategoria com este nome para a categoria informada.");
        }

        var subcategoria = new SubcategoriaChamado(request.CategoriaChamadoId, nome, request.Descricao, usuarioAtual.Login);
        await subcategoriaRepository.AddAsync(subcategoria, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubcategoriaChamadoDetalheResponse(
            subcategoria.Id,
            subcategoria.CategoriaChamadoId,
            categoria.Nome,
            subcategoria.Nome,
            subcategoria.Descricao,
            subcategoria.Ativo);
    }
}

public sealed class AtualizarSubcategoriaUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarSubcategoriaUseCase
{
    public async Task<SubcategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarSubcategoriaChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        if (request.CategoriaChamadoId == Guid.Empty)
        {
            throw new ArgumentException("CategoriaChamadoId invalido.", nameof(request.CategoriaChamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CategoriaChamadoId, cancellationToken)
            ?? throw new InvalidOperationException("Categoria informada nao encontrada.");

        var subcategoria = await subcategoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Subcategoria nao encontrada.");

        var nome = request.Nome.Trim();
        var duplicado = await subcategoriaRepository.Query()
            .AnyAsync(x =>
                x.Id != id &&
                x.CategoriaChamadoId == request.CategoriaChamadoId &&
                x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe subcategoria com este nome para a categoria informada.");
        }

        subcategoria.DefinirCategoriaChamado(request.CategoriaChamadoId);
        subcategoria.DefinirNome(nome);
        subcategoria.DefinirDescricao(request.Descricao);
        subcategoria.AtualizarAuditoria(usuarioAtual.Login);
        subcategoriaRepository.Update(subcategoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubcategoriaChamadoDetalheResponse(
            subcategoria.Id,
            subcategoria.CategoriaChamadoId,
            categoria.Nome,
            subcategoria.Nome,
            subcategoria.Descricao,
            subcategoria.Ativo);
    }
}

public sealed class InativarSubcategoriaUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarSubcategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var subcategoria = await subcategoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Subcategoria nao encontrada.");

        subcategoria.Desativar(usuarioAtual.Login);
        subcategoriaRepository.Update(subcategoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(subcategoria.Id, false, "Subcategoria inativada com sucesso.");
    }
}

public sealed class ReativarSubcategoriaUseCase(
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarSubcategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var subcategoria = await subcategoriaRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Subcategoria nao encontrada.");

        subcategoria.Ativar(usuarioAtual.Login);
        subcategoriaRepository.Update(subcategoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(subcategoria.Id, true, "Subcategoria reativada com sucesso.");
    }
}
