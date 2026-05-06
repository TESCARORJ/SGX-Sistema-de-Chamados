using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarDepartamentosAdminUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarDepartamentosAdminUseCase
{
    public async Task<PagedResultResponse<DepartamentoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = departamentoRepository.Query().AsNoTracking().AsQueryable();
        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x => x.Nome.Contains(texto) || x.Sigla.Contains(texto));
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = (request.OrdenarPor ?? "nome").Trim().ToLowerInvariant() switch
        {
            "sigla" => desc ? query.OrderByDescending(x => x.Sigla) : query.OrderBy(x => x.Sigla),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<DepartamentoResumoResponse>
        {
            Items = items.Select(x => new DepartamentoResumoResponse(x.Id, x.Nome, x.Sigla, x.Ativo)).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }
}

public sealed class ObterDepartamentoAdminUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterDepartamentoAdminUseCase
{
    public async Task<DepartamentoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var departamento = await departamentoRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento nao encontrado.");

        return new DepartamentoDetalheResponse(departamento.Id, departamento.Nome, departamento.Sigla, departamento.Descricao, departamento.Ativo);
    }
}

public sealed class CriarDepartamentoUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarDepartamentoUseCase
{
    public async Task<DepartamentoDetalheResponse> ExecutarAsync(CriarDepartamentoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        var sigla = request.Sigla.Trim().ToUpperInvariant();

        var duplicado = await departamentoRepository.Query().AnyAsync(x => x.Nome == nome || x.Sigla == sigla, cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe departamento com mesmo nome ou sigla.");
        }

        var departamento = new Departamento(nome, sigla, request.Descricao, usuarioAtual.Login);
        await departamentoRepository.AddAsync(departamento, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new DepartamentoDetalheResponse(departamento.Id, departamento.Nome, departamento.Sigla, departamento.Descricao, departamento.Ativo);
    }
}

public sealed class AtualizarDepartamentoUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarDepartamentoUseCase
{
    public async Task<DepartamentoDetalheResponse> ExecutarAsync(Guid id, AtualizarDepartamentoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var departamento = await departamentoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento nao encontrado.");

        var nome = request.Nome.Trim();
        var sigla = request.Sigla.Trim().ToUpperInvariant();
        var duplicado = await departamentoRepository.Query()
            .AnyAsync(x => x.Id != id && (x.Nome == nome || x.Sigla == sigla), cancellationToken);
        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe departamento com mesmo nome ou sigla.");
        }

        departamento.DefinirNome(nome);
        departamento.DefinirSigla(sigla);
        departamento.DefinirDescricao(request.Descricao);
        departamento.AtualizarAuditoria(usuarioAtual.Login);
        departamentoRepository.Update(departamento);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new DepartamentoDetalheResponse(departamento.Id, departamento.Nome, departamento.Sigla, departamento.Descricao, departamento.Ativo);
    }
}

public sealed class InativarDepartamentoUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarDepartamentoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var departamento = await departamentoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento nao encontrado.");

        departamento.Desativar(usuarioAtual.Login);
        departamentoRepository.Update(departamento);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(departamento.Id, false, "Departamento inativado com sucesso.");
    }
}

public sealed class ReativarDepartamentoUseCase(
    IRepository<Departamento> departamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarDepartamentoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var departamento = await departamentoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento nao encontrado.");

        departamento.Ativar(usuarioAtual.Login);
        departamentoRepository.Update(departamento);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(departamento.Id, true, "Departamento reativado com sucesso.");
    }
}
