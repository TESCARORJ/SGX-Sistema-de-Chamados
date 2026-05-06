using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarParametrosSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarParametrosSistemaUseCase
{
    public async Task<PagedResultResponse<ParametroSistemaResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var query = parametroRepository.Query().AsNoTracking().AsQueryable();
        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x => x.Chave.Contains(texto) || (x.Descricao ?? string.Empty).Contains(texto));
        }

        var desc = AdminCadastrosHelpers.DirecaoDesc(request.DirecaoOrdenacao);
        query = desc ? query.OrderByDescending(x => x.Chave) : query.OrderBy(x => x.Chave);

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<ParametroSistemaResumoResponse>
        {
            Items = items.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    private static ParametroSistemaResumoResponse MapResumo(ParametroSistema x)
    {
        var valor = x.Sensivel ? AdminCadastrosHelpers.MascararValorSensivel(x.Valor) : x.Valor;
        return new ParametroSistemaResumoResponse(x.Id, x.Chave, valor, x.Descricao, x.Sensivel, x.Ativo);
    }
}

public sealed class ObterParametroSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterParametroSistemaUseCase
{
    public async Task<ParametroSistemaDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var parametro = await parametroRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Parametro nao encontrado.");

        var valor = parametro.Sensivel ? AdminCadastrosHelpers.MascararValorSensivel(parametro.Valor) : parametro.Valor;
        return new ParametroSistemaDetalheResponse(parametro.Id, parametro.Chave, valor, parametro.Descricao, parametro.Sensivel, parametro.Ativo);
    }
}

public sealed class CriarParametroSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarParametroSistemaUseCase
{
    public async Task<ParametroSistemaDetalheResponse> ExecutarAsync(CriarParametroSistemaRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var chave = request.Chave.Trim();
        var duplicada = await parametroRepository.Query().AnyAsync(x => x.Chave == chave, cancellationToken);
        if (duplicada)
        {
            throw new InvalidOperationException("Ja existe parametro com esta chave.");
        }

        var parametro = new ParametroSistema(chave, request.Valor, request.Descricao, request.Sensivel, usuarioAtual.Login);
        await parametroRepository.AddAsync(parametro, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var valor = parametro.Sensivel ? AdminCadastrosHelpers.MascararValorSensivel(parametro.Valor) : parametro.Valor;
        return new ParametroSistemaDetalheResponse(parametro.Id, parametro.Chave, valor, parametro.Descricao, parametro.Sensivel, parametro.Ativo);
    }
}

public sealed class AtualizarParametroSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarParametroSistemaUseCase
{
    public async Task<ParametroSistemaDetalheResponse> ExecutarAsync(Guid id, AtualizarParametroSistemaRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var parametro = await parametroRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Parametro nao encontrado.");

        var chave = request.Chave.Trim();
        var duplicada = await parametroRepository.Query().AnyAsync(x => x.Id != id && x.Chave == chave, cancellationToken);
        if (duplicada)
        {
            throw new InvalidOperationException("Ja existe parametro com esta chave.");
        }

        parametro.DefinirChave(chave);
        parametro.AtualizarValor(request.Valor, usuarioAtual.Login);
        parametro.DefinirDescricao(request.Descricao, usuarioAtual.Login);
        parametro.DefinirSensivel(request.Sensivel, usuarioAtual.Login);
        parametro.AtualizarAuditoria(usuarioAtual.Login);
        parametroRepository.Update(parametro);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var valor = parametro.Sensivel ? AdminCadastrosHelpers.MascararValorSensivel(parametro.Valor) : parametro.Valor;
        return new ParametroSistemaDetalheResponse(parametro.Id, parametro.Chave, valor, parametro.Descricao, parametro.Sensivel, parametro.Ativo);
    }
}

public sealed class InativarParametroSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IInativarParametroSistemaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var parametro = await parametroRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Parametro nao encontrado.");

        parametro.Desativar(usuarioAtual.Login);
        parametroRepository.Update(parametro);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(parametro.Id, false, "Parametro inativado com sucesso.");
    }
}

public sealed class ReativarParametroSistemaUseCase(
    IRepository<ParametroSistema> parametroRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReativarParametroSistemaUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var parametro = await parametroRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Parametro nao encontrado.");

        parametro.Ativar(usuarioAtual.Login);
        parametroRepository.Update(parametro);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarSituacaoCadastroResponse(parametro.Id, true, "Parametro reativado com sucesso.");
    }
}
