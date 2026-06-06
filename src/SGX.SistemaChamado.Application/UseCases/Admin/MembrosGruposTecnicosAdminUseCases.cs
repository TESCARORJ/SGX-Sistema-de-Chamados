using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarMembrosGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarMembrosGrupoTecnicoAdminUseCase
{
    public async Task<IReadOnlyCollection<MembroGrupoTecnicoResponse>> ExecutarAsync(Guid grupoTecnicoId, ListarMembrosGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        GarantirIdValido(grupoTecnicoId, nameof(grupoTecnicoId));

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var grupoExiste = await grupoTecnicoRepository.Query().AsNoTracking().AnyAsync(x => x.Id == grupoTecnicoId, cancellationToken);
        if (!grupoExiste)
        {
            throw new KeyNotFoundException("Grupo tecnico nao encontrado.");
        }

        var query = membroGrupoTecnicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.GrupoTecnicoId == grupoTecnicoId);

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        var membros = await query
            .OrderBy(x => x.Usuario.Nome)
            .ThenBy(x => x.Usuario.Email)
            .ToListAsync(cancellationToken);

        return membros.Select(MapMembro).ToArray();
    }

    internal static MembroGrupoTecnicoResponse MapMembro(MembroGrupoTecnico membro)
        => new(
            membro.Id,
            membro.GrupoTecnicoId,
            membro.UsuarioId,
            membro.Usuario.Nome,
            membro.Usuario.Email,
            membro.Ativo,
            membro.CriadoEm,
            membro.AtualizadoEm);

    internal static void GarantirIdValido(Guid id, string parametro)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", parametro);
        }
    }
}

public sealed class AdicionarMembroGrupoTecnicoAdminUseCase(
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdicionarMembroGrupoTecnicoAdminUseCase
{
    public async Task<MembroGrupoTecnicoResponse> ExecutarAsync(Guid grupoTecnicoId, AdicionarMembroGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        ListarMembrosGrupoTecnicoAdminUseCase.GarantirIdValido(grupoTecnicoId, nameof(grupoTecnicoId));
        ListarMembrosGrupoTecnicoAdminUseCase.GarantirIdValido(request.UsuarioId, nameof(request.UsuarioId));

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var grupo = await grupoTecnicoRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == grupoTecnicoId, cancellationToken)
            ?? throw new KeyNotFoundException("Grupo tecnico nao encontrado.");

        if (!grupo.Ativo)
        {
            throw new InvalidOperationException("Grupo tecnico inativo nao pode receber membros.");
        }

        var usuario = await usuarioRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        var membroExistente = await membroGrupoTecnicoRepository.Query()
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.GrupoTecnicoId == grupoTecnicoId && x.UsuarioId == request.UsuarioId, cancellationToken);

        if (membroExistente is not null)
        {
            if (membroExistente.Ativo)
            {
                throw new InvalidOperationException("Usuario ja e membro ativo deste grupo tecnico.");
            }

            membroExistente.Reativar(usuarioAtual.Login);
            membroGrupoTecnicoRepository.Update(membroExistente);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return ListarMembrosGrupoTecnicoAdminUseCase.MapMembro(membroExistente);
        }

        var membro = new MembroGrupoTecnico(grupoTecnicoId, request.UsuarioId, usuarioAtual.Login);
        await membroGrupoTecnicoRepository.AddAsync(membro, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MembroGrupoTecnicoResponse(
            membro.Id,
            membro.GrupoTecnicoId,
            membro.UsuarioId,
            usuario.Nome,
            usuario.Email,
            membro.Ativo,
            membro.CriadoEm,
            membro.AtualizadoEm);
    }
}

public sealed class AtualizarStatusMembroGrupoTecnicoAdminUseCase(
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtualizarStatusMembroGrupoTecnicoAdminUseCase
{
    public async Task<MembroGrupoTecnicoResponse> ExecutarAsync(Guid membroId, AlterarStatusMembroGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        ListarMembrosGrupoTecnicoAdminUseCase.GarantirIdValido(membroId, nameof(membroId));

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var membro = await membroGrupoTecnicoRepository.Query()
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Id == membroId, cancellationToken)
            ?? throw new KeyNotFoundException("Membro de grupo tecnico nao encontrado.");

        if (request.Ativo)
        {
            membro.Reativar(usuarioAtual.Login);
        }
        else
        {
            membro.Inativar(usuarioAtual.Login);
        }

        membroGrupoTecnicoRepository.Update(membro);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ListarMembrosGrupoTecnicoAdminUseCase.MapMembro(membro);
    }
}

public sealed class ListarGruposTecnicosDoUsuarioAdminUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarGruposTecnicosDoUsuarioAdminUseCase
{
    public async Task<IReadOnlyCollection<GrupoTecnicoDoUsuarioResponse>> ExecutarAsync(Guid usuarioId, bool? ativo = true, CancellationToken cancellationToken = default)
    {
        ListarMembrosGrupoTecnicoAdminUseCase.GarantirIdValido(usuarioId, nameof(usuarioId));

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var usuarioExiste = await usuarioRepository.Query().AsNoTracking().AnyAsync(x => x.Id == usuarioId, cancellationToken);
        if (!usuarioExiste)
        {
            throw new KeyNotFoundException("Usuario nao encontrado.");
        }

        var query = membroGrupoTecnicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.GrupoTecnico)
            .Where(x => x.UsuarioId == usuarioId);

        if (ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == ativo.Value);
        }

        var membros = await query
            .OrderBy(x => x.GrupoTecnico.Nome)
            .ToListAsync(cancellationToken);

        return membros
            .Select(x => new GrupoTecnicoDoUsuarioResponse(x.GrupoTecnicoId, x.GrupoTecnico.Nome, x.GrupoTecnico.Ativo))
            .ToArray();
    }
}
