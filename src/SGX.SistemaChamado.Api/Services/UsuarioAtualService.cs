using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authentication;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public sealed class UsuarioAtualService(
    IHttpContextAccessor httpContextAccessor,
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions,
    IOptions<AzureAdOptions> azureAdOptions,
    IMetodosLoginAdminService metodosLoginAdminService,
    IConfiguracaoIntegracaoMicrosoftService configuracaoIntegracaoMicrosoftService,
    IAuditoriaService? auditoriaService = null) : IUsuarioAtualService
{
    private const string CacheKey = "sgx.usuario_atual";
    private const string OrigemAutenticacaoMicrosoft = "MicrosoftEntraId";
    private const string OrigemAutenticacaoLocalSgx = "LocalSgx";
    private const string OrigemAutenticacaoActiveDirectory = "ActiveDirectory";

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

        var autenticadoPorLocalDevelopment =
            string.Equals(principal.Identity.AuthenticationType, AuthSchemes.LocalDevelopment, StringComparison.Ordinal);
        var autenticadoPorActiveDirectory =
            string.Equals(ObterClaim(principal, "auth_provider", "autenticado_por"), OrigemAutenticacaoActiveDirectory, StringComparison.Ordinal);
        var autenticadoPorLocalSgx =
            (string.Equals(principal.Identity?.AuthenticationType, AuthSchemes.BearerLocalSgx, StringComparison.Ordinal) && !autenticadoPorActiveDirectory) ||
            string.Equals(ObterClaim(principal, "auth_provider", "autenticado_por"), OrigemAutenticacaoLocalSgx, StringComparison.Ordinal);
        var configuracaoAuthEfetiva = await configuracaoIntegracaoMicrosoftService.ObterConfiguracaoAutenticacaoEfetivaAsync(cancellationToken);
        var autenticadoPorMicrosoft = !autenticadoPorLocalDevelopment && !autenticadoPorLocalSgx && !autenticadoPorActiveDirectory;
        var configuracaoMetodoMicrosoft = autenticadoPorMicrosoft
            ? await metodosLoginAdminService.ObterMetodoEfetivoAsync(CodigoProvedorAutenticacao.MicrosoftEntraId, cancellationToken)
            : null;
        var autoProvisionamentoMicrosoft = configuracaoMetodoMicrosoft?.PermiteAutoProvisionamento
            ?? configuracaoAuthEfetiva.CriarUsuarioAutomaticamente;
        var perfilPadraoMicrosoft = configuracaoMetodoMicrosoft?.PerfilPadraoAutoProvisionamento
            ?? configuracaoAuthEfetiva.PerfilPadraoUsuarioMicrosoft;

        var identidade = ObterIdentidadeCorporativa(principal);

        if (string.IsNullOrWhiteSpace(identidade.Email) && string.IsNullOrWhiteSpace(identidade.Login))
        {
            await RegistrarAutenticacaoCorporativaAsync(
                false,
                "Falha de autenticacao corporativa por claims insuficientes.",
                tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                resultado: ResultadoEventoAutenticacao.Falha,
                provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                "Claims de identidade ausentes.",
                identidadeEmail: identidade.Email,
                identidadeLogin: identidade.Login,
                cancellationToken: cancellationToken);
            throw new InvalidOperationException(
                "Nao foi possivel identificar o usuario autenticado. Claims esperadas: preferred_username, email, upn ou unique_name.");
        }

        if (autenticadoPorMicrosoft)
        {
            if (!configuracaoAuthEfetiva.MicrosoftHabilitado
                || configuracaoMetodoMicrosoft is null
                || !configuracaoMetodoMicrosoft.Habilitado
                || !configuracaoMetodoMicrosoft.Funcional)
            {
                await RegistrarAutenticacaoCorporativaAsync(
                    false,
                    "Falha de autenticacao corporativa por provedor Microsoft desabilitado.",
                    tipoEvento: TipoEventoAutenticacao.ProvedorDesabilitadoTentativaLogin,
                    resultado: ResultadoEventoAutenticacao.Negado,
                    provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                    "Conta Microsoft nao permitida para este ambiente.",
                    identidadeEmail: identidade.Email,
                    identidadeLogin: identidade.Login,
                    cancellationToken: cancellationToken);
                throw new AcessoNegadoException("Conta Microsoft não permitida para este ambiente.");
            }

            try
            {
                ValidarTokenMicrosoftSingleTenant(principal);
            }
            catch (Exception ex) when (ex is AcessoNegadoException)
            {
                await RegistrarAutenticacaoCorporativaAsync(
                    false,
                    "Falha de autenticacao corporativa por tenant/token invalido.",
                    tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                    resultado: ResultadoEventoAutenticacao.Falha,
                    provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                    ex.Message,
                    identidadeEmail: identidade.Email,
                    identidadeLogin: identidade.Login,
                    tenantId: ObterClaim(principal, "tid"),
                    cancellationToken: cancellationToken);
                throw;
            }

            try
            {
                ValidarDominioPermitido(identidade.Email, configuracaoAuthEfetiva.DominiosPermitidos);
            }
            catch (Exception ex) when (ex is AcessoNegadoException)
            {
                await RegistrarAutenticacaoCorporativaAsync(
                    false,
                    "Falha de autenticacao corporativa por dominio nao permitido.",
                    tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                    resultado: ResultadoEventoAutenticacao.Falha,
                    provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                    ex.Message,
                    identidadeEmail: identidade.Email,
                    identidadeLogin: identidade.Login,
                    tenantId: ObterClaim(principal, "tid"),
                    cancellationToken: cancellationToken);
                throw;
            }
        }

        var usuario = await CarregarUsuarioInternoAsync(identidade.Email, identidade.Login, cancellationToken);

        if (usuario is null)
        {
            if (autenticadoPorLocalSgx || autenticadoPorActiveDirectory || (autenticadoPorMicrosoft && !autoProvisionamentoMicrosoft))
            {
                await RegistrarAutenticacaoCorporativaAsync(
                    false,
                    "Falha de autenticacao corporativa por usuario interno nao provisionado.",
                    tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                    resultado: ResultadoEventoAutenticacao.Negado,
                    provedor: autenticadoPorActiveDirectory
                        ? CodigoProvedorAutenticacao.ActiveDirectory
                        : autenticadoPorLocalSgx
                            ? CodigoProvedorAutenticacao.LocalSgx
                            : CodigoProvedorAutenticacao.MicrosoftEntraId,
                    "Usuario nao provisionado no SGX Sistema de Chamados.",
                    identidadeEmail: identidade.Email,
                    identidadeLogin: identidade.Login,
                    tenantId: ObterClaim(principal, "tid"),
                    cancellationToken: cancellationToken);
                throw new AcessoNegadoException("Usuário não provisionado no SGX Sistema de Chamados.");
            }

            usuario = await CriarUsuarioComPerfilPadraoAsync(
                identidade.Nome,
                identidade.Email,
                identidade.Login,
                perfilPadraoMicrosoft,
                cancellationToken);

            await RegistrarAutenticacaoCorporativaAsync(
                true,
                "Usuario interno criado automaticamente por autenticacao corporativa.",
                tipoEvento: TipoEventoAutenticacao.AutoProvisionamentoUsuario,
                resultado: ResultadoEventoAutenticacao.Sucesso,
                provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                identidadeEmail: usuario.Email,
                identidadeLogin: usuario.Login,
                tenantId: ObterClaim(principal, "tid"),
                usuarioId: usuario.Id,
                usuarioInterno: usuario.Email,
                cancellationToken: cancellationToken);
        }

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            await RegistrarAutenticacaoCorporativaAsync(
                false,
                "Falha de autenticacao corporativa para usuario interno inativo.",
                tipoEvento: TipoEventoAutenticacao.UsuarioInativoBloqueado,
                resultado: ResultadoEventoAutenticacao.Bloqueado,
                provedor: autenticadoPorActiveDirectory
                    ? CodigoProvedorAutenticacao.ActiveDirectory
                    : autenticadoPorLocalSgx
                        ? CodigoProvedorAutenticacao.LocalSgx
                        : CodigoProvedorAutenticacao.MicrosoftEntraId,
                "Usuario inativo no SGX Sistema de Chamados.",
                identidadeEmail: identidade.Email,
                identidadeLogin: identidade.Login,
                tenantId: ObterClaim(principal, "tid"),
                usuarioId: usuario.Id,
                usuarioInterno: usuario.Email,
                cancellationToken: cancellationToken);
            throw new AcessoNegadoException("Usuário inativo no SGX Sistema de Chamados.");
        }

        await SincronizarPerfilModoLocalAsync(principal, usuario, cancellationToken);

        usuario.AtualizarUltimoAcesso(DateTime.UtcNow, "auth.sync");
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfis = usuario.UsuarioPerfis
            .Where(x => x.PerfilAcesso.Ativo)
            .Select(x => x.PerfilAcesso.Nome)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToArray();

        var permissoes = usuario.UsuarioPerfis
            .Where(x => x.PerfilAcesso.Ativo)
            .SelectMany(x => x.PerfilAcesso.PerfilPermissoes)
            .Where(x => x.PermissaoSistema.Ativo)
            .Select(x => x.PermissaoSistema.Codigo)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToArray();

        var autenticadoPor = autenticadoPorLocalDevelopment
            ? AuthSchemes.LocalDevelopment
            : autenticadoPorActiveDirectory
                ? OrigemAutenticacaoActiveDirectory
            : autenticadoPorLocalSgx
                ? OrigemAutenticacaoLocalSgx
                : OrigemAutenticacaoMicrosoft;

        var contexto = new UsuarioAutenticadoContexto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            usuario.Situacao.ToString(),
            usuario.DepartamentoId,
            autenticadoPor,
            perfis,
            permissoes,
            usuario.DeveAlterarSenha);

        httpContext.Items[CacheKey] = contexto;

        if (!autenticadoPorLocalDevelopment && !autenticadoPorLocalSgx && !autenticadoPorActiveDirectory)
        {
            await RegistrarAutenticacaoCorporativaAsync(
                true,
                "Login Microsoft Entra ID realizado com sucesso.",
                tipoEvento: TipoEventoAutenticacao.LoginMicrosoftEntraIdSucesso,
                resultado: ResultadoEventoAutenticacao.Sucesso,
                provedor: CodigoProvedorAutenticacao.MicrosoftEntraId,
                identidadeEmail: usuario.Email,
                identidadeLogin: usuario.Login,
                tenantId: ObterClaim(principal, "tid"),
                usuarioId: usuario.Id,
                usuarioInterno: usuario.Email,
                cancellationToken: cancellationToken);
        }

        return contexto;
    }

    private async Task<Usuario?> CarregarUsuarioInternoAsync(string email, string login, CancellationToken cancellationToken)
    {
        var query = dbContext.Usuarios
            .AsSplitQuery()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .ThenInclude(x => x.PerfilPermissoes)
            .ThenInclude(x => x.PermissaoSistema);

        var usuarioPorLogin = await query.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        if (usuarioPorLogin is not null)
        {
            return usuarioPorLogin;
        }

        return await query.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    private async Task<Usuario> CriarUsuarioComPerfilPadraoAsync(
        string nome,
        string email,
        string login,
        string perfilPadraoConfigurado,
        CancellationToken cancellationToken)
    {
        var perfilPadrao = await ObterPerfilPadraoMicrosoftAsync(perfilPadraoConfigurado, cancellationToken);

        var usuario = new Usuario(nome, email, login, "auth.sync");
        await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.UsuariosPerfisAcesso.AddAsync(
            new UsuarioPerfilAcesso(usuario.Id, perfilPadrao.Id, "auth.sync"),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return await CarregarUsuarioInternoAsync(email, login, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar usuario interno apos sincronizacao.");
    }

    private async Task<PerfilAcesso> ObterPerfilPadraoMicrosoftAsync(
        string perfilPadraoConfigurado,
        CancellationToken cancellationToken)
    {
        var perfilPadraoNome = string.IsNullOrWhiteSpace(perfilPadraoConfigurado)
            ? PerfisInternos.Solicitante
            : perfilPadraoConfigurado.Trim();

        var perfilPadrao = await dbContext.PerfisAcesso
            .FirstOrDefaultAsync(
                x => x.Ativo && x.Nome == perfilPadraoNome,
                cancellationToken);

        if (perfilPadrao is not null)
        {
            return perfilPadrao;
        }

        throw new InvalidOperationException(
            $"Perfil padrao '{perfilPadraoNome}' nao encontrado. Verifique o cadastro de perfis internos.");
    }

    private static void ValidarDominioPermitido(string email, IReadOnlyCollection<string> dominiosPermitidosConfigurados)
    {
        var dominiosPermitidos = dominiosPermitidosConfigurados
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().TrimStart('@').ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (dominiosPermitidos.Length == 0)
        {
            return;
        }

        var arrobaIndex = email.LastIndexOf('@');
        if (arrobaIndex <= 0 || arrobaIndex == email.Length - 1)
        {
            throw new AcessoNegadoException("Domínio do usuário não permitido para acesso ao sistema.");
        }

        var dominio = email[(arrobaIndex + 1)..].Trim().ToLowerInvariant();
        var dominioPermitido = dominiosPermitidos.Contains(dominio, StringComparer.OrdinalIgnoreCase);
        if (!dominioPermitido)
        {
            throw new AcessoNegadoException("Domínio do usuário não permitido para acesso ao sistema.");
        }
    }

    private async Task SincronizarPerfilModoLocalAsync(
        ClaimsPrincipal principal,
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

        var perfilSolicitado = ObterClaim(principal, "sgx_dev_role", ClaimTypes.Role);
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

    private static IdentidadeCorporativa ObterIdentidadeCorporativa(ClaimsPrincipal principal)
    {
        var identificadorPrincipal = ObterClaim(principal, "preferred_username", "email", "upn", "unique_name");
        var emailFallback = ObterClaim(principal, "email", "upn", "preferred_username", "unique_name");
        var nome = ObterClaim(principal, "name")
            ?? NormalizarLogin(identificadorPrincipal)
            ?? NormalizarEmail(emailFallback)
            ?? "Usuario SGX";

        var login = NormalizarLogin(identificadorPrincipal)
            ?? NormalizarEmail(emailFallback);

        var email = NormalizarEmail(identificadorPrincipal)
            ?? NormalizarEmail(emailFallback)
            ?? (login is not null && login.Contains('@', StringComparison.Ordinal) ? login : null);

        if (string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(email))
        {
            login = email;
        }

        return new IdentidadeCorporativa(nome, email ?? string.Empty, login ?? string.Empty);
    }

    private static string? NormalizarEmail(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var normalizado = valor.Trim().ToLowerInvariant();
        var arrobaIndex = normalizado.LastIndexOf('@');
        if (arrobaIndex <= 0 || arrobaIndex == normalizado.Length - 1)
        {
            return null;
        }

        return normalizado;
    }

    private static string? NormalizarLogin(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return valor.Trim().ToLowerInvariant();
    }

    private static string? ObterClaim(ClaimsPrincipal principal, params string[] tiposClaim)
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

    private void ValidarTokenMicrosoftSingleTenant(ClaimsPrincipal principal)
    {
        var azureOptions = azureAdOptions.Value;
        var tenantConfigurado = (azureOptions.TenantId ?? string.Empty).Trim();
        var issuerConfigurado = (azureOptions.Issuer ?? string.Empty).Trim();
        var audienceConfigurada = (azureOptions.Audience ?? string.Empty).Trim();
        var clientIdConfigurado = (azureOptions.ClientId ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantConfigurado)
            || string.IsNullOrWhiteSpace(issuerConfigurado)
            || string.IsNullOrWhiteSpace(audienceConfigurada))
        {
            throw new AcessoNegadoException("Token Microsoft inválido para este ambiente.");
        }

        var tid = ObterClaim(principal, "tid");
        if (string.IsNullOrWhiteSpace(tid))
        {
            throw new AcessoNegadoException("Token Microsoft inválido para este ambiente.");
        }

        var tenantConsumidorMicrosoft = "9188040d-6c67-4c5b-b112-36a304b66dad";
        if (string.Equals(tid, tenantConsumidorMicrosoft, StringComparison.OrdinalIgnoreCase))
        {
            throw new AcessoNegadoException("Conta Microsoft não permitida para este ambiente.");
        }

        if (!string.Equals(tid, tenantConfigurado, StringComparison.OrdinalIgnoreCase))
        {
            throw new AcessoNegadoException("Tenant Microsoft não autorizado.");
        }

        var issuer = ObterClaim(principal, "iss");
        if (string.IsNullOrWhiteSpace(issuer)
            || !CompararUrlSemBarraFinal(issuer, issuerConfigurado))
        {
            throw new AcessoNegadoException("Token Microsoft inválido para este ambiente.");
        }

        var audience = ObterClaim(principal, "aud");
        var audienceValida = !string.IsNullOrWhiteSpace(audience)
            && (string.Equals(audience, audienceConfigurada, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(clientIdConfigurado)
                    && string.Equals(audience, clientIdConfigurado, StringComparison.OrdinalIgnoreCase)));
        if (!audienceValida)
        {
            throw new AcessoNegadoException("Token Microsoft inválido para este ambiente.");
        }

        var oid = ObterClaim(principal, "oid");
        if (string.IsNullOrWhiteSpace(oid))
        {
            throw new AcessoNegadoException("Token Microsoft inválido para este ambiente.");
        }
    }

    private static bool CompararUrlSemBarraFinal(string valorA, string valorB)
    {
        var normalizadoA = (valorA ?? string.Empty).Trim().TrimEnd('/');
        var normalizadoB = (valorB ?? string.Empty).Trim().TrimEnd('/');
        return string.Equals(normalizadoA, normalizadoB, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record IdentidadeCorporativa(
        string Nome,
        string Email,
        string Login);

    private async Task RegistrarAutenticacaoCorporativaAsync(
        bool sucesso,
        string descricao,
        TipoEventoAutenticacao tipoEvento,
        ResultadoEventoAutenticacao resultado,
        string provedor,
        string? mensagemErro = null,
        string? identidadeEmail = null,
        string? identidadeLogin = null,
        string? tenantId = null,
        Guid? usuarioId = null,
        string? usuarioInterno = null,
        CancellationToken cancellationToken = default)
    {
        if (auditoriaService is null)
        {
            return;
        }

        await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = AuditoriaAutenticacaoHelper.ModuloAutenticacao,
            Entidade = AuditoriaAutenticacaoHelper.EntidadeAutenticacao,
            EntidadeId = usuarioId?.ToString(),
            Acao = TipoAcaoAuditoria.Login,
            Descricao = descricao,
            Nivel = sucesso ? NivelAuditoria.Informacao : NivelAuditoria.Alerta,
            Sucesso = sucesso,
            MensagemErro = mensagemErro,
            UsuarioId = usuarioId,
            UsuarioEmail = identidadeEmail,
            UsuarioLogin = identidadeLogin,
            Metadados = System.Text.Json.JsonSerializer.Serialize(new
            {
                tipoEventoAutenticacao = tipoEvento.ToString(),
                resultadoAutenticacao = resultado.ToString(),
                provedor,
                tenantId,
                usuarioInterno,
                identidadeEmail,
                identidadeLogin
            })
        }, cancellationToken);
    }
}
