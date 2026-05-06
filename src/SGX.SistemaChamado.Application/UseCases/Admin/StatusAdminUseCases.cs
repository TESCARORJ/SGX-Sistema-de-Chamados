using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarStatusAdminUseCase(
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarStatusAdminUseCase
{
    public async Task<PagedResultResponse<StatusChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = statusRepository.Query().AsNoTracking().AsQueryable();
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
        query = (request.OrdenarPor ?? "codigo").Trim().ToLowerInvariant() switch
        {
            "nome" => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            _ => desc ? query.OrderByDescending(x => x.Codigo) : query.OrderBy(x => x.Codigo)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<StatusChamadoResumoResponse>
        {
            Items = items.Select(x => new StatusChamadoResumoResponse(x.Id, x.Nome, (int)x.Codigo, x.Descricao, x.EhStatusFinal, x.PausaSla, x.Ativo)).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }
}

public sealed class ObterStatusAdminUseCase(
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterStatusAdminUseCase
{
    public async Task<StatusChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var status = await statusRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Status nao encontrado.");

        return new StatusChamadoDetalheResponse(status.Id, status.Nome, (int)status.Codigo, status.Descricao, status.EhStatusFinal, status.PausaSla, status.Ativo);
    }
}

public sealed class CriarStatusUseCase(
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarStatusUseCase
{
    public async Task<StatusChamadoDetalheResponse> ExecutarAsync(CriarStatusChamadoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        if (!Enum.IsDefined(typeof(StatusChamadoEnum), request.Codigo))
        {
            throw new InvalidOperationException("Codigo de status invalido.");
        }

        var nome = request.Nome.Trim();
        var duplicado = await statusRepository.Query().AnyAsync(x => x.Codigo == (StatusChamadoEnum)request.Codigo, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe status com este codigo.");
        }

        var status = new StatusChamado(nome, (StatusChamadoEnum)request.Codigo, request.Descricao, request.EhStatusFinal, request.PausaSla, usuarioAtual.Login);
        await statusRepository.AddAsync(status, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StatusChamadoDetalheResponse(status.Id, status.Nome, (int)status.Codigo, status.Descricao, status.EhStatusFinal, status.PausaSla, status.Ativo);
    }
}

public sealed class AtualizarStatusUseCase(
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarStatusUseCase
{
    public async Task<StatusChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarStatusChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        if (!Enum.IsDefined(typeof(StatusChamadoEnum), request.Codigo))
        {
            throw new InvalidOperationException("Codigo de status invalido.");
        }

        var status = await statusRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Status nao encontrado.");

        var duplicado = await statusRepository.Query()
            .AnyAsync(x => x.Id != id && x.Codigo == (StatusChamadoEnum)request.Codigo, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe status com este codigo.");
        }

        status.DefinirNome(request.Nome);
        status.DefinirCodigo((StatusChamadoEnum)request.Codigo, usuarioAtual.Login);
        status.DefinirDescricao(request.Descricao);
        status.DefinirRegras(request.EhStatusFinal, request.PausaSla, usuarioAtual.Login);
        status.AtualizarAuditoria(usuarioAtual.Login);
        statusRepository.Update(status);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StatusChamadoDetalheResponse(status.Id, status.Nome, (int)status.Codigo, status.Descricao, status.EhStatusFinal, status.PausaSla, status.Ativo);
    }
}

public sealed class InativarStatusUseCase(
    IRepository<StatusChamado> statusRepository,
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarStatusUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var status = await statusRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Status nao encontrado.");

        if (status.Codigo == StatusChamadoEnum.Aberto)
        {
            throw new InvalidOperationException("Nao e permitido inativar o status inicial Aberto.");
        }

        var totalAtivos = await statusRepository.Query().CountAsync(x => x.Ativo, cancellationToken);
        if (status.Ativo && totalAtivos <= 1)
        {
            throw new InvalidOperationException("Nao e permitido inativar todos os status.");
        }

        var emUso = await chamadoRepository.Query().AnyAsync(x => x.StatusId == status.Id, cancellationToken);
        if (emUso)
        {
            throw new InvalidOperationException("Nao e permitido inativar status em uso por chamados.");
        }

        status.Desativar(usuarioAtual.Login);
        statusRepository.Update(status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(status.Id, false, "Status inativado com sucesso.");
    }
}

public sealed class ReativarStatusUseCase(
    IRepository<StatusChamado> statusRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarStatusUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var status = await statusRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Status nao encontrado.");

        status.Ativar(usuarioAtual.Login);
        statusRepository.Update(status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(status.Id, true, "Status reativado com sucesso.");
    }
}
