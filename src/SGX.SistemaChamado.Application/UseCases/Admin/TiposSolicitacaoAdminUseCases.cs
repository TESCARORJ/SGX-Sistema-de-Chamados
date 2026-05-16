using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarTiposSolicitacaoAdminUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarTiposSolicitacaoAdminUseCase
{
    public async Task<PagedResultResponse<TipoSolicitacaoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = tipoSolicitacaoRepository.Query().AsNoTracking().AsQueryable();
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
        query = desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome);

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<TipoSolicitacaoResumoResponse>
        {
            Items = items.Select(x => new TipoSolicitacaoResumoResponse(x.Id, x.Nome, x.Ativo)).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }
}

public sealed class ObterTipoSolicitacaoAdminUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterTipoSolicitacaoAdminUseCase
{
    public async Task<TipoSolicitacaoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var tipoSolicitacao = await tipoSolicitacaoRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de solicitacao nao encontrado.");

        return new TipoSolicitacaoDetalheResponse(tipoSolicitacao.Id, tipoSolicitacao.Nome, tipoSolicitacao.Descricao, tipoSolicitacao.Ativo);
    }
}

public sealed class CriarTipoSolicitacaoUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarTipoSolicitacaoUseCase
{
    public async Task<TipoSolicitacaoDetalheResponse> ExecutarAsync(CriarTipoSolicitacaoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        var duplicado = await tipoSolicitacaoRepository.Query().AnyAsync(x => x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe tipo de solicitacao com este nome.");
        }

        var tipoSolicitacao = new TipoSolicitacao(nome, request.Descricao, usuarioAtual.Login);
        await tipoSolicitacaoRepository.AddAsync(tipoSolicitacao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TipoSolicitacaoDetalheResponse(tipoSolicitacao.Id, tipoSolicitacao.Nome, tipoSolicitacao.Descricao, tipoSolicitacao.Ativo);
    }
}

public sealed class AtualizarTipoSolicitacaoUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarTipoSolicitacaoUseCase
{
    public async Task<TipoSolicitacaoDetalheResponse> ExecutarAsync(Guid id, AtualizarTipoSolicitacaoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var tipoSolicitacao = await tipoSolicitacaoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de solicitacao nao encontrado.");

        var nome = request.Nome.Trim();
        var duplicado = await tipoSolicitacaoRepository.Query().AnyAsync(x => x.Id != id && x.Nome == nome, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe tipo de solicitacao com este nome.");
        }

        tipoSolicitacao.DefinirNome(nome);
        tipoSolicitacao.DefinirDescricao(request.Descricao);
        tipoSolicitacao.AtualizarAuditoria(usuarioAtual.Login);
        tipoSolicitacaoRepository.Update(tipoSolicitacao);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TipoSolicitacaoDetalheResponse(tipoSolicitacao.Id, tipoSolicitacao.Nome, tipoSolicitacao.Descricao, tipoSolicitacao.Ativo);
    }
}

public sealed class InativarTipoSolicitacaoUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarTipoSolicitacaoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var tipoSolicitacao = await tipoSolicitacaoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de solicitacao nao encontrado.");

        tipoSolicitacao.Desativar(usuarioAtual.Login);
        tipoSolicitacaoRepository.Update(tipoSolicitacao);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(tipoSolicitacao.Id, false, "Tipo de solicitacao inativado com sucesso.");
    }
}

public sealed class ReativarTipoSolicitacaoUseCase(
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarTipoSolicitacaoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var tipoSolicitacao = await tipoSolicitacaoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de solicitacao nao encontrado.");

        tipoSolicitacao.Ativar(usuarioAtual.Login);
        tipoSolicitacaoRepository.Update(tipoSolicitacao);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(tipoSolicitacao.Id, true, "Tipo de solicitacao reativado com sucesso.");
    }
}
