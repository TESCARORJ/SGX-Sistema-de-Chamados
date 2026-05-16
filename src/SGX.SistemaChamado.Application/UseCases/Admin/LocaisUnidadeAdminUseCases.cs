using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarLocaisUnidadeAdminUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarLocaisUnidadeAdminUseCase
{
    public async Task<PagedResultResponse<LocalUnidadeResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = localUnidadeRepository.Query().AsNoTracking().AsQueryable();
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
                (x.Endereco ?? string.Empty).Contains(texto));
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome);

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<LocalUnidadeResumoResponse>
        {
            Items = items.Select(x => new LocalUnidadeResumoResponse(x.Id, x.Nome, x.Ativo)).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }
}

public sealed class ObterLocalUnidadeAdminUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterLocalUnidadeAdminUseCase
{
    public async Task<LocalUnidadeDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var localUnidade = await localUnidadeRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Local/unidade nao encontrado.");

        return new LocalUnidadeDetalheResponse(localUnidade.Id, localUnidade.Nome, localUnidade.Descricao, localUnidade.Endereco, localUnidade.Ativo);
    }
}

public sealed class CriarLocalUnidadeUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarLocalUnidadeUseCase
{
    public async Task<LocalUnidadeDetalheResponse> ExecutarAsync(CriarLocalUnidadeRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        var duplicado = await localUnidadeRepository.Query().AnyAsync(x => x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe local/unidade com este nome.");
        }

        var localUnidade = new LocalUnidade(nome, request.Descricao, request.Endereco, usuarioAtual.Login);
        await localUnidadeRepository.AddAsync(localUnidade, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LocalUnidadeDetalheResponse(localUnidade.Id, localUnidade.Nome, localUnidade.Descricao, localUnidade.Endereco, localUnidade.Ativo);
    }
}

public sealed class AtualizarLocalUnidadeUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarLocalUnidadeUseCase
{
    public async Task<LocalUnidadeDetalheResponse> ExecutarAsync(Guid id, AtualizarLocalUnidadeRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var localUnidade = await localUnidadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Local/unidade nao encontrado.");

        var nome = request.Nome.Trim();
        var duplicado = await localUnidadeRepository.Query().AnyAsync(x => x.Id != id && x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe local/unidade com este nome.");
        }

        localUnidade.DefinirNome(nome);
        localUnidade.DefinirDescricao(request.Descricao);
        localUnidade.DefinirEndereco(request.Endereco);
        localUnidade.AtualizarAuditoria(usuarioAtual.Login);
        localUnidadeRepository.Update(localUnidade);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LocalUnidadeDetalheResponse(localUnidade.Id, localUnidade.Nome, localUnidade.Descricao, localUnidade.Endereco, localUnidade.Ativo);
    }
}

public sealed class InativarLocalUnidadeUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarLocalUnidadeUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var localUnidade = await localUnidadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Local/unidade nao encontrado.");

        localUnidade.Desativar(usuarioAtual.Login);
        localUnidadeRepository.Update(localUnidade);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(localUnidade.Id, false, "Local/unidade inativado com sucesso.");
    }
}

public sealed class ReativarLocalUnidadeUseCase(
    IRepository<LocalUnidade> localUnidadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarLocalUnidadeUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var localUnidade = await localUnidadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Local/unidade nao encontrado.");

        localUnidade.Ativar(usuarioAtual.Login);
        localUnidadeRepository.Update(localUnidade);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(localUnidade.Id, true, "Local/unidade reativado com sucesso.");
    }
}
