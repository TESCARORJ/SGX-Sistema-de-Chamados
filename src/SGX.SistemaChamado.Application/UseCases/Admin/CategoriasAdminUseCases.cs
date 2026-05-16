using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarCategoriasAdminUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarCategoriasAdminUseCase
{
    public async Task<PagedResultResponse<CategoriaChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = categoriaRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x => x.Nome.Contains(texto) || (x.Descricao ?? string.Empty).Contains(texto));
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "nome").Trim().ToLowerInvariant() switch
        {
            "departamento" => desc ? query.OrderByDescending(x => x.Departamento!.Nome) : query.OrderBy(x => x.Departamento!.Nome),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<CategoriaChamadoResumoResponse>
        {
            Items = items.Select(x => new CategoriaChamadoResumoResponse(x.Id, x.Nome, x.DepartamentoId, x.Departamento?.Nome, x.Ativo)).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }
}

public sealed class ObterCategoriaAdminUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterCategoriaAdminUseCase
{
    public async Task<CategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .Include(x => x.Departamento)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria nao encontrada.");

        return new CategoriaChamadoDetalheResponse(categoria.Id, categoria.Nome, categoria.Descricao, categoria.DepartamentoId, categoria.Departamento?.Nome, categoria.Ativo);
    }
}

public sealed class CriarCategoriaUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarCategoriaUseCase
{
    public async Task<CategoriaChamadoDetalheResponse> ExecutarAsync(CriarCategoriaChamadoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        if (request.DepartamentoId.HasValue)
        {
            var departamentoValido = await departamentoRepository.Query()
                .AnyAsync(x => x.Id == request.DepartamentoId.Value && x.Ativo, cancellationToken);
            if (!departamentoValido)
            {
                throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
            }
        }

        var duplicado = await categoriaRepository.Query()
            .AnyAsync(x => x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe categoria com este nome.");
        }

        var categoria = new CategoriaChamado(nome, request.Descricao, request.DepartamentoId, usuarioAtual.Login);
        await categoriaRepository.AddAsync(categoria, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CategoriaChamadoDetalheResponse(categoria.Id, categoria.Nome, categoria.Descricao, categoria.DepartamentoId, null, categoria.Ativo);
    }
}

public sealed class AtualizarCategoriaUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarCategoriaUseCase
{
    public async Task<CategoriaChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarCategoriaChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await categoriaRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria nao encontrada.");

        var nome = request.Nome.Trim();
        if (request.DepartamentoId.HasValue)
        {
            var departamentoValido = await departamentoRepository.Query()
                .AnyAsync(x => x.Id == request.DepartamentoId.Value && x.Ativo, cancellationToken);
            if (!departamentoValido)
            {
                throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
            }
        }

        var duplicado = await categoriaRepository.Query()
            .AnyAsync(x => x.Id != id && x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe categoria com este nome.");
        }

        categoria.DefinirNome(nome);
        categoria.DefinirDescricao(request.Descricao);
        categoria.DefinirDepartamento(request.DepartamentoId, usuarioAtual.Login);
        categoria.AtualizarAuditoria(usuarioAtual.Login);
        categoriaRepository.Update(categoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoriaChamadoDetalheResponse(categoria.Id, categoria.Nome, categoria.Descricao, categoria.DepartamentoId, null, categoria.Ativo);
    }
}

public sealed class InativarCategoriaUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarCategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await categoriaRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria nao encontrada.");

        categoria.Desativar(usuarioAtual.Login);
        categoriaRepository.Update(categoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(categoria.Id, false, "Categoria inativada com sucesso.");
    }
}

public sealed class ReativarCategoriaUseCase(
    IRepository<CategoriaChamado> categoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarCategoriaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var categoria = await categoriaRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria nao encontrada.");

        categoria.Ativar(usuarioAtual.Login);
        categoriaRepository.Update(categoria);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(categoria.Id, true, "Categoria reativada com sucesso.");
    }
}
