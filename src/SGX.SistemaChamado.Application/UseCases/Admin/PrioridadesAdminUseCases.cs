using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarPrioridadesAdminUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarPrioridadesAdminUseCase
{
    public async Task<PagedResultResponse<PrioridadeChamadoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = prioridadeRepository.Query().AsNoTracking().AsQueryable();
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
        query = (request.OrdenarPor ?? "nivel").Trim().ToLowerInvariant() switch
        {
            "nome" => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            _ => desc ? query.OrderByDescending(x => x.Nivel) : query.OrderBy(x => x.Nivel)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<PrioridadeChamadoResumoResponse>
        {
            Items = items.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    private static PrioridadeChamadoResumoResponse MapResumo(PrioridadeChamado x)
        => new(x.Id, x.Nome, (int)x.Nivel, x.Descricao, x.PrazoPrimeiraRespostaHoras, x.PrazoResolucaoHoras, x.Ativo);
}

public sealed class ObterPrioridadeAdminUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterPrioridadeAdminUseCase
{
    public async Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var prioridade = await prioridadeRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Prioridade nao encontrada.");

        return new PrioridadeChamadoDetalheResponse(
            prioridade.Id,
            prioridade.Nome,
            (int)prioridade.Nivel,
            prioridade.Descricao,
            prioridade.PrazoPrimeiraRespostaHoras,
            prioridade.PrazoResolucaoHoras,
            prioridade.Ativo);
    }
}

public sealed class CriarPrioridadeUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarPrioridadeUseCase
{
    public async Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(CriarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        if (!Enum.IsDefined(typeof(PrioridadeChamadoEnum), request.Nivel))
        {
            throw new InvalidOperationException("Nivel de prioridade invalido.");
        }

        var nome = request.Nome.Trim();
        var duplicado = await prioridadeRepository.Query()
            .AnyAsync(x => x.Nome == nome || (int)x.Nivel == request.Nivel, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe prioridade com mesmo nome ou nivel.");
        }

        var prioridade = new PrioridadeChamado(
            nome,
            (PrioridadeChamadoEnum)request.Nivel,
            request.Descricao,
            request.PrazoPrimeiraRespostaHoras,
            request.PrazoResolucaoHoras,
            usuarioAtual.Login);

        await prioridadeRepository.AddAsync(prioridade, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new PrioridadeChamadoDetalheResponse(prioridade.Id, prioridade.Nome, (int)prioridade.Nivel, prioridade.Descricao, prioridade.PrazoPrimeiraRespostaHoras, prioridade.PrazoResolucaoHoras, prioridade.Ativo);
    }
}

public sealed class AtualizarPrioridadeUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarPrioridadeUseCase
{
    public async Task<PrioridadeChamadoDetalheResponse> ExecutarAsync(Guid id, AtualizarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        if (!Enum.IsDefined(typeof(PrioridadeChamadoEnum), request.Nivel))
        {
            throw new InvalidOperationException("Nivel de prioridade invalido.");
        }

        var prioridade = await prioridadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Prioridade nao encontrada.");

        var nome = request.Nome.Trim();
        var duplicado = await prioridadeRepository.Query()
            .AnyAsync(x => x.Id != id && (x.Nome == nome || (int)x.Nivel == request.Nivel), cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe prioridade com mesmo nome ou nivel.");
        }

        prioridade.DefinirNome(nome);
        prioridade.DefinirNivel((PrioridadeChamadoEnum)request.Nivel, usuarioAtual.Login);
        prioridade.DefinirDescricao(request.Descricao);
        prioridade.DefinirPrazos(request.PrazoPrimeiraRespostaHoras, request.PrazoResolucaoHoras);
        prioridade.AtualizarAuditoria(usuarioAtual.Login);
        prioridadeRepository.Update(prioridade);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PrioridadeChamadoDetalheResponse(prioridade.Id, prioridade.Nome, (int)prioridade.Nivel, prioridade.Descricao, prioridade.PrazoPrimeiraRespostaHoras, prioridade.PrazoResolucaoHoras, prioridade.Ativo);
    }
}

public sealed class InativarPrioridadeUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarPrioridadeUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var prioridade = await prioridadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Prioridade nao encontrada.");

        prioridade.Desativar(usuarioAtual.Login);
        prioridadeRepository.Update(prioridade);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(prioridade.Id, false, "Prioridade inativada com sucesso.");
    }
}

public sealed class ReativarPrioridadeUseCase(
    IRepository<PrioridadeChamado> prioridadeRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarPrioridadeUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var prioridade = await prioridadeRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Prioridade nao encontrada.");

        prioridade.Ativar(usuarioAtual.Login);
        prioridadeRepository.Update(prioridade);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(prioridade.Id, true, "Prioridade reativada com sucesso.");
    }
}
