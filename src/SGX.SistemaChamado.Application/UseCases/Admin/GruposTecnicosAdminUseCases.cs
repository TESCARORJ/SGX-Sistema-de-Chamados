using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarGruposTecnicosAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarGruposTecnicosAdminUseCase
{
    public async Task<PagedResultResponse<GrupoTecnicoResumoResponse>> ExecutarAsync(ListarGruposTecnicosRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = grupoTecnicoRepository.Query().AsNoTracking().AsQueryable();
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
            "ativo" => desc ? query.OrderByDescending(x => x.Ativo).ThenBy(x => x.Nome) : query.OrderBy(x => x.Ativo).ThenBy(x => x.Nome),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request.Pagina, request.TamanhoPagina);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<GrupoTecnicoResumoResponse>
        {
            Items = items.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    internal static GrupoTecnicoResumoResponse MapResumo(GrupoTecnico grupo)
        => new(grupo.Id, grupo.Nome, grupo.Ativo);
}

public sealed class ObterGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterGrupoTecnicoAdminUseCase
{
    public async Task<GrupoTecnicoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        GarantirIdValido(id);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var grupo = await grupoTecnicoRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Grupo tecnico nao encontrado.");

        return MapDetalhe(grupo);
    }

    internal static GrupoTecnicoResponse MapDetalhe(GrupoTecnico grupo)
        => new(grupo.Id, grupo.Nome, grupo.Descricao, grupo.Ativo, grupo.CriadoEm, grupo.AtualizadoEm);

    internal static void GarantirIdValido(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }
    }
}

public sealed class CriarGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ICriarGrupoTecnicoAdminUseCase
{
    public async Task<GrupoTecnicoResponse> ExecutarAsync(CriarGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = NormalizarNome(request.Nome);
        await GarantirNomeUnicoAsync(grupoTecnicoRepository, nome, null, cancellationToken);

        var grupo = new GrupoTecnico(nome, request.Descricao, usuarioAtual.Login);
        await grupoTecnicoRepository.AddAsync(grupo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ObterGrupoTecnicoAdminUseCase.MapDetalhe(grupo);
    }

    internal static string NormalizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do grupo tecnico e obrigatorio.", nameof(nome));
        }

        return nome.Trim();
    }

    internal static async Task GarantirNomeUnicoAsync(
        IRepository<GrupoTecnico> grupoTecnicoRepository,
        string nome,
        Guid? ignorarId,
        CancellationToken cancellationToken)
    {
        var duplicado = await grupoTecnicoRepository.Query()
            .AnyAsync(x => x.Nome == nome && (!ignorarId.HasValue || x.Id != ignorarId.Value), cancellationToken);

        if (duplicado)
        {
            throw new InvalidOperationException("Ja existe grupo tecnico com este nome.");
        }
    }
}

public sealed class AtualizarGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarGrupoTecnicoAdminUseCase
{
    public async Task<GrupoTecnicoResponse> ExecutarAsync(Guid id, AtualizarGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        ObterGrupoTecnicoAdminUseCase.GarantirIdValido(id);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var grupo = await grupoTecnicoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Grupo tecnico nao encontrado.");

        var nome = CriarGrupoTecnicoAdminUseCase.NormalizarNome(request.Nome);
        await CriarGrupoTecnicoAdminUseCase.GarantirNomeUnicoAsync(grupoTecnicoRepository, nome, id, cancellationToken);

        grupo.AlterarDados(nome, request.Descricao, usuarioAtual.Login);
        grupoTecnicoRepository.Update(grupo);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ObterGrupoTecnicoAdminUseCase.MapDetalhe(grupo);
    }
}

public sealed class AtualizarStatusGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarStatusGrupoTecnicoAdminUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, AlterarStatusGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        ObterGrupoTecnicoAdminUseCase.GarantirIdValido(id);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var grupo = await grupoTecnicoRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Grupo tecnico nao encontrado.");

        if (request.Ativo)
        {
            grupo.Reativar(usuarioAtual.Login);
        }
        else
        {
            grupo.Inativar(usuarioAtual.Login);
        }

        grupoTecnicoRepository.Update(grupo);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var mensagem = request.Ativo
            ? "Grupo tecnico reativado com sucesso."
            : "Grupo tecnico inativado com sucesso.";

        return new AlterarSituacaoCadastroResponse(grupo.Id, grupo.Ativo, mensagem);
    }
}
