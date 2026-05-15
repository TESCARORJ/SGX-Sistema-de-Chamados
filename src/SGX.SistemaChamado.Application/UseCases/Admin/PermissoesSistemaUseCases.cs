using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using System.Text.Json;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarPermissoesSistemaUseCase(
    IRepository<PermissaoSistema> permissaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarPermissoesSistemaUseCase
{
    public async Task<IReadOnlyCollection<PermissaoSistemaResponse>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var permissoes = await permissaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Acao)
            .ToListAsync(cancellationToken);

        return permissoes.Select(MapPermissao).ToArray();
    }

    internal static PermissaoSistemaResponse MapPermissao(PermissaoSistema permissao)
        => new(
            permissao.Id,
            permissao.Codigo,
            permissao.Codigo,
            permissao.Descricao,
            permissao.Modulo,
            permissao.Acao,
            permissao.Ativo);
}

public sealed class ObterPermissoesPerfilUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IRepository<PermissaoSistema> permissaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterPermissoesPerfilUseCase
{
    public async Task<PerfilPermissoesResponse> ExecutarAsync(Guid perfilId, CancellationToken cancellationToken = default)
    {
        if (perfilId == Guid.Empty)
        {
            throw new ArgumentException("Id do perfil invalido.", nameof(perfilId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var perfil = await CarregarPerfilComPermissoesAsync(perfilId, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");

        var permissoesDisponiveis = await permissaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Acao)
            .ToListAsync(cancellationToken);

        var permissoesVinculadas = perfil.PerfilPermissoes
            .Where(x => x.PermissaoSistema.Ativo)
            .Select(x => x.PermissaoSistema)
            .DistinctBy(x => x.Codigo)
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Acao)
            .ToArray();

        return new PerfilPermissoesResponse(
            perfil.Id,
            perfil.Nome,
            (int)perfil.TipoPerfil,
            permissoesDisponiveis.Select(ListarPermissoesSistemaUseCase.MapPermissao).ToArray(),
            permissoesVinculadas.Select(ListarPermissoesSistemaUseCase.MapPermissao).ToArray());
    }

    private Task<PerfilAcesso?> CarregarPerfilComPermissoesAsync(Guid perfilId, CancellationToken cancellationToken)
    {
        return perfilRepository.Query()
            .AsNoTracking()
            .Include(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema)
            .FirstOrDefaultAsync(x => x.Id == perfilId, cancellationToken);
    }
}

public sealed class AtualizarPermissoesPerfilUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IRepository<PermissaoSistema> permissaoRepository,
    IRepository<PerfilAcessoPermissao> perfilPermissaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarPermissoesPerfilUseCase
{
    public async Task<PerfilPermissoesResponse> ExecutarAsync(Guid perfilId, AtualizarPermissoesPerfilRequest request, CancellationToken cancellationToken = default)
    {
        if (perfilId == Guid.Empty)
        {
            throw new ArgumentException("Id do perfil invalido.", nameof(perfilId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var perfil = await perfilRepository.Query()
            .Include(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema)
            .FirstOrDefaultAsync(x => x.Id == perfilId, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");
        var dadosAntes = JsonSerializer.Serialize(new
        {
            PerfilId = perfil.Id,
            PerfilNome = perfil.Nome,
            Permissoes = perfil.PerfilPermissoes
                .Where(x => x.PermissaoSistema.Ativo)
                .Select(x => x.PermissaoSistema.Codigo)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToArray()
        });

        var codigosDesejados = (request.CodigosPermissoes ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissoesDesejadas = await permissaoRepository.Query()
            .Where(x => x.Ativo && codigosDesejados.Contains(x.Codigo))
            .ToListAsync(cancellationToken);

        if (permissoesDesejadas.Count != codigosDesejados.Length)
        {
            var codigosEncontrados = permissoesDesejadas.Select(x => x.Codigo).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var codigosInvalidos = codigosDesejados.Where(x => !codigosEncontrados.Contains(x)).ToArray();
            throw new InvalidOperationException($"Permissoes inexistentes ou inativas: {string.Join(", ", codigosInvalidos)}.");
        }

        var codigosDesejadosSet = codigosDesejados.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var vinculosAtuais = perfil.PerfilPermissoes.ToArray();

        foreach (var vinculo in vinculosAtuais)
        {
            if (!codigosDesejadosSet.Contains(vinculo.PermissaoSistema.Codigo))
            {
                perfilPermissaoRepository.Remove(vinculo);
            }
        }

        var permissaoIdsAtuais = vinculosAtuais
            .Select(x => x.PermissaoSistemaId)
            .ToHashSet();

        foreach (var permissao in permissoesDesejadas)
        {
            if (!permissaoIdsAtuais.Contains(permissao.Id))
            {
                await perfilPermissaoRepository.AddAsync(
                    new PerfilAcessoPermissao(perfil.Id, permissao.Id, usuarioAtual.Login),
                    cancellationToken);
            }
        }

        perfil.AtualizarAuditoria(usuarioAtual.Login);
        perfilRepository.Update(perfil);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var perfilAtualizado = await perfilRepository.Query()
            .AsNoTracking()
            .Include(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema)
            .FirstAsync(x => x.Id == perfilId, cancellationToken);

        var permissoesDisponiveis = await permissaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Acao)
            .ToListAsync(cancellationToken);

        var permissoesVinculadas = perfilAtualizado.PerfilPermissoes
            .Where(x => x.PermissaoSistema.Ativo)
            .Select(x => x.PermissaoSistema)
            .DistinctBy(x => x.Codigo)
            .OrderBy(x => x.Modulo)
            .ThenBy(x => x.Acao)
            .ToArray();

        if (auditoriaService is not null)
        {
            var dadosDepois = JsonSerializer.Serialize(new
            {
                PerfilId = perfilAtualizado.Id,
                PerfilNome = perfilAtualizado.Nome,
                Permissoes = permissoesVinculadas.Select(x => x.Codigo).OrderBy(x => x).ToArray()
            });

            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Perfis e Permissoes",
                Entidade = "PerfilAcesso",
                EntidadeId = perfilAtualizado.Id.ToString(),
                Acao = TipoAcaoAuditoria.AlteracaoPermissao,
                Descricao = $"Permissoes do perfil '{perfilAtualizado.Nome}' atualizadas.",
                DadosAntes = dadosAntes,
                DadosDepois = dadosDepois,
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
            }, cancellationToken);
        }

        return new PerfilPermissoesResponse(
            perfilAtualizado.Id,
            perfilAtualizado.Nome,
            (int)perfilAtualizado.TipoPerfil,
            permissoesDisponiveis.Select(ListarPermissoesSistemaUseCase.MapPermissao).ToArray(),
            permissoesVinculadas.Select(ListarPermissoesSistemaUseCase.MapPermissao).ToArray());
    }
}

public sealed class ObterPermissoesUsuarioAtualUseCase(
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterPermissoesUsuarioAtualUseCase
{
    public async Task<IReadOnlyCollection<string>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var usuario = await usuarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .ThenInclude(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema)
            .FirstOrDefaultAsync(x => x.Id == usuarioAtual.Id && x.Ativo, cancellationToken);

        if (usuario is null)
        {
            return [];
        }

        return usuario.UsuarioPerfis
            .Where(x => x.PerfilAcesso.Ativo)
            .SelectMany(x => x.PerfilAcesso.PerfilPermissoes)
            .Where(x => x.PermissaoSistema.Ativo)
            .Select(x => x.PermissaoSistema.Codigo)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToArray();
    }
}

