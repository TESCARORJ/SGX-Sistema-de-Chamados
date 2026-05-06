using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ObterAdminContextoUseCase(
    IRepository<Departamento> departamentoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterAdminContextoUseCase
{
    public async Task<AdminContextoResponse> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var departamentos = await departamentoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new DepartamentoAdminResponse(x.Id, x.Nome, x.Sigla))
            .ToListAsync(cancellationToken);

        var categorias = await categoriaRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nome)
            .Select(x => new CategoriaAdminResponse(x.Id, x.Nome, x.DepartamentoId))
            .ToListAsync(cancellationToken);

        var prioridades = await prioridadeRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Nivel)
            .Select(x => new PrioridadeAdminResponse(x.Id, x.Nome, (int)x.Nivel))
            .ToListAsync(cancellationToken);

        var status = await statusRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Codigo)
            .Select(x => new StatusAdminResponse(x.Id, x.Nome, (int)x.Codigo))
            .ToListAsync(cancellationToken);

        var atendentes = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .Where(x => x.Ativo && x.Situacao == SituacaoUsuario.Ativo)
            .Where(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Atendente || p.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador))
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        var atendentesResponse = atendentes
            .Select(x => new AtendenteResumoResponse(
                x.Id,
                x.Nome,
                x.Email,
                x.UsuarioPerfis.Select(p => p.PerfilAcesso.Nome).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()))
            .ToArray();

        var permissoes = usuario.Perfis
            .SelectMany(perfil => perfil switch
            {
                "Administrador" => ["admin.acessar", "chamados.visualizar.todos", "chamados.atender", "cadastros.gerenciar", "usuarios.gerenciar"],
                "Atendente" => ["admin.acessar", "chamados.visualizar.todos", "chamados.atender"],
                _ => Array.Empty<string>()
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new AdminContextoResponse
        {
            Usuario = new AdminUsuarioContextoResponse(
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Login,
                usuario.Perfis,
                permissoes),
            Departamentos = departamentos,
            Categorias = categorias,
            Prioridades = prioridades,
            Status = status,
            Atendentes = atendentesResponse
        };
    }
}
