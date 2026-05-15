using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using System.Text.Json;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarPerfisAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarPerfisAcessoUseCase
{
    public async Task<PagedResultResponse<PerfilAcessoResumoResponse>> ExecutarAsync(FiltroCadastroRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = perfilRepository.Query().AsNoTracking().AsQueryable();
        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim();
            query = query.Where(x => x.Nome.Contains(texto) || (x.Descricao ?? string.Empty).Contains(texto));
        }

        query = ApplyOrder(query, request.OrdenarPor, request.DirecaoOrdenacao);
        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);

        return new PagedResultResponse<PerfilAcessoResumoResponse>
        {
            Items = items.Select(AdminCadastrosHelpers.MapPerfilResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    private static IQueryable<PerfilAcesso> ApplyOrder(IQueryable<PerfilAcesso> query, string? ordenarPor, string? direcao)
    {
        var campo = (ordenarPor ?? "nome").Trim().ToLowerInvariant();
        var desc = AdminCadastrosHelpers.DirecaoDesc(direcao);
        return campo switch
        {
            "tipoperfil" => desc ? query.OrderByDescending(x => x.TipoPerfil) : query.OrderBy(x => x.TipoPerfil),
            "ativo" => desc ? query.OrderByDescending(x => x.Ativo) : query.OrderBy(x => x.Ativo),
            _ => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome)
        };
    }
}

public sealed class ObterPerfilAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterPerfilAcessoUseCase
{
    public async Task<PerfilAcessoDetalheResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var perfil = await perfilRepository.Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");

        return MapDetalhe(perfil);
    }

    internal static PerfilAcessoDetalheResponse MapDetalhe(PerfilAcesso perfil)
        => new(perfil.Id, perfil.Nome, (int)perfil.TipoPerfil, perfil.TipoPerfil.ToString(), perfil.Descricao, perfil.Ativo);
}

public sealed class CriarPerfilAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarPerfilAcessoUseCase
{
    public async Task<PerfilAcessoDetalheResponse> ExecutarAsync(CriarPerfilAcessoRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var nome = request.Nome.Trim();
        var existeNome = await perfilRepository.Query().AnyAsync(x => x.Nome == nome, cancellationToken);
        if (existeNome)
        {
            throw new InvalidOperationException("Ja existe perfil com este nome.");
        }

        var perfil = new PerfilAcesso(nome, request.TipoPerfil, request.Descricao, usuarioAtual.Login);
        await perfilRepository.AddAsync(perfil, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Perfis e Permissoes",
                "PerfilAcesso",
                perfil.Id.ToString(),
                $"Perfil '{perfil.Nome}' criado.",
                dadosDepois: JsonSerializer.Serialize(new
                {
                    perfil.Id,
                    perfil.Nome,
                    TipoPerfil = perfil.TipoPerfil.ToString(),
                    perfil.Descricao,
                    perfil.Ativo
                }),
                cancellationToken: cancellationToken);
        }

        return ObterPerfilAcessoUseCase.MapDetalhe(perfil);
    }
}

public sealed class AtualizarPerfilAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarPerfilAcessoUseCase
{
    public async Task<PerfilAcessoDetalheResponse> ExecutarAsync(Guid id, AtualizarPerfilAcessoRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var perfil = await perfilRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");
        var dadosAntes = JsonSerializer.Serialize(new
        {
            perfil.Id,
            perfil.Nome,
            TipoPerfil = perfil.TipoPerfil.ToString(),
            perfil.Descricao,
            perfil.Ativo
        });

        var nome = request.Nome.Trim();
        var existeNome = await perfilRepository.Query().AnyAsync(x => x.Id != id && x.Nome == nome, cancellationToken);
        if (existeNome)
        {
            throw new InvalidOperationException("Ja existe perfil com este nome.");
        }

        perfil.DefinirNome(nome);
        perfil.DefinirDescricao(request.Descricao);
        perfil.DefinirTipoPerfil(request.TipoPerfil, usuarioAtual.Login);
        perfil.AtualizarAuditoria(usuarioAtual.Login);
        perfilRepository.Update(perfil);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Perfis e Permissoes",
                "PerfilAcesso",
                perfil.Id.ToString(),
                $"Perfil '{perfil.Nome}' atualizado.",
                dadosAntes: dadosAntes,
                dadosDepois: JsonSerializer.Serialize(new
                {
                    perfil.Id,
                    perfil.Nome,
                    TipoPerfil = perfil.TipoPerfil.ToString(),
                    perfil.Descricao,
                    perfil.Ativo
                }),
                cancellationToken: cancellationToken);
        }

        return ObterPerfilAcessoUseCase.MapDetalhe(perfil);
    }
}

public sealed class InativarPerfilAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IInativarPerfilAcessoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var perfil = await perfilRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");

        if (!perfil.Ativo)
        {
            return new AlterarSituacaoCadastroResponse(perfil.Id, false, "Perfil ja esta inativo.");
        }

        if (perfil.TipoPerfil == TipoPerfil.Administrador)
        {
            var outrosPerfisAdminAtivos = await perfilRepository.Query()
                .AnyAsync(x => x.Id != perfil.Id && x.Ativo && x.TipoPerfil == TipoPerfil.Administrador, cancellationToken);

            if (!outrosPerfisAdminAtivos)
            {
                var adminsAtivos = await AdminCadastrosHelpers.ContarAdministradoresAtivosAsync(
                    usuarioRepository.Query().Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso));

                if (adminsAtivos > 0)
                {
                    throw new InvalidOperationException("Nao e permitido inativar o perfil Administrador deixando o sistema sem administrador ativo.");
                }
            }
        }

        perfil.Desativar(usuarioAtual.Login);
        perfilRepository.Update(perfil);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarInativacaoAsync(
                "Perfis e Permissoes",
                "PerfilAcesso",
                perfil.Id.ToString(),
                $"Perfil '{perfil.Nome}' inativado.",
                cancellationToken: cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(perfil.Id, false, "Perfil inativado com sucesso.");
    }
}

public sealed class ReativarPerfilAcessoUseCase(
    IRepository<PerfilAcesso> perfilRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReativarPerfilAcessoUseCase
{
    public async Task<AlterarSituacaoCadastroResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var perfil = await perfilRepository.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Perfil nao encontrado.");

        perfil.Ativar(usuarioAtual.Login);
        perfilRepository.Update(perfil);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAtivacaoAsync(
                "Perfis e Permissoes",
                "PerfilAcesso",
                perfil.Id.ToString(),
                $"Perfil '{perfil.Nome}' reativado.",
                cancellationToken: cancellationToken);
        }

        return new AlterarSituacaoCadastroResponse(perfil.Id, true, "Perfil reativado com sucesso.");
    }
}

