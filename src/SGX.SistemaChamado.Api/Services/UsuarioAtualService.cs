using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authentication;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public sealed class UsuarioAtualService(
    IHttpContextAccessor httpContextAccessor,
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions) : IUsuarioAtualService
{
    private const string CacheKey = "sgx.usuario_atual";

    public async Task<UsuarioAutenticadoContexto> ObterAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext nao disponivel.");

        if (httpContext.Items.TryGetValue(CacheKey, out var cache) && cache is UsuarioAutenticadoContexto contextoCache)
        {
            return contextoCache;
        }

        var principal = httpContext.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("Usuario nao autenticado.");
        }

        var email = ObterClaim(principal, "preferred_username", "email", "upn");
        var login = ObterClaim(principal, "preferred_username", "upn", "email", "sub");
        var nome = ObterClaim(principal, "name")
            ?? login
            ?? email
            ?? "Usuario SGX";

        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(login))
        {
            throw new InvalidOperationException(
                "Nao foi possivel identificar o usuario autenticado. Claims esperadas: preferred_username, email, upn ou sub.");
        }

        email = string.IsNullOrWhiteSpace(email) ? $"{login}@sgx.local" : email.Trim().ToLowerInvariant();
        login = string.IsNullOrWhiteSpace(login) ? email.Split('@')[0] : login.Trim().ToLowerInvariant();

        var usuario = await CarregarUsuarioInternoAsync(email, login, cancellationToken);
        if (usuario is null)
        {
            usuario = await CriarUsuarioSolicitanteAsync(nome, email, login, cancellationToken);
        }

        await SincronizarPerfilModoLocalAsync(principal, usuario, cancellationToken);

        usuario.AtualizarUltimoAcesso(DateTime.UtcNow, "auth.sync");
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfis = usuario.UsuarioPerfis
            .Where(x => x.PerfilAcesso.Ativo)
            .Select(x => x.PerfilAcesso.Nome)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var permissoes = PerfisInternos.ObterPermissoes(perfis);
        var autenticadoPor = principal.Identity?.AuthenticationType == AuthSchemes.LocalDevelopment
            ? "LocalDevelopment"
            : "AzureAd";

        var contexto = new UsuarioAutenticadoContexto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            usuario.Situacao.ToString(),
            usuario.DepartamentoId,
            autenticadoPor,
            perfis,
            permissoes);

        httpContext.Items[CacheKey] = contexto;
        return contexto;
    }

    private async Task<Usuario?> CarregarUsuarioInternoAsync(string email, string login, CancellationToken cancellationToken)
    {
        return await dbContext.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(
                x => x.Ativo && (x.Email == email || x.Login == login),
                cancellationToken);
    }

    private async Task<Usuario> CriarUsuarioSolicitanteAsync(
        string nome,
        string email,
        string login,
        CancellationToken cancellationToken)
    {
        var perfilSolicitante = await dbContext.PerfisAcesso
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Ativo && x.TipoPerfil == TipoPerfil.Solicitante,
                cancellationToken);

        if (perfilSolicitante is null)
        {
            throw new InvalidOperationException(
                "Perfil 'Solicitante' nao encontrado. Verifique o seed inicial de perfis.");
        }

        var usuario = new Usuario(nome, email, login, "auth.sync");
        await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.UsuariosPerfisAcesso.AddAsync(
            new UsuarioPerfilAcesso(usuario.Id, perfilSolicitante.Id, "auth.sync"),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return await CarregarUsuarioInternoAsync(email, login, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar usuario interno apos sincronizacao.");
    }

    private async Task SincronizarPerfilModoLocalAsync(
        System.Security.Claims.ClaimsPrincipal principal,
        Usuario usuario,
        CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment() || !authOptions.Value.ModoLocalHabilitado)
        {
            return;
        }

        if (!string.Equals(principal.Identity?.AuthenticationType, AuthSchemes.LocalDevelopment, StringComparison.Ordinal))
        {
            return;
        }

        var perfilSolicitado = ObterClaim(principal, "sgx_dev_role", System.Security.Claims.ClaimTypes.Role);
        if (!PerfisInternos.EhPerfilValido(perfilSolicitado))
        {
            return;
        }

        var perfil = await dbContext.PerfisAcesso
            .FirstOrDefaultAsync(
                x => x.Ativo && x.Nome == perfilSolicitado,
                cancellationToken);

        if (perfil is null)
        {
            return;
        }

        var jaPossuiPerfil = usuario.UsuarioPerfis.Any(x => x.PerfilAcessoId == perfil.Id);
        if (jaPossuiPerfil)
        {
            return;
        }

        var usuarioPerfil = new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "auth.sync");
        await dbContext.UsuariosPerfisAcesso.AddAsync(usuarioPerfil, cancellationToken);
        usuario.UsuarioPerfis.Add(usuarioPerfil);
    }

    private static string? ObterClaim(System.Security.Claims.ClaimsPrincipal principal, params string[] tiposClaim)
    {
        foreach (var tipoClaim in tiposClaim)
        {
            var valor = principal.FindFirst(tipoClaim)?.Value;
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }
}
